using Brave.BulletScript;
using System.Collections;
using UnityEngine;


namespace GlaurungItems.Items
{
	public class BulletScriptGunHighPriestFaceShoot1 : Script
	{
		// Token: 0x06000732 RID: 1842 RVA: 0x0002154C File Offset: 0x0001F74C
		protected override IEnumerator Top()
		{
			//yield return this.Wait(60);
			float aim = BulletScriptGun.playerGunCurrentAngle;//this.GetAimDirection(1f, 16f);
			this.Fire(new Direction(aim, DirectionType.Absolute, -1f), new Speed(16f, SpeedType.Absolute), new BulletScriptGunHighPriestFaceShoot1.FastHomingShot());
			//yield return this.Wait(30);
			yield break;
		}

		// Token: 0x020001E2 RID: 482
		public class FastHomingShot : Bullet
		{
			// Token: 0x06000733 RID: 1843 RVA: 0x00021567 File Offset: 0x0001F767
			public FastHomingShot() : base("quickHoming", false, false, false)
			{
			}

			// Token: 0x06000734 RID: 1844 RVA: 0x00021578 File Offset: 0x0001F778
			protected override IEnumerator Top()
			{
				for (int i = 0; i < 180; i++)
				{
					float aim = BulletScriptGun.playerGunCurrentAngle;// this.GetAimDirection(1f, 16f);
					float delta = BraveMathCollege.ClampAngle180(aim - this.Direction);
					if (Mathf.Abs(delta) > 100f)
					{
						yield break;
					}
					this.Direction += Mathf.MoveTowards(0f, delta, 3f);
					yield return this.Wait(1);
				}
				yield break;
			}
		}
	}
}
