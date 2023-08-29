using CalamityHunt.Common.Utilities;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using Terraria.Audio;

namespace CalamityHunt
{
    public static class AssetDirectory
    {
        public static class Textures
        {
            public static readonly Asset<Texture2D> Sparkle = AssetUtilities.RequestImmediate<Texture2D>($"{nameof(CalamityHunt)}/Assets/Textures/SharpSpark");
            public static readonly Asset<Texture2D> GlowRing = AssetUtilities.RequestImmediate<Texture2D>($"{nameof(CalamityHunt)}/Assets/Textures/GlowRing");
            public static readonly Asset<Texture2D> GlowRay = AssetUtilities.RequestImmediate<Texture2D>($"{nameof(CalamityHunt)}/Assets/Textures/GlowRay");
            public static readonly Asset<Texture2D> Glow = AssetUtilities.RequestImmediate<Texture2D>($"{nameof(CalamityHunt)}/Assets/Textures/GlowSoft");
            public static readonly Asset<Texture2D> GlowBig = AssetUtilities.RequestImmediate<Texture2D>($"{nameof(CalamityHunt)}/Assets/Textures/GlowSoftBig");
            public static readonly Asset<Texture2D> ShockRing = AssetUtilities.RequestImmediate<Texture2D>($"{nameof(CalamityHunt)}/Assets/Textures/ShockRing");
            public static readonly Asset<Texture2D> Empty = AssetUtilities.RequestImmediate<Texture2D>($"{nameof(CalamityHunt)}/Assets/Textures/Empty");

            public static class Buffs
            {
                public static readonly Asset<Texture2D>[] SlimeCane = AssetUtilities.RequestArrayImmediate<Texture2D>($"{nameof(CalamityHunt)}/Assets/Textures/Buffs/SlimeCane_", 2);
            }

            public static class Extras
            {
                public static readonly Asset<Texture2D> GoozmaGodEye = AssetUtilities.RequestImmediate<Texture2D>($"{nameof(CalamityHunt)}/Assets/Textures/Goozma/SpecialEye");
                public static readonly Asset<Texture2D> GoozmaEyeball = AssetUtilities.RequestImmediate<Texture2D>($"{nameof(CalamityHunt)}/Assets/Textures/Goozma/GoozmaSclera");
                public static readonly Asset<Texture2D>[] SwirlingRocks = AssetUtilities.RequestArrayImmediate<Texture2D>($"{nameof(CalamityHunt)}/Assets/Textures/Goozma/Crowns/SwirlingRocks_", 2);
                public static readonly Asset<Texture2D> PaladinPalanquinBall = AssetUtilities.RequestImmediate<Texture2D>($"{nameof(CalamityHunt)}/Assets/Textures/Mounts/PaladinPalanquinMount_Ball");
                public static readonly Asset<Texture2D> PaladinPalanquinWings = AssetUtilities.RequestImmediate<Texture2D>($"{nameof(CalamityHunt)}/Assets/Textures/Mounts/PaladinPalanquinMount_Wings");
                public static readonly Asset<Texture2D> InkyHats = AssetUtilities.RequestImmediate<Texture2D>($"{nameof(CalamityHunt)}/Assets/Textures/InkyHats");
                public static readonly Asset<Texture2D> InkyRings = AssetUtilities.RequestImmediate<Texture2D>($"{nameof(CalamityHunt)}/Assets/Textures/InkyRings");
                public static readonly Asset<Texture2D> GoozmoemEye = AssetUtilities.RequestImmediate<Texture2D>($"{nameof(CalamityHunt)}/Assets/Textures/GoozmoemEye");
                public static readonly Asset<Texture2D> GoozmoemCrown = AssetUtilities.RequestImmediate<Texture2D>($"{nameof(CalamityHunt)}/Assets/Textures/GoozmoemCrown");
                public static readonly Asset<Texture2D> CometKunaiFlame = AssetUtilities.RequestImmediate<Texture2D>($"{nameof(CalamityHunt)}/Assets/Textures/CometKunaiFlame");
                public static readonly Asset<Texture2D>[] FusionRay = AssetUtilities.RequestArrayImmediate<Texture2D>($"{nameof(CalamityHunt)}/Assets/Textures/FusionRay_", 4);
            }
            
            public static class Bars
            {
                public static readonly Asset<Texture2D> Stress = AssetUtilities.RequestImmediate<Texture2D>($"{nameof(CalamityHunt)}/Assets/Textures/ChargeBars/StressBar");
                public static readonly Asset<Texture2D> StressFill = AssetUtilities.RequestImmediate<Texture2D>($"{nameof(CalamityHunt)}/Assets/Textures/ChargeBars/StressBarFill");
                public static readonly Asset<Texture2D> StressTopped = AssetUtilities.RequestImmediate<Texture2D>($"{nameof(CalamityHunt)}/Assets/Textures/ChargeBars/StressBarTopped");
            }

            public static class SwordSwing
            {
                public static readonly Asset<Texture2D> Basic = AssetUtilities.RequestImmediate<Texture2D>($"{nameof(CalamityHunt)}/Assets/Textures/SwordSwing");
                public static readonly Asset<Texture2D> Bloody = AssetUtilities.RequestImmediate<Texture2D>($"{nameof(CalamityHunt)}/Assets/Textures/BloodSwing");
            }

            public static class SlimeMonsoon
            {
                public static readonly Asset<Texture2D> Lightning = AssetUtilities.RequestImmediate<Texture2D>($"{nameof(CalamityHunt)}/Common/Graphics/SlimeMonsoon/Lightning");
                public static readonly Asset<Texture2D> LightningGlow = AssetUtilities.RequestImmediate<Texture2D>($"{nameof(CalamityHunt)}/Common/Graphics/SlimeMonsoon/LightningGlow");

                public static readonly Asset<Texture2D> SkyTexture = AssetUtilities.RequestImmediate<Texture2D>($"{nameof(CalamityHunt)}/Common/Graphics/SlimeMonsoon/SkyNoise");
                public static readonly Asset<Texture2D> SkyDistortion = AssetUtilities.RequestImmediate<Texture2D>($"{nameof(CalamityHunt)}/Common/Graphics/SlimeMonsoon/DistortNoise");
                public static readonly Asset<Texture2D> SkyColorMap = AssetUtilities.RequestImmediate<Texture2D>($"{nameof(CalamityHunt)}/Common/Graphics/SlimeMonsoon/ColorMap");
            }
        }

        public static class Sounds
        {
            public static class Weapon
            {
                public static readonly SoundStyle ScytheOfTheOldGodSwing = new($"{nameof(CalamityHunt)}/Assets/Sounds/OldGodScytheSwing_" + 0) { MaxInstances = 0, PitchVariance = 0.1f };
                public static readonly SoundStyle ScytheOfTheOldGodSwingStrong = new($"{nameof(CalamityHunt)}/Assets/Sounds/OldGodScytheSwing_" + 1) { MaxInstances = 0, PitchVariance = 0.1f };
                public static readonly SoundStyle GoomoireWindLoop = new($"{nameof(CalamityHunt)}/Assets/Sounds/GoomoireWindLoop") { MaxInstances = 0, IsLooped = true };
                public static readonly SoundStyle SludgeShakerFiringLoop = new($"{nameof(CalamityHunt)}/Assets/Sounds/SludgeShakerFiringLoop") { MaxInstances = 0, IsLooped = true };
                public static readonly SoundStyle TrailBlazerFireStart = new($"{nameof(CalamityHunt)}/Assets/Sounds/TrailBlazerFireStart") { MaxInstances = 0, PitchVariance = 0.1f };
                public static readonly SoundStyle TrailblazerFireLoop = new($"{nameof(CalamityHunt)}/Assets/Sounds/TrailBlazerFireLoop") { MaxInstances = 0, IsLooped = true };
                public static readonly SoundStyle FissionFlyerExplode = new($"{nameof(CalamityHunt)}/Assets/Sounds/FissionFlyerExplode") { MaxInstances = 0 };
            }

            public static class Goozma
            {               
                public static readonly SoundStyle Sizzle = new($"{nameof(CalamityHunt)}/Assets/Sounds/Goozma/GoozmaSizzle") { MaxInstances = 0 };
                public static readonly SoundStyle WarbleLoop = new($"{nameof(CalamityHunt)}/Assets/Sounds/Goozma/GoozmaTravelLoop") { MaxInstances = 0, IsLooped = true };      
                public static readonly SoundStyle ShootLoop = new($"{nameof(CalamityHunt)}/Assets/Sounds/Goozma/GoozmaShootLoop") { MaxInstances = 0, IsLooped = true };
                public static readonly SoundStyle SimmerLoop = new($"{nameof(CalamityHunt)}/Assets/Sounds/Goozma/GoozmaSimmerLoop") { MaxInstances = 0, IsLooped = true };
                public static readonly SoundStyle FusionRayLoop = new($"{nameof(CalamityHunt)}/Assets/Sounds/Goozma/GoozmaFusionRayLoop") { MaxInstances = 0, IsLooped = true };
                public static readonly SoundStyle Pop = new($"{nameof(CalamityHunt)}/Assets/Sounds/Goozma/GoozmaPop") { MaxInstances = 0 };
                public static readonly SoundStyle Explode = new($"{nameof(CalamityHunt)}/Assets/Sounds/Goozma/GoozmaExplode") { MaxInstances = 0 };

                public static readonly SoundStyle WindLoop = new($"{nameof(CalamityHunt)}/Assets/Sounds/Goozma/GoozmaWindLoop") { MaxInstances = 0, IsLooped = true };

                public static readonly SoundStyle PixieBallBounce = new($"{nameof(CalamityHunt)}/Assets/Sounds/Goozma/Slimes/PixieBallBounce") { MaxInstances = 0, PitchVariance = 0.1f };
                public static readonly SoundStyle PixieBallLoop = new($"{nameof(CalamityHunt)}/Assets/Sounds/Goozma/Slimes/PixieBallLoop") { MaxInstances = 0, IsLooped = true };
                public static readonly SoundStyle Warning = new($"{nameof(CalamityHunt)}/Assets/Sounds/Alarm") { MaxInstances = 0 };

                public static readonly SoundStyle StellarReform = new($"{nameof(CalamityHunt)}/Assets/Sounds/Goozma/Slimes/StellarSlimeReform") { MaxInstances = 0 };
                public static readonly SoundStyle StellarBlackHoleLoop = new($"{nameof(CalamityHunt)}/Assets/Sounds/Goozma/Slimes/StellarBlackHoleLoop") { MaxInstances = 0, IsLooped = true };
                public static readonly SoundStyle StellarBlackHoleGulp = new($"{nameof(CalamityHunt)}/Assets/Sounds/Goozma/Slimes/StellarSlimeBlackHoleGulp") { MaxInstances = 0 };
                public static readonly SoundStyle StellarConstellationWave = new($"{nameof(CalamityHunt)}/Assets/Sounds/Goozma/Slimes/StellarSlimeConstellationWave") { MaxInstances = 0 };
                public static readonly SoundStyle StellarConstellationForm = new($"{nameof(CalamityHunt)}/Assets/Sounds/Goozma/Slimes/StellarSlimeConstellationForm") { MaxInstances = 0 };
            }

            public static readonly SoundStyle SupremeRestorationBigGulp = new($"{nameof(CalamityHunt)}/Assets/Sounds/SupremeRestorationBigGulp") { MaxInstances = 0, PitchVariance = 0.05f };
            public static readonly SoundStyle BloatBabyWarbleLoop = new($"{nameof(CalamityHunt)}/Assets/Sounds/BabyBloatTravelLoop") { MaxInstances = 0, IsLooped = true };
            public static readonly SoundStyle GoozmoemRay = new($"{nameof(CalamityHunt)}/Assets/Sounds/GoozmoemFusionRayShoot") { MaxInstances = 0, Volume = 0.8f };
            
            public static readonly SoundStyle GoozmaAuricSoulHeartbeat = new($"{nameof(CalamityHunt)}/Assets/Sounds/GoozmaAuricSoulHeartbeat") { MaxInstances = 0, IsLooped = true };
            public static readonly SoundStyle GoozmaAuricSoulDrone = new($"{nameof(CalamityHunt)}/Assets/Sounds/GoozmaAuricSoulDrone") { MaxInstances = 0, IsLooped = true };
            public static readonly SoundStyle GoozmaAuricSoulBreathe = new($"{nameof(CalamityHunt)}/Assets/Sounds/GoozmaAuricSoulBreathe") { MaxInstances = 0, PitchVariance = 0.4f };

            public static readonly SoundStyle YharonAuricSoulHeartbeat = new($"{nameof(CalamityHunt)}/Assets/Sounds/YharonAuricSoulHeartbeat") { MaxInstances = 0, IsLooped = true };
            public static readonly SoundStyle YharonAuricSoulDrone = new($"{nameof(CalamityHunt)}/Assets/Sounds/YharonAuricSoulDrone") { MaxInstances = 0, Volume = 0.5f, IsLooped = true };
            public static readonly SoundStyle YharonAuricSoulBreathe = new($"{nameof(CalamityHunt)}/Assets/Sounds/YharonAuricSoulBreathe") { MaxInstances = 0, PitchVariance = 0.2f };
        }
    }
}
