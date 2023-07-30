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
                public static TextureAsset[] SwirlingRocks = TextureAsset.LoadArray($"{nameof(CalamityHunt)}/Assets/Textures/Goozma/Crowns/SwirlingRocks_", 2);
            }
            
            public struct Bars
            {
                public static TextureAsset Stress = new TextureAsset($"{nameof(CalamityHunt)}/Assets/Textures/ChargeBars/StressBar");
                public static TextureAsset StressFill = new TextureAsset($"{nameof(CalamityHunt)}/Assets/Textures/ChargeBars/StressBarFill");
                public static TextureAsset StressTopped = new TextureAsset($"{nameof(CalamityHunt)}/Assets/Textures/ChargeBars/StressBarTopped");
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
                public static SoundStyle ScytheOfTheOldGodSwing = new SoundStyle($"{nameof(CalamityHunt)}/Assets/Sounds/OldGodScytheSwing_" + 0) { MaxInstances = 0, PitchVariance = 0.1f };
                public static SoundStyle ScytheOfTheOldGodSwingStrong = new SoundStyle($"{nameof(CalamityHunt)}/Assets/Sounds/OldGodScytheSwing_" + 1) { MaxInstances = 0, PitchVariance = 0.1f };
                public static SoundStyle GoomoireWindLoop = new SoundStyle($"{nameof(CalamityHunt)}/Assets/Sounds/GoomoireWindLoop") { MaxInstances = 0, IsLooped = true };
                public static SoundStyle SludgeShakerFiringLoop = new SoundStyle($"{nameof(CalamityHunt)}/Assets/Sounds/SludgeShakerFiringLoop") { MaxInstances = 0, IsLooped = true };
                public static SoundStyle TrailBlazerFireStart = new SoundStyle($"{nameof(CalamityHunt)}/Assets/Sounds/TrailBlazerFireStart") { MaxInstances = 0, PitchVariance = 0.1f };
                public static SoundStyle TrailblazerFireLoop = new SoundStyle($"{nameof(CalamityHunt)}/Assets/Sounds/TrailBlazerFireLoop") { MaxInstances = 0, IsLooped = true };
            }

            public struct Goozma
            {               
                public static SoundStyle Sizzle = new SoundStyle($"{nameof(CalamityHunt)}/Assets/Sounds/Goozma/GoozmaSizzle") { MaxInstances = 0 };
                public static SoundStyle WarbleLoop = new SoundStyle($"{nameof(CalamityHunt)}/Assets/Sounds/Goozma/GoozmaTravelLoop") { MaxInstances = 0, IsLooped = true };      
                public static SoundStyle ShootLoop = new SoundStyle($"{nameof(CalamityHunt)}/Assets/Sounds/Goozma/GoozmaShootLoop") { MaxInstances = 0, IsLooped = true };
                public static SoundStyle SimmerLoop = new SoundStyle($"{nameof(CalamityHunt)}/Assets/Sounds/Goozma/GoozmaSimmerLoop") { MaxInstances = 0, IsLooped = true };
                public static SoundStyle FusionRayLoop = new SoundStyle($"{nameof(CalamityHunt)}/Assets/Sounds/Goozma/GoozmaFusionRayLoop") { MaxInstances = 0, IsLooped = true };

                public static SoundStyle WindLoop = new SoundStyle($"{nameof(CalamityHunt)}/Assets/Sounds/Goozma/GoozmaWindLoop") { MaxInstances = 0, IsLooped = true };

                public static SoundStyle PixieBallLoop = new SoundStyle($"{nameof(CalamityHunt)}/Assets/Sounds/Goozma/Slimes/PixieBallLoop") { MaxInstances = 0, IsLooped = true };
                public static SoundStyle Warning = new SoundStyle($"{nameof(CalamityHunt)}/Assets/Sounds/Goozma/GoozmaWarning") { MaxInstances = 0 };

                public static SoundStyle StellarReform = new SoundStyle($"{nameof(CalamityHunt)}/Assets/Sounds/Goozma/Slimes/StellarSlimeReform") { MaxInstances = 0 };
                public static SoundStyle StellarBlackHoleLoop = new SoundStyle($"{nameof(CalamityHunt)}/Assets/Sounds/Goozma/Slimes/StellarBlackHoleLoop") { MaxInstances = 0, IsLooped = true };
                public static SoundStyle StellarBlackHoleGulp = new SoundStyle($"{nameof(CalamityHunt)}/Assets/Sounds/Goozma/Slimes/StellarSlimeBlackHoleGulp") { MaxInstances = 0 };
                public static SoundStyle StellarConstellationWave = new SoundStyle($"{nameof(CalamityHunt)}/Assets/Sounds/Goozma/Slimes/StellarSlimeConstellationWave") { MaxInstances = 0 };
                public static SoundStyle StellarConstellationForm = new SoundStyle($"{nameof(CalamityHunt)}/Assets/Sounds/Goozma/Slimes/StellarSlimeConstellationForm") { MaxInstances = 0 };
            }

            public static SoundStyle SupremeRestorationBigGulp = new SoundStyle($"{nameof(CalamityHunt)}/Assets/Sounds/SupremeRestorationBigGulp") { MaxInstances = 0, PitchVariance = 0.05f };
            public static SoundStyle BloatBabyWarbleLoop = new SoundStyle($"{nameof(CalamityHunt)}/Assets/Sounds/BabyBloatTravelLoop") { MaxInstances = 0, IsLooped = true };
        }
    }
}
