using CalamityHunt.Common.Systems.Particles;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Arch.Core.Extensions;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;
using Entity = Arch.Core.Entity;

namespace CalamityHunt.Content.Particles
{
    public struct ParticleEbonBombChunk
    {
        public int Variant { get; set; }

        public int Time { get; set; }

        public bool Stuck { get; set; }
    }
    
    public class EbonBombChunk : ParticleBehavior
    {
        public override void OnSpawn(in Entity entity)
        {
            ref var scale = ref entity.Get<ParticleScale>();
            scale.Value *= Main.rand.NextFloat(1f, 1.2f);
            
            var chunk = new ParticleEbonBombChunk
            {
                Variant = Main.rand.Next(2),
            };
            entity.Add(chunk);
        }

        public override void Update()
        {
            time++;

            if (!stuck)
            {
                if (velocity.Y < 30)
                    velocity.Y += 0.5f;

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

            Color lightColor = Lighting.GetColor(position.ToTileCoordinates());
            spriteBatch.Draw(texture.Value, position - Main.screenPosition, frame, Color.Lerp(color, color.MultiplyRGBA(lightColor), stuck ? 1f : Utils.GetLerpValue(10, 50, time, true)), rotation, frame.Size() * new Vector2(0.5f, 0.84f), scale * grow * squish, 0, 0);
        }
    }
}
