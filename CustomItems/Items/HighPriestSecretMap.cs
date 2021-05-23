using Brave.BulletScript;
using Dungeonator;
using EnemyAPI;
using ItemAPI;
using Pathfinding;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

namespace GlaurungItems.Items
{
    // a lot of this code is from Bl4ck_hunter N'Kuhana's Invitation item which i tweaked a bit to remove the bugs
    class HighPriestSecretMap : PlayerItem
    {
		public static void Init()
		{
			string text = "High Priest's Secret Map";
			string resourcePath = "GlaurungItems/Resources/highpriestmap";
			GameObject gameObject = new GameObject(text);
			HighPriestSecretMap item = gameObject.AddComponent<HighPriestSecretMap>();
			ItemBuilder.AddSpriteToObject(text, resourcePath, gameObject);
			string shortDesc = "Where could this lead ?";
			string longDesc = "This old and worn map covered by cryptic inscriptions along with symbols of the Abbey of the True Gun radiates a powerful and sinister magical aura." +
                " It smells of rewards, trickeries and dangers.";
            item.AddPassiveStatModifier(PlayerStats.StatType.AdditionalItemCapacity, 1f, StatModifier.ModifyMethod.ADDITIVE);
            item.SetupItem(shortDesc, longDesc, "gl");
            item.quality = ItemQuality.B;
		}



		protected override void DoEffect(PlayerController user)
		{
            this.CanBeDropped = false;

            if(!this.used)
            {
                GameManager.Instance.StartCoroutine(this.DoOrDie(user));
            }
            if (this.used && this.combatEnded)
            {
                GameManager.Instance.StartCoroutine(this.ReturnWarp(user));
                GameManager.Instance.Dungeon.data.rooms = roomsAtStartOfCombat;
            }
        }

        public override bool CanBeUsed(PlayerController user)
        {
            return user.CurrentRoom.GetActiveEnemiesCount(RoomHandler.ActiveEnemyType.All) <= 0 && user && user.IsValidPlayerPosition(user.transform.position);
        }

        public IEnumerator DoOrDie(PlayerController user)
        {
            RoomHandler currentRoom = user.CurrentRoom;
            bool flag = currentRoom.HasActiveEnemies(RoomHandler.ActiveEnemyType.RoomClear);
            if (!flag)
            {
                this.AddPassiveStatModifier(PlayerStats.StatType.AdditionalItemCapacity, 25f, StatModifier.ModifyMethod.ADDITIVE); // to prevent the owner to drop this if he pick up one too many active during the combat
                this.LastOwner.stats.RecalculateStats(this.LastOwner, false);
                this.isInCoop = GameManager.Instance.CurrentGameType == GameManager.GameType.COOP_2_PLAYER;
                this.used = true;

                // to prevent the player from moving during the teleport animation
                this.WarpTarget = user.CenterPosition;
                user.ForceStopDodgeRoll();
                user.IsStationary = true;
                user.IsGunLocked = true;
                user.MovementModifiers += this.NoMotionModifier;
                this.DoTentacleVFX(user);

                IntVector2? targetCenter = new IntVector2?(user.CenterPosition.ToIntVector2(VectorConversions.Floor));
                Vector2 startingpoint = user.CenterPosition;

                RoomHandler creepyRoom = GameManager.Instance.Dungeon.AddRuntimeRoom(new IntVector2(24, 24), (GameObject)BraveResources.Load("Global Prefabs/CreepyEye_Room", ".prefab"));
                yield return new WaitForSeconds(1.3f);

                // used to allow the mobs to know how they can move in the room 
                Pathfinder.Instance.InitializeRegion(GameManager.Instance.Dungeon.data, creepyRoom.area.basePosition, creepyRoom.area.dimensions);
                
                user.WarpToPoint((creepyRoom.area.basePosition + new IntVector2(12, 4)).ToVector2(), false, false);
                if (isInCoop)
                {
                    GameManager.Instance.GetOtherPlayer(user).ReuniteWithOtherPlayer(user, false);
                }
                user.IsStationary = false;
                user.IsGunLocked = false;
                user.MovementModifiers -= this.NoMotionModifier;

                yield return new WaitForSeconds(0.3f);
                this.CheckRitual(user);
                yield break;
            }
        }

        private void CheckRitual(PlayerController user)
        {
            try
            {
                // to prevent the CreepyEyeController monobehaviour of the creepy room from automatically teleporting the player back after 15 seconds if he has unlocked at least one teleporter on the current floor
                List<RoomHandler> rooms = GameManager.Instance.Dungeon.data.rooms;
                roomsAtStartOfCombat = GameManager.Instance.Dungeon.data.rooms;
                foreach (RoomHandler roomHandler in rooms)
                {
                    bool flag = roomHandler != this.LastOwner.CurrentRoom;
                    if (flag)
                    {
                        roomHandler.visibility = RoomHandler.VisibilityStatus.OBSCURED;
                    }
                }

                //user.CurrentRoom.SealRoom(); // bad for some reasons, makes the screen black when spawning an enemy in the creepy room and using a controller
                user.CurrentRoom.CompletelyPreventLeaving = true;

                this.combatStarted = true;
                this.firstKeyBoardMouseBoolCheck = BraveInput.GetInstanceForPlayer(0).IsKeyboardAndMouse(false);

                AkSoundEngine.PostEvent("Stop_MUS_All", base.gameObject);
                AkSoundEngine.PostEvent("Play_MUS_Cathedral_Theme_01", base.gameObject);

                AIActor lowPriestLoadByGuid = EnemyDatabase.GetOrLoadByGuid(EnemyGuidDatabase.Entries["psychoman"]);
                IntVector2? intVector = new IntVector2?((user.CenterPosition + new Vector2(-2f, 0f)).ToIntVector2(VectorConversions.Floor));
                AIActor aiactor = AIActor.Spawn(lowPriestLoadByGuid.aiActor, intVector.Value, GameManager.Instance.Dungeon.data.GetAbsoluteRoomFromPosition(intVector.Value), true, AIActor.AwakenAnimationType.Spawn, true);
                aiactor.healthHaver.OnDeath += this.EndFight;
                aiactor.healthHaver.forcePreventVictoryMusic = true;
                if (this.isInCoop)
                {
                    aiactor.BecomeBlackPhantom();
                }
                aiactor.HasBeenEngaged = true;
                
                aiactor.HandleReinforcementFallIntoRoom(0.5f);
            }
            catch (Exception e)
            {
                Tools.PrintException(e);
            }
        }
        
        private IEnumerator ReturnWarp(PlayerController user)
        {
            yield return new WaitForSeconds(1f);
            user.CurrentRoom.CompletelyPreventLeaving = false;

            // to prevent the player from moving during the teleport animation
            user.ForceStopDodgeRoll();
            user.MovementModifiers += this.NoMotionModifier;
            user.IsStationary = true;
            user.IsGunLocked = true;
            this.DoTentacleVFX(user);

            yield return new WaitForSeconds(1.3f);
            Vector2 target = this.WarpTarget;
            user.WarpToPoint(target, false, false);
            user.IsStationary = false;
            user.IsGunLocked = false;
            user.MovementModifiers -= this.NoMotionModifier;
            if (isInCoop)
            {
                GameManager.Instance.GetOtherPlayer(user).ReuniteWithOtherPlayer(user, false);
            }
            yield return new WaitForSeconds(0.2f);
            
            RoomHandler room = user.CurrentRoom;
            IntVector2 randomVisibleClearSpot = user.CurrentRoom.GetRandomVisibleClearSpot(2, 2);
            Chest chest = Chest.Spawn(GameManager.Instance.RewardManager.S_Chest, randomVisibleClearSpot, user.sprite.WorldCenter.GetAbsoluteRoom(), true);
            PickupObject kalibersItem = GetRandomKaliberItem();
            PickupObject mgsItem = GetRandomMgsItem();
            List<PickupObject> contents = new List<PickupObject> { kalibersItem, mgsItem };
            chest.contents = contents;
            chest.IsLocked = false;
            if (isInCoop)
            {
                IntVector2 randomVisibleClearSpot2 = user.CurrentRoom.GetRandomVisibleClearSpot(2, 2);
                Chest chest2 = Chest.Spawn(GameManager.Instance.RewardManager.S_Chest, randomVisibleClearSpot2, user.sprite.WorldCenter.GetAbsoluteRoom(), true);
                PickupObject kalibersItem2 = GetRandomKaliberItem();
                PickupObject mgsItem2 = GetRandomMgsItem();
                List<PickupObject> contents2 = new List<PickupObject> { kalibersItem2, mgsItem2 };
                chest2.contents = contents2;
                chest2.IsLocked = false;
            }
            this.AddPassiveStatModifier(PlayerStats.StatType.AdditionalItemCapacity, -25f, StatModifier.ModifyMethod.ADDITIVE); // to prevent the owner to drop this if he pick up one too many active during the combat
            this.LastOwner.stats.RecalculateStats(this.LastOwner, false);
            user.RemoveActiveItem(this.PickupObjectId);
            yield break;
        }

        private PickupObject GetRandomKaliberItem()
        {
            int randomSelect = Random.Range(1, 11);
            PickupObject kalibersItem = PickupObjectDatabase.GetById(377);
            switch (randomSelect)
            {
                case 1: kalibersItem = PickupObjectDatabase.GetById(377); break; //Excaliber
                case 2: kalibersItem = PickupObjectDatabase.GetById(689); break; //abyssal_tentacle+kalibers_grip
                case 3: kalibersItem = PickupObjectDatabase.GetById(631); break; //holey_grail
                case 4: kalibersItem = PickupObjectDatabase.GetById(407); break; //sixth_chamber
                case 5: kalibersItem = PickupObjectDatabase.GetById(761); break; //high_kaliber
                case 6: kalibersItem = PickupObjectDatabase.GetById(570); break; //yellow_chamber
                case 7: kalibersItem = PickupObjectDatabase.GetById(323); break; //angry_bullets
                case 8: kalibersItem = PickupObjectDatabase.GetById(118); break; //eyepatch
                case 9: kalibersItem = PickupObjectDatabase.GetById(677); break; //dragunfire+kalibreath
                case 10: kalibersItem = PickupObjectDatabase.GetById(271); break; //riddle_of_lead
                default: break;
            }
            return kalibersItem;
        }

        private PickupObject GetRandomMgsItem()
        {
            int randomSelect1 = Random.Range(1, 18);
            PickupObject mgsItem = PickupObjectDatabase.GetById(203); //cigarettes
            switch (randomSelect1)
            {
                case 1: mgsItem = PickupObjectDatabase.GetById(203); break; //cigarettes 
                case 2: mgsItem = PickupObjectDatabase.GetById(98); break; //patriot
                case 3: mgsItem = PickupObjectDatabase.GetById(492); break; //wolf
                case 4: mgsItem = PickupObjectDatabase.GetById(84); break; //vulcan_cannon
                case 5: mgsItem = PickupObjectDatabase.GetById(5); break; //awp
                case 6: mgsItem = PickupObjectDatabase.GetById(105); break; //fortunes_favor
                case 7: mgsItem = PickupObjectDatabase.GetById(358); break; //railgun
                case 8: mgsItem = PickupObjectDatabase.GetById(314); break; //nanomachines
                case 9: mgsItem = PickupObjectDatabase.GetById(816); break; //trank_gun_dupe_1
                case 10: mgsItem = PickupObjectDatabase.GetById(255); break; //ancient_heros_bandana
                case 11: mgsItem = PickupObjectDatabase.GetById(216); break; //box
                case 12: mgsItem = PickupObjectDatabase.GetById(104); break; //ration
                case 13: mgsItem = PickupObjectDatabase.GetById(474); break; //abyssal_tentacle
                case 14: mgsItem = PickupObjectDatabase.GetById(50); break; //saa
                case 15: mgsItem = PickupObjectDatabase.GetById(129); break; //com4nd0
                case 16: mgsItem = PickupObjectDatabase.GetById(500); break; //hip_holster
                case 17: mgsItem = PickupObjectDatabase.GetById(370); break; //prototype_railgun
                default: break;
            }
            return mgsItem;
        }

        private GameObject DoTentacleVFX(PlayerController user)
        {
            this.TentacleVFX = (GameObject)ResourceCache.Acquire("Global VFX/VFX_Tentacleport");
            GameObject gameObject = UnityEngine.Object.Instantiate<GameObject>(this.TentacleVFX);
            gameObject.GetComponent<tk2dBaseSprite>().PlaceAtLocalPositionByAnchor(user.specRigidbody.UnitBottomCenter + new Vector2(0f, -1f), tk2dBaseSprite.Anchor.LowerCenter);
            gameObject.transform.position = gameObject.transform.position.Quantize(0.0625f);
            gameObject.GetComponent<tk2dBaseSprite>().UpdateZDepth();
            return gameObject;
        }

        private void NoMotionModifier(ref Vector2 voluntaryVel, ref Vector2 involuntaryVel)
        {
            voluntaryVel = Vector2.zero;
            involuntaryVel = Vector2.zero;
        }

        private void EndFight(Vector2 target2)
        {
            this.combatEnded = true;
            dizzyness = 0;
            StatCorrector();
            this.LastOwner.stats.RecalculateStats(LastOwner, true);
            //AkSoundEngine.PostEvent("Stop_MUS_All", base.gameObject);
            //AkSoundEngine.PostEvent("Play_MUS_Dungeon_State_Winner", base.gameObject);
        }

        public override void Update()
        {
            if (this.combatStarted && !this.combatEnded && !this.isInCoop)
            {
                if (BraveInput.GetInstanceForPlayer(0).IsKeyboardAndMouse(false) == this.firstKeyBoardMouseBoolCheck)
                {
                    //Tools.Print("Moohahahaha", "FFFFFF", true);
                    this.dizzyness = 1;
                }
                else
                {
                    //Tools.Print("Cheater !", "FFFFFF", true);
                    this.dizzyness = 0;
                    StatCorrector();

                }
            }
            if (dizzyness == 1) // from knife to a gunfight with the accord of skilotar_
            {
                if(counter%200 == 0)
                {
                    int randomSelect = Random.Range(1, 5);
                    switch (randomSelect)
                    {
                        case 1:
                            this.LastOwner.CurrentStoneGunTimer = 2f; //Gorgun petrify effect
                            break;
                        case 2:
                            ItemBuilder.AddPassiveStatModifier(this, PlayerStats.StatType.MovementSpeed, -1f, StatModifier.ModifyMethod.MULTIPLICATIVE); // switch speed, changing the player movement direction 
                            break;
                        case 3:
                            this.LastOwner.ForceStopDodgeRoll();
                            break;
                        case 4:
                            AkSoundEngine.PostEvent("Play_ENM_screamer_scream_01", gameObject);
                            this.LastOwner.CurrentRoom.BecomeTerrifyingDarkRoom();
                            base.StartCoroutine(this.EndDarkRoom(this.LastOwner));
                            break;
                        default:
                            break;// reverse fire speed, reverse dodge roll speed
                            //GameUIRoot.Instance.ForceHideGunPanel = !GameUIRoot.Instance.ForceHideGunPanel;
                    }
                    this.LastOwner.stats.RecalculateStats(LastOwner, true);
                }
                counter++;
            }
            base.Update();
        }

        private IEnumerator EndDarkRoom(PlayerController lastOwner)
        {
            yield return new WaitForSeconds(3f);
            lastOwner.CurrentRoom.EndTerrifyingDarkRoom();
            yield break;
        }

        public void StatCorrector()
        {
            if (!this.LastOwner || !this.LastOwner.stats) return;
            float speed = this.LastOwner.stats.GetStatValue(PlayerStats.StatType.MovementSpeed);
            if (speed < 0)
            {
                ItemBuilder.AddPassiveStatModifier(this, PlayerStats.StatType.MovementSpeed, -1f, StatModifier.ModifyMethod.MULTIPLICATIVE);
                this.LastOwner.stats.RecalculateStats(this.LastOwner, true);
            }
        }

        private bool used;
        private bool combatStarted;
        private bool combatEnded; // bool to check if the combat ended and if the player can return to their original location
        private GameObject TentacleVFX;
        private Vector2 WarpTarget; // vector to send back the user
        private bool firstKeyBoardMouseBoolCheck;
        private bool isInCoop;
        public int dizzyness = 0;
        public int counter = 0;
        private List<RoomHandler> roomsAtStartOfCombat;
    }



    public class TestOverrideBehavior : OverrideBehavior
    {
        public override string OverrideAIActorGUID => EnemyGuidDatabase.Entries["psychoman"]; // Replace the GUID with whatever enemy you want to modify. This GUID is for the bullet kin.
                                                                                          // You can find a full list of GUIDs at https://github.com/ModTheGungeon/ETGMod/blob/master/Assembly-CSharp.Base.mm/Content/gungeon_id_map/enemies.txt
        public override void DoOverride()
        {
            // In this method, you can do whatever you want with the enemy using the fields "actor", "healthHaver", "behaviorSpec", and "bulletBank".
            actor.OverrideDisplayName = "#LOW_PRIEST";
            actor.ActorName = "Low Priest";
            actor.MovementSpeed *= 0.9f;

            healthHaver.SetHealthMaximum(healthHaver.GetMaxHealth() * 1.2f);
            
            AttackBehaviorGroup attackBehaviorGroup = (behaviorSpec.AttackBehaviors[0] as AttackBehaviorGroup);
            ShootBehavior shootBehavior = (attackBehaviorGroup.AttackBehaviors[0].Behavior as ShootBehavior);
            shootBehavior.LeadAmount = 1.5f;

            shootBehavior.BulletScript = new CustomBulletScriptSelector(typeof(TestBulletScript));
            
        }

        public class TestBulletScript : Script // This BulletScript is just a modified version of the script BulletManShroomed, which you can find with dnSpy.
        {
            protected override IEnumerator Top()
            {
                int randomSelect = Random.Range(1, 3);

                switch (randomSelect)
                {
                    case 1:
                        float num = base.RandomAngle();
                        float num2 = 30f;
                        int i = 0;
                        GameObject obj = PickupObjectDatabase.GetByEncounterName("High Priest's Secret Map").gameObject;
                        AkSoundEngine.PostEvent("Play_ENM_highpriest_blast_01", obj);
                        //m_BOSS_doormimic_eyes_01
                        for (i = 0; i < 12; i++)
                        {
                            base.Fire(new Direction(num + (float)i * num2, DirectionType.Absolute, -1f), new Speed(9f, SpeedType.Absolute), null);
                        }
                        yield return Wait(20); // Wait for 20 frames

                        base.Fire(new Direction(-40f, DirectionType.Aim, -1f), new Speed(6f, SpeedType.Absolute), null); // Shoot a bullet -40 degrees from the enemy aim angle, with a bullet speed of 6.
                        base.Fire(new Direction(40f, DirectionType.Aim, -1f), new Speed(6f, SpeedType.Absolute), null); // Shoot a bullet 40 degrees from the enemy aim angle, with a bullet speed of 6
                        base.Fire(new Direction(-20f, DirectionType.Aim, -1f), new Speed(9f, SpeedType.Absolute), null); // Shoot a bullet -20 degrees from the enemy aim angle, with a bullet speed of 9.
                        base.Fire(new Direction(20f, DirectionType.Aim, -1f), new Speed(9f, SpeedType.Absolute), null); // Shoot a bullet 20 degrees from the enemy aim angle, with a bullet speed of 9.

                        break;
                    case 2:
                        GameObject obj2 = PickupObjectDatabase.GetByEncounterName("High Priest's Secret Map").gameObject;
                        AkSoundEngine.PostEvent("Play_ENM_highpriest_blast_01", obj2);
                        bool facingRight = BraveMathCollege.AbsAngleBetween(this.BulletBank.aiAnimator.FacingDirection, 0f) < 90f; //from agumin
                        float startAngle = (!facingRight) ? -170f : -10f;
                        float deltaAngle = ((!facingRight) ? 160f : -160f) / 19f;
                        float deltaT = 0.7894737f;
                        float t = 0f;
                        int i1 = 0;
                        while ((float)i1 < 15f)
                        {
                            float angle = startAngle + (float)i1 * deltaAngle;
                            for (t += deltaT; t > 1f; t -= 1f)
                            {
                                yield return this.Wait(1);
                            }
                            Vector2 offset = BraveMathCollege.GetEllipsePoint(Vector2.zero, 2.25f, 1.5f, angle);
                            for (int j = 0; j < 2; j++)
                            {
                                this.Fire(new Offset(offset, 0f, string.Empty, DirectionType.Absolute), new Direction(angle + UnityEngine.Random.Range(-25f, 25f), DirectionType.Absolute, -1f), new Speed(UnityEngine.Random.Range(2f, 5f) - (float)j, SpeedType.Absolute), new DelayedBullet("default", j * 9));
                            }
                            i1++;
                        }
                        break;
                    default:
                        break;
                }
                yield break;
            }

            // Token: 0x04000CB9 RID: 3257
            private const int NumBullets = 12;
        }
    }
}
