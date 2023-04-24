namespace CalamityHunt.Content.Particles.FlyingSlimes
{
    public class FlyingFirstEncounter : FlyingSlime
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
