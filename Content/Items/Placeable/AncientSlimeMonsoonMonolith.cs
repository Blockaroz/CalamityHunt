using CalamityHunt.Common.Systems;
using CalamityHunt.Content.Items.Materials;
using CalamityHunt.Content.Items.Rarities;
using CalamityHunt.Content.Tiles;
using Terraria;
using Terraria.ModLoader;

namespace CalamityHunt.Content.Items.Placeable;

public class AncientSlimeMonsoonMonolith : ModItem
{
    public override void SetDefaults()
    {
        Item.DefaultToPlaceableTile(ModContent.TileType<AncientSlimeMonsoonMonolithTile>());
        Item.rare = ModContent.RarityType<VioletRarity>();
        Item.accessory = true;
        Item.vanity = true;
        if (ModLoader.HasMod("CalamityMod")) {
            ModRarity r;
            Mod calamity = ModLoader.GetMod("CalamityMod");
            calamity.TryFind<ModRarity>("Violet", out r);
            Item.rare = r.Type;
        }
    }

    public override void UpdateAccessory(Player player, bool visual)
    {
        UpdateVanity(player);
    }

    public override void UpdateVanity(Player player)
    {
        player.GetModPlayer<SceneEffectPlayer>().effectActive[(ushort)SceneEffectPlayer.EffectorType.SlimeMonsoonOld] = 30;
    }

    public override void AddRecipes()
    {
        if (ModLoader.HasMod("CalamityMod")) {
            Mod calamity = ModLoader.GetMod("CalamityMod");
            CreateRecipe()
                .AddIngredient<ChromaticMass>(15)
                .AddTile(calamity.Find<ModTile>("DraedonsForge").Type)
                .Register();
        }
        else {
            CreateRecipe()
                .AddIngredient<ChromaticMass>(15)
                .AddTile<SlimeNinjaStatueTile>()
                .Register();
        }
    }
}
