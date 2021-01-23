using Dungeonator;
using EnemyAPI;
using Gungeon;
using ItemAPI;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

namespace GlaurungItems.Items
{
	class GunjuringEncyclopedia : AdvancedGunBehavior
	{
		public static void Add()
		{
			Gun gun = ETGMod.Databases.Items.NewGun("Gunjuring Encyclopedia", "book");
			Game.Items.Rename("outdated_gun_mods:gunjuring_encyclopedia", "gl:gunjuring_encyclopedia");
			gun.gameObject.AddComponent<GunjuringEncyclopedia>();
			gun.SetShortDescription("Janky");
			gun.SetLongDescription("A book only accessible by one of the few ArchGunjurers. \n \nIt fires random attack patterns from different Gundead.");
			gun.SetupSprite(null, "book_idle_001", 8);
			gun.SetAnimationFPS(gun.shootAnimation, 12);
			GunExt.SetAnimationFPS(gun, gun.reloadAnimation, 12);

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
			//projectile.transform.parent = gun.barrelOffset;
			//projectile.SetProjectileSpriteRight("build_projectile", 5, 5);

			ETGMod.Databases.Items.Add(gun, null, "ANY");
		}

		protected override void OnPickup(PlayerController player)
		{
			base.OnPickup(player);
			player.OnRoomClearEvent += this.OnLeaveCombat;
		}

		protected override void OnPostDrop(PlayerController user)
		{
			user.OnRoomClearEvent -= this.OnLeaveCombat;
			RemoveHolders();
			base.OnPostDrop(user);
		}

		private void OnLeaveCombat(PlayerController user)
		{
			RemoveHolders();
		}

		private void RemoveHolders()
		{
			roomWhereThisWasFired = null;
			foreach (AIActor holder in spawnedScriptFirerers)
			{
				if (holder != null)
				{
					holder.EraseFromExistence(true);
				}
			}
			spawnedScriptFirerers = new List<AIActor>();
		}

		public override void OnPostFired(PlayerController player, Gun gun)
		{
			gun.PreventNormalFireAudio = true;
			//AkSoundEngine.PostEvent("Play_ENV_time_shatter_01", GameManager.Instance.gameObject);
			//AkSoundEngine.PostEvent("Play_ENM_wizard_book_01", gameObject);
			AkSoundEngine.PostEvent("Play_OBJ_book_drop_01", gameObject);
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
				if (roomWhereThisWasFired != null && gun.CurrentOwner is PlayerController && (gun.CurrentOwner as PlayerController).CurrentRoom != roomWhereThisWasFired)
				{
					RemoveHolders();
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
				RemoveHolders();
				if((this.Owner as PlayerController).HasPassiveItem(500))
                {
					SpawnBulletCompanion(this.Player, null);
					roomWhereThisWasFired = (gun.CurrentOwner as PlayerController).CurrentRoom;
				}
			}
		}

		public override void PostProcessProjectile(Projectile projectile)
		{
			if(countTimesFiredInTimeLaps <= 1)
            {
				SpawnBulletCompanion(this.Player, projectile);
				if (roomWhereThisWasFired == null && gun.CurrentOwner && (gun.CurrentOwner as PlayerController).CurrentRoom != null)
				{
					roomWhereThisWasFired = (gun.CurrentOwner as PlayerController).CurrentRoom;
				}
				if(countTimesFiredInTimeLaps == 1)
                {
					GameManager.Instance.StartCoroutine(this.ResetCountTimesFiredInTimeLaps());

				}
			}
			countTimesFiredInTimeLaps++;
		}

        private IEnumerator ResetCountTimesFiredInTimeLaps()
        {
			yield return new WaitForSeconds(0.5f);
			countTimesFiredInTimeLaps = 0;
			yield break;
		}

        private void SpawnBulletCompanion(PlayerController owner, Projectile projectile)
		{
			try
			{
				string enemyGuid = EnemyGuidDatabase.Entries["bullet_kin"];
				AIActor orLoadByGuid = EnemyDatabase.GetOrLoadByGuid(enemyGuid);


				playerGunCurrentAngle = owner.CurrentGun.CurrentAngle; //for the aim of the bullet scripts
				Vector2 positionVector = owner.sprite.WorldBottomCenter;
				AIActor aiactor = AIActor.Spawn(orLoadByGuid.aiActor, positionVector, GameManager.Instance.Dungeon.data.GetAbsoluteRoomFromPosition(positionVector.ToIntVector2()), true, AIActor.AwakenAnimationType.Default, true);

				// to prevent the aiActor from doing anything
				aiactor.behaviorSpeculator.MovementBehaviors = EnemyDatabase.GetOrLoadByGuid("b08ec82bef6940328c7ecd9ffc6bd16c").behaviorSpeculator.MovementBehaviors;
				aiactor.behaviorSpeculator.TargetBehaviors = EnemyDatabase.GetOrLoadByGuid("b08ec82bef6940328c7ecd9ffc6bd16c").behaviorSpeculator.TargetBehaviors;
				aiactor.behaviorSpeculator.OtherBehaviors = EnemyDatabase.GetOrLoadByGuid("b08ec82bef6940328c7ecd9ffc6bd16c").behaviorSpeculator.OtherBehaviors;
				aiactor.behaviorSpeculator.AttackBehaviors = EnemyDatabase.GetOrLoadByGuid("b08ec82bef6940328c7ecd9ffc6bd16c").behaviorSpeculator.AttackBehaviors;

				aiactor.sprite.renderer.enabled = false; // to make the companion invisible
				aiactor.aiShooter.ToggleGunAndHandRenderers(false, "bulletscript enemy with invisible gun");
				aiactor.procedurallyOutlined = false;
				aiactor.CorpseObject = null;
				aiactor.ToggleShadowVisiblity(false);
				aiactor.HasShadow = false;
				aiactor.ImmuneToAllEffects = true;
				aiactor.behaviorSpeculator.ImmuneToStun = true;
				aiactor.SetIsFlying(true, "I'm a bullet too!");

				aiactor.CanTargetEnemies = false;
				aiactor.CanTargetPlayers = false;
				aiactor.CompanionOwner = owner;
				aiactor.HitByEnemyBullets = false;
				PhysicsEngine.Instance.RegisterOverlappingGhostCollisionExceptions(aiactor.specRigidbody, null, false);

				aiactor.IsHarmlessEnemy = true;
				aiactor.IgnoreForRoomClear = true;
				aiactor.PreventAutoKillOnBossDeath = true;

				aiactor.ManualKnockbackHandling = true; //dunno if this is useful
				aiactor.knockbackDoer.SetImmobile(true, "Gunjuring Encyclopedia"); // from the TetherBehavior to prevent the companion from being pushed by explosions
				aiactor.PreventFallingInPitsEver = true;

				//aiactor.HandleReinforcementFallIntoRoom(0f); //don't use this if you want your mob to be invisible
				/*aiactor.gameObject.AddComponent<CompanionController>();
				CompanionController component = aiactor.gameObject.GetComponent<CompanionController>();
				component.CanInterceptBullets = false;
				component.Initialize(owner);*/

				aiactor.specRigidbody.AddCollisionLayerIgnoreOverride(CollisionMask.LayerToMask(CollisionLayer.EnemyHitBox, CollisionLayer.EnemyCollider, CollisionLayer.PlayerHitBox,
					CollisionLayer.Projectile, CollisionLayer.PlayerCollider, CollisionLayer.PlayerBlocker, CollisionLayer.BeamBlocker));

				aiactor.specRigidbody.AddCollisionLayerIgnoreOverride(CollisionMask.LayerToMask(CollisionLayer.BulletBlocker, CollisionLayer.BulletBreakable, CollisionLayer.Trap));


				aiactor.aiShooter.AimAtPoint(owner.AimCenter); //to orient the gun of the companion

				if (aiactor.healthHaver != null)
				{
					aiactor.healthHaver.PreventAllDamage = true;
				}

				SelectGunAttack(aiactor); //to fire once

				spawnedScriptFirerers.Add(aiactor);
                if (projectile)
                {
					projectile.DieInAir();
				}
			}
			catch (Exception e)
			{
				Tools.PrintException(e);
			}
		}

		private void SelectGunAttack(AIActor aiactor)
		{
			List<AIBulletBank.Entry> bullets = aiactor.bulletBank.Bullets;
			var bulletScriptSelected = new CustomBulletScriptSelector(typeof(ShootZeroProjectilesBulletScript));
			int lastOption = 16;
			if((this.Owner as PlayerController).PlayerHasActiveSynergy("Who's da Boss now ?"))
            {
				lastOption = 26;
			}
			int randomSelect = Random.Range(1, lastOption);
			string enemyGuid = EnemyGuidDatabase.Entries["faster_tutorial_turret"]; ;
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
					bulletScriptSelected = new CustomBulletScriptSelector(typeof(MetalGearRatTailgun1));
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


			aiactor.bulletBank.Bullets = Toolbox.CopyAIBulletBank(EnemyDatabase.GetOrLoadByGuid(enemyGuid).bulletBank).Bullets;//to prevent our gun from affecting the bulletbank of the enemy;
			if (aiactor.bulletBank != null)
			{
				AIBulletBank bulletBank = aiactor.bulletBank;
				bulletBank.CollidesWithEnemies = true;
				bulletBank.OnProjectileCreated = (Action<Projectile>)Delegate.Combine(bulletBank.OnProjectileCreated, new Action<Projectile>(this.OnPostProcessProjectile));
			}
			if (aiactor.aiShooter != null)
			{
				AIShooter aiShooter = aiactor.aiShooter;
				aiShooter.PostProcessProjectile = (Action<Projectile>)Delegate.Combine(aiShooter.PostProcessProjectile, new Action<Projectile>(this.OnPostProcessProjectile));
			}
			// to make the companion shoot once
			aiactor.aiShooter.ShootBulletScript(bulletScriptSelected);
		}

		private void OnPostProcessProjectile(Projectile proj)
		{
			//proj.AdjustPlayerProjectileTint(Color.yellow, 0);
			if ((proj.Owner is AIActor && (proj.Owner as AIActor).CompanionOwner == null) || proj.TrapOwner)
			{
				return; //to prevent the OnPostProcessProjectile from affecting enemies projectiles
			}

			proj.Owner = this.gun.CurrentOwner; //to allow the projectile damage modif, otherwise it stays at 10 for some reasons

			proj.baseData.damage = 1;
			proj.baseData.damage *= bulletsDamageMultiplier;

			if (this.gun.CurrentOwner is PlayerController)
			{
				PlayerController player = (this.gun.CurrentOwner as PlayerController);
				proj.baseData.damage *= player.stats.GetStatValue(PlayerStats.StatType.Damage);
				/*if (Random.value <= 0.1f)
				{
					player.DoPostProcessProjectile(proj);
				}*/
			}
			if (proj.IsBlackBullet)
			{
				proj.baseData.damage *= 2;
			}
			proj.collidesWithPlayer = false;
			proj.TreatedAsNonProjectileForChallenge = true;
			proj.specRigidbody.OnPreRigidbodyCollision = (SpeculativeRigidbody.OnPreRigidbodyCollisionDelegate)Delegate.Combine(proj.specRigidbody.OnPreRigidbodyCollision, new SpeculativeRigidbody.OnPreRigidbodyCollisionDelegate(GunjuringEncyclopedia.HandlePreCollision));
		}

		private static void HandlePreCollision(SpeculativeRigidbody myRigidbody, PixelCollider myPixelCollider, SpeculativeRigidbody otherRigidbody, PixelCollider otherPixelCollider)
		{
			bool flag = otherRigidbody && otherRigidbody.healthHaver && otherRigidbody.aiActor && otherRigidbody.aiActor.CompanionOwner;
			if (flag)
			{
				float damage = myRigidbody.projectile.baseData.damage;
				myRigidbody.projectile.baseData.damage = 0f;
				GameManager.Instance.StartCoroutine(GunjuringEncyclopedia.ChangeProjectileDamage(myRigidbody.projectile, damage));
			}
		}
		private static IEnumerator ChangeProjectileDamage(Projectile bullet, float oldDamage)
		{
			yield return new WaitForSeconds(0.1f);
			bool flag = bullet != null;
			if (flag)
			{
				bullet.baseData.damage = oldDamage;
			}
			yield break;
		}


		private bool HasReloaded;
		public static float playerGunCurrentAngle = 0f;
		private static float bulletsDamageMultiplier = 1f;
		private List<AIActor> spawnedScriptFirerers = new List<AIActor>();
		private RoomHandler roomWhereThisWasFired = null;
		private float countTimesFiredInTimeLaps;
	}
}

