﻿using System;
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
using Terraria.Chat;
using CalamityHunt.Content.Items.Consumable;

namespace CalamityHunt.Content.Tiles
{
    public class SlimeNinjaStatueTile : ModTile
    {
        public override void SetStaticDefaults()
        {
            Main.tileFrameImportant[Type] = true; 
            TileID.Sets.HasOutlines[Type] = true;
            TileID.Sets.DisableSmartCursor[Type] = true;
            TileObjectData.newTile.CopyFrom(TileObjectData.Style1x1);
            TileObjectData.newTile.Width = 4;
            TileObjectData.newTile.Height = 6;
            TileObjectData.newTile.Origin = new Point16(2, 5);
            TileObjectData.newTile.CoordinateHeights = new[] { 16, 16, 16, 16, 16, 16 };
            TileObjectData.newTile.LavaDeath = false;
            TileObjectData.newTile.DrawYOffset = 2;
            TileObjectData.addTile(Type);

            AddMapEntry(new Color(22, 18, 22));
        }

        public override void MouseOver(int i, int j)
        {
            Player player = Main.LocalPlayer;
            player.noThrow = 2;
            player.cursorItemIconEnabled = true;
            player.cursorItemIconID = ModContent.ItemType<SlimeNinjaStatue>();
        }

        public override bool HasSmartInteract(int i, int j, SmartInteractScanSettings settings) => true;

        public override bool RightClick(int i, int j)
        {
            //SoundEngine.PlaySound(SoundID.Mech, new Vector2(i * 16, j * 16));

            int top = j;
            int center = i;

            for (int y = 0; y <= 6; y++)
            {
                Tile checkTile = Framing.GetTileSafely(i, j + y);
                if (checkTile.HasTile && checkTile.TileType == Type)
                    top++;
                else
                    break;
            }

            top -= 5;

            for (int x = 0; x <= 4; x++)
            {
                Tile checkTile = Framing.GetTileSafely(i + x, top);
                if (checkTile.TileType != Type)
                {
                    center += x;
                    center -= 2;
                    break;
                }
                    
            }

            Player player = Main.LocalPlayer;

            if (GoozmaSystem.FindSlimeStatues(center, top, 10, 5))
            {
                if (player.ConsumeItem(ModContent.ItemType<SlimeNinjaStatue>()))
                {

                }
                else if (player.ConsumeItem(ModContent.ItemType<OverloadedSludge>()))
                {

                }
            }
            else
            {
                string text = "The shrine is incomplete without slime statues surrounding it.";
                Color color = new Color(255, 255, 0);
                if (Main.netMode == NetmodeID.Server)
                    ChatHelper.SendChatMessageToClient(NetworkText.FromLiteral(text), color, player.whoAmI);
                else
                    Main.NewText(text, color);
            }

            return true;
        }
    }
}
