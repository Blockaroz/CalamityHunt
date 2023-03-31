using CalamityHunt.Content.Items.Materials;
using CalamityHunt.Content.Tiles;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria.ModLoader;

namespace CalamityHunt.Content.Items.Placeable
{
    public class GoozMonolith : ModItem
    {
        public override void SetDefaults()
        {
            Item.DefaultToPlaceableTile(ModContent.TileType<GoozMonolithTile>());
        }

        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient<EntropyMatter>(5)
                .Register();
        }
    }
}
