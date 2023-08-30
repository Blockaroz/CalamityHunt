using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.Graphics.Shaders;
using Terraria.ModLoader;

namespace CalamityHunt.Common.Systems.Particles
{
    public abstract class Particle : ModTexturedType
    {
        public Vector2 position;

        public Vector2 velocity;

        public float rotation;

        public float scale;

        public Color color;

        public bool emit;

        public object data;

        public bool behindEntities;

        public ArmorShaderData shader;

        public bool Active { get; set; }

        public virtual void Update()
        {
        }

        public virtual void Draw(SpriteBatch spriteBatch)
        {
        }

        public virtual void OnSpawn()
        {
        }

        protected sealed override void Register()
        {
            ModTypeLookup<Particle>.Register(this);
            ParticleSystem.particleTypes.Add(this);
        }

        public sealed override void SetupContent()
        {
            SetStaticDefaults();
        }

        public static Particle NewParticle(Particle particle, Vector2 position, Vector2 velocity, Color color, float scale = 1f)
        {
            if (!Main.gamePaused && !Main.dedServ)
            {
                particle = (Particle)particle.MemberwiseClone();
                particle.position = position;
                particle.velocity = velocity;
                particle.color = color;
                particle.scale = scale;
                particle.rotation = velocity.ToRotation() + Main.rand.NextFloat(-0.2f, 0.2f) * MathHelper.TwoPi;
                particle.Active = true;
                particle.OnSpawn();
                ParticleSystem.particle.Add(particle);
                return particle;
            }

            return null;
        }
    }
}
