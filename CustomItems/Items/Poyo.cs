using ItemAPI;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

namespace GlaurungItems.Items
{
	class Poyo : PlayerItem
	{
		public static void Init()
		{
			string text = "Poyo";
			string resourcePath = "GlaurungItems/Resources/neuralyzer";
			GameObject gameObject = new GameObject(text);
			Poyo item = gameObject.AddComponent<Poyo>();
			ItemBuilder.AddSpriteToObject(text, resourcePath, gameObject);
			string shortDesc = "Nom nom nom";
			string longDesc = "Poyo poYo POyo PoYo";
			item.SetupItem(shortDesc, longDesc, "gl");
			item.SetCooldownType(ItemBuilder.CooldownType.Damage, 1);
			item.quality = ItemQuality.S;
		}

		protected override void DoEffect(PlayerController user)
		{
			float nearestEnemyPosition;
			AIActor nomTarget = user.CurrentRoom.GetNearestEnemy(user.CenterPosition, out nearestEnemyPosition, true, true);
			string nomTargetUuid = nomTarget.EnemyGuid;

			/*
			AIActor orLoadByGuid = EnemyDatabase.GetOrLoadByGuid(EnemyGuidDatabase.Entries["titan_bullet_kin_boss"]);
			IntVector2? intVector = new IntVector2?(user.CurrentRoom.GetRandomVisibleClearSpot(2, 2));
			AIActor aiactor = AIActor.Spawn(orLoadByGuid.aiActor, intVector.Value, GameManager.Instance.Dungeon.data.GetAbsoluteRoomFromPosition(intVector.Value), true, AIActor.AwakenAnimationType.Default, true);

			orLoadByGuid = EnemyDatabase.GetOrLoadByGuid(EnemyGuidDatabase.Entries["kalibullet"]);
			intVector = new IntVector2?(user.CurrentRoom.GetRandomVisibleClearSpot(2, 2));
			AIActor.Spawn(orLoadByGuid.aiActor, intVector.Value, GameManager.Instance.Dungeon.data.GetAbsoluteRoomFromPosition(intVector.Value), true, AIActor.AwakenAnimationType.Default, true);
			
			orLoadByGuid = EnemyDatabase.GetOrLoadByGuid(EnemyGuidDatabase.Entries["kbullet"]);
			intVector = new IntVector2?(user.CurrentRoom.GetRandomVisibleClearSpot(2, 2));
			AIActor.Spawn(orLoadByGuid.aiActor, intVector.Value, GameManager.Instance.Dungeon.data.GetAbsoluteRoomFromPosition(intVector.Value), true, AIActor.AwakenAnimationType.Default, true);
			*/

			RemovePowerUp();
			PickupObject powerUp = null;

			if (scattershots.Contains(nomTargetUuid))
            {
				powerUp = PickupObjectDatabase.GetById(241);
			}

			if (kaboomrounds.Contains(nomTargetUuid))
			{
				powerUp = PickupObjectDatabase.GetById(304);
			}
			
			if (lasersights.Contains(nomTargetUuid))
			{
				powerUp = PickupObjectDatabase.GetById(273);
			}
			
			if (icecubes.Contains(nomTargetUuid))
			{
				powerUp = PickupObjectDatabase.GetById(170);
			}
			
			if (standards.Contains(nomTargetUuid))
			{
				powerUp = PickupObjectDatabase.GetById(529);
			}
			
			if (springheels.Contains(nomTargetUuid))
			{
				powerUp = PickupObjectDatabase.GetById(526);
			}
			
			if (idols.Contains(nomTargetUuid))
			{
				powerUp = PickupObjectDatabase.GetById(434);
			}

			if (ghostsbulls.Contains(nomTargetUuid))
			{
				powerUp = PickupObjectDatabase.GetById(172);
			}
			
			if (scarfs.Contains(nomTargetUuid))
			{
				powerUp = PickupObjectDatabase.GetById(436);
			}

			if (sixs.Contains(nomTargetUuid))
			{
				powerUp = PickupObjectDatabase.GetById(407);
			}

			if (bouncys.Contains(nomTargetUuid))
			{
				powerUp = PickupObjectDatabase.GetById(288);
			}
			
			if (blankies.Contains(nomTargetUuid))
			{
				powerUp = PickupObjectDatabase.GetById(579);
			}
			
			if (cats.Contains(nomTargetUuid))
			{
				powerUp = PickupObjectDatabase.GetById(817);
			}
			
			if (backpacks.Contains(nomTargetUuid))
			{
				powerUp = PickupObjectDatabase.GetById(133);
			}
			
			if (crutchs.Contains(nomTargetUuid))
			{
				powerUp = PickupObjectDatabase.GetById(240);
			}
			
			if (rollingeyez.Contains(nomTargetUuid))
			{
				powerUp = PickupObjectDatabase.GetById(284);
			}
			
			if (homings.Contains(nomTargetUuid))
			{
				powerUp = PickupObjectDatabase.GetById(190);
			}
			
			if (orbitalz.Contains(nomTargetUuid))
			{
				powerUp = PickupObjectDatabase.GetById(661);
			}
			
			if (zombiez.Contains(nomTargetUuid))
			{
				powerUp = PickupObjectDatabase.GetById(528);
			}
			
			if (biolegs.Contains(nomTargetUuid))
			{
				powerUp = PickupObjectDatabase.GetById(114);
			}
			
			if (numbertwoz.Contains(nomTargetUuid))
			{
				powerUp = PickupObjectDatabase.GetById(326);
			}
			
			if (backupz.Contains(nomTargetUuid))
			{
				powerUp = PickupObjectDatabase.GetById(287);
			}
			
			if (cursedbullz.Contains(nomTargetUuid))
			{
				powerUp = PickupObjectDatabase.GetById(571);
			}
			
			if (shelletonz.Contains(nomTargetUuid))
			{
				powerUp = PickupObjectDatabase.GetById(818);
			}
			
			if (chancebz.Contains(nomTargetUuid))
			{
				powerUp = PickupObjectDatabase.GetById(521);
			}
			
			if (babymimics.Contains(nomTargetUuid))
			{
				powerUp = PickupObjectDatabase.GetById(664);
			}

			if (flaks.Contains(nomTargetUuid))
			{
				if(!user.HasPassiveItem(531))
				powerUp = PickupObjectDatabase.GetById(531);
			}

			if (books.Contains(nomTargetUuid))
			{
				if(Random.value <= 0.5)
                {
					powerUp = PickupObjectDatabase.GetById(487);
				}
                else
                {
					powerUp = PickupObjectDatabase.GetById(354);
				}
			}
			
			if (berds.Contains(nomTargetUuid))
			{
				if(Random.value <= 0.5)
                {
					powerUp = PickupObjectDatabase.GetById(572);
				}
                else
                {
					powerUp = PickupObjectDatabase.GetById(632);
				}
			}

			if (powerUp != null)
            {
				EncounterTrackable.SuppressNextNotification = true;
				powerUpInstance = Instantiate(powerUp.gameObject, Vector2.zero, Quaternion.identity).GetComponent<PickupObject>();
				powerUpInstance.CanBeDropped = false;
				powerUpInstance.CanBeSold = false;
				LootEngine.TryGivePrefabToPlayer(powerUpInstance.gameObject, user, true);
				
				EncounterTrackable.SuppressNextNotification = false;
			}

			DEVOUR(nomTarget);


		}

		private void RemovePowerUp()
        {
            if (this.LastOwner && powerUpInstance && this.LastOwner.passiveItems != null)
            {
				//this.LastOwner.RemovePassiveItem(powerUpInstance.PickupObjectId);
				for(int i = 0; i<this.LastOwner.passiveItems.Count; i++)
                {
					if(this.LastOwner.passiveItems[i].PickupObjectId == powerUpInstance.PickupObjectId && this.LastOwner.passiveItems[i].CanBeDropped == false)
                    {
						PassiveItem pu = this.LastOwner.passiveItems[i];
						this.LastOwner.passiveItems.RemoveAt(i);
						GameUIRoot.Instance.RemovePassiveItemFromDock(pu);
						UnityEngine.Object.Destroy(pu);
						this.LastOwner.stats.RecalculateStats(this.LastOwner);
						powerUpInstance = null;
						return;
                    }
                }
			}
		}

		public override bool CanBeUsed(PlayerController user)
		{
			if (user.CurrentRoom != null && user.IsInCombat)
			{
				float nearestEnemyPosition;
				AIActor nomTarget = user.CurrentRoom.GetNearestEnemy(user.CenterPosition, out nearestEnemyPosition, true, true);
				if (nomTarget != null && nearestEnemyPosition < 3.5f && nomTarget.healthHaver
					&& !nomTarget.healthHaver.IsBoss && nomTarget.healthHaver.IsAlive) return true;
			}
			return false;
		}

        public override void Pickup(PlayerController player)
        {
            base.Pickup(player);
            player.OnReceivedDamage += Player_OnReceivedDamage;
        }

        private void Player_OnReceivedDamage(PlayerController player)
        {
			RemovePowerUp();
		}

		protected override void OnPreDrop(PlayerController player)
		{
			RemovePowerUp();
			player.OnReceivedDamage -= Player_OnReceivedDamage;
			base.OnPreDrop(player);
		}

		private void DEVOUR(AIActor target)
		{
			if (target != null && !target.healthHaver.IsBoss)
			{
				GameManager.Instance.Dungeon.StartCoroutine(this.HandleEnemySuck(target));
				target.EraseFromExistenceWithRewards(true);
			}
		}
		private IEnumerator HandleEnemySuck(AIActor target)
		{
			PlayerController playerController = this.LastOwner;
			Transform copySprite = this.CreateEmptySprite(target);
			Vector3 startPosition = copySprite.transform.position;
			float elapsed = 0f;
			float duration = 0.3f;
			while (elapsed < duration)
			{
				elapsed += BraveTime.DeltaTime;
				bool TRESS = playerController.CurrentGun && copySprite;
				if (TRESS)
				{
					Vector3 position = playerController.CurrentGun.PrimaryHandAttachPoint.position;
					float t = elapsed / duration * (elapsed / duration);
					copySprite.position = Vector3.Lerp(startPosition, position, t);
					copySprite.rotation = Quaternion.Euler(0f, 0f, 360f * BraveTime.DeltaTime) * copySprite.rotation;
					copySprite.localScale = Vector3.Lerp(Vector3.one, new Vector3(0.1f, 0.1f, 0.1f), t);
					position = default(Vector3);
				}
				yield return null;
			}
			bool flag4 = copySprite;
			if (flag4)
			{
				UnityEngine.Object.Destroy(copySprite.gameObject);
			}
			yield break;
		}
		private Transform CreateEmptySprite(AIActor target)
		{
			GameObject gameObject = new GameObject("suck image");
			gameObject.layer = target.gameObject.layer;
			tk2dSprite tk2dSprite = gameObject.AddComponent<tk2dSprite>();
			gameObject.transform.parent = SpawnManager.Instance.VFX;
			tk2dSprite.SetSprite(target.sprite.Collection, target.sprite.spriteId);
			tk2dSprite.transform.position = target.sprite.transform.position;
			GameObject gameObject2 = new GameObject("image parent");
			gameObject2.transform.position = tk2dSprite.WorldCenter;
			tk2dSprite.transform.parent = gameObject2.transform;
			bool flag = target.optionalPalette != null;
			if (flag)
			{
				tk2dSprite.renderer.material.SetTexture("_PaletteTex", target.optionalPalette);
			}
			return gameObject2.transform;
		}

		private PickupObject powerUpInstance = null;
		
		private static List<string> scattershots = new List<string> {
			EnemyGuidDatabase.Entries["red_shotgun_kin"],
			EnemyGuidDatabase.Entries["red_shotgun_kin"],
		};

		private static List<string> kaboomrounds = new List<string> {
			EnemyGuidDatabase.Entries["dynamite_kin"],
			EnemyGuidDatabase.Entries["grenade_kin"]
		};

		private static List<string> lasersights = new List<string> {
			EnemyGuidDatabase.Entries["sniper_shell"],
			EnemyGuidDatabase.Entries["professional"]
		};

		private static List<string> icecubes = new List<string> {
			EnemyGuidDatabase.Entries["mountain_cube"]
		};
		
		private static List<string> books = new List<string> {
			EnemyGuidDatabase.Entries["bookllet"],
			EnemyGuidDatabase.Entries["blue_bookllet"],
			EnemyGuidDatabase.Entries["green_bookllet"],
			EnemyGuidDatabase.Entries["tablet_bookllett"]
		};

		private static List<string> springheels = new List<string> {
			EnemyGuidDatabase.Entries["gun_cultist"]
		};

		private static List<string> standards = new List<string> {
			EnemyGuidDatabase.Entries["gunsinger"],
			EnemyGuidDatabase.Entries["aged_gunsinger"]
		};
		
		private static List<string> idols = new List<string> {
			EnemyGuidDatabase.Entries["shambling_round"]
		};

		private static List<string> ghostsbulls = new List<string> {
			EnemyGuidDatabase.Entries["hollowpoint"]
		};
		
		private static List<string> scarfs = new List<string> {
			EnemyGuidDatabase.Entries["phaser_spider"]
		};

		private static List<string> sixs = new List<string> {
			EnemyGuidDatabase.Entries["fallen_bullet_kin"]
		};

		private static List<string> bouncys = new List<string> {
			EnemyGuidDatabase.Entries["rubber_kin"],
			EnemyGuidDatabase.Entries["creech"]
		};

		private static List<string> blankies = new List<string> {
			EnemyGuidDatabase.Entries["bombshee"]
		};
		
		private static List<string> flaks = new List<string> {
			EnemyGuidDatabase.Entries["blobulon"],
			EnemyGuidDatabase.Entries["blobuloid"],
			EnemyGuidDatabase.Entries["king_bullat"]
		};
		
		private static List<string> backpacks = new List<string> {
			EnemyGuidDatabase.Entries["tarnisher"]
		};
		
		private static List<string> numbertwoz = new List<string> {
			EnemyGuidDatabase.Entries["bandana_bullet_kin"]
		};

		private static List<string> crutchs = new List<string> {
			EnemyGuidDatabase.Entries["veteran_bullet_kin"],
			EnemyGuidDatabase.Entries["veteran_shotgun_kin"]
		};
		
		private static List<string> homings = new List<string> {
			EnemyGuidDatabase.Entries["gunzookie"],
			EnemyGuidDatabase.Entries["gunzockie"]
		};
		
		private static List<string> rollingeyez = new List<string> {
			EnemyGuidDatabase.Entries["lead_maiden"],
			EnemyGuidDatabase.Entries["minelet"],
			EnemyGuidDatabase.Entries["gat"]
		};
		
		private static List<string> orbitalz = new List<string> {
			EnemyGuidDatabase.Entries["skusket"]
		};
		
		private static List<string> biolegs = new List<string> {
			EnemyGuidDatabase.Entries["bullet_mech"]
		};
		
		private static List<string> backupz = new List<string> {
			EnemyGuidDatabase.Entries["grip_master"]
		};
		
		private static List<string> cursedbullz = new List<string> {
			EnemyGuidDatabase.Entries["jammomancer"],
			EnemyGuidDatabase.Entries["jamerlengo"]
		};
		
		private static List<string> zombiez = new List<string> {
			EnemyGuidDatabase.Entries["spent"],
			EnemyGuidDatabase.Entries["gummy_spent"]
		};
		
		private static List<string> cats = new List<string> {
			EnemyGuidDatabase.Entries["bullet_kings_toadie"],
			EnemyGuidDatabase.Entries["bullet_kings_toadie_revenge"]
		};
		
		private static List<string> shelletonz = new List<string> {
			EnemyGuidDatabase.Entries["shelleton"],
		};
		
		private static List<string> chancebz = new List<string> {
			EnemyGuidDatabase.Entries["chancebulon"],
		};
		
		private static List<string> berds = new List<string> {
			EnemyGuidDatabase.Entries["gigi"],
			EnemyGuidDatabase.Entries["bird_parrot"]
		};
		
		private static List<string> babymimics = new List<string> {
			EnemyGuidDatabase.Entries["brown_chest_mimic"],
			EnemyGuidDatabase.Entries["blue_chest_mimic"],
			EnemyGuidDatabase.Entries["green_chest_mimic"],
			EnemyGuidDatabase.Entries["red_chest_mimic"],
			EnemyGuidDatabase.Entries["black_chest_mimic"],
			EnemyGuidDatabase.Entries["rat_chest_mimic"],
			EnemyGuidDatabase.Entries["pedestal_mimic"],
			EnemyGuidDatabase.Entries["wall_mimic"]
		};

	}
}
