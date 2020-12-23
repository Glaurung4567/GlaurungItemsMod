using ItemAPI;
using System;
using System.Collections;
using UnityEngine;
using Random = UnityEngine.Random;

namespace GlaurungItems.Items
{
    class MinorityBullets : PassiveItem
	{
		public static void Init()
		{
			string name = "Minority Bullets";
			string resourcePath = "GlaurungItems/Resources/acme_crate";
			GameObject gameObject = new GameObject(name);
			MinorityBullets item = gameObject.AddComponent<MinorityBullets>();
			ItemBuilder.AddSpriteToObject(name, resourcePath, gameObject);
			string shortDesc = "Minority World";
			string longDesc = "";
			item.SetupItem(shortDesc, longDesc, "gl");
			item.quality = PickupObject.ItemQuality.C;
		}

		public override void Pickup(PlayerController player)
		{
			base.Pickup(player);
			player.PostProcessProjectile += this.PostProcessProjectile;
			player.PostProcessBeamTick += this.PostProcessBeamTick;
		}

		public override DebrisObject Drop(PlayerController player)
		{
			player.PostProcessProjectile -= this.PostProcessProjectile;
			player.PostProcessBeamTick -= this.PostProcessBeamTick;
			return base.Drop(player);
		}

		protected override void OnDestroy()
		{
			base.Owner.PostProcessProjectile -= this.PostProcessProjectile;
			base.Owner.PostProcessBeamTick -= this.PostProcessBeamTick;
			base.OnDestroy();
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
				if(aiactor.gameObject.GetComponent<MinorityWorldHandler>() == null)
                {
					MinorityWorldHandler minorityWorld = aiactor.gameObject.AddComponent<MinorityWorldHandler>();
					minorityWorld.inflicter = this.Owner;

				}
			}
		}

		private void OnProjectileHitEnemy(Projectile proj, SpeculativeRigidbody enemy, bool fatal)
		{
			if (enemy != null && !fatal)
			{
				AIActor aiactor = enemy.aiActor;
				if (aiactor.gameObject.GetComponent<MinorityWorldHandler>() == null)
				{
					MinorityWorldHandler minorityWorld = aiactor.gameObject.AddComponent<MinorityWorldHandler>();
					minorityWorld.inflicter = this.Owner;
				}
			}
		}
	}

	public class MinorityWorldHandler : MonoBehaviour
	{
		public MinorityWorldHandler()
		{
			this.inflicter = GameManager.Instance.PrimaryPlayer;
		}

		private void Awake()
		{
			this.m_aiActor = base.GetComponent<AIActor>();
			if (m_aiActor.bulletBank != null)
			{
				AIBulletBank bulletBank = m_aiActor.bulletBank;
				bulletBank.OnProjectileCreated = (Action<Projectile>)Delegate.Combine(bulletBank.OnProjectileCreated, new Action<Projectile>(this.OnPostProcessProjectile));
			}
			if (m_aiActor.aiShooter != null)
			{
				AIShooter aiShooter = m_aiActor.aiShooter;
				aiShooter.PostProcessProjectile = (Action<Projectile>)Delegate.Combine(aiShooter.PostProcessProjectile, new Action<Projectile>(this.OnPostProcessProjectile));
			}
		}

        private void OnPostProcessProjectile(Projectile proj)
        {
			if(Random.value <= 1f)
            {
				GameManager.Instance.StartCoroutine(this.ChangeProjectileTarget(proj));
			}
        }

        private IEnumerator ChangeProjectileTarget(Projectile proj)
        {
			yield return new WaitForSeconds(0.2f);
			proj.RemoveBulletScriptControl();
			proj.collidesWithEnemies = true;
			if (proj.Owner && proj.Owner.specRigidbody)//from PassiveReflectItem
			{
				proj.specRigidbody.DeregisterSpecificCollisionException(proj.Owner.specRigidbody);
			}
			proj.Owner = inflicter;
			proj.OwnerName = inflicter.name;
			proj.SetNewShooter(inflicter.specRigidbody);
			proj.allowSelfShooting = true;
			proj.UpdateCollisionMask();
			//proj.Direction = -proj.Direction;
			proj.ManualControl = false;
			proj.SendInDirection(-proj.Direction, true);
			proj.Speed *= 2;
			proj.UpdateSpeed();
            yield break;
        }

        private void Update()
		{
		}

		private AIActor m_aiActor;
		public PlayerController inflicter;
	}
}
