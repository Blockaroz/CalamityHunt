using CalamityHunt.Content.Bosses.Goozma;
using CalamityHunt.Content.Items.Rarities;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityHunt.Content.Items.Materials
{
    public class EntropyMatter : ModItem
    {
        public override void SetStaticDefaults()
        {
            ItemID.Sets.ItemNoGravity[Type] = true;
            ItemID.Sets.AnimatesAsSoul[Type] = true;
            Main.RegisterItemAnimation(Item.whoAmI, new DrawAnimationVertical(4, 4));
            Item.ResearchUnlockCount = 50;
        }

        public override void SetDefaults()
        {
            Item.width = 24;
            Item.height = 36;
            Item.value = Item.sellPrice(0, 30);
            Item.rare = ModContent.RarityType<VioletRarity>();
            Item.maxStack = Item.CommonMaxStack;
        }

        public override void PostDrawInInventory(SpriteBatch spriteBatch, Vector2 position, Rectangle frame, Color drawColor, Color itemColor, Vector2 origin, float scale)
        {
            Asset<Texture2D> glow = ModContent.Request<Texture2D>(Texture + "Glow");
            Color color = new GradientColor(SlimeUtils.GoozColorArray, 0.2f, 0.2f).ValueAt(Main.GlobalTimeWrappedHourly * 10f);
            spriteBatch.Draw(glow.Value, position, frame, color * 0.8f, 0, origin, scale, 0, 0);
            spriteBatch.Draw(glow.Value, position, frame, new Color(color.R, color.G, color.B, 0), 0, origin, scale, 0, 0);
        }

        public override void PostDrawInWorld(SpriteBatch spriteBatch, Color lightColor, Color alphaColor, float rotation, float scale, int whoAmI)
        {
            Asset<Texture2D> glow = ModContent.Request<Texture2D>(Texture + "Glow");
            Color color = new GradientColor(SlimeUtils.GoozColorArray, 0.2f, 0.2f).ValueAt(Main.GlobalTimeWrappedHourly * 10f);

            spriteBatch.Draw(glow.Value, Item.Center - Main.screenPosition, null, color * 0.5f, rotation, Item.Size * 0.5f, scale, 0, 0);
            spriteBatch.Draw(glow.Value, Item.Center - Main.screenPosition, null, new Color(color.R, color.G, color.B, 0), rotation, Item.Size * 0.5f, scale, 0, 0);
        }
    }
}
