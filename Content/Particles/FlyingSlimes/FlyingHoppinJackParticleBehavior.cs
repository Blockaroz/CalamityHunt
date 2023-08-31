using Terraria;
using Terraria.ID;

namespace CalamityHunt.Content.Particles.FlyingSlimes
{
    public class FlyingHoppinJackParticleBehavior : FlyingSlimeParticleBehavior
    {
        public override float SlimeSpeed => 16f;
        public override float SlimeAcceleration => 0.2f;
        public override void PostUpdate()
        {
            if (Main.rand.NextBool(8))
            {
                Dust slime = Dust.NewDustPerfect(position + Main.rand.NextVector2Circular(30, 30), DustID.Torch, velocity * 0.2f, 200, color, 0.5f + Main.rand.NextFloat());
                slime.noGravity = true;
            }
        }
    }
}
