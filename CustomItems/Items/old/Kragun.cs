using Gungeon;
using ItemAPI;
using UnityEngine;
using FakePrefab = ItemAPI.FakePrefab;
using Object = UnityEngine.Object;

namespace GlaurungItems.Items
{
	internal class Kragun : AdvancedGunBehavior
	{
		// Token: 0x0600028C RID: 652 RVA: 0x000162F8 File Offset: 0x000144F8
		public static void Add()
		{
			Gun gun = ETGMod.Databases.Items.NewGun("Kragun", "kragun");
			Game.Items.Rename("outdated_gun_mods:kragun", "gl:kragun");
			gun.gameObject.AddComponent<Kragun>();
			gun.gunSwitchGroup = (PickupObjectDatabase.GetById(37) as Gun).gunSwitchGroup;
			GunExt.SetShortDescription(gun, "Unleash the kragun !");
			GunExt.SetLongDescription(gun, "WIP");
			GunExt.SetupSprite(gun, null, "jpxfrd_idle_001", 8);
			GunExt.SetAnimationFPS(gun, gun.shootAnimation, 16);
			GunExt.SetAnimationFPS(gun, gun.chargeAnimation, 3);
			for (int i = 0; i < 4; i++)
			{
				GunExt.AddProjectileModuleFrom(gun, PickupObjectDatabase.GetById(60) as Gun, true, false);
			}
			foreach (ProjectileModule projectileModule in gun.Volley.projectiles)
			{
				projectileModule.ammoCost = 1;
				projectileModule.shootStyle = (ProjectileModule.ShootStyle.Beam);
				projectileModule.sequenceStyle = 0;
				projectileModule.cooldownTime = 1f;
				projectileModule.angleVariance = 20f;
				projectileModule.numberOfShotsInClip = 10;
				Projectile projectile = Object.Instantiate<Projectile>(projectileModule.projectiles[0]);
				projectileModule.projectiles[0] = projectile;
				projectile.gameObject.SetActive(false);
				FakePrefab.MarkAsFakePrefab(projectile.gameObject);
				Object.DontDestroyOnLoad(projectile);
				projectile.baseData.damage *= 1f;
				projectile.AdditionalScaleMultiplier *= 0.5f;
				projectile.baseData.range *= 0.25f;
				projectile.FireApplyChance = 0;
				projectile.AppliesFire = false;
				bool flag = projectileModule != gun.DefaultModule;
				if (flag)
				{
					//projectileModule.ammoCost = 0;
				}
				projectile.transform.parent = gun.barrelOffset;
			}
			gun.reloadTime = 1f;
			gun.SetBaseMaxAmmo(100);
			gun.quality = PickupObject.ItemQuality.C;

			ETGMod.Databases.Items.Add(gun, null, "ANY");
			gun.barrelOffset.transform.localPosition = new Vector3(1.37f, 0.37f, 0f);
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
