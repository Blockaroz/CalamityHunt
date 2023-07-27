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
            public static TextureAsset GlowRing = new TextureAsset($"{nameof(CalamityHunt)}/Assets/Textures/GlowRing");
            public static TextureAsset GlowRay = new TextureAsset($"{nameof(CalamityHunt)}/Assets/Textures/GlowRay");
            public static TextureAsset Glow = new TextureAsset($"{nameof(CalamityHunt)}/Assets/Textures/GlowSoft");
            public static TextureAsset GlowBig = new TextureAsset($"{nameof(CalamityHunt)}/Assets/Textures/GlowSoftBig");
            public static TextureAsset ShockRing = new TextureAsset($"{nameof(CalamityHunt)}/Assets/Textures/ShockRing");
            public static TextureAsset Empty = new TextureAsset($"{nameof(CalamityHunt)}/Assets/Textures/Empty");

            public struct Extras
            {
                public static TextureAsset GoozmaGodEye = new TextureAsset($"{nameof(CalamityHunt)}/Assets/Textures/Goozma/SpecialEye");
                public static TextureAsset GoozmaEyeball = new TextureAsset($"{nameof(CalamityHunt)}/Assets/Textures/Goozma/GoozmaSclera");
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
                public static TextureAsset SkyColorMap = new TextureAsset($"{nameof(CalamityHunt)}/Common/Graphics/SlimeMonsoon/ColorMap");
            }
        }

        public struct Sounds
        {
            public struct Weapon
            {
                public static SoundStyle ScytheOfTheOldGodSwing = new SoundStyle($"{nameof(CalamityHunt)}/Assets/Sounds/OldGodScytheSwing_" + 0)
                {
                    MaxInstances = 0,
                    PitchVariance = 0.1f
                };                
                public static SoundStyle ScytheOfTheOldGodSwingStrong = new SoundStyle($"{nameof(CalamityHunt)}/Assets/Sounds/OldGodScytheSwing_" + 1)
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
