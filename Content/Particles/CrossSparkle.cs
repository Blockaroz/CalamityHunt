using CalamityHunt.Common.Systems.Particles;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Arch.Core.Extensions;
using Terraria;
using Entity = Arch.Core.Entity;

namespace CalamityHunt.Content.Particles;

public struct ParticleCrossSparkle
{
    public int Time { get; set; }
}
    
public class CrossSparkle : ParticleBehavior
{
    public override void OnSpawn(in Entity entity)
    {
        ref var velocity = ref entity.Get<ParticleVelocity>();
        ref var rotation = ref entity.Get<ParticleRotation>();
            
        rotation.Value = velocity.Value.ToRotation() + Main.rand.NextFloat(-0.05f, 0.05f);
        velocity.Value = Vector2.Zero;
            
        entity.Add(new ParticleCrossSparkle());
    }

    public override void Update(in Entity entity)
    {
        time++;
        if (time > 15)
            Active = false;
        if (emit)
            Main.NewText("default is on");
    }

    public override void Draw(in Entity entity, SpriteBatch spriteBatch)
    {
        Texture2D texture = AssetDirectory.Textures.Sparkle.Value;

        float power = scale * MathF.Pow(Utils.GetLerpValue(15, 6, time, true), 2.5f) * Utils.GetLerpValue(0, 5, time, true);
        Color drawColor = color;
        drawColor.A = 0;
        spriteBatch.Draw(texture, position - Main.screenPosition, texture.Frame(), color * 0.2f, rotation - MathHelper.PiOver4, texture.Size() * 0.5f, power * new Vector2(0.3f, 0.5f), 0, 0);
        spriteBatch.Draw(texture, position - Main.screenPosition, texture.Frame(), color * 0.2f, rotation + MathHelper.PiOver4, texture.Size() * 0.5f, power * new Vector2(0.3f, 0.5f), 0, 0);
        spriteBatch.Draw(texture, position - Main.screenPosition, texture.Frame(), drawColor, rotation - MathHelper.PiOver4, texture.Size() * 0.5f, power * new Vector2(0.6f, 1f), 0, 0);
        spriteBatch.Draw(texture, position - Main.screenPosition, texture.Frame(), drawColor, rotation + MathHelper.PiOver4, texture.Size() * 0.5f, power * new Vector2(0.6f, 1f), 0, 0);
        spriteBatch.Draw(texture, position - Main.screenPosition, texture.Frame(), drawColor * 0.2f, rotation - MathHelper.PiOver4, texture.Size() * 0.5f, power * new Vector2(0.4f, 2f), 0, 0);
        spriteBatch.Draw(texture, position - Main.screenPosition, texture.Frame(), drawColor * 0.2f, rotation + MathHelper.PiOver4, texture.Size() * 0.5f, power * new Vector2(0.4f, 2f), 0, 0);
        spriteBatch.Draw(texture, position - Main.screenPosition, texture.Frame(), new Color(255, 255, 255, 0), rotation - MathHelper.PiOver4, texture.Size() * 0.5f, power * 0.5f, 0, 0);
        spriteBatch.Draw(texture, position - Main.screenPosition, texture.Frame(), new Color(255, 255, 255, 0), rotation + MathHelper.PiOver4, texture.Size() * 0.5f, power * 0.5f, 0, 0);
    }
}
