using CalamityHunt.Content.Items.Rarities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.ModLoader;

namespace CalamityHunt.Content.Items.Armor.Shogun
{
    [AutoloadEquip(EquipType.Legs)]
    public class ShogunPants : ModItem
    {
        public override void SetDefaults()
        {
            Item.width = 22;
            Item.height = 18;
            Item.value = Item.sellPrice(gold: 10);
            Item.rare = ModContent.RarityType<VioletRarity>();
            Item.defense = 46;
        }

        public override void UpdateEquip(Player player)
        {
            player.GetDamage(DamageClass.Generic) += 0.15f;
            player.moveSpeed += 0.5f;
            player.runAcceleration *= 1.2f;
            player.maxRunSpeed *= 1.2f;
            player.accRunSpeed *= 0.5f;
            player.runSlowdown *= 2f;
        }
    }
}
