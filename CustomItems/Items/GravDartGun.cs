using Gungeon;
using ItemAPI;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

/*
 remove collisions pb
 */
namespace GlaurungItems.Items
{
    class GravDartGun : AdvancedGunBehavior
    {
        public static void Add()
        {
            Gun gun = ETGMod.Databases.Items.NewGun("Grav Dart Launcher", "gravdart");
            Game.Items.Rename("outdated_gun_mods:grav_dart_launcher", "gl:grav_dart_launcher");
            gun.gameObject.AddComponent<GravDartGun>();
            gun.SetShortDescription("Yeet !");
            gun.SetLongDescription("One of the oddest weapons found by the last Ranger in the Arks. \n \nTag targets with non damaging grav darts. When the last shot in the clip is destroyed after being shot, " +
                "project violently all tagged targets toward it's last position. Targets take damage if they collide with something.");
            gun.SetupSprite(null, "gravdart_idle_001", 8);
            gun.SetAnimationFPS(gun.shootAnimation, 24);
            gun.SetAnimationFPS(gun.reloadAnimation, 12);
            gun.AddProjectileModuleFrom((PickupObjectDatabase.GetById(126) as Gun), true, false);

            gun.quality = PickupObject.ItemQuality.B;

            gun.DefaultModule.ammoCost = 1;
            gun.DefaultModule.angleVariance = 1f;
            gun.gunHandedness = GunHandedness.TwoHanded;
            gun.DefaultModule.shootStyle = ProjectileModule.ShootStyle.SemiAutomatic;
            gun.DefaultModule.sequenceStyle = ProjectileModule.ProjectileSequenceStyle.Random;
            gun.reloadTime = 1.5f;
            gun.DefaultModule.cooldownTime = 0.3f;
            gun.DefaultModule.numberOfShotsInClip = 15;
            gun.SetBaseMaxAmmo(360);
            gun.barrelOffset.transform.localPosition = new Vector3(1.7f, 0.5f, 0f);
            gun.muzzleFlashEffects.type = VFXPoolType.None;//(PickupObjectDatabase.GetById(81) as Gun).muzzleFlashEffects;


            Projectile projectile = UnityEngine.Object.Instantiate<Projectile>((PickupObjectDatabase.GetById(357) as Gun).DefaultModule.projectiles[0]);
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

            Gun gunFinalProj = (PickupObjectDatabase.GetById(41) as Gun);

            Projectile finalProj = UnityEngine.Object.Instantiate<Projectile>(gunFinalProj.DefaultModule.chargeProjectiles[0].Projectile);
            finalProj.gameObject.SetActive(false);
            FakePrefab.MarkAsFakePrefab(finalProj.gameObject);
            UnityEngine.Object.DontDestroyOnLoad(finalProj);

            finalProj.baseData.damage = 0;
            finalProj.baseData.force = 0;
            finalProj.baseData.range *= 3;

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
                GameManager.Instance.StartCoroutine(this.RemoveAnnoyingProjModifiers(projectile));

                projectile.OnDestruction += Projectile_Tracer_OnDestruction;
            }
        }

        private IEnumerator RemoveAnnoyingProjModifiers(Projectile projectile)
        {
            yield return new WaitForSeconds(0.1f);
            if (projectile.GetComponent<PierceProjModifier>())
            {
                UnityEngine.GameObject.Destroy(projectile.GetComponent<PierceProjModifier>());
            }
            if (projectile.GetComponent<BounceProjModifier>())
            {
                UnityEngine.GameObject.Destroy(projectile.GetComponent<BounceProjModifier>());
            }
        }

        private void HandleHitEnemy(Projectile proj, SpeculativeRigidbody sr, bool fatal)
        {
            if (sr.aiActor && !fatal && !dartedEnemies.Contains(sr.aiActor) && sr.aiActor.healthHaver && sr.aiActor.healthHaver.IsAlive && sr.aiActor.knockbackDoer)
            {
                dartedEnemies.Add(sr.aiActor);
                //Tools.Print(sr.aiActor.knockbackDoer.weight, "ffffff", true);
            }
        }

        private void Projectile_Tracer_OnDestruction(Projectile proj)
        {
            GameManager.Instance.StartCoroutine(this.OnProjDestructionCoroutine(proj)); 
        }

        private IEnumerator OnProjDestructionCoroutine(Projectile proj)
        {
            Vector2 pos = proj.LastPosition;
            if (dartedEnemies != null && pos != null)
            {
                int nbDarted = dartedEnemies.Count;

                for (int i = 0; i < nbDarted; i++)
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
                        if (dartArray != null && dartArray.Count() > 0)
                        {
                            int nbDarts = dartArray.Count();
                            aiActor.specRigidbody.AddCollisionLayerOverride(CollisionMask.LayerToMask(CollisionLayer.EnemyHitBox));
                            aiActor.specRigidbody.OnPreRigidbodyCollision = (SpeculativeRigidbody.OnPreRigidbodyCollisionDelegate)Delegate.Combine(aiActor.specRigidbody.OnPreRigidbodyCollision, new SpeculativeRigidbody.OnPreRigidbodyCollisionDelegate(this.HandleEnemyHitRigidBody));
                            aiActor.specRigidbody.OnPreTileCollision = (SpeculativeRigidbody.OnPreTileCollisionDelegate)Delegate.Combine(aiActor.specRigidbody.OnPreTileCollision, new SpeculativeRigidbody.OnPreTileCollisionDelegate(this.HandleEnemyHitTile));

                            aiActor.knockbackDoer.SetImmobile(false, "Like-a-boss");

                            this.SetKnockbackImmobileOff(aiActor);

                            aiActor.knockbackDoer.ApplyKnockback(direction, pushForce * nbDarts, true);
                            GameManager.Instance.StartCoroutine(this.CancelCollisionsCoroutine(aiActor.specRigidbody));
                        }
                    }
                }
            }

            dartedEnemies = new List<AIActor>(); 
            yield break;
        }

        private void SetKnockbackImmobileOff(AIActor aiActor)
        {
            foreach (AttackBehaviorBase attackBehav in aiActor.behaviorSpeculator.AttackBehaviors)
            {
                if (attackBehav is AttackBehaviorGroup)
                {
                    foreach (AttackBehaviorGroup.AttackGroupItem item in (attackBehav as AttackBehaviorGroup).AttackBehaviors)
                    {
                        if (item != null && item.Behavior != null)
                        {
                            if (item.Behavior is ShootBehavior)
                            {
                                aiActor.knockbackDoer.SetImmobile(false, "ShootBulletScript");
                            }
                            else if (item.Behavior is BuffEnemiesBehavior)
                            {
                                aiActor.knockbackDoer.SetImmobile(false, "BuffEnemiesBehavior");
                            }
                            else if (item.Behavior is ConsumeTargetBehavior)
                            {
                                aiActor.knockbackDoer.SetImmobile(false, "ConsumeTargetBehavior");
                            }
                            else if (item.Behavior is DeflectBulletsBehavior)
                            {
                                aiActor.knockbackDoer.SetImmobile(false, "DeflectBulletsBehavior");
                            }
                            else if (item.Behavior is DestroyBulletsBehavior)
                            {
                                aiActor.knockbackDoer.SetImmobile(true, "DestroyBulletsBehavior");
                            }
                            else if (item.Behavior is DisplaceBehavior)
                            {
                                aiActor.knockbackDoer.SetImmobile(false, "DisplaceBehavior");
                            }
                            else if (item.Behavior is MirrorImageBehavior)
                            {
                                aiActor.knockbackDoer.SetImmobile(false, "MirrorImageBehavior");
                            }
                            else if (item.Behavior is RemoteShootBehavior)
                            {
                                aiActor.knockbackDoer.SetImmobile(false, "SummonEnemyBehavior");
                            }
                            else if (item.Behavior is SummonEnemyBehavior)
                            {
                                aiActor.knockbackDoer.SetImmobile(false, "SummonEnemyBehavior");
                            }
                            else if (item.Behavior is TeleportBehavior)
                            {
                                aiActor.knockbackDoer.SetImmobile(false, "teleport");
                            }
                            else if (item.Behavior is WizardSpinShootBehavior)
                            {
                                aiActor.knockbackDoer.SetImmobile(false, "WizardSpinShootBehavior");
                            }
                            else if (item.Behavior is GiantPowderSkullMergoBehavior)
                            {
                                aiActor.knockbackDoer.SetImmobile(false, "CrosshairBehavior");
                            }
                        }
                    }
                }
                else if (attackBehav is ShootBehavior)
                {
                    aiActor.knockbackDoer.SetImmobile(false, "ShootBulletScript");
                }
                else if (attackBehav is BuffEnemiesBehavior)
                {
                    aiActor.knockbackDoer.SetImmobile(false, "BuffEnemiesBehavior");
                }
                else if (attackBehav is ConsumeTargetBehavior)
                {
                    aiActor.knockbackDoer.SetImmobile(false, "ConsumeTargetBehavior");
                }
                else if (attackBehav is DeflectBulletsBehavior) 
                {
                    aiActor.knockbackDoer.SetImmobile(false, "DeflectBulletsBehavior");
                }
                else if (attackBehav is DestroyBulletsBehavior) 
                {
                    aiActor.knockbackDoer.SetImmobile(true, "DestroyBulletsBehavior");
                }
                else if (attackBehav is DisplaceBehavior) 
                {
                    aiActor.knockbackDoer.SetImmobile(false, "DisplaceBehavior");
                }
                else if (attackBehav is MirrorImageBehavior) 
                {
                    aiActor.knockbackDoer.SetImmobile(false, "MirrorImageBehavior");
                }
                else if (attackBehav is RemoteShootBehavior) 
                {
                    aiActor.knockbackDoer.SetImmobile(false, "SummonEnemyBehavior");
                }
                else if (attackBehav is SummonEnemyBehavior) 
                {
                    aiActor.knockbackDoer.SetImmobile(false, "SummonEnemyBehavior");
                }
                else if (attackBehav is TeleportBehavior) 
                {
                    aiActor.knockbackDoer.SetImmobile(false, "teleport");
                }
                else if (attackBehav is WizardSpinShootBehavior) 
                {
                    aiActor.knockbackDoer.SetImmobile(false, "WizardSpinShootBehavior");
                }
                else if (attackBehav is GiantPowderSkullMergoBehavior)
                {
                    aiActor.knockbackDoer.SetImmobile(false, "CrosshairBehavior");
                }
            }
        }

        private IEnumerator CancelCollisionsCoroutine(SpeculativeRigidbody myRigidbody)
        {
            yield return new WaitForSeconds(1.5f);
            RemoveCollisions(myRigidbody);

            yield break;
        }

        private void HandleEnemyHitTile(SpeculativeRigidbody myRigidbody, PixelCollider myPixelCollider, PhysicsEngine.Tile tile, PixelCollider tilePixelCollider)
        {
            RemoveCollisions(myRigidbody);
            GameManager.Instance.StartCoroutine(this.HandleEnemyHitTileCoroutine(myRigidbody));  
        }

        private IEnumerator HandleEnemyHitTileCoroutine(SpeculativeRigidbody myRigidbody)
        {
            if (myRigidbody && myRigidbody.aiActor && myRigidbody.aiActor.healthHaver && myRigidbody.aiActor.healthHaver.IsAlive)
            {
                DelayedExplosiveBuff[] dartArray = myRigidbody.aiActor.gameObject.GetComponents<DelayedExplosiveBuff>();

                int nbDarts = 1;
                if (dartArray != null)
                {
                    nbDarts = dartArray.Count();
                }

                //myRigidbody.aiActor.KnockbackVelocity.magnitude

                for (int j = 0; j < nbDarts; j++)
                {
                    if (dartArray[j])
                    {
                        UnityEngine.GameObject.Destroy(dartArray[j]);
                    }
                }

                if (myRigidbody && myRigidbody.aiActor && myRigidbody.aiActor.healthHaver && myRigidbody.aiActor.healthHaver.IsAlive && myRigidbody.Velocity.magnitude > 0)
                {
                    if (myRigidbody.Velocity.magnitude > 15) SpawnImpactProj(myRigidbody.aiActor);
                    myRigidbody.aiActor.healthHaver.ApplyDamage(Math.Min(baseDmg * nbDarts * myRigidbody.Velocity.magnitude, maxDmg), myRigidbody.Velocity, "GravDart", CoreDamageTypes.None, DamageCategory.Normal, false, null, false);
                }
            }

            yield break;
        }

        private void HandleEnemyHitRigidBody(SpeculativeRigidbody myRigidbody, PixelCollider myPixelCollider, SpeculativeRigidbody otherRigidbody, PixelCollider otherPixelCollider)
        {
            RemoveCollisions(myRigidbody);
            GameManager.Instance.StartCoroutine(this.HandleEnemyHitRigidBodyCoroutine(myRigidbody, otherRigidbody));
        }

        private IEnumerator HandleEnemyHitRigidBodyCoroutine(SpeculativeRigidbody myRigidbody, SpeculativeRigidbody otherRigidbody)
        {
            if (myRigidbody && myRigidbody.aiActor && myRigidbody.aiActor.healthHaver && myRigidbody.aiActor.healthHaver.IsAlive)
            {
                DelayedExplosiveBuff[] dartArray = myRigidbody.aiActor.gameObject.GetComponents<DelayedExplosiveBuff>();

                int nbDarts = 1;
                if (dartArray != null)
                {
                    nbDarts = dartArray.Count();
                }

                if (otherRigidbody && otherRigidbody.aiActor && otherRigidbody.aiActor.healthHaver && otherRigidbody.aiActor.healthHaver.IsAlive)
                {
                    otherRigidbody.aiActor.healthHaver.ApplyDamage(Math.Min(baseDmg * nbDarts * myRigidbody.Velocity.magnitude, maxDmg), myRigidbody.Velocity, "GravDart", CoreDamageTypes.None, DamageCategory.Normal, false, null, false);
                }

                //myRigidbody.aiActor.KnockbackVelocity.magnitude

                for (int j = 0; j < nbDarts; j++)
                {
                    if (dartArray[j])
                    {
                        UnityEngine.GameObject.Destroy(dartArray[j]);
                    }
                }

                if (myRigidbody && myRigidbody.aiActor && myRigidbody.aiActor.healthHaver && myRigidbody.aiActor.healthHaver.IsAlive)
                {
                    if(myRigidbody.Velocity.magnitude > 15) SpawnImpactProj(myRigidbody.aiActor);
                    myRigidbody.aiActor.healthHaver.ApplyDamage(Math.Min(baseDmg * nbDarts * myRigidbody.Velocity.magnitude, maxDmg), myRigidbody.Velocity, "GravDart", CoreDamageTypes.None, DamageCategory.Normal, false, null, false);
                }
            }

            yield break;
        }

        private void SpawnImpactProj(AIActor actor)
        {
            PlayerController man = base.gun.CurrentOwner as PlayerController;
            Projectile projectile2 = ((Gun)global::ETGMod.Databases.Items[541]).DefaultModule.chargeProjectiles[0].Projectile;
            GameObject gameObject = SpawnManager.SpawnProjectile(projectile2.gameObject, actor.CenterPosition, Quaternion.Euler(0f, 0f, 0f), true);
            Projectile component = gameObject.GetComponent<Projectile>();
            bool flag8 = component != null;
            if (flag8)
            {
                component.AdditionalScaleMultiplier = 1.25f;
                component.baseData.damage = 0;
                component.baseData.force = 0f;
                component.baseData.speed = 0f;
                component.Owner = man;
                component.Shooter = man.specRigidbody;
                GameManager.Instance.StartCoroutine(DelProj(component));
            }
        }

        private IEnumerator DelProj(Projectile component)
        {
            yield return new WaitForSeconds(0.5f);
            if (component)
            {
                component.DieInAir(true);
            }
        }

        private void RemoveCollisions(SpeculativeRigidbody myRigidbody)
        {
            //Tools.Print("remove collissions", "ffffff", true);
            myRigidbody.AddCollisionLayerIgnoreOverride(CollisionMask.LayerToMask(CollisionLayer.EnemyHitBox));
            myRigidbody.OnPreRigidbodyCollision = (SpeculativeRigidbody.OnPreRigidbodyCollisionDelegate)Delegate.Remove(myRigidbody.OnPreRigidbodyCollision, new SpeculativeRigidbody.OnPreRigidbodyCollisionDelegate(this.HandleEnemyHitRigidBody));
            myRigidbody.OnPreTileCollision = (SpeculativeRigidbody.OnPreTileCollisionDelegate)Delegate.Remove(myRigidbody.OnPreTileCollision, new SpeculativeRigidbody.OnPreTileCollisionDelegate(this.HandleEnemyHitTile));
        }



        // boilerplate stuff
        //This block of code allows us to change the reload sounds.
        public override void OnPostFired(PlayerController player, Gun gun)
        {
            //This determines what sound you want to play when you fire a gun.
            //Sounds names are based on the Gungeon sound dump, which can be found at EnterTheGungeon/Etg_Data/StreamingAssets/Audio/GeneratedSoundBanks/Windows/sfx.txt
            gun.PreventNormalFireAudio = true;
            AkSoundEngine.PostEvent("Play_WPN_claw_blast_01", gameObject);
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
                if(gun.CurrentAmmo > 0 && gun.ClipShotsRemaining > 0)
                {
                    Projectile projectile2 = gun.DefaultModule.finalProjectile;
                    GameObject gameObject = SpawnManager.SpawnProjectile(projectile2.gameObject, player.CenterPosition, Quaternion.Euler(0f, 0f, player.CurrentGun ? player.CurrentGun.CurrentAngle : 0f), true);
                    Projectile component = gameObject.GetComponent<Projectile>();
                    bool flag8 = component != null;
                    if (flag8)
                    {
                        component.Owner = player;
                        component.Shooter = player.specRigidbody;
                        player.DoPostProcessProjectile(component);
                        PostProcessProjectile(component);
                    }
                    gun.CurrentAmmo -= 1;
                }
                HasReloaded = false;
                AkSoundEngine.PostEvent("Stop_WPN_All", base.gameObject);
                base.OnReloadPressed(player, gun, bSOMETHING);
                AkSoundEngine.PostEvent("Play_WPN_SAA_reload_01", base.gameObject);
            }
        }

        private bool HasReloaded;
        private static readonly float pushForce = 80f;
        private static readonly float baseDmg = 2f;
        private static readonly float maxDmg = 200f;

        [SerializeField]
        private List<AIActor> dartedEnemies = new List<AIActor>();
    }
}
