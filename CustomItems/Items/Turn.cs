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
			string text = "Turn";
			string resourcePath = "GlaurungItems/Resources/neuralyzer";
			GameObject gameObject = new GameObject(text);
			Neuralyzer item = gameObject.AddComponent<Neuralyzer>();
			ItemBuilder.AddSpriteToObject(text, resourcePath, gameObject);
			string shortDesc = "In circles";
			string longDesc = "";
			item.SetupItem(shortDesc, longDesc, "gl");
			item.SetCooldownType(ItemBuilder.CooldownType.Damage, 500f);
			item.quality = ItemQuality.A;
		}

		protected override void DoEffect(PlayerController user)
		{
            if (!isActive)
            {
				isActive = true;
            }
            else
            {
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
            if (base.LastOwner)
            {
				PlayerController user = base.LastOwner;
                if (user.IsDodgeRolling)
                {

                }
				else if (user.IsFiring)
                {

                }
				else if (!user.IsStationary)
                {

                }
            }
        }

        private bool isActive = false;
		private List<string> actions = new List<string>();
	}
}
