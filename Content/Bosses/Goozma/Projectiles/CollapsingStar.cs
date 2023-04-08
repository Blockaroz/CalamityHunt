using CalamityHunt.Content.Bosses.Goozma.Slimes;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using System;
using System.Linq;
using Terraria;
using Terraria.DataStructures;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityHunt.Content.Bosses.Goozma.Projectiles
{
    public class CollapsingStar : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Slime Shot"); 
            ProjectileID.Sets.DrawScreenCheckFluff[Type] = 5000;

        }

        public override void SetDefaults()
        {
            Projectile.width = 56;
            Projectile.height = 56;
            Projectile.tileCollide = false;
            Projectile.hostile = true;
            Projectile.friendly = false;
            Projectile.penetrate = -1;
            Projectile.aiStyle = -1;
        }

        public ref float Time => ref Projectile.ai[0];
        public ref NPC Host => ref Main.npc[(int)Projectile.ai[1]];

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

            if (Time < 40)
                Projectile.velocity = Vector2.Lerp(Projectile.velocity, new Vector2(2, 0).RotatedBy(Projectile.AngleTo(Main.npc[owner].Center)), 0.1f);
            else
                Projectile.velocity = Vector2.Lerp(Projectile.velocity, new Vector2(25f, 0).RotatedBy(Projectile.AngleTo(Main.npc[owner].Center)), 0.3f);

            if (Projectile.Distance(Main.npc[owner].Center) < 50)
                Projectile.Kill();

            //if (Time == 40)
            //    for (int i = 0; i < Main.rand.Next(10, 30); i++)
            //        Dust.NewDustPerfect(Projectile.Center + Main.rand.NextVector2Circular(45, 45), DustID.FireworksRGB, Main.rand.NextVector2Circular(10, 10), 0, new Color(28, 15, 90, 120), 2f).noGravity = true;

            //if (Time > 40 && Main.rand.NextBool(5))
            //    Dust.NewDustPerfect(Projectile.Center + Main.rand.NextVector2Circular(45, 45), DustID.FireworksRGB, Projectile.velocity, 0, new Color(28, 15, 90, 120), 2f).noGravity = true;

            Projectile.frameCounter++;
            if (Projectile.frameCounter > 4)
            {
                Projectile.frameCounter = 0;
                Projectile.frame = 0;// (Projectile.frame + 1) % 0;
            }

            Time++;
        }

        public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
        {
            if (Time > 40)
                return base.Colliding(projHitbox, targetHitbox);
            return false;
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Asset<Texture2D> star = ModContent.Request<Texture2D>(Texture);
            Rectangle frame = star.Frame(1, 1, 0, Projectile.frame);
            Main.EntitySpriteDraw(star.Value, Projectile.Center - Main.screenPosition, frame, Color.White, Projectile.rotation, frame.Size() * 0.5f, Projectile.scale * 3f, 0, 0);
            return false;
        }
    }
}
