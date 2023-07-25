using CalamityHunt.Common.Players;
using CalamityHunt.Content.Items.Rarities;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityHunt.Content.Items.Armor.Shogun
{
    [AutoloadEquip(EquipType.Body)]
    public class ShogunChestplate : ModItem
    {
        public override void Load()
        {
            if (Main.netMode != NetmodeID.Server)
            {
                EquipLoader.AddEquipTexture(Mod, Texture + "_Waist", EquipType.Waist, this);
                EquipLoader.AddEquipTexture(Mod, Texture.Replace("Chestplate", "Wings"), EquipType.Wings, this);
            }
        }

        public override void SetDefaults()
        {
            Item.width = 34;
            Item.height = 24;
            Item.value = Item.sellPrice(gold: 10);
            Item.rare = ModContent.RarityType<VioletRarity>();
            Item.defense = 50;
            Item.lifeRegen = 3;
            if (ModLoader.HasMod("CalamityMod"))
            {
                ModRarity r;
                Mod calamity = ModLoader.GetMod("CalamityMod");
                calamity.TryFind<ModRarity>("Violet", out r);
                Item.rare = r.Type;
            }
        }

        public override void UpdateEquip(Player player)
        {
            player.GetDamage(DamageClass.Generic) += 0.15f;
            player.endurance += 0.09f;
            player.statLifeMax2 += 150;
            player.statManaMax2 += 150;
        }

        public override bool WingUpdate(Player player, bool inUse)
        {
            bool set = player.GetModPlayer<ShogunArmorPlayer>().active;

            return false;
        }

        public override void EquipFrameEffects(Player player, EquipType type)
        {
            player.waist = EquipLoader.GetEquipSlot(Mod, Name, EquipType.Waist);
        }
    }
}
