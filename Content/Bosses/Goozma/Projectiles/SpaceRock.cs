using CalamityHunt.Content.Bosses.Goozma;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using System;
using Terraria;
using Terraria.DataStructures;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityHunt.Content.Bosses.Goozma.Projectiles
{
    public class SpaceRock : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.TrailCacheLength[Type] = 20;
            ProjectileID.Sets.TrailingMode[Type] = 2;
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

            Vector2 sized = new Vector2(42 * (1f + Size));
            if (Projectile.width != (int)sized.X || Projectile.height != (int)sized.Y)
            {
                Projectile.Resize((int)sized.X, (int)sized.Y);
                //Projectile.scale = (Projectile.scale * 0.33f * Size) % 1f;
            }

            if (Time < 50)
                Projectile.velocity *= Utils.GetLerpValue(0, 50, Time, true) * 0.4f;

            Projectile.rotation += 0.1f * Projectile.direction;
            if (Time > 2000)
                Projectile.Kill();

            Projectile.frameCounter++;
            if (Projectile.frameCounter > 1 + (int)(Size * 1.5f))
            {
                Projectile.frameCounter = 0;
                Projectile.frame = (Projectile.frame + 1) % 4;
            }

            Time++;
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
            Asset<Texture2D> flames = ModContent.Request<Texture2D>(Texture + "Flames");
            Asset<Texture2D> bloom = ModContent.Request<Texture2D>($"{nameof(CalamityHunt)}/Assets/Textures/Goozma/GlowSoft");

            float power = Utils.GetLerpValue(0, 20, Time, true);

            for (int i = 0; i < ProjectileID.Sets.TrailCacheLength[Type]; i++)
            {
                Vector2 oldPos = Projectile.oldPos[i] + Projectile.Size * 0.5f;
                float fade = 1f - (float)i / ProjectileID.Sets.TrailCacheLength[Type];
                Main.EntitySpriteDraw(bloom.Value, oldPos - Main.screenPosition, bloom.Frame(), new Color(10, 5, 40, 0) * 0.1f * power, Projectile.velocity.ToRotation() - MathHelper.PiOver2, bloom.Size() * 0.5f, Size * Projectile.scale * (1.5f + fade * 1f), 0, 0);
            }
            for (int i = 0; i < 8; i++)
            {
                Vector2 off = new Vector2(2).RotatedBy(MathHelper.TwoPi / 8f * i + Projectile.rotation) * power;
                Main.EntitySpriteDraw(texture.Value, Projectile.Center + off - Main.screenPosition, texture.Frame(), new Color(100, 60, 30, 0) * power, Projectile.rotation, texture.Size() * 0.5f, new Vector2((float)Projectile.width / texture.Width(), (float)Projectile.height / texture.Height()) * power, 0, 0);
            }

            Rectangle flamesFrame = flames.Frame(1, 4, 0, Projectile.frame);
            Main.EntitySpriteDraw(bloom.Value, Projectile.Center - Main.screenPosition, bloom.Frame(), new Color(15, 5, 50, 0) * 0.2f * power, Projectile.rotation, bloom.Size() * 0.5f, Size * Projectile.scale * 3f, 0, 0);
            Main.EntitySpriteDraw(flames.Value, Projectile.Center - Main.screenPosition, flamesFrame, new Color(100, 60, 30, 0) * 0.1f * power, Projectile.velocity.ToRotation() - MathHelper.PiOver2, flamesFrame.Size() * new Vector2(0.5f, 0.7f), Size * Projectile.scale * 1.2f, 0, 0);
            Main.EntitySpriteDraw(texture.Value, Projectile.Center - Main.screenPosition, texture.Frame(), lightColor, Projectile.rotation, texture.Size() * 0.5f, new Vector2((float)Projectile.width / texture.Width(), (float)Projectile.height / texture.Height()) * power, 0, 0);

            return false;
        }
    }
}
