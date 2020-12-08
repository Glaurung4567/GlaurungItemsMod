using Dungeonator;
using ItemAPI;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;

namespace GlaurungItems.Items
{
    class CreateTrap : PlayerItem
	{
		public static void Init()
		{
			string text = "Create Trap";
			string resourcePath = "GlaurungItems/Resources/acme_crate";
			GameObject gameObject = new GameObject(text);
			CreateTrap item = gameObject.AddComponent<CreateTrap>();
			ItemBuilder.AddSpriteToObject(text, resourcePath, gameObject);
			string shortDesc = "Your turn to avoid those";
			string longDesc = "A spell similar to the infamous Create Pit.";
			item.SetupItem(shortDesc, longDesc, "gl");
			item.SetCooldownType(ItemBuilder.CooldownType.Damage, 500f);
			item.quality = ItemQuality.B;
		}

		protected override void DoEffect(PlayerController user)
		{
			if (user.CurrentRoom != null)
			{
				float roomPosX = user.transform.position.x - user.CurrentRoom.area.basePosition.x;
				float roomPosY = user.transform.position.y - user.CurrentRoom.area.basePosition.y;
				float xOffSet = 0;
				float yOffSet = 0;
				float offsetAmount = 2f;
				float gunCurrentAngle = BraveMathCollege.Atan2Degrees(this.LastOwner.unadjustedAimPoint.XY() - this.LastOwner.CenterPosition);
				if (gunCurrentAngle > 45f && gunCurrentAngle <= 135f)
				{
					yOffSet = offsetAmount;//up
				}
				else if ((gunCurrentAngle > 0 && gunCurrentAngle > 135f) || (gunCurrentAngle < 0 && gunCurrentAngle <= -135f))
				{
					xOffSet = -offsetAmount;//left
				}
				else if (gunCurrentAngle > -135f && gunCurrentAngle <= -45f)
				{
					yOffSet = -offsetAmount;//bottom
				}
				else
				{
					xOffSet = offsetAmount;//right
				}
				Vector2 posInCurrentRoom = new Vector2(roomPosX + xOffSet, roomPosY + yOffSet);
				Vector2 posInMap = new Vector2(user.transform.position.x + xOffSet, user.transform.position.y + yOffSet).ToIntVector2().ToVector2();
                Vector2 cellPos = new Vector2(posInMap.x + 0.5f, posInMap.y + 0.5f);
                Vector2 cellPos2 = new Vector2(posInMap.x + 1.2f, posInMap.y + 0.5f);
                Vector2 cellPos3 = new Vector2(posInMap.x + 0.5f, posInMap.y + 1.2f);
                Vector2 cellPos4 = new Vector2(posInMap.x + 1.2f, posInMap.y + 1.2f);
                
                if (user.IsValidPlayerPosition(cellPos) 
                    && user.IsValidPlayerPosition(cellPos2)
                    && user.IsValidPlayerPosition(cellPos3)
                    && user.IsValidPlayerPosition(cellPos4)
                )
				{

                    AssetBundle sharedAssets = ResourceManager.LoadAssetBundle("shared_auto_001");
                    GameObject spikeTrap = sharedAssets.LoadAsset<GameObject>("trap_spike_gungeon_2x2");
                    GameObject spikePrefab = FakePrefab.Clone(spikeTrap);

                    ConvertTrapControllers.ConvertBasicTrapToAdvancedTrap(spikePrefab);
                    GameObject spike = spikePrefab.GetComponentInChildren<AdvancedTrapController>().InstantiateObject(user.CurrentRoom, posInCurrentRoom.ToIntVector2());
                    
                    CellData cellAfter = user.CurrentRoom.GetNearestCellToPosition(cellPos);
                    //Tools.Print(spikePrefab.GetComponentInChildren<AdvancedTrapController>().sprite.WorldTopLeft - spikePrefab.GetComponentInChildren<AdvancedTrapController>().sprite.WorldBottomLeft, "ffffff", true);
                    CellData cellAfter2 = user.CurrentRoom.GetNearestCellToPosition(cellPos2);
                    CellData cellAfter3 = user.CurrentRoom.GetNearestCellToPosition(cellPos3);
                    CellData cellAfter4 = user.CurrentRoom.GetNearestCellToPosition(cellPos4);
                    cellAfter.isOccupied = false;
                    cellAfter2.isOccupied = false;
                    cellAfter3.isOccupied = false;
                    cellAfter4.isOccupied = false;
                }
                else
                {
                    base.StartCoroutine(RefillOnWrongSpawn());
                }
            }
		}

        private IEnumerator RefillOnWrongSpawn()
        {
            yield return new WaitForSeconds(0.1f);
            ClearCooldowns();
            yield break;
        }

        public override bool CanBeUsed(PlayerController user)
		{
			return user.CurrentRoom != null;
		}

	}

    /*--------------------------------------------------------------------------------------------------------*/
    /*--------------------------------------------------------------------------------------------------------*/
    /*--------------------------------------------------------------------------------------------------------*/
    //Courtesy of nn
    public class ConvertTrapControllers
    {
        public static void ConvertBasicTrapToAdvancedTrap(GameObject trap)
        {
            BasicTrapController trapController = trap.GetComponent<BasicTrapController>();
            AdvancedTrapController advTrapController = trap.GetOrAddComponent<AdvancedTrapController>();
            advTrapController.footprintBuffer = trapController.footprintBuffer;
            advTrapController.activeAnimName = trapController.activeAnimName;
            advTrapController.activeTime = trapController.activeTime;
            advTrapController.activeVfx = trapController.activeVfx;
            advTrapController.animateChildren = trapController.animateChildren;
            advTrapController.damage = trapController.damage;
            advTrapController.damageMethod = trapController.damageMethod;
            advTrapController.damagesFlyingPlayers = trapController.damagesFlyingPlayers;
            advTrapController.damageTypes = trapController.damageTypes;
            advTrapController.difficulty = trapController.difficulty;
            advTrapController.IgnitesGoop = trapController.IgnitesGoop;
            advTrapController.isPassable = trapController.isPassable;
            advTrapController.LocalTimeScale = trapController.LocalTimeScale;
            advTrapController.placeableHeight = trapController.placeableHeight;
            advTrapController.placeableWidth = trapController.placeableWidth;
            advTrapController.resetAnimName = trapController.resetAnimName;
            advTrapController.resetDelay = trapController.resetDelay;
            advTrapController.TrapSwitchState = trapController.TrapSwitchState;
            advTrapController.triggerAnimName = trapController.triggerAnimName;
            advTrapController.triggerDelay = trapController.triggerDelay;
            advTrapController.triggerMethod = trapController.triggerMethod;
            advTrapController.triggerOnBlank = trapController.triggerOnBlank;
            advTrapController.triggerOnExplosion = trapController.triggerOnExplosion;
            advTrapController.triggerTimerDelay = trapController.triggerTimerDelay;
            advTrapController.triggerTimerDelay1 = trapController.triggerTimerDelay1;
            advTrapController.triggerTimerOffset = trapController.triggerTimerOffset;
            UnityEngine.Object.Destroy(trap.GetComponent<BasicTrapController>());
        }
        public static void ConvertBasicPitTrapToAdvancedPitTrap(GameObject trap)
        {
            PitTrapController trapController = trap.GetComponent<PitTrapController>();
            AdvancedPitTrapController advTrapController = trap.GetOrAddComponent<AdvancedPitTrapController>();
            advTrapController.footprintBuffer = trapController.footprintBuffer;
            advTrapController.activeAnimName = trapController.activeAnimName;
            advTrapController.activeTime = trapController.activeTime;
            advTrapController.activeVfx = trapController.activeVfx;
            advTrapController.animateChildren = trapController.animateChildren;
            advTrapController.damage = trapController.damage;
            advTrapController.damageMethod = trapController.damageMethod;
            advTrapController.damagesFlyingPlayers = trapController.damagesFlyingPlayers;
            advTrapController.damageTypes = trapController.damageTypes;
            advTrapController.difficulty = trapController.difficulty;
            advTrapController.IgnitesGoop = trapController.IgnitesGoop;
            advTrapController.isPassable = trapController.isPassable;
            advTrapController.LocalTimeScale = trapController.LocalTimeScale;
            advTrapController.placeableHeight = trapController.placeableHeight;
            advTrapController.placeableWidth = trapController.placeableWidth;
            advTrapController.resetAnimName = trapController.resetAnimName;
            advTrapController.resetDelay = trapController.resetDelay;
            advTrapController.TrapSwitchState = trapController.TrapSwitchState;
            advTrapController.triggerAnimName = trapController.triggerAnimName;
            advTrapController.triggerDelay = trapController.triggerDelay;
            advTrapController.triggerMethod = trapController.triggerMethod;
            advTrapController.triggerOnBlank = trapController.triggerOnBlank;
            advTrapController.triggerOnExplosion = trapController.triggerOnExplosion;
            advTrapController.triggerTimerDelay = trapController.triggerTimerDelay;
            advTrapController.triggerTimerDelay1 = trapController.triggerTimerDelay1;
            advTrapController.triggerTimerOffset = trapController.triggerTimerOffset;
            UnityEngine.Object.Destroy(trap.GetComponent<PitTrapController>());
        }
    }
    public class AdvancedTrapController : BasicTrapController, IPlaceConfigurable
    {
        /*
        public override void Start()
        {
            playerHasEnteredRoomOnce = false;
            base.Start();
        }

        

        public override void Update()
        {
            if (Time.timeScale == 0f)
            {
                return;
            }
            if (!playerHasEnteredRoomOnce && GameManager.Instance.PlayerIsInRoom(FetchParentRoom())) playerHasEnteredRoomOnce = true;
            if (TimerPlayerRadius == TimerPlayerRadiusType.NEARROOM && !GameManager.Instance.PlayerIsNearRoom(FetchParentRoom()))
            {
                return;
            }
            if (TimerPlayerRadius == TimerPlayerRadiusType.INROOM && !GameManager.Instance.PlayerIsInRoom(FetchParentRoom()))
            {
                return;
            }
            if (TimerPlayerRadius == TimerPlayerRadiusType.ANYWHEREAFTERENTERINGROOMONCE && !playerHasEnteredRoomOnce)
            {
                return;
            }
            this.m_stateTimer = Mathf.Max(0f, this.m_stateTimer - BraveTime.DeltaTime) * this.LocalTimeScale;
            this.m_triggerTimer -= BraveTime.DeltaTime * this.LocalTimeScale;
            this.m_disabledTimer = Mathf.Max(0f, this.m_disabledTimer - BraveTime.DeltaTime * this.LocalTimeScale);
            if (this.triggerMethod == BasicTrapController.TriggerMethod.Timer && this.m_triggerTimer < 0f)
            {
                this.TriggerTrap(null);
            }
            this.UpdateState();
        }

        public RoomHandler FetchParentRoom()
        {
            return GameManager.Instance.Dungeon.data.GetAbsoluteRoomFromPosition(base.transform.position.IntXY(VectorConversions.Round));
        }*/


        protected override void UpdateState()
        {
            if (this.GetTrapState() == BasicTrapController.State.Ready)
            {
                if (this.triggerMethod == BasicTrapController.TriggerMethod.PlaceableFootprint)
                {
                    SpeculativeRigidbody enemyRigidbodyInFootprint = GetEnemyRigidbodyInFootprint();//this.GetEnemyRigidbodyInFootprint();
                    if (enemyRigidbodyInFootprint)
                    {
                        bool flag = enemyRigidbodyInFootprint.spriteAnimator.QueryGroundedFrame();
                        if (enemyRigidbodyInFootprint.gameActor != null)
                        {
                            flag = (flag && !enemyRigidbodyInFootprint.gameActor.IsFlying);
                        }
                        if (flag)
                        {
                            this.TriggerTrap(null);
                        }
                    }
                }
            }
            else if (this.GetTrapState() == BasicTrapController.State.Triggered)
            {
                if (this.m_stateTimer == 0f)
                {
                    this.state = BasicTrapController.State.Active;
                }
            }
            else if (this.GetTrapState() == BasicTrapController.State.Active)
            {
                if (this.damageMethod == BasicTrapController.DamageMethod.PlaceableFootprint)
                {
                    SpeculativeRigidbody enemyRigidbodyInFootprint2 = this.GetEnemyRigidbodyInFootprint();//this.GetEnemyRigidbodyInFootprint();
                    if (enemyRigidbodyInFootprint2)
                    {
                        bool flag2 = enemyRigidbodyInFootprint2.spriteAnimator.QueryGroundedFrame();
                        if (enemyRigidbodyInFootprint2.gameActor != null)
                        {
                            flag2 = (flag2 && !enemyRigidbodyInFootprint2.gameActor.IsFlying);
                        }
                        if (flag2 || this.damagesFlyingPlayers)
                        {
                            this.Damage(enemyRigidbodyInFootprint2);
                        }
                    }
                }
                if (this.IgnitesGoop)
                {
                    DeadlyDeadlyGoopManager.IgniteGoopsCircle(base.sprite.WorldCenter, 1f);
                }
                if (this.m_stateTimer == 0f)
                {
                    this.state = BasicTrapController.State.Resetting;
                }
            }
            else if (this.GetTrapState() == BasicTrapController.State.Resetting && this.m_stateTimer == 0f)
            {
                this.state = BasicTrapController.State.Ready;
            }
        }

        //based of GetPlayerRigidbodyInFootprint from BasicTrapController
        protected virtual SpeculativeRigidbody GetEnemyRigidbodyInFootprint()
        {
            for (int i = 0; i < GameManager.Instance.AllPlayers.Length; i++)
            {
                PlayerController playerController = GameManager.Instance.AllPlayers[i];
                if (!(playerController == null) && playerController.CurrentRoom != null && playerController.CurrentRoom.GetActiveEnemies(RoomHandler.ActiveEnemyType.All) != null)
                {
                    List<AIActor> aiactors = playerController.CurrentRoom.GetActiveEnemies(RoomHandler.ActiveEnemyType.All);
                    for (int j = 0; j < aiactors.Count; j++)
                    {
                        PixelCollider primaryPixelCollider = aiactors[j].specRigidbody.PrimaryPixelCollider;//playerController.specRigidbody.PrimaryPixelCollider;
                        if (primaryPixelCollider != null)
                        {
                            if (this.m_cachedPixelMin.x <= primaryPixelCollider.MaxX && this.m_cachedPixelMax.x >= primaryPixelCollider.MinX && this.m_cachedPixelMin.y <= primaryPixelCollider.MaxY && this.m_cachedPixelMax.y >= primaryPixelCollider.MinY)
                            {
                                return aiactors[j].specRigidbody;
                            }
                        }
                    }
                }
            }
            return null;
        }

        protected BasicTrapController.State GetTrapState()
        {
            return (BasicTrapController.State)privateFieldInfo.GetValue(this);
        }

        public TimerPlayerRadiusType TimerPlayerRadius = TimerPlayerRadiusType.NEARROOM;
        private bool playerHasEnteredRoomOnce = false;
        public enum TimerPlayerRadiusType
        {
            INROOM,
            NEARROOM,
            ANYWHEREAFTERENTERINGROOMONCE,
        }

        //from spapi on discord
        private static FieldInfo privateFieldInfo = typeof(BasicTrapController).GetField("m_state", BindingFlags.NonPublic | BindingFlags.Instance);
    }

    public class AdvancedPitTrapController : AdvancedTrapController
    {
        public override GameObject InstantiateObject(RoomHandler targetRoom, IntVector2 loc, bool deferConfiguration = false)
        {
            IntVector2 intVector = loc + targetRoom.area.basePosition;
            for (int i = intVector.x; i < intVector.x + this.placeableWidth; i++)
            {
                for (int j = intVector.y; j < intVector.y + this.placeableHeight; j++)
                {
                    CellData cellData = GameManager.Instance.Dungeon.data.cellData[i][j];
                    cellData.type = CellType.PIT;
                    cellData.fallingPrevented = true;
                }
            }
            return base.InstantiateObject(targetRoom, loc, deferConfiguration);
        }
        protected override void BeginState(BasicTrapController.State newState)
        {
            if (newState == BasicTrapController.State.Active)
            {
                for (int i = this.m_cachedPosition.x; i < this.m_cachedPosition.x + this.placeableWidth; i++)
                {
                    for (int j = this.m_cachedPosition.y; j < this.m_cachedPosition.y + this.placeableHeight; j++)
                    {
                        GameManager.Instance.Dungeon.data.cellData[i][j].fallingPrevented = false;
                    }
                }
                if (base.specRigidbody)
                {
                    base.specRigidbody.enabled = false;
                }
            }
            else if (newState == BasicTrapController.State.Resetting)
            {
                for (int k = this.m_cachedPosition.x; k < this.m_cachedPosition.x + this.placeableWidth; k++)
                {
                    for (int l = this.m_cachedPosition.y; l < this.m_cachedPosition.y + this.placeableHeight; l++)
                    {
                        GameManager.Instance.Dungeon.data.cellData[k][l].fallingPrevented = true;
                    }
                }
                if (base.specRigidbody)
                {
                    base.specRigidbody.enabled = true;
                }
            }
            base.BeginState(newState);
        }
    }
}
