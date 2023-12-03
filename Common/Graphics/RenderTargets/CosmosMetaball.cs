using CalamityHunt.Common.Systems.Metaballs;
using CalamityHunt.Common.Systems.Particles;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ModLoader;

namespace CalamityHunt.Common.Graphics.RenderTargets;

public class CosmosMetaball : MetaballDrawer
{
    public static ParticleSystem particles;

    public override void Initialize()
    {
        particles = new ParticleSystem();
        particles.Initialize();

        content.width = Main.screenWidth;
        content.height = Main.screenHeight;
        content.draw = spriteBatch => {
            particles.Draw(spriteBatch);
        };

        On_Main.UpdateParticleSystems += UpdateCosmosParticleSystem;
        On_Main.DoDraw_DrawNPCsOverTiles += DrawTarget;
    }

    private void UpdateCosmosParticleSystem(On_Main.orig_UpdateParticleSystems orig, Main self)
    {
        orig(self);
        particles.Update();
    }

    private void DrawTarget(On_Main.orig_DoDraw_DrawNPCsOverTiles orig, Main self)
    {
        Effect effect = AssetDirectory.Effects.Cosmos.Value;
        effect.Parameters["uTime"].SetValue(Main.GlobalTimeWrappedHourly * 0.05f);
        effect.Parameters["uZoom"].SetValue(Main.GameZoomTarget);
        effect.Parameters["uTextureNoise"].SetValue(AssetDirectory.Textures.Noise[0].Value);
        effect.Parameters["uTextureClose"].SetValue(AssetDirectory.Textures.Space[0].Value);
        effect.Parameters["uTextureFar"].SetValue(AssetDirectory.Textures.Space[1].Value);
        effect.Parameters["uPosition"].SetValue(Main.screenPosition / 256f);
        effect.Parameters["uScrollClose"].SetValue(new Vector2(-Main.GlobalTimeWrappedHourly * 0.027f % 2f, -(Main.GlobalTimeWrappedHourly * 0.017f % 2f)));
        effect.Parameters["uScrollFar"].SetValue(new Vector2(Main.GlobalTimeWrappedHourly * 0.008f % 2f, -(Main.GlobalTimeWrappedHourly * 0.0004f % 2f)));
        effect.Parameters["uCloseColor"].SetValue(Color.Lerp(Color.SteelBlue, Color.Cyan, 0.1f).ToVector3());
        effect.Parameters["uFarColor"].SetValue((Color.MidnightBlue * 0.25f).ToVector3());
        effect.Parameters["uOutlineColor"].SetValue(new Color(10, 35, 85, 0).ToVector4());
        effect.Parameters["uImageSize"].SetValue(Main.ScreenSize.ToVector2());
        effect.Parameters["uNoiseRepeats"].SetValue(0.1f);

        Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, Main.DefaultSamplerState, DepthStencilState.None, Main.Rasterizer, effect, Main.GameViewMatrix.EffectMatrix);

        content.width = Main.screenWidth;
        content.height = Main.screenHeight;
        content.Request();

        if (content.IsReady) {
            Texture2D texture = content.GetTarget();
            Main.spriteBatch.Draw(texture, Vector2.Zero, texture.Frame(), Color.White, 0, Vector2.Zero, 1f, 0, 0);
        }

        Main.spriteBatch.End();

        orig(self);
    }
}
