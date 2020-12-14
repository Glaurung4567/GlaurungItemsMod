using EnemyAPI;
using Gungeon;
using ItemAPI;
using System;

namespace GlaurungItems.Items
{
    class GunjuringEncyclopedia : AdvancedGunBehavior
    {
        public static void Add()
        {
            Gun gun = ETGMod.Databases.Items.NewGun("Gunjuring Encyclopedia", "book");
            Game.Items.Rename("outdated_gun_mods:gunjuring_encyclopedia", "gl:gunjuring_encyclopedia");
            gun.gameObject.AddComponent<GunjuringEncyclopedia>();
            gun.SetShortDescription("WIP");
            gun.SetLongDescription("WIP");
            gun.SetupSprite(null, "jpxfrd_idle_001", 8);
            gun.SetAnimationFPS(gun.shootAnimation, 12);
            gun.SetAnimationFPS(gun.reloadAnimation, 12);
            //gun.SetAnimationFPS(gun.idleAnimation, 8);
            gun.AddProjectileModuleFrom("klobb", true, false);

            gun.DefaultModule.ammoCost = 1;
            gun.DefaultModule.angleVariance = 0f;
            gun.DefaultModule.shootStyle = ProjectileModule.ShootStyle.SemiAutomatic;
            gun.DefaultModule.sequenceStyle = ProjectileModule.ProjectileSequenceStyle.Random;
            gun.gunHandedness = GunHandedness.HiddenOneHanded;
            gun.reloadTime = 2.2f;
            gun.DefaultModule.cooldownTime = 2f;
            gun.DefaultModule.numberOfShotsInClip = 3;
            gun.SetBaseMaxAmmo(142);
            gun.muzzleFlashEffects.type = VFXPoolType.None;

            gun.quality = PickupObject.ItemQuality.A;

            Projectile projectile = UnityEngine.Object.Instantiate<Projectile>(gun.DefaultModule.projectiles[0]);
            projectile.gameObject.SetActive(false);
            FakePrefab.MarkAsFakePrefab(projectile.gameObject);
            UnityEngine.Object.DontDestroyOnLoad(projectile);
            gun.DefaultModule.projectiles[0] = projectile;

            projectile.baseData.damage = 0f;
            projectile.baseData.speed *= 0.001f;
            projectile.baseData.force = 0f;
            projectile.baseData.range *= 0.000f;
            //projectile.SetProjectileSpriteRight("build_projectile", 5, 5);

            ETGMod.Databases.Items.Add(gun, null, "ANY");
        }

        //This block of code allows us to change the reload sounds.
        public override void OnPostFired(PlayerController player, Gun gun)
        {
            gun.PreventNormalFireAudio = true;
            //AkSoundEngine.PostEvent("Play_ENV_time_shatter_01", GameManager.Instance.gameObject);
            //AkSoundEngine.PostEvent("Play_ENM_wizard_book_01", gameObject);
            AkSoundEngine.PostEvent("Play_OBJ_book_drop_01", gameObject);
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
                AkSoundEngine.PostEvent("Play_UI_page_turn_01", base.gameObject);
                if ((this.Owner as PlayerController).HasPassiveItem(500))
                {

                }
            }
        }

        public override void PostProcessProjectile(Projectile projectile)
        {
            BulletScriptSource source = this.gun.gameObject.GetOrAddComponent<BulletScriptSource>();

            //string enemyGuid = EnemyGuidDatabase.Entries["fungun"];
            string enemyGuid = EnemyGuidDatabase.Entries["chancebulon"];
            UnityEngine.GameObject obj = new UnityEngine.GameObject();
            Toolbox.CopyAIBulletBank(obj, EnemyDatabase.GetOrLoadByGuid(enemyGuid).bulletBank);
            AIBulletBank bulletBank = obj.GetComponent<AIBulletBank>();//to prevent our gun from affecting the bulletbank of the enemy
            bulletBank.OnProjectileCreated = (Action<Projectile>)Delegate.Combine(bulletBank.OnProjectileCreated, new Action<Projectile>(this.OnProjCreated));
            bulletBank.CollidesWithEnemies = true;
            source.BulletManager = bulletBank;

            //var bulletScriptSelected = new CustomBulletScriptSelector(typeof(MushroomGuySmallWaft1));
            var bulletScriptSelected = new CustomBulletScriptSelector(typeof(BulletScriptGunChancebulonDice1));
            GunjuringEncyclopedia.playerGunCurrentAngle = this.gun.CurrentAngle;
            source.BulletScript = bulletScriptSelected;

            source.Initialize();//to fire the script once
        }

        //from CompanionManager
        protected void OnProjCreated(Projectile projectile)
        {
            if (projectile)
            {
                projectile.collidesWithPlayer = false;
                projectile.collidesWithEnemies = true;
                if (this.gun.CurrentOwner)
                {
                    projectile.Owner = this.gun.CurrentOwner;
                    if(this.gun.CurrentOwner is PlayerController)
                    {
                        PlayerController player = this.gun.CurrentOwner as PlayerController;
                        player.DoPostProcessProjectile(projectile);
                    }
                }
            }
        }

        public static float playerGunCurrentAngle = 0f;
        private bool HasReloaded;
    }

}
