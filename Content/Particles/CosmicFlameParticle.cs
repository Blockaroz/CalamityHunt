using System;
using CalamityHunt.Common.Systems.Particles;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.Map;
using Terraria.ModLoader;

namespace CalamityHunt.Content.Particles;

public class CosmicFlameParticle : Particle
{
    private int time;

    public int maxTime;

    private int style;

    private int direction;

    private float rotationalVelocity;

    public Func<Vector2> anchor;

    public override void OnSpawn()
    {
        style = Main.rand.Next(5);
        direction = Main.rand.NextBool().ToDirectionInt();
        scale *= Main.rand.NextFloat(0.9f, 1.1f);
        rotationalVelocity = Main.rand.NextFloat(0.02f, 0.15f);
    }

    public override void Update()
    {
        float progress = time / (maxTime * 2f);

        velocity *= 1f - progress * 0.2f;

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
        float progress = time / (maxTime * 2f);

        Texture2D texture = AssetDirectory.Textures.Particle[Type].Value;
        Rectangle frame = texture.Frame(1, 10, 0, style);
        SpriteEffects flip = direction > 0 ? SpriteEffects.None : SpriteEffects.FlipVertically;
        float drawScale = scale * MathF.Sqrt(Utils.GetLerpValue(0f, 3f, time, true)) * (0.6f + progress);

        Effect dissolveEffect = AssetDirectory.Effects.FlameDissolve.Value;
        dissolveEffect.Parameters["uTexture0"].SetValue(AssetDirectory.Textures.Noise[9].Value);
        dissolveEffect.Parameters["uTextureScale"].SetValue(new Vector2(0.5f));
        dissolveEffect.Parameters["uFrameCount"].SetValue(10);
        dissolveEffect.Parameters["uProgress"].SetValue(1f - MathF.Sqrt(1f - progress));
        dissolveEffect.Parameters["uPower"].SetValue(10f + progress * 70f);
        dissolveEffect.Parameters["uNoiseStrength"].SetValue(0.8f);
        dissolveEffect.CurrentTechnique.Passes[0].Apply();

        Vector2 squish = new Vector2(1f - progress * 0.4f, 1f + progress * 0.4f);
        spriteBatch.Draw(texture, position - Main.screenPosition, frame, color, rotation + MathHelper.Pi / 3f * direction, frame.Size() * 0.5f, squish * drawScale * 0.5f, flip, 0);

        Main.pixelShader.CurrentTechnique.Passes[0].Apply();
    }
}
