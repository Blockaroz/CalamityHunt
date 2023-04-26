using CalamityHunt.Content.Items.Rarities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityHunt.Content.Items.Misc
{
    public class AuricSole : ModItem
    {
        //public override void SetStaticDefaults()
        //{
        //}

        public override void SetDefaults()
        {
            Item.width = 30;
            Item.height = 28;
            Item.rare = ModContent.RarityType<VioletRarity>();
        }

        public override void RightClick(Player player)
        {
            Item.type = ModContent.ItemType<DischargedAuricSole>();
        }

        public override void UpdateInventory(Player player)
        {
            player.runSlowdown = 0.0001f;
        }
    }
}
