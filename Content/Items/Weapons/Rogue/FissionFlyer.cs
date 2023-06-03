using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using CalamityHunt.Content.Items.Rarities;
using CalamityHunt.Content.Projectiles.Weapons.Rogue;
using Terraria.DataStructures;
using Microsoft.Xna.Framework;
using System;

namespace CalamityHunt.Content.Items.Weapons.Rogue
{
	public class FissionFlyer : ModItem
	{
		public override void SetDefaults()
		{
			Item.width = 50;
			Item.height = 50;
			Item.damage = 1200; 
			Item.noMelee = true;
			Item.noUseGraphic = true;
			Item.useAnimation = 50;
			Item.useTime = 5;
			Item.useLimitPerAnimation = 3;
			Item.useStyle = ItemUseStyleID.Swing;
			Item.knockBack = 5f;
			Item.UseSound = SoundID.DD2_LightningBugHurt;
			Item.autoReuse = true;
			Item.shootSpeed = 15f;
			Item.rare = ModContent.RarityType<VioletRarity>();
			Item.DamageType = DamageClass.Throwing;
			if (ModLoader.HasMod("CalamityMod"))
            {
				DamageClass d;
				ModRarity r;
				Mod calamity = ModLoader.GetMod("CalamityMod");
				calamity.TryFind<DamageClass>("RogueDamageClass", out d);
				calamity.TryFind<ModRarity>("Violet", out r);
				Item.DamageType = d;
				Item.rare = r.Type;
            }
			Item.shoot = ModContent.ProjectileType<FissionFlyerProj>();
		}

		public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
		{
			bool stealth = false;
			if (ModLoader.HasMod("CalamityMod"))
			{
				Mod calamity = ModLoader.GetMod("CalamityMod");

				if ((bool)calamity.Call("CanStealthStrike", player)) //setting the stealth strike
					stealth = true;

            }
			else if (player.vortexStealthActive || player.shroomiteStealth)
                stealth = true;

			if (stealth)
			{

			}
			else
			{

			}

			if (Main.myPlayer == player.whoAmI)
			{
				Vector2 mouseWorld = Main.MouseWorld;
				player.LimitPointToPlayerReachableArea(ref mouseWorld);
				velocity = velocity.SafeNormalize(Vector2.Zero).RotatedByRandom(0.3f) * Main.rand.NextFloat(0.8f, 1.3f) * Math.Max(170, player.Distance(mouseWorld)) * MathF.E * 0.009f;
                Projectile.NewProjectile(source, position, velocity, type, damage, knockback, player.whoAmI);

            }

            return false;
		}
	}
}