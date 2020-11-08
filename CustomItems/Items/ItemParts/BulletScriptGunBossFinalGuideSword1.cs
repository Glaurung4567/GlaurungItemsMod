using Brave.BulletScript;
using System.Collections;
using UnityEngine;

namespace GlaurungItems.Items
{
	class BulletScriptGunBossFinalGuideSword1 : Script
	{
		protected override IEnumerator Top()
		{
			this.EndOnBlank = true;
			this.m_sign = BraveUtility.RandomSign();
			this.m_doubleSwing = true;// BraveUtility.RandomBool();
			Vector2 leftOrigin = new Vector2(this.m_sign * 2f, -0.2f);
			this.FireLine(leftOrigin, new Vector2(this.m_sign * 3.8f, 0.2f), new Vector2(this.m_sign * 11f, 0.2f), 14);
			this.FireLine(leftOrigin, new Vector2(this.m_sign * 11.6f, -0.2f), new Vector2(this.m_sign * 11.6f, -0.2f), 2);
			this.FireLine(leftOrigin, new Vector2(this.m_sign * 3.8f, -0.6f), new Vector2(this.m_sign * 11f, -0.6f), 14);
			this.FireLine(leftOrigin, new Vector2(this.m_sign * 3.1f, -1.2f), new Vector2(this.m_sign * 3.1f, 0.8f), 4);
			this.FireLine(leftOrigin, new Vector2(this.m_sign * 2.2f, -0.2f), new Vector2(this.m_sign * 2.7f, -0.2f), 2);
			yield return this.Wait(75);
			yield break;
		}

		private void FireLine(Vector2 spawnPoint, Vector2 start, Vector2 end, int numBullets)
		{
			Vector2 a = (end - start) / (float)Mathf.Max(1, numBullets - 1);
			float num = 0.333333343f;
			for (int i = 0; i < numBullets; i++)
			{
				Vector2 a2 = (numBullets != 1) ? (start + a * (float)i) : end;
				float speed = Vector2.Distance(a2, spawnPoint) / num;
				base.Fire(new Offset(spawnPoint, 0f, string.Empty, DirectionType.Absolute), new Direction((a2 - spawnPoint).ToAngle(), DirectionType.Absolute, -1f), new Speed(speed, SpeedType.Absolute), new BulletScriptGunBossFinalGuideSword1.SwordBullet(base.Position, this.m_sign, this.m_doubleSwing));
			}
		}

		private const int SetupTime = 20;

		private const int HoldTime = 30;

		private const int SwingTime = 25;

		private float m_sign;

		private bool m_doubleSwing;

		public class SwordBullet : Bullet
		{
			public SwordBullet(Vector2 origin, float sign, bool doubleSwing) : base(null, false, false, false)
			{
				this.m_origin = origin;
				this.m_sign = sign;
				this.m_doubleSwing = doubleSwing;
			}

			protected override IEnumerator Top()
			{
				this.Projectile.PenetratesInternalWalls = true;
				this.Projectile.pierceMinorBreakables = true;
				this.Projectile.specRigidbody.CollideWithTileMap = false;
				this.Projectile.BulletScriptSettings.surviveRigidbodyCollisions = true;
				yield return this.Wait(20);
				float angle = (this.Position - this.m_origin).ToAngle();
				float dist = Vector2.Distance(this.Position, this.m_origin);
				this.Speed = 0f;
				yield return this.Wait(30);
				this.ManualControl = true;
				int swingtime = (!this.m_doubleSwing) ? 25 : 100;
				float swingDegrees = (float)((!this.m_doubleSwing) ? 180 : 540);
				for (int i = 0; i < swingtime; i++)
				{
					float newAngle = angle - Mathf.SmoothStep(0f, this.m_sign * swingDegrees, (float)i / (float)swingtime);
					this.Position = this.m_origin + BraveMathCollege.DegreesToVector(newAngle, dist);
					yield return this.Wait(1);
				}
				yield return this.Wait(5);
				this.Vanish(false);
				yield break;
			}

			private Vector2 m_origin;

			private float m_sign;

			private bool m_doubleSwing;
		}
	}
}
