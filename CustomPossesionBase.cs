using AltCtrler.CustomPosessions.Configuration;
using BepInEx;
using BepInEx.Configuration;
using BepInEx.Logging;
using System.Collections.Generic;

namespace AltCtrler.CustomPosessions;

[BepInPlugin(MyPluginInfo.PLUGIN_GUID, MyPluginInfo.PLUGIN_NAME, MyPluginInfo.PLUGIN_VERSION)]
[BepInProcess("REPO.exe")]
public class CustomPossesionBase : BaseUnityPlugin
{
    internal static ManualLogSource mLogSrcs;
    private static ConfigModule _configModule = new ConfigModule();

    private static ConfigEntry<string> configConsoleGreeting;
    

    private void Awake()
    {
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

        mLogSrcs.LogInfo($"{MyPluginInfo.PLUGIN_NAME} is now ready");

    }
}
