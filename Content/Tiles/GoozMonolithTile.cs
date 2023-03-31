using System;
using System.Collections.Generic;
using Terraria.DataStructures;
using Terraria;
using Terraria.ModLoader;
using Terraria.ObjectData;
using Microsoft.Xna.Framework;
using Terraria.Localization;
using CalamityHunt.Content.Items.Placeable;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using Terraria.ID;
using Terraria.GameContent.ObjectInteractions;
using CalamityHunt.Common.Systems;
using CalamityHunt.Common.Graphics.SlimeMonsoon;
using Terraria.Enums;
using Terraria.Audio;

namespace CalamityHunt.Content.Tiles
{
    public class GoozMonolithTile : ModTile
    {
        public override void SetStaticDefaults()
        {
            Main.tileFrameImportant[Type] = true; 
            TileID.Sets.HasOutlines[Type] = true;
            TileID.Sets.DisableSmartCursor[Type] = true;
            TileObjectData.newTile.CopyFrom(TileObjectData.Style2xX);
            TileObjectData.newTile.Height = 3;
            TileObjectData.newTile.Origin = new Point16(1, 2);
            TileObjectData.newTile.CoordinateHeights = new[] { 16, 16, 18 };
            TileObjectData.newTile.LavaDeath = false;
            TileObjectData.newTile.DrawYOffset = 2;
            TileObjectData.addTile(Type);

            AddMapEntry(new Color(22, 18, 22));

            AnimationFrameHeight = 54;
        }

        public bool active;

        public override void NearbyEffects(int i, int j, bool closer)
        {
            if (active)
            {
                if (closer)
                    Main.LocalPlayer.GetModPlayer<MonolithPlayer>().monolithCount = 1;
            }
        }

        public override void AnimateTile(ref int frame, ref int frameCounter)
        {
            if (active)
            {
                Main.tileFrameCounter[Type]++;
                if (Main.tileFrameCounter[Type] > 5)
                {
                    Main.tileFrameCounter[Type] = 0;
                    Main.tileFrame[Type]++;
                }
                if (Main.tileFrame[Type] > 8 || Main.tileFrame[Type] < 1)
                    Main.tileFrame[Type] = 1;
            }
            else
            {
                Main.tileFrameCounter[Type] = 0;
                Main.tileFrame[Type] = 0;
            }
        }

        public override void MouseOver(int i, int j)
        {
            Player player = Main.LocalPlayer;
            player.cursorItemIconEnabled = true;
            player.cursorItemIconID = ModContent.ItemType<GoozMonolith>();
        }

        public override bool HasSmartInteract(int i, int j, SmartInteractScanSettings settings)
        {
            return true;
        }

        public override bool RightClick(int i, int j)
        {
            active = !active;
            SoundEngine.PlaySound(SoundID.Mech, new Vector2(i * 16, j * 16));
            return true;
        }

        public override void HitWire(int i, int j)
        {
            Tile tile = Main.tile[i, j];

            int spawnX = i;
            int spawnY = j - (tile.TileFrameY % 52) / 18;

            Wiring.SkipWire(spawnX, spawnY);
            Wiring.SkipWire(spawnX, spawnY + 1);
            Wiring.SkipWire(spawnX, spawnY + 2);            
            Wiring.SkipWire(spawnX + 1, spawnY);
            Wiring.SkipWire(spawnX + 1, spawnY + 1);
            Wiring.SkipWire(spawnX + 1, spawnY + 2);

            if (Wiring.CheckMech(spawnX, spawnY, 60))
                RightClick(i, j);

        }

        public override void PostDraw(int i, int j, SpriteBatch spriteBatch)
        {
            Tile tile = Main.tile[i, j];
            Asset<Texture2D> glowTexture = ModContent.Request<Texture2D>(Texture + "Glow");

            Vector2 zero = (Main.drawToScreen ? Vector2.Zero : new Vector2(Main.offScreenRange));
            int height = tile.TileFrameY % AnimationFrameHeight == 36 ? 18 : 16;
            int frameYOffset = Main.tileFrame[Type] * AnimationFrameHeight;

            spriteBatch.Draw(
                glowTexture.Value,
                new Vector2(i * 16 - (int)Main.screenPosition.X, j * 16 - (int)Main.screenPosition.Y) + zero,
                new Rectangle(tile.TileFrameX, tile.TileFrameY + frameYOffset, 16, height),
                new Color(255, 255, 255, 0), 0f, Vector2.Zero, 1f, SpriteEffects.None, 0f);
        }
    }
}
