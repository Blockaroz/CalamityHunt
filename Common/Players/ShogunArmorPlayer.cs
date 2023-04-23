using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.ModLoader;

namespace CalamityHunt.Common.Players
{
    public class ShogunArmorPlayer : ModPlayer
    {
        public bool active;
        public float slamPower;

        public override void PostUpdateRunSpeeds()
        {
            if (Player.controlDownHold)
                slamPower += 0.1f;

            else
                slamPower -= 0.1f;

            slamPower = Math.Clamp(slamPower, 0, 1);

            Player.gravity += slamPower * 25f;
            Player.maxFallSpeed += slamPower * 25f;

            if (Player.dashDelay < 0)
                Main.NewText(Player.dashDelay);
        }

        public override void ResetEffects()
        {
            active = false;
        }

    //    public override void Load()
    //    {
    //        On_Player.DashMovement += ShogunDash;
    //    }

    //    private void ShogunDash(On_Player.orig_DashMovement orig, Player self)
    //    {
    //        if (self.GetModPlayer<ShogunArmorPlayer>().active)
    //        {
    //            if (self.dashDelay > 0)
    //            {
    //                if (self.eocDash > 0)
    //                    self.eocDash--;

    //                if (self.eocDash == 0)
    //                    self.eocHit = -1;

    //                self.dashDelay--;
    //            }
    //            else if (self.dashDelay < 0)
    //            {
    //            }
    //        }
    //        else
    //            orig(self);
    //    }
    }
}
