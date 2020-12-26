using System;
using System.Reflection;
using UnityEngine;

namespace ItemAPI
{
	internal class AdvancedGunBehavior : MonoBehaviour
	{
		protected virtual void Update()
		{
			bool flag = this.Player != null;
			if (flag)
			{
				this.lastPlayer = this.Player;
				bool flag2 = !this.everPickedUpByPlayer;
				if (flag2)
				{
					this.everPickedUpByPlayer = true;
				}
			}
			bool flag3 = this.Player != null && !this.pickedUpLast;
			if (flag3)
			{
				this.OnPickup(this.Player);
				this.pickedUpLast = true;
			}
			bool flag4 = this.Player == null && this.pickedUpLast;
			if (flag4)
			{
				bool flag5 = this.lastPlayer != null;
				if (flag5)
				{
					this.OnPostDrop(this.lastPlayer);
					this.lastPlayer = null;
				}
				this.pickedUpLast = false;
			}
			bool flag6 = this.gun != null && !this.gun.IsReloading && !this.hasReloaded;
			if (flag6)
			{
				this.hasReloaded = true;
			}
			this.gun.PreventNormalFireAudio = this.preventNormalFireAudio;
			this.gun.OverrideNormalFireAudioEvent = this.overrrideNormalFireAudio;
		}

		// Token: 0x060000B5 RID: 181 RVA: 0x000083DC File Offset: 0x000065DC
		public virtual void Start()
		{
			this.gun = base.GetComponent<Gun>();
			Gun gun = this.gun;
			gun.OnInitializedWithOwner = (Action<GameActor>)Delegate.Combine(gun.OnInitializedWithOwner, new Action<GameActor>(this.OnInitializedWithOwner));
			Gun gun2 = this.gun;
			gun2.PostProcessProjectile = (Action<Projectile>)Delegate.Combine(gun2.PostProcessProjectile, new Action<Projectile>(this.PostProcessProjectile));
			Gun gun3 = this.gun;
			gun3.PostProcessVolley = (Action<ProjectileVolleyData>)Delegate.Combine(gun3.PostProcessVolley, new Action<ProjectileVolleyData>(this.PostProcessVolley));
			Gun gun4 = this.gun;
			gun4.OnDropped = (Action)Delegate.Combine(gun4.OnDropped, new Action(this.OnDropped));
			Gun gun5 = this.gun;
			gun5.OnAutoReload = (Action<PlayerController, Gun>)Delegate.Combine(gun5.OnAutoReload, new Action<PlayerController, Gun>(this.OnAutoReload));
			Gun gun6 = this.gun;
			gun6.OnReloadPressed = (Action<PlayerController, Gun, bool>)Delegate.Combine(gun6.OnReloadPressed, new Action<PlayerController, Gun, bool>(this.OnReloadPressed));
			Gun gun7 = this.gun;
			gun7.OnFinishAttack = (Action<PlayerController, Gun>)Delegate.Combine(gun7.OnFinishAttack, new Action<PlayerController, Gun>(this.OnFinishAttack));
			Gun gun8 = this.gun;
			gun8.OnPostFired = (Action<PlayerController, Gun>)Delegate.Combine(gun8.OnPostFired, new Action<PlayerController, Gun>(this.OnPostFired));
			Gun gun9 = this.gun;
			gun9.OnAmmoChanged = (Action<PlayerController, Gun>)Delegate.Combine(gun9.OnAmmoChanged, new Action<PlayerController, Gun>(this.OnAmmoChanged));
			Gun gun10 = this.gun;
			gun10.OnBurstContinued = (Action<PlayerController, Gun>)Delegate.Combine(gun10.OnBurstContinued, new Action<PlayerController, Gun>(this.OnBurstContinued));
			Gun gun11 = this.gun;
			gun11.OnPreFireProjectileModifier = (Func<Gun, Projectile, ProjectileModule, Projectile>)Delegate.Combine(gun11.OnPreFireProjectileModifier, new Func<Gun, Projectile, ProjectileModule, Projectile>(this.OnPreFireProjectileModifier));
		}

		// Token: 0x060000B6 RID: 182 RVA: 0x000085AE File Offset: 0x000067AE
		public virtual void OnInitializedWithOwner(GameActor actor)
		{
		}

		// Token: 0x060000B7 RID: 183 RVA: 0x000085B1 File Offset: 0x000067B1
		public virtual void PostProcessProjectile(Projectile projectile)
		{
		}

		// Token: 0x060000B8 RID: 184 RVA: 0x000085B4 File Offset: 0x000067B4
		public virtual void PostProcessVolley(ProjectileVolleyData volley)
		{
		}

		// Token: 0x060000B9 RID: 185 RVA: 0x000085B7 File Offset: 0x000067B7
		public virtual void OnDropped()
		{
		}

		// Token: 0x060000BA RID: 186 RVA: 0x000085BA File Offset: 0x000067BA
		public virtual void OnAutoReload(PlayerController player, Gun gun)
		{
		}

		// Token: 0x060000BB RID: 187 RVA: 0x000085C0 File Offset: 0x000067C0
		public virtual void OnReloadPressed(PlayerController player, Gun gun, bool bSOMETHING)
		{
			bool flag = this.hasReloaded;
			if (flag)
			{
				this.OnReload(player, gun);
				this.hasReloaded = false;
			}
		}

		// Token: 0x060000BC RID: 188 RVA: 0x000085EC File Offset: 0x000067EC
		public virtual void OnReload(PlayerController player, Gun gun)
		{
			bool flag = this.preventNormalReloadAudio;
			if (flag)
			{
				AkSoundEngine.PostEvent("Stop_WPN_All", base.gameObject);
				bool flag2 = !string.IsNullOrEmpty(this.overrideNormalReloadAudio);
				if (flag2)
				{
					AkSoundEngine.PostEvent(this.overrideNormalReloadAudio, base.gameObject);
				}
			}
		}

		// Token: 0x060000BD RID: 189 RVA: 0x0000863D File Offset: 0x0000683D
		public virtual void OnFinishAttack(PlayerController player, Gun gun)
		{
		}

		// Token: 0x060000BE RID: 190 RVA: 0x00008640 File Offset: 0x00006840
		public virtual void OnPostFired(PlayerController player, Gun gun)
		{
			bool isHeroSword = gun.IsHeroSword;
			if (isHeroSword)
			{
				bool flag = (float)AdvancedGunBehavior.heroSwordCooldown.GetValue(gun) == 0.5f;
				if (flag)
				{
					this.OnHeroSwordCooldownStarted(player, gun);
				}
			}
		}

		// Token: 0x060000BF RID: 191 RVA: 0x00008680 File Offset: 0x00006880
		public virtual void OnHeroSwordCooldownStarted(PlayerController player, Gun gun)
		{
		}

		// Token: 0x060000C0 RID: 192 RVA: 0x00008683 File Offset: 0x00006883
		public virtual void OnAmmoChanged(PlayerController player, Gun gun)
		{
		}

		// Token: 0x060000C1 RID: 193 RVA: 0x00008686 File Offset: 0x00006886
		public virtual void OnBurstContinued(PlayerController player, Gun gun)
		{
		}

		// Token: 0x060000C2 RID: 194 RVA: 0x0000868C File Offset: 0x0000688C
		public virtual Projectile OnPreFireProjectileModifier(Gun gun, Projectile projectile, ProjectileModule mod)
		{
			return projectile;
		}

		// Token: 0x060000C4 RID: 196 RVA: 0x000086C5 File Offset: 0x000068C5
		protected virtual void OnPickup(PlayerController player)
		{
		}

		// Token: 0x060000C5 RID: 197 RVA: 0x000086C8 File Offset: 0x000068C8
		protected virtual void OnPostDrop(PlayerController player)
		{
		}

		// Token: 0x17000001 RID: 1
		// (get) Token: 0x060000C6 RID: 198 RVA: 0x000086CC File Offset: 0x000068CC
		public bool PickedUp
		{
			get
			{
				return this.gun.CurrentOwner != null;
			}
		}

		// Token: 0x17000002 RID: 2
		// (get) Token: 0x060000C7 RID: 199 RVA: 0x000086F0 File Offset: 0x000068F0
		public PlayerController Player
		{
			get
			{
				bool flag = this.gun.CurrentOwner is PlayerController;
				PlayerController result;
				if (flag)
				{
					result = (this.gun.CurrentOwner as PlayerController);
				}
				else
				{
					result = null;
				}
				return result;
			}
		}

		public float HeroSwordCooldown
		{
			get
			{
				bool flag = this.gun != null;
				float result;
				if (flag)
				{
					result = (float)AdvancedGunBehavior.heroSwordCooldown.GetValue(this.gun);
				}
				else
				{
					result = -1f;
				}
				return result;
			}
			set
			{
				bool flag = this.gun != null;
				if (flag)
				{
					AdvancedGunBehavior.heroSwordCooldown.SetValue(this.gun, value);
				}
			}
		}

		// Token: 0x17000004 RID: 4
		// (get) Token: 0x060000CA RID: 202 RVA: 0x000087A8 File Offset: 0x000069A8
		public GameActor Owner
		{
			get
			{
				return this.gun.CurrentOwner;
			}
		}

		// Token: 0x17000005 RID: 5
		// (get) Token: 0x060000CB RID: 203 RVA: 0x000087C8 File Offset: 0x000069C8
		public bool OwnerIsPlayer
		{
			get
			{
				return this.Player != null;
			}
		}

		// Token: 0x04000063 RID: 99
		private bool pickedUpLast = false;

		// Token: 0x04000064 RID: 100
		private PlayerController lastPlayer = null;

		// Token: 0x04000065 RID: 101
		public bool everPickedUpByPlayer = false;

		// Token: 0x04000066 RID: 102
		protected Gun gun;

		// Token: 0x04000067 RID: 103
		private bool hasReloaded = true;

		// Token: 0x04000068 RID: 104
		protected bool preventNormalFireAudio;

		protected bool preventNormalReloadAudio;

		protected string overrrideNormalFireAudio;

		protected string overrideNormalReloadAudio;

		private static FieldInfo heroSwordCooldown = typeof(Gun).GetField("HeroSwordCooldown", BindingFlags.Instance | BindingFlags.NonPublic);
	}
}
