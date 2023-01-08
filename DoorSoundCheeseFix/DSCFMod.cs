using BepInEx;
using BepInEx.Unity.IL2CPP;
using System.Collections;
using System.Reflection;
using UnityEngine;

namespace DoorSoundCheeseFix
{
    [BepInPlugin(GUID, nameof(DoorSoundCheeseFix), VERSION)]
    public class DSCFMod : BasePlugin
    {
        public const string GUID = "dev.aurirex.gtfo.doorsoundcheesefix";
        public const string VERSION = "1.0.0";

        private HarmonyLib.Harmony _harmonyInstance;
        private static MonoBehaviour _mainComponent;

        public override void Load()
        {
            _mainComponent = AddComponent<CoroutineComponentThing>();

            _harmonyInstance = new HarmonyLib.Harmony(GUID);
            _harmonyInstance.PatchAll(Assembly.GetExecutingAssembly());
        }

        public static Coroutine StartCoroutine(IEnumerator routine)
        {
            return BepInEx.Unity.IL2CPP.Utils.MonoBehaviourExtensions.StartCoroutine(_mainComponent, routine);
        }

        public static void StopCoroutine(Coroutine coroutine)
        {
            _mainComponent.StopCoroutine(coroutine);
        }

        public class CoroutineComponentThing : MonoBehaviour
        {
            public CoroutineComponentThing(IntPtr ptr) : base(ptr) { }

            public void Awake()
            {
                DontDestroyOnLoad(this);
                hideFlags = HideFlags.HideAndDontSave;
            }
        }
    }
}