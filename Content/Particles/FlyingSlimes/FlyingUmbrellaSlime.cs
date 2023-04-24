using Terraria;

namespace CalamityHunt.Content.Particles.FlyingSlimes
{
    public class FlyingUmbrellaSlime : FlyingSlime
    {
        public override float SlimeSpeed => 7f;
        public override float SlimeAcceleration => 0.2f;
        public override void KillEffect()
        {
            Gore.NewGore(Entity.GetSource_None(), position, Main.rand.NextVector2Circular(1, 1), 314);
        }
    }
}
