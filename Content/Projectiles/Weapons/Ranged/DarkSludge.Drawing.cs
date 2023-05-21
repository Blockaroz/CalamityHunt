using CalamityHunt.Common.Systems.Particles;
using CalamityHunt.Content.Particles;
using log4net.Util;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.ModLoader;

namespace CalamityHunt.Content.Projectiles.Weapons.Ranged
{
    public partial class DarkSludge : ModProjectile
    {
        public override void Load()
        {
            On_Main.CheckMonoliths += DrawShapes;
            On_Main.DrawProjectiles += DrawSludge;
        }

        public static RenderTarget2D SludgeTarget { get; set; }

        private void DrawShapes(On_Main.orig_CheckMonoliths orig)
        {
            if (SludgeTarget == null || SludgeTarget.IsDisposed)
                SludgeTarget = new RenderTarget2D(Main.graphics.GraphicsDevice, Main.screenWidth, Main.screenHeight);

            else if (SludgeTarget.Size() != new Vector2(Main.screenWidth, Main.screenHeight))
            {
                Main.QueueMainThreadAction(() =>
                {
                    SludgeTarget.Dispose();
                    SludgeTarget = new RenderTarget2D(Main.graphics.GraphicsDevice, Main.screenWidth, Main.screenHeight);
                });
                return;
            }

            Main.spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, Main.DefaultSamplerState, DepthStencilState.None, Main.Rasterizer, null);

            Main.graphics.GraphicsDevice.SetRenderTarget(SludgeTarget);
            Main.graphics.GraphicsDevice.Clear(Color.Transparent);

            foreach (Projectile projectile in Main.projectile.Where(n => n.active && n.ai[0] > 0 && n.type == ModContent.ProjectileType<DarkSludge>()))
            {
                Texture2D texture = ModContent.Request<Texture2D>(projectile.ModProjectile.Texture).Value;
                projectile.frame = (projectile.ai[1] == 1 ? 2 : 0) + (int)projectile.localAI[0];
                Rectangle frame = texture.Frame(4, 1, projectile.frame, 0);

                Vector2 squish = new Vector2(1f - projectile.velocity.Length() * 0.01f, 1f + projectile.velocity.Length() * 0.01f) * 0.8f;
                if (projectile.frame > 2)
                    squish = new Vector2(2f + MathF.Sin(projectile.ai[0] * 0.1f) * 0.1f, 1.7f + MathF.Cos(projectile.ai[0] * 0.1f) * 0.3f) * 0.7f;

                Main.spriteBatch.Draw(texture, (projectile.Bottom - Main.screenPosition) / 2f, frame, Color.DimGray * 0.2f, projectile.rotation - MathHelper.PiOver2, frame.Size() * new Vector2(0.5f, 0.8f), projectile.scale * squish * 0.5f, 0, 0);
            }

            foreach (DarkSludgeChunk particle in ParticleSystem.particle.Where(n => n.Active && n is DarkSludgeChunk))
            {
                if (particle.time > 0)
                {
                    Asset<Texture2D> texture = ModContent.Request<Texture2D>(Texture);
                    Rectangle frame = texture.Frame(4, 1, particle.variant, 0);
                    Vector2 squish = new Vector2(1f - particle.velocity.Length() * 0.01f, 1f + particle.velocity.Length() * 0.01f);
                    float grow = (float)Math.Sqrt(Utils.GetLerpValue(-20, 40, particle.time, true));
                    if (particle.stuck)
                    {
                        grow = 1f;
                        frame = texture.Frame(4, 1, particle.variant + 2, 0);
                        squish = new Vector2(1f + (float)Math.Sqrt(Utils.GetLerpValue(20, 0, particle.time, true)) * 0.33f, 1f - (float)Math.Sqrt(Utils.GetLerpValue(20, 0, particle.time, true)) * 0.33f);
                    }

                    Color lightColor = Lighting.GetColor(particle.position.ToTileCoordinates());
                    Main.spriteBatch.Draw(texture.Value, (particle.position - Main.screenPosition) / 2f, frame, Color.DimGray * 0.2f, particle.rotation, frame.Size() * new Vector2(0.5f, 0.84f), particle.scale * grow * squish * 0.5f, 0, 0);
                }
            }

            Main.graphics.GraphicsDevice.SetRenderTarget(null);

            Main.spriteBatch.End();

            orig();
        }

        private void DrawSludge(On_Main.orig_DrawProjectiles orig, Main self)
        {
            //Effect effect = ModContent.Request<Effect>($"{nameof(CalamityHunt)}/Assets/Effects/SludgeEffect", ReLogic.Content.AssetRequestMode.ImmediateLoad).Value;
            Effect effect = ModContent.Request<Effect>($"{nameof(CalamityHunt)}/Assets/Effects/CosmosEffect", AssetRequestMode.ImmediateLoad).Value;
            effect.Parameters["uTextureClose"].SetValue(ModContent.Request<Texture2D>($"{nameof(CalamityHunt)}/Assets/Textures/Space0").Value);
            effect.Parameters["uTextureFar"].SetValue(ModContent.Request<Texture2D>($"{nameof(CalamityHunt)}/Assets/Textures/Space1").Value);
            effect.Parameters["uPosition"].SetValue((Main.LocalPlayer.oldPosition - Main.LocalPlayer.oldVelocity) * 0.001f);
            effect.Parameters["uParallax"].SetValue(new Vector2(0.5f, 0.2f));
            effect.Parameters["uScrollClose"].SetValue(new Vector2(-Main.GlobalTimeWrappedHourly * 0.027f % 2f, -Main.GlobalTimeWrappedHourly * 0.017f % 2f));
            effect.Parameters["uScrollFar"].SetValue(new Vector2(Main.GlobalTimeWrappedHourly * 0.008f % 2f, Main.GlobalTimeWrappedHourly * 0.0004f % 2f));
            effect.Parameters["uCloseColor"].SetValue(Color.Chartreuse.ToVector3());
            effect.Parameters["uFarColor"].SetValue(Color.SeaGreen.ToVector3());
            effect.Parameters["uOutlineColor"].SetValue(Color.Chartreuse.ToVector4());
            effect.Parameters["uImageRatio"].SetValue(new Vector2(Main.screenWidth / (float)Main.screenHeight, 1f));

            Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, Main.DefaultSamplerState, DepthStencilState.None, Main.Rasterizer, effect, Main.Transform);

            if (SludgeTarget != null)
                Main.spriteBatch.Draw(SludgeTarget, Vector2.Zero, null, Color.White, 0f, Vector2.Zero, 2f, 0, 0);
            
            Main.spriteBatch.End();

            orig(self);
        }

    }
}
