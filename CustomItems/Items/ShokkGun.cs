﻿using Gungeon;
using ItemAPI;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using Random = UnityEngine.Random;

namespace GlaurungItems.Items
{
    internal class ShokkGun : AdvancedGunBehavior
    {
        public static void Add()
        {
            Gun gun = ETGMod.Databases.Items.NewGun("Shokk Attack Gun", "shokk");
            Game.Items.Rename("outdated_gun_mods:shokk_attack_gun", "gl:shokk_attack_gun");
            gun.gameObject.AddComponent<ShokkGun>();
            gun.SetShortDescription("WAAAGH !");
            gun.SetLongDescription("");
            gun.SetupSprite(null, "pewhand_idle_001", 8);
            gun.SetAnimationFPS(gun.shootAnimation, 24);
            gun.SetAnimationFPS(gun.reloadAnimation, 12);
            //gun.SetAnimationFPS(gun.idleAnimation, 8);
            gun.AddProjectileModuleFrom("klobb", true, false);

            gun.DefaultModule.ammoCost = 1;
            gun.DefaultModule.angleVariance = 1f;
            gun.gunHandedness = GunHandedness.NoHanded;
            gun.DefaultModule.shootStyle = ProjectileModule.ShootStyle.Automatic;
            gun.DefaultModule.sequenceStyle = ProjectileModule.ProjectileSequenceStyle.Random;
            gun.reloadTime = 1.5f;
            gun.DefaultModule.cooldownTime = 0.25f;
            gun.DefaultModule.burstShotCount = 2;
            gun.DefaultModule.burstCooldownTime = 0.3f;
            gun.DefaultModule.numberOfShotsInClip = 50;
            //gun.usesContinuousFireAnimation = true;
            gun.SetBaseMaxAmmo(500);
            gun.muzzleFlashEffects = (PickupObjectDatabase.GetById(81) as Gun).muzzleFlashEffects;

            gun.quality = PickupObject.ItemQuality.B;

            Projectile projectile = UnityEngine.Object.Instantiate<Projectile>(gun.DefaultModule.projectiles[0]);
            projectile.gameObject.SetActive(false);
            FakePrefab.MarkAsFakePrefab(projectile.gameObject);
            UnityEngine.Object.DontDestroyOnLoad(projectile);
            gun.DefaultModule.projectiles[0] = projectile;

            projectile.baseData.damage = 0;
            projectile.baseData.damage *= 0;
            projectile.baseData.speed *= 1f;
            projectile.baseData.force *= 0f;
            projectile.baseData.range *= 0.0001f;
            projectile.PenetratesInternalWalls = true;
            projectile.pierceMinorBreakables = true;
            projectile.ignoreDamageCaps = true;
            projectile.AdditionalScaleMultiplier = 3;
            projectile.sprite.renderer.enabled = false;
            projectile.transform.parent = gun.barrelOffset;

            //projectile.SetProjectileSpriteRight("build_projectile", 5, 5);

            ETGMod.Databases.Items.Add(gun, null, "ANY");
        }

        public override void PostProcessProjectile(Projectile projectile)
        {
            base.PostProcessProjectile(projectile);
            if(this.gun.CurrentOwner && (this.gun.CurrentOwner is PlayerController))
            {
                projectile.DieInAir();
                PlayerController user = this.gun.CurrentOwner as PlayerController;
                float nearestEnemyPosition;
                AIActor nomTarget = user.CurrentRoom.GetNearestEnemy(user.CenterPosition, out nearestEnemyPosition, true, true);
                if (nearestEnemyPosition < 5f && nomTarget.healthHaver
                    && !nomTarget.healthHaver.IsBoss && nomTarget.healthHaver.IsAlive &&
                    nomTarget.healthHaver.GetMaxHealth() <= (20 * AIActor.BaseLevelHealthModifier))
                {
                    absorbedEnemyUuids.Add(nomTarget.EnemyGuid);
                    DEVOUR(nomTarget);
                }
            }

        }

        private void DEVOUR(AIActor target)
        {
            if (target != null && !target.healthHaver.IsBoss)
            {
                GameManager.Instance.Dungeon.StartCoroutine(this.HandleEnemySuck(target));
                target.EraseFromExistenceWithRewards(true);
            }
        }
        private IEnumerator HandleEnemySuck(AIActor target)
        {
            PlayerController playerController = this.gun.CurrentOwner as PlayerController;
            Transform copySprite = this.CreateEmptySprite(target);
            Vector3 startPosition = copySprite.transform.position;
            float elapsed = 0f;
            float duration = 0.3f;
            while (elapsed < duration)
            {
                elapsed += BraveTime.DeltaTime;
                bool TRESS = playerController.CurrentGun && copySprite;
                if (TRESS)
                {
                    Vector3 position = playerController.CurrentGun.PrimaryHandAttachPoint.position;
                    float t = elapsed / duration * (elapsed / duration);
                    copySprite.position = Vector3.Lerp(startPosition, position, t);
                    copySprite.rotation = Quaternion.Euler(0f, 0f, 360f * BraveTime.DeltaTime) * copySprite.rotation;
                    copySprite.localScale = Vector3.Lerp(Vector3.one, new Vector3(0.1f, 0.1f, 0.1f), t);
                    position = default(Vector3);
                }
                yield return null;
            }
            bool flag4 = copySprite;
            if (flag4)
            {
                UnityEngine.Object.Destroy(copySprite.gameObject);
            }
            yield break;
        }
        private Transform CreateEmptySprite(AIActor target)
        {
            GameObject gameObject = new GameObject("suck image");
            gameObject.layer = target.gameObject.layer;
            tk2dSprite tk2dSprite = gameObject.AddComponent<tk2dSprite>();
            gameObject.transform.parent = SpawnManager.Instance.VFX;
            tk2dSprite.SetSprite(target.sprite.Collection, target.sprite.spriteId);
            tk2dSprite.transform.position = target.sprite.transform.position;
            GameObject gameObject2 = new GameObject("image parent");
            gameObject2.transform.position = tk2dSprite.WorldCenter;
            tk2dSprite.transform.parent = gameObject2.transform;
            bool flag = target.optionalPalette != null;
            if (flag)
            {
                tk2dSprite.renderer.material.SetTexture("_PaletteTex", target.optionalPalette);
            }
            return gameObject2.transform;
        }

        // boilerplate stuff
        //This block of code allows us to change the reload sounds.
        public override void OnPostFired(PlayerController player, Gun gun)
        {
            //This determines what sound you want to play when you fire a gun.
            //Sounds names are based on the Gungeon sound dump, which can be found at EnterTheGungeon/Etg_Data/StreamingAssets/Audio/GeneratedSoundBanks/Windows/sfx.txt
            gun.PreventNormalFireAudio = true;
            AkSoundEngine.PostEvent("Play_WPN_smileyrevolver_shot_01", gameObject);
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
                if(player && player.CurrentRoom != null && absorbedEnemyUuids != null && absorbedEnemyUuids.Count > 0)
                {
                    GameManager.Instance.StartCoroutine(KaboomsTeleports(player));
                }
            }
        }

        private IEnumerator KaboomsTeleports(PlayerController player)
        {
            Projectile Boomprojectile = ((Gun)ETGMod.Databases.Items[593]).DefaultModule.projectiles[0];
            ExplosiveModifier explo = Boomprojectile.gameObject.GetComponent<ExplosiveModifier>();
            ExplosiveModifier boomer = new ExplosiveModifier();
            boomer.explosionData = explo.explosionData.CopyExplosionData();
            boomer.explosionData.damageToPlayer = 0f;
            boomer.explosionData.damage = 10f;
            boomer.explosionData.pushRadius = 4f;
            boomer.explosionData.force = 5f;
            boomer.explosionData.doForce = true;
            boomer.explosionData.doDestroyProjectiles = true;
            boomer.explosionData.preventPlayerForce = true;

            int absorbedCount = absorbedEnemyUuids.Count;
            Dungeonator.RoomHandler userRoom = player.CurrentRoom;
            Vector2 firePos = player.CenterPosition;
            Vector2 fireDirection = (Vector2)(Quaternion.Euler(0, 0,player.CurrentGun.CurrentAngle) * Vector2.right);

            for (int i = 0; i < absorbedCount; i++)
            {
                float x;
                float y;

                if(fireDirection.x >= 0)
                {
                    x = Random.Range(-1f, 5f);
                }
                else
                {
                    x = Random.Range(-5f, 1f);
                }

                if (fireDirection.y >= 0)
                {
                    y = Random.Range(-1f, 5f);
                }
                else
                {
                    y = Random.Range(-5f, 1f);
                }

                Vector2 rndPoint = firePos + (new Vector2(x, y) * Random.Range(1f, 4f));

                Exploder.Explode(rndPoint, boomer.explosionData, Vector2.zero, null, false, 0, false);
                yield return new WaitForSeconds(0.1f);
            }
            absorbedEnemyUuids = new List<string>();
            yield break;
        }

        private bool HasReloaded;

        [SerializeField]
        private List<String> absorbedEnemyUuids = new List<string>();
    }
}
