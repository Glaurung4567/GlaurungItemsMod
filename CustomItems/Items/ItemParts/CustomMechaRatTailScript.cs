using System.Collections;
using Brave.BulletScript;
using UnityEngine;

namespace GlaurungItems.Items
{
	public class CustomMetalGearRatTailgun1 : Script
	{
		private bool Center;
		private bool Done;

		protected override IEnumerator Top()
		{
			this.EndOnBlank = true;
			CustomMetalGearRatTailgun1.TargetDummy targetDummy = new CustomMetalGearRatTailgun1.TargetDummy();
			targetDummy.Position = this.BulletBank.transform.position.GetAbsoluteRoom().area.UnitCenter + new Vector2(0f, 4.5f);
			targetDummy.Direction = this.AimDirection;
			targetDummy.BulletManager = this.BulletManager;
			targetDummy.Initialize();
			for (int j = 0; j < 16; j++)
			{
				float angle = this.SubdivideCircle(0f, 16, j, 1f, false);
				Vector2 overridePosition = targetDummy.Position + BraveMathCollege.DegreesToVector(angle, 0.75f);
				this.Fire(Offset.OverridePosition(overridePosition), new CustomMetalGearRatTailgun1.TargetBullet(this, targetDummy));
			}
			this.Fire(Offset.OverridePosition(targetDummy.Position), new CustomMetalGearRatTailgun1.TargetBullet(this, targetDummy));
			for (int k = 0; k < 4; k++)
			{
				float angle2 = (float)(k * 90);
				for (int l = 1; l < 4; l++)
				{
					float magnitude = 0.75f + Mathf.Lerp(0f, 0.625f, (float)l / 3f);
					Vector2 overridePosition2 = targetDummy.Position + BraveMathCollege.DegreesToVector(angle2, magnitude);
					this.Fire(Offset.OverridePosition(overridePosition2), new CustomMetalGearRatTailgun1.TargetBullet(this, targetDummy));
				}
			}
			for (int i = 0; i < 360; i++)
			{
				targetDummy.DoTick();
				yield return this.Wait(1);
			}
			this.Fire(Offset.OverridePosition(targetDummy.Position + new Vector2(0f, 30f)), new Direction(-90f, DirectionType.Absolute, -1f), new Speed(30f, SpeedType.Absolute), new CustomMetalGearRatTailgun1.BigBullet());
			this.PostWwiseEvent("Play_BOSS_RatMech_Whistle_01", null);
			this.Center = true;
			yield return this.Wait(60);
			this.Done = true;
			yield return this.Wait(60);
			yield break;
		}

		private const int NumTargetBullets = 16;
		private const float TargetRadius = 3f;
		private const float TargetLegLength = 2.5f;
		public const int TargetTrackTime = 360;
		private const float TargetRotationSpeed = 80f;
		private const int BigOneHeight = 30;
		private const int NumDeathWaves = 4;
		private const int NumDeathBullets = 39;

		public class TargetDummy : Bullet
		{
			public TargetDummy() : base(null, false, false, false)
			{
			}

			protected override IEnumerator Top()
			{
				for (; ; )
				{
					float distToTarget = (this.BulletManager.PlayerPosition() - this.Position).magnitude;
					if (this.Tick < 30)
					{
						this.Speed = 0f;
					}
					else
					{
						float a = Mathf.Lerp(12f, 4f, Mathf.InverseLerp(7f, 4f, distToTarget));
						this.Speed = Mathf.Min(a, (float)(this.Tick - 30) / 60f * 10f);
					}
					this.ChangeDirection(new Direction(0f, DirectionType.Aim, 3f), 1);
					yield return this.Wait(1);
				}
				yield break;
			}
		}

		public class TargetBullet : Bullet
		{
			public TargetBullet(CustomMetalGearRatTailgun1 parent, CustomMetalGearRatTailgun1.TargetDummy targetDummy) : base("target", false, false, false)
			{
				this.m_parent = parent;
				this.m_targetDummy = targetDummy;
			}

			protected override IEnumerator Top()
			{
				Vector2 toCenter = this.Position - this.m_targetDummy.Position;
				float angle = toCenter.ToAngle();
				float radius = toCenter.magnitude;
				float deltaRadius = radius / 60f;
				this.ManualControl = true;
				this.Projectile.specRigidbody.CollideWithTileMap = false;
				this.Projectile.specRigidbody.AddCollisionLayerIgnoreOverride(CollisionMask.LayerToMask(CollisionLayer.HighObstacle, CollisionLayer.LowObstacle));
				while (!this.m_parent.Destroyed && !this.m_parent.IsEnded && !this.m_parent.Done)
				{
					if (this.Tick < 60)
					{
						radius += deltaRadius * 3f;
					}
					if (this.m_parent.Center)
					{
						radius -= deltaRadius * 2f;
					}
					angle += 1.3333334f;
					this.Position = this.m_targetDummy.Position + BraveMathCollege.DegreesToVector(angle, radius);
					yield return this.Wait(1);
				}
				this.Vanish(false);
				this.PostWwiseEvent("Play_BOSS_RatMech_Bomb_01", null);
				yield break;
			}

			private CustomMetalGearRatTailgun1 m_parent;
			private CustomMetalGearRatTailgun1.TargetDummy m_targetDummy;
		}

		private class BigBullet : Bullet
		{
			public BigBullet() : base("big_one", false, false, false)
			{
			}

			public override void Initialize()
			{
				this.Projectile.spriteAnimator.StopAndResetFrameToDefault();
				base.Initialize();
			}

			protected override IEnumerator Top()
			{
				this.Projectile.specRigidbody.CollideWithTileMap = false;
				this.Projectile.specRigidbody.CollideWithOthers = false;
				yield return this.Wait(60);
				this.Speed = 0f;
				this.Projectile.spriteAnimator.Play();
				float startingAngle = this.RandomAngle();
				for (int i = 0; i < 4; i++)
				{
					bool flag = i % 2 == 0;
					for (int j = 0; j < 39; j++)
					{
						float startAngle = startingAngle;
						int numBullets = 39;
						int i2 = j;
						bool offset = flag;
						float direction = this.SubdivideCircle(startAngle, numBullets, i2, 1f, offset);
						this.Fire(new Direction(direction, DirectionType.Absolute, -1f), new Speed(0f, SpeedType.Absolute), new SpeedChangingBullet(10f, 17 * i, -1));
					}
				}
				yield return this.Wait(30);
				this.Vanish(true);
				yield break;
			}

			public override void OnBulletDestruction(Bullet.DestroyType destroyType, SpeculativeRigidbody hitRigidbody, bool preventSpawningProjectiles)
			{
				if (preventSpawningProjectiles)
				{
					return;
				}
			}
		}
	}
}