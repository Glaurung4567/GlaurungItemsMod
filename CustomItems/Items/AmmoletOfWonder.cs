using System.Collections.Generic;
using UnityEngine;
using ItemAPI;
using EnemyAPI;
using Dungeonator;
using Random = UnityEngine.Random;
using System.Collections;

namespace GlaurungItems.Items
{
    class AmmoletOfWonder : SpecialBlankModificationItem
    {
        public static void Init()
        {
            string itemName = "Ammolet Of Wonder";
            string resourceName = "GlaurungItems/Resources/ammolet_of_wonder";
            GameObject obj = new GameObject(itemName);
            var item = obj.AddComponent<AmmoletOfWonder>();
            ItemBuilder.AddSpriteToObject(itemName, resourceName, obj);
            string shortDesc = "Many Things";
            string longDesc = "Trigger a random effect when a blank is activated. \n \nImbued with the different energies present in the Gungeon, it makes blanks more unpredictable.";
            ItemBuilder.SetupItem(item, shortDesc, longDesc, "gl");
            ItemBuilder.AddPassiveStatModifier(item, PlayerStats.StatType.AdditionalBlanksPerFloor, 1, StatModifier.ModifyMethod.ADDITIVE);
            item.quality = ItemQuality.C;
            item.BlankStunTime = 0;
        }

        protected override void OnBlank(SilencerInstance silencerInstance, Vector2 centerPoint, PlayerController user)
        {
            List<AIActor> activeEnemies = GameManager.Instance.Dungeon.data.GetAbsoluteRoomFromPosition(centerPoint.ToIntVector2(VectorConversions.Round)).GetActiveEnemies(RoomHandler.ActiveEnemyType.All);

            int randomSelect = Random.Range(1, 46);
            switch (randomSelect)
            {
                case 1:
                    GameActorEffect greenFire = (PickupObjectDatabase.GetById(722) as Gun).DefaultModule.projectiles[0].fireEffect;
                    ApplyEffectOnEnnemies(activeEnemies, greenFire);
                    break;
                case 2:
                    string enemyGuid = EnemyGuidDatabase.Entries["rat_candle"];
                    SummonActors(user, enemyGuid);
                    break;
                case 3:
                    AkSoundEngine.PostEvent("Play_VO_bombshee_death_01", gameObject);
                    break;
                case 4:
                    silencerInstance.ForceNoDamage = true;
                    break;
                case 5:
                    base.StartCoroutine(this.RecursiveBlank(centerPoint, user));
                    break;
                case 6:
                    Vector2 vector = user.SpriteBottomCenter + new Vector3(-0.75f, +0.25f, 0f);
                    for (int i = 0; i < 8; i++)
                    {
                        GameObject original = (GameObject)ResourceCache.Acquire(AmmoletOfWonder.confettiPaths[UnityEngine.Random.Range(0, 3)]);
                        WaftingDebrisObject component = UnityEngine.Object.Instantiate<GameObject>(original).GetComponent<WaftingDebrisObject>();
                        component.sprite.PlaceAtPositionByAnchor(vector.ToVector3ZUp(0f) + new Vector3(0.5f, 0.5f, 0f), tk2dBaseSprite.Anchor.MiddleCenter);
                        Vector2 insideUnitCircle = UnityEngine.Random.insideUnitCircle;
                        insideUnitCircle.y = -Mathf.Abs(insideUnitCircle.y);
                        component.Trigger(insideUnitCircle.ToVector3ZUp(1.5f) * UnityEngine.Random.Range(0.5f, 2f), 0.5f, 0f);
                    }
                    if (activeEnemies != null)
                    {
                        for (int j = 0; j < activeEnemies.Count; j++)
                        {
                            AIActor actor = activeEnemies[j];
                            HealthHaver hh = actor.healthHaver;
                            for (int i = 0; i < hh.bodySprites.Count; i++)
                            {
                                hh.bodySprites[i].usesOverrideMaterial = true;
                                int num = 100;
                                float lerpTime = 0.1f;
                                float globalSparksForce = 5f;
                                float globalSparksOverrideLifespan = -1f;
                                GlobalSparksDoer.EmitFromRegion(GlobalSparksDoer.EmitRegionStyle.RANDOM, (float)num, lerpTime + 0.1f, 
                                    hh.bodySprites[i].WorldBottomLeft.ToVector3ZisY(0f), hh.bodySprites[i].WorldTopRight.ToVector3ZisY(0f), 
                                    new Vector3(globalSparksForce, globalSparksForce, globalSparksForce), 15f, 0.5f, null, (globalSparksOverrideLifespan <= 0f) ? null : new float?(globalSparksOverrideLifespan), 
                                    new Color?(Color.cyan), GlobalSparksDoer.SparksType.SOLID_SPARKLES);
                            }
                        }
                    }
                    break;
                case 7: //slow effect
                    Gun gun = ETGMod.Databases.Items["triple_crossbow"] as Gun;
                    GameActorSpeedEffect gameActorSpeedEffect = gun.DefaultModule.projectiles[0].speedEffect;
                    gameActorSpeedEffect.duration = 4f;
                    ApplyEffectOnEnnemies(activeEnemies, gameActorSpeedEffect);
                    break;
                case 8://debuff on enemies
                    AIActorDebuffEffect debuffEffect = null;
                    foreach (AttackBehaviorBase attackBehaviour in EnemyDatabase.GetOrLoadByGuid((PickupObjectDatabase.GetById(492) as CompanionItem).CompanionGuid).behaviorSpeculator.AttackBehaviors)
                    {
                        if (attackBehaviour is WolfCompanionAttackBehavior)
                        {
                            debuffEffect = (attackBehaviour as WolfCompanionAttackBehavior).EnemyDebuff;
                        }
                    }
                    ApplyEffectOnEnnemies(activeEnemies, debuffEffect);
                    break;
                case 9://buff enemies
                    AIActor aiactor = EnemyDatabase.GetOrLoadByGuid(EnemyGuidDatabase.Entries["aged_gunsinger"]);
                    AttackBehaviorGroup attackBehaviorGroup = (aiactor.behaviorSpeculator.AttackBehaviors[0] as AttackBehaviorGroup);
                    BuffEnemiesBehavior buffBehavior = (attackBehaviorGroup.AttackBehaviors[0].Behavior as BuffEnemiesBehavior);
                    AIActorBuffEffect buffEffect = buffBehavior.buffEffect;
                    buffEffect.AffectsEnemies = true;
                    buffEffect.duration = 10f;
                    //user.ApplyEffect(buffEffect);
                    ApplyEffectOnEnnemies(activeEnemies, buffEffect);
                    break;
                case 10:
                    if (activeEnemies != null)
                    {
                        for (int j = 0; j < activeEnemies.Count; j++)
                        {
                            AIActor actor = activeEnemies[j];
                            if (actor.behaviorSpeculator != null)
                            {
                                actor.RegisterOverrideColor(Color.red, "AoW");
                            }
                        }
                    }
                    break;
                case 11:
                    GameActorCheeseEffect gameActorCheeseEffect = (PickupObjectDatabase.GetById(626) as Gun).DefaultModule.projectiles[0].cheeseEffect;
                    ApplyEffectOnEnnemies(activeEnemies, gameActorCheeseEffect, 1, Random.Range(1,5));
                    break;
                case 12:
                    if (activeEnemies != null)
                    {
                        for (int j = 0; j < activeEnemies.Count; j++)
                        {
                            AIActor actor = activeEnemies[j];
                            if (actor.behaviorSpeculator != null)
                            {
                                actor.RegisterOverrideColor(Color.green, "AoW");
                            }
                        }
                    }
                    break;
                case 13:
                    string enemyGuid2 = EnemyGuidDatabase.Entries["chicken"];
                    SummonActors(user, enemyGuid2);
                    break;
                case 14:
                    string enemyGuid3 = EnemyGuidDatabase.Entries["poopulons_corn"];
                    SummonActors(user, enemyGuid3);//snake
                    break;
                case 15:
                    string enemyGuid4 = EnemyGuidDatabase.Entries["snake"];
                    SummonActors(user, enemyGuid4);
                    break;
                case 16:
                    string enemyGuid5 = EnemyGuidDatabase.Entries["rat"];
                    SummonActors(user, enemyGuid5);
                    break;
                case 17:
                    string enemyGuid6 = EnemyGuidDatabase.Entries["red_caped_bullet_kin"];
                    SummonActors(user, enemyGuid6, 1);
                    break;
                case 18:
                    FleePlayerData fleeData = new FleePlayerData();
                    fleeData.Player = user;
                    fleeData.StartDistance = 100f;
                    if (activeEnemies != null)
                    {
                        for (int j = 0; j < activeEnemies.Count; j++)
                        {
                            AIActor actor = activeEnemies[j];
                            if(actor.behaviorSpeculator != null)
                            {
                                actor.behaviorSpeculator.FleePlayerData = fleeData;
                            }
                        }
                    }
                    break;
                case 19:
                    if (activeEnemies != null)
                    {
                        for (int j = 0; j < activeEnemies.Count; j++)
                        {
                            AIActor actor = activeEnemies[j];
                            if (actor.behaviorSpeculator != null)
                            {
                                actor.behaviorSpeculator.Stun(3f);
                            }
                        }
                    }
                    break;
                case 20:
                    user.RespawnInPreviousRoom(false, PlayerController.EscapeSealedRoomStyle.TELEPORTER, true, null);
                    break;
                case 21:
                    if (activeEnemies != null)
                    {
                        for (int j = 0; j < activeEnemies.Count; j++)
                        {
                            AIActor actor = activeEnemies[j];
                            if (actor.IsBlackPhantom)
                            {
                                actor.UnbecomeBlackPhantom();
                            }
                            else if(actor.healthHaver && !actor.healthHaver.IsBoss && !actor.IsBlackPhantom)
                            {
                                actor.BecomeBlackPhantom();
                            }
                        }
                    }
                    break;
                case 22:
                    if (activeEnemies != null)
                    {
                        for (int j = 0; j < activeEnemies.Count; j++)
                        {
                            AIActor actor = activeEnemies[j];
                            if (!actor.IsFlying && !actor.healthHaver.IsBoss)
                            {
                                actor.SetIsFlying(true, "ammolet of wonder");
                            }
                        }
                    }
                    break;
                case 23:
                    //from https://github.com/SpecialAPI/SpecialItemPack/blob/2d321b45c267602e4cc3b47cd656fe3cd6273bba/TableTechChaos.cs#L499
                    this.BreakStealth(user);
                    user.OnItemStolen += this.BreakStealthOnSteal;
                    user.ChangeSpecialShaderFlag(1, 1f);
                    user.healthHaver.OnDamaged += this.OnDamaged;
                    user.SetIsStealthed(true, "ammolet of wonder");
                    user.SetCapableOfStealing(true, "ammolet of wonder", null);
                    GameManager.Instance.StartCoroutine(this.Unstealthy());
                    break;
                case 24:
                    GameActorEffect iceEffect = (PickupObjectDatabase.GetById(402) as Gun).DefaultModule.projectiles[0].freezeEffect;
                    ApplyEffectOnEnnemies(activeEnemies, iceEffect, 1, Random.Range(2, 5));
                    break;
                case 25:
                    GameActorEffect fireEffect = (PickupObjectDatabase.GetById(125) as Gun).DefaultModule.projectiles[0].fireEffect;
                    ApplyEffectOnEnnemies(activeEnemies, fireEffect);
                    break;
                case 26:
                    string enemyGuid7 = EnemyGuidDatabase.Entries["dragun_egg_slimeguy"];
                    SummonActors(user, enemyGuid7);
                    break;
                case 27:
                    SpawnGoop(user, EasyGoopDefinitions.PoisonDef);
                    break;
                case 28:
                    SpawnGoop(user, EasyGoopDefinitions.WaterGoop);
                    break;
                case 29:
                    SpawnGoop(user, EasyGoopDefinitions.OilDef);
                    break;
                case 30:
                    SpawnGoop(user, EasyGoopDefinitions.CharmGoopDef);
                    break;
                case 31:
                    SpawnGoop(user, EasyGoopDefinitions.CheeseDef);
                    break;
                case 32:
                    SpawnGoop(user, EasyGoopDefinitions.WebGoop);
                    break;
                case 33:
                    SpawnGoop(user, EasyGoopDefinitions.BlobulonGoopDef);
                    break;
                case 34:
                    SpawnGoop(user, EasyGoopDefinitions.WaterGoop, true);
                    break;
                case 35:
                    SpawnGoop(user, EasyGoopDefinitions.GreenFireDef);
                    break;
                case 36:
                    SpawnGoop(user, EasyGoopDefinitions.FireDef);
                    break;
                case 37:
                    GameActorEffect charmEffect = (PickupObjectDatabase.GetById(379) as Gun).DefaultModule.projectiles[0].charmEffect;
                    ApplyEffectOnEnnemies(activeEnemies, charmEffect);
                    break;
                case 38:
                    if (activeEnemies != null)
                    {
                        for (int j = 0; j < activeEnemies.Count; j++)
                        {
                            AIActor actor = activeEnemies[j];
                            if (!actor.IsHarmlessEnemy)
                            {
                                actor.IsHarmlessEnemy = true;
                                actor.IsWorthShootingAt = false;
                            }
                        }
                    }
                    break;
                case 39:
                    if(user.CurrentRoom != null)
                    {
                        IntVector2 randomVisibleClearSpot2 = user.CurrentRoom.GetRandomVisibleClearSpot(2, 2);
                        if (user.IsValidPlayerPosition(randomVisibleClearSpot2.ToCenterVector2()))
                        {
                            user.WarpToPoint(randomVisibleClearSpot2.ToCenterVector2(), true);
                        }
                    }
                    break;
                case 40:
                    StatModifier statModifier = new StatModifier();
                    statModifier.statToBoost = PlayerStats.StatType.TarnisherClipCapacityMultiplier;
                    statModifier.amount = -0.15f;
                    statModifier.modifyType = StatModifier.ModifyMethod.ADDITIVE;
                    user.ownerlessStatModifiers.Add(statModifier);
                    user.stats.RecalculateStats(user, true);
                    user.PlayEffectOnActor((GameObject)ResourceCache.Acquire("Global VFX/VFX_Tarnisher_Effect"), new Vector3(0f, 0.5f, 0f), true, false, false);
                    if (user.carriedConsumables != null)
                    {
                        user.carriedConsumables.ForceUpdateUI();
                    }
                    break;
                case 41:
                    if (activeEnemies != null)
                    {
                        for (int j = 0; j < activeEnemies.Count; j++)
                        {
                            AIActor actor = activeEnemies[j];
                            if (actor.healthHaver != null)
                            {
                                MindControlEffect orAddComponent = GameObjectExtensions.GetOrAddComponent<MindControlEffect>(actor.gameObject);
                                orAddComponent.owner = user;
                            }
                        }
                    }
                    break;
                case 42:
                    if (activeEnemies != null)
                    {
                        for (int j = 0; j < activeEnemies.Count; j++)
                        {
                            AIActor actor = activeEnemies[j];
                            if (actor.behaviorSpeculator != null)
                            {
                                actor.RegisterOverrideColor(Color.white, "AoW");
                            }
                        }
                    }
                    break;
                case 43:
                    if (activeEnemies != null)
                    {
                        for (int j = 0; j < activeEnemies.Count; j++)
                        {
                            AIActor actor = activeEnemies[j];
                            if (actor.behaviorSpeculator != null)
                            {
                                actor.RegisterOverrideColor(Color.blue, "AoW");
                            }
                        }
                    }
                    break;
                case 44:
                    if (activeEnemies != null)
                    {
                        for (int j = 0; j < activeEnemies.Count; j++)
                        {
                            AIActor actor = activeEnemies[j];
                            if (actor.behaviorSpeculator != null)
                            {
                                actor.RegisterOverrideColor(Color.gray, "AoW");
                            }
                        }
                    }
                    break;
                case 45:
                    GameActorEffect poisonEffect = (PickupObjectDatabase.GetById(513) as Gun).DefaultModule.projectiles[0].healthEffect;
                    ApplyEffectOnEnnemies(activeEnemies, poisonEffect);
                    break;
                default:
                    this.BlankForceMultiplier = 0;
                    base.StartCoroutine(this.ResetBlankModifierStats());
                    break;
                    //slow time
                    //color player (red/green/white/blue/gray)
                    //change size
                    //set on fire ko
                    //IsEthereal ko

            }
        }

        private IEnumerator ResetBlankModifierStats()
        {
            yield return new WaitForSeconds(0.2f);
            this.BlankForceMultiplier = 1f;
            this.BlankDamage = 20f;
            this.BlankDamageRadius = 10f;
            yield break;
        }

        private void SpawnGoop(PlayerController user, GoopDefinition goop, bool freeze = false, bool electrify = false)
        {
            float radius = Random.Range(1.5f, 4f);
            Vector2 position = user.sprite.WorldBottomCenter;
            DeadlyDeadlyGoopManager.GetGoopManagerForGoopType(goop).TimedAddGoopCircle(position, radius, Random.Range(0.5f, 2f));
            if (freeze)
            {
                DeadlyDeadlyGoopManager.FreezeGoopsCircle(position, radius);
            }
            if (electrify)
            {
                DeadlyDeadlyGoopManager.ElectrifyGoopsLine(position, position, radius);
            }
        }

        private IEnumerator Unstealthy()
        {
            PlayerController player = base.Owner;
            yield return new WaitForSeconds(0.15f);
            player.OnDidUnstealthyAction += this.BreakStealth;
            yield break;
        }

        private void OnDamaged(float resultValue, float maxValue, CoreDamageTypes damageTypes, DamageCategory damageCategory, Vector2 damageDirection)
        {
            PlayerController owner = base.Owner;
            this.BreakStealth(owner);
        }

        private void BreakStealthOnSteal(PlayerController arg1, ShopItemController arg2)
        {
            this.BreakStealth(arg1);
        }

        private void BreakStealth(PlayerController player)
        {
            player.ChangeSpecialShaderFlag(1, 0f);
            player.OnItemStolen -= this.BreakStealthOnSteal;
            player.SetIsStealthed(false, "ammolet of wonder");
            player.healthHaver.OnDamaged -= this.OnDamaged;
            player.SetCapableOfStealing(false, "ammolet of wonder", null);
            player.OnDidUnstealthyAction -= this.BreakStealth;
            AkSoundEngine.PostEvent("Play_ENM_wizardred_appear_01", base.gameObject);
        }

        private IEnumerator RecursiveBlank(Vector2 centerPoint, PlayerController user)
        {
            yield return new WaitForSeconds(0.5f);
            GameObject gameObjectBlankVFX = (GameObject)ResourceCache.Acquire("Global VFX/BlankVFX");
            GameObject gameObjectBlank = new GameObject("silencer");
            SilencerInstance silencerInstance = gameObjectBlank.AddComponent<SilencerInstance>();
            silencerInstance.TriggerSilencer(centerPoint, 50f, 25f, gameObjectBlankVFX, 0.25f, 0.2f, 50f, 10f, 140f, 15f, 0.5f, user, true, false);
            yield break;
        }

        private void ApplyEffectOnEnnemies(List<AIActor> activeEnemies, GameActorEffect effect, 
            float chanceToActivate = 1, int numberOfTimeToApplyEffect = 1)
        {
            if (activeEnemies != null)
            {
                for (int i = 0; i < numberOfTimeToApplyEffect; i++)
                {
                    for (int j = 0; j < activeEnemies.Count; j++)
                    {
                        if (UnityEngine.Random.value <= chanceToActivate)
                        {
                            AIActor aiactor = activeEnemies[j];
                            if (aiactor != null)
                            {
                                aiactor.ApplyEffect(effect);
                            }
                        }
                    }
                }
            }
        }

        private void SummonActors(PlayerController user, string guid, int maxNumToSummon = 4)
        {
            if(user != null && user.CurrentRoom != null)
            {
                int randSummon = Random.Range(1, maxNumToSummon);

                for (int j = 1; j <= randSummon; j++)
                {
                    IntVector2? intVector = new IntVector2?(user.CurrentRoom.GetRandomVisibleClearSpot(2, 2));
                    AIActor orLoadByGuid = EnemyDatabase.GetOrLoadByGuid(guid);
                    AIActor aiactor = AIActor.Spawn(orLoadByGuid.aiActor, intVector.Value, GameManager.Instance.Dungeon.data.GetAbsoluteRoomFromPosition(intVector.Value), true, AIActor.AwakenAnimationType.Default, true);
                }

            }

        }

        private static string[] confettiPaths = new string[]
        {
            "Global VFX/Confetti_Blue_001",
            "Global VFX/Confetti_Yellow_001",
            "Global VFX/Confetti_Green_001"
        };
    }
}
