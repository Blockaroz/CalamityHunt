using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria.ModLoader;

namespace CalamityHunt.Common.Players
{
    public class MovementModifyPlayer : ModPlayer
    {
        public bool stickyHand;
        public Vector2 preHookVelocity;

        public override void PostUpdateRunSpeeds()
        {
            if (stickyHand)
            {
                Player.noFallDmg = true;
                Player.gravity = 0f;
                Player.maxFallSpeed = 9999f;
                Player.runSlowdown = 0f;
                Player.GoingDownWithGrapple = true;
            }
            else
                preHookVelocity = Player.velocity;

            stickyHand = false;
        }

        public override void ResetEffects()
        {
        }
    }
}
