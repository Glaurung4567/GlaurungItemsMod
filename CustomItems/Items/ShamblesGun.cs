using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Dungeonator;
using Gungeon;
using UnityEngine;
using ItemAPI;
using Random = UnityEngine.Random;

namespace GlaurungItems.Items
{
    internal class ShamblesGun: AdvancedGunBehavior
    {
        public static void Add()
        {
            // Get yourself a new gun "base" first.
            // Let's just call it "Basic Gun", and use "jpxfrd" for all sprites and as "codename" All sprites must begin with the same word as the codename. For example, your firing sprite would be named "jpxfrd_fire_001".
            Gun gun = ETGMod.Databases.Items.NewGun("Shambles", "shambles");
            
            // "kp:basic_gun determines how you spawn in your gun through the console. You can change this command to whatever you want, as long as it follows the "name:itemname" template.
            Game.Items.Rename("outdated_gun_mods:shambles", "gl:shambles");
            gun.gameObject.AddComponent<ShamblesGun>();
            
            //These two lines determines the description of your gun, ".SetShortDescription" being the description that appears when you pick up the gun and ".SetLongDescription" being the description in the Ammonomicon entry. 
            gun.SetShortDescription("Room");
            gun.SetLongDescription("A gun prototype made with the intent to switch the places of the user and the target.\n \n" +
                "After the first few attemps and the death of one too many testers, the creator added as many safeties as he could think of for the user and the target (yes he was really paranoïd), " +
                "so be warned now the gun doesn't always work.");
            
            // This is required, unless you want to use the sprites of the base gun.
            // That, by default, is the pea shooter.
            // SetupSprite sets up the default gun sprite for the ammonomicon and the "gun get" popup.
            // WARNING: Add a copy of your default sprite to Ammonomicon Encounter Icon Collection!
            // That means, "sprites/Ammonomicon Encounter Icon Collection/defaultsprite.png" in your mod .zip. You can see an example of this with inside the mod folder.
            gun.SetupSprite(null, "shambles_idle_001", 8);
            
            // ETGMod automatically checks which animations are available.
            // The numbers next to "shootAnimation" determine the animation fps. You can also tweak the animation fps of the reload animation and idle animation using this method.
            gun.SetAnimationFPS(gun.shootAnimation, 12);
            GunExt.SetAnimationFPS(gun, gun.reloadAnimation, 12);
            GunExt.SetAnimationFPS(gun, gun.idleAnimation, 4);
            
            // Every modded gun has base projectile it works with that is borrowed from other guns in the game. 
            // The gun names are the names from the JSON dump! While most are the same, some guns named completely different things. If you need help finding gun names, ask a modder on the Gungeon discord.
            gun.AddProjectileModuleFrom("ak-47", true, false);
            
            // Here we just take the default projectile module and change its settings how we want it to be.
            gun.DefaultModule.ammoCost = 1;
            gun.DefaultModule.angleVariance = 0f;
            gun.DefaultModule.shootStyle = ProjectileModule.ShootStyle.SemiAutomatic;
            gun.DefaultModule.sequenceStyle = ProjectileModule.ProjectileSequenceStyle.Random;
            gun.reloadTime = 1.5f;
            gun.DefaultModule.cooldownTime = 0.5f;
            gun.DefaultModule.numberOfShotsInClip = 1;
            gun.SetBaseMaxAmmo(100);
            
            // Here we just set the quality of the gun and the "EncounterGuid", which is used by Gungeon to identify the gun.
            gun.quality = PickupObject.ItemQuality.B;
            gun.encounterTrackable.EncounterGuid = "change this for different guns, so the game doesn't think they're the same gun of  course yeah sure pal ";
            
            //This block of code helps clone our projectile. Basically it makes it so things like Shadow Clone and Hip Holster keep the stats/sprite of your custom gun's projectiles.
            Projectile projectile = UnityEngine.Object.Instantiate<Projectile>(gun.DefaultModule.projectiles[0]);
            projectile.gameObject.SetActive(false);
            FakePrefab.MarkAsFakePrefab(projectile.gameObject);
            UnityEngine.Object.DontDestroyOnLoad(projectile);
            gun.DefaultModule.projectiles[0] = projectile;
            
            //projectile.baseData allows you to modify the base properties of your projectile module.
            //In our case, our gun uses modified projectiles from the ak-47.
            //Setting static values for a custom gun's projectile stats prevents them from scaling with player stats and bullet modifiers (damage, shotspeed, knockback)
            //You have to multiply the value of the original projectile you're using instead so they scale accordingly. For example if the projectile you're using as a base has 10 damage and you want it to be 6 you use this
            //In our case, our projectile has a base damage of 5.5, so we multiply it by 1.1 so it does 10% more damage from the ak-47.
            projectile.baseData.damage *= 0.4f;
            projectile.baseData.speed *= 20f;
            projectile.baseData.force = 0f;
            projectile.transform.parent = gun.barrelOffset;
            /*
            BulletStunModifier bulletStunModifier = projectile.gameObject.AddComponent<BulletStunModifier>();
            bulletStunModifier.doVFX = true;
            bulletStunModifier.stunLength = 0.8f;
            bulletStunModifier.chanceToStun = 1f;
            */
            
            //This determines what sprite you want your projectile to use. Note this isn't necessary if you don't want to have a custom projectile sprite.
            //The x and y values determine the size of your custom projectile
            projectile.SetProjectileSpriteRight("build_projectile", 5, 5);
            
            AdvancedDualWieldSynergyProcessor advancedDualWieldSynergyProcessor = gun.gameObject.AddComponent<AdvancedDualWieldSynergyProcessor>();
            advancedDualWieldSynergyProcessor.SynergyNameToCheck = "Landing Hate";
            advancedDualWieldSynergyProcessor.PartnerGunID = 520;
            ETGMod.Databases.Items.Add(gun, null, "ANY");

        }

        public void OnProjectileHitEnemy(Projectile proj, SpeculativeRigidbody enemy, bool fatal)
        {
            bool flag = enemy != null;
            if (flag)
            {
                bool flag2 = enemy.aiActor != null && !enemy.aiActor.healthHaver.IsBoss && this.gun && this.gun.CurrentOwner;
                if (flag2)
                {
                    AIActor aiActor = enemy.aiActor;
                    PlayerController player = this.gun.CurrentOwner as PlayerController;
                    RoomHandler currentRoom = player.CurrentRoom;
                    if (currentRoom != null)
                    {
                        Vector2 playerPosition = this.gun.CurrentOwner.transform.position;
                        Vector2 enemyPosition = enemy.transform.position;
                        CellData cellAim = currentRoom.GetNearestCellToPosition(playerPosition);
                        CellData cellAimLeft = currentRoom.GetNearestCellToPosition(playerPosition+Vector2.left);
                        CellData cellAimRight = currentRoom.GetNearestCellToPosition(playerPosition+Vector2.right);
                        CellData cellAimUp = currentRoom.GetNearestCellToPosition(playerPosition+Vector2.up);
                        CellData cellAimDown = currentRoom.GetNearestCellToPosition(playerPosition+Vector2.down);

                        //check if the player will not be spawned into a bad spot and if the enemy might not end completely in a wall
                        if (player.IsValidPlayerPosition(enemyPosition) && !cellAim.isNextToWall && !ShamblesGun.BannedEnemies.Contains(aiActor.EnemyGuid)
                            && (!ShamblesGun.BigEnemies.Contains(aiActor.EnemyGuid) || 
                            (ShamblesGun.BigEnemies.Contains(aiActor.EnemyGuid) && !cellAimLeft.isNextToWall && !cellAimRight.isNextToWall 
                            && !cellAimUp.isNextToWall && !cellAimDown.isNextToWall))
                            )
                        {
                            GameManager.Instance.StartCoroutine(this.Shambles(player, aiActor, playerPosition, enemyPosition));
                        }
                    }
                }
            }
        }

        private IEnumerator Shambles(PlayerController player, AIActor aiActor, Vector2 playerPosition, Vector2 enemyPosition)
        {
            //yield return new WaitForSeconds(0.1f);
            Projectile Boomprojectile = ((Gun)ETGMod.Databases.Items[593]).DefaultModule.projectiles[0];
            ExplosiveModifier boomer = Boomprojectile.gameObject.GetComponent<ExplosiveModifier>();
            boomer.explosionData.damageToPlayer = 0f;
            boomer.explosionData.damage = 10f;
            boomer.explosionData.pushRadius = 4f;
            boomer.explosionData.doForce = true;
            boomer.explosionData.doDestroyProjectiles = true;
            boomer.explosionData.preventPlayerForce = true;
            this.DoMicroBlank(enemyPosition, 0f);
            yield return new WaitForSeconds(0.1f);
            aiActor.transform.position = playerPosition;
            player.transform.position = enemyPosition;
            aiActor.specRigidbody.Reinitialize();
            aiActor.specRigidbody.RecheckTriggers = true;
            Exploder.Explode(enemyPosition, boomer.explosionData, Vector2.zero, null, false, 0, false);
            player.specRigidbody.Reinitialize();
            player.specRigidbody.RecheckTriggers = true;
            yield break;
        }

        public override void PostProcessProjectile(Projectile projectile)
        {
            projectile.OnHitEnemy = (Action<Projectile, SpeculativeRigidbody, bool>)Delegate.Combine(projectile.OnHitEnemy, new Action<Projectile, SpeculativeRigidbody, bool>(this.OnProjectileHitEnemy));
        }

        private void DoMicroBlank(Vector2 center, float knockbackForce = 30f)
        {
            GameObject silencerVFX = (GameObject)ResourceCache.Acquire("Global VFX/BlankVFX_Ghost");
            AkSoundEngine.PostEvent("Play_OBJ_silenceblank_small_01", base.gameObject);
            GameObject gameObject = new GameObject("silencer");
            SilencerInstance silencerInstance = gameObject.AddComponent<SilencerInstance>();
            float additionalTimeAtMaxRadius = 0.25f;
            silencerInstance.ForceNoDamage = true;
            silencerInstance.TriggerSilencer(center, 20f, 5f, silencerVFX, 0f, 4f, 3f, 4f, knockbackForce, 4f, additionalTimeAtMaxRadius, this.gun.CurrentOwner as PlayerController, false, false);
        }

        protected override void OnPickup(PlayerController player)
        {
            base.OnPickup(player);
            this.GiveContactDamageImmunity(player); //to add contact damage immunity when the gun is held
            player.GunChanged += this.OnGunChanged;
        }

        protected override void OnPostDrop(PlayerController player)
        {
            player.GunChanged -= this.OnGunChanged;
            this.RemoveContactDamageImmunity(player);
            base.OnPostDrop(player);
        }

        private void OnGunChanged(Gun oldGun, Gun newGun, bool arg3)
        {
            LiveAmmoItem liveammo = PickupObjectDatabase.GetById(414).GetComponent<LiveAmmoItem>();

            if (this.gun && this.gun.CurrentOwner)
            {
                PlayerController player = this.gun.CurrentOwner as PlayerController;
                if (newGun == this.gun)
                {
                    this.GiveContactDamageImmunity(player);
                }
                else
                {
                    this.RemoveContactDamageImmunity(player);
                }
            }
        }

        private void GiveContactDamageImmunity(PlayerController player)
        {
            // code from retrash icy skull
            LiveAmmoItem liveammo = PickupObjectDatabase.GetById(414).GetComponent<LiveAmmoItem>();
            if (!PassiveItem.ActiveFlagItems.ContainsKey(player))
            {
                PassiveItem.ActiveFlagItems.Add(player, new Dictionary<Type, int>());
            }
            if (!PassiveItem.ActiveFlagItems[player].ContainsKey(liveammo.GetType()))
            {
                PassiveItem.ActiveFlagItems[player].Add(liveammo.GetType(), 1);
            }
            else
            {
                PassiveItem.ActiveFlagItems[player][liveammo.GetType()] = PassiveItem.ActiveFlagItems[player][liveammo.GetType()] + 1;
            }
        }

        private void RemoveContactDamageImmunity(PlayerController player)
        {
            LiveAmmoItem liveammo = PickupObjectDatabase.GetById(414).GetComponent<LiveAmmoItem>();
            if (PassiveItem.ActiveFlagItems[player].ContainsKey(liveammo.GetType()))
            {
                PassiveItem.ActiveFlagItems[player][liveammo.GetType()] = Mathf.Max(0, PassiveItem.ActiveFlagItems[player][liveammo.GetType()] - 1);
                if (PassiveItem.ActiveFlagItems[player][liveammo.GetType()] == 0)
                {
                    PassiveItem.ActiveFlagItems[player].Remove(liveammo.GetType());
                }
            }
        }

        public override void OnPostFired(PlayerController player, Gun gun)
        {
            //This determines what sound you want to play when you fire a gun.
            //Sounds names are based on the Gungeon sound dump, which can be found at EnterTheGungeon/Etg_Data/StreamingAssets/Audio/GeneratedSoundBanks/Windows/sfx.txt
            gun.PreventNormalFireAudio = true;
            AkSoundEngine.PostEvent("Play_WPN_smileyrevolver_shot_01", gameObject);
        }
        private bool HasReloaded;
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

        //All that's left now is sprite stuff. 
        //Your sprites should be organized, like how you see in the mod folder. 
        //Every gun requires that you have a .json to match the sprites or else the gun won't spawn at all
        //.Json determines the hand sprites for your character. You can make a gun two handed by having both "SecondaryHand" and "PrimaryHand" in the .json file, which can be edited through Notepad or Visual Studios
        //By default this gun is a one-handed weapon
        //If you need a basic two handed .json. Just use the jpxfrd2.json.
        //And finally, don't forget to add your Gun to your ETGModule class!

        public static string[] BannedEnemies = new string[]
        {
			EnemyGuidDatabase.Entries["grip_master"], //"22fc2c2c45fb47cf9fb5f7b043a70122",
			//EnemyGuidDatabase.Entries["spectre"], //"56f5a0f2c1fc4bc78875aea617ee31ac",
			EnemyGuidDatabase.Entries["draguns_knife"], //"78eca975263d4482a4bfa4c07b32e252",
			EnemyGuidDatabase.Entries["dragun_knife_advanced"], //"2e6223e42e574775b56c6349921f42cb",
			EnemyGuidDatabase.Entries["lord_of_the_jammed"], //"0d3f7c641557426fbac8596b61c9fb45",
			EnemyGuidDatabase.Entries["tutorial_turret"], //"e667fdd01f1e43349c03a18e5b79e579",
			EnemyGuidDatabase.Entries["faster_tutorial_turret"], //"41ba74c517534f02a62f2e2028395c58",
			EnemyGuidDatabase.Entries["candle_guy"], //"eeb33c3a5a8e4eaaaaf39a743e8767bc",
			EnemyGuidDatabase.Entries["diagonal_x_det"], //"c5a0fd2774b64287bf11127ca59dd8b4",
			EnemyGuidDatabase.Entries["vertical_det"], //"b67ffe82c66742d1985e5888fd8e6a03",
			EnemyGuidDatabase.Entries["diagonal_det"], //"d9632631a18849539333a92332895ebd",
			EnemyGuidDatabase.Entries["horizontal_det"], //"1898f6fe1ee0408e886aaf05c23cc216",
			EnemyGuidDatabase.Entries["vertical_x_det"], //"abd816b0bcbf4035b95837ca931169df",
			EnemyGuidDatabase.Entries["horizontal_x_det"], //"07d06d2b23cc48fe9f95454c839cb361",
			EnemyGuidDatabase.Entries["x_det"], //"48d74b9c65f44b888a94f9e093554977",
			EnemyGuidDatabase.Entries["rat"], //"6ad1cafc268f4214a101dca7af61bc91",
			EnemyGuidDatabase.Entries["rat_candle"], //"14ea47ff46b54bb4a98f91ffcffb656d",
			EnemyGuidDatabase.Entries["chicken"], //"76bc43539fc24648bff4568c75c686d1",
			EnemyGuidDatabase.Entries["red_caped_bullet_kin"], //"fa6a7ac20a0e4083a4c2ee0d86f8bbf7",
            EnemyGuidDatabase.Entries["lead_maiden"], // get stuck easily even in big 
            EnemyGuidDatabase.Entries["fridge_maiden"],
            EnemyGuidDatabase.Entries["misfire_beast"],
        };

        public static string[] BigEnemies = new string[]
        {
            EnemyGuidDatabase.Entries["tarnisher"], //"475c20c1fd474dfbad54954e7cba29c1",
			EnemyGuidDatabase.Entries["chain_gunner"], //"463d16121f884984abe759de38418e48", 
			EnemyGuidDatabase.Entries["titaness_bullet_kin_boss"], //"df4e9fedb8764b5a876517431ca67b86",
			EnemyGuidDatabase.Entries["titan_bullet_kin_boss"], //"1f290ea06a4c416cabc52d6b3cf47266",
			EnemyGuidDatabase.Entries["titan_bullet_kin"], //"c4cf0620f71c4678bb8d77929fd4feff",
			EnemyGuidDatabase.Entries["titan_bullet_kin"], //"c4cf0620f71c4678bb8d77929fd4feff",
			EnemyGuidDatabase.Entries["spectral_gun_nut"], 
			EnemyGuidDatabase.Entries["gun_nut"], 
			EnemyGuidDatabase.Entries["wall_mimic"], 
			EnemyGuidDatabase.Entries["spogre"], 
			EnemyGuidDatabase.Entries["ammoconda_ball"], 
			EnemyGuidDatabase.Entries["shambling_round"], 
			EnemyGuidDatabase.Entries["agonizer"], 
			EnemyGuidDatabase.Entries["revolvenant"], 
			EnemyGuidDatabase.Entries["shelleton"], 
			EnemyGuidDatabase.Entries["rat_chest_mimic"], 
        };

    }

    public class BulletStunModifier : MonoBehaviour
    {
        // Token: 0x06000355 RID: 853 RVA: 0x00020685 File Offset: 0x0001E885
        public BulletStunModifier()
        {
            this.chanceToStun = 0f;
            this.stunLength = 1f;
            this.doVFX = true;
        }

        // Token: 0x06000356 RID: 854 RVA: 0x000206AC File Offset: 0x0001E8AC
        private void Start()
        {
            this.m_projectile = base.GetComponent<Projectile>();
            bool flag = Random.value <= this.chanceToStun;
            if (flag)
            {
                Projectile projectile = this.m_projectile;
                projectile.OnHitEnemy = (Action<Projectile, SpeculativeRigidbody, bool>)Delegate.Combine(projectile.OnHitEnemy, new Action<Projectile, SpeculativeRigidbody, bool>(this.ApplyStun));
            }
        }

        // Token: 0x06000357 RID: 855 RVA: 0x00020703 File Offset: 0x0001E903
        private void ApplyStun(Projectile bullet, SpeculativeRigidbody enemy, bool fatal)
        {
            enemy.behaviorSpeculator.Stun(this.stunLength, this.doVFX);
        }

        // Token: 0x0400013C RID: 316
        private Projectile m_projectile;

        // Token: 0x0400013D RID: 317
        public float chanceToStun;

        // Token: 0x0400013E RID: 318
        public bool doVFX;

        // Token: 0x0400013F RID: 319
        public float stunLength;
    }
}

