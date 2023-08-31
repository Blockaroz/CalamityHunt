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
    public class GooBurst : ParticleBehavior
    {
        private int variant;
        private float colOffset;
        private int frameCounter;
        private int frame;

        public override void OnSpawn()
        {
            scale *= Main.rand.NextFloat(0.9f, 1.1f);
            variant = Main.rand.Next(0, 2);
            rotation = velocity.ToRotation() + MathHelper.PiOver2;
            velocity = Vector2.Zero;
        }

        public override void Update()
        {
            frameCounter++;
            if (frame < 3)
                frameCounter++;
            if (frameCounter % 4 == 0)
                frame++;
            if (frame > 7)
                Active = false;
            if (data is float)
                color = new GradientColor(SlimeUtils.GoozColors, 0.2f, 0.2f).ValueAt((float)data + Math.Max(0, frameCounter - 18) * 3f - 2f);
        }

        public override void Draw(SpriteBatch spriteBatch)
        { 
            Asset<Texture2D> texture = ModContent.Request<Texture2D>(Texture);
            Asset<Texture2D> glow = ModContent.Request<Texture2D>(Texture + "Glow");
            Rectangle drawFrame = texture.Frame(8, 2, frame, variant);

            Color glowColor = color;
            glowColor.A = 0;

            spriteBatch.Draw(texture.Value, position - Main.screenPosition, drawFrame, Color.Gray, rotation, drawFrame.Size() * new Vector2(0.5f, 1f), scale, 0, 0);
            spriteBatch.Draw(glow.Value, position - Main.screenPosition, drawFrame, color, rotation, drawFrame.Size() * new Vector2(0.5f, 1f), scale, 0, 0);
            spriteBatch.Draw(glow.Value, position - Main.screenPosition, drawFrame, glowColor * 0.5f, rotation, drawFrame.Size() * new Vector2(0.5f, 1f), scale * 1.01f, 0, 0);

        }
    }
}
