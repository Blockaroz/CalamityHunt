using CalamityHunt.Common.Systems.Particles;
using CalamityHunt.Content.Bosses.Goozma;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CalamityHunt.Core;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityHunt.Content.Particles
{
    public class GoozBombChunk : ParticleBehavior
    {
        private int variant;
        private float colOffset;
        private int time;
        private bool stuck;

        public override void OnSpawn()
        {
            scale *= Main.rand.NextFloat(1f, 1.2f);
            variant = Main.rand.Next(2);
            colOffset = Main.rand.NextFloat();
        }

        public override void Update()
        {
            time++;

            if (!stuck)
            {
                if (velocity.Y < 25)
                    velocity.Y += 0.45f;

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
                    scale *= 0.986f;

                if (scale < 0.1f)
                    Active = false;
            }

        }

        public override void Draw(SpriteBatch spriteBatch)
        { 
            Asset<Texture2D> texture = ModContent.Request<Texture2D>(Texture);
            Rectangle frame = texture.Frame(4, 2, variant, 0);
            Rectangle glowFrame = texture.Frame(4, 2, variant, 1);
            Vector2 squish = new Vector2(1f - velocity.Length() * 0.01f, 1f + velocity.Length() * 0.01f);
            float grow = (float)Math.Sqrt(Utils.GetLerpValue(-20, 40, time, true));
            if (stuck)
            {
                grow = 1f;
                frame = texture.Frame(4, 2, variant + 2, 0);
                glowFrame = texture.Frame(4, 2, variant + 2, 1);
                squish = new Vector2(1f + (float)Math.Sqrt(Utils.GetLerpValue(20, 0, time, true)) * 0.33f, 1f - (float)Math.Sqrt(Utils.GetLerpValue(20, 0, time, true)) * 0.33f);
            }
            Color glowColor = new GradientColor(SlimeUtils.GoozColors, 0.2f, 0.2f).ValueAt(time * 2f + colOffset);
            glowColor.A = 0; 
            Color lightColor = Lighting.GetColor(position.ToTileCoordinates());

            spriteBatch.Draw(texture.Value, position - Main.screenPosition, frame, Color.Lerp(color, color.MultiplyRGBA(lightColor), stuck ? 1f : Utils.GetLerpValue(10, 50, time, true)) * Utils.GetLerpValue(10, 40, time, true), rotation, frame.Size() * new Vector2(0.5f, 0.84f), scale * grow * squish, 0, 0);
            spriteBatch.Draw(texture.Value, position - Main.screenPosition, glowFrame, glowColor, rotation, frame.Size() * new Vector2(0.5f, 0.84f), scale * grow * squish, 0, 0);
            spriteBatch.Draw(texture.Value, position - Main.screenPosition, glowFrame, glowColor * 0.5f, rotation, frame.Size() * new Vector2(0.5f, 0.84f), scale * 1.05f * grow * squish, 0, 0);
        }
    }
}
