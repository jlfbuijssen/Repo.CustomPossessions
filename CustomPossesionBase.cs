using AltCtrler.CustomPosessions.Configuration;
using BepInEx;
using BepInEx.Configuration;
using BepInEx.Logging;
using HarmonyLib;
using System.Collections.Generic;
using System.Reflection;
using System.Text.RegularExpressions;
using UnityEngine;
using Photon.Pun;

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
                $"Zyrdann... really... loves... the... anal... \nadmiral 😍,This is the second test string",
                "The greeting shown in the console when this plugin is loaded (testing purposes, comma separated).");

            mLogSrcs.LogInfo(configConsoleGreeting.Value.Split(',')[0]);

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

        [HarmonyPatch(typeof(ValuableDirector), "Spawn")]
        private class ValueableSpawnPatch {
            private static MethodBase TargetMethod() {
                mLogSrcs.LogInfo("Getting TargetMethod");
                var type = AccessTools.TypeByName(nameof(ValuableDirector));
                return AccessTools.FirstMethod(type, method => method.Name.Contains("Spawn"));
            }

            [HarmonyPrefix]
            private static bool Prefix(object __instance, PrefabRef _valuable, ValuableVolume _volume, string _path, ref int ___valuableTargetAmount, ref float ___totalCurrentValue, ref float ___totalMaxValue, ref int ___totalMaxAmount, ref int ___valueableTargetAmount) {
                mLogSrcs.LogInfo($"[ValueableSpawnTest]{_valuable.PrefabName}");
                mLogSrcs.LogInfo($"[ValueableSpawnTest]{_volume.transform.position.ToString()}");
                GameObject prefab = _valuable.Prefab;
                if (GameManager.instance.gameMode == 0) {
                    Object.Instantiate(prefab, _volume.transform.position, _volume.transform.rotation);
                } else {
                    Photon.Pun.PhotonNetwork.InstantiateRoomObject(_valuable.ResourcePath, _volume.transform.position, _volume.transform.rotation, 0);
                }

                ValuableObject component = prefab.GetComponent<ValuableObject>();
                component.DollarValueSetLogic();
                ___valuableTargetAmount++;
                // Access dollarValueCurrent through reflection
                FieldInfo fieldInfo = AccessTools.Field(component.GetType(), "dollarValueCurrent");

                ___totalCurrentValue += ((float) fieldInfo.GetValue(component)) * 0.001f;
                if (___totalCurrentValue > ___totalMaxValue) {
                    ___totalMaxAmount = ___valueableTargetAmount;
                }


                return false;
            }

            [HarmonyPostfix]
            private static void Postfix(object __instance) {
                
            }
            
            // Original Spawn method
         //   private void Spawn(PrefabRef _valuable, ValuableVolume _volume, string _path)
	        //{
		       // GameObject prefab = _valuable.Prefab;
		       // if (GameManager.instance.gameMode == 0)
		       // {
			      //  Object.Instantiate(prefab, _volume.transform.position, _volume.transform.rotation);
		       // }
		       // else
		       // {
			      //  PhotonNetwork.InstantiateRoomObject(_valuable.ResourcePath, _volume.transform.position, _volume.transform.rotation, 0);
		       // }
		       // ValuableObject component = prefab.GetComponent<ValuableObject>();
		       // component.DollarValueSetLogic();
		       // valuableTargetAmount++;
		       // totalCurrentValue += component.dollarValueCurrent * 0.001f;
		       // if (totalCurrentValue > totalMaxValue)
		       // {
			      //  totalMaxAmount = valuableTargetAmount;
		       // }
	        //}

        }




        [HarmonyPatch]
        private class AltsTestMessagePatch {
            private static MethodBase TargetMethod() {
                return AccessTools.Method(AccessTools.TypeByName("ValuableLovePotion"), "GenerateAffectionateSentence", (System.Type[]) null, (System.Type[]) null);
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
                    bool num = text.Trim().Equals("this potion", System.StringComparison.OrdinalIgnoreCase);

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

            // OriginalGenerateAffectionateSentence()
            // private string GenerateAffectionateSentence()
	        //{
		       // List<string> list = new List<string>
		       // {
			      //  "Can't even with how {adjective} {playerName} is.", "{playerName} makes everything {intensifier} legit.", "Why is {playerName} so {adjective}? So cute!", "Every time I see {playerName}, I {intransitiveVerb}.", "{playerName} is just {intensifier} {adjective}, you know?", "Got me {adverb} thinking about {playerName} all day.", "Just want to {transitiveVerb} {playerName}.", "Oh my, {playerName} is {intensifier} {adjective}!", "When {playerName} smiles, I {intransitiveVerb}.", "{playerName}, you are so {adjective}!",
			      //  "Can we talk about how {adjective} {playerName} is?", "{playerName} has such a {adjective} vibe.", "Just saw {playerName} looking {adjective}, so sweet.", "Wow, {playerName} is so {adjective}!", "Every time {playerName} talks, I {intransitiveVerb}.", "{playerName} and me = {intensifier} {adjective} vibes.", "Is it just me or is {playerName} {intensifier} {adjective}?", "Not gonna lie, {playerName} is {adverb} {adjective}.", "{playerName} is always {adjective}, and I love it.", "I can't help {intransitiveVerb} over {playerName}.",
			      //  "Guess who has a crush on {playerName}? Me!", "{playerName} walking in makes my day {intensifier} {adjective}.", "Hey {playerName}, keep being you!", "With {playerName}, everything is {adjective}.", "Just {adverb} dreaming about {playerName}.", "{playerName} looks so {adjective} today.", "Low-key, {playerName} is the most {adjective} person.", "High-key crushing on {playerName}!", "{playerName} has that {adjective} something.", "For real, {playerName}'s vibe is {intensifier} {adjective}.",
			      //  "Can't help but {transitiveVerb} {playerName}; they're so {adjective}.", "{playerName} is {adverb} my {noun}!", "Life is more {adjective} with {playerName} around.", "{playerName}'s laugh is {intensifier} {adjective}.", "{playerName}, you {adverb} {transitiveVerb} my world.", "Why is {playerName} so {adjective}?", "Did you see {playerName} today? So {adjective}!", "It's {adverb} {adjective} how much I {transitiveVerb} {playerName}.", "Me, whenever I see {playerName}: So {adjective}!", "{playerName} has me {adverb} {intransitiveVerb}.",
			      //  "Just saw {playerName}, and yep, still {adjective}.", "{playerName} is my {intensifier} {adjective} crush.", "Can confirm, {playerName} is {adjective}!", "Everyday mood: {intransitiveVerb} about how {adjective} {playerName} is.", "{playerName}, stop being so {adjective}; I can't handle it.", "When {playerName} is {intensifier} {adjective}... *swoons*", "Just {intransitiveVerb} about {playerName} being so {adjective}.", "Yep, {playerName} keeps getting more {adjective}.", "{playerName} makes me believe in {intensifier} {adjective} things.", "Daily reminder: {playerName} is {intensifier} {adjective}.",
			      //  "To be honest, {playerName} rocks that {adjective} look {adverb}.", "Seeing {playerName} today was {adverb} the highlight.", "I can't stop {intransitiveVerb} when I think of {playerName}.", "{playerName}, you make my heart {intransitiveVerb}.", "Is it possible to {transitiveVerb} {playerName} more?", "{playerName} is just too {adjective}!", "Thinking about {playerName} makes me {intransitiveVerb}.", "My day gets {adjective} when I see {playerName}.", "{playerName} is my favorite {noun}.", "I {transitiveVerb} {playerName} so much!",
			      //  "Just {adverb} wishing I could {transitiveVerb} {playerName}.", "Whenever I see {playerName}, I {intransitiveVerb} inside.", "{playerName} has the most {adjective} smile.", "Can't wait to {transitiveVerb} {playerName} again.", "If only {playerName} knew how {adjective} they are.", "Feeling {adjective} thanks to {playerName}.", "{playerName}, you're {intensifier} {adjective}!", "I just want to {transitiveVerb} {playerName} all day.", "{playerName}, you make me {intransitiveVerb}.", "Life is {adjective} with {playerName}.",
			      //  "{playerName} is like the most {adjective} dream.", "Can't stop smiling because of {playerName}.", "I think I {transitiveVerb} {playerName}.", "{playerName} makes my heart {intransitiveVerb}.", "Oh, {playerName}, you're so {adjective}!", "Just thinking about {playerName} makes me happy.", "Wish I could {transitiveVerb} {playerName} right now.", "{playerName} is simply {adjective}.", "Feeling {adjective} whenever {playerName} is around.", "{playerName}, you brighten my day!",
			      //  "I {transitiveVerb} {playerName} more than anything.", "Just {adverb} thinking about {playerName}.", "{playerName} is {intensifier} {adjective}!", "Can't get enough of {playerName}'s {adjective} vibes.", "{playerName} is {adverb} {adjective}.", "Just {intransitiveVerb} about how {adjective} {playerName} is.", "{playerName} makes my day {intensifier} awesome."
		       // };
		       // string text = list[Random.Range(0, list.Count)];
		       // string text2 = text.Replace("{playerName}", playerName);
		       // if (text.Contains("{transitiveVerb}"))
		       // {
			      //  string newValue = transitiveVerbs[Random.Range(0, transitiveVerbs.Count)];
			      //  text2 = text2.Replace("{transitiveVerb}", newValue);
		       // }
		       // if (text.Contains("{intransitiveVerb}"))
		       // {
			      //  string text3 = intransitiveVerbs[Random.Range(0, intransitiveVerbs.Count)];
			      //  if (text2.Contains("{intransitiveVerb}s"))
			      //  {
				     //   text3 = ((!text3.EndsWith("e")) ? (text3 + "es") : (text3 + "s"));
				     //   text2 = text2.Replace("{intransitiveVerb}s", text3);
			      //  }
			      //  else
			      //  {
				     //   text2 = text2.Replace("{intransitiveVerb}", text3);
			      //  }
		       // }
		       // if (text.Contains("{adjective}"))
		       // {
			      //  string newValue2 = adjectives[Random.Range(0, adjectives.Count)];
			      //  text2 = text2.Replace("{adjective}", newValue2);
		       // }
		       // if (text.Contains("{intensifier}"))
		       // {
			      //  string newValue3 = intensifiers[Random.Range(0, intensifiers.Count)];
			      //  text2 = text2.Replace("{intensifier}", newValue3);
		       // }
		       // if (text.Contains("{adverb}"))
		       // {
			      //  string newValue4 = adverbs[Random.Range(0, adverbs.Count)];
			      //  text2 = text2.Replace("{adverb}", newValue4);
		       // }
		       // if (text.Contains("{noun}"))
		       // {
			      //  string newValue5 = nouns[Random.Range(0, nouns.Count)];
			      //  text2 = text2.Replace("{noun}", newValue5);
		       // }
		       // return char.ToUpper(text2[0]) + text2.Substring(1);
	        //}

        }

        
    }
}
