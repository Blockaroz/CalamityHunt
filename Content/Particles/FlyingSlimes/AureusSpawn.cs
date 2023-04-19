using CalamityHunt.Common.Systems.Particles;
using CalamityHunt.Content.Projectiles;
using Terraria.Audio;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityHunt.Content.Particles.FlyingSlimes
{
    public class AureusSpawn : Particle
    {
        public int time;
        public int currentFrame = 0;
        public float frameCounter = 0;
        public float distanceFade;

        public override void OnSpawn()
        {
            scale *= Main.rand.NextFloat(0.9f, 1.1f);
        }

        public override void Update()
        {
            if (data is Vector2)
            {
                velocity = Vector2.Lerp(velocity, position.DirectionTo((Vector2)data).SafeNormalize(Vector2.Zero) * 16f, 0.1f);
                if (position.Distance((Vector2)data) < 10)
                {
                    Active = false;
                    SoundEngine.PlaySound(GoozmaSpawn.slimeabsorb, position);
                }

                distanceFade = Utils.GetLerpValue(20, 80, position.Distance((Vector2)data), true);
            }
            else
                distanceFade = 1f;

            Lighting.AddLight(position, 0.6f * distanceFade, 0.25f * distanceFade, 0f);

            time++;

            if (time > 500)
                Active = false;

            velocity *= 0.98f;
            rotation = velocity.ToRotation();

            color = Color.White;

            frameCounter += 0.22f;
            frameCounter %= 4;
            int frame = (int)frameCounter;
            currentFrame = frame * 62;
        }

        public override void Draw(SpriteBatch spriteBatch)
        { 
            // 92 by 62
            Asset<Texture2D> texture = ModContent.Request<Texture2D>(Texture);
            Asset<Texture2D> glow = ModContent.Request<Texture2D>(Texture + "Glow");
            Rectangle frame = new(0, currentFrame, 92, 62);
            float fadeIn = Utils.GetLerpValue(0, 30, time, true) * distanceFade;

            for (int i = 0; i < 10; i++)

                spriteBatch.Draw(texture.Value, position - velocity * i * 0.5f - Main.screenPosition, frame, color.MultiplyRGBA(Lighting.GetColor(position.ToTileCoordinates())) * fadeIn * ((10f - i) / 80f), rotation + MathHelper.PiOver2, frame.Size() * 0.5f, scale * distanceFade * 1.05f, 0, 0);

            spriteBatch.Draw(texture.Value, position - Main.screenPosition, frame, color.MultiplyRGBA(Lighting.GetColor(position.ToTileCoordinates())) * fadeIn, rotation + MathHelper.PiOver2, frame.Size() * 0.5f, scale * distanceFade, 0, 0);
            spriteBatch.Draw(glow.Value, position - Main.screenPosition, frame, Color.White * fadeIn, rotation + MathHelper.PiOver2, frame.Size() * 0.5f, scale * distanceFade, 0, 0);
        }
    }
}
