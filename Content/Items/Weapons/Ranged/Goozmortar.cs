using CalamityHunt.Common.Systems;
using CalamityHunt.Content.Items.Rarities;
using CalamityHunt.Content.Particles;
using CalamityHunt.Content.Projectiles.Weapons.Ranged;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityHunt.Content.Items.Weapons.Ranged
{
    public class Goozmortar : ModItem
    {
        public override void SetDefaults()
        {
            Item.damage = 400;
            Item.DamageType = DamageClass.Ranged;
            Item.width = 50;
            Item.height = 36;
            Item.useTime = 15;
            Item.useAnimation = 15;
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.knockBack = 6;
            Item.value = Item.sellPrice(0, 30);
            Item.rare = ModContent.RarityType<VioletRarity>();
            Item.shoot = ModContent.ProjectileType<GoozmortarHeld>();
            Item.shootSpeed = 5f;
            Item.channel = true;
            Item.noUseGraphic = true;
            Item.noMelee = true;
        }

        public override bool AltFunctionUse(Player player) => player.GetModPlayer<GoozmaWeaponsPlayer>().mortarUses >= player.GetModPlayer<GoozmaWeaponsPlayer>().mortarMaxUses;

        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            if (player.ownedProjectileCounts[ModContent.ProjectileType<GoozmortarHeld>()] < 1 && player.altFunctionUse == 0)
                Projectile.NewProjectileDirect(source, position, velocity, ModContent.ProjectileType<GoozmortarHeld>(), 0, knockback, player.whoAmI, 0, 0);
            if (player.ownedProjectileCounts[ModContent.ProjectileType<GoozmortarHeld>()] < 1 && player.altFunctionUse == 2)
                Projectile.NewProjectileDirect(source, position, velocity, ModContent.ProjectileType<GoozmortarHeld>(), 0, knockback, player.whoAmI, 0, 1);

            return false;
        }

        public override void AddRecipes()
        {

        }
    }
}