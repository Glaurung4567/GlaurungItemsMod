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
			item.quality = ItemQuality.C;
		}

		protected override void DoEffect(PlayerController user)
		{
			List<AIActor> activeEnemies = GameManager.Instance.Dungeon.data.GetAbsoluteRoomFromPosition(user.CenterPosition.ToIntVector2(VectorConversions.Round)).GetActiveEnemies(RoomHandler.ActiveEnemyType.All);

			for (int j = 0; j < activeEnemies.Count; j++)
			{
				AIActor actor = activeEnemies[j];
				if (actor)
				{
					GameManager.Instance.StartCoroutine(HandleUpAndSlam(actor, user));

					//actor.sprite.UpdateZDepth();
				}
			}
		}

        private IEnumerator HandleUpAndSlam(AIActor actor, PlayerController user)
        {
            if (!reverseBool)
            {
				GameManager.Instance.MainCameraController.Camera.transform.Rotate(0f, 0f, 180f);
				Pixelator.Instance.DoOcclusionLayer = false;
				user.transform.transform.Rotate(0f, 0f, 180f);
				reverseBool = true;
			}

			for (int i = 0; i < nbOfSteps; i++)
            {
				//actor.sprite.HeightOffGround += heightByStep;
				actor.transform.position = actor.transform.position + new Vector3(0f, yByStep, 0f);
				if (actor.ShadowObject)
				{
					actor.ShadowObject.transform.position = actor.ShadowObject.transform.position + new Vector3(0f, -yByStep, 0f);
				}
				actor.sprite.UpdateZDepth();
				yield return new WaitForSeconds(waitBetweenEachStepUp);
			}

            if (!unreverseBool)
            {
				GameManager.Instance.MainCameraController.Camera.transform.Rotate(0f, 0f, -180f);
				Pixelator.Instance.DoOcclusionLayer = true;
				user.transform.transform.Rotate(0f, 0f, -180f);
				unreverseBool = true;
			}

			for (int i = 0; i < nbOfSteps; i++)
			{
				//actor.sprite.HeightOffGround -= .2f;
				actor.transform.position = actor.transform.position + new Vector3(0f, -yByStep, 0f);
				if (actor.ShadowObject)
				{
					actor.ShadowObject.transform.position = actor.ShadowObject.transform.position + new Vector3(0f, yByStep, 0f);
				}
				actor.sprite.UpdateZDepth();
				yield return new WaitForSeconds(waitBetweenEachStepDown);
			}

			reverseBool = false;
			unreverseBool = false;

			yield break;
		}

        public override bool CanBeUsed(PlayerController user)
		{
			return user.CurrentRoom != null;
		}

		private const float nbOfSteps = 25; 
		private const float yByStep = 0.15f; 
		private const float heightByStep = 0.33f; 
		private const float waitBetweenEachStepUp = 0.025f;
		private const float waitBetweenEachStepDown = 0.001f;
		private bool reverseBool = false;
		private bool unreverseBool = false;
	}
}
