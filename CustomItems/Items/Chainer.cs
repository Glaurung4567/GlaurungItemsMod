﻿using Brave.BulletScript;
using Dungeonator;
using EnemyAPI;
using Gungeon;
using ItemAPI;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GlaurungItems.Items
{
    class Chainer : AdvancedGunBehavior
    {
        public static void Add()
        {
            Gun gun = ETGMod.Databases.Items.NewGun("Chainer", "chainer");
            Game.Items.Rename("outdated_gun_mods:chainer", "gl:chainer");
            gun.gameObject.AddComponent<Chainer>();
            gun.SetShortDescription("Free Blocks");
            gun.SetLongDescription("Fires a volley of odd link-shaped bullets which together act like the chain arms of executioners. \n \nThis weapon is fairly recent and was made in the Gungeon." +
				" Weirdly enough it has the same weaknesses as the Gundead bullets...");
            gun.SetupSprite(null, "chainer_idle_001", 8);
            gun.SetAnimationFPS(gun.shootAnimation, 24);
            gun.SetAnimationFPS(gun.reloadAnimation, 12);
            gun.AddProjectileModuleFrom("klobb", true, false);

			gun.carryPixelOffset = new IntVector2((int)2f, (int)0f);
			gun.DefaultModule.ammoCost = 1;
            gun.DefaultModule.angleVariance = 0f;
            gun.DefaultModule.shootStyle = ProjectileModule.ShootStyle.SemiAutomatic;
            gun.DefaultModule.sequenceStyle = ProjectileModule.ProjectileSequenceStyle.Random;
            gun.reloadTime = 2.2f;
            gun.DefaultModule.cooldownTime = 1f;
            gun.DefaultModule.numberOfShotsInClip = 5;
            gun.SetBaseMaxAmmo(75);

            gun.quality = PickupObject.ItemQuality.B;

            Projectile projectile = UnityEngine.Object.Instantiate<Projectile>(gun.DefaultModule.projectiles[0]);
            projectile.gameObject.SetActive(false);
            FakePrefab.MarkAsFakePrefab(projectile.gameObject);
            UnityEngine.Object.DontDestroyOnLoad(projectile);
            gun.DefaultModule.projectiles[0] = projectile;

            projectile.baseData.damage = 0f;
            projectile.baseData.speed *= 0.001f;
            projectile.baseData.force = 0f;
            projectile.baseData.range *= 0.001f;
            projectile.transform.parent = gun.barrelOffset;
            //projectile.SetProjectileSpriteRight("build_projectile", 5, 5);

            ETGMod.Databases.Items.Add(gun, null, "ANY");
			//Toolbox.SetupUnlockOnCustomFlag(gun, CustomDungeonFlags.ITEMSPECIFIC_CHAINER, true);

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
			if ((this.Owner as PlayerController).HasPassiveItem(500))
			{
				SpawnChainCompanion(this.Player, null);
				roomWhereThisWasFired = (gun.CurrentOwner as PlayerController).CurrentRoom;
			}
		}

		private void RemoveHolders()
        {
			roomWhereThisWasFired = null;
			foreach (AIActor holder in spawnedChainHolders)
			{
				if(holder != null)
                {
					holder.EraseFromExistence(true);
				}
			}
			spawnedChainHolders = new List<AIActor>();
		}

		public override void PostProcessProjectile(Projectile projectile)
        {
			if(countTimesFiredInTimeLaps <= 1)
            {
				SpawnChainCompanion(this.Player, projectile);
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
		private void SpawnChainCompanion(PlayerController owner, Projectile projectile)
        {
            try
            {
                string enemyGuid = EnemyGuidDatabase.Entries["bullet_kin"]; 
                AIActor orLoadByGuid = EnemyDatabase.GetOrLoadByGuid(enemyGuid);

				playerGunCurrentAngle = owner.CurrentGun.CurrentAngle;
				Vector2 positionVector = owner.sprite.WorldBottomCenter;
                AIActor aiactor = AIActor.Spawn(orLoadByGuid.aiActor, positionVector, GameManager.Instance.Dungeon.data.GetAbsoluteRoomFromPosition(positionVector.ToIntVector2()), true, AIActor.AwakenAnimationType.Default, true);

				aiactor.bulletBank.Bullets = Toolbox.CopyAIBulletBank(EnemyDatabase.GetOrLoadByGuid(EnemyGuidDatabase.Entries["executioner"]).bulletBank).Bullets;
				aiactor.bulletBank.CollidesWithEnemies = true;

				// to prevent the aiActor from moving
				aiactor.behaviorSpeculator.MovementBehaviors = EnemyDatabase.GetOrLoadByGuid("b08ec82bef6940328c7ecd9ffc6bd16c").behaviorSpeculator.MovementBehaviors;
				aiactor.behaviorSpeculator.TargetBehaviors = EnemyDatabase.GetOrLoadByGuid("b08ec82bef6940328c7ecd9ffc6bd16c").behaviorSpeculator.TargetBehaviors;
				aiactor.behaviorSpeculator.OtherBehaviors = EnemyDatabase.GetOrLoadByGuid("b08ec82bef6940328c7ecd9ffc6bd16c").behaviorSpeculator.OtherBehaviors;
				aiactor.behaviorSpeculator.AttackBehaviors = EnemyDatabase.GetOrLoadByGuid("b08ec82bef6940328c7ecd9ffc6bd16c").behaviorSpeculator.AttackBehaviors;
				//EnemyAPI.EnemyAPITools.DebugInformation(aiactor.behaviorSpeculator); //EnemyGuidDatabase.Entries["bullet_kin"]

				aiactor.sprite.renderer.enabled = false; // to make the companion invisible
				aiactor.aiShooter.ToggleGunAndHandRenderers(false, "chainer enemy with invisible gun");
                aiactor.procedurallyOutlined = false;
                aiactor.CorpseObject = null;
                aiactor.ImmuneToAllEffects = true;
				aiactor.behaviorSpeculator.ImmuneToStun = true;
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

				aiactor.knockbackDoer.SetImmobile(true, "Chainer"); // from the TetherBehavior to prevent the companion from being pushed by explosions
				aiactor.PreventFallingInPitsEver = true;

				//aiactor.HandleReinforcementFallIntoRoom(0f); //don't use this if you want your mob to be invisible
				/*aiactor.gameObject.AddComponent<CompanionController>();
                CompanionController component = aiactor.gameObject.GetComponent<CompanionController>();
                component.CanInterceptBullets = false;
                component.Initialize(owner);*/

				aiactor.specRigidbody.AddCollisionLayerIgnoreOverride(CollisionMask.LayerToMask(CollisionLayer.EnemyHitBox, CollisionLayer.EnemyCollider, CollisionLayer.PlayerHitBox,
					CollisionLayer.Projectile, CollisionLayer.PlayerCollider, CollisionLayer.PlayerBlocker, CollisionLayer.BeamBlocker));

				aiactor.specRigidbody.AddCollisionLayerIgnoreOverride(CollisionMask.LayerToMask(CollisionLayer.BulletBlocker, CollisionLayer.BulletBreakable, CollisionLayer.Trap));

				if (aiactor.healthHaver != null)
				{
					aiactor.healthHaver.PreventAllDamage = true;
				}

				if (aiactor.bulletBank != null)
				{
					AIBulletBank bulletBank = aiactor.bulletBank;
					bulletBank.OnProjectileCreated = (Action<Projectile>)Delegate.Combine(bulletBank.OnProjectileCreated, new Action<Projectile>(this.OnPostProcessProjectile));
				}
				if (aiactor.aiShooter != null)
				{
					AIShooter aiShooter = aiactor.aiShooter;
					aiShooter.PostProcessProjectile = (Action<Projectile>)Delegate.Combine(aiShooter.PostProcessProjectile, new Action<Projectile>(this.OnPostProcessProjectile));
				}

				/*ChainCompanionisedEnemyBulletModifiers companionisedBullets = aiactor.gameObject.GetOrAddComponent<ChainCompanionisedEnemyBulletModifiers>();
				companionisedBullets.jammedDamageMultiplier = 2f;
				companionisedBullets.TintBullets = false;
				companionisedBullets.TintColor = Color.grey;
				companionisedBullets.baseBulletDamage = 2f;*/

				// to make the companion shoot once
				aiactor.aiShooter.AimAtPoint(owner.AimCenter);
				aiactor.aiShooter.ShootBulletScript(new CustomBulletScriptSelector(typeof(Chain1)));

				spawnedChainHolders.Add(aiactor);
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

		private void OnPostProcessProjectile(Projectile proj)
		{
			if (proj.Owner is AIActor && !(proj.Owner as AIActor).CompanionOwner)
			{
				return; //to prevent the OnPostProcessProjectile from affecting enemies projectiles
			}
			//proj.AdjustPlayerProjectileTint(Color.grey, 0);

			proj.Owner = this.gun.CurrentOwner; //to allow the projectile damage modif, otherwise it stays at 10 for some reasons
			proj.baseData.damage = 1;
			proj.collidesWithPlayer = false;
			proj.collidesWithEnemies = true;
			proj.TreatedAsNonProjectileForChallenge = true;
			proj.specRigidbody.OnPreRigidbodyCollision = (SpeculativeRigidbody.OnPreRigidbodyCollisionDelegate)Delegate.Combine(proj.specRigidbody.OnPreRigidbodyCollision, new SpeculativeRigidbody.OnPreRigidbodyCollisionDelegate(this.HandlePreCollision));
			GameManager.Instance.StartCoroutine(this.ChangeProjectileDamage(proj));
		}

		private IEnumerator ChangeProjectileDamage(Projectile proj)
		{
			yield return new WaitForSeconds(1f);
			proj.baseData.damage *= 6f;
			if (this.gun.CurrentOwner is PlayerController)
			{
				proj.baseData.damage *= (this.gun.CurrentOwner as PlayerController).stats.GetStatValue(PlayerStats.StatType.Damage);
			}
			if (proj.IsBlackBullet)
			{
				proj.baseData.damage *= 2;
			}
			yield break;
		}

		private void HandlePreCollision(SpeculativeRigidbody myRigidbody, PixelCollider myPixelCollider, SpeculativeRigidbody otherRigidbody, PixelCollider otherPixelCollider)
		{
			bool flag = otherRigidbody && otherRigidbody.healthHaver && otherRigidbody.aiActor && otherRigidbody.aiActor.CompanionOwner;
			if (flag)
			{
				float damage = myRigidbody.projectile.baseData.damage;
				myRigidbody.projectile.baseData.damage = 0f;
				GameManager.Instance.StartCoroutine(Chainer.ChangeProjectileDamage(myRigidbody.projectile, damage));
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
			//This determines what sound you want to play when you fire a gun.
			//Sounds names are based on the Gungeon sound dump, which can be found at EnterTheGungeon/Etg_Data/StreamingAssets/Audio/GeneratedSoundBanks/Windows/sfx.txt
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
				if(roomWhereThisWasFired != null && gun.CurrentOwner is PlayerController && (gun.CurrentOwner as PlayerController).CurrentRoom != roomWhereThisWasFired)
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
		private List<AIActor> spawnedChainHolders = new List<AIActor>();
		private RoomHandler roomWhereThisWasFired = null;
		private float countTimesFiredInTimeLaps;
	}

	/// <summary>
	/// from ShotgunExecutionerChain1
	/// </summary>
	public class Chain1 : Script
	{
		protected override IEnumerator Top()
		{
			//this.EndOnBlank = true;
			Chain1.HandBullet handBullet = null;
			handBullet = this.FireVolley(0, (float)(20 + 5));
			while (!handBullet.IsEnded && !handBullet.HasStoppedGet())
			{
				yield return this.Wait(1);
			}
			yield return this.Wait(120);
			yield break;
		}

		private Chain1.HandBullet FireVolley(int volleyIndex, float speed)
		{
			Chain1.HandBullet handBullet = new Chain1.HandBullet(this);
			float aimAngle = Chainer.playerGunCurrentAngle;
			//base.Fire(new Direction(base.AimDirection, DirectionType.Absolute, -1f), new Speed(speed, SpeedType.Absolute), handBullet);
			base.Fire(new Direction(aimAngle, DirectionType.Absolute, -1f), new Speed(speed, SpeedType.Absolute), handBullet);
			for (int i = 0; i < 20; i++)
			{
				//base.Fire(new Direction(base.AimDirection, DirectionType.Absolute, -1f), new Chain1.ArmBullet(this, handBullet, i));
				base.Fire(new Direction(aimAngle, DirectionType.Absolute, -1f), new Chain1.ArmBullet(this, handBullet, i));
			}
			return handBullet;
		}

		private const int NumArmBullets = 20;

		private const int NumVolley = 3;

		private const int FramesBetweenVolleys = 30;

		private class ArmBullet : Bullet
		{
			public ArmBullet(Chain1 parentScript, Chain1.HandBullet handBullet, int index) : base("chain", false, false, false)
			{
				this.m_parentScript = parentScript;
				this.m_handBullet = handBullet;
				this.m_index = index;
			}

			protected override IEnumerator Top()
			{
				this.ManualControl = true;
				while (!this.m_parentScript.IsEnded && !this.m_handBullet.IsEnded && !this.m_handBullet.HasStoppedGet() && this.BulletBank)
				{
					this.Position = Vector2.Lerp(this.m_parentScript.Position, this.m_handBullet.Position, (float)this.m_index / 20f);
					yield return this.Wait(1);
				}
				if (this.m_parentScript.IsEnded)
				{
					this.Vanish(false);
					yield break;
				}
				int delay = 20 - this.m_index - 5;
				if (delay > 0)
				{
					yield return this.Wait(delay);
				}
				float currentOffset = 0f;
				Vector2 truePosition = this.Position;
				int halfWiggleTime = 10;
				for (int i = 0; i < 30; i++)
				{
					if (i == 0 && delay < 0)
					{
						i = -delay;
					}
					float magnitude = 0.4f;
					magnitude = Mathf.Min(magnitude, Mathf.Lerp(0.2f, 0.4f, (float)this.m_index / 8f));
					magnitude = Mathf.Min(magnitude, Mathf.Lerp(0.2f, 0.4f, (float)(20 - this.m_index - 1) / 3f));
					magnitude = Mathf.Lerp(magnitude, 0f, (float)i / (float)halfWiggleTime - 2f);
					currentOffset = Mathf.SmoothStep(-magnitude, magnitude, Mathf.PingPong(0.5f + (float)i / (float)halfWiggleTime, 1f));
					this.Position = truePosition + BraveMathCollege.DegreesToVector(this.Direction - 90f, currentOffset);
					yield return this.Wait(1);
				}
				yield return this.Wait(this.m_index + 1 + 1000);
				this.Vanish(false);
				yield break;
			}

			public const int BulletDelay = 60;

			private const float WiggleMagnitude = 0.4f;

			public const int WiggleTime = 30;

			private const int NumBulletsToPreShake = 5;

			private Chain1 m_parentScript;

			private Chain1 shotgunExecutionerChain1;

			private Chain1.HandBullet m_handBullet;

			private int m_index;
		}

		private class HandBullet : Bullet
		{
			public HandBullet(Chain1 parentScript) : base("chain", false, false, false)
			{
				this.m_parentScript = parentScript;
			}

			// ! WARNING: in vs 2019 the properties get set doesn't seem to work and trigger an error, the comment below show the fix
			//public bool HasStopped { get; set; }

			private bool has_stopped;
			public bool HasStoppedGet()
			{
				return this.has_stopped;
			}

			// to prevent the chain from disappearing
			public void HasStoppedSet(bool value)
			{
				//this.has_stopped = value;
			}

			protected override IEnumerator Top()
			{
				this.Projectile.BulletScriptSettings.surviveRigidbodyCollisions = true;
				this.Projectile.BulletScriptSettings.surviveTileCollisions = true;
				SpeculativeRigidbody specRigidbody = this.Projectile.specRigidbody;
				specRigidbody.OnCollision = (Action<CollisionData>)Delegate.Combine(specRigidbody.OnCollision, new Action<CollisionData>(this.OnCollision));
				while (!this.m_parentScript.IsEnded && !this.HasStoppedGet())
				{
					yield return this.Wait(1);
				}
				if (this.m_parentScript.IsEnded)
				{
					this.Vanish(false);
					yield break;
				}
				yield return this.Wait(111);
				this.Vanish(false);
				yield break;
			}

			private void OnCollision(CollisionData collision)
			{
				bool flag = collision.collisionType == CollisionData.CollisionType.TileMap;
				SpeculativeRigidbody otherRigidbody = collision.OtherRigidbody;
				if (otherRigidbody)
				{
					flag = (otherRigidbody.majorBreakable || otherRigidbody.PreventPiercing || (!otherRigidbody.gameActor && !otherRigidbody.minorBreakable));
				}
				if (flag)
				{
					base.Position = collision.MyRigidbody.UnitCenter + PhysicsEngine.PixelToUnit(collision.NewPixelsToMove);
					this.Speed = 0f;
					this.HasStoppedSet(true);
					PhysicsEngine.PostSliceVelocity = new Vector2?(new Vector2(0f, 0f));
					SpeculativeRigidbody specRigidbody = this.Projectile.specRigidbody;
					specRigidbody.OnCollision = (Action<CollisionData>)Delegate.Remove(specRigidbody.OnCollision, new Action<CollisionData>(this.OnCollision));
				}
				else
				{
					PhysicsEngine.PostSliceVelocity = new Vector2?(collision.MyRigidbody.Velocity);
				}
			}

			public override void OnBulletDestruction(Bullet.DestroyType destroyType, SpeculativeRigidbody hitRigidbody, bool preventSpawningProjectiles)
			{
				if (this.Projectile)
				{
					SpeculativeRigidbody specRigidbody = this.Projectile.specRigidbody;
					specRigidbody.OnCollision = (Action<CollisionData>)Delegate.Remove(specRigidbody.OnCollision, new Action<CollisionData>(this.OnCollision));
				}
				this.HasStoppedSet(true);
			}

			private Chain1 m_parentScript;
		}
	}


}
