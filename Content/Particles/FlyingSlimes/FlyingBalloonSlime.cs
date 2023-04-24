using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Utilities;

namespace CalamityHunt.Content.Particles.FlyingSlimes
{
    public class FlyingBalloonSlime : FlyingSlime
    {
        public int balloonWobbleTime;
        public int balloonVariant;
        public override bool ShouldDraw => false;
        public override float SlimeSpeed => 13f;
        public override float SlimeAcceleration => 0.2f;

        public override void OnSpawn()
        {
            balloonVariant = Main.rand.Next(7);
        }

        public override void PostUpdate()
        {
            if (color == Color.White)
            {
                WeightedRandom<Color> slimeColor = new WeightedRandom<Color>();
                slimeColor.Add(ContentSamples.NpcsByNetId[NPCID.GreenSlime].color, 1f);
                slimeColor.Add(ContentSamples.NpcsByNetId[NPCID.BlueSlime].color, 0.9f);
                color = slimeColor.Get();

                if (Main.rand.NextBool(100))
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

        public override void DrawSlime(SpriteBatch spriteBatch)
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
