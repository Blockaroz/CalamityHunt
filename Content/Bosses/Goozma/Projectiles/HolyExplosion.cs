using CalamityHunt.Common.Systems.Particles;
using CalamityHunt.Common.Systems;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using System;
using System.Collections.Generic;
using Terraria;
using Terraria.Audio;
using Terraria.Graphics.CameraModifiers;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityHunt.Content.Bosses.Goozma.Projectiles
{
    public class HolyExplosion : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.DrawScreenCheckFluff[Type] = 10000;
        }

        public override void SetDefaults()
        {
            Projectile.width = 16;
            Projectile.height = 16;
            Projectile.tileCollide = false;
            Projectile.hostile = false;
            Projectile.friendly = false;
            Projectile.penetrate = -1;
            Projectile.aiStyle = -1;
            Projectile.manualDirectionChange = true;
            Projectile.hide = true;
        }

        public ref float Time => ref Projectile.ai[0];
        
        public override void AI()
        {
            if (Time > 85)
                Projectile.Kill();

            if (Config.Instance.epilepsy)
            {
                float strength = Utils.GetLerpValue(0, 30, Time, true) * Utils.GetLerpValue(80, 40, Time, true);
                if (Time % 4 == 0)
                    Main.instance.CameraModifiers.Add(new PunchCameraModifier(Projectile.Center, Main.rand.NextVector2CircularEdge(1, 1), strength * 120, 20, 30));
            }

            if (Time == 0)
            {
                SoundStyle explosion = new SoundStyle($"{nameof(CalamityHunt)}/Assets/Sounds/Goozma/Slimes/PixieBallExplode");
                SoundEngine.PlaySound(explosion, Projectile.Center);
            }
            if (Time == 35)
            {
                SoundStyle ringing = new SoundStyle($"{nameof(CalamityHunt)}/Assets/Sounds/Goozma/GoozmaEarRinging");
                SoundEngine.PlaySound(ringing.WithVolumeScale(2f), Projectile.Center);
            }

            if (Time < 50)
                for (int i = 0; i < 20; i++)
                {
                    Vector2 vel = Main.rand.NextVector2Circular(100, 100);
                    float distanceScale = 2f / Math.Max(vel.Length(), 0.9f) + Main.rand.NextFloat();
                    Particle.NewParticle(Particle.ParticleType<Particles.PrettySparkle>(), Projectile.Center, vel, Main.hslToRgb(Time / 50f, 0.5f, 0.5f, 128), distanceScale + 1f);
                }

            Time++;

            Projectile.localAI[0]++;

            Projectile.rotation += (85 - Time) / 800f;
        }

        public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox) => false;

        public override void DrawBehind(int index, List<int> behindNPCsAndTiles, List<int> behindNPCs, List<int> behindProjectiles, List<int> overPlayers, List<int> overWiresUI) => overPlayers.Add(index);

        public override bool PreDraw(ref Color lightColor)
        {
            Asset<Texture2D> texture = ModContent.Request<Texture2D>(Texture);
            Asset<Texture2D> sparkle = ModContent.Request<Texture2D>(Texture + "Sparkle");

            Color bloomColor = Main.hslToRgb(Time / 80f % 1f, 0.5f, 0.5f, 0) * Utils.GetLerpValue(80, 50, Time, true);
            Color sparkleColor = Main.hslToRgb(Time / 80f % 1f, 0.5f, 0.7f, 0) * Utils.GetLerpValue(70, 60, Time, true);
            float time = Utils.GetLerpValue(0, 30, Time, true);

            Vector2 scaleConstant = new Vector2(0.1f, 0.2f);

            Main.EntitySpriteDraw(texture.Value, Projectile.Center - Main.screenPosition, null, bloomColor * 0.1f, Projectile.rotation, texture.Size() * 0.5f, (float)Math.Pow(time * 2f, 2f) * 12f, 0, 0);
            Main.EntitySpriteDraw(texture.Value, Projectile.Center - Main.screenPosition, null, bloomColor * 0.3f, Projectile.rotation, texture.Size() * 0.5f, (float)Math.Pow(time * 2f, 2f) * 4f, 0, 0);
            Main.EntitySpriteDraw(texture.Value, Projectile.Center - Main.screenPosition, null, bloomColor * 0.5f, Projectile.rotation, texture.Size() * 0.5f, (float)Math.Pow(time * 2f, 2f) * 2f, 0, 0);

            //initial
            Main.EntitySpriteDraw(sparkle.Value, Projectile.Center - Main.screenPosition, null, bloomColor, Projectile.rotation, sparkle.Size() * 0.5f, scaleConstant * new Vector2(0.2f, 1f) * (float)Math.Pow(time, 3f) * 80f, 0, 0);
            Main.EntitySpriteDraw(sparkle.Value, Projectile.Center - Main.screenPosition, null, bloomColor, Projectile.rotation + MathHelper.PiOver2, sparkle.Size() * 0.5f, scaleConstant * new Vector2(0.2f, 1f) * (float)Math.Pow(time, 3f) * 80f, 0, 0);
            Main.EntitySpriteDraw(sparkle.Value, Projectile.Center - Main.screenPosition, null, sparkleColor, Projectile.rotation, sparkle.Size() * 0.5f, scaleConstant * new Vector2(0.15f, 1f) * (float)Math.Pow(time, 5f) * 50f, 0, 0);
            Main.EntitySpriteDraw(sparkle.Value, Projectile.Center - Main.screenPosition, null, sparkleColor, Projectile.rotation + MathHelper.PiOver2, sparkle.Size() * 0.5f, scaleConstant * new Vector2(0.15f, 1f) * (float)Math.Pow(time, 5f) * 50f, 0, 0);
            Main.EntitySpriteDraw(sparkle.Value, Projectile.Center - Main.screenPosition, null, bloomColor, Projectile.rotation + MathHelper.PiOver4, sparkle.Size() * 0.5f, scaleConstant * new Vector2(0.2f, 1f) * (float)Math.Pow(time, 3f) * 80f, 0, 0);
            Main.EntitySpriteDraw(sparkle.Value, Projectile.Center - Main.screenPosition, null, bloomColor, Projectile.rotation + MathHelper.PiOver2 + MathHelper.PiOver4, sparkle.Size() * 0.5f, scaleConstant * new Vector2(0.2f, 1f) * (float)Math.Pow(time, 3f) * 80f, 0, 0);
            Main.EntitySpriteDraw(sparkle.Value, Projectile.Center - Main.screenPosition, null, sparkleColor, Projectile.rotation + MathHelper.PiOver4, sparkle.Size() * 0.5f, scaleConstant * new Vector2(0.15f, 1f) * (float)Math.Pow(time, 5f) * 50f, 0, 0);
            Main.EntitySpriteDraw(sparkle.Value, Projectile.Center - Main.screenPosition, null, sparkleColor, Projectile.rotation + MathHelper.PiOver2 + MathHelper.PiOver4, sparkle.Size() * 0.5f, scaleConstant * new Vector2(0.15f, 1f) * (float)Math.Pow(time, 5f) * 50f, 0, 0);

            //after
            Main.EntitySpriteDraw(sparkle.Value, Projectile.Center - Main.screenPosition, null, bloomColor * Utils.GetLerpValue(60, 50, Time, true), Projectile.rotation, sparkle.Size() * 0.5f, scaleConstant * new Vector2(0.4f, 1f) * (float)Math.Pow(time, 5f) * 120f, 0, 0);
            Main.EntitySpriteDraw(sparkle.Value, Projectile.Center - Main.screenPosition, null, bloomColor * Utils.GetLerpValue(60, 50, Time, true), Projectile.rotation + MathHelper.PiOver2, sparkle.Size() * 0.5f, scaleConstant * new Vector2(0.4f, 1f) * (float)Math.Pow(time, 5f) * 120f, 0, 0);
            Main.EntitySpriteDraw(sparkle.Value, Projectile.Center - Main.screenPosition, null, sparkleColor * Utils.GetLerpValue(60, 50, Time, true), Projectile.rotation, sparkle.Size() * 0.5f, scaleConstant * new Vector2(0.2f, 1f) * (float)Math.Pow(time, 8f) * 70f, 0, 0);
            Main.EntitySpriteDraw(sparkle.Value, Projectile.Center - Main.screenPosition, null, sparkleColor * Utils.GetLerpValue(60, 50, Time, true), Projectile.rotation + MathHelper.PiOver2, sparkle.Size() * 0.5f, scaleConstant * new Vector2(0.2f, 1f) * (float)Math.Pow(time, 8f) * 70f, 0, 0);
            Main.EntitySpriteDraw(sparkle.Value, Projectile.Center - Main.screenPosition, null, bloomColor * Utils.GetLerpValue(60, 50, Time, true), Projectile.rotation + MathHelper.PiOver4, sparkle.Size() * 0.5f, scaleConstant * new Vector2(0.4f, 1f) * (float)Math.Pow(time, 5f) * 120f, 0, 0);
            Main.EntitySpriteDraw(sparkle.Value, Projectile.Center - Main.screenPosition, null, bloomColor * Utils.GetLerpValue(60, 50, Time, true), Projectile.rotation + MathHelper.PiOver2 + MathHelper.PiOver4, sparkle.Size() * 0.5f, scaleConstant * new Vector2(0.4f, 1f) * (float)Math.Pow(time, 5f) * 120f, 0, 0);
            Main.EntitySpriteDraw(sparkle.Value, Projectile.Center - Main.screenPosition, null, sparkleColor * Utils.GetLerpValue(60, 50, Time, true), Projectile.rotation + MathHelper.PiOver4, sparkle.Size() * 0.5f, scaleConstant * new Vector2(0.2f, 1f) * (float)Math.Pow(time, 8f) * 70f, 0, 0);
            Main.EntitySpriteDraw(sparkle.Value, Projectile.Center - Main.screenPosition, null, sparkleColor * Utils.GetLerpValue(60, 50, Time, true), Projectile.rotation + MathHelper.PiOver2 + MathHelper.PiOver4, sparkle.Size() * 0.5f, scaleConstant * new Vector2(0.2f, 1f) * (float)Math.Pow(time, 8f) * 70f, 0, 0);

            return false;
        }
    }
}
