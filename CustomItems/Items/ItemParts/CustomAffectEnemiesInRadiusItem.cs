using System;
using System.Collections;
using System.Collections.Generic;
using Dungeonator;
using ItemAPI;
using UnityEngine;

namespace GlaurungItems.Items
{
	public abstract class CustomAffectEnemiesInRadiusItem : PlayerItem
	{
		// Token: 0x06006FAD RID: 28589 RVA: 0x002B5F89 File Offset: 0x002B4189
		protected CustomAffectEnemiesInRadiusItem()
		{
			this.EffectRadius = 10f;
			this.OnUserEffectOffset = Vector3.zero;
			this.minTimeBetweenAmbientVFX = 0.1f;
			//base..ctor();
		}

		// Token: 0x06006FAE RID: 28590 RVA: 0x002B5FB8 File Offset: 0x002B41B8
		protected override void DoEffect(PlayerController user)
		{
			AffectEffect(user);
		}

		protected void AffectEffect(PlayerController user, bool overrideNormalDoEffect = false)
        {
			List<AIActor> activeEnemies = StaticReferenceManager.AllEnemies;//user.CurrentRoom.GetActiveEnemies(RoomHandler.ActiveEnemyType.All);
			if (this.OnUserEffectVFX != null)
			{
				if (this.OnUserEffectAttached)
				{
					user.PlayEffectOnActor(this.OnUserEffectVFX, this.OnUserEffectOffset, true, false, false);
				}
				else
				{
					SpawnManager.SpawnVFX(this.OnUserEffectVFX, user.CenterPosition + this.OnUserEffectOffset, Quaternion.identity, false);
				}
			}
			if (this.ShakeScreen)
			{
				GameManager.Instance.MainCameraController.DoScreenShake(this.ScreenShakeData, null, false);
			}
			if (this.FlashScreen)
			{
				Pixelator.Instance.FadeToColor(0.1f, Color.white, true, 0.1f);
			}
			if (this.DoEffectDistortionWave)
			{
				Exploder.DoDistortionWave(user.CenterPosition, 0.4f, 0.15f, this.EffectRadius, 0.4f);
			}
			if (!string.IsNullOrEmpty(this.AudioEvent))
			{
				AkSoundEngine.PostEvent(this.AudioEvent, base.gameObject);
			}
			if (this.EffectTime <= 0f || overrideNormalDoEffect)
			{
				Tools.Print("it works ?", "ffffff", true);
				if (activeEnemies != null)
				{
					for (int i = 0; i < activeEnemies.Count; i++)
					{
						AIActor aiactor = activeEnemies[i];
						if (aiactor.IsNormalEnemy)
						{
							float num = Vector2.Distance(user.CenterPosition, aiactor.CenterPosition);
							if (num <= this.EffectRadius)
							{
								this.AffectEnemy(aiactor);
								if (this.OnTargetEffectVFX != null)
								{
									SpawnManager.SpawnVFX(this.OnTargetEffectVFX, aiactor.CenterPosition, Quaternion.identity, false);
								}
							}
						}
					}
					if (this.AmbientVFXTime > 0f && this.AmbientVFX != null)
					{
						user.StartCoroutine(this.HandleAmbientSpawnTime(user.CenterPosition, this.AmbientVFXTime));
					}
				}
				List<ProjectileTrapController> allProjectileTraps = StaticReferenceManager.AllProjectileTraps;
				for (int j = 0; j < allProjectileTraps.Count; j++)
				{
					ProjectileTrapController projectileTrapController = allProjectileTraps[j];
					if (projectileTrapController && projectileTrapController.isActiveAndEnabled)
					{
						float num2 = Vector2.Distance(user.CenterPosition, projectileTrapController.shootPoint.position);
						if (num2 <= this.EffectRadius)
						{
							this.AffectProjectileTrap(projectileTrapController);
							if (this.OnTargetEffectVFX != null)
							{
								SpawnManager.SpawnVFX(this.OnTargetEffectVFX, projectileTrapController.shootPoint.position, Quaternion.identity, false);
							}
						}
					}
				}
				List<ForgeHammerController> allForgeHammers = StaticReferenceManager.AllForgeHammers;
				for (int k = 0; k < allForgeHammers.Count; k++)
				{
					ForgeHammerController forgeHammerController = allForgeHammers[k];
					if (forgeHammerController && forgeHammerController.isActiveAndEnabled)
					{
						float num3 = Vector2.Distance(user.CenterPosition, forgeHammerController.sprite.WorldCenter);
						if (num3 <= this.EffectRadius)
						{
							this.AffectForgeHammer(forgeHammerController);
						}
					}
				}
				List<BaseShopController> allShops = StaticReferenceManager.AllShops;
				for (int l = 0; l < allShops.Count; l++)
				{
					BaseShopController baseShopController = allShops[l];
					float num4 = Vector2.Distance(user.CenterPosition, baseShopController.CenterPosition);
					if (num4 <= this.EffectRadius)
					{
						this.AffectShop(baseShopController);
						if (this.OnTargetEffectVFX != null)
						{
							SpawnManager.SpawnVFX(this.OnTargetEffectVFX, baseShopController.CenterPosition, Quaternion.identity, false);
						}
					}
				}
				List<MajorBreakable> allMajorBreakables = StaticReferenceManager.AllMajorBreakables;
				for (int m = 0; m < allMajorBreakables.Count; m++)
				{
					MajorBreakable majorBreakable = allMajorBreakables[m];
					if (majorBreakable.specRigidbody && majorBreakable.specRigidbody.PrimaryPixelCollider != null)
					{
						float num5 = Vector2.Distance(user.CenterPosition, majorBreakable.specRigidbody.UnitCenter);
						if (num5 <= this.EffectRadius)
						{
							this.AffectMajorBreakable(majorBreakable);
							if (this.OnTargetEffectVFX != null)
							{
								SpawnManager.SpawnVFX(this.OnTargetEffectVFX, majorBreakable.specRigidbody.UnitCenter, Quaternion.identity, false);
							}
						}
					}
				}
			}
			else
			{
				user.StartCoroutine(this.ProcessEffectOverTime(user.CenterPosition, activeEnemies));
			}
		}

		// Token: 0x06006FAF RID: 28591 RVA: 0x002B6424 File Offset: 0x002B4624
		protected void HandleAmbientVFXSpawn(Vector2 centerPoint, float radius)
		{
			if (this.AmbientVFX == null)
			{
				return;
			}
			bool flag = false;
			this.m_ambientTimer -= BraveTime.DeltaTime;
			if (this.m_ambientTimer <= 0f)
			{
				flag = true;
				this.m_ambientTimer = this.minTimeBetweenAmbientVFX;
			}
			if (flag)
			{
				Vector2 v = centerPoint + UnityEngine.Random.insideUnitCircle * radius;
				SpawnManager.SpawnVFX(this.AmbientVFX, v, Quaternion.identity);
			}
		}

		// Token: 0x06006FB0 RID: 28592 RVA: 0x002B64A4 File Offset: 0x002B46A4
		protected IEnumerator HandleAmbientSpawnTime(Vector2 centerPoint, float remainingTime)
		{
			float elapsed = 0f;
			while (elapsed < remainingTime)
			{
				elapsed += BraveTime.DeltaTime;
				this.HandleAmbientVFXSpawn(centerPoint, this.EffectRadius);
				yield return null;
			}
			yield break;
		}

		// Token: 0x06006FB1 RID: 28593 RVA: 0x002B64D0 File Offset: 0x002B46D0
		protected IEnumerator ProcessEffectOverTime(Vector2 centerPoint, List<AIActor> enemiesInRoom)
		{
			float elapsed = 0f;
			List<AIActor> processedEnemies = new List<AIActor>();
			List<BaseShopController> processedShops = new List<BaseShopController>();
			List<ForgeHammerController> processedHammers = new List<ForgeHammerController>();
			List<ProjectileTrapController> processedTraps = new List<ProjectileTrapController>();
			while (elapsed < this.EffectTime)
			{
				elapsed += BraveTime.DeltaTime;
				float t = elapsed / this.EffectTime;
				float CurrentRadius = Mathf.Lerp(0f, this.EffectRadius, t);
				for (int i = 0; i < enemiesInRoom.Count; i++)
				{
					AIActor aiactor = enemiesInRoom[i];
					if (!processedEnemies.Contains(aiactor))
					{
						float num = Vector2.Distance(centerPoint, aiactor.CenterPosition);
						if (num <= CurrentRadius)
						{
							this.AffectEnemy(aiactor);
							if (this.OnTargetEffectVFX != null)
							{
								SpawnManager.SpawnVFX(this.OnTargetEffectVFX, aiactor.CenterPosition, Quaternion.identity, false);
							}
							processedEnemies.Add(aiactor);
						}
					}
				}
				List<ProjectileTrapController> allTraps = StaticReferenceManager.AllProjectileTraps;
				for (int j = 0; j < allTraps.Count; j++)
				{
					ProjectileTrapController projectileTrapController = allTraps[j];
					if (!processedTraps.Contains(projectileTrapController))
					{
						if (projectileTrapController && projectileTrapController.isActiveAndEnabled)
						{
							float num2 = Vector2.Distance(centerPoint, projectileTrapController.shootPoint.position);
							if (num2 <= CurrentRadius)
							{
								this.AffectProjectileTrap(projectileTrapController);
								if (this.OnTargetEffectVFX != null)
								{
									SpawnManager.SpawnVFX(this.OnTargetEffectVFX, projectileTrapController.shootPoint.position, Quaternion.identity, false);
								}
								processedTraps.Add(projectileTrapController);
							}
						}
					}
				}
				List<ForgeHammerController> allHammers = StaticReferenceManager.AllForgeHammers;
				for (int k = 0; k < allHammers.Count; k++)
				{
					ForgeHammerController forgeHammerController = allHammers[k];
					if (!processedHammers.Contains(forgeHammerController))
					{
						if (forgeHammerController && forgeHammerController.isActiveAndEnabled)
						{
							float num3 = Vector2.Distance(centerPoint, forgeHammerController.sprite.WorldCenter);
							if (num3 <= CurrentRadius)
							{
								this.AffectForgeHammer(forgeHammerController);
							}
							processedHammers.Add(forgeHammerController);
						}
					}
				}
				List<BaseShopController> allShops = StaticReferenceManager.AllShops;
				for (int l = 0; l < allShops.Count; l++)
				{
					BaseShopController baseShopController = allShops[l];
					if (!processedShops.Contains(baseShopController))
					{
						float num4 = Vector2.Distance(centerPoint, baseShopController.CenterPosition);
						if (num4 <= CurrentRadius)
						{
							this.AffectShop(baseShopController);
							if (this.OnTargetEffectVFX != null)
							{
								SpawnManager.SpawnVFX(this.OnTargetEffectVFX, baseShopController.CenterPosition, Quaternion.identity, false);
							}
							processedShops.Add(baseShopController);
						}
					}
				}
				this.HandleAmbientVFXSpawn(centerPoint, CurrentRadius);
				yield return null;
			}
			if (this.AmbientVFXTime > this.EffectTime)
			{
				this.StartCoroutine(this.HandleAmbientSpawnTime(centerPoint, this.AmbientVFXTime - this.EffectTime));
			}
			yield break;
		}

		// Token: 0x06006FB2 RID: 28594
		protected abstract void AffectEnemy(AIActor target);

		// Token: 0x06006FB3 RID: 28595 RVA: 0x00007B97 File Offset: 0x00005D97
		protected virtual void AffectProjectileTrap(ProjectileTrapController target)
		{
		}

		// Token: 0x06006FB4 RID: 28596 RVA: 0x00007B97 File Offset: 0x00005D97
		protected virtual void AffectShop(BaseShopController target)
		{
		}

		// Token: 0x06006FB5 RID: 28597 RVA: 0x00007B97 File Offset: 0x00005D97
		protected virtual void AffectForgeHammer(ForgeHammerController target)
		{
		}

		// Token: 0x06006FB6 RID: 28598 RVA: 0x00007B97 File Offset: 0x00005D97
		protected virtual void AffectMajorBreakable(MajorBreakable majorBreakable)
		{
		}

		// Token: 0x06006FB7 RID: 28599 RVA: 0x002B53DD File Offset: 0x002B35DD
		protected override void OnDestroy()
		{
			base.OnDestroy();
		}

		// Token: 0x04006EEB RID: 28395
		public float EffectRadius;

		// Token: 0x04006EEC RID: 28396
		public float EffectTime;

		// Token: 0x04006EED RID: 28397
		public Vector2 OnUserEffectOffset;

		// Token: 0x04006EEE RID: 28398
		public bool OnUserEffectAttached;

		// Token: 0x04006EEF RID: 28399
		public GameObject OnUserEffectVFX;

		// Token: 0x04006EF0 RID: 28400
		public GameObject OnTargetEffectVFX;

		// Token: 0x04006EF1 RID: 28401
		public string AudioEvent;

		// Token: 0x04006EF2 RID: 28402
		public float AmbientVFXTime;

		// Token: 0x04006EF3 RID: 28403
		public GameObject AmbientVFX;

		// Token: 0x04006EF4 RID: 28404
		public float minTimeBetweenAmbientVFX;

		// Token: 0x04006EF5 RID: 28405
		public bool FlashScreen;

		// Token: 0x04006EF6 RID: 28406
		public bool ShakeScreen;

		// Token: 0x04006EF7 RID: 28407
		[ShowInInspectorIf("ShakeScreen", false)]
		public ScreenShakeSettings ScreenShakeData;

		// Token: 0x04006EF8 RID: 28408
		public bool DoEffectDistortionWave;

		// Token: 0x04006EF9 RID: 28409
		private float m_ambientTimer;

	}
}