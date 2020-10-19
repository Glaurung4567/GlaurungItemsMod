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
            Gun gun1 = PickupObjectDatabase.GetById(368) as Gun;
            gun1.quality = PickupObject.ItemQuality.D;
            gun1.SetBaseMaxAmmo(300);
            gun1.ammo = 300;
            Gun gun2 = PickupObjectDatabase.GetById(227) as Gun;
            gun2.quality = PickupObject.ItemQuality.C;
            PassiveItem item1 = PickupObjectDatabase.GetById(473) as PassiveItem; // hidden compartment
            item1.quality = PickupObject.ItemQuality.C;
        }
    }
}
