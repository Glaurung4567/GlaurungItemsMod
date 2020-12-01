using ItemAPI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace GlaurungItems.Items
{
    class CreatePit : PlayerItem
	{
		public static void Init()
		{
			string text = "Create Pit";
			string resourcePath = "GlaurungItems/Resources/acme_crate";
			GameObject gameObject = new GameObject(text);
			CreatePit item = gameObject.AddComponent<CreatePit>();
			ItemBuilder.AddSpriteToObject(text, resourcePath, gameObject);
			string shortDesc = "WIP";
			string longDesc = "WIP";
			item.SetupItem(shortDesc, longDesc, "gl");
			item.SetCooldownType(ItemBuilder.CooldownType.Damage, 5f);
			item.quality = ItemQuality.C;
		}

		protected override void DoEffect(PlayerController user)
		{
			DungeonPlaceableBehaviour trap = RobotDave.GetPitTrap();
			//gameObject1.AddComponent<PitTrapEnemyController>();
			GameObject t = trap.InstantiateObject(user.CurrentRoom, user.CenterPosition.ToIntVector2());
		}

		public override bool CanBeUsed(PlayerController user)
		{
			return user.CurrentRoom != null;
		}
	}
}
