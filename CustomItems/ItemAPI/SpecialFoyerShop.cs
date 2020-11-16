using MonoMod.RuntimeDetour;
using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

namespace ItemAPI
{
    // by spapi the synergistic chamber 
    public static class SpecialFoyerShops
    {
        public static void DoSetup()
        {
            BaseMetaShopController = LoadHelper.LoadAssetFromAnywhere<GameObject>("Foyer_MetaShop").GetComponent<MetaShopController>();
            TrorcMetaShopItems = LoadHelper.LoadAssetFromAnywhere<GenericLootTable>("Shop_Truck_Meta");
            GooptonMetaShopItems = LoadHelper.LoadAssetFromAnywhere<GenericLootTable>("Shop_Goop_Meta");
            DougMetaShopItems = LoadHelper.LoadAssetFromAnywhere<GenericLootTable>("Shop_Beetle_Meta");
            Hook h = new Hook(
                typeof(PickupObject).GetMethod("HandleEncounterable", BindingFlags.NonPublic | BindingFlags.Instance),
                typeof(SpecialFoyerShops).GetMethod("HandleEncounterableHook")
            );
            Hook h2 = new Hook(
                typeof(BaseShopController).GetMethod("DoSetup", BindingFlags.NonPublic | BindingFlags.Instance),
                typeof(SpecialFoyerShops).GetMethod("BaseShopSetupHook")
            );
            Hook h3 = new Hook(
                typeof(MetaShopController).GetMethod("DoSetup", BindingFlags.NonPublic | BindingFlags.Instance),
                typeof(SpecialFoyerShops).GetMethod("MetaSetupHook")
            );
        }

        public static void HandleEncounterableHook(Action<PickupObject, PlayerController> orig, PickupObject po, PlayerController player)
        {
            orig(po, player);
            if (po != null && po.GetComponent<SpecialPickupObject>() != null && po.GetComponent<SpecialPickupObject>().CustomSaveFlagToSetOnAcquisition != CustomDungeonFlags.NONE)
            {
                AdvancedGameStatsManager.Instance.SetFlag(po.GetComponent<SpecialPickupObject>().CustomSaveFlagToSetOnAcquisition, true);
            }
        }

        public static void BaseShopSetupHook(Action<BaseShopController> orig, BaseShopController self)
        {
            orig(self);
            if (self.baseShopType == BaseShopController.AdditionalShopType.FOYER_META && self.ExampleBlueprintPrefab != null)
            {
                List<ShopItemController> shopItems = (List<ShopItemController>)BaseItemControllersInfo.GetValue(self);
                if (shopItems != null)
                {
                    foreach (ShopItemController shopItem in shopItems)
                    {
                        if (shopItem != null && shopItem.item != null && shopItem.item.encounterTrackable != null && shopItem.item.encounterTrackable.journalData != null)
                        {
                            PickupObject po = GetBlueprintUnlockedItem(shopItem.item.encounterTrackable);
                            if (po != null && po.encounterTrackable != null && po.encounterTrackable.prerequisites != null)
                            {
                                CustomDungeonFlags saveFlagToSetOnAcquisition = CustomDungeonFlags.NONE;
                                for (int i = 0; i < po.encounterTrackable.prerequisites.Length; i++)
                                {
                                    if (po.encounterTrackable.prerequisites[i] is AdvancedDungeonPrerequisite && (po.encounterTrackable.prerequisites[i] as AdvancedDungeonPrerequisite).advancedPrerequisiteType ==
                                        AdvancedDungeonPrerequisite.AdvancedPrerequisiteType.CUSTOM_FLAG)
                                    {
                                        saveFlagToSetOnAcquisition = (po.encounterTrackable.prerequisites[i] as AdvancedDungeonPrerequisite).customFlagToCheck;
                                    }
                                }
                                if (saveFlagToSetOnAcquisition != CustomDungeonFlags.NONE)
                                {
                                    shopItem.item.gameObject.AddComponent<SpecialPickupObject>().CustomSaveFlagToSetOnAcquisition = saveFlagToSetOnAcquisition;
                                }
                            }
                        }
                    }
                }
            }
        }

        public static void MetaSetupHook(Action<MetaShopController> orig, MetaShopController meta)
        {
            orig(meta);
            List<ShopItemController> shopItems = (List<ShopItemController>)ItemControllersInfo.GetValue(meta);
            if (shopItems != null)
            {
                foreach (ShopItemController shopItem in shopItems)
                {
                    if (shopItem != null && shopItem.item != null && shopItem.item.encounterTrackable != null && shopItem.item.encounterTrackable.journalData != null)
                    {
                        PickupObject po = GetBlueprintUnlockedItem(shopItem.item.encounterTrackable);
                        if (po != null && po.encounterTrackable != null && po.encounterTrackable.prerequisites != null)
                        {
                            CustomDungeonFlags saveFlagToSetOnAcquisition = GetFlagFromTargetItem(po.PickupObjectId);
                            if (saveFlagToSetOnAcquisition != CustomDungeonFlags.NONE)
                            {
                                shopItem.item.gameObject.AddComponent<SpecialPickupObject>().CustomSaveFlagToSetOnAcquisition = saveFlagToSetOnAcquisition;
                                if (AdvancedGameStatsManager.Instance.GetFlag(saveFlagToSetOnAcquisition))
                                {
                                    shopItem.ForceOutOfStock();
                                }
                            }
                        }
                    }
                }
            }
        }

        public static CustomDungeonFlags GetFlagFromTargetItem(int shopItemId)
        {
            CustomDungeonFlags result = CustomDungeonFlags.NONE;
            PickupObject byId = PickupObjectDatabase.GetById(shopItemId);
            for (int i = 0; i < byId.encounterTrackable.prerequisites.Length; i++)
            {
                if (byId.encounterTrackable.prerequisites[i] is AdvancedDungeonPrerequisite && (byId.encounterTrackable.prerequisites[i] as AdvancedDungeonPrerequisite).advancedPrerequisiteType ==
                    AdvancedDungeonPrerequisite.AdvancedPrerequisiteType.CUSTOM_FLAG)
                {
                    result = (byId.encounterTrackable.prerequisites[i] as AdvancedDungeonPrerequisite).customFlagToCheck;
                }
            }
            return result;
        }

        public static PickupObject GetBlueprintUnlockedItem(EncounterTrackable blueprintTrackable)
        {
            for (int i = 0; i < PickupObjectDatabase.Instance.Objects.Count; i++)
            {
                PickupObject pickupObject = PickupObjectDatabase.Instance.Objects[i];
                if (pickupObject)
                {
                    EncounterTrackable encounterTrackable = pickupObject.encounterTrackable;
                    if (encounterTrackable)
                    {
                        string itemkey = encounterTrackable.journalData.PrimaryDisplayName;
                        if (itemkey.Equals(blueprintTrackable.journalData.PrimaryDisplayName, StringComparison.OrdinalIgnoreCase))
                        {
                            string itemkey2 = encounterTrackable.journalData.NotificationPanelDescription;
                            if (itemkey2.Equals(blueprintTrackable.journalData.NotificationPanelDescription, StringComparison.OrdinalIgnoreCase))
                            {
                                string itemkey3 = encounterTrackable.journalData.AmmonomiconFullEntry;
                                if (itemkey3.Equals(blueprintTrackable.journalData.AmmonomiconFullEntry, StringComparison.OrdinalIgnoreCase))
                                {
                                    string sprite = encounterTrackable.journalData.AmmonomiconSprite;
                                    if (sprite.Equals(blueprintTrackable.journalData.AmmonomiconSprite, StringComparison.OrdinalIgnoreCase))
                                    {
                                        return pickupObject;
                                    }
                                }
                            }
                        }
                    }
                }
            }
            return null;
        }

        public static void AddItemToTrorcMetaShop(this PickupObject po, int cost, int? index = null)
        {
            WeightedGameObject wgo = new WeightedGameObject
            {
                rawGameObject = null,
                pickupId = po.PickupObjectId,
                weight = cost,
                forceDuplicatesPossible = false,
                additionalPrerequisites = new DungeonPrerequisite[0]
            };
            if (index == null)
            {
                TrorcMetaShopItems.defaultItemDrops.elements.Add(wgo);
            }
            else
            {
                if (index.Value < 0)
                {
                    TrorcMetaShopItems.defaultItemDrops.elements.Add(wgo);
                }
                else
                {
                    TrorcMetaShopItems.defaultItemDrops.elements.Insert(index.Value, wgo);
                }
            }
        }

        public static void AddItemToGooptonMetaShop(this PickupObject po, int cost, int? index = null)
        {
            WeightedGameObject wgo = new WeightedGameObject
            {
                rawGameObject = null,
                pickupId = po.PickupObjectId,
                weight = cost,
                forceDuplicatesPossible = false,
                additionalPrerequisites = new DungeonPrerequisite[0]
            };
            if (index == null)
            {
                GooptonMetaShopItems.defaultItemDrops.elements.Add(wgo);
            }
            else
            {
                if (index.Value < 0)
                {
                    TrorcMetaShopItems.defaultItemDrops.elements.Add(wgo);
                }
                else
                {
                    GooptonMetaShopItems.defaultItemDrops.elements.Insert(index.Value, wgo);
                }
            }
        }

        public static void AddItemToDougMetaShop(this PickupObject po, int cost, int? index = null)
        {
            WeightedGameObject wgo = new WeightedGameObject
            {
                rawGameObject = null,
                pickupId = po.PickupObjectId,
                weight = cost,
                forceDuplicatesPossible = false,
                additionalPrerequisites = new DungeonPrerequisite[0]
            };
            if (index == null)
            {
                DougMetaShopItems.defaultItemDrops.elements.Add(wgo);
            }
            else
            {
                if (index.Value < 0)
                {
                    DougMetaShopItems.defaultItemDrops.elements.Add(wgo);
                }
                else
                {
                    DougMetaShopItems.defaultItemDrops.elements.Insert(index.Value, wgo);
                }
            }
        }

        public static void AddBaseMetaShopDoubleTier(int topLeftItemId, int topLeftItemPrice, int topMiddleItemId, int topMiddleItemPrice, int topRightItemId, int topRightItemPrice, int bottomLeftItemId, int bottomLeftItemPrice,
            int bottomMiddleItemId, int bottomMiddleItemPrice, int bottomRightItemId, int bottomRightItemPrice, int? index = null)
        {
            AddBaseMetaShopDoubleTier(new DoubleMetaShopTier(new MetaShopTier()
            {
                itemId1 = topLeftItemId,
                overrideItem1Cost = topLeftItemPrice,
                itemId2 = topMiddleItemId,
                overrideItem2Cost = topMiddleItemPrice,
                itemId3 = topRightItemId,
                overrideItem3Cost = topRightItemPrice,
                overrideTierCost =
                topLeftItemId
            }, new MetaShopTier
            {
                itemId1 = bottomLeftItemId,
                overrideItem1Cost = bottomLeftItemPrice,
                itemId2 = bottomMiddleItemId,
                overrideItem2Cost = bottomMiddleItemPrice,
                itemId3 = bottomRightItemId,
                overrideItem3Cost = bottomRightItemPrice,
                overrideTierCost =
                topLeftItemId
            }), index);
        }

        public static void AddBaseMetaShopDoubleTier(int topLeftItemId, int topLeftItemPrice, int topMiddleItemId, int topMiddleItemPrice, int topRightItemId, int topRightItemPrice, int bottomLeftItemId, int bottomLeftItemPrice,
            int bottomMiddleItemId, int bottomMiddleItemPrice, int? index = null)
        {
            AddBaseMetaShopDoubleTier(new DoubleMetaShopTier(new MetaShopTier()
            {
                itemId1 = topLeftItemId,
                overrideItem1Cost = topLeftItemPrice,
                itemId2 = topMiddleItemId,
                overrideItem2Cost = topMiddleItemPrice,
                itemId3 = topRightItemId,
                overrideItem3Cost = topRightItemPrice,
                overrideTierCost =
                topLeftItemId
            }, new MetaShopTier
            {
                itemId1 = bottomLeftItemId,
                overrideItem1Cost = bottomLeftItemPrice,
                itemId2 = bottomMiddleItemId,
                overrideItem2Cost = bottomMiddleItemPrice,
                itemId3 = -1,
                overrideItem3Cost = -1,
                overrideTierCost =
                topLeftItemId
            }), index);
        }

        public static void AddBaseMetaShopDoubleTier(int topLeftItemId, int topLeftItemPrice, int topMiddleItemId, int topMiddleItemPrice, int topRightItemId, int topRightItemPrice, int bottomLeftItemId, int bottomLeftItemPrice, int? index = null)
        {
            AddBaseMetaShopDoubleTier(new DoubleMetaShopTier(new MetaShopTier()
            {
                itemId1 = topLeftItemId,
                overrideItem1Cost = topLeftItemPrice,
                itemId2 = topMiddleItemId,
                overrideItem2Cost = topMiddleItemPrice,
                itemId3 = topRightItemId,
                overrideItem3Cost = topRightItemPrice,
                overrideTierCost =
                topLeftItemId
            }, new MetaShopTier
            {
                itemId1 = bottomLeftItemId,
                overrideItem1Cost = bottomLeftItemPrice,
                itemId2 = -1,
                overrideItem2Cost = -1,
                itemId3 = -1,
                overrideItem3Cost = -1,
                overrideTierCost =
                topLeftItemId
            }), index);
        }

        public static void AddBaseMetaShopTier(int leftItemId, int leftItemPrice, int middleItemId, int middleItemPrice, int rightItemId, int rightItemPrice, int? index = null)
        {
            AddBaseMetaShopTier(new MetaShopTier()
            {
                itemId1 = leftItemId,
                overrideItem1Cost = leftItemPrice,
                itemId2 = middleItemId,
                overrideItem2Cost = middleItemPrice,
                itemId3 = rightItemId,
                overrideItem3Cost = rightItemPrice,
                overrideTierCost =
                leftItemPrice
            }, index);
        }

        public static void AddBaseMetaShopTier(int leftItemId, int leftItemPrice, int middleItemId, int middleItemPrice, int? index = null)
        {
            AddBaseMetaShopTier(new MetaShopTier()
            {
                itemId1 = leftItemId,
                overrideItem1Cost = leftItemPrice,
                itemId2 = middleItemId,
                overrideItem2Cost = middleItemPrice,
                itemId3 = -1,
                overrideItem3Cost = -1,
                overrideTierCost =
                leftItemPrice
            }, index);
        }

        public static void AddBaseMetaShopTier(int leftItemId, int leftItemPrice, int? index = null)
        {
            AddBaseMetaShopTier(new MetaShopTier()
            {
                itemId1 = leftItemId,
                overrideItem1Cost = leftItemPrice,
                itemId2 = -1,
                overrideItem2Cost = -1,
                itemId3 = -1,
                overrideItem3Cost = -1,
                overrideTierCost =
                leftItemPrice
            }, index);
        }

        public static void AddBaseMetaShopDoubleTier(DoubleMetaShopTier tier, int? index = null)
        {
            AddBaseMetaShopTier(tier.GetBottomTier(), index);
            AddBaseMetaShopTier(tier.GetTopTier(), index);
        }

        public static void AddBaseMetaShopTier(MetaShopTier tier, int? index = null)
        {
            if (index == null)
            {
                BaseMetaShopController.metaShopTiers.Add(tier);
            }
            else
            {
                if (index.Value < 0)
                {
                    BaseMetaShopController.metaShopTiers.Add(tier);
                }
                else
                {
                    BaseMetaShopController.metaShopTiers.Insert(index.Value, tier);
                }
            }
        }

        public static MetaShopController BaseMetaShopController;
        public static GenericLootTable TrorcMetaShopItems;
        public static GenericLootTable GooptonMetaShopItems;
        public static GenericLootTable DougMetaShopItems;
        private static FieldInfo ItemControllersInfo = typeof(ShopController).GetField("m_itemControllers", BindingFlags.NonPublic | BindingFlags.Instance);
        private static FieldInfo BaseItemControllersInfo = typeof(BaseShopController).GetField("m_itemControllers", BindingFlags.NonPublic | BindingFlags.Instance);

        public class DoubleMetaShopTier
        {
            public DoubleMetaShopTier(MetaShopTier topTier, MetaShopTier bottomTier)
            {
                this.m_topTier = topTier;
                this.m_bottomTier = bottomTier;
            }

            public DoubleMetaShopTier(DoubleMetaShopTier other)
            {
                this.m_topTier = other.m_topTier;
                this.m_bottomTier = other.m_bottomTier;
            }

            public MetaShopTier GetTopTier()
            {
                return this.m_topTier;
            }

            public MetaShopTier GetBottomTier()
            {
                return this.m_topTier;
            }

            public List<MetaShopTier> GetTierList()
            {
                return new List<MetaShopTier>
                {
                    this.m_topTier,
                    this.m_bottomTier
                };
            }

            private MetaShopTier m_topTier;
            private MetaShopTier m_bottomTier;
        }
    }

    public class SpecialPickupObject : MonoBehaviour
    {
        public CustomDungeonFlags CustomSaveFlagToSetOnAcquisition;
    }
}