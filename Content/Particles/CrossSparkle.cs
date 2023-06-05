using CalamityHunt.Common.Systems.Particles;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.GameContent;
using Terraria.ModLoader;

namespace CalamityHunt.Content.Particles
{
    public class CrossSparkle : Particle
    {
        public int time;

        public override void OnSpawn()
        {
            rotation = velocity.ToRotation() + Main.rand.NextFloat(-0.05f, 0.05f);
            velocity = Vector2.Zero;
        }

        public override void Update()
        {
            time++;
            if (time > 15)
                Active = false;
            rotation *= (1.001f + time * 0.02f);
            if (emit)
                Main.NewText("default is on");
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            Texture2D texture = AssetDirectory.Textures.Extra.Sparkle;

            float power = scale * MathF.Pow(Utils.GetLerpValue(15, 6, time, true), 2.5f) * Utils.GetLerpValue(0, 5, time, true);
            Color drawColor = color;
            drawColor.A = 0;
            spriteBatch.Draw(texture, position - Main.screenPosition, texture.Frame(), color * 0.2f, rotation - MathHelper.PiOver4, texture.Size() * 0.5f, power * new Vector2(0.3f, 0.5f), 0, 0);
            spriteBatch.Draw(texture, position - Main.screenPosition, texture.Frame(), color * 0.2f, rotation + MathHelper.PiOver4, texture.Size() * 0.5f, power * new Vector2(0.3f, 0.5f), 0, 0);
            spriteBatch.Draw(texture, position - Main.screenPosition, texture.Frame(), drawColor, rotation - MathHelper.PiOver4, texture.Size() * 0.5f, power * new Vector2(0.6f, 1f), 0, 0);
            spriteBatch.Draw(texture, position - Main.screenPosition, texture.Frame(), drawColor, rotation + MathHelper.PiOver4, texture.Size() * 0.5f, power * new Vector2(0.6f, 1f), 0, 0);
            spriteBatch.Draw(texture, position - Main.screenPosition, texture.Frame(), drawColor * 0.2f, rotation - MathHelper.PiOver4, texture.Size() * 0.5f, power * new Vector2(0.4f, 2f), 0, 0);
            spriteBatch.Draw(texture, position - Main.screenPosition, texture.Frame(), drawColor * 0.2f, rotation + MathHelper.PiOver4, texture.Size() * 0.5f, power * new Vector2(0.4f, 2f), 0, 0);
            spriteBatch.Draw(texture, position - Main.screenPosition, texture.Frame(), new Color(255, 255, 255, 0), rotation - MathHelper.PiOver4, texture.Size() * 0.5f, power * 0.5f, 0, 0);
            spriteBatch.Draw(texture, position - Main.screenPosition, texture.Frame(), new Color(255, 255, 255, 0), rotation + MathHelper.PiOver4, texture.Size() * 0.5f, power * 0.5f, 0, 0);
        }
    }
}
