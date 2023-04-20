using CalamityHunt.Common.Systems.Particles;
using CalamityHunt.Content.Particles;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using System;
using System.Linq;
using Terraria;
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

            foreach (Particle p in ParticleSystem.particle.Where(n => n.Active && n is CosmicSmoke && n.data is string))
            {
                CosmicSmoke smoke = p as CosmicSmoke;
                if ((string)smoke.data == "Cosmos")
                {
                    Asset<Texture2D> texture = ModContent.Request<Texture2D>(smoke.Texture);
                    Rectangle frame = texture.Frame(4, 2, smoke.variant % 4, (int)(smoke.variant / 4f));
                    float grow = (float)Math.Sqrt(Utils.GetLerpValue(0, 7, smoke.time, true));
                    float opacity = Utils.GetLerpValue(22, 0, smoke.time, true) * Math.Clamp(smoke.scale, 0, 1);
                    Main.spriteBatch.Draw(texture.Value, (smoke.position - Main.screenPosition), frame, Color.White * 0.5f * opacity * grow, smoke.rotation, frame.Size() * 0.5f, smoke.scale * (0.5f + grow * 0.5f), 0, 0);
                }
            }

            Main.graphics.GraphicsDevice.SetRenderTarget(null);

            Main.spriteBatch.End();

            orig();
        }

        private void DrawSpace(On_Main.orig_DoDraw_DrawNPCsOverTiles orig, Main self)
        {
            Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, Main.DefaultSamplerState, DepthStencilState.None, Main.Rasterizer, null, Main.Transform);

            foreach (Particle p in ParticleSystem.particle.Where(n => n.Active && n is CosmicSmoke && n.data is string))
            {
                CosmicSmoke smoke = p as CosmicSmoke;
                if ((string)smoke.data == "Cosmos")
                {
                    Asset<Texture2D> texture = ModContent.Request<Texture2D>($"{nameof(CalamityHunt)}/Assets/Textures/Goozma/GlowSoft");
                    float grow = (float)Math.Sqrt(Utils.GetLerpValue(0, 7, smoke.time, true));
                    float opacity = Utils.GetLerpValue(22, 10, smoke.time, true) * Math.Clamp(smoke.scale, 0, 1);
                    //new Color(20, 13, 11, 0)
                    Main.spriteBatch.Draw(texture.Value, (smoke.position - Main.screenPosition), null, new Color(15, 5, 35, 0) * 0.1f * opacity * grow, smoke.rotation, texture.Size() * 0.5f, smoke.scale * (0.5f + grow * 0.5f) * 2f, 0, 0);
                }
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
            effect.Parameters["uOutlineColor"].SetValue(Color.White.ToVector3());
            effect.Parameters["uImageRatio"].SetValue(new Vector2(Main.screenWidth / (float)Main.screenHeight, 1f));

            Main.spriteBatch.End();
            Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, Main.DefaultSamplerState, DepthStencilState.None, Main.Rasterizer, effect, Main.Transform);

            Main.spriteBatch.Draw(spaceTarget, Vector2.Zero, null, Color.White, 0, Vector2.Zero, 1f, 0, 0);

            Main.spriteBatch.End();

            //if (Main.mouseLeft)
            //{
            //    for (int i = 0; i < 10; i++)
            //    {
            //        Particle.NewParticle(Particle.ParticleType<CosmicSmoke>(), Main.MouseWorld, Main.rand.NextVector2Circular(8, 8), Color.White, 2f + Main.rand.NextFloat()).data = "Cosmos";
            //    }
            //}

            orig(self);
        }
    }
}
