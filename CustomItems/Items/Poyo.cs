using Gungeon;
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
			//https://www.youtube.com/watch?v=srDKPk-2SjI
			string longDesc = "Give the user a fraction of the tremendous absorption and copy powers of a strange giant sentient amoeba.";
			item.SetupItem(shortDesc, longDesc, "gl");
			item.SetCooldownType(ItemBuilder.CooldownType.Damage, 1000);
			item.quality = ItemQuality.S;
		}

		protected override void DoEffect(PlayerController user)
		{
			float nearestEnemyPosition;
			AIActor nomTarget = user.CurrentRoom.GetNearestEnemy(user.CenterPosition, out nearestEnemyPosition, true, true);
			string nomTargetUuid = nomTarget.EnemyGuid;

			if (nomTarget.healthHaver.IsAlive && !nomTarget.healthHaver.IsBoss)
            {
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
			

			
				if (crutchs.Contains(nomTargetUuid))
				{
					powerUp = PickupObjectDatabase.GetById(240);
				}
			
				if (homings.Contains(nomTargetUuid))
				{
					powerUp = PickupObjectDatabase.GetById(284);
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
			
				if (fairyz.Contains(nomTargetUuid))
				{
					powerUp = PickupObjectDatabase.GetById(310);
				}
			
				if (hotleadz.Contains(nomTargetUuid))
				{
					powerUp = PickupObjectDatabase.GetById(295);
				}
			
				if (holsterz.Contains(nomTargetUuid))
				{
					powerUp = PickupObjectDatabase.GetById(500);
				}
				
				if (bloodbrood.Contains(nomTargetUuid))
				{
					powerUp = PickupObjectDatabase.GetById(285);
				}
				
				if (fatbullz.Contains(nomTargetUuid))
				{
					powerUp = PickupObjectDatabase.GetById(277);
				}
				
				if (junkanz.Contains(nomTargetUuid))
				{
					powerUp = PickupObjectDatabase.GetById(580);
				}
				
				if (heavybooty.Contains(nomTargetUuid))
				{
					powerUp = PickupObjectDatabase.GetById(256);
				}
				
				if (gunjurers.Contains(nomTargetUuid))
				{
					powerUp = PickupObjectDatabase.GetById(456);
				}
				
				if (angrubullz.Contains(nomTargetUuid))
				{
					powerUp = PickupObjectDatabase.GetById(323);
				}
				
				if (frostbullz.Contains(nomTargetUuid))
				{
					powerUp = PickupObjectDatabase.GetById(278);
				}
				
				if (irradiatedleadz.Contains(nomTargetUuid))
				{
					powerUp = PickupObjectDatabase.GetById(204);
				}
				
				if (batteries.Contains(nomTargetUuid))
				{
					powerUp = PickupObjectDatabase.GetById(410);
				}
				
				if (militarytrainingz.Contains(nomTargetUuid))
				{
					powerUp = PickupObjectDatabase.GetById(354);
				}
				
				if (yellowz.Contains(nomTargetUuid))
				{
					powerUp = PickupObjectDatabase.GetById(570);
				}
				
				if (montipiz.Contains(nomTargetUuid))
				{
					powerUp = PickupObjectDatabase.GetById(631);
				}
				
				if (illuzions.Contains(nomTargetUuid))
				{
					powerUp = PickupObjectDatabase.GetById(352);
				}
				
				if (defectivez.Contains(nomTargetUuid))
				{
					powerUp = Game.Items["gl:turncoat_rounds"];
				}
				
				if (abbeyz.Contains(nomTargetUuid))
				{
					powerUp = Game.Items["gl:banishing_bullets"];
				}
			
				if (lichiez.Contains(nomTargetUuid))
				{
					if (Random.value <= 0.7)
					{
						powerUp = PickupObjectDatabase.GetById(213);
					}
					else
					{
						powerUp = PickupObjectDatabase.GetById(815);
					}
				}
			
				if (mimics.Contains(nomTargetUuid))
				{
					if (Random.value <= 0.5)
					{
						powerUp = PickupObjectDatabase.GetById(293);
					}
					else
					{
						powerUp = PickupObjectDatabase.GetById(664);
					}
				}
				
				if (vegiez.Contains(nomTargetUuid))
				{
					if (Random.value <= 0.5)
					{
						powerUp = PickupObjectDatabase.GetById(258);
					}
					else
					{
						powerUp = PickupObjectDatabase.GetById(253);
					}
				}

				if (flaks.Contains(nomTargetUuid))
				{
					if(!user.HasPassiveItem(531))
					powerUp = PickupObjectDatabase.GetById(531);
				}
				
				if (singers.Contains(nomTargetUuid))
				{
					if(Random.value <= 0.5)
					{
						powerUp = PickupObjectDatabase.GetById(119);
					}
					else
					{
						powerUp = PickupObjectDatabase.GetById(529);
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
				
				if (backpacks.Contains(nomTargetUuid))
				{
					if(Random.value <= 0.5)
					{
						powerUp = PickupObjectDatabase.GetById(133);
					}
					else
					{
						powerUp = PickupObjectDatabase.GetById(655);
					}
				}

				if (invinciblez.Contains(nomTargetUuid))
				{
					if (Random.value <= 0.5)
					{
						powerUp = PickupObjectDatabase.GetById(190);
					}
					else
					{
						powerUp = PickupObjectDatabase.GetById(634);
					}
				}

				if (looterz.Contains(nomTargetUuid))
				{
					int lootz = Random.Range(0, 5);
                    switch (lootz)
                    {
						case 0:
							powerUp = PickupObjectDatabase.GetById(140);//master of unlocking
							break;
						case 1:
							powerUp = PickupObjectDatabase.GetById(214);//coin croin
							break;
						case 2:
							powerUp = PickupObjectDatabase.GetById(254);//ring of chest friendhip
							break;
						case 3:
							powerUp = PickupObjectDatabase.GetById(164);//Heart Synthesizer
							break;
						case 4:
							powerUp = PickupObjectDatabase.GetById(450);//Armor Synthesizer
							break;
						default:
							break;
                    }
				}

				if (books.Contains(nomTargetUuid))
				{
					int lootz = Random.Range(0, 10);
					switch (lootz)
					{
						case 0:
							powerUp = PickupObjectDatabase.GetById(487);
							break;
						case 1:
							powerUp = PickupObjectDatabase.GetById(354);
							break;
						case 2:
							powerUp = PickupObjectDatabase.GetById(396);
							break;
						case 3:
							powerUp = PickupObjectDatabase.GetById(397);
							break;
						case 4:
							powerUp = PickupObjectDatabase.GetById(398);
							break;
						case 5:
							powerUp = PickupObjectDatabase.GetById(399);
							break;
						case 6:
							powerUp = PickupObjectDatabase.GetById(400);
							break;
						case 7:
							powerUp = PickupObjectDatabase.GetById(465);
							break;
						case 8:
							powerUp = PickupObjectDatabase.GetById(666);
							break;
						case 9:
							powerUp = PickupObjectDatabase.GetById(633);
							break;
						default:
							break;
					}
				}

				if (powerUp != null)
				{
					powerUpId = powerUp.PickupObjectId;
					EncounterTrackable.SuppressNextNotification = true;
					powerUpInstance = Instantiate(powerUp.gameObject, Vector2.zero, Quaternion.identity).GetComponent<PassiveItem>();
					powerUpInstance.CanBeDropped = false;
					powerUpInstance.CanBeSold = false;
					LootEngine.TryGivePrefabToPlayer(powerUpInstance.gameObject, user, true);
				
					EncounterTrackable.SuppressNextNotification = false;
				}

				DEVOUR(nomTarget);
			}

		}

		private void RemovePowerUp()
        {
            if (this.LastOwner && powerUpInstance && this.LastOwner.passiveItems != null)
            {
				//this.LastOwner.RemovePassiveItem(powerUpInstance.PickupObjectId);
				for(int i = 0; i<this.LastOwner.passiveItems.Count; i++)
                {
					if(this.LastOwner.passiveItems[i].PickupObjectId == powerUpInstance.PickupObjectId 
						&& this.LastOwner.passiveItems[i].CanBeDropped == false
						&& this.LastOwner.passiveItems[i].CanBeSold == false
						)
                    {
						PassiveItem pu = this.LastOwner.passiveItems[i];
						this.LastOwner.passiveItems.RemoveAt(i);
						GameUIRoot.Instance.RemovePassiveItemFromDock(pu);
						DebrisObject deb = pu.Drop(this.LastOwner);
						deb.ForceDestroyAndMaybeRespawn();
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
			GameManager.Instance.OnNewLevelFullyLoaded += this.RechargePerFloor;

		}

		protected override void OnPreDrop(PlayerController player)
		{
			RemovePowerUp();
			GameManager.Instance.OnNewLevelFullyLoaded -= this.RechargePerFloor;
			player.OnReceivedDamage -= Player_OnReceivedDamage;
			base.OnPreDrop(player);
		}

		private void RechargePerFloor()
        {
			if(powerUpId >= 0)
            {
				if (this.LastOwner &&  this.LastOwner.passiveItems != null)
				{
					//this.LastOwner.RemovePassiveItem(powerUpInstance.PickupObjectId);
					for (int i = 0; i < this.LastOwner.passiveItems.Count; i++)
					{
						if (this.LastOwner.passiveItems[i].PickupObjectId == powerUpId && this.LastOwner.passiveItems[i].CanBeDropped == false && this.LastOwner.passiveItems[i].CanBeSold == false)
						{
							PassiveItem pu = this.LastOwner.passiveItems[i];
							this.LastOwner.passiveItems.RemoveAt(i);
							GameUIRoot.Instance.RemovePassiveItemFromDock(pu);
							DebrisObject deb = pu.Drop(this.LastOwner);
							deb.ForceDestroyAndMaybeRespawn();
							UnityEngine.Object.Destroy(pu);
							this.LastOwner.stats.RecalculateStats(this.LastOwner);
							powerUpInstance = null;

							EncounterTrackable.SuppressNextNotification = true;
							PickupObject powerUp = PickupObjectDatabase.GetById(powerUpId);
							powerUpInstance = Instantiate(powerUp.gameObject, Vector2.zero, Quaternion.identity).GetComponent<PassiveItem>();
							powerUpInstance.CanBeDropped = false;
							powerUpInstance.CanBeSold = false;
							LootEngine.TryGivePrefabToPlayer(powerUpInstance.gameObject, this.LastOwner, true);
							EncounterTrackable.SuppressNextNotification = false;
							return;
						}
					}
				}
			}
		}

        private void Player_OnReceivedDamage(PlayerController player)
        {
			RemovePowerUp();
			powerUpId = -5;
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

		[SerializeField]
		private PassiveItem powerUpInstance;
		
		[SerializeField]
		private int powerUpId = -5;


		
		private static List<string> scattershots = new List<string> {
			EnemyGuidDatabase.Entries["red_shotgun_kin"],
			EnemyGuidDatabase.Entries["blue_shotgun_kin"],
			EnemyGuidDatabase.Entries["ashen_shotgun_kin"],
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
			EnemyGuidDatabase.Entries["tablet_bookllett"],
			EnemyGuidDatabase.Entries["lore_gunjurer"],
		};

		private static readonly List<string> springheels = new List<string> {
			EnemyGuidDatabase.Entries["gun_cultist"]
		};

		private static readonly List<string> singers = new List<string> {
			EnemyGuidDatabase.Entries["gunsinger"],
			EnemyGuidDatabase.Entries["aged_gunsinger"]
		};
		
		private static readonly List<string> idols = new List<string> {
			EnemyGuidDatabase.Entries["shambling_round"]
		};

		private static readonly List<string> ghostsbulls = new List<string> {
			EnemyGuidDatabase.Entries["hollowpoint"],
			EnemyGuidDatabase.Entries["spectre"],
		};
		
		private static readonly List<string> scarfs = new List<string> {
			EnemyGuidDatabase.Entries["phaser_spider"],
			EnemyGuidDatabase.Entries["wizbang"],
		};

		private static readonly List<string> sixs = new List<string> {
			EnemyGuidDatabase.Entries["fallen_bullet_kin"]
		};

		private static readonly List<string> bouncys = new List<string> {
			EnemyGuidDatabase.Entries["rubber_kin"],
			EnemyGuidDatabase.Entries["creech"]
		};

		private static readonly List<string> blankies = new List<string> {
			EnemyGuidDatabase.Entries["bombshee"]
		};
		
		private static readonly List<string> batteries = new List<string> {
			EnemyGuidDatabase.Entries["tazie"]
		};
		
		private static readonly List<string> flaks = new List<string> {
			EnemyGuidDatabase.Entries["blobulon"],
			EnemyGuidDatabase.Entries["blobuloid"],
			EnemyGuidDatabase.Entries["king_bullat"],
			EnemyGuidDatabase.Entries["poisbuloid"],
		};
		
		private static readonly List<string> backpacks = new List<string> {
			EnemyGuidDatabase.Entries["tarnisher"]
		};
		
		private static readonly List<string> numbertwoz = new List<string> {
			EnemyGuidDatabase.Entries["bandana_bullet_kin"]
		};

		private static readonly List<string> militarytrainingz = new List<string> {
			EnemyGuidDatabase.Entries["treadnaughts_bullet_kin"],
			EnemyGuidDatabase.Entries["summoned_treadnaughts_bullet_kin"],
		};

		private static readonly List<string> crutchs = new List<string> {
			EnemyGuidDatabase.Entries["veteran_bullet_kin"],
			EnemyGuidDatabase.Entries["veteran_shotgun_kin"],
			EnemyGuidDatabase.Entries["ashen_bullet_kin"]
		};
		
		private static readonly List<string> homings = new List<string> {
			EnemyGuidDatabase.Entries["gunzookie"],
			EnemyGuidDatabase.Entries["gunzockie"]
		};
		
		private static readonly List<string> invinciblez = new List<string> {
			EnemyGuidDatabase.Entries["lead_maiden"],
			EnemyGuidDatabase.Entries["minelet"],
			EnemyGuidDatabase.Entries["gat"]
		};
		
		private static readonly List<string> orbitalz = new List<string> {
			EnemyGuidDatabase.Entries["skusket"]
		};
		
		private static readonly List<string> biolegs = new List<string> {
			EnemyGuidDatabase.Entries["bullet_mech"]
		};
		
		private static readonly List<string> backupz = new List<string> {
			EnemyGuidDatabase.Entries["grip_master"]
		};
		
		private static readonly List<string> cursedbullz = new List<string> {
			EnemyGuidDatabase.Entries["jammomancer"],
			EnemyGuidDatabase.Entries["jamerlengo"]
		};
		
		private static readonly List<string> zombiez = new List<string> {
			EnemyGuidDatabase.Entries["spent"],
			EnemyGuidDatabase.Entries["gummy_spent"]
		};
		
		private static readonly List<string> cats = new List<string> {
			EnemyGuidDatabase.Entries["bullet_kings_toadie"],
			EnemyGuidDatabase.Entries["bullet_kings_toadie_revenge"]
		};
		
		private static readonly List<string> shelletonz = new List<string> {
			EnemyGuidDatabase.Entries["shelleton"],
			EnemyGuidDatabase.Entries["ammomancer"]
		};
		
		private static readonly List<string> chancebz = new List<string> {
			EnemyGuidDatabase.Entries["chancebulon"],
		};
		
		private static readonly List<string> berds = new List<string> {
			EnemyGuidDatabase.Entries["gigi"],
			EnemyGuidDatabase.Entries["bird_parrot"]
		};
		
		private static readonly List<string> fairyz = new List<string> {
			EnemyGuidDatabase.Entries["pot_fairy"],
		};
		
		private static readonly List<string> hotleadz = new List<string> {
			EnemyGuidDatabase.Entries["coaler"],
			EnemyGuidDatabase.Entries["muzzle_wisp"],
			EnemyGuidDatabase.Entries["muzzle_flare"],
		};
		
		private static readonly List<string> bloodbrood = new List<string> {
			EnemyGuidDatabase.Entries["bloodbulon"],
		};
		
		private static readonly List<string> fatbullz = new List<string> {
			EnemyGuidDatabase.Entries["titan_bullet_kin"],
			EnemyGuidDatabase.Entries["titan_bullet_kin_boss"],
			EnemyGuidDatabase.Entries["titaness_bullet_kin_boss"],
		};
		
		private static readonly List<string> junkanz = new List<string> {
			EnemyGuidDatabase.Entries["gun_nut"],
			EnemyGuidDatabase.Entries["spectral_gun_nut"],
			EnemyGuidDatabase.Entries["poopulon"],
		};
		
		private static readonly List<string> holsterz = new List<string> {
			EnemyGuidDatabase.Entries["beadie"],
		};
		
		private static readonly List<string> lichiez = new List<string> {
			EnemyGuidDatabase.Entries["revolvenant"],
		};
		
		private static readonly List<string> heavybooty = new List<string> {
			EnemyGuidDatabase.Entries["lead_cube"],
		};

        private static readonly List<string> angrubullz = new List<string> {
			EnemyGuidDatabase.Entries["leadbulon"],
		};
		
		private static readonly List<string> frostbullz = new List<string> {
			EnemyGuidDatabase.Entries["blizzbulon"],
		};
		
		private static readonly List<string> irradiatedleadz = new List<string> {
			EnemyGuidDatabase.Entries["mutant_bullet_kin"],
			EnemyGuidDatabase.Entries["poisbulon"],
			EnemyGuidDatabase.Entries["poisbulin"],
		};
		
		private static readonly List<string> gunjurers = new List<string> {
			EnemyGuidDatabase.Entries["apprentice_gunjurer"],
			EnemyGuidDatabase.Entries["gunjurer"],
			EnemyGuidDatabase.Entries["high_gunjurer"],
		};
		
		private static readonly List<string> looterz = new List<string> {
			EnemyGuidDatabase.Entries["key_bullet_kin"],
			EnemyGuidDatabase.Entries["chance_bullet_kin"],
		};
		
		private static readonly List<string> vegiez = new List<string> {
			EnemyGuidDatabase.Entries["fungun"],
			EnemyGuidDatabase.Entries["spogre"],
		};
		
		private static readonly List<string> defectivez = new List<string> {
			EnemyGuidDatabase.Entries["shroomer"],
		};
		
		private static readonly List<string> yellowz = new List<string> {
			EnemyGuidDatabase.Entries["hooded_bullet"],
		};
		
		private static readonly List<string> montipiz = new List<string> {
			EnemyGuidDatabase.Entries["killithid"],
		};
		
		private static readonly List<string> abbeyz = new List<string> {
			EnemyGuidDatabase.Entries["cardinal"],
		};
		
		private static readonly List<string> illuzions = new List<string> {
			EnemyGuidDatabase.Entries["misfire_beast"],
		};
		
		private static List<string> mimics = new List<string> {
			EnemyGuidDatabase.Entries["brown_chest_mimic"],
			EnemyGuidDatabase.Entries["blue_chest_mimic"],
			EnemyGuidDatabase.Entries["green_chest_mimic"],
			EnemyGuidDatabase.Entries["red_chest_mimic"],
			EnemyGuidDatabase.Entries["black_chest_mimic"],
			EnemyGuidDatabase.Entries["rat_chest_mimic"],
			EnemyGuidDatabase.Entries["pedestal_mimic"],
			EnemyGuidDatabase.Entries["wall_mimic"]
		};

		/*
		det
		tombstoner
		bullet shark
		flesh cube
		dead blow
		agonizer
		mine
		bell


		*/

	}
}
