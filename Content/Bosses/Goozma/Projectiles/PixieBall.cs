using CalamityHunt.Common.Systems.Particles;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using ReLogic.Utilities;
using System;
using System.Linq;
using Terraria;
using Terraria.Audio;
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
            Projectile.width = 70;
            Projectile.height = 70;
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
            Projectile.rotation += Projectile.velocity.Length() * 0.01f * (Projectile.velocity.X > 0 ? 1 : -1);
            Projectile.scale = (float)Math.Sqrt(Utils.GetLerpValue(0, 17, Time, true) * Utils.GetLerpValue(480, 440, Time, true)) * 1.3f;
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
                Projectile.velocity = Projectile.DirectionTo(Main.npc[owner].GetTargetData().Center).SafeNormalize(Vector2.Zero) * 36f;
                Cooldown = 0;
            }
            else
            {
                if (Main.rand.NextBool(5))
                    Projectile.velocity += Main.rand.NextVector2Circular(3, 3);

                Projectile.velocity = Vector2.Lerp(Projectile.velocity, Projectile.DirectionTo(Main.npc[owner].Center).SafeNormalize(Vector2.Zero) * 3f, 0.03f) * Utils.GetLerpValue(500, 470, Time, true);
                Projectile.velocity += Projectile.DirectionTo(Main.npc[owner].Center).SafeNormalize(Vector2.Zero) * 0.3f;
            }

            if (Cooldown <= 0)
            {
                foreach (Player player in Main.player.Where(n => n.active && !n.dead && n.Distance(Projectile.Center) < 64))
                {
                    if (HitCount < 0 || HitCount == 1)
                    {
                        HitCount++;
                        Cooldown += 15;
                        Projectile.velocity = -Vector2.UnitY * 10;
                    }
                    else
                        Projectile.velocity = Projectile.DirectionFrom(player.Center).SafeNormalize(Vector2.Zero) * (14f + Projectile.velocity.Length() + player.velocity.Length());
                    
                    Cooldown += 15;
                    
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
                    Main.npc[owner].localAI[1]++;
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

            if (Main.rand.NextBool(5))
            {
                Color glowColor = Main.hslToRgb((Projectile.localAI[0] * 0.01f) % 1f, 1f, 0.6f, 0);
                glowColor.A /= 2;
                Dust.NewDustPerfect(Projectile.Center + Main.rand.NextVector2Circular(36, 36), DustID.AncientLight, Main.rand.NextVector2Circular(3, 3), 0, glowColor, 1f + Main.rand.NextFloat(2f)).noGravity = true;
            }

            if (Main.rand.NextBool(5))
                Particle.NewParticle(Particle.ParticleType<Particles.PrettySparkle>(), Projectile.Center + Main.rand.NextVector2Circular(36, 36), Main.rand.NextVector2Circular(3, 3), Main.hslToRgb((Projectile.localAI[0] * 0.01f) % 1f, 1f, 0.6f, 0), 0.5f + Main.rand.NextFloat());


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

            HandleSound();

            Projectile.localAI[1] = Projectile.Distance(Main.npc[owner].Center) * 0.1f;
            Projectile.localAI[0]++;
            Time++;
        }

        public static SlotId auraSound;
        public static float volume;

        public void HandleSound()
        {
            SoundStyle pixieBallSound = new SoundStyle($"{nameof(CalamityHunt)}/Assets/Sounds/Goozma/Slimes/PixieBallLoop");
            pixieBallSound.IsLooped = true;

            volume = Math.Clamp(1f + Projectile.velocity.Length() * 0.0001f - Main.LocalPlayer.Distance(Projectile.Center) * 0.0005f, 0, 1) * Projectile.scale;

            bool active = SoundEngine.TryGetActiveSound(auraSound, out ActiveSound sound);
            if (!active || !auraSound.IsValid)
                auraSound = SoundEngine.PlaySound(pixieBallSound, Main.npc.First(n => n.type == ModContent.NPCType<Goozma>() && n.active).Center);

            else if (active)
            {
                sound.Volume = volume;
                sound.Position = Projectile.Center;
            }
        }

        public override bool PreKill(int timeLeft)
        {
            SoundEngine.TryGetActiveSound(auraSound, out ActiveSound sound);
            sound.Stop();
            return true;
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Asset<Texture2D> texture = ModContent.Request<Texture2D>(Texture);
            Asset<Texture2D> sparkle = TextureAssets.Extra[98];
            Asset<Texture2D> ring = ModContent.Request<Texture2D>($"{nameof(CalamityHunt)}/Assets/Textures/Goozma/GlowRing");
            Asset<Texture2D> glow = ModContent.Request<Texture2D>($"{nameof(CalamityHunt)}/Assets/Textures/Goozma/GlowSoft");

            Color bloomColor = Main.hslToRgb((Projectile.localAI[0] * 0.01f) % 1f, 1f, 0.7f, 0);
            Color solidColor = Main.hslToRgb((Projectile.localAI[0] * 0.01f) % 1f, 1f, 0.7f, 100);
            SpriteEffects direction = Projectile.velocity.X > 0 ? SpriteEffects.FlipHorizontally : SpriteEffects.None;
            
            Main.EntitySpriteDraw(texture.Value, Projectile.Center - Main.screenPosition, null, Color.Black * 0.1f, Projectile.rotation * 0.5f, texture.Size() * 0.5f, Projectile.scale * 1.5f, 0, 0);
            Main.EntitySpriteDraw(texture.Value, Projectile.Center - Main.screenPosition, null, bloomColor, Projectile.rotation * 1.5f, texture.Size() * 0.5f, Projectile.scale * 0.8f, 0, 0);

            for (int i = 0; i < ProjectileID.Sets.TrailCacheLength[Type]; i++)
            {
                Color trailColor = Main.hslToRgb((Projectile.localAI[0] * 0.03f - i * 0.04f) % 1f, 1f, 0.6f, 0) * 0.15f;
                trailColor.A = 0;
                float fadeOut = 1f - (float)i / ProjectileID.Sets.TrailCacheLength[Type];
                float outScale = (float)Math.Pow(fadeOut, 1.5f);
                Main.EntitySpriteDraw(texture.Value, Projectile.oldPos[i] + Projectile.velocity * i * 0.1f - Main.screenPosition, null, trailColor * outScale, Projectile.oldRot[i], texture.Size() * 0.5f, Projectile.scale * 1.5f - i * 0.2f, direction, 0);
            }

            Main.EntitySpriteDraw(texture.Value, Projectile.Center - Main.screenPosition, null, new Color(100, 100, 100, 0), Projectile.rotation, texture.Size() * 0.5f, Projectile.scale * 0.8f, direction, 0);
            Main.EntitySpriteDraw(texture.Value, Projectile.Center - Main.screenPosition, null, new Color(200, 200, 200, 0), Projectile.rotation, texture.Size() * 0.5f, Projectile.scale * 0.7f, direction, 0);
            Main.EntitySpriteDraw(glow.Value, Projectile.Center - Main.screenPosition, null, bloomColor * 0.3f, Projectile.rotation, glow.Size() * 0.5f, Projectile.scale * 2f, direction, 0);

            float lensAngle = Projectile.AngleFrom(Main.LocalPlayer.Center) + MathHelper.PiOver2;
            float lensPower = 1f + Projectile.Distance(Main.LocalPlayer.Center) * 0.003f;

            float sparkRotation = Projectile.velocity.X * 0.01f;
            float wobble = 1.2f + (float)Math.Sin(Projectile.localAI[0] * 0.5f) * 0.05f;
            Vector2 sparkleScale = new Vector2(0.5f, 6f) * wobble * Projectile.scale;
            Main.EntitySpriteDraw(sparkle.Value, Projectile.Center - Main.screenPosition, null, bloomColor * 0.4f, sparkRotation + MathHelper.PiOver2 + 0.2f, sparkle.Size() * 0.5f, sparkleScale, 0, 0);
            Main.EntitySpriteDraw(sparkle.Value, Projectile.Center - Main.screenPosition, null, bloomColor * 0.4f, sparkRotation + MathHelper.PiOver2 - 0.3f, sparkle.Size() * 0.5f, sparkleScale * 0.6f, 0, 0);
            
            float ringWobble0 = 1.05f + (float)Math.Sin(Projectile.localAI[0] * 0.1f + 0.6f) * 0.01f;
            float ringWobble1 = 1.05f + (float)Math.Sin(Projectile.localAI[0] * 0.1f + 0.3f) * 0.01f;
            Vector2 middleRingOff = new Vector2(0, 50).RotatedBy(lensAngle - 0.3f) * lensPower;

            Main.EntitySpriteDraw(ring.Value, Projectile.Center + middleRingOff - Main.screenPosition, null, bloomColor * 0.05f, sparkRotation + MathHelper.PiOver2 + 0.2f, ring.Size() * 0.5f, new Vector2(1f, 1.05f) * Projectile.scale * ringWobble0, 0, 0);
            Main.EntitySpriteDraw(ring.Value, Projectile.Center - Main.screenPosition, null, bloomColor * 0.1f, sparkRotation + MathHelper.PiOver2 + 0.2f, ring.Size() * 0.5f, new Vector2(1f, 1.05f) * Projectile.scale * 1.4f * ringWobble1, 0, 0);

            Vector2 bottomRingOff = new Vector2(0, 40).RotatedBy(lensAngle + 0.2f) * lensPower;
            Main.EntitySpriteDraw(ring.Value, Projectile.Center + bottomRingOff - Main.screenPosition, null, bloomColor * 0.3f, sparkRotation + MathHelper.PiOver2 + 0.2f, ring.Size() * 0.5f, new Vector2(1f, 1.05f) * Projectile.scale * 0.4f, 0, 0);
            Vector2 topRingOff = new Vector2(0, -60).RotatedBy(lensAngle) * lensPower;
            Main.EntitySpriteDraw(ring.Value, Projectile.Center + topRingOff - Main.screenPosition, null, bloomColor * 0.3f, sparkRotation + MathHelper.PiOver2 + 0.2f, ring.Size() * 0.5f, new Vector2(1f, 1.05f) * Projectile.scale * 0.8f, 0, 0);

            Vector2 topFlareOff = topRingOff * ringWobble0 * 1.1f;
            for (int i = 0; i < 3; i++)
                Main.EntitySpriteDraw(sparkle.Value, Projectile.Center + topFlareOff - Main.screenPosition, null, bloomColor * 0.3f, MathHelper.TwoPi / 3f * i, sparkle.Size() * 0.5f, new Vector2(0.3f, 1.5f), 0, 0);

            Main.EntitySpriteDraw(glow.Value, Projectile.Center - Main.screenPosition, null, bloomColor * (0.3f / lensPower), Projectile.rotation * 0.5f, glow.Size() * 0.5f, Projectile.scale * 6f, 0, 0);
            Main.EntitySpriteDraw(glow.Value, Projectile.Center - Main.screenPosition, null, bloomColor * (0.08f / lensPower), Projectile.rotation * 0.5f, glow.Size() * 0.5f, Projectile.scale * 15f, 0, 0);

            return false;
        }
    }
}
