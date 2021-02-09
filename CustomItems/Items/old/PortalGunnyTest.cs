using Gungeon;
using ItemAPI;
using System;
using System.Timers;
using UnityEngine;

namespace GlaurungItems.Items
{
    class PortalGunnyTest : AdvancedGunBehavior
    {
        public static void Add()
        {
            Gun gun = ETGMod.Databases.Items.NewGun("Portal Gunny", "jpxfrdfghjsddd");
            Game.Items.Rename("outdated_gun_mods:portal_gunny", "gl:portal_gunny");
            gun.gameObject.AddComponent<PortalGunnyTest>();
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
            gun.DefaultModule.numberOfShotsInClip = 2;
            gun.SetBaseMaxAmmo(100);

            gun.quality = PickupObject.ItemQuality.EXCLUDED;

            Projectile projectile = UnityEngine.Object.Instantiate<Projectile>(gun.DefaultModule.projectiles[0]);
            projectile.gameObject.SetActive(false);
            FakePrefab.MarkAsFakePrefab(projectile.gameObject);
            UnityEngine.Object.DontDestroyOnLoad(projectile);
            gun.DefaultModule.projectiles[0] = projectile;

            projectile.baseData.damage *= 0f;
            projectile.baseData.speed *= 0.0f;
            projectile.baseData.force *= 0f;
            projectile.baseData.range *= 3f;
            projectile.transform.parent = gun.barrelOffset;

            PierceProjModifier pierceMod = projectile.gameObject.GetOrAddComponent<PierceProjModifier>();
            pierceMod.penetratesBreakables = true;
            pierceMod.penetration = 9999;
            //projectile.SetProjectileSpriteRight("build_projectile", 5, 5);

            ETGMod.Databases.Items.Add(gun, null, "ANY");
        }

        public override void PostProcessProjectile(Projectile projectile)
        {


            GameObject gameObject = SpawnManager.SpawnProjectile(((Gun)ETGMod.Databases.Items[31]).DefaultModule.projectiles[0].gameObject, gun.CurrentOwner.sprite.WorldCenter + new Vector2(2, 2), Quaternion.Euler(0f, 0f, 0f), true);
            projectile.ForceDestruction();
            Projectile projectileInst = gameObject.GetComponent<Projectile>();
            projectileInst.collidesWithPlayer = true;
            projectileInst.baseData.damage = 0;
            projectileInst.baseData.speed = 0;
            projectileInst.collidesWithEnemies = true;
            projectileInst.collidesWithProjectiles = true;
            projectileInst.allowSelfShooting = true;
            //PierceProjModifier pierceMod = projectileInst.gameObject.GetOrAddComponent<PierceProjModifier>();
            //pierceMod.penetratesBreakables = true;
            //pierceMod.penetration = 9999;
            //projectileInst.specRigidbody.OnCollision += coll;
            PortalGunPortalController portal = projectileInst.gameObject.GetOrAddComponent<PortalGunPortalController>();
            if (nbIt % 2 == 0) {
                firstPortal = portal;
                portal.pairedPortal = portal;
            }
            else
            {
                portal.pairedPortal = firstPortal;
                firstPortal.pairedPortal = portal;
            }

            nbIt++;
        }

        private void coll(CollisionData obj)
        {
            Tools.Print("test coll player", "ffffff", true);
        }



        // used to give boosts when the numbers of kills reach a certain number and remove them when the kill streak end
        protected override void Update()
        {
            base.Update();
        }

        private int nbIt = 0;
        private PortalGunPortalController firstPortal;
    }
}
