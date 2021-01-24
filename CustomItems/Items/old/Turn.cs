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
give the transistor during turn and lock it

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
			item.SetCooldownType(ItemBuilder.CooldownType.Damage, 0f);
			item.quality = ItemQuality.B;
		}

		protected override void DoEffect(PlayerController user)
		{
            if (!isActive)
            {
				actions = new List<actionsToBeRecorded>();
				dodgeRollDirection = new List<Vector2>();
				playerPositionsDuringActivation = new List<Vector3>();
				startingTurnPosition = user.transform.position;
				isActive = true;
            }
            else
            {
				user.WarpToPoint(startingTurnPosition);
				GameManager.Instance.StartCoroutine(doTurn(user));
				isActive = false;
			}
		}

        private IEnumerator doTurn(PlayerController user)
        {
			foreach (actionsToBeRecorded act in actions)
			{
				//Tools.Print(act, "ffffff", true);
				if (act == actionsToBeRecorded.Dodgeroll)
				{
					user.ForceStartDodgeRoll(dodgeRollDirection[0]);
					dodgeRollDirection.RemoveAt(0);
					yield return new WaitForSeconds(1f);
				}
				if (act == actionsToBeRecorded.Moving)
				{

					user.ForceMoveToPoint(playerPositionsDuringActivation[0]);
					yield return null;
					playerPositionsDuringActivation.RemoveAt(0);
				}
			}
			yield break;
		}

        public override bool CanBeUsed(PlayerController user)
		{
			return !user.IsInCombat && !user.HasPassiveItem(436);
		}

		protected override void OnPreDrop(PlayerController user)
		{
			isActive = false;
			base.OnPreDrop(user);
		}

        public override void Update()
        {
            base.Update();
            if (base.LastOwner && isActive)
            {
				PlayerController user = base.LastOwner;
                if (user.IsDodgeRolling && !isCurrentlyDodgeRolling)
                {
					if(playerPositionsDuringActivation.Count > 0)
                    {
						isCurrentlyDodgeRolling = true;
						actions.Add(actionsToBeRecorded.Dodgeroll);
						dodgeRollDirection.Add(user.transform.position - playerPositionsDuringActivation[playerPositionsDuringActivation.Count - 1]);
					}
					
				}
				else if (user.IsFiring && !user.IsDodgeRolling)
                {
					isCurrentlyDodgeRolling = false;
					actions.Add(actionsToBeRecorded.Shooting);
					
				}
				else if (!user.IsDodgeRolling)
                {
					isCurrentlyDodgeRolling = false;
					actions.Add(actionsToBeRecorded.Moving);
					playerPositionsDuringActivation.Add(user.transform.position);
				}
			}
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
    }
}
