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
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityHunt.Content.Particles
{
    public class CrimBombChunk : Particle
    {
        private int variant;
        private int time;
        private bool stuck;

        public override void OnSpawn()
        {
            scale *= Main.rand.NextFloat(1f, 1.2f);
            variant = Main.rand.Next(2);
        }

        public override void Update()
        {
            time++;

            if (!stuck)
            {
                if (velocity.Y < 30)
                    velocity.Y += 0.6f;

                rotation = velocity.ToRotation() - MathHelper.PiOver2;

                if (Collision.SolidTiles(position - new Vector2(0, 16) + velocity * 1.7f, 4, 4) && time > 20)
                {
                    if (!Main.dedServ)
                        SoundEngine.PlaySound(SoundID.NPCDeath9.WithPitchOffset(Main.rand.NextFloat(-0.2f, 0.3f)), position);
                    if (Main.rand.NextBool())
                    {
                        time = 0;
                        stuck = true;
                    }
                    else
                    {
                        Active = false;

                        for (int i = 0; i < 8; i++)
                            Dust.NewDustPerfect(position + Main.rand.NextVector2Circular(4, 4), DustID.TintableDust, -velocity * 0.1f + Main.rand.NextVector2Circular(2, 2), 200, Color.Red, Main.rand.NextFloat(2f) * scale);

                    }
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

        public override void Draw(SpriteBatch spriteBatch)
        { 
            Asset<Texture2D> texture = ModContent.Request<Texture2D>(Texture);
            Rectangle frame = texture.Frame(4, 1, variant, 0);
            Vector2 squish = new Vector2(1f - velocity.Length() * 0.01f, 1f + velocity.Length() * 0.01f);
            float grow = (float)Math.Sqrt(Utils.GetLerpValue(-20, 40, time, true));
            if (stuck)
            {
                grow = 1f;
                frame = texture.Frame(4, 1, variant + 2, 0);
                squish = new Vector2(1f + (float)Math.Sqrt(Utils.GetLerpValue(20, 0, time, true)) * 0.33f, 1f - (float)Math.Sqrt(Utils.GetLerpValue(20, 0, time, true)) * 0.33f);
            }
            spriteBatch.Draw(texture.Value, position - Main.screenPosition, frame, color, rotation, frame.Size() * new Vector2(0.5f, 0.84f), scale * grow * squish, 0, 0);

        }
    }
}
