using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using CalamityHunt.Content.Items.Rarities;
using CalamityHunt.Content.Projectiles.Weapons.Rogue;
using Terraria.DataStructures;
using Microsoft.Xna.Framework;

namespace CalamityHunt.Content.Items.Weapons.Rogue
{
	public class Goozmaga : ModItem
	{
		public override void SetStaticDefaults()
		{
			Item.ResearchUnlockCount = 1;
		}

		public override void SetDefaults()
		{
			Item.width = 100;
			Item.damage = 200; 
			Item.noMelee = true;
			Item.noUseGraphic = true;
			Item.useAnimation = Item.useTime = 40;
			Item.useStyle = ItemUseStyleID.Swing;
			Item.knockBack = 10f;
			Item.UseSound = SoundID.Item1;
			Item.autoReuse = true;
			Item.shootSpeed = 22f;
			Item.rare = ModContent.RarityType<VioletRarity>();
			Item.DamageType = DamageClass.Throwing;
			if (ModLoader.HasMod("CalamityMod"))
            {
				DamageClass d;
				Mod calamity = ModLoader.GetMod("CalamityMod");
				calamity.TryFind<DamageClass>("RogueDamageClass", out d);
				Item.DamageType = d;
            }
			Item.shoot = ModContent.ProjectileType<GoozmagaBomb>();
		}
		public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
		{
			if (ModLoader.HasMod("CalamityMod"))
			{
				Mod calamity = ModLoader.GetMod("CalamityMod");

				if ((bool)calamity.Call("CanStealthStrike", player)) //setting the stealth strike
				{
					Projectile.NewProjectile(source, position, velocity, type, damage, knockback, player.whoAmI, -1f, 1f);
					return false;
				}
			}
			else if (player.vortexStealthActive || player.shroomiteStealth)
			{
				// stealth strike
				Projectile.NewProjectile(source, position, velocity, type, damage, knockback, player.whoAmI, -1f, 1f);
				return false;
			}
			Projectile.NewProjectile(source, position, velocity, type, damage, knockback, player.whoAmI, -1f);
			return false;
		}
	}
}