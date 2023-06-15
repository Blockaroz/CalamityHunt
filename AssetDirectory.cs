using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria.Audio;
using Terraria.ModLoader;

namespace CalamityHunt
{
    public readonly record struct AssetDirectory
    {
        public struct Textures
        {
            public static Texture2D Sparkle => ModContent.Request<Texture2D>($"{nameof(CalamityHunt)}/Assets/Textures/SharpSpark", AssetRequestMode.ImmediateLoad).Value;

            public struct TempGoozmaExtras
            {
                public static Texture2D Glow => ModContent.Request<Texture2D>($"{nameof(CalamityHunt)}/Assets/Textures/Goozma/GlowSoft", AssetRequestMode.ImmediateLoad).Value;
                public static Texture2D BigGlow => ModContent.Request<Texture2D>($"{nameof(CalamityHunt)}/Assets/Textures/Goozma/GlowSoftBig", AssetRequestMode.ImmediateLoad).Value;
                public static Texture2D GoozmaEye => ModContent.Request<Texture2D>($"{nameof(CalamityHunt)}/Assets/Textures/Goozma/SpecialEye", AssetRequestMode.ImmediateLoad).Value;
            }

            public struct SwordSwing
            {
                public static TextureAsset Basic => new TextureAsset($"{nameof(CalamityHunt)}/Assets/Textures/SwordSwing");
                public static TextureAsset Bloody => new TextureAsset($"{nameof(CalamityHunt)}/Assets/Textures/BloodSwing");
            }
        }

        public struct Sounds
        {
            public struct Weapon
            {
                public static SoundStyle ScytheOfTheOldGodSwing => new SoundStyle($"{nameof(CalamityHunt)}/Assets/Sounds/OldGodScytheSwing")
                {
                    MaxInstances = 0,
                    PitchVariance = 0.1f
                };
            }

            public struct Goozma
            {
                //todo later
            }
        }
    }
}
