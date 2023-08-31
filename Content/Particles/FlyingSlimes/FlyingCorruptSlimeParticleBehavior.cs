using CalamityHunt.Common.Systems.Particles;
using Terraria;
using Terraria.ID;

namespace CalamityHunt.Content.Particles.FlyingSlimes
{
    public class FlyingCorruptSlimeParticleBehavior : FlyingSlimeParticleBehavior
    {
        public override float SlimeSpeed => 25f;
        public override void PostUpdate()
        {
            if (Main.rand.NextBool(5))
            {
                Dust slime = Dust.NewDustPerfect(position + Main.rand.NextVector2Circular(20, 20), DustID.Corruption, velocity * 0.2f, 200, color, 0.5f + Main.rand.NextFloat());
                slime.noGravity = true;
            }
        }
    }
}
