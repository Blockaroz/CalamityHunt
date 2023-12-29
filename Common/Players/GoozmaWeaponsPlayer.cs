using System;
using System.Linq;
using CalamityHunt.Content.Items.Weapons.Summoner;
using CalamityHunt.Content.Projectiles.Weapons.Magic;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.ModLoader;

namespace CalamityHunt.Common.Players
{
    public class GoozmaWeaponsPlayer : ModPlayer
    {
        public bool gobfloggerBuff;

        public override void PostUpdateMiscEffects()
        {
         
        }

        public override float UseSpeedMultiplier(Item item)
        {
            if (gobfloggerBuff && item.type == ModContent.ItemType<Gobflogger>()) {
                return 6.66f;
            }

            return 1f;
        }

        public override void ResetEffects()
        {
            gobfloggerBuff = false;
        }
    }
}
