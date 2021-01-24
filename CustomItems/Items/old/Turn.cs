using ItemAPI;
using System;
using System.Collections.Generic;
using UnityEngine;

/*
on first use, slow time for other enemies
while the user move or fire, refills the item use meter and stock the path and shot places
on second use tp back to where the user was then redo the travel really fast

cannot interact with interactables while using turn
immunity to fire/poison/contact dmg while using turn

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
			item.quality = ItemQuality.A;
		}

		protected override void DoEffect(PlayerController user)
		{
            if (!isActive)
            {
				actions = new List<string>();
				startingTurnPosition = user.transform.position;
				isActive = true;
            }
            else
            {
				foreach(string act in actions)
                {
					Tools.Print(act, "ffffff", true);
                }
				isActive = false;
            }
		}


		public override bool CanBeUsed(PlayerController user)
		{
			return !user.IsInCombat;
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
					isCurrentlyDodgeRolling = true;
					actions.Add(actionsToBeRecorded[0]);
				}
				else if (user.IsFiring && !user.IsDodgeRolling)
                {
					isCurrentlyDodgeRolling = false;
					actions.Add(actionsToBeRecorded[1]);
				}
				else if (!user.IsDodgeRolling)
                {
					isCurrentlyDodgeRolling = false;
					actions.Add(actionsToBeRecorded[2]);
				}
			}
        }

        private bool isActive = false;
		private bool isCurrentlyDodgeRolling = false;
		private Vector3 startingTurnPosition;
		private List<string> actions = new List<string>();
		private readonly static string[] actionsToBeRecorded = new string[]
		{
			"dodgeroll",
			"shooting",
			"moving",
		};
	}
}
