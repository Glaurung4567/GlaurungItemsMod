using Gungeon;
using ItemAPI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GlaurungItems.Items
{
    class GravDartGun : AdvancedGunBehavior
    {
        public static void Add()
        {
            Gun gun = ETGMod.Databases.Items.NewGun("Grav Dart Launcher", "gravd");
            Game.Items.Rename("outdated_gun_mods:grav_dart_launcher", "gl:grav_dart_launcher");
            gun.gameObject.AddComponent<GravDartGun>();
            gun.SetShortDescription("");
            gun.SetLongDescription("");
            gun.SetupSprite(null, "pewhand_idle_001", 8);
            gun.SetAnimationFPS(gun.shootAnimation, 24);
            gun.SetAnimationFPS(gun.reloadAnimation, 12);
            gun.AddProjectileModuleFrom("klobb", true, false);

            gun.DefaultModule.ammoCost = 1;
            gun.DefaultModule.angleVariance = 1f;
            gun.gunHandedness = GunHandedness.TwoHanded;
            gun.DefaultModule.shootStyle = ProjectileModule.ShootStyle.SemiAutomatic;
            gun.DefaultModule.sequenceStyle = ProjectileModule.ProjectileSequenceStyle.Random;
            gun.reloadTime = 2f;
            gun.DefaultModule.cooldownTime = 0.2f;
            gun.DefaultModule.numberOfShotsInClip = 30;
            gun.SetBaseMaxAmmo(360);
            gun.muzzleFlashEffects = (PickupObjectDatabase.GetById(81) as Gun).muzzleFlashEffects;

            gun.quality = PickupObject.ItemQuality.S;

            Projectile projectile = UnityEngine.Object.Instantiate<Projectile>((PickupObjectDatabase.GetById(357) as Gun).DefaultModule.projectiles[0]);
            projectile.gameObject.SetActive(false);
            FakePrefab.MarkAsFakePrefab(projectile.gameObject);
            UnityEngine.Object.DontDestroyOnLoad(projectile);
            gun.DefaultModule.projectiles[0] = projectile;

            projectile.baseData.damage = 1;
            projectile.baseData.damage *= 0;
            projectile.baseData.speed *= 1f;
            projectile.baseData.force *= 0f;
            projectile.baseData.range *= 30f;
            projectile.transform.parent = gun.barrelOffset;

            if (projectile.gameObject.GetComponent<HomingModifier>() != null)
            {
                Destroy(projectile.gameObject.GetComponent<HomingModifier>());
            }
            if (projectile.gameObject.GetComponent<DelayedExplosiveBuff>() != null)
            {
                DelayedExplosiveBuff delayedExplosiveBuff = projectile.gameObject.GetComponent<DelayedExplosiveBuff>();
                delayedExplosiveBuff.delayBeforeBurst *= 2.5f;
                ExplosionData explosion = delayedExplosiveBuff.explosionData.CopyExplosionData();
                explosion.breakSecretWalls = false;
                explosion.damage = 0f;
                delayedExplosiveBuff.explosionData = explosion;
            }

            //projectile.SetProjectileSpriteRight("build_projectile", 5, 5);

            ETGMod.Databases.Items.Add(gun, null, "ANY");
        }
    }
}
