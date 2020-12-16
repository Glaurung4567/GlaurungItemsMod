using ItemAPI;
using System;
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
			item.quality = ItemQuality.EXCLUDED;
		}

		protected override void DoEffect(PlayerController user)
		{
			Tools.Print(user.CurrentRoom.GetRoomName(), "ffffff", true);
			string header = user.CurrentRoom.DungeonWingID.ToString();
			string text = user.CurrentRoom.GetRoomName();
			this.Notify(header, text);
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
