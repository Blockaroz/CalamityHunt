using System;
using Terraria;
using Terraria.ModLoader;

namespace CalamityHunt.Common.Systems
{
    public class MonolithPlayer : ModPlayer
    {
        public int monolithCount;

        public override void ResetEffects()
        {
            monolithCount = 0;
        }

        public override void PostUpdateMiscEffects()
        {
            if (GoozmaSystem.GoozmaActive)
                monolithCount = 1;

            Player.ManageSpecialBiomeVisuals("HuntOfTheOldGods:SlimeMonsoon", monolithCount > 4, Player.Center);
        }
    }
}
