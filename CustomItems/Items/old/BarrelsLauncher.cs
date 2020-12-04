using Gungeon;
using ItemAPI;
using System;
using System.Collections.Generic;
using System.Timers;
using UnityEngine;
using Random = UnityEngine.Random;

namespace GlaurungItems.Items
{
    //crash game on room change...
    class BarrelsLauncher : AdvancedGunBehavior
    {
        public static void Add()
        {
            Gun gun = ETGMod.Databases.Items.NewGun("Barrels Launcher", "jpxfrd");
            Game.Items.Rename("outdated_gun_mods:barrels_launcher", "gl:barrels_launcher");
            gun.gameObject.AddComponent<BarrelsLauncher>();
            gun.SetShortDescription("WIP");
            gun.SetLongDescription("WIP");
            gun.SetupSprite(null, "jpxfrd_idle_001", 8);
            gun.SetAnimationFPS(gun.shootAnimation, 24);
            gun.SetAnimationFPS(gun.reloadAnimation, 12);
            //gun.SetAnimationFPS(gun.idleAnimation, 8);
            gun.AddProjectileModuleFrom("klobb", true, false);

            gun.DefaultModule.ammoCost = 1;
            gun.DefaultModule.angleVariance = 2f;
            gun.DefaultModule.shootStyle = ProjectileModule.ShootStyle.SemiAutomatic;
            gun.DefaultModule.sequenceStyle = ProjectileModule.ProjectileSequenceStyle.Random;
            gun.reloadTime = 2f;
            gun.DefaultModule.cooldownTime = 1f;
            gun.DefaultModule.numberOfShotsInClip = 3;
            gun.muzzleFlashEffects.type = VFXPoolType.None;
            gun.barrelOffset.transform.localPosition = new Vector3(-0.4f, 0f, 0f);
            gun.SetBaseMaxAmmo(100);

            gun.quality = PickupObject.ItemQuality.D;

            Projectile projectile = UnityEngine.Object.Instantiate<Projectile>((PickupObjectDatabase.GetById(31) as Gun).DefaultModule.projectiles[0]);
            projectile.gameObject.SetActive(false);
            FakePrefab.MarkAsFakePrefab(projectile.gameObject);
            UnityEngine.Object.DontDestroyOnLoad(projectile);
            gun.DefaultModule.projectiles[0] = projectile;

            projectile.baseData.damage = 0f;
            projectile.baseData.speed = 0f;
            projectile.baseData.force = 0f;
            projectile.baseData.range = 0f;
            projectile.AdditionalScaleMultiplier = 0.01f;
            projectile.transform.parent = gun.barrelOffset;
            //projectile.hitEffects.suppressMidairDeathVfx = true;

            ETGMod.Databases.Items.Add(gun, null, "ANY");
        }

        public override void PostProcessProjectile(Projectile projectile)
        {
            projectile.DieInAir(true);
            //UnityEngine.Object.Destroy(projectile.gameObject);
            base.PostProcessProjectile(projectile);
            if (this.gun && this.gun.CurrentOwner && this.gun.CurrentOwner is PlayerController && (this.gun.CurrentOwner as PlayerController).CurrentRoom != null)
            {
                AssetBundle sharedAssets = ResourceManager.LoadAssetBundle("shared_auto_001");
                AssetBundle sharedAssets2 = ResourceManager.LoadAssetBundle("shared_auto_002");
                GameObject Drum = sharedAssets2.LoadAsset<GameObject>("Blue Drum");

                int randomSelect = Random.Range(1, 4);
                switch (randomSelect)
                {
                    case 1:
                        Drum = sharedAssets.LoadAsset<GameObject>("Red Drum");
                        break;
                    case 2:
                        Drum = sharedAssets2.LoadAsset<GameObject>("Yellow Drum");
                        break;
                    case 3:
                        Drum = sharedAssets2.LoadAsset<GameObject>("Blue Drum");
                        break;
                    default:
                        break;
                }

                PlayerController user = this.gun.CurrentOwner as PlayerController;
                float roomPosX = user.transform.position.x - user.CurrentRoom.area.basePosition.x;
                float roomPosY = user.transform.position.y - user.CurrentRoom.area.basePosition.y;
                float xOffSet = 0;
                float yOffSet = 0;
                float offsetAmount = 1.5f;
                float currentAngle = this.gun.CurrentAngle;
                if (currentAngle > 45f && currentAngle <= 135f)
                {
                    yOffSet = offsetAmount;//up
                }
                else if((currentAngle > 0 && currentAngle > 135f) || (currentAngle < 0 && currentAngle <= -135f))
                {
                    xOffSet = -offsetAmount;//left
                }
                else if(currentAngle > -135f && currentAngle <= -45f)
                {
                    yOffSet = -offsetAmount;//bottom
                }
                else
                {
                    xOffSet = offsetAmount;//right
                }
                Vector2 posInCurrentRoom = new Vector2(roomPosX + xOffSet, roomPosY + yOffSet);
                Vector2 posInMap = new Vector2(user.transform.position.x + xOffSet, user.transform.position.y + yOffSet);

                if (user.IsValidPlayerPosition(posInMap))
                {
                    GameObject spawnedDrum = Drum.GetComponent<DungeonPlaceableBehaviour>().InstantiateObject(user.CurrentRoom, posInCurrentRoom.ToIntVector2(), false);
                    KickableObject componentInChildren = spawnedDrum.GetComponentInChildren<KickableObject>();
                    if (componentInChildren)
                    {
                        componentInChildren.specRigidbody.Reinitialize();
                        componentInChildren.rollSpeed = 5f;
                        user.CurrentRoom.RegisterInteractable(componentInChildren);
                    }
                }

            }
        }

        public override void OnReload(PlayerController player, Gun gun)
        {

            base.OnReload(player, gun);
        }

        public override void OnPostFired(PlayerController player, Gun gun)
        {
            gun.PreventNormalFireAudio = true;
            AkSoundEngine.PostEvent("Play_WPN_metalbullet_impact_01", gameObject);
        }
        //This block of code allows us to change the reload sounds.
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
    }
}
