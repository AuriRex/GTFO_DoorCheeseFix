using HarmonyLib;
using LevelGeneration;
using System.Collections;
using UnityEngine;
using static BepInEx.Unity.IL2CPP.Utils.MonoBehaviourExtensions;

namespace DoorSoundCheeseFix
{
    public class Patches
    {
        [HarmonyPatch(typeof(LG_WeakDoor_Anim), nameof(LG_WeakDoor_Anim.Setup))]
        internal static class LG_WeakDoor_Anim_Setup_Patch
        {
            public static void Postfix(LG_WeakDoor_Anim __instance)
            {
                // Closing does not consider doors closed right after pressing the button!
                var valueClosing = __instance.m_animData[eDoorStatus.Closed];
                valueClosing.traversableAtBegin = true;
                __instance.m_animData[eDoorStatus.Closed] = valueClosing;
            }
        }

        [HarmonyPatch(typeof(LG_WeakDoor_Anim), nameof(LG_WeakDoor_Anim.OnDoorState))]
        internal static class LG_WeakDoor_Anim_OnDoorState_Patch
        {
            public static void Postfix(LG_WeakDoor_Anim __instance, pDoorState state, bool isDropinState)
            {
                if (isDropinState)
                    return;

                if (state.status == eDoorStatus.Open || state.status == eDoorStatus.Closed)
                {
                    __instance.StopCoroutine(__instance.m_doorAnimRoutine);

                    __instance.m_doorAnimRoutine = __instance.StartCoroutine(DoorAnimSequence(__instance, __instance.m_animData[state.status], state));
                }
            }

            private static IEnumerator DoorAnimSequence(LG_WeakDoor_Anim instance, LG_DoorAnimData data, pDoorState state)
            {
                instance.InAnimation = true;
                instance.m_doorBladeCuller.StartAnimation();
                if (instance.m_gate != null)
                {
                    instance.m_gate.IsTraversable = data.traversableAtBegin;
                }
                instance.m_operationAnim.enabled = true;
                instance.m_operationAnim.Play(data.anim);
                instance.m_sound.Post(data.sfxBegin, true);
                
                yield return new WaitForSeconds(data.duration / 4f);

                if (state.status == eDoorStatus.Open)
                {
                    if (instance.m_gate != null)
                    {
                        instance.m_gate.IsTraversable = true;
                    }
                }

                yield return new WaitForSeconds(data.duration / 4f);
                yield return new WaitForSeconds(data.duration / 4f);

                if (state.status == eDoorStatus.Closed)
                {
                    if (instance.m_gate != null)
                    {
                        instance.m_gate.IsTraversable = false;
                    }
                }

                yield return new WaitForSeconds(data.duration / 4f);
                instance.m_sound.Post(data.sfxEnd, true);
                instance.m_doorBladeCuller.EndAnimation();
                if (instance.m_gate != null)
                {
                    instance.m_gate.IsTraversable = data.traversableAtEnd;
                }
                instance.m_operationAnim.enabled = false;
                instance.InAnimation = false;
                yield break;
            }
        }
    }
}
