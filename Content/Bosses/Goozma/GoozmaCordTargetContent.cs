using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using Terraria;
using Terraria.GameContent;
using Terraria.Graphics;
using Terraria.ModLoader;

namespace CalamityHunt.Content.Bosses.Goozma;

public class GoozmaCordTargetContent : ARenderTargetContentByRequest
{
    public Goozma Host;

    protected override void HandleUseReqest(GraphicsDevice device, SpriteBatch spriteBatch)
    {
        Vector2 targetSize = new Vector2(1024);
        Matrix realMatrix = Matrix.CreateOrthographicOffCenter(0, targetSize.X, targetSize.Y, 0, -1, 1);
        PrepareARenderTarget_AndListenToEvents(ref _target, device, (int)targetSize.X, (int)targetSize.Y, RenderTargetUsage.PreserveContents);
        device.SetRenderTarget(_target);
        device.Clear(Color.Transparent);
        List<Vector2> positions = new List<Vector2>();
        List<float> rotations = new List<float>();

        float partitions = 50;
        for (int i = 0; i < partitions; i++) {
            Vector2 offset = new Vector2(20 + Utils.GetLerpValue(0, partitions / 2.1f, i, true) * Utils.GetLerpValue(partitions * 1.3f, partitions / 3f, i, true) * (140 + (float)Math.Sin((Host.NPC.localAI[0] * 0.125f - i / (partitions * 0.036f)) % MathHelper.TwoPi) * 18 * (i / partitions)), 0).RotatedBy(MathHelper.SmoothStep(0.15f, -3.3f, i / partitions));
            offset.X *= -1;// Host.NPC.direction;
            offset -= Host.drawVelocity * Utils.GetLerpValue(partitions / 3f, partitions, i, true);
            offset *= 0.5f;
            positions.Add(new Vector2(0, 8).RotatedBy(Host.NPC.rotation) + offset.RotatedBy(Host.NPC.rotation));
            rotations.Add(offset.RotatedBy(Host.NPC.rotation).ToRotation() - MathHelper.PiOver2 * (i / partitions) * -1);
        }

        spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, Main.DefaultSamplerState, DepthStencilState.None, Main.Rasterizer, null);

        Effect effect = ModContent.Request<Effect>($"{nameof(CalamityHunt)}/Assets/Effects/GoozmaCordMap", AssetRequestMode.ImmediateLoad).Value;
        effect.Parameters["uTransformMatrix"].SetValue(realMatrix);
        effect.Parameters["uTime"].SetValue(Host.NPC.localAI[0] * 0.005f % 1f);
        effect.Parameters["uTexture"].SetValue(AssetDirectory.Textures.Goozma.LiquidTrail.Value);
        effect.Parameters["uMap"].SetValue(AssetDirectory.Textures.ColorMap[0].Value);
        effect.CurrentTechnique.Passes[0].Apply();

        VertexStrip cord = new VertexStrip();

        Color ColorFunc(float progress) => Color.White;
        float WidthFunc(float progress) => MathF.Pow(Utils.GetLerpValue(1.1f, 0.1f, progress, true), 0.7f) * 18f;

        cord.PrepareStripWithProceduralPadding(positions.ToArray(), rotations.ToArray(), ColorFunc, WidthFunc, targetSize * 0.5f, true);
        cord.DrawTrail();
        Main.pixelShader.CurrentTechnique.Passes[0].Apply();

        spriteBatch.End();

        device.SetRenderTarget(null);
        _wasPrepared = true;
    }
}
