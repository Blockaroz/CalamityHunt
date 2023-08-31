using Terraria;
using Terraria.ID;

namespace CalamityHunt.Content.Particles.FlyingSlimes
{
    public class FlyingCragmawMireParticleBehavior : FlyingSlimeParticleBehavior
    {
        public override float SlimeSpeed => 30f;

        public override void OnSpawn()
        {
            scale *= Main.rand.NextFloat(0.9f, 1.1f);
        }

        public override void PostUpdate()
        {
            if (Main.rand.NextBool(3))
            {
                Dust slime = Dust.NewDustPerfect(position + Main.rand.NextVector2Circular(20, 20), DustID.CursedTorch, velocity * 0.5f, 200, color, 0.5f + Main.rand.NextFloat());
                slime.noGravity = true;
            }
        }
    }
}
