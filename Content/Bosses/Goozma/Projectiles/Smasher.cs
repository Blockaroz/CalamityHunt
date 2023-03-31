using CalamityHunt.Common.Systems.Particles;
using CalamityHunt.Content.Particles;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityHunt.Content.Bosses.Goozma.Projectiles
{
    public class Smasher : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Smasher");
            ProjectileID.Sets.DrawScreenCheckFluff[Type] = 10000;
        }

        public override void SetDefaults()
        {
            Projectile.width = 180;
            Projectile.height = 120;
            Projectile.tileCollide = false;
            Projectile.hostile = true;
            Projectile.friendly = false;
            Projectile.penetrate = -1;
            Projectile.aiStyle = -1;
            Projectile.timeLeft = 700;
        }

        public ref float Time => ref Projectile.ai[0];

        public ref Player Target => ref Main.player[(int)Projectile.ai[1]];

        private Vector2 saveTarget;

        public override void AI()
        {
            if (Projectile.ai[1] < 0)
            {
                foreach (Player player in Main.player.Where(n => n.active && !n.dead))
                    Projectile.ai[1] = player.whoAmI;
            }

            if (Time < 1)
                saveTarget = new Vector2(Projectile.Center.X, Projectile.FindSmashSpot(Target.Center).Y);

            if (Time < 1)
            {
                Projectile.velocity = Vector2.Zero;
                Projectile.Center = saveTarget - new Vector2(0, 900);
            }

            if (Time > 0)
                Projectile.Center = Vector2.Lerp(Projectile.Center, saveTarget, 0.001f + Utils.GetLerpValue(0, 10, Time, true));
            
            Color glowColor = GetAlpha(Color.White).Value * 0.5f;
            glowColor.A = 0;

            if (Time == 8 && !Main.dedServ)
                SoundEngine.PlaySound(SoundID.Item167.WithPitchOffset(0.2f), Projectile.Center);

            if (Time > 20)
                Projectile.Kill();

            Time++;
        }

        public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
        {
            if (Time > 2 && Time < 12)
                return new Rectangle(projHitbox.X, projHitbox.Y - 500, projHitbox.Width, projHitbox.Height + 500).Intersects(targetHitbox);
            return false;
        }

        public override Color? GetAlpha(Color lightColor)
        {
            Color final = Projectile.localAI[0] switch
            {
                1 => new Color(255, 20, 20, 128),
                _ => Color.White
            };
            return final;
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Asset<Texture2D> texture = ModContent.Request<Texture2D>(Texture);
            Asset<Texture2D> trail = ModContent.Request<Texture2D>($"{nameof(CalamityHunt)}/Assets/Textures/Goozma/SmashTrail");
            Asset<Texture2D> tell = TextureAssets.Extra[178];

            Color glowColor = GetAlpha(Color.White).Value * 0.5f;
            glowColor.A = 0;
            if (Time < 0)
            {
                float width = (float)Math.Pow(Utils.GetLerpValue(0, -70, Time, true), 0.75f) * Projectile.width * 1.5f;
                Color tellColor = glowColor * (float)Math.Pow(Utils.GetLerpValue(-70, 0, Time, true), 2f) * Utils.GetLerpValue(100, 0, Time, true) * 0.6f;
                Main.EntitySpriteDraw(tell.Value, new Vector2(Projectile.Center.X - Main.screenPosition.X, 0), null, tellColor, Projectile.rotation + MathHelper.PiOver2, tell.Size() * new Vector2(0f, 0.5f), new Vector2(1.5f, width), 0, 0);
                return false;

            }

            Vector2 squish = new Vector2(0.5f, 1.5f);

            if (Time > 9)
                squish = new Vector2(1f + Utils.GetLerpValue(18, 10, Time, true) * 0.7f, 1f - Utils.GetLerpValue(18, 10, Time, true) * 0.7f);
            float fadeOut = Utils.GetLerpValue(20, 15, Time, true) * Utils.GetLerpValue(-1, 5, Time, true);
            float trailOut = fadeOut * Utils.GetLerpValue(14, 9, Time, true) * 0.9f;
            Main.EntitySpriteDraw(trail.Value, Projectile.Bottom + new Vector2(0, -20) - Main.screenPosition, null, glowColor * trailOut * 0.8f, Projectile.rotation, trail.Size() * new Vector2(0.5f, 1f), Projectile.scale * new Vector2(squish.X * 0.9f, 1f), 0, 0);
            //Main.EntitySpriteDraw(trail.Value, Projectile.Bottom + new Vector2(0, -20) - Main.screenPosition, null, glowColor * trailOut, Projectile.rotation, trail.Size() * new Vector2(0.5f, 1f), Projectile.scale * new Vector2(squish.X * 0.6f, 1f), 0, 0);
            Main.EntitySpriteDraw(texture.Value, Projectile.Bottom - Main.screenPosition, null, GetAlpha(Color.White).Value * 0.5f, Projectile.rotation, texture.Size() * new Vector2(0.5f, 1f), Projectile.scale * 1.1f * squish * (0.5f + fadeOut * 0.5f), 0, 0);
            Main.EntitySpriteDraw(texture.Value, Projectile.Bottom - Main.screenPosition, null, lightColor.MultiplyRGBA(GetAlpha(Color.White).Value) * 0.8f, Projectile.rotation, texture.Size() * new Vector2(0.5f, 1f), Projectile.scale * squish * (0.5f + fadeOut * 0.5f), 0, 0);

            return false;
        }
    }
}
