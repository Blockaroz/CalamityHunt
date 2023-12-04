using System.Collections.Generic;
using CalamityHunt.Content.Items.Misc;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using Terraria;
using Terraria.ModLoader;

namespace CalamityHunt.Content.Projectiles
{
    public class ChromaticBombProjectile : ModProjectile
    {
        public override string Texture => $"{nameof(CalamityHunt)}/Content/Items/Misc/ChromaticBomb";

        public override void SetDefaults()
        {
            Projectile.width = 80;
            Projectile.height = 80;
            Projectile.friendly = true;
            Projectile.tileCollide = false;
            Projectile.penetrate = -1;
            Projectile.timeLeft = 300;

            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 10;
        }

        public override void AI()
        {
            // explosives don't scale with any damage
            Projectile.damage = ChromaticBomb.Damage;
            Projectile.knockBack = ChromaticBomb.Knockback;

            Projectile.rotation += 0.2f;

            if (Projectile.owner == Main.myPlayer) {
                DynamoRodProjectile.ExplodeandDestroyTiles(Projectile, 6, false, new List<int>() { }, new List<int>() { });
            }
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Asset<Texture2D> shadowTexture = ModContent.Request<Texture2D>($"{nameof(CalamityHunt)}/Content/Bosses/Goozma/Projectiles/BlackHoleBlenderShadow");

            Main.EntitySpriteDraw(shadowTexture.Value, Projectile.Center - Main.screenPosition, shadowTexture.Frame(), Color.Black * 0.5f, -Projectile.rotation * 0.7f, shadowTexture.Size() * 0.5f, Projectile.scale * 0.3f, SpriteEffects.FlipHorizontally, 0);
            Main.EntitySpriteDraw(shadowTexture.Value, Projectile.Center - Main.screenPosition, shadowTexture.Frame(), Color.Black * 0.2f, -Projectile.rotation * 0.5f, shadowTexture.Size() * 0.5f, Projectile.scale * 0.5f, SpriteEffects.FlipHorizontally, 0);
            Main.EntitySpriteDraw(shadowTexture.Value, Projectile.Center - Main.screenPosition, shadowTexture.Frame(), Color.Black, Projectile.rotation * 0.33f, shadowTexture.Size() * 0.5f, Projectile.scale * 0.2f, 0, 0);

            return false;
        }
    }
}
