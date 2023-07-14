using CalamityHunt.Common.Systems;
using CalamityHunt.Content.Items.Materials;
using CalamityHunt.Content.Items.Rarities;
using CalamityHunt.Content.Tiles;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityHunt.Content.Items.Placeable
{
    public class SlimeNinjaStatue : ModItem
    {
        public override void SetDefaults()
        {
            Item.DefaultToPlaceableTile(ModContent.TileType<SlimeNinjaStatueTile>());
            Item.rare = ItemRarityID.Blue;
        }

        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient(ItemID.StoneBlock, 150)
                .AddIngredient(ItemID.ArmorStatue, 1)
                .AddIngredient(ItemID.Gel, 20)
                .AddTile(TileID.WorkBenches)
                .AddCondition(Condition.Hardmode)
                .Register();

            CreateRecipe()
                .AddIngredient(ItemID.StoneBlock, 100)
                .AddIngredient(ItemID.Gel)
                .AddCondition(Condition.InGraveyard);
        }
    }
}
