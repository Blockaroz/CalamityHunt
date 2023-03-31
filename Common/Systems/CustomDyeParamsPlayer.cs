using CalamityHunt.Content.Items.Dyes;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using System.Linq;
using Terraria.DataStructures;
using Terraria.Graphics.Shaders;
using Terraria.ModLoader;

namespace CalamityHunt.Common.Systems
{
    public class CustomDyeParamsPlayer : ModPlayer
    {
        public override void DrawEffects(PlayerDrawSet drawInfo, ref float r, ref float g, ref float b, ref float a, ref bool fullBright)
        {
            if (Player.dye.Any(n => n.type == ModContent.ItemType<GoopDye>()))
                GameShaders.Armor.GetShaderFromItemId(ModContent.ItemType<GoopDye>())
                    .Shader.Parameters["uMap"].SetValue(ModContent.Request<Texture2D>($"{nameof(CalamityHunt)}/Assets/Textures/RainbowMap", AssetRequestMode.ImmediateLoad).Value);
        }
    }
}
