using CalamityHunt.Common.Players;
using CalamityHunt.Common.Systems.Particles;
using CalamityHunt.Content.Items.Rarities;
using CalamityHunt.Content.Particles;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.DataStructures;
using Terraria.GameContent;
using Terraria.Graphics;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityHunt.Content.Items.Misc
{
    public class PureAuricSoul : ModItem
    {
        public override void SetStaticDefaults()
        {
            ItemID.Sets.ItemNoGravity[Type] = true;
            ItemID.Sets.ItemsThatShouldNotBeInInventory[Type] = true;
            ItemID.Sets.IgnoresEncumberingStone[Type] = true;
            ItemID.Sets.ItemSpawnDecaySpeed[Type] = 4;

            Main.RegisterItemAnimation(Type, new DrawAnimationVertical(5, 4));
            ItemID.Sets.AnimatesAsSoul[Type] = true;
            ItemID.Sets.ItemIconPulse[Type] = true;
        }

        public override void SetDefaults()
        {
            Item.width = 16;
            Item.height = 16;
            Item.rare = ModContent.RarityType<VioletRarity>();
            if (ModLoader.HasMod("CalamityMod"))
            {
                ModRarity r;
                Mod calamity = ModLoader.GetMod("CalamityMod");
                calamity.TryFind<ModRarity>("Violet", out r);
                Item.rare = r.Type;
            }
        }

        public override bool OnPickup(Player player)
        {
            for (int i = 0; i < 150; i++)
            {
                if (Main.rand.NextBool(15))
                {
                    Vector2 off = Main.rand.NextVector2Circular(20, 20);
                    float scale = Main.rand.NextFloat() + Utils.GetLerpValue(50, 0, off.Length(), true);
                    Particle.NewParticle(Particle.ParticleType<CrossSparkle>(), Item.Center + off, Main.rand.NextVector2Circular(1, 1), GetAlpha(Color.White).Value * 0.2f, scale);
                }

                if (Main.rand.NextBool(5))
                {
                    Vector2 off = Main.rand.NextVector2Circular(20, 20);
                    float scale = Main.rand.NextFloat() + Utils.GetLerpValue(50, 0, off.Length(), true);
                    Particle.NewParticle(Particle.ParticleType<PrettySparkle>(), Item.Center + off, Main.rand.NextVector2Circular(7, 7), GetAlpha(Color.White).Value * 0.2f, scale * 0.6f);
                }

                Dust soul = Dust.NewDustDirect(Item.Center - new Vector2(30, 18), 60, 40, DustID.PortalBoltTrail, 0f, -Main.rand.NextFloat(1f, 2f), 0, GetAlpha(Color.White).Value, Main.rand.NextFloat(2f));
                soul.noGravity = true;
            }
            player.GetModPlayer<AuricSoulPlayer>().pureSoul = true;

            return false;
        }

        public override Color? GetAlpha(Color lightColor)
        {
            Color[] array = new Color[]
            {
                new Color(255, 160, 20, 0),
                new Color(255, 80, 120, 0),
                new Color(255, 160, 20, 0),
                new Color(255, 80, 120, 0),
                new Color(30, 120, 255, 0),
            };
            Color final = new GradientColor(array, 0.15f, 0.5f).ValueAt(Main.GlobalTimeWrappedHourly * 15f);
            return final;
        }

        public override void Update(ref float gravity, ref float maxFallSpeed)
        {
            if (Main.rand.NextBool(15))
            {
                Vector2 off = Main.rand.NextVector2Circular(30, 30);
                float scale = Main.rand.NextFloat() + Utils.GetLerpValue(50, 0, off.Length(), true);
                Particle.NewParticle(Particle.ParticleType<CrossSparkle>(), Item.Center + off, Main.rand.NextVector2Circular(1, 1), GetAlpha(Color.White).Value * 0.2f, scale);
            }                  
            
            if (Main.rand.NextBool(25))
            {
                Vector2 off = Main.rand.NextVector2Circular(20, 20);
                float scale = Main.rand.NextFloat() + Utils.GetLerpValue(50, 0, off.Length(), true);
                Particle.NewParticle(Particle.ParticleType<PrettySparkle>(), Item.Center + off, Main.rand.NextVector2Circular(4, 4), GetAlpha(Color.White).Value * 0.2f, scale * 0.6f);
            }            
            
            if (Main.rand.NextBool(5))
            {
                Dust soul = Dust.NewDustDirect(Item.Center - new Vector2(15), 30, 30, DustID.PortalBoltTrail, 0f, -Main.rand.NextFloat(1f, 2f), 0, GetAlpha(Color.White).Value, Main.rand.NextFloat(2f));
                soul.noGravity = true;
            }
        }

        public override bool PreDrawInWorld(SpriteBatch spriteBatch, Color lightColor, Color alphaColor, ref float rotation, ref float scale, int whoAmI)
        {
            bool includeChains = false;
            bool includeLensFlare = false;

            Texture2D texture = ModContent.Request<Texture2D>(Texture).Value;
            Texture2D spark = ModContent.Request<Texture2D>(Texture + "Spark").Value;
            Texture2D chain = ModContent.Request<Texture2D>(Texture + "Chain").Value;
            Texture2D glow = ModContent.Request<Texture2D>($"{nameof(CalamityHunt)}/Assets/Textures/Goozma/GlowSoftBig").Value;

            float soulScale = scale;
            scale = 1f + MathF.Sin(Main.GlobalTimeWrappedHourly * 3f % MathHelper.TwoPi) * 0.01f;

            spriteBatch.Draw(spark, Item.Center - Main.screenPosition, spark.Frame(), Color.Black * 0.1f, -MathHelper.PiOver4, spark.Size() * 0.5f, scale * new Vector2(2f, 0.5f), 0, 0);
            spriteBatch.Draw(spark, Item.Center - Main.screenPosition, spark.Frame(), Color.Black * 0.1f, MathHelper.PiOver4, spark.Size() * 0.5f, scale * new Vector2(2f, 0.5f), 0, 0);

            if (includeChains)
            {
                VertexStrip strip1 = new VertexStrip();
                VertexStrip strip2 = new VertexStrip();
                VertexStrip strip3 = new VertexStrip();

                int count = 120;
                Vector2[] offs1 = new Vector2[count];
                Vector2[] offs2 = new Vector2[count];
                Vector2[] offs3 = new Vector2[count];
                float[] offRots1 = new float[count];
                float[] offRots2 = new float[count];
                float[] offRots3 = new float[count];

                for (int i = 0; i < count; i++)
                {
                    Vector2 x = new Vector2(35 + MathF.Sin(Main.GlobalTimeWrappedHourly - i * 0.04f) * 10f, 0).RotatedBy(MathHelper.TwoPi / 120f * i - Main.GlobalTimeWrappedHourly);
                    x.X *= 1f + MathF.Cos(Main.GlobalTimeWrappedHourly * 1.5f) * 0.2f;
                    Vector2 y = new Vector2(40 + MathF.Cos(Main.GlobalTimeWrappedHourly - i * 0.07f) * 17f, 0).RotatedBy(MathHelper.TwoPi / 130f * i - Main.GlobalTimeWrappedHourly * 0.7f + 2f);
                    y.Y *= 0.6f + MathF.Sin(Main.GlobalTimeWrappedHourly * 1.2f) * 0.1f;
                    Vector2 z = new Vector2(60 + MathF.Cos(Main.GlobalTimeWrappedHourly - i * 0.01f) * 10f, 0).RotatedBy(MathHelper.TwoPi / 150f * i + Main.GlobalTimeWrappedHourly * 2f + 2f);
                    z.Y *= 0.8f + MathF.Sin(Main.GlobalTimeWrappedHourly) * 0.1f;
                    offs1[i] = x.RotatedBy(-Main.GlobalTimeWrappedHourly * 1f);
                    offs2[i] = y.RotatedBy(-Main.GlobalTimeWrappedHourly * 0.8f);
                    offs3[i] = z.RotatedBy(-Main.GlobalTimeWrappedHourly * 0.1f) * new Vector2(1f, 0.8f);

                    Vector2 f = new Vector2(20 + (Main.GlobalTimeWrappedHourly * 0.4f % 1f) * 150f, 0).RotatedBy(MathHelper.TwoPi / 80f * i - Main.GlobalTimeWrappedHourly + 2f);
                    f.Y *= 1f + MathF.Sin(Main.GlobalTimeWrappedHourly * 2f) * 0.1f;
                }

                offRots1[0] = offs1[0].AngleTo(offs1[1]);
                offRots2[0] = offs2[0].AngleTo(offs2[1]);
                offRots3[0] = offs2[0].AngleTo(offs2[1]);
                for (int i = 1; i < count; i++)
                {
                    offRots1[i] = offs1[i - 1].AngleTo(offs1[i]);
                    offRots2[i] = offs2[i - 1].AngleTo(offs2[i]);
                    offRots3[i] = offs3[i - 1].AngleTo(offs3[i]);
                }

                Color StripColor(float p)
                {
                    Color[] array = new Color[]
                    {
                        new Color(255, 160, 20, 0),
                        new Color(255, 80, 120, 0),
                        new Color(255, 160, 20, 0),
                        new Color(255, 80, 120, 0),
                        new Color(30, 120, 255, 0),
                    };
                    Color final = new GradientColor(array, 0.15f, 0.5f).ValueAt((p * 0.6f + Main.GlobalTimeWrappedHourly) * 15f);
                    return final * 0.8f;
                }
                float StripWidth(float p) => Utils.GetLerpValue(0f, 0.3f, p, true) * Utils.GetLerpValue(1f, 0.7f, p, true) * 30f * (1f + MathF.Sin(p * MathHelper.TwoPi * 8) * 0.1f);

                strip1.PrepareStrip(offs1, offRots1, StripColor, StripWidth, Item.Center - Main.screenPosition, offs1.Length, true);
                strip2.PrepareStrip(offs2, offRots2, StripColor, StripWidth, Item.Center - Main.screenPosition, offs2.Length, true);
                strip3.PrepareStrip(offs3, offRots3, StripColor, StripWidth, Item.Center - Main.screenPosition, offs3.Length, true);

                Effect effect = ModContent.Request<Effect>($"{nameof(CalamityHunt)}/Assets/Effects/CrystalLightningEffect", AssetRequestMode.ImmediateLoad).Value;
                effect.Parameters["uTransformMatrix"].SetValue(Main.GameViewMatrix.NormalizedTransformationmatrix);
                effect.Parameters["uTexture"].SetValue(chain);
                effect.Parameters["uGlow"].SetValue(TextureAssets.Extra[197].Value);
                effect.Parameters["uColor"].SetValue(Vector3.One);
                effect.Parameters["uTime"].SetValue(Main.GlobalTimeWrappedHourly * 1f % 1f);
                effect.CurrentTechnique.Passes[0].Apply();

                strip1.DrawTrail();
                strip2.DrawTrail();
                strip3.DrawTrail();

                spriteBatch.Draw(glow, Item.Center - Main.screenPosition, glow.Frame(), GetAlpha(Color.White).Value * 0.3f, 0, glow.Size() * 0.5f, scale, 0, 0);

                Main.pixelShader.CurrentTechnique.Passes[0].Apply();
            }

            if (includeLensFlare)
            {
                spriteBatch.Draw(spark, Item.Center - Main.screenPosition, spark.Frame(), GetAlpha(Color.White).Value * 0.3f, 0, spark.Size() * 0.5f, scale * new Vector2(0.4f, 5f), 0, 0);
                spriteBatch.Draw(spark, Item.Center - Main.screenPosition, spark.Frame(), GetAlpha(Color.White).Value * 0.3f, MathHelper.PiOver2, spark.Size() * 0.5f, scale * new Vector2(0.2f, 15f), 0, 0);
                spriteBatch.Draw(spark, Item.Center - Main.screenPosition, spark.Frame(), GetAlpha(Color.White).Value * 0.3f, 0, spark.Size() * 0.5f, scale * new Vector2(2f, 0.5f), 0, 0);
                spriteBatch.Draw(spark, Item.Center - Main.screenPosition, spark.Frame(), GetAlpha(Color.White).Value * 0.3f, MathHelper.PiOver2, spark.Size() * 0.5f, scale * new Vector2(2f, 0.5f), 0, 0);
                spriteBatch.Draw(glow, Item.Center - Main.screenPosition, glow.Frame(), GetAlpha(Color.White).Value * 0.1f, 0, glow.Size() * 0.5f, scale * 1.5f, 0, 0);
            }

            spriteBatch.Draw(glow, Item.Center - Main.screenPosition, glow.Frame(), GetAlpha(Color.White).Value * 0.1f, 0, glow.Size() * 0.5f, scale, 0, 0);

            Rectangle frame = Main.itemAnimations[Type].GetFrame(texture, Main.itemFrameCounter[whoAmI]);
            spriteBatch.Draw(texture, Item.Center - Main.screenPosition, frame, GetAlpha(Color.White).Value, 0, frame.Size() * 0.5f, soulScale + 0.2f, 0, 0);
            spriteBatch.Draw(texture, Item.Center - Main.screenPosition, frame, new Color(255, 255, 255, 0), 0, frame.Size() * 0.5f, soulScale + 0.2f, 0, 0);

            spriteBatch.Draw(glow, Item.Center - Main.screenPosition, glow.Frame(), GetAlpha(Color.White).Value * 0.1f, 0, glow.Size() * 0.5f, scale * 0.5f, 0, 0);
            spriteBatch.Draw(glow, Item.Center - Main.screenPosition, glow.Frame(), GetAlpha(Color.White).Value, 0, glow.Size() * 0.5f, scale * 0.17f, 0, 0);


            return false;
        }

        public override bool PreDrawInInventory(SpriteBatch spriteBatch, Vector2 position, Rectangle frame, Color drawColor, Color itemColor, Vector2 origin, float scale)
        {
            Texture2D texture = ModContent.Request<Texture2D>(Texture).Value;
            Texture2D glow = ModContent.Request<Texture2D>($"{nameof(CalamityHunt)}/Assets/Textures/Goozma/GlowSoftBig").Value;

            spriteBatch.Draw(texture, position, frame, GetAlpha(Color.White).Value, 0, frame.Size() * 0.5f, scale + 0.2f, 0, 0);
            spriteBatch.Draw(texture, position, frame, new Color(255, 255, 255, 0), 0, frame.Size() * 0.5f, scale + 0.2f, 0, 0);

            spriteBatch.Draw(glow, position, glow.Frame(), GetAlpha(Color.White).Value * 0.1f, 0, glow.Size() * 0.5f, scale * 0.5f, 0, 0);
            spriteBatch.Draw(glow, position, glow.Frame(), GetAlpha(Color.White).Value, 0, glow.Size() * 0.5f, scale * 0.17f, 0, 0);

            return false;
        }
    }
}
