using CalamityHunt.Common.Systems.Particles;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using System;
using Terraria;
using Terraria.ModLoader;

namespace CalamityHunt.Content.Particles
{
    public class PrettySparkle : ParticleBehavior
    {
        private int time;

        public override void OnSpawn()
        {
            scale *= Main.rand.NextFloat(0.9f, 1.1f);
            velocity *= Main.rand.NextFloat(0.9f, 1.1f);
            rotation *= 0.05f;
        }

        public override void Update()
        {
            velocity *= 0.95f;
            time++;

            if (time > 40 + scale)
                scale *= 0.8f + Math.Min(scale * 0.2f, 0.18f);

            if (scale < 0.1f)
                Active = false;
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            Asset<Texture2D> texture = ModContent.Request<Texture2D>(Texture);

            float drawScale = scale * (float)Math.Sqrt(Utils.GetLerpValue(-5, 10, time, true));
            Color drawColor = Color.Lerp(color, Color.White, 0.6f);
            drawColor.A = 0;
            spriteBatch.Draw(texture.Value, position - Main.screenPosition, null, new Color(color.R, color.G, color.B, color.A / 2), rotation, texture.Size() * 0.5f, drawScale * 0.6f, 0, 0);
            spriteBatch.Draw(texture.Value, position - Main.screenPosition, null, drawColor, rotation, texture.Size() * 0.5f, drawScale * 0.3f, 0, 0);
        }
    }
}
