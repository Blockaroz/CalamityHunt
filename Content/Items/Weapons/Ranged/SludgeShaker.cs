using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using CalamityHunt.Content.Items.Rarities;
using CalamityHunt.Content.Projectiles.Weapons.Rogue; //lol?
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
			Item.damage = 200000;
			Item.noMelee = true;
			Item.noUseGraphic = true;
			Item.useAnimation = Item.useTime = 40;
			Item.useStyle = ItemUseStyleID.Swing;
			Item.knockBack = 10f;
			Item.UseSound = SoundID.Item1;
			Item.autoReuse = true;
			Item.shootSpeed = 22f;
			Item.rare = ModContent.RarityType<VioletRarity>();
			Item.DamageType = DamageClass.Ranged;
			Item.shoot = ModContent.ProjectileType<GoozmagaBomb>();
		}
	}
}
