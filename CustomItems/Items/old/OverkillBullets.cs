using ItemAPI;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Object = UnityEngine.Object;
using Random = UnityEngine.Random;

namespace GlaurungItems.Items
{
    class OverkillBullets : PassiveItem
    {
		public static void Init()
		{
			string name = "Overkill Bullets";
			string resourcePath = "GlaurungItems/Resources/banishing_bullets";
			GameObject gameObject = new GameObject(name);
				OverkillBullets item = gameObject.AddComponent<OverkillBullets>();
			ItemBuilder.AddSpriteToObject(name, resourcePath, gameObject);
			string shortDesc = "No kill like";
			string longDesc = "";
			item.SetupItem(shortDesc, longDesc, "gl");
			item.quality = PickupObject.ItemQuality.S;
			BanishingBullets.ID = item.PickupObjectId;
		}

		public override void Pickup(PlayerController player)
		{
			base.Pickup(player);
			player.PostProcessProjectile += this.PostProcessProjectile;
			//player.PostProcessBeamTick += this.PostProcessBeamTick;
		}

			public override DebrisObject Drop(PlayerController player)
		{
			player.PostProcessProjectile -= this.PostProcessProjectile;
			return base.Drop(player);
		}

		protected override void OnDestroy()
		{
			base.Owner.PostProcessProjectile -= this.PostProcessProjectile;
			base.OnDestroy();
		}

		private void PostProcessProjectile(Projectile projectile, float Chance)
		{
			PlayerController owner = base.Owner;
			projectile.baseData.damage += excessDmg;
			excessDmg = 0;
			projectile.specRigidbody.OnPreRigidbodyCollision += this.OnPreRigidCollision;
		}

        private void OnPreRigidCollision(SpeculativeRigidbody myRigidbody, PixelCollider myPixelCollider, SpeculativeRigidbody otherRigidbody, PixelCollider otherPixelCollider)
        {
			if(otherRigidbody.aiActor != null && myRigidbody.projectile != null)
            {
				AIActor actor = otherRigidbody.aiActor;
				if(actor.healthHaver && actor.healthHaver.IsAlive && 
					myRigidbody.projectile.ModifiedDamage > actor.healthHaver.GetCurrentHealth())
                {
					excessDmg = myRigidbody.projectile.ModifiedDamage - actor.healthHaver.GetCurrentHealth();

				}
			}
        }

		private float excessDmg;
	}
}
