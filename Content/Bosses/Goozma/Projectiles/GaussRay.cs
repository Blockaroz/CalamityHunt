using CalamityHunt.Common.Systems.Particles;
using CalamityHunt.Content.Particles;
using Microsoft.CodeAnalysis;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using ReLogic.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.GameContent;
using Terraria.Graphics;
using Terraria.ID;
using Terraria.ModLoader;
using static tModPorter.ProgressUpdate;

namespace CalamityHunt.Content.Bosses.Goozma.Projectiles
{
    public class GaussRay : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.DrawScreenCheckFluff[Type] = 5000;
            ProjectileID.Sets.TrailingMode[Type] = 2;
            ProjectileID.Sets.TrailCacheLength[Type] = 10;
        }
        public override void SetDefaults()
        {
            Projectile.width = 800;
            Projectile.height = 800;
            Projectile.tileCollide = false;
            Projectile.hostile = true;
            Projectile.friendly = false;
            Projectile.penetrate = -1;
            Projectile.aiStyle = -1;
            Projectile.manualDirectionChange = true;
            Projectile.appliesImmunityTimeOnSingleHits = false;
        }

        public ref float Time => ref Projectile.ai[0];

        private int ChargeTime = 300;
        private int LaserDuration = 700;

        public override void AI()
        {
            int owner = -1;
            if (!Main.npc.Any(n => n.type == ModContent.NPCType<Goozma>() && n.active))
            {
                Projectile.active = false;
                return;
            }
            else
                owner = Main.npc.First(n => n.type == ModContent.NPCType<Goozma>() && n.active).whoAmI;

            Projectile.Center = Main.npc[owner].Center + new Vector2(7 * Main.npc[owner].direction, -10);
            Main.npc[owner].velocity += Projectile.rotation.ToRotationVector2() * -0.3f;
            Main.npc[owner].velocity *= 0.8f;
            if (Time > ChargeTime)
            {
                if (Math.Abs(Projectile.velocity.X) > 0.9f)
                    Main.npc[owner].direction = Projectile.velocity.X > 0 ? 1 : -1;
            }

            if (Main.npc[owner].ai[2] >= 3 && Time < ChargeTime + LaserDuration - 2)
                Time = ChargeTime + LaserDuration - 2;

            if (Time < ChargeTime)
                Projectile.direction = Main.rand.NextBool().ToDirectionInt();

            float totalOffRot = (float)Math.Sin(Time * 0.03f) * 0.07f * Utils.GetLerpValue(ChargeTime - 10, ChargeTime + 10, Time, true);

            Projectile.velocity = Vector2.Lerp(Projectile.velocity, Projectile.DirectionTo(Main.npc[owner].GetTargetData().Center).SafeNormalize(Vector2.Zero), 0.028f);
            Projectile.rotation = Projectile.velocity.ToRotation() + totalOffRot;
            Projectile.localAI[0] = Main.npc[owner].localAI[0];
            
            float smokePower = Utils.GetLerpValue(0, ChargeTime * 0.9f, Time, true) * Utils.GetLerpValue(ChargeTime + LaserDuration + 70, ChargeTime + LaserDuration + 40, Time, true);
            Color smokeColor = new GradientColor(SlimeUtils.GoozColorArray, 0.2f, 0.2f).ValueAt(Projectile.localAI[0]) * (0.5f + smokePower * 0.5f);
            smokeColor.A = 0;
            int smokeCount = (int)(smokePower * 12f);
            for (int i = 0; i < smokeCount; i++)
                Particle.NewParticle(Particle.ParticleType<CosmicSmoke>(), Projectile.Center + Projectile.rotation.ToRotationVector2() * 20, Projectile.rotation.ToRotationVector2().RotatedByRandom((float)Math.Sqrt(1f - smokePower * 0.5f)) * Main.rand.NextFloat(15f, 25f) * smokePower, smokeColor, 1f + Main.rand.NextFloat());

            if (!Main.rand.NextBool((int)(Time * 0.5f + 15)))
            {
                Particle hue = Particle.NewParticle(Particle.ParticleType<HueLightDust>(), Projectile.Center, Projectile.rotation.ToRotationVector2().RotatedByRandom(1f) * Main.rand.NextFloat(5f, 15f), Color.White, 2f);
                hue.data = Projectile.localAI[0];
            }
            if (Time > ChargeTime - 15 && Time < ChargeTime + LaserDuration + 60)
            {
                for (int i = 0; i < 35; i++)
                {
                    float grow = Utils.GetLerpValue(ChargeTime - 15, ChargeTime + 40, Time, true) * Utils.GetLerpValue(ChargeTime + LaserDuration + 50, ChargeTime + LaserDuration, Time, true);
                    float progress = Main.rand.NextFloat(3300);
                    Color color = new GradientColor(SlimeUtils.GoozColorArray, 0.2f, 0.2f).ValueAt(Projectile.localAI[0] - (progress / 3500f) * 60) * grow;
                    color.A = 0;
                    Vector2 position = Projectile.Center + new Vector2(progress, Main.rand.NextFloat(-80f, 80f) * (progress / 3300f)).RotatedBy(Projectile.rotation);
                    Particle smoke = Particle.NewParticle(Particle.ParticleType<CosmicSmoke>(), position, Projectile.velocity.RotatedByRandom(0.4f) * Main.rand.NextFloat(15f, 45f), color, 1f + Main.rand.NextFloat(2f));
                    smoke.behindEntities = true;
                    if (Main.rand.NextBool(5))
                    {
                        Particle hue = Particle.NewParticle(Particle.ParticleType<HueLightDust>(), position, Projectile.velocity.RotatedByRandom(0.4f) * Main.rand.NextFloat(15f, 45f), Color.White, 4f * grow);
                        hue.data = Projectile.localAI[0] - (progress / 3300f) * 60;
                    }
                }
            }

            if (Time == ChargeTime + LaserDuration + 20 && !Main.dedServ)
            {
                SoundStyle sizzle = new SoundStyle($"{nameof(CalamityHunt)}/Assets/Sounds/Goozma/GoozmaSizzle");
                sizzle.MaxInstances = 0;
                SoundEngine.PlaySound(sizzle, Projectile.Center);
            }
            //Projectile.velocity = (Projectile.rotation.AngleLerp(Projectile.AngleTo(Main.LocalPlayer.Center) + (initialSweep + secondSweep) * Projectile.direction, 0.025f) + (float)Math.Sin(Time * 0.03f) * 0.005f).ToRotationVector2();
            //Projectile.rotation = Projectile.velocity.ToRotation();
            //Projectile.localAI[0]++;

            Time++;
            Projectile.timeLeft = 10;
            HandleSound();

            if (Time > ChargeTime + LaserDuration + 100)
                Projectile.Kill();

            Projectile.damage = 20;
        }

        public static SlotId laserSound;
        public static float volume;
        public static float pitch;

        public void HandleSound()
        {
            SoundStyle raySound = new SoundStyle($"{nameof(CalamityHunt)}/Assets/Sounds/Goozma/GoozmaGaussRayLoop");
            raySound.IsLooped = true;

            pitch = MathHelper.SmoothStep(0, 1, Utils.GetLerpValue(ChargeTime - 25, ChargeTime + 50, Time, true) * Utils.GetLerpValue(ChargeTime + LaserDuration + 60, ChargeTime + LaserDuration, Time, true)) - 1f;
            volume = Math.Clamp(1f + Projectile.velocity.Length() * 0.0001f + Main.LocalPlayer.Distance(Projectile.Center) * 0.0005f, 0, 1) * Projectile.scale * Utils.GetLerpValue(ChargeTime - 30, ChargeTime + 20, Time, true) * Utils.GetLerpValue(ChargeTime + LaserDuration + 60, ChargeTime + LaserDuration + 30, Time, true);
            bool active = SoundEngine.TryGetActiveSound(laserSound, out ActiveSound sound);
            if ((!active || !laserSound.IsValid) && Time > ChargeTime - 35)
                laserSound = SoundEngine.PlaySound(raySound, Projectile.Center);

            if (active)
            {
                sound.Volume = volume;
                sound.Pitch = pitch;
                sound.Position = Projectile.Center;
            }
        }

        public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
        {
            if (Time > ChargeTime && Time < ChargeTime + LaserDuration + 20)
            {
                float angle = 0.23f;
                float grow = (float)Math.Pow(Utils.GetLerpValue(ChargeTime - 20, ChargeTime + 40, Time, true) * Utils.GetLerpValue(ChargeTime + LaserDuration + 60, ChargeTime + LaserDuration, Time, true), 3f);

                //Dust.QuickDustLine(Projectile.Center, Projectile.Center + new Vector2(3500f * (0.5f + grow * 0.5f), 0).RotatedBy(Projectile.rotation + angle), 200, Color.Blue);
                //Dust.QuickDustLine(Projectile.Center, Projectile.Center + new Vector2(3500f * (0.5f + grow * 0.5f), 0).RotatedBy(Projectile.rotation - angle), 200, Color.Blue);
                //if (targetHitbox.IntersectsConeFastInaccurate(Projectile.Center, 3500f * (0.5f + grow * 0.5f), Projectile.rotation, angle * grow))
                //    Dust.NewDustPerfect(targetHitbox.Center(), DustID.TintableDust, Vector2.Zero, 0, Color.Black, 2f).noGravity = true;

                return targetHitbox.IntersectsConeFastInaccurate(Projectile.Center, 3500f * (0.5f + grow * 0.5f), Projectile.rotation, angle * grow);
            }
            return false;
        }

        public override void DrawBehind(int index, List<int> behindNPCsAndTiles, List<int> behindNPCs, List<int> behindProjectiles, List<int> overPlayers, List<int> overWiresUI) => overPlayers.Add(index);

        public override bool PreDraw(ref Color lightColor)
        {
            Asset<Texture2D> texture = ModContent.Request<Texture2D>(Texture);
            Asset<Texture2D> textureSecond = ModContent.Request<Texture2D>(Texture + "Second");
            Asset<Texture2D> textureGlow = ModContent.Request<Texture2D>(Texture + "Glow");
            Asset<Texture2D> textureBits = ModContent.Request<Texture2D>(Texture + "Bits");
            Asset<Texture2D> glow = ModContent.Request<Texture2D>($"{nameof(CalamityHunt)}/Assets/Textures/Goozma/GlowSoft");
            Asset<Texture2D> spark = ModContent.Request<Texture2D>(Texture + "Spark");

            Color startColor = new GradientColor(SlimeUtils.GoozColorArray, 0.2f, 0.2f).ValueAt(Projectile.localAI[0]);
            startColor.A = 0;

            float laserCharge = Utils.GetLerpValue(50, ChargeTime, Time, true) * Utils.GetLerpValue(ChargeTime + LaserDuration + 50, ChargeTime + LaserDuration + 20, Time, true);
            Main.EntitySpriteDraw(glow.Value, Projectile.Center - Main.screenPosition, null, startColor, 0, glow.Size() * 0.5f, 2f * laserCharge, 0, 0);
            Main.EntitySpriteDraw(glow.Value, Projectile.Center - Main.screenPosition, null, new Color(255, 255, 255, 0) * laserCharge, 0, glow.Size() * 0.5f, 1.5f * laserCharge, 0, 0);

            if (Time >= ChargeTime - 20)
            {
                float laserActive = Utils.GetLerpValue(ChargeTime - 30, ChargeTime + 70, Time, true) * Utils.GetLerpValue(ChargeTime + LaserDuration + 60, ChargeTime + LaserDuration, Time, true);

                Vector2[] positions = new Vector2[1500];
                float[] rotations = new float[1500];
                for (int i = 0; i < 1500; i++)
                {
                    positions[i] = Projectile.Center + new Vector2(3500 * (i / 1500f) * (0.5f + laserActive * 0.5f), 0).RotatedBy(Projectile.rotation);
                    rotations[i] = Projectile.rotation;
                }

                VertexStrip strip = new VertexStrip();
                strip.PrepareStripWithProceduralPadding(positions, rotations, StripColor, StripWidth, -Main.screenPosition, true);

                Effect lightningEffect = ModContent.Request<Effect>($"{nameof(CalamityHunt)}/Assets/Effects/GaussRayEffect", AssetRequestMode.ImmediateLoad).Value;
                lightningEffect.Parameters["uTransformMatrix"].SetValue(Main.GameViewMatrix.NormalizedTransformationmatrix);
                lightningEffect.Parameters["uTexture0"].SetValue(texture.Value);
                lightningEffect.Parameters["uTexture1"].SetValue(textureSecond.Value);
                lightningEffect.Parameters["uGlow"].SetValue(textureGlow.Value);
                lightningEffect.Parameters["uBits"].SetValue(textureBits.Value);
                lightningEffect.Parameters["uTime"].SetValue(-Projectile.localAI[0] * 0.015f);
                lightningEffect.Parameters["uFreq"].SetValue(1.5f);
                lightningEffect.CurrentTechnique.Passes[0].Apply();
                strip.DrawTrail();
                Main.pixelShader.CurrentTechnique.Passes[0].Apply();

                Color endColor = new GradientColor(SlimeUtils.GoozColorArray, 0.2f, 0.2f).ValueAt(Projectile.localAI[0] - 60f);
                endColor.A = 0;

                for (int i = 0; i < 12; i++)
                {
                    Main.EntitySpriteDraw(spark.Value, positions[1499] - Main.screenPosition, null, endColor * 0.3f, (MathHelper.TwoPi / 12f * i) + (Time * 0.01f * Projectile.direction), spark.Size() * 0.5f, laserActive * 0.7f * new Vector2(1f, 2f), 0, 0);
                    Main.EntitySpriteDraw(spark.Value, positions[1499] - Main.screenPosition, null, endColor * 0.3f, (MathHelper.TwoPi / 12f * i) + (Time * 0.03f * Projectile.direction), spark.Size() * 0.5f, laserActive * new Vector2(1f, 2f), 0, 0);
                    Main.EntitySpriteDraw(spark.Value, positions[1499] - Main.screenPosition, null, new Color(255, 255, 255, 0), (MathHelper.TwoPi / 12f * i) + (Time * 0.01f * Projectile.direction), spark.Size() * 0.5f, laserActive * 0.33f * new Vector2(1f, 2f), 0, 0);
                    Main.EntitySpriteDraw(glow.Value, positions[1499] - Main.screenPosition, null, endColor * 0.3f, (MathHelper.TwoPi / 12f * i) + (Time * 0.01f * Projectile.direction), glow.Size() * 0.5f, laserActive * new Vector2(3f, 20f), 0, 0);

                }
                Main.EntitySpriteDraw(glow.Value, positions[1499] - Main.screenPosition, null, endColor, 0, glow.Size() * 0.5f, laserActive * 15f, 0, 0);
            }
            return false;
        }

        public Color StripColor(float progress)
        {
            float grow = 0.1f + (float)Math.Pow(Utils.GetLerpValue(ChargeTime - 20, ChargeTime + 40, Time, true) * Utils.GetLerpValue(ChargeTime + LaserDuration + 60, ChargeTime + LaserDuration, Time, true), 3f);

            Color color = new GradientColor(SlimeUtils.GoozColorArray, 0.2f, 0.2f).ValueAt(Projectile.localAI[0] - progress * 60f);
            color.A = 0;
            return color * grow;
        }
        public float StripWidth(float progress)
        {
            float start = (float)Math.Pow(progress, 0.6f);
            float cap = (float)Math.Cbrt(Utils.GetLerpValue(1.01f, 0.7f, progress, true));
            float grow = (float)Math.Pow(Utils.GetLerpValue(ChargeTime - 20, ChargeTime + 40, Time, true) * Utils.GetLerpValue(ChargeTime + LaserDuration + 60, ChargeTime + LaserDuration, Time, true), 3f);
            return start * cap * grow * 1600f * (1.1f + (float)Math.Cos(Time) * (0.06f - progress * 0.06f));
        }
    }
}
