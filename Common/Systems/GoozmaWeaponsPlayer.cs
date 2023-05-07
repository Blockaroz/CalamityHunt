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
        public static int ParasolBloodMax = 8000;
        public int parasolBlood;
        public int parasolBloodWaitTime;
        public float ParasolBloodPercent => Math.Clamp((float)parasolBlood / ParasolBloodMax, 0, 1);

        public override void PostUpdateMiscEffects()
        {
            if (parasolBloodWaitTime > 0)
                parasolBloodWaitTime--;

            if (parasolBlood > 0 && parasolBloodWaitTime <= 0)
                parasolBlood -= 100;

            parasolBlood = Math.Clamp(parasolBlood, 0, ParasolBloodMax);
        }
    }
}
