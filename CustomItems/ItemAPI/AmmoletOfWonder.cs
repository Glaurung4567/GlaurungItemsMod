using System.Collections.Generic;
using UnityEngine;
using ItemAPI;
using Dungeonator;
using Random = UnityEngine.Random;

namespace GlaurungItems.Items
{
    class AmmoletOfWonder : SpecialBlankModificationItem
    {
        public static void Init()
        {
            string itemName = "Ammolet Of Wonder";
            string resourceName = "GlaurungItems/Resources/acme_crate";
            GameObject obj = new GameObject(itemName);
            var item = obj.AddComponent<AmmoletOfWonder>();
            ItemBuilder.AddSpriteToObject(itemName, resourceName, obj);
            string shortDesc = "WIP";
            string longDesc = "WIP";
            ItemBuilder.SetupItem(item, shortDesc, longDesc, "gl");
            ItemBuilder.AddPassiveStatModifier(item, PlayerStats.StatType.AdditionalBlanksPerFloor, 1, StatModifier.ModifyMethod.ADDITIVE);
            item.quality = ItemQuality.A;
            item.BlankStunTime = 0;
        }

        protected override void OnBlank(Vector2 centerPoint, PlayerController user)
        {
            List<AIActor> activeEnemies = GameManager.Instance.Dungeon.data.GetAbsoluteRoomFromPosition(centerPoint.ToIntVector2(VectorConversions.Round)).GetActiveEnemies(RoomHandler.ActiveEnemyType.All);

            int randomSelect = Random.Range(1, 3);
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
                default:
                    break;
                
            }
        }

        private void ApplyEffectOnEnnemies(List<AIActor> activeEnemies, GameActorEffect effect, float chanceToActivate = 1)
        {
            if (activeEnemies != null)
            {
                for (int j = 0; j < activeEnemies.Count; j++)
                {
                    if(UnityEngine.Random.value <= chanceToActivate)
                    {
                        AIActor aiactor = activeEnemies[j];
                        if (aiactor != null) { aiactor.ApplyEffect(effect); }
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

    }
}