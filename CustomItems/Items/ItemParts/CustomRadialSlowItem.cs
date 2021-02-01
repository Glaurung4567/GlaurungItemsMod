using System;
using System.Collections;
using UnityEngine;

namespace GlaurungItems.Items
{
	public class CustomRadialSlowItem : AffectEnemiesInRadiusItem
	{
		public CustomRadialSlowItem()
		{
			this.HoldTime = 5f;
			this.OutTime = 2f;
			this.EffectRadius = 30f;
			this.MaxTimeModifier = 0f;
		}

		// Token: 0x06007714 RID: 30484 RVA: 0x002E8A24 File Offset: 0x002E6C24
		protected override void DoEffect(PlayerController user)
		{
			//AkSoundEngine.PostEvent("Play_OBJ_time_bell_01", base.gameObject);
			base.DoEffect(user);
		}


		protected override void AffectEnemy(AIActor target)
		{
			if (!base.IsCurrentlyActive)
			{
				//GameManager.Instance.Dungeon.StartCoroutine(this.HandleActive());
			}
			target.StartCoroutine(this.ProcessSlow(target));
		}

		protected override void AffectForgeHammer(ForgeHammerController target)
		{
			if (!base.IsCurrentlyActive)
			{
				//GameManager.Instance.Dungeon.StartCoroutine(this.HandleActive());
			}
			target.StartCoroutine(this.ProcessHammerSlow(target));
		}

		protected override void AffectProjectileTrap(ProjectileTrapController target)
		{
			if (!base.IsCurrentlyActive)
			{
				//GameManager.Instance.Dungeon.StartCoroutine(this.HandleActive());
			}
			target.StartCoroutine(this.ProcessTrapSlow(target));
		}


		protected override void AffectMajorBreakable(MajorBreakable target)
		{
			if (target.behaviorSpeculator)
			{
				target.StartCoroutine(this.ProcessBehaviorSpeculatorSlow(target.behaviorSpeculator));
			}
		}

		private IEnumerator HandleActive()
		{
			this.IsCurrentlyActive = true;
			this.m_activeDuration = this.InTime + this.HoldTime + this.OutTime;
			while (this.m_activeElapsed < this.m_activeDuration)
			{
				this.m_activeElapsed += BraveTime.DeltaTime;
				yield return null;
			}
			this.IsCurrentlyActive = false;
			yield break;
		}

		private IEnumerator ProcessSlow(AIActor target)
		{
			float elapsed = 0f;
			/*if (this.InTime > 0f)
			{
				while (elapsed < this.InTime)
				{
					if (!target || target.healthHaver.IsDead)
					{
						break;
					}
					elapsed += BraveTime.DeltaTime;
					float t = elapsed / this.InTime;
					target.LocalTimeScale = Mathf.Lerp(1f, this.MaxTimeModifier, t);
					yield return null;
				}
			}*/
			elapsed = 0f;
			if (this.HoldTime > 0f)
			{
				while (stopLocalTime)//elapsed < this.HoldTime)
				{
					if (!target || target.healthHaver.IsDead)
					{
						break;
					}
					elapsed += BraveTime.DeltaTime;
					target.LocalTimeScale = this.MaxTimeModifier;
					yield return null;
				}
			}
			elapsed = 0f;
			if (this.OutTime > 0f)
			{
				while (elapsed < this.OutTime)
				{
					if (!target || target.healthHaver.IsDead)
					{
						break;
					}
					elapsed += BraveTime.DeltaTime;
					float t2 = elapsed / this.OutTime;
					target.LocalTimeScale = Mathf.Lerp(this.MaxTimeModifier, 1f, t2);
					yield return null;
				}
			}
			if (target)
			{
				target.LocalTimeScale = 1f;
			}
			yield break;
		}

		private IEnumerator ProcessHammerSlow(ForgeHammerController target)
		{
			float elapsed = 0f;
			/*if (this.InTime > 0f)
			{
				while (elapsed < this.InTime)
				{
					elapsed += BraveTime.DeltaTime;
					target.LocalTimeScale = Mathf.Lerp(1f, this.MaxTimeModifier, elapsed / this.InTime);
					yield return null;
				}
			}
			elapsed = 0f;*/
			if (this.HoldTime > 0f)
			{
				while (stopLocalTime)//elapsed < this.HoldTime)
				{
					elapsed += BraveTime.DeltaTime;
					target.LocalTimeScale = this.MaxTimeModifier;
					yield return null;
				}
			}
			elapsed = 0f;
			if (this.OutTime > 0f)
			{
				while (elapsed < this.OutTime)
				{
					elapsed += BraveTime.DeltaTime;
					target.LocalTimeScale = Mathf.Lerp(this.MaxTimeModifier, 1f, elapsed / this.OutTime);
					yield return null;
				}
			}
			if (target)
			{
				target.LocalTimeScale = 1f;
			}
			yield break;
		}

		private IEnumerator ProcessTrapSlow(ProjectileTrapController target)
		{
			float elapsed = 0f;
			/*if (this.InTime > 0f)
			{
				while (elapsed < this.InTime)
				{
					elapsed += BraveTime.DeltaTime;
					target.LocalTimeScale = Mathf.Lerp(1f, this.MaxTimeModifier, elapsed / this.InTime);
					yield return null;
				}
			}*/
			elapsed = 0f;
			if (this.HoldTime > 0f)
			{
				while (stopLocalTime)//elapsed < this.HoldTime)
				{
					elapsed += BraveTime.DeltaTime;
					target.LocalTimeScale = this.MaxTimeModifier;
					yield return null;
				}
			}
			elapsed = 0f;
			if (this.OutTime > 0f)
			{
				while (elapsed < this.OutTime)
				{
					elapsed += BraveTime.DeltaTime;
					target.LocalTimeScale = Mathf.Lerp(this.MaxTimeModifier, 1f, elapsed / this.OutTime);
					yield return null;
				}
			}
			if (target)
			{
				target.LocalTimeScale = 1f;
			}
			yield break;
		}

		private IEnumerator ProcessBehaviorSpeculatorSlow(BehaviorSpeculator target)
		{
			float elapsed = 0f;
			AIAnimator aiAnimator = (!target) ? null : target.aiAnimator;
			/*if (this.InTime > 0f)
			{
				while (elapsed < this.InTime)
				{
					if (!target)
					{
						break;
					}
					elapsed += BraveTime.DeltaTime;
					float t = elapsed / this.InTime;
					target.LocalTimeScale = Mathf.Lerp(1f, this.MaxTimeModifier, t);
					if (aiAnimator)
					{
						aiAnimator.FpsScale = Mathf.Lerp(1f, this.MaxTimeModifier, t);
					}
					yield return null;
				}
			}*/
			elapsed = 0f;
			if (this.HoldTime > 0f)
			{
				while (stopLocalTime)//elapsed < this.HoldTime)
				{
					if (!target)
					{
						break;
					}
					elapsed += BraveTime.DeltaTime;
					target.LocalTimeScale = this.MaxTimeModifier;
					if (aiAnimator)
					{
						aiAnimator.FpsScale = this.MaxTimeModifier;
					}
					yield return null;
				}
			}
			elapsed = 0f;
			if (this.OutTime > 0f)
			{
				while (elapsed < this.OutTime)
				{
					if (!target)
					{
						break;
					}
					elapsed += BraveTime.DeltaTime;
					float t2 = elapsed / this.OutTime;
					target.LocalTimeScale = Mathf.Lerp(this.MaxTimeModifier, 1f, t2);
					if (aiAnimator)
					{
						aiAnimator.FpsScale = Mathf.Lerp(this.MaxTimeModifier, 1f, t2);
					}
					yield return null;
				}
			}
			if (aiAnimator)
			{
				aiAnimator.FpsScale = 1f;
			}
			if (target)
			{
				target.LocalTimeScale = 1f;
			}
			yield break;
		}

		protected override void OnDestroy()
		{
			base.OnDestroy();
		}

		public float InTime;

		public float HoldTime;

		public float OutTime;

		public float MaxTimeModifier;

		public bool stopLocalTime;

		public bool AllowStealing;
	}
}