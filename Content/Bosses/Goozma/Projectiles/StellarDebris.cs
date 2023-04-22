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
    public class StellarDebris : ModProjectile
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
            Projectile.hostile = false;
            Projectile.friendly = false;
            Projectile.penetrate = -1;
            Projectile.aiStyle = -1;
            Projectile.manualDirectionChange = true;
        }

        public ref float Time => ref Projectile.ai[0];
        public ref float TopTime => ref Projectile.ai[1];

        public override void OnSpawn(IEntitySource source)
        {
            //Projectile.frame = Main.rand.Next(3);
            Projectile.scale *= Main.rand.NextFloat(0.7f, 1.2f);
            Projectile.direction = Main.rand.NextBool().ToDirectionInt();
            TopTime = 5;
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

            Projectile.velocity = Vector2.Lerp(Projectile.velocity, Projectile.DirectionTo(Main.npc[owner].GetTargetData().Center).SafeNormalize(Vector2.Zero) * 15, 0.0001f);

            if (Time % TopTime == 1)
            {
                Projectile.direction *= -1;
                TopTime = Main.rand.Next(80, 140);
            }

            if (Time % TopTime > TopTime * 0.8f)
                Projectile.velocity = Projectile.velocity.RotatedBy(0.1f * Projectile.direction);

            if (Time > 500)
            {
                Projectile.damage = 0;
                Projectile.scale = Utils.GetLerpValue(40, 120, Projectile.Distance(Main.npc[owner].Center), true);
                Projectile.velocity = Vector2.Lerp(Projectile.velocity, Projectile.DirectionTo(Main.npc[owner].Center).SafeNormalize(Vector2.Zero) * Projectile.Distance(Main.npc[owner].GetTargetData().Center) * 0.2f * Utils.GetLerpValue(500, 600, Time, true), 0.2f * Utils.GetLerpValue(500, 600, Time, true)).RotatedBy(0.01f * Projectile.direction);
            }

            if (Time > 600)
                Projectile.Kill();

            Particle smoke = Particle.NewParticle(Particle.ParticleType<CosmicSmoke>(), Projectile.Center + Projectile.velocity * 2f + Main.rand.NextVector2Circular(24, 24), Main.rand.NextVector2Circular(5, 5) + Projectile.velocity * 0.2f, Color.White, (1f + Main.rand.NextFloat()) * Projectile.scale);
            smoke.data = "Cosmos";

            if (Main.rand.NextBool(50))
                Particle.NewParticle(Particle.ParticleType<PrettySparkle>(), Projectile.Center + Main.rand.NextVector2Circular(24, 24) * Projectile.scale + Projectile.velocity, Main.rand.NextVector2Circular(3, 3), new Color(30, 15, 10, 0), (0.2f + Main.rand.NextFloat()) * Projectile.scale);

            Projectile.rotation += Projectile.velocity.Length() * Projectile.direction * 0.02f;

            Time++;
            Projectile.localAI[0]++;
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Asset<Texture2D> texture = ModContent.Request<Texture2D>(Texture);
            Asset<Texture2D> aura = ModContent.Request<Texture2D>(Texture + "Aura");
            Asset<Texture2D> glow = ModContent.Request<Texture2D>($"{nameof(CalamityHunt)}/Assets/Textures/Goozma/GlowSoft");

            for (int i = 0; i < 2; i++)
            {
                float wobble = (float)Math.Sin(Projectile.localAI[0] * 0.15f + i * 1.3f) * 0.05f;
                float rotation = Projectile.rotation * (0.5f + i * 0.1f);
                Main.EntitySpriteDraw(aura.Value, Projectile.Center - Main.screenPosition, aura.Frame(), new Color(30, 15, 10, 0) * 0.5f, rotation, aura.Size() * 0.5f, Projectile.scale * (0.8f + wobble + i * 0.3f), 0, 0);
            }
            Main.EntitySpriteDraw(glow.Value, Projectile.Center - Main.screenPosition, glow.Frame(), new Color(30, 15, 10, 0), Projectile.rotation, glow.Size() * 0.5f, Projectile.scale * 2f, 0, 0);

            Main.EntitySpriteDraw(texture.Value, Projectile.Center - Main.screenPosition, texture.Frame(), Color.White, Projectile.rotation, texture.Size() * 0.5f, Projectile.scale, 0, 0);

            return false;
        }
    }
}
