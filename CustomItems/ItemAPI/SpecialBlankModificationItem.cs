using System;
using MonoMod.RuntimeDetour;
using System.Reflection;
using UnityEngine;

namespace ItemAPI
{
    class SpecialBlankModificationItem : BlankModificationItem
    {
        public static void InitHooks()
        {
            Hook hook = new Hook(
                typeof(SilencerInstance).GetMethod("ProcessBlankModificationItemAdditionalEffects", BindingFlags.NonPublic | BindingFlags.Instance),
                typeof(SpecialBlankModificationItem).GetMethod("BlankModificationHook")
            );
        }

        public static void BlankModificationHook(Action<SilencerInstance, BlankModificationItem, Vector2, PlayerController> orig, SilencerInstance self, BlankModificationItem bmi, Vector2 centerPoint, PlayerController user)
        {
            orig(self, bmi, centerPoint, user);
            if (bmi is SpecialBlankModificationItem)
            {
                (bmi as SpecialBlankModificationItem).OnBlank(self, centerPoint, user);
            }
        }

        protected virtual void OnBlank(SilencerInstance silencerInstance, Vector2 centerPoint, PlayerController user)
        {

        }
    }
}