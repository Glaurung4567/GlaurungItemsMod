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
            string itemName = "Matriochk Ammolet";
            string resourceName = "GlaurungItems/Resources/ammolet_of_wonder";
            GameObject obj = new GameObject(itemName);
            var item = obj.AddComponent<MatriochkAmmolet>();
            ItemBuilder.AddSpriteToObject(itemName, resourceName, obj);
            string shortDesc = "How many are stored in those ?";
            string longDesc = "Has a chance to trigger additional blanks when a blank is activated. \n \n Created by a Gungeonner who missed his cold and harsh Mother Land.";
            ItemBuilder.SetupItem(item, shortDesc, longDesc, "gl");
            ItemBuilder.AddPassiveStatModifier(item, PlayerStats.StatType.AdditionalBlanksPerFloor, 1, StatModifier.ModifyMethod.ADDITIVE);
            item.quality = ItemQuality.B;
            item.BlankStunTime = 0;
        }

        protected override void OnBlank(SilencerInstance silencerInstance, Vector2 centerPoint, PlayerController user)
        {
            if (!cooldown)
            {
                cooldown = true;
                int randomSelect = Random.Range(1, 10);
                //Tools.Print(randomSelect, "ffffff", true);
                switch (randomSelect)
                {
                    case 1:
                    case 2:
                    case 3:
                        GameManager.Instance.StartCoroutine(this.EndCooldownOneSmallDoll(centerPoint, user));
                        break;
                    case 4:
                    case 5:
                        GameManager.Instance.StartCoroutine(this.EndCooldownOneDoll(centerPoint, user));
                        break;
                    case 6:
                        GameManager.Instance.StartCoroutine(this.EndCooldownTwoDolls(centerPoint, user));
                        break;
                    default:
                        GameManager.Instance.StartCoroutine(this.EndCooldown());
                        break;
                }
            }
        }

        private void DoMicroBlank(Vector2 center, PlayerController user, float knockbackForce = 30f)
        {
            GameObject silencerVFX = (GameObject)ResourceCache.Acquire("Global VFX/BlankVFX_Ghost");
            AkSoundEngine.PostEvent("Play_OBJ_silenceblank_small_01", base.gameObject);
            GameObject gameObject = new GameObject("silencer");
            SilencerInstance silencerInstance = gameObject.AddComponent<SilencerInstance>();
            float additionalTimeAtMaxRadius = 0.25f;
            silencerInstance.TriggerSilencer(center, 20f, 5f, silencerVFX, 0f, 4f, 3f, 4f, knockbackForce, 4f, additionalTimeAtMaxRadius, user, false, false);
        }

        private void DoBlank(Vector2 centerPoint, PlayerController user)
        {
            GameObject gameObjectBlankVFX = (GameObject)ResourceCache.Acquire("Global VFX/BlankVFX");
            GameObject gameObjectBlank = new GameObject("silencer");
            SilencerInstance silencerInstance = gameObjectBlank.AddComponent<SilencerInstance>();
            silencerInstance.TriggerSilencer(centerPoint, 50f, 25f, gameObjectBlankVFX, 0.25f, 0.2f, 50f, 10f, 140f, 15f, 0.5f, user, true, false);
        }

        private IEnumerator EndCooldown()
        {
            yield return new WaitForSeconds(commonLastCooldownDuration);
            cooldown = false;
            yield break;
        }

        private IEnumerator EndCooldownOneSmallDoll(Vector2 center, PlayerController user)
        {
            yield return null;
            this.DoMicroBlank(center, user);
            GameManager.Instance.StartCoroutine(this.EndCooldown());
        }

        private IEnumerator EndCooldownOneDoll(Vector2 center, PlayerController user)
        {
            yield return new WaitForSeconds(0.1f);
            this.DoBlank(center, user);
            yield return null;
            GameManager.Instance.StartCoroutine(this.EndCooldown());
        }

        private IEnumerator EndCooldownTwoDolls(Vector2 center, PlayerController user)
        {
            yield return new WaitForSeconds(0.1f);
            this.DoBlank(center, user);
            yield return null;
            this.DoMicroBlank(center, user);
            GameManager.Instance.StartCoroutine(this.EndCooldown());
        }

        private bool cooldown;
        private static float commonLastCooldownDuration = 0.2f;
    }
}
