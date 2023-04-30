using CalamityHunt.Content.Items.Rarities;
using System.Collections.Generic;
using System.Linq;
using Terraria;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;

namespace CalamityHunt.Content.Items.Lore
{
    public class GoozmaLore : ModItem
    {
        public override void SetStaticDefaults()
        {
            ItemID.Sets.ItemIconPulse[Type] = true;
            ItemID.Sets.ItemNoGravity[Type] = true;
        }

        public override void SetDefaults()
        {
            Item.width = 40;
            Item.height = 40;
            Item.value = Item.sellPrice(0, 30);
            Item.rare = ModContent.RarityType<VioletRarity>();
            Item.maxStack = Item.CommonMaxStack;
        }

        public override void ModifyTooltips(List<TooltipLine> tooltips)
        {
            if (!Main.keyState.IsKeyDown(Microsoft.Xna.Framework.Input.Keys.LeftShift))
                return;

            //stuff is in HuntOfTheoldGodUtils
            string tooltip = Language.GetOrRegister($"{nameof(CalamityHunt)}.Lore.Goozma").Value;

            TooltipLine line = tooltips.FirstOrDefault(x => x.Mod == "Terraria" && x.Name == "Tooltip2");
            if (line != null)
                line.Text = tooltip;
        }
    }
}
