using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using ItemAPI;
using Random = UnityEngine.Random;
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
            gun.SetShortDescription("WIP");
            gun.SetLongDescription("WIP");
            gun.SetupSprite(null, "jpxfrd_idle_001", 8);
            gun.SetAnimationFPS(gun.shootAnimation, 24);
            gun.SetAnimationFPS(gun.reloadAnimation, 12);
            //gun.SetAnimationFPS(gun.idleAnimation, 8);
            gun.AddProjectileModuleFrom("klobb", true, false);

            gun.DefaultModule.ammoCost = 1;
            gun.DefaultModule.angleVariance = PeacemakerCarbine.baseAngleVar;
            gun.DefaultModule.shootStyle = ProjectileModule.ShootStyle.Automatic;
            gun.DefaultModule.sequenceStyle = ProjectileModule.ProjectileSequenceStyle.Random;
            gun.reloadTime = 1.5f;
            gun.DefaultModule.cooldownTime = PeacemakerCarbine.baseCooldownTime;
            gun.DefaultModule.numberOfShotsInClip = 30;
            gun.SetBaseMaxAmmo(200);
            gun.gunClass = GunClass.FULLAUTO;
            gun.muzzleFlashEffects = (PickupObjectDatabase.GetById(81) as Gun).muzzleFlashEffects;

            gun.quality = PickupObject.ItemQuality.B;

            Projectile projectile = UnityEngine.Object.Instantiate<Projectile>(gun.DefaultModule.projectiles[0]);
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
                SwitchFire();
            }
        }

        private void SwitchFire()
        {
            if (!altFireOn)
            {
                this.gun.DefaultModule.cooldownTime = 0.0001f;
                this.gun.DefaultModule.angleVariance = 0f;
                //faster proj + fire
                altFireOn = true;
            }
            else
            {
                this.gun.DefaultModule.cooldownTime = PeacemakerCarbine.baseCooldownTime;
                this.gun.DefaultModule.angleVariance = PeacemakerCarbine.baseAngleVar;
                altFireOn = false;
            }
        }

        private bool HasReloaded;
        private static float baseCooldownTime = 0.2f;
        private static float baseAngleVar = 3f;
        [SerializeField]
        private bool altFireOn;
    }
}
