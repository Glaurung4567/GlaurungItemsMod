using EnemyAPI;
using System.Collections.Generic;
using UnityEngine;
using ItemAPI;
using Gungeon;

namespace GlaurungItems.Items
{
    internal class Linc : AdvancedGunBehavior
    {
        public static void Add()
        {
            Gun gun = ETGMod.Databases.Items.NewGun("Linc", "linc");
            Game.Items.Rename("outdated_gun_mods:linc", "gl:linc");
            gun.gameObject.AddComponent<Linc>();
            gun.SetShortDescription("Power is Pizza");
            gun.SetLongDescription("This gun was created by a reborn weapons manufacturing corporation in a distant galaxy. The signature feature of their guns is a smart projectile tracking system. " +
                "\n \nThis one can fire rounds or a non damaging projectile which mark nearby enemies when it's reloaded with a full mag, which will make the standard projectiles home on marked enemies, one at a time.");
            gun.SetupSprite(null, "linc_idle_001", 8);
            gun.SetAnimationFPS(gun.shootAnimation, 24);
            gun.SetAnimationFPS(gun.reloadAnimation, 12);
            //gun.SetAnimationFPS(gun.idleAnimation, 8);
            gun.AddProjectileModuleFrom("klobb", true, false);

            gun.DefaultModule.ammoCost = 1;
            gun.DefaultModule.angleVariance = Linc.baseAngleVar;
            gun.DefaultModule.shootStyle = Linc.baseShootStyle;
            gun.DefaultModule.sequenceStyle = ProjectileModule.ProjectileSequenceStyle.Random;
            gun.reloadTime = 0.6f;
            gun.DefaultModule.cooldownTime = 0.1f;
            gun.DefaultModule.numberOfShotsInClip = Linc.baseMagSize;
            //gun.usesContinuousFireAnimation = true;
            gun.SetBaseMaxAmmo(400);
            gun.gunClass = GunClass.FULLAUTO;
            gun.muzzleFlashEffects = (PickupObjectDatabase.GetById(81) as Gun).muzzleFlashEffects;

            gun.quality = PickupObject.ItemQuality.C;

            Projectile projectile = UnityEngine.Object.Instantiate<Projectile>(gun.DefaultModule.projectiles[0]);
            projectile.gameObject.SetActive(false);
            FakePrefab.MarkAsFakePrefab(projectile.gameObject);
            UnityEngine.Object.DontDestroyOnLoad(projectile);
            gun.DefaultModule.projectiles[0] = projectile;

            projectile.baseData.damage = 1;
            projectile.baseData.damage *= Linc.baseDmgMultiplier;
            projectile.baseData.speed *= 1.5f;
            projectile.baseData.force *= 1f;
            projectile.baseData.range *= 3f;
            projectile.transform.parent = gun.barrelOffset;

            //projectile.SetProjectileSpriteRight("build_projectile", 5, 5);

            ETGMod.Databases.Items.Add(gun, null, "ANY");
        }

        public override void PostProcessProjectile(Projectile projectile)
        {
            base.PostProcessProjectile(projectile);
            if (!altFireOn)
            {
                foreach(AIActor actor in targetedEnnemies)
                {
                    if(actor && actor.healthHaver && actor.healthHaver.IsAlive)
                    {
                        LockOnHomingModifier homing = projectile.gameObject.GetOrAddComponent<LockOnHomingModifier>();
                        homing.HomingRadius = 50;
                        homing.lockOnTarget = actor;
                        homing.AngularVelocity = 600;
                        return;
                    }
                }
            }
            else
            {
                projectile.OnDestruction += Projectile_OnDestruction;
            }
        }

        private void Projectile_OnDestruction(Projectile proj)
        {
            //from cel fireworkRifle
            AIActor Firecracker = EnemyDatabase.GetOrLoadByGuid(EnemyGuidDatabase.Entries["m80_kin"]);
            ExplosionData firework = new ExplosionData
            {
                damageRadius = 1.5f,
                damageToPlayer = 0f,
                doDamage = false,
                damage = 0,
                doExplosionRing = true,
                doDestroyProjectiles = false,
                doForce = false,
                debrisForce = 0,
                pushRadius = 0,
                force = 0,
                preventPlayerForce = true,
                explosionDelay = 0f,
                usesComprehensiveDelay = false,
                doScreenShake = false,
                playDefaultSFX = true,
                effect = Firecracker.GetComponent<ExplodeOnDeath>().explosionData.effect,
                //AssetBundle assetBundle = ResourceManager.LoadAssetBundle("shared_auto_001");
                //  GameObject TestingVFX = assetBundle.LoadAsset<GameObject>("VFX_Dust_Explosion");
            };

            Exploder.Explode(proj.LastPosition, firework, Vector2.zero, null, true, CoreDamageTypes.None, false);
            if (this.gun.CurrentOwner && this.gun.CurrentOwner is PlayerController &&
                (this.gun.CurrentOwner as PlayerController).CurrentRoom != null
                && (this.gun.CurrentOwner as PlayerController).CurrentRoom.GetActiveEnemies(Dungeonator.RoomHandler.ActiveEnemyType.All) != null)
            {
                List<AIActor> actorsInRoom = (this.gun.CurrentOwner as PlayerController).CurrentRoom.GetActiveEnemies(Dungeonator.RoomHandler.ActiveEnemyType.All);
                targetedEnnemies = new List<AIActor>();
                foreach (AIActor actor in actorsInRoom)
                {
                    //Tools.Print(Vector2.Distance(actor.transform.position, proj.transform.position), "ffffff", true);
                    if(Vector2.Distance(actor.transform.position, proj.transform.position) <= 6f &&  actor.healthHaver && actor.healthHaver.IsAlive)
                    {
                        //Tools.Print(actor.ActorName, "ffffff", true);
                        targetedEnnemies.Add(actor);
                    }
                }
            }
        }

        protected override void OnPickup(PlayerController player)
        {
            base.OnPickup(player);
            //player.GunChanged += this.OnGunChanged;
            player.OnRoomClearEvent += this.OnLeaveCombat;
        }

        protected override void OnPostDrop(PlayerController user)
        {
            user.OnRoomClearEvent -= this.OnLeaveCombat;
            base.OnPostDrop(user);
        }

        private void OnLeaveCombat(PlayerController user)
        {
            if (user != null)
            {
                targetedEnnemies = new List<AIActor>();
            }
        }

        // boilerplate stuff
        //This block of code allows us to change the reload sounds.
        public override void OnPostFired(PlayerController player, Gun gun)
        {
            //This determines what sound you want to play when you fire a gun.
            //Sounds names are based on the Gungeon sound dump, which can be found at EnterTheGungeon/Etg_Data/StreamingAssets/Audio/GeneratedSoundBanks/Windows/sfx.txt
            gun.PreventNormalFireAudio = true;
            AkSoundEngine.PostEvent("Play_WPN_beretta_shot_01", gameObject);
            //AkSoundEngine.PostEvent("Play_pewpew", gameObject);
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
                if(gun.CurrentOwner is PlayerController)
                {
                    if(oldAmmo < 0 && currAmmo < 0)
                    {
                        oldAmmo = currAmmo = gun.CurrentAmmo;
                    }
                    else
                    {
                        currAmmo = gun.CurrentAmmo;
                        if(currAmmo > oldAmmo)
                        {
                            altFireOn = false;
                        }
                        oldAmmo = currAmmo;
                    }
                }
            }
        }

        public override void OnReloadPressed(PlayerController player, Gun gun, bool bSOMETHING)
        {
            if ((gun.ClipCapacity == gun.ClipShotsRemaining) || (gun.CurrentAmmo == gun.ClipShotsRemaining))
            {
                SwitchFire();
            }
            if (gun.IsReloading && this.HasReloaded)
            {
                HasReloaded = false;
                AkSoundEngine.PostEvent("Stop_WPN_All", base.gameObject);
                base.OnReloadPressed(player, gun, bSOMETHING);
                AkSoundEngine.PostEvent("Play_WPN_SAA_reload_01", base.gameObject);
                if (altFireOn)
                {
                    SwitchFire();
                }
            }
        }

        private void SwitchFire()
        {
            if (!altFireOn)
            {
                this.gun.DefaultModule.angleVariance = 0f;
                this.gun.DefaultModule.numberOfShotsInClip = 1;
                this.gun.DefaultModule.shootStyle = ProjectileModule.ShootStyle.SemiAutomatic;
                this.gun.DefaultModule.projectiles[0].baseData.damage = 0;
                //faster proj + fire
                altFireOn = true;
                if ((this.gun.CurrentOwner is PlayerController) && (this.gun.CurrentOwner as PlayerController).carriedConsumables != null) { (this.gun.CurrentOwner as PlayerController).carriedConsumables.ForceUpdateUI(); }
            }
            else
            {
                this.gun.DefaultModule.shootStyle = Linc.baseShootStyle;
                this.gun.DefaultModule.angleVariance = Linc.baseAngleVar;
                this.gun.DefaultModule.numberOfShotsInClip = Linc.baseMagSize;
                this.gun.DefaultModule.projectiles[0].baseData.damage = 1;
                this.gun.DefaultModule.projectiles[0].baseData.damage *= Linc.baseDmgMultiplier;

                if ((this.gun.CurrentOwner is PlayerController) && (this.gun.CurrentOwner as PlayerController).carriedConsumables != null) { (this.gun.CurrentOwner as PlayerController).carriedConsumables.ForceUpdateUI(); }

                altFireOn = false;
            }
        }

        private bool HasReloaded;
        private int oldAmmo = -10;
        private int currAmmo = -10;
        private static int baseMagSize = 20;
        private static float baseAngleVar = 5f;
        private static float baseDmgMultiplier = 4f;
        private static ProjectileModule.ShootStyle baseShootStyle = ProjectileModule.ShootStyle.SemiAutomatic;
        [SerializeField]
        private bool altFireOn;
        [SerializeField]
        private List<AIActor> targetedEnnemies = new List<AIActor>();
    }
}


