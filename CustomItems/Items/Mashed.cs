using Gungeon;
using ItemAPI;
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
			GunExt.SetShortDescription(gun, "Unleash the Mashed !");
			GunExt.SetLongDescription(gun, "WIP");
			GunExt.SetupSprite(gun, null, "jpxfrd_idle_001", 8);
			GunExt.SetAnimationFPS(gun, gun.shootAnimation, 16);
			GunExt.SetAnimationFPS(gun, gun.chargeAnimation, 3);

			GunExt.AddProjectileModuleFrom(gun, PickupObjectDatabase.GetById(60) as Gun, true, false);
			gun.DefaultModule.shootStyle = ProjectileModule.ShootStyle.Beam;
			gun.DefaultModule.ammoCost = 2;
			gun.DefaultModule.sequenceStyle = ProjectileModule.ProjectileSequenceStyle.Random;
			gun.reloadTime = 0.5f;
			gun.gunClass = GunClass.BEAM;
			gun.DefaultModule.numberOfShotsInClip = 50;

			Projectile projectile = UnityEngine.Object.Instantiate<Projectile>(gun.DefaultModule.projectiles[0]);
			projectile.gameObject.SetActive(false);
			FakePrefab.MarkAsFakePrefab(projectile.gameObject);
			UnityEngine.Object.DontDestroyOnLoad(projectile);
			gun.DefaultModule.projectiles[0] = projectile;
			projectile.baseData.damage *= 3f;
			projectile.baseData.force *= 2f;
			projectile.baseData.speed *= 2.5f;
			projectile.baseData.range *= 2.25f;


			GunExt.AddProjectileModuleFrom(gun, PickupObjectDatabase.GetById(365) as Gun, true, false);
			gun.Volley.projectiles[1].shootStyle = ProjectileModule.ShootStyle.Automatic;
			gun.Volley.projectiles[1].ammoCost = 1;
			gun.Volley.projectiles[1].cooldownTime = 1;
			gun.Volley.projectiles[1].numberOfShotsInClip = 10;
			Projectile projectile2 = UnityEngine.Object.Instantiate<Projectile>((PickupObjectDatabase.GetById(365) as Gun).DefaultModule.projectiles[0]);
			projectile2.gameObject.SetActive(false);
			FakePrefab.MarkAsFakePrefab(projectile2.gameObject);
			UnityEngine.Object.DontDestroyOnLoad(projectile2);

			projectile2.baseData.damage *= 1;
			projectile2.baseData.speed *= 2;
			gun.Volley.projectiles[1].projectiles[0] = projectile2;

			gun.reloadTime = 1f;
			gun.SetBaseMaxAmmo(1000);
			gun.quality = PickupObject.ItemQuality.B;

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
			player.PostProcessBeam += this.PostProcessBeam;
			player.GunChanged += this.OnGunChanged;
		}

		protected override void OnPostDrop(PlayerController player)
		{
			player.PostProcessBeam -= this.PostProcessBeam;
			player.GunChanged -= this.OnGunChanged;
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
			beam.AdjustPlayerBeamTint(Color.green, 1); //works
			if (beam is BasicBeamController)
			{
				BasicBeamController basicBeamController = (beam as BasicBeamController);
				basicBeamController.penetration = 10; //it works 
				basicBeamController.ProjectileScale = 0.5f;//it works !!!
				basicBeamController.PenetratesCover = true; //works to pass through tables
			}
		}
	}
}
