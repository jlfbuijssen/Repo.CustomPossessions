using BepInEx;
using BepInEx.Logging;

namespace alt.dev.plugins.CustomPosessions;

[BepInPlugin("dev.AltCtrler.plugin.CustomPossesion", "Alt's custom possesions", pluginVersion)]
public class Plugin : BaseUnityPlugin
{
    private const string pluginGUID = "dev.AltCtrler.plugin.CustomPossesion";
    private const string pluginName = "Alt's custom possesions";
    private const string pluginVersion = "0.0.0.1";


    //internal static new ManualLogSource Logger;

    private void Awake()
    {
        // Plugin startup logic
        //Logger = base.Logger;
        //Logger.LogInfo($"Plugin {MyPluginInfo.PLUGIN_GUID} is loaded!");
    }
}
