using System;
using System.Collections;
using System.Collections.Generic;
using System.Timers;
using ItemAPI;
using UnityEngine;

namespace GlaurungItems.Items
{
	internal class KillStreakBullets: PassiveItem
	{
		public static void Init()
		{
			string name = "Kill Streak Bullets";
			string resourcePath = "GlaurungItems/Resources/killstreak_bullets";
			GameObject gameObject = new GameObject(name);
			KillStreakBullets item = gameObject.AddComponent<KillStreakBullets>();
			ItemBuilder.AddSpriteToObject(name, resourcePath, gameObject);
			string shortDesc = "Monster Kill";
			string longDesc = "Give boosts the more kills you get in a short time window. Theses bullets were used by a maniac who ravished in endless slaughter.";
			item.SetupItem(shortDesc, longDesc, "gl");
			item.quality = PickupObject.ItemQuality.A;
			KillStreakBullets.ID = item.PickupObjectId;
		}

		private void PostProcessProjectile(Projectile projectile, float Chance)
		{
			PlayerController owner = base.Owner;
			projectile.OnHitEnemy = (Action<Projectile, SpeculativeRigidbody, bool>)Delegate.Combine(projectile.OnHitEnemy, new Action<Projectile, SpeculativeRigidbody, bool>(this.OnProjectileHitEnemy));

		}

		private void OnProjectileHitEnemy(Projectile proj, SpeculativeRigidbody enemy, bool fatal)
		{
			if (enemy != null)
			{
				AIActor aiActor = enemy.aiActor;
				if (aiActor != null && kills <= 0 && fatal)
				{
					StartKillStreak();
				}
				else if (fatal)
				{
					IncreaseKillStreak();
				}

			}
		}

		private void StartKillStreak()
        {
			this.CanBeDropped = false;
			killStreakTimer = new System.Timers.Timer(timerInterval); // launch an event each 0.25 seconds (1000 == 1 second)
			killStreakTimer.Elapsed += OnTimedEvent;
			killStreakTimer.Start();
			numberOfTimeIntervalsBeforeKillStreakEndIterator = maxNumberOfTimeIntervalsBeforeKillStreakEnd;
			kills++;
		}

		private void IncreaseKillStreak()
        {
			numberOfTimeIntervalsBeforeKillStreakEndIterator = maxNumberOfTimeIntervalsBeforeKillStreakEnd;
			kills++;
			label.Text = $"Kill Streak: {kills}";
		}

		private void OnTimedEvent(System.Object source, System.Timers.ElapsedEventArgs e)
		{
			if (numberOfTimeIntervalsBeforeKillStreakEndIterator >= 0)
			{
				numberOfTimeIntervalsBeforeKillStreakEndIterator--;
			}
			else
			{
				kills = 0;
				killStreakTimer.Stop();
				label.Text = "";
				this.CanBeDropped = true;
			}
		}

		//thx to nevernamed for this
		private void PostProcessBeamTick(BeamController beam, SpeculativeRigidbody hitRigidBody, float tickrate)
		{
			AIActor aiactor = hitRigidBody.aiActor;
			if (!aiactor)
			{
				return;
			}
			bool fatal = aiactor.healthHaver && aiactor.healthHaver.IsDead && !monstersKilledByBeamList.Contains(aiactor);
			if (fatal)
			{
				if(kills <= 0)
                {
					StartKillStreak();
                }
                else
                {
					IncreaseKillStreak();
                }
				monstersKilledByBeamList.Add(aiactor);
			}
		}

		private void OnLeaveCombat(PlayerController obj)
		{
			monstersKilledByBeamList = new List<AIActor>();
		}

		// used to give boosts when the numbers of kills reach a certain number and remove them when the kill streak end
		protected override void Update()
		{
			if (kills <= 0 && firstKillStreakBoostActive)
			{
				firstKillStreakBoostActive = false;

				this.AddPassiveStatModifier(PlayerStats.StatType.Damage, -firstKillStreakBoostAmount, StatModifier.ModifyMethod.ADDITIVE);
                if (secondKillStreakBoostActive)
                {
					secondKillStreakBoostActive = false;
					this.AddPassiveStatModifier(PlayerStats.StatType.ReloadSpeed, -secondKillStreakBoostAmount, StatModifier.ModifyMethod.ADDITIVE);

				}
                if (thirdKillStreakBoostActive)
                {
					thirdKillStreakBoostActive = false;
					this.AddPassiveStatModifier(PlayerStats.StatType.MovementSpeed, -thirdKillStreakBoostAmount, StatModifier.ModifyMethod.ADDITIVE);

				}
				PlayerController owner = base.Owner;
				owner.stats.RecalculateStats(owner, false);
			}
			else if (kills >= 5 && !firstKillStreakBoostActive)
			{
				firstKillStreakBoostActive = true;

				this.AddPassiveStatModifier(PlayerStats.StatType.Damage, firstKillStreakBoostAmount, StatModifier.ModifyMethod.ADDITIVE);
				PlayerController owner = base.Owner;
				owner.stats.RecalculateStats(owner, false);
			}
			else if (kills >= 7 && !secondKillStreakBoostActive)
			{
				secondKillStreakBoostActive = true;

				this.AddPassiveStatModifier(PlayerStats.StatType.ReloadSpeed, secondKillStreakBoostAmount, StatModifier.ModifyMethod.ADDITIVE);
				PlayerController owner = base.Owner;
				owner.stats.RecalculateStats(owner, false);
			}
			else if (kills >= 9 && !thirdKillStreakBoostActive)
			{
				thirdKillStreakBoostActive = true;

				this.AddPassiveStatModifier(PlayerStats.StatType.MovementSpeed, thirdKillStreakBoostAmount, StatModifier.ModifyMethod.ADDITIVE);
				PlayerController owner = base.Owner;
				owner.stats.RecalculateStats(owner, false);
			}
			base.Update();
		}



		public override void Pickup(PlayerController player)
		{
			base.Pickup(player);
			player.PostProcessProjectile += this.PostProcessProjectile;
			player.PostProcessBeamTick += this.PostProcessBeamTick;
			player.OnRoomClearEvent += this.OnLeaveCombat;

			dfLabel kLabel;
			kLabel = GameUIRoot.Instance.p_playerKeyLabel; // thanks to explosivePanda and Zatherz from the Gungeon Discord server for the dfLabel bit
			label = kLabel.AddControl<dfLabel>();
			label.Text = "";
			label.AutoSize = true;
			label.TextScale = 2f;
			label.RelativePosition = new Vector3(-40f, 47f);
			if (this.Owner == GameManager.Instance.SecondaryPlayer)
			{
				label.RelativePosition = new Vector3(1220f, 47f);
			}
			label.Enable();
		}

        public override DebrisObject Drop(PlayerController player)
		{
			if (kills <= 0 && firstKillStreakBoostActive)
			{
				firstKillStreakBoostActive = false;

				this.AddPassiveStatModifier(PlayerStats.StatType.Damage, -firstKillStreakBoostAmount, StatModifier.ModifyMethod.ADDITIVE);
				if (secondKillStreakBoostActive)
				{
					secondKillStreakBoostActive = false;
					this.AddPassiveStatModifier(PlayerStats.StatType.ReloadSpeed, -secondKillStreakBoostAmount, StatModifier.ModifyMethod.ADDITIVE);

				}
				if (thirdKillStreakBoostActive)
				{
					thirdKillStreakBoostActive = false;
					this.AddPassiveStatModifier(PlayerStats.StatType.MovementSpeed, -thirdKillStreakBoostAmount, StatModifier.ModifyMethod.ADDITIVE);

				}
				PlayerController owner = base.Owner;
				owner.stats.RecalculateStats(owner, false);
			}
			player.PostProcessProjectile -= this.PostProcessProjectile;
			player.PostProcessBeamTick -= this.PostProcessBeamTick;
			player.OnRoomClearEvent -= this.OnLeaveCombat;
			return base.Drop(player);
		}

		protected override void OnDestroy()
		{
			base.Owner.PostProcessProjectile -= this.PostProcessProjectile;
			base.Owner.PostProcessBeamTick -= this.PostProcessBeamTick;
			base.Owner.OnRoomClearEvent += this.OnLeaveCombat;

			base.OnDestroy();
		}


		public static int ID;
		private static float timerInterval = 250;
		private static int maxNumberOfTimeIntervalsBeforeKillStreakEnd = 16; 
		private int kills = 0;
		private int numberOfTimeIntervalsBeforeKillStreakEndIterator = 0;
		private Timer killStreakTimer;
		private List<AIActor> monstersKilledByBeamList = new List<AIActor>();
		private bool firstKillStreakBoostActive = false;
		private float firstKillStreakBoostAmount = 0.15f;
		private bool secondKillStreakBoostActive = false;
		private float secondKillStreakBoostAmount = 0.25f;
		private bool thirdKillStreakBoostActive = false;
		private float thirdKillStreakBoostAmount = 1f;
		private dfLabel label;
	}
}
