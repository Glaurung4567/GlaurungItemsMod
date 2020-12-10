using Gungeon;
using ItemAPI;
using System;
using System.Timers;
using UnityEngine;
using Object = UnityEngine.Object;

namespace GlaurungItems.Items
{
    class BeamTest : AdvancedGunBehavior
    {
        public static void Add()
        {
            Gun gun = ETGMod.Databases.Items.NewGun("Beam Test", "jpxfrd");
            Game.Items.Rename("outdated_gun_mods:beam_test", "gl:beam_test");
            gun.gameObject.AddComponent<BeamTest>();
            gun.SetShortDescription("WIP");
            gun.SetLongDescription("WIP");
            gun.SetupSprite(null, "jpxfrd_idle_001", 8);
            gun.SetAnimationFPS(gun.shootAnimation, 24);
            gun.SetAnimationFPS(gun.reloadAnimation, 12);
            //gun.SetAnimationFPS(gun.idleAnimation, 8);
            //gun.AddProjectileModuleFrom("klobb", true, false);
            gun.AddProjectileModuleFrom(PickupObjectDatabase.GetById(60) as Gun, true, false);


            gun.DefaultModule.ammoCost = 1;
            gun.DefaultModule.angleVariance = 0f;
            gun.DefaultModule.shootStyle = ProjectileModule.ShootStyle.Beam;
            gun.DefaultModule.sequenceStyle = ProjectileModule.ProjectileSequenceStyle.Random;
            gun.reloadTime = 1.5f;
            
            gun.DefaultModule.cooldownTime = 0.2f;
            gun.DefaultModule.numberOfShotsInClip = 30;
            gun.SetBaseMaxAmmo(400);

            Projectile projectile = UnityEngine.Object.Instantiate<Projectile>(gun.DefaultModule.projectiles[0]);
            projectile.gameObject.SetActive(false);
            FakePrefab.MarkAsFakePrefab(projectile.gameObject);
            UnityEngine.Object.DontDestroyOnLoad(projectile);
            gun.DefaultModule.projectiles[0] = projectile;

            projectile.baseData.damage *= 6f;
            projectile.baseData.speed *= 2.8f;
            projectile.baseData.force *= 1f;
            projectile.baseData.range *= 10f;
            projectile.FireApplyChance = 0;
            projectile.AppliesFire = false;
            projectile.AdditionalScaleMultiplier = 10f;//doesn't work on beam apparently
            projectile.AdjustPlayerProjectileTint(Color.cyan, 10, 0f); //doesn't change anything


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
            beam.AdjustPlayerBeamTint(Color.blue, 1);
        }
    }
}

