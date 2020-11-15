using Dungeonator;
using ItemAPI;
using Pathfinding;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace GlaurungItems.Items
{
    class GunDeadArmyStronghold : PlayerItem
	{
		public static void Init()
		{
			string text = "Gundead Army Stronghold Simulator";
			string resourcePath = "GlaurungItems/Resources/acme_crate";
			GameObject gameObject = new GameObject(text);
			GunDeadArmyStronghold item = gameObject.AddComponent<GunDeadArmyStronghold>();
			ItemBuilder.AddSpriteToObject(text, resourcePath, gameObject);
			string shortDesc = "Training is everything";
			string longDesc = "";
			item.AddPassiveStatModifier(PlayerStats.StatType.AdditionalItemCapacity, 1f, StatModifier.ModifyMethod.ADDITIVE);
			item.SetupItem(shortDesc, longDesc, "gl");
			item.quality = ItemQuality.EXCLUDED;
		}

		protected override void DoEffect(PlayerController user)
		{
			this.CanBeDropped = false;
			RoomHandler absoluteRoom = user.transform.position.GetAbsoluteRoom();
			this.m_inEffect = true;
			GameManager.Instance.Dungeon.StartCoroutine(this.HandleTransitionToFallbackCombatRoom(absoluteRoom));

		}

		public override bool CanBeUsed(PlayerController user)
		{
			return !user.IsInCombat;
		}

		protected IEnumerator HandleTransitionToFallbackCombatRoom(RoomHandler sourceRoom)
		{
			Dungeon d = GameManager.Instance.Dungeon;

			Tools.Print("tes", "ffffff", true);

			Tools.Print(this.LastOwner.CurrentRoom.GetRoomName(), "ffffff", true);

			if(!GunDeadArmyStronghold.floorsVisited.Contains(d.DungeonFloorName))
            {
				GunDeadArmyStronghold.floorsVisited.Add(d.DungeonFloorName);
				this.newRoom = null;
			}

			if (this.newRoom == null)
            {
				GenericFallbackCombatRoom = (PickupObjectDatabase.GetById(625) as PaydayDrillItem).GenericFallbackCombatRoom;
				/*
				GenericFallbackCombatRoom = (PrototypeDungeonRoom)ScriptableObject.CreateInstance("PrototypeDungeonRoom");
				GenericFallbackCombatRoom.Width = 48;
				GenericFallbackCombatRoom.Height = 32;
				GenericFallbackCombatRoom.allowFloorDecoration = true;
				GenericFallbackCombatRoom.usesProceduralDecoration = true;
				Tools.Print(GenericFallbackCombatRoom.Width, "ffffff", true);
				Tools.Print(GenericFallbackCombatRoom.Height, "ffffff", true);
				*/
				this.newRoom = d.AddRuntimeRoom(this.GenericFallbackCombatRoom, null, DungeonData.LightGenerationStyle.FORCE_COLOR);
			}

			Tools.Print(newRoom == null, "ffffff", true);
			newRoom.CompletelyPreventLeaving = true;

			Vector2 oldPlayerPosition = GameManager.Instance.BestActivePlayer.transform.position.XY();
			Vector2 newPlayerPosition = newRoom.Epicenter.ToVector2() + new Vector2(0f, -5f);
			Pathfinder.Instance.InitializeRegion(d.data, newRoom.area.basePosition, newRoom.area.dimensions);

			yield return new WaitForSeconds(1f);
			Pixelator.Instance.FadeToColor(0.25f, Color.white, true, 0.125f);
			GameManager.Instance.BestActivePlayer.WarpToPoint(newPlayerPosition, false, false);
			if (GameManager.Instance.CurrentGameType == GameManager.GameType.COOP_2_PLAYER)
			{
				GameManager.Instance.GetOtherPlayer(GameManager.Instance.BestActivePlayer).ReuniteWithOtherPlayer(GameManager.Instance.BestActivePlayer, false);
			}
			yield return null;
			for (int i = 0; i < GameManager.Instance.AllPlayers.Length; i++)
			{
				GameManager.Instance.AllPlayers[i].WarpFollowersToPlayer(false);
				GameManager.Instance.AllPlayers[i].WarpCompanionsToPlayer(false);
			}

			yield return this.StartCoroutine(this.HandleCombatWaves(d, newRoom));
			newRoom.CompletelyPreventLeaving = false;
			yield return new WaitForSeconds(3f);
			Pixelator.Instance.FadeToColor(0.25f, Color.white, true, 0.125f);
			//GameManager.Instance.MainCameraController.SetManualControl(false, false);
			GameManager.Instance.BestActivePlayer.WarpToPoint(oldPlayerPosition, false, false);
			if (GameManager.Instance.CurrentGameType == GameManager.GameType.COOP_2_PLAYER)
			{
				GameManager.Instance.GetOtherPlayer(GameManager.Instance.BestActivePlayer).ReuniteWithOtherPlayer(GameManager.Instance.BestActivePlayer, false);
			}
			this.m_inEffect = false;
			yield break;
		}

		protected IEnumerator HandleCombatWaves(Dungeon d, RoomHandler newRoom)
		{

			/*int numEnemiesToSpawn = UnityEngine.Random.Range(1, 3 + 1);
			for (int i = 0; i < numEnemiesToSpawn; i++)
			{
				//newRoom.AddSpecificEnemyToRoomProcedurally(d.GetWeightedProceduralEnemy().enemyGuid, true, null);
			}*/
			AIActor bunkerLoadByGuid = EnemyDatabase.GetOrLoadByGuid(EnemyGuidDatabase.Entries["bunker"]);
			IntVector2? intVector = new IntVector2?((this.LastOwner.CenterPosition + new Vector2(-5f, 5f)).ToIntVector2(VectorConversions.Floor));
			AIActor aiactor = AIActor.Spawn(bunkerLoadByGuid.aiActor, intVector.Value, GameManager.Instance.Dungeon.data.GetAbsoluteRoomFromPosition(intVector.Value), true, AIActor.AwakenAnimationType.Default, true);
			aiactor.HasBeenEngaged = true;

			yield return new WaitForSeconds(3f);
			while (newRoom.GetActiveEnemiesCount(RoomHandler.ActiveEnemyType.RoomClear) > 0)
			{
				yield return new WaitForSeconds(1f);
			}
			if (newRoom.GetActiveEnemiesCount(RoomHandler.ActiveEnemyType.All) > 0)
			{
				List<AIActor> activeEnemies = newRoom.GetActiveEnemies(RoomHandler.ActiveEnemyType.All);
				for (int j = 0; j < activeEnemies.Count; j++)
				{
					if (activeEnemies[j].IsNormalEnemy)
					{
						activeEnemies[j].EraseFromExistence(false);
					}
				}	
			}
			yield break;
		}


		public PrototypeDungeonRoom GenericFallbackCombatRoom = (PickupObjectDatabase.GetById(625) as PaydayDrillItem).GenericFallbackCombatRoom;
		private static List<string> floorsVisited = new List<string>(); 
		//private static int nbOfUses = 0;
		private RoomHandler newRoom;
		private bool m_inEffect;

	}
}
