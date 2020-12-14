using Brave.BulletScript;
using System.Collections;
using UnityEngine;

namespace GlaurungItems.Items
{
    class BulletScriptGunTankTreaderScatterShot1 : Script
	{
		protected override IEnumerator Top()
		{
			base.Fire(new Direction(GunjuringEncyclopedia.playerGunCurrentAngle, DirectionType.Absolute, -1f), new Speed(12f, SpeedType.Absolute), new BulletScriptGunTankTreaderScatterShot1.ScatterBullet());
			return null;
		}

		private const int AirTime = 30;

		private const int NumDeathBullets = 16;

		private class ScatterBullet : Bullet
		{
			public ScatterBullet() : base("scatterBullet", false, false, false)
			{
			}

			protected override IEnumerator Top()
			{
				yield return this.Wait(30);
				for (int i = 0; i < 16; i++)
				{
					this.Fire(new Direction((float)UnityEngine.Random.Range(-35, 35), DirectionType.Relative, -1f), new Speed((float)UnityEngine.Random.Range(3, 12), SpeedType.Absolute), new BulletScriptGunTankTreaderScatterShot1.LittleScatterBullet());
				}
				this.Vanish(false);
				yield break;
			}
		}

		private class LittleScatterBullet : Bullet
		{
			public LittleScatterBullet() : base(null, false, false, false)
			{
			}

			protected override IEnumerator Top()
			{
				this.ChangeSpeed(new Speed(12f, SpeedType.Absolute), 40);
				yield return this.Wait(300);
				this.Vanish(false);
				yield break;
			}
		}
	}

}
