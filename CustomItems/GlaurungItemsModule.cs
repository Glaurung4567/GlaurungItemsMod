using System;
using System.IO;
using System.Linq;
using System.Reflection;
using EnemyAPI;
using GlaurungItems.Items;
using ItemAPI;
using Items;
using MonoMod.RuntimeDetour;
using UnityEngine;

namespace GlaurungItems
{
    public class GlaurungItems : ETGModule
    {
        public bool setup;
        private static string version = "1.3.0";
        public static AdvancedStringDB Strings;

        public override void Init()
        {
            AdvancedGameStatsManager.AdvancedGameSave = new SaveManager.SaveType
            {
                filePattern = "Slot{0}.glaurungSave",
                encrypted = true,
                backupCount = 3,
                backupPattern = "Slot{0}.glaurungBackup.{1}",
                backupMinTimeMin = 45,
                legacyFilePattern = "glaurungGameStatsSlot{0}.txt"
            };
            for (int i = 0; i < 3; i++)
            {
                SaveManager.SaveSlot saveSlot = (SaveManager.SaveSlot)i;
                Toolbox.SafeMove(Path.Combine(SaveManager.OldSavePath, string.Format(AdvancedGameStatsManager.AdvancedGameSave.legacyFilePattern, saveSlot)), Path.Combine(SaveManager.OldSavePath,
                    string.Format(AdvancedGameStatsManager.AdvancedGameSave.filePattern, saveSlot)), false);
                Toolbox.SafeMove(Path.Combine(SaveManager.OldSavePath, string.Format(AdvancedGameStatsManager.AdvancedGameSave.filePattern, saveSlot)), Path.Combine(SaveManager.OldSavePath,
                    string.Format(AdvancedGameStatsManager.AdvancedGameSave.filePattern, saveSlot)), false);
                Toolbox.SafeMove(Toolbox.PathCombine(SaveManager.SavePath, "01", string.Format(AdvancedGameStatsManager.AdvancedGameSave.filePattern, saveSlot)), Path.Combine(SaveManager.SavePath,
                    string.Format(AdvancedGameStatsManager.AdvancedGameSave.filePattern, saveSlot)), true);
            }
            Hook mainMenuAwakeHook = new Hook(
                typeof(MainMenuFoyerController).GetMethod("InitializeMainMenu", BindingFlags.Public | BindingFlags.Instance),
                typeof(GlaurungItems).GetMethod("MainMenuAwakeHook")
            );
            //Toolbox.specialeverything = this.LoadAssetBundleFromLiterallyAnywhere();
            AdvancedGameStatsManager.Init();
        }

        public override void Start()
        {
            Setup();
        }

        public void Setup()
        {
            if (setup) return;
            try
            {
                GlaurungItems.Strings = new AdvancedStringDB();
                Tools.Init();
                Toolbox.Init();
                ItemBuilder.Init();
                Hooks.Init();
                EnemyAPITools.Init();
                SpecialFoyerShops.DoSetup();
                SpecialBlankModificationItem.InitHooks();
                EasyGoopDefinitions.DefineDefaultGoops();


                GlaurungItems.Strings.Enemies.Set("#LOW_PRIEST", "Low Priest");

                ShamblesGun.Add();
                Neuralyzer.Init();
                HowlOfTheJammed.Init();
                Chainer.Add();
                RaiseDead.Init();
                Overhealer.Add();
                HighPriestSecretMap.Init();
                JosephsLastResort.Init();
                KillStreakBullets.Init();
                BerserkerRoar.Init();
                HitBoxGlasses.Init();
                LooseCannon.Add();
                BanishingBullets.Init();
                Yoink.Init();

                BulletScriptGun.Add();
                BlinkbackDevice.Init();
                OzzieHelm.Init();
                AmmoletOfWonder.Init();
                SappingBullets.Init();

                IndecisiveGun.Add();
                CreateTrap.Init();
                MineCrafter.Init();
                BeamTest.Add();

                AstralCounterweight.Init();
                //GunDeadArmyStronghold.Init();

                //my own items modif based on cel's modif of the gilded hydra
                AddModifGungeonItems.Init();

                SpecialFoyerShops.AddBaseMetaShopTier(ETGMod.Databases.Items["Chainer"].PickupObjectId, 10, ETGMod.Databases.Items["Shambles"].PickupObjectId, 25, ETGMod.Databases.Items["Yoink"].PickupObjectId, 75);

                // synergies 
                GameManager.Instance.SynergyManager.synergies = GameManager.Instance.SynergyManager.synergies.Concat(new AdvancedSynergyEntry[]
                {
                    new SynergyHub.KlobbeCogSynergy()
                }).ToArray<AdvancedSynergyEntry>();
                GameManager.Instance.SynergyManager.synergies = GameManager.Instance.SynergyManager.synergies.Concat(new AdvancedSynergyEntry[]
                {
                    new SynergyHub.RaiseDeadGhostSynergy()
                }).ToArray<AdvancedSynergyEntry>();
                GameManager.Instance.SynergyManager.synergies = GameManager.Instance.SynergyManager.synergies.Concat(new AdvancedSynergyEntry[]
                {
                    new SynergyHub.RaiseDeadSkusketSynergy()
                }).ToArray<AdvancedSynergyEntry>();
                GameManager.Instance.SynergyManager.synergies = GameManager.Instance.SynergyManager.synergies.Concat(new AdvancedSynergyEntry[]
                {
                    new SynergyHub.BulletScriptGunSynergy1()
                }).ToArray<AdvancedSynergyEntry>();

                GameManager.Instance.SynergyManager.synergies = GameManager.Instance.SynergyManager.synergies.Concat(new AdvancedSynergyEntry[]
                {
                    new SynergyHub.ShamblesSynergy()
                }).ToArray<AdvancedSynergyEntry>();
                AdvancedDualWieldSynergyProcessor advancedDualWieldSynergyProcessor = Toolbox.GetGunById(520).gameObject.AddComponent<AdvancedDualWieldSynergyProcessor>();
                advancedDualWieldSynergyProcessor.SynergyNameToCheck = "Landing Hate";
                advancedDualWieldSynergyProcessor.PartnerGunID = ETGMod.Databases.Items["shambles"].PickupObjectId;

                GameManager.Instance.SynergyManager.synergies = GameManager.Instance.SynergyManager.synergies.Concat(new AdvancedSynergyEntry[]
                {
                    new SynergyHub.SSTGunSynergy()
                }).ToArray<AdvancedSynergyEntry>();
                AdvancedTransformGunSynergyProcessor synergyProcessor = (PickupObjectDatabase.GetById(299) as Gun).gameObject.AddComponent<AdvancedTransformGunSynergyProcessor>();
                synergyProcessor.NonSynergyGunId = 299;
                synergyProcessor.SynergyGunId = 747;
                synergyProcessor.SynergyToCheck = "Big Exploding Pew Pew !";

                setup = true;
            }
            catch (Exception e)
            {
                Tools.PrintException(e);
            }

            ETGModConsole.Log($"Glaurung Items Pack {version} Initialized");
        }

        // Token: 0x060001F5 RID: 501 RVA: 0x000140F4 File Offset: 0x000122F4
        

        public override void Exit()
        {
        }

        public static void MainMenuAwakeHook(Action<MainMenuFoyerController> orig, MainMenuFoyerController self)
        {
            orig(self);
            self.VersionLabel.Text = self.VersionLabel.Text + " | " + Version;
        }

        public static string Version = "v0.0";
    }
}
