﻿using System.Collections.Generic;
using CalamityHunt.Common.Players;
using CalamityHunt.Content.Items.Weapons.Magic;
using CalamityHunt.Content.Items.Weapons.Melee;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ModLoader;
using Terraria.UI;

namespace CalamityHunt.Common.UI;

public class WeaponBar : ModSystem
{
    private static int showTime;
    private static Color baseColor;
    private static Color fillColor;
    private static float fillPercent;
    private static int style;

    public static void DisplayBar(Color baseColor, Color fillColor, float percent, int showTime = 120, int style = 0)
    {
        WeaponBar.showTime = showTime;
        WeaponBar.baseColor = baseColor;
        WeaponBar.fillColor = fillColor;
        WeaponBar.fillPercent = percent;
        WeaponBar.style = style;
    }

    public override void UpdateUI(GameTime gameTime)
    {
        if (showTime > 0) {
            showTime--;
        }

        if (showTime <= 0) {
            style = 0;
            baseColor = Color.DimGray;
            fillColor = Color.White;
        }    
    }

    public override void ModifyInterfaceLayers(List<GameInterfaceLayer> layers)
    {
        if (showTime > 0) {
            int mouseTextIndex = layers.FindIndex(layer => layer.Name.Equals("Vanilla: Entity Health Bars"));
            if (mouseTextIndex != -1) {
                layers.Insert(mouseTextIndex, new LegacyGameInterfaceLayer(
                    "HuntOfTheOldGod: Weapon Charge Bars",
                    delegate
                    {
                        if (showTime > 0) {
                            float fade = Utils.GetLerpValue(0, 30, showTime, true);

                            Texture2D bar = AssetDirectory.Textures.Bars.Bar[style].Value;
                            Texture2D barCharge = AssetDirectory.Textures.Bars.BarFill[style].Value;

                            int fillAmount = (fillPercent > 0.99f) ? barCharge.Width : (int)(barCharge.Width * fillPercent);
                            Rectangle fillFrame = new Rectangle(0, 0, fillAmount, barCharge.Height);
                            Vector2 position = (Main.LocalPlayer.Center - Main.screenPosition) / Main.UIScale - new Vector2(barCharge.Width / 2f, 48f / Main.UIScale);

                            float fadeOut = Utils.GetLerpValue(0, 30, showTime, true);
                            Main.spriteBatch.Draw(bar, position, bar.Frame(), baseColor * fadeOut, 0, Vector2.Zero, 1f, 0, 0);
                            Main.spriteBatch.Draw(barCharge, position, fillFrame, fillColor * fadeOut, 0, Vector2.Zero, 1f, 0, 0);
                        }

                        return true;
                    },
                    InterfaceScaleType.UI));
            }
        }
    }
}
