using Gungeon;
using ItemAPI;
using MonoMod.RuntimeDetour;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Timers;
using UnityEngine;

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
            gun.SetLongDescription("Created by a gunsmith who had too many concepts he wanted to try but not enough resources " +
                "to create all of them before going bankrupt, " +
                "so he just crammed as much as he could in his final gun.");
            gun.SetupSprite(null, "jpxfrd_idle_001", 8);
            gun.SetAnimationFPS(gun.shootAnimation, 24);
            gun.SetAnimationFPS(gun.reloadAnimation, 12);
            gun.SetAnimationFPS(gun.chargeAnimation, 12);
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

            gun.quality = PickupObject.ItemQuality.A;

            Projectile projectile = UnityEngine.Object.Instantiate<Projectile>((PickupObjectDatabase.GetById(31) as Gun).DefaultModule.projectiles[0]);
            projectile.gameObject.SetActive(false);
            FakePrefab.MarkAsFakePrefab(projectile.gameObject);
            UnityEngine.Object.DontDestroyOnLoad(projectile);
            gun.DefaultModule.projectiles[0] = projectile;

            projectile.baseData.damage *= 6f;
            projectile.baseData.speed *= 2.8f;
            projectile.baseData.force *= 1f;
            projectile.baseData.range *= 3f;
            projectile.transform.parent = gun.barrelOffset;

            BounceProjModifier bounceMod = projectile.gameObject.GetOrAddComponent<BounceProjModifier>();
            bounceMod.numberOfBounces = 4;
            PierceProjModifier pierceMod = projectile.gameObject.GetOrAddComponent<PierceProjModifier>();
            pierceMod.penetratesBreakables = true;
            pierceMod.penetration = 5;

            Projectile projectile2 = UnityEngine.Object.Instantiate<Projectile>((PickupObjectDatabase.GetById(365) as Gun).DefaultModule.projectiles[0]);
            projectile2.gameObject.SetActive(false);
            FakePrefab.MarkAsFakePrefab(projectile2.gameObject);
            UnityEngine.Object.DontDestroyOnLoad(projectile2);

            projectile2.baseData.damage *= 1;
            projectile2.baseData.speed *= 2;
            ProjectileModule.ChargeProjectile chargeProj = new ProjectileModule.ChargeProjectile
            {
                Projectile = projectile2,
                ChargeTime = 0.5f,
                AmmoCost = 3,
            };
            gun.DefaultModule.chargeProjectiles = new List<ProjectileModule.ChargeProjectile> { chargeProj };

            ETGMod.Databases.Items.Add(gun, null, "ANY");
        }

        private void SetSavedShootingStyle(PlayerController player)
        {
            if (shootStyle == ProjectileModule.ShootStyle.Automatic)
            {
                SetAuto(player);
            }
            if (shootStyle == ProjectileModule.ShootStyle.Burst)
            {
                SetBurst(player);
            }
            if (shootStyle == ProjectileModule.ShootStyle.Charged)
            {
                SetCharged(player);
            }
        }

        public override void OnReload(PlayerController player, Gun gun)
        {
            if (this.gun.DefaultModule.shootStyle == ProjectileModule.ShootStyle.SemiAutomatic)
            {
                SetAuto(player);
            }

            else if (this.gun.DefaultModule.shootStyle == ProjectileModule.ShootStyle.Automatic)
            {
                SetBurst(player);
            }

            else if (this.gun.DefaultModule.shootStyle == ProjectileModule.ShootStyle.Burst)
            {
                SetCharged(player);
            }

            else if (this.gun.DefaultModule.shootStyle == ProjectileModule.ShootStyle.Charged)
            {
                this.gun.DefaultModule.shootStyle = ProjectileModule.ShootStyle.SemiAutomatic;
                shootStyle = this.gun.DefaultModule.shootStyle;
                this.gun.DefaultModule.cooldownTime = 0.2f;
                this.gun.DefaultModule.ammoCost = 1;
                this.gun.DefaultModule.numberOfShotsInClip = 6;

                Projectile proj = this.gun.DefaultModule.projectiles[0];
                proj.baseData.damage = 1f;
                proj.baseData.damage *= 8f;
                
                BounceProjModifier bounceMod = proj.gameObject.GetOrAddComponent<BounceProjModifier>();
                bounceMod.numberOfBounces = 4;
                PierceProjModifier pierceMod = proj.gameObject.GetOrAddComponent<PierceProjModifier>();
                pierceMod.penetratesBreakables = true;
                pierceMod.penetration = 5;

                if (player.carriedConsumables != null) { player.carriedConsumables.ForceUpdateUI(); }
                RemoveChainLightningModifier();
                this.gun.Update();
            }
            base.OnReload(player, gun);
        }

        private void SetAuto(PlayerController player)
        {
            this.gun.DefaultModule.shootStyle = ProjectileModule.ShootStyle.Automatic;
            shootStyle = this.gun.DefaultModule.shootStyle;
            this.gun.DefaultModule.cooldownTime = 0.12f;
            this.gun.DefaultModule.angleVariance = 15f;
            this.gun.DefaultModule.numberOfShotsInClip = 30;

            Projectile proj = this.gun.DefaultModule.projectiles[0];
            proj.baseData.damage = 1f;
            proj.baseData.damage *= 5f;

            chainLightning.maximumLinkDistance = 10f;
            if (this.gun.DefaultModule.projectiles[0].gameObject.GetComponent<ChainLightningModifier>() == null && !(this.Owner as PlayerController).HasPassiveItem(298))
            {
                this.gun.DefaultModule.projectiles[0].gameObject.AddComponent(chainLightning);
            }


            if (player.carriedConsumables != null) { player.carriedConsumables.ForceUpdateUI(); }
            RemoveBouncingModifier();
            RemovePiercingModifier();
            this.gun.Update();

        }

        private void SetBurst(PlayerController player)
        {
            this.gun.DefaultModule.shootStyle = ProjectileModule.ShootStyle.Burst;
            shootStyle = this.gun.DefaultModule.shootStyle;
            this.gun.DefaultModule.cooldownTime = 0.2f;
            this.gun.DefaultModule.angleVariance = 1f;
            this.gun.DefaultModule.numberOfShotsInClip = 24;

            Projectile proj = this.gun.DefaultModule.projectiles[0];
            proj.baseData.damage = 1f;
            proj.baseData.damage *= 4f;

            if (player.carriedConsumables != null) { player.carriedConsumables.ForceUpdateUI(); }
            RemoveChainLightningModifier();
            RemoveBouncingModifier();
            RemovePiercingModifier();
        }

        private void SetCharged(PlayerController player)
        {
            this.gun.DefaultModule.shootStyle = ProjectileModule.ShootStyle.Charged;
            shootStyle = this.gun.DefaultModule.shootStyle;
            this.gun.DefaultModule.cooldownTime = 1f;
            this.gun.DefaultModule.numberOfShotsInClip = 3;
            if (player.carriedConsumables != null) { player.carriedConsumables.ForceUpdateUI(); }
            this.gun.Update();
        }

        private void RemoveChainLightningModifier()
        {
            if (this.gun.DefaultModule.projectiles[0].gameObject.GetComponent<ChainLightningModifier>() != null && !(this.Owner as PlayerController).HasPassiveItem(298))
            {
                Destroy(this.gun.DefaultModule.projectiles[0].gameObject.GetComponent<ChainLightningModifier>());
            }
        }

        private void RemoveBouncingModifier()
        {
            if (this.gun.DefaultModule.projectiles[0].gameObject.GetComponent<BounceProjModifier>() != null)
            {
                Destroy(this.gun.DefaultModule.projectiles[0].gameObject.GetComponent<BounceProjModifier>());
            }
        }
        
        private void RemovePiercingModifier()
        {
            if (this.gun.DefaultModule.projectiles[0].gameObject.GetComponent<PierceProjModifier>() != null)
            {
                Destroy(this.gun.DefaultModule.projectiles[0].gameObject.GetComponent<PierceProjModifier>());
            }
        }

        //private void StatsChanged

        //This block of code allows us to change the reload sounds.
        public override void OnPostFired(PlayerController player, Gun gun)
        {
            gun.PreventNormalFireAudio = true;
            AkSoundEngine.PostEvent("Play_WPN_metalbullet_impact_01", gameObject);
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
                if(gun.CurrentOwner is PlayerController)
                {
                    PlayerController player = gun.CurrentOwner as PlayerController;
                    if(this.gun.DefaultModule.shootStyle != this.shootStyle && !this.gun.IsReloading)
                    {
                        SetSavedShootingStyle(player);
                    }
                }
            }
        }

        public override void OnReloadPressed(PlayerController player, Gun gun, bool bSOMETHING)
        {
            if ((gun.ClipCapacity == gun.ClipShotsRemaining) || (gun.CurrentAmmo == gun.ClipShotsRemaining))
            {
                OnReload(player, gun);
            }
            if (gun.IsReloading && this.HasReloaded)
            {
                HasReloaded = false;
                AkSoundEngine.PostEvent("Stop_WPN_All", base.gameObject);
                base.OnReloadPressed(player, gun, bSOMETHING);
                AkSoundEngine.PostEvent("Play_WPN_SAA_reload_01", base.gameObject);
            }
        }

        private bool HasReloaded;
        private static ChainLightningModifier chainLightning = (PickupObjectDatabase.GetById(330) as Gun).DefaultModule.projectiles[0].GetComponent<ChainLightningModifier>();

        [SerializeField]
        private ProjectileModule.ShootStyle shootStyle = ProjectileModule.ShootStyle.SemiAutomatic;
    }
}
