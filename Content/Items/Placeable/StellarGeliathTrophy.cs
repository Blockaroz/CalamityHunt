using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria;

namespace CalamityHunt.Content.Items.Placeable;

public class StellarGeliathTrophy : ModItem
{
    public override void SetDefaults()
    {
        Item.DefaultToPlaceableTile(ModContent.TileType<Tiles.Trophies.StellarGeliathTrophyTile>());

        Item.width = 32;
        Item.height = 32;
        Item.rare = ItemRarityID.Blue;
        Item.value = Item.buyPrice(0, 1);
    }
}
