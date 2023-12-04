using CalamityHunt.Content.Items.Rarities;
using Terraria;
using Terraria.ModLoader;

namespace CalamityHunt.Content.Items.Misc;

public class IOUASoul : ModItem
{
    public override void SetDefaults()
    {
        Item.width = 32;
        Item.height = 32;
        Item.value = Item.buyPrice(0, 10, 0, 0);
        Item.maxStack = 9999;
        Item.rare = ModContent.RarityType<VioletRarity>();
        if (ModLoader.HasMod(HUtils.CalamityMod)) {
            ModRarity r;
            Mod calamity = ModLoader.GetMod(HUtils.CalamityMod);
            calamity.TryFind<ModRarity>("Violet", out r);
            Item.rare = r.Type;
        }
    }
}
