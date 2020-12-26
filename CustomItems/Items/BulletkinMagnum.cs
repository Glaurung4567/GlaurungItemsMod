using Brave.BulletScript;
using EnemyAPI;
using ItemAPI;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GlaurungItems.Items
{
	class BulletkinMagnum : PlayerItem
	{
		public static void Init()
		{
			string text = "Bulletkin Magnum";
			string resourcePath = "GlaurungItems/Resources/acme_crate";
			GameObject gameObject = new GameObject(text);
			BulletkinMagnum item = gameObject.AddComponent<BulletkinMagnum>();
			ItemBuilder.AddSpriteToObject(text, resourcePath, gameObject);
			string shortDesc = "Joys of Pooling";
			string longDesc = "";
			item.SetupItem(shortDesc, longDesc, "gl");
			item.SetCooldownType(ItemBuilder.CooldownType.Damage, 100f);
			item.quality = ItemQuality.D;
		}

		protected override void DoEffect(PlayerController user)
		{
			GameObject gameObject = new GameObject();
			gameObject.transform.position = user.CenterPosition;
			//(user.CurrentGun == null) ? 0f : user.CurrentGun.CurrentAngle)
			BulletScriptSource source = gameObject.GetOrAddComponent<BulletScriptSource>();
			gameObject.AddComponent<BulletSourceKiller>();

			AIBulletBank bulletBank = Toolbox.CopyAIBulletBank(EnemyDatabase.GetOrLoadByGuid(EnemyGuidDatabase.Entries["blue_shotgun_kin"]).bulletBank);//to prevent our gun from affecting the bulletbank of the enemy
			bulletBank.OnProjectileCreated = (Action<Projectile>)Delegate.Combine(bulletBank.OnProjectileCreated, new Action<Projectile>(this.OnProjCreated));
			bulletBank.CollidesWithEnemies = true;
			source.BulletManager = bulletBank;

			var bulletScriptSelected = new CustomBulletScriptSelector(typeof(BulletkinMagnumScript));
			source.BulletScript = bulletScriptSelected;

			BulletkinMagnum.playerGunCurrentAngle = (user.CurrentGun == null) ? 0f : user.CurrentGun.CurrentAngle;
			
			source.Initialize();//to fire the script once
		}

		protected void OnProjCreated(Projectile projectile)
		{
			if (projectile)
			{
				projectile.collidesWithPlayer = false;
				projectile.collidesWithEnemies = true;
				if (this.LastOwner)
				{
					projectile.Owner = this.LastOwner;
					this.LastOwner.DoPostProcessProjectile(projectile);
				}
			}
		}

		public static float playerGunCurrentAngle = 0f;

	}

	public class BulletkinMagnumScript : Script
	{
		protected override IEnumerator Top()
		{
			float aimDirection = BulletkinMagnum.playerGunCurrentAngle;
			this.Fire(new Direction(aimDirection, DirectionType.Absolute, -1f), new Speed(5f, SpeedType.Absolute), null);
			yield return this.Wait(40);
			yield break;
		}
	}
}
