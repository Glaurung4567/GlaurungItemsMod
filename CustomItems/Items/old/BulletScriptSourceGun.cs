using EnemyAPI;
using Gungeon;
using ItemAPI;
using System;
using UnityEngine;
using Random = UnityEngine.Random;

namespace GlaurungItems.Items
{
    class BulletScriptSourceGun : AdvancedGunBehavior
    {
        public static void Add()
        {
            Gun gun = ETGMod.Databases.Items.NewGun("Script Gun", "bok");
            Game.Items.Rename("outdated_gun_mods:script_gun", "gl:script_gun");
            gun.gameObject.AddComponent<BulletScriptSourceGun>();
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

            gun.quality = PickupObject.ItemQuality.EXCLUDED;

            Projectile projectile = UnityEngine.Object.Instantiate<Projectile>(gun.DefaultModule.projectiles[0]);
            projectile.gameObject.SetActive(false);
            FakePrefab.MarkAsFakePrefab(projectile.gameObject);
            UnityEngine.Object.DontDestroyOnLoad(projectile);
            gun.DefaultModule.projectiles[0] = projectile;

            projectile.baseData.damage = 0f;
            projectile.baseData.speed = 0.00f;
            projectile.baseData.force = 0f;
            projectile.baseData.range *= 1.000f;
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
			GameObject gameObject = new GameObject();
			gameObject.transform.position = this.gun.barrelOffset.position;
			BulletScriptSource source = gameObject.GetOrAddComponent<BulletScriptSource>();
			gameObject.AddComponent<BulletSourceKiller>();

			//string enemyGuid = EnemyGuidDatabase.Entries["fungun"];
			string enemyGuid = EnemyGuidDatabase.Entries["faster_tutorial_turret"];
			var bulletScriptSelected = new CustomBulletScriptSelector(typeof(ShootZeroProjectilesBulletScript));
			int lastOption = 16;
			if ((this.Owner as PlayerController).PlayerHasActiveSynergy("Who's da Boss now ?"))
			{
				lastOption = 26;
			}
			int randomSelect = 16;//Random.Range(1, lastOption);
			bulletsDamageMultiplier = 1;

			switch (randomSelect)
			{
				case 1:
					enemyGuid = EnemyGuidDatabase.Entries["lead_maiden"];
					bulletScriptSelected = new CustomBulletScriptSelector(typeof(LeadMaidenSustain1));
					bulletsDamageMultiplier = 4f;
					break;
				case 2:
					enemyGuid = EnemyGuidDatabase.Entries["bookllet"];
					bulletScriptSelected = new CustomBulletScriptSelector(typeof(BulletScriptGunAngryBookBasicAttack2));
					bulletsDamageMultiplier = 4f;
					break;
				case 3:
					enemyGuid = EnemyGuidDatabase.Entries["chancebulon"];
					bulletScriptSelected = new CustomBulletScriptSelector(typeof(BulletScriptGunChancebulonDice1));
					bulletsDamageMultiplier = 4f;
					break;
				case 4:
					enemyGuid = EnemyGuidDatabase.Entries["chancebulon"];
					bulletScriptSelected = new CustomBulletScriptSelector(typeof(BulletScriptGunChancebulonBouncingRing1));
					bulletsDamageMultiplier = 8f;
					break;
				case 5:
					enemyGuid = EnemyGuidDatabase.Entries["blue_shotgun_kin"];
					bulletScriptSelected = new CustomBulletScriptSelector(typeof(BulletScriptGunBulletShotgunManBlueBasicAttack1));
					bulletsDamageMultiplier = 10f;
					break;
				case 6:
					enemyGuid = EnemyGuidDatabase.Entries["cubulead"];
					bulletScriptSelected = new CustomBulletScriptSelector(typeof(CubuleadSlam1));
					bulletsDamageMultiplier = 8f;
					break;
				case 7:
					enemyGuid = EnemyGuidDatabase.Entries["fungun"];
					bulletScriptSelected = new CustomBulletScriptSelector(typeof(MushroomGuySmallWaft1));
					bulletsDamageMultiplier = 10f;
					break;
				case 8:
					enemyGuid = EnemyGuidDatabase.Entries["shotgrub"];
					bulletScriptSelected = new CustomBulletScriptSelector(typeof(BulletScriptGunShotgrubManAttack1));
					bulletsDamageMultiplier = 10f;
					break;
				case 9:
					enemyGuid = EnemyGuidDatabase.Entries["creech"];
					bulletScriptSelected = new CustomBulletScriptSelector(typeof(ShotgunCreecherUglyCircle1));
					bulletsDamageMultiplier = 6f;
					break;
				case 10: // maybe needs modifications to make the whip disappear when the companion die
					enemyGuid = EnemyGuidDatabase.Entries["wall_mimic"];
					bulletScriptSelected = new CustomBulletScriptSelector(typeof(BulletScriptGunWallMimicSlam1));
					bulletsDamageMultiplier = 6f;
					break;
				case 11:
					enemyGuid = EnemyGuidDatabase.Entries["green_bookllet"];
					bulletScriptSelected = new CustomBulletScriptSelector(typeof(AngryBookGreenClover1));
					bulletsDamageMultiplier = 6f;
					break;
				case 12:
					enemyGuid = EnemyGuidDatabase.Entries["phaser_spider"];
					bulletScriptSelected = new CustomBulletScriptSelector(typeof(BulletScriptGunPhaseSpiderWeb1));
					bulletsDamageMultiplier = 6f;
					break;
				case 13:
					enemyGuid = EnemyGuidDatabase.Entries["gun_nut"];
					bulletScriptSelected = new CustomBulletScriptSelector(typeof(BulletScriptGunGunNutCone));
					bulletsDamageMultiplier = 6f;
					break;
				case 14:
					enemyGuid = EnemyGuidDatabase.Entries["bookllet"];
					bulletScriptSelected = new CustomBulletScriptSelector(typeof(AngryBookBasicAttack1));
					bulletsDamageMultiplier = 6f;
					break;
				case 15:
					enemyGuid = EnemyGuidDatabase.Entries["gigi"];
					bulletScriptSelected = new CustomBulletScriptSelector(typeof(BulletScriptGunBirdEggVomit2));
					bulletsDamageMultiplier = 8f;
					break;
				case 16:
					enemyGuid = EnemyGuidDatabase.Entries["dr_wolfs_monster"];
					bulletScriptSelected = new CustomBulletScriptSelector(typeof(BulletScriptGunBossFinalGuideSword1));
					bulletsDamageMultiplier = 12f;
					break;
				case 17:
					enemyGuid = EnemyGuidDatabase.Entries["gatling_gull"];
					bulletScriptSelected = new CustomBulletScriptSelector(typeof(BulletScriptGunGatlingGullFanSpray1));
					bulletsDamageMultiplier = 12f;
					break;
				case 18:
					enemyGuid = EnemyGuidDatabase.Entries["bullet_king"];
					bulletScriptSelected = new CustomBulletScriptSelector(typeof(BulletKingQuadShot1));
					bulletsDamageMultiplier = 6f;
					break;
				case 19:
					enemyGuid = EnemyGuidDatabase.Entries["dragun"];
					bulletScriptSelected = new CustomBulletScriptSelector(typeof(DraGunFlameBreath1));
					bulletsDamageMultiplier = 6f;
					break;
				case 20:
					enemyGuid = EnemyGuidDatabase.Entries["treadnaught"];
					bulletScriptSelected = new CustomBulletScriptSelector(typeof(BulletScriptGunTankTreaderScatterShot1));
					bulletsDamageMultiplier = 10f;
					break;
				case 21:
					enemyGuid = EnemyGuidDatabase.Entries["high_priest"];
					bulletScriptSelected = new CustomBulletScriptSelector(typeof(BulletScriptGunHighPriestFaceShoot1));
					bulletsDamageMultiplier = 60f;
					break;
				case 22:
					enemyGuid = EnemyGuidDatabase.Entries["resourceful_rat"];
					bulletScriptSelected = new CustomBulletScriptSelector(typeof(ResourcefulRatTail1));
					bulletsDamageMultiplier = 4f;
					break;
				case 23:
					enemyGuid = EnemyGuidDatabase.Entries["last_human"];
					bulletScriptSelected = new CustomBulletScriptSelector(typeof(BossFinalRobotGrenades1));
					bulletsDamageMultiplier = 6f;
					break;
				case 24:
					enemyGuid = EnemyGuidDatabase.Entries["resourceful_rat_mech"];
					bulletScriptSelected = new CustomBulletScriptSelector(typeof(CustomMetalGearRatTailgun1));
					bulletsDamageMultiplier = 6f;
					break;
				case 25:
					enemyGuid = EnemyGuidDatabase.Entries["resourceful_rat"];
					bulletScriptSelected = new CustomBulletScriptSelector(typeof(ResourcefulRatSpinFire1));
					bulletsDamageMultiplier = 6f;
					break;
				/*case 26: //do a first projectile dmg *
					enemyGuid = EnemyGuidDatabase.Entries["dragun_advanced"];
					bullets = EnemyDatabase.GetOrLoadByGuid(enemyGuid).bulletBank.Bullets;
					bulletScriptSelected = new CustomBulletScriptSelector(typeof(BulletScriptGunDraGunRocket2));
					bulletsDamageMultiplier = 6f;
					break;*/
				default:

					break;

					//meh
					/*BubbleLizardRedBubble1*/
					//op
					/*MetalGearRatSpinners1*/
					// the directional ones
					/* DraGunGlockRicochet1 */
					/* DraGunRocket1 */
					// the ones which don't work
					/*ResourcefulRatDaggers1*/
					/* DraGunGrenade1 */
					/* DraGunSpotlight1 */
					/*MetalGearRatMissiles1*/
					/*MetalGearRatLaserBullets1*/
					/*BossFinalBulletAgunimLightning1*/
			}

			AIBulletBank bulletBank = Toolbox.CopyAIBulletBank(EnemyDatabase.GetOrLoadByGuid(enemyGuid).bulletBank);//to prevent our gun from affecting the bulletbank of the enemy
            bulletBank.OnProjectileCreated = (Action<Projectile>)Delegate.Combine(bulletBank.OnProjectileCreated, new Action<Projectile>(this.OnProjCreated));
            bulletBank.CollidesWithEnemies = true;
            source.BulletManager = bulletBank;
			source.BulletScript = bulletScriptSelected;
			//var bulletScriptSelected = new CustomBulletScriptSelector(typeof(MushroomGuySmallWaft1));
			GunjuringEncyclopedia.playerGunCurrentAngle = this.gun.CurrentAngle;

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
                        //player.DoPostProcessProjectile(projectile);
                    }
                }
            }
        }

        public static float playerGunCurrentAngle = 0f;
        private bool HasReloaded;
		private static float bulletsDamageMultiplier = 1f;

	}

}
