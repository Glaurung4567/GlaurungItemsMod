using ItemAPI;
using System;
using UnityEngine;
using EnemyAPI;

namespace GlaurungItems.Items
{
    class AddModifGungeonItems
    {
        public static void Init()
        {
            BuffGuns();
            AddItemsToLootPool();
        }
        public static void BuffGuns()
        {
            /*Gun gun = PickupObjectDatabase.GetById(231) as Gun; // gilded hydra
            gun.SetBaseMaxAmmo(120);
            gun.reloadTime = 1.6f;
            gun.ammo = 120;*/
        }

        public static void AddItemsToLootPool()
        {
            Gun gun1 = PickupObjectDatabase.GetById(368) as Gun; //el_tigre
            gun1.quality = PickupObject.ItemQuality.D;
            gun1.SetBaseMaxAmmo(300);
            gun1.ammo = 300;

            PickupObject gun2 = PickupObjectDatabase.GetById(227); //wrist_bow
            gun2.quality = PickupObject.ItemQuality.C;

            PickupObject gun3 = PickupObjectDatabase.GetById(299);
            gun3.quality = PickupObject.ItemQuality.D;
            
            Gun gun4 = PickupObjectDatabase.GetById(747) as Gun; //high_dragunfire+unknown
            gun4.quality = PickupObject.ItemQuality.EXCLUDED;
            gun4.DefaultModule.projectiles[0].baseData.damage *= 0.3f;

            PickupObject item1 = PickupObjectDatabase.GetById(473); // hidden compartment
            item1.quality = PickupObject.ItemQuality.C;

            //PlayerItem item2 = PickupObjectDatabase.GetById(168) as PlayerItem; // double vision
            //item2.OnPickedUp = (Action<PlayerController>)Delegate.Combine(item2.OnPickedUp, new Action<PlayerController>(AddModifGungeonItems.Pickup));
        }

        //fail
        /*
        public static void Pickup(PlayerController player)
        {
            PlayerItem item2 = PickupObjectDatabase.GetById(168) as PlayerItem; // double vision
            item2.OnActivationStatusChanged = (Action<PlayerItem>)Delegate.Combine(item2.OnActivationStatusChanged, new Action<PlayerItem>(AddModifGungeonItems.DrunkEffect));
        }

        public static void DrunkEffect(PlayerItem item)
        {
            if (item.IsActive)
            {
                Tools.Print("ouech", "ffffff", true);
                Pixelator.Instance.RegisterAdditionalRenderPass(GonnerMat);
                if (GameManager.Instance.PrimaryPlayer != null)
                {
                    GameManager.Instance.PrimaryPlayer.SetOverrideShader(GonnerMat.shader);
                }
                if (GameManager.Instance.SecondaryPlayer != null)
                {
                    GameManager.Instance.SecondaryPlayer.SetOverrideShader(GonnerMat.shader);
                }
            }
            else
            {
                Pixelator.Instance.DeregisterAdditionalRenderPass(GonnerMat);

            }
        }

        public static Material GonnerMat = new Material((PickupObjectDatabase.GetById(602) as Gun).sprite.renderer.material.shader);
        */
    }
}
