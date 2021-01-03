using ItemAPI;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

namespace GlaurungItems.Items
{
	//error when primary gun is dumped need fix
	class GunzerkingPotion : PlayerItem
	{
		public static void Init()
		{
			string text = "Gunzerking Potion";
			string resourcePath = "GlaurungItems/Resources/acme_crate";
			GameObject gameObject = new GameObject(text);
			GunzerkingPotion item = gameObject.AddComponent<GunzerkingPotion>();
			ItemBuilder.AddSpriteToObject(text, resourcePath, gameObject);
			string shortDesc = "Sexual Tyrannosaurus";
			string longDesc = "";
			item.SetupItem(shortDesc, longDesc, "gl");
			item.SetCooldownType(ItemBuilder.CooldownType.Damage, 1);
			item.quality = ItemQuality.A;
		}

		public override bool CanBeUsed(PlayerController user)
		{
			return !user.inventory.DualWielding && user.inventory.AllGuns != null && (user.inventory.AllGuns.Count > 1) && user.inventory.GunLocked.BaseValue == false;
		}

		protected override void DoEffect(PlayerController user)
		{
			this.StartEffect(user);
			base.StartCoroutine(ItemBuilder.HandleDuration(this, this.duration, user, new Action<PlayerController>(this.EndEffect)));
		}

		private void StartEffect(PlayerController user)
		{
			wasUsed = true;
			if (user)
			{
				user.inventory.FrameUpdate();
				user.inventory.SwapDualGuns();
				SetDualWield(user);
			}
		}

		private void SetDualWield(PlayerController user)
        {
			Tools.Print(user.inventory.CurrentGun.name, "ffffff", true);
			if (user.inventory.CurrentSecondaryGun != null)
            {
				Tools.Print(user.inventory.CurrentSecondaryGun.name, "ffffff", true);
            }

			int currentGunIndex = user.inventory.AllGuns.IndexOf(user.CurrentGun);
			Tools.Print(currentGunIndex, "ffffff", true);

			int partnerID = 0;
			if (user.inventory.AllGuns.Count == 2)
			{
				if (currentGunIndex == 0)
				{
					partnerID = user.inventory.AllGuns[1].PickupObjectId;
				}
				else
				{
					partnerID = user.inventory.AllGuns[0].PickupObjectId;
				}
			}
			else
			{
				int randPartner = 0;
				if (currentGunIndex == 0)
				{
					randPartner = Random.Range(1, currentGunIndex);
				}
				else if (currentGunIndex == (user.inventory.AllGuns.Count - 1))
				{
					randPartner = Random.Range(0, currentGunIndex);
				}
				else if (Random.value <= 0.5f)
				{
					randPartner = Random.Range(0, currentGunIndex);
				}
				else
				{
					randPartner = Random.Range(currentGunIndex + 1, user.inventory.AllGuns.Count);
				}
				partnerID = user.inventory.AllGuns[randPartner].PickupObjectId;
			}
			Tools.Print(PickupObjectDatabase.GetById(partnerID).name, "ffffff", true);
			Tools.Print(user.CurrentGun.name, "ffffff", true);
			Tools.Print(user.inventory.AllGuns.Count, "ffffff", true);

			GunzerkingDualWieldForcer dualWieldForcer = user.CurrentGun.gameObject.AddComponent<GunzerkingDualWieldForcer>();
			dualWieldForcer.gun = user.CurrentGun;
			dualWieldForcer.PartnerGunID = partnerID;
			dualWieldForcer.TargetPlayer = user;

		}

		private void EndEffect(PlayerController user)
		{
			if (wasUsed && user)
			{
				foreach(Gun gun in user.inventory.AllGuns)
                {
					if (gun.gameObject.GetComponent<GunzerkingDualWieldForcer>() != null)
					{
						Tools.Print("endEffect destroy", "ffffff", true);
						Destroy(gun.gameObject.GetComponent<GunzerkingDualWieldForcer>());
					}
				}
			}
			wasUsed = false;
		}

		protected override void OnPreDrop(PlayerController user)
		{
			EndEffect(user);
			base.OnPreDrop(user);
		}

		private float duration = 10f;
		private bool wasUsed = false;
		private bool cooldown;

    }

	/*-------------------------------------------from magic smoke----------------------------------------------*/
	public class GunzerkingDualWieldForcer : MonoBehaviour
	{
		public void Activate()
		{
			if (this.EffectValid(this.TargetPlayer))
			{
				this.m_isCurrentlyActive = true;
				this.TargetPlayer.inventory.SetDualWielding(true, "DualWieldForcer");
				int indexForGun = this.GetIndexForGun(this.TargetPlayer, this.gun.PickupObjectId);
				int indexForGun2 = this.GetIndexForGun(this.TargetPlayer, this.PartnerGunID);
				//this.TargetPlayer.inventory.SwapDualGuns();
				if (indexForGun >= 0 && indexForGun2 >= 0)
				{
					while (this.TargetPlayer.inventory.CurrentGun.PickupObjectId != this.PartnerGunID)
					{
						this.TargetPlayer.inventory.ChangeGun(1, false, false);
					}
				}
				//this.TargetPlayer.inventory.SwapDualGuns();
				if (this.TargetPlayer.CurrentGun && !this.TargetPlayer.CurrentGun.gameObject.activeSelf)
				{
					this.TargetPlayer.CurrentGun.gameObject.SetActive(true);
				}
				if (this.TargetPlayer.CurrentSecondaryGun && !this.TargetPlayer.CurrentSecondaryGun.gameObject.activeSelf)
				{
					this.TargetPlayer.CurrentSecondaryGun.gameObject.SetActive(true);
				}
				this.TargetPlayer.GunChanged += this.HandleGunChanged;
				this.TargetPlayer.inventory.GunLocked.SetOverride("gunzerking", true, null);
			}
		}

		public void Awake()
		{
			this.gun = base.GetComponent<Gun>();
		}

		private void CheckStatus()
		{
			bool isCurrentlyActive = this.m_isCurrentlyActive;
			bool flag = isCurrentlyActive;
			if (flag)
			{
				if (!this.PlayerUsingCorrectGuns() || !this.EffectValid(this.TargetPlayer))
				{
					Console.WriteLine("DISABLING EFFECT");
					this.DisableEffect();
				}
			}
			else
			{
				if (this.gun && this.gun.CurrentOwner is PlayerController)
				{
					PlayerController playerController = this.gun.CurrentOwner as PlayerController;
					if (playerController.inventory.DualWielding && playerController.CurrentSecondaryGun.PickupObjectId == this.gun.PickupObjectId && playerController.CurrentGun.PickupObjectId == this.PartnerGunID)
					{
						this.m_isCurrentlyActive = true;
						this.TargetPlayer = playerController;
					}
					else
					{
						this.Activate();
					}
				}
			}
		}

		private void DisableEffect(bool forceDisable = false)
		{
			if (this.m_isCurrentlyActive || forceDisable)
			{
				this.m_isCurrentlyActive = false;
				this.TargetPlayer.inventory.GunLocked.RemoveOverride("gunzerking");
				this.TargetPlayer.inventory.SetDualWielding(false, "DualWieldForcer");
				this.TargetPlayer.GunChanged -= this.HandleGunChanged;
				this.TargetPlayer.stats.RecalculateStats(this.TargetPlayer, false, false);
				this.TargetPlayer = null;
			}
		}

		private bool EffectValid(PlayerController p)
		{
			bool result;
			if (!p)
			{
				Console.WriteLine("NULL PLAYER");
				result = false;
			}
			else
			{
				bool flag3 = this.gun.CurrentAmmo == 0;
				bool flag4 = flag3;
				if (flag4)
				{
					Console.WriteLine("CURAMMO 0");
					result = false;
				}
				else
				{
					bool flag5 = !this.m_isCurrentlyActive;
					bool flag6 = flag5;
					if (flag6)
					{
						int indexForGun = this.GetIndexForGun(p, this.PartnerGunID);
						bool flag7 = indexForGun < 0;
						bool flag8 = flag7;
						if (flag8)
						{
							Console.WriteLine("IDX4GUN <0");
							return false;
						}
						bool flag9 = p.inventory.AllGuns[indexForGun].CurrentAmmo == 0;
						bool flag10 = flag9;
						if (flag10)
						{
							Console.WriteLine("PARTNERAMMO 0");
							return false;
						}
					}
					else
					{
						bool flag11 = p.CurrentSecondaryGun != null && p.CurrentSecondaryGun.PickupObjectId == this.PartnerGunID && p.CurrentSecondaryGun.CurrentAmmo == 0;
						bool flag12 = flag11;
						if (flag12)
						{
							Console.WriteLine("SECONDARYAMMO 0");
							return false;
						}
					}
					Console.WriteLine("EFFECT VALID");
					result = true;
				}
			}
			return result;
		}

		private int GetIndexForGun(PlayerController p, int gunID)
		{
			for (int i = 0; i < p.inventory.AllGuns.Count; i++)
			{
				bool flag = p.inventory.AllGuns[i].PickupObjectId == gunID;
				bool flag2 = flag;
				if (flag2)
				{
					return i;
				}
			}
			return -1;
		}

		private void HandleGunChanged(Gun arg1, Gun newGun, bool arg3)
		{
			this.CheckStatus();
		}

		private bool PlayerUsingCorrectGuns()
		{
			return this.gun && this.gun.CurrentOwner && this.TargetPlayer && this.TargetPlayer.inventory.DualWielding && (!(this.TargetPlayer.CurrentGun != this.gun) || this.TargetPlayer.CurrentGun.PickupObjectId == this.PartnerGunID) && (!(this.TargetPlayer.CurrentSecondaryGun != this.gun) || this.TargetPlayer.CurrentSecondaryGun.PickupObjectId == this.PartnerGunID);
		}

		private void Update()
		{
			this.CheckStatus();
		}

		private void OnDestroy()
        {
			Tools.Print(this.gun.name, "ffffff", true);
			Tools.Print(PickupObjectDatabase.GetById(PartnerGunID).name, "ffffff", true);
			DisableEffect(true);
		}

		public int PartnerGunID;

		public PlayerController TargetPlayer;

		public Gun gun;

		private bool m_isCurrentlyActive;
	}
}
