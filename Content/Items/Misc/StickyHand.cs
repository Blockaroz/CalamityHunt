using CalamityHunt.Content.Items.Rarities;
using CalamityHunt.Content.Projectiles;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityHunt.Content.Items.Misc
{
    public class StickyHand : ModItem
    {
        public override void SetDefaults()
        {
            Item.CloneDefaults(ItemID.GrapplingHook);
            Item.shoot = ModContent.ProjectileType<StickyHandProj>();
            Item.shootSpeed = 25f;
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
