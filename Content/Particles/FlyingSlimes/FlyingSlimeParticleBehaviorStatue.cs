using Microsoft.Xna.Framework;
using Terraria;

namespace CalamityHunt.Content.Particles.FlyingSlimes
{
    public class FlyingSlimeParticleBehaviorStatue : FlyingSlimeParticleBehavior
    {
        public override float SlimeSpeed => 16f;
        public override bool ShouldRotate => false;

        public override void OnSpawn()
        {
            scale = 1;
        }

        public override void PostUpdate()
        {
            Lighting.AddLight(position + velocity, Color.Pink.ToVector3());
            rotation += 0.1f;
        }
    }
}
