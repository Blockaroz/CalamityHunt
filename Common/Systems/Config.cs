using System.ComponentModel;
using Terraria.ModLoader.Config;

namespace CalamityHunt.Common.Systems
{
    public class Config : ModConfig
    {
        public static Config Instance;
        public override ConfigScope Mode => ConfigScope.ClientSide;

        [Header("$Mods.CalamityHunt.Config.VisualHeader")]
        [Label("$Mods.CalamityHunt.Config.HolyExplosionToggle.Label")]
        [DefaultValue(true)]
        [Tooltip("$Mods.CalamityHunt.Config.HolyExplosionToggle.Tooltip")]
        public bool epilepsy { get; set; }

        [Label("$Mods.CalamityHunt.Config.StressPosX.Label")]
        [Range(0f, 100f)]
        [DefaultValue(35.77406f)]
        [Tooltip("$Mods.CalamityHunt.Config.StressPosX.Tooltip")]
        public float stressX { get; set; }

        [Label("$Mods.CalamityHunt.Config.StressPosY.Label")]
        [Range(0f, 100f)]
        [DefaultValue(3.97614312f)]
        [Tooltip("$Mods.CalamityHunt.Config.StressPosY.Tooltip")]
        public float stressY { get; set; }
        [Label("$Mods.CalamityHunt.Config.StressShake.Label")]
        [Range(0f, 4f)]
        [DefaultValue(2f)]
        [Tooltip("$Mods.CalamityHunt.Config.StressShake.Tooltip")]
        public float stressShake { get; set; }

    }
}
