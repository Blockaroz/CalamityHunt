using CalamityHunt.Content.Items.Materials;
using CalamityHunt.Content.Particles;
using Microsoft.Xna.Framework;
using CalamityHunt.Common.Systems.Particles;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using CalamityHunt.Content.Tiles;

namespace CalamityHunt.Content.Items.Misc
{
    public class EntropicSlimeConcentration : ModItem
    {
        public override void SetDefaults()
        {
            Item.width = 40;
            Item.height = 40;
            Item.knockBack = 6;
            Item.value = 10000;
            Item.rare = ItemRarityID.Blue;
            Item.channel = true;
        }

        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient<EntropyMatter>(5)
                .AddIngredient(ItemID.Gel, 100)
                .AddIngredient<GelatinousCatalyst>()
                .AddTile<SlimeNinjaStatueTile>()
                .Register();
        }
    }
}