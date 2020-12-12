using Gungeon;
using ItemAPI;
using UnityEngine;

namespace GlaurungItems.Items
{
    class BeamTest : AdvancedGunBehavior
    {
        public static void Add()
        {
            Gun gun = ETGMod.Databases.Items.NewGun("Beamer Prime", "beamer");
            Game.Items.Rename("outdated_gun_mods:beamer_prime", "gl:beamer_prime");
            gun.gameObject.AddComponent<BeamTest>();
            gun.SetShortDescription("WIP");
            gun.SetLongDescription("WIP");
            gun.SetupSprite(null, "jpxfrd_idle_001", 8);
            gun.SetAnimationFPS(gun.shootAnimation, 24);
            gun.SetAnimationFPS(gun.reloadAnimation, 12);
            //gun.SetAnimationFPS(gun.idleAnimation, 8);
            //20, 60, 40, 331, 121, 179, 10, 208, 107, 333, 196, 87, 100, 474, 595, 610, 
            gun.AddProjectileModuleFrom(PickupObjectDatabase.GetById(60) as Gun, true, false);

            gun.DefaultModule.shootStyle = ProjectileModule.ShootStyle.Beam;
            gun.DefaultModule.ammoCost = 3;//dis work
            gun.DefaultModule.angleVariance = 10f;//dis doesn't seem ta work
            gun.DefaultModule.sequenceStyle = ProjectileModule.ProjectileSequenceStyle.Random;
            gun.reloadTime = 1.5f;
            gun.gunClass = GunClass.BEAM;
            
            gun.DefaultModule.cooldownTime = 0.2f;//dunno if it's useful, don't think so 
            gun.DefaultModule.numberOfShotsInClip = 400;
            gun.SetBaseMaxAmmo(400);

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
            projectile.FireApplyChance = 0;
            projectile.AppliesFire = false;
            projectile.baseData.speed *= 2.5f;
            projectile.baseData.range *= 0.75f;

            projectile.PenetratesInternalWalls = true;//doesn't seem to work


            projectile.AdditionalScaleMultiplier = 10f;//doesn't work on beam width apparently here
            projectile.AdjustPlayerProjectileTint(Color.cyan, 10, 0f); //doesn't change anything here

            //doesn't work here
            BounceProjModifier bounceMod = projectile.gameObject.GetOrAddComponent<BounceProjModifier>();
            bounceMod.numberOfBounces = 4;
            PierceProjModifier pierceMod = projectile.gameObject.GetOrAddComponent<PierceProjModifier>();
            pierceMod.penetratesBreakables = true;
            pierceMod.penetration = 5;


            gun.quality = PickupObject.ItemQuality.EXCLUDED;

            //projectile.SetProjectileSpriteRight("build_projectile", 5, 5);

            ETGMod.Databases.Items.Add(gun, null, "ANY");

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
                if (!basicBeamController.IsReflectedBeam)
                {
                    basicBeamController.reflections = 2; //reflection = bounce and it works 
                    //create lag when hitting a broken lamp thingy on walls though for some reasons
                }
                basicBeamController.ProjectileScale = 1.1f;//it works !!!
                basicBeamController.PenetratesCover = true; //works to pass through tables

                basicBeamController.homingRadius = 9999f;//work
                basicBeamController.homingAngularVelocity = 9999f;//work
                basicBeamController.projectile.PenetratesInternalWalls = true;//don't work
            }
        }
    }
}

