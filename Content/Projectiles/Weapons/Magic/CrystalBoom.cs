using CalamityHunt.Content.Items.Rarities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria.ID;
using Terraria;
using Terraria.ModLoader;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using CalamityHunt.Content.Items.Weapons.Magic;
using CalamityHunt.Common.Systems.Particles;
using CalamityHunt.Content.Particles;

namespace CalamityHunt.Content.Projectiles.Weapons.Magic
{
    public class CrystalBoom : ModProjectile
    {
        public override void SetDefaults()
        {
            Projectile.width = 24;
            Projectile.height = 24;
            Projectile.friendly = true;
            Projectile.timeLeft = 180;
            Projectile.penetrate = -1;
            Projectile.tileCollide = false;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 20;
            Projectile.DamageType = DamageClass.Magic;
            Projectile.extraUpdates = 5;
        }

        public ref float Time => ref Projectile.ai[0];

        public override void AI()
        {
            Projectile.velocity = Vector2.Zero;

            Time++;
            Projectile.localAI[0]++;
            Projectile.direction = Projectile.velocity.X > 0 ? 1 : -1;
            Projectile.rotation += (1f - Time / 40f);
        }

        public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
        {

            return false;
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D texture = ModContent.Request<Texture2D>(Texture).Value;
            Texture2D glow = ModContent.Request<Texture2D>($"{nameof(CalamityHunt)}/Assets/Textures/Goozma/GlowSoft").Value;
            float scale = MathF.Sqrt(Projectile.scale) * (1f + MathF.Sin(Time * 0.9f) * 0.08f) * 1.5f * Utils.GetLerpValue(0, 20, Time, true);

            Color rainbowColor = Main.hslToRgb(Projectile.localAI[0] * 0.03f % 1f, 0.5f, 0.55f, 0);


            Main.EntitySpriteDraw(texture, Projectile.Center - Main.screenPosition, texture.Frame(), rainbowColor, Projectile.rotation * 0.6f, texture.Size() * 0.5f, scale * new Vector2(1f, 0.8f), 0, 0);
            Main.EntitySpriteDraw(texture, Projectile.Center - Main.screenPosition, texture.Frame(), rainbowColor, Projectile.rotation, texture.Size() * 0.5f, scale * new Vector2(1f, 0.8f), 0, 0);
            Main.EntitySpriteDraw(texture, Projectile.Center - Main.screenPosition, texture.Frame(), rainbowColor, Projectile.rotation * -1.3f, texture.Size() * 0.5f, scale * new Vector2(1f, 0.8f) * 1.05f, 0, 0);

            return false;
        }
    }
}
