using CalamityHunt.Common.Systems;
using CalamityHunt.Content.Items.Materials;
using CalamityHunt.Content.Items.Rarities;
using CalamityHunt.Content.Tiles;
using Terraria;
using Terraria.ModLoader;

namespace CalamityHunt.Content.Items.Placeable
{
    public class SlimeMonsoonMonolith : ModItem
    {
        public override void SetDefaults()
        {
            Item.DefaultToPlaceableTile(ModContent.TileType<SlimeMonsoonMonolithTile>());
            Item.rare = ModContent.RarityType<VioletRarity>();
            Item.accessory = true;
            Item.vanity = true;
        }

        public override void UpdateAccessory(Player player, bool visual)
        {
            player.GetModPlayer<MonolithPlayer>().monolithCount = 40;
        }
        public override void UpdateVanity(Player player)
        {
            player.GetModPlayer<MonolithPlayer>().monolithCount = 40;
        }

        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient<EntropyMatter>(5)
                .Register();
        }
    }
}
