using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using System;
using System.Linq;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityHunt.Content.Bosses.Goozma.Projectiles
{
    public class BloatedBlast : ModProjectile
    {
        public override void SetDefaults()
        {
            Projectile.width = 56;
            Projectile.height = 56;
            Projectile.tileCollide = false;
            Projectile.hostile = true;
            Projectile.friendly = false;
            Projectile.penetrate = -1;
            Projectile.aiStyle = -1; 
            Projectile.timeLeft = 180;

        }

        public override void OnSpawn(IEntitySource source)
        {
            Projectile.localAI[0] += Main.rand.NextFloat(20f);
            Projectile.frame = Main.rand.Next(3);
        }

        public float colOffset;

        public ref float Time => ref Projectile.ai[0];

        public override void AI()
        {
            Projectile.rotation = Projectile.velocity.ToRotation() + MathHelper.PiOver2;
            Projectile.scale = (float)Math.Sqrt(Utils.GetLerpValue(-4, 8, Time, true) * Utils.GetLerpValue(0, 20, Projectile.timeLeft, true));

            if (Main.rand.NextBool(3))
            {
                Color glowColor = new GradientColor(SlimeUtils.GoozColorArray, 0.2f, 0.2f).ValueAt(Projectile.localAI[0]);
                glowColor.A /= 2;
                Dust.NewDustPerfect(Projectile.Center + Main.rand.NextVector2Circular(Projectile.width / 2, Projectile.height / 2), DustID.FireworksRGB, Projectile.velocity * 0.4f, 0, glowColor, 1f).noGravity = true;
            }

            Time++;

            int frameMax = 2;
            if (Projectile.ai[1] == 0)
            {
                int target = -1;
                if (Main.player.Any(n => n.active && !n.dead))
                    target = Main.player.First(n => n.active && !n.dead).whoAmI;

                if (target > -1 && Time < 50)
                {
                    Projectile.velocity = Vector2.Lerp(Projectile.velocity, Projectile.DirectionTo(Main.player[target].MountedCenter).SafeNormalize(Vector2.Zero) * 10f, 0.1f);
                    Projectile.Center += Main.player[target].velocity * 0.8f * Utils.GetLerpValue(100, 0, Time, true); 
                }
                else
                    Projectile.velocity *= 0.96f;

                frameMax = 4;

                if (Time > 60)
                    Projectile.Kill();
            }
            else
                Projectile.Resize(24, 24);

            Projectile.frameCounter++;
            if (Projectile.frameCounter > frameMax)
            {
                Projectile.frameCounter = 0;
                Projectile.frame = (Projectile.frame + 1) % 3;
            }

            Projectile.localAI[0]++;
        }

        public override void Kill(int timeLeft)
        {
            if (Projectile.ai[1] == 0)
            {
                for (int i = 0; i < 6; i++)
                {
                    float size = Main.rand.NextFloat(4f, 5f) + i * 2f;
                    float rotation = Main.rand.NextFloat(-2f, 2f);

                    for (int j = 0; j < 30; j++)
                    {
                        Color glowColor = new GradientColor(SlimeUtils.GoozColorArray, 2f, 2f).ValueAt(j * 30f + i);
                        glowColor.A /= 2;
                        Vector2 outward = new Vector2(size + Main.rand.NextFloat(), 0).RotatedBy(MathHelper.TwoPi / 30f * j);
                        outward.X *= 0.4f;
                        Dust.NewDustPerfect(Projectile.Center, DustID.FireworksRGB, outward.RotatedBy(rotation), 0, glowColor, 1.5f).noGravity = true;
                    }
                }

                for (int i = 0; i < 40; i++)
                    Dust.NewDustPerfect(Projectile.Center, DustID.TintableDust, Main.rand.NextVector2Circular(5, 5), 180, Color.Black, 2f).noGravity = true;


                for (int i = 0; i < 16; i++)
                {
                    Projectile dart = Projectile.NewProjectileDirect(Projectile.GetSource_FromAI(), Projectile.Center, new Vector2(20, 0).RotatedBy(MathHelper.TwoPi / 16f * i), Type, Projectile.damage, 0, ai1: 1);
                    dart.ai[1] = 1;
                    dart.localAI[0] = Projectile.localAI[0] + i / 16f * 90f;
                }

                if (!Main.dedServ)
                {
                    SoundStyle dartSound = new SoundStyle($"{nameof(CalamityHunt)}/Assets/Sounds/Goozma/GoozmaDartShoot", 1, 2);
                    dartSound.PitchVariance = 0.2f;
                    dartSound.Pitch = 0.3f;
                    SoundEngine.PlaySound(dartSound);
                }
            }

            for (int i = 0; i < 10; i++)
            {
                Color glowColor = new GradientColor(SlimeUtils.GoozColorArray, 0.2f, 0.2f).ValueAt(Projectile.localAI[0]);
                glowColor.A /= 2;
                Dust.NewDustPerfect(Projectile.Center, DustID.FireworksRGB, Main.rand.NextVector2Circular(10, 10), 0, glowColor, 1f).noGravity = true;
                Dust.NewDustPerfect(Projectile.Center, 4, Main.rand.NextVector2Circular(4, 4), 0, Color.Black, 1.5f).noGravity = true;
            }
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Asset<Texture2D> texture = ModContent.Request<Texture2D>(Texture);
            Asset<Texture2D> textureSmall = ModContent.Request<Texture2D>(Texture + "Small");
            Asset<Texture2D> glow = ModContent.Request<Texture2D>($"{nameof(CalamityHunt)}/Assets/Textures/Goozma/GlowSoft");
            Vector2 squishFactor = new Vector2(1f - Projectile.velocity.Length() * 0.0045f, 1f + Projectile.velocity.Length() * 0.0075f);
            
            Color bloomColor = new GradientColor(SlimeUtils.GoozColorArray, 0.2f, 0.2f).ValueAt(Projectile.localAI[0]);
            bloomColor.A = 0;

            if (Projectile.ai[1] == 0)
            {
                Color growColor = Color.Lerp(Color.Transparent, bloomColor, Utils.GetLerpValue(20, 60, Time, true));
                Rectangle baseFrame = texture.Frame(3, 3, 0, Projectile.frame);
                Rectangle glowFrame = texture.Frame(3, 3, 1, Projectile.frame);
                Rectangle outlineFrame = texture.Frame(3, 3, 2, Projectile.frame);
                Main.EntitySpriteDraw(texture.Value, Projectile.Center - Main.screenPosition, outlineFrame, bloomColor * 0.7f, Projectile.rotation, outlineFrame.Size() * 0.5f, Projectile.scale * 1.1f * squishFactor, 0, 0);
                Main.EntitySpriteDraw(texture.Value, Projectile.Center - Main.screenPosition, baseFrame, lightColor, Projectile.rotation, baseFrame.Size() * 0.5f, Projectile.scale * squishFactor, 0, 0);
                Main.EntitySpriteDraw(texture.Value, Projectile.Center - Main.screenPosition, glowFrame, growColor, Projectile.rotation, glowFrame.Size() * 0.5f, Projectile.scale * squishFactor, 0, 0);
                Main.EntitySpriteDraw(texture.Value, Projectile.Center - Main.screenPosition, glowFrame, growColor, Projectile.rotation, glowFrame.Size() * 0.5f, Projectile.scale * 1.05f * squishFactor, 0, 0);
                Main.EntitySpriteDraw(glow.Value, Projectile.Center + Projectile.velocity * 0.2f - Main.screenPosition, null, growColor * 0.5f, Projectile.rotation, glow.Size() * 0.5f, Projectile.scale * 2f * squishFactor, 0, 0);
            }
            if (Projectile.ai[1] == 1)
            {
                Rectangle baseFrame = textureSmall.Frame(3, 1, 0, 0);
                Rectangle glowFrame = textureSmall.Frame(3, 1, 1, 0);
                Rectangle outlineFrame = textureSmall.Frame(3, 1, 2, 0);

                Main.EntitySpriteDraw(textureSmall.Value, Projectile.Center - Main.screenPosition, outlineFrame, bloomColor * 0.7f, Projectile.rotation, outlineFrame.Size() * 0.5f, Projectile.scale * 1.1f * squishFactor, 0, 0);
                Main.EntitySpriteDraw(textureSmall.Value, Projectile.Center - Main.screenPosition, baseFrame, lightColor, Projectile.rotation, baseFrame.Size() * 0.5f, Projectile.scale * squishFactor, 0, 0);
                Main.EntitySpriteDraw(textureSmall.Value, Projectile.Center - Main.screenPosition, glowFrame, bloomColor, Projectile.rotation, glowFrame.Size() * 0.5f, Projectile.scale * squishFactor, 0, 0);
                Main.EntitySpriteDraw(textureSmall.Value, Projectile.Center - Main.screenPosition, glowFrame, bloomColor * 0.8f, Projectile.rotation, glowFrame.Size() * 0.5f, Projectile.scale * 1.05f * squishFactor, 0, 0);
                Main.EntitySpriteDraw(glow.Value, Projectile.Center + Projectile.velocity * 0.2f - Main.screenPosition, null, bloomColor * 0.1f, Projectile.rotation, glow.Size() * 0.5f, Projectile.scale * squishFactor, 0, 0);

            }

            return false;
        }
    }
}
