using CalamityHunt.Content.Bosses.Goozma.Slimes;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using System;
using System.Linq;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityHunt.Content.Bosses.Goozma.Projectiles
{
    public class TrailingStar : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Trailing Star");
            ProjectileID.Sets.DrawScreenCheckFluff[Type] = 10000;
        }

        public override void SetDefaults()
        {
            Projectile.width = 48;
            Projectile.height = 48;
            Projectile.tileCollide = false;
            Projectile.hostile = true;
            Projectile.friendly = false;
            Projectile.penetrate = -1;
            Projectile.aiStyle = -1;
            Projectile.timeLeft = 200;
        }

        public ref float Time => ref Projectile.ai[0];

        public override void AI()
        {
            Projectile.scale = (float)Math.Sqrt(Utils.GetLerpValue(0, 10, Time, true) * Utils.GetLerpValue(200, 180, Time, true));

            if (Main.rand.NextBool(10))
            {
                int goreID = Utils.SelectRandom(Main.rand, 16, 17, 16, 17);
                Gore.NewGore(Projectile.GetSource_FromAI(), Projectile.Center, Projectile.velocity * 0.2f, goreID);
            }

            if (Main.rand.NextBool(2))
                Dust.NewDustDirect(Projectile.position, Projectile.width, Projectile.height, DustID.YellowStarDust);

            Time++;
            Projectile.rotation += Projectile.direction * 0.07f;
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Asset<Texture2D> star = ModContent.Request<Texture2D>(Texture);
            Asset<Texture2D> trail = TextureAssets.Extra[91];

            Main.EntitySpriteDraw(trail.Value, Projectile.Center - Main.screenPosition, null, new Color(10, 0, 10, 100), Projectile.velocity.ToRotation() + MathHelper.PiOver2, trail.Size() * new Vector2(0.5f, 0.18f), Projectile.scale * 1.5f, 0, 0);

            for (int i = 0; i < 5; i++)
            {
                Vector2 offset = Projectile.velocity * i * 5 * Projectile.scale;
                Main.EntitySpriteDraw(trail.Value, Projectile.Center - offset - Main.screenPosition, null, new Color(20, 0, 170, 0) * 0.2f * ((5f - i) / 5f), Projectile.velocity.ToRotation() + MathHelper.PiOver2, trail.Size() * new Vector2(0.5f, 0.18f), Projectile.scale * 3 * ((5f - i) / 5f), 0, 0);
            }
                        
            for (int i = 0; i < 5; i++)
            {
                Vector2 offset = new Vector2(16, 0).RotatedBy(MathHelper.TwoPi / 3f * i + Projectile.rotation * 0.5f);
                Main.EntitySpriteDraw(trail.Value, Projectile.Center + offset - Main.screenPosition, null, new Color(30, 0, 170, 0) * 0.4f, Projectile.velocity.ToRotation() + MathHelper.PiOver2, trail.Size() * new Vector2(0.5f, 0.18f), Projectile.scale * 2, 0, 0);
            }

            Main.EntitySpriteDraw(star.Value, Projectile.Center - Main.screenPosition, null, Color.LightGoldenrodYellow, Projectile.rotation, star.Size() * new Vector2(0.5f, 0.57f), Projectile.scale * 1.4f, 0, 0);
            Main.EntitySpriteDraw(star.Value, Projectile.Center - Main.screenPosition, null, new Color(255, 10, 30, 0), Projectile.rotation, star.Size() * new Vector2(0.5f, 0.57f), Projectile.scale * 1.8f, 0, 0);

            return false;
        }
    }
}
