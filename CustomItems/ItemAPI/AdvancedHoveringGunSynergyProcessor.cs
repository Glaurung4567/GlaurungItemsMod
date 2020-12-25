using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Object = UnityEngine.Object;

namespace ItemAPI
{
	public class AdvancedHoveringGunSynergyProcessor : MonoBehaviour
	{
		public AdvancedHoveringGunSynergyProcessor()
		{
			this.FireCooldown = 1f;
			this.FireDuration = 2f;
			this.NumToTrigger = 1;
			this.TriggerDuration = -1f;
			this.ChanceToConsumeTargetGunAmmo = 0.5f;
			this.m_hovers = new List<HoveringGunController>();
			this.m_initialized = new List<bool>();
		}

		// Token: 0x060004A3 RID: 1187 RVA: 0x0003429E File Offset: 0x0003249E
		public void Awake()
		{
			this.m_gun = base.GetComponent<Gun>();
			this.m_item = base.GetComponent<PassiveItem>();
		}

		private bool IsInitialized(int index)
		{
			return this.m_initialized.Count > index && this.m_initialized[index];
		}

		public void Update()
		{
			bool flag = this.Trigger == AdvancedHoveringGunSynergyProcessor.TriggerStyle.CONSTANT;
			if (flag)
			{
				bool flag2 = this.m_gun;
				if (flag2)
				{
					bool flag3 = this.m_gun && this.m_gun.isActiveAndEnabled && this.m_gun.CurrentOwner && this.m_gun.OwnerHasSynergy(this.RequiredSynergy);
					if (flag3)
					{
						for (int i = 0; i < this.NumToTrigger; i++)
						{
							bool flag4 = !this.IsInitialized(i);
							if (flag4)
							{
								this.Enable(i);
							}
						}
					}
					else
					{
						this.DisableAll();
					}
				}
				else
				{
					bool flag5 = this.m_item;
					if (flag5)
					{
						bool flag6 = this.m_item && this.m_item.Owner && this.m_item.Owner.PlayerHasActiveSynergy(this.RequiredSynergy);
						if (flag6)
						{
							for (int j = 0; j < this.NumToTrigger; j++)
							{
								bool flag7 = !this.IsInitialized(j);
								if (flag7)
								{
									this.Enable(j);
								}
							}
						}
						else
						{
							this.DisableAll();
						}
					}
				}
			}
			else
			{
				bool flag8 = this.Trigger == AdvancedHoveringGunSynergyProcessor.TriggerStyle.ON_DAMAGE;
				if (flag8)
				{
					bool flag9 = !this.m_actionsLinked && this.m_gun && this.m_gun.CurrentOwner;
					if (flag9)
					{
						PlayerController playerController = this.m_gun.CurrentOwner as PlayerController;
						this.m_cachedLinkedPlayer = playerController;
						playerController.OnReceivedDamage += this.HandleOwnerDamaged;
						this.m_actionsLinked = true;
					}
					else
					{
						bool flag10 = this.m_actionsLinked && this.m_gun && !this.m_gun.CurrentOwner && this.m_cachedLinkedPlayer;
						if (flag10)
						{
							this.m_cachedLinkedPlayer.OnReceivedDamage -= this.HandleOwnerDamaged;
							this.m_cachedLinkedPlayer = null;
							this.m_actionsLinked = false;
						}
					}
				}
				else
				{
					bool flag11 = this.Trigger == AdvancedHoveringGunSynergyProcessor.TriggerStyle.ON_ACTIVE_ITEM;
					if (flag11)
					{
						bool flag12 = !this.m_actionsLinked && this.m_gun && this.m_gun.CurrentOwner;
						if (flag12)
						{
							PlayerController playerController2 = this.m_gun.CurrentOwner as PlayerController;
							this.m_cachedLinkedPlayer = playerController2;
							playerController2.OnUsedPlayerItem += this.HandleOwnerItemUsed;
							this.m_actionsLinked = true;
						}
						else
						{
							bool flag13 = this.m_actionsLinked && this.m_gun && !this.m_gun.CurrentOwner && this.m_cachedLinkedPlayer;
							if (flag13)
							{
								this.m_cachedLinkedPlayer.OnUsedPlayerItem -= this.HandleOwnerItemUsed;
								this.m_cachedLinkedPlayer = null;
								this.m_actionsLinked = false;
							}
						}
					}
				}
			}
		}

		// Token: 0x060004A6 RID: 1190 RVA: 0x0003460C File Offset: 0x0003280C
		private void HandleOwnerItemUsed(PlayerController sourcePlayer, PlayerItem sourceItem)
		{
			bool flag = sourcePlayer.PlayerHasActiveSynergy(this.RequiredSynergy) && this.GetOwner();
			if (flag)
			{
				for (int i = 0; i < this.NumToTrigger; i++)
				{
					int num = 0;
					while (this.IsInitialized(num))
					{
						num++;
					}
					this.Enable(num);
					base.StartCoroutine(this.ActiveItemDisable(num, sourcePlayer));
				}
			}
		}

		// Token: 0x060004A7 RID: 1191 RVA: 0x00034684 File Offset: 0x00032884
		private void HandleOwnerDamaged(PlayerController sourcePlayer)
		{
			bool flag = sourcePlayer.PlayerHasActiveSynergy(this.RequiredSynergy);
			if (flag)
			{
				for (int i = 0; i < this.NumToTrigger; i++)
				{
					int num = 0;
					while (this.IsInitialized(num))
					{
						num++;
					}
					this.Enable(num);
					base.StartCoroutine(this.TimedDisable(num, this.TriggerDuration));
				}
			}
		}

		// Token: 0x060004A8 RID: 1192 RVA: 0x000346EE File Offset: 0x000328EE
		private IEnumerator ActiveItemDisable(int index, PlayerController player)
		{
			yield return null;
			while (player && player.CurrentItem && player.CurrentItem.IsActive)
			{
				yield return null;
			}
			this.Disable(index);
			yield break;
		}

		// Token: 0x060004A9 RID: 1193 RVA: 0x0003470B File Offset: 0x0003290B
		private IEnumerator TimedDisable(int index, float duration)
		{
			yield return new WaitForSeconds(duration);
			this.Disable(index);
			yield break;
		}

		// Token: 0x060004AA RID: 1194 RVA: 0x00034728 File Offset: 0x00032928
		private void OnDisable()
		{
			this.DisableAll();
		}

		// Token: 0x060004AB RID: 1195 RVA: 0x00034734 File Offset: 0x00032934
		private PlayerController GetOwner()
		{
			bool flag = this.m_gun;
			PlayerController result;
			if (flag)
			{
				result = (this.m_gun.CurrentOwner as PlayerController);
			}
			else
			{
				bool flag2 = this.m_item;
				if (flag2)
				{
					result = this.m_item.Owner;
				}
				else
				{
					result = null;
				}
			}
			return result;
		}

		// Token: 0x060004AC RID: 1196 RVA: 0x00034788 File Offset: 0x00032988
		private void Enable(int index)
		{
			bool flag = this.m_initialized.Count > index && this.m_initialized[index];
			if (!flag)
			{
				PlayerController owner = this.GetOwner();
				GameObject gameObject = Object.Instantiate<GameObject>(ResourceCache.Acquire("Global Prefabs/HoveringGun") as GameObject, Vector2Extensions.ToVector3ZisY(owner.CenterPosition, 0f), Quaternion.identity);
				gameObject.transform.parent = owner.transform;
				while (this.m_hovers.Count < index + 1)
				{
					this.m_hovers.Add(null);
					this.m_initialized.Add(false);
				}
				this.m_hovers[index] = gameObject.GetComponent<HoveringGunController>();
				this.m_hovers[index].ShootAudioEvent = this.ShootAudioEvent;
				this.m_hovers[index].OnEveryShotAudioEvent = this.OnEveryShotAudioEvent;
				this.m_hovers[index].FinishedShootingAudioEvent = this.FinishedShootingAudioEvent;
				this.m_hovers[index].ConsumesTargetGunAmmo = this.ConsumesTargetGunAmmo;
				this.m_hovers[index].ChanceToConsumeTargetGunAmmo = this.ChanceToConsumeTargetGunAmmo;
				this.m_hovers[index].Position = this.PositionType;
				this.m_hovers[index].Aim = this.AimType;
				this.m_hovers[index].Trigger = this.FireType;
				this.m_hovers[index].CooldownTime = this.FireCooldown;
				this.m_hovers[index].ShootDuration = this.FireDuration;
				this.m_hovers[index].OnlyOnEmptyReload = this.OnlyOnEmptyReload;
				Gun gun = null;
				int num = this.TargetGunID;
				bool usesMultipleGuns = this.UsesMultipleGuns;
				if (usesMultipleGuns)
				{
					num = this.TargetGunIDs[index];
				}
				for (int i = 0; i < owner.inventory.AllGuns.Count; i++)
				{
					bool flag2 = owner.inventory.AllGuns[i].PickupObjectId == num;
					if (flag2)
					{
						gun = owner.inventory.AllGuns[i];
					}
				}
				bool flag3 = !gun;
				if (flag3)
				{
					gun = (PickupObjectDatabase.Instance.InternalGetById(num) as Gun);
				}
				this.m_hovers[index].Initialize(gun, owner);
				this.m_initialized[index] = true;
			}
		}

		private void Disable(int index)
		{
			bool flag = this.m_hovers[index];
			if (flag)
			{
				Object.Destroy(this.m_hovers[index].gameObject);
			}
		}

		private void DisableAll()
		{
			for (int i = 0; i < this.m_hovers.Count; i++)
			{
				bool flag = this.m_hovers[i];
				if (flag)
				{
					Object.Destroy(this.m_hovers[i].gameObject);
				}
			}
			this.m_hovers.Clear();
			this.m_initialized.Clear();
		}

		public void OnDestroy()
		{
			bool flag = this.m_actionsLinked && this.m_cachedLinkedPlayer;
			if (flag)
			{
				this.m_cachedLinkedPlayer.OnReceivedDamage -= this.HandleOwnerDamaged;
				this.m_cachedLinkedPlayer = null;
				this.m_actionsLinked = false;
			}
		}

		public string RequiredSynergy;

		// Token: 0x04000204 RID: 516
		public int TargetGunID;

		// Token: 0x04000205 RID: 517
		public bool UsesMultipleGuns;

		// Token: 0x04000206 RID: 518
		public int[] TargetGunIDs;

		// Token: 0x04000207 RID: 519
		public HoveringGunController.HoverPosition PositionType;

		// Token: 0x04000208 RID: 520
		public HoveringGunController.AimType AimType;

		// Token: 0x04000209 RID: 521
		public HoveringGunController.FireType FireType;

		// Token: 0x0400020A RID: 522
		public float FireCooldown;

		// Token: 0x0400020B RID: 523
		public float FireDuration;

		// Token: 0x0400020C RID: 524
		public bool OnlyOnEmptyReload;

		// Token: 0x0400020D RID: 525
		public string ShootAudioEvent;

		// Token: 0x0400020E RID: 526
		public string OnEveryShotAudioEvent;

		// Token: 0x0400020F RID: 527
		public string FinishedShootingAudioEvent;

		// Token: 0x04000210 RID: 528
		public AdvancedHoveringGunSynergyProcessor.TriggerStyle Trigger;

		// Token: 0x04000211 RID: 529
		public int NumToTrigger;

		// Token: 0x04000212 RID: 530
		public float TriggerDuration;

		// Token: 0x04000213 RID: 531
		public bool ConsumesTargetGunAmmo;

		// Token: 0x04000214 RID: 532
		public float ChanceToConsumeTargetGunAmmo;

		// Token: 0x04000215 RID: 533
		private bool m_actionsLinked;

		// Token: 0x04000216 RID: 534
		private PlayerController m_cachedLinkedPlayer;

		// Token: 0x04000217 RID: 535
		private Gun m_gun;

		// Token: 0x04000218 RID: 536
		private PassiveItem m_item;

		// Token: 0x04000219 RID: 537
		private List<HoveringGunController> m_hovers;

		// Token: 0x0400021A RID: 538
		private List<bool> m_initialized;

		// Token: 0x02000110 RID: 272
		public enum TriggerStyle
		{
			// Token: 0x0400050A RID: 1290
			CONSTANT,
			// Token: 0x0400050B RID: 1291
			ON_DAMAGE,
			// Token: 0x0400050C RID: 1292
			ON_ACTIVE_ITEM
		}
	}
}
