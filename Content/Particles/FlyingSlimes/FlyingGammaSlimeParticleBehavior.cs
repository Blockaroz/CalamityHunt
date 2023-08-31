using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;

namespace CalamityHunt.Content.Particles.FlyingSlimes
{
    public class FlyingGammaSlimeParticleBehavior : FlyingSlimeParticleBehavior
    {
        public override void PostUpdate()
        {
            Lighting.AddLight(position + velocity, Color.LightGreen.ToVector3());

            if (Main.rand.NextBool(3))
            {
                Dust slime = Dust.NewDustPerfect(position + Main.rand.NextVector2Circular(20, 20), DustID.CursedTorch, velocity * 0.5f, 200, color, 0.5f + Main.rand.NextFloat());
                slime.noGravity = true;
            }
        }
    }
}
