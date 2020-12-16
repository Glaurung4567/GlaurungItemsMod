﻿using System;
using System.Collections.Generic;
using UnityEngine;
using ItemAPI;
using Gungeon;

namespace GlaurungItems.Items
{
    internal class PeacemakerCarbine : AdvancedGunBehavior
    {
        public static void Add()
        {
            Gun gun = ETGMod.Databases.Items.NewGun("Peacemaker Carbine", "peacemakercarbine");
            Game.Items.Rename("outdated_gun_mods:peacemaker_carbine", "gl:peacemaker_carbine");
            gun.gameObject.AddComponent<PeacemakerCarbine>();
            gun.SetShortDescription("Bullets Storm");
            gun.SetLongDescription("WIP");
            gun.SetupSprite(null, "jpxfrd_idle_001", 8);
            gun.SetAnimationFPS(gun.shootAnimation, 24);
            gun.SetAnimationFPS(gun.reloadAnimation, 12);
            //gun.SetAnimationFPS(gun.idleAnimation, 8);
            gun.AddProjectileModuleFrom("klobb", true, false);

            gun.DefaultModule.ammoCost = PeacemakerCarbine.baseAmmoCost;
            gun.DefaultModule.preventFiringDuringCharge = true;
            gun.DefaultModule.triggerCooldownForAnyChargeAmount = true;
            gun.DefaultModule.angleVariance = PeacemakerCarbine.baseAngleVar;
            gun.DefaultModule.shootStyle = PeacemakerCarbine.baseShootStyle;
            gun.DefaultModule.sequenceStyle = ProjectileModule.ProjectileSequenceStyle.Random;
            gun.reloadTime = 1.5f;
            gun.DefaultModule.cooldownTime = 0.1f; 
            gun.DefaultModule.numberOfShotsInClip = PeacemakerCarbine.baseMagSize;
            gun.SetBaseMaxAmmo(200);
            gun.muzzleFlashEffects = (PickupObjectDatabase.GetById(81) as Gun).muzzleFlashEffects;

            gun.quality = PickupObject.ItemQuality.B;

            Projectile projectile = UnityEngine.Object.Instantiate<Projectile>((PickupObjectDatabase.GetById(31) as Gun).DefaultModule.projectiles[0]);
            projectile.gameObject.SetActive(false);
            FakePrefab.MarkAsFakePrefab(projectile.gameObject);
            UnityEngine.Object.DontDestroyOnLoad(projectile);
            gun.DefaultModule.projectiles[0] = projectile;

            projectile.baseData.damage *= 4f;
            projectile.baseData.speed *= 2f;
            projectile.baseData.force *= 1f;
            projectile.baseData.range *= 3f;
            projectile.transform.parent = gun.barrelOffset;
            //projectile.SetProjectileSpriteRight("build_projectile", 5, 5);

            Projectile projectile2 = UnityEngine.Object.Instantiate<Projectile>((PickupObjectDatabase.GetById(748) as Gun).DefaultModule.projectiles[0]);
            projectile2.gameObject.SetActive(false);
            FakePrefab.MarkAsFakePrefab(projectile2.gameObject);
            UnityEngine.Object.DontDestroyOnLoad(projectile2);

            projectile2.baseData.damage *= 2;
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

        public override void PostProcessProjectile(Projectile projectile)
        {
            base.PostProcessProjectile(projectile);
        }


        // boilerplate stuff
        //This block of code allows us to change the reload sounds.
        public override void OnPostFired(PlayerController player, Gun gun)
        {
            //This determines what sound you want to play when you fire a gun.
            //Sounds names are based on the Gungeon sound dump, which can be found at EnterTheGungeon/Etg_Data/StreamingAssets/Audio/GeneratedSoundBanks/Windows/sfx.txt
            gun.PreventNormalFireAudio = true;
            AkSoundEngine.PostEvent("Play_WPN_smileyrevolver_shot_01", gameObject);
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
            if ((gun.ClipCapacity == gun.ClipShotsRemaining) || (gun.CurrentAmmo == gun.ClipShotsRemaining))
            {
                SwitchFire();
            }
            if (gun.IsReloading && this.HasReloaded)
            {
                HasReloaded = false;
                AkSoundEngine.PostEvent("Stop_WPN_All", base.gameObject);
                base.OnReloadPressed(player, gun, bSOMETHING);
                AkSoundEngine.PostEvent("Play_WPN_SAA_reload_01", base.gameObject);
            }
        }

        private void SwitchFire()
        {
            if (!altFireOn)
            {
                this.gun.DefaultModule.angleVariance = 0f;
                this.gun.DefaultModule.numberOfShotsInClip = 1;
                this.gun.DefaultModule.shootStyle = ProjectileModule.ShootStyle.Charged;
                this.gun.DefaultModule.ammoCost = 20;
                //faster proj + fire
                altFireOn = true;
                if ((this.gun.CurrentOwner is PlayerController) && (this.gun.CurrentOwner as PlayerController).carriedConsumables != null) { (this.gun.CurrentOwner as PlayerController).carriedConsumables.ForceUpdateUI(); }
            }
            else
            {
                this.gun.DefaultModule.shootStyle = PeacemakerCarbine.baseShootStyle;
                this.gun.DefaultModule.angleVariance = PeacemakerCarbine.baseAngleVar;
                this.gun.DefaultModule.ammoCost = PeacemakerCarbine.baseAmmoCost;
                this.gun.DefaultModule.numberOfShotsInClip = PeacemakerCarbine.baseMagSize;
                if ((this.gun.CurrentOwner is PlayerController) && (this.gun.CurrentOwner as PlayerController).carriedConsumables != null) { (this.gun.CurrentOwner as PlayerController).carriedConsumables.ForceUpdateUI(); }

                altFireOn = false;
            }
        }

        private bool HasReloaded;

        private static int baseAmmoCost = 1;
        private static int baseMagSize = 20;
        private static float baseAngleVar = 3f;
        private static ProjectileModule.ShootStyle baseShootStyle = ProjectileModule.ShootStyle.Automatic;

        [SerializeField]
        private bool altFireOn;
    }
}
