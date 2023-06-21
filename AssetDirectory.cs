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
            public static TextureAsset Sparkle = new TextureAsset($"{nameof(CalamityHunt)}/Assets/Textures/SharpSpark");

            public struct TempGoozmaExtras
            {
                public static TextureAsset Glow = new TextureAsset($"{nameof(CalamityHunt)}/Assets/Textures/Goozma/GlowSoft");
                public static TextureAsset BigGlow = new TextureAsset($"{nameof(CalamityHunt)}/Assets/Textures/Goozma/GlowSoftBig");
                public static TextureAsset GoozmaEye = new TextureAsset($"{nameof(CalamityHunt)}/Assets/Textures/Goozma/SpecialEye");
            }

            public struct SwordSwing
            {
                public static TextureAsset Basic = new TextureAsset($"{nameof(CalamityHunt)}/Assets/Textures/SwordSwing");
                public static TextureAsset Bloody = new TextureAsset($"{nameof(CalamityHunt)}/Assets/Textures/BloodSwing");
            }

            public struct SlimeMonsoon
            {
                public static TextureAsset Lightning = new TextureAsset($"{nameof(CalamityHunt)}/Common/Graphics/SlimeMonsoon/Lightning");
                public static TextureAsset LightningGlow = new TextureAsset($"{nameof(CalamityHunt)}/Common/Graphics/SlimeMonsoon/LightningGlow");

                public static TextureAsset SkyTexture = new TextureAsset($"{nameof(CalamityHunt)}/Common/Graphics/SlimeMonsoon/SkyNoise");
                public static TextureAsset SkyDistortion = new TextureAsset($"{nameof(CalamityHunt)}/Common/Graphics/SlimeMonsoon/DistortNoise");
                public static TextureAsset SkyColorMap =>new TextureAsset($"{nameof(CalamityHunt)}/Common/Graphics/SlimeMonsoon/ColorMap");
            }
        }

        public struct Sounds
        {
            public struct Weapon
            {
                public static SoundStyle ScytheOfTheOldGodSwing = new SoundStyle($"{nameof(CalamityHunt)}/Assets/Sounds/OldGodScytheSwing")
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
