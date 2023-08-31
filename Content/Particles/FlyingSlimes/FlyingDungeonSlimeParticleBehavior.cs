using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using Terraria;
using Terraria.ModLoader;

namespace CalamityHunt.Content.Particles.FlyingSlimes
{
    public class FlyingDungeonSlimeParticleBehavior : FlyingSlimeParticleBehavior
    {
        public override float SlimeSpeed => 7f;
        public override float SlimeAcceleration => 0.2f;
        public override bool ShouldDraw => false;

        public override void DrawSlime(SpriteBatch spriteBatch)
        {
            Asset<Texture2D> texture = ModContent.Request<Texture2D>(Texture);
            Asset<Texture2D> keyTexture = ModContent.Request<Texture2D>(Texture + "Key");
            float fadeIn = Utils.GetLerpValue(0, 30, time, true) * distanceFade;

            for (int i = 0; i < 10; i++)
                spriteBatch.Draw(texture.Value, position - velocity * i * 0.6f - Main.screenPosition, null, color.MultiplyRGBA(Lighting.GetColor(position.ToTileCoordinates())) * 0.765f * fadeIn * ((10f - i) / 100f), rotation + MathHelper.PiOver2, texture.Size() * 0.5f, scale * 1.25f * distanceFade * 1.05f, 0, 0);

            spriteBatch.Draw(keyTexture.Value, position + new Vector2(texture.Height() / 4f * scale * distanceFade, 0).RotatedBy(rotation) - Main.screenPosition, null, Lighting.GetColor(position.ToTileCoordinates()) * fadeIn, rotation + MathHelper.PiOver2, keyTexture.Size() * 0.5f, scale * 1.25f * distanceFade, 0, 0);
            spriteBatch.Draw(texture.Value, position - Main.screenPosition, null, color.MultiplyRGBA(Lighting.GetColor(position.ToTileCoordinates())) * 0.765f * fadeIn, rotation + MathHelper.PiOver2, texture.Size() * 0.5f, scale * 1.25f * distanceFade, 0, 0);
        }
    }
}
