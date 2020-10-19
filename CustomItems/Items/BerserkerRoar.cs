using ItemAPI;
using System.Collections;
using System.Collections.Generic;
using System.Timers;
using UnityEngine;

namespace GlaurungItems.Items
{
	class BerserkerRoar : PlayerItem
	{
		public static void Init()
		{
			string text = "Berserker's Roar";
			string resourcePath = "GlaurungItems/Resources/berserkeroar";
			GameObject gameObject = new GameObject(text);
			BerserkerRoar item = gameObject.AddComponent<BerserkerRoar>();
			ItemBuilder.AddSpriteToObject(text, resourcePath, gameObject);
			string shortDesc = "Angerier";
			string longDesc = "Makes all enemies enter a blind rage during which they will attack friends and foes alike.";
			item.SetupItem(shortDesc, longDesc, "gl");
			item.SetCooldownType(ItemBuilder.CooldownType.Damage, 1500f);
			item.quality = ItemQuality.B;
		}

		public override void Pickup(PlayerController player)
		{
			base.Pickup(player);
			player.OnRoomClearEvent += this.OnLeaveCombat;
		}

		protected override void OnPreDrop(PlayerController user)
		{
			user.OnRoomClearEvent -= this.OnLeaveCombat;
			base.OnPreDrop(user);
		}

		protected override void DoEffect(PlayerController user)
		{
			if (user && user.CurrentRoom != null && user.CurrentRoom.GetActiveEnemies(0) != null)
			{
				AkSoundEngine.PostEvent("Play_VO_dragun_death_01", base.gameObject);
				rageInstanceVFX = user.PlayEffectOnActor(this.RageOverheadVFX, new Vector3(0f, 1.375f, 0f), true, true, false);
				base.StartCoroutine(this.EndRageVFX(user));


				List<AIActor> enemies = user.CurrentRoom.GetActiveEnemies(0);
				foreach (AIActor enemy in enemies)
				{
					if (!enemy.healthHaver.IsBoss)
					{
						enemy.RegisterOverrideColor(Color.red, "Anger");
						berserkersList.Add(enemy);
						this.BerserkerTargetSwitch(enemy);
						enemy.IsWorthShootingAt = true;
					}
				}
				berserkerSwitchTimer = new System.Timers.Timer(timerInterval); // launch an event each 0.25 seconds (1000 == 1 second)
				berserkerSwitchTimer.Elapsed += OnTimedEvent;
				berserkerSwitchTimer.Start();
			}
		}

		public override bool CanBeUsed(PlayerController user)
		{
			return user.IsInCombat;
		}

		private IEnumerator EndRageVFX(PlayerController player)
        {
			yield return new WaitForSeconds(2f);
			rageInstanceVFX.GetComponent<tk2dSpriteAnimator>().PlayAndDestroyObject("rage_face_vfx_out", null);
			rageInstanceVFX = null;
			yield break;
		}

		private void BerserkerTargetSwitch(AIActor enemy)
		{
			int randomSelect = Random.Range(1, 3);
			switch (randomSelect)
			{
				case 1:
					enemy.CanTargetPlayers = false;
					enemy.CanTargetEnemies = true;
					break;
				case 2:
					enemy.CanTargetPlayers = true;
					enemy.CanTargetEnemies = false;
					break;
				default:
					break;
			}
		}

		private void OnTimedEvent(System.Object source, System.Timers.ElapsedEventArgs e)
		{
			if(!this.LastOwner && !this.LastOwner.IsInCombat)
            {
				berserkerSwitchTimer.Stop();
            }
            else if(this.LastOwner.CurrentRoom.GetActiveEnemies(0).Count == 1)
			{
				foreach (AIActor enemy in berserkersList)
				{
					if (enemy.healthHaver && enemy.healthHaver.IsAlive)
					{
						enemy.CanTargetPlayers = true;
						enemy.CanTargetEnemies = false;
					}
				}
				this.berserkerSwitchTimer.Stop();
			}
			else
			{
				foreach(AIActor enemy in berserkersList)
                {
					if(enemy.healthHaver && enemy.healthHaver.IsAlive)
                    {
						this.BerserkerTargetSwitch(enemy);
                    }
                }
            }
		}

		private void OnLeaveCombat(PlayerController user)
		{
			berserkersList = new List<AIActor>();
		}

		private List<AIActor> berserkersList = new List<AIActor>();
		private static float timerInterval = 3000;
		private Timer berserkerSwitchTimer;
		private GameObject RageOverheadVFX = PickupObjectDatabase.GetById(399).GetComponent<TableFlipItem>().RageOverheadVFX;
		private GameObject rageInstanceVFX;
	}
}
