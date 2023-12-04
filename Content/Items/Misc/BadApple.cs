using CalamityHunt.Content.Items.Rarities;
using Terraria;
using Terraria.ModLoader;

namespace CalamityHunt.Content.Items.Misc
{
    public class BadApple : ModItem
    {
        public override void SetDefaults()
        {
            Item.width = 26;
            Item.height = 32;
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
}
