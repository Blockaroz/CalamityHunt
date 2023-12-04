using System;
using CalamityHunt.Common.Systems.Particles;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;

namespace CalamityHunt.Content.Particles;

public class SmokeSplatterParticle : Particle
{
    private int time;

    public int maxTime;

    private int style;

    private int direction;

    private float rotationalVelocity;

    public Func<Vector2> anchor;

    public Vector2 gravity;

    public Color fadeColor;

    public override void OnSpawn()
    {
        style = Main.rand.Next(5);
        direction = Main.rand.NextBool().ToDirectionInt();
        scale *= Main.rand.NextFloat(0.9f, 1.1f);
        rotationalVelocity = Main.rand.NextFloat(0.05f);
    }

    public override void Update()
    {
        float progress = time / (maxTime * 2f);

        velocity *= 1f - progress * 0.3f;
        velocity += gravity * (1f + progress * 0.5f);

        if (time++ > maxTime) {
            ShouldRemove = true;
        }

        if (anchor != null) {
            position += anchor.Invoke();
        }

        rotation += (1f - MathF.Cbrt(progress)) * rotationalVelocity * direction;
    }

    public override void Draw(SpriteBatch spriteBatch)
    {
        float progress = time / (maxTime * 1.5f);

        Texture2D texture = AssetDirectory.Textures.Particle[Type].Value;
        Rectangle frame = texture.Frame(1, 5, 0, style);
        SpriteEffects flip = direction > 0 ? SpriteEffects.None : SpriteEffects.FlipVertically;
        Color drawColor = Color.Lerp(color, fadeColor, Utils.GetLerpValue(0f, 0.2f, progress, true));
        float drawScale = scale * MathF.Sqrt(Utils.GetLerpValue(-0.1f, 5f, time, true)) * (0.5f + progress * 1.33f);

        Effect dissolveEffect = AssetDirectory.Effects.FlameDissolve.Value;
        dissolveEffect.Parameters["uTexture0"].SetValue(AssetDirectory.Textures.Noise[11].Value);
        dissolveEffect.Parameters["uTextureScale"].SetValue(new Vector2(0.5f + scale * 0.05f));
        dissolveEffect.Parameters["uFrameCount"].SetValue(5);
        dissolveEffect.Parameters["uProgress"].SetValue(Math.Clamp(1f - MathF.Sqrt(1f - progress), 0f, 1f));
        dissolveEffect.Parameters["uPower"].SetValue(1f + Utils.GetLerpValue(0.15f, 0.8f, progress, true) * 50f);
        dissolveEffect.Parameters["uNoiseStrength"].SetValue(0.7f);
        dissolveEffect.CurrentTechnique.Passes[0].Apply();

        Vector2 squish = new Vector2(1f - progress * 0.4f, 1f + progress * 0.4f);
        spriteBatch.Draw(texture, position - Main.screenPosition, frame, drawColor, rotation + MathHelper.Pi / 3f * direction, frame.Size() * 0.5f, squish * drawScale * 0.66f, flip, 0);

        Main.pixelShader.CurrentTechnique.Passes[0].Apply();
    }
}
