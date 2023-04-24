using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using Terraria;
using Terraria.ModLoader;

namespace CalamityHunt.Content.Particles.FlyingSlimes
{
    public class AureusSpawn : FlyingSlime
    {
        public int currentFrame = 0;
        public float frameCounter = 0;
        public override float SlimeSpeed => 16;
        public override bool ShouldDraw => false;

        public override void PostUpdate()
        {
            Lighting.AddLight(position, 0.6f * distanceFade, 0.25f * distanceFade, 0f);

            frameCounter += 0.22f;
            frameCounter %= 4;
            int frame = (int)frameCounter;
            currentFrame = frame * 62;
        }

        public override void DrawSlime(SpriteBatch spriteBatch)
        { 
            // 92 by 62
            Asset<Texture2D> texture = ModContent.Request<Texture2D>(Texture);
            Asset<Texture2D> glow = ModContent.Request<Texture2D>(Texture + "Glow");
            Rectangle frame = new(0, currentFrame, 92, 62);
            float fadeIn = Utils.GetLerpValue(0, 30, time, true) * distanceFade;

            for (int i = 0; i < 10; i++)

                spriteBatch.Draw(texture.Value, position - velocity * i * 0.5f - Main.screenPosition, frame, color.MultiplyRGBA(Lighting.GetColor(position.ToTileCoordinates())) * fadeIn * ((10f - i) / 80f), rotation + MathHelper.PiOver2, frame.Size() * 0.5f, scale * distanceFade * 1.05f, 0, 0);

            spriteBatch.Draw(texture.Value, position - Main.screenPosition, frame, color.MultiplyRGBA(Lighting.GetColor(position.ToTileCoordinates())) * fadeIn, rotation + MathHelper.PiOver2, frame.Size() * 0.5f, scale * distanceFade, 0, 0);
            spriteBatch.Draw(glow.Value, position - Main.screenPosition, frame, Color.White * fadeIn, rotation + MathHelper.PiOver2, frame.Size() * 0.5f, scale * distanceFade, 0, 0);
        }
    }
}
