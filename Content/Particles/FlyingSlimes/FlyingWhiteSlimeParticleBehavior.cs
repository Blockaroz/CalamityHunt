using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using ReLogic.Content;
using Terraria;
using Terraria.ModLoader;

namespace CalamityHunt.Content.Particles.FlyingSlimes
{
    public class FlyingWhiteSlimeParticleBehavior : FlyingSlimeParticleBehavior // White Slime from the 'Aequus' mod
    {
        public override float SlimeSpeed => 30f;
        public override bool ShouldDraw => false;

        public override void PostUpdate()
        {
            Lighting.AddLight(position + velocity, Color.White.ToVector3());
        }

        public override void DrawSlime(SpriteBatch spriteBatch)
        {
            Asset<Texture2D> texture = ModContent.Request<Texture2D>(Texture);
            float fadeIn = Utils.GetLerpValue(0, 30, time, true) * distanceFade;
            Color drawColor = color;
            drawColor.A /= 2;
            spriteBatch.Draw(texture.Value, position - Main.screenPosition, null, drawColor * fadeIn, rotation + MathHelper.PiOver2, texture.Size() * 0.5f, scale * distanceFade, 0, 0);
            for (int i = 0; i < 10; i++)
                spriteBatch.Draw(texture.Value, position - velocity * i * 0.5f - Main.screenPosition, null, drawColor * fadeIn * ((10f - i) / 30f), rotation + MathHelper.PiOver2, texture.Size() * 0.5f, scale * distanceFade, 0, 0);
        }
    }
}
