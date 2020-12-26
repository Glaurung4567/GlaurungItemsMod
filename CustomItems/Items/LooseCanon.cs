using Gungeon;
using ItemAPI;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace GlaurungItems.Items
{
    class LooseCannon: GunBehaviour
    {

        public static void Add()
        {
            Gun gun = ETGMod.Databases.Items.NewGun("Loose Cannon", "loosecannon");
            Game.Items.Rename("outdated_gun_mods:loose_cannon", "gl:loose_cannon");
            gun.gameObject.AddComponent<LooseCannon>();
            gun.SetShortDescription("Capricious");
            gun.SetLongDescription("This cannon was used by a drunk Gungeonner who was the only one able to master it and use it effectively.\n \nAre you up to the challenge?");
            gun.SetupSprite(null, "loosecannon_idle_001", 8);
            gun.SetAnimationFPS(gun.shootAnimation, 12);
            gun.SetAnimationFPS(gun.reloadAnimation, 12);
            gun.SetAnimationFPS(gun.chargeAnimation, 3);
            
            gun.AddProjectileModuleFrom(PickupObjectDatabase.GetById(332) as Gun, true, false);
            gun.DefaultModule.ammoCost = 0;
            gun.DefaultModule.angleVariance = 7f;
            gun.DefaultModule.shootStyle = ProjectileModule.ShootStyle.Charged;
            gun.DefaultModule.sequenceStyle = ProjectileModule.ProjectileSequenceStyle.Random;
            gun.reloadTime = 1.5f;
            gun.DefaultModule.triggerCooldownForAnyChargeAmount = true;
            gun.DefaultModule.preventFiringDuringCharge = true;
            gun.DefaultModule.cooldownTime = 1.5f;
            gun.DefaultModule.numberOfShotsInClip = 1;
            gun.SetBaseMaxAmmo(75);
            gun.gunClass = GunClass.CHARGE;
            gun.muzzleFlashEffects = (PickupObjectDatabase.GetById(37) as Gun).muzzleFlashEffects;

            gun.quality = PickupObject.ItemQuality.B;

            Projectile projectile = UnityEngine.Object.Instantiate<Projectile>(gun.DefaultModule.projectiles[0]);
            projectile.gameObject.SetActive(false);
            FakePrefab.MarkAsFakePrefab(projectile.gameObject);
            UnityEngine.Object.DontDestroyOnLoad(projectile);
            gun.DefaultModule.projectiles[0] = projectile;

            projectile.baseData.damage = 0f;
            projectile.baseData.range = 0f;
            //projectile.GetComponent<ExplosiveModifier>().explosionData.effect = null;

            ProjectileModule.ChargeProjectile chargeProj = new ProjectileModule.ChargeProjectile
            {
                Projectile = projectile,
                ChargeTime = 2f,
            };
            gun.DefaultModule.chargeProjectiles = new List<ProjectileModule.ChargeProjectile> { chargeProj };

            //projectile.SetProjectileSpriteRight("build_projectile", 5, 5);

            ETGMod.Databases.Items.Add(gun, null, "ANY");
        }

        public override void OnFinishAttack(PlayerController player, Gun gun)
        {
            //Tools.Print(chargeFraction, "ffffff", true);
            if(gun.CurrentAmmo > 0 && chargeFraction > 0 && chargeFraction < 1) {
                if (chargeFraction >= 0.1 && chargeFraction < 0.3)
                {
                    Projectile component = this.GetGrenadeProjectile(player);
                    if (component != null)
                    {
                        ExplosiveModifier component2 = component.GetComponent<ExplosiveModifier>();
                        var rollBomb = PickupObjectDatabase.GetById(567).GetComponent<FireVolleyOnRollItem>();//roll bomb
                        ProjectileModule projectileModule = rollBomb.ModVolley.projectiles[0];
                        Projectile currentProjectile = projectileModule.GetCurrentProjectile();
                        ExplosionData miniExplosionData = currentProjectile.GetComponent<ExplosiveModifier>().explosionData.CopyExplosionData();                        
                        miniExplosionData.damage = 5f;
                        miniExplosionData.damageRadius = 1.5f;
                        miniExplosionData.doForce = false;
                        component2.explosionData = miniExplosionData;
                        component.baseData.damage *= 0.3f;
                        component.Owner = player;

                        component.Shooter = player.specRigidbody;
                        component.transform.parent = gun.barrelOffset;
                    }
                    this.GunConsumeAmmo();
                }
                else if (chargeFraction >= 0.3 && chargeFraction < 0.6)
                {
                    Projectile component = this.GetGrenadeProjectile(player);
                    if (component != null)
                    {
                        ExplosiveModifier component2 = component.GetComponent<ExplosiveModifier>();
                        ExplosionData defaultSafeSmallExplosionData = GameManager.Instance.Dungeon.sharedSettingsPrefab.DefaultSmallExplosionData.CopyExplosionData();
                        this.playerSafeSmallExplosion.effect = defaultSafeSmallExplosionData.effect;
                        this.playerSafeSmallExplosion.ignoreList = defaultSafeSmallExplosionData.ignoreList;
                        this.playerSafeSmallExplosion.ss = defaultSafeSmallExplosionData.ss;
                        component2.explosionData = playerSafeSmallExplosion;
                        component.baseData.damage *= 0.5f;
                        component.Owner = player;
                        component.Shooter = player.specRigidbody;
                        component.transform.parent = gun.barrelOffset;
                    }
                    this.GunConsumeAmmo();
                }
                else if (chargeFraction >= 0.6 && chargeFraction < 0.95)
                {
                    Projectile component = this.GetGrenadeProjectile(player);
                    if (component != null)
                    {
                        ExplosiveModifier component2 = component.GetComponent<ExplosiveModifier>();
                        component2.explosionData.damage *= 1.1f; 
                        component2.explosionData.doDestroyProjectiles = true; 
                        component.baseData.damage *= 1f;
                        component.Owner = player;
                        component.Shooter = player.specRigidbody;
                        component.transform.parent = gun.barrelOffset;

                    }
                    this.GunConsumeAmmo();
                }
                else if (chargeFraction >= 0.95 && chargeFraction < 1)
                {
                    Projectile component = this.GetGrenadeProjectile(player);
                    if (component != null)
                    {
                        ExplosiveModifier component2 = component.GetComponent<ExplosiveModifier>();
                        component2.explosionData = (PickupObjectDatabase.GetById(443) as TargetedAttackPlayerItem).strikeExplosionData.CopyExplosionData();
                        component2.explosionData.damage *= 0.4f;

                        component.baseData.damage *= 2.5f;
                        component.baseData.range *= 10.75f;
                        component.baseData.speed *= 2.75f;
                        component.specRigidbody.OnCollision = (Action<CollisionData>)Delegate.Combine(component.specRigidbody.OnCollision, new Action<CollisionData>(this.OnCritCollide));                      

                        component.Owner = player;
                        component.Shooter = player.specRigidbody;
                        component.transform.parent = gun.barrelOffset;
                    }
                    this.GunConsumeAmmo();
                }
            }
            base.OnFinishAttack(player, gun);
        }

        protected void Update()
        {
            if (gun.CurrentOwner)
            {
                chargeFraction = gun.GetChargeFraction();
                //This block of code allows us to change the reload sounds.
                if (!gun.PreventNormalFireAudio)
                {
                    this.gun.PreventNormalFireAudio = true;
                }
                if (!gun.IsReloading && !hasReloaded)
                {
                    this.hasReloaded = true;

                }
                if (chargeFraction >= 1 && gun.CurrentAmmo > 0)
                {
                    ExplosionData defaultExplosionData = GameManager.Instance.Dungeon.sharedSettingsPrefab.DefaultExplosionData;
                    this.playerMisfireExplosion.effect = defaultExplosionData.effect;
                    this.playerMisfireExplosion.ignoreList = defaultExplosionData.ignoreList;
                    this.playerMisfireExplosion.ss = defaultExplosionData.ss;

                    Exploder.Explode(gun.CurrentOwner.CenterPosition, playerMisfireExplosion, Vector2.zero, null, true, CoreDamageTypes.None, false);
                    this.GunConsumeAmmo();
                }
            }
        }

        private Projectile GetGrenadeProjectile(PlayerController player)
        {
            Projectile grenade = ((Gun)ETGMod.Databases.Items[19]).DefaultModule.projectiles[0];
            GameObject gameObject = SpawnManager.SpawnProjectile(grenade.gameObject, player.sprite.WorldCenter, Quaternion.Euler(0f, 0f, (player.CurrentGun == null) ? 0f : player.CurrentGun.CurrentAngle), true);
            Projectile projectile = gameObject.GetComponent<Projectile>();
            Destroy(projectile.gameObject.GetComponent<BounceProjModifier>());
            projectile.ChangeColor(0f, Color.grey);
            player.DoPostProcessProjectile(projectile); // to make projectiles affected by the player modifiers (by spapi)
            //Tools.Print(gun.muzzleFlashEffects.effects.Length, "ffffff", true);
            //gun.muzzleFlashEffects.effects[0].SpawnAtPosition(player.AimCenter, (player.CurrentGun == null) ? 0f : player.CurrentGun.CurrentAngle, player.transform);

            return projectile;
        }

        private void GunConsumeAmmo()
        {
            gun.LoseAmmo(1);
            AkSoundEngine.PostEvent("Play_WPN_seriouscannon_shot_01", gameObject);

            //Play_ENM_cannonball_blast_01
        }

        private void OnCritCollide(CollisionData collisionData)
        {
            GameObject gameObjectBlankVFX = (GameObject)ResourceCache.Acquire("Global VFX/BlankVFX");
            GameObject gameObjectBlank = new GameObject("silencer");
            SilencerInstance silencerInstance = gameObjectBlank.AddComponent<SilencerInstance>();
            silencerInstance.TriggerSilencer(collisionData.MyRigidbody.UnitBottomCenter, 50f, 25f, gameObjectBlankVFX, 0.25f, 0.2f, 50f, 10f, 140f, 15f, 0.5f, gun.CurrentOwner as PlayerController, true, false);
        }

        // boilerplate stuff
        public override void OnPostFired(PlayerController player, Gun gun)
        {
            //This determines what sound you want to play when you fire a gun.
            //Sounds names are based on the Gungeon sound dump, which can be found at EnterTheGungeon/Etg_Data/StreamingAssets/Audio/GeneratedSoundBanks/Windows/sfx.txt
            gun.PreventNormalFireAudio = true;
            AkSoundEngine.PostEvent("Play_WPN_seriouscannon_charge_01", gameObject);
        }

        public override void OnReloadPressed(PlayerController player, Gun gun, bool bSOMETHING)
        {
            if (gun.IsReloading && this.hasReloaded)
            {
                hasReloaded = false;
                AkSoundEngine.PostEvent("Stop_WPN_All", base.gameObject);
                base.OnReloadPressed(player, gun, bSOMETHING);
                AkSoundEngine.PostEvent("Play_WPN_SAA_reload_01", base.gameObject);
            }
        }

        private bool hasReloaded;
        private float chargeFraction;
        private ExplosionData playerMisfireExplosion = new ExplosionData
        {
            damageRadius = 4f,
            doDamage = true,
            damageToPlayer = 25f,
            damage = 25f,
            doExplosionRing = true,
            doDestroyProjectiles = true,
            doForce = true,
            debrisForce = 50f,
            preventPlayerForce = true,
            explosionDelay = 0f,
            usesComprehensiveDelay = false,
            doScreenShake = true,
            playDefaultSFX = true,
        };
        private ExplosionData playerSafeSmallExplosion = new ExplosionData
        {
            damageRadius = 3f,
            doDamage = true,
            damageToPlayer = 0f,
            damage = 15f,
            doExplosionRing = true,
            doDestroyProjectiles = true,
            doForce = true,
            debrisForce = 30f,
            preventPlayerForce = true,
            explosionDelay = 0f,
            usesComprehensiveDelay = false,
            doScreenShake = false,
            breakSecretWalls = false,
            playDefaultSFX = true,
        };
    }
}
