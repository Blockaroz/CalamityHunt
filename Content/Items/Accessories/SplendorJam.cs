using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using CalamityHunt.Content.Items.Rarities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CalamityHunt.Content.Items.Accessories
{
    public class SplendorJam : ModItem
    {
		public override void SetDefaults()
		{
			Item.width = 32;
			Item.height = 30;
			Item.value = Item.sellPrice(gold: 35);
			Item.accessory = true;
			Item.rare = ModContent.RarityType<VioletRarity>();
		}
	}
}
