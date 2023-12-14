﻿using System.Collections.Generic;
using CalamityHunt.Common.Utilities;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using Terraria.Audio;
using Terraria.ModLoader;

namespace CalamityHunt;

public static class AssetDirectory
{
    public static readonly string AssetPath = $"{nameof(CalamityHunt)}/Assets/";

    public static class Textures
    {
        public static readonly Asset<Texture2D> Sparkle = AssetUtilities.RequestImmediate<Texture2D>(AssetPath + "Textures/Extra/Sparkle");
        public static readonly Asset<Texture2D> GlowRing = AssetUtilities.RequestImmediate<Texture2D>(AssetPath + "Textures/Extra/GlowRing");
        public static readonly Asset<Texture2D> GlowRay = AssetUtilities.RequestImmediate<Texture2D>(AssetPath + "Textures/Extra/GlowRay");
        public static readonly Asset<Texture2D> ShockRing = AssetUtilities.RequestImmediate<Texture2D>(AssetPath + "Textures/Extra/ShockRing");
        public static readonly Asset<Texture2D> Empty = AssetUtilities.RequestImmediate<Texture2D>(AssetPath + "Textures/Extra/Empty");
        public static readonly Asset<Texture2D> Template = AssetUtilities.RequestImmediate<Texture2D>(AssetPath + "Textures/Extra/Template");

        public static readonly Asset<Texture2D>[] Noise = AssetUtilities.RequestArrayTotalImmediate<Texture2D>(AssetPath + "Textures/Extra/Noise_");
        public static readonly Asset<Texture2D>[] Space = AssetUtilities.RequestArrayTotalImmediate<Texture2D>(AssetPath + "Textures/Extra/Space_");
        public static readonly Asset<Texture2D>[] ColorMap = AssetUtilities.RequestArrayTotalImmediate<Texture2D>(AssetPath + "Textures/Extra/ColorMap_");
        public static readonly Asset<Texture2D>[] Glow = AssetUtilities.RequestArrayTotalImmediate<Texture2D>(AssetPath + "Textures/Extra/Glow_");

        public static readonly Asset<Texture2D>[] SwordSwing = AssetUtilities.RequestArrayTotalImmediate<Texture2D>(AssetPath + "Textures/Extra/SwordSwing_");
        public static readonly Asset<Texture2D> QuestionMark = AssetUtilities.RequestImmediate<Texture2D>($"{nameof(CalamityHunt)}/icon_small");

        public static Dictionary<int, Asset<Texture2D>> Particle = new Dictionary<int, Asset<Texture2D>>();
        public static Dictionary<int, Asset<Texture2D>> Relic = new Dictionary<int, Asset<Texture2D>>();
        public static Dictionary<int, Asset<Texture2D>> FlyingSlime = new Dictionary<int, Asset<Texture2D>>();

        //Jokes and Seasons
        internal static readonly Asset<Texture2D> FrogParticle = AssetUtilities.RequestImmediate<Texture2D>(AssetPath + "Textures/Extra/FrogParticle");
        internal static readonly Asset<Texture2D> WideMoto = AssetUtilities.RequestImmediate<Texture2D>(AssetPath + "Textures/Extra/WideMoto");
        internal static readonly Asset<Texture2D> SantaHat = AssetUtilities.RequestImmediate<Texture2D>(AssetPath + "Textures/Extra/SantaHat");
        internal static readonly Asset<Texture2D> ElfHat = AssetUtilities.RequestImmediate<Texture2D>(AssetPath + "Textures/Extra/ElfHat");

        public static class Goozma
        {
            public static readonly string GoozmaPrefix = AssetPath + "Textures/NPCs/Bosses/GoozmaBoss/";

            public static readonly Asset<Texture2D> Dress = AssetUtilities.RequestImmediate<Texture2D>(GoozmaPrefix + "GoozmaDress");
            public static readonly Asset<Texture2D> Tentacle = AssetUtilities.RequestImmediate<Texture2D>(GoozmaPrefix + "GoozmaTentacle");
            public static readonly Asset<Texture2D> MicroTentacle = AssetUtilities.RequestImmediate<Texture2D>(GoozmaPrefix + "GoozmaMicroTentacle");

            public static readonly Asset<Texture2D> BossPortrait = AssetUtilities.RequestImmediate<Texture2D>(GoozmaPrefix + "GoozmaBC");
            public static readonly Asset<Texture2D> GodEye = AssetUtilities.RequestImmediate<Texture2D>(GoozmaPrefix + "SpecialEye");
            public static readonly Asset<Texture2D> Sclera = AssetUtilities.RequestImmediate<Texture2D>(GoozmaPrefix + "GoozmaSclera");
            public static readonly Asset<Texture2D> Wormhole = AssetUtilities.RequestImmediate<Texture2D>(GoozmaPrefix + "Wormhole");
            public static readonly Asset<Texture2D> DarkSludge = AssetUtilities.RequestImmediate<Texture2D>(GoozmaPrefix + "DarkSludgeTexture");
            public static readonly Asset<Texture2D> DarkSludgeGlow = AssetUtilities.RequestImmediate<Texture2D>(GoozmaPrefix + "DarkSludgeTextureGlow");
            public static readonly Asset<Texture2D> Lightning = AssetUtilities.RequestImmediate<Texture2D>(GoozmaPrefix + "Lightning");
            public static readonly Asset<Texture2D> LightningGlow = AssetUtilities.RequestImmediate<Texture2D>(GoozmaPrefix + "LightningGlow");
            public static readonly Asset<Texture2D> LiquidTrail = AssetUtilities.RequestImmediate<Texture2D>(GoozmaPrefix + "LiquidTrail");

            public static readonly Asset<Texture2D> Crown = AssetUtilities.RequestImmediate<Texture2D>(GoozmaPrefix + "Crowns/GoozmaCrown");
            public static readonly Asset<Texture2D> CrownMask = AssetUtilities.RequestImmediate<Texture2D>(GoozmaPrefix + "Crowns/GoozmaCrown_Mask");
            public static readonly Asset<Texture2D> Ornament = AssetUtilities.RequestImmediate<Texture2D>(GoozmaPrefix + "Crowns/GoozmaOrnament");
            public static readonly Asset<Texture2D> Ninja = AssetUtilities.RequestImmediate<Texture2D>(GoozmaPrefix + "Crowns/DeadNinja");
            public static readonly Asset<Texture2D> CorruptEye = AssetUtilities.RequestImmediate<Texture2D>(GoozmaPrefix + "Crowns/CorruptEye");
            public static readonly Asset<Texture2D> CrystalMine = AssetUtilities.RequestImmediate<Texture2D>(GoozmaPrefix + "Crowns/CrystalMine");

            public static readonly Asset<Texture2D> DivineWings = AssetUtilities.RequestImmediate<Texture2D>(GoozmaPrefix + "DivineGargooptuarWingsNevermindSorryDominic");
            public static readonly Asset<Texture2D> PixieBeachBall = AssetUtilities.RequestImmediate<Texture2D>(GoozmaPrefix + "PixieBeachBall");
            public static readonly Asset<Texture2D> PixieHitMeSign = AssetUtilities.RequestImmediate<Texture2D>(GoozmaPrefix + "PixieHitMeSign");
            public static readonly Asset<Texture2D> PixieHitMeHand = AssetUtilities.RequestImmediate<Texture2D>(GoozmaPrefix + "PixieHitMeHand");

            public static readonly Asset<Texture2D>[] SwirlingRocks = AssetUtilities.RequestArrayImmediate<Texture2D>(GoozmaPrefix + "Crowns/SwirlingRocks_", 2);
            public static readonly Asset<Texture2D> ConstellationArea = AssetUtilities.RequestImmediate<Texture2D>(GoozmaPrefix + "ConstellationArea");
            public static readonly Asset<Texture2D> SpaceTrail = AssetUtilities.RequestImmediate<Texture2D>(GoozmaPrefix + "SpaceTrail");
            public static readonly Asset<Texture2D> SpaceTrailGlow = AssetUtilities.RequestImmediate<Texture2D>(GoozmaPrefix + "SpaceTrailGlow");

            public static readonly Asset<Texture2D> PaladinPalanquinBall = AssetUtilities.RequestImmediate<Texture2D>(AssetPath + "Textures/Mounts/PaladinPalanquinMount_Ball");
            public static readonly Asset<Texture2D> PaladinPalanquinWings = AssetUtilities.RequestImmediate<Texture2D>(AssetPath + "Textures/Mounts/PaladinPalanquinMount_Wings");
            public static readonly Asset<Texture2D> InkyHats = AssetUtilities.RequestImmediate<Texture2D>(AssetPath + "Textures/Projectiles/Weapons/Summoner/InkyHats");
            public static readonly Asset<Texture2D> InkyRings = AssetUtilities.RequestImmediate<Texture2D>(AssetPath + "Textures/Projectiles/Weapons/Summoner/InkyRings");
            public static readonly Asset<Texture2D> GoozmoemEye = AssetUtilities.RequestImmediate<Texture2D>(AssetPath + "Textures/Projectiles/Weapons/Summoner/GoozmoemEye");
            public static readonly Asset<Texture2D> GoozmoemCrown = AssetUtilities.RequestImmediate<Texture2D>(AssetPath + "Textures/Projectiles/Weapons/Summoner/GoozmoemCrown");
            public static readonly Asset<Texture2D> CometKunaiFlame = AssetUtilities.RequestImmediate<Texture2D>(AssetPath + "Textures/Projectiles/Weapons/Rogue/CometKunaiFlame");
            public static readonly Asset<Texture2D>[] FusionRay = AssetUtilities.RequestArrayImmediate<Texture2D>(GoozmaPrefix + "Projectiles/FusionRay_", 4);
        }

        public static class Inventors
        {
            public static readonly string InventorsPrefix = AssetPath + "Textures/NPCs/Bosses/Inventors/";

            //lol
        }

        public static class Buffs
        {
            public static readonly Asset<Texture2D>[] SlimeCane = AssetUtilities.RequestArrayImmediate<Texture2D>(AssetPath + "Textures/Buffs/SlimeCane_", 2);
        }

        public static class Bars
        {
            public static readonly Asset<Texture2D> Bar = AssetUtilities.RequestImmediate<Texture2D>(AssetPath + "Textures/ChargeBars/Style0_0");
            public static readonly Asset<Texture2D> BarCharge = AssetUtilities.RequestImmediate<Texture2D>(AssetPath + "Textures/ChargeBars/Style0_1");
            public static readonly Asset<Texture2D> Stress = AssetUtilities.RequestImmediate<Texture2D>(AssetPath + "Textures/ChargeBars/StressBar");
            public static readonly Asset<Texture2D> StressCharge = AssetUtilities.RequestImmediate<Texture2D>(AssetPath + "Textures/ChargeBars/StressBarFill");
            public static readonly Asset<Texture2D> StressTopped = AssetUtilities.RequestImmediate<Texture2D>(AssetPath + "Textures/ChargeBars/StressBarTopped");
        }

        public static class SlimeMonsoon
        {
            public static readonly Asset<Texture2D> MapBG = AssetUtilities.RequestImmediate<Texture2D>(AssetPath + "Textures/SlimeMonsoonBG");
            public static readonly Asset<Texture2D> Lightning = AssetUtilities.RequestImmediate<Texture2D>($"{nameof(CalamityHunt)}/Common/Graphics/SlimeMonsoon/Lightning");
            public static readonly Asset<Texture2D> LightningGlow = AssetUtilities.RequestImmediate<Texture2D>($"{nameof(CalamityHunt)}/Common/Graphics/SlimeMonsoon/LightningGlow");
        }
    }

    public static class Music
    {
        public static readonly int GlutinousArbitration = MusicLoader.GetMusicSlot(AssetPath + "Music/GlutinousArbitration");
        public static readonly int ViscousDesperation = MusicLoader.GetMusicSlot(AssetPath + "Music/ViscousDesperation");

        //auric soul shorts
        public static readonly int Iridescence = MusicLoader.GetMusicSlot(AssetPath + "Music/Souls/Iridescence");
        public static readonly int DraconicSoulUnnamedSong = MusicLoader.GetMusicSlot(AssetPath + "Music/Souls/YharonAuricSoulMusic");
    }

    public static class Sounds
    {
        public static class Goozma
        {
            public static readonly SoundStyle SlimeAbsorb = new(AssetPath + "Sounds/Goozma/GoozmaSlimeAbsorb", 8) { MaxInstances = 0, Volume = 0.1f };
            public static readonly SoundStyle Intro = new(AssetPath + "Sounds/Goozma/GoozmaIntro") { MaxInstances = 0, PlayOnlyIfFocused = true };
            public static readonly SoundStyle Awaken = new(AssetPath + "Sounds/Goozma/GoozmaAwaken") { MaxInstances = 0 };
            public static readonly SoundStyle Hurt = new(AssetPath + "Sounds/Goozma/GoozmaHurt", 1, 3) { MaxInstances = 0, PitchVariance = 0.1f };
            public static readonly SoundStyle SpawnSlime = new(AssetPath + "Sounds/Goozma/GoozmaSpawnSlime") { MaxInstances = 0 };
            public static readonly SoundStyle EyeAppear = new(AssetPath + "Sounds/Goozma/GoozmaEyeAppear") { MaxInstances = 0 };
            public static readonly SoundStyle Reform = new(AssetPath + "Sounds/Goozma/GoozmaReform") { MaxInstances = 0 };
            public static readonly SoundStyle Reawaken = new(AssetPath + "Sounds/Goozma/GoozmaReawaken") { MaxInstances = 0 };
            public static readonly SoundStyle Dash = new(AssetPath + "Sounds/Goozma/GoozmaDash", 1, 2) { MaxInstances = 0, PitchVariance = 0.15f };

            public static readonly SoundStyle SlimeShoot = new(AssetPath + "Sounds/Goozma/GoozmaSlimeShoot", 1, 3) { MaxInstances = 0, PitchVariance = 0.1f, Volume = 0.66f };
            public static readonly SoundStyle Dart = new(AssetPath + "Sounds/Goozma/GoozmaDartShoot", 1, 2) { MaxInstances = 0, PitchVariance = 0.2f };
            public static readonly SoundStyle Shot = new(AssetPath + "Sounds/Goozma/GoozmaShot", 1, 2) { MaxInstances = 0 };
            public static readonly SoundStyle RainbowShoot = new(AssetPath + "Sounds/Goozma/GoozmaRainbowShoot", 1, 2) { MaxInstances = 0, PitchVariance = 0.15f };
            public static readonly SoundStyle BombCharge = new(AssetPath + "Sounds/Goozma/GoozmaBombCharge") { MaxInstances = 0 };
            public static readonly SoundStyle BloatedBlastShoot = new(AssetPath + "Sounds/Goozma/GoozmaBloatedBlastShoot") { MaxInstances = 0 };
            public static readonly SoundStyle GoozmiteShoot = new(AssetPath + "Sounds/Goozma/GoozmiteShoot", 1, 2) { MaxInstances = 0 };
            public static readonly SoundStyle BigThunder = new(AssetPath + "Sounds/Goozma/GoozmaBigThunder") { MaxInstances = 0, PitchVariance = 0.15f };
            public static readonly SoundStyle SmallThunder = new(AssetPath + "Sounds/Goozma/GoozmaSmallThunder", 1, 3) { MaxInstances = 0, PitchVariance = 0.15f };

            public static readonly SoundStyle Sizzle = new(AssetPath + "Sounds/Goozma/GoozmaSizzle") { MaxInstances = 0 };
            public static readonly SoundStyle WarbleLoop = new(AssetPath + "Sounds/Goozma/GoozmaDeepWarbleLoop") { MaxInstances = 0, IsLooped = true };
            public static readonly SoundStyle ShootLoop = new(AssetPath + "Sounds/Goozma/GoozmaShootLoop") { MaxInstances = 0, IsLooped = true };
            public static readonly SoundStyle SimmerLoop = new(AssetPath + "Sounds/Goozma/GoozmaSimmerLoop") { MaxInstances = 0, IsLooped = true };
            public static readonly SoundStyle FusionRayLoop = new(AssetPath + "Sounds/Goozma/GoozmaFusionRayLoop") { MaxInstances = 0, IsLooped = true };

            public static readonly SoundStyle DeathBuildup = new(AssetPath + "Sounds/Goozma/GoozmaDeathBuildup") { MaxInstances = 0 };
            public static readonly SoundStyle Pop = new(AssetPath + "Sounds/Goozma/GoozmaPop") { MaxInstances = 0 };
            public static readonly SoundStyle Explode = new(AssetPath + "Sounds/Goozma/GoozmaExplode") { MaxInstances = 0 };
            public static readonly SoundStyle EarRinging = new(AssetPath + "Sounds/Goozma/GoozmaEarRinging") { MaxInstances = 0 };
            public static readonly SoundStyle WindLoop = new(AssetPath + "Sounds/Goozma/GoozmaWindLoop") { MaxInstances = 0, IsLooped = true };

            public static readonly SoundStyle SlimeJump = new(AssetPath + "Sounds/Goozma/GoozmaSlimeJump", 1, 2) { MaxInstances = 0, PitchVariance = 0.1f };
        }

        public static class Goozmite
        {
            public static readonly SoundStyle Ambient = new(AssetPath + "Sounds/Goozma/Goozmite/GoozmiteAmbient", 1, 3) { MaxInstances = 0, Volume = 0.5f };
            public static readonly SoundStyle Death = new(AssetPath + "Sounds/Goozma/Goozmite/GoozmiteDeath", 1, 3) { MaxInstances = 0 };
            public static readonly SoundStyle Impact = new(AssetPath + "Sounds/Goozma/Goozmite/GoozmiteImpact", 1, 3) { MaxInstances = 0 };
        }

        public static class GoozmaMinions
        {
            public static readonly SoundStyle SlimeSlam = new(AssetPath + "Sounds/Goozma/Slimes/GoozmaSlimeSlam", 1, 3) { MaxInstances = 0 };

            public static readonly SoundStyle CrimslimeTelegraph = new(AssetPath + "Sounds/Goozma/Slimes/CrimslimeTelegraph") { MaxInstances = 0 };

            public static readonly SoundStyle PrismDestroyerTelegraph = new(AssetPath + "Sounds/Goozma/Slimes/PrismDestroyerTelegraph") { MaxInstances = 0, PitchVariance = 0.1f };
            public static readonly SoundStyle PrismDestroyerExpand = new(AssetPath + "Sounds/Goozma/Slimes/PrismDestroyerExpand") { MaxInstances = 0, PitchVariance = 0.1f };
            public static readonly SoundStyle PixiePrismDestroyed = new(AssetPath + "Sounds/Goozma/Slimes/PixiePrismDestroyed") { MaxInstances = 0 };
            public static readonly SoundStyle PixieBallExplode = new(AssetPath + "Sounds/Goozma/Slimes/PixieBallExplode") { MaxInstances = 0 };
            public static readonly SoundStyle PixieBallBounce = new(AssetPath + "Sounds/Goozma/Slimes/PixieBallBounce") { MaxInstances = 0, PitchVariance = 0.1f };
            public static readonly SoundStyle PixieBallLoop = new(AssetPath + "Sounds/Goozma/Slimes/PixieBallLoop") { MaxInstances = 0, IsLooped = true };
            public static readonly SoundStyle Warning = new(AssetPath + "Sounds/Alarm") { MaxInstances = 0 };

            public static readonly SoundStyle EbonstoneRaise = new(AssetPath + "Sounds/Goozma/Slimes/EbonstoneRaise") { MaxInstances = 0 };
            public static readonly SoundStyle EbonstoneToothTelegraph = new(AssetPath + "Sounds/Goozma/Slimes/EbonstoneToothTelegraph") { MaxInstances = 0, PitchVariance = 0.2f };
            public static readonly SoundStyle EbonstoneToothEmerge = new(AssetPath + "Sounds/Goozma/Slimes/EbonstoneToothEmerge", 1, 2) { MaxInstances = 0, PitchVariance = 0.3f };
            public static readonly SoundStyle EbonstoneCrumble = new(AssetPath + "Sounds/Goozma/Slimes/EbonstoneCrumble") { MaxInstances = 0 };

            public static readonly SoundStyle StellarSlimeStarfallTelegraph = new(AssetPath + "Sounds/Goozma/Slimes/StellarSlimeStarfallTelegraph") { MaxInstances = 0 };
            public static readonly SoundStyle StellarSlimeImpact = new(AssetPath + "Sounds/Goozma/Slimes/StellarSlimeImpact") { MaxInstances = 0 };
            public static readonly SoundStyle StellarReform = new(AssetPath + "Sounds/Goozma/Slimes/StellarSlimeReform") { MaxInstances = 0 };
            public static readonly SoundStyle StellarBlackHoleSummon = new(AssetPath + "Sounds/Goozma/Slimes/StellarBlackHoleSummon") { MaxInstances = 0 };
            public static readonly SoundStyle StellarBlackHoleLoop = new(AssetPath + "Sounds/Goozma/Slimes/StellarBlackHoleLoop") { MaxInstances = 0, IsLooped = true };
            public static readonly SoundStyle StellarBlackHoleGulp = new(AssetPath + "Sounds/Goozma/Slimes/StellarSlimeBlackHoleGulp") { MaxInstances = 0 };
            public static readonly SoundStyle StellarConstellationWave = new(AssetPath + "Sounds/Goozma/Slimes/StellarSlimeConstellationWave") { MaxInstances = 0 };
            public static readonly SoundStyle StellarConstellationForm = new(AssetPath + "Sounds/Goozma/Slimes/StellarSlimeConstellationForm") { MaxInstances = 0 };
        }

        public static class Weapons
        {
            public static readonly SoundStyle CrystalGauntletClap = new(AssetPath + "Sounds/Weapons/CrystalGauntletClap") { MaxInstances = 2, PitchVariance = 0.1f };

            public static readonly SoundStyle ScytheOfTheOldGodSwing = new(AssetPath + "Sounds/Weapons/OldGodScytheSwing_" + 0) { MaxInstances = 0, PitchVariance = 0.1f };
            public static readonly SoundStyle ScytheOfTheOldGodSwingStrong = new(AssetPath + "Sounds/Weapons/OldGodScytheSwing_" + 1) { MaxInstances = 0, PitchVariance = 0.1f };

            public static readonly SoundStyle GoomoireWindLoop = new(AssetPath + "Sounds/Weapons/GoomoireWindLoop") { MaxInstances = 0, IsLooped = true };
            public static readonly SoundStyle SludgeShakerFiringLoop = new(AssetPath + "Sounds/Weapons/SludgeShakerFiringLoop") { MaxInstances = 0, IsLooped = true };
            public static readonly SoundStyle TrailBlazerFireStart = new(AssetPath + "Sounds/Weapons/TrailBlazerFireStart") { MaxInstances = 0, PitchVariance = 0.1f };
            public static readonly SoundStyle TrailblazerFireLoop = new(AssetPath + "Sounds/Weapons/TrailBlazerFireLoop") { MaxInstances = 0, IsLooped = true };
            public static readonly SoundStyle FissionFlyerExplode = new(AssetPath + "Sounds/Weapons/FissionFlyerExplode") { MaxInstances = 0 };
            public static readonly SoundStyle GoozmoemRay = new(AssetPath + "Sounds/Weapons/GoozmoemFusionRayShoot") { MaxInstances = 0, Volume = 0.8f };

            public static readonly SoundStyle AntiMassColliderLaserBlast = new(AssetPath + "Sounds/Weapons/AntiMassColliderLaserBlast") { MaxInstances = 0, Volume = 0.8f, PitchVariance = 0.15f };
            public static readonly SoundStyle AntiMassColliderFire = new(AssetPath + "Sounds/Weapons/AntiMassColliderFire") { MaxInstances = 0, Volume = 0.66f, PitchVariance = 0.05f };
            public static readonly SoundStyle ElectricLoop = new(AssetPath + "Sounds/Weapons/ElectricLoop") { MaxInstances = 0, IsLooped = true };
        }

        public static class Souls
        {
            public static readonly SoundStyle ChromaticSoulHeartbeat = new(AssetPath + "Sounds/GoozmaAuricSoulHeartbeat") { MaxInstances = 0, IsLooped = true };
            public static readonly SoundStyle ChromaticSoulDrone = new(AssetPath + "Sounds/GoozmaAuricSoulDrone") { MaxInstances = 0, IsLooped = true };
            public static readonly SoundStyle ChromaticSoulBreathe = new(AssetPath + "Sounds/GoozmaAuricSoulBreathe") { MaxInstances = 0, PitchVariance = 0.4f };

            public static readonly SoundStyle DragonSoulHeartbeat = new(AssetPath + "Sounds/YharonAuricSoulHeartbeat") { MaxInstances = 0, IsLooped = true };
            public static readonly SoundStyle DragonSoulDrone = new(AssetPath + "Sounds/YharonAuricSoulDrone") { MaxInstances = 0, Volume = 0.5f, IsLooped = true };
            public static readonly SoundStyle DragonSoulBreathe = new(AssetPath + "Sounds/YharonAuricSoulBreathe") { MaxInstances = 0, PitchVariance = 0.2f };
        }

        public static readonly SoundStyle TestLoop = new(AssetPath + "Sounds/TestLoop") { MaxInstances = 0, IsLooped = true };

        public static readonly SoundStyle MonsoonThunder = new(AssetPath + "Sounds/SlimeMonsoon/GoozmaMonsoonThunder", 3, SoundType.Ambient) { MaxInstances = 0 };
        public static readonly SoundStyle SlimeRainActivate = new(AssetPath + "Sounds/SlimeRainActivate") { MaxInstances = 0 };

        public static readonly SoundStyle GunCocking = new(AssetPath + "Sounds/Weapons/GunCocking") { MaxInstances = 0, PitchVariance = 0.02f };
        public static readonly SoundStyle SupremeRestorationBigGulp = new(AssetPath + "Sounds/SupremeRestorationBigGulp") { MaxInstances = 0, PitchVariance = 0.05f };
        public static readonly SoundStyle BloatBabyWarbleLoop = new(AssetPath + "Sounds/BabyBloatTravelLoop") { MaxInstances = 0, IsLooped = true };

        public static readonly SoundStyle StressActivate = new(AssetPath + "Sounds/GoozmaRageActivate") { MaxInstances = 0, Volume = 0.8f };
        public static readonly SoundStyle StressPing = new(AssetPath + "Sounds/GoozmaRageIndicator") { MaxInstances = 0, Volume = 0.65f };
        public static readonly SoundStyle StressLoop = new(AssetPath + "Sounds/GoozmaRageLoop") { MaxInstances = 0, Volume = 0.65f, IsLooped = true };
        public static readonly SoundStyle StressFull = new(AssetPath + "Sounds/GoozmaRageFull") { MaxInstances = 0, Volume = 0.75f };
    }

    public static class Effects
    {
        //Basic
        public static readonly Asset<Effect> BasicTrail = AssetUtilities.RequestImmediate<Effect>(AssetPath + "Effects/BasicTrail");
        public static readonly Asset<Effect> LightningBeam = AssetUtilities.RequestImmediate<Effect>(AssetPath + "Effects/LightningBeam");
        public static readonly Asset<Effect> FlameDissolve = AssetUtilities.RequestImmediate<Effect>(AssetPath + "Effects/FlameDissolve");

        //Goozma related
        public static readonly Asset<Effect> SlimeMonsoonOldCloudLayer = AssetUtilities.RequestImmediate<Effect>(AssetPath + "Effects/SlimeMonsoonOldCloudLayer");
        public static readonly Asset<Effect> SlimeMonsoonSkyEffect = AssetUtilities.RequestImmediate<Effect>(AssetPath + "Effects/SlimeMonsoonSkyEffect");
        public static readonly Asset<Effect> SlimeMonsoonDistortion = AssetUtilities.RequestImmediate<Effect>(AssetPath + "Effects/RadialDistortion");

        public static readonly Asset<Effect> GoozmaCordMap = AssetUtilities.RequestImmediate<Effect>(AssetPath + "Effects/GoozmaCordMap");
        public static readonly Asset<Effect> GooLightning = AssetUtilities.RequestImmediate<Effect>(AssetPath + "Effects/GooLightningEffect");
        public static readonly Asset<Effect> Cosmos = AssetUtilities.RequestImmediate<Effect>(AssetPath + "Effects/CosmosEffect");
        public static readonly Asset<Effect> StellarRing = AssetUtilities.RequestImmediate<Effect>(AssetPath + "Effects/StellarRing");
        public static readonly Asset<Effect> RainbowGel = AssetUtilities.RequestImmediate<Effect>(AssetPath + "Effects/RainbowGel");
        public static readonly Asset<Effect> HolographicGel = AssetUtilities.RequestImmediate<Effect>(AssetPath + "Effects/HolographEffect");
        public static readonly Asset<Effect> BlackHole = AssetUtilities.RequestImmediate<Effect>(AssetPath + "Effects/SpaceHole");

        public static readonly Asset<Effect> ShakerSludge = AssetUtilities.RequestImmediate<Effect>(AssetPath + "Effects/ShakerSludge");
        public static readonly Asset<Effect> CrystalLightning = AssetUtilities.RequestImmediate<Effect>(AssetPath + "Effects/CrystalLightningEffect");
        public static readonly Asset<Effect> FusionRay = AssetUtilities.RequestImmediate<Effect>(AssetPath + "Effects/FusionRayEffect");
        public static readonly Asset<Effect> GoomoireWind = AssetUtilities.RequestImmediate<Effect>(AssetPath + "Effects/GoomoireSuckEffect");

        public static class Dyes
        {
            public static readonly Asset<Effect> Goop = AssetUtilities.RequestImmediate<Effect>(AssetPath + "Effects/GoopDye");
            public static readonly Asset<Effect> Holograph = AssetUtilities.RequestImmediate<Effect>(AssetPath + "Effects/HolographDyeEffect");
        }
    }
}
