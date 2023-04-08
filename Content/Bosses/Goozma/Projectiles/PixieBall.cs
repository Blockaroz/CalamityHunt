using CalamityHunt.Content.Bosses.Goozma.Slimes;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using System;
using System.Linq;
using Terraria;
using Terraria.DataStructures;
using Terraria.GameContent;
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
        public ref float Cooldown => ref Projectile.ai[1];
        public ref float HitCount => ref Projectile.ai[2];

        public override void AI()
        {
            Projectile.rotation = Projectile.velocity.X * 0.02f;
            Projectile.scale = (float)Math.Sqrt(Utils.GetLerpValue(0, 17, Time, true) * Utils.GetLerpValue(480, 440, Time, true)) * 2f;
            int owner = -1;
            if (!Main.npc.Any(n => n.type == ModContent.NPCType<DivineGargooptuar>() && n.active))
            {
                Projectile.active = false;
                return;
            }
            else
                owner = Main.npc.First(n => n.type == ModContent.NPCType<DivineGargooptuar>() && n.active).whoAmI;

            if (HitCount < 0 || HitCount == 1)
            {
                Projectile.velocity = Projectile.DirectionTo(Main.npc[owner].GetTargetData().Center).SafeNormalize(Vector2.Zero) * 50f;
                Cooldown = 0;
            }
            else
            {
                if (Main.rand.NextBool(5))
                    Projectile.velocity += Main.rand.NextVector2Circular(3, 3);

                Projectile.velocity = Vector2.Lerp(Projectile.velocity, Projectile.DirectionTo(Main.npc[owner].Center).SafeNormalize(Vector2.Zero) * 3f, 0.03f) * Utils.GetLerpValue(500, 470, Time, true);
                Projectile.velocity += Projectile.DirectionTo(Main.npc[owner].Center).SafeNormalize(Vector2.Zero) * 0.3f;
            }

            if (Cooldown == 0)
            {
                foreach (Player player in Main.player.Where(n => n.active && !n.dead && n.Distance(Projectile.Center) < 64))
                {
                    if (HitCount < 0 || HitCount == 1)
                    {
                        HitCount++;
                        Projectile.velocity = -Vector2.UnitY * 10;
                    }
                    else
                        Projectile.velocity = Projectile.DirectionFrom(player.Center).SafeNormalize(Vector2.Zero) * (12f + Projectile.velocity.Length() + player.velocity.Length());
                    Cooldown = 15;
                    for (int i = 0; i < 40; i++)
                    {
                        Color glowColor = Main.hslToRgb(Projectile.localAI[0] * 0.01f % 1f, 1f, 0.5f, 0);
                        glowColor.A /= 2;
                        Dust.NewDustPerfect(Projectile.Center + Main.rand.NextVector2Circular(36, 36), DustID.AncientLight, Main.rand.NextVector2Circular(15, 15) + Projectile.velocity, 0, glowColor, 1f + Main.rand.NextFloat(2f)).noGravity = true;
                    }

                    break;
                }

                //foreach(NPC goozma in Main.npc.Where(n => n.active && n.type == ModContent.NPCType<Goozma>() && n.Distance(Projectile.Center) < 64))
                //{
                //    Projectile.velocity += Projectile.DirectionTo(goozma.GetTargetData().Center).SafeNormalize(Vector2.Zero) * (12f + Projectile.velocity.Length());
                //    Cooldown = 15;
                //    for (int i = 0; i < 40; i++)
                //    {
                //        Color glowColor = Main.hslToRgb((Projectile.localAI[0] * 0.01f + i / 120f) % 1f, 1f, 0.6f, 0);
                //        glowColor.A /= 2;
                //        Dust.NewDustPerfect(Projectile.Center, DustID.AncientLight, Main.rand.NextVector2Circular(5, 5) + Projectile.velocity, 0, glowColor, 1f + Main.rand.NextFloat(2f)).noGravity = true;
                //    }
                //}

                if (Projectile.Distance(Main.npc[owner].Center) < 64 && Time > 60)
                {
                    HitCount++;
                    Projectile.velocity = Projectile.DirectionTo(Main.npc[owner].GetTargetData().Center).SafeNormalize(Vector2.Zero) * (12f + Projectile.velocity.Length());
                    Cooldown = 15;
                    for (int i = 0; i < 40; i++)
                    {
                        Color glowColor = Main.hslToRgb(Projectile.localAI[0] * 0.01f % 1f, 1f, 0.5f, 0);
                        glowColor.A /= 2;
                        Dust.NewDustPerfect(Projectile.Center + Main.rand.NextVector2Circular(36, 36), DustID.AncientLight, Main.rand.NextVector2Circular(15, 15) + Projectile.velocity, 0, glowColor, 1f + Main.rand.NextFloat(2f)).noGravity = true;
                    }
                }
            }
            if (Cooldown > 0)
                Cooldown--;

            if (HitCount > 2)
            {
                Main.npc[owner].ai[0] = 0;
                Main.npc[owner].ai[1] = -1;
                if (Main.netMode == NetmodeID.MultiplayerClient)
                    Main.npc[owner].netUpdate = true;
                Projectile.Kill();
            }

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

            if (Time > 400)
                for (int i = 0; i < 40 - (Time / 2); i++)
                {
                    Color glowColor = Main.hslToRgb((Projectile.localAI[0] * 0.01f) % 1f, 1f, 0.5f, 0);
                    glowColor.A /= 2;
                    Dust.NewDustPerfect(Projectile.Center, DustID.AncientLight, Main.rand.NextVector2Circular(25 - Time / 4f, 25 - Time / 4f), 0, glowColor, 2f + Main.rand.NextFloat(2f)).noGravity = true;
                }

            Projectile.localAI[0]++;
            Time++;
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Asset<Texture2D> texture = ModContent.Request<Texture2D>(Texture);
            Asset<Texture2D> sparkle = TextureAssets.Extra[98];
            Asset<Texture2D> glow = ModContent.Request<Texture2D>($"{nameof(CalamityHunt)}/Assets/Textures/Goozma/GlowSoft");

            Color bloomColor = Main.hslToRgb((Projectile.localAI[0] * 0.01f) % 1f, 1f, 0.6f, 0);
            Color solidColor = Main.hslToRgb((Projectile.localAI[0] * 0.01f) % 1f, 1f, 0.7f, 100);
            SpriteEffects direction = Projectile.velocity.X > 0 ? SpriteEffects.FlipHorizontally : SpriteEffects.None;
            
            Main.EntitySpriteDraw(texture.Value, Projectile.Center - Main.screenPosition, null, solidColor * 0.5f, Projectile.rotation * 1.5f, texture.Size() * 0.5f, Projectile.scale, 0, 0);
            Main.EntitySpriteDraw(texture.Value, Projectile.Center - Main.screenPosition, null, bloomColor, Projectile.rotation * 1.5f, texture.Size() * 0.5f, Projectile.scale, 0, 0);

            for (int i = 0; i < ProjectileID.Sets.TrailCacheLength[Type]; i++)
            {
                Color trailColor = Main.hslToRgb((Projectile.localAI[0] * 0.03f - i * 0.04f) % 1f, 1f, 0.6f, 0) * 0.15f;
                trailColor.A = 0;
                float fadeOut = 1f - (float)i / ProjectileID.Sets.TrailCacheLength[Type];
                float outScale = (float)Math.Pow(fadeOut, 1.5f);
                Main.EntitySpriteDraw(texture.Value, Projectile.oldPos[i] + Projectile.velocity * i * 0.1f - Main.screenPosition, null, trailColor * outScale, Projectile.oldRot[i], texture.Size() * 0.5f, Projectile.scale, direction, 0);
            }

            Main.EntitySpriteDraw(texture.Value, Projectile.Center - Main.screenPosition, null, solidColor, Projectile.rotation, texture.Size() * 0.5f, Projectile.scale, direction, 0);
            Main.EntitySpriteDraw(texture.Value, Projectile.Center - Main.screenPosition, null, new Color(200, 200, 200, 0), Projectile.rotation, texture.Size() * 0.5f, Projectile.scale * 0.7f, direction, 0);
            Main.EntitySpriteDraw(glow.Value, Projectile.Center - Main.screenPosition, null, bloomColor * 0.3f, Projectile.rotation, glow.Size() * 0.5f, Projectile.scale * 2f, direction, 0);

            float wobble = 1.05f + (float)Math.Sin(Time * 0.1f) * 0.1f;
            Vector2 sparkleScale = new Vector2(0.5f, 2.5f) * wobble * Projectile.scale;
            Main.EntitySpriteDraw(sparkle.Value, Projectile.Center - Main.screenPosition, null, bloomColor, Projectile.rotation, sparkle.Size() * 0.5f, sparkleScale, 0, 0);
            Main.EntitySpriteDraw(sparkle.Value, Projectile.Center - Main.screenPosition, null, bloomColor, Projectile.rotation + MathHelper.PiOver2, sparkle.Size() * 0.5f, sparkleScale, 0, 0);
            Main.EntitySpriteDraw(sparkle.Value, Projectile.Center - Main.screenPosition, null, bloomColor, Projectile.rotation + MathHelper.PiOver4, sparkle.Size() * 0.5f, sparkleScale, 0, 0);
            Main.EntitySpriteDraw(sparkle.Value, Projectile.Center - Main.screenPosition, null, bloomColor, Projectile.rotation + MathHelper.PiOver2 + MathHelper.PiOver4, sparkle.Size() * 0.5f, sparkleScale, 0, 0);
            Main.EntitySpriteDraw(sparkle.Value, Projectile.Center - Main.screenPosition, null, new Color(255, 255, 255, 0), Projectile.rotation, sparkle.Size() * 0.5f, 0.8f * sparkleScale, 0, 0);
            Main.EntitySpriteDraw(sparkle.Value, Projectile.Center - Main.screenPosition, null, new Color(255, 255, 255, 0), Projectile.rotation + MathHelper.PiOver2, sparkle.Size() * 0.5f, 0.8f * sparkleScale, 0, 0);
            Main.EntitySpriteDraw(sparkle.Value, Projectile.Center - Main.screenPosition, null, new Color(255, 255, 255, 0), Projectile.rotation + MathHelper.PiOver4, sparkle.Size() * 0.5f, 0.8f * sparkleScale, 0, 0);
            Main.EntitySpriteDraw(sparkle.Value, Projectile.Center - Main.screenPosition, null, new Color(255, 255, 255, 0), Projectile.rotation + MathHelper.PiOver2 + MathHelper.PiOver4, sparkle.Size() * 0.5f, 0.8f * sparkleScale, 0, 0);

            return false;
        }
    }
}
