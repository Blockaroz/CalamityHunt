using CalamityHunt.Content.Bosses.Goozma;
using CalamityHunt.Content.Bosses.Goozma.Projectiles;
using CalamityHunt.Content.Items.Consumable;
using CalamityHunt.Content.Items.Dyes;
using CalamityHunt.Content.Projectiles;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
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
            int ks = -1;
            if (Main.slimeRain && Main.hardMode)
            {
                foreach (NPC nPC in Main.npc.Where(n => n.type == NPCID.KingSlime && n.active))
                    ks = nPC.whoAmI;
                if (ks > -1)
                {
                    Mod cal;
                    ModLoader.TryGetMod("CalamityMod", out cal);
                    if (cal != null)
                    {
                        foreach (Item item in Main.item.Where(n => n.active && n.type == cal.Find<ModItem>("OverloadedSludge").Type))
                            if (Main.npc[ks].Hitbox.Intersects(item.Hitbox))
                            {
                                item.active = false;
                                conditionsMet = true;
                            }
                    }
                    else
                    {
                        foreach (Item item in Main.item.Where(n => n.active && n.type == ModContent.ItemType<OverloadedSludge>()))
                            if (Main.npc[ks].Hitbox.Intersects(item.Hitbox))
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
                    for (int i = 0; i < 200; i++)
                        Dust.NewDustPerfect(Main.npc[ks].Center, 4, Main.rand.NextVector2Circular(15, 5), 150, new Color(78, 136, 255, 80), 2f);
                   
                    SoundEngine.PlaySound(SoundID.NPCDeath1.WithPitchOffset(-0.5f), spawnPos);
                }

                Projectile.NewProjectile(Entity.GetSource_NaturalSpawn(), spawnPos, Vector2.Zero, ModContent.ProjectileType<GoozmaSpawn>(), 0, 0);
                Gore.NewGore(Entity.GetSource_NaturalSpawn(), Main.npc[ks].Top, -Vector2.UnitY, GoreID.KingSlimeCrown);
                Main.npc[ks].active = false;
            }
        }

        public static bool GoozmaActive => Main.npc.Any(n => n.type == ModContent.NPCType<Goozma>() && n.active) || Main.projectile.Any(n => n.type == ModContent.ProjectileType<GoozmaSpawn>() && n.active);       
    }
}
