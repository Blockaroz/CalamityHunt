using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using ReLogic.Content;
using System;
using System.Collections.Generic;
using Terraria;
using Terraria.ModLoader;
using Terraria.GameContent;
using Terraria.Graphics;
using Terraria.ID;
using System.Linq;

namespace CalamityHunt.Content.Bosses.Goozma
{
    //IbanPlay shader code
    public partial class Goozma : ModNPC
    {
        //private static float maxBright;

        //public static float Modulo(float dividend, float divisor) => dividend - (float)Math.Floor(dividend / divisor) * divisor;

        //public static readonly Vector3[] gradientColors = new Vector3[]
        //{
        //    new Color(0, 0, 0).ToVector3(),
        //    new Color(51, 46, 78).ToVector3(),
        //    new Color(113, 53, 146).ToVector3(),
        //    new Color(174, 23, 189).ToVector3(),
        //    new Color(237, 128, 60).ToVector3(),
        //    new Color(247, 255, 101).ToVector3(),
        //    new Color(176, 234, 85).ToVector3(),
        //    new Color(102, 219, 249).ToVector3(),
        //    new Color(0, 0, 0).ToVector3()
        //};

        //public void GetGradientMapValues(out float[] brightnesses, out Vector3[] colors)
        //{
        //    maxBright = 0.66f;
        //    brightnesses = new float[10];
        //    colors = new Vector3[10];

        //    float rainbowStartOffset = 0.35f + NPC.localAI[0] * 0.005f;
        //    //Calculate and store every non-modulo brightness, with the shifting offset. 
        //    //The first brightness is ignored for the moment, it will be relevant later. Setting it to -1 temporarily
        //    brightnesses[0] = -1;
        //    brightnesses[1] = rainbowStartOffset + 0.35f;
        //    brightnesses[2] = rainbowStartOffset + 0.42f;
        //    brightnesses[3] = rainbowStartOffset + 0.47f;
        //    brightnesses[4] = rainbowStartOffset + 0.51f;
        //    brightnesses[5] = rainbowStartOffset + 0.56f;
        //    brightnesses[6] = rainbowStartOffset + 0.61f;
        //    brightnesses[7] = rainbowStartOffset + 0.64f;
        //    brightnesses[8] = rainbowStartOffset + 0.72f;
        //    brightnesses[9] = rainbowStartOffset + 0.75f;

        //    //Pass the entire rainbow through modulo 1
        //    for (int i = 1; i < 10; i++)
        //        brightnesses[i] = Modulo(brightnesses[i], maxBright) * maxBright;

        //    //Store the first element's value so we can find it again later
        //    float firstBrightnessValue = brightnesses[1];

        //    //Sort the values from lowest to highest
        //    Array.Sort(brightnesses);

        //    //Find the new index of the original first element after the list being sorted
        //    int rainbowStartIndex = Array.IndexOf(brightnesses, firstBrightnessValue);
        //    //Substract 1 from the index, because we are ignoring the currently negative first array slot.
        //    rainbowStartIndex--;

        //    //9 loop, filling a list of colors in a array of 10 elements (ignoring the first one)
        //    for (int i = 0; i < 9; i++)
        //    {
        //        colors[1 + (rainbowStartIndex + i) % 9] = gradientColors[i];
        //    }

        //    //We always want a brightness at index 0 to be the lower bound
        //    brightnesses[0] = 0;
        //    //Make the color at index 0 be a mix between the first and last colors in the list, based on the distance between the 2.
        //    float interpolant = (1 - brightnesses[9]) / (brightnesses[1] + (1 - brightnesses[9]));
        //    colors[0] = Vector3.Lerp(colors[9], colors[0], interpolant);
        //}

        public override bool PreDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {
            Asset<Texture2D> crownMask = ModContent.Request<Texture2D>($"{nameof(CalamityHunt)}/Assets/Textures/Goozma/Crowns/GoozmaCrown_Mask");
            Asset<Texture2D> ornament = ModContent.Request<Texture2D>($"{nameof(CalamityHunt)}/Assets/Textures/Goozma/Crowns/GoozmaOrnament");
            Asset<Texture2D> glow = ModContent.Request<Texture2D>($"{nameof(CalamityHunt)}/Assets/Textures/Goozma/GlowSoft");
            Asset<Texture2D> flare = TextureAssets.Extra[98];

            //Rectangle ornamentBase = ornament.Frame(1, 2, 0, 0);
            //Rectangle ornamentGlow = ornament.Frame(1, 2, 0, 1);

            SpriteEffects direction = NPC.direction < 0 ? SpriteEffects.None : SpriteEffects.FlipHorizontally;
            Color glowColor = new GradientColor(SlimeUtils.GoozColorArray, 0.2f, 0.2f).ValueAt(NPC.localAI[0]);
            glowColor.A = 0;
            //config?
            //Asset<Texture2D> stick = TextureAssets.FishingLine;
            //spriteBatch.Draw(stick.Value, NPC.Center - Main.screenPosition, null, Color.DimGray, 0, TextureAssets.FishingLine.Size() * new Vector2(0.5f, 0f), new Vector2(6f, 150f / stick.Height() + 10 * NPC.scale), 0, 0);
            //spriteBatch.Draw(stick.Value, NPC.Center - Main.screenPosition, null, Color.LightGray, 0, TextureAssets.FishingLine.Size() * new Vector2(0.5f, 0f), new Vector2(4f, 148f / stick.Height() + 10 * NPC.scale), 0, 0);

            if (oldVel == null)
                oldVel = new Vector2[NPCID.Sets.TrailCacheLength[Type]];

            if (NPC.IsABestiaryIconDummy)
            {
                NPC.localAI[0] = Main.GlobalTimeWrappedHourly * 3f;
                NPC.localAI[1] = Main.GlobalTimeWrappedHourly;
            }
            float trailStrength = Phase > 1 ? 1.4f : 0.5f;
            for (int i = 0; i < NPCID.Sets.TrailCacheLength[Type]; i++)
            {
                Color trailColor = new GradientColor(SlimeUtils.GoozColorArray, 0.2f, 0.2f).ValueAt(i * 4f - NPC.localAI[0]) * ((float)(NPCID.Sets.TrailCacheLength[Type] - i) / NPCID.Sets.TrailCacheLength[Type]);
                trailColor.A = 0;
                DrawGoozma(spriteBatch, NPC.oldPos[i] + NPC.Size * 0.5f, NPC.oldRot[i], oldVel[i], trailColor * trailStrength);
            }

            //GetGradientMapValues(out float[] brightnesses, out Vector3[] colors);

            //for (int i = 0; i < 8; i++)
            //{
            //    Color ornamentColor = new GradientColor(SlimeUtils.GoozColorArray, 0.2f, 0.2f).ValueAt(NPC.localAI[0] + i * 10f);
            //    ornamentColor.A /= 2;
            //    float ornamentRotation = extraTilt + NPC.rotation + NPC.localAI[1] + MathHelper.TwoPi / 8f * i;
            //    float rotFix = MathHelper.PiOver2 + MathHelper.PiOver4 * NPC.direction;
            //    Vector2 ornamentPos = NPC.Center + (new Vector2(-20 * NPC.direction, -10).RotatedBy(NPC.rotation) + new Vector2(80, 0).RotatedBy(ornamentRotation)) * NPC.scale;

            //    spriteBatch.Draw(ornament.Value, ornamentPos - Main.screenPosition, ornamentBase, Color.White, ornamentRotation + rotFix, ornamentBase.Size() * 0.5f, NPC.scale, direction, 0);
            //    spriteBatch.Draw(ornament.Value, ornamentPos - Main.screenPosition, ornamentGlow, ornamentColor, ornamentRotation + rotFix, ornamentGlow.Size() * 0.5f, NPC.scale, direction, 0);
            //    spriteBatch.Draw(flare.Value, ornamentPos - Main.screenPosition, null, new Color(ornamentColor.R, ornamentColor.G, ornamentColor.B, 0) * 0.2f, ornamentRotation + MathHelper.PiOver2, flare.Size() * 0.5f, NPC.scale * new Vector2(1f, 2f), direction, 0);
            //}

            Effect effect = ModContent.Request<Effect>($"{nameof(CalamityHunt)}/Assets/Effects/HolographEffect", AssetRequestMode.ImmediateLoad).Value;
            effect.Parameters["uTime"].SetValue(NPC.localAI[0] * 0.01f % 1f);
            effect.Parameters["uColorMap"].SetValue(ModContent.Request<Texture2D>($"{nameof(CalamityHunt)}/Assets/Textures/Goozma/GoozmaColorMap").Value);
            effect.Parameters["baseToScreenPercent"].SetValue(1f);
            effect.Parameters["baseToMapPercent"].SetValue(0.3f - (float)Math.Sin(NPC.localAI[0] * 0.01f % MathHelper.TwoPi) * 0.3f);
            if (Phase == 3)
            {
                float fadeDownProg = Utils.GetLerpValue(120, 220, Time, true);
                effect.Parameters["baseToMapPercent"].SetValue(MathHelper.Lerp(0.3f - (float)Math.Sin(Main.GlobalTimeWrappedHourly * 0.1f % MathHelper.TwoPi) * 0.3f, 0f, fadeDownProg));
                effect.Parameters["baseToScreenPercent"].SetValue(1f - fadeDownProg);
            }

            //spriteBatch.End();
            //spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, Main.DefaultSamplerState, DepthStencilState.None, Main.Rasterizer, effect, Main.Transform);

            DrawGoozma(spriteBatch, NPC.Center, NPC.rotation, NPC.velocity, Color.Lerp(drawColor, Color.White, 0.3f));
            
            //spriteBatch.End();
            //spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, Main.DefaultSamplerState, DepthStencilState.None, Main.Rasterizer, null, Main.Transform);
            
            Vector2 crownPos = NPC.Center + drawOffset - new Vector2(6 * NPC.direction, 44).RotatedBy(extraTilt * 0.8f + NPC.rotation) * NPC.scale;
            spriteBatch.Draw(crownMask.Value, crownPos - Main.screenPosition, null, Color.White, extraTilt + NPC.rotation, crownMask.Size() * new Vector2(0.5f, 1f), 1f, direction, 0);
           
            Vector2 eyePos = NPC.Center + drawOffset + new Vector2(15 * NPC.direction, -22).RotatedBy(-extraTilt * 0.4f + NPC.rotation) * NPC.scale;
            float eyeRot = -MathHelper.PiOver4;
            float eyeScale = (1.1f + (float)Math.Sin(NPC.localAI[0] * 0.025f % MathHelper.TwoPi) * 0.15f) * NPC.scale;
            //spriteBatch.Draw(flare.Value, eyePos - Main.screenPosition, null, new Color(255, 255, 255, 0), eyeRot + MathHelper.PiOver2, flare.Size() * 0.5f, eyeScale * new Vector2(0.7f, 0.8f), 0, 0);
            //spriteBatch.Draw(flare.Value, eyePos - Main.screenPosition, null, new Color(255, 255, 255, 0), eyeRot, flare.Size() * 0.5f, eyeScale * new Vector2(0.7f, 0.8f), 0, 0);
            //spriteBatch.Draw(flare.Value, eyePos - Main.screenPosition, null, Color.Lerp(glowColor, new Color(255, 255, 255, 0), 0.2f), eyeRot + MathHelper.PiOver2, flare.Size() * 0.5f, eyeScale * new Vector2(0.5f, 1.5f) + new Vector2(0, eyePower.Y), 0, 0);
            //spriteBatch.Draw(flare.Value, eyePos - Main.screenPosition, null, Color.Lerp(glowColor, new Color(255, 255, 255, 0), 0.2f), eyeRot, flare.Size() * 0.5f, eyeScale * new Vector2(0.5f, 1.1f) + new Vector2(0, eyePower.X), 0, 0);

            //eye alt
            Asset<Texture2D> godEye = ModContent.Request<Texture2D>($"{nameof(CalamityHunt)}/Assets/Textures/SpecialEye");
            spriteBatch.Draw(godEye.Value, eyePos - Main.screenPosition, null, Color.Black * 0.5f, eyeRot, godEye.Size() * 0.5f, eyeScale * (1f + eyePower.Length() * 0.06f), 0, 0);
            spriteBatch.Draw(godEye.Value, eyePos - Main.screenPosition, null, glowColor * 0.7f, eyeRot, godEye.Size() * 0.5f, eyeScale * (1f + eyePower.Length() * 0.06f), 0, 0);
            spriteBatch.Draw(godEye.Value, eyePos - Main.screenPosition, null, new Color(150, 150, 150, 0), eyeRot, godEye.Size() * 0.5f, eyeScale * (1f + eyePower.Length() * 0.06f), 0, 0);

            spriteBatch.Draw(flare.Value, eyePos - Main.screenPosition, null, glowColor * 0.3f, eyeRot + MathHelper.PiOver2, flare.Size() * 0.5f, eyeScale * new Vector2(0.7f, 3f) + new Vector2(0, eyePower.Y), 0, 0);
            spriteBatch.Draw(flare.Value, eyePos - Main.screenPosition, null, glowColor * 0.3f, eyeRot, flare.Size() * 0.5f, eyeScale * new Vector2(0.7f, 4f) + new Vector2(0, eyePower.X), 0, 0);

            spriteBatch.Draw(glow.Value, eyePos - Main.screenPosition, null, glowColor * 0.2f, extraTilt + NPC.rotation, glow.Size() * 0.5f, 2f * eyeScale, 0, 0);
            spriteBatch.Draw(glow.Value, eyePos - Main.screenPosition, null, glowColor * 0.2f, extraTilt + NPC.rotation, glow.Size() * 0.5f, 5f * eyeScale, 0, 0);

            return false;
        }

        private void DrawGoozma(SpriteBatch spriteBatch, Vector2 position, float rotation, Vector2 velocity, Color color)
        {
            Asset<Texture2D> texture = ModContent.Request<Texture2D>(Texture);
            Asset<Texture2D> dress = ModContent.Request<Texture2D>($"{nameof(CalamityHunt)}/Content/Bosses/Goozma/GoozmaDress");
            Asset<Texture2D> crown = ModContent.Request<Texture2D>($"{nameof(CalamityHunt)}/Assets/Textures/Goozma/Crowns/GoozmaCrown");
            Asset<Texture2D> tentacle = ModContent.Request<Texture2D>($"{nameof(CalamityHunt)}/Content/Bosses/Goozma/GoozmaTentacle");

            SpriteEffects direction = NPC.direction < 0 ? SpriteEffects.None : SpriteEffects.FlipHorizontally;

            Vector2 crownPos = position + drawOffset - new Vector2(6 * NPC.direction, 44).RotatedBy(extraTilt * 0.8f + rotation) * NPC.scale;
            Vector2 dressPos = position + drawOffset + new Vector2(4 * NPC.direction, 16).RotatedBy(-extraTilt * 0.4f + rotation) * NPC.scale;

            Vector2 basePos = position + new Vector2(0, 10).RotatedBy(-extraTilt * 0.4f + rotation) * NPC.scale;
            
            float tentaCount = 5;
            for (int j = 0; j < tentaCount; j++)
            {
                float rot = rotation + (0.4f - j * 0.1f) * NPC.direction + MathHelper.PiOver2;
                Vector2 pos = basePos + new Vector2(0, 30).RotatedBy(MathHelper.Lerp(0.5f, -1.3f, j / tentaCount) * NPC.direction + rotation);
                Vector2 stick = (rot.ToRotationVector2() * 12 - velocity * 0.1f) * (0.5f + NPC.scale * 0.5f);
                int segments = 12 - Math.Clamp(Math.Abs(j - (int)(tentaCount / 2f)), 1, 2);

                Vector2 lastPos = pos;
                for (int i = 0; i < segments; i++)
                {
                    float prog = i / (float)segments;
                    int segFrame = Math.Clamp((int)(prog * 5f), 1, 3);
                    if (i == 0)
                        segFrame = 0;
                    if (i == segments - 1)
                        segFrame = 4;
                    Rectangle frame = tentacle.Frame(1, 5, 0, segFrame);
                    Vector2 nextStick = stick.RotatedBy(Math.Clamp(velocity.X * 0.03f, -1, 1) * prog + (float)Math.Sin((NPC.localAI[0] * 0.06 - i * 0.8f) % MathHelper.TwoPi) * 1f * Utils.GetLerpValue(tentaCount / 2f, tentaCount, j - velocity.X * 0.001f * NPC.direction * (1f - prog)) * (i / (float)segments));
                    float stickRot = lastPos.AngleTo(lastPos + nextStick);
                    Vector2 stretch = new Vector2(1f, 0.5f + lastPos.Distance(lastPos + nextStick) / 16f) * MathHelper.Lerp(NPC.scale, 1f, i / (float)segments);
                    lastPos += nextStick;
                    spriteBatch.Draw(tentacle.Value, lastPos + drawOffset * (1f - prog) - Main.screenPosition, frame, color, stickRot - MathHelper.PiOver2, frame.Size() * 0.5f, stretch, 0, 0);
                }
            }

            if (cordTarget != null)
                spriteBatch.Draw(cordTarget, Vector2.Zero + (position - NPC.Center), null, color * 2f, 0, Vector2.Zero, 2f, 0, 0);

            spriteBatch.Draw(dress.Value, dressPos - Main.screenPosition, null, color, -extraTilt * 0.66f + rotation + (float)Math.Sin(NPC.localAI[0] * 0.35f % MathHelper.TwoPi) * 0.02f, dress.Size() * new Vector2(0.5f, 0f), NPC.scale, direction, 0);
            spriteBatch.Draw(texture.Value, position + drawOffset - Main.screenPosition, null, color, rotation, texture.Size() * 0.5f, NPC.scale, direction, 0);
            spriteBatch.Draw(crown.Value, crownPos - Main.screenPosition, null, color, extraTilt + rotation, crown.Size() * new Vector2(0.5f, 1f), 1f, direction, 0);
        } 
    }
}
