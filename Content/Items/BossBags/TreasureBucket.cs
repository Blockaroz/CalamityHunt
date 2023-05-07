using CalamityHunt.Common.Systems;
using CalamityHunt.Content.Bosses.Goozma;
using CalamityHunt.Content.Items.Masks;
using CalamityHunt.Content.Items.Materials;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using System;
using System.Collections.Generic;
using Terraria;
using Terraria.GameContent.ItemDropRules;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityHunt.Content.Items.BossBags
{
    public class TreasureBucket : ModItem
    {
        public override void SetStaticDefaults()
        {
            ItemID.Sets.BossBag[Type] = true;
            ItemID.Sets.OpenableBag[Type] = true;
        }

        public override void SetDefaults()
        {
            Item.width = 42;
            Item.height = 46;
            Item.maxStack = Item.CommonMaxStack;
            Item.rare = ItemRarityID.Expert;
            Item.expert = true;
        }

        public override void ModifyItemLoot(ItemLoot itemLoot)
        {
            itemLoot.Add(ItemDropRule.Common(ModContent.ItemType<EntropyMatter>(), 1, 20, 30));
            itemLoot.Add(ItemDropRule.FewFromOptions(1, 7, ModContent.ItemType<EbonianMask>(), ModContent.ItemType<DivineMask>(), ModContent.ItemType<CrimulanMask>(), ModContent.ItemType<StellarMask>()));
        }

        public override void PostDrawInInventory(SpriteBatch spriteBatch, Vector2 position, Rectangle frame, Color drawColor, Color itemColor, Vector2 origin, float scale)
        {
            Asset<Texture2D> glow = ModContent.Request<Texture2D>(Texture + "Glow");
            Color color = new GradientColor(SlimeUtils.GoozColors, 0.2f, 0.2f).ValueAt(Main.GlobalTimeWrappedHourly * 20f);
            spriteBatch.Draw(glow.Value, position, frame, color * 0.8f, 0, origin, scale, 0, 0);
            spriteBatch.Draw(glow.Value, position, frame, new Color(color.R, color.G, color.B, 0), 0, origin, scale, 0, 0);
        }

        public override void PostDrawInWorld(SpriteBatch spriteBatch, Color lightColor, Color alphaColor, float rotation, float scale, int whoAmI)
        {
            Asset<Texture2D> glow = ModContent.Request<Texture2D>(Texture + "Glow");
            Color color = new GradientColor(SlimeUtils.GoozColors, 0.2f, 0.2f).ValueAt(Main.GlobalTimeWrappedHourly * 20f);
           
            spriteBatch.Draw(glow.Value, Item.Center - Main.screenPosition, null, color * 0.5f, rotation, Item.Size * 0.5f, scale, 0, 0);
            spriteBatch.Draw(glow.Value, Item.Center - Main.screenPosition, null, new Color(color.R, color.G, color.B, 0), rotation, Item.Size * 0.5f, scale, 0, 0);
        }
    }
}
