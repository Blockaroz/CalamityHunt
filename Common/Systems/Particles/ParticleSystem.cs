using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using Microsoft.Xna.Framework.Graphics;
using Terraria;

namespace CalamityHunt.Common.Systems.Particles;

public class ParticleSystem
{
    private HashSet<Particle> particles;

    public void Initialize()
    {
        particles = new HashSet<Particle>();
    }

    public void Add(Particle particle) => particles.Add(particle);

    public void Clear() => particles.Clear();

    public void Update()
    {
        if (Main.dedServ) {
            if (particles.Count > 0)
                particles.Clear();
            return;
        }

        foreach (Particle particle in particles.ToHashSet()) {
            if (particle is null) {
                continue;
            }

            particle.Update();
            particle.position += particle.velocity;

            if (particle.ShouldRemove) {
                particles.Remove(particle);
            }
        }
    }

    public void Draw(SpriteBatch spriteBatch, bool begin = true)
    {
        if (Main.dedServ) {
            return;
        }

        if (!begin) {
            spriteBatch.End();
        }

        spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, Main.DefaultSamplerState, DepthStencilState.None, Main.Rasterizer, null, Main.Transform);

        foreach (Particle particle in particles.ToHashSet()) {
            if (particle is null) {
                continue;
            }

            particle.Draw(spriteBatch);
        }

        spriteBatch.End();

        if (!begin) {
            spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, Main.DefaultSamplerState, DepthStencilState.None, Main.Rasterizer, null, Main.Transform);
        }
    }
}
