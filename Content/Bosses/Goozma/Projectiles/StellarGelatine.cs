using CalamityHunt.Common.Systems.Particles;
using CalamityHunt.Content.Particles;
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
    public class StellarGelatine : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.DrawScreenCheckFluff[Type] = 2000;
            ProjectileID.Sets.TrailCacheLength[Type] = 20;
            ProjectileID.Sets.TrailingMode[Type] = -1;
        }

        public override void SetDefaults()
        {
            Projectile.width = 60;
            Projectile.height = 60;
            Projectile.tileCollide = false;
            Projectile.hostile = true;
            Projectile.friendly = false;
            Projectile.penetrate = -1;
            Projectile.aiStyle = -1;
            Projectile.manualDirectionChange = true;
        }

        public ref float Time => ref Projectile.ai[0];

        public override void OnSpawn(IEntitySource source)
        {
            Projectile.frame = Main.rand.Next(3);
            Projectile.scale *= Main.rand.NextFloat(0.6f, 1.3f);
            Projectile.direction = Main.rand.NextBool().ToDirectionInt();
        }

        private Vector2 saveTarget;
        private float direction;

        public override void AI()
        {
            int owner = -1;
            if (!Main.npc.Any(n => n.type == ModContent.NPCType<StellarGeliath>() && n.active))
            {
                Projectile.active = false;
                return;
            }
            else
                owner = Main.npc.First(n => n.type == ModContent.NPCType<StellarGeliath>() && n.active).whoAmI;

            for (int i = 0; i < 2; i++)
            {
                Particle smoke = Particle.NewParticle(Particle.ParticleType<CosmicSmoke>(), Projectile.Center + Projectile.velocity * 2f + Main.rand.NextVector2Circular(24, 24), Main.rand.NextVector2Circular(5, 5) + Projectile.velocity * i * 0.5f, Color.White, (0.5f + Main.rand.NextFloat()) * Projectile.scale);
                smoke.data = "Cosmos";
            }

            if (Projectile.ai[1] == 0)
            {
                Projectile.velocity += Main.npc[owner].GetTargetData().Velocity * 0.012f;
                Projectile.velocity.Y += 0.3f;
                Projectile.velocity.X += (Main.npc[owner].GetTargetData().Center.X - Projectile.Center.X) * 0.0001f;

                foreach (Projectile otherBit in Main.projectile.Where(n => n.active && n.type == Type && n.whoAmI != Projectile.whoAmI && n.Distance(Projectile.Center) < 30))
                {
                    otherBit.velocity += otherBit.DirectionFrom(Projectile.Center).SafeNormalize(Vector2.Zero) * 0.15f;
                    Projectile.velocity += Projectile.DirectionFrom(otherBit.Center).SafeNormalize(Vector2.Zero) * 0.15f;
                }
            }
            else if (Time < 40)
                Projectile.velocity *= 0.94f;
            else
            {
                Projectile.scale = Utils.GetLerpValue(30, 100, Projectile.Distance(Main.npc[owner].Center), true);
                Projectile.velocity = Vector2.Lerp(Projectile.velocity, Projectile.DirectionTo(Main.npc[owner].Center).SafeNormalize(Vector2.Zero) * Projectile.Distance(Main.npc[owner].GetTargetData().Center) * 0.07f, 0.15f * Utils.GetLerpValue(50, 150, Time, true)).RotatedBy(0.015f * Projectile.direction);
            }
            if (Time > 300 || Projectile.scale < 0.1f)
                Projectile.Kill();


            if (Time > 0)
                Projectile.ai[1] = 1;

            if (Main.rand.NextBool(30))
                Particle.NewParticle(Particle.ParticleType<PrettySparkle>(), Projectile.Center + Main.rand.NextVector2Circular(12, 12) * Projectile.scale + Projectile.velocity, Main.rand.NextVector2Circular(3, 3), new Color(30, 15, 10, 0), (0.4f + Main.rand.NextFloat()) * Projectile.scale);

            Projectile.rotation += Projectile.velocity.Length() * Projectile.direction * 0.02f;

            Time++;
            Projectile.localAI[0]++;
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Asset<Texture2D> texture = ModContent.Request<Texture2D>(Texture);
            Asset<Texture2D> sparkle = TextureAssets.Extra[98];
            Asset<Texture2D> glow = ModContent.Request<Texture2D>($"{nameof(CalamityHunt)}/Assets/Textures/Goozma/GlowSoft");
            Rectangle frame = texture.Frame(3, 1, Projectile.frame, 0);

            float scale = 1f;

            if (Projectile.ai[1] == 0)
            {
                scale = 1f;
            }
            if (Projectile.ai[1] == 1 && Time < 0)
            {
                int telegraphCount = 150;
                Vector2[] telegraphs = new Vector2[telegraphCount];
                telegraphs[0] = new Vector2(0, 40).RotatedBy(direction * 60);
                for (int i = 1; i < telegraphCount; i++)
                {
                    telegraphs[i] = telegraphs[i - 1] + new Vector2(0, 40).RotatedBy(direction * 60 - direction * i);
                }
                for (int i = 1; i < telegraphCount; i++)
                {
                    Vector2 telegraphScale = new Vector2(0.1f + (float)Math.Pow(Utils.GetLerpValue(0, telegraphCount, i, true), 2f) * 2f, 1f);
                    Main.EntitySpriteDraw(sparkle.Value, Projectile.Center + telegraphs[i] - Main.screenPosition, sparkle.Frame(), new Color(255, 10, 20, 0), telegraphs[i].AngleFrom(telegraphs[i - 1]) - MathHelper.PiOver2, sparkle.Size() * 0.5f, telegraphScale, 0, 0);
                }
            }

            Main.EntitySpriteDraw(texture.Value, Projectile.Center - Main.screenPosition, frame, Color.White, Projectile.rotation, frame.Size() * 0.5f, Projectile.scale * scale, 0, 0);

            return false;
        }
    }
}
