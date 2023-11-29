using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CalamityHunt.Content.Projectiles.Weapons.Ranged;
using Terraria.DataStructures;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;

namespace CalamityHunt.Content.Items.Weapons.Ranged;

public class AntiMassAccumulator : ModItem
{
    public override void SetDefaults()
    {
        Item.damage = 545;
        Item.DamageType = DamageClass.Ranged;
        Item.width = 100;
        Item.height = 42;
        Item.useTime = 90;
        Item.useAnimation = Item.useTime;
        Item.useStyle = ItemUseStyleID.Shoot;
        Item.knockBack = 6;
        Item.value = 10000;
        Item.rare = ItemRarityID.Red;
        Item.autoReuse = true;
        Item.shootSpeed = 5;
        Item.shoot = ModContent.ProjectileType<AntiMassAccumulatorProj>();
        Item.noMelee = true;
        Item.noUseGraphic = true;
        Item.channel = true;
    }

    public override void AddRecipes()
    {
        if (ModLoader.TryGetMod("CalamityMod", out Mod calamityMod)) {

        }
        else {
            CreateRecipe()
                .AddIngredient(ItemID.AdamantiteBar, 20)
                .AddIngredient(ItemID.SoulofFright, 5)
                .AddIngredient(ItemID.IllegalGunParts, 2)
                .AddIngredient(ItemID.Glass, 25)
                .AddTile(TileID.MythrilAnvil)
                .Register();
        }
    }

    public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
    {
        Projectile.NewProjectileDirect(source, position, velocity, type, damage, 0, player.whoAmI);

        return false;
    }

    public static Asset<Texture2D> glowmask;

    public override void Load()
    {
        glowmask = ModContent.Request<Texture2D>($"{Texture}_Glow", AssetRequestMode.ImmediateLoad);
    }

    public override void PostDrawInInventory(SpriteBatch spriteBatch, Vector2 position, Rectangle frame, Color drawColor, Color itemColor, Vector2 origin, float scale)
    {
        Texture2D glow = glowmask.Value;
        spriteBatch.Draw(glow, position, frame, Color.Gold with { A = 40 }, 0, origin, scale, 0, 0);
    }

    public override void PostDrawInWorld(SpriteBatch spriteBatch, Color lightColor, Color alphaColor, float rotation, float scale, int whoAmI)
    {
        Texture2D glow = glowmask.Value;
        spriteBatch.Draw(glow, Item.Center - Main.screenPosition, glow.Frame(), Color.Gold with { A = 40 }, rotation, glow.Size() * 0.5f, scale, 0, 0);
    }
}
