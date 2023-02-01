using BepInEx;
using BepInEx.Logging;
using BepInEx.Unity.IL2CPP;
using LevelGeneration;
using System.Reflection;
using UnityEngine;

[assembly: AssemblyInformationalVersion(DoorSoundCheeseFix.DSCFMod.VERSION)]
[assembly: AssemblyVersion(DoorSoundCheeseFix.DSCFMod.VERSION)]
[assembly: AssemblyFileVersion(DoorSoundCheeseFix.DSCFMod.VERSION)]
namespace DoorSoundCheeseFix
{
    [BepInPlugin(GUID, nameof(DoorSoundCheeseFix), VERSION)]
    public class DSCFMod : BasePlugin
    {
        public const string GUID = "dev.aurirex.gtfo.doorsoundcheesefix";
        public const string VERSION = "1.3.0";

        private HarmonyLib.Harmony _harmonyInstance;

        private static ManualLogSource _logger;

        public override void Load()
        {
            _harmonyInstance = new HarmonyLib.Harmony(GUID);
            _harmonyInstance.PatchAll(Assembly.GetExecutingAssembly());
            _logger = Log;
        }

        public static void LogMsg(string msg)
        {
            _logger.LogMessage(msg);
        }

        internal static Dictionary<IntPtr, OpeningDoorData> ActiveDoors { get; private set; } = new();

        public static bool GateIsOpening(LG_Gate gate)
        {
            if (gate == null)
                return false;

            if (!ActiveDoors.TryGetValue(gate.Pointer, out var doorAnimData))
                return false;

            if (doorAnimData != null && doorAnimData.LG_Gate != null)
            {
                var curTime = Time.fixedTime;
                if (doorAnimData.ActivationTime + doorAnimData.UnlockDuration < curTime
                    && doorAnimData.ActivationTime + doorAnimData.UnlockDuration + doorAnimData.OpenDuration > curTime)
                {
                    return true;
                }
            }
            else
            {
                if (ActiveDoors.ContainsKey(gate.Pointer))
                    ActiveDoors.Remove(gate.Pointer);
            }
            return false;
        }

        public class OpeningDoorData
        {
            public LG_Gate LG_Gate { get; set; }
            public float ActivationTime { get; set; }
            public float UnlockDuration { get; set; }
            public float OpenDuration { get; set; }
        }
    }
}