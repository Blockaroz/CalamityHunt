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

    }
}
