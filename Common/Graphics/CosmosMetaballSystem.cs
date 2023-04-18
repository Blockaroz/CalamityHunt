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
                    Asset<Texture2D> outline = ModContent.Request<Texture2D>(smoke.Texture + "Outline");
                    Rectangle frame = texture.Frame(4, 2, smoke.variant % 4, (int)(smoke.variant / 4f));
                    float grow = (float)Math.Sqrt(Utils.GetLerpValue(0, 10, smoke.time, true));
                    float opacity = Utils.GetLerpValue(25, 0, smoke.time, true) * Math.Clamp(smoke.scale, 0, 1);
                    //for (int i = 0; i < 8; i++)
                    //{
                    //    Vector2 off = new Vector2(1, 0).RotatedBy(MathHelper.TwoPi / 8f * i + smoke.rotation);
                    //    Main.spriteBatch.Draw(outline.Value, (smoke.position + off - Main.screenPosition), frame, new Color(0, 255, 0, 0) * opacity, smoke.rotation, frame.Size() * 0.5f, smoke.scale * grow, 0, 0);
                    //}
                    Main.spriteBatch.Draw(texture.Value, (smoke.position - Main.screenPosition), frame, new Color(255, 0, 0, 128) * opacity * grow, smoke.rotation, frame.Size() * 0.5f, smoke.scale * (0.5f + grow * 0.5f), 0, 0);
                }
            }

            Main.graphics.GraphicsDevice.SetRenderTarget(null);

            Main.spriteBatch.End();

            orig();
        }

        private void DrawSpace(On_Main.orig_DoDraw_DrawNPCsOverTiles orig, Main self)
        {
            Effect effect = ModContent.Request<Effect>($"{nameof(CalamityHunt)}/Assets/Effects/CosmosEffect", AssetRequestMode.ImmediateLoad).Value;
            effect.Parameters["uTextureClose"].SetValue(ModContent.Request<Texture2D>($"{nameof(CalamityHunt)}/Assets/Textures/Space1").Value);
            effect.Parameters["uTextureFar"].SetValue(ModContent.Request<Texture2D>($"{nameof(CalamityHunt)}/Assets/Textures/Space0").Value);
            effect.Parameters["uPosition"].SetValue(Main.screenPosition * 0.001f);
            effect.Parameters["uParallax"].SetValue(new Vector2(0.24f, 0.15f));
            effect.Parameters["uScrollClose"].SetValue(new Vector2(Main.GlobalTimeWrappedHourly * 0.004f % 2f, Main.GlobalTimeWrappedHourly * 0.014f % 2f));
            effect.Parameters["uScrollFar"].SetValue(new Vector2(-Main.GlobalTimeWrappedHourly * 0.01f % 2f, Main.GlobalTimeWrappedHourly * 0.012f % 2f));
            effect.Parameters["uCloseColor"].SetValue(new Color(10, 60, 180).ToVector3());
            effect.Parameters["uFarColor"].SetValue(new Color(150, 20, 110).ToVector3());
            effect.Parameters["uOutlineColor"].SetValue(Color.White.ToVector3());
            effect.Parameters["uImageRatio"].SetValue(new Vector2(Main.screenWidth / (float)Main.screenHeight, 1f));

            Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, Main.DefaultSamplerState, DepthStencilState.None, Main.Rasterizer, effect, Main.Transform);

            Main.spriteBatch.Draw(spaceTarget, Vector2.Zero, null, Color.White, 0, Vector2.Zero, 1f, 0, 0);

            Main.spriteBatch.End();

            if (!Main.gamePaused && Main.mouseRight)
            {
                for (int i = 0; i < 5; i++)
                {
                    Particle.NewParticle(Particle.ParticleType<CosmicSmoke>(), Main.MouseWorld + Main.rand.NextVector2Circular(50, 36), Main.rand.NextVector2Circular(16, 14), Color.White, 4f + Main.rand.NextFloat()).data = "Cosmos";
                }
            }

            orig(self);
        }
    }
}
