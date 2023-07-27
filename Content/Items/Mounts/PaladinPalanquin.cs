using CalamityHunt.Content.Items.Rarities;
using CalamityHunt.Content.Mounts;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityHunt.Content.Items.Mounts
{
    public class PaladinPalanquin : ModItem
    {
        public override void SetDefaults()
        {
            Item.DefaultToMount(ModContent.MountType<PaladinPalanquinMount>());
            Item.width = 34;
            Item.height = 22;
            Item.value = Item.sellPrice(0, 30, 0, 0);
            Item.rare = ModContent.RarityType<VioletRarity>();
            if (ModLoader.HasMod("CalamityMod"))
            {
                ModRarity r;
                Mod calamity = ModLoader.GetMod("CalamityMod");
                calamity.TryFind<ModRarity>("Violet", out r);
                Item.rare = r.Type;
            }
        }
    }
}
