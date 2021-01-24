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
Stop time
Give transistor, prevent drop and lock
Fire gun save
Make user intangible
Prevent interactions 
Prevent blanks
Cancel action

*/
namespace GlaurungItems.Items
{
    class Turn : PlayerItem
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
            if (!isActive)
            {
				startingTurnPosition = user.transform.position;

				actions = new List<actionsToBeRecorded>();
				dodgeRollDirection = new List<Vector2>();
				playerPositionsDuringActivation = new List<Vector3>();
				aimDirectionWhileFiring = new List<Vector2>();

				isActive = true;
            }
            else
            {
				user.WarpToPoint(startingTurnPosition);
				GameManager.Instance.StartCoroutine(DoTurn(user));
				isActive = false;
			}
		}

		public override void Update()
		{
			base.Update();
			if (base.LastOwner && isActive)
			{
				PlayerController user = base.LastOwner;
				if (this.CurrentDamageCooldown > 0)
				{
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
						Vector2 aim = user.unadjustedAimPoint.XY() - user.CenterPosition;
						aimDirectionWhileFiring.Add(aim);
						this.CurrentDamageCooldown -= 100f;
					}
					else if (!user.IsDodgeRolling)
					{
						isCurrentlyDodgeRolling = false;

						actions.Add(actionsToBeRecorded.Moving);
						playerPositionsDuringActivation.Add(user.transform.position);
						int lenPos = playerPositionsDuringActivation.Count;
						if (lenPos > 1 && playerPositionsDuringActivation[lenPos - 2] != playerPositionsDuringActivation[lenPos -1])
                        {
							this.CurrentDamageCooldown -= 2f;
						}
					}
				}

			}
		}


		private IEnumerator DoTurn(PlayerController user)
        {
			user.SetInputOverride("turn");
			foreach (actionsToBeRecorded act in actions)
			{
				//Tools.Print(act, "ffffff", true);
				if (act == actionsToBeRecorded.Dodgeroll)
				{
					user.ForceStartDodgeRoll(dodgeRollDirection[0]);
					dodgeRollDirection.RemoveAt(0);
					yield return new WaitForSeconds(0.8f);
				}
				if (act == actionsToBeRecorded.Moving)
				{
					user.ForceMoveToPoint(playerPositionsDuringActivation[0]);
					yield return null;
					playerPositionsDuringActivation.RemoveAt(0);
				}
				if(act == actionsToBeRecorded.Shooting)
                {
					user.forceAimPoint = aimDirectionWhileFiring[0];
					user.CurrentGun.ForceFireProjectile(user.CurrentGun.DefaultModule.projectiles[0]);
					yield return null;
					user.forceAimPoint = null;
					aimDirectionWhileFiring.RemoveAt(0);
				}
			}
			user.ClearInputOverride("turn");
			yield break;
		}

        public override bool CanBeUsed(PlayerController user)
		{
			return !user.IsInCombat;// && !user.HasPassiveItem(436);
		}

		protected override void OnPreDrop(PlayerController user)
		{
			isActive = false;
			base.OnPreDrop(user);
		}


        private bool isActive = false;
		private bool isCurrentlyDodgeRolling = false;
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
		private List<Vector2> aimDirectionWhileFiring = new List<Vector2>();
    }
}
