using Arch.Core.Extensions;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using Terraria;
using Terraria.ModLoader;

namespace CalamityHunt.Content.Particles.FlyingSlimes
{
    public struct ParticleFlyingPresentSlime
    {
        public int Variant { get; set; }
    }
    
    public class FlyingPresentSlimeParticleBehavior : FlyingSlimeParticleBehavior
    {
        public override float SlimeSpeed => 25f;
        public override bool ShouldDraw => false;

        public override void OnSpawn(in Arch.Core.Entity entity)
        {
            base.OnSpawn(in entity);

            entity.Add(new ParticleFlyingPresentSlime { Variant = Main.rand.Next(4) });
        }

        public override void KillEffect()
        {
            Gore.NewGore(Entity.GetSource_None(), position, Main.rand.NextVector2Circular(1, 1), 76);
            Gore.NewGore(Entity.GetSource_None(), position, Main.rand.NextVector2Circular(1, 1), 77);
        }

        public override void DrawSlime(SpriteBatch spriteBatch)
        {
            Asset<Texture2D> texture = ModContent.Request<Texture2D>(Texture);
            Rectangle frame = texture.Frame(4, 1, variant, 0);
            float fadeIn = Utils.GetLerpValue(0, 30, time, true) * distanceFade;

            for (int i = 0; i < 10; i++)
                spriteBatch.Draw(texture.Value, position - velocity * i * 0.6f - Main.screenPosition, frame, color.MultiplyRGBA(Lighting.GetColor(position.ToTileCoordinates())) * 0.78f * fadeIn * ((10f - i) / 100f), rotation + MathHelper.PiOver2, frame.Size() * 0.5f, scale * 1.1f * distanceFade * 1.05f, 0, 0);

            spriteBatch.Draw(texture.Value, position - Main.screenPosition, frame, color.MultiplyRGBA(Lighting.GetColor(position.ToTileCoordinates())) * 0.78f * fadeIn, rotation + MathHelper.PiOver2, frame.Size() * 0.5f, scale * 1.1f * distanceFade, 0, 0);
        }
    }
}
