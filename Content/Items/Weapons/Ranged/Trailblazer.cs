using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using CalamityHunt.Content.Items.Rarities;
using CalamityHunt.Content.Projectiles.Weapons.Ranged;
using Terraria.DataStructures;
using Microsoft.Xna.Framework;
using Terraria.Audio;
using Terraria.GameContent;
using Microsoft.Xna.Framework.Graphics;
using System;

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
			SoundStyle useSound = SoundID.DD2_DrakinShot;
			useSound.Pitch = 0.5f;
			useSound.MaxInstances = 0;
			Item.UseSound = useSound;
			Item.channel = true;
			Item.autoReuse = true;
			Item.shoot = ModContent.ProjectileType<TrailblazerFlame>();
			Item.shootSpeed = 9.5f;
			Item.rare = ModContent.RarityType<VioletRarity>();
			Item.useAmmo = AmmoID.Gel;
            if (ModLoader.HasMod("CalamityMod"))
            {
                ModRarity r;
                Mod calamity = ModLoader.GetMod("CalamityMod");
                calamity.TryFind<ModRarity>("Violet", out r);
                Item.rare = r.Type;
            }
            Item.DamageType = DamageClass.Ranged;
            Item.consumeAmmoOnFirstShotOnly = true;
		}

        public override Vector2? HoldoutOffset() => new Vector2(-16f, 0f);

        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            Projectile.NewProjectileDirect(source, position, velocity.RotatedBy(MathF.Sin(Main.GlobalTimeWrappedHourly * 15) * 0.1f).RotatedByRandom(0.1f), type, damage, knockback);
            return false;
        }
    }

    public class TrailblazerBackpackLayer : PlayerDrawLayer
    {
        public override Position GetDefaultPosition() => new AfterParent(PlayerDrawLayers.Backpacks);

        public override bool GetDefaultVisibility(PlayerDrawSet drawInfo) => drawInfo.drawPlayer.HeldItem.type == ModContent.ItemType<Trailblazer>() && !drawInfo.drawPlayer.turtleArmor && drawInfo.drawPlayer.body != 106 && drawInfo.drawPlayer.body != 170 && drawInfo.drawPlayer.backpack <= 0 && !drawInfo.drawPlayer.mount.Active;

        protected override void Draw(ref PlayerDrawSet drawInfo)
        {
            Texture2D backpackTexture = ModContent.Request<Texture2D>($"{nameof(CalamityHunt)}/Content/Items/Weapons/Ranged/Trailblazer_Back").Value;

            Vector2 vec5 = drawInfo.Position - Main.screenPosition + drawInfo.drawPlayer.bodyPosition + new Vector2(drawInfo.drawPlayer.width / 2, drawInfo.drawPlayer.height - drawInfo.drawPlayer.bodyFrame.Height / 2) + new Vector2(0f, -4f);
            vec5 = vec5.Floor();
            vec5.ApplyVerticalOffset(drawInfo);

            DrawData item = new DrawData(backpackTexture, vec5, new Rectangle(0, 0, backpackTexture.Width, drawInfo.drawPlayer.bodyFrame.Height), drawInfo.colorArmorBody, drawInfo.drawPlayer.bodyRotation, new Vector2(backpackTexture.Width * 0.5f, drawInfo.bodyVect.Y), 1f, drawInfo.playerEffect);
            drawInfo.DrawDataCache.Add(item);
        }
    }
}
