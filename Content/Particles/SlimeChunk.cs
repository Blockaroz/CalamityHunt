using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using CalamityHunt.Common.Systems.Particles;
using ReLogic.Content;
using Terraria;
using Terraria.ModLoader;

namespace CalamityHunt.Content.Particles
{
    public class SlimeChunk : Particle
    {
        private int time;
        private int variant;
        private Vector2 finalPos;

        public override void OnSpawn()
        {
            variant = Main.rand.Next(3);
        }

        public override void Update()
        {
            if (data is not null)
                if (data is Vector2)
                {
                    finalPos = (Vector2)data;
                    if (position.Distance(finalPos) > 20)
                    {
                        velocity += position.DirectionTo(finalPos).SafeNormalize(Vector2.Zero) * 0.3f;
                        velocity = Vector2.Lerp(velocity, position.DirectionTo(finalPos).SafeNormalize(Vector2.Zero) * 20, 0.07f);
                    }
                    else
                        velocity *= 0.2f;

                    if (time > 20 && position.Distance(finalPos) > 20)
                        time = 20;

                    if (Main.rand.NextBool(3))
                        Dust.NewDustPerfect(position + Main.rand.NextVector2Circular(40, 40), 4, velocity * 0.2f, 150, color, scale).noGravity = true;
                }

            rotation = velocity.ToRotation() - MathHelper.PiOver2;

            if (time > 50)
                Active = false;

            time++;
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            Asset<Texture2D> texture = ModContent.Request<Texture2D>(Texture);
            Rectangle frame = texture.Frame(1, 3, 0, variant);
            Vector2 squish = new Vector2(1f - velocity.Length() * 0.01f, 1f + velocity.Length() * 0.01f);
            float inOut = Utils.GetLerpValue(0, 20, time, true) * Utils.GetLerpValue(50, 20, time, true);
            Color drawColor = color;
            if (data is not null)
                if (data is Vector2)
                {
                    drawColor = Color.Lerp(color, Color.Black * 0.3f, Utils.GetLerpValue(200, 80, position.Distance(finalPos), true));
                    spriteBatch.Draw(texture.Value, position - Main.screenPosition, frame, Lighting.GetColor(position.ToTileCoordinates()).MultiplyRGBA(drawColor).MultiplyRGBA(new Color(150, 150, 150)), rotation, frame.Size() * 0.5f, squish * scale * inOut, 0, 0);
                    spriteBatch.Draw(texture.Value, position - Main.screenPosition, frame, drawColor.MultiplyRGBA(new Color(260, 260, 260, 0)), rotation, frame.Size() * 0.5f, squish * scale * inOut, 0, 0);
                    return;
                }

            spriteBatch.Draw(texture.Value, position - Main.screenPosition, frame, Lighting.GetColor(position.ToTileCoordinates()).MultiplyRGBA(drawColor).MultiplyRGBA(new Color(150, 150, 150)), rotation, frame.Size() * 0.5f, squish * scale * inOut, 0, 0);
            spriteBatch.Draw(texture.Value, position - Main.screenPosition, frame, Lighting.GetColor(position.ToTileCoordinates()).MultiplyRGBA(drawColor).MultiplyRGBA(new Color(270, 270, 270, 0)), rotation, frame.Size() * 0.5f, squish * scale * inOut, 0, 0);
        }
    }
}
