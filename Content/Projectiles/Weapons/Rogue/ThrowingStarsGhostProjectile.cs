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

namespace CalamityHunt.Content.Projectiles.Weapons.Rogue
{
    public class ThrowingStarsGhostProjectile : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.TrailCacheLength[Type] = 10;
            ProjectileID.Sets.TrailingMode[Type] = 2;
        }

        public override void SetDefaults()
        {
            Projectile.width = 20;
            Projectile.height = 20;
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
            Projectile.localNPCHitCooldown = 1;
        }

        public override void AI()
        {
            Color randomColor = Color.Lerp(Color.RoyalBlue, Color.Gold, Main.rand.NextFloat());
            randomColor.A /= 2;
            Dust d = Dust.NewDustPerfect(Projectile.Center - Projectile.velocity * Main.rand.NextFloat(), DustID.ShimmerSpark, Projectile.velocity * 0.5f, 0, randomColor, 0.7f);
            d.noGravity = true;

            Projectile.ai[1]++;
            if (Projectile.ai[0] > -1 && CanDamage().Value)
            {
                NPC target = Main.npc[(int)Projectile.ai[0]];
                if (target.CanBeChasedBy(this) && target.active)
                {
                    Projectile.extraUpdates = 2;
                    Projectile.velocity = Vector2.Lerp(Projectile.velocity, Projectile.DirectionTo(target.Center).SafeNormalize(Vector2.Zero) * 24, 0.1f);
                }
            }
            else
            {
                Projectile.extraUpdates = 1;
                Projectile.FindTargetWithLineOfSight(400);
                Projectile.velocity *= 0.99f;
            }

            Projectile.rotation = Projectile.velocity.ToRotation();
        }

        public override bool? CanDamage() => Projectile.ai[1] > 20;

        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D texture = TextureAssets.Projectile[Type].Value;
            Texture2D sparkle = AssetDirectory.Textures.Sparkle;
            Vector2 direction = Projectile.rotation.ToRotationVector2() * 10;

            for (int i = 0; i < ProjectileID.Sets.TrailCacheLength[Type]; i++)
            {
                float p = 1f - (float)i / ProjectileID.Sets.TrailCacheLength[Type];
                Color drawColor = Color.Lerp(new Color(0, 10, 190, 0), new Color(0, 180, 255, 0), p);
                Main.EntitySpriteDraw(sparkle, Projectile.oldPos[i] + Projectile.Size * 0.5f - Main.screenPosition, sparkle.Frame(), drawColor, Projectile.oldRot[i] + MathHelper.PiOver2, sparkle.Size() * 0.5f, Projectile.scale * new Vector2(0.7f * p, 0.7f), 0, 0);
            }
            Main.EntitySpriteDraw(texture, Projectile.Center + direction - Main.screenPosition, texture.Frame(), Color.White, Projectile.rotation, texture.Size() * new Vector2(1f, 0.5f), Projectile.scale, 0, 0);
            Main.EntitySpriteDraw(sparkle, Projectile.Center - Main.screenPosition, sparkle.Frame(), new Color(0, 10, 60, 0), Projectile.rotation + MathHelper.PiOver2, sparkle.Size() * 0.5f, Projectile.scale * new Vector2(1.5f, 0.6f), 0, 0);

            return false;
        }
    }
}
