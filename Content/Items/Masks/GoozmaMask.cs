using CalamityHunt.Content.Bosses.Goozma;
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
using Terraria.Graphics.Shaders;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityHunt.Content.Items.Masks
{
    [AutoloadEquip(EquipType.Head)]
    public class GoozmaMask : ModItem
    {
        public override void SetDefaults()
        {
            Item.width = 22;
            Item.height = 24;
            Item.vanity = true;
            Item.rare = ItemRarityID.Blue;
        }

        public override void PostDrawInInventory(SpriteBatch spriteBatch, Vector2 position, Rectangle frame, Color drawColor, Color itemColor, Vector2 origin, float scale)
        {
            for (int i = 0; i < 3; i++)
            {
                Asset<Texture2D> glow = ModContent.Request<Texture2D>(Texture + "Glow" + i);
                Color color = new GradientColor(SlimeUtils.GoozColorArray, 0.2f, 0.2f).ValueAt(Main.GlobalTimeWrappedHourly * 30f + i * 5f) * 0.8f;
                spriteBatch.Draw(glow.Value, position, frame, color, 0, origin, scale, 0, 0);
                spriteBatch.Draw(glow.Value, position, frame, new Color(color.R, color.G, color.B, 0) * 0.5f, 0, origin, scale, 0, 0);
            }
        }

        public override void PostDrawInWorld(SpriteBatch spriteBatch, Color lightColor, Color alphaColor, float rotation, float scale, int whoAmI)
        {
            for (int i = 0; i < 3; i++)
            {
                Asset<Texture2D> glow = ModContent.Request<Texture2D>(Texture + "Glow" + i);
                Color color = new GradientColor(SlimeUtils.GoozColorArray, 0.2f, 0.2f).ValueAt(Main.GlobalTimeWrappedHourly * 30f + i * 5f) * 0.8f;
                spriteBatch.Draw(glow.Value, Item.Center - Main.screenPosition, null, color, rotation, Item.Size * 0.5f, scale, 0, 0);
                spriteBatch.Draw(glow.Value, Item.Center - Main.screenPosition, null, new Color(color.R, color.G, color.B, 0) * 0.5f, rotation, Item.Size * 0.5f, scale, 0, 0);
            }
        }
    }

    public class GoozmaMaskGlow : PlayerDrawLayer
    {
        public override Position GetDefaultPosition() => new AfterParent(PlayerDrawLayers.Head);

        public override bool IsHeadLayer => true;

        public override bool GetDefaultVisibility(PlayerDrawSet drawInfo) => drawInfo.drawPlayer.armor[10].IsAir && drawInfo.drawPlayer.armor[0].ModItem is GoozmaMask || drawInfo.drawPlayer.armor[10].ModItem is GoozmaMask;

        protected override void Draw(ref PlayerDrawSet drawInfo)
        {
            for (int i = 0; i < 3; i++)
            {
                Asset<Texture2D> glow = ModContent.Request<Texture2D>($"{nameof(CalamityHunt)}/Content/Items/Masks/GoozmaMask_HeadGlow" + i);

                Color glowColor = new GradientColor(SlimeUtils.GoozColorArray, 0.2f, 0.2f).ValueAt(Main.GlobalTimeWrappedHourly * 30f + i * 5f) * 0.8f;
                if (drawInfo.cHead > 0)
                    glowColor = new GradientColor(new Color[] { Color.DimGray, Color.DarkGray }, 0.2f, 0.2f).ValueAt(Main.GlobalTimeWrappedHourly * 30f + i * 5f) * 0.8f;

                DrawData glowData1 = new DrawData(glow.Value, drawInfo.HeadPosition(), drawInfo.drawPlayer.legFrame, glowColor, drawInfo.drawPlayer.headRotation, drawInfo.headVect, 1f, drawInfo.playerEffect, 0);
                DrawData glowData2 = new DrawData(glow.Value, drawInfo.HeadPosition(), drawInfo.drawPlayer.legFrame, new Color(glowColor.R, glowColor.G, glowColor.B, 0), drawInfo.drawPlayer.headRotation, drawInfo.headVect, 1f, drawInfo.playerEffect, 0);
                glowData1.shader = drawInfo.cHead;
                glowData2.shader = drawInfo.cHead;
                drawInfo.DrawDataCache.Add(glowData1);
                drawInfo.DrawDataCache.Add(glowData2);
            }
        }
    }
}
