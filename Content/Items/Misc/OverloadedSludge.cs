using CalamityHunt.Content.Items.Materials;
using CalamityHunt.Content.Particles;
using Microsoft.Xna.Framework;
using CalamityHunt.Common.Systems.Particles;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityHunt.Content.Items.Misc
{
    public class OverloadedSludge : ModItem
    {
        public override void SetDefaults()
        {
            Item.width = 40;
            Item.height = 40;
            Item.knockBack = 6;
            Item.value = 10000;
            Item.rare = ItemRarityID.LightRed;
            Item.channel = true;
            Item.maxStack = 9999;
        }
    }
}