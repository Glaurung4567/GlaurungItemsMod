using Gungeon;
using ItemAPI;
using System;
using System.Collections;
using System.Collections.Generic;
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
Fire all projectiles of the usb swordgun

Check items interactions
Set dmgCooldown to max
Prevent companions, coop, goop or orbital intervention
Prevent enemy spawn mob or proj on death
Prevent inventory modif
Prevent interactions 
Prevent blanks
end turn early if onleavecombat, ondrop, onitemswitch, onchangedroom, onNoEnemy, onReinforcement
*/
namespace GlaurungItems.Items
{
    class Turn : CustomRadialSlowItem
	{
		public static void Init()
		{
			string text = "Transistor";
			string resourcePath = "GlaurungItems/Resources/neuralyzer";
			GameObject gameObject = new GameObject(text);
			Turn item = gameObject.AddComponent<Turn>();
			ItemBuilder.AddSpriteToObject(text, resourcePath, gameObject);
			string shortDesc = "In circles";
			string longDesc = "";
			item.SetupItem(shortDesc, longDesc, "gl");
			item.SetCooldownType(ItemBuilder.CooldownType.Damage, 1000f);
			item.quality = ItemQuality.A;
		}

		protected override void DoEffect(PlayerController user)
		{
			if (!isRecordTimeActive)
            {
				startingTurnPosition = user.transform.position;

				user.PostProcessProjectile += User_PostProcessProjectile;
				
				actions = new List<actionsToBeRecorded>();
				dodgeRollDirection = new List<Vector2>();
				playerPositionsDuringActivation = new List<Vector3>();
				aimDirectionWhileFiring = new List<Vector3>();
				gunAngleWhenFired = new List<float>();
				projsFired = new List<int>();

				actions.Add(actionsToBeRecorded.Moving);
				playerPositionsDuringActivation.Add(user.transform.position);

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

				transistorGunInstance = user.inventory.AddGunToInventory(usb, true);
				transistorGunInstance.CanBeDropped = false;
				transistorGunInstance.CanBeSold = false;
				user.inventory.GunLocked.SetOverride("turn", true, null);

				isRecordTimeActive = true;
				stopLocalTime = true;
				Exploder.DoDistortionWave(user.CenterPosition, 0.4f, 0.15f, this.EffectRadius, 0.4f);
			}
			else
            {
				user.WarpToPoint(startingTurnPosition);

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

        public override void Update()
		{
			base.Update();
			if (base.LastOwner && isRecordTimeActive)
			{
				PlayerController user = base.LastOwner;
				
				if (this.CurrentDamageCooldown > 0)
				{

					user.CurrentInputState = PlayerInputState.AllInput;

					if (user.IsDodgeRolling && !isCurrentlyDodgeRolling)
					{
						if (playerPositionsDuringActivation.Count > 0)
						{
							isCurrentlyDodgeRolling = true;
							actions.Add(actionsToBeRecorded.Dodgeroll);
							dodgeRollDirection.Add(user.transform.position - playerPositionsDuringActivation[playerPositionsDuringActivation.Count - 1]);
							float c = Math.Min(dodgerollCost, CurrentDamageCooldown);
							if(c < dodgerollCost)
                            {
								notFullLastActionCost = c;
							}
							this.CurrentDamageCooldown -= c;
						}

					}

					else if (user.IsFiring && !user.IsDodgeRolling)
					{
						isCurrentlyDodgeRolling = false;

						//this.CurrentDamageCooldown -= shootCost1;
					}

					else if (!user.IsDodgeRolling) //for movement recording
					{
						isCurrentlyDodgeRolling = false;

						int lenPos = playerPositionsDuringActivation.Count;
						if (lenPos > 1 && playerPositionsDuringActivation[lenPos - 1] != user.transform.position)
                        {
							actions.Add(actionsToBeRecorded.Moving);
							playerPositionsDuringActivation.Add(user.transform.position);
							this.CurrentDamageCooldown -= Math.Min(movementCost, CurrentDamageCooldown);
                        }
                        else if(lenPos <= 1)
                        {
							actions.Add(actionsToBeRecorded.Moving);
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
					if(actions.Count > 0)
                    {
						int actionsLen = actions.Count;
						//Tools.Print(actions[actionsLen - 1], "ffffff", true);

						if (actions[actionsLen - 1] == actionsToBeRecorded.Shooting)
                        {
							actions.RemoveAt(actionsLen - 1);
							aimDirectionWhileFiring.RemoveAt(aimDirectionWhileFiring.Count - 1);
							gunAngleWhenFired.RemoveAt(gunAngleWhenFired.Count - 1);

							if (notFullLastActionCost > -999)
							{
								this.CurrentDamageCooldown += notFullLastActionCost;
								notFullLastActionCost = -999;
							}
							else
							{
								this.CurrentDamageCooldown += shootCosts[projsFired[projsFired.Count - 1]];
							}

							projsFired.RemoveAt(projsFired.Count - 1);
						}

						else if (actionsLen > 2 && actions[actionsLen - 1] == actionsToBeRecorded.Moving && actions[actionsLen - 2] == actionsToBeRecorded.Shooting)
						{
							actions.RemoveAt(actions.Count - 1);
							actions.RemoveAt(actions.Count - 1);
							aimDirectionWhileFiring.RemoveAt(aimDirectionWhileFiring.Count - 1);
							gunAngleWhenFired.RemoveAt(gunAngleWhenFired.Count - 1);

							user.WarpToPoint(playerPositionsDuringActivation[playerPositionsDuringActivation.Count - 2]);
							playerPositionsDuringActivation.RemoveAt(playerPositionsDuringActivation.Count - 1);
							
							if (notFullLastActionCost > -999)
							{
								this.CurrentDamageCooldown += (notFullLastActionCost + movementCost);
								notFullLastActionCost = -999;
							}
							else
							{
								this.CurrentDamageCooldown += (shootCosts[projsFired[projsFired.Count - 1]] + movementCost);
							}

							projsFired.RemoveAt(projsFired.Count - 1);
						}

						else if (actions[actionsLen - 1] == actionsToBeRecorded.Dodgeroll)
                        {
							actions.RemoveAt(actionsLen - 1);
							dodgeRollDirection.RemoveAt(dodgeRollDirection.Count - 1);
							user.WarpToPoint(playerPositionsDuringActivation[playerPositionsDuringActivation.Count - 1]);

							if (notFullLastActionCost > -999)
							{
								this.CurrentDamageCooldown += notFullLastActionCost;
								notFullLastActionCost = -999;
							}
							else
							{
								this.CurrentDamageCooldown += dodgerollCost;
							}
						}

						else if (actionsLen > 2 && actions[actionsLen - 1] == actionsToBeRecorded.Moving && actions[actionsLen - 2] == actionsToBeRecorded.Dodgeroll)
						{
							actions.RemoveAt(actions.Count- 1);
							actions.RemoveAt(actions.Count- 1);
							dodgeRollDirection.RemoveAt(dodgeRollDirection.Count - 1);

							user.WarpToPoint(playerPositionsDuringActivation[playerPositionsDuringActivation.Count - 2]);
							playerPositionsDuringActivation.RemoveAt(playerPositionsDuringActivation.Count - 1);
							
							if(notFullLastActionCost > -999)
                            {
								this.CurrentDamageCooldown += (notFullLastActionCost + movementCost);
								notFullLastActionCost = -999;
							}
                            else
                            {
								this.CurrentDamageCooldown += (dodgerollCost + movementCost);
							}
						}

						else if (actions[actionsLen - 1] == actionsToBeRecorded.Moving && actionsLen > 1)
						{
							int nbOfMovesToRemove = 1;
							for(int i = actionsLen - 1; i > 1; i--)
                            {
								if(actions[i] == actionsToBeRecorded.Moving && actions[i-1] == actionsToBeRecorded.Moving)
                                {
									nbOfMovesToRemove++;
                                }
                                else
                                {
									break;
                                }
                            }
							user.WarpToPoint(playerPositionsDuringActivation[playerPositionsDuringActivation.Count - nbOfMovesToRemove]);
							while(nbOfMovesToRemove > 0)
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
		}


        private IEnumerator DoTurn(PlayerController user)
        {
			
			user.SetInputOverride("turn");
			user.CurrentInputState = PlayerInputState.NoInput;
			Time.timeScale = 1.6f;
			foreach (actionsToBeRecorded act in actions)
			{
				if (act == actionsToBeRecorded.Dodgeroll)
				{
					user.ForceStartDodgeRoll(dodgeRollDirection[0]);
					dodgeRollDirection.RemoveAt(0);
					yield return new WaitForSeconds(1f);
				}

				if (act == actionsToBeRecorded.Moving)
				{
					if((playerPositionsDuringActivation.Count > 1 && playerPositionsDuringActivation[0] != playerPositionsDuringActivation[1]) 
						|| playerPositionsDuringActivation.Count == 1)
                    {
						//user.ForceMoveToPoint(playerPositionsDuringActivation[0]);
						user.WarpToPoint(playerPositionsDuringActivation[0]);
					}
					yield return null;
					playerPositionsDuringActivation.RemoveAt(0);
				}

				if(act == actionsToBeRecorded.Shooting)
                {
					yield return null;
                    //user.unadjustedaimpoint = aimdirectionwhilefiring[0];
                    //user.forcestaticfacedirection(aimdirectionwhilefiring[0]);
                    //user.forceidlefacepoint(aimdirectionwhilefiring[0]);
                    user.CurrentGun.HandleAimRotation(aimDirectionWhileFiring[0]);
					
					GameObject gameObject = SpawnManager.SpawnProjectile(usb.DefaultModule.chargeProjectiles[projsFired[0]].Projectile.gameObject, user.CurrentGun.barrelOffset.position, Quaternion.Euler(0f, 0f, gunAngleWhenFired[0]), true);
					Projectile projectile = gameObject.GetComponent<Projectile>();
					if(projsFired[0] == 2)
                    {
						projectile.CurseSparks = true;
                    }
					projectile.transform.parent = user.CurrentGun.barrelOffset;
					user.DoPostProcessProjectile(projectile);
					//user.CurrentGun.ForceFireProjectile(user.CurrentGun.DefaultModule.projectiles[0]);
					//user.forceAimPoint = null;
					yield return new WaitForSeconds(0.2f);
					aimDirectionWhileFiring.RemoveAt(0);
					gunAngleWhenFired.RemoveAt(0);
					projsFired.RemoveAt(0);
				}
			}

			yield return null;

			Time.timeScale = 1;
			user.inventory.GunLocked.RemoveOverride("turn");
			user.inventory.DestroyGun(transistorGunInstance);
			this.transistorGunInstance = null;
			user.CurrentInputState = PlayerInputState.AllInput;
			user.ClearInputOverride("turn");
			stopLocalTime = false;
			isReplayTimeActive = false;
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

		protected override void OnPreDrop(PlayerController user)
		{
			isRecordTimeActive = false;
			base.OnPreDrop(user);
		}

		private void User_PostProcessProjectile(Projectile proj, float arg2)
		{
            if (!hasFired)
            {
				PlayerController user = this.LastOwner;
				hasFired = true;
				int projNb = 0;
				List<string> validProjs = new List<string>{
					usb.DefaultModule.chargeProjectiles[0].Projectile.name + "(Clone)",
					usb.DefaultModule.chargeProjectiles[1].Projectile.name + "(Clone)",
					usb.DefaultModule.chargeProjectiles[2].Projectile.name + "(Clone)"
				};
                if (validProjs.Contains(proj.name) && isRecordTimeActive && CurrentDamageCooldown > 0)
                {
					if (proj.name == validProjs[0])
					{
						projNb = 0;
					}
					else if (proj.name == validProjs[1])
					{
						projNb = 1;
					}
					else if (proj.name == validProjs[2])
					{
						projNb = 2;
					}
					projsFired.Add(projNb);
					actions.Add(actionsToBeRecorded.Shooting);
					Vector3 aim = (user.unadjustedAimPoint);// - user.CenterPosition);
					aimDirectionWhileFiring.Add(aim);
					gunAngleWhenFired.Add(user.CurrentGun.CurrentAngle);
					float c = Math.Min(shootCosts[projNb], CurrentDamageCooldown);
					if (c < dodgerollCost)
					{
						notFullLastActionCost = c;
					}
					this.CurrentDamageCooldown -= c;


					Projectile projCopy = usb.DefaultModule.chargeProjectiles[projNb].Projectile;
					GameObject gameObject = SpawnManager.SpawnProjectile(projCopy.gameObject, user.CurrentGun.barrelOffset.position, Quaternion.Euler(0f, 0f, user.CurrentGun.CurrentAngle), true);
					Projectile projectileInst = gameObject.GetComponent<Projectile>();
					projectileInst.specRigidbody.AddCollisionLayerIgnoreOverride(collisionMask);
					projectileInst.specRigidbody.AddCollisionLayerIgnoreOverride(collisionMask2);
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

		private IEnumerator ResetHasFired()
        {
			yield return null;
			hasFired = false;
			yield break;
		}

		private IEnumerator CancelCooldownCoroutine()
		{
			yield return new WaitForSeconds(1.25f);
			cancelActionCooldown = false;
			yield break;
		}

		//from kyle's ileveler app
		public float KeyTime(GungeonActions.GungeonActionType action)
		{
			return BraveInput.GetInstanceForPlayer(LastOwner.PlayerIDX).ActiveActions.GetActionFromType(action).PressedDuration;
		}

		public bool KeyDown(GungeonActions.GungeonActionType action)
		{
			return BraveInput.GetInstanceForPlayer(LastOwner.PlayerIDX).ActiveActions.GetActionFromType(action).WasPressed;
		}

		public bool Key(GungeonActions.GungeonActionType action)
		{
			return BraveInput.GetInstanceForPlayer(LastOwner.PlayerIDX).ActiveActions.GetActionFromType(action).IsPressed;
		}


		//------------------------------
		private static int collisionMask = CollisionMask.LayerToMask(CollisionLayer.EnemyHitBox, CollisionLayer.EnemyCollider, 
			CollisionLayer.EnemyBulletBlocker, CollisionLayer.EnemyBlocker, CollisionLayer.Projectile, CollisionLayer.Pickup, 
			CollisionLayer.BeamBlocker);
		private static int collisionMask2 = CollisionMask.LayerToMask(CollisionLayer.BulletBlocker, CollisionLayer.BulletBreakable,
			CollisionLayer.PlayerBlocker, CollisionLayer.PlayerCollider);
		private enum actionsToBeRecorded
		{
			Dodgeroll,
			Shooting,
			Moving,
		};

		private readonly static float dodgerollCost = 100f;
		private readonly static float movementCost = .5f;
		private readonly static float[] shootCosts = {
			40f,
			200f,
			500f
		};
		private Gun usb = Game.Items["gl:usb_swordgun"] as Gun;


		private bool isRecordTimeActive = false;
		private bool isReplayTimeActive = false;

		private bool wasFlyingAtTheStart = false;
		private Vector3 startingTurnPosition;
		private Gun transistorGunInstance;

		private bool isCurrentlyDodgeRolling = false;
		private bool hasFired = false;
		private bool cancelActionCooldown = false;
		private float notFullLastActionCost = -999;

		private List<actionsToBeRecorded> actions = new List<actionsToBeRecorded>();

		private List<Vector2> dodgeRollDirection = new List<Vector2>();
		private List<Vector3> playerPositionsDuringActivation = new List<Vector3>();
		private List<Vector3> aimDirectionWhileFiring = new List<Vector3>();
		private List<float> gunAngleWhenFired = new List<float>();
		private List<int> projsFired = new List<int>();
    }
}
