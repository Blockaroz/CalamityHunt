using CalamityHunt.Content.Bosses.Goozma.Slimes;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using System;
using System.Linq;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityHunt.Content.Bosses.Goozma.Projectiles
{
    public class PixieBall : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.TrailCacheLength[Type] = 20;
            ProjectileID.Sets.TrailingMode[Type] = -1;
        }

        public override void SetDefaults()
        {
            Projectile.width = 48;
            Projectile.height = 48;
            Projectile.tileCollide = false;
            Projectile.hostile = false;
            Projectile.friendly = false;
            Projectile.penetrate = -1;
            Projectile.aiStyle = -1;
        }

        public ref float Time => ref Projectile.ai[0];
        public ref float HitCount => ref Projectile.ai[2];

        public override void AI()
        {
            Projectile.rotation += Projectile.velocity.Length() * 0.01f * Math.Sign(Projectile.velocity.X);
            Projectile.scale = (float)Math.Sqrt(Utils.GetLerpValue(0, 17, Time, true) * Utils.GetLerpValue(480, 440, Time, true)) * 2f;
            int owner = -1;
            if (!Main.npc.Any(n => n.type == ModContent.NPCType<DivineGargooptuar>() && n.active))
            {
                Projectile.active = false;
                return;
            }
            else
                owner = Main.npc.First(n => n.type == ModContent.NPCType<DivineGargooptuar>() && n.active).whoAmI;

            if (Projectile.ai[1] == 0)
            {
                foreach (Player player in Main.player.Where(n => n.active && !n.dead && n.Distance(Projectile.Center) < 64))
                {
                    Projectile.velocity = Projectile.DirectionFrom(player.Center).SafeNormalize(Vector2.Zero) * (12f + Projectile.velocity.Length() + player.velocity.Length());
                    Projectile.ai[1] = 15;
                    for (int i = 0; i < 40; i++)
                    {
                        Color glowColor = Main.hslToRgb((Projectile.localAI[0] * 0.01f + i / 120f) % 1f, 1f, 0.6f, 0);
                        glowColor.A /= 2;
                        Dust.NewDustPerfect(Projectile.Center, DustID.AncientLight, Main.rand.NextVector2Circular(5, 5) + Projectile.velocity, 0, glowColor, 1f + Main.rand.NextFloat(2f)).noGravity = true;
                    }

                    break;
                }

                //foreach(NPC goozma in Main.npc.Where(n => n.active && n.type == ModContent.NPCType<Goozma>() && n.Distance(Projectile.Center) < 64))
                //{
                //    Projectile.velocity += Projectile.DirectionTo(goozma.GetTargetData().Center).SafeNormalize(Vector2.Zero) * (12f + Projectile.velocity.Length());
                //    Projectile.ai[1] = 15;
                //    for (int i = 0; i < 40; i++)
                //    {
                //        Color glowColor = Main.hslToRgb((Projectile.localAI[0] * 0.01f + i / 120f) % 1f, 1f, 0.6f, 0);
                //        glowColor.A /= 2;
                //        Dust.NewDustPerfect(Projectile.Center, DustID.AncientLight, Main.rand.NextVector2Circular(5, 5) + Projectile.velocity, 0, glowColor, 1f + Main.rand.NextFloat(2f)).noGravity = true;
                //    }
                //}

                if (Main.rand.NextBool(5))
                    Projectile.velocity += Main.rand.NextVector2Circular(3, 3);

                if (Projectile.Distance(Main.npc[owner].Center) < 64 && Time > 60)
                {
                    HitCount++;
                    Projectile.velocity = Projectile.DirectionTo(Main.npc[owner].GetTargetData().Center).SafeNormalize(Vector2.Zero) * (12f + Projectile.velocity.Length());
                    Projectile.ai[1] = 15;
                    for (int i = 0; i < 40; i++)
                    {
                        Color glowColor = Main.hslToRgb((Projectile.localAI[0] * 0.01f + i / 120f) % 1f, 1f, 0.6f, 0);
                        glowColor.A /= 2;
                        Dust.NewDustPerfect(Projectile.Center, DustID.AncientLight, Main.rand.NextVector2Circular(5, 5) + Projectile.velocity, 0, glowColor, 1f + Main.rand.NextFloat(2f)).noGravity = true;
                    }
                }
            }
            if (Projectile.ai[1] > 0)
                Projectile.ai[1]--;

            if (HitCount > 1)
            {
                Main.npc[owner].ai[0] = 0;
                Main.npc[owner].ai[1] = 3;
                if (Main.netMode == NetmodeID.MultiplayerClient)
                    Main.npc[owner].netUpdate = true;
                Projectile.Kill();
            }

            Projectile.velocity = Vector2.Lerp(Projectile.velocity, Projectile.DirectionTo(Main.npc[owner].Center).SafeNormalize(Vector2.Zero) * 2f, 0.033f) * Utils.GetLerpValue(500, 470, Time, true);
            Projectile.velocity += Projectile.DirectionTo(Main.npc[owner].Center).SafeNormalize(Vector2.Zero) * 0.3f;

            if (Time > 480)
                Projectile.Kill();

            if (Main.rand.NextBool(2))
            {
                Color glowColor = Main.hslToRgb((Projectile.localAI[0] * 0.01f) % 1f, 1f, 0.6f, 0);
                glowColor.A /= 2;
                Dust.NewDustPerfect(Projectile.Center + Main.rand.NextVector2Circular(36, 36), DustID.AncientLight, Main.rand.NextVector2Circular(3, 3), 0, glowColor, 1f + Main.rand.NextFloat(2f)).noGravity = true;
            }

            for (int i = ProjectileID.Sets.TrailCacheLength[Type] - 1; i > 0; i--)
            {
                Projectile.oldPos[i] = Projectile.oldPos[i - 1];
                Projectile.oldRot[i] = Projectile.oldRot[i - 1];
            }
            Projectile.oldPos[0] = Projectile.Center;
            Projectile.oldRot[0] = Projectile.rotation;

            Projectile.localAI[0]++;
            Time++;
        }

        public override void Kill(int timeLeft)
        {
            for (int i = 0; i < 60; i++)
            {
                Color glowColor = Main.hslToRgb((Projectile.localAI[0] * 0.01f) % 1f, 1f, 0.6f, 0);
                glowColor.A /= 2;
                Dust.NewDustPerfect(Projectile.Center, DustID.AncientLight, Main.rand.NextVector2Circular(10, 10), 0, glowColor, 2f + Main.rand.NextFloat(2f)).noGravity = true;
            }
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Asset<Texture2D> texture = ModContent.Request<Texture2D>(Texture);
            Asset<Texture2D> glow = ModContent.Request<Texture2D>($"{nameof(CalamityHunt)}/Assets/Textures/Goozma/GlowSoft");

            Color bloomColor = Main.hslToRgb((Projectile.localAI[0] * 0.01f) % 1f, 1f, 0.6f, 0);
            bloomColor.A = 0;
            Color solidColor = Main.hslToRgb((Projectile.localAI[0] * 0.01f) % 1f, 1f, 0.7f, 0);
            solidColor.A /= 2;
            SpriteEffects direction = Projectile.velocity.X > 0 ? SpriteEffects.FlipHorizontally : SpriteEffects.None;
            
            Main.EntitySpriteDraw(texture.Value, Projectile.Center - Main.screenPosition, null, solidColor, Projectile.rotation * 1.5f, texture.Size() * 0.5f, Projectile.scale, 0, 0);

            for (int i = 0; i < ProjectileID.Sets.TrailCacheLength[Type]; i++)
            {
                Color trailColor = Color.DarkGoldenrod * 0.05f;
                trailColor.A = 0;
                float fadeOut = 1f - (float)i / ProjectileID.Sets.TrailCacheLength[Type];
                float outScale = (float)Math.Pow(fadeOut, 1.5f);
                Main.EntitySpriteDraw(texture.Value, Projectile.oldPos[i] + Projectile.velocity * i * 0.1f - Main.screenPosition, null, trailColor * outScale, Projectile.oldRot[i], texture.Size() * 0.5f, Projectile.scale * fadeOut, direction, 0);
            }

            Main.EntitySpriteDraw(texture.Value, Projectile.Center - Main.screenPosition, null, bloomColor, Projectile.rotation, texture.Size() * 0.5f, Projectile.scale, direction, 0);
            Main.EntitySpriteDraw(texture.Value, Projectile.Center - Main.screenPosition, null, new Color(255, 255, 255, 0), Projectile.rotation * 1.1f, texture.Size() * 0.5f, Projectile.scale * 0.9f, direction, 0);
            Main.EntitySpriteDraw(glow.Value, Projectile.Center - Main.screenPosition, null, bloomColor * 0.5f, Projectile.rotation, glow.Size() * 0.5f, Projectile.scale * 2.5f, direction, 0);

            return false;
        }
    }
}
