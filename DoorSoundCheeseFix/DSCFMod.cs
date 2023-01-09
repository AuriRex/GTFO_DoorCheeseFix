using BepInEx;
using BepInEx.Unity.IL2CPP;
using System.Reflection;

[assembly: AssemblyInformationalVersion(DoorSoundCheeseFix.DSCFMod.VERSION)]
[assembly: AssemblyVersion(DoorSoundCheeseFix.DSCFMod.VERSION)]
[assembly: AssemblyFileVersion(DoorSoundCheeseFix.DSCFMod.VERSION)]
namespace DoorSoundCheeseFix
{
    [BepInPlugin(GUID, nameof(DoorSoundCheeseFix), VERSION)]
    public class DSCFMod : BasePlugin
    {
        public const string GUID = "dev.aurirex.gtfo.doorsoundcheesefix";
        public const string VERSION = "1.1.0";

        private HarmonyLib.Harmony _harmonyInstance;

        public override void Load()
        {
            _harmonyInstance = new HarmonyLib.Harmony(GUID);
            _harmonyInstance.PatchAll(Assembly.GetExecutingAssembly());
        }
    }
}