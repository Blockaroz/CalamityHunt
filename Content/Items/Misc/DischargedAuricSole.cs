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
    public class DischargedAuricSole : ModItem
    {
        public override void SetDefaults()
        {
            Item.width = 30;
            Item.height = 28;
            Item.rare = ModContent.RarityType<VioletRarity>();
        }

        public override void RightClick(Player player)
        {
            Item.type = ModContent.ItemType<AuricSole>();
        }
    }
}
