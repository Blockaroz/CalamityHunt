using System;
using CalamityHunt.Common.Systems.Particles;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;

namespace CalamityHunt.Content.Particles;

public class StraightLightningParticle : Particle
{
    private int time;

    public int maxTime;

    private int style;

    private int direction;

    public Func<Vector2> anchor;

    public float stretch;

    public float flickerSpeed;

    public override void OnSpawn()
    {
        style = Main.rand.Next(10);
        direction = Main.rand.NextBool().ToDirectionInt();
        maxTime = maxTime <= 0 ? Main.rand.Next(3, 6) : maxTime;
    }

    public override void Update()
    {
        velocity *= 0.97f;

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
        Vector2 drawScale = scale * new Vector2(stretch, 1f - (float)time / maxTime * 0.2f);
        Color drawColor = color * Utils.GetLerpValue(maxTime, maxTime / 3f, time, true) * (0.6f + MathF.Sin(time * flickerSpeed) * 0.4f);
        spriteBatch.Draw(texture, position - Main.screenPosition, frame, drawColor, rotation, frame.Size() * 0.5f, drawScale, flip, 0);
    }
}
