using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using System;
using Terraria;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityHunt.Content.Bosses.Goozma.Projectiles
{
    public class Novacracker : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Novacracker");
        }

        public override void SetDefaults()
        {
            Projectile.width = 156;
            Projectile.height = 156;
            Projectile.tileCollide = false;
            Projectile.hostile = true;
            Projectile.friendly = false;
            Projectile.penetrate = -1;
            Projectile.aiStyle = -1;
        }

        public ref float Time => ref Projectile.ai[0];

        public override void AI()
        {
            Projectile.velocity *= 0.985f;
            if (Time == 0)
                Projectile.rotation = Main.rand.NextFloat(-3f, 3f);
            Projectile.scale = (1.5f + (float)Math.Pow(Time / 40f, 2f)) * Utils.GetLerpValue(0, 2, Time, true);

            if (Main.rand.NextBool(120))
                Dust.NewDustDirect(Projectile.position, Projectile.width, Projectile.height, DustID.YellowStarDust, Projectile.velocity.X, Projectile.velocity.Y);

            Time++;
            if (Time > Main.rand.Next(35, 45))
                Projectile.Kill();

            Projectile.rotation -= Projectile.direction * 0.03f;
        }

        public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
        {
            return projHitbox.Intersects(targetHitbox) && Time < 20;
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Asset<Texture2D> texture = ModContent.Request<Texture2D>(Texture);
            Asset<Texture2D> bloom = ModContent.Request<Texture2D>($"{nameof(CalamityHunt)}/Assets/Textures/Goozma/GlowSoft");
            Rectangle frame = texture.Frame(1, 7, 0, (int)(Utils.GetLerpValue(0, 45, Time, true) * 7));

            Color drawColor = Color.Lerp(Color.Lerp(new Color(255, 245, 52, 0), new Color(255, 50, 10, 100), Utils.GetLerpValue(4, 12, Time, true)), new Color(35, 30, 70, 110), Utils.GetLerpValue(15, 25, Time, true));
            drawColor.A += 10;
            Color glowColor = Color.Lerp(Color.Lerp(Color.DarkOrange * 0.4f, Color.Red * 0.4f, Utils.GetLerpValue(2, 12, Time, true)), Color.Indigo * 0.2f, Utils.GetLerpValue(15, 25, Time, true));
            glowColor.A = 0;
            Main.EntitySpriteDraw(texture.Value, Projectile.Center - Main.screenPosition, frame, drawColor * Utils.GetLerpValue(37, 15, Time, true), Projectile.rotation, frame.Size() * 0.5f, Projectile.scale, 0, 0);
            Main.EntitySpriteDraw(bloom.Value, Projectile.Center - Main.screenPosition, null, glowColor * 0.3f * Utils.GetLerpValue(40, 15, Time, true), Projectile.rotation, bloom.Size() * 0.5f, Projectile.scale * (3f + Utils.GetLerpValue(0, 40, Time, true) * 5f), 0, 0);

            return false;
        }
    }
}
