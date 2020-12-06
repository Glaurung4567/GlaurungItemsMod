using Dungeonator;
using ItemAPI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace GlaurungItems.Items
{
    class CreatePit : PlayerItem
	{
		public static void Init()
		{
			string text = "Create Pit";
			string resourcePath = "GlaurungItems/Resources/acme_crate";
			GameObject gameObject = new GameObject(text);
			CreatePit item = gameObject.AddComponent<CreatePit>();
			ItemBuilder.AddSpriteToObject(text, resourcePath, gameObject);
			string shortDesc = "WIP";
			string longDesc = "WIP";
			item.SetupItem(shortDesc, longDesc, "gl");
			item.SetCooldownType(ItemBuilder.CooldownType.Damage, 5f);
			item.quality = ItemQuality.EXCLUDED;
		}

		protected override void DoEffect(PlayerController user)
		{
			

			//Destroy(trap.GetComponent<PitTrapController>());
            /*
			PitTrapEnemyController pit2 = trap.GetOrAddComponent<PitTrapEnemyController>();
			pit2.triggerMethod = (BasicTrapEnemyController.TriggerMethod)pit.triggerMethod;
			pit2.triggerTimerDelay = pit.triggerTimerDelay;
			pit2.triggerTimerDelay1 = pit.triggerTimerDelay1;
			pit2.triggerTimerOffset = pit.triggerTimerOffset;
			pit2.footprintBuffer = pit.footprintBuffer;
			pit2.damagesFlyingPlayers = pit.damagesFlyingPlayers;
			pit2.triggerOnBlank = pit.triggerOnBlank;
			pit2.triggerOnExplosion = pit.triggerOnExplosion;
			pit2.animateChildren = pit.animateChildren;
			pit2.triggerAnimName = pit.triggerAnimName;
			pit2.triggerDelay = pit.triggerDelay;
			pit2.activeAnimName = pit.activeAnimName;
			pit2.activeVfx = pit.activeVfx;
			pit2.activeTime = pit.activeTime;
			pit2.resetAnimName = pit.resetAnimName;
			pit2.triggerDelay = pit.resetDelay;
			pit2.damageMethod = pit.damageMethod;
			pit2.damage = pit.damage;
			pit2.damageTypes = pit.damageTypes;
			pit2.IgnitesGoop = pit.IgnitesGoop;
			pit2.LocalTimeScale = pit.LocalTimeScale;

			Destroy(trap.GetComponent<PitTrapController>());
			*/
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
				if (user.IsValidPlayerPosition(posInMap))
				{
                    AssetBundle sharedAssets2 = ResourceManager.LoadAssetBundle("shared_auto_002");
                    DungeonPlaceable PitTrap = sharedAssets2.LoadAsset<DungeonPlaceable>("Pit Trap");
                    GameObject trap = PitTrap.InstantiateObject(user.CurrentRoom, posInCurrentRoom.ToIntVector2());
                    //ConvertTrapControllers.ConvertBasicPitTrapToAdvancedPitTrap(trap);
                }
			}
		}

		public override bool CanBeUsed(PlayerController user)
		{
			return user.CurrentRoom != null;
		}

	}

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
        }
        public TimerPlayerRadiusType TimerPlayerRadius = TimerPlayerRadiusType.NEARROOM;
        private bool playerHasEnteredRoomOnce = false;
        public enum TimerPlayerRadiusType
        {
            INROOM,
            NEARROOM,
            ANYWHEREAFTERENTERINGROOMONCE,
        }
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
