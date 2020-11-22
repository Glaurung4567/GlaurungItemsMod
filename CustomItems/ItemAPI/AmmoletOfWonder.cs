using System.Collections.Generic;
using UnityEngine;
using ItemAPI;
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
            string longDesc = "WIP";
            ItemBuilder.SetupItem(item, shortDesc, longDesc, "gl");
            ItemBuilder.AddPassiveStatModifier(item, PlayerStats.StatType.AdditionalBlanksPerFloor, 1, StatModifier.ModifyMethod.ADDITIVE);
            item.quality = ItemQuality.B;
            item.BlankStunTime = 0;
        }

        protected override void OnBlank(SilencerInstance silencerInstance, Vector2 centerPoint, PlayerController user)
        {
            List<AIActor> activeEnemies = GameManager.Instance.Dungeon.data.GetAbsoluteRoomFromPosition(centerPoint.ToIntVector2(VectorConversions.Round)).GetActiveEnemies(RoomHandler.ActiveEnemyType.All);

            int randomSelect = Random.Range(6, 7);
            Tools.Print(randomSelect, "ffffff", true);
            switch (randomSelect)
            {
                case 1:
                    GameActorEffect greenFire = (PickupObjectDatabase.GetById(722) as Gun).DefaultModule.projectiles[0].fireEffect;
                    ApplyEffectOnEnnemies(centerPoint, activeEnemies, greenFire);
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
                            actor.HasBeenGlittered = true;
                        }
                    }
                    break;

                default:
                    break;
                
            }
        }

        private IEnumerator RecursiveBlank(Vector2 centerPoint, PlayerController user)
        {
            yield return new WaitForSeconds(0.5f);
            SilencerInstance silencer = new SilencerInstance();
            GameObject blankVFXPrefab = (GameObject)BraveResources.Load("Global VFX/BlankVFX", ".prefab");
            silencer.TriggerSilencer(centerPoint, 20f, 5f, blankVFXPrefab, 0f, 4f, 3f, 4f, 30f, 4f, 0.25f, user, false, false);
            yield break;
        }

        private void ApplyEffectOnEnnemies(Vector2 centerPoint, List<AIActor> activeEnemies, GameActorEffect effect, float chanceToActivate = 1)
        {
            if (activeEnemies != null)
            {
                for (int j = 0; j < activeEnemies.Count; j++)
                {
                    if(UnityEngine.Random.value <= chanceToActivate)
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