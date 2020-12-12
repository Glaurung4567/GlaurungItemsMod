using ItemAPI;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Object = UnityEngine.Object;
using Random = UnityEngine.Random;

namespace GlaurungItems.Items
{
	class NuArcana : PassiveItem
	{
		public static void Init()
		{
			string name = "Nu Arcana";
			string resourcePath = "GlaurungItems/Resources/acme_crate";
			GameObject gameObject = new GameObject(name);
			NuArcana item = gameObject.AddComponent<NuArcana>();
			ItemBuilder.AddSpriteToObject(name, resourcePath, gameObject);
			string shortDesc = "All life begins and ends with Nu";
			string longDesc = "A long forgotten technique used by strange immortal spheric creatures, it allows the user to put the target just at death door without killing them, no matter how tough they are.";
			item.SetupItem(shortDesc, longDesc, "gl");
			item.quality = PickupObject.ItemQuality.B;
			NuArcana.ID = item.PickupObjectId;
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
				if (aiActor.healthHaver && aiActor.healthHaver.IsAlive && !aiActor.healthHaver.IsBoss && UnityEngine.Random.value < 0.01f)
				{
					aiActor.healthHaver.ForceSetCurrentHealth(1);
					aiActor.SetOverrideOutlineColor(Color.clear);
				}
			}
		}

		private void OnProjectileHitEnemy(Projectile proj, SpeculativeRigidbody enemy, bool fatal)
		{
			if (enemy != null)
			{
				AIActor aiActor = enemy.aiActor;
				//Tools.Print(aiActor.EnemyGuid, "ffffff", true);
				if(!fatal && aiActor.healthHaver && !aiActor.healthHaver.IsBoss && UnityEngine.Random.value < 0.01f)
                {
					aiActor.healthHaver.ForceSetCurrentHealth(1);
					aiActor.SetOverrideOutlineColor(Color.clear);
				}
			}
		}




		public static int ID;
	}
}
