using CalamityHunt.Common.Systems.Particles;
using CalamityHunt.Content.Bosses.Goozma;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using Terraria;
using Terraria.ModLoader;

namespace CalamityHunt.Content.Particles
{
    public class HueLightDust : Particle
    {
        private int frame;
        private float life;

        public override void OnSpawn()
        {
            rotation += Main.rand.NextFloat(-3f, 3f);
            scale *= 1.2f;
            frame = Main.rand.Next(3);
        }

        public override void Update()
        {
            life += 0.1f;
            scale *= 0.96f;
            rotation += velocity.X * 0.2f;

            scale -= 0.01f;
            velocity *= 0.97f;
            velocity.X += Main.rand.Next(-5, 5) * 0.01f;
            velocity.Y += Main.rand.Next(-5, 5) * 0.02f;

            if (data is float)
                color = new GradientColor(SlimeUtils.GoozColorArray, 0.2f, 0.2f).ValueAt((float)data + life * 0.05f);

            int chance = 70;
            if (!Collision.SolidTiles(position, 2, 2))
            {
                chance += 10;
                scale *= 1.0005f;
                Lighting.AddLight(position, color.ToVector3() * 0.2f * scale);
            }
            if (Main.rand.NextBool(chance) && scale > 0.25f)
            {
                Particle hue = NewParticle(Type, position, Main.rand.NextVector2Circular(1, 1), color * 0.99f, MathHelper.Clamp(scale * 2.5f, 0.1f, 2f));
                hue.data = data;
            }

            if (scale < 0.05f)
                Active = false;
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            Asset<Texture2D> texture = ModContent.Request<Texture2D>(Texture);
            Asset<Texture2D> glowSoft = ModContent.Request<Texture2D>($"{nameof(CalamityHunt)}/Assets/Textures/Goozma/GlowSoft");
            Rectangle rect = texture.Frame(1, 3, 0, frame);
            Color drawColor = color;
            drawColor.A /= 2;
            Color glowColor = color * 0.2f;
            glowColor.A = 0;
            Color whiteColor = Color.White;
            whiteColor.A = 0;

            spriteBatch.Draw(texture.Value, position - Main.screenPosition, rect, drawColor, rotation, rect.Size() * 0.5f, scale, 0, 0);
            spriteBatch.Draw(glowSoft.Value, position - Main.screenPosition, null, glowColor * 0.5f, rotation, glowSoft.Size() * 0.5f, scale * 0.5f, 0, 0);

            float innerGlowScale = 0.7f - Utils.GetLerpValue(0f, 1f, life, true) * 0.3f;
            //spriteBatch.Draw(glowSoft.Value, position - Main.screenPosition, null, whiteColor * 0.6f, rotation, glowSoft.Size() * 0.5f, scale * 0.1f * innerGlowScale, 0, 0);
            spriteBatch.Draw(texture.Value, position - Main.screenPosition, rect, whiteColor, rotation, rect.Size() * 0.5f, scale * innerGlowScale * 0.7f, 0, 0);
        }

    }
}
