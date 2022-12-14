using FrooxEngine;
using HarmonyLib;
using NeosModLoader;

namespace StopBlindingMe
{
    public class Patch : NeosMod
    {
        public override string Name => "StopBlindingMe";
        public override string Author => "ScarsTRF";
        public override string Version => "1.0.1";
        public override string Link => "https://github.com/ScarsTRF/StopBlindingMe";

        [AutoRegisterConfigKey]
        private static readonly ModConfigurationKey<bool> Hide = new ModConfigurationKey<bool>("Hide", "Weather or not to hide the Notifications.", () => false);
        [AutoRegisterConfigKey]
        private static readonly ModConfigurationKey<float> Time = new ModConfigurationKey<float>("Time", "How long Notifications will last for in seconds.", () => 10f);

        private static ModConfiguration config;

        private static NotificationPanel NotificationLayer;

        public override void OnEngineInit()
        {
            config = GetConfiguration();
            config.Save(true);

            Harmony harmony = new Harmony($"dev.{Author}.{Name}");
            harmony.PatchAll();
            config.OnThisConfigurationChanged += c =>
            {
                UpdatePanel(NotificationLayer);
            };
        }
        
        [HarmonyPatch(typeof(NotificationPanel))]
        class PatchNotificationPanel
        {
            [HarmonyPostfix]
            [HarmonyPatch("OnAttach")]
            static void OnAttach(NotificationPanel __instance)
            {
                NotificationLayer = __instance;
                UpdatePanel(__instance);
            }
        }

        public static void UpdatePanel(NotificationPanel panel)
        {
            if (config.GetValue(Time) >= 30f)
            {
                panel.DisplayDuration.Value = config.GetValue(Hide) ? 0f : 30f;
            }
            else if (config.GetValue(Time) <= 1f)
            {
                panel.DisplayDuration.Value = config.GetValue(Hide) ? 0f : 1f;
            }
            else
            {
                panel.DisplayDuration.Value = config.GetValue(Hide) ? 0f : config.GetValue(Time);
            }
        }
    }
}