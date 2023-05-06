using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria.ModLoader;

namespace CalamityHunt.Common.Systems
{
    public class GoozmaWeaponsPlayer : ModPlayer
    {
        public static int ParasolBloodMinimum = 2000;
        public static int ParasolBloodMaximum = 5000;
        public int parasolBlood;
        public int parasolBloodWaitTime;

        public override void PostUpdateMiscEffects()
        {
            if (parasolBloodWaitTime > 0)
                parasolBloodWaitTime--;

            if (parasolBlood > 0 && parasolBloodWaitTime <= 0)
                parasolBlood -= 150;

            parasolBlood = Math.Clamp(parasolBlood, 0, ParasolBloodMaximum);
        }
    }
}
