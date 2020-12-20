using ItemAPI;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace GlaurungItems.Items
{
	class PrismaticAura : PlayerItem
	{
		public static void Init()
		{
			string text = "Prismatic Aura";
			string resourcePath = "GlaurungItems/Resources/acme_crate";
			GameObject gameObject = new GameObject(text);
			PrismaticAura item = gameObject.AddComponent<PrismaticAura>();
			ItemBuilder.AddSpriteToObject(text, resourcePath, gameObject);
			string shortDesc = "WIP";
			string longDesc = "";
			item.SetupItem(shortDesc, longDesc, "gl");
			item.SetCooldownType(ItemBuilder.CooldownType.Damage, 500f);
			item.quality = ItemQuality.B;
		}

		protected override void DoEffect(PlayerController user)
		{
			this.StartEffect(user);
			base.StartCoroutine(ItemBuilder.HandleDuration(this, this.duration, user, new Action<PlayerController>(this.EndEffect)));
		}

		private void StartEffect(PlayerController user)
		{
			wasUsed = true;
		}

		public override bool CanBeUsed(PlayerController user)
		{
			return user.IsInCombat;
		}

		private void EndEffect(PlayerController user)
		{
			wasUsed = false;
		}

		public override void Update()
		{
			if (this.LastOwner && this.wasUsed)
			{
				this.DoAura();
				this.HandleRadialIndicator();
				
			}
			else
			{
				this.UnhandleRadialIndicator();
			}
			base.Update();
		}

		protected override void OnPreDrop(PlayerController user)
		{
			EndEffect(user);
		}

		protected virtual void DoAura()
		{
			bool didDamageEnemies = false;
			PlayerController playerController = this.LastOwner as PlayerController;
			if (this.AuraAction == null)
			{
				this.AuraAction = delegate (AIActor actor, float dist)
				{
					float num2 = this.DamagePerSecond * BraveTime.DeltaTime;
					if (num2 > 0f)
					{
						didDamageEnemies = true;
					}
					actor.healthHaver.ApplyDamage(num2, Vector2.zero, "Aura", this.damageTypes, DamageCategory.Normal, false, null, false);
				};
			}
			if (playerController != null && playerController.CurrentRoom != null)
			{
				float num = this.AuraRadius;

				if (this.m_radialIndicator)
				{
					this.m_radialIndicator.CurrentRadius = num;
				}
				playerController.CurrentRoom.ApplyActionToNearbyEnemies(playerController.CenterPosition, num, this.AuraAction);
			}
			if (didDamageEnemies)
			{
				playerController.DidUnstealthyAction();
			}
		}

		private void HandleRadialIndicator()
		{
			if (!this.m_radialIndicatorActive)
			{
				this.m_radialIndicatorActive = true;
				this.m_radialIndicator = ((GameObject)UnityEngine.Object.Instantiate(ResourceCache.Acquire("Global VFX/HeatIndicator"), this.LastOwner.CenterPosition.ToVector3ZisY(0f), Quaternion.identity, this.LastOwner.transform)).GetComponent<HeatIndicatorController>();
				this.m_radialIndicator.CurrentColor = Color.cyan.WithAlpha(30f);
			}
		}

		private void UnhandleRadialIndicator()
		{
			if (this.m_radialIndicatorActive)
			{
				this.m_radialIndicatorActive = false;
				if (this.m_radialIndicator)
				{
					this.m_radialIndicator.EndEffect();
				}
				this.m_radialIndicator = null;
			}
		}

		private float duration = 10f;
		private bool wasUsed = false;
		private bool m_radialIndicatorActive;
		private HeatIndicatorController m_radialIndicator;
		private Action<AIActor, float> AuraAction;
		public float AuraRadius = 5;
		public CoreDamageTypes damageTypes;
		public float DamagePerSecond = 5;
	}
}
