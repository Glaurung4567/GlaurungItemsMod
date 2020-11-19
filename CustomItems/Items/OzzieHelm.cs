using ItemAPI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using Random = UnityEngine.Random;

namespace GlaurungItems.Items
{
    class OzzieHelm : PassiveItem
	{
		public static void Init()
		{
			string name = "Ozzie 'Helm'";
			string resourcePath = "GlaurungItems/Resources/acme_crate";
			GameObject gameObject = new GameObject(name);
			OzzieHelm item = gameObject.AddComponent<OzzieHelm>();
			ItemBuilder.AddSpriteToObject(name, resourcePath, gameObject);
			string shortDesc = "Gah !";
			string longDesc = "A really sturdy underpant which can be used as a helm, though it's not advised as it come which confusing side effects.";
			item.SetupItem(shortDesc, longDesc, "gl");
			item.quality = PickupObject.ItemQuality.D;
			item.AddPassiveStatModifier(PlayerStats.StatType.Accuracy, 0.3f, StatModifier.ModifyMethod.ADDITIVE);
			item.AddPassiveStatModifier(PlayerStats.StatType.Coolness, -2f, StatModifier.ModifyMethod.ADDITIVE);
			item.AddPassiveStatModifier(PlayerStats.StatType.DodgeRollDistanceMultiplier, -1.2f, StatModifier.ModifyMethod.MULTIPLICATIVE);
		}

		public override void Pickup(PlayerController player)
		{
			base.Pickup(player);
			player.healthHaver.ModifyDamage += this.PreventDamage;
		}

		public override DebrisObject Drop(PlayerController player)
		{
			player.healthHaver.ModifyDamage -= this.PreventDamage;
			return base.Drop(player);
		}

		protected override void OnDestroy()
		{
			base.Owner.healthHaver.ModifyDamage -= this.PreventDamage;
			base.OnDestroy();
		}

		private void PreventDamage(HealthHaver healthHaver, HealthHaver.ModifyDamageEventArgs args)
        {
			if (args == EventArgs.Empty)
			{
				return;
			}
			float num = Random.Range(0f, 1f);
			if(num <= 0.15f)
            {
				args.ModifiedDamage = 0;
			}
		}

	}
}
