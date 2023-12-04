using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.GameContent;
using Terraria.Graphics;

namespace CalamityHunt.Common.Graphics.RenderTargets;

public class GoozmoemCordTextureContent : ARenderTargetContentByRequest
{
    public int width;
    public int height;
    public Vector2[] positions;

    protected override void HandleUseReqest(GraphicsDevice device, SpriteBatch spriteBatch)
    {
        var asset = AssetDirectory.Textures.Goozma.PaladinPalanquinBall.Value;
        PrepareARenderTarget_AndListenToEvents(ref _target, device, width, height, RenderTargetUsage.PreserveContents);
        device.SetRenderTarget(_target);
        device.Clear(Color.Transparent);

        var rotations = new float[positions.Length];
        for (var i = 1; i < positions.Length; i++)
            rotations[i] = positions[i - 1].AngleTo(positions[i]);
        rotations[positions.Length - 1] = rotations[positions.Length - 2];

        var effect = AssetDirectory.Effects.GoozmaCordMap.Value;
        effect.Parameters["uTransformMatrix"].SetValue(Matrix.CreateOrthographicOffCenter(0, width, height, 0, -1, 1));
        if (!Main.gameInactive) {
            effect.Parameters["uTime"].SetValue(Main.GlobalTimeWrappedHourly * 0.5f);
        }
        effect.Parameters["uTexture"].SetValue(AssetDirectory.Textures.Goozma.LiquidTrail.Value);
        effect.Parameters["uMap"].SetValue(AssetDirectory.Textures.ColorMap[0].Value);

        spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, Main.DefaultSamplerState, DepthStencilState.None, Main.Rasterizer, effect, Main.GameViewMatrix.EffectMatrix);
        effect.CurrentTechnique.Passes[0].Apply();

        VertexStrip strip = new VertexStrip();

        Color ColorFunc(float progress) => Color.White;
        float WidthFunc(float progress) => MathF.Pow(Utils.GetLerpValue(0f, 0.3f, progress, true) * Utils.GetLerpValue(1f, 0.8f, progress, true), 0.7f) * 6f;
        strip.PrepareStrip(positions, rotations, ColorFunc, WidthFunc, Vector2.Zero, positions.Length, true);
        strip.DrawTrail();

        Main.pixelShader.CurrentTechnique.Passes[0].Apply();
        spriteBatch.End();

        device.SetRenderTarget(null);
        _wasPrepared = true;
    }
}
