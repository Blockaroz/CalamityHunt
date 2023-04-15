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
    public class SlimeMonsoonMonolith : ModItem
    {
        public override void SetDefaults()
        {
            Item.DefaultToPlaceableTile(ModContent.TileType<SlimeMonsoonMonolithTile>());
        }

        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient<EntropyMatter>(5)
                .Register();
        }
    }
}
