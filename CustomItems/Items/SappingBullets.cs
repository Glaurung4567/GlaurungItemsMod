using ItemAPI;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using EnemyAPI;
using UnityEngine;

namespace GlaurungItems.Items
{
    class SappingBullets : PassiveItem
	{
		public static void Init()
		{
			string name = "Sapping Bullets";
			string resourcePath = "GlaurungItems/Resources/sapping_bullets.png";
			GameObject gameObject = new GameObject(name);
			SappingBullets item = gameObject.AddComponent<SappingBullets>();
			ItemBuilder.AddSpriteToObject(name, resourcePath, gameObject);
			string shortDesc = "Not so tough now, are ya ?";
			string longDesc = "Sap most of the defenses of enemies when a bullet hit. \n \n" +
				"Created by a powerful wizard specialized in elemental magics who became fed up with all the enemies who resisted his various attacks " +
				"(he had a bad memory so he didn't remember which enemy was weak to which type of magic...).";
			item.SetupItem(shortDesc, longDesc, "gl");
			item.quality = PickupObject.ItemQuality.C;
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
				target.PlayEffectOnActor((GameObject)ResourceCache.Acquire("Global VFX/VFX_Tarnisher_Effect"), new Vector3(0f, 0.5f, 0f), true, false, false);
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

	/*---------------------------------------------------------------------------------------------*/

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
			if(it == 0 || it%30 == 0)
            {
				if(m_aiActor && m_aiActor.healthHaver && !m_aiActor.healthHaver.IsBoss 
					&& m_aiActor.healthHaver.IsAlive && m_aiActor.IsOverPit)
                {
					m_aiActor.ForceFall();
                }

				if(m_aiActor && m_aiActor.healthHaver && m_aiActor.healthHaver.IsAlive && m_aiActor.ParentRoom != null && m_aiActor.ParentRoom.RoomGoops.Count > 0)
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
                                if (goop.CanBeElectrified)
                                {
									string enemyGuid = EnemyGuidDatabase.Entries["bullet_kin"];
									AIActor orLoadByGuid = EnemyDatabase.GetOrLoadByGuid(EnemyGuidDatabase.Entries["bullet_kin"]);

									Vector2 positionVector = m_aiActor.sprite.WorldBottomCenter;
									AIActor aiactor = AIActor.Spawn(orLoadByGuid.aiActor, positionVector, GameManager.Instance.Dungeon.data.GetAbsoluteRoomFromPosition(positionVector.ToIntVector2()), true, AIActor.AwakenAnimationType.Default, true);


									// to prevent the aiActor from moving
									aiactor.behaviorSpeculator.MovementBehaviors = EnemyDatabase.GetOrLoadByGuid("b08ec82bef6940328c7ecd9ffc6bd16c").behaviorSpeculator.MovementBehaviors;
									aiactor.behaviorSpeculator.TargetBehaviors = EnemyDatabase.GetOrLoadByGuid("b08ec82bef6940328c7ecd9ffc6bd16c").behaviorSpeculator.TargetBehaviors;
									aiactor.behaviorSpeculator.OtherBehaviors = EnemyDatabase.GetOrLoadByGuid("b08ec82bef6940328c7ecd9ffc6bd16c").behaviorSpeculator.OtherBehaviors;
									aiactor.behaviorSpeculator.AttackBehaviors = EnemyDatabase.GetOrLoadByGuid("b08ec82bef6940328c7ecd9ffc6bd16c").behaviorSpeculator.AttackBehaviors;
									//EnemyAPI.EnemyAPITools.DebugInformation(aiactor.behaviorSpeculator); //EnemyGuidDatabase.Entries["bullet_kin"]

									aiactor.sprite.renderer.enabled = false; // to make the companion invisible
									aiactor.aiShooter.ToggleGunAndHandRenderers(false, "Sapping Bullet Electrified Goop");
									aiactor.procedurallyOutlined = false;
									aiactor.CorpseObject = null;
									aiactor.behaviorSpeculator.ImmuneToStun = true;
									aiactor.ToggleShadowVisiblity(false);
									aiactor.HasShadow = false;

									aiactor.CanTargetEnemies = false;
									aiactor.CanTargetPlayers = false;
									aiactor.CompanionOwner = GameManager.Instance.PrimaryPlayer;
									aiactor.HitByEnemyBullets = false;
									PhysicsEngine.Instance.RegisterOverlappingGhostCollisionExceptions(aiactor.specRigidbody, null, false);

									aiactor.IsHarmlessEnemy = true;
									aiactor.IgnoreForRoomClear = true;
									aiactor.PreventAutoKillOnBossDeath = true;

									aiactor.knockbackDoer.SetImmobile(true, "Chainer"); // from the TetherBehavior to prevent the companion from being pushed by explosions
									aiactor.PreventFallingInPitsEver = true;
									aiactor.ImmuneToAllEffects = true;

									aiactor.SetResistance(EffectResistanceType.Fire, 1f);
									aiactor.SetResistance(EffectResistanceType.Poison, 1f);
									aiactor.SetResistance(EffectResistanceType.Freeze, 1f);
									aiactor.SetResistance(EffectResistanceType.Charm, 1f);
									aiactor.healthHaver.SetHealthMaximum(1000f);
									aiactor.healthHaver.ForceSetCurrentHealth(1000f);
									aiactor.healthHaver.forcePreventVictoryMusic = true;

									//aiactor.HandleReinforcementFallIntoRoom(0f); //don't use this if you want your mob to be invisible
									aiactor.gameObject.AddComponent<CompanionController>();
									CompanionController component = aiactor.gameObject.GetComponent<CompanionController>();
									component.CanInterceptBullets = false;
									component.Initialize(GameManager.Instance.PrimaryPlayer);

									base.StartCoroutine(CheckIfPoolIsElectrified(aiactor));
								}
								/*if(goop.goopDamageTypeInteractions.Count > 0)
                                {
									List<GoopDefinition.GoopDamageTypeInteraction> tys = goop.goopDamageTypeInteractions;
									foreach(GoopDefinition.GoopDamageTypeInteraction ty in tys)
                                    {
                                        if (ty.electrifiesGoop)
                                        {
											Tools.Print(ty.electrifiesGoop, "ffffff", true);
                                        }
                                    }
								}*/

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

        private IEnumerator CheckIfPoolIsElectrified(AIActor aiactor)
        {
			yield return new WaitForSeconds(0.05f);
			if (aiactor && aiactor.healthHaver && aiactor.healthHaver.IsAlive && aiactor.healthHaver.GetCurrentHealth() < aiactor.healthHaver.GetMaxHealth())
			{
				//Tools.Print("bzzt", "ffffff", true);
				base.StartCoroutine(ElectrifyPeskyFlyingLittleShit());
			}
			aiactor.EraseFromExistence(true);
			yield break;
        }

        private IEnumerator ElectrifyPeskyFlyingLittleShit()
        {
			yield return null;
			float t = 0.5f;
            while (t > 0)
            {
				Electrify();
				t -= BraveTime.DeltaTime;
			}
			yield break;
		}

		private void Electrify()
        {
			if (m_aiActor && m_aiActor.healthHaver && m_aiActor.healthHaver.IsAlive)
			{
				m_aiActor.healthHaver.ApplyDamage(1f, Vector2.zero, "Sapping Bullet Electrified Goop", CoreDamageTypes.Electric, DamageCategory.Normal,
					true, null, true);
			}
		}

		private int it = 0;
		private AIActor m_aiActor;

	}
}
