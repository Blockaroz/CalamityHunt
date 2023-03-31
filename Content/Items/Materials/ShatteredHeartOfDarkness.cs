using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityHunt.Content.Items.Materials
{
    public class ShatteredHeartOfDarkness : ModItem
    {
        public override void SetStaticDefaults()
        {
            //DrawAnimation animation = new DrawAnimation();
            //animation.FrameCount = 4;
            //animation.TicksPerFrame = 10;
            //ItemID.Sets.AnimatesAsSoul[Type] = true;
            //Main.RegisterItemAnimation(Type, animation);
        }

        public override void SetDefaults()
        {
            Item.width = 40;
            Item.height = 40;
            Item.value = 1000;
            Item.rare = ItemRarityID.Red;
        }
    }
}
