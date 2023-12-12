using System;
using CalamityHunt.Common.Systems.Particles;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;

namespace CalamityHunt.Content.Particles;

public class FusionFlameParticle : Particle
{
    private int time;

    public int maxTime;

    private int style;

    private int direction;

    private float rotationalVelocity;

    public Func<Vector2> anchor;

    public Vector2 gravity;

    public Color fadeColor;

    public bool emitLight;

    public override void OnSpawn()
    {
        style = Main.rand.Next(15);
        direction = Main.rand.NextBool().ToDirectionInt();
        scale *= Main.rand.NextFloat(0.9f, 1.1f);
        maxTime = (maxTime <= 0) ? Main.rand.Next(50, 80) : maxTime;
        rotationalVelocity = Main.rand.NextFloat(0.01f, 0.06f);
    }

    public override void Update()
    {
        float progress = time / (maxTime * 2f);

        velocity *= 0.98f - progress * 0.15f;
        velocity += gravity;

        if (time++ > maxTime) {
            ShouldRemove = true;
        }

        if (anchor != null) {
            position += anchor.Invoke();
        }

        rotation += (1f - MathF.Cbrt(progress)) * rotationalVelocity * direction;

        if (emitLight) {
            Lighting.AddLight(position, fadeColor.ToVector3() * Utils.GetLerpValue(0.5f, 0, progress, true));
        }
    }

    public override void Draw(SpriteBatch spriteBatch)
    {
        float progress = time / (maxTime * 1.66f);

        Texture2D texture = AssetDirectory.Textures.Particle[Type].Value;
        Texture2D glow = AssetDirectory.Textures.GlowBig.Value;
        Rectangle frame = texture.Frame(1, 15, 0, style);
        SpriteEffects flip = direction > 0 ? SpriteEffects.None : SpriteEffects.FlipVertically;
        Color drawColor = Color.Lerp(color, fadeColor, Utils.GetLerpValue(0f, 0.2f, progress, true));
        float drawScale = scale * MathF.Sqrt(Utils.GetLerpValue(0f, 3f, time, true)) * (0.6f + progress);

        spriteBatch.Draw(glow, position - Main.screenPosition, glow.Frame(), fadeColor * 0.07f * Utils.GetLerpValue(0.33f, 0f, progress, true), rotation, glow.Size() * 0.5f, drawScale * 0.17f, 0, 0);

        Effect dissolveEffect = AssetDirectory.Effects.FlameDissolve.Value;

        bool moto = false;
        if (moto) {
            dissolveEffect.Parameters["uTexture0"].SetValue(AssetDirectory.Textures.WideMoto.Value);
            dissolveEffect.Parameters["uTextureScale"].SetValue(new Vector2(1f + scale * 0.01f));
            dissolveEffect.Parameters["uFrameCount"].SetValue(1);
            dissolveEffect.Parameters["uProgress"].SetValue(progress * 0.1f);
            dissolveEffect.Parameters["uPower"].SetValue(progress * 10f);
            dissolveEffect.Parameters["uNoiseStrength"].SetValue(0f);
            dissolveEffect.CurrentTechnique.Passes[0].Apply();

            spriteBatch.Draw(AssetDirectory.Textures.WideMoto.Value, position - Main.screenPosition, AssetDirectory.Textures.WideMoto.Frame(), drawColor, 0, AssetDirectory.Textures.WideMoto.Size() * 0.5f, drawScale * 0.1f, flip, 0);

            Main.pixelShader.CurrentTechnique.Passes[0].Apply();
            return;
        }

        dissolveEffect.Parameters["uTexture0"].SetValue(AssetDirectory.Textures.Noise[11].Value);
        dissolveEffect.Parameters["uTextureScale"].SetValue(new Vector2(0.6f + scale * 0.01f));
        dissolveEffect.Parameters["uFrameCount"].SetValue(10);
        dissolveEffect.Parameters["uProgress"].SetValue(progress * 0.8f);
        dissolveEffect.Parameters["uPower"].SetValue(10f + progress * 60f);
        dissolveEffect.Parameters["uNoiseStrength"].SetValue(1f);
        dissolveEffect.CurrentTechnique.Passes[0].Apply();

        Vector2 squish = new Vector2(1f - progress * 0.1f, 1f + progress * 0.1f);
        spriteBatch.Draw(texture, position - Main.screenPosition, frame, drawColor, rotation - MathHelper.PiOver2 * direction, frame.Size() * 0.5f, squish * drawScale * 0.45f, flip, 0);

        Main.pixelShader.CurrentTechnique.Passes[0].Apply();
    }
}
