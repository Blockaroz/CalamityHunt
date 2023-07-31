using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria.ID;
using Terraria;
using Terraria.ModLoader;
using CalamityHunt.Common.Players;
using CalamityHunt.Content.Projectiles;
using Microsoft.Xna.Framework;
using CalamityHunt.Content.Projectiles.Weapons.Summoner;

namespace CalamityHunt.Content.Buffs
{
    public class SlimeCaneBuff : ModBuff
    {
        public override void SetStaticDefaults()
        {
            Main.buffNoTimeDisplay[Type] = true;
            Main.buffNoSave[Type] = true;
        }

        public override void Update(Player player, ref int buffIndex)
        {
            if (player.ownedProjectileCounts[ModContent.ProjectileType<SlimeCaneGemCounter>()] > 0)
                player.GetModPlayer<SlimeCanePlayer>().slimes = true;

            if (!player.GetModPlayer<SlimeCanePlayer>().slimes)
                player.DelBuff(buffIndex);
            else
                player.buffTime[buffIndex] = 18000;
        }
    }

    public class SlimeCanePlayer : ModPlayer
    {
        public bool slimes;

        public int highestOriginalDamage;

        public int SlimeRank()
        {
            int num = 0;

            int count = Player.ownedProjectileCounts[ModContent.ProjectileType<SlimeCaneGemCounter>()];

            if (count > 5)
                num = 2;

            return num;
        }
    }
}
