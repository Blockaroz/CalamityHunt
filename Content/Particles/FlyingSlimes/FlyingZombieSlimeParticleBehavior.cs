using Microsoft.Xna.Framework;
using Terraria;

namespace CalamityHunt.Content.Particles.FlyingSlimes
{
    public class FlyingZombieSlimeParticleBehavior : FlyingSlimeParticleBehavior
    {
        public override float SlimeSpeed => 30f;
        public override bool ShouldRotate => false;

        public override void KillEffect()
        {
            Gore.NewGore(Entity.GetSource_None(), position, Main.rand.NextVector2Circular(1, 1), 4);
            Gore.NewGore(Entity.GetSource_None(), position, Main.rand.NextVector2Circular(1, 1), 5);
        }

        public override void PostUpdate()
        {
            Lighting.AddLight(position + velocity, Color.Pink.ToVector3());
        }
    }
}
