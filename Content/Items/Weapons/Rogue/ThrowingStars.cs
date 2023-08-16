using CalamityHunt.Content.Items.Rarities;
using CalamityHunt.Content.Projectiles.Weapons.Rogue;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityHunt.Content.Items.Weapons.Rogue
{
    public class ThrowingStars : ModItem
    {
        public override void SetStaticDefaults()
        {
            ItemID.Sets.SkipsInitialUseSound[Type] = true;
        }

        public override void SetDefaults()
        {
            Item.width = 100;
            Item.height = 100;
            Item.damage = 2222;
            Item.noMelee = true;
            Item.noUseGraphic = true;
            Item.useAnimation = 12;
            Item.useTime = 4;
            Item.useStyle = ItemUseStyleID.Swing;
            Item.knockBack = 10f;
            Item.UseSound = SoundID.DD2_JavelinThrowersAttack with { MaxInstances = 0, Pitch = 1f, Volume = 0.5f };
            Item.autoReuse = true;
            Item.shootSpeed = 14f;
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
            Item.shoot = ModContent.ProjectileType<ThrowingStarsProjectile>();
        }

        public override Color? GetAlpha(Color lightColor) => new Color(200, 200, 200, 200);

        public override void ModifyShootStats(Player player, ref Vector2 position, ref Vector2 velocity, ref int type, ref int damage, ref float knockback)
        {
            if (Main.rand.NextBool(7))
                type = Item.shoot;//ModContent.ProjectileType<ThrowingStarsCritProjectile>();
        }

        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            if (!Main.dedServ && Item.UseSound.HasValue)
                SoundEngine.PlaySound(Item.UseSound.Value, player.Center);

            if (player.whoAmI == Main.myPlayer)
            {
                Vector2 newVelocity = velocity * Main.rand.NextFloat(0.5f, 1.1f);
                Projectile.NewProjectile(source, player.MountedCenter + Main.rand.NextVector2Circular(16, 16) + newVelocity, newVelocity.RotatedByRandom(0.1f), type, damage, knockback, player.whoAmI);
            }

            return false;
        }
    }
}
