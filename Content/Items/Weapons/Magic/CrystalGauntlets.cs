using CalamityHunt.Common.Players;
using CalamityHunt.Common.Utilities;
using CalamityHunt.Content.Items.Rarities;
using CalamityHunt.Content.Projectiles.Weapons.Magic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityHunt.Content.Items.Weapons.Magic
{
    public class CrystalGauntlets : ModItem
    {
        public override void SetStaticDefaults()
        {
            ItemID.Sets.HasAProjectileThatHasAUsabilityCheck[Type] = true;
            ItemID.Sets.gunProj[Type] = true;
        }

        public override void SetDefaults()
        {
            Item.width = 46;
            Item.height = 54;
            Item.damage = 1000;
            Item.DamageType = DamageClass.Magic;
            Item.rare = ModContent.RarityType<VioletRarity>();
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.mana = 15;
            Item.useTime = 12;
            Item.useAnimation = 12;
            Item.channel = true;
            Item.noUseGraphic = true;
            Item.noMelee = true;
            Item.value = Item.sellPrice(gold: 20);
            Item.shootSpeed = 4f;
            if (ModLoader.HasMod(HUtils.CalamityMod)) {
                ModRarity r;
                Mod calamity = ModLoader.GetMod(HUtils.CalamityMod);
                calamity.TryFind<ModRarity>("Violet", out r);
                Item.rare = r.Type;
            }
        }

        public override bool CanUseItem(Player player) => false;

        public override bool AltFunctionUse(Player player) => true;

        public static Color[] SpectralColor => new Color[]
        {
            new Color(232, 140, 167),
            new Color(184, 130, 207),
            new Color(82, 172, 158),
            new Color(114, 189, 201)
        };

        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            player.manaCost = 0f;

            return false;
        }

        public override void Update(ref float gravity, ref float maxFallSpeed)
        {
            gravity *= 0.8f;
            Color glowColor = new GradientColor(SpectralColor, 0.1f, 0.1f).ValueAt(Main.GlobalTimeWrappedHourly * 7f);
            Lighting.AddLight(Item.Center, glowColor.ToVector3() * 0.4f);
        }

        public override Color? GetAlpha(Color lightColor) => new Color(255, 255, 255, 200);

        public override void EquipFrameEffects(Player player, EquipType type)
        {
            player.handon = -1;
            player.handoff = -1;
        }

        public override void HoldItem(Player player)
        {
            Color glowColor = new GradientColor(SpectralColor, 0.1f, 0.1f).ValueAt(Main.GlobalTimeWrappedHourly * 7f);
            Lighting.AddLight(player.MountedCenter, glowColor.ToVector3() * 0.4f);
        }
    }
}
