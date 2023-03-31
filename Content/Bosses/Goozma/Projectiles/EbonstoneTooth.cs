using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using System.Collections.Generic;
using Terraria;
using Terraria.DataStructures;
using Terraria.ModLoader;

namespace CalamityHunt.Content.Bosses.Goozma.Projectiles
{
    public class EbonstoneTooth : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Ebonstone Tooth");
        }

        public override void SetDefaults()
        {
            Projectile.width = 16;
            Projectile.height = 16;
            Projectile.tileCollide = false;
            Projectile.hostile = true;
            Projectile.friendly = false;
            Projectile.penetrate = -1;
            Projectile.aiStyle = -1;
            Projectile.manualDirectionChange = true;
        }

        public ref float Time => ref Projectile.ai[0];
        public ref float OutTime => ref Projectile.ai[1];

        public override void OnSpawn(IEntitySource source)
        {
            Projectile.direction = Projectile.velocity.X > 0 ? -1 : 1;
            Projectile.rotation = Projectile.velocity.ToRotation() + MathHelper.PiOver2;
            Projectile.velocity = Vector2.Zero;
            Projectile.localAI[0] = Main.rand.Next(3);
        }

        public override void AI()
        {
            Projectile.scale = (0.2f + Utils.GetLerpValue(10, 15, Time, true) * 0.8f) * Utils.GetLerpValue(OutTime, OutTime - 30, Time, true);
            Time++;

            if (Time > OutTime)
                Projectile.Kill();
        }

        public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
        {
            float collisionPoint = 0;
            bool onLine = Collision.CheckAABBvLineCollision(targetHitbox.TopRight(), targetHitbox.Size(), Projectile.Center, Projectile.Center + new Vector2(0, -150 * Projectile.scale).RotatedBy(Projectile.rotation), 6, ref collisionPoint);
            return onLine && Time > 10;
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Asset<Texture2D> texture = ModContent.Request<Texture2D>(Texture);
            Rectangle frame = texture.Frame(4, 1, (int)Projectile.localAI[0], 0);

            SpriteEffects flip = Projectile.direction > 0 ? SpriteEffects.FlipHorizontally : SpriteEffects.None;

            if (Time < 10)
            {
                Main.EntitySpriteDraw(texture.Value, Projectile.Center - Main.screenPosition, frame, new Color(80, 0, 225, 0) * Utils.GetLerpValue(10, -10, Time, true), Projectile.rotation, frame.Size() * new Vector2(0.5f, 1f), Utils.GetLerpValue(-30, -20, Time, true), flip, 0);
                Main.EntitySpriteDraw(texture.Value, Projectile.Center - Main.screenPosition, frame, new Color(0, 0, 0, 20), Projectile.rotation, frame.Size() * new Vector2(0.5f, 1f), Utils.GetLerpValue(-30, -20, Time, true) * 0.6f, flip, 0);
            }

            Main.EntitySpriteDraw(texture.Value, Projectile.Center - Main.screenPosition, frame, Lighting.GetColor(Projectile.Top.ToTileCoordinates()), Projectile.rotation, frame.Size() * new Vector2(0.5f, 1f), Projectile.scale, flip, 0);

            return false;
        }
    }
}
