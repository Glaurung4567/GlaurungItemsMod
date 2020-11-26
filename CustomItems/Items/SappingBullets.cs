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
			string shortDesc = "Not so tough now, are ya ? Are ya ?";
			string longDesc = "";
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
			float procChance = 0.3f; //This is your proc-chance, 
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
				Tools.Print(target.GetResistanceForEffectType(EffectResistanceType.Poison), "ffffff", true);
				Tools.Print(target.GetResistanceForEffectType(EffectResistanceType.Fire), "ffffff", true);
				Tools.Print(target.GetResistanceForEffectType(EffectResistanceType.Freeze), "ffffff", true);
				Tools.Print(target.GetResistanceForEffectType(EffectResistanceType.Charm), "ffffff", true);
				Tools.Print(target.GetResistanceForEffectType(EffectResistanceType.None), "ffffff", true);
				Tools.Print(target.ImmuneToAllEffects, "ffffff", true);
				Tools.Print(target.behaviorSpeculator.ImmuneToStun, "ffffff", true);
				Tools.Print(target.IsFlying, "ffffff", true);
				Tools.Print(target.FallingProhibited, "ffffff", true);
				/*----------------------------------------------------------*/

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
					//target.FallingProhibited = false;
					target.specRigidbody.Reinitialize();
					target.specRigidbody.RecheckTriggers = true;
					Tools.Print("ouech", "ffffff", true);
					target.gameObject.AddComponent<AffectedByTheGroundHandler>();
				}

				/*----------------------------------------------------------*/
				Tools.Print(target.GetResistanceForEffectType(EffectResistanceType.Poison), "ffffff", true);
				Tools.Print(target.GetResistanceForEffectType(EffectResistanceType.Fire), "ffffff", true);
				Tools.Print(target.GetResistanceForEffectType(EffectResistanceType.Freeze), "ffffff", true);
				Tools.Print(target.GetResistanceForEffectType(EffectResistanceType.Charm), "ffffff", true);
				Tools.Print(target.GetResistanceForEffectType(EffectResistanceType.None), "ffffff", true);
				Tools.Print(target.ImmuneToAllEffects, "ffffff", true);
				Tools.Print(target.behaviorSpeculator.ImmuneToStun, "ffffff", true);
				Tools.Print(target.IsFlying, "ffffff", true);
				Tools.Print(target.FallingProhibited, "ffffff", true);

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
			if(it == 0)
            {
				Tools.Print(m_aiActor, "ffffff", true);
				if(m_aiActor.ParentRoom != null)
                {
					Tools.Print(m_aiActor.ParentRoom.GetNearestCellToPosition(m_aiActor.transform.position), "ffffff", true);
					Tools.Print(m_aiActor.ParentRoom.RoomGoops.Count, "ffffff", true);
                }
				it++;
            }
		}

		private int it = 0;
		private AIActor m_aiActor;

	}
}
