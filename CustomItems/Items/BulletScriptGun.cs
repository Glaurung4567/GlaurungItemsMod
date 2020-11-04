using Brave.BulletScript;
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
	class BulletScriptGun : AdvancedGunBehavior
	{
		public static void Add()
		{
			Gun gun = ETGMod.Databases.Items.NewGun("BulletScript Gun", "bulletscriptgun");
			Game.Items.Rename("outdated_gun_mods:bulletscript_gun", "gl:bulletscript_gun");
			gun.gameObject.AddComponent<BulletScriptGun>();
			gun.SetShortDescription("Janky");
			gun.SetLongDescription("WIP");
			gun.SetupSprite(null, "jpxfrd_idle_001", 8);
			gun.SetAnimationFPS(gun.shootAnimation, 24);
			gun.SetAnimationFPS(gun.reloadAnimation, 12);
			gun.AddProjectileModuleFrom("klobb", true, false);

			gun.carryPixelOffset = new IntVector2((int)2f, (int)0f);
			gun.DefaultModule.ammoCost = 1;
			gun.DefaultModule.angleVariance = 0f;
			gun.DefaultModule.shootStyle = ProjectileModule.ShootStyle.SemiAutomatic;
			gun.DefaultModule.sequenceStyle = ProjectileModule.ProjectileSequenceStyle.Random;
			gun.reloadTime = 2.2f;
			gun.DefaultModule.cooldownTime = 2f;
			gun.DefaultModule.numberOfShotsInClip = 3;
			gun.SetBaseMaxAmmo(72);

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
			projectile.transform.parent = gun.barrelOffset;
			//projectile.SetProjectileSpriteRight("build_projectile", 5, 5);

			ETGMod.Databases.Items.Add(gun, null, "ANY");
		}

		protected override void OnPickup(PlayerController player)
		{
			base.OnPickup(player);
			player.OnRoomClearEvent += this.OnLeaveCombat;
		}

		// Token: 0x0600040C RID: 1036 RVA: 0x000034D8 File Offset: 0x000016D8
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

		public override void OnReload(PlayerController player, Gun gun)
		{
			base.OnReload(player, gun);
			RemoveHolders();
		}

		private void RemoveHolders()
		{
			roomWhereThisWasFired = null;
			foreach (AIActor holder in spawnedChainHolders)
			{
				if (holder != null)
				{
					holder.EraseFromExistence(true);
				}
			}
			spawnedChainHolders = new List<AIActor>();
		}

		public override void PostProcessProjectile(Projectile projectile)
		{
			SpawnBulletCompanion(this.Player, projectile);
			if (roomWhereThisWasFired == null && gun.CurrentOwner && (gun.CurrentOwner as PlayerController).CurrentRoom != null)
			{
				roomWhereThisWasFired = (gun.CurrentOwner as PlayerController).CurrentRoom;
			}
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


				// to prevent the aiActor from moving
				aiactor.behaviorSpeculator.MovementBehaviors = EnemyDatabase.GetOrLoadByGuid("b08ec82bef6940328c7ecd9ffc6bd16c").behaviorSpeculator.MovementBehaviors;
				aiactor.behaviorSpeculator.TargetBehaviors = EnemyDatabase.GetOrLoadByGuid("b08ec82bef6940328c7ecd9ffc6bd16c").behaviorSpeculator.TargetBehaviors;
				aiactor.behaviorSpeculator.OtherBehaviors = EnemyDatabase.GetOrLoadByGuid("b08ec82bef6940328c7ecd9ffc6bd16c").behaviorSpeculator.OtherBehaviors;
				aiactor.behaviorSpeculator.AttackBehaviors = EnemyDatabase.GetOrLoadByGuid("b08ec82bef6940328c7ecd9ffc6bd16c").behaviorSpeculator.AttackBehaviors;
				//EnemyAPI.EnemyAPITools.DebugInformation(aiactor.behaviorSpeculator); //EnemyGuidDatabase.Entries["bullet_kin"]

				aiactor.sprite.renderer.enabled = false; // to make the companion invisible
				aiactor.aiShooter.ToggleGunAndHandRenderers(false, "bulletscript enemy with invisible gun");
				aiactor.procedurallyOutlined = false;
				aiactor.CorpseObject = null;
				aiactor.ImmuneToAllEffects = true;
				aiactor.SetIsFlying(true, "I'm a bullet too!");
				aiactor.ToggleShadowVisiblity(false);
				aiactor.HasShadow = false;


				aiactor.CanTargetEnemies = false;
				aiactor.CanTargetPlayers = false;
				aiactor.CompanionOwner = owner;
				aiactor.HitByEnemyBullets = false;
				PhysicsEngine.Instance.RegisterOverlappingGhostCollisionExceptions(aiactor.specRigidbody, null, false);

				aiactor.IsHarmlessEnemy = true;
				aiactor.IgnoreForRoomClear = true;
				aiactor.PreventAutoKillOnBossDeath = true;

				aiactor.ManualKnockbackHandling = true; //dunno if this is useful
				aiactor.knockbackDoer.SetImmobile(true, "Chainer"); // from the TetherBehavior to prevent the companion from being pushed by explosions
				aiactor.PreventFallingInPitsEver = true;

				//aiactor.HandleReinforcementFallIntoRoom(0f); //don't use this if you want your mob to be invisible
				aiactor.gameObject.AddComponent<CompanionController>();
				CompanionController component = aiactor.gameObject.GetComponent<CompanionController>();
				component.CanInterceptBullets = false;
				component.Initialize(owner);

				aiactor.aiShooter.AimAtPoint(owner.AimCenter);


				if (aiactor.healthHaver != null)
				{
					aiactor.healthHaver.PreventAllDamage = true;
				}

				SelectGunAttack(aiactor);

				spawnedChainHolders.Add(aiactor);
				projectile.DieInAir();
			}
			catch (Exception e)
			{
				Tools.PrintException(e);
			}
		}

		private void SelectGunAttack(AIActor aiactor)
        {
			List<AIBulletBank.Entry>  bullets = aiactor.bulletBank.Bullets;
			var bulletScriptSelected = new CustomBulletScriptSelector(typeof(ShootZeroProjectilesBulletScript));
			int randomSelect = Random.Range(1, 6);
			bulletsDamageMultiplier = 1;

			switch (randomSelect)
			{
				case 1:
					bullets = EnemyDatabase.GetOrLoadByGuid(EnemyGuidDatabase.Entries["lead_maiden"]).bulletBank.Bullets;
					bulletScriptSelected = new CustomBulletScriptSelector(typeof(LeadMaidenSustain1));
					bulletsDamageMultiplier = 4f;
					break;
				case 2:
					bullets = EnemyDatabase.GetOrLoadByGuid(EnemyGuidDatabase.Entries["dragun"]).bulletBank.Bullets;
					bulletScriptSelected = new CustomBulletScriptSelector(typeof(DraGunFlameBreath1));
					bulletsDamageMultiplier = 6f;
					break;
				case 3:
					bullets = EnemyDatabase.GetOrLoadByGuid(EnemyGuidDatabase.Entries["chancebulon"]).bulletBank.Bullets;
					bulletScriptSelected = new CustomBulletScriptSelector(typeof(BulletScriptGunChancebulonDice1));
					bulletsDamageMultiplier = 6f;
					break;
				case 4:
					bullets = EnemyDatabase.GetOrLoadByGuid(EnemyGuidDatabase.Entries["chancebulon"]).bulletBank.Bullets;
					bulletScriptSelected = new CustomBulletScriptSelector(typeof(BulletScriptGunChancebulonBouncingRing1));
					bulletsDamageMultiplier = 8f;
					break;
				case 5:
					bullets = EnemyDatabase.GetOrLoadByGuid(EnemyGuidDatabase.Entries["blue_shotgun_kin"]).bulletBank.Bullets;
					bulletScriptSelected = new CustomBulletScriptSelector(typeof(BulletScriptGunBulletShotgunManBlueBasicAttack1));
					bulletsDamageMultiplier = 10f;
					break;
				default:
					break;//BulletShotgunManBlueBasicAttack1
					// dunno, needs rework
					/*
					bullets = EnemyDatabase.GetOrLoadByGuid(EnemyGuidDatabase.Entries["cubulead"]).bulletBank.Bullets;
					bulletScriptSelected = new CustomBulletScriptSelector(typeof(CubuleadSlam1));
					*/
					// the too laggy ones
					/*
					bullets = EnemyDatabase.GetOrLoadByGuid(EnemyGuidDatabase.Entries["bullet_king"]).bulletBank.Bullets;
					//Tools.Print(bullets.Count, "ffffff", true);
					//EnemyAPI.EnemyAPITools.DebugInformation(EnemyDatabase.GetOrLoadByGuid(EnemyGuidDatabase.Entries["bullet_king"]).aiActor.behaviorSpeculator);
					bulletScriptSelected = new CustomBulletScriptSelector(typeof(BulletKingQuadShot1));
					*/
					/*
					bullets = EnemyDatabase.GetOrLoadByGuid(EnemyGuidDatabase.Entries["dragun"]).bulletBank.Bullets;
					bulletScriptSelected = new CustomBulletScriptSelector(typeof(DraGunRocket1));
					*/
					// the directional ones
					/* DraGunGlockRicochet1 */
					// the ones which don't work
					/* DraGunGrenade1 */
					/* DraGunSpotlight1 */
			}
			aiactor.bulletBank.Bullets = bullets;
			if (aiactor.bulletBank != null)
			{
				AIBulletBank bulletBank = aiactor.bulletBank;
				bulletBank.OnProjectileCreated = (Action<Projectile>)Delegate.Combine(bulletBank.OnProjectileCreated, new Action<Projectile>(BulletScriptGun.OnPostProcessProjectile));
			}
			if (aiactor.aiShooter != null)
			{
				AIShooter aiShooter = aiactor.aiShooter;
				aiShooter.PostProcessProjectile = (Action<Projectile>)Delegate.Combine(aiShooter.PostProcessProjectile, new Action<Projectile>(BulletScriptGun.OnPostProcessProjectile));
			}

			/*
			BulletScriptGunCompanionisedEnemyBulletModifiers companionisedBullets = aiactor.gameObject.GetOrAddComponent<BulletScriptGunCompanionisedEnemyBulletModifiers>();
			companionisedBullets.jammedDamageMultiplier = 2f;
			companionisedBullets.TintBullets = false;
			companionisedBullets.TintColor = Color.grey;
			companionisedBullets.baseBulletDamage = bulletsDamageMultiplier;
			*/

			// to make the companion shoot once
			aiactor.aiShooter.ShootBulletScript(bulletScriptSelected);
		}


		private static void OnPostProcessProjectile(Projectile proj)
		{
			//proj.AdjustPlayerProjectileTint(Color.yellow, 0);
			proj.baseData.damage *= bulletsDamageMultiplier;
			proj.collidesWithPlayer = false;
			proj.TreatedAsNonProjectileForChallenge = true;
			proj.specRigidbody.OnPreRigidbodyCollision = (SpeculativeRigidbody.OnPreRigidbodyCollisionDelegate)Delegate.Combine(proj.specRigidbody.OnPreRigidbodyCollision, new SpeculativeRigidbody.OnPreRigidbodyCollisionDelegate(BulletScriptGun.HandlePreCollision));

		}

		private static void HandlePreCollision(SpeculativeRigidbody myRigidbody, PixelCollider myPixelCollider, SpeculativeRigidbody otherRigidbody, PixelCollider otherPixelCollider)
		{
			bool flag = otherRigidbody && otherRigidbody.healthHaver && otherRigidbody.aiActor && otherRigidbody.aiActor.CompanionOwner;
			if (flag)
			{
				float damage = myRigidbody.projectile.baseData.damage;
				myRigidbody.projectile.baseData.damage = 0f;
				GameManager.Instance.StartCoroutine(BulletScriptGun.ChangeProjectileDamage(myRigidbody.projectile, damage));
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

		public override void OnPostFired(PlayerController player, Gun gun)
		{
			gun.PreventNormalFireAudio = true;
			//AkSoundEngine.PostEvent("Play_OBJ_heart_heal_01", gameObject);
		}
		private bool HasReloaded;
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
				AkSoundEngine.PostEvent("Play_WPN_SAA_reload_01", base.gameObject);
			}
		}

		public static float playerGunCurrentAngle = 0f;
		private static float bulletsDamageMultiplier = 1f;
		private List<AIActor> spawnedChainHolders = new List<AIActor>();
		private RoomHandler roomWhereThisWasFired = null;
	}


	/* ------------------------------------------------------------------------------------------------------------------------ */

	/// <summary>
	/// from NN https://github.com/Nevernamed22/OnceMoreIntoTheBreach/blob/master/MakingAnItem/KalibersEye.cs 
	/// </summary>
	public class BulletScriptGunCompanionisedEnemyBulletModifiers : BraveBehaviour
	{
		public BulletScriptGunCompanionisedEnemyBulletModifiers()
		{
			this.baseBulletDamage = 10f;
			this.TintBullets = false;
			this.TintColor = Color.grey;
			this.jammedDamageMultiplier = 2f;
		}
		public void Start()
		{
			enemy = base.aiActor;
			AIBulletBank bulletBank2 = enemy.bulletBank;
			foreach (AIBulletBank.Entry bullet in bulletBank2.Bullets)
			{
				bullet.BulletObject.GetComponent<Projectile>().BulletScriptSettings.preventPooling = true;
			}
			if (enemy.aiShooter != null)
			{
				AIShooter aiShooter = enemy.aiShooter;
				aiShooter.PostProcessProjectile = (Action<Projectile>)Delegate.Combine(aiShooter.PostProcessProjectile, new Action<Projectile>(this.PostProcessSpawnedEnemyProjectiles));
			}

			if (enemy.bulletBank != null)
			{
				AIBulletBank bulletBank = enemy.bulletBank;
				bulletBank.OnProjectileCreated = (Action<Projectile>)Delegate.Combine(bulletBank.OnProjectileCreated, new Action<Projectile>(this.PostProcessSpawnedEnemyProjectiles));
			}
		}
		private void PostProcessSpawnedEnemyProjectiles(Projectile proj)
		{
			//if (TintBullets) { proj.AdjustPlayerProjectileTint(this.TintColor, 0); }
			if (enemy != null)
			{
				if (enemy.aiActor != null)
				{
					proj.TreatedAsNonProjectileForChallenge = true;
					proj.ImmuneToBlanks = true;
					proj.ImmuneToSustainedBlanks = true; //don't work
					proj.baseData.damage *= jammedDamageMultiplier;

					//if (enemy.aiActor.IsBlackPhantom) { proj.baseData.damage = baseBulletDamage * jammedDamageMultiplier; }
				}
			}
			else { ETGModConsole.Log("Shooter is NULL"); }
		}

		private AIActor enemy;
		public float baseBulletDamage;
		public float jammedDamageMultiplier;
		public bool TintBullets;
		public Color TintColor;

	}

	/* ------------------------------------------------------------------------------------------------------------------------ */

	public class BulletScriptGunChancebulonDice1 : Script
	{
		//public float aimDirection { get; private set; }
		private float aimDirection;
		public float AimDirectionGet()
		{
			return this.aimDirection;
		}
		public void AimDirectionSet(float value)
		{
			this.aimDirection = value;
		}

		// Token: 0x060004AD RID: 1197 RVA: 0x0001558C File Offset: 0x0001378C
		protected override IEnumerator Top()
		{
			this.EndOnBlank = true;
			this.FireSquare();
			AimDirectionSet(BulletScriptGun.playerGunCurrentAngle);// this.AimDirection;
			yield return this.Wait(15);
			float distanceToTarget = (this.BulletManager.PlayerPosition() - this.Position).magnitude;
			if (distanceToTarget > 4.5f)
			{
				AimDirectionSet(this.GetAimDirection(1f, 10f));
			}
			yield break;
		}

		// Token: 0x060004AE RID: 1198 RVA: 0x000155A8 File Offset: 0x000137A8
		private void FireSquare()
		{
			Vector2 vector = new Vector2(2.2f, 0f).Rotate(45f);
			Vector2 vector2 = new Vector2(2.2f, 0f).Rotate(135f);
			Vector2 vector3 = new Vector2(2.2f, 0f).Rotate(225f);
			Vector2 vector4 = new Vector2(2.2f, 0f).Rotate(-45f);
			this.FireExpandingLine(vector, vector2, 5);
			this.FireExpandingLine(vector2, vector3, 5);
			this.FireExpandingLine(vector3, vector4, 5);
			this.FireExpandingLine(vector4, vector, 5);
			base.Fire(new BulletScriptGunChancebulonDice1.ExpandingBullet(this, new Vector2(0f, 0f), new int?(0)));
			base.Fire(new BulletScriptGunChancebulonDice1.ExpandingBullet(this, new Vector2(0f, 0f), new int?(1)));
			base.Fire(new BulletScriptGunChancebulonDice1.ExpandingBullet(this, new Vector2(0f, 0f), new int?(2)));
			base.Fire(new BulletScriptGunChancebulonDice1.ExpandingBullet(this, new Vector2(0f, 0f), new int?(3)));
			base.Fire(new BulletScriptGunChancebulonDice1.ExpandingBullet(this, new Vector2(0f, 0f), new int?(4)));
			base.Fire(new BulletScriptGunChancebulonDice1.ExpandingBullet(this, new Vector2(0f, 0f), new int?(5)));
		}

		// Token: 0x060004AF RID: 1199 RVA: 0x00015708 File Offset: 0x00013908
		private void FireExpandingLine(Vector2 start, Vector2 end, int numBullets)
		{
			for (int i = 0; i < numBullets; i++)
			{
				base.Fire(new BulletScriptGunChancebulonDice1.ExpandingBullet(this, Vector2.Lerp(start, end, (float)i / ((float)numBullets - 1f)), null));
			}
		}

		// Token: 0x04000485 RID: 1157
		public const float Radius = 2f;

		// Token: 0x04000486 RID: 1158
		public const int GrowTime = 15;

		// Token: 0x04000487 RID: 1159
		public const float RotationSpeed = 180f;

		// Token: 0x04000488 RID: 1160
		public const float BulletSpeed = 10f;

		// Token: 0x0200013C RID: 316
		public class ExpandingBullet : Bullet
		{
			// Token: 0x060004B0 RID: 1200 RVA: 0x0001574E File Offset: 0x0001394E
			public ExpandingBullet(BulletScriptGunChancebulonDice1 parent, Vector2 offset, int? numeralIndex = null) : base(null, false, false, false)
			{
				this.m_parent = parent;
				this.m_offset = offset;
				this.m_numeralIndex = numeralIndex;
			}

			// Token: 0x060004B1 RID: 1201 RVA: 0x00015770 File Offset: 0x00013970
			protected override IEnumerator Top()
			{
				this.ManualControl = true;
				Vector2 centerPosition = this.Position;
				for (int i = 0; i < 15; i++)
				{
					this.UpdateVelocity();
					centerPosition += this.Velocity / 60f;
					Vector2 actualOffset = Vector2.Lerp(Vector2.zero, this.m_offset, (float)i / 14f);
					actualOffset = actualOffset.Rotate(3f * (float)i);
					this.Position = centerPosition + actualOffset;
					yield return this.Wait(1);
				}
				this.Direction = this.m_parent.AimDirectionGet();
				this.Speed = 10f;
				for (int j = 0; j < 300; j++)
				{
					this.UpdateVelocity();
					centerPosition += this.Velocity / 60f;
					if (this.m_numeralIndex != null && j % 13 == 0 && j != 0)
					{
						this.m_currentNumeral = (this.m_currentNumeral + 1) % 6;
						switch (this.m_currentNumeral)
						{
							case 0:
								{
									int? numeralIndex = this.m_numeralIndex;
									if (numeralIndex != null && numeralIndex.GetValueOrDefault() < 3)
									{
										this.m_offset = new Vector2(-0.7f, 0.7f);
									}
									else
									{
										this.m_offset = new Vector2(0.7f, -0.7f);
									}
									break;
								}
							case 1:
								{
									int? numeralIndex2 = this.m_numeralIndex;
									if (numeralIndex2 != null && numeralIndex2.GetValueOrDefault() < 2)
									{
										this.m_offset = new Vector2(-0.7f, 0.7f);
									}
									else
									{
										int? numeralIndex3 = this.m_numeralIndex;
										if (numeralIndex3 != null && numeralIndex3.GetValueOrDefault() < 4)
										{
											this.m_offset = new Vector2(0f, 0f);
										}
										else
										{
											this.m_offset = new Vector2(0.7f, -0.7f);
										}
									}
									break;
								}
							case 2:
								{
									int? numeralIndex4 = this.m_numeralIndex;
									if (numeralIndex4 != null && numeralIndex4.GetValueOrDefault() < 1)
									{
										this.m_offset = new Vector2(-0.6f, -0.6f);
									}
									else
									{
										int? numeralIndex5 = this.m_numeralIndex;
										if (numeralIndex5 != null && numeralIndex5.GetValueOrDefault() < 2)
										{
											this.m_offset = new Vector2(-0.6f, 0.6f);
										}
										else
										{
											int? numeralIndex6 = this.m_numeralIndex;
											if (numeralIndex6 != null && numeralIndex6.GetValueOrDefault() < 3)
											{
												this.m_offset = new Vector2(0f, 0f);
											}
											else
											{
												int? numeralIndex7 = this.m_numeralIndex;
												if (numeralIndex7 != null && numeralIndex7.GetValueOrDefault() < 4)
												{
													this.m_offset = new Vector2(0.6f, -0.6f);
												}
												else
												{
													this.m_offset = new Vector2(0.6f, 0.6f);
												}
											}
										}
									}
									break;
								}
							case 3:
								{
									int? numeralIndex8 = this.m_numeralIndex;
									if (numeralIndex8 != null && numeralIndex8.GetValueOrDefault() < 2)
									{
										this.m_offset = new Vector2(-0.6f, -0.6f);
									}
									else
									{
										int? numeralIndex9 = this.m_numeralIndex;
										if (numeralIndex9 != null && numeralIndex9.GetValueOrDefault() < 3)
										{
											this.m_offset = new Vector2(-0.6f, 0.6f);
										}
										else
										{
											int? numeralIndex10 = this.m_numeralIndex;
											if (numeralIndex10 != null && numeralIndex10.GetValueOrDefault() < 4)
											{
												this.m_offset = new Vector2(0.6f, -0.6f);
											}
											else
											{
												this.m_offset = new Vector2(0.6f, 0.6f);
											}
										}
									}
									break;
								}
							case 4:
								{
									int? numeralIndex11 = this.m_numeralIndex;
									if (numeralIndex11 != null && numeralIndex11.GetValueOrDefault() < 1)
									{
										this.m_offset = new Vector2(-0.6f, -0.6f);
									}
									else
									{
										int? numeralIndex12 = this.m_numeralIndex;
										if (numeralIndex12 != null && numeralIndex12.GetValueOrDefault() < 2)
										{
											this.m_offset = new Vector2(-0.6f, 0f);
										}
										else
										{
											int? numeralIndex13 = this.m_numeralIndex;
											if (numeralIndex13 != null && numeralIndex13.GetValueOrDefault() < 3)
											{
												this.m_offset = new Vector2(-0.6f, 0.6f);
											}
											else
											{
												int? numeralIndex14 = this.m_numeralIndex;
												if (numeralIndex14 != null && numeralIndex14.GetValueOrDefault() < 4)
												{
													this.m_offset = new Vector2(0.6f, -0.6f);
												}
												else
												{
													int? numeralIndex15 = this.m_numeralIndex;
													if (numeralIndex15 != null && numeralIndex15.GetValueOrDefault() < 5)
													{
														this.m_offset = new Vector2(0.6f, 0f);
													}
													else
													{
														this.m_offset = new Vector2(0.6f, 0.6f);
													}
												}
											}
										}
									}
									break;
								}
							case 5:
								this.m_offset = new Vector2(0f, 0f);
								break;
						}
					}
					this.Position = centerPosition + this.m_offset.Rotate(3f * (float)(15 + j));
					yield return this.Wait(1);
				}
				this.Vanish(false);
				yield break;
			}

			// Token: 0x0400048A RID: 1162
			private const int SingleFaceShowTime = 13;

			// Token: 0x0400048B RID: 1163
			private BulletScriptGunChancebulonDice1 m_parent;

			// Token: 0x0400048C RID: 1164
			private Vector2 m_offset;

			// Token: 0x0400048D RID: 1165
			private int? m_numeralIndex;

			// Token: 0x0400048E RID: 1166
			private int m_currentNumeral;
		}
	}

	/*--------------------------------------------------------------------------------------------------------------------------------------*/

	public class BulletScriptGunChancebulonBouncingRing1 : Script
	{
		// Token: 0x06000486 RID: 1158 RVA: 0x000149F4 File Offset: 0x00012BF4
		protected override IEnumerator Top()
		{
			float direction = BulletScriptGun.playerGunCurrentAngle;//base.GetAimDirection((float)((UnityEngine.Random.value >= 0.4f) ? 0 : 1), 8f) + UnityEngine.Random.Range(-10f, 10f);
			for (int i = 0; i < 18; i++)
			{
				float angle = (float)i * 20f;
				Vector2 desiredOffset = BraveMathCollege.DegreesToVector(angle, 1.8f);
				base.Fire(new Direction(direction, DirectionType.Absolute, -1f), new Speed(8f, SpeedType.Absolute), new BulletScriptGunChancebulonBouncingRing1.BouncingRingBullet("bouncingRing", desiredOffset));
			}
			base.Fire(new Direction(direction, DirectionType.Absolute, -1f), new Speed(8f, SpeedType.Absolute), new BulletScriptGunChancebulonBouncingRing1.BouncingRingBullet("bouncingRing", new Vector2(-0.7f, 0.7f)));
			base.Fire(new Direction(direction, DirectionType.Absolute, -1f), new Speed(8f, SpeedType.Absolute), new BulletScriptGunChancebulonBouncingRing1.BouncingRingBullet("bouncingMouth", new Vector2(0f, 0.4f)));
			base.Fire(new Direction(direction, DirectionType.Absolute, -1f), new Speed(8f, SpeedType.Absolute), new BulletScriptGunChancebulonBouncingRing1.BouncingRingBullet("bouncingRing", new Vector2(0.7f, 0.7f)));
			return null;
		}

		// Token: 0x04000465 RID: 1125
		private const int NumBullets = 18;

		// Token: 0x02000133 RID: 307
		public class BouncingRingBullet : Bullet
		{
			// Token: 0x06000487 RID: 1159 RVA: 0x00014B27 File Offset: 0x00012D27
			public BouncingRingBullet(string name, Vector2 desiredOffset) : base(name, false, false, false)
			{
				this.m_desiredOffset = desiredOffset;
			}

			// Token: 0x06000488 RID: 1160 RVA: 0x00014B3C File Offset: 0x00012D3C
			protected override IEnumerator Top()
			{
				Vector2 centerPoint = this.Position;
				Vector2 lowestOffset = BraveMathCollege.DegreesToVector(-90f, 1.5f);
				Vector2 currentOffset = Vector2.zero;
				float squishFactor = 1f;
				float verticalOffset = 0f;
				int unsquishIndex = 100;
				this.ManualControl = true;
				for (int i = 0; i < 300; i++)
				{
					if (i < 30)
					{
						currentOffset = Vector2.Lerp(Vector2.zero, this.m_desiredOffset, (float)i / 30f);
					}
					verticalOffset = (Mathf.Abs(Mathf.Cos((float)i / 90f * 3.14159274f)) - 1f) * 2.5f;
					if (unsquishIndex <= 10)
					{
						squishFactor = Mathf.Abs(Mathf.SmoothStep(0.6f, 1f, (float)unsquishIndex / 10f));
						unsquishIndex++;
					}
					Vector2 relativeOffset = currentOffset - lowestOffset;
					Vector2 squishedOffset = lowestOffset + relativeOffset.Scale(1f, squishFactor);
					this.UpdateVelocity();
					centerPoint += this.Velocity / 60f;
					this.Position = centerPoint + squishedOffset + new Vector2(0f, verticalOffset);
					if (i % 90 == 45)
					{
						for (int j = 1; j <= 10; j++)
						{
							squishFactor = Mathf.Abs(Mathf.SmoothStep(1f, 0.5f, (float)j / 10f));
							relativeOffset = currentOffset - lowestOffset;
							squishedOffset = lowestOffset + relativeOffset.Scale(1f, squishFactor);
							centerPoint += 0.333f * this.Velocity / 60f;
							this.Position = centerPoint + squishedOffset + new Vector2(0f, verticalOffset);
							yield return this.Wait(1);
						}
						unsquishIndex = 1;
					}
					yield return this.Wait(1);
				}
				this.Vanish(false);
				yield break;
			}

			// Token: 0x04000466 RID: 1126
			private Vector2 m_desiredOffset;
		}
	}

	/*-------------------------------------------------------------------------------------------------------------------------------------------*/

	public class BulletScriptGunBulletShotgunManBlueBasicAttack1 : Script
	{
		// Token: 0x0600041D RID: 1053 RVA: 0x000130D8 File Offset: 0x000112D8
		protected override IEnumerator Top()
		{
			float aimDirection = BulletScriptGun.playerGunCurrentAngle;//this.AimDirection;
			for (int i = -2; i <= 2; i++)
			{
				this.Fire(new Direction((float)(i * 20) + aimDirection, DirectionType.Absolute, -1f), new Speed(5f, SpeedType.Absolute), null);
			}
			yield return this.Wait(40);
			if (this.BulletBank && this.BulletBank.behaviorSpeculator.IsStunned)
			{
				yield break;
			}
			if (BraveMathCollege.AbsAngleBetween(this.AimDirection, aimDirection) > 30f)
			{
				aimDirection = BulletScriptGun.playerGunCurrentAngle;//this.AimDirection;
			}
			for (float num = -1.5f; num <= 1.5f; num += 1f)
			{
				this.Fire(new Direction(num * 20f + aimDirection, DirectionType.Absolute, -1f), new Speed(5f, SpeedType.Absolute), null);
			}
			yield break;
		}
	}

}
