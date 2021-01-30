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
Make user intangible ok
No movement when record full
Give transistor, prevent drop and lock
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
			string text = "Turns";
			string resourcePath = "GlaurungItems/Resources/neuralyzer";
			GameObject gameObject = new GameObject(text);
			Turn item = gameObject.AddComponent<Turn>();
			ItemBuilder.AddSpriteToObject(text, resourcePath, gameObject);
			string shortDesc = "In circles";
			string longDesc = "";
			item.SetupItem(shortDesc, longDesc, "gl");
			item.SetCooldownType(ItemBuilder.CooldownType.Damage, 1000f);
			item.quality = ItemQuality.B;
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
				projsFired = new List<Projectile>();

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

				isRecordTimeActive = true;
				stopLocalTime = true;
			}
            else
            {
				user.WarpToPoint(startingTurnPosition);

				user.healthHaver.IsVulnerable = true;
				user.specRigidbody.RemoveCollisionLayerIgnoreOverride(collisionMask);


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

				GameManager.Instance.StartCoroutine(DoTurn(user));
				isRecordTimeActive = false;
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
							this.CurrentDamageCooldown -= 100f;
							isCurrentlyDodgeRolling = true;
							actions.Add(actionsToBeRecorded.Dodgeroll);
							dodgeRollDirection.Add(user.transform.position - playerPositionsDuringActivation[playerPositionsDuringActivation.Count - 1]);
						}

					}

					else if (user.IsFiring && !user.IsDodgeRolling)
					{
						isCurrentlyDodgeRolling = false;

						actions.Add(actionsToBeRecorded.Shooting);
						Vector3 aim = (user.unadjustedAimPoint);// - user.CenterPosition);
						aimDirectionWhileFiring.Add(aim);
						gunAngleWhenFired.Add(user.CurrentGun.CurrentAngle);
						this.CurrentDamageCooldown -= 100f;
					}

					else if (!user.IsDodgeRolling)
					{
						isCurrentlyDodgeRolling = false;

						int lenPos = playerPositionsDuringActivation.Count;
						if (lenPos > 1 && playerPositionsDuringActivation[lenPos - 1] != user.transform.position)
                        {
							actions.Add(actionsToBeRecorded.Moving);
							playerPositionsDuringActivation.Add(user.transform.position);
							this.CurrentDamageCooldown -= 1f;
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

				if (Key(GungeonActions.GungeonActionType.Reload) && KeyTime(GungeonActions.GungeonActionType.Reload) > 1.25f)
				{

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
					user.unadjustedAimPoint = aimDirectionWhileFiring[0];
					user.ForceStaticFaceDirection(aimDirectionWhileFiring[0]);
					user.ForceIdleFacePoint(aimDirectionWhileFiring[0]);
					user.CurrentGun.HandleAimRotation(aimDirectionWhileFiring[0]);
					
					GameObject gameObject = SpawnManager.SpawnProjectile(user.CurrentGun.DefaultModule.projectiles[0].gameObject, user.sprite.WorldCenter, Quaternion.Euler(0f, 0f, gunAngleWhenFired[0]), true);
					Projectile projectile = gameObject.GetComponent<Projectile>();
					//projectile.transform.parent = user.CurrentGun.barrelOffset;
					user.DoPostProcessProjectile(projectile);
					//user.CurrentGun.ForceFireProjectile(user.CurrentGun.DefaultModule.projectiles[0]);
					//user.forceAimPoint = null;
					yield return new WaitForSeconds(0.2f);
					aimDirectionWhileFiring.RemoveAt(0);
					gunAngleWhenFired.RemoveAt(0);
				}
			}

			yield return null;
			Time.timeScale = 1;
			user.CurrentInputState = PlayerInputState.AllInput;
			user.ClearInputOverride("turn");
			stopLocalTime = false;
			yield break;
		}

        public override bool CanBeUsed(PlayerController user)
		{
			return user.IsInCombat 
				&& user.CurrentRoom != null
				&& user.CurrentRoom.IsSealed
				//&& ((!isActive && !user.inventory.GunLocked.Value) || (isActive && user.inventory.GunLocked.Value))
				&& !user.HasPassiveItem(436) //bloodied scarf
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
				hasFired = true;
				projsFired.Add(proj);
				GameManager.Instance.StartCoroutine(ResetHasFired());
				proj.specRigidbody.AddCollisionLayerIgnoreOverride(collisionMask);
            }
            else
            {
				proj.DieInAir(true, false, false);
			}
		}

		private IEnumerator ResetHasFired()
        {
			yield return null;
			hasFired = false;
			yield break;
		}

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

		private static int collisionMask = CollisionMask.LayerToMask(CollisionLayer.EnemyHitBox, CollisionLayer.EnemyCollider, 
			CollisionLayer.EnemyBulletBlocker, CollisionLayer.BulletBreakable, CollisionLayer.Projectile, CollisionLayer.Pickup, 
			CollisionLayer.BeamBlocker);

		private bool isRecordTimeActive = false;
		private bool isReplayTimeActive = false;
		private bool isCurrentlyDodgeRolling = false;
		private bool hasFired = false;
		private bool wasFlyingAtTheStart = false;
		private Vector3 startingTurnPosition;

		private enum actionsToBeRecorded
		{
			Dodgeroll,
			Shooting,
			Moving,
		};
		private List<actionsToBeRecorded> actions = new List<actionsToBeRecorded>();

		private List<Vector2> dodgeRollDirection = new List<Vector2>();
		private List<Vector3> playerPositionsDuringActivation = new List<Vector3>();
		private List<Vector3> aimDirectionWhileFiring = new List<Vector3>();
		private List<float> gunAngleWhenFired = new List<float>();
		private List<Projectile> projsFired = new List<Projectile>();
    }
}
