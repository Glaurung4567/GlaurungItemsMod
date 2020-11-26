using Gungeon;
using ItemAPI;
using System;
using System.Timers;

namespace GlaurungItems.Items
{
    class VladofGun : AdvancedGunBehavior
    {
        public static void Add()
        {
            Gun gun = ETGMod.Databases.Items.NewGun("Vladof Gun", "jpxfrd");
            Game.Items.Rename("outdated_gun_mods:vladof_gun", "gl:vladof_gun");
            gun.gameObject.AddComponent<VladofGun>();
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
            gun.DefaultModule.burstShotCount = 5;
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

        public override void OnReload(PlayerController player, Gun gun)
        {
            Tools.Print("reload", "ffffff", true);
            if (this.gun.DefaultModule.shootStyle == ProjectileModule.ShootStyle.SemiAutomatic)
            {
                this.gun.DefaultModule.shootStyle = ProjectileModule.ShootStyle.Automatic;
            }
            else if (this.gun.DefaultModule.shootStyle == ProjectileModule.ShootStyle.Automatic)
            {
                this.gun.DefaultModule.shootStyle = ProjectileModule.ShootStyle.Burst;
            }
            else if (this.gun.DefaultModule.shootStyle == ProjectileModule.ShootStyle.Burst)
            {
                this.gun.DefaultModule.shootStyle = ProjectileModule.ShootStyle.SemiAutomatic;
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
