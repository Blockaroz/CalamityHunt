using CalamityHunt.Common.Systems.Particles;
using CalamityHunt.Content.Projectiles;
using Terraria.Audio;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using Terraria;
using Terraria.ModLoader;

namespace CalamityHunt.Content.Particles.FlyingSlimes
{
    public class FlyingSlimeParticleBehaviorFish : FlyingSlimeParticleBehavior
    {
        public override float SlimeSpeed => 35f;
    }
}
