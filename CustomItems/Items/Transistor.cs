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
            Gun gun = ETGMod.Databases.Items.NewGun("USB Gun", "transistorgun");
            Game.Items.Rename("outdated_gun_mods:usb_gun", "gl:usb_gun");
            gun.gameObject.AddComponent<Transistor>();
            gun.SetShortDescription("");
            gun.SetLongDescription("");
            gun.SetupSprite(null, "transistorgun_idle_001", 8);
            gun.SetAnimationFPS(gun.shootAnimation, 24);
            gun.SetAnimationFPS(gun.reloadAnimation, 12);
            //gun.SetAnimationFPS(gun.idleAnimation, 8);
            gun.AddProjectileModuleFrom((PickupObjectDatabase.GetById(97) as Gun), true, false);

            gun.DefaultModule.ammoCost = 1;
            gun.DefaultModule.angleVariance = 0f;
            gun.DefaultModule.shootStyle = ProjectileModule.ShootStyle.Charged;
            gun.DefaultModule.sequenceStyle = ProjectileModule.ProjectileSequenceStyle.Random;
            gun.reloadTime = 0f;
            gun.DefaultModule.cooldownTime = 0.2f;
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
            projectile.baseData.damage *= 1.3f;
            projectile.baseData.speed *= 1.5f;
            projectile.baseData.force *= 0f;
            projectile.baseData.range = 5.15f;
            projectile.transform.parent = gun.barrelOffset;

            Projectile projectile2 = UnityEngine.Object.Instantiate<Projectile>((PickupObjectDatabase.GetById(156) as Gun).DefaultModule.projectiles[0]);
            projectile2.gameObject.SetActive(false);
            FakePrefab.MarkAsFakePrefab(projectile2.gameObject);
            UnityEngine.Object.DontDestroyOnLoad(projectile2);
            projectile2.baseData.damage *= 1.5f;
            projectile2.baseData.speed *= 1f;
            projectile2.baseData.force *= 0f;
            projectile2.baseData.range *= 1f;
            projectile2.transform.parent = gun.barrelOffset;

            Projectile projectile3 = UnityEngine.Object.Instantiate<Projectile>((PickupObjectDatabase.GetById(541) as Gun).DefaultModule.chargeProjectiles[0].Projectile);
            projectile3.gameObject.SetActive(false);
            FakePrefab.MarkAsFakePrefab(projectile3.gameObject);
            UnityEngine.Object.DontDestroyOnLoad(projectile3);
            projectile3.baseData.damage *= 1f;
            projectile3.baseData.speed *= 1f;
            projectile3.baseData.force *= 0.5f;
            projectile3.baseData.range *= 1.25f;
            projectile3.CurseSparks = true;
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
                ChargeTime = .5f,
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

        public override void PostProcessProjectile(Projectile projectile)
        {
            if (projectile.name == this.gun.DefaultModule.chargeProjectiles[2].Projectile.name + "(Clone)")
            {
                projectile.CurseSparks = true;
            }
            base.PostProcessProjectile(projectile);
        }

        protected override void OnPickup(PlayerController player)
        {
            base.OnPickup(player);
            this.GiveElectricDamageImmunity(player); //to add contact damage immunity when the gun is held
            player.GunChanged += this.OnGunChanged;
        }

        protected override void OnPostDrop(PlayerController player)
        {
            player.GunChanged -= this.OnGunChanged;
            this.RemoveElectricDamageImmunity(player);
            base.OnPostDrop(player);
        }

        private void OnGunChanged(Gun oldGun, Gun newGun, bool arg3)
        {
            if (this.gun && this.gun.CurrentOwner)
            {
                PlayerController player = this.gun.CurrentOwner as PlayerController;
                if (newGun == this.gun)
                {
                    this.GiveElectricDamageImmunity(player);
                }
                else
                {
                    this.RemoveElectricDamageImmunity(player);
                }
            }
        }

        private void GiveElectricDamageImmunity(PlayerController player)
        {
            this.m_electricityImmunity = new DamageTypeModifier();
            this.m_electricityImmunity.damageMultiplier = 0f;
            this.m_electricityImmunity.damageType = CoreDamageTypes.Electric;
            player.healthHaver.damageTypeModifiers.Add(this.m_electricityImmunity);
        }

        private void RemoveElectricDamageImmunity(PlayerController player)
        {
            player.healthHaver.damageTypeModifiers.Remove(this.m_electricityImmunity);
        }

        public override void OnFinishAttack(PlayerController player, Gun gun)
        {
            gun.PreventNormalFireAudio = true;
            AkSoundEngine.PostEvent("Play_WPN_sflaser_shot_01", gameObject);
        }

        // boilerplate stuff
        //This block of code allows us to change the reload sounds.
        public override void OnPostFired(PlayerController player, Gun gun)
        {
            //This determines what sound you want to play when you fire a gun.
            //Sounds names are based on the Gungeon sound dump, which can be found at EnterTheGungeon/Etg_Data/StreamingAssets/Audio/GeneratedSoundBanks/Windows/sfx.txt
            gun.PreventNormalFireAudio = true;
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
        private DamageTypeModifier m_electricityImmunity;

    }
}


