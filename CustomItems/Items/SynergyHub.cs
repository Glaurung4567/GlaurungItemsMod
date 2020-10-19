using System.Collections.Generic;

namespace GlaurungItems.Items
{
    public static class SynergyHub
    {
        public class KlobbeCogSynergy : AdvancedSynergyEntry
        {
            // Token: 0x0600045D RID: 1117 RVA: 0x0002C42C File Offset: 0x0002A62C
            public KlobbeCogSynergy()
            {
                this.NameKey = "Not so hidden anymore";
                this.MandatoryItemIDs = new List<int>
                {
                    135,
                    31
                };
                this.IgnoreLichEyeBullets = true;
                this.statModifiers = new List<StatModifier>(0);
                this.bonusSynergies = new List<CustomSynergyType>();
            }
        }

        public class ShamblesSynergy : AdvancedSynergyEntry
        {
            // Token: 0x0600022C RID: 556 RVA: 0x00015230 File Offset: 0x00013430
            public ShamblesSynergy()
            {
                this.NameKey = "Landing Hate";
                this.MandatoryItemIDs = new List<int>
                {
                    ETGMod.Databases.Items["shambles"].PickupObjectId,
                    520
                };
                this.IgnoreLichEyeBullets = true;
                this.statModifiers = new List<StatModifier>(0);
                this.bonusSynergies = new List<CustomSynergyType>();
            }
        }

        public class RaiseDeadGhostSynergy : AdvancedSynergyEntry
        {
            // Token: 0x0600022C RID: 556 RVA: 0x00015230 File Offset: 0x00013430
            public RaiseDeadGhostSynergy()
            {
                this.NameKey = "Restless Spirit";
                this.MandatoryItemIDs = new List<int>
                {
                    PickupObjectDatabase.GetByEncounterName("Raise Dead").PickupObjectId
                };
                this.OptionalGunIDs = new List<int>
                {
                    198
                };
                this.OptionalItemIDs = new List<int>
                {
                    172
                };
                this.IgnoreLichEyeBullets = true;
                this.statModifiers = new List<StatModifier>(0);
                this.bonusSynergies = new List<CustomSynergyType>();
            }
        }
        
        public class RaiseDeadSkusketSynergy : AdvancedSynergyEntry
        {
            // Token: 0x0600022C RID: 556 RVA: 0x00015230 File Offset: 0x00013430
            public RaiseDeadSkusketSynergy()
            {
                this.NameKey = "Skull Bros";
                this.MandatoryItemIDs = new List<int>
                {
                    PickupObjectDatabase.GetByEncounterName("Raise Dead").PickupObjectId,
                    45
                };
                this.IgnoreLichEyeBullets = true;
                this.statModifiers = new List<StatModifier>(0);
                this.bonusSynergies = new List<CustomSynergyType>();
            }
        }
    }
}