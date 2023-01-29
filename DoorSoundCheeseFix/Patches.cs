using AIGraph;
using HarmonyLib;
using LevelGeneration;
using PlayerCoverage;
using System.Collections;
using UnityEngine;
using static BepInEx.Unity.IL2CPP.Utils.MonoBehaviourExtensions;

namespace DoorSoundCheeseFix
{
    public class Patches
    {
        /*[HarmonyPatch(typeof(LG_SecurityDoor_Anim), nameof(LG_SecurityDoor_Anim.EnableAnimator))]
        internal static class Papapapapap
        {
            public static void Postfix()
            {
                DSCFMod.LogMsg("EnableAnimator running!");
            }
        }

        [HarmonyPatch(typeof(LG_SecurityDoor_Anim), nameof(LG_SecurityDoor_Anim.DelayedDisableAnimator))]
        internal static class adasdasdsadsad
        {
            public static void Postfix()
            {
                DSCFMod.LogMsg("DelayedDisableAnimator running!");
            }
        }*/

        /*[HarmonyPatch(typeof(LG_Gate), nameof(LG_Gate.IsTraversable), MethodType.Getter)]
        internal static class LG_Gate_IsTraversable_Patch
        {
            // HAS BEEN INLINED
            public static void Postfix(LG_Gate __instance, ref bool __result)
            {
                var door = __instance.SpawnedDoor;

                *//*if (!PlayerCoverageDataPropagator_UpdatePropagate_NodeDistanceUnblocked_Patch.CurrentlyProcessingThings)
                    return;*/

                /*DSCFMod.LogMsg("TESTING");

                if(door.GetType().Name == nameof(LG_SecurityDoor))
                {
                    var secDoor = door.Cast<LG_SecurityDoor>();
                    if (secDoor != null)
                    {
                        DSCFMod.LogMsg("LG_SecurityDoor found!");
                        var doorAnimData = LG_SecurityDoor_Anim_OnDoorState_Patch.PotentiallyActiveDoors.FirstOrDefault(x => x.LG_Gate == __instance.Pointer);
                        if (doorAnimData != null)
                        {
                            DSCFMod.LogMsg("Found anim data!");
                            var curTime = Time.fixedTime;
                            if (doorAnimData.ActivationTime + doorAnimData.UnlockDuration < curTime
                                && doorAnimData.ActivationTime + doorAnimData.UnlockDuration + doorAnimData.OpenDuration > curTime)
                            {
                                DSCFMod.LogMsg("LG_Gate.IsTraversable - returning true as door is opening!");
                                __result = true;
                            }
                            else
                            {
                                DSCFMod.LogMsg("LG_Gate.IsTraversable - DOOR NOT READY YET!");
                            }
                        }
                    }
                }*//*
                

                
            }
        }*/

        

        [HarmonyPatch(typeof(ElevatorRide), nameof(ElevatorRide.OnGSWantToStartExpedition))]
        internal static class ElevatorRide_OnGSWantToStartExpedition_Patch
        {
            public static void Postfix()
            {
                DSCFMod.LogMsg("Clearing ActiveDoors.");
                DSCFMod.ActiveDoors.Clear();
            }
        }

        //public void OnDoorState(pDoorState state, bool isRecall)
        [HarmonyPatch(typeof(LG_SecurityDoor_Anim), nameof(LG_SecurityDoor_Anim.OnDoorState))]
        internal static class LG_SecurityDoor_Anim_OnDoorState_Patch
        {

            public static void Postfix(LG_SecurityDoor_Anim __instance, pDoorState state, bool isRecall)
            {
                if (isRecall)
                    return;

                if (state.status == eDoorStatus.Open)
                {
                    if (__instance.m_hasActiveEnemyWave)
                        return;

                    DSCFMod.LogMsg($"A door is opening (LG_Gate IntPtr: {__instance.m_gate.Pointer})!");
                    DSCFMod.ActiveDoors.Add(__instance.m_gate.Pointer, new DSCFMod.OpeningDoorData()
                    {
                        LG_Gate = __instance.m_gate,
                        ActivationTime = Time.fixedTime,
                        UnlockDuration = __instance.m_unlock_anim_length_frames / 30f + 3f,
                        OpenDuration = __instance.m_open_anim_length_frames / 30f
                    });
                }
            }
            
        }

        //private static void ProcessNoise(NM_NoiseData noiseData)
        /*[HarmonyPatch(typeof(NoiseManager), nameof(NoiseManager.ProcessNoise))]
        public static class NoiseManager_ProcessNoise_Patch
        {
            // NOT CALLED FOR SHOTS FIRED!!! x-x
            public static bool CurrentlyProcessingNoise { get; private set; }
            public static NM_NoiseData CurrentNoiseData { get; private set; }

            public static void Prefix(NM_NoiseData noiseData)
            {
                CurrentlyProcessingNoise = true;
                DSCFMod.LogMsg("Processing Noise now");
                CurrentNoiseData = noiseData;
            }

            public static void Postfix()
            {
                CurrentlyProcessingNoise = false;
                DSCFMod.LogMsg("NOT Processing Noise now");
            }
        }*/

        //public static List<EnemyAgent> GetReachableEnemiesInNodes(AIG_CourseNode from, int maxNodeDistance)
        // Useless
        /*[HarmonyPatch(typeof(AIG_CourseGraph), nameof(AIG_CourseGraph.GetReachableEnemiesInNodes))]
        public static class AIG_CourseGraph_GetReachableEnemiesInNodes_Patch
        {
            // NOT CALLED FOR SHOTS FIRED!!! x-x
            public static bool CurrentlyProcessingNoise { get; private set; }

            public static void Prefix(AIG_CourseNode from, int maxNodeDistance)
            {
                CurrentlyProcessingNoise = true;
                DSCFMod.LogMsg("Processing GetReachableEnemiesInNodes now");
            }

            public static void Postfix()
            {
                CurrentlyProcessingNoise = false;
                DSCFMod.LogMsg("NOT Processing GetReachableEnemiesInNodes now ANYMORE");
            }
        }*/

        //UpdatePropagate_NodeDistanceUnblocked
        [HarmonyPatch(typeof(PlayerCoverageSystem.PlayerCoverageDataPropagator), nameof(PlayerCoverageSystem.PlayerCoverageDataPropagator.UpdatePropagate_NodeDistanceUnblocked))]
        public static class PlayerCoverageDataPropagator_UpdatePropagate_NodeDistanceUnblocked_Patch
        {
            //public static bool CurrentlyProcessingThings { get; private set; }

            // Gets called very often!
            public static bool Prefix(PlayerCoverageSystem.PlayerCoverageDataPropagator __instance)
            {
                //CurrentlyProcessingThings = true;
                //DSCFMod.LogMsg("Processing PlayerCoverageDataPropagator_UpdatePropagate_NodeDistanceUnblocked_Patch");
                Custom_UpdatePropagate_NodeDistanceUnblocked(__instance);
                return false;
            }

            /*public static void Postfix()
            {
                //CurrentlyProcessingThings = false;
                //DSCFMod.LogMsg("STOPPED Processing PlayerCoverageDataPropagator_UpdatePropagate_NodeDistanceUnblocked_Patch");
            }*/

            public static void Custom_UpdatePropagate_NodeDistanceUnblocked(PlayerCoverageSystem.PlayerCoverageDataPropagator instance)
            {
                if (instance.m_playerAgent.CourseNode == null)
                    return;

                AIG_SearchID.IncrementSearchID();
                AIG_CourseNode aig_CourseNode = instance.m_playerAgent.CourseNode;
                PlayerCoverageSystem.PlayerCoverageData playerCoverageData = aig_CourseNode.m_playerCoverage.m_coverageDatas[instance.m_playerID];
                playerCoverageData.m_nodeDistanceUnblocked = 0;
                instance.IncrementUnblockedDistanceKey();
                playerCoverageData.m_unblockedDistanceKey = instance.m_unblockedDistanceKey;
                aig_CourseNode.m_searchID = AIG_SearchID.SearchID;
                instance.m_nodesToPropagate.Enqueue(aig_CourseNode);
                while (instance.m_nodesToPropagate.Count > 0)
                {
                    aig_CourseNode = instance.m_nodesToPropagate.Dequeue();
                    int num = aig_CourseNode.m_playerCoverage.m_coverageDatas[instance.m_playerID].m_nodeDistanceUnblocked + 1;
                    for (int i = 0; i < aig_CourseNode.m_portals.Count; i++)
                    {
                        AIG_CoursePortal aig_CoursePortal = aig_CourseNode.m_portals[i];
                        if (aig_CoursePortal.IsTraversable || DSCFMod.GateIsOpening(aig_CoursePortal.Gate))
                        {
                            PlayerCoverageSystem.PlayerCoverageData playerCoverageData2 = aig_CoursePortal.m_playerCoverage.m_coverageDatas[instance.m_playerID];
                            if (aig_CoursePortal.m_searchID == AIG_SearchID.SearchID)
                            {
                                if (num >= playerCoverageData2.m_nodeDistanceUnblocked)
                                {
                                    continue;
                                }
                            }
                            else
                            {
                                aig_CoursePortal.m_searchID = AIG_SearchID.SearchID;
                            }
                            playerCoverageData2.m_nodeDistanceUnblocked = num;
                            playerCoverageData2.m_bestCoverageFromNode = aig_CourseNode;
                            AIG_CourseNode oppositeNode = aig_CoursePortal.GetOppositeNode(aig_CourseNode);
                            PlayerCoverageSystem.PlayerCoverageData playerCoverageData3 = oppositeNode.m_playerCoverage.m_coverageDatas[instance.m_playerID];
                            if (oppositeNode.m_searchID == AIG_SearchID.SearchID)
                            {
                                if (num >= playerCoverageData3.m_nodeDistanceUnblocked)
                                {
                                    continue;
                                }
                            }
                            else
                            {
                                oppositeNode.m_searchID = AIG_SearchID.SearchID;
                            }
                            playerCoverageData3.m_nodeDistanceUnblocked = num;
                            playerCoverageData3.m_unblockedDistanceKey = instance.m_unblockedDistanceKey;
                            instance.m_nodesToPropagate.Enqueue(oppositeNode);
                        }
                    }
                }
            }
        }

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

                if (state.status == eDoorStatus.Destroyed)
                {
                    // Janky Door Bug fix maybe?
                    __instance.m_gate.SpawnedDoor.Cast<LG_WeakDoor>().Callback(new Action(() => {
                        if(!__instance.m_gate.IsTraversable)
                        {
                            DSCFMod.LogMsg("Door was set to be not traversable after being destroyed!! Setting door to be traversable.");
                            __instance.m_gate.IsTraversable = true;
                        }
                    }), 0.1f);
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
