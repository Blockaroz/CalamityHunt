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
    public class CosmicDust : ModProjectile
    {
        public override void SetDefaults()
        {
            Projectile.width = 100;
            Projectile.height = 100;
            Projectile.tileCollide = false;
            Projectile.hostile = true;
            Projectile.friendly = false;
            Projectile.penetrate = -1;
            Projectile.aiStyle = -1;
            Projectile.manualDirectionChange = true;
        }

        public ref float Time => ref Projectile.ai[0];
        public ref float Scale => ref Projectile.ai[1];
        public ref float Rotation => ref Projectile.ai[2];

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

            if (Time > 20)
                Projectile.Kill();

            Projectile.velocity *= 0.1f;

            Particle smoke = Particle.NewParticle(Particle.ParticleType<CosmicSmoke>(), Projectile.Center + Main.rand.NextVector2Circular(70, 30).RotatedBy(Rotation) * Scale, Main.rand.NextVector2Circular(5, 5) + Rotation.ToRotationVector2() * 10f, Color.White, (2f + Main.rand.NextFloat()) * Projectile.scale);
            smoke.data = "Cosmos";

            Projectile.scale = Utils.GetLerpValue(20, 0, Time, true) * Scale;

            Time++;
        }

        public override bool PreDraw(ref Color lightColor) => false;
    }
}
