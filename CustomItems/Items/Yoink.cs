using EnemyAPI;
using ItemAPI;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Random = UnityEngine.Random;

namespace GlaurungItems.Items
{
    class Yoink: PlayerItem
    {
		public static void Init()
		{
			string text = "Yoink";
			string resourcePath = "GlaurungItems/Resources/yoink";
			GameObject gameObject = new GameObject(text);
			Yoink item = gameObject.AddComponent<Yoink>();
			ItemBuilder.AddSpriteToObject(text, resourcePath, gameObject);
			string shortDesc = "Mine";
			string longDesc = "Gives you the ability to steal things in various ways.";
			item.SetupItem(shortDesc, longDesc, "gl");
			item.SetCooldownType(ItemBuilder.CooldownType.Damage, 2500f);
			ItemBuilder.AddPassiveStatModifier(item, PlayerStats.StatType.Curse, 0.5f, 0);
			item.quality = ItemQuality.S;
			//Toolbox.SetupUnlockOnCustomFlag(item, CustomDungeonFlags.ITEMSPECIFIC_YOINK, true);
		}

		protected override void DoEffect(PlayerController user)
		{
			base.DoEffect(user);
			if (user && user.CurrentRoom != null && user.IsInCombat)
			{
				//Tools.Print("yoink enemy", "ffffff", true);
				float nearestEnemyPosition;
				AIActor yoinkTarget =  user.CurrentRoom.GetNearestEnemy(user.CenterPosition, out nearestEnemyPosition, true, true);
				if(yoinkTarget != null)
                {
					//EnemyAPITools.DebugInformation(yoinkTarget.behaviorSpeculator);
					foreach (AttackBehaviorBase attackBehav in yoinkTarget.behaviorSpeculator.AttackBehaviors)
					{
						//Tools.Print(attackBehav.GetType(), "ffffff", true);
						if (attackBehav is AttackBehaviorGroup)
						{
							foreach (AttackBehaviorGroup.AttackGroupItem item in (attackBehav as AttackBehaviorGroup).AttackBehaviors)
							{
								if (item != null && item.Behavior != null)
								{
									if (item.Behavior is ShootBehavior)
									{
										//Tools.Print("removed AttackBehaviorGroup ShootBehavior", "ffffff", true);
										(item.Behavior as ShootBehavior).BulletScript = new CustomBulletScriptSelector(typeof(ShootZeroProjectilesBulletScript));
										YoinkEnemyFirePowerReward(user, yoinkTarget);
										return;
									}
									else if(item.Behavior is ShootGunBehavior)
                                    {
										//Tools.Print("removed AttackBehaviorGroup ShootGunBehavior", "ffffff", true);
										(item.Behavior as ShootGunBehavior).BulletScript = new CustomBulletScriptSelector(typeof(ShootZeroProjectilesBulletScript));
										YoinkEnemyFirePowerReward(user, yoinkTarget);
										return;
									}
								}
							}
						}
						else if(attackBehav is ShootGunBehavior)
                        {
							//Tools.Print("removed ShootGunBehavior", "ffffff", true);
                            (attackBehav as ShootGunBehavior).BulletScript = new CustomBulletScriptSelector(typeof(ShootZeroProjectilesBulletScript));
							if(yoinkTarget.behaviorSpeculator.AttackBehaviors.Count == 1) // to remove the attacks of bullet kin and ak47 bullet kin
                            {
								yoinkTarget.behaviorSpeculator.AttackBehaviors = EnemyDatabase.GetOrLoadByGuid("b08ec82bef6940328c7ecd9ffc6bd16c").behaviorSpeculator.AttackBehaviors;

							}
							YoinkEnemyFirePowerReward(user, yoinkTarget);
							return;
						}
						else if (attackBehav is ShootBehavior)
						{
							//Tools.Print("removed ShootBehavior", "ffffff", true);
							(attackBehav as ShootBehavior).BulletScript = new CustomBulletScriptSelector(typeof(ShootZeroProjectilesBulletScript));
							YoinkEnemyFirePowerReward(user, yoinkTarget);
							return;
						}
						else if (attackBehav is DashBehavior) // for sharks
                        {
							//Tools.Print("removed DashBehavior", "ffffff", true);
							(attackBehav as DashBehavior).bulletScript = new CustomBulletScriptSelector(typeof(ShootZeroProjectilesBulletScript));
							YoinkEnemyFirePowerReward(user, yoinkTarget);
							return;
						}
					}
				}
			}

			else if(user.CurrentRoom != null && user.CurrentRoom.IsShop) 
			{
				//Tools.Print("yoink shop", "ffffff", true);
				user.SetIsStealthed(true, "Yoink");
				user.SetCapableOfStealing(true, "Yoink", null);
				isYoinkingShop = true;
				base.StartCoroutine(ItemBuilder.HandleDuration(this, 5f, user, new Action<PlayerController>(this.EndShopYoinking)));

			}

			else if (user.CurrentRoom != null && user.CurrentRoom.GetNearestInteractable(user.CenterPosition, 2f, user) is Chest 
				&& CanYoinkChest((user.CurrentRoom.GetNearestInteractable(user.CenterPosition, 2f, user) as Chest)))
            {
				Chest chest = (user.CurrentRoom.GetNearestInteractable(user.CenterPosition, 2f, user) as Chest);
				List<PickupObject> contents = chest.PredictContents(user);

				//Tools.Print("yoink chest", "ffffff", true);

				if (contents == null)
                {
					Tools.Print("shit", "ffffff", true);
					return;
                }
                if (GameStatsManager.Instance.IsRainbowRun && chest.IsRainbowChest)
                {
					//this.AddStat(PlayerStats.StatType.Curse, 2.5f);
					user.stats.SetBaseStatValue(PlayerStats.StatType.Curse, user.stats.GetStatValue(PlayerStats.StatType.Curse) + 2f, user);

				}
				foreach (PickupObject content in contents) 
				{
					if (content is Gun) 
					{
						//Tools.Print("gun", "ffffff", true);
						LootEngine.TryGiveGunToPlayer(content.gameObject, user, true); 
					}
					if (content is PassiveItem) 
					{
						//Tools.Print("Passive", "ffffff", true);
						LootEngine.TryGivePrefabToPlayer(content.gameObject, user, true); 
					}
					if (content is PlayerItem)
					{
						//Tools.Print("Active", "ffffff", true);
						LootEngine.SpawnItem(content.gameObject, user.specRigidbody.UnitCenter, Vector2.up, 1f, false, true, false); 
					}
				}
				chest.BreakLock();
				return;
			}
		}


		public override bool CanBeUsed(PlayerController user)
		{
			if (user.CurrentRoom != null && user.IsInCombat)
			{
				float nearestEnemyPosition;
				AIActor yoinkTarget = user.CurrentRoom.GetNearestEnemy(user.CenterPosition, out nearestEnemyPosition, true, true);
				if (yoinkTarget != null && nearestEnemyPosition < 3.5f && !yoinkedTargets.Contains(yoinkTarget)) return true;
				else return false;
			}
			else if (user.CurrentRoom != null && user.CurrentRoom.IsShop)
			{
				return true;
			}
			else if (user.CurrentRoom != null && user.CurrentRoom.GetNearestInteractable(user.CenterPosition, 2f, user) is Chest)
			{
				return CanYoinkChest((user.CurrentRoom.GetNearestInteractable(user.CenterPosition, 2f, user) as Chest));
			}

			return false;
		}

		private void YoinkEnemyFirePowerReward(PlayerController user, AIActor yoinkTarget)
        {
			ItemQuality[] mobQualities = { ItemQuality.D, ItemQuality.C, ItemQuality.B };
			ItemQuality[] toughMobQualities = { ItemQuality.C, ItemQuality.B, ItemQuality.A };
			ItemQuality[] bossQualities = { ItemQuality.B, ItemQuality.A, ItemQuality.S };
			ItemQuality[] possibleQualities;
			if (yoinkTarget.healthHaver.IsBoss) { possibleQualities = bossQualities; }
			else if(user.CurrentRoom.GetToughestEnemy() == yoinkTarget && user.CurrentRoom.GetActiveEnemies(0).Count > 1) { possibleQualities = toughMobQualities; }
            else { possibleQualities = mobQualities; }

			float randQuality = Random.Range(0f, 1f);
			ItemQuality[] selectedQuality = { ItemQuality.D };
			if(randQuality <= 0.5f) { selectedQuality[0] = possibleQualities[0]; }
			else if(randQuality <= 0.8f) { selectedQuality[0] = possibleQualities[1]; }
			else { selectedQuality[0] = possibleQualities[2]; }

			List<int> excludedGunsIds = new List<int>();
			foreach(Gun gun in user.inventory.AllGuns) { excludedGunsIds.Add(gun.PickupObjectId); }

			LootEngine.TryGiveGunToPlayer(PickupObjectDatabase.GetRandomGunOfQualities(new System.Random(), excludedGunsIds, selectedQuality).gameObject, user, true);
			user.stats.SetBaseStatValue(PlayerStats.StatType.Curse, user.stats.GetStatValue(PlayerStats.StatType.Curse) + 0.5f, user);
			//this.AddStat(PlayerStats.StatType.Curse, 1f);
			user.stats.RecalculateStats(user, false);

			yoinkedTargets.Add(yoinkTarget);
		}


		private void EndShopYoinking(PlayerController user)
        {
			user.SetIsStealthed(false, "Yoink");
			user.SetCapableOfStealing(false, "Yoink", null);
			isYoinkingShop = false;
		}

		private bool CanYoinkChest(Chest chest)
        {
			return !chest.IsBroken && !chest.IsMimic && !chest.IsOpen && !chest.IsGlitched && !chest.IsLockBroken && !chest.IsMirrorChest;
        }

		private void AddStat(PlayerStats.StatType statType, float amount, StatModifier.ModifyMethod method = StatModifier.ModifyMethod.ADDITIVE)
		{
			StatModifier statModifier = new StatModifier
			{
				amount = amount,
				statToBoost = statType,
				modifyType = method
			};
			bool flag = this.passiveStatModifiers == null;
			if (flag)
			{
				this.passiveStatModifiers = new StatModifier[]
				{
					statModifier
				};
			}
			else
			{
				this.passiveStatModifiers = this.passiveStatModifiers.Concat(new StatModifier[]
				{
					statModifier
				}).ToArray<StatModifier>();
			}
		}

		public override void Pickup(PlayerController player)
		{
			base.Pickup(player);
			player.OnRoomClearEvent += this.OnLeaveCombat;
		}

		protected override void OnPreDrop(PlayerController user)
		{
			user.OnRoomClearEvent -= this.OnLeaveCombat;
            if (isYoinkingShop)
            {
				EndShopYoinking(user);

			}
			base.OnPreDrop(user);
		}

		private void OnLeaveCombat(PlayerController user)
		{
			yoinkedTargets = new List<AIActor>();
		}

		private List<AIActor> yoinkedTargets = new List<AIActor>();
		private bool isYoinkingShop = false;
	}


}
