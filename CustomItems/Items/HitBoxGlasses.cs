using Dungeonator;
using HutongGames.PlayMaker.Actions;
using ItemAPI;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace GlaurungItems.Items
{
    class HitBoxGlasses: PassiveItem
    {
        public static void Init()
        {
            //The name of the item
            string itemName = "Box Glasses";

            //Refers to an embedded png in the project. Make sure to embed your resources! Google it
            string resourceName = "GlaurungItems/Resources/boxglasses";

            //Create new GameObject
            GameObject obj = new GameObject(itemName);

            //Add a PassiveItem component to the object
            var item = obj.AddComponent<HitBoxGlasses>();

            //Adds a tk2dSprite component to the object and adds your texture to the item sprite collection
            ItemBuilder.AddSpriteToObject(itemName, resourceName, obj);

            //Ammonomicon entry variables
            string shortDesc = "How did that miss ?";
            string longDesc = "Let you see where you must strike your foes to actually hurt them.\n \n" +
                "Theses glasses were created by a mad sorcerer who though that what he was seeing might not be what really was in front of him." +
                "It is unknown what he became after putting them on, nor how they ended up in the Gungeon..."; 

            //Adds the item to the gungeon item list, the ammonomicon, the loot table, etc.
            //Do this after ItemBuilder.AddSpriteToObject!
            ItemBuilder.SetupItem(item, shortDesc, longDesc, "gl");

            //Adds the actual passive effect to the item
            ItemBuilder.AddPassiveStatModifier(item, PlayerStats.StatType.Accuracy, 0.1f, StatModifier.ModifyMethod.ADDITIVE);
            ItemBuilder.AddPassiveStatModifier(item, PlayerStats.StatType.Damage, 0.1f, StatModifier.ModifyMethod.ADDITIVE);
            ItemBuilder.AddPassiveStatModifier(item, PlayerStats.StatType.Curse, 1f, StatModifier.ModifyMethod.ADDITIVE);


            //Set the rarity of the item
            item.quality = PickupObject.ItemQuality.C;
        }


        public override void Pickup(PlayerController player)
        {
            base.Pickup(player);
            ShootBehavior.DrawDebugFiringArea = true;
            player.OnEnteredCombat += OnEnterCombat;
            player.PostProcessProjectile += this.PostProcessProjectile;
            player.PostProcessBeamTick += this.PostProcessBeamTick;
            player.OnRoomClearEvent += this.OnLeaveCombat;
        }

        public override DebrisObject Drop(PlayerController player)
        {
            ShootBehavior.DrawDebugFiringArea = false;
            player.OnRoomClearEvent -= this.OnLeaveCombat;
            player.PostProcessProjectile -= this.PostProcessProjectile;
            player.PostProcessBeamTick -= this.PostProcessBeamTick;
            return base.Drop(player);
        }

        protected override void OnDestroy()
        {
            base.Owner.OnRoomClearEvent -= this.OnLeaveCombat;
            base.Owner.PostProcessBeamTick -= this.PostProcessBeamTick;
            base.Owner.PostProcessProjectile -= this.PostProcessProjectile;
            base.OnDestroy();
        }

        private void OnEnterCombat()
        {
            //Tools.Print("OnEnteredCombat", "FFFFFF", true);
            //RoomHandler.DrawRandomCellLines = true;
            BraveUtility.DrawDebugSquare(base.Owner.CurrentRoom.area.basePosition.ToVector2(), base.Owner.CurrentRoom.area.basePosition.ToVector2() + base.Owner.CurrentRoom.area.dimensions.ToVector2(), Color.cyan, 1000f);
        }

        private void OnLeaveCombat(PlayerController user)
        {
            targetsHitList = new List<AIActor>();
        }

        private void PostProcessProjectile(Projectile projectile, float Chance)
        {
            PlayerController owner = base.Owner;
            projectile.OnHitEnemy = (Action<Projectile, SpeculativeRigidbody, bool>)Delegate.Combine(projectile.OnHitEnemy, new Action<Projectile, SpeculativeRigidbody, bool>(this.OnProjectileHitEnemy));

        }

        private void OnProjectileHitEnemy(Projectile proj, SpeculativeRigidbody enemy, bool fatal)
        {
            if (enemy != null)
            {
                AIActor aiActor = enemy.aiActor;
                PlayerController owner = this.Owner;
                if (!targetsHitList.Contains(aiActor))
                {
                    targetsHitList.Add(aiActor);
                    enemy.ShowHitBox();//DrawDebugFiringArea
                }
            }
        }

        private void PostProcessBeamTick(BeamController beam, SpeculativeRigidbody hitRigidBody, float tickrate)
        {
            AIActor aiactor = hitRigidBody.aiActor;
            if (!aiactor)
            {
                return;
            }
            bool fatal = aiactor.healthHaver && !aiactor.healthHaver.IsDead && !targetsHitList.Contains(aiactor);
            if (fatal)
            {
                targetsHitList.Add(aiactor);
                hitRigidBody.ShowHitBox();//DrawDebugFiringArea
            }
        }

        private List<AIActor> targetsHitList = new List<AIActor>();
    }
}
