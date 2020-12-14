using Brave.BulletScript;
using System;
using System.Collections;
using UnityEngine;

namespace GlaurungItems.Items
{
    class BulletScriptGunShotgrubManAttack1 : Script
	{
		protected override IEnumerator Top()
		{
			float num = -22.5f;
			float num2 = 9f;
			for (int i = 0; i < 5; i++)
			{
				base.Fire(new Direction(GunjuringEncyclopedia.playerGunCurrentAngle, DirectionType.Absolute, -1f), new Speed(9f, SpeedType.Absolute), new BulletScriptGunShotgrubManAttack1.GrossBullet(num + (float)i * num2));
			}
			return null;
		}

		private const int NumBullets = 5;

		private const float Spread = 45f;

		private const int NumDeathBullets = 6;

		private const float GrubMagnitude = 0.75f;

		private const float GrubPeriod = 3f;

		public class GrossBullet : Bullet
		{
			public GrossBullet(float deltaAngle) : base("gross", false, false, false)
			{
				this.deltaAngle = deltaAngle;
			}

			protected override IEnumerator Top()
			{
				yield return this.Wait(20);
				this.Direction += this.deltaAngle;
				this.Speed += UnityEngine.Random.Range(-1f, 1f);
				yield break;
			}

			public override void OnBulletDestruction(Bullet.DestroyType destroyType, SpeculativeRigidbody hitRigidbody, bool preventSpawningProjectiles)
			{
				try
				{
					if (preventSpawningProjectiles)
					{
						return;
					}
					float num = base.RandomAngle();
					float num2 = 60f;
					for (int i = 0; i < 6; i++)
					{
						base.Fire(new Direction(num + num2 * (float)i, DirectionType.Absolute, -1f), new Speed(8f, SpeedType.Absolute), new BulletScriptGunShotgrubManAttack1.GrubBullet());
					}
				}
				catch (Exception e)
				{
					//Tools.PrintException(e);
					//to sweep sweep sweep the error triggered when the aiactor is erased, a gross bullet hit a wall and cutting room floor is installed too
				}
			}

			private float deltaAngle;
		}

		public class GrubBullet : Bullet
		{
			public GrubBullet() : base(null, false, false, false)
			{
				base.SuppressVfx = true;
			}

			protected override IEnumerator Top()
			{
				this.ManualControl = true;
				Vector2 truePosition = this.Position;
				float startVal = UnityEngine.Random.value;
				for (int i = 0; i < 360; i++)
				{
					float offsetMagnitude = Mathf.SmoothStep(-0.75f, 0.75f, Mathf.PingPong(startVal + (float)i / 60f * 3f, 1f));
					truePosition += BraveMathCollege.DegreesToVector(this.Direction, this.Speed / 60f);
					this.Position = truePosition + BraveMathCollege.DegreesToVector(this.Direction - 90f, offsetMagnitude);
					yield return this.Wait(1);
				}
				this.Vanish(false);
				yield break;
			}
		}
	}

}
