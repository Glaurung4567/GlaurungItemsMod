using Brave.BulletScript;
using System.Collections;
using UnityEngine;

namespace GlaurungItems.Items
{
    class BulletScriptGunBirdEggVomit2 : Script
	{
		protected override IEnumerator Top()
		{
			float num = BraveMathCollege.ClampAngle360(BulletScriptGun.playerGunCurrentAngle);
			float direction = (float)((num <= 90f || num > 180f) ? 20 : 160);
			base.Fire(new Direction(direction, DirectionType.Absolute, -1f), new Speed(2f, SpeedType.Absolute), new BulletScriptGunBirdEggVomit2.EggBullet());
			return null;
		}

		private const int ClusterBullets = 0;

		private const float ClusterRotation = 150f;

		private const float ClusterRadius = 0.5f;

		private const float ClusterRadiusSpeed = 2f;

		private const int InnerBullets = 12;

		private const int InnerBounceTime = 30;

		public class EggBullet : Bullet
		{
			public EggBullet() : base("egg", false, false, false)
			{
			}

			protected override IEnumerator Top()
			{
				this.Projectile.sprite.SetSprite("egg_projectile_001");
				float startRotation = (float)((BulletScriptGun.playerGunCurrentAngle <= 90f || BulletScriptGun.playerGunCurrentAngle >= 270f) ? 135 : -135);
				for (int i = 0; i < 45; i++)
				{
					Vector2 velocity = BraveMathCollege.DegreesToVector(this.Direction, this.Speed);
					velocity += new Vector2(0f, -7f) / 60f;
					this.Direction = velocity.ToAngle();
					this.Speed = velocity.magnitude;
					this.Projectile.transform.rotation = Quaternion.Euler(0f, 0f, Mathf.Lerp(startRotation, 0f, (float)i / 45f));
					yield return this.Wait(1);
				}
				this.Projectile.transform.rotation = Quaternion.identity;
				this.Speed = 0f;
				this.Projectile.spriteAnimator.Play();
				int animTime = Mathf.RoundToInt(this.Projectile.spriteAnimator.DefaultClip.BaseClipLength * 60f);
				yield return this.Wait(animTime / 2);
				if (!this.spawnedBursts)
				{
					this.SpawnBursts();
				}
				yield return this.Wait(animTime / 2);
				this.Vanish(false);
				yield break;
			}

			public override void OnBulletDestruction(Bullet.DestroyType destroyType, SpeculativeRigidbody hitRigidbody, bool preventSpawningProjectiles)
			{
				if (!this.spawnedBursts && !preventSpawningProjectiles)
				{
					this.SpawnBursts();
				}
			}

			private void SpawnBursts()
			{
				float positiveInfinity = float.PositiveInfinity;
				for (int i = 0; i < 0; i++)
				{
					base.Fire(new Direction(BulletScriptGun.playerGunCurrentAngle, DirectionType.Absolute, -1f), new Speed(9f, SpeedType.Absolute), new BulletScriptGunBirdEggVomit2.ClusterBullet((float)i * positiveInfinity));
				}
				for (int j = 0; j < 12; j++)
				{
					base.Fire(new Direction(BulletScriptGun.playerGunCurrentAngle, DirectionType.Absolute, -1f), new Speed(9f, SpeedType.Absolute), new BulletScriptGunBirdEggVomit2.InnerBullet());
				}
				this.spawnedBursts = true;
			}

			private bool spawnedBursts;
		}

		public class ClusterBullet : Bullet
		{
			public ClusterBullet(float clusterAngle) : base(null, false, false, false)
			{
				this.clusterAngle = clusterAngle;
			}

			protected override IEnumerator Top()
			{
				this.ManualControl = true;
				Vector2 centerPosition = this.Position;
				float radius = 0.5f;
				for (int i = 0; i < 180; i++)
				{
					this.UpdateVelocity();
					centerPosition += this.Velocity / 60f;
					radius += 0.0333333351f;
					this.clusterAngle += 2.5f;
					this.Position = centerPosition + BraveMathCollege.DegreesToVector(this.clusterAngle, radius);
					yield return this.Wait(1);
				}
				this.Vanish(false);
				yield break;
			}

			private float clusterAngle;
		}

		public class InnerBullet : Bullet
		{
			public InnerBullet() : base(null, false, false, false)
			{
			}

			protected override IEnumerator Top()
			{
				this.ManualControl = true;
				Vector2 centerPosition = this.Position;
				float radius = 0.5f;
				int bounceDelay = UnityEngine.Random.Range(0, 30);
				Vector2 startOffset = BraveMathCollege.DegreesToVector(this.RandomAngle(), UnityEngine.Random.Range(0f, radius));
				float goalAngle = this.RandomAngle();
				float goalRadiusPercent = UnityEngine.Random.value;
				for (int i = 0; i < 180; i++)
				{
					this.UpdateVelocity();
					centerPosition += this.Velocity / 60f;
					radius += 0.0333333351f;
					Vector2 goalOffset = BraveMathCollege.DegreesToVector(goalAngle, goalRadiusPercent * radius);
					if (bounceDelay == 0)
					{
						startOffset = goalOffset;
						goalAngle = this.RandomAngle();
						goalRadiusPercent = UnityEngine.Random.value;
						goalOffset = BraveMathCollege.DegreesToVector(goalAngle, goalRadiusPercent * radius);
						bounceDelay = 30;
						if (radius > 1f)
						{
							bounceDelay = Mathf.RoundToInt(radius * (float)bounceDelay);
						}
					}
					else
					{
						bounceDelay--;
					}
					this.Position = centerPosition + Vector2.Lerp(startOffset, goalOffset, 1f - (float)bounceDelay / 30f);
					yield return this.Wait(1);
				}
				this.Vanish(false);
				yield break;
			}
		}
	}


}
