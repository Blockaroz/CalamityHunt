using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using CalamityHunt.Content.Items.Rarities;
using CalamityHunt.Content.Projectiles.Weapons.Ranged;
using Terraria.DataStructures;
using Microsoft.Xna.Framework;
using Terraria.Audio;

namespace CalamityHunt.Content.Items.Weapons.Ranged
{
    public class Trailblazer : ModItem
    {
		public override void SetDefaults()
		{
			Item.width = 90;
			Item.height = 38;
			Item.damage = 2000;
			Item.noMelee = true;
			Item.useAnimation = 15;
			Item.useTime = 3;
			Item.useStyle = ItemUseStyleID.Shoot;
			Item.knockBack = 10f;
			SoundStyle useSound = SoundID.DD2_DarkMageSummonSkeleton;
			useSound.Pitch = -0.5f;
			useSound.MaxInstances = 0;
			Item.UseSound = useSound;
			Item.channel = true;
			Item.autoReuse = true;
			Item.shoot = ModContent.ProjectileType<TrailblazerFlame>();
			Item.shootSpeed = 12f;
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

		public override Vector2? HoldoutOffset() => new Vector2(-24f, -2f);
    }
}
