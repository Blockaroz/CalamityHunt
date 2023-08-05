using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Reflection;
using Terraria;
using Terraria.Chat;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;
using Terraria.ModLoader.Config;
using Terraria.UI;
using Terraria.UI.Chat;

namespace CalamityHunt.Common.Systems
{
    public class WMITFOverride : ModSystem
    {

        static public string MouseText;
        static public bool SecondLine;
        static public bool Hunted;
        public override void ModifyInterfaceLayers(List<GameInterfaceLayer> layers)
        {
            int index = layers.FindIndex(layer => layer.Name.Equals("Vanilla: Mouse Text"));
            if (index != -1 && Hunted)
            {
                layers.RemoveAll(l => l.Name.Equals("WMITF: Mouse Text"));
                layers.Insert(index, new LegacyGameInterfaceLayer("Calamity Hunt: Mouse Text", delegate
                {
                    if (ModLoader.TryGetMod("WMITF", out Mod wmitf) && wmitf.TryFind("Config", out ModConfig config))
                    {
                        var t = config.GetType().GetField("DisplayWorldTooltips", BindingFlags.Public | BindingFlags.Instance);
                        if ((bool)t.GetValue(config) && !String.IsNullOrEmpty(MouseText))
                        {
                            string coloredString = String.Format("[c/{1}:[{0}][c/{1}:]]", MouseText, Colors.RarityBlue.Hex3());
                            var text = ChatManager.ParseMessage(coloredString, Color.White).ToArray();
                            float x = ChatManager.GetStringSize(Terraria.GameContent.FontAssets.MouseText.Value, text, Vector2.One).X;
                            var pos = Main.MouseScreen + new Vector2(16f, 16f);
                            if (pos.Y > (float)(Main.screenHeight - 30))
                                pos.Y = (float)(Main.screenHeight - 30);
                            if (pos.X > (float)(Main.screenWidth - x))
                                pos.X = (float)(Main.screenWidth - x);
                            if (SecondLine)
                                pos.Y += Terraria.GameContent.FontAssets.MouseText.Value.LineSpacing;
                            ChatManager.DrawColorCodedStringWithShadow(Main.spriteBatch, Terraria.GameContent.FontAssets.MouseText.Value, text, pos, 0f, Vector2.Zero, Vector2.One, out int hoveredSnippet);
                        }
                        return true;
                    }
                    return false;
                }, InterfaceScaleType.UI));
            }
        }
    }
    class ItemTooltipOverride : GlobalItem
	{
		public override void ModifyTooltips(Item item, List<TooltipLine> tooltips)
		{
            if (ModLoader.TryGetMod("WMITF", out Mod wmitf) && wmitf.TryFind("Config", out ModConfig config))
			{
                var t = config.GetType().GetField("DisplayItemTooltips", BindingFlags.Public | BindingFlags.Instance);
                if ((bool)t.GetValue(config) && item.ModItem != null)
                {
                    if (item.ModItem.Mod == Mod && !item.Name.Contains("[" + item.ModItem.Mod.Name + "]") && !item.Name.Contains("[" + item.ModItem.Mod.DisplayName + "]"))
                    {
                        tooltips.RemoveAll(i => i.Text.Contains(Mod.Name));
                        tooltips.RemoveAll(i => i.Text.Contains(Mod.DisplayName));
                        var n = config.GetType().GetField("DisplayTechnicalNames", BindingFlags.Public | BindingFlags.Instance);
                        string text = ((bool)n.GetValue(config)) ? ("CalamityMod:" + item.ModItem.Name) : "Calamity Mod";
                        TooltipLine line = new(Mod, Mod.Name, "[" + text + "]");
                        line.OverrideColor = Colors.RarityBlue;
                        tooltips.Add(line);
                    }
                    else if (item.ModItem.Mod == Mod && item.Name.Contains("[" + item.ModItem.Mod.Name + "]"))
                    {
                        TooltipLine tt = tooltips.Find(i => i.Text.Contains(Mod.Name));
                        if (tt.Name.Equals("Terraria.ItemName"))
                        {
                            tt.Text = item.Name + " [CalamityMod]";
                        }
                    }
                    else if (item.ModItem.Mod == Mod && !item.Name.Contains("[" + item.ModItem.Mod.DisplayName + "]"))
                    {
                        TooltipLine tt = tooltips.Find(i => i.Text.Contains(Mod.DisplayName));
                        if (tt.Name.Equals("Terraria.ItemName"))
                        {
                            tt.Text = item.Name + " [Calamity Mod]";
                        }
                    }
                }
            }
		}
    }
    class WorldTooltipsOverride : ModPlayer
    {
        public override void PostUpdate()
        {
            if (!Main.dedServ && ModLoader.TryGetMod("WMITF", out Mod wmitf) && wmitf.TryFind("Config", out ModConfig config))
            {
                WMITFOverride.MouseText = String.Empty;
                WMITFOverride.SecondLine = false;
                WMITFOverride.Hunted = false;

                bool tech = (bool)config.GetType().GetField("DisplayTechnicalNames", BindingFlags.Public | BindingFlags.Instance).GetValue(config);
                var tile = Main.tile[Player.tileTargetX, Player.tileTargetY];
                if (tile.HasTile)
                {
                    var modTile = TileLoader.GetTile(tile.TileType);
                    if (modTile != null)
                    {
                        if (modTile.Mod == Mod)
                        {
                            WMITFOverride.MouseText = tech ? ("CalamityMod:" + modTile.Name) : "Calamity Mod";
                            WMITFOverride.Hunted = true;
                        }
                    }
                }
                else
                {
                    var modWall = WallLoader.GetWall(tile.WallType);
                    if (modWall != null)
                    {
                        if (modWall.Mod == Mod)
                        {
                            WMITFOverride.MouseText = tech ? ("CalamityMod:" + modWall.Name) : "Calamity Mod";
                            WMITFOverride.Hunted = true;
                        }
                    }
                }
                var mousePos = Main.MouseWorld;
                for (int i = 0; i < Main.maxNPCs; i++)
                {
                    var npc = Main.npc[i];
                    if (mousePos.Between(npc.TopLeft, npc.BottomRight))
                    {
                        var modNPC = NPCLoader.GetNPC(npc.type);
                        if (modNPC != null && npc.active && !NPCID.Sets.ProjectileNPC[npc.type])
                        {
                            if (modNPC.Mod == Mod)
                            {
                                WMITFOverride.MouseText = tech ? ("CalamityMod:" + modNPC.Name) : "Calamity Mod";
                                WMITFOverride.SecondLine = true;
                                WMITFOverride.Hunted = true;
                                break;
                            }
                        }
                    }
                }
                if (WMITFOverride.MouseText != String.Empty && Main.mouseText)
                    WMITFOverride.SecondLine = true;
            }
        }
    }
}
