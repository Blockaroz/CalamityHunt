using System;
using CalamityHunt.Common.Systems.Particles;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;

namespace CalamityHunt.Content.Particles;

public class LightningParticle : Particle
{
    private int time;

    public int maxTime;

    private int style;

    private int direction;

    public Func<Vector2> anchor;

    public float flickerSpeed;

    public override void OnSpawn()
    {
        style = Main.rand.Next(10);
        direction = Main.rand.NextBool().ToDirectionInt();
        maxTime = maxTime <= 0 ? Main.rand.Next(3, 6) : maxTime;
        maxTime += 5;
    }

    public override void Update()
    {
        velocity *= 0.98f;

        if (time++ > maxTime) {
            ShouldRemove = true;
        }

        if (anchor != null) {
            position += anchor.Invoke();
        }
    }

    public override void Draw(SpriteBatch spriteBatch)
    {
        Texture2D texture = AssetDirectory.Textures.Particle[Type].Value;
        Rectangle frame = texture.Frame(1, 10, 0, style);
        SpriteEffects flip = direction > 0 ? SpriteEffects.None : SpriteEffects.FlipVertically;
        Color drawColor = color * Utils.GetLerpValue(maxTime, maxTime / 2f, time, true) * (0.8f + MathF.Sin(time * flickerSpeed) * 0.2f);

        Effect dissolveEffect = AssetDirectory.Effects.FlameDissolve.Value;
        dissolveEffect.Parameters["uTexture0"].SetValue(AssetDirectory.Textures.Noise[10].Value);
        dissolveEffect.Parameters["uTextureScale"].SetValue(new Vector2(2f + scale * 0.01f));
        dissolveEffect.Parameters["uFrameCount"].SetValue(10);
        dissolveEffect.Parameters["uProgress"].SetValue(Utils.GetLerpValue(maxTime / 3f, maxTime, time, true) * 0.5f);
        dissolveEffect.Parameters["uPower"].SetValue(1f + Utils.GetLerpValue(maxTime / 4f, maxTime / 2f, time, true) * 15f);
        dissolveEffect.Parameters["uNoiseStrength"].SetValue(0.5f);
        dissolveEffect.CurrentTechnique.Passes[0].Apply();

        spriteBatch.Draw(texture, position - Main.screenPosition, frame, drawColor, rotation + MathHelper.Pi / 3f * direction, frame.Size() * 0.5f, scale * new Vector2(1f, 1f + time * 0.05f), flip, 0);

        Main.pixelShader.CurrentTechnique.Passes[0].Apply();
    }
}
