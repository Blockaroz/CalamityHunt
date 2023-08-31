using Microsoft.Xna.Framework;
using Terraria;

namespace CalamityHunt.Content.Particles.FlyingSlimes
{
    public class FlyingGastropodParticleBehavior : FlyingSlimeParticleBehavior
    {
        public override float SlimeSpeed => 30f;

        public override void PostUpdate()
        {            
            Lighting.AddLight(position + velocity, Color.Pink.ToVector3());
        }
    }
}
