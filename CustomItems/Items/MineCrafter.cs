using Dungeonator;
using ItemAPI;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace GlaurungItems.Items
{
    class MineCrafter : PlayerItem
	{
		public static void Init()
		{
			string text = "Mine Crafter";
			string resourcePath = "GlaurungItems/Resources/mine_crafter";
			GameObject gameObject = new GameObject(text);
			MineCrafter item = gameObject.AddComponent<MineCrafter>();
			ItemBuilder.AddSpriteToObject(text, resourcePath, gameObject);
			string shortDesc = "Do a Barrel Roll !";
			string longDesc = "Spawns a rolling tnt barrel which can be remotely detonated. \n \n Created by a fan of explosives who wanted his C4 to move after being deployed.";
			item.SetupItem(shortDesc, longDesc, "gl");
			item.SetCooldownType(ItemBuilder.CooldownType.Damage, cooldown);
			item.quality = ItemQuality.C;
		}

		protected override void DoEffect(PlayerController user)
		{
			base.DoEffect(user);
            if (!barrelSpawned)
            {
				barrelSpawned = true;
				if (user.CurrentRoom != null)
				{
					float roomPosX = user.transform.position.x - user.CurrentRoom.area.basePosition.x;
					float roomPosY = user.transform.position.y - user.CurrentRoom.area.basePosition.y;
					float xOffSet = 0;
					float yOffSet = 0;
					float offsetAmount = 2f;
					float gunCurrentAngle = BraveMathCollege.Atan2Degrees(this.LastOwner.unadjustedAimPoint.XY() - this.LastOwner.CenterPosition);
					if (gunCurrentAngle > 45f && gunCurrentAngle <= 135f)
					{
						yOffSet = offsetAmount;//up
					}
					else if ((gunCurrentAngle > 0 && gunCurrentAngle > 135f) || (gunCurrentAngle < 0 && gunCurrentAngle <= -135f))
					{
						xOffSet = -offsetAmount;//left
					}
					else if (gunCurrentAngle > -135f && gunCurrentAngle <= -45f)
					{
						yOffSet = -offsetAmount;//bottom
					}
					else
					{
						xOffSet = offsetAmount;//right
					}
					Vector2 posInCurrentRoom = new Vector2(roomPosX + xOffSet, roomPosY + yOffSet);
					Vector2 posInMap = new Vector2(user.transform.position.x + xOffSet, user.transform.position.y + yOffSet).ToIntVector2().ToVector2();
					if (user.IsValidPlayerPosition(posInMap))
					{
						AssetBundle sharedAssets2 = ResourceManager.LoadAssetBundle("shared_auto_002");
						DungeonPlaceable ExplodyBarrel = sharedAssets2.LoadAsset<DungeonPlaceable>("ExplodyBarrel_Maybe");
						GameObject spawnedDrum = ExplodyBarrel.InstantiateObject(user.CurrentRoom, posInCurrentRoom.ToIntVector2());
					
						MinorBreakable minorBreakComponentInChildren = spawnedDrum.GetComponentInChildren<MinorBreakable>();
						if (minorBreakComponentInChildren)
						{
							ExplosionData defaultExplosionData = GameManager.Instance.Dungeon.sharedSettingsPrefab.DefaultExplosionData;
							this.playerFriendlyExplosion.effect = defaultExplosionData.effect;
							this.playerFriendlyExplosion.ignoreList = defaultExplosionData.ignoreList;
							this.playerFriendlyExplosion.ss = defaultExplosionData.ss;
							minorBreakComponentInChildren.explosionData = playerFriendlyExplosion;
						}

						KickableObject kickableComponentInChildren = spawnedDrum.GetComponentInChildren<KickableObject>();
						if (kickableComponentInChildren)
						{
							kickableComponentInChildren.specRigidbody.Reinitialize();
							kickableComponentInChildren.rollSpeed = 5f;
							user.CurrentRoom.RegisterInteractable(kickableComponentInChildren);
							kickableComponentInChildren.Interact(user);
						}
						this.barrel = spawnedDrum;
					}
                    else
                    {
						barrelSpawned = false;
						base.StartCoroutine(RefillOnWrongSpawn());
					}
				}
			}
            else
            {
				barrelSpawned = false;
				if(barrel != null)
                {
					MinorBreakable minorBreakComponentInChildren = barrel.GetComponentInChildren<MinorBreakable>();
					if (minorBreakComponentInChildren)
					{
						minorBreakComponentInChildren.Break();
					}
				}
            }
		}

        private IEnumerator RefillOnWrongSpawn()
        {
			yield return new WaitForSeconds(0.1f);
			ClearCooldowns();
			yield break;
		}

		public override bool CanBeUsed(PlayerController user)
		{
			return user.CurrentRoom != null;
		}

		protected override void AfterCooldownApplied(PlayerController user)
		{
			if (barrelSpawned)
			{
				ClearCooldowns();
			}
			this.CurrentDamageCooldown = Mathf.Min(CurrentDamageCooldown, cooldown);
		}

        public override void Update()
        {
            base.Update();
            if (this.LastOwner && barrelSpawned && barrel.GetComponentInChildren<MinorBreakable>() && barrel.GetComponentInChildren<MinorBreakable>().IsBroken)
            {
				barrelSpawned = false;
				this.ForceApplyCooldown(this.LastOwner);

			}
        }

		[SerializeField]
		private bool barrelSpawned;
		private static float cooldown = 200f;
		[SerializeField]
		private GameObject barrel = null;
		private ExplosionData playerFriendlyExplosion = new ExplosionData
		{
			damageRadius = 4f,
			doDamage = true,
			damageToPlayer = 0f,
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
	}
}
