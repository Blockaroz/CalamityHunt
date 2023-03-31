using CalamityHunt.Content.Bosses.Goozma.Slimes;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using System;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityHunt.Content.Bosses.Goozma.Projectiles
{
    public class ToxicSludge : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Toxic Sludge");
        }

        public override void SetDefaults()
        {
            Projectile.width = 42;
            Projectile.height = 42;
            Projectile.tileCollide = false;
            Projectile.hostile = true;
            Projectile.friendly = false;
            Projectile.penetrate = -1;
            Projectile.aiStyle = -1;
            Projectile.timeLeft = 180;
        }

        public ref float Time => ref Projectile.ai[0];

        public override void OnSpawn(IEntitySource source)
        {
            Projectile.localAI[0] = Main.rand.NextFloat(0.9f, 1.15f);
            Projectile.localAI[1] = Main.rand.Next(250);
            Projectile.rotation = Main.rand.NextFloat(-1f, 1f);
        }

        public override void AI()
        {
            Projectile.scale = (float)Math.Sqrt(Utils.GetLerpValue(-5, 12, Time, true)) * Projectile.localAI[0];
            Projectile.velocity.Y -= Main.rand.NextFloat(0.05f, 0.4f);

            if (Main.rand.NextBool(9))
                Dust.NewDustPerfect(Projectile.Center + Main.rand.NextVector2Circular(24, 24), DustID.Corruption, Main.rand.NextVector2Circular(5, 5), 0, Color.White, 1.5f).noGravity = true;

            if (Main.rand.NextBool())
            {
                Color color = Color.Lerp(Color.DarkGreen, Color.DarkOrchid, 0.7f + Main.rand.NextFloat(0.3f));
                color.A /= 2;
                Dust.NewDustPerfect(Projectile.Center + Main.rand.NextVector2Circular(24, 24), 4, Projectile.velocity * 0.5f, 150, color, 2f).noGravity = true;
            }

            Time++;
            Projectile.rotation += 0.07f * Projectile.direction;
        }

        public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
        {
            if (Time > 20)
                return base.Colliding(projHitbox, targetHitbox);

            return false;
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Asset<Texture2D> texture = ModContent.Request<Texture2D>(Texture);
            Asset<Texture2D> bloom = ModContent.Request<Texture2D>($"{nameof(CalamityHunt)}/Assets/Textures/Goozma/GlowSoft");
            Rectangle frame = texture.Frame(1, 1);

            Color glowColor = new Color(60, 20, 170, 0);
            float scaleUp = 1f;
            if (Projectile.localAI[1] > 190)
            {
                texture = ModContent.Request<Texture2D>($"{nameof(CalamityHunt)}/Assets/Textures/Goozma/Crowns/CorruptEye");
                frame = texture.Frame(5, 1, (int)(Projectile.localAI[1] % 3), 0);
                scaleUp = 1.1f;
            }

            Main.EntitySpriteDraw(bloom.Value, Projectile.Center - Main.screenPosition, null, glowColor * 0.5f, Projectile.rotation, bloom.Size() * 0.5f, Projectile.scale * scaleUp * 2f, 0, 0);
            Main.EntitySpriteDraw(texture.Value, Projectile.Center - Main.screenPosition, frame, Color.Black * 0.05f, Projectile.rotation, frame.Size() * 0.5f, Projectile.scale * scaleUp * 1.2f, 0, 0);
            Main.EntitySpriteDraw(texture.Value, Projectile.Center - Main.screenPosition, frame, glowColor, Projectile.rotation, frame.Size() * 0.5f, Projectile.scale * scaleUp * 1.1f, 0, 0);
            Main.EntitySpriteDraw(texture.Value, Projectile.Center - Main.screenPosition, frame, lightColor, Projectile.rotation, frame.Size() * 0.5f, Projectile.scale * scaleUp, 0, 0);


            return false;
        }
    }
}
