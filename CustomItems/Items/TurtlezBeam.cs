using Gungeon;
using ItemAPI;
using UnityEngine;

namespace GlaurungItems.Items
{
    class TurtlezBeam : AdvancedGunBehavior
    {
        public static void Add()
        {
            Gun gun = ETGMod.Databases.Items.NewGun("Turtlez Beam", "turtlez");
            Game.Items.Rename("outdated_gun_mods:turtlez_beam", "gl:turtlez_beam");
            gun.gameObject.AddComponent<TurtlezBeam>();
            gun.SetShortDescription("Kamehameha !");
            gun.SetLongDescription("Gives the user the power to fire powerful energy blast, similar to the ones used by a famous alien warrior.");
            gun.SetupSprite(null, "turtlez_idle_001", 8);
            gun.SetAnimationFPS(gun.shootAnimation, 18);
            gun.SetAnimationFPS(gun.reloadAnimation, 12);
            gun.AddProjectileModuleFrom(PickupObjectDatabase.GetById(20) as Gun, true, false);

            gun.DefaultModule.shootStyle = ProjectileModule.ShootStyle.Beam;
            gun.DefaultModule.ammoCost = 2;
            gun.DefaultModule.sequenceStyle = ProjectileModule.ProjectileSequenceStyle.Random;
            gun.reloadTime = 0.5f;
            gun.gunClass = GunClass.BEAM;

            gun.DefaultModule.numberOfShotsInClip = 5;
            gun.SetBaseMaxAmmo(150);

            Projectile projectile = UnityEngine.Object.Instantiate<Projectile>(gun.DefaultModule.projectiles[0]);
            projectile.gameObject.SetActive(false);
            FakePrefab.MarkAsFakePrefab(projectile.gameObject);
            UnityEngine.Object.DontDestroyOnLoad(projectile);
            gun.DefaultModule.projectiles[0] = projectile;

            projectile.baseData.damage *= 3f;
            projectile.baseData.force *= 2f;
            projectile.baseData.speed *= 2.5f;
            projectile.baseData.range *= 2.25f;

            gun.quality = PickupObject.ItemQuality.B;
            ETGMod.Databases.Items.Add(gun, null, "ANY");
        }

        protected override void OnPickup(PlayerController player)
        {
            base.OnPickup(player);
            player.PostProcessBeam += this.PostProcessBeam;
            player.GunChanged += this.OnGunChanged;
        }


        protected override void OnPostDrop(PlayerController player)
        {
            player.PostProcessBeam -= this.PostProcessBeam;
            player.GunChanged -= this.OnGunChanged;
            if (auraActive)
            {
                SpriteOutlineManager.RemoveOutlineFromSprite(player.sprite);
                auraActive = false;
            }
            base.OnPostDrop(player);
        }

        private void OnGunChanged(Gun oldGun, Gun newGun, bool arg3)
        {

            if (this.gun && this.gun.CurrentOwner)
            {
                PlayerController player = this.gun.CurrentOwner as PlayerController;
                if (newGun == this.gun)
                {
                    player.PostProcessBeam += this.PostProcessBeam;
                }
                else
                {
                    player.PostProcessBeam -= this.PostProcessBeam;
                }
            }
        }

        private void PostProcessBeam(BeamController beam)
        {
            beam.AdjustPlayerBeamTint(Color.cyan, 1); 
            beam.usesChargeDelay = true;
            beam.chargeDelay = 0.5f;
            if (beam is BasicBeamController)
            {
                BasicBeamController basicBeamController = (beam as BasicBeamController);
                
                basicBeamController.penetration += 100;  
                if (!basicBeamController.IsReflectedBeam)
                {
                    basicBeamController.reflections = 0; 
                }
                basicBeamController.ProjectileScale = 4f;
                basicBeamController.PenetratesCover = true;
            }
        }

        // boilerplate stuff
        public override void OnPostFired(PlayerController player, Gun gun)
        {
            //This determines what sound you want to play when you fire a gun.
            //Sounds names are based on the Gungeon sound dump, which can be found at EnterTheGungeon/Etg_Data/StreamingAssets/Audio/GeneratedSoundBanks/Windows/sfx.txt
            if (!startedBeamSound)
            {
                startedBeamSound = true;
                gun.PreventNormalFireAudio = true;
                AkSoundEngine.PostEvent("Play_WPN_moonscraperLaser_shot_01", gameObject);
            }
        }

        public override void OnFinishAttack(PlayerController player, Gun gun)
        {
            //Tools.Print(chargeFraction, "ffffff", true);
            startedBeamSound = false;
            base.OnFinishAttack(player, gun);
        }



        //This block of code allows us to change the reload sounds.
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
                if(gun.CurrentOwner is PlayerController)
                {
                    PlayerController player = gun.CurrentOwner as PlayerController;
                    if (gun.IsFiring && !auraActive)
                    {
                        auraActive = true;
                        SpriteOutlineManager.AddOutlineToSprite(player.sprite, Color.yellow);
                    }
                    else if(!gun.IsFiring && auraActive)
                    {
                        gun.PreventNormalFireAudio = true;
                        auraActive = false;
                        SpriteOutlineManager.RemoveOutlineFromSprite(player.sprite);
                    }

                    if (gun.CurrentAmmo <= 0 && !flashed)
                    {
                        Projectile projectile = ((Gun)ETGMod.Databases.Items[481]).DefaultModule.chargeProjectiles[0].Projectile;
                        GameObject gameObject = SpawnManager.SpawnProjectile(projectile.gameObject, player.CenterPosition, Quaternion.Euler(0f, 0f, 0f), true);
                        Projectile flash = gameObject.GetComponent<Projectile>();
                        flash.SetOwnerSafe(player, "Player");
                        flash.Shooter = player.specRigidbody;
                        flash.Owner = player;
                        flash.baseData.damage *= 1f;
                        flash.baseData.force *= 1f;
                        player.DoPostProcessProjectile(flash);
                        flashed = true;
                    }
                    else if(gun.CurrentAmmo > 0 && flashed)
                    {
                        flashed = false;

                    }
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
        private bool auraActive;
        private bool startedBeamSound;

        [SerializeField]
        private bool flashed;
    }
}

