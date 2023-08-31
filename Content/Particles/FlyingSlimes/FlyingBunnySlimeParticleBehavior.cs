using Terraria;

namespace CalamityHunt.Content.Particles.FlyingSlimes
{
    public class FlyingBunnySlimeParticleBehavior : FlyingSlimeParticleBehavior
    {
        public override float SlimeSpeed => 25f;
        public override void KillEffect()
        {
            Gore.NewGore(Entity.GetSource_None(), position, Main.rand.NextVector2Circular(1, 1), 76);
            Gore.NewGore(Entity.GetSource_None(), position, Main.rand.NextVector2Circular(1, 1), 77);
        }
    }
}
