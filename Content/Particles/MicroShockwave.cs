using CalamityHunt.Common.Systems.Particles;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Arch.Core.Extensions;
using CalamityHunt.Common.Utilities;
using ReLogic.Content;
using Terraria;
using Entity = Arch.Core.Entity;

namespace CalamityHunt.Content.Particles;

public struct ParticleMicroShockwave
{
    public float CurScale { get; set; }

    public Color SecondColor { get; set; }
}
    
public class MicroShockwave : ParticleBehavior
{
    public override void OnSpawn(in Entity entity)
    {
        ref var velocity = ref entity.Get<ParticleVelocity>();
        ref var rotation = ref entity.Get<ParticleRotation>();
        rotation.Value = velocity.Value.ToRotation();
        velocity.Value = Vector2.Zero;

        entity.Add(new ParticleMicroShockwave());
    }

    public override void Update(in Entity entity)
    {
        curScale += (scale - curScale * 0.8f) * 0.09f;
        if (curScale > scale)
            Active = false;

        if (data is Color newSecondColor)
            secondColor = newSecondColor;
        else
            secondColor = Color.White;
    }

    private static Asset<Texture2D> texture;

    public override void Load()
    {
        texture = AssetUtilities.RequestImmediate<Texture2D>(Texture);
    }

    public override void Draw(in Entity entity, SpriteBatch spriteBatch)
    {
        Rectangle solidFrame = texture.Value.Frame(1, 3, 0, 0);
        Rectangle colorFrame = texture.Value.Frame(1, 3, 0, 1);
        Rectangle glowFrame = texture.Value.Frame(1, 3, 0, 2);
        float power = Utils.GetLerpValue(scale, scale * 0.7f, curScale, true);
        spriteBatch.Draw(texture.Value, position - Main.screenPosition, solidFrame, Color.Black * 0.1f * power, rotation, solidFrame.Size() * 0.5f, new Vector2(curScale, curScale * 0.5f), 0, 0);
        spriteBatch.Draw(texture.Value, position - Main.screenPosition, colorFrame, color * power, rotation, colorFrame.Size() * 0.5f, new Vector2(curScale, curScale * 0.5f), 0, 0);
        spriteBatch.Draw(texture.Value, position - Main.screenPosition, glowFrame, secondColor * power, rotation, glowFrame.Size() * 0.5f, new Vector2(curScale, curScale * 0.5f), 0, 0);
    }
}
