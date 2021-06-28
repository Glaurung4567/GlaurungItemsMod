using Dungeonator;
using Gungeon;
using ItemAPI;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace GlaurungItems.Items
{
	class TopsyTurvyBomb : PlayerItem
	{
		public static void Init()
		{
			string text = "Topsy-Turvy Gun";
			string resourcePath = "GlaurungItems/Resources/corkscrewgun";
			GameObject gameObject = new GameObject(text);
			TopsyTurvyBomb item = gameObject.AddComponent<TopsyTurvyBomb>();
			ItemBuilder.AddSpriteToObject(text, resourcePath, gameObject);
			string shortDesc = "Up is Down";
			string longDesc = "A strange corkscrew shape'd gun which allow the user to screw with gravity.";
			item.SetupItem(shortDesc, longDesc, "gl");
			item.SetCooldownType(ItemBuilder.CooldownType.Timed, 1f);
			ItemBuilder.AddPassiveStatModifier(item, PlayerStats.StatType.AdditionalItemCapacity, 1, StatModifier.ModifyMethod.ADDITIVE);
			item.quality = ItemQuality.C;
		}

		public override bool CanBeUsed(PlayerController user)
		{
			return user.CurrentRoom != null && user.IsInCombat;
		}

		protected override void DoEffect(PlayerController user)
		{
			List<AIActor> activeEnemies = GameManager.Instance.Dungeon.data.GetAbsoluteRoomFromPosition(user.CenterPosition.ToIntVector2(VectorConversions.Round)).GetActiveEnemies(RoomHandler.ActiveEnemyType.All);

			for (int j = 0; j < activeEnemies.Count; j++)
			{
				AIActor actor = activeEnemies[j];
                if (actor && actor.behaviorSpeculator)
                {
					actor.behaviorSpeculator.Stun(3, false);
                }
				if (actor && actor.healthHaver && actor.healthHaver.IsAlive && !actor.healthHaver.IsBoss && !actor.IsFlying)
				{
					GameManager.Instance.StartCoroutine(HandleUpAndSlam(actor, user));
				}
			}
		}

        private IEnumerator HandleUpAndSlam(AIActor actor, PlayerController user)
        {
            if (!reverseBool)
            {
				user.SetInputOverride("topsy");
				user.CurrentInputState = PlayerInputState.NoInput;
				GameManager.Instance.MainCameraController.Camera.transform.Rotate(0f, 0f, 180f);
				Pixelator.Instance.DoOcclusionLayer = false;
				user.transform.transform.Rotate(0f, 0f, 180f);
				GameUIRoot.Instance.ForceHideGunPanel = true;
				user.inventory.GunLocked.SetOverride("topsy", true);

				reverseBool = true;
			}

			for (int i = 0; i < nbOfSteps; i++)
            {
				Time.timeScale = 1;

				//actor.sprite.HeightOffGround += heightByStep;
				actor.transform.position = actor.transform.position + new Vector3(0f, yByStep, 0f);
				if (actor.ShadowObject)
				{
					actor.ShadowObject.transform.position = actor.ShadowObject.transform.position + new Vector3(0f, -yByStep, 0f);
				}
				actor.sprite.UpdateZDepth();
				yield return new WaitForSeconds(waitBetweenEachStepUp);
			}

            if (!unreverseBool)
            {
				user.CurrentInputState = PlayerInputState.AllInput;
				user.ClearInputOverride("topsy");
				GameManager.Instance.MainCameraController.Camera.transform.Rotate(0f, 0f, -180f);
				Pixelator.Instance.DoOcclusionLayer = true;
				user.transform.transform.Rotate(0f, 0f, -180f);
				GameUIRoot.Instance.ForceHideGunPanel = false;
				user.inventory.GunLocked.SetOverride("topsy", false);

				unreverseBool = true;
			}

			for (int i = 0; i < nbOfSteps; i++)
			{
				//actor.sprite.HeightOffGround -= .2f;
				actor.transform.position = actor.transform.position + new Vector3(0f, -yByStep, 0f);
				if (actor.ShadowObject)
				{
					actor.ShadowObject.transform.position = actor.ShadowObject.transform.position + new Vector3(0f, yByStep, 0f);
				}
				actor.sprite.UpdateZDepth();
				yield return new WaitForSeconds(waitBetweenEachStepDown);
			}

			if(actor.healthHaver && actor.healthHaver.IsAlive)
            {
				/*
				Projectile proj = SpawnImpactProj(actor);

				yield return null;
				if (proj)
				{
					//proj.DieInAir(false, false, false);
					Tools.Print("inpact", "ffffff", true);
					Tools.Print(actor.transform.position, "ffffff", true);
					Tools.Print(proj.transform.position, "ffffff", true);
					Tools.Print(Time.timeScale, "ffffff", true);

				}
				*/
				actor.healthHaver.ApplyDamage(actor.healthHaver.GetMaxHealth(), Vector2.zero, "Topsy", CoreDamageTypes.None, DamageCategory.Normal, false, null, false);
			}

			reverseBool = false;
			unreverseBool = false;

			yield break;
		}

		private Projectile SpawnImpactProj(AIActor actor)
		{
			Projectile projectile2 = ((Gun)global::ETGMod.Databases.Items[541]).DefaultModule.chargeProjectiles[0].Projectile;
			GameObject gameObject = SpawnManager.SpawnProjectile(projectile2.gameObject, actor.specRigidbody.UnitCenter, Quaternion.Euler(0f, 0f, 0f), true);
			Projectile component = gameObject.GetComponent<Projectile>();
			bool flag8 = component != null;
			if (flag8)
			{
				component.AdditionalScaleMultiplier = 3.25f;
				component.baseData.damage = 0;
				component.baseData.force = 0f;
				component.collidesWithEnemies = true;
				component.baseData.speed = 0f;
				component.Owner = GameManager.Instance.PrimaryPlayer;
				component.Shooter = GameManager.Instance.PrimaryPlayer.specRigidbody;
				GameManager.Instance.StartCoroutine(DelProj(component));
			}
			return component;
		}

		private IEnumerator DelProj(Projectile component)
		{
			yield return new WaitForSeconds(0.5f);
			if (component)
			{
				component.DieInAir(true);
			}
		}

		private const float nbOfSteps = 25; 
		private const float yByStep = 0.15f; 
		private const float heightByStep = 0.33f; 
		private const float waitBetweenEachStepUp = 0.025f;
		private const float waitBetweenEachStepDown = 0.0005f;
		private bool reverseBool = false;
		private bool unreverseBool = false;
	}

    class TopsyGun : AdvancedGunBehavior
    {
        public static void Add()
        {
            Gun gun = ETGMod.Databases.Items.NewGun("Corkscrew Fake Gun", "corkscrewgun");
            Game.Items.Rename("outdated_gun_mods:corkscrew_fake_gun", "gl:corkscrew_fake_gun");
            gun.gameObject.AddComponent<TopsyGun>();

            gun.SetShortDescription("[TODO]");
            gun.SetLongDescription("[TODO]");

            gun.SetupSprite(null, "corkscrewgun_idle_001", 8);
            gun.SetAnimationFPS(gun.shootAnimation, 24);
			gun.AddProjectileModuleFrom("ak-47", true, false);
            gun.DefaultModule.ammoCost = 1;
            gun.DefaultModule.shootStyle = ProjectileModule.ShootStyle.SemiAutomatic;
            gun.DefaultModule.sequenceStyle = ProjectileModule.ProjectileSequenceStyle.Random;
            gun.reloadTime = 0f;
            gun.DefaultModule.cooldownTime = 88888.1f;
            gun.DefaultModule.numberOfShotsInClip = 10;
            gun.SetBaseMaxAmmo(300);
            // Here we just set the quality of the gun and the "EncounterGuid", which is used by Gungeon to identify the gun.
            gun.quality = PickupObject.ItemQuality.EXCLUDED;
			
			gun.preventRotation = true;
			gun.carryPixelOffset = new IntVector2(0, 14);

			Projectile projectile = UnityEngine.Object.Instantiate<Projectile>(gun.DefaultModule.projectiles[0]);
            projectile.gameObject.SetActive(false);
            FakePrefab.MarkAsFakePrefab(projectile.gameObject);
            UnityEngine.Object.DontDestroyOnLoad(projectile);
            gun.DefaultModule.projectiles[0] = projectile;

            projectile.baseData.damage = 0f;
            projectile.baseData.speed = 0.000001f;
            projectile.transform.parent = gun.barrelOffset;

			gun.CeaseAttack(false);

            ETGMod.Databases.Items.Add(gun, null, "ANY");
        }

        public override void OnPostFired(PlayerController player, Gun gun)
        {
            //This determines what sound you want to play when you fire a gun.
            //Sounds names are based on the Gungeon sound dump, which can be found at EnterTheGungeon/Etg_Data/StreamingAssets/Audio/GeneratedSoundBanks/Windows/sfx.txt
            gun.PreventNormalFireAudio = true;
            AkSoundEngine.PostEvent("Play_WPN_smileyrevolver_shot_01", gameObject);
        }

    }
}
