using ItemAPI;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GlaurungItems.Items
{
	class Poyo : PlayerItem
	{
		public static void Init()
		{
			string text = "Poyo";
			string resourcePath = "GlaurungItems/Resources/neuralyzer";
			GameObject gameObject = new GameObject(text);
			Poyo item = gameObject.AddComponent<Poyo>();
			ItemBuilder.AddSpriteToObject(text, resourcePath, gameObject);
			string shortDesc = "Nom nom nom";
			string longDesc = "Poyo poYo POyo PoYo";
			item.SetupItem(shortDesc, longDesc, "gl");
			item.SetCooldownType(ItemBuilder.CooldownType.Damage, 1);
			item.quality = ItemQuality.S;
		}

		protected override void DoEffect(PlayerController user)
		{
			float nearestEnemyPosition;
			AIActor nomTarget = user.CurrentRoom.GetNearestEnemy(user.CenterPosition, out nearestEnemyPosition, true, true);
			string nomTargetUuid = nomTarget.EnemyGuid;


			RemovePowerUp();
			PickupObject powerUp = null;

			if (scattershots.Contains(nomTargetUuid))
            {
				powerUp = PickupObjectDatabase.GetById(241);

			}
			if(powerUp != null)
            {
				EncounterTrackable.SuppressNextNotification = true;
				powerUpInstance = Instantiate(powerUp.gameObject).GetComponent<PickupObject>();
				powerUpInstance.CanBeDropped = false;
				powerUpInstance.CanBeSold = false;
				LootEngine.TryGivePrefabToPlayer(powerUpInstance.gameObject, user, true);
				
				EncounterTrackable.SuppressNextNotification = false;
			}

			DEVOUR(nomTarget);


		}

		private void RemovePowerUp()
        {
            if (this.LastOwner && powerUpInstance && this.LastOwner.passiveItems != null)
            {
				//this.LastOwner.RemovePassiveItem(powerUpInstance.PickupObjectId);
				for(int i = 0; i<this.LastOwner.passiveItems.Count; i++)
                {
					if(this.LastOwner.passiveItems[i].PickupObjectId == powerUpInstance.PickupObjectId && this.LastOwner.passiveItems[i].CanBeDropped == false)
                    {
						Destroy(this.LastOwner.passiveItems[i]);
						powerUpInstance = null;
						return;
                    }
                }
			}
		}

		public override bool CanBeUsed(PlayerController user)
		{
			if (user.CurrentRoom != null && user.IsInCombat)
			{
				float nearestEnemyPosition;
				AIActor nomTarget = user.CurrentRoom.GetNearestEnemy(user.CenterPosition, out nearestEnemyPosition, true, true);
				if (nomTarget != null && nearestEnemyPosition < 3.5f && nomTarget.healthHaver
					&& !nomTarget.healthHaver.IsBoss && nomTarget.healthHaver.IsAlive) return true;
			}
			return false;
		}

        public override void Pickup(PlayerController player)
        {
            base.Pickup(player);
            player.OnReceivedDamage += Player_OnReceivedDamage;
        }

        private void Player_OnReceivedDamage(PlayerController player)
        {
			RemovePowerUp();
		}

		protected override void OnPreDrop(PlayerController player)
		{
			RemovePowerUp();
			player.OnReceivedDamage -= Player_OnReceivedDamage;
			base.OnPreDrop(player);
		}

		private void DEVOUR(AIActor target)
		{
			if (target != null && !target.healthHaver.IsBoss)
			{
				GameManager.Instance.Dungeon.StartCoroutine(this.HandleEnemySuck(target));
				target.EraseFromExistence(true);
			}
		}
		private IEnumerator HandleEnemySuck(AIActor target)
		{
			PlayerController playerController = this.LastOwner;
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

		private PickupObject powerUpInstance = null;
		private static List<string> scattershots = new List<string> {
			EnemyGuidDatabase.Entries["red_shotgun_kin"]
		};

	}
}
