using System;
using System.Collections.Generic;
using Dungeonator;
using UnityEngine;
using UnityEngine.Serialization;

namespace GlaurungItems.Items
{
	public class BasicTrapEnemyController : TrapController, IPlaceConfigurable
	{
		public BasicTrapEnemyController()
		{
			this.triggerTimerDelay = 1f;
			this.LocalTimeScale = 1f;
		}

		protected BasicTrapEnemyController.State state
		{
			get
			{
				return this.m_state;
			}
			set
			{
				if (this.m_state != value)
				{
					this.EndState(this.m_state);
					this.m_state = value;
					this.BeginState(this.m_state);
				}
			}
		}

		public virtual void Awake()
		{
			if (base.specRigidbody)
			{
				SpeculativeRigidbody specRigidbody = base.specRigidbody;
				specRigidbody.OnTriggerCollision = (SpeculativeRigidbody.OnTriggerDelegate)Delegate.Combine(specRigidbody.OnTriggerCollision, new SpeculativeRigidbody.OnTriggerDelegate(this.OnTriggerCollision));
			}
			if (this.animateChildren)
			{
				this.m_childrenAnimators = base.GetComponentsInChildren<tk2dSpriteAnimator>();
			}
			if (this.triggerOnBlank || this.triggerOnExplosion)
			{
				//StaticReferenceManager.AllTriggeredTraps.Add(this);
			}
		}

		public override void Start()
		{
			this.m_parentRoom = GameManager.Instance.Dungeon.data.GetAbsoluteRoomFromPosition(base.transform.position.IntXY(VectorConversions.Round));
			this.m_cachedPosition = base.transform.position.IntXY(VectorConversions.Floor);
			this.m_cachedPixelMin = this.m_cachedPosition * PhysicsEngine.Instance.PixelsPerUnit + new IntVector2(this.footprintBuffer.left, this.footprintBuffer.bottom);
			this.m_cachedPixelMax = (this.m_cachedPosition + new IntVector2(this.placeableWidth, this.placeableHeight)) * PhysicsEngine.Instance.PixelsPerUnit - IntVector2.One - new IntVector2(this.footprintBuffer.right, this.footprintBuffer.top);
			if (this.triggerMethod == BasicTrapEnemyController.TriggerMethod.Timer)
			{
				this.m_triggerTimerDelayArray = new List<float>();
				if (this.triggerTimerDelay != 0f)
				{
					this.m_triggerTimerDelayArray.Add(this.triggerTimerDelay);
				}
				if (this.triggerTimerDelay1 != 0f)
				{
					this.m_triggerTimerDelayArray.Add(this.triggerTimerDelay1);
				}
				if (this.m_triggerTimerDelayArray.Count == 0)
				{
					this.m_triggerTimerDelayArray.Add(0f);
				}
				this.m_triggerTimer = this.triggerTimerOffset;
			}
			for (int i = 0; i < this.activeVfx.Count; i++)
			{
				if (this.activeVfx[i])
				{
					this.activeVfx[i].onlyDisable = true;
					this.activeVfx[i].Disable();
				}
			}
			base.Start();
		}

		public virtual void Update()
		{
			if (Time.timeScale == 0f)
			{
				return;
			}
			if (!GameManager.Instance.PlayerIsNearRoom(this.m_parentRoom))
			{
				return;
			}
			this.m_stateTimer = Mathf.Max(0f, this.m_stateTimer - BraveTime.DeltaTime) * this.LocalTimeScale;
			this.m_triggerTimer -= BraveTime.DeltaTime * this.LocalTimeScale;
			this.m_disabledTimer = Mathf.Max(0f, this.m_disabledTimer - BraveTime.DeltaTime * this.LocalTimeScale);
			if (this.triggerMethod == BasicTrapEnemyController.TriggerMethod.Timer && this.m_triggerTimer < 0f)
			{
				this.TriggerTrap(null);
			}
			this.UpdateState();
		}

		protected override void OnDestroy()
		{
			if (this.triggerOnBlank || this.triggerOnExplosion)
			{
				//StaticReferenceManager.AllTriggeredTraps.Remove(this);
			}
			base.OnDestroy();
		}

		public override GameObject InstantiateObject(RoomHandler targetRoom, IntVector2 loc, bool deferConfiguration = false)
		{
			return base.InstantiateObject(targetRoom, loc, deferConfiguration);
		}

		private void OnTriggerCollision(SpeculativeRigidbody rigidbody, SpeculativeRigidbody source, CollisionData collisionData)
		{
			PlayerController component = rigidbody.GetComponent<PlayerController>();
			if (component)
			{
				bool flag = component.spriteAnimator.QueryGroundedFrame() && !component.IsFlying;
				if (this.triggerMethod == BasicTrapEnemyController.TriggerMethod.SpecRigidbody && this.m_state == BasicTrapEnemyController.State.Ready && flag)
				{
					this.TriggerTrap(rigidbody);
				}
				if (this.damageMethod == BasicTrapEnemyController.DamageMethod.SpecRigidbody && this.m_state == BasicTrapEnemyController.State.Active && (flag || this.damagesFlyingPlayers))
				{
					this.Damage(rigidbody);
				}
			}
		}

		public void Trigger()
		{
			this.TriggerTrap(null);
		}

		protected virtual void TriggerTrap(SpeculativeRigidbody target)
		{
			if (this.m_disabledTimer > 0f)
			{
				return;
			}
			if (this.m_state == BasicTrapEnemyController.State.Ready)
			{
				this.state = BasicTrapEnemyController.State.Triggered;
				if (this.damageMethod == BasicTrapEnemyController.DamageMethod.OnTrigger)
				{
					this.Damage(target);
				}
			}
		}

		protected bool ArePlayersNearby()
		{
			for (int i = 0; i < GameManager.Instance.AllPlayers.Length; i++)
			{
				if (GameManager.Instance.AllPlayers[i] && GameManager.Instance.AllPlayers[i].CurrentRoom == this.m_parentRoom)
				{
					return true;
				}
			}
			return false;
		}

		protected bool ArePlayersSortOfNearby()
		{
			for (int i = 0; i < GameManager.Instance.AllPlayers.Length; i++)
			{
				if (GameManager.Instance.AllPlayers[i] && GameManager.Instance.AllPlayers[i].CurrentRoom != null && GameManager.Instance.AllPlayers[i].CurrentRoom.connectedRooms != null && GameManager.Instance.AllPlayers[i].CurrentRoom.connectedRooms.Contains(this.m_parentRoom))
				{
					return true;
				}
			}
			return false;
		}

		protected virtual void BeginState(BasicTrapEnemyController.State newState)
		{
			bool flag = this.ArePlayersNearby();
			bool flag2 = flag || this.ArePlayersSortOfNearby();
			if (this.m_state == BasicTrapEnemyController.State.Triggered)
			{
				this.PlayAnimation(this.triggerAnimName);
				this.m_stateTimer = this.triggerDelay;
				if (this.triggerMethod == BasicTrapEnemyController.TriggerMethod.Timer)
				{
					this.m_triggerTimer += this.GetNextTriggerTimerDelay();
				}
				if (this.m_stateTimer == 0f)
				{
					this.state = BasicTrapEnemyController.State.Active;
				}
				if (flag)
				{
					AkSoundEngine.PostEvent("Play_ENV_trap_trigger", base.gameObject);
				}
			}
			else if (this.m_state == BasicTrapEnemyController.State.Active)
			{
				this.PlayAnimation(this.activeAnimName);
				if (flag2)
				{
					this.SpawnVfx(this.activeVfx);
				}
				this.m_stateTimer = this.activeTime;
				if (this.m_stateTimer == 0f)
				{
					this.state = BasicTrapEnemyController.State.Resetting;
				}
				if (flag)
				{
					AkSoundEngine.PostEvent("Play_ENV_trap_active", base.gameObject);
				}
			}
			else if (this.m_state == BasicTrapEnemyController.State.Resetting)
			{
				this.PlayAnimation(this.resetAnimName);
				this.m_stateTimer = this.resetDelay;
				if (this.m_stateTimer == 0f)
				{
					this.state = BasicTrapEnemyController.State.Ready;
				}
				if (flag)
				{
					AkSoundEngine.PostEvent("Play_ENV_trap_reset", base.gameObject);
				}
			}
		}

		protected virtual void UpdateState()
		{
			if (this.m_state == BasicTrapEnemyController.State.Ready)
			{
				if (this.triggerMethod == BasicTrapEnemyController.TriggerMethod.PlaceableFootprint)
				{
					SpeculativeRigidbody playerRigidbodyInFootprint = this.GetPlayerRigidbodyInFootprint();
					if (playerRigidbodyInFootprint)
					{
						bool flag = playerRigidbodyInFootprint.spriteAnimator.QueryGroundedFrame();
						if (playerRigidbodyInFootprint.gameActor != null)
						{
							flag = (flag && !playerRigidbodyInFootprint.gameActor.IsFlying);
						}
						if (flag)
						{
							this.TriggerTrap(null);
						}
					}
				}
			}
			else if (this.m_state == BasicTrapEnemyController.State.Triggered)
			{
				if (this.m_stateTimer == 0f)
				{
					this.state = BasicTrapEnemyController.State.Active;
				}
			}
			else if (this.m_state == BasicTrapEnemyController.State.Active)
			{
				if (this.damageMethod == BasicTrapEnemyController.DamageMethod.PlaceableFootprint)
				{
					SpeculativeRigidbody playerRigidbodyInFootprint2 = this.GetPlayerRigidbodyInFootprint();
					if (playerRigidbodyInFootprint2)
					{
						bool flag2 = playerRigidbodyInFootprint2.spriteAnimator.QueryGroundedFrame();
						if (playerRigidbodyInFootprint2.gameActor != null)
						{
							flag2 = (flag2 && !playerRigidbodyInFootprint2.gameActor.IsFlying);
						}
						if (flag2 || this.damagesFlyingPlayers)
						{
							this.Damage(playerRigidbodyInFootprint2);
						}
					}
				}
				if (this.IgnitesGoop)
				{
					DeadlyDeadlyGoopManager.IgniteGoopsCircle(base.sprite.WorldCenter, 1f);
				}
				if (this.m_stateTimer == 0f)
				{
					this.state = BasicTrapEnemyController.State.Resetting;
				}
			}
			else if (this.m_state == BasicTrapEnemyController.State.Resetting && this.m_stateTimer == 0f)
			{
				this.state = BasicTrapEnemyController.State.Ready;
			}
		}

		protected virtual void EndState(BasicTrapEnemyController.State newState)
		{
		}

		public void TemporarilyDisableTrap(float disableTime)
		{
			this.m_disabledTimer = Mathf.Max(disableTime, this.m_disabledTimer);
		}

		public Vector2 CenterPoint()
		{
			if (base.specRigidbody)
			{
				return base.specRigidbody.UnitCenter;
			}
			if (this.triggerMethod == BasicTrapEnemyController.TriggerMethod.PlaceableFootprint)
			{
				return new Vector2((float)(this.m_cachedPixelMin.x + this.m_cachedPixelMax.x), (float)(this.m_cachedPixelMin.y + this.m_cachedPixelMax.y)) / 32f;
			}
			return base.transform.position;
		}

		protected virtual void PlayAnimation(string animationName)
		{
			if (string.IsNullOrEmpty(animationName))
			{
				return;
			}
			if (this.animateChildren)
			{
				if (this.m_childrenAnimators != null)
				{
					for (int i = 0; i < this.m_childrenAnimators.Length; i++)
					{
						if (base.spriteAnimator != this.m_childrenAnimators[i])
						{
							this.m_childrenAnimators[i].Play(animationName);
						}
					}
				}
			}
			else
			{
				base.spriteAnimator.Play(animationName);
			}
		}

		protected virtual void SpawnVfx(List<SpriteAnimatorKiller> vfx)
		{
			for (int i = 0; i < vfx.Count; i++)
			{
				if (vfx[i])
				{
					vfx[i].Restart();
				}
			}
		}

		protected virtual SpeculativeRigidbody GetPlayerRigidbodyInFootprint()
		{
			for (int i = 0; i < GameManager.Instance.AllPlayers.Length; i++)
			{
				PlayerController playerController = GameManager.Instance.AllPlayers[i];
				if (!(playerController == null))
				{
					PixelCollider primaryPixelCollider = playerController.specRigidbody.PrimaryPixelCollider;
					if (primaryPixelCollider != null)
					{
						if (this.m_cachedPixelMin.x <= primaryPixelCollider.MaxX && this.m_cachedPixelMax.x >= primaryPixelCollider.MinX && this.m_cachedPixelMin.y <= primaryPixelCollider.MaxY && this.m_cachedPixelMax.y >= primaryPixelCollider.MinY)
						{
							return playerController.specRigidbody;
						}
					}
				}
			}
			return null;
		}

		protected virtual void Damage(SpeculativeRigidbody rigidbody)
		{
			if (this.damage > 0f && rigidbody && rigidbody.healthHaver && rigidbody.healthHaver.IsVulnerable)
			{
				if (rigidbody.gameActor && rigidbody.gameActor.IsFalling)
				{
					return;
				}
				rigidbody.healthHaver.ApplyDamage(this.damage, Vector2.zero, StringTableManager.GetEnemiesString("#TRAP", -1), this.damageTypes, DamageCategory.Normal, false, null, false);
			}
		}

		protected float GetNextTriggerTimerDelay()
		{
			float result = this.m_triggerTimerDelayArray[this.m_triggerTimerDelayIndex];
			this.m_triggerTimerDelayIndex = (this.m_triggerTimerDelayIndex + 1) % this.m_triggerTimerDelayArray.Count;
			return result;
		}

		public void ConfigureOnPlacement(RoomHandler room)
		{
			IntVector2 b = base.transform.position.IntXY(VectorConversions.Floor);
			for (int i = 0; i < this.placeableWidth; i++)
			{
				for (int j = 0; j < this.placeableHeight; j++)
				{
					IntVector2 key = new IntVector2(i, j) + b;
					GameManager.Instance.Dungeon.data[key].cellVisualData.containsObjectSpaceStamp = true;
					GameManager.Instance.Dungeon.data[key].cellVisualData.containsWallSpaceStamp = true;
				}
			}
			room.ForcePreventChannels = true;
		}

		public BasicTrapEnemyController.TriggerMethod triggerMethod;

		[DwarfConfigurable]
		[ShowInInspectorIf("triggerMethod", 2, false)]
		public float triggerTimerDelay;

		[DwarfConfigurable]
		[ShowInInspectorIf("triggerMethod", 2, false)]
		public float triggerTimerDelay1;

		[DwarfConfigurable]
		[ShowInInspectorIf("triggerMethod", 2, false)]
		public float triggerTimerOffset;

		public BasicTrapEnemyController.PlaceableFootprintBuffer footprintBuffer;

		public bool damagesFlyingPlayers;

		public bool triggerOnBlank;

		public bool triggerOnExplosion;

		[Header("Animations")]
		public bool animateChildren;

		[CheckAnimation(null)]
		public string triggerAnimName;

		public float triggerDelay;

		[CheckAnimation(null)]
		public string activeAnimName;

		public List<SpriteAnimatorKiller> activeVfx;

		public float activeTime;

		[CheckAnimation(null)]
		public string resetAnimName;

		public float resetDelay;

		[Header("Damage")]
		public BasicTrapEnemyController.DamageMethod damageMethod;

		[FormerlySerializedAs("activeDamage")]
		public float damage;

		public CoreDamageTypes damageTypes;

		[Header("Goop Interactions")]
		public bool IgnitesGoop;

		[NonSerialized]
		public float LocalTimeScale;

		private RoomHandler m_parentRoom;

		private BasicTrapEnemyController.State m_state;

		protected float m_stateTimer;

		protected float m_triggerTimer;

		protected float m_disabledTimer;

		protected IntVector2 m_cachedPosition;

		protected IntVector2 m_cachedPixelMin;

		protected IntVector2 m_cachedPixelMax;

		protected tk2dSpriteAnimator[] m_childrenAnimators;

		protected List<float> m_triggerTimerDelayArray;

		protected int m_triggerTimerDelayIndex;

		public enum TriggerMethod
		{
			SpecRigidbody,
			PlaceableFootprint,
			Timer,
			Script
		}

		public enum DamageMethod
		{
			SpecRigidbody,
			PlaceableFootprint,
			OnTrigger
		}

		protected enum State
		{
			Ready,
			Triggered,
			Active,
			Resetting
		}

		[Serializable]
		public class PlaceableFootprintBuffer
		{
			public int left;

			public int bottom;

			public int right;

			public int top;
		}
	}
}