using EnemyAPI;
using System.Collections.Generic;
using UnityEngine;
using ItemAPI;
using Gungeon;
using System.Collections;
using System;

namespace GlaurungItems.Items
{
    internal class HandCannon : AdvancedGunBehavior
    {
        public static void Add()
        {
            Gun gun = ETGMod.Databases.Items.NewGun("Panda's Hand Cannon", "handcannon");
            Game.Items.Rename("outdated_gun_mods:panda's_hand_cannon", "gl:panda's_hand_cannon");
            gun.gameObject.AddComponent<HandCannon>();
            gun.SetShortDescription("Bang Bang");
            gun.SetLongDescription("This regular-looking foam sport hand is in fact one of the strongest weapon in the universe, as it uses the power of the user's imagination to kill enemies.");
            gun.SetupSprite(null, "linc_idle_001", 8);
            gun.SetAnimationFPS(gun.shootAnimation, 24);
            gun.SetAnimationFPS(gun.reloadAnimation, 12);
            //gun.SetAnimationFPS(gun.idleAnimation, 8);
            gun.AddProjectileModuleFrom("klobb", true, false);

            gun.DefaultModule.ammoCost = 1;
            gun.DefaultModule.angleVariance = 1f;
            gun.DefaultModule.shootStyle = ProjectileModule.ShootStyle.Burst;
            gun.DefaultModule.sequenceStyle = ProjectileModule.ProjectileSequenceStyle.Random;
            gun.reloadTime = 2f;
            gun.DefaultModule.cooldownTime = cooldownPew;
            gun.DefaultModule.burstShotCount = 2;
            gun.DefaultModule.burstCooldownTime = 0.3f;
            gun.DefaultModule.numberOfShotsInClip = 10;
            //gun.usesContinuousFireAnimation = true;
            gun.SetBaseMaxAmmo(50);
            gun.muzzleFlashEffects = (PickupObjectDatabase.GetById(81) as Gun).muzzleFlashEffects;

            gun.quality = PickupObject.ItemQuality.S;

            Projectile projectile = UnityEngine.Object.Instantiate<Projectile>(gun.DefaultModule.projectiles[0]);
            projectile.gameObject.SetActive(false);
            FakePrefab.MarkAsFakePrefab(projectile.gameObject);
            UnityEngine.Object.DontDestroyOnLoad(projectile);
            gun.DefaultModule.projectiles[0] = projectile;

            projectile.baseData.damage = 1;
            projectile.baseData.damage *= 50;
            projectile.baseData.speed *= 50f;
            projectile.baseData.force *= 10f;
            projectile.baseData.range *= 30f;
            projectile.PenetratesInternalWalls = true;
            projectile.pierceMinorBreakables = true;
            projectile.ignoreDamageCaps = true;
            projectile.AdditionalScaleMultiplier = 3;
            projectile.sprite.renderer.enabled = false;
            projectile.transform.parent = gun.barrelOffset;

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
            AkSoundEngine.PostEvent("Play_pewpew", gameObject);
            if (!startedPewSound)
            {
                startedPewSound = true;
                gun.PreventNormalFireAudio = true;
                AkSoundEngine.PostEvent("Play_pewpew", gameObject);
                GameManager.Instance.StartCoroutine(this.PewCooldown());
            }
        }

        private IEnumerator PewCooldown()
        {
            yield return new WaitForSeconds(cooldownPew);
            startedPewSound = false;
            yield break;
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
                AkSoundEngine.PostEvent("Play_pewpew_reload", base.gameObject);
            }
        }

        private bool HasReloaded;
        private bool startedPewSound;
        private static float cooldownPew = 0.6f;
    }
}


