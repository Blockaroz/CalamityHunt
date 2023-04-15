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
        public static Vector2 HeadPosition(this PlayerDrawSet drawInfo) => drawInfo.drawPlayer.GetHelmetDrawOffset() + new Vector2((int)(drawInfo.Position.X - Main.screenPosition.X - (float)(drawInfo.drawPlayer.bodyFrame.Width / 2) + (float) (drawInfo.drawPlayer.width / 2)), (int) (drawInfo.Position.Y - Main.screenPosition.Y + (float) drawInfo.drawPlayer.height - (float) drawInfo.drawPlayer.bodyFrame.Height + 4f)) + drawInfo.drawPlayer.headPosition + drawInfo.headVect;

        public static Vector2 BodyPosition(this PlayerDrawSet drawInfo) => new Vector2((int)(drawInfo.Position.X - Main.screenPosition.X - (float)(drawInfo.drawPlayer.bodyFrame.Width / 2) + (float)(drawInfo.drawPlayer.width / 2)), (int)(drawInfo.Position.Y - Main.screenPosition.Y + (float)drawInfo.drawPlayer.height - (float)drawInfo.drawPlayer.bodyFrame.Height + 4f)) + drawInfo.drawPlayer.bodyPosition + new Vector2(drawInfo.drawPlayer.bodyFrame.Width / 2, drawInfo.drawPlayer.bodyFrame.Height / 2);

        public static void ApplyVerticalOffset(ref this Vector2 drawPos, PlayerDrawSet drawInfo)
        {
            Vector2 value = Main.OffsetsPlayerHeadgear[drawInfo.drawPlayer.bodyFrame.Y / drawInfo.drawPlayer.bodyFrame.Height];
            value.Y -= 2f;
            drawPos += value * -drawInfo.playerEffect.HasFlag(SpriteEffects.FlipVertically).ToDirectionInt();
        }
    }
}
