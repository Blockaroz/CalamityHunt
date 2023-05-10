using CalamityHunt.Content.Items.Weapons.Summoner;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.ModLoader;

namespace CalamityHunt.Common.Systems
{
    public class GoozmaWeaponsPlayer : ModPlayer
    {
        public static int ParasolBloodMax = 8000;
        public int parasolBlood;
        public int parasolBloodWaitTime;
        public float ParasolBloodPercent => Math.Clamp((float)parasolBlood / ParasolBloodMax, 0, 1);

        public bool gobfloggerBuff;

        public override void PostUpdateMiscEffects()
        {
            if (parasolBloodWaitTime > 0)
                parasolBloodWaitTime--;

            if (parasolBlood > 0 && parasolBloodWaitTime <= 0)
                parasolBlood -= 100;

            parasolBlood = Math.Clamp(parasolBlood, 0, ParasolBloodMax);
        }

        public override float UseSpeedMultiplier(Item item)
        {
            if (gobfloggerBuff && item.type == ModContent.ItemType<Gobflogger>())
                return 4f;
            return 1f;
        }

        public override void ResetEffects()
        {
            gobfloggerBuff = false;
        }
    }
}
