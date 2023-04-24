using Microsoft.Xna.Framework;
using Terraria;

namespace CalamityHunt.Content.Particles.FlyingSlimes
{
    public class FlyingGastropod : FlyingSlime
    {
        public override float SlimeSpeed => 30f;

        public override void PostUpdate()
        {            
            Lighting.AddLight(position + velocity, Color.Pink.ToVector3());
        }
    }
}
