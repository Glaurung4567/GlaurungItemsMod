using Gungeon;
using ItemAPI;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FakePrefab = ItemAPI.FakePrefab;
using Object = UnityEngine.Object;

namespace GlaurungItems.Items
{
	internal class Mashed : AdvancedGunBehavior
	{
		// Token: 0x0600028C RID: 652 RVA: 0x000162F8 File Offset: 0x000144F8
		public static void Add()
		{
			Gun gun = ETGMod.Databases.Items.NewGun("Mashed", "mashed");
			Game.Items.Rename("outdated_gun_mods:mashed", "gl:mashed");
			gun.gameObject.AddComponent<Mashed>();
			GunExt.SetShortDescription(gun, "Shoot Style: Yes");
			GunExt.SetLongDescription(gun, "WIP");
			GunExt.SetupSprite(gun, null, "jpxfrd_idle_001", 8);
			GunExt.SetAnimationFPS(gun, gun.shootAnimation, 16);
			GunExt.SetAnimationFPS(gun, gun.chargeAnimation, 3);

			gun.reloadTime = 1f;
			gun.SetBaseMaxAmmo(maxAmmo);
			gun.quality = PickupObject.ItemQuality.B;
			gun.gunClass = GunClass.BEAM;
			gun.muzzleFlashEffects = null;

			//charged
			GunExt.AddProjectileModuleFrom(gun, PickupObjectDatabase.GetById(292) as Gun, true, false);
			gun.DefaultModule.shootStyle = ProjectileModule.ShootStyle.Charged;
			gun.DefaultModule.ammoCost = 5;
			gun.DefaultModule.cooldownTime = 0.2f;
			gun.DefaultModule.numberOfShotsInClip = maxAmmo;
			Projectile projectile3 = UnityEngine.Object.Instantiate<Projectile>(gun.DefaultModule.projectiles[0]);
			projectile3.gameObject.SetActive(false);
			FakePrefab.MarkAsFakePrefab(projectile3.gameObject);
			UnityEngine.Object.DontDestroyOnLoad(projectile3);

			projectile3.baseData.damage *= 1;
			projectile3.baseData.speed *= 1;

			ProjectileModule.ChargeProjectile chargeProj = new ProjectileModule.ChargeProjectile
			{
				Projectile = projectile3,
				ChargeTime = 3.5f,
				AmmoCost = 5,
			};
			gun.DefaultModule.chargeProjectiles = new List<ProjectileModule.ChargeProjectile> { chargeProj };


			//auto
			GunExt.AddProjectileModuleFrom(gun, PickupObjectDatabase.GetById(124) as Gun, true, false);
			gun.Volley.projectiles[1].shootStyle = ProjectileModule.ShootStyle.Automatic;
			gun.Volley.projectiles[1].ammoCost = 1;
			gun.Volley.projectiles[1].angleVariance = 5;
			gun.Volley.projectiles[1].cooldownTime = 0.12f;
			gun.Volley.projectiles[1].numberOfShotsInClip = maxAmmo;
			Projectile projectile1 = UnityEngine.Object.Instantiate<Projectile>(gun.Volley.projectiles[1].projectiles[0]);
			projectile1.gameObject.SetActive(false);
			FakePrefab.MarkAsFakePrefab(projectile1.gameObject);
			UnityEngine.Object.DontDestroyOnLoad(projectile1);

			projectile1.baseData.damage *= 1.5f;
			projectile1.baseData.speed *= 1.2f;
			projectile1.baseData.range *= 0.75f;
			gun.Volley.projectiles[1].projectiles[0] = projectile1;
			
			//semi auto
			GunExt.AddProjectileModuleFrom(gun, PickupObjectDatabase.GetById(357) as Gun, true, false);
			gun.Volley.projectiles[2].shootStyle = ProjectileModule.ShootStyle.SemiAutomatic;
			gun.Volley.projectiles[2].ammoCost = 2;
			gun.Volley.projectiles[2].angleVariance = 2f;
			gun.Volley.projectiles[2].cooldownTime = 0.4f;
			gun.Volley.projectiles[2].numberOfShotsInClip = maxAmmo;
			Projectile projectile2 = UnityEngine.Object.Instantiate<Projectile>(gun.Volley.projectiles[2].projectiles[0]);
			projectile2.gameObject.SetActive(false);
			FakePrefab.MarkAsFakePrefab(projectile2.gameObject);
			UnityEngine.Object.DontDestroyOnLoad(projectile2);

			projectile2.baseData.damage *= 3f;
			projectile2.baseData.speed *= 2;
			if(projectile2.gameObject.GetComponent<HomingModifier>() != null)
            {
				Destroy(projectile2.gameObject.GetComponent<HomingModifier>());
			}
			if(projectile2.gameObject.GetComponent<DelayedExplosiveBuff>() != null)
            {
				DelayedExplosiveBuff delayedExplosiveBuff = projectile2.gameObject.GetComponent<DelayedExplosiveBuff>();
				delayedExplosiveBuff.delayBeforeBurst *= 2.5f;
				ExplosionData explosion = delayedExplosiveBuff.explosionData.CopyExplosionData();
				explosion.breakSecretWalls = false;
				explosion.damage = 3f;
				delayedExplosiveBuff.explosionData = explosion;
			}
			gun.Volley.projectiles[2].projectiles[0] = projectile2;

			//beam
			GunExt.AddProjectileModuleFrom(gun, PickupObjectDatabase.GetById(87) as Gun, true, false);
			gun.Volley.projectiles[3].shootStyle = ProjectileModule.ShootStyle.Beam;
			gun.Volley.projectiles[3].ammoCost = 1;
			gun.Volley.projectiles[3].angleVariance = 0;
			gun.Volley.projectiles[3].sequenceStyle = ProjectileModule.ProjectileSequenceStyle.Random;
			gun.Volley.projectiles[3].numberOfShotsInClip = maxAmmo;

			Projectile projectile = UnityEngine.Object.Instantiate<Projectile>(gun.Volley.projectiles[3].projectiles[0]);
			projectile.gameObject.SetActive(false);
			FakePrefab.MarkAsFakePrefab(projectile.gameObject);
			UnityEngine.Object.DontDestroyOnLoad(projectile);
			gun.Volley.projectiles[3].projectiles[0] = projectile;
			projectile.baseData.damage *= 0.5f;
			projectile.baseData.force *= 0.15f;
			projectile.baseData.speed *= 1f;
			projectile.AppliesPoison = false;
			projectile.PoisonApplyChance = 0;
			BasicBeamController beam = projectile.GetComponentInChildren<BasicBeamController>();
			if (!beam.IsReflectedBeam)
			{
				beam.reflections = 0;
			}
			beam.penetration = 10;
			beam.ProjectileScale = 0.5f;
			beam.PenetratesCover = true;
			beam.projectile.baseData.range = 3f;
			beam.AdjustPlayerBeamTint(Color.white, 1);
			//gun.Volley.projectiles[3].positionOffset = new Vector3(0.0f, -0.75f, 0.0f);


			/*projectile.gameObject.AddComponent<MeshRenderer>();
			projectile.gameObject.AddComponent<MeshFilter>();
			tk2dSpriteCollectionData itemCollection = ETGMod.Databases.Items.ProjectileCollection;//PickupObjectDatabase.GetByEncounterName("singularity").sprite.Collection;
			int spriteID = SpriteBuilder.AddSpriteToCollection("GlaurungItems/Resources/billiard_cue", itemCollection);
			tk2dTiledSprite tiledSprite = projectile.gameObject.AddComponent<tk2dTiledSprite>();
			tiledSprite.SetSprite(itemCollection, spriteID);
			tiledSprite.spriteAnimator = projectile.gameObject.AddComponent<tk2dSpriteAnimator>();
			Destroy(projectile.GetComponentInChildren<tk2dSprite>());*/
			//BasicBeamController beam = Toolbox.GenerateBeamPrefab(projectile, "GlaurungItems/Resources/billiard_cue");




			ETGMod.Databases.Items.Add(gun, null, "ANY");
		}

		public override void PostProcessProjectile(Projectile projectile)
		{
			base.PostProcessProjectile(projectile);
		}


		protected override void OnPickup(PlayerController player)
		{
			base.OnPickup(player);
			//player.GunChanged += this.OnGunChanged;
			//player.PostProcessBeam += this.PostProcessBeam;
			//player.GunChanged += this.OnGunChanged;
		}

		protected override void OnPostDrop(PlayerController player)
		{
			//player.PostProcessBeam -= this.PostProcessBeam;
			//player.GunChanged -= this.OnGunChanged;
			base.OnPostDrop(player);
		}

		private void OnGunChanged(Gun oldGun, Gun newGun, bool arg3)
		{

			if (this.gun && this.gun.CurrentOwner)
			{
				PlayerController player = this.gun.CurrentOwner as PlayerController;
				if (newGun == this.gun)
				{
					player.PostProcessBeam += this.PostProcessBeam;
				}
				else
				{
					player.PostProcessBeam -= this.PostProcessBeam;
				}
			}
		}

		private void PostProcessBeam(BeamController beam)
		{
			beam.AdjustPlayerBeamTint(Color.white, 1);
			if (beam is BasicBeamController)
			{
				BasicBeamController basicBeamController = (beam as BasicBeamController);
				if (!basicBeamController.IsReflectedBeam)
				{
					basicBeamController.reflections = 0;
				}
				basicBeamController.penetration = 10;
				basicBeamController.ProjectileScale = 0.5f;
				basicBeamController.PenetratesCover = true;
			}
		}

		//This block of code allows us to change the reload sounds.
		public override void OnPostFired(PlayerController player, Gun gun)
		{
			gun.PreventNormalFireAudio = true;
			//Play_WPN_radiationlaser_shot_01
			if (!fireSoundCooldown)
			{
				fireSoundCooldown = true;
				gun.PreventNormalFireAudio = true;
				AkSoundEngine.PostEvent("Play_WPN_beretta_shot_01", gameObject);
				GameManager.Instance.StartCoroutine(this.CooldownFireSound());
			}
		}

        private IEnumerator CooldownFireSound()
        {
			yield return new WaitForSeconds(0.08f);
			fireSoundCooldown = false;
			yield break;
        }

        public override void OnFinishAttack(PlayerController player, Gun gun)
		{
			base.OnFinishAttack(player, gun);
		}

		protected override void Update()
		{
			base.Update();

			if (gun.CurrentOwner)
			{
				if (!gun.PreventNormalFireAudio)
				{
					this.gun.PreventNormalFireAudio = true;
				}
				if (!gun.IsReloading && !HasReloaded)
				{
					this.HasReloaded = true;
				}
			}
		}

		public override void OnReloadPressed(PlayerController player, Gun gun, bool bSOMETHING)
		{
			if (gun.IsReloading && this.HasReloaded)
			{
				HasReloaded = false;
				AkSoundEngine.PostEvent("Stop_WPN_All", base.gameObject);
				base.OnReloadPressed(player, gun, bSOMETHING);
				AkSoundEngine.PostEvent("Play_WPN_SAA_reload_01", base.gameObject);
			}
		}

		private bool fireSoundCooldown;
		private bool HasReloaded;
		private static int maxAmmo = 1000;
	}
}
