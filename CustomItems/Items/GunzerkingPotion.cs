using ItemAPI;
using System;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

namespace GlaurungItems.Items
{
	class GunzerkingPotion : PlayerItem
	{
		public static void Init()
		{
			string text = "Gunzerking Potion";
			string resourcePath = "GlaurungItems/Resources/acme_crate";
			GameObject gameObject = new GameObject(text);
			GunzerkingPotion item = gameObject.AddComponent<GunzerkingPotion>();
			ItemBuilder.AddSpriteToObject(text, resourcePath, gameObject);
			string shortDesc = "Sexual Tyrannosaurus";
			string longDesc = "";
			item.SetupItem(shortDesc, longDesc, "gl");
			item.SetCooldownType(ItemBuilder.CooldownType.Damage, 1);
			item.quality = ItemQuality.C;
		}

		protected override void DoEffect(PlayerController user)
		{
			this.StartEffect(user);
			base.StartCoroutine(ItemBuilder.HandleDuration(this, this.duration, user, new Action<PlayerController>(this.EndEffect)));
		}

		private void StartEffect(PlayerController user)
		{
			wasUsed = true;
			if (user)
			{
				user.inventory.SetDualWielding(true, "gunzerking");
				user.inventory.GunChangeForgiveness = true;
				user.ChangeGun(1, false, false);
				//user.ChangeGun(-1, false, false);
				user.inventory.GunChangeForgiveness = false;
			}
		}

		public override bool CanBeUsed(PlayerController user)
		{
			return !user.inventory.DualWielding && user.inventory.AllGuns != null && (user.inventory.AllGuns.Count > 1);
		}

		private void EndEffect(PlayerController user)
		{
			if (wasUsed && user)
			{
				user.inventory.SetDualWielding(false, "gunzerking");
			}
			wasUsed = false;
		}

		protected override void OnPreDrop(PlayerController user)
		{
			EndEffect(user);
			base.OnPreDrop(user);
		}

		private float duration = 10f;
		private bool wasUsed = false;

	}
}
