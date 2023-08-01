using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Utilities;
using System;
using System.Diagnostics.CodeAnalysis;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.ModLoader;

namespace CalamityHunt
{
    public static class HuntOfTheOldGodUtils
    {
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

        public class NPCAudioTracker
        {
            private int _expectedType;

            private int _expectedIndex;

            public NPCAudioTracker(NPC npc)
            {
                _expectedIndex = npc.whoAmI;
                _expectedType = npc.type;
            }

            public bool IsActiveAndInGame()
            {
                if (Main.gameMenu)
                    return false;

                NPC npc = Main.npc[_expectedIndex];
                if (npc.active)
                    return npc.type == _expectedType;

                return false;
            }
        }
        public class ItemAudioTracker
        {
            private int _expectedType;

            private int _expectedIndex;

            public ItemAudioTracker(Item item)
            {
                _expectedIndex = item.whoAmI;
                _expectedType = item.type;
            }

            public bool IsActiveAndInGame()
            {
                if (Main.gameMenu)
                    return false;

                Item item = Main.item[_expectedIndex];
                if (item.active)
                    return item.type == _expectedType;

                return false;
            }
        }
    }
}
