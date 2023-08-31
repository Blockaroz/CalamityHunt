using Arch.Core.Extensions;
using CalamityHunt.Common.Systems.Particles;
using CalamityHunt.Common.Utilities;
using CalamityHunt.Content.Bosses.Goozma;
using CalamityHunt.Core;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using Terraria;
using Entity = Arch.Core.Entity;

namespace CalamityHunt.Content.Particles;

public struct ParticleHueLightDust
{
    public int Frame { get; set; }

    public float Life { get; set; }
}
    
public class HueLightDust : ParticleBehavior
{
    public override void OnSpawn(in Entity entity)
    {
        ref var rotation = ref entity.Get<ParticleRotation>();
        ref var scale = ref entity.Get<ParticleScale>();
        rotation.Value += Main.rand.NextFloat(-3f, 3f);
        scale.Value *= 1.5f;

        var dust = new ParticleHueLightDust
        {
            Frame = Main.rand.Next(3),
        };
        entity.Add(dust);
    }

    public override void Update(in Entity entity)
    {
        life += 0.1f;
        scale *= 0.94f;
        rotation += velocity.X * 0.2f;

        scale -= 0.02f;
        velocity *= 0.97f;
        velocity.X += Main.rand.Next(-3, 3) * 0.005f;
        velocity.Y += Main.rand.Next(-3, 3) * 0.008f;

        if (data is float)
            color = new GradientColor(SlimeUtils.GoozColors, 0.2f, 0.2f).ValueAt((float)data + life * 0.05f);

        if (!Collision.SolidTiles(position, 2, 2))
        {
            scale *= 1.0004f;
            Lighting.AddLight(position, color.ToVector3() * 0.2f * scale);
        }

        if (Main.rand.NextBool(150) && scale > 0.25f)
        {
            ParticleBehavior hue = NewParticle(this, position, Main.rand.NextVector2Circular(1, 1), color * 0.99f, MathHelper.Clamp(scale * 2f, 0.1f, 1.5f));
            hue.data = data;
        }

        if (scale < 0.1f)
            Active = false;
    }

    public static Asset<Texture2D> texture;

    public override void Load()
    {
        texture = AssetUtilities.RequestImmediate<Texture2D>(Texture);
    }

    public override void Draw(in Entity entity, SpriteBatch spriteBatch)
    {
        Texture2D glow = AssetDirectory.Textures.Glow.Value;
        Rectangle rect = texture.Value.Frame(1, 3, 0, frame);
        Color drawColor = color;
        drawColor.A /= 3;
        Color glowColor = color * 0.2f;
        glowColor.A = 0;
        Color whiteColor = Color.White;
        whiteColor.A = 0;

        spriteBatch.Draw(texture.Value, position - Main.screenPosition, rect, drawColor, rotation, rect.Size() * 0.5f, scale, 0, 0);
        spriteBatch.Draw(glow, position - Main.screenPosition, null, glowColor * 0.5f, rotation, glow.Size() * 0.5f, scale * 0.5f, 0, 0);

        float innerGlowScale = 0.7f - Utils.GetLerpValue(0f, 1f, life, true) * 0.2f;
        spriteBatch.Draw(texture.Value, position - Main.screenPosition, rect, whiteColor, rotation, rect.Size() * 0.5f, scale * innerGlowScale * 0.7f, 0, 0);
    }
}
