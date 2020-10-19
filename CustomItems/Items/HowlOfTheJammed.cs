using ItemAPI;
using System.Collections.Generic;
using UnityEngine;


namespace GlaurungItems.Items
{
	class HowlOfTheJammed : PlayerItem
	{
		public static void Init()
		{
			string text = "Howl of the Jammed";
			string resourcePath = "GlaurungItems/Resources/howlofthejammed";
			GameObject gameObject = new GameObject(text);
			HowlOfTheJammed item = gameObject.AddComponent<HowlOfTheJammed>();
			ItemBuilder.AddSpriteToObject(text, resourcePath, gameObject);
			string shortDesc = "Angery";
			string longDesc = "This record plays the mad screech of the Lord of the Jammed when used.";
			item.SetupItem(shortDesc, longDesc, "gl");
			item.SetCooldownType(ItemBuilder.CooldownType.Damage, 500f);
			ItemBuilder.AddPassiveStatModifier(item, PlayerStats.StatType.Curse, 1.5f, 0);
			item.quality = ItemQuality.D;
		}

		protected override void DoEffect(PlayerController user)
		{
			if (user && user.CurrentRoom != null && user.CurrentRoom.GetActiveEnemies(0) != null)
			{
				AkSoundEngine.PostEvent("Play_ENM_reaper_spawn_01", base.gameObject);
				user.PlayEffectOnActor(ResourceCache.Acquire("Global VFX/VFX_Curse") as GameObject, Vector3.zero, true, false, false);

				List<AIActor> enemies = user.CurrentRoom.GetActiveEnemies(0);
				foreach (AIActor enemy in enemies)
				{
					enemy.BecomeBlackPhantom();
				}
			}
		}

		public override bool CanBeUsed(PlayerController user)
		{
			return user.IsInCombat;
		}
	}
}
