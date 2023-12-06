using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CalamityHunt.Content.Items.Misc;
using Terraria;
using Terraria.ModLoader;

namespace CalamityHunt.Common.Players;

public class VanityPlayer : ModPlayer
{
    public bool tendrilCursor;
    public int tendrilCount;

    public override void Load()
    {
        On_Player.UpdateVisibleAccessory += UpdateTendrilCount;
    }

    private void UpdateTendrilCount(On_Player.orig_UpdateVisibleAccessory orig, Player self, int itemSlot, Item item, bool modded)
    {
        if (item.type == ModContent.ItemType<TendrilCursorAttachment>()) {
            self.GetModPlayer<VanityPlayer>().tendrilCount = (itemSlot - 1) % 10;
        }

        orig(self, itemSlot, item, modded);
    }

    public override void ResetEffects()
    {
        tendrilCursor = false;
    }
}
