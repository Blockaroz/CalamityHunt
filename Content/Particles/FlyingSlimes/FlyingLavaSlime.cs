using CalamityHunt.Common.Systems.Particles;
using CalamityHunt.Content.Projectiles;
using Terraria.Audio;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using Terraria;
using Terraria.ModLoader;

namespace CalamityHunt.Content.Particles.FlyingSlimes
{
    public class FlyingLavaSlime : FlyingSlime
    {
        public override float SlimeSpeed => 30f;
        public override bool ShouldDraw => false;

        public override void PostUpdate()
        {
            Lighting.AddLight(position + velocity, Color.Orange.ToVector3());
        }

        public override void DrawSlime(SpriteBatch spriteBatch)
        {
            Asset<Texture2D> texture = ModContent.Request<Texture2D>(Texture);

            float fadeIn = Utils.GetLerpValue(0, 30, time, true) * distanceFade;

            Color drawColor = color;
            drawColor.A /= 2;

            for (int i = 0; i < 10; i++)
                spriteBatch.Draw(texture.Value, position - velocity * i * 0.5f - Main.screenPosition, null, drawColor.MultiplyRGBA(Lighting.GetColor(position.ToTileCoordinates())) * fadeIn * ((10f - i) / 60f), rotation + MathHelper.PiOver2, texture.Size() * 0.5f, scale * distanceFade, 0, 0);

            spriteBatch.Draw(texture.Value, position - Main.screenPosition, null, drawColor.MultiplyRGBA(Lighting.GetColor(position.ToTileCoordinates())) * fadeIn, rotation + MathHelper.PiOver2, texture.Size() * 0.5f, scale * distanceFade, 0, 0);
        }
    }
}
