using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace ItemAPI
{
	public class CustomGunFormeSynergyProcessor : MonoBehaviour
	{
		protected virtual void Awake()
		{
			this.m_gun = base.GetComponent<Gun>();
			Gun gun = this.m_gun;
			gun.OnReloadPressed = (Action<PlayerController, Gun, bool>)Delegate.Combine(gun.OnReloadPressed, new Action<PlayerController, Gun, bool>(this.HandleReloadPressed));
		}

		private void Update()
		{
			if (this.m_gun && !this.m_gun.CurrentOwner && this.CurrentForme != 0)
			{
				this.ChangeForme(this.Formes[0]);
				this.CurrentForme = 0;
			}
			this.JustActiveReloaded = false;
		}

		private void HandleReloadPressed(PlayerController ownerPlayer, Gun sourceGun, bool manual)
		{
			if (this.JustActiveReloaded)
			{
				return;
			}
			if (manual && !sourceGun.IsReloading)
			{
				int nextValidForme = this.GetNextValidForme(ownerPlayer);
				if (nextValidForme != this.CurrentForme)
				{
					this.ChangeForme(this.Formes[nextValidForme]);
					this.CurrentForme = nextValidForme;
				}
			}
		}

		private int GetNextValidForme(PlayerController ownerPlayer)
		{
			try
			{
				if (this.Formes != null)
				{
					for (int i = 0; i < this.Formes.Length; i++)
					{
						int num = (i + this.CurrentForme) % this.Formes.Length;
						if (num != this.CurrentForme)
						{
							if (this.Formes[num].IsValid(ownerPlayer))
							{
								return num;
							}
						}
					}
				}
				else
				{
					Tools.Print("fuck frick aaaaaaaaaaaaaaaaaaaaaaaaa", "ffffff", true);
				}

			}
			catch (Exception e)
			{
				Tools.PrintException(e);
				Tools.PrintError(e);
			}
			return this.CurrentForme;
		}

		private void ChangeForme(CustomGunFormeData targetForme)
		{
			Gun gun = PickupObjectDatabase.GetById(targetForme.FormeID) as Gun;
			this.m_gun.TransformToTargetGun(gun);
			if (this.m_gun.encounterTrackable && gun.encounterTrackable)
			{
				this.m_gun.encounterTrackable.journalData.PrimaryDisplayName = gun.encounterTrackable.journalData.PrimaryDisplayName;
				this.m_gun.encounterTrackable.journalData.ClearCache();
				PlayerController playerController = this.m_gun.CurrentOwner as PlayerController;
				if (playerController)
				{
					GameUIRoot.Instance.TemporarilyShowGunName(playerController.IsPrimaryPlayer);
				}
			}
		}

		public static void AssignTemporaryOverrideGun(PlayerController targetPlayer, int gunID, float duration)
		{
			if (targetPlayer && !targetPlayer.IsGhost)
			{
				targetPlayer.StartCoroutine(GunFormeSynergyProcessor.HandleTransformationDuration(targetPlayer, gunID, duration));
			}
		}

		// Token: 0x0600887C RID: 34940 RVA: 0x0037805C File Offset: 0x0037625C
		public static IEnumerator HandleTransformationDuration(PlayerController targetPlayer, int gunID, float duration)
		{
			float elapsed = 0f;
			if (targetPlayer && targetPlayer.inventory.GunLocked.Value && targetPlayer.CurrentGun)
			{
				MimicGunController component = targetPlayer.CurrentGun.GetComponent<MimicGunController>();
				if (component)
				{
					component.ForceClearMimic(false);
				}
			}
			targetPlayer.inventory.GunChangeForgiveness = true;
			Gun limitGun = PickupObjectDatabase.GetById(gunID) as Gun;
			Gun m_extantGun = targetPlayer.inventory.AddGunToInventory(limitGun, true);
			m_extantGun.CanBeDropped = false;
			m_extantGun.CanBeSold = false;
			targetPlayer.inventory.GunLocked.SetOverride("override gun", true, null);
			elapsed = 0f;
			while (elapsed < duration)
			{
				elapsed += BraveTime.DeltaTime;
				yield return null;
			}
			CustomGunFormeSynergyProcessor.ClearTemporaryOverrideGun(targetPlayer, m_extantGun);
			yield break;
		}

		protected static void ClearTemporaryOverrideGun(PlayerController targetPlayer, Gun m_extantGun)
		{
			if (!targetPlayer || !m_extantGun)
			{
				return;
			}
			if (targetPlayer)
			{
				targetPlayer.inventory.GunLocked.RemoveOverride("override gun");
				targetPlayer.inventory.DestroyGun(m_extantGun);
				m_extantGun = null;
			}
			targetPlayer.inventory.GunChangeForgiveness = false;
		}

		// Token: 0x04008DD0 RID: 36304
		public CustomGunFormeData[] Formes;

		// Token: 0x04008DD1 RID: 36305
		private Gun m_gun;

		// Token: 0x04008DD2 RID: 36306
		private int CurrentForme;

		// Token: 0x04008DD3 RID: 36307
		[NonSerialized]
		public bool JustActiveReloaded;
	}

	public class CustomGunFormeData
	{
		// Token: 0x06008884 RID: 34948 RVA: 0x00378284 File Offset: 0x00376484
		public CustomGunFormeData()
		{
			this.RequiresSynergy = true;
			//base..ctor();
		}

		// Token: 0x06008885 RID: 34949 RVA: 0x00378293 File Offset: 0x00376493
		public bool IsValid(PlayerController p)
		{
			return !this.RequiresSynergy || p.PlayerHasActiveSynergy(this.RequiredSynergy);
		}

		public bool RequiresSynergy;

		public string RequiredSynergy;

		// Token: 0x04008DDF RID: 36319
		[PickupIdentifier]
		public int FormeID;
	}
}
