using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using System;
using System.Collections.Generic;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityHunt.Content.Bosses.Goozma.Projectiles
{
    public class GelCrystalShard : ModProjectile
    {
        public override void SetDefaults()
        {
            Projectile.width = 36;
            Projectile.height = 36;
            Projectile.tileCollide = false;
            Projectile.hostile = true;
            Projectile.friendly = false;
            Projectile.penetrate = -1;
            Projectile.aiStyle = -1;
            Projectile.timeLeft = 400;
            Projectile.extraUpdates = 1;
        }

        public override void OnSpawn(IEntitySource source)
        {
            Projectile.frame = Main.rand.Next(4);
        }

        public override void AI()
        {
            Projectile.rotation = Projectile.velocity.ToRotation() + MathHelper.PiOver2;
            if (Projectile.ai[0] == 0)
                Projectile.velocity.Y += 0.12f;

            if (Main.rand.NextBool(2))
            {
                int dustType = Utils.SelectRandom(Main.rand, DustID.PinkCrystalShard, DustID.BlueCrystalShard, DustID.PurpleCrystalShard);
                Dust dust = Dust.NewDustDirect(Projectile.Center - new Vector2(15), 30, 30, dustType, Projectile.velocity.X * 0.2f, Projectile.velocity.Y * 0.2f, 100, Color.White, Main.rand.NextFloat());
                dust.noGravity = true;
            }

            Lighting.AddLight(Projectile.Center, Color.Lerp(Color.DarkBlue, Color.HotPink, (float)Math.Sqrt(Math.Sin(Projectile.timeLeft * 0.1f))).ToVector3() * 0.2f);

            if (Projectile.timeLeft < 20)
                Projectile.velocity *= 0.9f;
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Asset<Texture2D> texture = ModContent.Request<Texture2D>(Texture);
            Asset<Texture2D> glow = ModContent.Request<Texture2D>($"{nameof(CalamityHunt)}/Assets/Textures/Goozma/GlowSoft");
            Rectangle frame = texture.Frame(4, 1, Projectile.frame, 0);

            Color darkBack = Color.BlueViolet * 0.15f;
            darkBack.A /= 2;
            Color bloom = Color.MediumVioletRed * 0.25f;
            bloom.A = 0;

            Main.EntitySpriteDraw(texture.Value, Projectile.Center - Main.screenPosition, frame, Color.Black * 0.5f, Projectile.rotation, frame.Size() * 0.5f, Projectile.scale * 1.4f, 0, 0);
            Main.EntitySpriteDraw(texture.Value, Projectile.Center - Main.screenPosition, frame, darkBack, Projectile.rotation, frame.Size() * 0.5f, Projectile.scale * 1.66f, 0, 0);
            Main.EntitySpriteDraw(texture.Value, Projectile.Center - Main.screenPosition, frame, Color.White, Projectile.rotation, frame.Size() * 0.5f, Projectile.scale, 0, 0);
            Main.EntitySpriteDraw(glow.Value, Projectile.Center - Main.screenPosition, null, bloom, Projectile.rotation, glow.Size() * 0.5f, Projectile.scale * new Vector2(1.5f, 2f), 0, 0);

            return false;
        }
    }
}
