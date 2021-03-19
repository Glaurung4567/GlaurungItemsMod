using ItemAPI;

namespace GlaurungItems.Items
{
    class AddModifGungeonItems
    {
        public static void Init()
        {
            BuffGuns();
            AddItemsToLootPool();
            (PickupObjectDatabase.GetById(647) as Gun).gameObject.GetOrAddComponent<CustomChamberGunFormeSynergyProcessor>();
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

    public class CustomChamberGunFormeSynergyProcessor : CustomGunFormeSynergyProcessor
    {
        protected override void Awake()
        {
            base.Awake();
            string synergyName = "Secrets of the Otter's Parry";

            CustomGunFormeData form0 = new CustomGunFormeData();
            form0.RequiresSynergy = true;
            form0.RequiredSynergy = synergyName;
            form0.FormeID = 647;//keep

            CustomGunFormeData form1 = new CustomGunFormeData();
            form1.RequiresSynergy = true;
            form1.RequiredSynergy = synergyName;
            form1.FormeID = 657;//oubliette

            CustomGunFormeData form2 = new CustomGunFormeData();
            form2.RequiresSynergy = true;
            form2.RequiredSynergy = synergyName;
            form2.FormeID = 660;//proper

            CustomGunFormeData form3 = new CustomGunFormeData();
            form3.RequiresSynergy = true;
            form3.RequiredSynergy = synergyName;
            form3.FormeID = 806;//abbey

            CustomGunFormeData form4 = new CustomGunFormeData();
            form4.RequiresSynergy = true;
            form4.RequiredSynergy = synergyName;
            form4.FormeID = 807;//mine

            CustomGunFormeData form5 = new CustomGunFormeData();
            form5.RequiresSynergy = true;
            form5.RequiredSynergy = synergyName;
            form5.FormeID = 808;//rat

            CustomGunFormeData form6 = new CustomGunFormeData();
            form6.RequiresSynergy = true;
            form6.RequiredSynergy = synergyName;
            form6.FormeID = 659;//hollow

            CustomGunFormeData form7 = new CustomGunFormeData();
            form7.RequiresSynergy = true;
            form7.RequiredSynergy = synergyName;
            form7.FormeID = 823;//r&g

            CustomGunFormeData form8 = new CustomGunFormeData();
            form8.RequiresSynergy = true;
            form8.RequiredSynergy = synergyName;
            form8.FormeID = 658;//forge

            CustomGunFormeData form9 = new CustomGunFormeData();
            form9.RequiresSynergy = true;
            form9.RequiredSynergy = synergyName;
            form9.FormeID = 763;//hell

            this.Formes = new CustomGunFormeData[] { form0, form1, form2, form3, form4, form5, form6, form7, form8 };
        }
    }
}
