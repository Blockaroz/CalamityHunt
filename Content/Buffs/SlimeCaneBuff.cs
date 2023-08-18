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
using Mono.Cecil;

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

            if (count > 1)
                num = 1;
            if (count > 2)
                num = 3;            
            if (count > 3)
                num = 4;

            return num;
        }

        public void SetSlimes()
        {
            for (int i = 0; i < 4; i++)
            {
                int slimeType;
                switch (i)
                {
                    case 1:
                        slimeType = ModContent.ProjectileType<EbonianBlinky>();
                        break;
                    case 2:
                        slimeType = ModContent.ProjectileType<DivinePinky>();
                        break;
                    case 3:
                        slimeType = ModContent.ProjectileType<StellarInky>();
                        break;
                    default:
                        slimeType = ModContent.ProjectileType<CrimulanClyde>();
                        break;
                }
                if (Player.ownedProjectileCounts[slimeType] <= 0)
                {
                    int p = Projectile.NewProjectile(Player.GetSource_FromThis(), Player.position.X - (20 * i), Player.position.Y, 0, 0, slimeType, highestOriginalDamage, 0, Player.whoAmI);
                    if (Main.projectile.IndexInRange(p))
                        Main.projectile[p].originalDamage = highestOriginalDamage;
                }
            }
        }

        public override void PostUpdateBuffs()
        {
            if (Player.HasBuff<SlimeCaneBuff>())
            {
                if (Player.ownedProjectileCounts[ModContent.ProjectileType<SlimeCaneGemCounter>()] > 0)
                    slimes = true;

                int buffIndex = Player.FindBuffIndex(ModContent.BuffType<SlimeCaneBuff>());
                if (!slimes)
                    Player.DelBuff(buffIndex);
                else
                    Player.buffTime[buffIndex] = 18000;

                if (Player.whoAmI == Main.myPlayer)
                    SetSlimes();
            }
        }

        public override void ResetEffects()
        {
            slimes = false;
        }
    }
}
