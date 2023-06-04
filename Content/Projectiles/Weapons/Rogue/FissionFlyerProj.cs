using CalamityHunt.Common.Systems.Particles;
using CalamityHunt.Content.Bosses.Goozma;
using CalamityHunt.Content.Particles;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityHunt.Content.Projectiles.Weapons.Rogue
{
    public class FissionFlyerProj : ModProjectile
    {
        public override void SetDefaults()
        {
            Projectile.width = 36;
            Projectile.height = 36;
            Projectile.tileCollide = true;
            Projectile.aiStyle = -1;
            Projectile.penetrate = -1;
            Projectile.friendly = true;
            Projectile.hostile = false;
            Projectile.timeLeft = 10000;
            Projectile.DamageType = DamageClass.Throwing;
            if (ModLoader.HasMod("CalamityMod"))
            {
                DamageClass d;
                Mod calamity = ModLoader.GetMod("CalamityMod");
                calamity.TryFind<DamageClass>("RogueDamageClass", out d);
                Projectile.DamageType = d;
            }
        }

        public ref float Time => ref Projectile.ai[0];

        public override void AI()
        {
            Color glowColor = new GradientColor(SlimeUtils.GoozOilColors, 0.2f, 0.2f).ValueAt(Projectile.localAI[0]);

            if (Projectile.ai[1] == 0)
            {
                Projectile.scale = 0.3f + Utils.GetLerpValue(0, 60, Time, true) * 0.7f;
                if (Time == 105)
                {
                    SoundEngine.PlaySound(SoundID.Item84, Projectile.Center);

                    for (int i = 0; i < 3; i++)
                    {
                        Vector2 velocity = new Vector2(0, 10).RotatedBy(MathHelper.TwoPi / 3f * i);
                        Projectile ring = Projectile.NewProjectileDirect(Projectile.GetSource_FromThis(), Projectile.Center, velocity, ModContent.ProjectileType<FissionFlyerMiniRing>(), Projectile.damage, Projectile.knockBack, Projectile.owner);
                        ring.ai[0] = Time;
                    }

                    for (int j = 0; j < 40; j++)
                    {
                        Particle.NewParticle(Particle.ParticleType<HueLightDust>(), Projectile.Center, Main.rand.NextVector2Circular(5, 5), glowColor, 1f + Main.rand.NextFloat());
                        Particle.NewParticle(Particle.ParticleType<HueLightDust>(), Projectile.Center, Main.rand.NextVector2Circular(2, 2), glowColor, Main.rand.NextFloat());
                    }
                }

                int target = Projectile.FindTargetWithLineOfSight();
                if (target > -1)
                {
                    Projectile.velocity += Projectile.DirectionTo(Main.npc[target].Center) * 0.5f;
                    Projectile.velocity = Projectile.velocity.SafeNormalize(Vector2.Zero) * Projectile.oldVelocity.Length();
                }

                Projectile.extraUpdates = Time < 40 ? 2 : 0;
                Projectile.velocity *= Time < 40 ? 0.99f : 0.95f;

                if (Time > 60)
                    Projectile.rotation = Projectile.rotation.AngleLerp(Projectile.velocity.Y, 0.01f);
                else
                    Projectile.rotation = Projectile.velocity.ToRotation() + MathHelper.PiOver2;

                if (Time == 60)
                    Gore.NewGorePerfect(Projectile.GetSource_FromThis(), Projectile.Center, -Vector2.UnitY, GoreID.TreeLeaf_GemTreeRuby, 1f);

                if (Time > 130)
                    Projectile.ai[1] = 1f;
            }
            if (Projectile.ai[1] == 1f)
            {
                Projectile.tileCollide = false;
                Projectile.extraUpdates = 2;

                Projectile.rotation = Projectile.velocity.ToRotation() + MathHelper.PiOver2;
                Projectile.scale = 0.1f + Utils.GetLerpValue(0, 600, Projectile.Distance(Main.player[Projectile.owner].MountedCenter), true) * 0.9f;

                Projectile.velocity += Projectile.DirectionTo(Main.player[Projectile.owner].MountedCenter);
                Projectile.velocity = Vector2.Lerp(Projectile.velocity, Projectile.DirectionTo(Main.player[Projectile.owner].MountedCenter) * Projectile.oldVelocity.Length(), 0.1f);

                if (Projectile.Distance(Main.player[Projectile.owner].MountedCenter) < 40)
                    Projectile.Kill();
            }

            if (Time < 60 || Time > 120)
            {
                Vector2 off = new Vector2(22, 0).RotatedBy(Projectile.rotation) * Projectile.scale;
                Particle.NewParticle(Particle.ParticleType<HueLightDust>(), Projectile.Center + off, Projectile.velocity * 0.1f, glowColor, 2f * Projectile.scale);
                Particle.NewParticle(Particle.ParticleType<HueLightDust>(), Projectile.Center - off, Projectile.velocity * 0.1f, glowColor, 2f * Projectile.scale);
            }

            if (Main.rand.NextBool(3))
                Particle.NewParticle(Particle.ParticleType<HueLightDust>(), Projectile.Center - Main.rand.NextVector2Circular(20, 20), Projectile.velocity * Main.rand.NextFloat(), glowColor, 0.5f + Main.rand.NextFloat());

            Dust dust = Dust.NewDustPerfect(Projectile.Center - Main.rand.NextVector2Circular(30, 30), DustID.Sand, Projectile.velocity * Main.rand.NextFloat(), 0, Color.Black, Main.rand.NextFloat());
            dust.noGravity = true;

            Time++;
            Projectile.localAI[0] = Main.GlobalTimeWrappedHourly * 40f + Time;
        }

        public override bool OnTileCollide(Vector2 oldVelocity)
        {
            if (MathF.Abs(oldVelocity.X - Projectile.velocity.X) > 0)
                Projectile.velocity.X = -oldVelocity.X * 1.33f;            
            if (MathF.Abs(oldVelocity.Y - Projectile.velocity.Y) > 0)
                Projectile.velocity.Y = -oldVelocity.Y * 1.33f;
            return false;
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D texture = ModContent.Request<Texture2D>(Texture).Value;
            Texture2D ring = ModContent.Request<Texture2D>(Texture + "Ring").Value;
            Texture2D handle = ModContent.Request<Texture2D>(Texture + "Handle").Value;

            if (Time < 60)
                Main.EntitySpriteDraw(handle, Projectile.Center - Main.screenPosition, handle.Frame(), Color.White, Projectile.rotation, handle.Size() * 0.5f, Projectile.scale, 0, 0);

            Color backColor = new GradientColor(SlimeUtils.GoozOilColors, 0.2f, 0.2f).ValueAt(Projectile.localAI[0]);
            backColor.A = 200;
            Color glowColor = Color.Lerp(new GradientColor(SlimeUtils.GoozOilColors, 0.2f, 0.2f).ValueAt(Projectile.localAI[0]), Color.White, 0.3f);
            glowColor.A = 0;

            float expand = MathF.Cbrt(Utils.GetLerpValue(70, 90, Time, true)) * (1f - MathF.Cbrt(Utils.GetLerpValue(100, 110, Time, true)) * 0.5f) * MathF.Sqrt(Utils.GetLerpValue(130, 120, Time, true));

            float ringScale = (1f - expand * 0.8f) * Projectile.scale;
            Main.EntitySpriteDraw(ring, Projectile.Center - Main.screenPosition, ring.Frame(), backColor, Main.GlobalTimeWrappedHourly * 9f * Projectile.direction, ring.Size() * 0.5f, ringScale * 0.55f, 0, 0);
            Main.EntitySpriteDraw(ring, Projectile.Center - Main.screenPosition, ring.Frame(), glowColor, Main.GlobalTimeWrappedHourly * 7f * Projectile.direction, ring.Size() * 0.5f, ringScale * 0.5f, 0, 0);
            Main.EntitySpriteDraw(ring, Projectile.Center - Main.screenPosition, ring.Frame(), glowColor * 0.1f, Main.GlobalTimeWrappedHourly * 5f * Projectile.direction, ring.Size() * 0.5f, ringScale * 0.8f, 0, 0);

            Rectangle left = texture.Frame(2, 1, 0, 0);
            Rectangle right = texture.Frame(2, 1, 1, 0);

            Vector2 off = new Vector2(2f + expand * 30f, 0).RotatedBy(Projectile.rotation) * Projectile.scale;
            Main.EntitySpriteDraw(texture, Projectile.Center - off - Main.screenPosition, left, Color.White, Projectile.rotation, left.Size() * new Vector2(1f, 0.5f), Projectile.scale, 0, 0);
            Main.EntitySpriteDraw(texture, Projectile.Center + off - Main.screenPosition, right, Color.White, Projectile.rotation, right.Size() * new Vector2(0f, 0.5f), Projectile.scale, 0, 0);

            return false;
        }
    }
}
