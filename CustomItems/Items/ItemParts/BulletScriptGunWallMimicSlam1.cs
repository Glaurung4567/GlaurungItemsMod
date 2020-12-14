using Brave.BulletScript;
using System.Collections;
using UnityEngine;

namespace GlaurungItems.Items
{
    class BulletScriptGunWallMimicSlam1 : Script
	{
		protected override IEnumerator Top()
		{
			float facingDirection = GunjuringEncyclopedia.playerGunCurrentAngle;//this.BulletBank.aiAnimator.CurrentArtAngle;
			this.FireLine(facingDirection - 90f, 5f, 45f, -15f, false);
			this.FireLine(facingDirection, 11f, -45f, 45f, false);
			this.FireLine(facingDirection + 90f, 5f, -45f, 15f, false);
			yield return this.Wait(10);
			this.FireLine(facingDirection - 90f, 4f, 45f, -15f, false);
			this.FireLine(facingDirection, 10f, -45f, 45f, false);
			this.FireLine(facingDirection + 90f, 4f, -45f, 15f, false);
			yield break;
		}

		protected void FireLine(float centralAngle, float numBullets, float minAngle, float maxAngle, bool addBlackBullets = false)
		{
			float num = (maxAngle - minAngle) / (numBullets - 1f);
			int num2 = 0;
			while ((float)num2 < numBullets)
			{
				float num3 = Mathf.Atan((minAngle + (float)num2 * num) / 45f) * 57.29578f;
				float num4 = Mathf.Cos(num3 * 0.0174532924f);
				float num5 = ((double)Mathf.Abs(num4) >= 0.0001) ? (1f / num4) : 1f;
				Bullet bullet = new Bullet(null, false, false, false);
				if (addBlackBullets && num2 % 2 == 1)
				{
					bullet.ForceBlackBullet = true;
				}
				base.Fire(new Direction(num3 + centralAngle, DirectionType.Absolute, -1f), new Speed(num5 * 9f, SpeedType.Absolute), bullet);
				num2++;
			}
		}
	}
}
