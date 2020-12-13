using System.Collections.Generic;
using UnityEngine;
using ItemAPI;
using Dungeonator;
using Random = UnityEngine.Random;
using System.Collections;
using EnemyAPI;

namespace GlaurungItems.Items
{
    class MatriochkAmmolet : SpecialBlankModificationItem
    {
        public static void Init()
        {
            string itemName = "Ammolet Of Wonder";
            string resourceName = "GlaurungItems/Resources/ammolet_of_wonder";
            GameObject obj = new GameObject(itemName);
            var item = obj.AddComponent<MatriochkAmmolet>();
            ItemBuilder.AddSpriteToObject(itemName, resourceName, obj);
            string shortDesc = "How many charges are stored in those ?";
            string longDesc = "Trigger a mini blank when a blank is activated. \n \n";
            ItemBuilder.SetupItem(item, shortDesc, longDesc, "gl");
            ItemBuilder.AddPassiveStatModifier(item, PlayerStats.StatType.AdditionalBlanksPerFloor, 1, StatModifier.ModifyMethod.ADDITIVE);
            item.quality = ItemQuality.C;
            item.BlankStunTime = 0;
        }

        protected override void OnBlank(SilencerInstance silencerInstance, Vector2 centerPoint, PlayerController user)
        {
            if (!cooldown)
            {
                this.DoMicroBlank(user.transform.position, 0f);
                cooldown = true;
                GameManager.Instance.StartCoroutine(this.EndCooldown());              
            }
        }

        private void DoMicroBlank(Vector2 center, float knockbackForce = 30f)
        {
            GameObject silencerVFX = (GameObject)ResourceCache.Acquire("Global VFX/BlankVFX_Ghost");
            AkSoundEngine.PostEvent("Play_OBJ_silenceblank_small_01", base.gameObject);
            GameObject gameObject = new GameObject("silencer");
            SilencerInstance silencerInstance = gameObject.AddComponent<SilencerInstance>();
            float additionalTimeAtMaxRadius = 0.25f;
            silencerInstance.TriggerSilencer(center, 20f, 5f, silencerVFX, 0f, 4f, 3f, 4f, knockbackForce, 4f, additionalTimeAtMaxRadius, base.Owner, false, false);
        }

        private IEnumerator EndCooldown()
        {
            yield return new WaitForSeconds(0.2f);
            cooldown = false;
            yield break;
        }

        private bool cooldown;
    }
}
