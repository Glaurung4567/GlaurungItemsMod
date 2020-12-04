using Dungeonator;
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
			item.quality = ItemQuality.EXCLUDED;
		}

		protected override void DoEffect(PlayerController user)
		{
			/*
			AssetBundle sharedAssets2 = ResourceManager.LoadAssetBundle("shared_auto_002");
			DungeonPlaceable PitTrap = sharedAssets2.LoadAsset<DungeonPlaceable>("Pit Trap");
			GameObject trap = PitTrap.InstantiateObject(user.CurrentRoom, user.PlacedPosition);
			PitTrapController pit = trap.GetComponent<PitTrapController>();
			//Destroy(trap.GetComponent<PitTrapController>());

			PitTrapEnemyController pit2 = trap.GetOrAddComponent<PitTrapEnemyController>();
			pit2.triggerMethod = (BasicTrapEnemyController.TriggerMethod)pit.triggerMethod;
			pit2.triggerTimerDelay = pit.triggerTimerDelay;
			pit2.triggerTimerDelay1 = pit.triggerTimerDelay1;
			pit2.triggerTimerOffset = pit.triggerTimerOffset;
			pit2.footprintBuffer = pit.footprintBuffer;
			pit2.damagesFlyingPlayers = pit.damagesFlyingPlayers;
			pit2.triggerOnBlank = pit.triggerOnBlank;
			pit2.triggerOnExplosion = pit.triggerOnExplosion;
			pit2.animateChildren = pit.animateChildren;
			pit2.triggerAnimName = pit.triggerAnimName;
			pit2.triggerDelay = pit.triggerDelay;
			pit2.activeAnimName = pit.activeAnimName;
			pit2.activeVfx = pit.activeVfx;
			pit2.activeTime = pit.activeTime;
			pit2.resetAnimName = pit.resetAnimName;
			pit2.triggerDelay = pit.resetDelay;
			pit2.damageMethod = pit.damageMethod;
			pit2.damage = pit.damage;
			pit2.damageTypes = pit.damageTypes;
			pit2.IgnitesGoop = pit.IgnitesGoop;
			pit2.LocalTimeScale = pit.LocalTimeScale;

			Destroy(trap.GetComponent<PitTrapController>());
			*/
			if (user.CurrentRoom != null)
			{



				float roomPosX = user.transform.position.x - user.CurrentRoom.area.basePosition.x;
				float roomPosY = user.transform.position.y - user.CurrentRoom.area.basePosition.y;
				float xOffSet = 0;
				float yOffSet = 0;
				float offsetAmount = 2f;
				float gunCurrentAngle = BraveMathCollege.Atan2Degrees(this.LastOwner.unadjustedAimPoint.XY() - this.LastOwner.CenterPosition);
				if (gunCurrentAngle > 45f && gunCurrentAngle <= 135f)
				{
					yOffSet = offsetAmount;//up
				}
				else if ((gunCurrentAngle > 0 && gunCurrentAngle > 135f) || (gunCurrentAngle < 0 && gunCurrentAngle <= -135f))
				{
					xOffSet = -offsetAmount;//left
				}
				else if (gunCurrentAngle > -135f && gunCurrentAngle <= -45f)
				{
					yOffSet = -offsetAmount;//bottom
				}
				else
				{
					xOffSet = offsetAmount;//right
				}
				Vector2 posInCurrentRoom = new Vector2(roomPosX + xOffSet, roomPosY + yOffSet);
				Vector2 posInMap = new Vector2(user.transform.position.x + xOffSet, user.transform.position.y + yOffSet).ToIntVector2().ToVector2();
				if (user.IsValidPlayerPosition(posInMap))
				{
					AssetBundle sharedAssets2 = ResourceManager.LoadAssetBundle("shared_auto_002");
					DungeonPlaceable ExplodyBarrel = sharedAssets2.LoadAsset<DungeonPlaceable>("ExplodyBarrel_Maybe");
					GameObject spawnedDrum = ExplodyBarrel.InstantiateObject(user.CurrentRoom, posInCurrentRoom.ToIntVector2());
					KickableObject componentInChildren = spawnedDrum.GetComponentInChildren<KickableObject>();
					if (componentInChildren)
					{
						componentInChildren.specRigidbody.Reinitialize();
						componentInChildren.rollSpeed = 5f;
						user.CurrentRoom.RegisterInteractable(componentInChildren);
					}
				}
				/*
				AssetBundle sharedAssets = ResourceManager.LoadAssetBundle("shared_auto_001");
				GameObject Drum = sharedAssets.LoadAsset<GameObject>("Red Drum");
				GameObject spawnedDrum = Drum.GetComponent<DungeonPlaceableBehaviour>().InstantiateObject(user.CurrentRoom, posInCurrentRoom.ToIntVector2(), false);

				KickableObject componentInChildren = spawnedDrum.GetComponentInChildren<KickableObject>();
				if (componentInChildren)
				{
					componentInChildren.specRigidbody.Reinitialize();
					componentInChildren.rollSpeed = 5f;
					user.CurrentRoom.RegisterInteractable(componentInChildren);
				}
				*/
			}
		}

		public override bool CanBeUsed(PlayerController user)
		{
			return user.CurrentRoom != null;
		}
	}
}
