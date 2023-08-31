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
using Terraria.ModLoader;

namespace CalamityHunt.Content.Particles
{
    public struct ParticleCosmicSmoke
    {
        public int Time { get; set; }

        public int MaxTime { get; set; }

        public int Variant { get; set; }

        public float RotationalVelocity { get; set; }
    }
    
    public class CosmicSmoke : ParticleBehavior
    {
        public override void OnSpawn()
        {
            variant = Main.rand.Next(8);
            scale *= 0.8f + Main.rand.NextFloat(0.9f, 1.1f);
            rotationalVelocity = Main.rand.NextFloat(-0.15f, 0.15f);
            rotation += Main.rand.NextFloat(-3f, 3f);
            maxTime = (int)(15 * (scale * 0.2f + 0.8f));
        }

        public override void Update()
        {
            scale *= 0.99f;
            time++;

            rotationalVelocity *= 0.96f * Math.Clamp(1 - time * 0.001f, 0.7f, 1f);
            rotation += rotationalVelocity;

            if (time > maxTime * 0.5f)
            {
                scale *= 0.93f - Math.Clamp(scale * 0.0001f, 0f, 0.5f);
                velocity *= 0.83f;
            }
            else
                velocity *= 1.01f;

            if (time > maxTime * 0.8f)
                Active = false;
        }
        public override void Draw(SpriteBatch spriteBatch)
        {
            if ((string)data == "Cosmos")
            {
                behindEntities = true;
                return;
            }

            Asset<Texture2D> texture = ModContent.Request<Texture2D>(Texture);
            Rectangle frame = texture.Frame(4, 2, variant % 4, (int)(variant / 4f));
            float grow = (float)Math.Sqrt(Utils.GetLerpValue(0, maxTime * 0.2f, time, true));
            float opacity = Utils.GetLerpValue(maxTime * 0.8f, 0, time, true) * Math.Clamp(scale, 0, 1);
            spriteBatch.Draw(texture.Value, position - Main.screenPosition, frame, color * opacity, rotation, frame.Size() * 0.5f, scale * grow * 0.5f, 0, 0);
        }
    }
}
