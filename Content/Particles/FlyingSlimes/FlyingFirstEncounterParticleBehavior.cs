namespace CalamityHunt.Content.Particles.FlyingSlimes
{
    public class FlyingFirstEncounterParticleBehavior : FlyingSlimeParticleBehavior
    {
        public override bool ShouldRotate => false;
        public override void OnSpawn()
        {
            scale = 1;
        }

        public override void PostUpdate()
        {
            rotation += 0.1f;
        }
    }
}
