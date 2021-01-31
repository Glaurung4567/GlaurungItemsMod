using EnemyAPI;
using System.Collections.Generic;
using UnityEngine;
using ItemAPI;
using Gungeon;

namespace GlaurungItems.Items
{
    internal class Transistor : AdvancedGunBehavior
    {
        public static void Add()
        {
            Gun gun = ETGMod.Databases.Items.NewGun("Transistor", "transistor");
            Game.Items.Rename("outdated_gun_mods:transistor", "gl:transistor");
            gun.gameObject.AddComponent<Transistor>();
            gun.SetShortDescription("USB Swordgun Thingy");
            gun.SetLongDescription("");
            gun.SetupSprite(null, "jpxfrd_idle_001", 8);
            gun.SetAnimationFPS(gun.shootAnimation, 24);
            gun.SetAnimationFPS(gun.reloadAnimation, 12);
            //gun.SetAnimationFPS(gun.idleAnimation, 8);
            gun.AddProjectileModuleFrom((PickupObjectDatabase.GetById(223) as Gun), true, false);

            gun.DefaultModule.ammoCost = 1;
            gun.DefaultModule.angleVariance = 0f;
            gun.DefaultModule.shootStyle = ProjectileModule.ShootStyle.Charged;
            gun.DefaultModule.sequenceStyle = ProjectileModule.ProjectileSequenceStyle.Random;
            gun.reloadTime = 0f;
            gun.DefaultModule.cooldownTime = 0.1f;
            gun.DefaultModule.numberOfShotsInClip = Transistor.baseMaxAmmo;
            gun.SetBaseMaxAmmo(Transistor.baseMaxAmmo);
            gun.InfiniteAmmo = true;
            gun.gunClass = GunClass.CHARGE;
            gun.muzzleFlashEffects = (PickupObjectDatabase.GetById(81) as Gun).muzzleFlashEffects;

            gun.quality = PickupObject.ItemQuality.EXCLUDED;

            Projectile projectile = UnityEngine.Object.Instantiate<Projectile>((PickupObjectDatabase.GetById(97) as Gun).DefaultModule.projectiles[0]);
            projectile.gameObject.SetActive(false);
            FakePrefab.MarkAsFakePrefab(projectile.gameObject);
            UnityEngine.Object.DontDestroyOnLoad(projectile);
            gun.DefaultModule.projectiles[0] = projectile;
            projectile.baseData.damage *= 4f;
            projectile.baseData.speed *= 1.5f;
            projectile.baseData.force *= 0f;
            projectile.baseData.range *= 0.25f;
            projectile.transform.parent = gun.barrelOffset;

            Projectile projectile2 = UnityEngine.Object.Instantiate<Projectile>((PickupObjectDatabase.GetById(390) as Gun).DefaultModule.projectiles[0]);
            projectile2.gameObject.SetActive(false);
            FakePrefab.MarkAsFakePrefab(projectile2.gameObject);
            UnityEngine.Object.DontDestroyOnLoad(projectile2);
            projectile2.baseData.damage *= 4f;
            projectile2.baseData.speed *= 1f;
            projectile2.baseData.force *= 0f;
            projectile2.baseData.range *= 1.25f;
            projectile2.transform.parent = gun.barrelOffset;

            Projectile projectile3 = UnityEngine.Object.Instantiate<Projectile>((PickupObjectDatabase.GetById(541) as Gun).DefaultModule.projectiles[0]);
            projectile3.gameObject.SetActive(false);
            FakePrefab.MarkAsFakePrefab(projectile3.gameObject);
            UnityEngine.Object.DontDestroyOnLoad(projectile3);
            projectile3.baseData.damage *= 1f;
            projectile3.baseData.speed *= 1f;
            projectile3.baseData.force *= 0.25f;
            projectile3.baseData.range *= 1.25f;
            projectile3.transform.parent = gun.barrelOffset;

            ProjectileModule.ChargeProjectile chargeProj1 = new ProjectileModule.ChargeProjectile()
            {
                Projectile = projectile,
                ChargeTime = .0f,
                AmmoCost = 1,
            };
            ProjectileModule.ChargeProjectile chargeProj2 = new ProjectileModule.ChargeProjectile()
            {
                Projectile = projectile2,
                ChargeTime = 1f,
                AmmoCost = 2,
            };
            ProjectileModule.ChargeProjectile chargeProj3 = new ProjectileModule.ChargeProjectile()
            {
                Projectile = projectile3,
                ChargeTime = 2f,
                AmmoCost = 3,
            };
            gun.DefaultModule.chargeProjectiles = new List<ProjectileModule.ChargeProjectile>
            {
                chargeProj1,
                chargeProj2,
                chargeProj3
            };

            //projectile.SetProjectileSpriteRight("build_projectile", 5, 5);

            ETGMod.Databases.Items.Add(gun, null, "ANY");
        }

        // boilerplate stuff
        //This block of code allows us to change the reload sounds.
        public override void OnPostFired(PlayerController player, Gun gun)
        {
            //This determines what sound you want to play when you fire a gun.
            //Sounds names are based on the Gungeon sound dump, which can be found at EnterTheGungeon/Etg_Data/StreamingAssets/Audio/GeneratedSoundBanks/Windows/sfx.txt
            gun.PreventNormalFireAudio = true;
            AkSoundEngine.PostEvent("Play_WPN_beretta_shot_01", gameObject);
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
            if (gun.IsReloading && this.HasReloaded)
            {
                HasReloaded = false;
                AkSoundEngine.PostEvent("Stop_WPN_All", base.gameObject);
                base.OnReloadPressed(player, gun, bSOMETHING);
                AkSoundEngine.PostEvent("Play_WPN_SAA_reload_01", base.gameObject);
            }
        }


        private bool HasReloaded;
        private static int baseMaxAmmo = 1000;

    }
}


