using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using Terraria;
using Terraria.ModLoader;

namespace CalamityHunt.Content.Particles.FlyingSlimes
{
    public class FlyingRainbowSlime : FlyingSlime
    {
        public override float SlimeSpeed => 10f;
        public override bool ShouldDraw => false;

        public override void PostUpdate()
        {
            Lighting.AddLight(position, Main.DiscoColor.ToVector3() * 0.5f);
        }

        public override void DrawSlime(SpriteBatch spriteBatch)
        {
            Asset<Texture2D> texture = ModContent.Request<Texture2D>(Texture);

            float fadeIn = Utils.GetLerpValue(0, 30, time, true) * distanceFade;

            Color drawColor = Main.DiscoColor * 0.7f;
            drawColor.A = 0;

            for (int i = 0; i < 10; i++)
                spriteBatch.Draw(texture.Value, position - velocity * i * 0.5f - Main.screenPosition, null, drawColor * fadeIn * ((10f - i) / 150f), rotation + MathHelper.PiOver2, texture.Size() * 0.5f, scale * distanceFade * 1.05f, 0, 0);

            spriteBatch.Draw(texture.Value, position - Main.screenPosition, null, drawColor * fadeIn, rotation + MathHelper.PiOver2, texture.Size() * 0.5f, scale * distanceFade, 0, 0);
            spriteBatch.Draw(texture.Value, position - Main.screenPosition, null, drawColor * fadeIn, rotation + MathHelper.PiOver2, texture.Size() * 0.5f, scale * 1.01f * distanceFade, 0, 0);
        }
    }
}
