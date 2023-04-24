using Microsoft.Xna.Framework;
using Terraria;

namespace CalamityHunt.Content.Particles.FlyingSlimes
{
    public class FlyingYuH : FlyingSlime
    {
        public override float SlimeSpeed => 35f;

        public override void PostUpdate()
        {
            Lighting.AddLight(position + velocity, Color.DarkCyan.ToVector3() * 0.2f);
        }
    }
}
