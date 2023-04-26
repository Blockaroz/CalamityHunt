using CalamityHunt.Content.Items.Rarities;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityHunt.Content.Items.Misc
{
    public class AuricSole : ModItem
    {
        public override void SetDefaults()
        {
            Item.width = 30;
            Item.height = 28;
            Item.rare = ModContent.RarityType<VioletRarity>();
        }
        public override bool CanRightClick() => true;

        public override void RightClick(Player player)
        {
            Terraria.Audio.SoundEngine.PlaySound(SoundID.DD2_DarkMageHealImpact);
            Item.SetDefaults(ModContent.ItemType<DischargedAuricSole>());
            Item.stack++;
        }

        public override void UpdateInventory(Player player)
        {
            player.runSlowdown = 0.0001f;
        }
    }
}
