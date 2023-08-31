using CalamityHunt.Common.Systems.Particles;
using CalamityHunt.Content.Bosses.Goozma;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CalamityHunt.Common.Utilities;
using CalamityHunt.Core;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityHunt.Content.Particles
{
    public class GoozGelBit : ParticleBehavior
    {
        private bool colorful;
        private int variant;
        private int direction;
        private float colOffset;
        private int time;
        private Vector2 homePos;

        public override void OnSpawn()
        {
            scale *= Main.rand.NextFloat(1f, 1.1f);
            variant = Main.rand.Next(5);
            if (Main.rand.NextBool(1000))
                variant = Main.rand.Next(5, 8);            

            direction = Main.rand.NextBool() ? 1 : -1;
            colOffset = Main.rand.NextFloat();
            homePos = position;
            colorful = Main.rand.NextBool(2);
            behindEntities = true;
        }

        public override void Update()
        {
            time++;
            rotation += velocity.X * 0.02f;

            if (data is int)
            {
                if (time > 20 && time < (int)data)
                {
                    velocity *= 0.99f;
                    velocity = Vector2.Lerp(velocity, Main.rand.NextVector2Circular(78, 67), 0.02f);
                }
                else if (time > (int)data + 20)
                {
                    velocity = Vector2.Lerp(velocity, position.DirectionTo(homePos).SafeNormalize(Vector2.Zero) * (position.Distance(homePos) + 10) * 0.02f, 0.1f * Utils.GetLerpValue((int)data + 20, (int)data + 40, time, true));
                    velocity = velocity.RotatedBy(0.03f * direction);
                    if (position.Distance(homePos) < 30)
                        scale *= 0.9f;
                }
            }
            else
                velocity *= 0.98f;

            if (scale < 0.5f)
                Active = false;

            if (Main.rand.NextBool(50))
            {
                ParticleBehavior hue = NewParticle(ModContent.GetInstance<HueLightDust>(), position + Main.rand.NextVector2Circular(30, 30), Main.rand.NextVector2Circular(2, 2) - Vector2.UnitY * 2f, Color.White, 1f);
                hue.data = time * 2f + colOffset; 
            }


            if (Main.rand.NextBool(120))
            {
                Vector2 gooVelocity = -velocity.SafeNormalize(Vector2.Zero).RotatedByRandom(0.2f);
                ParticleBehavior goo = NewParticle(ModContent.GetInstance<GooBurst>(), position + Main.rand.NextVector2Circular(30, 30), gooVelocity, Color.White, 0.1f + Main.rand.NextFloat(1.5f));
                goo.data = time * 2f + colOffset;

            }

            if (Main.rand.NextBool(70))
                Dust.NewDustPerfect(position + Main.rand.NextVector2Circular(10, 10), DustID.TintableDust, Main.rand.NextVector2CircularEdge(3, 3), 100, Color.Black, Main.rand.NextFloat(2, 4)).noGravity = true;
        }

        public static Asset<Texture2D> texture;

        public override void Load()
        {
            texture = AssetUtilities.RequestImmediate<Texture2D>(Texture);
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            Texture2D glow = AssetDirectory.Textures.Glow.Value;
            Rectangle frame = texture.Value.Frame(8, 1, variant, 0);

            Color glowColor = new GradientColor(SlimeUtils.GoozColors, 0.2f, 0.2f).ValueAt(time * 2f + colOffset);
            glowColor.A = 0;

            for (int i = 0; i < 4; i++)
            {
                Vector2 off = new Vector2(2).RotatedBy(MathHelper.TwoPi / 4f * i + rotation);
                spriteBatch.Draw(texture.Value, position + off - Main.screenPosition, frame, glowColor, rotation, frame.Size() * 0.5f, scale, 0, 0);
            }

            if (colorful)
            {
                GetGradientMapValues(out float[] brightnesses, out Vector3[] colors);
                Effect effect = ModContent.Request<Effect>($"{nameof(CalamityHunt)}/Assets/Effects/HolographEffect", AssetRequestMode.ImmediateLoad).Value;
                effect.Parameters["uTime"].SetValue(Main.GlobalTimeWrappedHourly % 1f);
                effect.Parameters["colors"].SetValue(colors);
                effect.Parameters["brightnesses"].SetValue(brightnesses);
                effect.Parameters["baseToScreenPercent"].SetValue(1f);
                effect.Parameters["baseToMapPercent"].SetValue(0f);
                spriteBatch.End();
                spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, Main.DefaultSamplerState, DepthStencilState.None, Main.Rasterizer, effect, Main.Transform);
            }

            spriteBatch.Draw(texture.Value, position - Main.screenPosition, frame, Color.Lerp(color, Color.Black, 0.6f), rotation, frame.Size() * 0.5f, scale, 0, 0);

            if (colorful)
            {
                spriteBatch.End();
                spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, Main.DefaultSamplerState, DepthStencilState.None, Main.Rasterizer, null, Main.Transform);
            }
        }

        public void GetGradientMapValues(out float[] brightnesses, out Vector3[] colors)
        {
            float maxBright = 0.667f;
            brightnesses = new float[10];
            colors = new Vector3[10];

            float rainbowStartOffset = 0.35f + time * 0.01f % (maxBright * 2f);
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
    }
}
