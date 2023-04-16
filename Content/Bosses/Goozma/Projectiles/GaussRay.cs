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

            Projectile.Center = Main.npc[owner].Center;
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
                Projectile.direction = (int)Main.rand.NextFloatDirection();

            float initialSweep = MathHelper.SmoothStep(-0.8f, 1f, Utils.GetLerpValue(ChargeTime, ChargeTime + 200, Time, true));
            float secondSweep = MathHelper.SmoothStep(0f, -2f, Utils.GetLerpValue(ChargeTime + 250, ChargeTime + 600, Time, true)) * Utils.GetLerpValue(ChargeTime + LaserDuration - 10, ChargeTime + 550, Time, true);

            Projectile.velocity = (Projectile.rotation.AngleLerp(Projectile.AngleTo(Main.npc[owner].GetTargetData().Center) + (initialSweep + secondSweep) * Projectile.direction, 0.025f) + (float)Math.Sin(Time * 0.03f) * 0.005f).ToRotationVector2();
            Projectile.rotation = Projectile.velocity.ToRotation();
            Projectile.localAI[0] = Main.npc[owner].localAI[0];

            if (Time < ChargeTime && !Main.rand.NextBool((int)(Time * 0.5f + 2)))
            {
                Particle hue = Particle.NewParticle(Particle.ParticleType<HueLightDust>(), Projectile.Center, Projectile.velocity.RotatedByRandom(0.4f) * Main.rand.NextFloat(5f, 25f), Color.White, 2f);
                hue.data = Projectile.localAI[0];
            }
            else
            {
                if (Time < ChargeTime + LaserDuration + 5 || Main.rand.NextBool((int)(Time - ChargeTime - LaserDuration) + 5))
                {
                    for (int i = 0; i < 5; i++)
                    {
                        Particle hue = Particle.NewParticle(Particle.ParticleType<HueLightDust>(), Projectile.Center, Projectile.velocity.RotatedByRandom(0.4f) * Main.rand.NextFloat(5f, 25f), Color.White, 2f);
                        hue.data = Projectile.localAI[0];
                    }
                }
            }

            if (Time == ChargeTime + LaserDuration + 5 && !Main.dedServ)
            {
                SoundStyle sizzle = new SoundStyle($"{nameof(CalamityHunt)}/Assets/Sounds/Goozma/GoozmaSizzle");
                sizzle.MaxInstances = 0;
                SoundEngine.PlaySound(sizzle, Projectile.Center);
            }

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
                float grow = (float)Math.Pow(Utils.GetLerpValue(ChargeTime - 20, ChargeTime + 40, Time, true) * Utils.GetLerpValue(ChargeTime + LaserDuration + 60, ChargeTime + LaserDuration, Time, true), 3f);

                //if (targetHitbox.IntersectsConeFastInaccurate(Projectile.Center, 3500f * (0.5f + grow * 0.5f), Projectile.rotation, 0.22f * grow))
                //    Dust.NewDustPerfect(targetHitbox.Center(), DustID.TintableDust, Vector2.Zero, 0, Color.Black, 2f).noGravity = true;

                return targetHitbox.IntersectsConeFastInaccurate(Projectile.Center, 3500f * (0.5f + grow * 0.5f), Projectile.rotation, 0.22f * grow);
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

            Color startColor = new GradientColor(SlimeUtils.GoozColorArray, 0.2f, 0.2f).ValueAt(Projectile.localAI[0]);
            startColor.A = 0;

            float laserCharge = Utils.GetLerpValue(50, ChargeTime, Time, true) * Utils.GetLerpValue(ChargeTime + LaserDuration + 90, ChargeTime + LaserDuration + 20, Time, true);
            Main.EntitySpriteDraw(glow.Value, Projectile.Center - Main.screenPosition, null, startColor, 0, glow.Size() * 0.5f, 2f, 0, 0);
            Main.EntitySpriteDraw(glow.Value, Projectile.Center - Main.screenPosition, null, new Color(255, 255, 255, 0) * laserCharge, 0, glow.Size() * 0.5f, 1.5f, 0, 0);

            if (Time >= ChargeTime)
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
                lightningEffect.Parameters["uFreq"].SetValue(1.2f);
                lightningEffect.CurrentTechnique.Passes[0].Apply();
                strip.DrawTrail();
                Main.pixelShader.CurrentTechnique.Passes[0].Apply();
            }
            return false;
        }

        public Color StripColor(float progress)
        {
            float grow = 0.1f + (float)Math.Pow(Utils.GetLerpValue(ChargeTime - 20, ChargeTime + 40, Time, true) * Utils.GetLerpValue(ChargeTime + LaserDuration + 60, ChargeTime + LaserDuration, Time, true), 3f);

            Color color = new GradientColor(SlimeUtils.GoozColorArray, 0.2f, 0.2f).ValueAt(Projectile.localAI[0] - progress * 40f);
            color.A = 0;
            return color * grow;
        }
        public float StripWidth(float progress)
        {
            float start = (float)Math.Pow(progress, 0.6f);
            float cap = (float)Math.Cbrt(Utils.GetLerpValue(1.01f, 0.7f, progress, true));
            float grow = (float)Math.Pow(Utils.GetLerpValue(ChargeTime - 20, ChargeTime + 40, Time, true) * Utils.GetLerpValue(ChargeTime + LaserDuration + 60, ChargeTime + LaserDuration, Time, true), 3f);
            return start * cap * grow * 1300f * (1.1f + (float)Math.Cos(Time) * (0.02f - progress * 0.02f));
        }
    }
}
