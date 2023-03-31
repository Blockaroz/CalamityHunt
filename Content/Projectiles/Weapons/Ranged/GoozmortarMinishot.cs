using CalamityHunt.Content.Bosses.Goozma;
using System;
using Terraria.ID;
using Terraria;
using Terraria.ModLoader;
using Microsoft.Xna.Framework;
using ReLogic.Content;
using Microsoft.Xna.Framework.Graphics;
using Terraria.Audio;
using Terraria.DataStructures;
using System.Linq;

namespace CalamityHunt.Content.Projectiles.Weapons.Ranged
{
    public class GoozmortarMinishot : ModProjectile
    {
        public override void SetDefaults()
        {
            Projectile.width = 24;
            Projectile.height = 24;
            Projectile.tileCollide = true;
            Projectile.hostile = false;
            Projectile.friendly = true;
            Projectile.aiStyle = -1;
            Projectile.timeLeft = 240;
            Projectile.usesIDStaticNPCImmunity = true;
            Projectile.idStaticNPCHitCooldown = 5;
        }

        public ref float Time => ref Projectile.ai[0];

        public override void OnSpawn(IEntitySource source)
        {
            Projectile.localAI[0] = Main.rand.NextFloat(0.9f, 1.15f);
            Projectile.localAI[1] = Main.rand.NextFloat(30f);
            Projectile.rotation = Main.rand.NextFloat(-1f, 1f);
        }

        public override void AI()
        {
            Projectile.scale = (float)Math.Sqrt(Utils.GetLerpValue(-3, 12, Time, true)) * Projectile.localAI[0];
            if (Time > 25)
            {
                Projectile.velocity.Y += 0.4f;
                int target = Projectile.FindTargetWithLineOfSight(400);
                if (target > -1)
                {
                    Projectile.velocity += Projectile.DirectionTo(Main.npc[target].Center).SafeNormalize(Vector2.Zero) * 0.3f;
                    Projectile.velocity = Vector2.Lerp(Projectile.velocity, Projectile.DirectionTo(Main.npc[target].Center).SafeNormalize(Vector2.Zero) * 20, 0.03f);
                }

            }

            if (Main.rand.NextBool(15))
            {
                Color glowColor = new GradientColor(SlimeUtils.GoozColorArray, 0.4f, 0.5f).ValueAt(Projectile.localAI[1]);
                glowColor.A /= 2;
                Dust.NewDustPerfect(Projectile.Center + Main.rand.NextVector2Circular(20, 20), DustID.FireworksRGB, Main.rand.NextVector2Circular(5, 5), 0, glowColor, 1.2f).noGravity = true;
            }

            bool hittingLobs = Main.projectile.Any(n => n.active && n.ai[1] >= 0 && n.type == ModContent.ProjectileType<GoozmortarLob>() && n.Distance(Projectile.Center) < 20);
            if (hittingLobs)
            {
                Projectile lob = Main.projectile.First(n => n.active && n.ai[1] >= 0 && n.type == ModContent.ProjectileType<GoozmortarLob>() && n.Distance(Projectile.Center) < 20);
                lob.ai[1] = -1;
                Projectile.Kill();
            }

            Time++;
        }

        public override void Kill(int timeLeft)
        {
            for (int i = 0; i < 15; i++)
            {
                Color glowColor = new GradientColor(SlimeUtils.GoozColorArray, 0.4f, 0.5f).ValueAt(Projectile.localAI[1]);
                glowColor.A /= 2;
                Dust.NewDustPerfect(Projectile.Center, DustID.RainbowMk2, Projectile.velocity * 0.1f + Main.rand.NextVector2Circular(4, 4), 0, glowColor, 1.5f).noGravity = true;
            }
            SoundEngine.PlaySound(SoundID.LiquidsHoneyLava, Projectile.Center);
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Asset<Texture2D> texture = ModContent.Request<Texture2D>(Texture);
            Main.EntitySpriteDraw(texture.Value, Projectile.Center - Main.screenPosition, null, Color.White, Projectile.rotation, texture.Size() * 0.5f, Projectile.scale, 0, 0);
            return false;
        }
    }
}
