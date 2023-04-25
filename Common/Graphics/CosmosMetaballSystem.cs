using CalamityHunt.Common.Systems.Particles;
using CalamityHunt.Content.Bosses.Goozma;
using CalamityHunt.Content.Bosses.Goozma.Projectiles;
using CalamityHunt.Content.Particles;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using System;
using System.Linq;
using Terraria;
using Terraria.Map;
using Terraria.ModLoader;

namespace CalamityHunt.Common.Graphics
{
    public class CosmosMetaballSystem : ILoadable
    {
        public void Load(Mod mod)
        {
            On_Main.CheckMonoliths += DrawSpaceShapes;
            On_Main.DoDraw_DrawNPCsOverTiles += DrawSpace;
        }
        public void Unload()
        {
            On_Main.CheckMonoliths -= DrawSpaceShapes;
            On_Main.DoDraw_DrawNPCsOverTiles -= DrawSpace;
            Main.QueueMainThreadAction(spaceTarget.Dispose);
        }

        public RenderTarget2D spaceTarget;

        private void DrawSpaceShapes(On_Main.orig_CheckMonoliths orig)
        {
            if (spaceTarget == null || spaceTarget.IsDisposed)
                spaceTarget = new RenderTarget2D(Main.graphics.GraphicsDevice, Main.screenWidth, Main.screenHeight);

            else if (spaceTarget.Size() != new Vector2(Main.screenWidth, Main.screenHeight))
            {
                Main.QueueMainThreadAction(() =>
                {
                    spaceTarget.Dispose();
                    spaceTarget = new RenderTarget2D(Main.graphics.GraphicsDevice, Main.screenWidth, Main.screenHeight);
                });
                return;
            }

            Main.spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, Main.DefaultSamplerState, DepthStencilState.None, Main.Rasterizer, null);

            Main.graphics.GraphicsDevice.SetRenderTarget(spaceTarget);
            Main.graphics.GraphicsDevice.Clear(Color.Transparent);

            foreach (Particle particle in ParticleSystem.particle.Where(n => n.Active && n is CosmicSmoke && n.data is string))
            {
                CosmicSmoke smoke = particle as CosmicSmoke;

                if ((string)particle.data == "Cosmos")
                {
                    Asset<Texture2D> texture = ModContent.Request<Texture2D>(smoke.Texture);
                    Rectangle frame = texture.Frame(4, 2, smoke.variant % 4, (int)(smoke.variant / 4f));
                    float grow = (float)Math.Sqrt(Utils.GetLerpValue(0, smoke.maxTime * 0.2f, smoke.time, true));
                    float opacity = Utils.GetLerpValue(smoke.maxTime * 0.7f, smoke.maxTime * 0.2f, smoke.time, true) * Math.Clamp(smoke.scale, 0, 1);
                    Main.spriteBatch.Draw(texture.Value, smoke.position - Main.screenPosition, frame, Color.White * 0.5f * opacity * grow, smoke.rotation, frame.Size() * 0.5f, smoke.scale * grow * 0.5f, 0, 0);
                }
                if ((string)particle.data == "Interstellar")
                {
                    Asset<Texture2D> texture = ModContent.Request<Texture2D>(smoke.Texture);
                    Rectangle frame = texture.Frame(4, 2, smoke.variant % 4, (int)(smoke.variant / 4f));
                    float grow = (float)Math.Sqrt(Utils.GetLerpValue(0, smoke.maxTime * 0.2f, smoke.time, true));
                    float opacity = Utils.GetLerpValue(smoke.maxTime * 0.7f, 0, smoke.time, true) * Math.Clamp(smoke.scale, 0, 1);
                    Main.spriteBatch.Draw(texture.Value, smoke.position - Main.screenPosition, frame, Color.White * 0.7f * opacity * smoke.color.ToVector4().Length(), smoke.rotation, frame.Size() * 0.5f, smoke.scale * grow * 0.5f, 0, 0);
                }
            }

            Effect absorbEffect = ModContent.Request<Effect>($"{nameof(CalamityHunt)}/Assets/Effects/SpaceAbsorb", AssetRequestMode.ImmediateLoad).Value;
            absorbEffect.Parameters["uRepeats"].SetValue(1f);
            absorbEffect.Parameters["uTexture0"].SetValue(ModContent.Request<Texture2D>($"{nameof(CalamityHunt)}/Assets/Textures/Noise/Noise0").Value);
            absorbEffect.Parameters["uTexture1"].SetValue(ModContent.Request<Texture2D>($"{nameof(CalamityHunt)}/Assets/Textures/Noise/Noise1").Value);

            foreach (Projectile projectile in Main.projectile.Where(n => n.active && n.ModProjectile is BlackHoleBlender))
            {
                BlackHoleBlender blender = projectile.ModProjectile as BlackHoleBlender;
                Asset<Texture2D> texture = ModContent.Request<Texture2D>(blender.Texture);
                Asset<Texture2D> shadow = ModContent.Request<Texture2D>(blender.Texture + "Shadow");

                absorbEffect.Parameters["uTime"].SetValue(projectile.localAI[0] * 0.002f % 1f);
                absorbEffect.Parameters["uSize"].SetValue(projectile.scale * new Vector2(8f));

                Main.spriteBatch.End();
                Main.spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, Main.DefaultSamplerState, DepthStencilState.None, Main.Rasterizer, absorbEffect);

                Asset<Texture2D> bloom = ModContent.Request<Texture2D>($"{nameof(CalamityHunt)}/Assets/Textures/Goozma/GlowSoftBig");
                Main.spriteBatch.Draw(bloom.Value, projectile.Center - Main.screenPosition, bloom.Frame(), Color.White, 0, bloom.Size() * 0.5f, projectile.scale * 7f, 0, 0);

                Main.spriteBatch.End();
                Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, Main.DefaultSamplerState, DepthStencilState.None, Main.Rasterizer, null);

                Main.spriteBatch.Draw(texture.Value, (projectile.Center - Main.screenPosition), texture.Frame(), Color.White * projectile.scale * 0.2f, projectile.rotation, texture.Size() * 0.5f, projectile.scale * 0.8f, 0, 0);
                Main.spriteBatch.Draw(texture.Value, (projectile.Center - Main.screenPosition), texture.Frame(), Color.White * projectile.scale * 0.1f, projectile.rotation * 0.8f - 0.5f, texture.Size() * 0.5f, projectile.scale * 0.7f, 0, 0);
                Main.spriteBatch.Draw(texture.Value, (projectile.Center - Main.screenPosition), texture.Frame(), Color.White * projectile.scale * 0.1f, -projectile.rotation * 0.9f - 1f, texture.Size() * 0.5f, projectile.scale * 0.5f, 0, 0);
                Main.spriteBatch.Draw(texture.Value, (projectile.Center - Main.screenPosition), texture.Frame(), Color.White * projectile.scale * 0.1f, projectile.rotation * 0.9f - 0.2f, texture.Size() * 0.5f, projectile.scale * 0.9f, 0, 0);
                Main.spriteBatch.Draw(shadow.Value, (projectile.Center - Main.screenPosition), shadow.Frame(), Color.White * projectile.scale * 0.1f, projectile.rotation, shadow.Size() * 0.5f, projectile.scale * 0.5f, 0, 0);
                Main.spriteBatch.Draw(shadow.Value, (projectile.Center - Main.screenPosition), shadow.Frame(), Color.White * projectile.scale * 0.1f, projectile.rotation * 0.5f, shadow.Size() * 0.5f, projectile.scale * 0.7f, 0, 0);
            }

            Main.graphics.GraphicsDevice.SetRenderTarget(null);

            Main.spriteBatch.End();

            orig();
        }

        private void DrawSpace(On_Main.orig_DoDraw_DrawNPCsOverTiles orig, Main self)
        {
            Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, Main.DefaultSamplerState, DepthStencilState.None, Main.Rasterizer, null, Main.Transform);

            //foreach (Particle particle in ParticleSystem.particle.Where(n => n.Active && n is CosmicSmoke && n.data is string))
            //{
            //    CosmicSmoke smoke = particle as CosmicSmoke;
            //    if ((string)smoke.data == "Cosmos")
            //    {
            //        Asset<Texture2D> texture = ModContent.Request<Texture2D>($"{nameof(CalamityHunt)}/Assets/Textures/Goozma/GlowSoft");
            //        float grow = (float)Math.Sqrt(Utils.GetLerpValue(0, 7, smoke.time, true));
            //        float opacity = Utils.GetLerpValue(22, 10, smoke.time, true) * Math.Clamp(smoke.scale, 0, 1);
            //        //new Color(20, 13, 11, 0)
            //        Main.spriteBatch.Draw(texture.Value, (smoke.position - Main.screenPosition), null, new Color(13, 7, 20, 0) * 0.1f, smoke.rotation, texture.Size() * 0.5f, smoke.scale * (0.5f + grow * 0.5f) * 4f * opacity, 0, 0);
            //    }
            //}

            Effect absorbEffect = ModContent.Request<Effect>($"{nameof(CalamityHunt)}/Assets/Effects/SpaceAbsorb", AssetRequestMode.ImmediateLoad).Value;
            absorbEffect.Parameters["uRepeats"].SetValue(1f);
            absorbEffect.Parameters["uTexture0"].SetValue(ModContent.Request<Texture2D>($"{nameof(CalamityHunt)}/Assets/Textures/Noise/Noise0").Value);
            absorbEffect.Parameters["uTexture1"].SetValue(ModContent.Request<Texture2D>($"{nameof(CalamityHunt)}/Assets/Textures/Noise/Noise1").Value);

            foreach (Projectile projectile in Main.projectile.Where(n => n.active && n.ModProjectile is BlackHoleBlender))
            {
                Asset<Texture2D> bloom = ModContent.Request<Texture2D>($"{nameof(CalamityHunt)}/Assets/Textures/Goozma/GlowSoftBig");
                Main.spriteBatch.Draw(bloom.Value, (projectile.Center - Main.screenPosition), null, new Color(33, 5, 65, 0) * (float)Math.Pow(projectile.scale, 1.5f), projectile.rotation, bloom.Size() * 0.5f, 3.5f, 0, 0);

                absorbEffect.Parameters["uTime"].SetValue(projectile.localAI[0] * 0.002f % 1f);
                absorbEffect.Parameters["uSize"].SetValue(new Vector2(8f));

                Main.spriteBatch.End();
                Main.spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, Main.DefaultSamplerState, DepthStencilState.None, Main.Rasterizer, absorbEffect);

                Main.spriteBatch.Draw(bloom.Value, projectile.Center - Main.screenPosition, bloom.Frame(), new Color(255, 150, 60, 0) * projectile.scale, 0, bloom.Size() * 0.5f, projectile.scale * 6f, 0, 0);

                Main.spriteBatch.End();
                Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, Main.DefaultSamplerState, DepthStencilState.None, Main.Rasterizer, null);
            }
            
            Effect effect = ModContent.Request<Effect>($"{nameof(CalamityHunt)}/Assets/Effects/CosmosEffect", AssetRequestMode.ImmediateLoad).Value;
            effect.Parameters["uTextureClose"].SetValue(ModContent.Request<Texture2D>($"{nameof(CalamityHunt)}/Assets/Textures/Space0").Value);
            effect.Parameters["uTextureFar"].SetValue(ModContent.Request<Texture2D>($"{nameof(CalamityHunt)}/Assets/Textures/Space1").Value);
            effect.Parameters["uPosition"].SetValue((Main.LocalPlayer.oldPosition - Main.LocalPlayer.oldVelocity) * 0.001f);
            effect.Parameters["uParallax"].SetValue(new Vector2(0.5f, 0.2f));
            effect.Parameters["uScrollClose"].SetValue(new Vector2(-Main.GlobalTimeWrappedHourly * 0.027f % 2f, -Main.GlobalTimeWrappedHourly * 0.017f % 2f));
            effect.Parameters["uScrollFar"].SetValue(new Vector2(Main.GlobalTimeWrappedHourly * 0.008f % 2f, Main.GlobalTimeWrappedHourly * 0.0004f % 2f));
            effect.Parameters["uCloseColor"].SetValue(new Color(20, 50, 255).ToVector3());
            effect.Parameters["uFarColor"].SetValue(new Color(110, 20, 200).ToVector3());
            effect.Parameters["uOutlineColor"].SetValue(new Color(20, 5, 45, 0).ToVector4());
            effect.Parameters["uImageRatio"].SetValue(new Vector2(Main.screenWidth / (float)Main.screenHeight, 1f));

            Main.spriteBatch.End();
            Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, Main.DefaultSamplerState, DepthStencilState.None, Main.Rasterizer, effect, Main.Transform);

            Main.spriteBatch.Draw(spaceTarget, Vector2.Zero, null, Color.White, 0, Vector2.Zero, 1f, 0, 0);

            Main.spriteBatch.End();

            //if (Main.UpdateTimeAccumulator < 0.0166666)
            //{
            //    if (Main.mouseLeft)
            //    {
            //        for (int i = 0; i < 5; i++)
            //        {
            //            Particle smoke = Particle.NewParticle(Particle.ParticleType<CosmicSmoke>(), Main.MouseWorld, Main.rand.NextVector2Circular(8, 8), Color.White, 1f + Main.rand.NextFloat(2f));
            //            smoke.data = "Cosmos";
            //        }
            //    }

            //    if (Main.mouseRight)
            //    {
            //        for (int i = 0; i < 5; i++)
            //        {
            //            Color drawColor = new GradientColor(SlimeUtils.GoozColorArray, 0.1f, 0.1f).Value;
            //            drawColor.A = 0;
            //            Particle smoke = Particle.NewParticle(Particle.ParticleType<CosmicSmoke>(), Main.MouseWorld, Main.rand.NextVector2Circular(5, 5), drawColor, 1f + Main.rand.NextFloat(2f));
            //        }
            //    }
            //}

            orig(self);
        }
    }
}
