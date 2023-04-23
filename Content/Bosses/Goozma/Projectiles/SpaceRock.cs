using CalamityHunt.Content.Bosses.Goozma;
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
    public class SpaceRock : ModProjectile
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
        public ref float Size => ref Projectile.ai[1];

        public override void OnSpawn(IEntitySource source)
        {
            Projectile.localAI[0] = Main.rand.NextFloat(0.9f, 1.15f);
            Projectile.localAI[1] = Main.rand.Next(250);
            Projectile.rotation = Main.rand.NextFloat(-1f, 1f);
        }

        public override void AI()
        {
            if (Main.rand.NextBool(3))
                Dust.NewDustPerfect(Projectile.Center + Main.rand.NextVector2Circular(Projectile.width, Projectile.height) * 0.5f, DustID.Stone, Main.rand.NextVector2Circular(3, 3) - Projectile.velocity, 0, Color.White, 1f + Main.rand.NextFloat(2f)).noGravity = true;

            Size = 1f + (float)Math.Sin(Time * 0.2f) * 0.5f;
            Vector2 sized = new Vector2(48 * (1f + Size));
            if (Projectile.width != (int)sized.X || Projectile.height != (int)sized.Y)
            {
                Projectile.Resize((int)sized.X, (int)sized.Y);
                Projectile.scale = (Projectile.scale * 0.33f * Size) % 3f;
            }

            Time++;
            Projectile.rotation += 0.1f * Projectile.direction;
        }

        public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
        {
            if (Time > 20)
            {
                Vector2 radialPoint = Projectile.Center + Projectile.DirectionTo(targetHitbox.Center()).SafeNormalize(Vector2.Zero) * Math.Min(Projectile.Distance(targetHitbox.Center()), Projectile.width);
                if (targetHitbox.Contains(radialPoint.ToPoint()))
                    return true;
            }

            return false;
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Asset<Texture2D> texture = ModContent.Request<Texture2D>(Texture);
            Asset<Texture2D> bloom = ModContent.Request<Texture2D>($"{nameof(CalamityHunt)}/Assets/Textures/Goozma/GlowSoft");

            float power = Utils.GetLerpValue(0, 20, Time, true);
            Main.EntitySpriteDraw(texture.Value, Projectile.Center - Main.screenPosition, texture.Frame(), lightColor, Projectile.rotation, texture.Size() * 0.5f, new Vector2((float)Projectile.width / texture.Width(), (float)Projectile.height / texture.Height()), 0, 0);

            return false;
        }
    }
}
