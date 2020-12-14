using Brave.BulletScript;
using System.Collections;
using UnityEngine;

namespace GlaurungItems.Items
{
    class BulletScriptGunGatlingGullFanSpray1 : Script
	{
		protected override IEnumerator Top()
		{
			float angle = GunjuringEncyclopedia.playerGunCurrentAngle - 45f;
			float totalDuration = 2.4f;
			int numBullets = Mathf.RoundToInt(totalDuration * 10f);
			for (int i = 0; i < numBullets; i++)
			{
				float t = (float)i / (float)numBullets;
				float tInFullPass = t * 4f % 2f;
				float currentAngle = angle + Mathf.PingPong(tInFullPass * 90f, 90f);
				this.Fire(new Direction(currentAngle, DirectionType.Absolute, -1f), new Speed((float)((i != 12) ? 12 : 30), SpeedType.Absolute), null);
				yield return this.Wait(6);
			}
			yield break;
		}

		private const float SprayAngle = 90f;

		private const float SpraySpeed = 150f;

		private const int SprayIterations = 4;
	}
}
