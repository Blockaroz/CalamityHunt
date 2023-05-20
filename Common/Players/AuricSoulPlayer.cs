using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;

namespace CalamityHunt.Common.Players
{
    public class AuricSoulPlayer : ModPlayer
    {
        public bool pureSoul;
        public bool corruptSoul;

        public override void SaveData(TagCompound tag)
        {
            tag["HuntOfTheOldGod:PureSoul"] = pureSoul;
        }

        public override void LoadData(TagCompound tag)
        {
            pureSoul = tag.GetBool("HuntOfTheOldGod:PureSoul");
        }

        public override void PostUpdate()
        {
            if (pureSoul)
            {
                Player.GetDamage(DamageClass.Generic) += 0.03f;
                Player.GetCritChance(DamageClass.Generic) += 0.03f;
                Player.GetKnockback(DamageClass.Generic) += 0.03f;
                Player.moveSpeed += 0.03f;
                Player.GetAttackSpeed(DamageClass.Melee) += 0.03f;
                Player.endurance += 0.01f;
                Player.statLifeMax2 += 10;
                Player.statManaMax2 += 20;
                Player.statDefense += 3;
            }
        }
    }
}
