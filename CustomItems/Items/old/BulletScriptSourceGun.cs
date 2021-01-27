using Brave.BulletScript;
using EnemyAPI;
using Gungeon;
using ItemAPI;
using System;
using System.Collections;
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

            projectile.sprite.renderer.enabled = false;
            projectile.hitEffects.suppressMidairDeathVfx = true;
            projectile.baseData.damage = 0f;
            projectile.baseData.speed = 100.00f;
            projectile.baseData.force = 0f;
            projectile.baseData.range *= 0.001f;
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
			string enemyGuid = EnemyGuidDatabase.Entries["chancebulon"];
			var bulletScriptSelected = new CustomBulletScriptSelector(typeof(BulletScriptSourceGunChancebulonDice1));

			bulletsDamageMultiplier = 3;


			AIBulletBank bulletBank = Toolbox.CopyAIBulletBank(EnemyDatabase.GetOrLoadByGuid(enemyGuid).bulletBank);//to prevent our gun from affecting the bulletbank of the enemy
            bulletBank.OnProjectileCreated = (Action<Projectile>)Delegate.Combine(bulletBank.OnProjectileCreated, new Action<Projectile>(this.OnProjCreated));
            bulletBank.CollidesWithEnemies = true;
            source.BulletManager = bulletBank;
			source.BulletScript = bulletScriptSelected;
			//var bulletScriptSelected = new CustomBulletScriptSelector(typeof(MushroomGuySmallWaft1));
			BulletScriptSourceGun.playerGunCurrentAngle = this.gun.CurrentAngle;

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
                        projectile.baseData.damage *= bulletsDamageMultiplier;
                        //player.DoPostProcessProjectile(projectile);
                    }
                }
            }
        }

        public static float playerGunCurrentAngle = 0f;
        private bool HasReloaded;
		private static float bulletsDamageMultiplier = 1f;

	}

	//------------------------------------------------------------
	public class BulletScriptSourceGunChancebulonDice1 : Script
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

		protected override IEnumerator Top()
		{
			this.EndOnBlank = true;
			this.FireSquare();
			AimDirectionSet(GunjuringEncyclopedia.playerGunCurrentAngle);// this.AimDirection;
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
			base.Fire(new BulletScriptSourceGunChancebulonDice1.ExpandingBullet(this, new Vector2(0f, 0f), new int?(0)));
			base.Fire(new BulletScriptSourceGunChancebulonDice1.ExpandingBullet(this, new Vector2(0f, 0f), new int?(1)));
			base.Fire(new BulletScriptSourceGunChancebulonDice1.ExpandingBullet(this, new Vector2(0f, 0f), new int?(2)));
			base.Fire(new BulletScriptSourceGunChancebulonDice1.ExpandingBullet(this, new Vector2(0f, 0f), new int?(3)));
			base.Fire(new BulletScriptSourceGunChancebulonDice1.ExpandingBullet(this, new Vector2(0f, 0f), new int?(4)));
			base.Fire(new BulletScriptSourceGunChancebulonDice1.ExpandingBullet(this, new Vector2(0f, 0f), new int?(5)));
		}

		private void FireExpandingLine(Vector2 start, Vector2 end, int numBullets)
		{
			for (int i = 0; i < numBullets; i++)
			{
				base.Fire(new BulletScriptSourceGunChancebulonDice1.ExpandingBullet(this, Vector2.Lerp(start, end, (float)i / ((float)numBullets - 1f)), null));
			}
		}

		public const float Radius = 2f;

		public const int GrowTime = 15;

		public const float RotationSpeed = 180f;

		public const float BulletSpeed = 10f;

		public class ExpandingBullet : Bullet
		{
			public ExpandingBullet(BulletScriptSourceGunChancebulonDice1 parent, Vector2 offset, int? numeralIndex = null) : base(null, false, false, false)
			{
				this.m_parent = parent;
				this.m_offset = offset;
				this.m_numeralIndex = numeralIndex;
			}

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

			private const int SingleFaceShowTime = 13;

			private BulletScriptSourceGunChancebulonDice1 m_parent;

			private Vector2 m_offset;

			private int? m_numeralIndex;

			private int m_currentNumeral;
		}
	}

}
