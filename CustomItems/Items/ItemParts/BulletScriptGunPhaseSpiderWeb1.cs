using Brave.BulletScript;
using System.Collections;
using UnityEngine;

namespace GlaurungItems.Items
{
    class BulletScriptGunPhaseSpiderWeb1 : Script
	{
		protected override IEnumerator Top()
		{
			float startDirection = BulletScriptGun.playerGunCurrentAngle - 60f;
			for (int i = 0; i < 7; i++)
			{
				int baseDelay = i * 7;
				if (i % 3 == 1)
				{
					for (int j = 0; j < 13; j++)
					{
						float num = 9.230769f;
						int num2 = 0;
						if (j % 4 == 1 || j % 4 == 3)
						{
							num2 = 3;
						}
						if (j % 4 == 2)
						{
							num2 = 5;
						}
						this.Fire(new Direction(startDirection + (float)j * num, DirectionType.Absolute, -1f), new Speed(9f, SpeedType.Absolute), new BulletScriptGunPhaseSpiderWeb1.WebBullet(baseDelay + num2, false));
					}
				}
				else
				{
					for (int k = 0; k < 13; k++)
					{
						float num3 = 9.230769f;
						if (k % 4 == 0)
						{
							this.Fire(new Direction(startDirection + (float)k * num3, DirectionType.Absolute, -1f), new Speed(9f, SpeedType.Absolute), new BulletScriptGunPhaseSpiderWeb1.WebBullet(baseDelay, false));//i == 0));
						}
					}
				}
				yield return this.Wait(3);
			}
			yield break;
		}

		private const int NumWaves = 7;

		private const int BulletsPerWave = 13;

		private const float WebDegrees = 120f;

		private const float BulletSpeed = 9f;

		private class WebBullet : Bullet
		{
			public WebBullet(int delayFrames, bool spawnGoop = false) : base((!spawnGoop) ? "default" : "web", false, false, false)
			{
				this.m_delayFrames = delayFrames;
				this.m_spawnGoop = spawnGoop;
			}

			protected override IEnumerator Top()
			{
				if (this.m_delayFrames == 0)
				{
					yield break;
				}
				float speed = this.Speed;
				this.Speed = 0f;
				yield return this.Wait(this.m_delayFrames);
				this.Speed = speed;
				yield break;
			}

			public override void OnBulletDestruction(Bullet.DestroyType destroyType, SpeculativeRigidbody hitRigidbody, bool preventSpawningProjectiles)
			{
				if (this.m_spawnGoop && destroyType != Bullet.DestroyType.DieInAir && base.BulletBank)
				{
					GoopDoer component = base.BulletBank.GetComponent<GoopDoer>();
					DeadlyDeadlyGoopManager.GetGoopManagerForGoopType(component.goopDefinition).AddGoopCircle(base.Position, 1.5f, -1, false, -1);
				}
				base.OnBulletDestruction(destroyType, hitRigidbody, preventSpawningProjectiles);
			}

			private int m_delayFrames;

			private bool m_spawnGoop;
		}
	}

}
