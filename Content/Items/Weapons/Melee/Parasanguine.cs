using CalamityHunt.Common.Systems;
using CalamityHunt.Content.Items.Rarities;
using CalamityHunt.Content.Projectiles.Weapons.Melee;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityHunt.Content.Items.Weapons.Melee
{
    public class Parasanguine : ModItem
    {
        public override void SetDefaults()
        {
            Item.width = 78;
            Item.height = 72;
            Item.damage = 16000;
            Item.DamageType = DamageClass.Melee;
            Item.rare = ModContent.RarityType<VioletRarity>();
            Item.useStyle = ItemUseStyleID.Swing;
            Item.useTime = 50;
            Item.useAnimation = 50;
            Item.channel = true;
            Item.noUseGraphic = true;
            Item.noMelee = true;
            Item.value = Item.sellPrice(gold: 20);
            Item.shoot = ModContent.ProjectileType<ParasanguineHeld>();
            Item.shootSpeed = 5f;
        }

        public override void PostDrawInInventory(SpriteBatch spriteBatch, Vector2 position, Rectangle frame, Color drawColor, Color itemColor, Vector2 origin, float scale)
        {
            Texture2D bar = ModContent.Request<Texture2D>($"{nameof(CalamityHunt)}/Assets/Textures/ChargeBars/Style0_0").Value;
            Texture2D barCharge = ModContent.Request<Texture2D>($"{nameof(CalamityHunt)}/Assets/Textures/ChargeBars/Style0_1").Value;

            Rectangle chargeFrame = new Rectangle(0, 0, (int)(barCharge.Width * Main.LocalPlayer.GetModPlayer<GoozmaWeaponsPlayer>().ParasolBloodPercent), barCharge.Height);

            spriteBatch.Draw(bar, position + new Vector2(0, 60) * scale, bar.Frame(), Color.DarkRed, 0, bar.Size() * 0.5f, scale * 1.75f, 0, 0);
            spriteBatch.Draw(barCharge, position + new Vector2(0, 60) * scale, chargeFrame, new Color(255, 20, 30), 0, barCharge.Size() * 0.5f, scale * 1.75f, 0, 0);
        }

        public override bool CanUseItem(Player player) => player.ownedProjectileCounts[ModContent.ProjectileType<ParasanguineHeld>()] <= 0;

        public override bool AltFunctionUse(Player player) => true;

        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            if (player.ownedProjectileCounts[ModContent.ProjectileType<ParasanguineHeld>()] <= 0)
            {
                int blood = 0;
                if (player.altFunctionUse > 0 && player.GetModPlayer<GoozmaWeaponsPlayer>().ParasolBloodPercent > 0.5f)
                    blood = 2;

                Projectile.NewProjectileDirect(source, position, velocity, type, damage, 0, player.whoAmI, ai1: blood);
            }

            return false;
        }
    }
}
