using Brave.BulletScript;
using System.Collections;
using UnityEngine;


namespace GlaurungItems.Items
{
	public class BulletScriptGunChancebulonBouncingRing1 : Script
	{
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
}
