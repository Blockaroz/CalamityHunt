using CalamityHunt.Common.Systems.Particles;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using CalamityHunt.Common.Utilities;
using ReLogic.Content;
using Terraria;

namespace CalamityHunt.Content.Particles
{
    public class MicroPortal : Particle
    {
        public Color secondColor;
        public int direction;
        public float time;

        public override void OnSpawn()
        {
            rotation = velocity.ToRotation();
            velocity = Vector2.Zero;
            direction = Main.rand.NextBool().ToDirectionInt();
        }

        public override void Update()
        {
            time += 0.05f / scale;
            if (time > 1f)
                Active = false;

            if (data is Color newSecondColor)
                secondColor = newSecondColor;
            else
                secondColor = Color.White;

            rotation += (1f - time * 0.5f) * 0.2f * direction;
        }

        private static Asset<Texture2D> texture;

        public override void Load()
        {
            texture = AssetUtilities.RequestImmediate<Texture2D>(Texture);
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            Rectangle solidFrame = texture.Value.Frame(1, 3, 0, 0);
            Rectangle colorFrame = texture.Value.Frame(1, 3, 0, 1);
            Rectangle glowFrame = texture.Value.Frame(1, 3, 0, 2);
            float curScale = MathF.Sqrt(Utils.GetLerpValue(0, 0.1f, time, true) * Utils.GetLerpValue(1f, 0.5f, time, true));
            spriteBatch.Draw(texture.Value, position - Main.screenPosition, solidFrame, Color.Black * 0.5f, -rotation * 2f, solidFrame.Size() * 0.5f, scale * 0.9f * curScale * (1f + MathF.Sin(time * 5f) * 0.15f), 0, 0);
            spriteBatch.Draw(texture.Value, position - Main.screenPosition, colorFrame, color * 0.5f, -rotation * 0.7f, colorFrame.Size() * 0.5f, scale * 1.1f * curScale * (1f + MathF.Sin(time * 5f) * 0.1f), 0, 0);
            spriteBatch.Draw(texture.Value, position - Main.screenPosition, solidFrame, Color.Black * 0.5f, rotation * 1.3f, solidFrame.Size() * 0.5f, scale * 0.6f * curScale, 0, 0);

            spriteBatch.Draw(texture.Value, position - Main.screenPosition, colorFrame, color, rotation, colorFrame.Size() * 0.5f, scale * curScale * (1f + MathF.Sin(time * 10f) * 0.05f), 0, 0);
            spriteBatch.Draw(texture.Value, position - Main.screenPosition, glowFrame, secondColor, rotation, glowFrame.Size() * 0.5f, scale * 1.05f * curScale * (1f + MathF.Sin(time * 10f) * 0.05f), 0, 0);
        }
    }
}
