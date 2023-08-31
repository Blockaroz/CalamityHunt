using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using Terraria;
using Terraria.DataStructures;
using Terraria.Graphics.Shaders;
using Terraria.ModLoader;

namespace CalamityHunt.Content.Particles.FlyingSlimes
{
    public class FlyingShimmerSlimeParticleBehavior : FlyingSlimeParticleBehavior
    {
        public override bool ShouldDraw => false;
        public override void PostUpdate()
        {
            if (Main.rand.NextBool(10))
            {
                Dust slime = Dust.NewDustPerfect(position + Main.rand.NextVector2Circular(20, 20), 306, velocity * 0.2f, 128, color, 0.5f + Main.rand.NextFloat() * 0.3f);
                slime.fadeIn = 0.9f;
                slime.color = Main.hslToRgb(((float)Main.timeForVisualEffects / 300f + Main.rand.NextFloat() * 0.1f) % 1f, 1f, 0.65f, 0);
                slime.noLightEmittence = true;
                slime.noGravity = true;
            }

            Lighting.AddLight(position + velocity, Main.hslToRgb(((float)Main.timeForVisualEffects / 300f + Main.rand.NextFloat() * 0.1f) % 1f, 1f, 0.65f, 0).ToVector3() * 0.1f);
        }

        public override void DrawSlime(SpriteBatch spriteBatch)
        {
            Asset<Texture2D> texture = ModContent.Request<Texture2D>(Texture);

            float fadeIn = Utils.GetLerpValue(0, 30, time, true) * distanceFade;
            spriteBatch.End();
            spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, SamplerState.PointClamp, DepthStencilState.Default, RasterizerState.CullNone, null, Main.Transform);

            for (int i = 0; i < 10; i++)
            {
                DrawData trailData = new DrawData(texture.Value, position - velocity * i * 0.5f - Main.screenPosition, texture.Frame(), color.MultiplyRGBA(Lighting.GetColor(position.ToTileCoordinates())) * fadeIn * ((10f - i) / 50f), rotation + MathHelper.PiOver2, texture.Size() * 0.5f, scale * distanceFade * 1.05f, 0, 0);
                GameShaders.Misc["RainbowTownSlime"].Apply(trailData);
                trailData.Draw(spriteBatch);
            }
            DrawData data = new DrawData(texture.Value, position - Main.screenPosition, texture.Frame(), color.MultiplyRGBA(Lighting.GetColor(position.ToTileCoordinates())) * fadeIn, rotation + MathHelper.PiOver2, texture.Size() * 0.5f, scale * distanceFade, 0, 0);
            GameShaders.Misc["RainbowTownSlime"].Apply(data);
            data.Draw(spriteBatch); 
            Main.pixelShader.CurrentTechnique.Passes[0].Apply();

            spriteBatch.End();
            spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, Main.DefaultSamplerState, DepthStencilState.None, Main.Rasterizer, null, Main.Transform);
        }
    }
}
