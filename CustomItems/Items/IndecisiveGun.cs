using Gungeon;
using ItemAPI;
using System;
using System.Collections.Generic;
using System.Timers;

namespace GlaurungItems.Items
{
    class IndecisiveGun : AdvancedGunBehavior
    {
        public static void Add()
        {
            Gun gun = ETGMod.Databases.Items.NewGun("The Indecisive", "jpxfrd");
            Game.Items.Rename("outdated_gun_mods:the_indecisive", "gl:the_indecisive");
            gun.gameObject.AddComponent<IndecisiveGun>();
            gun.SetShortDescription("4 in 1");
            gun.SetLongDescription("WIP");
            gun.SetupSprite(null, "jpxfrd_idle_001", 8);
            gun.SetAnimationFPS(gun.shootAnimation, 24);
            gun.SetAnimationFPS(gun.reloadAnimation, 12);
            //gun.SetAnimationFPS(gun.idleAnimation, 8);
            gun.AddProjectileModuleFrom("klobb", true, false);

            gun.DefaultModule.ammoCost = 1;
            gun.DefaultModule.angleVariance = 2f;
            gun.DefaultModule.shootStyle = ProjectileModule.ShootStyle.SemiAutomatic;
            gun.DefaultModule.sequenceStyle = ProjectileModule.ProjectileSequenceStyle.Random;
            gun.reloadTime = 0.5f;
            gun.DefaultModule.burstShotCount = 5;
            gun.DefaultModule.burstCooldownTime = 0.01f;
            gun.DefaultModule.cooldownTime = 0.2f;
            gun.DefaultModule.numberOfShotsInClip = 8;
            gun.DefaultModule.preventFiringDuringCharge = true;
            gun.SetBaseMaxAmmo(300);

            gun.quality = PickupObject.ItemQuality.EXCLUDED;

            Projectile projectile = UnityEngine.Object.Instantiate<Projectile>((PickupObjectDatabase.GetById(31) as Gun).DefaultModule.projectiles[0]);
            projectile.gameObject.SetActive(false);
            FakePrefab.MarkAsFakePrefab(projectile.gameObject);
            UnityEngine.Object.DontDestroyOnLoad(projectile);
            gun.DefaultModule.projectiles[0] = projectile;

            projectile.baseData.damage *= 8f;
            projectile.baseData.speed *= 2.8f;
            projectile.baseData.force *= 1f;
            projectile.baseData.range *= 3f;
            projectile.transform.parent = gun.barrelOffset;
            //projectile.SetProjectileSpriteRight("build_projectile", 5, 5);

            Projectile projectile2 = UnityEngine.Object.Instantiate<Projectile>((PickupObjectDatabase.GetById(748) as Gun).DefaultModule.projectiles[0]);
            projectile2.gameObject.SetActive(false);
            FakePrefab.MarkAsFakePrefab(projectile2.gameObject);
            UnityEngine.Object.DontDestroyOnLoad(projectile2);
            ProjectileModule.ChargeProjectile chargeProj = new ProjectileModule.ChargeProjectile
            {
                Projectile = projectile2,
                ChargeTime = 1.5f,
            };
            gun.DefaultModule.chargeProjectiles = new List<ProjectileModule.ChargeProjectile> { chargeProj };

            ETGMod.Databases.Items.Add(gun, null, "ANY");
        }

        public override void OnReload(PlayerController player, Gun gun)
        {
            Tools.Print("reload", "ffffff", true);
            if (this.gun.DefaultModule.shootStyle == ProjectileModule.ShootStyle.SemiAutomatic)
            {
                this.gun.DefaultModule.shootStyle = ProjectileModule.ShootStyle.Automatic;
                this.gun.DefaultModule.cooldownTime = 0.1f;
            }
            else if (this.gun.DefaultModule.shootStyle == ProjectileModule.ShootStyle.Automatic)
            {
                this.gun.DefaultModule.shootStyle = ProjectileModule.ShootStyle.Burst;
            }
            else if (this.gun.DefaultModule.shootStyle == ProjectileModule.ShootStyle.Burst)
            {
                this.gun.DefaultModule.shootStyle = ProjectileModule.ShootStyle.Charged;
                this.gun.DefaultModule.cooldownTime = 1f;
                this.gun.DefaultModule.numberOfShotsInClip = 9;
                this.gun.DefaultModule.ammoCost = 3;
                if (player.carriedConsumables != null)
                {
                    player.carriedConsumables.ForceUpdateUI();
                }
            }
            else if (this.gun.DefaultModule.shootStyle == ProjectileModule.ShootStyle.Charged)
            {
                this.gun.DefaultModule.shootStyle = ProjectileModule.ShootStyle.SemiAutomatic;
                gun.DefaultModule.cooldownTime = 0.2f;
            }
            Tools.Print(this.gun.DefaultModule.shootStyle, "ffffff", true);

            base.OnReload(player, gun);
        }

        protected override void Update()
        {
            base.Update();
        }

    }
}
