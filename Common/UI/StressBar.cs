using CalamityHunt.Common.Players;
using CalamityHunt.Common.Systems;
using CalamityHunt.Content.Items.Weapons.Magic;
using CalamityHunt.Content.Items.Weapons.Melee;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.ModLoader;
using Terraria.UI;

namespace CalamityHunt.Common.UI
{
    public class StressBar : ModSystem
    {
        private static bool active;

        private static Color baseColor;
        private static Color fillColor;
        public static int style;
        public static float fillPercent;

        public override void UpdateUI(GameTime gameTime)
        {
            style = 0;
            baseColor = Color.Black;
            fillColor = Color.White;
            fillColor.A = 128;
            fillPercent = Main.LocalPlayer.GetModPlayer<SplendorJamPlayer>().stress;
            if (Main.LocalPlayer.GetModPlayer<SplendorJamPlayer>().active)
            {
                if (Main.LocalPlayer.GetModPlayer<SplendorJamPlayer>().stressedOut || Main.LocalPlayer.GetModPlayer<SplendorJamPlayer>().stress >= 1)
                {
                    fillColor = Main.DiscoColor;
                    return;
                }
                if (Main.LocalPlayer.GetModPlayer<SplendorJamPlayer>().stress < 0.25)
                {
                    fillColor = Color.Lerp(Color.Black, Color.Orange, Utils.GetLerpValue(0f, 0.25f, Main.LocalPlayer.GetModPlayer<SplendorJamPlayer>().stress, true));
                }
                else if (Main.LocalPlayer.GetModPlayer<SplendorJamPlayer>().stress >= 0.25 && Main.LocalPlayer.GetModPlayer<SplendorJamPlayer>().stress < 0.5)
                {
                    fillColor = Color.Lerp(Color.Orange, Color.LimeGreen, Utils.GetLerpValue(0.25f, 0.5f, Main.LocalPlayer.GetModPlayer<SplendorJamPlayer>().stress, true));
                }
                else if (Main.LocalPlayer.GetModPlayer<SplendorJamPlayer>().stress >= 0.5 && Main.LocalPlayer.GetModPlayer<SplendorJamPlayer>().stress < 0.75)
                {
                    fillColor = Color.Lerp(Color.LimeGreen, Color.MediumPurple, Utils.GetLerpValue(0.5f, 0.75f, Main.LocalPlayer.GetModPlayer<SplendorJamPlayer>().stress, true));
                }
                else
                {
                    fillColor = Color.Lerp(Color.MediumPurple, Color.White, Utils.GetLerpValue(0.75f, 1f, Main.LocalPlayer.GetModPlayer<SplendorJamPlayer>().stress, true));
                }
            }
        }
        public override void ModifyInterfaceLayers(List<GameInterfaceLayer> layers)
        {
            if (Main.LocalPlayer.GetModPlayer<SplendorJamPlayer>().active)
            {
                int mouseTextIndex = layers.FindIndex(layer => layer.Name.Equals("Vanilla: Entity Health Bars"));
                if (mouseTextIndex != -1)
                {
                    layers.Insert(mouseTextIndex, new LegacyGameInterfaceLayer(
                        "HuntOfTheOldGod: Stress Bar",
                        delegate
                        {
                            Texture2D bar = ModContent.Request<Texture2D>($"{nameof(CalamityHunt)}/Assets/Textures/ChargeBars/Style0_0").Value;
                            Texture2D barCharge = ModContent.Request<Texture2D>($"{nameof(CalamityHunt)}/Assets/Textures/ChargeBars/Style0_1").Value;

                            int fillAmount = (fillPercent > 0.99f) ? barCharge.Width : (int)(barCharge.Width * fillPercent);
                            Rectangle fillFrame = new Rectangle(0, 0, fillAmount, barCharge.Height);
                            Vector2 position = (Main.LocalPlayer.Center - Main.screenPosition) / Main.UIScale - new Vector2(barCharge.Width / 2f, 64f / Main.UIScale);
                            Main.spriteBatch.Draw(bar, position, bar.Frame(), baseColor, 0, Vector2.Zero, 1f, 0, 0);
                            Main.spriteBatch.Draw(barCharge, position, fillFrame, fillColor, 0, Vector2.Zero, 1f, 0, 0);

                            return true;
                        },
                        InterfaceScaleType.UI));
                }
            }
        }
    }
}
