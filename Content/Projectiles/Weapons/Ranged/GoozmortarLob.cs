using CalamityHunt.Content.Bosses.Goozma;
using System;
using Terraria.ID;
using Terraria;
using Terraria.ModLoader;
using Microsoft.Xna.Framework;
using ReLogic.Content;
using Microsoft.Xna.Framework.Graphics;
using Terraria.Graphics.CameraModifiers;

namespace CalamityHunt.Content.Projectiles.Weapons.Ranged
{
    public class GoozmortarLob : ModProjectile
    {
        public override void SetDefaults()
        {
            Projectile.width = 24;
            Projectile.height = 24;
            Projectile.tileCollide = true;
            Projectile.hostile = false;
            Projectile.friendly = true;
            Projectile.penetrate = -1;
            Projectile.aiStyle = -1;
            Projectile.timeLeft = 1200;
            Projectile.usesIDStaticNPCImmunity = true;
            Projectile.idStaticNPCHitCooldown = 5;
        }

        public ref float Time => ref Projectile.ai[0];

        public override void AI()
        {
            if (Time > 10 && Projectile.ai[1] >= 0)
                Projectile.velocity.Y += 0.11f;

            if (Main.rand.NextBool(8))
            {
                Color glowColor = new GradientColor(SlimeUtils.GoozColorArray, 0.4f, 0.5f).ValueAt(Projectile.localAI[1]);
                glowColor.A /= 2;
                Dust.NewDustPerfect(Projectile.Center + Main.rand.NextVector2Circular(20, 20), DustID.FireworksRGB, Main.rand.NextVector2Circular(5, 5), 0, glowColor, 1.2f).noGravity = true;
            }

            if (Projectile.ai[1] < 0)
            {
                Projectile.velocity *= 0.77f;
                Projectile.ai[1]--;

                Projectile.Resize(280, 280);
                if (Projectile.ai[1] < -2 && Projectile.ai[1] > -20)
                {
                    Main.instance.CameraModifiers.Add(new PunchCameraModifier(Projectile.Center, Main.rand.NextVector2CircularEdge(1, 1), 8f * Utils.GetLerpValue(-20, -2, Time, true), 11, 20));

                    for (int i = 0; i < 30; i++)
                        Dust.NewDustPerfect(Projectile.Center, DustID.Smoke, Main.rand.NextVector2Circular(10, 10), 0, Color.Black, 1f);
                }

                if (Projectile.ai[1] < -30)
                    Projectile.Kill();
            }

            Time++;
        }

        public override bool OnTileCollide(Vector2 oldVelocity)
        {
            if (Projectile.ai[1] >= 0)
                Projectile.ai[1] = -1;

            return false;
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            if (Projectile.ai[1] >= 0)
                Projectile.ai[1] = -1;
        }

        public override bool PreDraw(ref Color lightColor)
        {
            if (Projectile.ai[1] > -5)
            {
                Asset<Texture2D> texture = ModContent.Request<Texture2D>(Texture);
                Main.EntitySpriteDraw(texture.Value, Projectile.Center - Main.screenPosition, null, Color.White, Projectile.rotation, texture.Size() * 0.5f, Projectile.scale, 0, 0);
            }
            
            if (Projectile.ai[1] < 0)
            {

            }
            return false;
        }
    }
}
