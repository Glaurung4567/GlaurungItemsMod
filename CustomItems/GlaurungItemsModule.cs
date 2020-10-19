﻿using System;
using System.Linq;
using EnemyAPI;
using GlaurungItems.Items;
using ItemAPI;

namespace GlaurungItems
{
    public class GlaurungItems : ETGModule
    {
        public bool setup;
        private static string version = "0.0.1";
        public static AdvancedStringDB Strings;

        public override void Init()
        {
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
                ItemBuilder.Init();
                Hooks.Init();
                EnemyAPITools.Init();

                GlaurungItems.Strings.Enemies.Set("#LOW_PRIEST", "Low Priest");

                ShamblesGun.Add();
                Neuralyzer.Init();
                HowlOfTheJammed.Init();
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

                //my own items modif based on cel's modif of the gilded hydra
                AddModifGungeonItems.Init();
                
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
                    new SynergyHub.ShamblesSynergy()
                }).ToArray<AdvancedSynergyEntry>();
                AdvancedDualWieldSynergyProcessor advancedDualWieldSynergyProcessor = Toolbox.GetGunById(520).gameObject.AddComponent<AdvancedDualWieldSynergyProcessor>();
                advancedDualWieldSynergyProcessor.SynergyNameToCheck = "Landing Hate";
                advancedDualWieldSynergyProcessor.PartnerGunID = ETGMod.Databases.Items["shambles"].PickupObjectId;

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
    }
}
