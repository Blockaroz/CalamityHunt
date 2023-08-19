using CalamityHunt.Common.Systems.Particles;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;

namespace CalamityHunt.Content.Particles
{
    public class MicroShockwave : Particle
    {
        public float curScale;
        public Color secondColor;

        public override void OnSpawn()
        {
            rotation = velocity.ToRotation();
            velocity = Vector2.Zero;
        }

        public override void Update()
        {
            curScale += (scale - curScale * 0.8f) * 0.09f;
            if (curScale > scale)
                Active = false;

            if (data is Color newSecondColor)
                secondColor = newSecondColor;
            else
                secondColor = Color.White;
        }

        private static Texture2D texture;

        public override void Load()
        {
            texture = new TextureAsset(Texture);
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            Rectangle solidFrame = texture.Frame(1, 3, 0, 0);
            Rectangle colorFrame = texture.Frame(1, 3, 0, 1);
            Rectangle glowFrame = texture.Frame(1, 3, 0, 2);
            float power = Utils.GetLerpValue(scale, scale * 0.7f, curScale, true);
            spriteBatch.Draw(texture, position - Main.screenPosition, solidFrame, Color.Black * 0.1f * power, rotation, solidFrame.Size() * 0.5f, new Vector2(curScale, curScale * 0.5f), 0, 0);
            spriteBatch.Draw(texture, position - Main.screenPosition, colorFrame, color * power, rotation, colorFrame.Size() * 0.5f, new Vector2(curScale, curScale * 0.5f), 0, 0);
            spriteBatch.Draw(texture, position - Main.screenPosition, glowFrame, secondColor * power, rotation, glowFrame.Size() * 0.5f, new Vector2(curScale, curScale * 0.5f), 0, 0);
        }
    }
}
