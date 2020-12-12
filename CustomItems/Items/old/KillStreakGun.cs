using Gungeon;
using ItemAPI;
using System;
using System.Timers;

namespace GlaurungItems.Items
{
    class KillStreakGun : AdvancedGunBehavior
    {
        public static void Add()
        {
            Gun gun = ETGMod.Databases.Items.NewGun("Kill Streak", "jpxfrd");
            Game.Items.Rename("outdated_gun_mods:kill_streak", "gl:kill_streak");
            gun.gameObject.AddComponent<KillStreakGun>();
            gun.SetShortDescription("WIP");
            gun.SetLongDescription("WIP");
            gun.SetupSprite(null, "jpxfrd_idle_001", 8);
            gun.SetAnimationFPS(gun.shootAnimation, 24);
            gun.SetAnimationFPS(gun.reloadAnimation, 12);
            //gun.SetAnimationFPS(gun.idleAnimation, 8);
            gun.AddProjectileModuleFrom("klobb", true, false);

            gun.DefaultModule.ammoCost = 1;
            gun.DefaultModule.angleVariance = 0f;
            gun.DefaultModule.shootStyle = ProjectileModule.ShootStyle.SemiAutomatic;
            gun.DefaultModule.sequenceStyle = ProjectileModule.ProjectileSequenceStyle.Random;
            gun.reloadTime = 1.5f;
            gun.DefaultModule.cooldownTime = 0.2f;
            gun.DefaultModule.numberOfShotsInClip = 30;
            gun.SetBaseMaxAmmo(400);

            gun.quality = PickupObject.ItemQuality.EXCLUDED;

            Projectile projectile = UnityEngine.Object.Instantiate<Projectile>(gun.DefaultModule.projectiles[0]);
            projectile.gameObject.SetActive(false);
            FakePrefab.MarkAsFakePrefab(projectile.gameObject);
            UnityEngine.Object.DontDestroyOnLoad(projectile);
            gun.DefaultModule.projectiles[0] = projectile;

            projectile.baseData.damage *= 4f;
            projectile.baseData.speed *= 2.8f;
            projectile.baseData.force *= 1f;
            projectile.baseData.range *= 3f;
            projectile.transform.parent = gun.barrelOffset;
            //projectile.SetProjectileSpriteRight("build_projectile", 5, 5);

            ETGMod.Databases.Items.Add(gun, null, "ANY");
        }

        public override void PostProcessProjectile(Projectile projectile)
        {
            projectile.OnHitEnemy = (Action<Projectile, SpeculativeRigidbody, bool>)Delegate.Combine(projectile.OnHitEnemy, new Action<Projectile, SpeculativeRigidbody, bool>(this.OnProjectileHitEnemy));
        }

        public void OnProjectileHitEnemy(Projectile proj, SpeculativeRigidbody enemy, bool fatal)
        {
            if (enemy != null)
            {
                AIActor aiActor = enemy.aiActor;
                if (aiActor != null && this.gun && this.gun.CurrentOwner && kills <= 0 && fatal)
                {
                    killStreakTimer = new System.Timers.Timer(timerInterval); // launch an event each 0.25 seconds (1000 == 1 second)
                    killStreakTimer.Elapsed += OnTimedEvent;
                    killStreakTimer.Start();
                    numberOfTimeIntervalsBeforeKillStreakEndIterator = maxNumberOfTimeIntervalsBeforeKillStreakEnd;
                    kills++;
                }
                else if (fatal) 
                {
                    numberOfTimeIntervalsBeforeKillStreakEndIterator = maxNumberOfTimeIntervalsBeforeKillStreakEnd;
                    kills++;
                    dfLabel dfLab = new dfLabel();
                    dfLab.Text = $"Kill Streak: {kills}";
                }
                
            }
        }

        private static void OnTimedEvent(Object source, System.Timers.ElapsedEventArgs e)
        {
            if(numberOfTimeIntervalsBeforeKillStreakEndIterator >= 0)
            {
                Tools.Print($"Killstreak {kills}", "FFFFFF", true);
                numberOfTimeIntervalsBeforeKillStreakEndIterator--;
            }
            else
            {
                Tools.Print($"Killstreak ended", "FFFFFF", true);
                kills = 0;
                killStreakTimer.Stop();
            }
        }

        // used to give boosts when the numbers of kills reach a certain number and remove them when the kill streak end
        protected override void Update()
        {
            if(kills <=0 && firstKillStreakBoostActive)
            {
                firstKillStreakBoostActive = false;
                Tools.Print($"Killstreak Remove Boost", "FFFFFF", true);
                
                gun.AddCurrentGunStatModifier(PlayerStats.StatType.Damage, -0.5f, StatModifier.ModifyMethod.ADDITIVE);
                PlayerController owner = (gun.CurrentOwner as PlayerController);
                owner.stats.RecalculateStats(owner, false);
            }
            else if(kills >= 3 && !firstKillStreakBoostActive)
            {
                firstKillStreakBoostActive = true;
                Tools.Print($"Killstreak First Boost", "FFFFFF", true);

                gun.AddCurrentGunStatModifier(PlayerStats.StatType.Damage, 0.5f, StatModifier.ModifyMethod.ADDITIVE);
                PlayerController owner = (gun.CurrentOwner as PlayerController);
                owner.stats.RecalculateStats(owner, false);
            }
            base.Update();
        }

        private static float timerInterval = 250;
        private static int maxNumberOfTimeIntervalsBeforeKillStreakEnd = 28; //
        private static int kills = 0;
        private static int numberOfTimeIntervalsBeforeKillStreakEndIterator = 0;
        private static Timer killStreakTimer;
        private bool firstKillStreakBoostActive = false;
    }
}
