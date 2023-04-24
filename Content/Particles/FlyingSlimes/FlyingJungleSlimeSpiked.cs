using Terraria;
using Terraria.ID;

namespace CalamityHunt.Content.Particles.FlyingSlimes
{
    public class FlyingJungleSlimeSpiked : FlyingSlime
    {
        public override void PostUpdate()
        {
            if (Main.rand.NextBool(150))
            {
                Dust slime = Dust.NewDustPerfect(position + Main.rand.NextVector2Circular(20, 20), 306, velocity * 0.2f, DustID.JungleSpore, color, 0.5f + Main.rand.NextFloat() * 0.3f);
                slime.noGravity = true;
            }
        }
    }
}
