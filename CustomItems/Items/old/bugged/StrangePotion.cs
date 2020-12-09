using ItemAPI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using Random = UnityEngine.Random;

namespace GlaurungItems.Items
{
    class StrangePotion : PlayerItem
	{
		// do not use, it f* up the player health if RecalculateStats is called (dropping a gun)
		public static void Init()
		{
			string text = "Strange Potion";
			string resourcePath = "GlaurungItems/Resources/neuralyzer";
			GameObject gameObject = new GameObject(text);
			StrangePotion item = gameObject.AddComponent<StrangePotion>();
			ItemBuilder.AddSpriteToObject(text, resourcePath, gameObject);
			string shortDesc = "Could be poisonous";
			string longDesc = "This flask contains an odd oily-looking liquid... Will you drink it ?";
			item.SetupItem(shortDesc, longDesc, "gl");
			item.SetCooldownType(ItemBuilder.CooldownType.Damage, 1500);
			item.quality = ItemQuality.B;
		}

		protected override void DoEffect(PlayerController user)
		{
			base.DoEffect(user);
			float sel = Random.Range(0f, 1f);
			if(sel <= 0.3f)
            {
				AIActorDebuffEffect debuffEffect2 = null;
				foreach (AttackBehaviorBase attackBehaviour in EnemyDatabase.GetOrLoadByGuid((PickupObjectDatabase.GetById(492) as CompanionItem).CompanionGuid).behaviorSpeculator.AttackBehaviors)
				{
					if (attackBehaviour is WolfCompanionAttackBehavior)
					{
						debuffEffect2 = (attackBehaviour as WolfCompanionAttackBehavior).EnemyDebuff;
					}
				}
				debuffEffect2.AffectsPlayers = true;
				debuffEffect2.KeepHealthPercentage = false;
				user.ApplyEffect(debuffEffect2);
				user.stats.RecalculateStats(user, true);
			}
			else
            {
				AIActor aiactor2 = EnemyDatabase.GetOrLoadByGuid(EnemyGuidDatabase.Entries["aged_gunsinger"]);
				AttackBehaviorGroup attackBehaviorGroup2 = (aiactor2.behaviorSpeculator.AttackBehaviors[0] as AttackBehaviorGroup);
				BuffEnemiesBehavior buffBehavior2 = (attackBehaviorGroup2.AttackBehaviors[0].Behavior as BuffEnemiesBehavior);
				AIActorBuffEffect buffEffect2 = buffBehavior2.buffEffect;
				buffEffect2.AffectsEnemies = false;
				buffEffect2.AffectsPlayers = true;
				buffEffect2.KeepHealthPercentage = false;
				buffEffect2.duration = 10f;
				user.ApplyEffect(buffEffect2);
				user.stats.RecalculateStats(user, true);
			}
		}
	}
}
