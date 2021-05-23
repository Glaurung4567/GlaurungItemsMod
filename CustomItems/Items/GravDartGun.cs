﻿using Gungeon;
using ItemAPI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace GlaurungItems.Items
{
    class GravDartGun : AdvancedGunBehavior
    {
        public static void Add()
        {
            Gun gun = ETGMod.Databases.Items.NewGun("Grav Dart Launcher", "gravd");
            Game.Items.Rename("outdated_gun_mods:grav_dart_launcher", "gl:grav_dart_launcher");
            gun.gameObject.AddComponent<GravDartGun>();
            gun.SetShortDescription("");
            gun.SetLongDescription("");
            gun.SetupSprite(null, "pewhand_idle_001", 8);
            gun.SetAnimationFPS(gun.shootAnimation, 24);
            gun.SetAnimationFPS(gun.reloadAnimation, 12);
            gun.AddProjectileModuleFrom((PickupObjectDatabase.GetById(357) as Gun), true, false);

            gun.quality = PickupObject.ItemQuality.S;

            gun.DefaultModule.ammoCost = 1;
            gun.DefaultModule.angleVariance = 1f;
            gun.gunHandedness = GunHandedness.TwoHanded;
            gun.DefaultModule.shootStyle = ProjectileModule.ShootStyle.SemiAutomatic;
            gun.DefaultModule.sequenceStyle = ProjectileModule.ProjectileSequenceStyle.Random;
            gun.reloadTime = 2f;
            gun.DefaultModule.cooldownTime = 0.2f;
            gun.DefaultModule.numberOfShotsInClip = 15;
            gun.SetBaseMaxAmmo(360);
            gun.muzzleFlashEffects = (PickupObjectDatabase.GetById(81) as Gun).muzzleFlashEffects;


            Projectile projectile = UnityEngine.Object.Instantiate<Projectile>(gun.DefaultModule.projectiles[0]);
            projectile.gameObject.SetActive(false);
            FakePrefab.MarkAsFakePrefab(projectile.gameObject);
            UnityEngine.Object.DontDestroyOnLoad(projectile);
            gun.DefaultModule.projectiles[0] = projectile;

            projectile.baseData.damage = 1;
            projectile.baseData.damage *= 0;
            projectile.baseData.speed *= 1f;
            projectile.baseData.force *= 0f;
            projectile.baseData.range *= 30f;
            projectile.transform.parent = gun.barrelOffset;

            if (projectile.gameObject.GetComponent<HomingModifier>() != null)
            {
                Destroy(projectile.gameObject.GetComponent<HomingModifier>());
            }
            if (projectile.gameObject.GetComponent<DelayedExplosiveBuff>() != null)
            {
                DelayedExplosiveBuff delayedExplosiveBuff = projectile.gameObject.GetComponent<DelayedExplosiveBuff>();
                delayedExplosiveBuff.delayBeforeBurst *= 9999f;
                ExplosionData explosion = delayedExplosiveBuff.explosionData.CopyExplosionData();
                explosion.breakSecretWalls = false;
                explosion.damage = 0f;
                explosion.breakSecretWalls = false;
                explosion.doDestroyProjectiles = false;
                explosion.pushRadius = 0f;
                delayedExplosiveBuff.explosionData = explosion;
            }

            Gun gunFinalProj = (PickupObjectDatabase.GetById(47) as Gun);

            Projectile finalProj = UnityEngine.Object.Instantiate<Projectile>(gunFinalProj.DefaultModule.projectiles[0]);
            finalProj.gameObject.SetActive(false);
            FakePrefab.MarkAsFakePrefab(finalProj.gameObject);
            UnityEngine.Object.DontDestroyOnLoad(finalProj);

            finalProj.baseData.damage = 0;

            gun.DefaultModule.usesOptionalFinalProjectile = true;
            gun.DefaultModule.numberOfFinalProjectiles = 1;
            gun.DefaultModule.finalProjectile = finalProj;
            gun.DefaultModule.finalCustomAmmoType = gunFinalProj.DefaultModule.customAmmoType;
            gun.DefaultModule.finalAmmoType = gunFinalProj.DefaultModule.ammoType;

            ETGMod.Databases.Items.Add(gun, null, "ANY");
        }

        public override void PostProcessProjectile(Projectile projectile)
        {
            base.PostProcessProjectile(projectile);
            if(projectile.name == this.gun.DefaultModule.projectiles[0].name + "(Clone)")
            {
                projectile.OnHitEnemy = (Action<Projectile, SpeculativeRigidbody, bool>)Delegate.Combine(projectile.OnHitEnemy, new Action<Projectile, SpeculativeRigidbody, bool>(this.HandleHitEnemy));
            }
            else if (projectile.name == this.gun.DefaultModule.finalProjectile.name + "(Clone)")
            {
                if (projectile.GetComponent<PierceProjModifier>())
                {
                    UnityEngine.GameObject.Destroy(projectile.GetComponent<PierceProjModifier>());
                }
                if (projectile.GetComponent<BounceProjModifier>())
                {
                    UnityEngine.GameObject.Destroy(projectile.GetComponent<BounceProjModifier>());
                }
                projectile.OnDestruction += Projectile_Tracer_OnDestruction;
            }
        }

        private void HandleHitEnemy(Projectile proj, SpeculativeRigidbody sr, bool fatal)
        {
            if (sr.aiActor && !fatal && !dartedEnemies.Contains(sr.aiActor) && sr.aiActor.healthHaver && sr.aiActor.healthHaver.IsAlive)
            {
                dartedEnemies.Add(sr.aiActor);
                Tools.Print(sr.aiActor.healthHaver.IsBoss, "ffffff", true);
                Tools.Print(sr.aiActor.knockbackDoer == null, "ffffff", true);
            }
        }

        private void Projectile_Tracer_OnDestruction(Projectile proj)
        {
            Vector2 pos = proj.LastPosition;
            if (dartedEnemies != null && pos != null)
            {
                int nbDarted = dartedEnemies.Count;
                for(int i = 0; i < nbDarted; i++)
                {
                    if (dartedEnemies[i] && 
                        dartedEnemies[i].healthHaver && dartedEnemies[i].healthHaver.IsAlive
                        && dartedEnemies[i].specRigidbody
                        && dartedEnemies[i].knockbackDoer)
                    {
                        AIActor aiActor = dartedEnemies[i];
                        if (aiActor.GetComponent<ExplodeOnDeath>())
                        {
                            UnityEngine.Object.Destroy(aiActor.GetComponent<ExplodeOnDeath>());
                        }

                        Vector2 direction = pos - aiActor.CenterPosition;
                        DelayedExplosiveBuff[] dartArray = aiActor.gameObject.GetComponents<DelayedExplosiveBuff>();
                        if(dartArray != null && dartArray.Count() > 0)
                        {
                            int nbDarts = dartArray.Count();
                            aiActor.specRigidbody.AddCollisionLayerOverride(CollisionMask.LayerToMask(CollisionLayer.EnemyHitBox));
                            aiActor.specRigidbody.OnPreRigidbodyCollision = (SpeculativeRigidbody.OnPreRigidbodyCollisionDelegate)Delegate.Combine(aiActor.specRigidbody.OnPreRigidbodyCollision, new SpeculativeRigidbody.OnPreRigidbodyCollisionDelegate(this.HandleHitEnemyHitEnemy));

                            aiActor.knockbackDoer.ApplyKnockback(direction, 800 * nbDarts);

                            for(int j = 0; j < nbDarts; j++)
                            {
                                if(dartArray[j])
                                {
                                    UnityEngine.GameObject.Destroy(dartArray[j]);
                                }
                            }
                        }
                    }
                }
            }

            dartedEnemies = new List<AIActor>();
        }

        private void HandleHitEnemyHitEnemy(SpeculativeRigidbody myRigidbody, PixelCollider myPixelCollider, SpeculativeRigidbody otherRigidbody, PixelCollider otherPixelCollider)
        {

            if (myRigidbody && myRigidbody.aiActor && myRigidbody.aiActor.healthHaver && myRigidbody.aiActor.healthHaver.IsAlive)
            {
                Tools.Print(myRigidbody.aiActor.MovementSpeed, "ffffff", true);
                Tools.Print(myRigidbody.Velocity, "ffffff", true);
                Tools.Print(myRigidbody.Velocity.magnitude, "ffffff", true);
                Tools.Print(myRigidbody.aiActor.KnockbackVelocity, "ffffff", true);
                Tools.Print(myRigidbody.aiActor.KnockbackVelocity.magnitude, "ffffff", true);
                
            }

            myRigidbody.OnPreRigidbodyCollision = (SpeculativeRigidbody.OnPreRigidbodyCollisionDelegate)Delegate.Remove(myRigidbody.OnPreRigidbodyCollision, new SpeculativeRigidbody.OnPreRigidbodyCollisionDelegate(this.HandleHitEnemyHitEnemy));
        }



        // boilerplate stuff
        //This block of code allows us to change the reload sounds.
        public override void OnPostFired(PlayerController player, Gun gun)
        {
            //This determines what sound you want to play when you fire a gun.
            //Sounds names are based on the Gungeon sound dump, which can be found at EnterTheGungeon/Etg_Data/StreamingAssets/Audio/GeneratedSoundBanks/Windows/sfx.txt
            gun.PreventNormalFireAudio = true;
            AkSoundEngine.PostEvent("Play_pewpew", gameObject);
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
                AkSoundEngine.PostEvent("Play_pewpew_reload", base.gameObject);
            }
        }

        private bool HasReloaded;

        [SerializeField]
        private List<AIActor> dartedEnemies = new List<AIActor>();
    }
}
