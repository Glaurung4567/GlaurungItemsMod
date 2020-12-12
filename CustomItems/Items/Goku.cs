using Gungeon;
using ItemAPI;
using UnityEngine;

namespace GlaurungItems.Items
{
    class Goku : AdvancedGunBehavior
    {
        public static void Add()
        {
            Gun gun = ETGMod.Databases.Items.NewGun("Goku", "goku");
            Game.Items.Rename("outdated_gun_mods:goku", "gl:goku");
            gun.gameObject.AddComponent<Goku>();
            gun.SetShortDescription("WIP");
            gun.SetLongDescription("WIP");
            gun.SetupSprite(null, "jpxfrd_idle_001", 8);
            gun.SetAnimationFPS(gun.shootAnimation, 24);
            gun.SetAnimationFPS(gun.reloadAnimation, 12);
            //gun.SetAnimationFPS(gun.idleAnimation, 8);
            //20, 60, 40, 331, 121, 179, 10, 208, 107, 333, 196, 87, 100, 474, 595, 610, 
            gun.AddProjectileModuleFrom(PickupObjectDatabase.GetById(20) as Gun, true, false);

            gun.DefaultModule.shootStyle = ProjectileModule.ShootStyle.Beam;
            gun.DefaultModule.ammoCost = 3;//dis work
            gun.DefaultModule.sequenceStyle = ProjectileModule.ProjectileSequenceStyle.Random;
            gun.reloadTime = 1.5f;
            gun.gunClass = GunClass.BEAM;

            gun.DefaultModule.numberOfShotsInClip = 20;
            gun.SetBaseMaxAmmo(100);

            //changing the projectile to a projectile from a non beam weapon create a null error and break the beam
            //Projectile projectile = UnityEngine.Object.Instantiate<Projectile>((PickupObjectDatabase.GetById(31) as Gun).DefaultModule.projectiles[0]);

            Projectile projectile = UnityEngine.Object.Instantiate<Projectile>(gun.DefaultModule.projectiles[0]);
            projectile.gameObject.SetActive(false);
            FakePrefab.MarkAsFakePrefab(projectile.gameObject);
            UnityEngine.Object.DontDestroyOnLoad(projectile);
            gun.DefaultModule.projectiles[0] = projectile;

            //those work
            projectile.baseData.damage *= 1f;
            projectile.baseData.force *= 1f;
            projectile.baseData.speed *= 2.5f;
            projectile.baseData.range *= 2.25f;

            gun.quality = PickupObject.ItemQuality.EXCLUDED;

            ETGMod.Databases.Items.Add(gun, null, "ANY");

        }

        protected override void OnPickup(PlayerController player)
        {
            base.OnPickup(player);
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
            beam.AdjustPlayerBeamTint(Color.cyan, 1); //works
            beam.usesChargeDelay = true;
            beam.chargeDelay = 0.5f;
            if (beam is BasicBeamController)
            {
                BasicBeamController basicBeamController = (beam as BasicBeamController);
                basicBeamController.penetration += 100; //it works 
                if (!basicBeamController.IsReflectedBeam)
                {
                    basicBeamController.reflections = 0; 
                }
                basicBeamController.ProjectileScale = 4f;
                basicBeamController.PenetratesCover = true;
            }
        }
    }
}

