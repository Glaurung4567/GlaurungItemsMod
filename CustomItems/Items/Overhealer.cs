using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using ItemAPI;
using Random = UnityEngine.Random;
using Gungeon;

namespace GlaurungItems.Items
{
    internal class Overhealer: AdvancedGunBehavior
    {
        public static void Add()
        {
            Gun gun = ETGMod.Databases.Items.NewGun("OverHealer", "overhealer");
            Game.Items.Rename("outdated_gun_mods:overhealer", "gl:overhealer");
            gun.gameObject.AddComponent<Overhealer>();
            gun.SetShortDescription("Counter-Intuitive");
            gun.SetLongDescription("This was created with the intent to heal allies, but a malfunction made it kill the target instead if he was fully healed due to positive energy overdose. \n \n" +
                "A sadistic medic brought it to the Gungeon to kheal friends and foes alike.");
            gun.SetupSprite(null, "overhealer_idle_001", 8);
            gun.SetAnimationFPS(gun.shootAnimation, 24);
            gun.SetAnimationFPS(gun.reloadAnimation, 12);
            //gun.SetAnimationFPS(gun.idleAnimation, 8);
            gun.AddProjectileModuleFrom("klobb", true, false);

            gun.DefaultModule.ammoCost = 1;
            gun.DefaultModule.angleVariance = 7f;
            gun.DefaultModule.shootStyle = ProjectileModule.ShootStyle.Automatic;
            gun.DefaultModule.sequenceStyle = ProjectileModule.ProjectileSequenceStyle.Random;
            gun.reloadTime = 1.5f;
            gun.DefaultModule.cooldownTime = 0.1f;
            gun.DefaultModule.numberOfShotsInClip = 30;
            //gun.usesContinuousFireAnimation = true;
            gun.SetBaseMaxAmmo(400);
            gun.gunClass = GunClass.FULLAUTO;
            gun.DefaultModule.burstShotCount = 1;
            gun.muzzleFlashEffects = (PickupObjectDatabase.GetById(81) as Gun).muzzleFlashEffects;

            gun.quality = PickupObject.ItemQuality.B;
            //gun.encounterTrackable.EncounterGuid = "Incredible healer lay on hands channel energy healer's hands signature skill heal skill focus heal";

            Projectile projectile = UnityEngine.Object.Instantiate<Projectile>(gun.DefaultModule.projectiles[0]);
            projectile.gameObject.SetActive(false);
            FakePrefab.MarkAsFakePrefab(projectile.gameObject);
            UnityEngine.Object.DontDestroyOnLoad(projectile);
            gun.DefaultModule.projectiles[0] = projectile;

            projectile.baseData.damage = 0f;
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
            base.PostProcessProjectile(projectile);
        }

        public void OnProjectileHitEnemy(Projectile proj, SpeculativeRigidbody enemy, bool fatal)
        {
            if (enemy != null)
            {
                AIActor aiActor = enemy.aiActor;
                if (aiActor != null && this.gun && this.gun.CurrentOwner)
                {
                    if (aiActor.healthHaver.IsAlive && !aiActor.healthHaver.IsBoss && aiActor.healthHaver.GetCurrentHealthPercentage()<0.75 
                        && !this.targetForOverhealKill.Contains(aiActor))
                    {
                        //Tools.Print(aiActor.healthHaver.GetCurrentHealthPercentage(), "FFFFFF", true);
                        this.targetForOverhealKill.Add(aiActor);
                        aiActor.SetOverrideOutlineColor(Color.green);
                    }

                    if(aiActor.healthHaver.IsAlive && aiActor.healthHaver.GetCurrentHealthPercentage() < 1)
                    {
                        aiActor.healthHaver.ApplyHealing(7f);
                        if(aiActor.healthHaver.GetCurrentHealthPercentage() == 1 && this.targetForOverhealKill.Contains(aiActor))
                        {
                            Instantiate<GameObject>(Overhealer.TeleporterPrototypeTelefragVFX, aiActor.sprite.WorldCenter, Quaternion.identity);
                            aiActor.healthHaver.ApplyDamage(10000f, Vector2.zero, "OverHealed !", CoreDamageTypes.Void, 0, true, null, true);
                            //Tools.Print("Killed", "FFFFFF", true);
                        }else if (aiActor.healthHaver.IsBoss && aiActor.healthHaver.GetCurrentHealthPercentage() == 1)
                        {
                            aiActor.healthHaver.ApplyDamage(100f, Vector2.zero, "OverHealed !", CoreDamageTypes.Void, 0, true, null, true);
                        }
                    }
                }
            }
        }

        protected override void OnPickup(PlayerController player)
        {
            base.OnPickup(player);
            this.targetForOverhealKill = new List<AIActor>();
            //player.GunChanged += this.OnGunChanged;
            player.OnRoomClearEvent += this.OnLeaveCombat;
        }

        protected override void OnPostDrop(PlayerController user)
        {
            user.OnRoomClearEvent -= this.OnLeaveCombat;
            //user.GunChanged -= this.OnGunChanged;
            this.targetForOverhealKill = new List<AIActor>();
            base.OnPostDrop(user);
        }

        private void OnLeaveCombat(PlayerController user)
        {
            if (user != null)
            {
                this.targetForOverhealKill = new List<AIActor>();
            }
        }

        // boilerplate stuff
        //This block of code allows us to change the reload sounds.
        public override void OnPostFired(PlayerController player, Gun gun)
        {
            //This determines what sound you want to play when you fire a gun.
            //Sounds names are based on the Gungeon sound dump, which can be found at EnterTheGungeon/Etg_Data/StreamingAssets/Audio/GeneratedSoundBanks/Windows/sfx.txt
            gun.PreventNormalFireAudio = true;
            AkSoundEngine.PostEvent("Play_OBJ_heart_heal_01", gameObject);
        }

        protected override void Update()
        {
            base.Update();
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
                AkSoundEngine.PostEvent("Play_WPN_SAA_reload_01", base.gameObject);
            }
        }

        private bool HasReloaded;
        private List<AIActor> targetForOverhealKill = new List<AIActor>();
        private static GameObject TeleporterPrototypeTelefragVFX = PickupObjectDatabase.GetById(449).GetComponent<TeleporterPrototypeItem>().TelefragVFXPrefab.gameObject;
    }
}
