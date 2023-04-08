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
    public class ShootingStar : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Shooting Star");
            ProjectileID.Sets.DrawScreenCheckFluff[Type] = 10000;
        }

        public override void SetDefaults()
        {
            Projectile.width = 32;
            Projectile.height = 32;
            Projectile.tileCollide = false;
            Projectile.hostile = true;
            Projectile.friendly = false;
            Projectile.penetrate = -1;
            Projectile.aiStyle = -1;
            Projectile.timeLeft = 200;
        }

        public ref float Time => ref Projectile.ai[0];

        public ref float TargetRot => ref Projectile.ai[1];

        private int waitTime = 40;

        private float speed = 0;

        public override void AI()
        {
            if (Time < 1)
                speed = Projectile.velocity.Length();
            if (Time > waitTime)
                Projectile.velocity = new Vector2(1 + Utils.GetLerpValue(waitTime - 5, waitTime + 15, Time, true) * speed, 0).RotatedBy(TargetRot) * Utils.GetLerpValue(waitTime + 10, waitTime + 70, Time, true) * 1.01f;
            else
                Projectile.velocity = new Vector2(1 + Utils.GetLerpValue(waitTime - 5, waitTime + 15, Time, true) * speed, 0).RotatedBy(TargetRot) * Utils.GetLerpValue(waitTime + 10, waitTime + 70, Time, true);
            Projectile.rotation = Projectile.velocity.ToRotation() + MathHelper.PiOver2;
            Projectile.scale = (float)Math.Sqrt(Utils.GetLerpValue(0, speed, Time, true));

            int target = -1;
            foreach (Player player in Main.player.Where(n => n.active && !n.dead && n.Distance(Projectile.Center) < 1500))
                target = player.whoAmI;

            if (target > -1 && Time < waitTime + 10)
                TargetRot = TargetRot.AngleLerp(Projectile.AngleTo(Main.player[target].Center), 0.006f);

            if (Main.rand.NextBool(10))
            {
                int goreID = Utils.SelectRandom(Main.rand, 16, 17, 16, 17);
                Gore.NewGore(Projectile.GetSource_FromAI(), Projectile.Center, Projectile.velocity * 0.2f, goreID);
            }

            if (Main.rand.NextBool(5))
                Dust.NewDustDirect(Projectile.position, Projectile.width, Projectile.height, DustID.YellowStarDust);

            Time++;

            if (Projectile.timeLeft < 20)
                Dust.NewDustPerfect(Projectile.Center + Main.rand.NextVector2Circular(10, 10), DustID.YellowStarDust, Main.rand.NextVector2Circular(4, 4));
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Asset<Texture2D> star = ModContent.Request<Texture2D>(Texture);
            Asset<Texture2D> tell = TextureAssets.Extra[178];
            Asset<Texture2D> trail = TextureAssets.Extra[91];

            Projectile.localAI[0] += Projectile.direction * 0.1f;

            Main.EntitySpriteDraw(trail.Value, Projectile.Center - Main.screenPosition, null, new Color(10, 0, 10, 100), Projectile.rotation, trail.Size() * new Vector2(0.5f, 0.18f), Projectile.scale * 1.5f, 0, 0);
            Main.EntitySpriteDraw(trail.Value, Projectile.Center - Main.screenPosition, null, new Color(17, 0, 70, 0), Projectile.rotation, trail.Size() * new Vector2(0.5f, 0.1f), Projectile.scale * new Vector2(1.2f - Projectile.velocity.Length() * 0.01f, Projectile.velocity.Length() * 0.1f), 0, 0);

            for (int i = 0; i < 3; i++)
            {
                Vector2 offset = new Vector2(16, 0).RotatedBy(MathHelper.TwoPi / 3f * i + Projectile.localAI[0] * 0.5f);
                Main.EntitySpriteDraw(trail.Value, Projectile.Center + offset - Main.screenPosition, null, new Color(30, 0, 170, 0) * 0.3f, Projectile.rotation, trail.Size() * new Vector2(0.5f, 0.18f), Projectile.scale * new Vector2(1.5f, 1.8f), 0, 0);
            }

            Main.EntitySpriteDraw(star.Value, Projectile.Center - Main.screenPosition, null, Color.LightGoldenrodYellow, Projectile.localAI[0], star.Size() * new Vector2(0.5f, 0.57f), Projectile.scale * 1.4f, 0, 0);
            Main.EntitySpriteDraw(star.Value, Projectile.Center - Main.screenPosition, null, new Color(255, 10, 30, 0), Projectile.localAI[0], star.Size() * new Vector2(0.5f, 0.57f), Projectile.scale * 1.8f, 0, 0);

            Main.EntitySpriteDraw(tell.Value, Projectile.Center - Main.screenPosition, null, new Color(255, 10, 30, 0) * Utils.GetLerpValue(waitTime + 20, waitTime, Time, true), TargetRot, new Vector2(0f, 1f), new Vector2(Utils.GetLerpValue(waitTime + 40, 0, Time, true) * (float)Math.Pow(speed, 1.1f) * 0.1f, Utils.GetLerpValue(waitTime + 40, waitTime - 20, Time, true) * 5f), 0, 0);


            return false;
        }
    }
}
