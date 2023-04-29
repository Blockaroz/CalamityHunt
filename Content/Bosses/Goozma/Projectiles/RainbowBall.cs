using CalamityHunt.Common.Systems.Particles;
using CalamityHunt.Content.Particles;
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
    public class RainbowBall : ModProjectile, IDieWithGoozma
    {
        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.TrailCacheLength[Type] = 20;
            ProjectileID.Sets.TrailingMode[Type] = -1;
        }

        public override void SetDefaults()
        {
            Projectile.width = 50;
            Projectile.height = 50;
            Projectile.tileCollide = false;
            Projectile.hostile = true;
            Projectile.friendly = false;
            Projectile.penetrate = -1;
            Projectile.aiStyle = -1;
            Projectile.timeLeft = 180;
        }

        public ref float Time => ref Projectile.ai[0];

        public override void AI()
        {
            if (Time == 0)
            {
                Projectile.localAI[0] = Main.rand.NextFloat(20);

                for (int i = 0; i < ProjectileID.Sets.TrailCacheLength[Type]; i++)
                {
                    Projectile.oldPos[i] = Projectile.Center;
                    Projectile.oldRot[i] = Projectile.rotation;
                }
            }
            Projectile.rotation += Projectile.velocity.X * 0.04f;
            Projectile.scale = (float)Math.Sqrt(Utils.GetLerpValue(0, 25, Time, true));

            int target = -1;
            if (Main.player.Any(n => n.active && !n.dead))
                target = Main.player.First(n => n.active && !n.dead).whoAmI;

            if (target > -1)
            {
                Projectile.velocity = Vector2.Lerp(Projectile.velocity, Projectile.DirectionTo(Main.player[target].MountedCenter).SafeNormalize(Vector2.Zero).RotatedByRandom(1f) * new Vector2(Main.rand.Next(22, 25), Main.rand.Next(22, 30)), 0.03f);
                Projectile.velocity += Projectile.DirectionTo(Main.player[target].MountedCenter).SafeNormalize(Vector2.Zero).RotatedByRandom(1f) * 0.03f;
                Projectile.velocity *= (0.7f + Utils.GetLerpValue(0, 600, Projectile.Distance(Main.player[target].MountedCenter), true) * 0.3f);

                if (Math.Abs(Projectile.velocity.ToRotation() - Projectile.AngleTo(Main.player[target].MountedCenter)) < 0.5f && Time > 40)
                    Projectile.velocity *= 1.02f;
            }

            Particle hue = Particle.NewParticle(Particle.ParticleType<HueLightDust>(), Projectile.Center + Main.rand.NextVector2Circular(30, 30) + Projectile.velocity, -Projectile.velocity * 0.5f, Color.White, 2f * Projectile.scale);
            hue.data = Projectile.localAI[0];

            for (int i = ProjectileID.Sets.TrailCacheLength[Type] - 1; i > 0; i--)
            {
                Projectile.oldPos[i] = Vector2.Lerp(Projectile.oldPos[i], Projectile.oldPos[i - 1], 0.66f);
                Projectile.oldRot[i] = MathHelper.Lerp(Projectile.oldRot[i], Projectile.oldRot[i - 1], 0.66f);
            }
            Projectile.oldPos[0] = Vector2.Lerp(Projectile.oldPos[0], Projectile.Center, 0.66f);
            Projectile.oldRot[0] = MathHelper.Lerp(Projectile.oldRot[0], Projectile.rotation, 0.66f);

            Projectile.frameCounter++;
            if (Projectile.frameCounter > (int)Math.Clamp((10f - Projectile.velocity.Length() * 0.33f), 2, 20))
            {
                Projectile.frame = (Projectile.frame + 1) % 3;
                Projectile.frameCounter = 0;
            }

            Projectile.localAI[0]++;
            Time++;
        }

        public override void Kill(int timeLeft)
        {
            for (int i = 0; i < 40; i++)
            {
                Particle hue = Particle.NewParticle(Particle.ParticleType<HueLightDust>(), Projectile.Center, -Projectile.velocity * 0.2f + Main.rand.NextVector2Circular(3, 3), Color.White, 2f);
                hue.data = Projectile.localAI[0];
            }
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Asset<Texture2D> texture = ModContent.Request<Texture2D>(Texture);
            Asset<Texture2D> speed = ModContent.Request<Texture2D>(Texture + "Speed");
            Asset<Texture2D> glow = ModContent.Request<Texture2D>($"{nameof(CalamityHunt)}/Assets/Textures/Goozma/GlowSoft");
            Rectangle frame = speed.Frame(3, 1, Projectile.frame, 0);

            Color bloomColor = new GradientColor(SlimeUtils.GoozColorArray, 0.2f, 0.2f).ValueAt(Projectile.localAI[0]);
            bloomColor.A = 0;
            Color solidColor = bloomColor;
            solidColor.A = 100;
            SpriteEffects direction = Projectile.velocity.X > 0 ? SpriteEffects.FlipHorizontally : SpriteEffects.None;
            
            Main.EntitySpriteDraw(texture.Value, Projectile.Center - Main.screenPosition, null, solidColor * 0.5f, Projectile.rotation * 1.5f, texture.Size() * 0.5f, Projectile.scale, 0, 0);
            Main.EntitySpriteDraw(texture.Value, Projectile.Center - Main.screenPosition, null, bloomColor * 0.5f, Projectile.rotation * 1.5f, texture.Size() * 0.5f, Projectile.scale, 0, 0);

            for (int i = 0; i < ProjectileID.Sets.TrailCacheLength[Type]; i++)
            {
                Color trailColor = new GradientColor(SlimeUtils.GoozColorArray, 0.2f, 0.2f).ValueAt(Projectile.localAI[0] - i * 2f) * 0.15f;
                trailColor.A = 0;
                float fadeOut = 1f - (float)i / ProjectileID.Sets.TrailCacheLength[Type];
                float outScale = (float)Math.Pow(fadeOut, 1.5f);
                Main.EntitySpriteDraw(texture.Value, Projectile.oldPos[i] + Projectile.velocity * i * 0.1f - Main.screenPosition, null, trailColor * outScale, Projectile.oldRot[i], texture.Size() * 0.5f, Projectile.scale * 1.6f * fadeOut, direction, 0);
            }

            Main.EntitySpriteDraw(texture.Value, Projectile.Center - Main.screenPosition, null, bloomColor * 0.2f, Projectile.rotation, texture.Size() * 0.5f, Projectile.scale * 1.1f, direction, 0);
            Main.EntitySpriteDraw(texture.Value, Projectile.Center - Main.screenPosition, null, new Color(255, 255, 255, 0), Projectile.rotation * 1.1f, texture.Size() * 0.5f, Projectile.scale * 0.9f, direction, 0);
            Main.EntitySpriteDraw(glow.Value, Projectile.Center - Main.screenPosition, null, bloomColor * 0.3f, Projectile.rotation, glow.Size() * 0.5f, Projectile.scale * 2f, direction, 0);

            Main.EntitySpriteDraw(speed.Value, Projectile.Center + new Vector2(25, 0).RotatedBy(Projectile.velocity.ToRotation()) * Projectile.scale - Main.screenPosition, frame, bloomColor * 0.6f, Projectile.velocity.ToRotation() - MathHelper.PiOver2, frame.Size() * new Vector2(0.5f, 1f), Projectile.scale * 1.18f, direction, 0);
            Main.EntitySpriteDraw(speed.Value, Projectile.Center + new Vector2(33, 0).RotatedBy(Projectile.velocity.ToRotation()) * Projectile.scale - Main.screenPosition, frame, Color.Lerp(Color.Black, bloomColor, 0.1f), Projectile.velocity.ToRotation() - MathHelper.PiOver2, frame.Size() * new Vector2(0.5f, 1f), Projectile.scale * 1.15f, direction, 0);
            Main.EntitySpriteDraw(glow.Value, Projectile.Center - Main.screenPosition, null, bloomColor * 0.2f, 0, glow.Size() * 0.5f, Projectile.scale * 3f, direction, 0);

            return false;
        }
    }
}
