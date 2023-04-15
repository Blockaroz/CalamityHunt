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
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Utilities;

namespace CalamityHunt.Content.Particles.FlyingSlimes
{
    public class FlyingBalloonSlime : Particle
    {
        public int time;
        public float distanceFade;
        public int balloonWobbleTime;
        public int balloonVariant;

        public override void OnSpawn()
        {
            scale *= Main.rand.NextFloat(0.9f, 1.1f);
            balloonVariant = Main.rand.Next(7);
        }

        public override void Update()
        {
            if (data is Vector2)
            {
                velocity = Vector2.Lerp(velocity, position.DirectionTo((Vector2)data).SafeNormalize(Vector2.Zero) * 13f, 0.2f);
                if (position.Distance((Vector2)data) < 10)
                    Active = false;

                distanceFade = Utils.GetLerpValue(20, 80, position.Distance((Vector2)data), true);
            }
            else
                distanceFade = 1f;

            time++;

            if (time > 500)
                Active = false;

            velocity *= 0.98f;
            rotation = velocity.ToRotation();

            if (color == Color.White)
            {
                WeightedRandom<Color> slimeColor = new WeightedRandom<Color>();
                slimeColor.Add(ContentSamples.NpcsByNetId[NPCID.GreenSlime].color, 1f);
                slimeColor.Add(ContentSamples.NpcsByNetId[NPCID.BlueSlime].color, 0.9f);
                color = slimeColor.Get();

                if (Main.rand.NextBool(1000))
                {
                    color = ContentSamples.NpcsByNetId[NPCID.PurpleSlime].color;
                    scale = ContentSamples.NpcsByNetId[NPCID.PurpleSlime].scale;
                }
                if (Main.rand.NextBool(3000))
                {
                    color = ContentSamples.NpcsByNetId[NPCID.Pinky].color;
                    scale = ContentSamples.NpcsByNetId[NPCID.Pinky].scale;
                }
            }
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            balloonWobbleTime++;

            Asset<Texture2D> texture = ModContent.Request<Texture2D>(Texture);
            Asset<Texture2D> balloon = ModContent.Request<Texture2D>(Texture + "Balloons");
            Rectangle balloonFrame = balloon.Frame(7, 1, balloonVariant, 0);
            float fadeIn = Utils.GetLerpValue(0, 30, time, true) * distanceFade;

            for (int i = 0; i < 10; i++)
                spriteBatch.Draw(texture.Value, position - velocity * i * 0.5f - Main.screenPosition, null, color.MultiplyRGBA(Lighting.GetColor(position.ToTileCoordinates())) * fadeIn * ((10f - i) / 50f), rotation + MathHelper.PiOver2, texture.Size() * 0.5f, scale * distanceFade * 1.05f, 0, 0);

            spriteBatch.Draw(texture.Value, position - Main.screenPosition, null, color.MultiplyRGBA(Lighting.GetColor(position.ToTileCoordinates())) * fadeIn, rotation + MathHelper.PiOver2, texture.Size() * 0.5f, scale * distanceFade, 0, 0);
            spriteBatch.Draw(balloon.Value, position + new Vector2(texture.Height() / 2f * scale * distanceFade, 0).RotatedBy(rotation) - Main.screenPosition, balloonFrame, Lighting.GetColor(position.ToTileCoordinates()) * fadeIn, rotation - MathHelper.PiOver2 + (float)Math.Sin(balloonWobbleTime * 0.5f) * 0.07f, balloonFrame.Size() * new Vector2(0.5f, 1f), distanceFade, 0, 0);
        }
    }
}
