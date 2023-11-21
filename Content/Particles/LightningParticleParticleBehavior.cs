using System;
using Arch.Core.Extensions;
using CalamityHunt.Common.Systems.Particles;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Entity = Arch.Core.Entity;

namespace CalamityHunt.Content.Particles;

public struct ParticleLightningParticle
{
    public float Time { get; set; }

    public float MaxTime { get; set; }

    public int Variant { get; set; }

    public int Direction { get; set; }
}

public class LightningParticleParticleBehavior : ParticleBehavior
{
    public override void OnSpawn(in Entity entity)
    {
        var portal = new ParticleLightningParticle
        { 
            Variant = Main.rand.Next(5),
            Direction = Main.rand.NextBool().ToDirectionInt(),
            MaxTime = Main.rand.Next(3, 6)
        };
        entity.Add(portal);
    }

    public override void Update(in Entity entity)
    {
        ref var particle = ref entity.Get<ParticleLightningParticle>();
        ref var position = ref entity.Get<ParticlePosition>();
        ref var active = ref entity.Get<ParticleActive>();

        if (particle.Time++ > particle.MaxTime) {
            active.Value = false;
        }

        if (entity.TryGet<ParticleData<Func<Vector2>>>(out var data)) {
            position.Value += data.Value.Invoke();
        }
    }

    public override void Draw(in Entity entity, SpriteBatch spriteBatch)
    {
        ref var particle = ref entity.Get<ParticleLightningParticle>();
        ref var position = ref entity.Get<ParticlePosition>();
        ref var rotation = ref entity.Get<ParticleRotation>();
        ref var scale = ref entity.Get<ParticleScale>();
        ref var color = ref entity.Get<ParticleColor>();

        Texture2D texture = AssetDirectory.Textures.Particle[Type].Value;
        Rectangle frame = texture.Frame(1, 5, 0, particle.Variant);
        SpriteEffects flip = particle.Direction > 0 ? SpriteEffects.None : SpriteEffects.FlipVertically;
        spriteBatch.Draw(texture, position.Value - Main.screenPosition, frame, color.Value * Utils.GetLerpValue(1, particle.MaxTime, particle.Time, true), rotation.Value + MathHelper.Pi / 3f * particle.Direction, frame.Size() * 0.5f, scale.Value * new Vector2(1f, 1f + particle.Time * 0.05f), flip, 0);
    }
}
