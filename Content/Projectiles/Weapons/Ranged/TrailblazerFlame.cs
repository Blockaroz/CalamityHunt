using CalamityHunt.Common.Systems.Particles;
using CalamityHunt.Content.Bosses.Goozma;
using CalamityHunt.Content.Buffs;
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
            Projectile.width = 32;
            Projectile.height = 32;
            Projectile.aiStyle = -1;
            Projectile.penetrate = -1;
            Projectile.timeLeft = 80;
        }

        public ref float Time => ref Projectile.ai[0];
        public ref float Target => ref Projectile.ai[1];

        public override void AI()
        {
            if (Time == 0)
            {
                Projectile.rotation = Main.rand.NextFloat(-2f, 2f);
                Target = -1;
            }

            Projectile.scale = Utils.GetLerpValue(-5, 30, Time, true) + Utils.GetLerpValue(65, 80, Time, true);
            float expand = Utils.GetLerpValue(0, 80, Time, true);

            Color glowColor = new GradientColor(SlimeUtils.GoozOilColors, 0.2f, 0.2f).ValueAt(Projectile.localAI[0]);
            glowColor.A /= 3;

            if (Main.rand.NextBool(8))
            {
                Particle hue = Particle.NewParticle(Particle.ParticleType<HueLightDust>(), Projectile.Center + Main.rand.NextVector2Circular(100, 100) * expand, Projectile.velocity * Main.rand.NextFloat(), glowColor, (1f + Main.rand.NextFloat()));
                hue.data = Projectile.localAI[0];
            }

            if (Main.rand.NextBool(3))
                Particle.NewParticle(Particle.ParticleType<CosmicSmoke>(), Projectile.Center + Main.rand.NextVector2Circular(100, 100) * expand, Projectile.velocity * Main.rand.NextFloat(), glowColor, (1f + Main.rand.NextFloat()) * expand * 0.5f);

            if (Main.rand.NextBool(5) && Projectile.velocity.LengthSquared() > 2f)
            {
                Dust dust = Dust.NewDustPerfect(Projectile.Center + Main.rand.NextVector2Circular(100, 100) * expand, DustID.Sand, Projectile.velocity * Main.rand.NextFloat(3f), 0, Color.Black, 0.3f + Main.rand.NextFloat());
                dust.noGravity = true;
            }

            if (Time > 20)
            {
                if (Target > -1)
                {
                    Projectile.velocity += Projectile.DirectionTo(Main.npc[(int)Target].Center) * Utils.GetLerpValue(1200, 0, Projectile.Distance(Main.npc[(int)Target].Center), true);
                    Projectile.velocity = Projectile.velocity.SafeNormalize(Vector2.Zero) * Projectile.oldVelocity.Length();
                }
                else
                {
                    Target = Projectile.FindTargetWithLineOfSight(1200);
                    if (Target > -1 && Main.netMode != NetmodeID.MultiplayerClient)
                        Projectile.netUpdate = true;
                }
            }

            if (Time == 30 || Time > 70)
                Projectile.velocity += Main.rand.NextVector2Circular(1, 1);

            Projectile.frame = (int)(Utils.GetLerpValue(8, 30, Time, true) * 5f + Utils.GetLerpValue(40, 90, Time, true) * 4f);
            Time++;
            Projectile.localAI[0] = Main.GlobalTimeWrappedHourly * 70f - Time * 0.5f;
        }

        public override bool OnTileCollide(Vector2 oldVelocity)
        {
            Projectile.velocity *= 0.1f;
            return false;
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            //Projectile.velocity *= 0.85f;
            target.AddBuff(ModContent.BuffType<FusionBurn>(), 180);
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D texture = ModContent.Request<Texture2D>(Texture).Value;
            Rectangle frame = texture.Frame(1, 9, 0, Projectile.frame);

            Color backColor = new GradientColor(SlimeUtils.GoozOilColors, 0.2f, 0.2f).ValueAt(Projectile.localAI[0]) * 0.9f;
            backColor.A = 180;
            Color glowColor = Color.Lerp(new GradientColor(SlimeUtils.GoozOilColors, 0.2f, 0.2f).ValueAt(Projectile.localAI[0]), Color.White, 0.5f) * Utils.GetLerpValue(70, 50, Time, true);
            glowColor.A = 0;
            Color backDrawColor = backColor * Utils.GetLerpValue(80, 50, Time, true);
            Color drawColor = glowColor * Utils.GetLerpValue(80, 30, Time, true);

            for (int i = 0; i < 4; i++)
            {
                Color trailColor = backDrawColor * (1f - i / 4f);
                Vector2 off = Projectile.velocity * i * 3f * Utils.GetLerpValue(1, 15, Time, true);
                Main.EntitySpriteDraw(texture, Projectile.Center - off - Main.screenPosition, frame, trailColor, Projectile.rotation + Main.GlobalTimeWrappedHourly * 9f * (1f + i / 4f) * -Projectile.direction, frame.Size() * 0.5f, Projectile.scale * 1.2f, 0, 0);
                Main.EntitySpriteDraw(texture, Projectile.Center - off - Main.screenPosition, frame, drawColor * (1f - i / 4f), Projectile.rotation + Main.GlobalTimeWrappedHourly * 9f * -Projectile.direction, frame.Size() * 0.5f, Projectile.scale, 0, 0);
            }

            Main.EntitySpriteDraw(texture, Projectile.Center - Main.screenPosition, frame, backDrawColor, Projectile.rotation + Main.GlobalTimeWrappedHourly * 9f * -Projectile.direction, frame.Size() * 0.5f, Projectile.scale * 1.2f, 0, 0);
            Main.EntitySpriteDraw(texture, Projectile.Center - Main.screenPosition, frame, drawColor, Projectile.rotation + Main.GlobalTimeWrappedHourly * 9f * -Projectile.direction, frame.Size() * 0.5f, Projectile.scale, 0, 0);

            return false;
        }
    }
}
