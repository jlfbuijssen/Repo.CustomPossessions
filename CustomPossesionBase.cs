using AltCtrler.CustomPosessions.Configuration;
using BepInEx;
using BepInEx.Configuration;
using BepInEx.Logging;
using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text.RegularExpressions;
using UnityEngine;

namespace AltCtrler.CustomPosessions {

    [BepInPlugin(MyPluginInfo.PLUGIN_GUID, MyPluginInfo.PLUGIN_NAME, MyPluginInfo.PLUGIN_VERSION)]
    [BepInProcess("REPO.exe")]
    public class CustomPossesionBase : BaseUnityPlugin {
        internal static ManualLogSource mLogSrcs;
        private static ConfigModule _configModule = new ConfigModule();

        private static ConfigEntry<string> configConsoleGreeting;
        private static Harmony _harmony;


        private void Awake() {
            // Plugin startup logic
            mLogSrcs = BepInEx.Logging.Logger.CreateLogSource(MyPluginInfo.PLUGIN_GUID);
            mLogSrcs.LogInfo($"{MyPluginInfo.PLUGIN_GUID} is awakening");

            configConsoleGreeting = Config.Bind(
                "General",
                "Console Greeting",
                $"Hello, thanks for using {MyPluginInfo.PLUGIN_NAME}.",
                "The greeting shown in the console when this plugin is loaded (testing purposes).");

            mLogSrcs.LogInfo(configConsoleGreeting.Value);

            mLogSrcs.LogInfo("Attemptingto load patches...");
            // Patch here
            _harmony = new Harmony("Alt.Possesions");
            //_harmony.PatchAll();
            _harmony = Harmony.CreateAndPatchAll(typeof(HarmonyPatch));
            LoadDebugPlugins();
            mLogSrcs.LogInfo($"{MyPluginInfo.PLUGIN_NAME} is now ready");

        }

        private void LoadDebugPlugins() {
            mLogSrcs.LogInfo("Are you sure all debug plugins have been loaded?");
        }

        private void OnDestroy() {
            _harmony?.UnpatchSelf();
            UnloadMyResources();
        }

        private void UnloadMyResources() {
            
        }

        [HarmonyPatch]
        private class ValueableSpawnPatch {
            private static MethodBase TargetMethod() {
                return AccessTools.Method(AccessTools.TypeByName("ValuableDiractor"), "Spawn");
            }

            [HarmonyPrefix]
            private static bool Prefix(object __instance) {
                return true;
            }

            [HarmonyPostfix]
            private static void Postfix(object __instance) {
                
            }


        }




        [HarmonyPatch]
        private class AltsTestMessagePatch {
            private static MethodBase TargetMethod() {
                return AccessTools.Method(AccessTools.TypeByName("ValuableLovePotion"), "GenerateAffectionateSentence", (Type[]) null, (Type[]) null);
            }

            [HarmonyPrefix]
            private static bool Prefix(object __instance, ref string __result) {
                if (UnityEngine.Random.value < .5f) {
                    string input = "";
                    FieldInfo fieldInfo = AccessTools.Field(__instance.GetType(), "playerName");
                    if (fieldInfo != null) {
                        input = (fieldInfo.GetValue(__instance) as string) ?? "";
                    }

                    string text = Regex.Replace(input, "[^a-zA-Z0-9 ]", "");
                    bool num = text.Trim().Equals("this potion", StringComparison.OrdinalIgnoreCase);

                    List<string> list = new List<string> {
                        "This is alt's first custom possesion string"
                    };

                    List<string> list2 = new List<string>() {
                        "List2 item 1", "List2 item 2", "List2 item 3"    
                    };

                    string text2;

                    if (num) {
                        text2 = list2[UnityEngine.Random.Range(0, list2.Count)];

                    } else {
                        text2 = list[UnityEngine.Random.Range(0, list.Count)];
                        text2 = text2.Replace("[player]", text);
                    }

                    __result = text2;

                    return false;
                }
                return true;
            }

            [HarmonyPostfix]
            private static void Postfix(object __instance, ref string __result) {
                FieldInfo fieldInfo = AccessTools.Field(__instance.GetType(), "playerName");
                if (fieldInfo != null) {
                    string playerName = fieldInfo.GetValue(__instance) as string;
                    if (!string.IsNullOrEmpty(playerName)) {
                        string newValue = Regex.Replace(playerName, "[^a-zA-z0-0]", "");
                        __result = __result.Replace(playerName, newValue);
                    }
                }
            }

        }

        
    }
}
