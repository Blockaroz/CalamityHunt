﻿using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using CalamityHunt.Content.Items.Rarities;
using CalamityHunt.Content.Projectiles.Weapons.Ranged;
using Terraria.DataStructures;
using Microsoft.Xna.Framework;

namespace CalamityHunt.Content.Items.Weapons.Ranged
{
    internal class SludgeShaker : ModItem
    {
		public override void SetStaticDefaults()
		{
			Item.ResearchUnlockCount = 1;
		}

		public override void SetDefaults()
		{
			Item.width = 116;
			Item.height = 76;
			Item.damage = 2200;
			Item.noMelee = true;
			Item.noUseGraphic = true;
			Item.useAnimation = 30;
			Item.useTime = 30;
			Item.useStyle = ItemUseStyleID.Shoot;
			Item.knockBack = 10f;
			Item.UseSound = SoundID.Item1;
			Item.channel = true;
			Item.autoReuse = true;
			Item.shoot = ModContent.ProjectileType<SludgeShakerHeld>();
			Item.shootSpeed = 5f;
			Item.rare = ModContent.RarityType<VioletRarity>();
            if (ModLoader.HasMod("CalamityMod"))
            {
                ModRarity r;
                Mod calamity = ModLoader.GetMod("CalamityMod");
                calamity.TryFind<ModRarity>("Violet", out r);
                Item.rare = r.Type;
            }
            Item.DamageType = DamageClass.Ranged;
		}

        public override bool CanUseItem(Player player) => player.ownedProjectileCounts[ModContent.ProjectileType<SludgeShakerHeld>()] <= 0;

        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            if (player.ownedProjectileCounts[ModContent.ProjectileType<SludgeShakerHeld>()] <= 0)
                Projectile.NewProjectileDirect(source, position, velocity, type, damage, 0, player.whoAmI);

            return false;
        }
    }
}