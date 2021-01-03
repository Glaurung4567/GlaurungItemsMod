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
				//user.inventory.GunLocked.SetOverride("gunzerking", true, null);
				SetDualWield(user);
                //user.inventory.OnGunChanged += Inventory_OnGunChanged;
				//user.inventory.GunChangeForgiveness = true;
				//user.ChangeGun(-1, false, false);
				//user.inventory.GunChangeForgiveness = false;
			}
		}

        private void Inventory_OnGunChanged(Gun previous, Gun current, Gun previousSecondary, Gun currentSecondary, bool newGun)
        {

            if (!cooldown)
            {
				if (previous.gameObject.GetComponent<DualWieldForcer>() != null)
				{
					Destroy(previous.gameObject.GetComponent<DualWieldForcer>());
				}
				GameManager.Instance.StartCoroutine(StartDualSwitch(base.LastOwner));
            }
		}

        private IEnumerator StartDualSwitch(PlayerController user)
        {
			cooldown = true;
			SetDualWield(user);
			yield return new WaitForSeconds(0.1f);
			cooldown = false;
			yield break;
		}

		private void SetDualWield(PlayerController user)
        {
			//user.ChangeToGunSlot(Random.Range(1, user.inventory.AllGuns.Count));
			int currentGunIndex = user.inventory.AllGuns.IndexOf(user.CurrentGun);
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
			DualWieldForcer dualWieldForcer = user.CurrentGun.gameObject.AddComponent<DualWieldForcer>();
			dualWieldForcer.Gun = user.CurrentGun;
			dualWieldForcer.PartnerGunID = partnerID;
			dualWieldForcer.TargetPlayer = user;
			//user.inventory.GunLocked.SetOverride("gunzerking", true, null);

		}

		private void EndEffect(PlayerController user)
		{
			if (wasUsed && user)
			{
                if(user.CurrentGun.gameObject.GetComponent<DualWieldForcer>() != null)
				{
					Destroy(user.CurrentGun.gameObject.GetComponent<DualWieldForcer>());
                }
				//user.inventory.OnGunChanged -= Inventory_OnGunChanged;
				user.ChangeGun(1);
				//user.inventory.GunLocked.RemoveOverride("gunzerking");
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
	public class DualWieldForcer : MonoBehaviour
	{
		public void Activate()
		{
			bool flag = this.EffectValid(this.TargetPlayer);
			bool flag2 = flag;
			if (flag2)
			{
				this.m_isCurrentlyActive = true;
				this.TargetPlayer.inventory.SetDualWielding(true, "DualWieldForcer");
				int indexForGun = this.GetIndexForGun(this.TargetPlayer, this.Gun.PickupObjectId);
				int indexForGun2 = this.GetIndexForGun(this.TargetPlayer, this.PartnerGunID);
				this.TargetPlayer.inventory.SwapDualGuns();
				bool flag3 = indexForGun >= 0 && indexForGun2 >= 0;
				bool flag4 = flag3;
				if (flag4)
				{
					while (this.TargetPlayer.inventory.CurrentGun.PickupObjectId != this.PartnerGunID)
					{
						this.TargetPlayer.inventory.ChangeGun(1, false, false);
					}
				}
				this.TargetPlayer.inventory.SwapDualGuns();
				bool flag5 = this.TargetPlayer.CurrentGun && !this.TargetPlayer.CurrentGun.gameObject.activeSelf;
				bool flag6 = flag5;
				if (flag6)
				{
					this.TargetPlayer.CurrentGun.gameObject.SetActive(true);
				}
				bool flag7 = this.TargetPlayer.CurrentSecondaryGun && !this.TargetPlayer.CurrentSecondaryGun.gameObject.activeSelf;
				bool flag8 = flag7;
				if (flag8)
				{
					this.TargetPlayer.CurrentSecondaryGun.gameObject.SetActive(true);
				}
				this.TargetPlayer.GunChanged += this.HandleGunChanged;
			}
		}

		public void Awake()
		{
			this.Gun = base.GetComponent<Gun>();
		}

		private void CheckStatus()
		{
			bool isCurrentlyActive = this.m_isCurrentlyActive;
			bool flag = isCurrentlyActive;
			if (flag)
			{
				bool flag2 = !this.PlayerUsingCorrectGuns() || !this.EffectValid(this.TargetPlayer);
				bool flag3 = flag2;
				if (flag3)
				{
					Console.WriteLine("DISABLING EFFECT");
					this.DisableEffect();
				}
			}
			else
			{
				bool flag4 = this.Gun && this.Gun.CurrentOwner is PlayerController;
				bool flag5 = flag4;
				if (flag5)
				{
					PlayerController playerController = this.Gun.CurrentOwner as PlayerController;
					bool flag6 = playerController.inventory.DualWielding && playerController.CurrentSecondaryGun.PickupObjectId == this.Gun.PickupObjectId && playerController.CurrentGun.PickupObjectId == this.PartnerGunID;
					bool flag7 = flag6;
					if (flag7)
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

		private void DisableEffect()
		{
			bool isCurrentlyActive = this.m_isCurrentlyActive;
			bool flag = isCurrentlyActive;
			if (flag)
			{
				this.m_isCurrentlyActive = false;
				this.TargetPlayer.inventory.SetDualWielding(false, "DualWieldForcer");
				this.TargetPlayer.GunChanged -= this.HandleGunChanged;
				this.TargetPlayer.stats.RecalculateStats(this.TargetPlayer, false, false);
				this.TargetPlayer = null;
			}
		}

		private bool EffectValid(PlayerController p)
		{
			bool flag = !p;
			bool flag2 = flag;
			bool result;
			if (flag2)
			{
				Console.WriteLine("NULL PLAYER");
				result = false;
			}
			else
			{
				bool flag3 = this.Gun.CurrentAmmo == 0;
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
			return this.Gun && this.Gun.CurrentOwner && this.TargetPlayer && this.TargetPlayer.inventory.DualWielding && (!(this.TargetPlayer.CurrentGun != this.Gun) || this.TargetPlayer.CurrentGun.PickupObjectId == this.PartnerGunID) && (!(this.TargetPlayer.CurrentSecondaryGun != this.Gun) || this.TargetPlayer.CurrentSecondaryGun.PickupObjectId == this.PartnerGunID);
		}

		private void Update()
		{
			this.CheckStatus();
		}

		private void OnDestroy()
        {
			DisableEffect();
		}

		public int PartnerGunID;

		public PlayerController TargetPlayer;

		public Gun Gun;

		private bool m_isCurrentlyActive;
	}
}
