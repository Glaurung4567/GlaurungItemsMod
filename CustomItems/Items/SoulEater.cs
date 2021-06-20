using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Gungeon;
using ItemAPI;
using UnityEngine;

namespace GlaurungItems.Items
{
    class SoulEater : AdvancedGunBehavior
    {
        public static int nonBossSoulsCollected = 0;
        public static int BossSoulsCollected = 0;
        public static int Add()
        {
            // Get yourself a new gun "base" first.
            // Let's just call it "Basic Gun", and use "jpxfrd" for all sprites and as "codename" All sprites must begin with the same word as the codename. For example, your firing sprite would be named "jpxfrd_fire_001".
            Gun gun = ETGMod.Databases.Items.NewGun("Soul Eater", "souleater");
            // "kp:basic_gun determines how you spawn in your gun through the console. You can change this command to whatever you want, as long as it follows the "name:itemname" template.
            Game.Items.Rename("outdated_gun_mods:soul_eater", "gl:soul_eater");
            gun.gameObject.AddComponent<SoulEater>();
            //These two lines determines the description of your gun, ".SetShortDescription" being the description that appears when you pick up the gun and ".SetLongDescription" being the description in the Ammonomicon entry. 
            gun.SetShortDescription("[TODO]");
            gun.SetLongDescription("[TODO]");
            // This is required, unless you want to use the sprites of the base gun.
            // That, by default, is the pea shooter.
            // SetupSprite sets up the default gun sprite for the ammonomicon and the "gun get" popup.
            // WARNING: Add a copy of your default sprite to Ammonomicon Encounter Icon Collection!
            // That means, "sprites/Ammonomicon Encounter Icon Collection/defaultsprite.png" in your mod .zip. You can see an example of this with inside the mod folder.
            gun.SetupSprite(null, "pewhand_idle_001", 8);
            // ETGMod automatically checks which animations are available.
            // The numbers next to "shootAnimation" determine the animation fps. You can also tweak the animation fps of the reload animation and idle animation using this method.
            gun.SetAnimationFPS(gun.shootAnimation, 24);
            // Every modded gun has base projectile it works with that is borrowed from other guns in the game. 
            // The gun names are the names from the JSON dump! While most are the same, some guns named completely different things. If you need help finding gun names, ask a modder on the Gungeon discord.
            gun.AddProjectileModuleFrom("ak-47", true, false);
            // Here we just take the default projectile module and change its settings how we want it to be.
            gun.DefaultModule.ammoCost = 1;
            gun.DefaultModule.shootStyle = ProjectileModule.ShootStyle.SemiAutomatic;
            gun.DefaultModule.sequenceStyle = ProjectileModule.ProjectileSequenceStyle.Random;
            gun.reloadTime = 1.1f;
            gun.DefaultModule.cooldownTime = 0.1f;
            gun.DefaultModule.numberOfShotsInClip = 8;
            gun.SetBaseMaxAmmo(250);
            // Here we just set the quality of the gun and the "EncounterGuid", which is used by Gungeon to identify the gun.
            gun.quality = PickupObject.ItemQuality.B;
            //gun.encounterTrackable.EncounterGuid = "fucking weeb gun";
            //This block of code helps clone our projectile. Basically it makes it so things like Shadow Clone and Hip Holster keep the stats/sprite of your custom gun's projectiles.
            Projectile projectile = UnityEngine.Object.Instantiate<Projectile>(gun.DefaultModule.projectiles[0]);
            projectile.gameObject.SetActive(false);
            FakePrefab.MarkAsFakePrefab(projectile.gameObject);
            UnityEngine.Object.DontDestroyOnLoad(projectile);
            gun.DefaultModule.projectiles[0] = projectile;
            //projectile.baseData allows you to modify the base properties of your projectile module.
            //In our case, our gun uses modified projectiles from the ak-47.
            //You can modify a good number of stats but for now, let's just modify the damage and speed.
            projectile.baseData.damage = 7f;
            projectile.baseData.speed = 20f;
            projectile.transform.parent = gun.barrelOffset;
            //This determines what sprite you want your projectile to use. Note this isn't necessary if you don't want to have a custom projectile sprite.
            //The x and y values determine the size of your custom projectile
            //ProjectileSlashingBehaviour slashingBehaviour = new ProjectileSlashingBehaviour();
            //slashingBehaviour.

            // projectile.SetProjectileSpriteRight("build_projectile", 1, 1, null, null);
            ETGMod.Databases.Items.Add(gun, null, "ANY");

            SoulEaterID = gun.PickupObjectId;
            pickup = gun;
            return gun.PickupObjectId;
        }
        private static PickupObject pickup;
        public static int SoulEaterID;
        public override void OnPostFired(PlayerController player, Gun gun)
        {
            //This determines what sound you want to play when you fire a gun.
            //Sounds names are based on the Gungeon sound dump, which can be found at EnterTheGungeon/Etg_Data/StreamingAssets/Audio/GeneratedSoundBanks/Windows/sfx.txt
            gun.PreventNormalFireAudio = true;
            AkSoundEngine.PostEvent("Play_WPN_smileyrevolver_shot_01", gameObject);
        }

        private bool HasReloaded;
        private bool hasUpgraded = false;
        //This block of code allows us to change the reload sounds.

        protected override void Update()
        {
            if (nonBossSoulsCollected >= 99 && BossSoulsCollected >= 1 && hasUpgraded == false)
            {
                PlayerController p = GameManager.Instance.PrimaryPlayer;
                p.GiveItem("hotg:soul_eater_upgrade");
                p.inventory.RemoveGunFromInventory(pickup as Gun);
                hasUpgraded = true;
            }
            if (gun.CurrentOwner)
            {

                if (!gun.PreventNormalFireAudio)
                {
                    this.gun.PreventNormalFireAudio = true;
                }
                if (!gun.IsReloading && !HasReloaded)
                {
                    this.HasReloaded = true;
                }
            }
        }

        public override void PostProcessProjectile(Projectile proj)
        {
            proj.OnHitEnemy += this.OnKilledEnemy;

        }

        private void OnKilledEnemy(Projectile proj, SpeculativeRigidbody enemy, bool fatal)
        {
            if (fatal)
            {
                if (!enemy.healthHaver.IsBoss)
                    LootEngine.SpawnItem(ETGMod.Databases.Items["Mob Soul"].gameObject, enemy.specRigidbody.UnitCenter, Vector2.zero, 1f, false, true, false);
                else
                    LootEngine.SpawnItem(ETGMod.Databases.Items["Boss Soul"].gameObject, enemy.specRigidbody.UnitCenter, Vector2.zero, 1f, false, true, false);
            }
        }


        //Now add the Tools class to your project.
        //All that's left now is sprite stuff. 
        //Your sprites should be organized, like how you see in the mod folder. 
        //Every gun requires that you have a .json to match the sprites or else the gun won't spawn at all
        //.Json determines the hand sprites for your character. You can make a gun two handed by having both "SecondaryHand" and "PrimaryHand" in the .json file, which can be edited through Notepad or Visual Studios
        //By default this gun is a one-handed weapon
        //If you need a basic two handed .json. Just use the jpxfrd2.json.
        //And finally, don't forget to add your Gun to your ETGModule class!
    }


    class SoulEaterEvolution : AdvancedGunBehavior
    {
        public static void Add()
        {
            // Get yourself a new gun "base" first.
            // Let's just call it "Basic Gun", and use "jpxfrd" for all sprites and as "codename" All sprites must begin with the same word as the codename. For example, your firing sprite would be named "jpxfrd_fire_001".
            Gun gun = ETGMod.Databases.Items.NewGun("Soul Eater Upgrade", "souleaterup");
            // "kp:basic_gun determines how you spawn in your gun through the console. You can change this command to whatever you want, as long as it follows the "name:itemname" template.
            Game.Items.Rename("outdated_gun_mods:soul_eater_upgrade", "gl:soul_eater_upgrade");
            gun.SetName("Soul Eater");
            gun.gameObject.AddComponent<SoulEaterEvolution>();
            //These two lines determines the description of your gun, ".SetShortDescription" being the description that appears when you pick up the gun and ".SetLongDescription" being the description in the Ammonomicon entry. 
            gun.SetShortDescription("[TODO]");
            gun.SetLongDescription("[TODO]");
            // This is required, unless you want to use the sprites of the base gun.
            // That, by default, is the pea shooter.
            // SetupSprite sets up the default gun sprite for the ammonomicon and the "gun get" popup.
            // WARNING: Add a copy of your default sprite to Ammonomicon Encounter Icon Collection!
            // That means, "sprites/Ammonomicon Encounter Icon Collection/defaultsprite.png" in your mod .zip. You can see an example of this with inside the mod folder.
            gun.SetupSprite(null, "pewhand_idle_001", 8);
            // ETGMod automatically checks which animations are available.
            // The numbers next to "shootAnimation" determine the animation fps. You can also tweak the animation fps of the reload animation and idle animation using this method.
            gun.SetAnimationFPS(gun.shootAnimation, 24);
            // Every modded gun has base projectile it works with that is borrowed from other guns in the game. 
            // The gun names are the names from the JSON dump! While most are the same, some guns named completely different things. If you need help finding gun names, ask a modder on the Gungeon discord.
            gun.AddProjectileModuleFrom("ak-47", true, false);
            // Here we just take the default projectile module and change its settings how we want it to be.
            gun.DefaultModule.ammoCost = 1;
            gun.DefaultModule.shootStyle = ProjectileModule.ShootStyle.SemiAutomatic;
            gun.DefaultModule.sequenceStyle = ProjectileModule.ProjectileSequenceStyle.Random;
            gun.reloadTime = 1.1f;
            gun.DefaultModule.cooldownTime = 0.1f;
            gun.DefaultModule.numberOfShotsInClip = 10;
            gun.SetBaseMaxAmmo(300);
            // Here we just set the quality of the gun and the "EncounterGuid", which is used by Gungeon to identify the gun.
            gun.quality = PickupObject.ItemQuality.SPECIAL;
            //gun.encounterTrackable.EncounterGuid = "fucking weeb gun v2";
            //This block of code helps clone our projectile. Basically it makes it so things like Shadow Clone and Hip Holster keep the stats/sprite of your custom gun's projectiles.
            Projectile projectile = UnityEngine.Object.Instantiate<Projectile>(gun.DefaultModule.projectiles[0]);
            projectile.gameObject.SetActive(false);
            FakePrefab.MarkAsFakePrefab(projectile.gameObject);
            UnityEngine.Object.DontDestroyOnLoad(projectile);
            gun.DefaultModule.projectiles[0] = projectile;
            //projectile.baseData allows you to modify the base properties of your projectile module.
            //In our case, our gun uses modified projectiles from the ak-47.
            //You can modify a good number of stats but for now, let's just modify the damage and speed.
            projectile.baseData.damage = 8f;
            projectile.baseData.speed = 20f;
            projectile.transform.parent = gun.barrelOffset;
            //This determines what sprite you want your projectile to use. Note this isn't necessary if you don't want to have a custom projectile sprite.
            //The x and y values determine the size of your custom projectile
            VFXPool VFXSlash = VFXLibrary.CreateMuzzleflash("royal_guard's_knife_slash", new List<string> { "royal_guard's_knife_slash_001", "royal_guard's_knife_slash_002", "royal_guard's_knife_slash_003", "royal_guard's_knife_slash_004" }, 10, new List<IntVector2> { new IntVector2(72, 67), new IntVector2(72, 67), new IntVector2(72, 67), new IntVector2(72, 67) }, new List<tk2dBaseSprite.Anchor> {
                tk2dBaseSprite.Anchor.MiddleLeft, tk2dBaseSprite.Anchor.MiddleLeft, tk2dBaseSprite.Anchor.MiddleLeft, tk2dBaseSprite.Anchor.MiddleLeft}, 
                new List<Vector2> { Vector2.zero, Vector2.zero, Vector2.zero, Vector2.zero  }, false, false, false, false, 0, VFXAlignment.Fixed, true, new List<float> { 0, 0, 0, 0 }, new List<Color> { VFXLibrary.emptyColor, VFXLibrary.emptyColor, VFXLibrary.emptyColor, VFXLibrary.emptyColor });
            slash = VFXSlash;

            // projectile.SetProjectileSpriteRight("build_projectile", 1, 1, null, null);
            ETGMod.Databases.Items.Add(gun, null, "ANY");
        }
        private static VFXPool slash;
        public override void OnPostFired(PlayerController player, Gun gun)
        {
            //This determines what sound you want to play when you fire a gun.
            //Sounds names are based on the Gungeon sound dump, which can be found at EnterTheGungeon/Etg_Data/StreamingAssets/Audio/GeneratedSoundBanks/Windows/sfx.txt
            gun.PreventNormalFireAudio = true;
            AkSoundEngine.PostEvent("Play_WPN_smileyrevolver_shot_01", gameObject);
        }

        private bool HasReloaded;
        //This block of code allows us to change the reload sounds.

        protected override void Update()
        {
            if (gun.CurrentOwner)
            {

                if (!gun.PreventNormalFireAudio)
                {
                    this.gun.PreventNormalFireAudio = true;
                }
                if (!gun.IsReloading && !HasReloaded)
                {
                    this.HasReloaded = true;
                }
            }
        }
        public override void OnReloadPressed(PlayerController player, Gun gun, bool bSOMETHING)
        {

            if (gun.IsReloading && this.HasReloaded)
            {

                HasReloaded = false;
                AkSoundEngine.PostEvent("Stop_WPN_All", base.gameObject);
                base.OnReloadPressed(player, gun, bSOMETHING);
                AkSoundEngine.PostEvent("Play_WPN_sword_impact_01", base.gameObject);
                // SlashDoer.DoSwordSlash(player.transform.position, base.gun.CurrentAngle, player, 0f, SlashDoer.ProjInteractMode.REFLECT, 12f, 5f, null, null, 1, 1, 6f, 55f);              

            }

        }

    }

    class NonBossSoul : PassiveItem
    {
        public static void Init()
        {
            string itemName = "Mob Soul";
            string resourceName = "GlaurungItems/Resources/NonBossSoul";
            GameObject obj = new GameObject(itemName);
            var item = obj.AddComponent<NonBossSoul>();
            ItemBuilder.AddSpriteToObject(itemName, resourceName, obj);
            string shortDesc = "one step closer";
            string longDesc = "If you are seeing this i did something wrong, lemme know.";
            ItemBuilder.SetupItem(item, shortDesc, longDesc, "gl");
            item.quality = PickupObject.ItemQuality.EXCLUDED;
        }

        public override void Pickup(PlayerController player)
        {
            base.Pickup(player);

            if (player.HasPickupID(SoulEater.SoulEaterID))
            {
                SoulEater.nonBossSoulsCollected++;
                player.RemovePassiveItem(this.PickupObjectId);
            }

        }
    }

    class BossSoul : PassiveItem
    {
        public static void Init()
        {
            string itemName = "Boss Soul";
            string resourceName = "GlaurungItems/Resources/BossSoul";
            GameObject obj = new GameObject(itemName);
            var item = obj.AddComponent<BossSoul>();
            ItemBuilder.AddSpriteToObject(itemName, resourceName, obj);
            string shortDesc = "your power grows";
            string longDesc = "If you are seeing this i did something wrong, lemme know.";
            ItemBuilder.SetupItem(item, shortDesc, longDesc, "gl");
            item.quality = PickupObject.ItemQuality.EXCLUDED;
        }

        public override void Pickup(PlayerController player)
        {
            base.Pickup(player);

            if (player.HasPickupID(SoulEater.SoulEaterID))
            {
                SoulEater.BossSoulsCollected++;
                player.RemovePassiveItem(this.PickupObjectId);
            }

        }
    }
}
