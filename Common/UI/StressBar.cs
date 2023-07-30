using CalamityHunt.Common.Players;
using CalamityHunt.Common.Systems;
using CalamityHunt.Content.Items.Weapons.Magic;
using CalamityHunt.Content.Items.Weapons.Melee;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using ReLogic.Content;
using System;
using System.Collections.Generic;
using System.Text;
using Terraria;
using Terraria.ModLoader;
using Terraria.UI;

namespace CalamityHunt.Common.UI
{
    public class StressBar : ModSystem
    {
        private static bool active;

        public static float fillPercent;
        public static float oldPercent;

        private static int stressFrame = 0;
        private static int stressAnimDelay = 0;
        private static int stressTopFrame = -1;
        private static int stressTopTimer = 0;
        public override void UpdateUI(GameTime gameTime)
        {
            fillPercent = Main.LocalPlayer.GetModPlayer<SplendorJamPlayer>().stress;
            oldPercent = Main.LocalPlayer.GetModPlayer<SplendorJamPlayer>().checkStress;
            if (Main.LocalPlayer.GetModPlayer<SplendorJamPlayer>().active)
            {
                stressAnimDelay++;
                if (stressAnimDelay >= 8 - (int)(oldPercent * 4))
                {
                    stressFrame++;
                    stressAnimDelay = 0;
                }
                if (stressFrame >= 5)
                    stressFrame = 0;
                if (fillPercent >= 1f && stressTopFrame <= 10)
                {
                    stressTopTimer++;
                    if (stressTopTimer >= 5)
                    {
                        stressTopFrame++;
                        stressTopTimer = 0;
                    }
                }
                else if (fillPercent <= 1f)
                    stressTopFrame = -1;
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
                            Texture2D bar = ModContent.Request<Texture2D>($"{nameof(CalamityHunt)}/Assets/Textures/ChargeBars/StressBar").Value;
                            Texture2D barCharge = ModContent.Request<Texture2D>($"{nameof(CalamityHunt)}/Assets/Textures/ChargeBars/StressBarFill").Value;
                            Texture2D barTop = ModContent.Request<Texture2D>($"{nameof(CalamityHunt)}/Assets/Textures/ChargeBars/StressBarTopped").Value;

                            Vector2 shake = Main.LocalPlayer.GetModPlayer<SplendorJamPlayer>().stressedOut ? Shake() * oldPercent : Vector2.Zero;

                            Vector2 vector = new Vector2(Config.Instance.stressX, Config.Instance.stressY);
                            if (vector.X < 0f || vector.X > 100f)
                                vector.X = 35.77406f;
                            if (vector.Y < 0f || vector.Y > 100f)
                                vector.Y = 3.97614312f;
                            Vector2 position = new Vector2((int)(vector.X * 0.01f * (Main.screenWidth - bar.Width)), (int)(vector.Y * 0.01f * (Main.screenHeight - (bar.Height / 5))));
                            int fillAmount = (fillPercent > 0.99f) ? 60 + 42 : (int)(60 * fillPercent + 42);

                            Rectangle mouse = new Rectangle((int)Main.MouseScreen.X, (int)Main.MouseScreen.Y, 8, 8);
                            Rectangle value = new Rectangle((int)position.X, (int)position.Y, bar.Width, bar.Height / 5);
                            if (mouse.Intersects(value))
                                Main.instance.MouseText("Stress: " + StressString(fillPercent, 1), 0, 0);

                            Rectangle barFrame = new Rectangle(0, stressFrame * (bar.Height / 5), bar.Width, bar.Height / 5);
                            Rectangle fillFrame = new Rectangle(0, (int)(oldPercent / 0.25f)*(barCharge.Height / 5), fillAmount, barCharge.Height / 5);
                            Rectangle topFrame = new Rectangle(0, stressTopFrame * (barTop.Height / 9), barTop.Width, barTop.Height / 9);

                            Main.spriteBatch.Draw(bar, position + shake, barFrame, Color.White, 0, Vector2.Zero, 1f, 0, 0);
                            Main.spriteBatch.Draw(barCharge, position + shake, fillFrame, Color.White, 0, Vector2.Zero, 1f, 0, 0);
                            Main.spriteBatch.Draw(barTop, position + shake, topFrame, Color.White, 0, Vector2.Zero, 1f, 0, 0);
                            return true;
                        },
                        InterfaceScaleType.UI));
                }
            }
        }
        private static Vector2 Shake()
        {
            float shakeIntensity = Config.Instance.stressShake;
            return new Vector2(Main.rand.NextFloat(0f - shakeIntensity, shakeIntensity), Main.rand.NextFloat(0f - shakeIntensity, shakeIntensity));
        }
        private static string StressString(float value, float maxValue)
        {
            StringBuilder stringBuilder = new StringBuilder(32);
            stringBuilder.Append((100f * value / maxValue).ToString("0.00"));
            stringBuilder.Append("%");
            return stringBuilder.ToString();
        }
    }
}
