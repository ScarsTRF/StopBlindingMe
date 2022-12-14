using FrooxEngine;
using HarmonyLib;
using NeosModLoader;

namespace StopBlindingMe
{
    public class Patch : NeosMod
    {
        public override string Name => "StopBlindingMe";
        public override string Author => "ScarsTRF";
        public override string Version => "1.0";
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
                NotificationLayer.DisplayDuration.Value = config.GetValue(Hide) ? 0f : config.GetValue(Time);
            };
        }
        
        [HarmonyPatch(typeof(NotificationPanel))]
        class PatchNotificationPanel
        {
            [HarmonyPrefix]
            [HarmonyPatch("OnAwake")]
            static void OnAwake(NotificationPanel __instance)
            {
                __instance.RunInUpdates(3, () =>
                {
                    NotificationLayer = __instance;
                    __instance.DisplayDuration.Value = config.GetValue(Hide) ? 0f : config.GetValue(Time);
                });
            }
        }
    }
}