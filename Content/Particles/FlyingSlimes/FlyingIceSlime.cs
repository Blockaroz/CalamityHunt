using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityHunt.Content.Particles.FlyingSlimes
{
    public struct ParticleFlyingIceSlime
    {
        public bool Spiked { get; set; }
    }
    
    public class FlyingIceSlime : FlyingSlime
    {
        public override bool ShouldDraw => false;

        public override void OnSpawn()
        {
            spiked = Main.rand.NextBool();
        }

        public override void PostUpdate()
        {
            if (Main.rand.NextBool(10))
            {
                Dust slime = Dust.NewDustPerfect(position + Main.rand.NextVector2Circular(20, 20), 306, velocity * 0.2f, DustID.IceTorch, color, 0.5f + Main.rand.NextFloat() * 0.3f);
                slime.noGravity = true;
            }
        }

        public override void DrawSlime(SpriteBatch spriteBatch)
        {
            Asset<Texture2D> texture = ModContent.Request<Texture2D>(Texture);
            if (spiked)
                texture = ModContent.Request<Texture2D>(Texture + "Spiked");

            float fadeIn = Utils.GetLerpValue(0, 30, time, true) * distanceFade;

            for (int i = 0; i < 10; i++)
                spriteBatch.Draw(texture.Value, position - velocity * i * 0.5f - Main.screenPosition, null, color.MultiplyRGBA(Lighting.GetColor(position.ToTileCoordinates())) * fadeIn * ((10f - i) / 80f), rotation + MathHelper.PiOver2, texture.Size() * 0.5f, scale * distanceFade * 1.05f, 0, 0);

            spriteBatch.Draw(texture.Value, position - Main.screenPosition, null, color.MultiplyRGBA(Lighting.GetColor(position.ToTileCoordinates())) * fadeIn, rotation + MathHelper.PiOver2, texture.Size() * 0.5f, scale * distanceFade, 0, 0);
        }
    }
}
