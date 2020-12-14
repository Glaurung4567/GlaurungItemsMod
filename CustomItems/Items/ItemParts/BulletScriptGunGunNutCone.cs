using Brave.BulletScript;
using System.Collections;
using UnityEngine;

namespace GlaurungItems.Items
{
    class BulletScriptGunGunNutCone: Script
	{
		// Token: 0x060006B6 RID: 1718 RVA: 0x0001F250 File Offset: 0x0001D450
		protected override IEnumerator Top()
		{
			//this.FireCluster(this.Direction);
			this.FireCluster(GunjuringEncyclopedia.playerGunCurrentAngle);
			yield return this.Wait(10);
			for (int i = 0; i < 25; i++)
			{
				float num = -45f + (float)i * 3.75f;
				//this.Fire(new Offset(0.5f, 0f, this.Direction + num, string.Empty, DirectionType.Absolute), new Direction(num, DirectionType.Relative, -1f), new Speed(10f, SpeedType.Absolute), null);
				this.Fire(new Offset(0.5f, 0f, GunjuringEncyclopedia.playerGunCurrentAngle + num, string.Empty, DirectionType.Absolute), new Direction(GunjuringEncyclopedia.playerGunCurrentAngle + num, DirectionType.Absolute, -1f), new Speed(10f, SpeedType.Absolute), null);
			}
			//this.FireCluster(this.Direction - 45f);
			this.FireCluster(GunjuringEncyclopedia.playerGunCurrentAngle - 45f);
			//this.FireCluster(this.Direction + 45f);
			this.FireCluster(GunjuringEncyclopedia.playerGunCurrentAngle + 45f);
			yield return this.Wait(10);
			//this.FireCluster(this.Direction);
			this.FireCluster(GunjuringEncyclopedia.playerGunCurrentAngle);
			yield break;
		}

		// Token: 0x060006B7 RID: 1719 RVA: 0x0001F26C File Offset: 0x0001D46C
		private void FireCluster(float direction)
		{
			base.Fire(new Offset(0.5f, 0f, direction, string.Empty, DirectionType.Absolute), new Direction(direction, DirectionType.Absolute, -1f), new Speed(12f, SpeedType.Absolute), null);
			base.Fire(new Offset(0.275f, 0.25f, direction, string.Empty, DirectionType.Absolute), new Direction(direction, DirectionType.Absolute, -1f), new Speed(12f, SpeedType.Absolute), null);
			base.Fire(new Offset(0.275f, -0.25f, direction, string.Empty, DirectionType.Absolute), new Direction(direction, DirectionType.Absolute, -1f), new Speed(12f, SpeedType.Absolute), null);
			base.Fire(new Offset(0f, 0.4f, direction, string.Empty, DirectionType.Absolute), new Direction(direction, DirectionType.Absolute, -1f), new Speed(12f, SpeedType.Absolute), null);
			base.Fire(new Offset(0f, -0.4f, direction, string.Empty, DirectionType.Absolute), new Direction(direction, DirectionType.Absolute, -1f), new Speed(12f, SpeedType.Absolute), null);
		}

		// Token: 0x04000691 RID: 1681
		private const int NumBulletsMainWave = 25;
	}
}
