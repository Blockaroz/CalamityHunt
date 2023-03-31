using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;
using System.Linq;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityHunt.Common.Systems.Particles
{
    [Autoload(Side = ModSide.Client)]
    public class ParticleSystem : ModSystem
    {
        private static int nextID;

        internal static readonly IList<Particle> particleTypes = new List<Particle>();

        public static IList<Particle> particle = new List<Particle>();

        internal static int ReserveParticleID() => nextID++;

        public static Particle GetParticle(int type) => (type == -1) ? null : particleTypes[type];

        public override void OnModLoad()
        {
            Main.QueueMainThreadAction(() => particleBatch = new SpriteBatch(Main.graphics.GraphicsDevice));
            On_Main.UpdateParticleSystems += UpdateParticles;
            On_Main.DoDraw_DrawNPCsOverTiles += DrawParticlesUnderEntities;
            On_Main.DrawDust += DrawParticles;
        }

        public override void OnModUnload()
        {
            On_Main.UpdateParticleSystems -= UpdateParticles;
            On_Main.DoDraw_DrawNPCsOverTiles -= DrawParticlesUnderEntities;
            On_Main.DrawDust -= DrawParticles;

            particleTypes.Clear();
            nextID = 0;
        }

        public static void UpdateParticles()
        {
            if (Main.dedServ || Main.gamePaused || Main.netMode == NetmodeID.Server)
                return;

            foreach (Particle item in particle.ToList())
            {
                item.position += item.velocity;
                item.Update();
                if (!item.Active)
                    particle.Remove(item);

            }
        }

        public static void DrawParticlesUnderEntities(SpriteBatch spriteBatch)
        {
            if (Main.dedServ || Main.gameMenu || Main.netMode == NetmodeID.Server)
                return;

            spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.PointClamp, DepthStencilState.None, Main.Rasterizer, null, Main.Transform);
            Rectangle value = new Rectangle((int)Main.screenPosition.X - Main.screenWidth, (int)Main.screenPosition.Y - Main.screenHeight, Main.screenWidth * 2, Main.screenHeight * 2);

            foreach (Particle particle in particle.Where((Particle p) => p.shader == null && p.behindEntities))
            {
                if (new Rectangle((int)particle.position.X - 3, (int)particle.position.Y - 3, 6, 6).Intersects(value))
                {
                    particle.Draw(spriteBatch);
                }
            }

            spriteBatch.End();
            spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, SamplerState.PointClamp, DepthStencilState.None, Main.Rasterizer, null, Main.Transform);

            foreach (Particle particle2 in particle.Where((Particle p) => p.shader != null && p.behindEntities))
            {
                if (new Rectangle((int)particle2.position.X - 3, (int)particle2.position.Y - 3, 6, 6).Intersects(value))
                {
                    particle2.shader.Apply(null);
                    particle2.Draw(spriteBatch);
                    Main.pixelShader.CurrentTechnique.Passes[0].Apply();
                }
            }

            spriteBatch.End();
        }

        public static SpriteBatch particleBatch;

        public static void DrawParticles(SpriteBatch spriteBatch)
        {
            if (Main.dedServ || Main.gameMenu || Main.netMode == NetmodeID.Server)
                return;

            spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.PointClamp, DepthStencilState.None, Main.Rasterizer, null, Main.Transform);
            Rectangle value = new Rectangle((int)Main.screenPosition.X - Main.screenWidth, (int)Main.screenPosition.Y - Main.screenHeight, Main.screenWidth * 2, Main.screenHeight * 2);

            foreach (Particle particle in particle.Where((Particle p) => p.shader == null && !p.behindEntities))
            {
                if (new Rectangle((int)particle.position.X - 3, (int)particle.position.Y - 3, 6, 6).Intersects(value))
                {
                    particle.Draw(spriteBatch);
                }
            }

            spriteBatch.End();
            spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, SamplerState.PointClamp, DepthStencilState.None, Main.Rasterizer, null, Main.Transform);

            foreach (Particle particle2 in particle.Where((Particle p) => p.shader != null && !p.behindEntities))
            {
                if (new Rectangle((int)particle2.position.X - 3, (int)particle2.position.Y - 3, 6, 6).Intersects(value))
                {
                    particle2.shader.Apply(null);
                    particle2.Draw(spriteBatch);
                    Main.pixelShader.CurrentTechnique.Passes[0].Apply();
                }
            }

            spriteBatch.End();
        }

        private static void DrawParticlesUnderEntities(On_Main.orig_DoDraw_DrawNPCsOverTiles orig, Main self)
        {
            DrawParticlesUnderEntities(Main.spriteBatch);

            orig(self);
        }

        private static void DrawParticles(On_Main.orig_DrawDust orig, Main self)
        {
            DrawParticles(Main.spriteBatch);

            orig(self);
        }

        private static void UpdateParticles(On_Main.orig_UpdateParticleSystems orig, Main self)
        {
            orig(self);
            UpdateParticles();
        }
    }
}