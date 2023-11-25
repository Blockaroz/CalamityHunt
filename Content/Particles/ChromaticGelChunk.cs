using System;
using CalamityHunt.Common.Utilities;
using CalamityHunt.Content.Bosses.Goozma;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;

namespace CalamityHunt.Content.Particles;

public class ChromaticGelChunk : BaseGelChunk
{
    public ColorOffsetData colorData;

    public override void Draw(SpriteBatch spriteBatch)
    {
        Texture2D texture = AssetDirectory.Textures.Particle[Type].Value;
        Rectangle frame = texture.Frame(4, 2, style, 0);
        Rectangle glowFrame = texture.Frame(4, 2, style, 1);
        Vector2 squish = new Vector2(1f - velocity.Length() * 0.01f, 1f + velocity.Length() * 0.01f);
        float grow = (float)Math.Sqrt(Utils.GetLerpValue(-20, 40, time, true));
        if (sticking) {
            grow = 1f;
            frame = texture.Frame(4, 2, style + 2, 0);
            glowFrame = texture.Frame(4, 2, style + 2, 1);
            squish = new Vector2(1f + (float)Math.Sqrt(Utils.GetLerpValue(20, 0, time, true)) * 0.33f, 1f - (float)Math.Sqrt(Utils.GetLerpValue(20, 0, time, true)) * 0.33f);
        }
        Color glowColor = new GradientColor(SlimeUtils.GoozColors, 0.2f, 0.2f).ValueAt(time * 2f + colorData.offset);
        glowColor.A = 0;
        Color lightColor = Lighting.GetColor(position.ToTileCoordinates());

        spriteBatch.Draw(texture, position - Main.screenPosition, frame, Color.Lerp(color, color.MultiplyRGBA(lightColor), sticking ? 1f : Utils.GetLerpValue(10, 50, time, true)) * Utils.GetLerpValue(10, 40, time, true), rotation, frame.Size() * new Vector2(0.5f, 0.84f), scale * grow * squish, 0, 0);
        spriteBatch.Draw(texture, position - Main.screenPosition, glowFrame, glowColor, rotation, frame.Size() * new Vector2(0.5f, 0.84f), scale * grow * squish, 0, 0);
        spriteBatch.Draw(texture, position - Main.screenPosition, glowFrame, glowColor * 0.5f, rotation, frame.Size() * new Vector2(0.5f, 0.84f), scale * 1.05f * grow * squish, 0, 0);
    }
}
