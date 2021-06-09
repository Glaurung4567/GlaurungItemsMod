﻿using Dungeonator;
using ItemAPI;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace GlaurungItems.Items
{
	class TopsyTurvyBomb : PlayerItem
	{
		public static void Init()
		{
			string text = "Topsy-Turvy Bomb";
			string resourcePath = "GlaurungItems/Resources/neuralyzer";
			GameObject gameObject = new GameObject(text);
			TopsyTurvyBomb item = gameObject.AddComponent<TopsyTurvyBomb>();
			ItemBuilder.AddSpriteToObject(text, resourcePath, gameObject);
			string shortDesc = "Gungo is 3d ?!?";
			string longDesc = "";
			item.SetupItem(shortDesc, longDesc, "gl");
			item.SetCooldownType(ItemBuilder.CooldownType.Timed, 1f);
			ItemBuilder.AddPassiveStatModifier(item, PlayerStats.StatType.AdditionalItemCapacity, 1, StatModifier.ModifyMethod.ADDITIVE);
			item.quality = ItemQuality.EXCLUDED;
		}

		protected override void DoEffect(PlayerController user)
		{
			List<AIActor> activeEnemies = GameManager.Instance.Dungeon.data.GetAbsoluteRoomFromPosition(user.CenterPosition.ToIntVector2(VectorConversions.Round)).GetActiveEnemies(RoomHandler.ActiveEnemyType.All);

			for (int j = 0; j < activeEnemies.Count; j++)
			{
				AIActor actor = activeEnemies[j];
				if (actor)
				{
					GameManager.Instance.StartCoroutine(HandleUpAndSlam(actor));

					//actor.sprite.UpdateZDepth();
				}
			}
		}

        private IEnumerator HandleUpAndSlam(AIActor actor)
        {
			for(int i = 0; i < 50; i++)
            {
				actor.sprite.HeightOffGround += 1.02f;
				actor.transform.position = actor.transform.position + new Vector3(0f, 0.02f, 0f);
				if (actor.ShadowObject)
				{
					actor.ShadowObject.transform.position = actor.ShadowObject.transform.position + new Vector3(0f, -0.02f, 0f);
				}
				actor.sprite.UpdateZDepth();
				yield return new WaitForSeconds(0.1f);
			}

			for (int i = 0; i < 50; i++)
			{
				actor.sprite.HeightOffGround -= 1.02f;
				actor.transform.position = actor.transform.position + new Vector3(0f, -0.02f, 0f);
				if (actor.ShadowObject)
				{
					actor.ShadowObject.transform.position = actor.ShadowObject.transform.position + new Vector3(0f, 0.02f, 0f);
				}
				actor.sprite.UpdateZDepth();
				yield return new WaitForSeconds(0.1f);
			}

			yield break;
		}

        public override bool CanBeUsed(PlayerController user)
		{
			return user.CurrentRoom != null;
		}
	}
}
