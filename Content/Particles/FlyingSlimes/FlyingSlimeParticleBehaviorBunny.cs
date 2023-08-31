using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using Terraria;
using Terraria.ModLoader;

namespace CalamityHunt.Content.Particles.FlyingSlimes
{
    public class FlyingSlimeParticleBehaviorBunny : FlyingSlimeParticleBehavior
    {
        public override bool ShouldDraw => false;
        public override void KillEffect()
        {
            Gore.NewGore(Entity.GetSource_None(), position, Main.rand.NextVector2Circular(1, 1), 440);
        }

        public override void PostUpdate()
        {
            if (Main.rand.NextBool(10))
            {
                Dust slime = Dust.NewDustPerfect(position + Main.rand.NextVector2Circular(20, 20), 306, velocity * 0.2f, 128, color, 0.5f + Main.rand.NextFloat() * 0.3f);
                slime.fadeIn = 0.9f;
                slime.color = Main.hslToRgb(((float)Main.timeForVisualEffects / 300f + Main.rand.NextFloat() * 0.1f) % 1f, 1f, 0.65f, 0);
                slime.noLightEmittence = true;
                slime.noGravity = true;
            }

            Lighting.AddLight(position + velocity, Main.hslToRgb(((float)Main.timeForVisualEffects / 300f + Main.rand.NextFloat() * 0.1f) % 1f, 1f, 0.65f, 0).ToVector3() * 0.1f);
        }

        public override void DrawSlime(SpriteBatch spriteBatch)
        {
            Asset<Texture2D> texture = ModContent.Request<Texture2D>(Texture);
            float fadeIn = Utils.GetLerpValue(0, 30, time, true) * distanceFade;

            for (int i = 0; i < 10; i++)
                spriteBatch.Draw(texture.Value, position - velocity * i * 0.6f - Main.screenPosition, null, color.MultiplyRGBA(Lighting.GetColor(position.ToTileCoordinates())) * 0.78f * fadeIn * ((10f - i) / 100f), rotation + MathHelper.PiOver2, texture.Size() * 0.5f, scale * 1.1f * distanceFade * 1.05f, 0, 0);

            spriteBatch.Draw(texture.Value, position - Main.screenPosition, null, color.MultiplyRGBA(Lighting.GetColor(position.ToTileCoordinates())) * 0.78f * fadeIn, rotation + MathHelper.PiOver2, texture.Size() * 0.5f, scale * 1.1f * distanceFade, 0, 0);
        }
    }
}
