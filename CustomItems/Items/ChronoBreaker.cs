using ItemAPI;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GlaurungItems.Items
{
    class ChronoBreaker : PlayerItem
    {

		public static void Init()
		{
			string text = "ChronoBreaker";
			string resourcePath = "GlaurungItems/Resources/acme_crate";
			GameObject gameObject = new GameObject(text);
			ChronoBreaker item = gameObject.AddComponent<ChronoBreaker>();
			ItemBuilder.AddSpriteToObject(text, resourcePath, gameObject);
			string shortDesc = "WIP";
			string longDesc = "WIP";
			item.SetupItem(shortDesc, longDesc, "gl");
			item.SetCooldownType(ItemBuilder.CooldownType.Timed, cooldown);
			item.quality = ItemQuality.C;
		}

        protected override void DoEffect(PlayerController user)
        {
            base.DoEffect(user);
            if (!timeAfterImgsActive)
            {
				SpriteBuilder.AddComponent(user.gameObject, timeAfterImgs);
				playerPositionsDuringActivation = new List<Vector2>();
				timeAfterImgsActive = true;
            }
            else
            {
				if(user.gameObject.GetComponent<ImprovedAfterImage>() != null)
                {
					Destroy(user.gameObject.GetComponent<ImprovedAfterImage>());
                }
				int playerPositionsDuringActivationLength = playerPositionsDuringActivation.Count;
				if (playerPositionsDuringActivationLength <= numberOfPreviousSteps)
                {
					AttemptToWarpPlayer(user, 0);
                }
                else
                {
					AttemptToWarpPlayer(user, playerPositionsDuringActivationLength - numberOfPreviousSteps);
				}
				timeAfterImgsActive = false;
				playerPositionsDuringActivation = new List<Vector2>();
			}

		}

		private void AttemptToWarpPlayer(PlayerController user, int index)
        {
			Vector2 pos = playerPositionsDuringActivation[index];
			if (
				user.IsValidPlayerPosition(pos) && 
				(!user.IsInCombat || (user.IsInCombat && user.CurrentRoom.ContainsPosition(pos.ToIntVector2()) ))
			)
            {
				user.WarpToPoint(playerPositionsDuringActivation[index]);
			}
		}

        public override void Update()
        {
            base.Update();
            if (timeAfterImgsActive)
            {
				playerPositionsDuringActivation.Add(this.LastOwner.CenterPosition);
            }
        }

        protected override void AfterCooldownApplied(PlayerController user)
		{
			if (timeAfterImgsActive)
			{
				ClearCooldowns();
			}
			this.CurrentDamageCooldown = Mathf.Min(CurrentDamageCooldown, cooldown);
		}

		private static float cooldown = 5f;
		private static int numberOfPreviousSteps = 75;
		private bool timeAfterImgsActive;
		private List<Vector2> playerPositionsDuringActivation;
		private readonly ImprovedAfterImage timeAfterImgs = new ImprovedAfterImage
		{
			dashColor = Color.green,
			spawnShadows = true
		};
	}

	public class ImprovedAfterImage : BraveBehaviour
	{
		public ImprovedAfterImage()
		{
			this.shaders = new List<Shader>
			{
				ShaderCache.Acquire("Brave/Internal/RainbowChestShader"),
				ShaderCache.Acquire("Brave/Internal/GlitterPassAdditive"),
				ShaderCache.Acquire("Brave/Internal/HologramShader"),
				ShaderCache.Acquire("Brave/Internal/HighPriestAfterImage")
			};
			this.IsRandomShader = false;
			this.spawnShadows = true;
			this.shadowTimeDelay = 0.1f;
			this.shadowLifetime = 1.5f;
			this.minTranslation = 0.2f;
			this.maxEmission = 800f;
			this.minEmission = 100f;
			this.targetHeight = -2f;
			this.dashColor = new Color(1f, 0f, 1f, 1f);
			this.m_activeShadows = new LinkedList<ImprovedAfterImage.Shadow>();
			this.m_inactiveShadows = new LinkedList<ImprovedAfterImage.Shadow>();
		}

		public void Start()
		{
			bool flag = this.OptionalImageShader != null;
			if (flag)
			{
				this.OverrideImageShader = this.OptionalImageShader;
			}
			bool flag2 = base.transform.parent != null && base.transform.parent.GetComponent<Projectile>() != null;
			if (flag2)
			{
				base.transform.parent.GetComponent<Projectile>().OnDestruction += this.ProjectileDestruction;
			}
			this.m_lastSpawnPosition = base.transform.position;
		}

		private void ProjectileDestruction(Projectile source)
		{
			bool flag = this.m_activeShadows.Count > 0;
			if (flag)
			{
				GameManager.Instance.StartCoroutine(this.HandleDeathShadowCleanup());
			}
		}

		public void LateUpdate()
		{
			bool flag = this.spawnShadows && !this.m_previousFrameSpawnShadows;
			if (flag)
			{
				this.m_spawnTimer = this.shadowTimeDelay;
			}
			this.m_previousFrameSpawnShadows = this.spawnShadows;
			LinkedListNode<ImprovedAfterImage.Shadow> next;
			for (LinkedListNode<ImprovedAfterImage.Shadow> linkedListNode = this.m_activeShadows.First; linkedListNode != null; linkedListNode = next)
			{
				next = linkedListNode.Next;
				linkedListNode.Value.timer -= BraveTime.DeltaTime;
				bool flag2 = linkedListNode.Value.timer <= 0f;
				if (flag2)
				{
					this.m_activeShadows.Remove(linkedListNode);
					this.m_inactiveShadows.AddLast(linkedListNode);
					bool flag3 = linkedListNode.Value.sprite;
					if (flag3)
					{
						linkedListNode.Value.sprite.renderer.enabled = false;
					}
				}
				else
				{
					bool flag4 = linkedListNode.Value.sprite;
					if (flag4)
					{
						float num = linkedListNode.Value.timer / this.shadowLifetime;
						Material sharedMaterial = linkedListNode.Value.sprite.renderer.sharedMaterial;
						sharedMaterial.SetFloat("_EmissivePower", Mathf.Lerp(this.maxEmission, this.minEmission, num));
						sharedMaterial.SetFloat("_Opacity", num);
					}
				}
			}
			bool flag5 = this.spawnShadows;
			if (flag5)
			{
				bool flag6 = this.m_spawnTimer > 0f;
				if (flag6)
				{
					this.m_spawnTimer -= BraveTime.DeltaTime;
				}
				bool flag7 = this.m_spawnTimer <= 0f && Vector2.Distance(this.m_lastSpawnPosition, base.transform.position) > this.minTranslation;
				if (flag7)
				{
					this.SpawnNewShadow();
					this.m_spawnTimer += this.shadowTimeDelay;
					this.m_lastSpawnPosition = base.transform.position;
				}
			}
		}

		private IEnumerator HandleDeathShadowCleanup()
		{
			while (this.m_activeShadows.Count > 0)
			{
				LinkedListNode<ImprovedAfterImage.Shadow> node;
				LinkedListNode<ImprovedAfterImage.Shadow> next;
				for (node = this.m_activeShadows.First; node != null; node = next)
				{
					next = node.Next;
					node.Value.timer -= BraveTime.DeltaTime;
					bool flag = node.Value.timer <= 0f;
					if (flag)
					{
						this.m_activeShadows.Remove(node);
						this.m_inactiveShadows.AddLast(node);
						bool flag2 = node.Value.sprite;
						if (flag2)
						{
							node.Value.sprite.renderer.enabled = false;
						}
					}
					else
					{
						bool flag3 = node.Value.sprite;
						if (flag3)
						{
							float num = node.Value.timer / this.shadowLifetime;
							Material sharedMaterial = node.Value.sprite.renderer.sharedMaterial;
							sharedMaterial.SetFloat("_EmissivePower", Mathf.Lerp(this.maxEmission, this.minEmission, num));
							sharedMaterial.SetFloat("_Opacity", num);
							sharedMaterial = null;
						}
					}
				}
				node = null;
				yield return null;
				next = null;
			}
			yield break;
		}

		protected override void OnDestroy()
		{
			GameManager.Instance.StartCoroutine(this.HandleDeathShadowCleanup());
			base.OnDestroy();
		}

		// Token: 0x0600018C RID: 396 RVA: 0x0000BE94 File Offset: 0x0000A094
		private void SpawnNewShadow()
		{
			bool flag = this.m_inactiveShadows.Count == 0;
			if (flag)
			{
				this.CreateInactiveShadow();
			}
			LinkedListNode<ImprovedAfterImage.Shadow> first = this.m_inactiveShadows.First;
			tk2dSprite sprite = first.Value.sprite;
			this.m_inactiveShadows.RemoveFirst();
			bool flag2 = !sprite || !sprite.renderer;
			if (!flag2)
			{
				first.Value.timer = this.shadowLifetime;
				sprite.SetSprite(base.sprite.Collection, base.sprite.spriteId);
				sprite.transform.position = base.sprite.transform.position;
				sprite.transform.rotation = base.sprite.transform.rotation;
				sprite.scale = base.sprite.scale;
				sprite.usesOverrideMaterial = true;
				sprite.IsPerpendicular = true;
				bool flag3 = sprite.renderer && this.IsRandomShader;
				if (flag3)
				{
					sprite.renderer.enabled = true;
					sprite.renderer.material.shader = this.shaders[UnityEngine.Random.Range(0, this.shaders.Count)];
					bool flag4 = sprite.renderer.material.shader == this.shaders[3];
					if (flag4)
					{
						sprite.renderer.sharedMaterial.SetFloat("_EmissivePower", this.minEmission);
						sprite.renderer.sharedMaterial.SetFloat("_Opacity", 1f);
						sprite.renderer.sharedMaterial.SetColor("_DashColor", Color.HSVToRGB(UnityEngine.Random.value, 1f, 1f));
					}
					bool flag5 = sprite.renderer.material.shader == this.shaders[0];
					if (flag5)
					{
						sprite.renderer.sharedMaterial.SetFloat("_AllColorsToggle", 1f);
					}
				}
				else
				{
					bool flag6 = sprite.renderer;
					if (flag6)
					{
						sprite.renderer.enabled = true;
						sprite.renderer.material.shader = (this.OverrideImageShader ?? ShaderCache.Acquire("Brave/Internal/HighPriestAfterImage"));
						sprite.renderer.sharedMaterial.SetFloat("_EmissivePower", this.minEmission);
						sprite.renderer.sharedMaterial.SetFloat("_Opacity", 1f);
						sprite.renderer.sharedMaterial.SetColor("_DashColor", this.dashColor);
						sprite.renderer.sharedMaterial.SetFloat("_AllColorsToggle", 1f);
					}
				}
				sprite.HeightOffGround = this.targetHeight;
				sprite.UpdateZDepth();
				this.m_activeShadows.AddLast(first);
			}
		}

		// Token: 0x0600018D RID: 397 RVA: 0x0000C194 File Offset: 0x0000A394
		private void CreateInactiveShadow()
		{
			GameObject gameObject = new GameObject("after image");
			bool useTargetLayer = this.UseTargetLayer;
			if (useTargetLayer)
			{
				gameObject.layer = LayerMask.NameToLayer(this.TargetLayer);
			}
			tk2dSprite sprite = gameObject.AddComponent<tk2dSprite>();
			gameObject.transform.parent = SpawnManager.Instance.VFX;
			this.m_inactiveShadows.AddLast(new ImprovedAfterImage.Shadow
			{
				timer = this.shadowLifetime,
				sprite = sprite
			});
		}

		// Token: 0x0400006A RID: 106
		public bool IsRandomShader;

		// Token: 0x0400006B RID: 107
		public bool spawnShadows;

		// Token: 0x0400006C RID: 108
		public float shadowTimeDelay;

		// Token: 0x0400006D RID: 109
		public float shadowLifetime;

		// Token: 0x0400006E RID: 110
		public float minTranslation;

		// Token: 0x0400006F RID: 111
		public float maxEmission;

		// Token: 0x04000070 RID: 112
		public float minEmission;

		// Token: 0x04000071 RID: 113
		public float targetHeight;

		// Token: 0x04000072 RID: 114
		public Color dashColor;

		public Shader OptionalImageShader;

		// Token: 0x04000074 RID: 116
		public bool UseTargetLayer;

		// Token: 0x04000075 RID: 117
		public string TargetLayer;

		// Token: 0x04000076 RID: 118
		[NonSerialized]
		public Shader OverrideImageShader;

		private readonly LinkedList<ImprovedAfterImage.Shadow> m_activeShadows;

		private readonly LinkedList<ImprovedAfterImage.Shadow> m_inactiveShadows;

		private readonly List<Shader> shaders;

		private float m_spawnTimer;

		private Vector2 m_lastSpawnPosition;

		private bool m_previousFrameSpawnShadows;

		private class Shadow
		{
			public float timer;

			public tk2dSprite sprite;
		}
	}
}
