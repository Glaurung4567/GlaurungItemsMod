using ItemAPI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace GlaurungItems.Items
{
    class MindBlown : PlayerItem
    {

		public static void Init()
		{
			string text = "Mind Blown";
			string resourcePath = "GlaurungItems/Resources/blinkback_device";
			GameObject gameObject = new GameObject(text);
			MindBlown item = gameObject.AddComponent<MindBlown>();
			ItemBuilder.AddSpriteToObject(text, resourcePath, gameObject);
			string shortDesc = "Nani ?";
			string longDesc = "";
			item.SetupItem(shortDesc, longDesc, "gl");
			item.SetCooldownType(ItemBuilder.CooldownType.Damage, 1000);
			item.quality = ItemQuality.A;
		}

		protected override void DoEffect(PlayerController user)
		{
			base.DoEffect(user);
			DoBlank(user.CenterPosition, user);
		}

		private void DoBlank(Vector2 centerPoint, PlayerController user)
		{
			GameObject gameObjectBlankVFX = (GameObject)ResourceCache.Acquire("Global VFX/BlankVFX");
			GameObject gameObjectBlank = new GameObject("silencer");
			SilencerInstance silencerInstance = gameObjectBlank.AddComponent<SilencerInstance>();
			silencerInstance.TriggerSilencer(centerPoint, 15f, 35f, gameObjectBlankVFX, 1f, 0.5f, 50f, 10f, 140f, 15f, 1f, user, true, false);
		}

	}
}
