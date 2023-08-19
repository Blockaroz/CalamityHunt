using Terraria;
using Microsoft.Xna.Framework;
namespace CalamityHunt.Content.Particles.FlyingSlimes
{
    public class FlyingNovaSlimer : FlyingSlime
    {
        public override bool ShouldRotate => false;
        public override void OnSpawn()
        {
            scale = 1;
        }

        public override void PostUpdate()
        {
            Lighting.AddLight(position + velocity, Color.Purple.ToVector3());
            rotation += 0.1f;
        }
    }
}
