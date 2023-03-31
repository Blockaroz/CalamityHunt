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
    public class RainbowBall : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.TrailCacheLength[Type] = 20;
            ProjectileID.Sets.TrailingMode[Type] = -1;
        }

        public override void SetDefaults()
        {
            Projectile.width = 54;
            Projectile.height = 54;
            Projectile.tileCollide = false;
            Projectile.hostile = true;
            Projectile.friendly = false;
            Projectile.penetrate = -1;
            Projectile.aiStyle = -1;
            Projectile.timeLeft = 140;
        }

        public ref float Time => ref Projectile.ai[0];

        public override void AI()
        {
            Projectile.rotation += Projectile.velocity.X * 0.04f;
            Projectile.scale = (float)Math.Sqrt(Utils.GetLerpValue(0, 17, Time, true) * Utils.GetLerpValue(0, 20, Projectile.timeLeft, true));

            int target = -1;
            if (Main.player.Any(n => n.active && !n.dead))
                target = Main.player.First(n => n.active && !n.dead).whoAmI;

            if (target > -1)
                Projectile.velocity = Vector2.Lerp(Projectile.velocity, Projectile.DirectionTo(Main.player[target].MountedCenter).SafeNormalize(Vector2.Zero).RotatedByRandom(2f) * new Vector2(Main.rand.Next(8, 30), Main.rand.Next(18, 30)), 0.04f);

            if (Main.rand.NextBool(3))
            {
                Color glowColor = new GradientColor(SlimeUtils.GoozColorArray, 0.2f, 0.2f).ValueAt(Projectile.localAI[0]);
                glowColor.A /= 2;
                Dust.NewDustPerfect(Projectile.Center + Main.rand.NextVector2Circular(24, 24), DustID.FireworksRGB, Main.rand.NextVector2Circular(2, 2), 0, glowColor, 1f).noGravity = true;
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
            for (int i = 0; i < 20; i++)
            {
                Color glowColor = new GradientColor(SlimeUtils.GoozColorArray, 0.2f, 0.2f).ValueAt(Projectile.localAI[0]);
                glowColor.A /= 2;
                Dust.NewDustPerfect(Projectile.Center, DustID.FireworksRGB, Main.rand.NextVector2Circular(10, 10), 0, glowColor, 1f).noGravity = true;

                if (Main.rand.NextBool(2))
                    Dust.NewDustPerfect(Projectile.Center, 4, Main.rand.NextVector2Circular(4, 4), 0, Color.Black, 1.5f).noGravity = true;
            }
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Asset<Texture2D> texture = ModContent.Request<Texture2D>(Texture);
            Asset<Texture2D> glow = ModContent.Request<Texture2D>($"{nameof(CalamityHunt)}/Assets/Textures/Goozma/GlowSoft");
            
            Color bloomColor = new GradientColor(SlimeUtils.GoozColorArray, 0.2f, 0.2f).ValueAt(Projectile.localAI[0]);
            bloomColor.A = 0;
            Color solidColor = bloomColor;
            solidColor.A /= 2;
            SpriteEffects direction = Projectile.velocity.X > 0 ? SpriteEffects.FlipHorizontally : SpriteEffects.None;
            
            Main.EntitySpriteDraw(texture.Value, Projectile.Center - Main.screenPosition, null, solidColor, Projectile.rotation * 1.5f, texture.Size() * 0.5f, Projectile.scale * 1.1f, 0, 0);

            for (int i = 0; i < ProjectileID.Sets.TrailCacheLength[Type]; i++)
            {
                Color trailColor = new GradientColor(SlimeUtils.GoozColorArray, 0.2f, 0.2f).ValueAt(Projectile.localAI[0] - i * 2f) * 0.05f;
                trailColor.A = 0;
                float fadeOut = 1f - (float)i / ProjectileID.Sets.TrailCacheLength[Type];
                float outScale = (float)Math.Pow(fadeOut, 1.5f);
                Main.EntitySpriteDraw(texture.Value, Projectile.oldPos[i] + Projectile.velocity * i * 0.1f - Main.screenPosition, null, trailColor * outScale, Projectile.oldRot[i], texture.Size() * 0.5f, Projectile.scale * 1.6f * fadeOut, direction, 0);
            }

            Main.EntitySpriteDraw(texture.Value, Projectile.Center - Main.screenPosition, null, bloomColor * 0.2f, Projectile.rotation, texture.Size() * 0.5f, Projectile.scale * 1.2f, direction, 0);
            Main.EntitySpriteDraw(texture.Value, Projectile.Center - Main.screenPosition, null, new Color(255, 255, 255, 0), Projectile.rotation * 1.1f, texture.Size() * 0.5f, Projectile.scale * 0.9f, direction, 0);
            Main.EntitySpriteDraw(glow.Value, Projectile.Center - Main.screenPosition, null, bloomColor * 0.5f, Projectile.rotation, glow.Size() * 0.5f, Projectile.scale * 2.5f, direction, 0);

            return false;
        }
    }
}
