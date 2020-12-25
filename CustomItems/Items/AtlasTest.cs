using System;
using System.Collections.Generic;
using UnityEngine;
using ItemAPI;
using Gungeon;

namespace GlaurungItems.Items
{
    internal class AtlasTest : AdvancedGunBehavior
    {
        public static void Add()
        {
            Gun gun = ETGMod.Databases.Items.NewGun("Atlas Test", "atlastest");
            Game.Items.Rename("outdated_gun_mods:atlas_test", "gl:atlas_test");
            gun.gameObject.AddComponent<AtlasTest>();
            gun.SetShortDescription("WIP");
            gun.SetLongDescription("");
            gun.SetupSprite(null, "jpxfrd_idle_001", 8);
            gun.SetAnimationFPS(gun.shootAnimation, 24);
            gun.SetAnimationFPS(gun.reloadAnimation, 12);
            //gun.SetAnimationFPS(gun.idleAnimation, 8);
            gun.AddProjectileModuleFrom("klobb", true, false);

            gun.DefaultModule.ammoCost = 1;
            gun.DefaultModule.angleVariance = 7f;
            gun.DefaultModule.shootStyle = ProjectileModule.ShootStyle.Automatic;
            gun.DefaultModule.sequenceStyle = ProjectileModule.ProjectileSequenceStyle.Random;
            gun.reloadTime = 1.5f;
            gun.DefaultModule.cooldownTime = 0.1f;
            gun.DefaultModule.numberOfShotsInClip = 30;
            //gun.usesContinuousFireAnimation = true;
            gun.SetBaseMaxAmmo(400);
            gun.gunClass = GunClass.FULLAUTO;
            gun.DefaultModule.burstShotCount = 1;
            gun.muzzleFlashEffects = (PickupObjectDatabase.GetById(81) as Gun).muzzleFlashEffects;

            gun.quality = PickupObject.ItemQuality.B;

            Projectile projectile = UnityEngine.Object.Instantiate<Projectile>(gun.DefaultModule.projectiles[0]);
            projectile.gameObject.SetActive(false);
            FakePrefab.MarkAsFakePrefab(projectile.gameObject);
            UnityEngine.Object.DontDestroyOnLoad(projectile);
            gun.DefaultModule.projectiles[0] = projectile;

            projectile.baseData.damage *= 2f;
            projectile.baseData.speed *= 1.5f;
            projectile.baseData.force *= 1f;
            projectile.baseData.range *= 3f;
            projectile.transform.parent = gun.barrelOffset;
            //projectile.SetProjectileSpriteRight("build_projectile", 5, 5);

            ETGMod.Databases.Items.Add(gun, null, "ANY");
        }

        public override void PostProcessProjectile(Projectile projectile)
        {
            AtlasProjMod atlas = projectile.gameObject.AddComponent<AtlasProjMod>();
            atlas.targetedEnemies = targetedEnemies;
            projectile.OnHitEnemy = (Action<Projectile, SpeculativeRigidbody, bool>)Delegate.Combine(projectile.OnHitEnemy, new Action<Projectile, SpeculativeRigidbody, bool>(this.OnProjectileHitEnemy));
            base.PostProcessProjectile(projectile);
        }

        public void OnProjectileHitEnemy(Projectile proj, SpeculativeRigidbody enemy, bool fatal)
        {
            if (enemy != null)
            {
                AIActor aiactor = enemy.aiActor;
                if (!targetedEnemies.Contains(aiactor))
                {
                    targetedEnemies.Add(aiactor);
                }
            }
        }

        protected override void OnPickup(PlayerController player)
        {
            base.OnPickup(player);
            this.targetedEnemies = new List<AIActor>();
            //player.GunChanged += this.OnGunChanged;
            player.OnRoomClearEvent += this.OnLeaveCombat;
        }

        protected override void OnPostDrop(PlayerController user)
        {
            user.OnRoomClearEvent -= this.OnLeaveCombat;
            this.targetedEnemies = new List<AIActor>();
            base.OnPostDrop(user);
        }

        private void OnLeaveCombat(PlayerController user)
        {
            if (user != null)
            {
                this.targetedEnemies = new List<AIActor>();
            }
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
        private List<AIActor> targetedEnemies = new List<AIActor>();
    }


    //---------------------------------------------------------------------------------
    public class AtlasProjMod : MonoBehaviour
    {
        public AtlasProjMod()
        {
            
        }

        private void Awake()
        {
            this.m_Projectile = base.GetComponent<Projectile>();
            
        }

        private void Update()
        {
            foreach(AIActor actor in targetedEnemies)
            {
                if(Vector3.Distance(actor.transform.position, m_Projectile.transform.position) <= 3 && m_Projectile.gameObject.GetComponent<HomingModifier>() == null)
                {
                    HomingModifier homing = m_Projectile.gameObject.GetOrAddComponent<HomingModifier>();
                    homing.HomingRadius = 3;
                    homing.AngularVelocity = 10;
                }
            }
        }

        public Projectile m_Projectile;
        public List<AIActor> targetedEnemies = new List<AIActor>();
    }

}
