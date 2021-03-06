﻿using Gungeon;
using ItemAPI;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using UnityEngine;

/*
on first use, slow time for other enemies
while the user move or fire, refills the item use meter and stock the path and shot places
on second use tp back to where the user was then redo the travel really fast

cannot interact with interactables while using turn
immunity to fire/poison/contact dmg while using turn
give the transistor during turn and lock it (charge weapon)

To do
Recharge item with actions ok
remove idle movement ok
Stop time look at aged bell yiss ok
increase game speed ok
Fire gun save ok
Make user fly ok
No movement when record full ok
Cancel action during record ok
Give usb swordgun, prevent drop and lock ok
Fix boss collision ok
Fire all projectiles of the usb swordgun ok
Prevent dmgCooldown to go over max with last action ok
Check death during replay fun ok
Remove goops ok
end turn early if ondrop ok
end turn early if PlayerItem.onitemswitch/PC.CurrentItem ok
remove player projs at start ok
end turn early if onleavecombat ok
Prevent companions(CompanionItem) or orbital(IounStoneOrbitalItem) intervention
Prevent dodgeroll spam ok
end turn early if onchangedroom ok
end turn early if onNoEnemy, onReinforcement no need
Freeze enemy on spawn ok
Prevent blanks ok
Prevent interactions ok
Prevent passives drop ok
SetGunLock in update ok
Keep projs moving when enemy killed ? destroy dem ok
Make enemies invulnerable during record ? ok
Cancel enemies effects no need

Check items interactions (bloodied scarf/super hot watch/gunboots/full metal jacket ok)
see if cancel works properly => dunno what cause it, dmg during record ?
Check coolness

Add sound and screen effects
On load new floor test
improve blank prevention during record (pickup)
Prevent item pickup ?
Prevent coop intervention ?
*/
namespace GlaurungItems.Items
{
    class Turn : CustomRadialSlowItem
	{
		public static void Init()
		{
			string text = "Turn()";
			string resourcePath = "GlaurungItems/Resources/turn";
			GameObject gameObject = new GameObject(text);
			Turn item = gameObject.AddComponent<Turn>();
			ItemBuilder.AddSpriteToObject(text, resourcePath, gameObject);
			string shortDesc = "It's my Turn() now !";
			string longDesc = "On first use, gives the USB Gun, stop enemies, companions and projectiles and records three types of actions (moving/shooting/dodgerolling), " +
				"refilling the cooldown bar for a certain amounBt until it's fully charged. Keeping the reload pressed cancel the most recent actions. " +
				"On second use, replay all the actions recorded. Beware, you don't collide with enemies or projectiles nor fall during the record phase " +
				"but you will collide/fall and take damage during the replay phase.";
			item.SetupItem(shortDesc, longDesc, "gl");
			item.SetCooldownType(ItemBuilder.CooldownType.Damage, turnCooldown);
			item.quality = ItemQuality.A;
		}



		protected override void DoEffect(PlayerController user)
		{

			if (!isRecordTimeActive)
            {
				startingTurnPosition = user.transform.position;

				user.PostProcessProjectile += User_PostProcessProjectile;
				user.OnRoomClearEvent += OnRoomClear;
				
				//remove goops at start
				if(user.CurrentRoom != null && user.CurrentRoom.RoomGoops != null && user.CurrentRoom.RoomGoops.Count > 0)
                {
					List<DeadlyDeadlyGoopManager> goopManagers = user.CurrentRoom.RoomGoops;
					//int i = 0;
					foreach (DeadlyDeadlyGoopManager goopManager in goopManagers)
					{
						goopManager.RemoveGoopCircle(user.CenterPosition, 30f);
					}
				}

				//from SilencerInstance to remove the user projectiles at turn activation
				ReadOnlyCollection<Projectile> allProjectiles = StaticReferenceManager.AllProjectiles;
				for (int l = allProjectiles.Count - 1; l >= 0; l--)
				{
					Projectile projectile = allProjectiles[l];
					if (projectile.Owner is PlayerController)
					{
						projectile.ForceDestruction();
					}
				}

				actions = new List<ActionsToBeRecorded>();
				dodgeRollDirection = new List<Vector2>();
				playerPositionsDuringActivation = new List<Vector3>();
				aimDirectionWhileFiring = new List<Vector3>();
				projsPositions = new List<Vector3>();
				gunAngleWhenFired = new List<float>();
				projsFired = new List<int>();
				compsSaved = new List<AIActor>();
				savedRoomInteractables = new List<IPlayerInteractable>();
				savePassivesCanBeDropped = new Dictionary<PassiveItem, bool>();
				saveEnemiesAreInvulnerable = new Dictionary<AIActor, bool>();

				//for the check of room change during combat
				roomWhereTurnWasActivated = user.CurrentRoom;

				//prevent blanks during record
				nbConsumableBlanksAtStart = user.Blanks;
				user.Blanks = 0;

				//for the check if a new enemy spawned in update
				UpdateEnemiesInRoom(user);		

				//to stop companions
				if(user.passiveItems != null)
                {
					foreach (PassiveItem passive in user.passiveItems)
					{
						if (passive is CompanionItem)
						{
							CompanionItem compItem = (passive as CompanionItem);
							if (compItem.ExtantCompanion && compItem.ExtantCompanion.GetComponent<CompanionController>())
							{
								CompanionController compCont = compItem.ExtantCompanion.GetComponent<CompanionController>();
								if (compCont.GetComponent<AIActor>() != null)
								{
									AIActor comp = compCont.GetComponent<AIActor>();
									if (comp.healthHaver && comp.healthHaver.IsAlive)
									{
										comp.LocalTimeScale = 0;
										compsSaved.Add(comp);
									}

								}
							}
						}
						savePassivesCanBeDropped.Add(passive, passive.CanBeDropped);
						passive.CanBeDropped = false;
					}
				}

				List<AIActor> actorsAtStart = user.CurrentRoom.GetActiveEnemies(Dungeonator.RoomHandler.ActiveEnemyType.All);
				foreach(AIActor actor in actorsAtStart)
                {
					saveEnemiesAreInvulnerable.Add(actor, actor.ImmuneToAllEffects);
					actor.ImmuneToAllEffects = true;
					if (actor.healthHaver != null)
					{
						saveEnemiesHaveHealthHaverInvunerability.Add(actor, actor.healthHaver.PreventAllDamage);
						actor.healthHaver.PreventAllDamage = true;
					}
				}

				//to make guon orbitals not collides with bullets during the record phase
				if (user.orbitals != null && user.orbitals.Count > 0)
                {
					foreach(IPlayerOrbital orb in user.orbitals)
                    {
						if(orb is PlayerOrbital)
                        {
							(orb as PlayerOrbital).specRigidbody.CollideWithOthers = false;
                        }
                    }
                }

				if (user.CurrentRoom != null && user.CurrentRoom.GetRoomInteractables() != null)
				{
					List<IPlayerInteractable> interactables = user.CurrentRoom.GetRoomInteractables().ToList();
					foreach(IPlayerInteractable interactable in interactables)
                    {
						savedRoomInteractables.Add(interactable);
						user.CurrentRoom.DeregisterInteractable(interactable);
					}
				}

				//for the possibility of dodgerolling at the start
				actions.Add(ActionsToBeRecorded.Moving);
				playerPositionsDuringActivation.Add(user.transform.position);

				//invulnerability during record time
				if (user.IsFlying)
                {
					wasFlyingAtTheStart = true;
                }
                else
                {
					user.SetIsFlying(true, "turn");
					user.AdditionalCanDodgeRollWhileFlying.SetOverride("turn", true);
                }
				user.healthHaver.IsVulnerable = false;
				user.specRigidbody.AddCollisionLayerIgnoreOverride(collisionMask);
				user.specRigidbody.AddCollisionLayerIgnoreOverride(collisionMask2);

				//give usb gun
				transistorGunInstance = user.inventory.AddGunToInventory(usb, true);
				transistorGunInstance.CanBeDropped = false;
				transistorGunInstance.CanBeSold = false;
				user.inventory.GunLocked.SetOverride("turn", true, null);

				isRecordTimeActive = true;
				this.OutTime = normalOutTimeDuration;
				stopLocalTime = true;
				Exploder.DoDistortionWave(user.CenterPosition, 0.4f, 0.15f, this.EffectRadius, .7f);
			}

			else
            {
				user.WarpToPoint(startingTurnPosition);

				ResetCompanions(user);
				ResetIouns(user);
				ResetInteractables(user);
				ResetPassivesCanBeDropped(user);
				ResetEnemiesAreInvulnerable(user);

				user.Blanks = nbConsumableBlanksAtStart;

				user.healthHaver.IsVulnerable = true;
				user.specRigidbody.RemoveCollisionLayerIgnoreOverride(collisionMask);
				user.specRigidbody.RemoveCollisionLayerIgnoreOverride(collisionMask2);

				user.PostProcessProjectile -= User_PostProcessProjectile;
				if (!wasFlyingAtTheStart)
                {
					user.SetIsFlying(false, "turn");
					user.AdditionalCanDodgeRollWhileFlying.RemoveOverride("turn");
				}
				else
                {
					wasFlyingAtTheStart = false;
                }
				isRecordTimeActive = false;
				isReplayTimeActive = true;
				GameManager.Instance.StartCoroutine(DoTurn(user));
			}
			base.DoEffect(user);
		}

		//------------------
        public override void Update()
		{
			base.Update();
			if (base.LastOwner)
			{
				PlayerController user = base.LastOwner;
				if (isRecordTimeActive)
				{
					if (this.CurrentDamageCooldown > 0)
					{
						if (!dodgeRollCooldown)
						{
							user.CurrentInputState = PlayerInputState.AllInput;
						}

						if (user.IsDodgeRolling && !isCurrentlyDodgeRolling)
						{
							if (playerPositionsDuringActivation.Count > 0)
							{
								isCurrentlyDodgeRolling = true;
								dodgeRollCooldown = true;
								actions.Add(ActionsToBeRecorded.Dodgeroll);
								dodgeRollDirection.Add(user.transform.position - playerPositionsDuringActivation[playerPositionsDuringActivation.Count - 1]);
								float c = Math.Min(dodgerollCost, CurrentDamageCooldown);
								if (c < dodgerollCost)
								{
									notFullLastActionCost = c;
								}
								this.CurrentDamageCooldown -= c;
							}
							user.CurrentInputState = PlayerInputState.NoMovement;
						}

						else if (user.IsFiring && !user.IsDodgeRolling)
						{
							if (isCurrentlyDodgeRolling)
							{
								GameManager.Instance.StartCoroutine(ReallowMovementAfterDodgeRoll(user));
							}
							isCurrentlyDodgeRolling = false;
						}

						else if (!user.IsDodgeRolling) //for movement recording
						{
							if (isCurrentlyDodgeRolling)
							{
								GameManager.Instance.StartCoroutine(ReallowMovementAfterDodgeRoll(user));
							}
							isCurrentlyDodgeRolling = false;

							int lenPos = playerPositionsDuringActivation.Count;
							if (lenPos > 1 && playerPositionsDuringActivation[lenPos - 1] != user.transform.position)
							{
								actions.Add(ActionsToBeRecorded.Moving);
								playerPositionsDuringActivation.Add(user.transform.position);
								this.CurrentDamageCooldown -= Math.Min(movementCost, CurrentDamageCooldown);
							}
							else if (lenPos <= 1)
							{
								actions.Add(ActionsToBeRecorded.Moving);
								playerPositionsDuringActivation.Add(user.transform.position);
							}
						}
					}
					else
					{
						user.CurrentInputState = PlayerInputState.NoMovement;
					}

					if (Key(GungeonActions.GungeonActionType.Reload) && KeyTime(GungeonActions.GungeonActionType.Reload) > 0.75f && !cancelActionCooldown)
					{
						cancelActionCooldown = true;
						if (actions.Count > 1)
						{
							int actionsLen = actions.Count;
							//Tools.Print(actions[actionsLen - 1], "ffffff", true);

							if (actions[actionsLen - 1] == ActionsToBeRecorded.Shooting)
							{
								actions.RemoveAt(actionsLen - 1);
								aimDirectionWhileFiring.RemoveAt(aimDirectionWhileFiring.Count - 1);
								gunAngleWhenFired.RemoveAt(gunAngleWhenFired.Count - 1);
								projsPositions.RemoveAt(projsPositions.Count - 1);

								if (notFullLastActionCost > notFullLastActionCostNormalConst)
								{
									this.CurrentDamageCooldown += notFullLastActionCost;
									notFullLastActionCost = notFullLastActionCostNormalConst;
								}
								else
								{
									this.CurrentDamageCooldown += shootCosts[projsFired[projsFired.Count - 1]];
								}

								projsFired.RemoveAt(projsFired.Count - 1);
							}

							else if (actionsLen > 2 && actions[actionsLen - 1] == ActionsToBeRecorded.Moving && actions[actionsLen - 2] == ActionsToBeRecorded.Shooting)
							{
								actions.RemoveAt(actions.Count - 1);
								actions.RemoveAt(actions.Count - 1);
								aimDirectionWhileFiring.RemoveAt(aimDirectionWhileFiring.Count - 1);
								gunAngleWhenFired.RemoveAt(gunAngleWhenFired.Count - 1);
								projsPositions.RemoveAt(projsPositions.Count - 1);

								user.WarpToPoint(playerPositionsDuringActivation[playerPositionsDuringActivation.Count - 2]);
								playerPositionsDuringActivation.RemoveAt(playerPositionsDuringActivation.Count - 1);

								if (notFullLastActionCost > notFullLastActionCostNormalConst)
								{
									this.CurrentDamageCooldown += (notFullLastActionCost + movementCost);
									notFullLastActionCost = notFullLastActionCostNormalConst;
								}
								else
								{
									this.CurrentDamageCooldown += (shootCosts[projsFired[projsFired.Count - 1]] + movementCost);
								}

								projsFired.RemoveAt(projsFired.Count - 1);
							}

							else if (actions[actionsLen - 1] == ActionsToBeRecorded.Dodgeroll)
							{
								actions.RemoveAt(actionsLen - 1);
								dodgeRollDirection.RemoveAt(dodgeRollDirection.Count - 1);
								user.WarpToPoint(playerPositionsDuringActivation[playerPositionsDuringActivation.Count - 1]);

								if (notFullLastActionCost > notFullLastActionCostNormalConst)
								{
									this.CurrentDamageCooldown += notFullLastActionCost;
									notFullLastActionCost = notFullLastActionCostNormalConst;
								}
								else
								{
									this.CurrentDamageCooldown += dodgerollCost;
								}
							}

							else if (actionsLen > 2 && actions[actionsLen - 1] == ActionsToBeRecorded.Moving && actions[actionsLen - 2] == ActionsToBeRecorded.Dodgeroll)
							{
								actions.RemoveAt(actions.Count - 1);
								actions.RemoveAt(actions.Count - 1);
								dodgeRollDirection.RemoveAt(dodgeRollDirection.Count - 1);

								user.WarpToPoint(playerPositionsDuringActivation[playerPositionsDuringActivation.Count - 2]);
								playerPositionsDuringActivation.RemoveAt(playerPositionsDuringActivation.Count - 1);

								if (notFullLastActionCost > notFullLastActionCostNormalConst)
								{
									this.CurrentDamageCooldown += (notFullLastActionCost + movementCost);
									notFullLastActionCost = notFullLastActionCostNormalConst;
								}
								else
								{
									this.CurrentDamageCooldown += (dodgerollCost + movementCost);
								}
							}

							else if (actions[actionsLen - 1] == ActionsToBeRecorded.Moving && actionsLen > 1)
							{
								int nbOfMovesToRemove = 1;
								for (int i = actionsLen - 1; i > 0; i--)
								{
									if (actions[i] == ActionsToBeRecorded.Moving && actions[i - 1] == ActionsToBeRecorded.Moving)
									{
										nbOfMovesToRemove++;
									}
									else
									{
										break;
									}
								}
								user.WarpToPoint(playerPositionsDuringActivation[playerPositionsDuringActivation.Count - nbOfMovesToRemove]);
								while (nbOfMovesToRemove > 0)
								{
									nbOfMovesToRemove--;
									actions.RemoveAt(actions.Count - 1);
									playerPositionsDuringActivation.RemoveAt(playerPositionsDuringActivation.Count - 1);
									this.CurrentDamageCooldown += movementCost;
								}
							}

						}
						GameManager.Instance.StartCoroutine(CancelCooldownCoroutine());
					}
				}

				if (isRecordTimeActive || isReplayTimeActive)
				{
					if (user.CurrentRoom != roomWhereTurnWasActivated)
					{
						CancelEarly(user);
					}

					user.inventory.GunLocked.SetOverride("turn", true, null);

					ReadOnlyCollection<Projectile> allProjectiles = StaticReferenceManager.AllProjectiles;
					if(allProjectiles != null)
                    {
						for (int l = allProjectiles.Count - 1; l >= 0; l--)
						{
							Projectile projectile = allProjectiles[l];
							if (
								projectile.Owner == null
								|| (projectile.Owner is AIActor && (projectile.Owner as AIActor).healthHaver && (projectile.Owner as AIActor).healthHaver.IsDead)
							)
							{
								projectile.ForceDestruction();
							}
						}
					}

					//to freeze newcomers
					List<AIActor> actorsDuringThisFrame = user.CurrentRoom.GetActiveEnemies(Dungeonator.RoomHandler.ActiveEnemyType.All);
					//from https://stackoverflow.com/questions/12795882/quickest-way-to-compare-two-generic-lists-for-differences
					if (enemiesInRoom.Except(actorsDuringThisFrame).ToList().Any() || actorsDuringThisFrame.Except(enemiesInRoom).ToList().Any())
                    {
						AffectEnemiesInRadiusEffect(user, true);
                        /*if (enemiesInRoom.Except(actorsDuringThisFrame).ToList().Any())
                        {
							List<AIActor> ded = enemiesInRoom.Except(actorsDuringThisFrame).ToList();
							ReadOnlyCollection<Projectile> allProjectiles = StaticReferenceManager.AllProjectiles;
							for (int l = allProjectiles.Count - 1; l >= 0; l--)
							{
								Projectile projectile = allProjectiles[l];
								if (
									projectile.Owner == null 
									|| (projectile.Owner is AIActor && ded.Contains(projectile.Owner as AIActor))
									|| (projectile.Owner is AIActor && (projectile.Owner as AIActor).healthHaver && (projectile.Owner as AIActor).healthHaver.IsDead)
								)
								{
									projectile.ForceDestruction();
								}
							}
						}*/
					}
					UpdateEnemiesInRoom(user);	
					
				}
			}

			
		}

        //-----------
        private void User_PostProcessProjectile(Projectile proj, float arg2)
		{
			if (!hasFiredCooldown)
			{
				PlayerController user = this.LastOwner;
				hasFiredCooldown = true;

				int projNb = 0;
				List<string> validProjs = new List<string>{
					usb.DefaultModule.chargeProjectiles[0].Projectile.name + "(Clone)",
					usb.DefaultModule.chargeProjectiles[1].Projectile.name + "(Clone)",
					usb.DefaultModule.chargeProjectiles[2].Projectile.name + "(Clone)"
				};
				if (validProjs.Contains(proj.name) && isRecordTimeActive && CurrentDamageCooldown > 0)
				{
					projNb = validProjs.IndexOf(proj.name);

					projsFired.Add(projNb);
					actions.Add(ActionsToBeRecorded.Shooting);
					Vector3 aim = (user.unadjustedAimPoint);// - user.CenterPosition);
					aimDirectionWhileFiring.Add(aim);
					gunAngleWhenFired.Add(user.CurrentGun.CurrentAngle);
					projsPositions.Add(user.CurrentGun.barrelOffset.position);
					
					float c = Math.Min(shootCosts[projNb], CurrentDamageCooldown);
					if (c < shootCosts[projNb])
					{
						notFullLastActionCost = c;
					}
					this.CurrentDamageCooldown -= c;

					Projectile projCopy = usb.DefaultModule.chargeProjectiles[projNb].Projectile;
					GameObject gameObject = SpawnManager.SpawnProjectile(projCopy.gameObject, user.CurrentGun.barrelOffset.position, Quaternion.Euler(0f, 0f, user.CurrentGun.CurrentAngle), true);
					Projectile projectileInst = gameObject.GetComponent<Projectile>();
					projectileInst.Owner = user;
					projectileInst.specRigidbody.AddCollisionLayerIgnoreOverride(collisionMask);
					projectileInst.specRigidbody.AddCollisionLayerIgnoreOverride(collisionMask2);
					projectileInst.specRigidbody.AddCollisionLayerIgnoreOverride(collisionMaskProj);
					projectileInst.transform.parent = user.CurrentGun.barrelOffset;
					if (projNb == 2)
					{
						projectileInst.CurseSparks = true;
					}
				}

				GameManager.Instance.StartCoroutine(ResetHasFired());
			}

			proj.DieInAir(true, false, false);
		}


		//-------------
		private IEnumerator DoTurn(PlayerController user)
        {
			user.SetInputOverride("turn");
			user.CurrentInputState = PlayerInputState.NoInput;
			Time.timeScale = accelerateTimeFactor;
			foreach (ActionsToBeRecorded act in actions)
			{
                if (!isReplayTimeActive)
                {
					break;
                }
				if (act == ActionsToBeRecorded.Dodgeroll)
				{
					user.ForceStartDodgeRoll(dodgeRollDirection[0]);
					dodgeRollDirection.RemoveAt(0);
					yield return new WaitForSeconds(1f);
				}

				if (act == ActionsToBeRecorded.Moving)
				{
					if((playerPositionsDuringActivation.Count > 1 && playerPositionsDuringActivation[0] != playerPositionsDuringActivation[1]) 
						|| playerPositionsDuringActivation.Count == 1)
                    {
						//user.ForceMoveToPoint(playerPositionsDuringActivation[0]);//tp better
						user.WarpToPoint(playerPositionsDuringActivation[0]);
					}
					yield return null;
					playerPositionsDuringActivation.RemoveAt(0);
				}

				if(act == ActionsToBeRecorded.Shooting)
                {
					yield return null;
                    //user.unadjustedaimpoint = aimdirectionwhilefiring[0];
                    //user.forcestaticfacedirection(aimdirectionwhilefiring[0]);
                    //user.forceidlefacepoint(aimdirectionwhilefiring[0]);
                    user.CurrentGun.HandleAimRotation(aimDirectionWhileFiring[0]);
					
					GameObject gameObject = SpawnManager.SpawnProjectile(usb.DefaultModule.chargeProjectiles[projsFired[0]].Projectile.gameObject, projsPositions[0], Quaternion.Euler(0f, 0f, gunAngleWhenFired[0]), true);
					Projectile projectile = gameObject.GetComponent<Projectile>();
					if(projsFired[0] == 2)
                    {
						projectile.CurseSparks = true;
                    }
					//projectile.transform.parent.position = projsPositions[0]; //bad displace bullet during replay
					user.DoPostProcessProjectile(projectile);
					//user.CurrentGun.ForceFireProjectile(user.CurrentGun.DefaultModule.projectiles[0]);
					//user.forceAimPoint = null;
					yield return new WaitForSeconds(0.2f);
					aimDirectionWhileFiring.RemoveAt(0);
					gunAngleWhenFired.RemoveAt(0);
					projsFired.RemoveAt(0);
					projsPositions.RemoveAt(0);
				}
			}

			yield return null;

			ReplayTimeEnd(user);

			yield break;
		}


        public override bool CanBeUsed(PlayerController user)
		{
			return user.IsInCombat 
				&& user.CurrentRoom != null
				&& user.CurrentRoom.IsSealed
				&& ((!isRecordTimeActive && !user.inventory.GunLocked.Value) || (isRecordTimeActive && user.inventory.GunLocked.Value))
				//&& !user.HasPassiveItem(436) //bloodied scarf
				;
		}

		private void CancelEarly(PlayerController user)
        {
            if (isRecordTimeActive)
            {
				user.inventory.GunLocked.RemoveOverride("turn");
				user.inventory.DestroyGun(transistorGunInstance);
				this.transistorGunInstance = null;

				user.CurrentInputState = PlayerInputState.AllInput;

				user.healthHaver.IsVulnerable = true;
				user.specRigidbody.RemoveCollisionLayerIgnoreOverride(collisionMask);
				user.specRigidbody.RemoveCollisionLayerIgnoreOverride(collisionMask2);

				user.PostProcessProjectile -= User_PostProcessProjectile;
				user.OnRoomClearEvent -= OnRoomClear;
				if (!wasFlyingAtTheStart)
				{
					user.SetIsFlying(false, "turn");
					user.AdditionalCanDodgeRollWhileFlying.RemoveOverride("turn");
				}
				else
				{
					wasFlyingAtTheStart = false;
				}

				this.OutTime = 0f;
				stopLocalTime = false;
				isRecordTimeActive = false;

				ResetCompanions(user);
				ResetIouns(user);
				ResetInteractables(user);
				ResetPassivesCanBeDropped(user);
				ResetEnemiesAreInvulnerable(user);

				this.ClearCooldowns();
				user.WarpToPoint(startingTurnPosition);
			}

			else if(isReplayTimeActive)
			{
				ReplayTimeEnd(user);
			}
		}

		private void ReplayTimeEnd(PlayerController user)
        {
			Time.timeScale = 1;

			user.OnRoomClearEvent -= OnRoomClear;

			user.inventory.GunLocked.RemoveOverride("turn");
			if(transistorGunInstance != null)
            {
				user.inventory.DestroyGun(transistorGunInstance);
				this.transistorGunInstance = null;
			}
			user.CurrentInputState = PlayerInputState.AllInput;
			user.ClearInputOverride("turn");
			stopLocalTime = false;
			isReplayTimeActive = false;
		}

		protected override void OnPreDrop(PlayerController user)
		{
			CancelEarly(user);
			base.OnPreDrop(user);
		}

        public override void OnItemSwitched(PlayerController user)
        {
			CancelEarly(user);
            base.OnItemSwitched(user);
        }

        private IEnumerator ResetHasFired()
        {
			yield return null;
			hasFiredCooldown = false;
			yield break;
		}

		private IEnumerator CancelCooldownCoroutine()
		{
			yield return new WaitForSeconds(0.5f);
			cancelActionCooldown = false;
			yield break;
		}

		private void ResetCompanions(PlayerController user)
        {
			foreach(AIActor comp in compsSaved)
            {
				comp.LocalTimeScale = 1;
            }
		}

		private void ResetIouns(PlayerController user)
        {
			if (user.orbitals != null && user.orbitals.Count > 0)
			{
				foreach (IPlayerOrbital orb in user.orbitals)
				{
					if (orb is PlayerOrbital)
					{
						(orb as PlayerOrbital).specRigidbody.CollideWithOthers = true;
					}
				}
			}
		}

		private void ResetInteractables(PlayerController user)
        {
			if (user.CurrentRoom != null)
			{
				foreach (IPlayerInteractable interactable in savedRoomInteractables)
				{
                    if (interactable != null)
                    {
						user.CurrentRoom.RegisterInteractable(interactable);
					}
				}
			}
		}

		private void ResetPassivesCanBeDropped(PlayerController user)
        {
			if(savePassivesCanBeDropped != null)
            {
				foreach(PassiveItem passive in savePassivesCanBeDropped.Keys)
                {
					passive.CanBeDropped = savePassivesCanBeDropped[passive];
                }
            }
        }

		private void ResetEnemiesAreInvulnerable(PlayerController user)
		{
			if (saveEnemiesAreInvulnerable != null)
			{
				foreach (AIActor actor in saveEnemiesAreInvulnerable.Keys)
				{
					actor.ImmuneToAllEffects = saveEnemiesAreInvulnerable[actor];
				}
			}
			if(saveEnemiesHaveHealthHaverInvunerability != null)
            {
				foreach (AIActor actor in saveEnemiesHaveHealthHaverInvunerability.Keys)
				{
					actor.healthHaver.PreventAllDamage = saveEnemiesHaveHealthHaverInvunerability[actor];
				}
			}
		}


		private void OnRoomClear(PlayerController user)
		{
			CancelEarly(user);
		}

		private IEnumerator ReallowMovementAfterDodgeRoll(PlayerController user)
		{
			yield return new WaitForSeconds(0.1f);
			dodgeRollCooldown = false;
			yield break;
		}

		//for the checking of new enemies during turn 
		private void UpdateEnemiesInRoom(PlayerController user)
        {
			if(user && user.CurrentRoom != null)
            {
				enemiesInRoom = new List<AIActor>();
				List<AIActor> actors = user.CurrentRoom.GetActiveEnemies(Dungeonator.RoomHandler.ActiveEnemyType.All);
				foreach(AIActor actor in actors)
                {
					enemiesInRoom.Add(actor);
                }
			}
		}

		//from kyle's ileveler app
		public float KeyTime(GungeonActions.GungeonActionType action)
		{
			return BraveInput.GetInstanceForPlayer(LastOwner.PlayerIDX).ActiveActions.GetActionFromType(action).PressedDuration;
		}

		public bool Key(GungeonActions.GungeonActionType action)
		{
			return BraveInput.GetInstanceForPlayer(LastOwner.PlayerIDX).ActiveActions.GetActionFromType(action).IsPressed;
		}



		//------------------------------
		private readonly static int collisionMask = CollisionMask.LayerToMask(CollisionLayer.EnemyHitBox, CollisionLayer.EnemyCollider, 
			CollisionLayer.EnemyBulletBlocker, CollisionLayer.EnemyBlocker, CollisionLayer.Projectile, CollisionLayer.Pickup, 
			CollisionLayer.BeamBlocker);
		private readonly static int collisionMask2 = CollisionMask.LayerToMask(CollisionLayer.BulletBlocker, CollisionLayer.BulletBreakable,
			CollisionLayer.PlayerBlocker, CollisionLayer.PlayerCollider);
		private readonly static int collisionMaskProj = CollisionMask.LayerToMask(CollisionLayer.LowObstacle, CollisionLayer.HighObstacle);
        private enum ActionsToBeRecorded
		{
			Dodgeroll,
			Shooting,
			Moving,
		};

		private readonly static float turnCooldown = 1000f;
		private readonly static float dodgerollCost = 100f;
		private readonly static float movementCost = .6f;
		private readonly static float[] shootCosts = {
			40f,
			200f,
			500f
		};
		private Gun usb = Game.Items["gl:usb_gun"] as Gun;
		private readonly static int notFullLastActionCostNormalConst = -999;
		private readonly static float normalOutTimeDuration = 2f;
		private readonly static float accelerateTimeFactor = 2.5f;

		private bool isRecordTimeActive = false;
		private bool isReplayTimeActive = false;

		private bool wasFlyingAtTheStart = false;
		private Vector3 startingTurnPosition;
		private Dungeonator.RoomHandler roomWhereTurnWasActivated;
		private int nbConsumableBlanksAtStart;
		private Gun transistorGunInstance;

		private bool isCurrentlyDodgeRolling = false;
		private bool hasFiredCooldown = false;
		private bool cancelActionCooldown = false;
		private bool dodgeRollCooldown = false;
		private float notFullLastActionCost = -999;

		private List<AIActor> enemiesInRoom = new List<AIActor>();
		private List<IPlayerInteractable> savedRoomInteractables = new List<IPlayerInteractable>();
		private List<AIActor> compsSaved = new List<AIActor>();
		private Dictionary<PassiveItem, bool> savePassivesCanBeDropped = new Dictionary<PassiveItem, bool>(); 
		private Dictionary<AIActor, bool> saveEnemiesAreInvulnerable = new Dictionary<AIActor, bool>(); 
		private Dictionary<AIActor, bool> saveEnemiesHaveHealthHaverInvunerability = new Dictionary<AIActor, bool>(); 

		private List<ActionsToBeRecorded> actions = new List<ActionsToBeRecorded>();

		private List<Vector2> dodgeRollDirection = new List<Vector2>();
		private List<Vector3> playerPositionsDuringActivation = new List<Vector3>();
		private List<Vector3> aimDirectionWhileFiring = new List<Vector3>();
		private List<Vector3> projsPositions = new List<Vector3>();
		private List<float> gunAngleWhenFired = new List<float>();
		private List<int> projsFired = new List<int>();
    }
}
