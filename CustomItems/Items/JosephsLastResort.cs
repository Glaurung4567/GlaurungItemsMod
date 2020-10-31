using ItemAPI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace GlaurungItems.Items
{
    class JosephsLastResort: PlayerItem
    {
		public static void Init()
		{
			string text = "Joseph's Last Resort";
			string resourcePath = "GlaurungItems/Resources/josephlastresort";
			GameObject gameObject = new GameObject(text);
			JosephsLastResort item = gameObject.AddComponent<JosephsLastResort>();
			ItemBuilder.AddSpriteToObject(text, resourcePath, gameObject);
			string shortDesc = "Nigerundayo !";
			string longDesc = "This secret technique was used by a crafty and courageous Gungeonner to survive dangerous situations.";
			item.SetupItem(shortDesc, longDesc, "gl");
			item.SetCooldownType(ItemBuilder.CooldownType.Timed, 100f);
			item.quality = ItemQuality.C;
		}

		public override bool CanBeUsed(PlayerController user)
		{
			return user.IsInCombat;
		}

		protected override void DoEffect(PlayerController user)
		{
			this.StartEffect(user);
			base.StartCoroutine(ItemBuilder.HandleDuration(this, this.duration, user, new Action<PlayerController>(this.EndEffect)));
		}

		private void StartEffect(PlayerController user)
		{
			if (user && user.CurrentRoom != null && user.CurrentRoom.GetActiveEnemies(0) != null)
			{
				user.CurrentRoom.UnsealRoom();
				this.AddPassiveStatModifier(PlayerStats.StatType.MovementSpeed, 3f, StatModifier.ModifyMethod.ADDITIVE);
				user.stats.RecalculateStats(user, true);
				wasActivated = true;
			}
		}

		private void EndEffect(PlayerController user)
		{
            if (wasActivated)
            {
				this.AddPassiveStatModifier(PlayerStats.StatType.MovementSpeed, -3f, StatModifier.ModifyMethod.ADDITIVE);
				user.stats.RecalculateStats(user, true);
				wasActivated = false;
			}
		}

		protected override void OnPreDrop(PlayerController user)
        {
			EndEffect(user);
		}


		private float duration = 7f;
		private bool wasActivated = false;
	}
}
