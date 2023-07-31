using CalamityHunt.Common.Players;
using CalamityHunt.Content.Items.Rarities;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityHunt.Content.Items.Armor.Shogun
{
    [AutoloadEquip(EquipType.Body)]
    public class ShogunChestplate : ModItem
    {
        public override void Load()
        {
            EquipLoader.AddEquipTexture(Mod, Texture + "_Waist", EquipType.Waist, this);
            EquipLoader.AddEquipTexture(Mod, Texture.Replace("Chestplate", "Wings"), EquipType.Wings, this);
        }

        public override void SetStaticDefaults()
        {
            Item.wingSlot = EquipLoader.GetEquipSlot(Mod, Name, EquipType.Wings);
            ArmorIDs.Wing.Sets.Stats[Item.wingSlot] = new WingStats(9000, 13f, 3f, true);
        }

        public override void SetDefaults()
        {
            Item.wingSlot = EquipLoader.GetEquipSlot(Mod, Name, EquipType.Wings);
            Item.width = 34;
            Item.height = 24;
            Item.value = Item.sellPrice(gold: 10);
            Item.rare = ModContent.RarityType<VioletRarity>();
            Item.defense = 50;
            Item.lifeRegen = 3;

            if (ModLoader.HasMod("CalamityMod"))
            {
                ModRarity r;
                Mod calamity = ModLoader.GetMod("CalamityMod");
                calamity.TryFind<ModRarity>("Violet", out r);
                Item.rare = r.Type;
            }
        }

        public override void UpdateEquip(Player player)
        {
            player.GetDamage(DamageClass.Generic) += 0.15f;
            player.endurance += 0.09f;
            player.statLifeMax2 += 150;
            player.statManaMax2 += 150;
        }

        public override bool WingUpdate(Player player, bool inUse)
        {
            if (inUse)
            {
                Dust d = Dust.NewDustDirect(player.Center - new Vector2(15), 30, 30, DustID.Sand, 0, 0, 100, Color.Black, 0.5f);
                d.noGravity = true;
            }

            return false;
        }

        public override void VerticalWingSpeeds(Player player, ref float ascentWhenFalling, ref float ascentWhenRising, ref float maxCanAscendMultiplier, ref float maxAscentMultiplier, ref float constantAscend)
        {
            ascentWhenFalling = 0.5f;
            ascentWhenRising = 0.3f;
            maxCanAscendMultiplier = 1f;
            maxAscentMultiplier = 3f;
            constantAscend = 0.2f;
        }

        public override void HorizontalWingSpeeds(Player player, ref float speed, ref float acceleration)
        {
            speed = 2f;
            acceleration = 3f;
        }

        public override void EquipFrameEffects(Player player, EquipType type)
        {
            if (player.equippedWings != null)
            {
                if (player.equippedWings.wingSlot == player.wingsLogic)
                {
                    player.wings = Item.wingSlot;
                    player.cWings = player.cBody;
                }
            }

            if (player.wingsLogic == Item.wingSlot && player.wings <= 0)
                player.wings = Item.wingSlot;

            if (player.body == Item.bodySlot)
            {
                player.waist = EquipLoader.GetEquipSlot(Mod, Name, EquipType.Waist);
                player.cWaist = player.cBody;
            }
        }
    }
}
