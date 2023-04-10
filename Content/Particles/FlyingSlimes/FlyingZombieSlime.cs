using CalamityHunt.Common.Systems.Particles;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityHunt.Content.Particles.FlyingSlimes
{
    public class FlyingZombieSlime : Particle
    {
        public int time;
        public float distanceFade;

        public override void OnSpawn()
        {
            scale *= Main.rand.NextFloat(0.9f, 1.1f);
        }

        public override void Update()
        {
            if (data is Vector2)
            {
                velocity = Vector2.Lerp(velocity, position.DirectionTo((Vector2)data).SafeNormalize(Vector2.Zero) * 30f, 0.1f);
                if (position.Distance((Vector2)data) < 10)
                {
                    Active = false;
                    Gore.NewGore(Entity.GetSource_None(), position, Main.rand.NextVector2Circular(1, 1), 4);
                    Gore.NewGore(Entity.GetSource_None(), position, Main.rand.NextVector2Circular(1, 1), 5);
                }
                distanceFade = Utils.GetLerpValue(20, 80, position.Distance((Vector2)data), true);
            }
            else
                distanceFade = 1f;
            
            Lighting.AddLight(position + velocity, Color.Pink.ToVector3());

            time++;

            if (time > 500)
                Active = false;

            velocity *= 0.98f;
            rotation += 0.1f;
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            Asset<Texture2D> texture = ModContent.Request<Texture2D>(Texture);

            float fadeIn = Utils.GetLerpValue(0, 30, time, true) * distanceFade;

            Color drawColor = color;
            drawColor.A /= 2;

            for (int i = 0; i < 10; i++)
                spriteBatch.Draw(texture.Value, position - velocity * i * 0.5f - Main.screenPosition, null, drawColor.MultiplyRGBA(Lighting.GetColor(position.ToTileCoordinates())) * fadeIn * ((10f - i) / 60f), rotation + MathHelper.PiOver2, texture.Size() * 0.5f, scale * distanceFade, 0, 0);

            spriteBatch.Draw(texture.Value, position - Main.screenPosition, null, drawColor.MultiplyRGBA(Lighting.GetColor(position.ToTileCoordinates())) * fadeIn, rotation + MathHelper.PiOver2, texture.Size() * 0.5f, scale * distanceFade, 0, 0);
        }
    }
}
