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
            gun.DefaultModule.burstShotCount = 3;
            gun.DefaultModule.burstCooldownTime = 0.02f;
            gun.DefaultModule.cooldownTime = 0.2f;
            gun.DefaultModule.numberOfShotsInClip = 6;
            gun.DefaultModule.preventFiringDuringCharge = true;
            gun.SetBaseMaxAmmo(500);

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

            Projectile projectile2 = UnityEngine.Object.Instantiate<Projectile>((PickupObjectDatabase.GetById(748) as Gun).DefaultModule.projectiles[0]);
            projectile2.gameObject.SetActive(false);
            FakePrefab.MarkAsFakePrefab(projectile2.gameObject);
            UnityEngine.Object.DontDestroyOnLoad(projectile2);

            projectile2.baseData.damage *= 1;
            ProjectileModule.ChargeProjectile chargeProj = new ProjectileModule.ChargeProjectile
            {
                Projectile = projectile2,
                ChargeTime = 1.5f,
                AmmoCost = 3,
            };
            gun.DefaultModule.chargeProjectiles = new List<ProjectileModule.ChargeProjectile> { chargeProj };

            ETGMod.Databases.Items.Add(gun, null, "ANY");
        }

        public override void OnReload(PlayerController player, Gun gun)
        {
            if (this.gun.DefaultModule.shootStyle == ProjectileModule.ShootStyle.SemiAutomatic)
            {
                this.gun.DefaultModule.shootStyle = ProjectileModule.ShootStyle.Automatic;
                this.gun.DefaultModule.cooldownTime = 0.05f;
                this.gun.DefaultModule.numberOfShotsInClip = 40;
                if (player.carriedConsumables != null)
                {
                    player.carriedConsumables.ForceUpdateUI();
                }
            }

            else if (this.gun.DefaultModule.shootStyle == ProjectileModule.ShootStyle.Automatic)
            {
                this.gun.DefaultModule.shootStyle = ProjectileModule.ShootStyle.Burst;
                this.gun.DefaultModule.cooldownTime = 0.2f;
                this.gun.DefaultModule.numberOfShotsInClip = 24;
                if (player.carriedConsumables != null)
                {
                    player.carriedConsumables.ForceUpdateUI();
                }
            }

            else if (this.gun.DefaultModule.shootStyle == ProjectileModule.ShootStyle.Burst)
            {
                this.gun.DefaultModule.shootStyle = ProjectileModule.ShootStyle.Charged;
                this.gun.DefaultModule.cooldownTime = 1f;
                this.gun.DefaultModule.numberOfShotsInClip = 9;
                this.gun.Update();
                if (player.carriedConsumables != null)
                {
                    player.carriedConsumables.ForceUpdateUI();
                }
            }

            else if (this.gun.DefaultModule.shootStyle == ProjectileModule.ShootStyle.Charged)
            {
                this.gun.DefaultModule.shootStyle = ProjectileModule.ShootStyle.SemiAutomatic;
                gun.DefaultModule.cooldownTime = 0.2f;
                this.gun.DefaultModule.ammoCost = 1;
                this.gun.DefaultModule.numberOfShotsInClip = 6;
                if (player.carriedConsumables != null)
                {
                    player.carriedConsumables.ForceUpdateUI();
                }
            }

            base.OnReload(player, gun);
        }

        public override void OnPostFired(PlayerController player, Gun gun)
        {
            gun.PreventNormalFireAudio = true;
            AkSoundEngine.PostEvent("Play_WPN_metalbullet_impact_01", gameObject);
        }
        //This block of code allows us to change the reload sounds.
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

        private bool HasReloaded;

    }
}
