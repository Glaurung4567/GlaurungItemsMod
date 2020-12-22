using ItemAPI;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace GlaurungItems.Items
{
    class Neuralyzer: PlayerItem
    {
		public static void Init()
		{
			string text = "Neuralyzer";
			string resourcePath = "GlaurungItems/Resources/neuralyzer";
			GameObject gameObject = new GameObject(text);
			Neuralyzer item = gameObject.AddComponent<Neuralyzer>();
			ItemBuilder.AddSpriteToObject(text, resourcePath, gameObject);
			string shortDesc = "Flashy Thing";
			string longDesc = "This was brought to the Gungeon by a strange man in a black suit. \n \n" +
				"After being used too many times, it only erase the memories of the targets for 5 seconds after which they regain their composure.";
			item.SetupItem(shortDesc, longDesc, "gl");
			item.SetCooldownType(ItemBuilder.CooldownType.Damage, 500f);
			item.quality = ItemQuality.A;
			item.AddItemToTrorcMetaShop(5);//second argument is the position in the shop item list
			Toolbox.SetupUnlockOnCustomFlag(item, CustomDungeonFlags.ITEMSPECIFIC_NEURALIZER, true);
		}

		protected override void DoEffect(PlayerController user)
		{
			this.StartEffect(user);
			base.StartCoroutine(ItemBuilder.HandleDuration(this, this.duration, user, new Action<PlayerController>(this.EndEffect)));
		}

		private void StartEffect(PlayerController user)
		{
			wasUsed = true;
			if(user && user.CurrentRoom != null && user.CurrentRoom.GetActiveEnemies(0) != null)
            {

				Projectile projectile = ((Gun)ETGMod.Databases.Items[481]).DefaultModule.chargeProjectiles[0].Projectile;
				GameObject gameObject = SpawnManager.SpawnProjectile(projectile.gameObject, user.CenterPosition, Quaternion.Euler(0f, 0f, 0f), true);
				Projectile flash = gameObject.GetComponent<Projectile>();
				flash.SetOwnerSafe(user, "Player");
				flash.Shooter = user.specRigidbody;
				flash.Owner = user;
				flash.baseData.damage = 0f;
				flash.baseData.force = 0f;

				List<AIActor> enemies = user.CurrentRoom.GetActiveEnemies(0);
				foreach (AIActor enemy in enemies)
                {
					enemy.CanTargetPlayers = false;
                }
            }
		}

		public override bool CanBeUsed(PlayerController user)
		{
			return user.IsInCombat;
		}

		private void EndEffect(PlayerController user)
		{
			if (wasUsed && user.CurrentRoom != null && user.IsInCombat && user.CurrentRoom.GetActiveEnemies(0) != null)
			{
				List<AIActor> enemies = user.CurrentRoom.GetActiveEnemies(0);
				foreach (AIActor enemy in enemies)
				{
					enemy.CanTargetPlayers = true;
				}
			}
			wasUsed = false;
		}

		protected override void OnPreDrop(PlayerController user)
		{
			EndEffect(user);
			base.OnPreDrop(user);
		}

		private float duration = 5f;
		private bool wasUsed = false;

	}
}
