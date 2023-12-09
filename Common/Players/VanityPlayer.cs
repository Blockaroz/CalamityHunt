using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CalamityHunt.Content.Items.Misc;
using Terraria;
using Terraria.ModLoader;
using Terraria.UI;

namespace CalamityHunt.Common.Players;

public class VanityPlayer : ModPlayer
{
    public bool tendrilCursor;
    public int tendrilCount;
    public int cTendril;
    public int tendrilSlot;

    public override void Load()
    {
        On_Player.UpdateVisibleAccessory += UpdateTendrilCount;
    }

    public override void UpdateDyes()
    {
        if (tendrilSlot > -1) {
            cTendril = Player.dye[tendrilSlot % 10].dye;
        }
    }

    private void UpdateTendrilCount(On_Player.orig_UpdateVisibleAccessory orig, Player self, int itemSlot, Item item, bool modded)
    {
        if (item.type == ModContent.ItemType<TendrilCursorAttachment>()) {
            int tendrilCount = 7;
            if (!modded) {
                tendrilCount = Math.Clamp((itemSlot - 1) % 10, 1, 8);
            }
            self.GetModPlayer<VanityPlayer>().tendrilCount = tendrilCount;
            self.GetModPlayer<VanityPlayer>().tendrilSlot = itemSlot;
        }

        orig(self, itemSlot, item, modded);
    }

    public override void ResetEffects()
    {
        tendrilCursor = false;
    }
}
