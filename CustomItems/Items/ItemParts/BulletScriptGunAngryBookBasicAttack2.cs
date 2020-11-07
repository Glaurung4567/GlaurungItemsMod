using Brave.BulletScript;
using System.Collections;
using UnityEngine;

namespace GlaurungItems.Items
{
	class BulletScriptGunAngryBookBasicAttack2 : Script
	{
		public BulletScriptGunAngryBookBasicAttack2()
		{
			this.LineBullets = 10;
			//base..ctor();
		}

		protected override IEnumerator Top()
		{
			this.EndOnBlank = true;
			int count = 0;
			float xOffset = 1.9f / (float)(this.LineBullets - 1);
			float yOffset = 2.5f / (float)(this.LineBullets - 1);
			for (int i = 0; i < this.LineBullets; i++)
			{
				Offset offset = new Offset(-0.95f, -1.25f + yOffset * (float)i, 0f, string.Empty, DirectionType.Absolute);
				int spawnTime;
				count = (spawnTime = count) + 1;
				this.Fire(offset, new BulletScriptGunAngryBookBasicAttack2.DefaultBullet(spawnTime));
				yield return this.Wait(1);
			}
			for (int j = 0; j < this.LineBullets; j++)
			{
				Offset offset2 = new Offset(-0.95f + xOffset * (float)j, 1.25f - yOffset * (float)j, 0f, string.Empty, DirectionType.Absolute);
				int spawnTime;
				count = (spawnTime = count) + 1;
				this.Fire(offset2, new BulletScriptGunAngryBookBasicAttack2.DefaultBullet(spawnTime));
				yield return this.Wait(1);
			}
			for (int k = 0; k < this.LineBullets; k++)
			{
				Offset offset3 = new Offset(0.95f, -1.25f + yOffset * (float)k, 0f, string.Empty, DirectionType.Absolute);
				int spawnTime;
				count = (spawnTime = count) + 1;
				this.Fire(offset3, new BulletScriptGunAngryBookBasicAttack2.DefaultBullet(spawnTime));
				yield return this.Wait(1);
			}
			yield break;
		}

		public int LineBullets;

		public const float Height = 2.5f;

		public const float Width = 1.9f;

		public class DefaultBullet : Bullet
		{
			public DefaultBullet(int spawnTime) : base(null, false, false, false)
			{
				this.spawnTime = spawnTime;
			}

			protected override IEnumerator Top()
			{
				yield return this.Wait(45 - this.spawnTime);
				//this.ChangeDirection(new Direction(0f, DirectionType.Aim, -1f), 1);
				this.ChangeDirection(new Direction(BulletScriptGun.playerGunCurrentAngle, DirectionType.Absolute, -1f), 1);
				this.ChangeSpeed(new Speed(12f, SpeedType.Absolute), 1);
				yield break;
			}

			public int spawnTime;
		}
	}
}
