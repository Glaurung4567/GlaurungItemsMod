using ItemAPI;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace GlaurungItems.Items
{
    class AstralCounterweight : PassiveItem
    {

        public static void Init()
        {
            string itemName = "Astral Counterweight";
            string resourceName = "GlaurungItems/Resources/acme_crate";
            GameObject obj = new GameObject(itemName);
            AstralCounterweight greandeParasite = obj.AddComponent<AstralCounterweight>();
            ItemBuilder.AddSpriteToObject(itemName, resourceName, obj);
            string shortDesc = "Equillibrium";
            string longDesc = "Those unaffected by the great leveller shall now be raised in kind to hold their own.";
            greandeParasite.SetupItem(shortDesc, longDesc, "bny");
            greandeParasite.quality = PickupObject.ItemQuality.EXCLUDED;
        }

        protected override void Update()
        {
            PlayerController player = base.Owner;
            if (player != null && !cooldown)
            {
                List<PlayerStats.StatType> statTypes = new List<PlayerStats.StatType>(stats.Keys);
                if (this.previousStats.Values.Sum() == -8)//at init
                {
                    foreach (PlayerStats.StatType stat in statTypes)
                    {
                        this.previousStats[stat] = player.stats.GetStatValue(stat);
                        this.stats[stat] = player.stats.GetStatValue(stat);
                    }
                }
                else
                {
                    foreach (PlayerStats.StatType stat in statTypes)
                    {
                        this.stats[stat] = player.stats.GetStatValue(stat);
                    }

                    foreach (PlayerStats.StatType stat in statTypes)
                    {
                        if (this.previousStats[stat] != this.stats[stat])
                        {
                            statModified = stat;
                            amountModified = this.stats[stat] - this.previousStats[stat];
                            cooldown = true;
                            base.StartCoroutine(this.RecalculateStats());
                            break;
                        }
                    }
                    if (!cooldown)
                    {
                        foreach (PlayerStats.StatType stat in statTypes)
                        {
                            this.previousStats[stat] = this.stats[stat];
                        }
                    }
                }
            }
        }

        private IEnumerator RecalculateStats()
        {
            List<PlayerStats.StatType> statTypes = new List<PlayerStats.StatType>(stats.Keys);

            this.AddPassiveStatModifier(statModified, -amountModified);
            foreach (PlayerStats.StatType stat in statTypes)
            {
                this.AddPassiveStatModifier(stat, amountModified/8);
            }
            base.Owner.stats.RecalculateStats(base.Owner, true);
            //do balance
            yield return new WaitForSeconds(0.2f);
            foreach (PlayerStats.StatType stat in statTypes)
            {
                this.previousStats[stat] = -1;
                this.stats[stat] = -1;
            }
            cooldown = false;
            yield break;
        }

        [SerializeField]
        private bool cooldown = false;

        [SerializeField]
        private PlayerStats.StatType statModified = PlayerStats.StatType.MovementSpeed;

        [SerializeField]
        private float amountModified = 0f;



        [SerializeField]
        private Dictionary<PlayerStats.StatType, float> previousStats { get; set; } = new Dictionary<PlayerStats.StatType, float>
        {
            {
                PlayerStats.StatType.MovementSpeed,
                -1
            },
            {
                PlayerStats.StatType.RateOfFire,
                -1
            },
            {
                PlayerStats.StatType.Coolness,
                -1
            },
            {
                PlayerStats.StatType.Damage,
                -1
            },
            {
                PlayerStats.StatType.ProjectileSpeed,
                -1
            },
            {
                PlayerStats.StatType.ReloadSpeed,
                -1
            },
            {
                PlayerStats.StatType.Curse,
                -1
            },
            {
                PlayerStats.StatType.DamageToBosses,
                -1
            },
        };

        [SerializeField]
        public Dictionary<PlayerStats.StatType, float> stats { get; set; } = new Dictionary<PlayerStats.StatType, float>
        {
            {
                PlayerStats.StatType.MovementSpeed,
                -1
            },
            {
                PlayerStats.StatType.RateOfFire,
                -1
            },
            {
                PlayerStats.StatType.Coolness,
                -1
            },
            {
                PlayerStats.StatType.Damage,
                -1
            },
            {
                PlayerStats.StatType.ProjectileSpeed,
                -1
            },
            {
                PlayerStats.StatType.ReloadSpeed,
                -1
            },
            {
                PlayerStats.StatType.Curse,
                -1
            },
            {
                PlayerStats.StatType.DamageToBosses,
                -1
            },
        };

        //dico with all stats if u need it
        public Dictionary<PlayerStats.StatType, float> fullStats { get; set; } = new Dictionary<PlayerStats.StatType, float>
        {
            {
                PlayerStats.StatType.MovementSpeed,
                -1
            },
            {
                PlayerStats.StatType.RateOfFire,
                -1
            },
            {
                PlayerStats.StatType.Accuracy,
                -1
            },
            {
                PlayerStats.StatType.Health,
                -1
            },
            {
                PlayerStats.StatType.Coolness,
                -1
            },
            {
                PlayerStats.StatType.Damage,
                -1
            },
            {
                PlayerStats.StatType.ProjectileSpeed,
                -1
            },
            {
                PlayerStats.StatType.AdditionalGunCapacity,
                -1
            },
            {
                PlayerStats.StatType.AdditionalItemCapacity,
                -1
            },
            {
                PlayerStats.StatType.AmmoCapacityMultiplier,
                -1
            },
            {
                PlayerStats.StatType.ReloadSpeed,
                -1
            },
            {
                PlayerStats.StatType.AdditionalShotPiercing,
                -1
            },
            {
                PlayerStats.StatType.KnockbackMultiplier,
                -1
            },
            {
                PlayerStats.StatType.GlobalPriceMultiplier,
                -1
            },
            {
                PlayerStats.StatType.Curse,
                -1
            },
            {
                PlayerStats.StatType.PlayerBulletScale,
                -1
            },
            {
                PlayerStats.StatType.AdditionalClipCapacityMultiplier,
                -1
            },
            {
                PlayerStats.StatType.AdditionalShotBounces,
                -1
            },
            {
                PlayerStats.StatType.AdditionalBlanksPerFloor,
                -1
            },
            {
                PlayerStats.StatType.ShadowBulletChance,
                -1
            },
            {
                PlayerStats.StatType.ThrownGunDamage,
                -1
            },
            {
                PlayerStats.StatType.DodgeRollDamage,
                -1
            },
            {
                PlayerStats.StatType.DamageToBosses,
                -1
            },
            {
                PlayerStats.StatType.EnemyProjectileSpeedMultiplier,
                -1
            },
            {
                PlayerStats.StatType.ExtremeShadowBulletChance,
                -1
            },
            {
                PlayerStats.StatType.ChargeAmountMultiplier,
                -1
            },
            {
                PlayerStats.StatType.RangeMultiplier,
                -1
            },
            {
                PlayerStats.StatType.DodgeRollDistanceMultiplier,
                -1
            },
            {
                PlayerStats.StatType.DodgeRollSpeedMultiplier,
                -1
            },
            {
                PlayerStats.StatType.TarnisherClipCapacityMultiplier,
                -1
            },
            {
                PlayerStats.StatType.MoneyMultiplierFromEnemies,
                -1
            },
        };
    }
}
