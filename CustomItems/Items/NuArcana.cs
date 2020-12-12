using ItemAPI;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Object = UnityEngine.Object;
using Random = UnityEngine.Random;

namespace GlaurungItems.Items
{
	class NuArcana : PassiveItem
	{
		public static void Init()
		{
			string name = "Nu Arcana";
			string resourcePath = "GlaurungItems/Resources/acme_crate";
			GameObject gameObject = new GameObject(name);
			NuArcana item = gameObject.AddComponent<NuArcana>();
			ItemBuilder.AddSpriteToObject(name, resourcePath, gameObject);
			string shortDesc = "Vade Retro, Monster !";
			string longDesc = "Theses bullets have a chance to banish targets directly to Bullet Hell, unless, well, they are already in it...";
			item.SetupItem(shortDesc, longDesc, "gl");
			item.quality = PickupObject.ItemQuality.B;
			NuArcana.ID = item.PickupObjectId;
		}

		public override void Pickup(PlayerController player)
		{
			base.Pickup(player);
			GameManager.Instance.OnNewLevelFullyLoaded += this.RechargePerFloor;
			player.PostProcessProjectile += this.PostProcessProjectile;
			player.PostProcessBeamTick += this.PostProcessBeamTick;
			player.OnRoomClearEvent += this.OnLeaveCombat;
		}

		public override DebrisObject Drop(PlayerController player)
		{
			GameManager.Instance.OnNewLevelFullyLoaded -= this.RechargePerFloor;
			player.PostProcessProjectile -= this.PostProcessProjectile;
			player.PostProcessBeamTick -= this.PostProcessBeamTick;
			player.OnRoomClearEvent -= this.OnLeaveCombat;
			return base.Drop(player);
		}

		protected override void OnDestroy()
		{
			GameManager.Instance.OnNewLevelFullyLoaded -= this.RechargePerFloor;
			base.Owner.PostProcessProjectile -= this.PostProcessProjectile;
			base.Owner.PostProcessBeamTick -= this.PostProcessBeamTick;
			base.Owner.OnRoomClearEvent -= this.OnLeaveCombat;
			base.OnDestroy();
		}

		private void OnLeaveCombat(PlayerController user)
		{
			targetForBanishing = new List<AIActor>();
		}

		private void PostProcessProjectile(Projectile projectile, float Chance)
		{
			PlayerController owner = base.Owner;
			projectile.OnHitEnemy = (Action<Projectile, SpeculativeRigidbody, bool>)Delegate.Combine(projectile.OnHitEnemy, new Action<Projectile, SpeculativeRigidbody, bool>(this.OnProjectileHitEnemy));

		}

		private void PostProcessBeamTick(BeamController beam, SpeculativeRigidbody hitRigidBody, float tickrate)
		{
			float procChance = 0.3f; //This is your proc-chance, 
			AIActor aiactor = hitRigidBody.aiActor;
			if (!aiactor)
			{
				return;
			}
			bool stillAlive = aiactor.healthHaver && aiactor.healthHaver.IsAlive;
			bool canTrigger = UnityEngine.Random.value < BraveMathCollege.SliceProbability(procChance, tickrate);
			if (stillAlive && canTrigger)
			{
				CheckIfCanBeBanished(aiactor, stillAlive);
			}
		}

		private void RechargePerFloor()
		{
			if (GameManager.Instance.Dungeon.DungeonFloorName == "#BULLETHELL_SHORTNAME")
			{
				this.AddPassiveStatModifier(PlayerStats.StatType.Damage, 0.33f, StatModifier.ModifyMethod.ADDITIVE);
				base.Owner.stats.RecalculateStats(base.Owner, false);
			}
		}

		private void OnProjectileHitEnemy(Projectile proj, SpeculativeRigidbody enemy, bool fatal)
		{
			if (enemy != null)
			{
				AIActor aiActor = enemy.aiActor;
				//Tools.Print(aiActor.EnemyGuid, "ffffff", true);
				CheckIfCanBeBanished(aiActor, !fatal);
			}
		}

		private void CheckIfCanBeBanished(AIActor aiActor, bool stillAlive)
		{
			PlayerController owner = this.Owner;
			float randomF = Random.Range(0f, 1f);
			if (aiActor != null && stillAlive && randomF < 0.01f && !aiActor.healthHaver.IsBoss && GameManager.Instance.Dungeon.DungeonFloorName != "#BULLETHELL_SHORTNAME" && !targetForBanishing.Contains(aiActor))
			{
				//Tools.Print(GameManager.Instance.Dungeon.DungeonFloorName != "#BULLETHELL_SHORTNAME", "FFFFFF", true);
				targetForBanishing.Add(aiActor);
				aiActor.healthHaver.TriggerInvulnerabilityPeriod(0.6f);
				base.StartCoroutine(this.BanishEnemy(aiActor));
			}
		}

		private IEnumerator BanishEnemy(AIActor enemy)
		{
			//Tools.Print(enemy.transform.position, "FFFFFF", true);
			//Tools.Print(enemy.specRigidbody.UnitBottomCenter, "FFFFFF", true);

			float xOffset = enemy.specRigidbody.UnitBottomCenter.x - enemy.transform.position.x;
			float yOffset = enemy.specRigidbody.UnitBottomCenter.y - enemy.transform.position.y;

			GameObject hellPortal = Object.Instantiate<GameObject>(this.hellSynergyVFX, enemy.transform.localPosition + new Vector3(xOffset, yOffset, 0f), Quaternion.Euler(45f, 0f, 0f), enemy.transform);
			//Tools.Print(hellPortal.layer, "ffffff", true);
			//Tools.Print(enemy.gameObject.layer, "ffffff", true);

			MeshRenderer component = gameObject.GetComponent<MeshRenderer>();
			base.StartCoroutine(this.HoldPortalOpen(component));
			yield return new WaitForSeconds(0.6f);
			enemy.ForceFall();
			targetForBanishing.Remove(enemy);
			yield break;
		}

		// from BlackHoleDoer
		private IEnumerator HoldPortalOpen(MeshRenderer portal)
		{
			portal.material.SetFloat("_UVDistCutoff", 0f);
			yield return new WaitForSeconds(this.introDuration);
			float elapsed = 0f;
			float duration = this.coreDuration;
			float t = 0f;
			while (elapsed < duration)
			{
				elapsed += BraveTime.DeltaTime;
				t = Mathf.Clamp01(elapsed / 0.25f);
				portal.material.SetFloat("_UVDistCutoff", Mathf.Lerp(0f, 0.21f, t));
				yield return null;
			}
			yield break;
		}

		private GameObject hellSynergyVFX = PickupObjectDatabase.GetById(155).GetComponent<SpawnObjectPlayerItem>().objectToSpawn.GetComponent<BlackHoleDoer>().HellSynergyVFX;
		public static int ID;
		private float introDuration = 0.1f;
		private float coreDuration = 0.5f;
		private static List<AIActor> targetForBanishing = new List<AIActor>();
	}
}
