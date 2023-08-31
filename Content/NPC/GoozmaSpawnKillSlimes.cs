using Arch.Core.Extensions;
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
                            Slime(proj, npc, velocity, ModContent.GetInstance<FlyingNormalSlime>());
                            break;
                        case NPCID.GreenSlime:
                            Slime(proj, npc, velocity, ModContent.GetInstance<FlyingNormalSlime>());
                            break;
                        case NPCID.Pinky:
                            Slime(proj, npc, velocity, ModContent.GetInstance<FlyingNormalSlime>());
                            break;
                        case NPCID.RedSlime:
                            Slime(proj, npc, velocity, ModContent.GetInstance<FlyingNormalSlime>());
                            break;
                        case NPCID.JungleSlime:
                            Slime(proj, npc, velocity, ModContent.GetInstance<FlyingNormalSlime>());
                            break;
                        case NPCID.BlackSlime:
                            Slime(proj, npc, velocity, ModContent.GetInstance<FlyingNormalSlime>());
                            break;
                        case NPCID.BabySlime:
                            Slime(proj, npc, velocity, ModContent.GetInstance<FlyingNormalSlime>());
                            break;
                        case NPCID.PurpleSlime:
                            Slime(proj, npc, velocity, ModContent.GetInstance<FlyingBigSlime>());
                            break;
                        case NPCID.YellowSlime:
                            Slime(proj, npc, velocity, ModContent.GetInstance<FlyingBigSlime>());
                            break;
                        case NPCID.MotherSlime:
                            Slime(proj, npc, velocity, ModContent.GetInstance<FlyingBigSlime>());
                            break;
                        case NPCID.WindyBalloon:
                            NPC npc2 = npc.AI_113_WindyBalloon_GetSlaveNPC();
                            switch (npc2.type)
                            {
                                case NPCID.BlueSlime:
                                    Balloon(proj, npc2, velocity, ModContent.GetInstance<FlyingBalloonSlime>(), npc.frame.Y);
                                    break;
                                case NPCID.GreenSlime:
                                    Balloon(proj, npc2, velocity, ModContent.GetInstance<FlyingBalloonSlime>(), npc.frame.Y);
                                    break;
                                case NPCID.PurpleSlime:
                                    Balloon(proj, npc2, velocity, ModContent.GetInstance<FlyingBalloonSlime>(), npc.frame.Y);
                                    break;
                                case NPCID.Pinky:
                                    Balloon(proj, npc2, velocity, ModContent.GetInstance<FlyingBalloonSlime>(), npc.frame.Y);
                                    break;
                                default:
                                    break;
                            }
                            npc.life = 0;
                            break;
                        case NPCID.RainbowSlime:
                            Slime2(proj, npc, velocity, ModContent.GetInstance<FlyingRainbowSlime>());
                            break;
                        case NPCID.Gastropod:
                            Slime2(proj, npc, velocity, ModContent.GetInstance<FlyingGastropod>());
                            break;
                        case NPCID.IlluminantSlime:
                            Slime2(proj, npc, velocity, ModContent.GetInstance<FlyingIlluminantSlime>());
                            break;
                        case NPCID.LavaSlime:
                            Slime2(proj, npc, velocity, ModContent.GetInstance<FlyingLavaSlime>());
                            break;
                        case NPCID.SlimedZombie:
                            Slime2(proj, npc, velocity, ModContent.GetInstance<FlyingZombieSlime>());
                            break;
                        case NPCID.ArmedZombieSlimed:
                            Slime2(proj, npc, velocity, ModContent.GetInstance<FlyingZombieSlime>());
                            break;
                        case NPCID.ShimmerSlime:
                            Slime2(proj, npc, velocity, ModContent.GetInstance<FlyingShimmerSlime>());
                            break;
                        case NPCID.IceSlime:
                            Ice(proj, npc, velocity, ModContent.GetInstance<FlyingIceSlime>(), false);
                            break;
                        case NPCID.SandSlime:
                            Slime2(proj, npc, velocity, ModContent.GetInstance<FlyingSandSlime>());
                            break;
                        case NPCID.SpikedJungleSlime:
                            Slime2(proj, npc, velocity, ModContent.GetInstance<FlyingJungleSlimeSpiked>());
                            break;
                        case NPCID.SpikedIceSlime:
                            Ice(proj, npc, velocity, ModContent.GetInstance<FlyingIceSlime>(), true);
                            break;
                        case NPCID.SlimeSpiked:
                            Slime2(proj, npc, velocity, ModContent.GetInstance<FlyingSpikedSlime>());
                            break;
                        case NPCID.QueenSlimeMinionPink:
                            Slime2(proj, npc, velocity, ModContent.GetInstance<FlyingBouncySlime>());
                            break;
                        case NPCID.QueenSlimeMinionBlue:
                            Slime2(proj, npc, velocity, ModContent.GetInstance<FlyingCrystalSlime>());
                            break;
                        case NPCID.QueenSlimeMinionPurple:
                            Slime2(proj, npc, velocity, ModContent.GetInstance<FlyingHeavenlySlime>());
                            break;
                        case NPCID.UmbrellaSlime:
                            Slime2(proj, npc, velocity, ModContent.GetInstance<FlyingUmbrellaSlime>());
                            break;
                        case NPCID.CorruptSlime:
                            Slime2(proj, npc, velocity, ModContent.GetInstance<FlyingCorruptSlime>());
                            break;
                        case NPCID.Slimeling:
                            Slime2(proj, npc, velocity, ModContent.GetInstance<FlyingCorruptSlime>());
                            break;
                        case NPCID.Slimer:
                            Slime2(proj, npc, velocity, ModContent.GetInstance<FlyingSlimer>());
                            break;
                        case NPCID.Slimer2:
                            Slime2(proj, npc, velocity, ModContent.GetInstance<FlyingCorruptSlime>());
                            break;
                        case NPCID.Crimslime:
                            Slime2(proj, npc, velocity, ModContent.GetInstance<FlyingCrimslime>());
                            break;
                        case NPCID.ToxicSludge:
                            Slime2(proj, npc, velocity, ModContent.GetInstance<FlyingToxicSludge>());
                            break;
                        case NPCID.DungeonSlime:
                            Slime2(proj, npc, velocity, ModContent.GetInstance<FlyingDungeonSlime>());
                            break;
                        case NPCID.HoppinJack:
                            Slime2(proj, npc, velocity, ModContent.GetInstance<FlyingHoppinJack>());
                            break;
                        case NPCID.GoldenSlime:
                            Slime2(proj, npc, velocity, ModContent.GetInstance<FlyingGoldSlime>());
                            break;
                        case NPCID.BunnySlimed:
                            Slime2(proj, npc, velocity, ModContent.GetInstance<FlyingBunnySlime>());
                            break;
                        case NPCID.SlimeMasked:
                            Slime2(proj, npc, velocity, ModContent.GetInstance<FlyingSlimeBunny>());
                            break;
                        case NPCID.SlimeRibbonRed:
                            Ribbon(proj, npc, velocity, ModContent.GetInstance<FlyingPresentSlime>(), 0);
                            break;
                        case NPCID.SlimeRibbonGreen:
                            Ribbon(proj, npc, velocity, ModContent.GetInstance<FlyingPresentSlime>(), 1);
                            break;
                        case NPCID.SlimeRibbonYellow:
                            Ribbon(proj, npc, velocity, ModContent.GetInstance<FlyingPresentSlime>(), 2);
                            break;
                        case NPCID.SlimeRibbonWhite:
                            Ribbon(proj, npc, velocity, ModContent.GetInstance<FlyingPresentSlime>(), 3);
                            break;
                        default:
                            break;
                    }
                    if (ModLoader.HasMod("CalamityMod"))
                    {
                        if (npc.type == ModContent.Find<ModNPC>("CalamityMod/AeroSlime").Type)
                            Slime2(proj, npc, velocity, ModContent.GetInstance<FlyingAeroSlime>());
                        else if (npc.type == ModContent.Find<ModNPC>("CalamityMod/EbonianBlightSlime").Type)
                            Slime2(proj, npc, velocity, ModContent.GetInstance<FlyingEbonianBlightSlime>());
                        else if (npc.type == ModContent.Find<ModNPC>("CalamityMod/CrimulanBlightSlime").Type)
                            Slime2(proj, npc, velocity, ModContent.GetInstance<FlyingCrimulanBlightSlime>());
                        else if (npc.type == ModContent.Find<ModNPC>("CalamityMod/CorruptSlimeSpawn").Type)
                            Slime2(proj, npc, velocity, ModContent.GetInstance<FlyingCorruptSlimeSpawn>());
                        else if (npc.type == ModContent.Find<ModNPC>("CalamityMod/CorruptSlimeSpawn2").Type)
                            Slime2(proj, npc, velocity, ModContent.GetInstance<FlyingCorruptSlimeSpawn>());
                        else if (npc.type == ModContent.Find<ModNPC>("CalamityMod/CrimsonSlimeSpawn").Type)
                            Slime2(proj, npc, velocity, ModContent.GetInstance<FlyingCrimsonSlimeSpawn>());
                        else if (npc.type == ModContent.Find<ModNPC>("CalamityMod/CrimsonSlimeSpawn2").Type)
                            Slime2(proj, npc, velocity, ModContent.GetInstance<FlyingCrimsonSlimeSpawn>());
                        else if (npc.type == ModContent.Find<ModNPC>("CalamityMod/AstralSlime").Type)
                            Slime2(proj, npc, velocity, ModContent.GetInstance<FlyingAstralSlime>());
                        else if (npc.type == ModContent.Find<ModNPC>("CalamityMod/CryoSlime").Type)
                            Slime2(proj, npc, velocity, ModContent.GetInstance<FlyingCryoSlime>());
                        else if (npc.type == ModContent.Find<ModNPC>("CalamityMod/IrradiatedSlime").Type)
                            Slime2(proj, npc, velocity, ModContent.GetInstance<FlyingIrradiatedSlime>());
                        else if (npc.type == ModContent.Find<ModNPC>("CalamityMod/InfernalCongealment").Type)
                            Slime2(proj, npc, velocity, ModContent.GetInstance<FlyingCharredSlime>());
                        else if (npc.type == ModContent.Find<ModNPC>("CalamityMod/PerennialSlime").Type)
                            Slime2(proj, npc, velocity, ModContent.GetInstance<FlyingPerennialSlime>());
                        else if (npc.type == ModContent.Find<ModNPC>("CalamityMod/AureusSpawn").Type)
                            Slime2(proj, npc, velocity, ModContent.GetInstance<AureusSpawn>());
                        else if (npc.type == ModContent.Find<ModNPC>("CalamityMod/PestilentSlime").Type)
                            Slime2(proj, npc, velocity, ModContent.GetInstance<FlyingPestilentSlime>());
                        else if (npc.type == ModContent.Find<ModNPC>("CalamityMod/BloomSlime").Type)
                            Slime2(proj, npc, velocity, ModContent.GetInstance<FlyingBloomSlime>());
                        else if (npc.type == ModContent.Find<ModNPC>("CalamityMod/GammaSlime").Type)
                            Slime2(proj, npc, velocity, ModContent.GetInstance<FlyingGammaSlime>());
                        else if (npc.type == ModContent.Find<ModNPC>("CalamityMod/CragmawMire").Type)
                            Slime2(proj, npc, velocity, ModContent.GetInstance<FlyingCragmawMire>());
                    }
                    if (ModLoader.HasMod("CatalystMod"))
                    {
                        if (npc.type == ModContent.Find<ModNPC>("CatalystMod/NovaSlime").Type)
                            Slime2(proj, npc, velocity, ModContent.GetInstance<FlyingNovaSlime>());
                        else if (npc.type == ModContent.Find<ModNPC>("CatalystMod/NovaSlimer").Type)
                            Slime2(proj, npc, velocity, ModContent.GetInstance<FlyingNovaSlimer>());
                        else if (npc.type == ModContent.Find<ModNPC>("CatalystMod/MetanovaSlime").Type)
                            Slime2(proj, npc, velocity, ModContent.GetInstance<FlyingMetanovaSlime>());
                        else if (npc.type == ModContent.Find<ModNPC>("CatalystMod/WulfrumSlime").Type)
                            Slime2(proj, npc, velocity, ModContent.GetInstance<FlyingWulfrumSlime>());
                        else if (npc.type == ModContent.Find<ModNPC>("CatalystMod/AscendedAstralSlime").Type)
                            Slime2(proj, npc, velocity, ModContent.GetInstance<FlyingAscendedAstralSlime>());
                    }
                }
            }
        }
        private static void Slime(Projectile proj, NPC npc, Vector2 velocity, ParticleBehavior particleBehavior)
        {
            var entity = ParticleBehavior.NewParticle(particleBehavior, npc.Center, velocity, npc.color, npc.scale);
            entity.Add(new ParticleData<Vector2> { Value = proj.Center }, new ParticleDrawBehindEntities(), new ParticleFlyingSlime { Time = 20 });
            npc.life = 0;
        }
        private static void Slime2(Projectile proj, NPC npc, Vector2 velocity, ParticleBehavior particleBehavior)
        {
            var entity = ParticleBehavior.NewParticle(particleBehavior, npc.Center, velocity, Color.White, npc.scale);
            entity.Add(new ParticleData<Vector2> { Value = proj.Center }, new ParticleDrawBehindEntities(), new ParticleFlyingSlime { Time = 20 });
            npc.life = 0;
        }
        private static void Ice(Projectile proj, NPC npc, Vector2 velocity, ParticleBehavior particleBehavior, bool ice)
        {
            var entity = ParticleBehavior.NewParticle(particleBehavior, npc.Center, velocity, Color.White, npc.scale);
            entity.Add(new ParticleData<Vector2> { Value = proj.Center }, new ParticleDrawBehindEntities(), new ParticleFlyingSlime { Time = 20 }, new ParticleFlyingIceSlime { Spiked = ice });
            npc.life = 0;
        }
        private static void Balloon(Projectile proj, NPC npc, Vector2 velocity, ParticleBehavior particleBehavior, int frame)
        {
            var entity = ParticleBehavior.NewParticle(particleBehavior, npc.Center, velocity, npc.color, npc.scale);
            entity.Add(new ParticleData<Vector2> { Value = proj.Center }, new ParticleDrawBehindEntities(), new ParticleFlyingSlime { Time = 20 }, new ParticleFlyingBalloonSlime { BalloonVariant = frame - 1});
            npc.life = 0;
        }
        private static void Ribbon(Projectile proj, NPC npc, Vector2 velocity, ParticleBehavior particleBehavior, int variant)
        {
            var entity = ParticleBehavior.NewParticle(particleBehavior, npc.Center, velocity, Color.White, npc.scale);
            entity.Add(new ParticleData<Vector2> { Value = proj.Center }, new ParticleDrawBehindEntities(), new ParticleFlyingSlime { Time = 20 }, new ParticleFlyingPresentSlime { Variant = variant });
            npc.life = 0;
        }
    }
}
