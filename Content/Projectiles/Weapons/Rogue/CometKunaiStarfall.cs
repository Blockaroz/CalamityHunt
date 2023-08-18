using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria.GameContent;
using Humanizer;
using Terraria.Audio;

namespace CalamityHunt.Content.Projectiles.Weapons.Rogue
{
    public class CometKunaiStarfall : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.TrailCacheLength[Type] = 20;
            ProjectileID.Sets.TrailingMode[Type] = 2;
        }

        public override void SetDefaults()
        {
            Projectile.width = 30;
            Projectile.height = 30;
            Projectile.friendly = true;
            Projectile.tileCollide = false;
            Projectile.DamageType = DamageClass.Throwing;
            if (ModLoader.HasMod("CalamityMod"))
            {
                DamageClass d;
                Mod calamity = ModLoader.GetMod("CalamityMod");
                calamity.TryFind<DamageClass>("RogueDamageClass", out d);
                Projectile.DamageType = d;
            }
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 60;
            Projectile.penetrate = -1;
            Projectile.timeLeft = 80;
            Projectile.extraUpdates = 4;
        }

        public override void AI()
        {
            Projectile.rotation = Projectile.velocity.ToRotation();
            float p = Utils.GetLerpValue(0, 50, Projectile.timeLeft, true) * Utils.GetLerpValue(80, 30, Projectile.timeLeft, true);
            Color randomColor = Color.Lerp(Color.Blue, Color.RoyalBlue, Main.rand.NextFloat());
            randomColor.A = 0;
            Dust d = Dust.NewDustPerfect(Projectile.Center - Projectile.velocity * Main.rand.NextFloat(), DustID.SparkForLightDisc, Projectile.velocity.RotatedByRandom(0.1f) * Main.rand.NextFloat(), 0, randomColor, p * (1f - p) * 2.5f);
            d.noGravity = true;
        }

        public override void Kill(int timeLeft)
        {
        }

        public override bool? CanDamage() => true;

        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D sparkle = AssetDirectory.Textures.Sparkle;

            float p = Utils.GetLerpValue(10, 50, Projectile.timeLeft, true) * Utils.GetLerpValue(80, 30, Projectile.timeLeft, true);
            Vector2 scale = new Vector2(p * 0.8f, MathF.Pow(p + 0.3f, 2) * 3f) * Projectile.scale;
            Main.EntitySpriteDraw(sparkle, Projectile.Center - Main.screenPosition, sparkle.Frame(), new Color(10, 20, 180, 10), Projectile.rotation + MathHelper.PiOver2, sparkle.Size() * new Vector2(0.5f, 0.25f + p * 0.5f), scale * 2.5f, 0, 0);
            Main.EntitySpriteDraw(sparkle, Projectile.Center - Main.screenPosition, sparkle.Frame(), new Color(50, 180, 255, 0), Projectile.rotation + MathHelper.PiOver2, sparkle.Size() * new Vector2(0.5f, 0.25f + p * 0.5f), scale * new Vector2(1.5f, 1f), 0, 0);
            Main.EntitySpriteDraw(sparkle, Projectile.Center - Main.screenPosition, sparkle.Frame(), new Color(255, 255, 0, 20), Projectile.rotation + MathHelper.PiOver2, sparkle.Size() * new Vector2(0.5f, 0.25f + p * 0.5f), scale * new Vector2(0.6f, 0.6f), 0, 0);

            return false;
        }
    }
}
