using CalamityHunt.Content.Bosses.Goozma;
using CalamityHunt.Content.Projectiles;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using System;
using System.Collections.Generic;
using System.Linq;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent;
using Terraria.Graphics;
using Terraria.Graphics.Effects;
using Terraria.ModLoader;
using Terraria.ModLoader.Config;
using Terraria.Utilities;

namespace CalamityHunt.Common.Graphics.SlimeMonsoon
{
    public class SlimeMonsoonBackground : CustomSky
    {
        public override void OnLoad()
        {
            lightningTexture = new Asset<Texture2D>[3];
            skyTexture = new Asset<Texture2D>[3];

            lightningTexture[0] = ModContent.Request<Texture2D>($"{nameof(CalamityHunt)}/Assets/Textures/Goozma/Lightning");
            lightningTexture[1] = ModContent.Request<Texture2D>($"{nameof(CalamityHunt)}/Assets/Textures/Goozma/LightningGlow");
            lightningTexture[2] = ModContent.Request<Texture2D>($"{nameof(CalamityHunt)}/Common/Graphics/SlimeMonsoon/Thunder");

            skyTexture[0] = ModContent.Request<Texture2D>($"{nameof(CalamityHunt)}/Common/Graphics/SlimeMonsoon/SkyNoise");
            skyTexture[1] = ModContent.Request<Texture2D>($"{nameof(CalamityHunt)}/Common/Graphics/SlimeMonsoon/DistortNoise");
            skyTexture[2] = ModContent.Request<Texture2D>($"{nameof(CalamityHunt)}/Common/Graphics/SlimeMonsoon/ColorMap");
        }

        public override void Activate(Vector2 position, params object[] args)
        {
            _active = true;
            radialDistortPos = position;
            strengthTarget = 1f;
        }

        public override void Deactivate(params object[] args)
        {
            _active = false;
            strengthTarget = 0;
        }

        public override bool IsActive() => _strength > 0.001f && !Main.gameMenu;

        public override void Reset()
        {
            _active = false;
            strengthTarget = 0;
        }

        private static UnifiedRandom _random = new UnifiedRandom();
        private bool _active;
        private float _strength;
        private float _distanceMod;
        private float _windSpeed;
        private float _brightness;
        public static Vector2 radialDistortPos;
        public static float strengthTarget;

        private static Asset<Texture2D>[] lightningTexture;
        private static Asset<Texture2D>[] skyTexture;

        public override Color OnTileColor(Color inColor)
        {
            if (inColor.R + inColor.G + inColor.B > 20)
            {
                float fastStrength = Math.Clamp(_strength * 3f, 0, 1f);
                Main.ColorOfTheSkies = Color.Lerp(Main.ColorOfTheSkies, Color.Black, fastStrength);
                return inColor.MultiplyRGBA(Color.Lerp(Color.White, new Color(50, 35, 120, 255), fastStrength));
            }
            return inColor;
        }

        public override float GetCloudAlpha() => MathHelper.Lerp(Main.cloudAlpha, 1f, _strength);

        private List<GooThunder>[] thunder;

        public override void Update(GameTime gameTime)
        {
            Main.windSpeedTarget = -0.5f;
            _distanceMod = Utils.GetLerpValue(10000, 9000, Main.LocalPlayer.Distance(radialDistortPos) * 0.5f, true);
            if (!_active)
                _strength = Math.Max(0f, _strength - 0.005f) * _distanceMod;
            else if (strengthTarget != 0f)
            {
                if (_active && _strength < strengthTarget)
                    _strength = Math.Min(strengthTarget * _distanceMod, _strength + 0.005f);
                else
                    _strength = Math.Max(0, _strength - 0.005f);
            }
            if (Main.projectile.Any(n => n.active && n.type == ModContent.ProjectileType<GoozmaSpawn>()))
                radialDistortPos = Vector2.Lerp(radialDistortPos, Vector2.Lerp(Main.LocalPlayer.Center, Main.projectile.FirstOrDefault(n => n.active && n.type == ModContent.ProjectileType<GoozmaSpawn>()).Center, 0.5f), 0.1f);
            else if (Main.npc.Any(n => n.active && n.type == ModContent.NPCType<Goozma>()))
                radialDistortPos = Vector2.Lerp(radialDistortPos, Vector2.Lerp(Main.LocalPlayer.Center, Main.npc.FirstOrDefault(n => n.active && n.type == ModContent.NPCType<Goozma>()).Center, 0.5f), 0.1f);
            else
                radialDistortPos = Vector2.Lerp(radialDistortPos, Main.LocalPlayer.Center, 0.3f);

            _brightness = MathHelper.Lerp(_brightness, 0.15f, 0.08f);
            _windSpeed -= Main.WindForVisuals * 0.0035f;
            _windSpeed = _windSpeed % 1f;

            if (thunder == null)
                thunder = new List<GooThunder>[]
                {
                    new List<GooThunder>(),
                    new List<GooThunder>(),
                    new List<GooThunder>(),
                };

            for (int i = 0; i < thunder.Length; i++)
            {
                if (_random.NextBool(150) && _strength > 0.5f)
                    thunder[i].Add(new GooThunder(Main.screenPosition.X, _random.NextFloat(0.5f, 1.5f), _random.Next(50, 100), i));

                for (int j = 0; j < thunder[i].Count; j++)
                {
                    thunder[i][j].time--;
                    thunder[i][j].strength *= 0.9999f;
                    if (thunder[i][j].time < 0)
                        thunder[i].Remove(thunder[i][j]);

                }
            }
        }

        public override void Draw(SpriteBatch spriteBatch, float minDepth, float maxDepth)
        {
            SkyManager.Instance["Ambience"].Deactivate();
            
            float yOffPower = (float)Utils.GetLerpValue(200, Main.rockLayer - 100, Main.LocalPlayer.Center.Y / 16f, true);
            int yOffset = (int)(yOffPower * 1600f + (float)Math.Sin(Main.GlobalTimeWrappedHourly * 0.275f % MathHelper.TwoPi) * 100f);

            if (maxDepth >= float.MaxValue && minDepth < float.MaxValue)
            {
                spriteBatch.Draw(TextureAssets.BlackTile.Value, new Rectangle(0, 0, Main.screenWidth, Main.screenHeight), Color.Black * (float)Math.Sqrt(_strength));
                spriteBatch.Draw(skyTexture[0].Value, new Rectangle(0, -yOffset, Main.screenWidth, Main.screenHeight * 2), new Color(15, 0, 18) * _strength * 0.66f);
            }

            Effect skyClouds = ModContent.Request<Effect>($"{nameof(CalamityHunt)}/Assets/Effects/SlimeMonsoonCloudLayer", AssetRequestMode.ImmediateLoad).Value;
            skyClouds.Parameters["uTex0"].SetValue(skyTexture[0].Value);
            skyClouds.Parameters["uTex1"].SetValue(skyTexture[1].Value);
            skyClouds.Parameters["uMap"].SetValue(skyTexture[2].Value);
            skyClouds.Parameters["uBrightness"].SetValue(_brightness - yOffPower * 0.1f);

            for (int i = 0; i < 4; i++)
            {
                switch (i)
                {
                    case 0:
                    case 1:
                        if (!(maxDepth >= float.MaxValue && minDepth < float.MaxValue))
                            continue;
                        break;
                    default:
                        if ((maxDepth >= float.MaxValue && minDepth < float.MaxValue))
                            continue;
                        break;
                }
                
                if (i < 3 && thunder != null)
                    thunder[i].ForEach(n => n.Draw(spriteBatch));

                skyClouds.Parameters["uWorldPos"].SetValue(Main.screenPosition / (7000f - i * 500f));
                skyClouds.Parameters["uColorBase"].SetValue(Color.Lerp(new Color(15, 0, 18), new Color(25, 10, 35, 255), Utils.GetLerpValue(0, 4, i, true)).ToVector4());
                skyClouds.Parameters["uTime"].SetValue(_windSpeed + i * 200);
                skyClouds.Parameters["uStrength"].SetValue(Math.Clamp((float)Math.Cbrt(_strength) * 0.9f - Utils.GetLerpValue(0, 2, i, true) * 0.2f, 0.0001f, 1f));

                spriteBatch.End();
                spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, Main.DefaultSamplerState, DepthStencilState.None, Main.Rasterizer, skyClouds, Main.BackgroundViewMatrix.TransformationMatrix);

                spriteBatch.Draw(TextureAssets.BlackTile.Value, new Rectangle(0, 0, Main.screenWidth, Main.screenHeight), Color.White);

                spriteBatch.End();
                spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, Main.DefaultSamplerState, DepthStencilState.None, Main.Rasterizer, null, Main.BackgroundViewMatrix.TransformationMatrix);

            }

            if (Filters.Scene["HuntOfTheOldGods:SlimeMonsoon"].Active)
            {
                Filters.Scene["HuntOfTheOldGods:SlimeMonsoon"].GetShader()
                    .UseColor(Color.White)
                    .UseTargetPosition(radialDistortPos)
                    .UseProgress(Main.GlobalTimeWrappedHourly * 0.005f % 5f)
                    .UseIntensity(1f)
                    .UseOpacity(_strength * 0.1f);
                Filters.Scene["HuntOfTheOldGods:SlimeMonsoon"].GetShader().Shader.Parameters["distortionSample0"].SetValue(skyTexture[1].Value);
                Filters.Scene["HuntOfTheOldGods:SlimeMonsoon"].GetShader().Shader.Parameters["distortionSample1"].SetValue(skyTexture[1].Value);
                Filters.Scene["HuntOfTheOldGods:SlimeMonsoon"].GetShader().Shader.Parameters["distortSize"].SetValue(Vector2.One * 0.4f);
                Filters.Scene["HuntOfTheOldGods:SlimeMonsoon"].GetShader().Shader.Parameters["inEdge"].SetValue(-1f);
                Filters.Scene["HuntOfTheOldGods:SlimeMonsoon"].GetShader().Shader.Parameters["outEdge"].SetValue(0.8f - Main.LocalPlayer.Distance(radialDistortPos) * 0.0001f);
                Filters.Scene["HuntOfTheOldGods:SlimeMonsoon"].GetShader().Shader.Parameters["uSize"].SetValue(new Vector2(1f, 0.7f));

                if (_strength < 0.02f)
                    Filters.Scene["HuntOfTheOldGods:SlimeMonsoon"].Deactivate();
            }
        }

        private class GooThunder
        {
            public GooThunder(float position, float strength, int time, int layer)
            {
                this.position = position / (2 + layer * 3f) + Main.screenWidth / 2 + _random.NextFloat(-1000, 1000);
                rotation = _random.NextFloat(-0.2f, 0.2f);
                this.strength = strength;
                this.time = (int)(time / (0.5f + strength * 0.5f));
                this.layer = layer;
                maxTime = this.time;
                colorOffset = _random.NextFloat(0, 100f);

                points = new List<Vector2>();
                rots = new List<float>();
                int totalPoints = (int)(Main.worldSurface / 13f) + _random.Next(-2, 5);
                for (int i = 0; i < totalPoints; i++)
                {
                    float yPos = MathHelper.Lerp(-3200, (float)Main.worldSurface * 13f, i / (float)totalPoints);
                    points.Add((new Vector2(this.position, yPos) + _random.NextVector2Circular(50, 20)).RotatedBy(rotation));
                }
                for (int i = 0; i < totalPoints - 1; i++)
                    rots.Add(points[i].AngleTo(points[i + 1]));

                rots.Add(points[totalPoints - 2].AngleTo(points[totalPoints - 1]));

                //if (!Main.dedServ)
                //{
                //    SoundStyle thunderSound = new SoundStyle($"{nameof(CalamityHunt)}/Assets/Sounds/SlimeMonsoon/GooThunder", 3, SoundType.Ambient);
                //    SoundEngine.PlaySound(thunderSound, Main.LocalPlayer.Center + new Vector2(position, 0));
                //}
            }

            public float position;
            public float rotation;
            public float strength;
            public int time;
            public int maxTime;
            public float colorOffset;
            private List<Vector2> points;
            private List<float> rots;
            public int layer;

            public void Draw(SpriteBatch spriteBatch)
            {
                VertexStrip strip = new VertexStrip();
                strip.PrepareStrip(points.ToArray(), rots.ToArray(), ColorFunction, WidthFunction, -Main.screenPosition / (2 + layer * 3f), points.Count, true);

                Effect lightningEffect = ModContent.Request<Effect>($"{nameof(CalamityHunt)}/Assets/Effects/GooLightningEffect", AssetRequestMode.ImmediateLoad).Value;
                lightningEffect.Parameters["uTransformMatrix"].SetValue(Main.BackgroundViewMatrix.NormalizedTransformationmatrix);
                lightningEffect.Parameters["uTexture"].SetValue(lightningTexture[0].Value);
                lightningEffect.Parameters["uGlow"].SetValue(lightningTexture[1].Value);
                lightningEffect.Parameters["uColor"].SetValue(Vector3.One);
                lightningEffect.Parameters["uTime"].SetValue(time * 0.0033f);
                lightningEffect.CurrentTechnique.Passes[0].Apply();

                strip.DrawTrail();

                Main.pixelShader.CurrentTechnique.Passes[0].Apply();

                Color drawColor = new GradientColor(SlimeUtils.GoozColorArray, 0.5f, 0.5f).ValueAt(time * 3 + colorOffset);
                drawColor.A = 0;
                float power = Utils.GetLerpValue(maxTime, maxTime * 0.9f, time, true) * Utils.GetLerpValue(0, maxTime, time, true);
                spriteBatch.Draw(lightningTexture[2].Value, new Vector2(points[0].X, 0) - Main.screenPosition / (2 + layer * 3f), null, drawColor * 0.3f * strength * power, rotation, lightningTexture[2].Size() * new Vector2(0.5f, 0f), new Vector2(20 * strength, Main.screenHeight), 0, 0);

            }

            public Color ColorFunction(float progress)
            {
                Color color = new GradientColor(SlimeUtils.GoozColorArray, 0.5f, 0.5f).ValueAt(time * 3 + colorOffset + progress) * (1f / layer);
                color.A /= 2;
                return color * ((float)time / maxTime);
            }

            public float WidthFunction(float progress)
            {
                return 300f * (float)Math.Pow((float)time / maxTime, 0.6f) * Utils.GetLerpValue(maxTime, maxTime * 0.9f, time, true);
            }
        }
    }
}
