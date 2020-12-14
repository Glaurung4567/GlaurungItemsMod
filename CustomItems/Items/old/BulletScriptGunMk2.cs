using EnemyAPI;
using Gungeon;
using ItemAPI;
using System;
using System.Timers;

namespace GlaurungItems.Items
{
    class BulletScriptGunSource : AdvancedGunBehavior
    {
        public static void Add()
        {
            Gun gun = ETGMod.Databases.Items.NewGun("Source Gun", "source");
            Game.Items.Rename("outdated_gun_mods:source_gun", "gl:source_gun");
            gun.gameObject.AddComponent<BulletScriptGunSource>();
            gun.SetShortDescription("WIP");
            gun.SetLongDescription("WIP");
            gun.SetupSprite(null, "jpxfrd_idle_001", 8);
            gun.SetAnimationFPS(gun.shootAnimation, 24);
            gun.SetAnimationFPS(gun.reloadAnimation, 12);
            //gun.SetAnimationFPS(gun.idleAnimation, 8);
            gun.AddProjectileModuleFrom("klobb", true, false);

            gun.DefaultModule.ammoCost = 1;
            gun.DefaultModule.angleVariance = 0f;
            gun.DefaultModule.shootStyle = ProjectileModule.ShootStyle.SemiAutomatic;
            gun.DefaultModule.sequenceStyle = ProjectileModule.ProjectileSequenceStyle.Random;
            gun.reloadTime = 1.5f;
            gun.DefaultModule.cooldownTime = 0.2f;
            gun.DefaultModule.numberOfShotsInClip = 30;
            gun.SetBaseMaxAmmo(400);

            gun.quality = PickupObject.ItemQuality.EXCLUDED;

            Projectile projectile = UnityEngine.Object.Instantiate<Projectile>(gun.DefaultModule.projectiles[0]);
            projectile.gameObject.SetActive(false);
            FakePrefab.MarkAsFakePrefab(projectile.gameObject);
            UnityEngine.Object.DontDestroyOnLoad(projectile);
            gun.DefaultModule.projectiles[0] = projectile;

            projectile.baseData.damage *= 4f;
            projectile.baseData.speed *= 2.8f;
            projectile.baseData.force *= 1f;
            projectile.baseData.range *= 3f;
            projectile.transform.parent = gun.barrelOffset;
            //projectile.SetProjectileSpriteRight("build_projectile", 5, 5);

            ETGMod.Databases.Items.Add(gun, null, "ANY");
        }

        public override void PostProcessProjectile(Projectile projectile)
        {
            BulletScriptSource source = this.gun.gameObject.GetOrAddComponent<BulletScriptSource>();
            string enemyGuid = EnemyGuidDatabase.Entries["fungun"];
            UnityEngine.GameObject obj = new UnityEngine.GameObject();
            BulletScriptGunSource.CopyAIBulletBank(obj, EnemyDatabase.GetOrLoadByGuid(enemyGuid).bulletBank);
            AIBulletBank bulletBank = obj.GetComponent<AIBulletBank>();
            bulletBank.OnProjectileCreated = (Action<Projectile>)Delegate.Combine(bulletBank.OnProjectileCreated, new Action<Projectile>(this.OnProjCreated));
            bulletBank.CollidesWithEnemies = true;
            source.BulletManager = bulletBank;

            var bulletScriptSelected = new CustomBulletScriptSelector(typeof(MushroomGuySmallWaft1));
            source.BulletScript = bulletScriptSelected;

            source.Initialize();
        }

        //from CompanionManager
        protected void OnProjCreated(Projectile obj)
        {
            if (obj)
            {
                obj.collidesWithPlayer = false;
                obj.collidesWithEnemies = true;
            }
        }


        public static void CopyAIBulletBank(UnityEngine.GameObject obj, AIBulletBank bank)
        {
            AIBulletBank newBank = obj.GetOrAddComponent<AIBulletBank>();
            newBank.Bullets = bank.Bullets;
            newBank.FixedPlayerPosition = bank.FixedPlayerPosition;
            newBank.OnProjectileCreated = bank.OnProjectileCreated;
            newBank.OverrideGun = bank.OverrideGun;
            newBank.rampTime = bank.rampTime;
            newBank.OnProjectileCreatedWithSource = bank.OnProjectileCreatedWithSource;
            newBank.rampBullets = bank.rampBullets;
            newBank.transforms = bank.transforms;
            newBank.useDefaultBulletIfMissing = bank.useDefaultBulletIfMissing;
            newBank.rampStartHeight = bank.rampStartHeight;
            newBank.SpecificRigidbodyException = bank.SpecificRigidbodyException;
            newBank.PlayShells = bank.PlayShells;
            newBank.PlayAudio = bank.PlayAudio;
            newBank.PlayVfx = bank.PlayVfx;
            newBank.CollidesWithEnemies = bank.CollidesWithEnemies;
            newBank.FixedPlayerRigidbodyLastPosition = bank.FixedPlayerRigidbodyLastPosition;
            newBank.ActorName = bank.ActorName;
            newBank.TimeScale = bank.TimeScale;
            newBank.SuppressPlayerVelocityAveraging = bank.SuppressPlayerVelocityAveraging;
            newBank.FixedPlayerRigidbody = bank.FixedPlayerRigidbody;
            /*newBank.spriteAnimator = bank.spriteAnimator;
            newBank.sprite = bank.sprite;
            newBank.specRigidbody = bank.specRigidbody;
            newBank.encounterTrackable = bank.encounterTrackable;
            newBank.majorBreakable = bank.majorBreakable;
            newBank.gameActor = bank.gameActor;
            newBank.aiAnimator = bank.aiAnimator;
            newBank.healthHaver = bank.healthHaver;
            newBank.aiActor = bank.aiActor;
            newBank.renderer = bank.renderer;*/
        }
    }

}
