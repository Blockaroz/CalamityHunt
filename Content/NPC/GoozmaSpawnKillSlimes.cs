using Terraria;
using Terraria.ModLoader;
using Microsoft.Xna.Framework;
using Terraria.ID;
using CalamityHunt.Common.Systems.Particles;
using CalamityHunt.Content.Particles.FlyingSlimes;
using CalamityHunt.Content.Projectiles;

namespace CalamityHunt.Content.NPCs
{
    public class SlimeSucking : GlobalNPC
    {
        public override bool InstancePerEntity => true;
        public override void PostAI(NPC npc)
        {
            for (int i = 0; i < Main.maxProjectiles; i++)
            {
                Projectile proj = Main.projectile[i];
                if (proj.active && proj.type == ModContent.ProjectileType<GoozmaSpawn>())
                {
                    Vector2 velocity = npc.Center.DirectionTo(proj.Center).SafeNormalize(Vector2.One).RotatedByRandom(3.0);
                    switch (npc.type)
                    {
                        case NPCID.BlueSlime:
                            Slime(proj, npc, velocity, Particle.ParticleType<FlyingNormalSlime>());
                            break;
                        case NPCID.GreenSlime:
                            Slime(proj, npc, velocity, Particle.ParticleType<FlyingNormalSlime>());
                            break;
                        case NPCID.Pinky:
                            Slime(proj, npc, velocity, Particle.ParticleType<FlyingNormalSlime>());
                            break;
                        case NPCID.RedSlime:
                            Slime(proj, npc, velocity, Particle.ParticleType<FlyingNormalSlime>());
                            break;
                        case NPCID.JungleSlime:
                            Slime(proj, npc, velocity, Particle.ParticleType<FlyingNormalSlime>());
                            break;
                        case NPCID.BlackSlime:
                            Slime(proj, npc, velocity, Particle.ParticleType<FlyingNormalSlime>());
                            break;
                        case NPCID.BabySlime:
                            Slime(proj, npc, velocity, Particle.ParticleType<FlyingNormalSlime>());
                            break;
                        case NPCID.PurpleSlime:
                            Slime(proj, npc, velocity, Particle.ParticleType<FlyingBigSlime>());
                            break;
                        case NPCID.YellowSlime:
                            Slime(proj, npc, velocity, Particle.ParticleType<FlyingBigSlime>());
                            break;
                        case NPCID.MotherSlime:
                            Slime(proj, npc, velocity, Particle.ParticleType<FlyingBigSlime>());
                            break;
                        case NPCID.WindyBalloon:
                            NPC npc2 = npc.AI_113_WindyBalloon_GetSlaveNPC();
                            switch (npc2.type)
                            {
                                case NPCID.BlueSlime:
                                    Balloon(proj, npc2, velocity, Particle.ParticleType<FlyingBalloonSlime>(), npc.frame.Y);
                                    break;
                                case NPCID.GreenSlime:
                                    Balloon(proj, npc2, velocity, Particle.ParticleType<FlyingBalloonSlime>(), npc.frame.Y);
                                    break;
                                case NPCID.PurpleSlime:
                                    Balloon(proj, npc2, velocity, Particle.ParticleType<FlyingBalloonSlime>(), npc.frame.Y);
                                    break;
                                case NPCID.Pinky:
                                    Balloon(proj, npc2, velocity, Particle.ParticleType<FlyingBalloonSlime>(), npc.frame.Y);
                                    break;
                                default:
                                    break;
                            }
                            npc.life = 0;
                            break;
                        case NPCID.RainbowSlime:
                            Slime2(proj, npc, velocity, Particle.ParticleType<FlyingRainbowSlime>());
                            break;
                        case NPCID.Gastropod:
                            Slime2(proj, npc, velocity, Particle.ParticleType<FlyingGastropod>());
                            break;
                        case NPCID.IlluminantSlime:
                            Slime2(proj, npc, velocity, Particle.ParticleType<FlyingIlluminantSlime>());
                            break;
                        case NPCID.LavaSlime:
                            Slime2(proj, npc, velocity, Particle.ParticleType<FlyingLavaSlime>());
                            break;
                        case NPCID.SlimedZombie:
                            Slime2(proj, npc, velocity, Particle.ParticleType<FlyingZombieSlime>());
                            break;
                        case NPCID.ArmedZombieSlimed:
                            Slime2(proj, npc, velocity, Particle.ParticleType<FlyingZombieSlime>());
                            break;
                        case NPCID.ShimmerSlime:
                            Slime2(proj, npc, velocity, Particle.ParticleType<FlyingShimmerSlime>());
                            break;
                        case NPCID.IceSlime:
                            Ice(proj, npc, velocity, Particle.ParticleType<FlyingIceSlime>(), false);
                            break;
                        case NPCID.SandSlime:
                            Slime2(proj, npc, velocity, Particle.ParticleType<FlyingSandSlime>());
                            break;
                        case NPCID.SpikedJungleSlime:
                            Slime2(proj, npc, velocity, Particle.ParticleType<FlyingJungleSlimeSpiked>());
                            break;
                        case NPCID.SpikedIceSlime:
                            Ice(proj, npc, velocity, Particle.ParticleType<FlyingIceSlime>(), true);
                            break;
                        case NPCID.SlimeSpiked:
                            Slime2(proj, npc, velocity, Particle.ParticleType<FlyingSpikedSlime>());
                            break;
                        case NPCID.QueenSlimeMinionPink:
                            Slime2(proj, npc, velocity, Particle.ParticleType<FlyingBouncySlime>());
                            break;
                        case NPCID.QueenSlimeMinionBlue:
                            Slime2(proj, npc, velocity, Particle.ParticleType<FlyingCrystalSlime>());
                            break;
                        case NPCID.QueenSlimeMinionPurple:
                            Slime2(proj, npc, velocity, Particle.ParticleType<FlyingHeavenlySlime>());
                            break;
                        case NPCID.UmbrellaSlime:
                            Slime2(proj, npc, velocity, Particle.ParticleType<FlyingUmbrellaSlime>());
                            break;
                        case NPCID.CorruptSlime:
                            Slime2(proj, npc, velocity, Particle.ParticleType<FlyingCorruptSlime>());
                            break;
                        case NPCID.Slimeling:
                            Slime2(proj, npc, velocity, Particle.ParticleType<FlyingCorruptSlime>());
                            break;
                        case NPCID.Slimer:
                            Slime2(proj, npc, velocity, Particle.ParticleType<FlyingSlimer>());
                            break;
                        case NPCID.Slimer2:
                            Slime2(proj, npc, velocity, Particle.ParticleType<FlyingCorruptSlime>());
                            break;
                        case NPCID.Crimslime:
                            Slime2(proj, npc, velocity, Particle.ParticleType<FlyingCrimslime>());
                            break;
                        case NPCID.ToxicSludge:
                            Slime2(proj, npc, velocity, Particle.ParticleType<FlyingToxicSludge>());
                            break;
                        case NPCID.DungeonSlime:
                            Slime2(proj, npc, velocity, Particle.ParticleType<FlyingDungeonSlime>());
                            break;
                        case NPCID.HoppinJack:
                            Slime2(proj, npc, velocity, Particle.ParticleType<FlyingHoppinJack>());
                            break;
                        case NPCID.GoldenSlime:
                            Slime2(proj, npc, velocity, Particle.ParticleType<FlyingGoldSlime>());
                            break;
                        case NPCID.BunnySlimed:
                            Slime2(proj, npc, velocity, Particle.ParticleType<FlyingBunnySlime>());
                            break;
                        case NPCID.SlimeMasked:
                            Slime2(proj, npc, velocity, Particle.ParticleType<FlyingSlimeBunny>());
                            break;
                        case NPCID.SlimeRibbonRed:
                            Ribbon(proj, npc, velocity, Particle.ParticleType<FlyingPresentSlime>(), 0);
                            break;
                        case NPCID.SlimeRibbonGreen:
                            Ribbon(proj, npc, velocity, Particle.ParticleType<FlyingPresentSlime>(), 1);
                            break;
                        case NPCID.SlimeRibbonYellow:
                            Ribbon(proj, npc, velocity, Particle.ParticleType<FlyingPresentSlime>(), 2);
                            break;
                        case NPCID.SlimeRibbonWhite:
                            Ribbon(proj, npc, velocity, Particle.ParticleType<FlyingPresentSlime>(), 3);
                            break;
                        default:
                            break;
                    }
                    if (ModLoader.HasMod("CalamityMod"))
                    {
                        if (npc.type == ModContent.Find<ModNPC>("CalamityMod/AeroSlime").Type)
                            Slime2(proj, npc, velocity, Particle.ParticleType<FlyingAeroSlime>());
                        else if (npc.type == ModContent.Find<ModNPC>("CalamityMod/EbonianBlightSlime").Type)
                            Slime2(proj, npc, velocity, Particle.ParticleType<FlyingEbonianBlightSlime>());
                        else if (npc.type == ModContent.Find<ModNPC>("CalamityMod/CrimulanBlightSlime").Type)
                            Slime2(proj, npc, velocity, Particle.ParticleType<FlyingCrimulanBlightSlime>());
                        else if (npc.type == ModContent.Find<ModNPC>("CalamityMod/CorruptSlimeSpawn").Type)
                            Slime2(proj, npc, velocity, Particle.ParticleType<FlyingCorruptSlimeSpawn>());
                        else if (npc.type == ModContent.Find<ModNPC>("CalamityMod/CorruptSlimeSpawn2").Type)
                            Slime2(proj, npc, velocity, Particle.ParticleType<FlyingCorruptSlimeSpawn>());
                        else if (npc.type == ModContent.Find<ModNPC>("CalamityMod/CrimsonSlimeSpawn").Type)
                            Slime2(proj, npc, velocity, Particle.ParticleType<FlyingCrimsonSlimeSpawn>());
                        else if (npc.type == ModContent.Find<ModNPC>("CalamityMod/CrimsonSlimeSpawn2").Type)
                            Slime2(proj, npc, velocity, Particle.ParticleType<FlyingCrimsonSlimeSpawn>());
                        else if (npc.type == ModContent.Find<ModNPC>("CalamityMod/AstralSlime").Type)
                            Slime2(proj, npc, velocity, Particle.ParticleType<FlyingAstralSlime>());
                        else if (npc.type == ModContent.Find<ModNPC>("CalamityMod/CryoSlime").Type)
                            Slime2(proj, npc, velocity, Particle.ParticleType<FlyingCryoSlime>());
                        else if (npc.type == ModContent.Find<ModNPC>("CalamityMod/IrradiatedSlime").Type)
                            Slime2(proj, npc, velocity, Particle.ParticleType<FlyingIrradiatedSlime>());
                        else if (npc.type == ModContent.Find<ModNPC>("CalamityMod/InfernalCongealment").Type)
                            Slime2(proj, npc, velocity, Particle.ParticleType<FlyingCharredSlime>());
                        else if (npc.type == ModContent.Find<ModNPC>("CalamityMod/PerennialSlime").Type)
                            Slime2(proj, npc, velocity, Particle.ParticleType<FlyingPerennialSlime>());
                        else if (npc.type == ModContent.Find<ModNPC>("CalamityMod/AureusSpawn").Type)
                            Slime2(proj, npc, velocity, Particle.ParticleType<AureusSpawn>());
                        else if (npc.type == ModContent.Find<ModNPC>("CalamityMod/PestilentSlime").Type)
                            Slime2(proj, npc, velocity, Particle.ParticleType<FlyingPestilentSlime>());
                        else if (npc.type == ModContent.Find<ModNPC>("CalamityMod/BloomSlime").Type)
                            Slime2(proj, npc, velocity, Particle.ParticleType<FlyingBloomSlime>());
                        else if (npc.type == ModContent.Find<ModNPC>("CalamityMod/GammaSlime").Type)
                            Slime2(proj, npc, velocity, Particle.ParticleType<FlyingGammaSlime>());
                        else if (npc.type == ModContent.Find<ModNPC>("CalamityMod/CragmawMire").Type)
                            Slime2(proj, npc, velocity, Particle.ParticleType<FlyingCragmawMire>());
                    }
                }
            }
        }
        private static void Slime(Projectile proj, NPC npc, Vector2 velocity, int type)
        {
            Particle particle = Particle.NewParticle(type, npc.Center, velocity, npc.color, npc.scale);
            FlyingSlime slime = particle as FlyingSlime;
            slime.time = 20;
            particle.data = proj.Center;
            particle.behindEntities = true;
            npc.life = 0;
        }
        private static void Slime2(Projectile proj, NPC npc, Vector2 velocity, int type)
        {
            Particle particle = Particle.NewParticle(type, npc.Center, velocity, Color.White, npc.scale);
            FlyingSlime slime = particle as FlyingSlime;
            slime.time = 20;
            particle.data = proj.Center;
            particle.behindEntities = true;
            npc.life = 0;
        }
        private static void Ice(Projectile proj, NPC npc, Vector2 velocity, int type, bool ice)
        {
            Particle particle = Particle.NewParticle(type, npc.Center, velocity, Color.White, npc.scale);
            FlyingSlime slime = particle as FlyingSlime;
            slime.time = 20;
            FlyingIceSlime iced = particle as FlyingIceSlime;
            iced.spiked = ice;
            particle.data = proj.Center;
            particle.behindEntities = true;
            npc.life = 0;
        }
        private static void Balloon(Projectile proj, NPC npc, Vector2 velocity, int type, int frame)
        {
            Particle particle = Particle.NewParticle(type, npc.Center, velocity, npc.color, npc.scale);
            FlyingSlime slime = particle as FlyingSlime;
            slime.time = 20;
            FlyingBalloonSlime ball = particle as FlyingBalloonSlime;
            ball.balloonVariant = frame - 1;
            particle.data = proj.Center;
            particle.behindEntities = true;
            npc.life = 0;
        }
        private static void Ribbon(Projectile proj, NPC npc, Vector2 velocity, int type, int variant)
        {
            Particle particle = Particle.NewParticle(type, npc.Center, velocity, Color.White, npc.scale);
            FlyingSlime slime = particle as FlyingSlime;
            slime.time = 20;
            FlyingPresentSlime present = particle as FlyingPresentSlime;
            present.variant = variant;
            particle.data = proj.Center;
            particle.behindEntities = true;
            npc.life = 0;
        }
    }
}
