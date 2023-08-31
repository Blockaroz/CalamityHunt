using CalamityHunt.Common.Systems.Particles;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using System;
using Arch.Core.Extensions;
using Terraria;
using Terraria.ModLoader;
using Entity = Arch.Core.Entity;

namespace CalamityHunt.Content.Particles;

public struct ParticlePrettySparkle
{
    public int Time { get; set; }
}
    
public class PrettySparkle : ParticleBehavior
{
    public override void OnSpawn(in Entity entity)
    {
        ref var scale = ref entity.Get<ParticleScale>();
        ref var velocity = ref entity.Get<ParticleVelocity>();
        ref var rotation = ref entity.Get<ParticleRotation>();
        scale.Value *= Main.rand.NextFloat(0.9f, 1.1f);
        velocity.Value *= Main.rand.NextFloat(0.9f, 1.1f);
        rotation.Value *= 0.05f;

        entity.Add(new ParticlePrettySparkle());
    }

    public override void Update(in Entity entity)
    {
        velocity *= 0.95f;
        time++;

        if (time > 40 + scale)
            scale *= 0.8f + Math.Min(scale * 0.2f, 0.18f);

        if (scale < 0.1f)
            Active = false;
    }

    public override void Draw(in Entity entity, SpriteBatch spriteBatch)
    {
        Asset<Texture2D> texture = ModContent.Request<Texture2D>(Texture);

        float drawScale = scale * (float)Math.Sqrt(Utils.GetLerpValue(-5, 10, time, true));
        Color drawColor = Color.Lerp(color, Color.White, 0.6f);
        drawColor.A = 0;
        spriteBatch.Draw(texture.Value, position - Main.screenPosition, null, new Color(color.R, color.G, color.B, color.A / 2), rotation, texture.Size() * 0.5f, drawScale * 0.6f, 0, 0);
        spriteBatch.Draw(texture.Value, position - Main.screenPosition, null, drawColor, rotation, texture.Size() * 0.5f, drawScale * 0.3f, 0, 0);
    }
}
