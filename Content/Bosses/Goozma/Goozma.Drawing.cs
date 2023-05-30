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
using System.Drawing.Drawing2D;
using Terraria.Graphics.Shaders;

namespace CalamityHunt.Content.Bosses.Goozma
{
    //IbanPlay shader code
    public partial class Goozma : ModNPC
    {
        private static float maxBright;

        public void GetGradientMapValues(out float[] brightnesses, out Vector3[] colors)
        {
            maxBright = 0.667f;
            brightnesses = new float[10];
            colors = new Vector3[10];

            float rainbowStartOffset = 0.35f + Main.GlobalTimeWrappedHourly * 0.5f % (maxBright * 2f);
            //Calculate and store every non-modulo brightness, with the shifting offset. 
            //The first brightness is ignored for the moment, it will be relevant later. Setting it to -1 temporarily
            brightnesses[0] = -1;
            brightnesses[1] = rainbowStartOffset + 0.35f;
            brightnesses[2] = rainbowStartOffset + 0.42f;
            brightnesses[3] = rainbowStartOffset + 0.47f;
            brightnesses[4] = rainbowStartOffset + 0.51f;
            brightnesses[5] = rainbowStartOffset + 0.56f;
            brightnesses[6] = rainbowStartOffset + 0.61f;
            brightnesses[7] = rainbowStartOffset + 0.64f;
            brightnesses[8] = rainbowStartOffset + 0.72f;
            brightnesses[9] = rainbowStartOffset + 0.75f;

            //Pass the entire rainbow through modulo 1
            for (int i = 1; i < 10; i++)
                brightnesses[i] = HuntOfTheOldGodUtils.Modulo(brightnesses[i], maxBright) * maxBright;

            //Store the first element's value so we can find it again later
            float firstBrightnessValue = brightnesses[1];

            //Sort the values from lowest to highest
            Array.Sort(brightnesses);

            //Find the new index of the original first element after the list being sorted
            int rainbowStartIndex = Array.IndexOf(brightnesses, firstBrightnessValue);
            //Substract 1 from the index, because we are ignoring the currently negative first array slot.
            rainbowStartIndex--;

            //9 loop, filling a list of colors in a array of 10 elements (ignoring the first one)
            for (int i = 0; i < 9; i++)
            {
                colors[1 + (rainbowStartIndex + i) % 9] = SlimeUtils.GoozColorsVector3[i];
            }

            //We always want a brightness at index 0 to be the lower bound
            brightnesses[0] = 0;
            //Make the color at index 0 be a mix between the first and last colors in the list, based on the distance between the 2.
            float interpolant = (1 - brightnesses[9]) / (brightnesses[1] + (1 - brightnesses[9]));
            colors[0] = Vector3.Lerp(colors[9], colors[0], interpolant);
        }

        public override bool PreDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {
            Asset<Texture2D> crownMask = ModContent.Request<Texture2D>($"{nameof(CalamityHunt)}/Assets/Textures/Goozma/Crowns/GoozmaCrown_Mask");
            Asset<Texture2D> ornament = ModContent.Request<Texture2D>($"{nameof(CalamityHunt)}/Assets/Textures/Goozma/Crowns/GoozmaOrnament");
            Asset<Texture2D> glow = ModContent.Request<Texture2D>($"{nameof(CalamityHunt)}/Assets/Textures/Goozma/GlowSoft");
            Asset<Texture2D> flare = TextureAssets.Extra[98];

            //Rectangle ornamentBase = ornament.Frame(1, 2, 0, 0);
            //Rectangle ornamentGlow = ornament.Frame(1, 2, 0, 1);

            SpriteEffects direction = NPC.direction < 0 ? SpriteEffects.None : SpriteEffects.FlipHorizontally;
            Color glowColor = new GradientColor(SlimeUtils.GoozColors, 0.2f, 0.2f).ValueAt(NPC.localAI[0]);
            glowColor.A = 0;
            //config?
            //Asset<Texture2D> stick = TextureAssets.FishingLine;
            //spriteBatch.Draw(stick.Value, NPC.Center - Main.screenPosition, null, Color.DimGray, 0, TextureAssets.FishingLine.Size() * new Vector2(0.5f, 0f), new Vector2(6f, 150f / stick.Height() + 10 * NPC.scale), 0, 0);
            //spriteBatch.Draw(stick.Value, NPC.Center - Main.screenPosition, null, Color.LightGray, 0, TextureAssets.FishingLine.Size() * new Vector2(0.5f, 0f), new Vector2(4f, 148f / stick.Height() + 10 * NPC.scale), 0, 0);

            if (oldVel == null)
                oldVel = new Vector2[NPCID.Sets.TrailCacheLength[Type]];

            float trailStrength = Phase > 1 ? 1.3f : 0.5f;

            if (!(Phase == 2 && Attack == (int)AttackList.FusionRay && Time > 520))
                drawVelocity = NPC.velocity;

            //FlipShadersOnOff(spriteBatch, null, true);
            //GameShaders.Armor.Apply(ContentSamples.CommonlyUsedContentSamples.ColorOnlyShaderIndex, NPC);

            if (NPC.IsABestiaryIconDummy)
                headScale = 1f;
            else
            {
                for (int i = 0; i < NPCID.Sets.TrailCacheLength[Type]; i++)
                {
                    Color trailColor = new GradientColor(SlimeUtils.GoozColors, 0.2f, 0.2f).ValueAt(i * 4f - NPC.localAI[0]) * ((float)(NPCID.Sets.TrailCacheLength[Type] - i) / NPCID.Sets.TrailCacheLength[Type]);
                    trailColor.A = 0;
                    DrawGoozma(spriteBatch, screenPos, NPC.oldPos[i] + NPC.Size * 0.5f, NPC.oldRot[i], oldVel[i], oldTentacleVel[i], trailColor * trailStrength * NPC.scale);
                }
            }

            for (int i = 0; i < 8; i++)
            {
                Vector2 off = new Vector2(2).RotatedBy(MathHelper.TwoPi / 8f * i + NPC.rotation);
                DrawGoozma(spriteBatch, screenPos, NPC.Center + off, NPC.rotation, drawVelocity, tentacleVelocity, glowColor);
            }

            //FlipShadersOnOff(spriteBatch, null, false);

            GetGradientMapValues(out float[] brightnesses, out Vector3[] colors);

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

            spriteBatch.Draw(glow.Value, NPC.Center - screenPos, null, glowColor * 0.2f, extraTilt + NPC.rotation, glow.Size() * 0.5f, 5f * NPC.scale, 0, 0);
            Effect effect = ModContent.Request<Effect>($"{nameof(CalamityHunt)}/Assets/Effects/HolographEffect", AssetRequestMode.ImmediateLoad).Value;
            effect.Parameters["uTime"].SetValue(Main.GlobalTimeWrappedHourly % 1f); 
            effect.Parameters["colors"].SetValue(colors);
            effect.Parameters["brightnesses"].SetValue(brightnesses);
            effect.Parameters["baseToScreenPercent"].SetValue(1.05f);
            effect.Parameters["baseToMapPercent"].SetValue(-0.05f);
            if (Phase == 3)
            {
                effect.Parameters["baseToScreenPercent"].SetValue(1.05f - Utils.GetLerpValue(80, 180, Time, true) * 1.05f);
                effect.Parameters["baseToMapPercent"].SetValue(-0.05f + Utils.GetLerpValue(20, 150, Time, true) * 0.1f);//-1f + Utils.GetLerpValue(20, 180, Time, true)
            }

            FlipShadersOnOff(spriteBatch, effect, false);
            DrawGoozma(spriteBatch, screenPos, NPC.Center, NPC.rotation, drawVelocity, tentacleVelocity, Color.White);
            FlipShadersOnOff(spriteBatch, null, false);

            Vector2 crownPos = NPC.Center + drawOffset - new Vector2(6 * NPC.direction, 44).RotatedBy(extraTilt * 0.8f + NPC.rotation) * headScale * NPC.scale;
            spriteBatch.Draw(crownMask.Value, crownPos - screenPos, null, Color.White, extraTilt + NPC.rotation, crownMask.Size() * new Vector2(0.5f, 1f), NPC.scale, direction, 0);

            Vector2 eyePos = NPC.Center + drawOffset + new Vector2(15 * NPC.direction, -22).RotatedBy(extraTilt * 0.9f + NPC.rotation) * headScale * NPC.scale;
            float eyeRot = -MathHelper.PiOver4;
            float eyeScale = (1.1f + (float)Math.Sin(NPC.localAI[0] * 0.025f % MathHelper.TwoPi) * 0.15f);
            //spriteBatch.Draw(flare.Value, eyePos - Main.screenPosition, null, new Color(255, 255, 255, 0), eyeRot + MathHelper.PiOver2, flare.Size() * 0.5f, eyeScale * new Vector2(0.7f, 0.8f), 0, 0);
            //spriteBatch.Draw(flare.Value, eyePos - Main.screenPosition, null, new Color(255, 255, 255, 0), eyeRot, flare.Size() * 0.5f, eyeScale * new Vector2(0.7f, 0.8f), 0, 0);
            //spriteBatch.Draw(flare.Value, eyePos - Main.screenPosition, null, Color.Lerp(glowColor, new Color(255, 255, 255, 0), 0.2f), eyeRot + MathHelper.PiOver2, flare.Size() * 0.5f, eyeScale * new Vector2(0.5f, 1.5f) + new Vector2(0, eyePower.Y), 0, 0);
            //spriteBatch.Draw(flare.Value, eyePos - Main.screenPosition, null, Color.Lerp(glowColor, new Color(255, 255, 255, 0), 0.2f), eyeRot, flare.Size() * 0.5f, eyeScale * new Vector2(0.5f, 1.1f) + new Vector2(0, eyePower.X), 0, 0);

            Asset<Texture2D> godEye = ModContent.Request<Texture2D>($"{nameof(CalamityHunt)}/Assets/Textures/SpecialEye");

            if (Phase == 1 && Time >= 50)
            {
                eyeScale *= (float)Math.Sqrt(Utils.GetLerpValue(300, 310, Time, true));
                float eyeFlash = Utils.GetLerpValue(300, 360, Time, true);
                spriteBatch.Draw(godEye.Value, eyePos - screenPos, null, glowColor * (1f - eyeFlash) * 0.7f, eyeRot, godEye.Size() * 0.5f, eyeScale * (1f + eyePower.Length() * 0.06f + eyeFlash * 3f), 0, 0);
            }

            spriteBatch.Draw(godEye.Value, eyePos - screenPos, null, Color.Black * 0.5f, eyeRot, godEye.Size() * 0.5f, eyeScale * (1f + eyePower.Length() * 0.06f), 0, 0);
            spriteBatch.Draw(godEye.Value, eyePos - screenPos, null, new Color(200, 200, 200, 0), eyeRot, godEye.Size() * 0.5f, eyeScale * 0.95f * (1f + eyePower.Length() * 0.06f), 0, 0);
            spriteBatch.Draw(godEye.Value, eyePos - screenPos, null, glowColor, eyeRot, godEye.Size() * 0.5f, eyeScale * (1f + eyePower.Length() * 0.06f), 0, 0);

            spriteBatch.Draw(flare.Value, eyePos - screenPos, null, glowColor * 0.15f, eyeRot + MathHelper.PiOver2, flare.Size() * 0.5f, eyeScale * new Vector2(0.33f, 3f) + new Vector2(0, eyePower.Y), 0, 0);
            spriteBatch.Draw(flare.Value, eyePos - screenPos, null, glowColor * 0.15f, eyeRot, flare.Size() * 0.5f, eyeScale * new Vector2(0.33f, 4f) + new Vector2(0, eyePower.X), 0, 0);
            spriteBatch.Draw(flare.Value, eyePos - screenPos, null, glowColor * 0.15f, eyeRot + MathHelper.PiOver4, flare.Size() * 0.5f, eyeScale * new Vector2(0.33f, 3f) + new Vector2(0, eyePower.X), 0, 0);
            spriteBatch.Draw(flare.Value, eyePos - screenPos, null, glowColor * 0.15f, eyeRot - MathHelper.PiOver4, flare.Size() * 0.5f, eyeScale * new Vector2(0.33f, 3f) + new Vector2(0, eyePower.Y), 0, 0);


            spriteBatch.Draw(glow.Value, eyePos - screenPos, null, glowColor * 0.2f, extraTilt + NPC.rotation, glow.Size() * 0.5f, 2f * eyeScale, 0, 0);
            spriteBatch.Draw(glow.Value, eyePos - screenPos, null, glowColor * 0.05f, extraTilt + NPC.rotation, glow.Size() * 0.5f, 5f * eyeScale, 0, 0);

            return false;
        }

        public void FlipShadersOnOff(SpriteBatch spriteBatch, Effect effect, bool immediate)
        {
            if (NPC.IsABestiaryIconDummy)
            {
                RasterizerState priorRasterizer = spriteBatch.GraphicsDevice.RasterizerState;
                Rectangle priorScissorRectangle = spriteBatch.GraphicsDevice.ScissorRectangle;
                spriteBatch.End();
                spriteBatch.GraphicsDevice.RasterizerState = priorRasterizer;
                spriteBatch.GraphicsDevice.ScissorRectangle = priorScissorRectangle;
                spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, Main.DefaultSamplerState, DepthStencilState.None, priorRasterizer, effect, Main.UIScaleMatrix);
            }
            else
            {
                spriteBatch.End();
                SpriteSortMode sortMode = SpriteSortMode.Deferred;
                if (immediate)
                    sortMode = SpriteSortMode.Immediate;
                spriteBatch.Begin(sortMode, BlendState.AlphaBlend, Main.DefaultSamplerState, DepthStencilState.None, Main.Rasterizer, effect, Main.Transform);
            }
        }

        private void DrawGoozma(SpriteBatch spriteBatch, Vector2 screenPos, Vector2 position, float rotation, Vector2 velocity, Vector2 tentacleVelocity, Color color)
        {
            Asset<Texture2D> texture = ModContent.Request<Texture2D>(Texture);
            Asset<Texture2D> dress = ModContent.Request<Texture2D>($"{nameof(CalamityHunt)}/Content/Bosses/Goozma/GoozmaDress");
            Asset<Texture2D> crown = ModContent.Request<Texture2D>($"{nameof(CalamityHunt)}/Assets/Textures/Goozma/Crowns/GoozmaCrown");
            Asset<Texture2D> tentacle = ModContent.Request<Texture2D>($"{nameof(CalamityHunt)}/Content/Bosses/Goozma/GoozmaTentacle");

            SpriteEffects direction = NPC.direction < 0 ? SpriteEffects.None : SpriteEffects.FlipHorizontally;

            Vector2 crownPos = position + drawOffset - new Vector2(6 * NPC.direction, 44).RotatedBy(extraTilt * 0.8f + rotation) * headScale * NPC.scale;
            Vector2 dressPos = position + drawOffset + new Vector2(4 * NPC.direction, 16).RotatedBy(-extraTilt * 0.4f + rotation) * headScale * NPC.scale;

            Vector2 basePos = position + new Vector2(0, 10).RotatedBy(-extraTilt * 0.4f + rotation) * NPC.scale;

            float tentaCount = 5;
            int segmentCount = 11;
            if (tentacle != null)
            {
                for (int j = 0; j < tentaCount; j++)
                {
                    SpriteEffects spriteEffects = NPC.direction > 0 ? SpriteEffects.FlipHorizontally : SpriteEffects.None;
                    float rot = rotation + (0.4f - (j / tentaCount) * 0.6f) * NPC.direction + MathHelper.PiOver2;
                    Vector2 pos = basePos + new Vector2(0, 20).RotatedBy(MathHelper.Lerp(0.5f, -1.3f, j / tentaCount) * NPC.direction + rotation + extraTilt) * NPC.scale;
                    Vector2 stick = (rot.ToRotationVector2() * 12 - tentacleVelocity * 0.01f) * (0.5f + headScale * 0.5f) * NPC.scale;
                    int segments = segmentCount - Math.Clamp(Math.Abs(j - (int)(tentaCount / 2f)), 1, 2);
                    Vector2 lastPos = pos;
                    float freq = 6f;
                    for (int i = 0; i < segments; i++)
                    {
                        float prog = i / (float)segments;
                        Rectangle frame = tentacle.Frame(1, 10, 0, Math.Clamp((int)(prog * 8f) + 1, 2, 9));
                        if (i == 0)
                            frame = tentacle.Frame(1, 5, 0, 0);
                        if (i >= segments - 1)
                            frame = tentacle.Frame(1, 5, 0, 4);

                        float tentacleLerp = Utils.GetLerpValue(tentaCount * 0.5f, tentaCount, j - Math.Abs(tentacleVelocity.X) * 0.001f * (1f - prog)) * (i / (float)segments);
                        float newRot = Math.Clamp(tentacleVelocity.X * 0.01f, -1, 1) * prog - Math.Clamp(tentacleVelocity.Y * 0.015f, -0.6f, 1f) * tentacleLerp * NPC.direction;
                        float tentacleWobble = (float)Math.Sin((NPC.localAI[0] * 0.06f - prog * freq - (j / tentaCount) * 0.5f) % MathHelper.TwoPi) * (1f - tentacleVelocity.Length() * 0.01f);
                        Vector2 nextStick = stick.RotatedBy(newRot + tentacleWobble * tentacleLerp);
                        Vector2 stretch = new Vector2(1f * NPC.scale * (0.6f + prog * 0.7f), lastPos.Distance(lastPos + nextStick) / (frame.Height - 4)) * MathHelper.Lerp(headScale, 1f, i / (float)segments);
                        if (i == 0 || i >= segments - 1)
                            stretch = new Vector2(NPC.scale);

                        float stickRot = lastPos.AngleTo(lastPos + nextStick);
                        lastPos += nextStick;
                        Color tentaColor = Color.Lerp(Color.Black * (color.A / 255), color, (float)Math.Sqrt(prog));
                        spriteBatch.Draw(tentacle.Value, lastPos + drawOffset - screenPos, frame, tentaColor, stickRot - MathHelper.PiOver2, frame.Size() * new Vector2(0.5f, 0f), stretch, spriteEffects, 0);
                    }
                }

            }

            spriteBatch.Draw(dress.Value, dressPos - screenPos, null, color, extraTilt * 0.2f + rotation + (float)Math.Sin(NPC.localAI[0] * 0.35f % MathHelper.TwoPi) * 0.02f, dress.Size() * new Vector2(0.5f, 0f), headScale * NPC.scale, direction, 0);

            if (cordTarget != null)
            {
                if (NPC.IsABestiaryIconDummy)
                    spriteBatch.Draw(cordTarget, position, null, color, 0, cordTarget.Size() * 0.5f, NPC.scale * 2f, 0, 0);

                else
                    spriteBatch.Draw(cordTarget, position - screenPos, null, color, 0, cordTarget.Size() * 0.5f, NPC.scale * 2f, 0, 0);
            }

            spriteBatch.Draw(texture.Value, position + drawOffset - screenPos, null, color, extraTilt * 0.9f + rotation, texture.Size() * 0.5f, headScale * NPC.scale, direction, 0);
            spriteBatch.Draw(crown.Value, crownPos - screenPos, null, color, extraTilt + rotation, crown.Size() * new Vector2(0.5f, 1f), NPC.scale, direction, 0);
        } 
    }
}
