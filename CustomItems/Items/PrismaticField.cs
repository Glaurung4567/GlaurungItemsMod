using ItemAPI;
using System;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

namespace GlaurungItems.Items
{
	class PrismaticField : PlayerItem
	{
		public static void Init()
		{
			string text = "Prismatic Field";
			string resourcePath = "GlaurungItems/Resources/prismatic_field";
			GameObject gameObject = new GameObject(text);
			PrismaticField item = gameObject.AddComponent<PrismaticField>();
			ItemBuilder.AddSpriteToObject(text, resourcePath, gameObject);
			string shortDesc = "Sphere and Spray Combined";
			string longDesc = "Create an aura of chaotic magical energies around the user. Used by a Gungeoneer expert in evocation magic who liked to fight at close range, even though he was squishy...";
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
			iter = 0;
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
                if (iter % framesEffectInterval == 0)
                {
					int randInt = Random.Range(0, 8);
					GameActorEffect effect = null;
					damagePerSecond = 5;
					bool stun = false;
					bool planeShift = false;
                    switch (randInt)
                    {
						case 0:
							selectedColor = Color.red;
							effect = (PickupObjectDatabase.GetById(125) as Gun).DefaultModule.projectiles[0].fireEffect;
							break;
						case 1:
							selectedColor = Color.cyan;
							effect = (PickupObjectDatabase.GetById(402) as Gun).DefaultModule.projectiles[0].freezeEffect;
							break;
						case 2:
							selectedColor = Color.grey;
							effect = new GameActorPetrifyEffect(this.LastOwner);
							break;
						case 3:
							selectedColor = Color.white;
							stun = true;
							break;
						case 4:
							selectedColor = Color.green;
							effect = (PickupObjectDatabase.GetById(513) as Gun).DefaultModule.projectiles[0].healthEffect;
							break;
						case 5:
							selectedColor = new Color(244f / 255f, 3f / 255f, 252f / 255f);
							effect = (PickupObjectDatabase.GetById(379) as Gun).DefaultModule.projectiles[0].charmEffect;
							break;
						case 6:
							selectedColor = Color.black;
							damagePerSecond = 25;
							break;
						case 7:
							selectedColor = Color.magenta;
							planeShift = true;
							break;
						default:
							break;
                    }

					this.AuraAction = delegate (AIActor actor, float dist)
					{
						float num2 = this.damagePerSecond * BraveTime.DeltaTime;
						if (num2 > 0f)
						{
							if(effect != null)
                            {
								actor.ApplyEffect(effect);
							}
                            if (stun)
                            {
								actor.behaviorSpeculator.Stun(3f);
                            }
							if(planeShift)
                            {
								if(actor.healthHaver && actor.healthHaver.IsAlive && !actor.healthHaver.IsBoss && 
								this.LastOwner && this.LastOwner.CurrentRoom != null)
                                {
									actor.TeleportSomewhere();
                                }
                            }
							didDamageEnemies = true;
						}
						actor.healthHaver.ApplyDamage(num2, Vector2.zero, "Aura", this.damageTypes, DamageCategory.Normal, false, null, false);
					};
				}
				this.DoAura();
				this.HandleRadialIndicator();
				iter++;
			}
			else
			{
				this.UnhandleRadialIndicator();
			}
			base.Update();
		}

		protected override void OnPreDrop(PlayerController user)
		{
			this.UnhandleRadialIndicator();
			EndEffect(user);
			base.OnPreDrop(user);
		}

		protected virtual void DoAura()
		{
			didDamageEnemies = false;
			PlayerController playerController = this.LastOwner as PlayerController;

			if (this.AuraAction == null)
			{
				this.AuraAction = delegate (AIActor actor, float dist)
				{
					float num2 = this.damagePerSecond * BraveTime.DeltaTime;
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
				this.m_radialIndicator.IsFire = false;
			}
			this.m_radialIndicator.CurrentColor = selectedColor.WithAlpha(30f);
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

		private float duration = 12f;
		private int framesEffectInterval = 160;
		private bool wasUsed = false;
		private int iter;

		private List<Color> colors = new List<Color>
		{
			Color.green,
			Color.red,
			Color.cyan,
			Color.grey,
			new Color(244f/255f, 3f/255f, 252f/255f)
		};
		private Color selectedColor;

		private bool didDamageEnemies;
		private bool m_radialIndicatorActive;
		private HeatIndicatorController m_radialIndicator;
		private Action<AIActor, float> AuraAction;
		public float AuraRadius = 5;
		public CoreDamageTypes damageTypes;
		public float damagePerSecond = 5;
	}

	/*----------------------------------------------------------------------------------*/
	public class GameActorPetrifyEffect : GameActorEffect
	{
		public GameActorPetrifyEffect(PlayerController owner)
		{
			this.AffectsPlayers = false;
			this.duration = 4f;
			this.AppliesTint = true;
			this.TintColor = new Color(0.2f, 0.2f, 0.2f, Mathf.Clamp01(this.duration));
			this.Owner = owner;
		}

		public bool ShouldVanishOnDeath(GameActor actor)
		{
			return (!actor.healthHaver || !actor.healthHaver.IsBoss) && (!(actor is AIActor) || !(actor as AIActor).IsSignatureEnemy);
		}

		public override void OnEffectApplied(GameActor actor, RuntimeGameActorEffectData effectData, float partialAmount = 1f)
		{
			bool flag = !actor.healthHaver.IsBoss && !actor.healthHaver.IsDead;
			if (flag)
			{
				AIActor aiActor = actor.aiActor;
				this.prev = aiActor.CanTargetPlayers;
				aiActor.CanTargetPlayers = false;
				aiActor.MovementSpeed = 0f;
				base.OnEffectApplied(actor, effectData, partialAmount);
			}
		}

		public override void OnEffectRemoved(GameActor actor, RuntimeGameActorEffectData effectData)
		{
			AIActor aiActor = actor.aiActor;
			aiActor.MovementSpeed = aiActor.BaseMovementSpeed;
			aiActor.CanTargetPlayers = this.prev;
			base.OnEffectRemoved(actor, effectData);
		}

		private bool prev = true;

		public PlayerController Owner;
	}
}


