using CalamityHunt.Content.Bosses.Goozma;
using CalamityHunt.Content.Bosses.Goozma.Projectiles;
using CalamityHunt.Content.Items.Consumable;
using CalamityHunt.Content.Items.Dyes;
using CalamityHunt.Content.Pets.BloatBabyPet;
using CalamityHunt.Content.Projectiles;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using ReLogic.Utilities;
using System;
using System.Linq;
using Terraria;
using Terraria.Audio;
using Terraria.Graphics;
using Terraria.Graphics.Effects;
using Terraria.Graphics.Shaders;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityHunt.Common.Systems
{
    public class GoozmaSystem : ModSystem
    {
        public static bool conditionsMet;
        private Vector2 spawnPos;

        public override void PreUpdateNPCs()
        {
            conditionsMet = false;
            int slimeBoss = -1;
            bool king = true;
            if (Main.slimeRain && Main.hardMode)
            {
                foreach (NPC nPC in Main.npc.Where(n => (n.type == NPCID.KingSlime || n.type == NPCID.QueenSlimeBoss) && n.active))
                {
                    slimeBoss = nPC.whoAmI;
                    if (nPC.type == NPCID.QueenSlimeBoss)
                        king = false;
                    break;
                }
                if (slimeBoss > -1)
                {
                    Mod cal;
                    ModLoader.TryGetMod("CalamityMod", out cal);
                    if (cal != null)
                    {
                        foreach (Item item in Main.item.Where(n => n.active && n.type == cal.Find<ModItem>("OverloadedSludge").Type))
                            if (Main.npc[slimeBoss].Hitbox.Intersects(item.Hitbox))
                            {
                                item.active = false;
                                conditionsMet = true;
                            }
                    }
                    else
                    {
                        foreach (Item item in Main.item.Where(n => n.active && n.type == ModContent.ItemType<OverloadedSludge>()))
                            if (Main.npc[slimeBoss].Hitbox.Intersects(item.Hitbox))
                            {
                                spawnPos = item.Center;
                                item.active = false;
                                conditionsMet = true;
                            }
                    }
                }
            }

            if (conditionsMet)
            {
                if (!Main.dedServ)
                {
                    if (king)
                    {
                        for (int i = 0; i < 200; i++)
                        {
                            Dust slime = Dust.NewDustPerfect(Main.npc[slimeBoss].Center + Main.rand.NextVector2Circular(200, 110), 4, Main.rand.NextVector2Circular(15, 12) - Vector2.UnitY * 6f, 150, new Color(78, 136, 255, 80), 2f);
                            slime.noGravity = true;
                            slime.velocity *= 2f;
                        }
                    }
                    else
                    {
                        for (int i = 0; i < 200; i++)
                        {
                            Color qsColor = NPC.AI_121_QueenSlime_GetDustColor();
                            qsColor.A = 150;
                            Dust slime = Dust.NewDustPerfect(Main.npc[slimeBoss].Center + Main.rand.NextVector2Circular(200, 110), 4, Main.rand.NextVector2Circular(15, 12) - Vector2.UnitY * 6f, 50, qsColor, 2f);
                            slime.noGravity = true;
                            slime.velocity *= 2f;
                        }
                    }
                    SoundEngine.PlaySound(SoundID.NPCDeath1.WithPitchOffset(-0.5f), spawnPos);
                }

                Projectile.NewProjectile(Entity.GetSource_NaturalSpawn(), spawnPos, Vector2.Zero, ModContent.ProjectileType<GoozmaSpawn>(), 0, 0);
                Gore.NewGore(Entity.GetSource_NaturalSpawn(), Main.npc[slimeBoss].Top, -Vector2.UnitY, GoreID.KingSlimeCrown);
                Main.npc[slimeBoss].active = false;
            }

            StopSoundsIfNotActive();
        }

        public void StopSoundsIfNotActive()
        {
            if (!GoozmaActive)
            {
                bool warbleActive = SoundEngine.TryGetActiveSound(Goozma.goozmaWarble, out ActiveSound warbleSound);
                if (warbleActive)
                    warbleSound.Stop();

                bool shootActive = SoundEngine.TryGetActiveSound(Goozma.goozmaShoot, out ActiveSound shootSound);
                if (shootActive)
                    shootSound.Stop();                
                
                bool simmerActive = SoundEngine.TryGetActiveSound(Goozma.goozmaSimmer, out ActiveSound simmerSound);
                if (simmerActive)
                    simmerSound.Stop();

            }

            if (!Main.projectile.Any(n => n.active && n.type == ModContent.ProjectileType<BloatBabyProj>()))
            {
                bool active = SoundEngine.TryGetActiveSound(BloatBabyProj.travelSound, out ActiveSound sound);
                if (active)
                    sound.Stop();
            }
            
            if (!Main.projectile.Any(n => n.active && n.type == ModContent.ProjectileType<PixieBall>()))
            {
                bool active = SoundEngine.TryGetActiveSound(PixieBall.auraSound, out ActiveSound sound);
                if (active)
                    sound.Stop();
            }
            
            if (!Main.projectile.Any(n => n.active && n.type == ModContent.ProjectileType<GaussRay>()))
            {
                bool active = SoundEngine.TryGetActiveSound(GaussRay.laserSound, out ActiveSound sound);
                if (active)
                    sound.Stop();
            }            
            
            if (!Main.projectile.Any(n => n.active && n.type == ModContent.ProjectileType<BlackHoleBlender>()))
            {
                if (Filters.Scene["HuntOfTheOldGods:StellarBlackHole"].Active)
                {
                    float intensity = Filters.Scene["HuntOfTheOldGods:StellarBlackHole"].GetShader().Intensity;
                    Filters.Scene["HuntOfTheOldGods:StellarBlackHole"].GetShader().UseIntensity(intensity * 0.5f);
                    Filters.Scene["HuntOfTheOldGods:StellarBlackHole"].Deactivate();
                }

                bool active = SoundEngine.TryGetActiveSound(BlackHoleBlender.holeSound, out ActiveSound sound);
                if (active)
                    sound.Stop();                
                bool windActive = SoundEngine.TryGetActiveSound(BlackHoleBlender.windSound, out ActiveSound windSound);
                if (windActive)
                    windSound.Stop();
            }
        }

        public static bool GoozmaActive => Main.npc.Any(n => n.type == ModContent.NPCType<Goozma>() && n.active) || Main.projectile.Any(n => n.type == ModContent.ProjectileType<GoozmaSpawn>() && n.active);       
    }
}
