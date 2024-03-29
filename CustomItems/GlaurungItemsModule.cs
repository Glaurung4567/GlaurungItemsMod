﻿using System;
using System.Linq;
using EnemyAPI;
using GlaurungItems.Items;
using Gungeon;
using ItemAPI;
using SaveAPI;
using CustomDungeonFlags = SaveAPI.CustomDungeonFlags;

namespace GlaurungItems
{
    public class GlaurungItems : ETGModule
    {
        public bool setup;
        private static string version = "1.8.0";
        public static AdvancedStringDB Strings;

        public override void Init()
        {
            SaveAPIManager.Setup("GlaurungItems");
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
                //Toolbox.Init();
                ItemBuilder.Init();
                Hooks.Init();
                EnemyAPITools.Init();
                SpecialBlankModificationItem.InitHooks();
                EasyGoopDefinitions.DefineDefaultGoops();

                ZipFilePath = this.Metadata.Archive;
                FilePath = this.Metadata.Directory;
                AudioResourceLoader.InitAudio();


                GlaurungItems.Strings.Enemies.Set("#LOW_PRIEST", "Low Priest");


                ShamblesGun.Add();
                Neuralyzer.Init();
                HowlOfTheJammed.Init();
                Chainer.Add();
                GunjuringEncyclopedia.Add();
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

                BlinkbackDevice.Init();
                OzzieHelm.Init();
                AmmoletOfWonder.Init();
                SappingBullets.Init();

                IndecisiveGun.Add();
                CreateTrap.Init();
                MineCrafter.Init();

                NuArcana.Init();
                MatriochkAmmolet.Init();
                TurtlezBeam.Add();
                PeacemakerCarbine.Add();
                SwissArmyRifle.Add();

                PrismaticField.Init();
                TurncoatRounds.Init();
                Linc.Add();
                GunzerkingPotion.Init();
                Poyo.Init();

                Transistor.Add();
                Turn.Init();

                HandCannon.Add();
                InfiltratorRounds.Init();

                ShokkGun.Add();
                MindBlown.Init();
                GravDartGun.Add();

                TopsyGun.Add();
                TopsyTurvyBomb.Init();
                NonBossSoul.Init();
                BossSoul.Init();
                SoulEaterEvolution.Add();
                SoulEater.Add();

                //PortalGunnyTest.Add();
                //GunDeadArmyStronghold.Init();

                //modder tools
                RoomTeller.Init();

                //my own items modif based on cel's modif of the gilded hydra
                AddModifGungeonItems.Init();

                Game.Items["gl:peacemaker_carbine"].SetupUnlockOnCustomFlag(CustomDungeonFlags.GLAURUNG_PEACEMAKER_FLAG, true);
                Game.Items["gl:peacemaker_carbine"].AddItemToTrorcMetaShop(10);
                //SpecialFoyerShops.AddBaseMetaShopTier(ETGMod.Databases.Items["Chainer"].PickupObjectId, 10, ETGMod.Databases.Items["Shambles"].PickupObjectId, 25, ETGMod.Databases.Items["Yoink"].PickupObjectId, 75);

                // synergies 
                GameManager.Instance.SynergyManager.synergies = GameManager.Instance.SynergyManager.synergies.Concat(new AdvancedSynergyEntry[]
                {
                    new SynergyHub.KlobbeCogSynergy()
                }).ToArray<AdvancedSynergyEntry>();
                /*GameManager.Instance.SynergyManager.synergies = GameManager.Instance.SynergyManager.synergies.Concat(new AdvancedSynergyEntry[]
                {
                    new SynergyHub.RaiseDeadGhostSynergy()
                }).ToArray<AdvancedSynergyEntry>();
                GameManager.Instance.SynergyManager.synergies = GameManager.Instance.SynergyManager.synergies.Concat(new AdvancedSynergyEntry[]
                {
                    new SynergyHub.RaiseDeadSkusketSynergy()
                }).ToArray<AdvancedSynergyEntry>();*/
                GameManager.Instance.SynergyManager.synergies = GameManager.Instance.SynergyManager.synergies.Concat(new AdvancedSynergyEntry[]
                {
                    new SynergyHub.BulletScriptGunSynergy1()
                }).ToArray<AdvancedSynergyEntry>();
                GameManager.Instance.SynergyManager.synergies = GameManager.Instance.SynergyManager.synergies.Concat(new AdvancedSynergyEntry[]
                {
                    new SynergyHub.ShokkSynergy()
                }).ToArray<AdvancedSynergyEntry>();
                /*GameManager.Instance.SynergyManager.synergies = GameManager.Instance.SynergyManager.synergies.Concat(new AdvancedSynergyEntry[]
                {
                    new SynergyHub.TurncoatRoundsSynergy()
                }).ToArray<AdvancedSynergyEntry>();*/

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


                GameManager.Instance.SynergyManager.synergies = GameManager.Instance.SynergyManager.synergies.Concat(new AdvancedSynergyEntry[]
                {
                    new SynergyHub.ChamberHiddenSynergy()
                }).ToArray<AdvancedSynergyEntry>();

                setup = true;
            }
            catch (Exception e)
            {
                Tools.PrintException(e);
            }
            ETGModConsole.Log($"Glaurung Items Pack {version} Initialized");
        }



        public override void Exit()
        {
        }

        public static string ZipFilePath;
        public static string FilePath;
    }
}
