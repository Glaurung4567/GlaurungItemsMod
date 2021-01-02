using System;
using UnityEngine;

namespace ItemAPI
{
	// Token: 0x020000A3 RID: 163
	internal class AdvancedDualWieldSynergyProcessor : MonoBehaviour
	{
		// Token: 0x06000498 RID: 1176 RVA: 0x00033D38 File Offset: 0x00031F38
		public void Awake()
		{
			this.m_gun = base.GetComponent<Gun>();
		}

		// Token: 0x06000499 RID: 1177 RVA: 0x00033D48 File Offset: 0x00031F48
		private bool EffectValid(PlayerController p)
		{
			bool flag = !p;
			bool result;
			if (flag)
			{
				result = false;
			}
			else
			{
				bool flag2 = !p.HasPickupID(this.PartnerGunID);
				if (flag2)
				{
					result = false;
				}
				else
				{
					bool flag3 = this.m_gun.CurrentAmmo == 0;
					if (flag3)
					{
						result = false;
					}
					else
					{
						bool value = p.inventory.GunLocked.Value;
						if (value)
						{
							result = false;
						}
						else
						{
							bool flag4 = !this.m_isCurrentlyActive;
							if (flag4)
							{
								int indexForGun = this.GetIndexForGun(p, this.PartnerGunID);
								bool flag5 = indexForGun < 0;
								if (flag5)
								{
									return false;
								}
								bool flag6 = p.inventory.AllGuns[indexForGun].CurrentAmmo == 0;
								if (flag6)
								{
									return false;
								}
							}
							else
							{
								bool flag7 = p.CurrentSecondaryGun != null && p.CurrentSecondaryGun.PickupObjectId == this.PartnerGunID && p.CurrentSecondaryGun.CurrentAmmo == 0;
								if (flag7)
								{
									return false;
								}
							}
							result = true;
						}
					}
				}
			}
			return result;
		}

		// Token: 0x0600049A RID: 1178 RVA: 0x00033E58 File Offset: 0x00032058
		private bool PlayerUsingCorrectGuns()
		{
			return this.m_gun && this.m_gun.CurrentOwner && this.m_cachedPlayer && this.m_cachedPlayer.inventory.DualWielding && this.m_cachedPlayer.HasPickupID(this.PartnerGunID) && (!(this.m_cachedPlayer.CurrentGun != this.m_gun) || this.m_cachedPlayer.CurrentGun.PickupObjectId == this.PartnerGunID) && (!(this.m_cachedPlayer.CurrentSecondaryGun != this.m_gun) || this.m_cachedPlayer.CurrentSecondaryGun.PickupObjectId == this.PartnerGunID);
		}

		// Token: 0x0600049B RID: 1179 RVA: 0x00033F2A File Offset: 0x0003212A
		private void Update()
		{
			this.CheckStatus();
		}

		// Token: 0x0600049C RID: 1180 RVA: 0x00033F34 File Offset: 0x00032134
		private void CheckStatus()
		{
			bool isCurrentlyActive = this.m_isCurrentlyActive;
			if (isCurrentlyActive)
			{
				bool flag = !this.PlayerUsingCorrectGuns() || !this.EffectValid(this.m_cachedPlayer);
				if (flag)
				{
					this.DisableEffect();
				}
			}
			else
			{
				bool flag2 = this.m_gun && this.m_gun.CurrentOwner is PlayerController;
				if (flag2)
				{
					PlayerController playerController = this.m_gun.CurrentOwner as PlayerController;
					bool flag3 = playerController.inventory.DualWielding && playerController.CurrentSecondaryGun.PickupObjectId == this.m_gun.PickupObjectId && playerController.CurrentGun.PickupObjectId == this.PartnerGunID;
					if (flag3)
					{
						this.m_isCurrentlyActive = true;
						this.m_cachedPlayer = playerController;
					}
					else
					{
						this.AttemptActivation(playerController);
					}
				}
			}
		}

		private void AttemptActivation(PlayerController ownerPlayer)
		{
			bool flag = this.EffectValid(ownerPlayer);
			if (flag)
			{
				this.m_isCurrentlyActive = true;
				this.m_cachedPlayer = ownerPlayer;
				ownerPlayer.inventory.SetDualWielding(true, "synergy");
				int indexForGun = this.GetIndexForGun(ownerPlayer, this.m_gun.PickupObjectId);
				int indexForGun2 = this.GetIndexForGun(ownerPlayer, this.PartnerGunID);
				ownerPlayer.inventory.SwapDualGuns();
				bool flag2 = indexForGun >= 0 && indexForGun2 >= 0;
				if (flag2)
				{
					while (ownerPlayer.inventory.CurrentGun.PickupObjectId != this.PartnerGunID)
					{
						ownerPlayer.inventory.ChangeGun(1, false, false);
					}
				}
				ownerPlayer.inventory.SwapDualGuns();
				bool flag3 = ownerPlayer.CurrentGun && !ownerPlayer.CurrentGun.gameObject.activeSelf;
				if (flag3)
				{
					ownerPlayer.CurrentGun.gameObject.SetActive(true);
				}
				bool flag4 = ownerPlayer.CurrentSecondaryGun && !ownerPlayer.CurrentSecondaryGun.gameObject.activeSelf;
				if (flag4)
				{
					ownerPlayer.CurrentSecondaryGun.gameObject.SetActive(true);
				}
				this.m_cachedPlayer.GunChanged += this.HandleGunChanged;
			}
		}

		// Token: 0x0600049E RID: 1182 RVA: 0x00034164 File Offset: 0x00032364
		private int GetIndexForGun(PlayerController p, int gunID)
		{
			for (int i = 0; i < p.inventory.AllGuns.Count; i++)
			{
				bool flag = p.inventory.AllGuns[i].PickupObjectId == gunID;
				if (flag)
				{
					return i;
				}
			}
			return -1;
		}

		// Token: 0x0600049F RID: 1183 RVA: 0x000341BA File Offset: 0x000323BA
		private void HandleGunChanged(Gun arg1, Gun newGun, bool arg3)
		{
			this.CheckStatus();
		}

		// Token: 0x060004A0 RID: 1184 RVA: 0x000341C4 File Offset: 0x000323C4
		private void DisableEffect()
		{
			bool isCurrentlyActive = this.m_isCurrentlyActive;
			if (isCurrentlyActive)
			{
				this.m_isCurrentlyActive = false;
				this.m_cachedPlayer.inventory.SetDualWielding(false, "synergy");
				this.m_cachedPlayer.GunChanged -= this.HandleGunChanged;
				this.m_cachedPlayer.stats.RecalculateStats(this.m_cachedPlayer, false, false);
				this.m_cachedPlayer = null;
			}
		}

		// Token: 0x040001FE RID: 510
		public string SynergyNameToCheck;

		// Token: 0x040001FF RID: 511
		public int PartnerGunID;

		// Token: 0x04000200 RID: 512
		private Gun m_gun;

		// Token: 0x04000201 RID: 513
		private bool m_isCurrentlyActive;

		// Token: 0x04000202 RID: 514
		private PlayerController m_cachedPlayer;
	}
}
