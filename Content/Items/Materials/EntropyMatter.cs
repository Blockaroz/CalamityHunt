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
        }

        public override void SetDefaults()
        {
            Item.width = 24;
            Item.height = 36;
            Item.value = Item.sellPrice(0, 30);
            Item.rare = ModContent.RarityType<VioletRarity>();
            Item.ResearchUnlockCount = 250;
            Item.maxStack = Item.CommonMaxStack;
        }

        public override void PostDrawInWorld(SpriteBatch spriteBatch, Color lightColor, Color alphaColor, float rotation, float scale, int whoAmI)
        {
            Asset<Texture2D> glow = ModContent.Request<Texture2D>(Texture + "_Glow");
            for (int i = 0; i < 3; i++)
            {
                Rectangle frame = glow.Frame(1, 3, 0, i);
                Color gradient = new GradientColor(SlimeUtils.GoozColorArray, 1f, 1f).ValueAt(Main.GlobalTimeWrappedHourly * 120 + i * 20);
                gradient.A = 0;
                spriteBatch.Draw(glow.Value, Item.Center - Main.screenPosition, frame, Color.DimGray * 0.5f, rotation, frame.Size() * 0.5f, scale, 0, 0);
                spriteBatch.Draw(glow.Value, Item.Center - Main.screenPosition, frame, gradient, rotation, frame.Size() * 0.5f, scale, 0, 0);
                spriteBatch.Draw(glow.Value, Item.Center - Main.screenPosition, frame, gradient, rotation, frame.Size() * 0.5f, scale * 1.045f, 0, 0);
            }
        }

        public override void PostDrawInInventory(SpriteBatch spriteBatch, Vector2 position, Rectangle frame, Color drawColor, Color itemColor, Vector2 origin, float scale)
        {
            Asset<Texture2D> glow = ModContent.Request<Texture2D>(Texture + "_Glow");
            for (int i = 0; i < 3; i++)
            {
                Rectangle glowFrame = glow.Frame(1, 3, 0, i);
                Color gradient = new GradientColor(SlimeUtils.GoozColorArray, 1f, 1f).ValueAt(Main.GlobalTimeWrappedHourly * 120 - i * 20).MultiplyRGBA(drawColor);
                gradient.A = 0;
                spriteBatch.Draw(glow.Value, position, glowFrame, Color.DimGray * 0.5f, 0, origin, scale, 0, 0);
                spriteBatch.Draw(glow.Value, position, glowFrame, gradient, 0, origin, scale, 0, 0);
                spriteBatch.Draw(glow.Value, position, glowFrame, gradient, 0, origin, scale * 1.045f, 0, 0);
            }
        }
    }
}
