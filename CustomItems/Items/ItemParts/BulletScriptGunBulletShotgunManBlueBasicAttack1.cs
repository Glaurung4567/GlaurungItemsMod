using Brave.BulletScript;
using System.Collections;
using UnityEngine;

namespace GlaurungItems.Items
{
	public class BulletScriptGunBulletShotgunManBlueBasicAttack1 : Script
	{
		protected override IEnumerator Top()
		{
			float aimDirection = GunjuringEncyclopedia.playerGunCurrentAngle;//this.AimDirection;
			for (int i = -2; i <= 2; i++)
			{
				this.Fire(new Direction((float)(i * 20) + aimDirection, DirectionType.Absolute, -1f), new Speed(5f, SpeedType.Absolute), null);
			}
			yield return this.Wait(40);
			if (this.BulletBank && this.BulletBank.behaviorSpeculator.IsStunned)
			{
				yield break;
			}
			if (BraveMathCollege.AbsAngleBetween(this.AimDirection, aimDirection) > 30f)
			{
				aimDirection = GunjuringEncyclopedia.playerGunCurrentAngle;//this.AimDirection;
			}
			for (float num = -1.5f; num <= 1.5f; num += 1f)
			{
				this.Fire(new Direction(num * 20f + aimDirection, DirectionType.Absolute, -1f), new Speed(5f, SpeedType.Absolute), null);
			}
			yield break;
		}
	}
}
