using CalamityHunt.Common.Systems.Particles;
using Microsoft.Xna.Framework;
using Arch.Core.Extensions;
using Terraria;
using Entity = Arch.Core.Entity;

namespace CalamityHunt.Content.Particles;

public struct ParticleDarkSludgeChunk
{
    public int Variant { get; set; }

    public int Time { get; set; }

    public bool Stuck { get; set; }
}
    
public class DarkSludgeChunk : ParticleBehavior
{
    public override void OnSpawn(in Entity entity)
    {
        ref var scale = ref entity.Get<ParticleScale>();
        scale.Value *= Main.rand.NextFloat(0.7f, 0.9f);
            
        var chunk = new ParticleDarkSludgeChunk
        {
            Variant = Main.rand.Next(2),
        };
        entity.Add(chunk);
    }

    public override void Update(in Entity entity)
    {
        time++;

        if (!stuck)
        {
            if (velocity.Y < 30)
                velocity.Y += 0.4f;

            rotation = velocity.ToRotation() - MathHelper.PiOver2;

            if (Collision.IsWorldPointSolid(position + velocity) && time > 2)
            {
                time = 0;
                stuck = true;
                position.Y = (int)(position.Y / 16f) * 16 + 16;
                for (int i = 0; i < 8; i++)
                {
                    if (Collision.IsWorldPointSolid(position + velocity - new Vector2(0, 8 * i)))
                        position.Y -= 8;                        
                }
                position.Y -= 3;
            }
        }
        else
        {
            rotation = 0;
            velocity = Vector2.Zero;
            if (time > 10)
                scale *= 0.95f;

            if (scale < 0.1f)
                Active = false;
        }

    }
}
