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
            Projectile.timeLeft = 90;
        }

        public ref float Time => ref Projectile.ai[0];
        public ref NPC Host => ref Main.npc[(int)Projectile.ai[1]];

        public override void AI()
        {
            if (!Main.npc.Any(n => n.type == ModContent.NPCType<StellarGeliath>() && n.active))
                Projectile.active = false;
            else
                Projectile.ai[1] = Main.npc.First(n => n.type == ModContent.NPCType<StellarGeliath>() && n.active).whoAmI;

            Projectile.velocity = Vector2.Lerp(Projectile.velocity * 0.96f, Projectile.DirectionTo(Host.Center).SafeNormalize(Vector2.Zero) * Projectile.Distance(Host.Center) * 0.2f, (float)Math.Pow(Utils.GetLerpValue(30, 100, Time, true), 5f) * 0.7f);
            Projectile.rotation = Projectile.velocity.ToRotation() - MathHelper.PiOver2;
            Projectile.scale = Utils.GetLerpValue(35, 43, Time, true) * Utils.GetLerpValue(80, 70, Time, true);

            if (Time > 40 && Main.rand.NextBool(5))
                Dust.NewDustPerfect(Projectile.Center + Main.rand.NextVector2Circular(45, 45), DustID.RainbowMk2, Projectile.velocity, 0, new Color(80, 30, 255, 120), 2f).noGravity = true;

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
            Asset<Texture2D> flash = TextureAssets.Extra[98];
            Asset<Texture2D> trail = TextureAssets.Extra[91];
            Asset<Texture2D> tell = TextureAssets.Extra[178];

            Projectile.localAI[0] += Projectile.direction * 0.2f;

            Main.EntitySpriteDraw(trail.Value, Projectile.Center - Main.screenPosition, null, new Color(17, 0, 70, 0), Projectile.velocity.ToRotation() + MathHelper.PiOver2, trail.Size() * new Vector2(0.5f, 0.13f), Projectile.scale * new Vector2(1.2f - Projectile.velocity.Length() * 0.01f, 0.2f + Projectile.velocity.Length() * 0.05f), 0, 0);

            Main.EntitySpriteDraw(flash.Value, Projectile.Center - Main.screenPosition, null, new Color(10, 0, 10, 100), 0, flash.Size() * 0.5f, Projectile.scale * 2.5f * new Vector2(0.5f, 1f), 0, 0);
            Main.EntitySpriteDraw(flash.Value, Projectile.Center - Main.screenPosition, null, new Color(10, 0, 10, 100), MathHelper.PiOver2, flash.Size() * 0.5f, Projectile.scale * 2.5f * new Vector2(0.5f, 1f), 0, 0);
            
            Main.EntitySpriteDraw(flash.Value, Projectile.Center - Main.screenPosition, null, new Color(55, 0, 170, 0), 0, flash.Size() * 0.5f, Projectile.scale * 3.5f * new Vector2(0.5f, 1f), 0, 0);
            Main.EntitySpriteDraw(flash.Value, Projectile.Center - Main.screenPosition, null, new Color(55, 0, 170, 0), MathHelper.PiOver2, flash.Size() * 0.5f, Projectile.scale * 3.5f * new Vector2(0.5f, 1f), 0, 0);

            Main.EntitySpriteDraw(flash.Value, Projectile.Center - Main.screenPosition, null, new Color(255, 10, 30, 60), 0, flash.Size() * 0.5f, Projectile.scale * new Vector2(1.8f, 2.1f), 0, 0);
            Main.EntitySpriteDraw(flash.Value, Projectile.Center - Main.screenPosition, null, new Color(255, 10, 30, 60), MathHelper.PiOver2, flash.Size() * 0.5f, Projectile.scale * new Vector2(1.8f, 2.1f), 0, 0);
            
            Main.EntitySpriteDraw(star.Value, Projectile.Center - Main.screenPosition, null, Color.Gold, Projectile.localAI[0], star.Size() * new Vector2(0.5f, 0.57f), Projectile.scale * 0.7f, 0, 0);
            Main.EntitySpriteDraw(star.Value, Projectile.Center - Main.screenPosition, null, new Color(255, 10, 30, 0), Projectile.localAI[0], star.Size() * new Vector2(0.5f, 0.57f), Projectile.scale, 0, 0);

            Main.EntitySpriteDraw(flash.Value, Projectile.Center - Main.screenPosition, null, new Color(255, 180, 170, 0), 0, flash.Size() * 0.5f, Projectile.scale * 1.2f * new Vector2(0.6f, 1f), 0, 0);
            Main.EntitySpriteDraw(flash.Value, Projectile.Center - Main.screenPosition, null, new Color(255, 180, 170, 0), MathHelper.PiOver2, flash.Size() * 0.5f, Projectile.scale * 1.2f * new Vector2(0.6f, 1f), 0, 0);

            if (Time < 35)
            {
                Main.EntitySpriteDraw(tell.Value, Projectile.Center - Main.screenPosition, null, new Color(255, 10, 30, 20) * Utils.GetLerpValue(80, 90, Projectile.timeLeft, true), Projectile.AngleTo(Host.Center), tell.Size() * new Vector2(0f, 0.5f), new Vector2(2f, 3f), 0, 0);

                Vector2 flashScale = Utils.GetLerpValue(90, 88, Projectile.timeLeft, true) * Utils.GetLerpValue(60, 90, Projectile.timeLeft, true) * 10f * new Vector2(0.1f, 1f);
                Vector2 origin = flash.Size() * new Vector2(0.5f, 0.4f + Utils.GetLerpValue(32, 0, Time, true) * 0.2f);
                Main.EntitySpriteDraw(flash.Value, Projectile.Center - Main.screenPosition, null, new Color(30, 0, 150, 0), Projectile.rotation, origin, flashScale * 2f, 0, 0);
            }

            return false;
        }
    }
}
