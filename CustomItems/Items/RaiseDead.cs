using ItemAPI;
using System;
using EnemyAPI;
using Random = UnityEngine.Random;
using UnityEngine;
using System.Collections;

namespace GlaurungItems.Items
{
    class RaiseDead: PlayerItem
    {
		public static void Init()
		{
			string text = "Raise Dead";
			string resourcePath = "GlaurungItems/Resources/resurrectdead";
			GameObject gameObject = new GameObject(text);
			RaiseDead raiseDead = gameObject.AddComponent<RaiseDead>();
			ItemBuilder.AddSpriteToObject(text, resourcePath, gameObject);
			string shortDesc = "Arise, minions !";
			string longDesc = "This spell animates some of the corpses of dead bullet kin buried in the ground of the gungeon and make them crave for the flesh of their kin." +
				"Be cautious though, if none of them are in sight you might become their next meal...";
			raiseDead.SetupItem(shortDesc, longDesc, "gl");
			raiseDead.SetCooldownType(ItemBuilder.CooldownType.Damage, 600f);
			raiseDead.quality = ItemQuality.B;
		}

		protected override void DoEffect(PlayerController user)
		{
            bool isInRoom = user && user.CurrentRoom != null;
            string spentEnemyGuid = EnemyGuidDatabase.Entries["spent"];
            int additionalSpent = Random.Range(1, 4);
            int totalOfSpentSpawned = this.numberOfSpentSummoned + additionalSpent;
            if (isInRoom && user.CurrentRoom.GetActiveEnemies(0) != null)
			{
                for(int i=0; i< totalOfSpentSpawned; i++)
                {
                    this.SpawnUndeadCompanion(user, spentEnemyGuid, 10f, true);
                }
                /*if(user.PlayerHasActiveSynergy("Restless Spirit"))
                {
                    this.SpawnUndeadCompanion(user, EnemyGuidDatabase.Entries["hollowpoint"], 15f);
                }
                if (user.PlayerHasActiveSynergy("Skull Bros"))
                {
                    this.SpawnUndeadCompanion(user, EnemyGuidDatabase.Entries["skusket"], 8f);
                }*/
            }
            else if(isInRoom && !user.IsInCombat)
            {
                for (int i = 0; i < totalOfSpentSpawned; i++)
                {
                    IntVector2? intVector = new IntVector2?(user.CurrentRoom.GetRandomVisibleClearSpot(2, 2));
                    AIActor orLoadByGuid = EnemyDatabase.GetOrLoadByGuid(spentEnemyGuid);
                    AIActor aiactor = AIActor.Spawn(orLoadByGuid.aiActor, intVector.Value, GameManager.Instance.Dungeon.data.GetAbsoluteRoomFromPosition(intVector.Value), true, AIActor.AwakenAnimationType.Default, true);
                    if (aiactor.gameObject.GetComponent<SpawnEnemyOnDeath>()) // to deal with spent spawning other spent on death
                    {
                        Destroy(aiactor.gameObject.GetComponent<SpawnEnemyOnDeath>());
                    }
                }
            }
        }

        private void SpawnUndeadCompanion(PlayerController owner, string enemyGuid, float maxHealth, bool dealsContactDmg = false)
        {
            try
            {
                AIActor orLoadByGuid = EnemyDatabase.GetOrLoadByGuid(enemyGuid);

                IntVector2? intVector = new IntVector2?(owner.CurrentRoom.GetRandomVisibleClearSpot(2, 2));
                //IntVector2? intVector = new IntVector2?(owner.CenterPosition.ToIntVector2());
                AIActor aiactor = AIActor.Spawn(orLoadByGuid.aiActor, intVector.Value, GameManager.Instance.Dungeon.data.GetAbsoluteRoomFromPosition(intVector.Value), true, AIActor.AwakenAnimationType.Default, true);

                aiactor.CanTargetEnemies = true;
                aiactor.CanTargetPlayers = false;
                aiactor.CompanionOwner = owner;
                aiactor.HitByEnemyBullets = true;
                aiactor.healthHaver.SetHealthMaximum(maxHealth);
                PhysicsEngine.Instance.RegisterOverlappingGhostCollisionExceptions(aiactor.specRigidbody, null, false);
                aiactor.gameObject.AddComponent<KillOnRoomClear>();
                aiactor.IsHarmlessEnemy = true;
                aiactor.IgnoreForRoomClear = true;
                aiactor.HandleReinforcementFallIntoRoom(0f);
                aiactor.gameObject.AddComponent<CompanionController>();
                CompanionController component = aiactor.gameObject.GetComponent<CompanionController>();
                component.CanInterceptBullets = true;
                component.Initialize(owner);
                if (dealsContactDmg)
                {
                    aiactor.OverrideHitEnemies = true;
                    aiactor.CollisionDamage = 1f;
                    aiactor.CollisionDamageTypes = (CoreDamageTypes)64;
                }
                if (aiactor.gameObject.GetComponent<SpawnEnemyOnDeath>()) // to deal with spent spawning other spent on death
                {
                    Destroy(aiactor.gameObject.GetComponent<SpawnEnemyOnDeath>());
                }
                /*if (aiactor.bulletBank != null)
                {
                    AIBulletBank bulletBank = aiactor.bulletBank;
                    bulletBank.OnProjectileCreated = (Action<Projectile>)Delegate.Combine(bulletBank.OnProjectileCreated, new Action<Projectile>(RaiseDead.OnPostProcessProjectile));
                }
                if (aiactor.aiShooter != null)
                {
                    AIShooter aiShooter = aiactor.aiShooter;
                    aiShooter.PostProcessProjectile = (Action<Projectile>)Delegate.Combine(aiShooter.PostProcessProjectile, new Action<Projectile>(RaiseDead.OnPostProcessProjectile));
                }*/

                
                /*CompanionisedEnemyBulletModifiers companionisedBullets = aiactor.gameObject.GetOrAddComponent<CompanionisedEnemyBulletModifiers>();
                companionisedBullets.jammedDamageMultiplier = 2f;
                companionisedBullets.TintBullets = false;
                companionisedBullets.baseBulletDamage = 10f;*/
                
            }
            catch (Exception e)
            {
                Tools.PrintException(e);
            }
        }

        private static void OnPostProcessProjectile(Projectile proj)
        {
            proj.specRigidbody.OnPreRigidbodyCollision = (SpeculativeRigidbody.OnPreRigidbodyCollisionDelegate)Delegate.Combine(proj.specRigidbody.OnPreRigidbodyCollision, new SpeculativeRigidbody.OnPreRigidbodyCollisionDelegate(RaiseDead.HandlePreCollision));
            proj.TreatedAsNonProjectileForChallenge = true;
            proj.collidesWithPlayer = false;
            proj.UpdateCollisionMask();
        }

        // from https://github.com/Nevernamed22/OnceMoreIntoTheBreach/blob/master/MakingAnItem/PromethianBullets.cs
        private static void HandlePreCollision(SpeculativeRigidbody myRigidbody, PixelCollider myPixelCollider, SpeculativeRigidbody otherRigidbody, PixelCollider otherPixelCollider)
        {
            bool flag = otherRigidbody && otherRigidbody.healthHaver && otherRigidbody.aiActor && otherRigidbody.aiActor.CompanionOwner;
            if (flag)
            {
                float damage = myRigidbody.projectile.baseData.damage;
                myRigidbody.projectile.baseData.damage = 0f;
                GameManager.Instance.StartCoroutine(RaiseDead.ChangeProjectileDamage(myRigidbody.projectile, damage));
            }
        }

        private static IEnumerator ChangeProjectileDamage(Projectile bullet, float oldDamage)
        {
            yield return new WaitForSeconds(0.1f);
            bool flag = bullet != null;
            if (flag)
            {
                bullet.baseData.damage = oldDamage;
            }
            yield break;
        }

        private int numberOfSpentSummoned = 1;
    }

}
