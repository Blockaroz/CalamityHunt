using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.DataStructures;
using Terraria.ModLoader;

namespace CalamityHunt
{
    public static class HuntOfTheOldGodUtils
    {
        public static Vector2 HeadPosition(this PlayerDrawSet drawInfo) => drawInfo.drawPlayer.GetHelmetDrawOffset() + new Vector2((int)(drawInfo.Position.X - Main.screenPosition.X - (float)(drawInfo.drawPlayer.bodyFrame.Width / 2) + (float)(drawInfo.drawPlayer.width / 2)), (int)(drawInfo.Position.Y - Main.screenPosition.Y + (float)drawInfo.drawPlayer.height - (float)drawInfo.drawPlayer.bodyFrame.Height + 4f)) + drawInfo.drawPlayer.headPosition + drawInfo.headVect;

        public static Vector2 BodyPosition(this PlayerDrawSet drawInfo) => new Vector2((int)(drawInfo.Position.X - Main.screenPosition.X - (float)(drawInfo.drawPlayer.bodyFrame.Width / 2) + (float)(drawInfo.drawPlayer.width / 2)), (int)(drawInfo.Position.Y - Main.screenPosition.Y + (float)drawInfo.drawPlayer.height - (float)drawInfo.drawPlayer.bodyFrame.Height + 4f)) + drawInfo.drawPlayer.bodyPosition + new Vector2(drawInfo.drawPlayer.bodyFrame.Width / 2, drawInfo.drawPlayer.bodyFrame.Height / 2);

        public static void ApplyVerticalOffset(ref this Vector2 drawPos, PlayerDrawSet drawInfo)
        {
            Vector2 value = Main.OffsetsPlayerHeadgear[drawInfo.drawPlayer.bodyFrame.Y / drawInfo.drawPlayer.bodyFrame.Height];
            value.Y -= 2f;
            drawPos += value * -drawInfo.playerEffect.HasFlag(SpriteEffects.FlipVertically).ToDirectionInt();
        }

        public static Vector2 GetDesiredVelocityForDistance(Vector2 start, Vector2 end, float slowDownFactor, int time)
        {
            Vector2 velocity = start.DirectionTo(end).SafeNormalize(Vector2.Zero);
            float distance = start.Distance(end);
            float velocityFactor = (distance * (float)Math.Log(slowDownFactor)) / ((float)Math.Pow(slowDownFactor, time) - 1);
            return velocity * velocityFactor;
        }

        public static float Modulo(float dividend, float divisor) => dividend - (float)Math.Floor(dividend / divisor) * divisor;

        public static string ShortTooltip => "Whispers from on high dance in your ears...";
        public static Color ShortTooltipColor => new(227, 175, 64); // #E3AF40

        // This line is what tells the player to hold Shift. There is essentially no reason to change it
        public static string LeftShiftExpandTooltip => "Press REPLACE THIS NOW to listen closer";
        public static Color LeftShiftExpandColor => new(190, 190, 190); // #BEBEBE

    }
}
