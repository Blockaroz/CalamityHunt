using Microsoft.Xna.Framework;
using Terraria;

namespace CalamityHunt.Content.Particles.FlyingSlimes
{
    public class FlyingGoldSlime : FlyingSlime
    {
        public override float SlimeSpeed => 30f;

        public override void PostUpdate()
        {
            if (Main.rand.NextBool(2))
            {
                Dust slime = Dust.NewDustPerfect(position + Main.rand.NextVector2Circular(20, 20), 246, velocity * 0.2f, 200, color, 1.5f);
                slime.noLightEmittence = true;
                slime.noGravity = true;
            }

            Lighting.AddLight(position + velocity, new Color(204, 181, 72, 255).ToVector3());
        }
    }
}
