using System;
using CalamityHunt.Common.Systems.Particles;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.Map;
using Terraria.ModLoader;

namespace CalamityHunt.Content.Particles;

public class CosmicFlame : Particle
{
    private int time;

    public int maxTime;

    private int style;

    private int direction;

    public Func<Vector2> anchor;

    public override void OnSpawn()
    {
        style = Main.rand.Next(5);
        direction = Main.rand.NextBool().ToDirectionInt();
        scale *= Main.rand.NextFloat(0.9f, 1.1f);
    }

    public override void Update()
    {
        velocity *= 0.85f;

        if (time > maxTime / 2f) {
            scale *= 0.95f;

        }if (time++ > maxTime) {
            ShouldRemove = true;
        }

        if (anchor != null) {
            position += anchor.Invoke();
        }

        rotation -= (1f - time / maxTime) * 0.12f * direction;
    }

    public override void Draw(SpriteBatch spriteBatch)
    {
        Texture2D texture = AssetDirectory.Textures.Particle[Type].Value;
        Rectangle frame = texture.Frame(1, 5, 0, style);
        SpriteEffects flip = direction > 0 ? SpriteEffects.None : SpriteEffects.FlipVertically;
        Color drawColor = color * (1f - MathF.Pow(Utils.GetLerpValue(maxTime / 2f, maxTime, time, true), 2f));
        float drawScale = scale * Utils.GetLerpValue(-maxTime / 3f, maxTime / 5f, time, true);
        spriteBatch.Draw(texture, position - Main.screenPosition, frame, drawColor, rotation + MathHelper.Pi / 3f * direction, frame.Size() * 0.5f, drawScale * (1f + (float)time / maxTime * 0.1f), flip, 0);
    }
}
