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
    public class CosmicSmoke : Particle
    {
        public int time;
        public int variant;
        public float rotationalVelocity;

        public override void OnSpawn()
        {
            variant = Main.rand.Next(8);
            scale *= 0.5f + Main.rand.NextFloat(0.9f, 1.1f);
            rotationalVelocity = Main.rand.NextFloat(-0.4f, 0.4f);
            rotation += Main.rand.NextFloat(-3f, 3f);
        }

        public override void Update()
        {
            scale *= 0.99f;
            time++;
            velocity *= 0.95f;

            rotationalVelocity *= 0.97f * Math.Clamp(1 - time * 0.001f, 0.7f, 1f);
            rotation += rotationalVelocity;

            if (time > 20)
                scale *= 0.9f + Math.Clamp(scale * 0.0001f, 0f, 1f);

            if (scale < 0.1f)
                Active = false;
        }
        public override void Draw(SpriteBatch spriteBatch)
        {
            Asset<Texture2D> texture = ModContent.Request<Texture2D>(Texture);
            Rectangle frame = texture.Frame(4, 2, variant % 4, (int)(variant / 4f));
            float grow = (float)Math.Sqrt(Utils.GetLerpValue(0, 7, time, true));
            spriteBatch.Draw(texture.Value, position - Main.screenPosition, frame, color, rotation, frame.Size() * 0.5f, scale * grow, 0, 0);
        }
    }
}
