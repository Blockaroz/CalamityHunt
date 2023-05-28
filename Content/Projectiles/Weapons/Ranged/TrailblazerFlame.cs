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
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityHunt.Content.Projectiles.Weapons.Ranged
{
    public class TrailblazerFlame : ModProjectile
    {
        public override void SetDefaults()
        {
            Projectile.CloneDefaults(ProjectileID.Flames);
            Projectile.aiStyle = -1;
            Projectile.penetrate = -1;
            Projectile.timeLeft = 80;
        }

        public ref float Time => ref Projectile.ai[0];

        public override void AI()
        {
            if (Time == 0)
            {
                Projectile.rotation = Main.rand.NextFloat(-2f, 2f);
            }

            Projectile.scale = MathF.Sqrt(Utils.GetLerpValue(-3, 35, Time, true));
            float expand = Utils.GetLerpValue(0, 40, Time, true);

            Color glowColor = new GradientColor(SlimeUtils.GoozOilColors, 0.2f, 0.2f).ValueAt(Projectile.localAI[0]);
            glowColor.A /= 3;
            if (Main.rand.NextBool(8))
            {
                Particle hue = Particle.NewParticle(Particle.ParticleType<HueLightDust>(), Projectile.Center + Main.rand.NextVector2Circular(100, 100) * expand, Projectile.velocity * Main.rand.NextFloat(), glowColor, (1f + Main.rand.NextFloat()));
                hue.data = Projectile.localAI[0];
            }                   
            
            if (Main.rand.NextBool(12))
            {
                Dust dust = Dust.NewDustPerfect(Projectile.Center + Main.rand.NextVector2Circular(100, 100) * expand, DustID.DarkCelestial, Projectile.velocity * Main.rand.NextFloat(), 0, Color.Black, 0.3f + Main.rand.NextFloat());
                dust.noGravity = true;
            }            
            
            if (Main.rand.NextBool(3))
                Particle.NewParticle(Particle.ParticleType<CosmicSmoke>(), Projectile.Center + Main.rand.NextVector2Circular(60, 60) * expand, Projectile.velocity * Main.rand.NextFloat(), glowColor, (1f + Main.rand.NextFloat()) * expand * 0.5f);

            Projectile.frame = (int)(Utils.GetLerpValue(0, 30, Time, true) * 3f + Utils.GetLerpValue(70, 80, Time, true) * 4f);
            Time++;
            Projectile.localAI[0] = Main.GlobalTimeWrappedHourly * 50f - Time * 0.4f;
        }

        public override bool OnTileCollide(Vector2 oldVelocity)
        {
            Projectile.velocity *= 0.1f;
            return false;
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D texture = ModContent.Request<Texture2D>(Texture).Value;
            Rectangle frame = texture.Frame(1, 7, 0, Projectile.frame);

            Color backColor = new GradientColor(SlimeUtils.GoozOilColors, 0.2f, 0.2f).ValueAt(Projectile.localAI[0]) * 0.8f;
            backColor.A = 100;
            Color glowColor = new GradientColor(SlimeUtils.GoozOilColors, 0.2f, 0.2f).ValueAt(Projectile.localAI[0]);
            glowColor.A = 0;
            Color backDrawColor = Color.Lerp(backColor, Color.DimGray * 0.5f, Utils.GetLerpValue(50, 70, Time, true)) * Utils.GetLerpValue(80, 60, Time, true);
            Color drawColor = Color.Lerp(glowColor, Color.DimGray * 0.5f, Utils.GetLerpValue(50, 70, Time, true)) * Utils.GetLerpValue(80, 60, Time, true);

            for (int i = 0; i < 5; i++)
            {
                Color trailColor = backDrawColor * (1f - i / 5f);
                Main.EntitySpriteDraw(texture, Projectile.Center - Projectile.velocity * i - Main.screenPosition, frame, trailColor, Projectile.rotation + Main.GlobalTimeWrappedHourly * 7f * (1f + i / 3f) * -Projectile.direction, frame.Size() * 0.5f, Projectile.scale * 1.1f, 0, 0);
            }

            Main.EntitySpriteDraw(texture, Projectile.Center - Main.screenPosition, frame, backDrawColor, Projectile.rotation + Main.GlobalTimeWrappedHourly * 7f * -Projectile.direction, frame.Size() * 0.5f, Projectile.scale * 1.1f, 0, 0);
            Main.EntitySpriteDraw(texture, Projectile.Center - Main.screenPosition, frame, drawColor, Projectile.rotation + Main.GlobalTimeWrappedHourly * 7f * -Projectile.direction, frame.Size() * 0.5f, Projectile.scale, 0, 0);
            Main.EntitySpriteDraw(texture, Projectile.Center - Main.screenPosition, frame, drawColor, Projectile.rotation + Main.GlobalTimeWrappedHourly * 7f * -Projectile.direction, frame.Size() * 0.5f, Projectile.scale, 0, 0);

            return false;
        }
    }
}
