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
            projectile.baseData.range *= 3f;
            projectile.FireApplyChance = 0;
            projectile.AppliesFire = false;
            projectile.AdjustPlayerProjectileTint(Color.cyan, 10, 0f);



            gun.quality = PickupObject.ItemQuality.EXCLUDED;

            //projectile.SetProjectileSpriteRight("build_projectile", 5, 5);

            ETGMod.Databases.Items.Add(gun, null, "ANY");

        }

        protected override void OnPickup(PlayerController player)
        {
            base.OnPickup(player);
            //player.GunChanged += this.OnGunChanged;
            Tools.Print(this.gun.ActiveBeams.Count, "ffffff", true);

            Tools.Print(this.gun.DefaultModule.projectiles.Count, "ffffff", true);
            
        }

        public override void PostProcessProjectile(Projectile projectile)
        {

        }

        public override Projectile OnPreFireProjectileModifier(Gun gun, Projectile projectile, ProjectileModule mod)
        {

            return base.OnPreFireProjectileModifier(gun, projectile, mod);
        }

        protected override void Update()
        {
            base.Update();
            if (gun.CurrentOwner)
            {

                if (this.gun.ActiveBeams != null && this.gun.ActiveBeams.Count > 0 && this.gun.ActiveBeams[0] != null && this.gun.ActiveBeams[0].beam != null)
                {
                    this.gun.ActiveBeams[0].beam.AdjustPlayerBeamTint(Color.black, 1);
                }
            }
        }

    }
}

