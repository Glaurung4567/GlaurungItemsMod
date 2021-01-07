using ItemAPI;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
using Random = UnityEngine.Random;

namespace GlaurungItems.Items
{
	class GunzerkingPotion : PlayerItem
	{
		public static void Init()
		{
			string text = "Gunzerking Potion";
			string resourcePath = "GlaurungItems/Resources/gunzerking_potion";
			GameObject gameObject = new GameObject(text);
			GunzerkingPotion item = gameObject.AddComponent<GunzerkingPotion>();
			ItemBuilder.AddSpriteToObject(text, resourcePath, gameObject);
			string shortDesc = "Sexual Tyrannosaurus";
			string longDesc = "A potion allowing the drinker to temporary replicate the prowesses of the legendary Gunzerkers.";
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
				SetDualWield(user);
				/*user.inventory.GunChangeForgiveness = true;
				user.ChangeToGunSlot(0);
				user.inventory.GunChangeForgiveness = false;

				user.inventory.SetDualWielding(true, "gunzerk");
				user.inventory.GunChangeForgiveness = true;
				user.ChangeGun(-1, false, false);
				user.inventory.GunChangeForgiveness = false;*/

			}
		}

		private void EndEffect(PlayerController user)
		{
			if (wasUsed && user)
			{
				foreach(Gun gun in user.inventory.AllGuns)
                {
					if (gun.gameObject.GetComponent<GunzerkingDualWieldForcer>() != null)
					{
						Destroy(gun.gameObject.GetComponent<GunzerkingDualWieldForcer>());
					}
				}
				//user.inventory.SetDualWielding(false, "gunzerk");
			}
			wasUsed = false;
		}

		protected override void OnPreDrop(PlayerController user)
		{
			EndEffect(user);
			base.OnPreDrop(user);
		}

		private void SetDualWield(PlayerController user)
        {
			//Tools.Print("-------------------------------------", "ffffff", true);

			int currentGunIndex = user.inventory.AllGuns.IndexOf(user.CurrentGun);
			
			int partnerID = 0;
			Gun partnerGun = user.inventory.AllGuns[0];
			if (user.inventory.AllGuns.Count == 2)
			{
				if (currentGunIndex == 0)
				{
					partnerGun = user.inventory.AllGuns[1];
				}
				else
				{
					partnerGun = user.inventory.AllGuns[0];
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
				partnerGun = user.inventory.AllGuns[randPartner];
			}

			if(user.CurrentGun.InfiniteAmmo == false && user.CurrentGun.CurrentAmmo < user.CurrentGun.AdjustedMaxAmmo)
            {
				user.CurrentGun.GainAmmo(user.CurrentGun.AdjustedMaxAmmo / 10);
			}

			partnerID = partnerGun.PickupObjectId;
			if (partnerGun.InfiniteAmmo == false && partnerGun.CurrentAmmo < partnerGun.AdjustedMaxAmmo)
			{
				partnerGun.GainAmmo(user.CurrentGun.AdjustedMaxAmmo / 10);
			}

			//Tools.Print(user.CurrentGun.name, "ffffff", true);
			//Tools.Print(PickupObjectDatabase.GetById(partnerID).name, "ffffff", true);

			//to fix an error when primary gun of previous gunzerk is dumped then trigger potion again, the previous gun is picked up wierdly
			//it happens between the SetDualWielding true and SwapDualGuns of  the activate of GunzerkingDualWieldForcer
			Gun m_sec_gunz = (Gun)privateMSecGunzFieldInfo.GetValue(user.inventory);
			if(m_sec_gunz != null)
            {
				privateMSecGunzFieldInfo.SetValue(user.inventory, user.CurrentGun);
			}

			GunzerkingDualWieldForcer dualWieldForcer = user.CurrentGun.gameObject.AddComponent<GunzerkingDualWieldForcer>();
			dualWieldForcer.PartnerGunID = partnerID;
			dualWieldForcer.TargetPlayer = user;			
		}


		private static FieldInfo privateMSecGunzFieldInfo = typeof(GunInventory).GetField("m_currentSecondaryGun", BindingFlags.NonPublic | BindingFlags.Instance);

		private float duration = 10f;
		private bool wasUsed = false;
    }

	/*-------------------------------------------from magic smoke----------------------------------------------*/
	public class GunzerkingDualWieldForcer : MonoBehaviour
	{
		public void Activate()
		{
			try
			{
				if (this.EffectValid(this.TargetPlayer))
				{
					this.m_isCurrentlyActive = true;
					/*
					Tools.Print("user guns", "ffffff", true);
					Tools.Print(TargetPlayer.inventory.CurrentGun.name, "ffffff", true);
					if (TargetPlayer.inventory.CurrentSecondaryGun)
					{
						Tools.Print(TargetPlayer.inventory.CurrentSecondaryGun.name, "ffffff", true);
                    }
                    else
                    {
						Tools.Print("no dual", "ffffff", true);
					}
					foreach (Gun gun in TargetPlayer.inventory.AllGuns)
					{
						Tools.Print(gun.name, "ffffff", true);
					}
					*/

					this.TargetPlayer.inventory.SetDualWielding(true, "synergy");
					
					/*
					Tools.Print("-------------user guns after dual", "ffffff", true);
					Tools.Print(TargetPlayer.inventory.CurrentGun.name, "ffffff", true);
					if (TargetPlayer.inventory.CurrentSecondaryGun)
					{
						Tools.Print(TargetPlayer.inventory.CurrentSecondaryGun.name, "ffffff", true);
					}
					else
					{
						Tools.Print("no dual", "ffffff", true);
					}
					*/
					if(TargetPlayer.inventory.CurrentGun && TargetPlayer.inventory.CurrentSecondaryGun && TargetPlayer.inventory.CurrentGun != TargetPlayer.inventory.CurrentSecondaryGun)
                    {
						//this.TargetPlayer.inventory.SetDualWielding(false, "synergy");
						Tools.Print("----SecondDualGunzerkingFuckfrickfraggingbastitch----", "ffffff", true);
						//return;
					}



					int indexForGun = this.GetIndexForGun(this.TargetPlayer, this.gun.PickupObjectId);
					int indexForGun2 = this.GetIndexForGun(this.TargetPlayer, this.PartnerGunID);
					this.TargetPlayer.inventory.SwapDualGuns();

					/*
					Tools.Print("---------------user guns after swap", "ffffff", true);
					Tools.Print(TargetPlayer.inventory.CurrentGun.name, "ffffff", true);
					if (TargetPlayer.inventory.CurrentSecondaryGun)
					{
						Tools.Print(TargetPlayer.inventory.CurrentSecondaryGun.name, "ffffff", true);
					}
					else
					{
						Tools.Print("no dual", "ffffff", true);
					}
					*/

					if (indexForGun >= 0 && indexForGun2 >= 0)
					{
						while (this.TargetPlayer.inventory.CurrentGun.PickupObjectId != this.PartnerGunID)
						{
							this.TargetPlayer.inventory.ChangeGun(1, false, false);
						}
					}

					/*
					Tools.Print("-------------user guns after while", "ffffff", true);
					Tools.Print(TargetPlayer.inventory.CurrentGun.name, "ffffff", true);
					if (TargetPlayer.inventory.CurrentSecondaryGun)
					{
						Tools.Print(TargetPlayer.inventory.CurrentSecondaryGun.name, "ffffff", true);
					}
					else
					{
						Tools.Print("no dual", "ffffff", true);
					}
					*/

					this.TargetPlayer.inventory.SwapDualGuns();

					/*
					Tools.Print("-------------------user guns after swap2", "ffffff", true);
					Tools.Print(TargetPlayer.inventory.CurrentGun.name, "ffffff", true);
					if (TargetPlayer.inventory.CurrentSecondaryGun)
					{
						Tools.Print(TargetPlayer.inventory.CurrentSecondaryGun.name, "ffffff", true);
					}
					else
					{
						Tools.Print("no dual", "ffffff", true);
					}
					*/

					if (this.TargetPlayer.CurrentGun && !this.TargetPlayer.CurrentGun.gameObject.activeSelf)
					{
						this.TargetPlayer.CurrentGun.gameObject.SetActive(true);
					}
					if (this.TargetPlayer.CurrentSecondaryGun && !this.TargetPlayer.CurrentSecondaryGun.gameObject.activeSelf)
					{
						this.TargetPlayer.CurrentSecondaryGun.gameObject.SetActive(true);
					}
					this.TargetPlayer.GunChanged += this.HandleGunChanged;
					//this.TargetPlayer.inventory.GunLocked.SetOverride("gunzerking", true, null);
				}
			}
			catch(Exception e)
            {
				Tools.PrintException(e);
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
				//this.TargetPlayer.inventory.GunLocked.RemoveOverride("gunzerking");
				this.TargetPlayer.inventory.SetDualWielding(false, "synergy");
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
			DisableEffect();
		}

		public int PartnerGunID;

		public PlayerController TargetPlayer;

		public Gun gun;

		private bool m_isCurrentlyActive;
	}
}
