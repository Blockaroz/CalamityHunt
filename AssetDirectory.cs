using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria.ModLoader;

namespace CalamityHunt
{
    public readonly record struct AssetDirectory
    {
        public struct Textures
        {
            public struct Extra
            {
                public static Texture2D Glow => ModContent.Request<Texture2D>($"{nameof(CalamityHunt)}/Assets/Textures/Goozma/GlowSoft", AssetRequestMode.ImmediateLoad).Value;

                public static Texture2D BigGlow => ModContent.Request<Texture2D>($"{nameof(CalamityHunt)}/Assets/Textures/Goozma/GlowSoftBig", AssetRequestMode.ImmediateLoad).Value;

                public static Texture2D Spark => ModContent.Request<Texture2D>($"{nameof(CalamityHunt)}/Assets/Textures/SharpSpark", AssetRequestMode.ImmediateLoad).Value;

            }
        }
    }
}
