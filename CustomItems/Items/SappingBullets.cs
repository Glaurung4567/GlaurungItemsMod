using ItemAPI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace GlaurungItems.Items
{
    class SappingBullets : PassiveItem
	{
		public static void Init()
		{
			string name = "Sapping Bullets";
			string resourcePath = "GlaurungItems/Resources/banishing_bullets";
			GameObject gameObject = new GameObject(name);
			SappingBullets item = gameObject.AddComponent<SappingBullets>();
			ItemBuilder.AddSpriteToObject(name, resourcePath, gameObject);
			string shortDesc = "Not so tough now, are ya ?";
			string longDesc = "Sap the defenses of enemies";
			item.SetupItem(shortDesc, longDesc, "gl");
			item.quality = PickupObject.ItemQuality.A;
		}

		public override void Pickup(PlayerController player)
		{
			base.Pickup(player);
			player.PostProcessProjectile += this.PostProcessProjectile;
			player.PostProcessBeamTick += this.PostProcessBeamTick;
			player.OnRoomClearEvent += this.OnLeaveCombat;
		}

		public override DebrisObject Drop(PlayerController player)
		{
			player.PostProcessProjectile -= this.PostProcessProjectile;
			player.PostProcessBeamTick -= this.PostProcessBeamTick;
			player.OnRoomClearEvent -= this.OnLeaveCombat;
			return base.Drop(player);
		}

		protected override void OnDestroy()
		{
			base.Owner.PostProcessProjectile -= this.PostProcessProjectile;
			base.Owner.PostProcessBeamTick -= this.PostProcessBeamTick;
			base.Owner.OnRoomClearEvent -= this.OnLeaveCombat;
			base.OnDestroy();
		}

		private void OnLeaveCombat(PlayerController user)
		{
			sappedTargets = new List<AIActor>();
		}

		private void PostProcessProjectile(Projectile projectile, float Chance)
		{
			PlayerController owner = base.Owner;
			projectile.OnHitEnemy = (Action<Projectile, SpeculativeRigidbody, bool>)Delegate.Combine(projectile.OnHitEnemy, new Action<Projectile, SpeculativeRigidbody, bool>(this.OnProjectileHitEnemy));

		}

		private void PostProcessBeamTick(BeamController beam, SpeculativeRigidbody hitRigidBody, float tickrate)
		{
			float procChance = 1f; //This is your proc-chance, 
			AIActor aiactor = hitRigidBody.aiActor;
			if (!aiactor)
			{
				return;
			}
			bool stillAlive = aiactor.healthHaver && aiactor.healthHaver.IsAlive;
			bool canTrigger = UnityEngine.Random.value < BraveMathCollege.SliceProbability(procChance, tickrate);
			if (stillAlive && canTrigger)
			{
				SapTarget(aiactor);
			}
		}

		private void OnProjectileHitEnemy(Projectile proj, SpeculativeRigidbody enemy, bool fatal)
		{
			if (enemy != null && !fatal)
			{
				AIActor aiactor = enemy.aiActor;
				SapTarget(aiactor);
				//Tools.Print(aiActor.EnemyGuid, "ffffff", true);
			}
		}

		private void SapTarget(AIActor target)
        {
            if (!sappedTargets.Contains(target))
            {
				if (target.behaviorSpeculator != null && target.behaviorSpeculator.ImmuneToStun)
                {
					target.behaviorSpeculator.ImmuneToStun = false;
					target.Update();
				}
				target.SetResistance(EffectResistanceType.Fire, 0f);
				target.SetResistance(EffectResistanceType.Poison, 0f);
				target.SetResistance(EffectResistanceType.Freeze, 0f);
				target.SetResistance(EffectResistanceType.Charm, 0f);
				target.SetResistance(EffectResistanceType.None, 0f);
				target.ImmuneToAllEffects = false;
                if (target.IsFlying)
                {
					target.SetIsFlying(false, "Sapping Bullets");
					target.Update();
					Tools.Print("ouech", "ffffff", true);
                    if (target.IsFlying)
                    {
						target.gameObject.AddComponent<AffectedByTheGroundHandler>();
					}
				}
				sappedTargets.Add(target);
			}
        }


		private static List<AIActor> sappedTargets = new List<AIActor>();
	}

	public class AffectedByTheGroundHandler : MonoBehaviour
	{
		public AffectedByTheGroundHandler()
		{

		}

		private void Awake()
		{
			this.m_aiActor = base.GetComponent<AIActor>();
		}

		private void Update()
		{
			if(it == 0 || it%90 == 0)
            {
				if(m_aiActor && m_aiActor.healthHaver && !m_aiActor.healthHaver.IsBoss 
					&& m_aiActor.healthHaver.IsAlive && m_aiActor.IsOverPit)
                {
					m_aiActor.ForceFall();
                }
				if(m_aiActor && m_aiActor.ParentRoom != null && m_aiActor.ParentRoom.RoomGoops.Count > 0)
                {
					List<DeadlyDeadlyGoopManager> goopManagers = m_aiActor.ParentRoom.RoomGoops;
					//int i = 0;
					foreach(DeadlyDeadlyGoopManager goopManager in goopManagers)
                    {
						//Tools.Print(i, "ffffff", true);
						//i++;
						if (goopManager.IsPositionInGoop(m_aiActor.transform.position))
                        {
							//Tools.Print("apply", "ffffff", true);
							
							if(goopManager.goopDefinition != null)
                            {
								GoopDefinition goop = goopManager.goopDefinition;
								if (goop.AppliesCharm && goop.CharmModifierEffect != null)
                                {
									m_aiActor.ApplyEffect(goop.CharmModifierEffect);
                                }
								if (goop.AppliesDamageOverTime && goop.HealthModifierEffect != null)
								{
									m_aiActor.ApplyEffect(goop.HealthModifierEffect);
								}
								if (goop.AppliesCheese && goop.CheeseModifierEffect != null)
								{
									m_aiActor.ApplyEffect(goop.CheeseModifierEffect);
								}
								if (goop.AppliesSpeedModifier && goop.SpeedModifierEffect != null)
								{
									m_aiActor.ApplyEffect(goop.SpeedModifierEffect);
								}
								if(goop.goopDamageTypeInteractions.Count > 0)
                                {

                                }
								//always on
								/*if (goop.fireBurnsEnemies && goop.fireEffect != null)
								{
									m_aiActor.ApplyEffect(goop.fireEffect);
								}*/

							}
							//don't seem to work on flying enemies for pools like poison goop
							//goopManager.DoGoopEffect(m_aiActor, m_aiActor.transform.PositionVector2().ToIntVector2());
                        }
						

					}
				}
            }
			it++;
		}

		private int it = 0;
		private AIActor m_aiActor;

	}
}
