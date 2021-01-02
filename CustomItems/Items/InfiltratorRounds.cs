using Brave.BulletScript;
using EnemyAPI;
using ItemAPI;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GlaurungItems.Items
{
	class InfiltratorRounds : PlayerItem
	{
		public static void Init()
		{
            try
            {
			string text = "Infiltrator Rounds";
			string resourcePath = "GlaurungItems/Resources/acme_crate";
			GameObject gameObject = new GameObject(text);
			InfiltratorRounds item = gameObject.AddComponent<InfiltratorRounds>();
			ItemBuilder.AddSpriteToObject(text, resourcePath, gameObject);
			string shortDesc = "Misuse turned Feature";
			string longDesc = "";
			item.SetupItem(shortDesc, longDesc, "gl");
			item.SetCooldownType(ItemBuilder.CooldownType.Damage, 100f);
			item.quality = ItemQuality.D;
			}
            catch (Exception e)
			{
				Tools.PrintException(e);
			}
		}

        public override bool CanBeUsed(PlayerController user)
        {
            return user && user.CurrentGun != null && base.CanBeUsed(user);
        }

        protected override void DoEffect(PlayerController user)
		{	
			GameObject gameObject = new GameObject();
			gameObject.transform.position = user.AimCenter;
			//(user.CurrentGun == null) ? 0f : user.CurrentGun.CurrentAngle)
			BulletScriptSource source = gameObject.GetOrAddComponent<BulletScriptSource>();
			gameObject.AddComponent<BulletSourceKiller>();

			AIBulletBank bulletBank = Toolbox.CopyAIBulletBank(EnemyDatabase.GetOrLoadByGuid(EnemyGuidDatabase.Entries["blue_shotgun_kin"]).bulletBank);//to prevent our gun from affecting the bulletbank of the enemy
			bulletBank.OnProjectileCreated = (Action<Projectile>)Delegate.Combine(bulletBank.OnProjectileCreated, new Action<Projectile>(this.OnProjCreated));
			bulletBank.CollidesWithEnemies = true;
			source.BulletManager = bulletBank;

			var bulletScriptSelected = new CustomBulletScriptSelector(typeof(BulletkinMagnumScript));
			source.BulletScript = bulletScriptSelected;

			InfiltratorRounds.playerGunCurrentAngle = (user.CurrentGun == null) ? 0f : user.CurrentGun.CurrentAngle;
			
			source.Initialize();//to fire the script once
			
			/*---------------------------------------------------*/

			/*string enemyGuid = EnemyGuidDatabase.Entries["bullet_kin"];
			AIActor orLoadByGuid = EnemyDatabase.GetOrLoadByGuid(enemyGuid);

			InfiltratorRounds.playerGunCurrentAngle = (user.CurrentGun == null) ? 0f : user.CurrentGun.CurrentAngle;
			Vector2 positionVector = user.sprite.WorldBottomCenter;
			AIActor aiactor = AIActor.Spawn(orLoadByGuid.aiActor, positionVector, GameManager.Instance.Dungeon.data.GetAbsoluteRoomFromPosition(positionVector.ToIntVector2()), true, AIActor.AwakenAnimationType.Default, true);

			aiactor.bulletBank.Bullets = Toolbox.CopyAIBulletBank(EnemyDatabase.GetOrLoadByGuid(EnemyGuidDatabase.Entries["bullet_kin"]).bulletBank).Bullets;
			aiactor.bulletBank.CollidesWithEnemies = true;

			// to prevent the aiActor from moving
			aiactor.behaviorSpeculator.MovementBehaviors = EnemyDatabase.GetOrLoadByGuid("b08ec82bef6940328c7ecd9ffc6bd16c").behaviorSpeculator.MovementBehaviors;
			aiactor.behaviorSpeculator.TargetBehaviors = EnemyDatabase.GetOrLoadByGuid("b08ec82bef6940328c7ecd9ffc6bd16c").behaviorSpeculator.TargetBehaviors;
			aiactor.behaviorSpeculator.OtherBehaviors = EnemyDatabase.GetOrLoadByGuid("b08ec82bef6940328c7ecd9ffc6bd16c").behaviorSpeculator.OtherBehaviors;
			aiactor.behaviorSpeculator.AttackBehaviors = EnemyDatabase.GetOrLoadByGuid("b08ec82bef6940328c7ecd9ffc6bd16c").behaviorSpeculator.AttackBehaviors;
			//EnemyAPI.EnemyAPITools.DebugInformation(aiactor.behaviorSpeculator); //EnemyGuidDatabase.Entries["bullet_kin"]

			aiactor.sprite.renderer.enabled = false; // to make the companion invisible
			aiactor.aiShooter.ToggleGunAndHandRenderers(false, "poyo enemy with invisible gun");
			aiactor.procedurallyOutlined = false;
			aiactor.CorpseObject = null;
			aiactor.ImmuneToAllEffects = true;
			aiactor.behaviorSpeculator.ImmuneToStun = true;
			aiactor.SetIsFlying(true, "I'm a bullet too!");
			aiactor.ToggleShadowVisiblity(false);
			aiactor.HasShadow = false;

			aiactor.CanTargetEnemies = false;
			aiactor.CanTargetPlayers = false;
			aiactor.CompanionOwner = user;
			aiactor.HitByEnemyBullets = false;
			PhysicsEngine.Instance.RegisterOverlappingGhostCollisionExceptions(aiactor.specRigidbody, null, false);

			aiactor.IsHarmlessEnemy = true;
			aiactor.IgnoreForRoomClear = true;
			aiactor.PreventAutoKillOnBossDeath = true;

			aiactor.knockbackDoer.SetImmobile(true, "Infiltration"); // from the TetherBehavior to prevent the companion from being pushed by explosions
			aiactor.PreventFallingInPitsEver = true;

			aiactor.gameObject.AddComponent<CompanionController>();
			CompanionController component = aiactor.gameObject.GetComponent<CompanionController>();
			component.CanInterceptBullets = false;
			component.Initialize(user);

			if (aiactor.healthHaver != null)
			{
				aiactor.healthHaver.PreventAllDamage = true;
				//aiactor.OverrideBuffEffectPosition
			}

			if (aiactor.bulletBank != null)
			{
				AIBulletBank bulletBank = aiactor.bulletBank;
				bulletBank.OnProjectileCreated = (Action<Projectile>)Delegate.Combine(bulletBank.OnProjectileCreated, new Action<Projectile>(this.OnProjCreated));
			}
			if (aiactor.aiShooter != null)
			{
				AIShooter aiShooter = aiactor.aiShooter;
				aiShooter.PostProcessProjectile = (Action<Projectile>)Delegate.Combine(aiShooter.PostProcessProjectile, new Action<Projectile>(this.OnProjCreated));
			}

			aiactor.aiShooter.AimAtPoint(user.AimCenter);
			aiactor.aiShooter.ShootBulletScript(new CustomBulletScriptSelector(typeof(BulletkinMagnumScript)));
			GameManager.Instance.StartCoroutine(this.RemoveCompanion(aiactor));*/
		}
		/*
        private IEnumerator RemoveCompanion(AIActor aiactor)
        {
			yield return new WaitForSeconds(0.1f);
			aiactor.EraseFromExistence(true);
			yield break;
		}
		*/

        protected void OnProjCreated(Projectile projectile)
		{
			if (projectile)
			{
				projectile.TreatedAsNonProjectileForChallenge = true;
				if (this.LastOwner)
				{
					projectile.Owner = this.LastOwner;
					this.LastOwner.DoPostProcessProjectile(projectile);
					projectile.AdjustPlayerProjectileTint(Color.grey, 0);
					projectile.specRigidbody.AddCollisionLayerOverride(CollisionMask.LayerToMask(CollisionLayer.EnemyHitBox, CollisionLayer.EnemyBulletBlocker));
					projectile.specRigidbody.AddCollisionLayerIgnoreOverride(CollisionMask.LayerToMask(CollisionLayer.PlayerHitBox, CollisionLayer.PlayerBlocker, CollisionLayer.PlayerCollider));
					projectile.collidesWithPlayer = false;
					projectile.collidesWithEnemies = true;
					projectile.baseData.damage *= 10;
					projectile.UpdateCollisionMask();
					//projectile.collidesWithPlayer = true; //doesn't seem to work
				}
			}
		}

		public static float playerGunCurrentAngle = 0f;
	}

	public class BulletkinMagnumScript : Script
	{
		protected override IEnumerator Top()
		{
			float aimDirection = InfiltratorRounds.playerGunCurrentAngle;
			this.Fire(new Direction(aimDirection, DirectionType.Absolute, -1f), new Speed(6f, SpeedType.Absolute), null);
			yield return this.Wait(40);
			yield break;
		}
	}
}
