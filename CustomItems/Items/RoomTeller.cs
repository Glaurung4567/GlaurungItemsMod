using EnemyAPI;
using ItemAPI;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GlaurungItems.Items
{
	class RoomTeller : PlayerItem
	{
		public static void Init()
		{
			string text = "RoomTeller";
			string resourcePath = "GlaurungItems/Resources/neuralyzer";
			GameObject gameObject = new GameObject(text);
			RoomTeller item = gameObject.AddComponent<RoomTeller>();
			ItemBuilder.AddSpriteToObject(text, resourcePath, gameObject);
			string shortDesc = "For room modders";
			string longDesc = "";
			item.SetupItem(shortDesc, longDesc, "gl");
			item.SetCooldownType(ItemBuilder.CooldownType.Timed, 1f);
			ItemBuilder.AddPassiveStatModifier(item, PlayerStats.StatType.AdditionalItemCapacity, 1, StatModifier.ModifyMethod.ADDITIVE);
			item.quality = ItemQuality.EXCLUDED;
		}

		protected override void DoEffect(PlayerController user)
		{
			Tools.Print(user.CurrentRoom.GetRoomName(), "ffffff", true);
			string header = user.CurrentRoom.DungeonWingID.ToString();
			string text = user.CurrentRoom.GetRoomName();
			this.Notify(header, text);

			GameManager.Instance.StartCoroutine(SpawnActiveRecharger(user));
		}

        private IEnumerator SpawnActiveRecharger(PlayerController user)
        {
			AIActor orLoadByGuid = EnemyDatabase.GetOrLoadByGuid(EnemyGuidDatabase.Entries["test_dummy"]);
			IntVector2? intVector = new IntVector2?(user.CurrentRoom.GetRandomVisibleClearSpot(1, 1));
			AIActor aiactor = AIActor.Spawn(orLoadByGuid.aiActor, intVector.Value, GameManager.Instance.Dungeon.data.GetAbsoluteRoomFromPosition(intVector.Value), true, AIActor.AwakenAnimationType.Default, true);
			yield return null;
			Projectile grenade = ((Gun)ETGMod.Databases.Items[480]).DefaultModule.chargeProjectiles[0].Projectile;
			GameObject gameObject = SpawnManager.SpawnProjectile(grenade.gameObject, intVector.Value.ToVector3(), Quaternion.Euler(0f, 0f, 0f) , true);
			Projectile projectile = gameObject.GetComponent<Projectile>();
			projectile.baseData.force = 0;
			projectile.baseData.range *= 0.001f;
			user.DoPostProcessProjectile(projectile);
			yield return null;
			for(int i = 0; i < 15; i++)
            {
				gameObject = SpawnManager.SpawnProjectile(grenade.gameObject, intVector.Value.ToVector3(), Quaternion.Euler(0f, 0f, 0f), true);
				projectile = gameObject.GetComponent<Projectile>();
				projectile.baseData.force = 0;
				projectile.baseData.range *= 0.001f;
				user.DoPostProcessProjectile(projectile);
				yield return null;
			}
			if(aiactor && aiactor.healthHaver && aiactor.healthHaver.IsAlive)
            {
				aiactor.EraseFromExistence(true);
            }
			
		}

		private void Notify(string header, string text)
		{
			tk2dBaseSprite notificationObjectSprite = GameUIRoot.Instance.notificationController.notificationObjectSprite;
			GameUIRoot.Instance.notificationController.DoCustomNotification(header, text, notificationObjectSprite.Collection, notificationObjectSprite.spriteId, (UINotificationController.NotificationColor)2, true, false);
		}

		public override bool CanBeUsed(PlayerController user)
		{
			return user.CurrentRoom != null;
		}
	}
}
