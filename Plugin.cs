using BepInEx;
using BepInEx.Configuration;
using BepInEx.Logging;
using TMPro;

namespace Cutscenes
{
    [BepInPlugin(MyPluginInfo.PLUGIN_GUID, MyPluginInfo.PLUGIN_NAME, MyPluginInfo.PLUGIN_VERSION)]
    [BepInProcess("Project Arrhythmia.exe")]
    public class Plugin : BaseUnityPlugin
    {
        internal static new ManualLogSource Logger;

        ConfigEntry<string> keyConfig;
        ConfigEntry<float> glitchIntensityConfig;

        internal static UnityEngine.KeyCode key;
        internal static float glitchIntensity;
        internal static TextMeshProUGUI SkipLabel, RewindIcon;

        private void Awake()
        {
            // Plugin startup logic
            Logger = base.Logger;

            keyConfig = Config.Bind("General", "Key", "K", "The key that skips the cutscene when pressed. (Enter => Return)");
            glitchIntensityConfig = Config.Bind("General", "Glitch", 0.4f, "The intensity of glitch effect on rewinding [0.00-1.00].");
            key = (UnityEngine.KeyCode)System.Enum.Parse(typeof(UnityEngine.KeyCode), keyConfig.Value);
            glitchIntensity = glitchIntensityConfig.Value;

            HarmonyLib.Harmony.CreateAndPatchAll(typeof(Patcher));

            Logger.LogInfo("Plugin is loaded!");
        }
    }
}