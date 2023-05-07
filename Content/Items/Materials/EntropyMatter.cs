using CalamityHunt.Common.Systems.Particles;
using CalamityHunt.Content.Bosses.Goozma;
using CalamityHunt.Content.Items.Rarities;
using CalamityHunt.Content.Particles;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using System;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityHunt.Content.Items.Materials
{
    public class EntropyMatter : ModItem
    {
        public override void SetStaticDefaults()
        {
            Main.RegisterItemAnimation(Type, new DrawAnimationVertical(8, 5));
            ItemID.Sets.AnimatesAsSoul[Type] = true;

            ItemID.Sets.ItemNoGravity[Type] = true;
            Item.ResearchUnlockCount = 50;
        }

        public override void SetDefaults()
        {
            Item.width = 34;
            Item.height = 30;
            Item.value = Item.sellPrice(0, 30);
            Item.rare = ModContent.RarityType<VioletRarity>();
            Item.maxStack = Item.CommonMaxStack;
        }

        public override void PostUpdate()
        {
            if (Main.rand.NextBool(5))
            {
                Dust dark = Dust.NewDustPerfect(Item.Center + Main.rand.NextVector2Circular(18, 18), DustID.TintableDust, -Vector2.UnitY.RotatedByRandom(0.1f) * Main.rand.NextFloat(3f), 150, Color.Black, 1f + Main.rand.NextFloat());
                dark.noGravity = true;
            }            
            
            if (Main.rand.NextBool(20))
            {
                Particle spark = Particle.NewParticle(Particle.ParticleType<HueLightDust>(), Item.Center + Main.rand.NextVector2Circular(15, 15), -Vector2.UnitY * Main.rand.NextFloat(2f), Color.White, 0.7f + Main.rand.NextFloat());
                spark.data = Main.GlobalTimeWrappedHourly * 10f;
            }
        }

        public override bool PreDrawInInventory(SpriteBatch spriteBatch, Vector2 position, Rectangle frame, Color drawColor, Color itemColor, Vector2 origin, float scale)
        {
            Asset<Texture2D> glow = ModContent.Request<Texture2D>(Texture + "Aura");
            float backScale = 1f + (float)Math.Sin(Main.GlobalTimeWrappedHourly * 8f % MathHelper.TwoPi) * 0.1f;
            spriteBatch.Draw(glow.Value, position, null, Color.Black * 0.4f, 1f, glow.Size() * 0.5f, scale * backScale * 1.2f, 0, 0);

            return true;
        }

        public override bool PreDrawInWorld(SpriteBatch spriteBatch, Color lightColor, Color alphaColor, ref float rotation, ref float scale, int whoAmI)
        {
            Asset<Texture2D> glow = ModContent.Request<Texture2D>(Texture + "Aura");
            float backScale = 1f + (float)Math.Sin(Main.GlobalTimeWrappedHourly * 8f % MathHelper.TwoPi) * 0.1f;
            spriteBatch.Draw(glow.Value, Item.Center - Main.screenPosition, null, Color.Black * 0.7f, 1f, glow.Size() * 0.5f, scale * backScale * 1.2f, 0, 0);
            return true;
        }

        public override void PostDrawInInventory(SpriteBatch spriteBatch, Vector2 position, Rectangle frame, Color drawColor, Color itemColor, Vector2 origin, float scale)
        {
            Asset<Texture2D> glow = ModContent.Request<Texture2D>(Texture + "Glow");
            Asset<Texture2D> bloom = ModContent.Request<Texture2D>(Texture + "Aura");
            Color color = new GradientColor(SlimeUtils.GoozColors, 0.2f, 0.2f).ValueAt(Main.GlobalTimeWrappedHourly * 10f);
            spriteBatch.Draw(glow.Value, position, frame, color * 0.8f, 0, origin, scale, 0, 0);
            spriteBatch.Draw(glow.Value, position, frame, new Color(color.R, color.G, color.B, 0), 0, origin, scale, 0, 0);
            spriteBatch.Draw(bloom.Value, position, null, new Color(color.R, color.G, color.B, 0) * 0.5f, 0, bloom.Size() * 0.5f, scale, 0, 0);
        }

        public override void PostDrawInWorld(SpriteBatch spriteBatch, Color lightColor, Color alphaColor, float rotation, float scale, int whoAmI)
        {
            Asset<Texture2D> glow = ModContent.Request<Texture2D>(Texture + "Glow");
            Asset<Texture2D> bloom = ModContent.Request<Texture2D>(Texture + "Aura");
            Rectangle frame = Main.itemAnimations[Type].GetFrame(glow.Value, Main.itemFrameCounter[whoAmI]);
            Color color = new GradientColor(SlimeUtils.GoozColors, 0.2f, 0.2f).ValueAt(Main.GlobalTimeWrappedHourly * 10f);
            spriteBatch.Draw(glow.Value, Item.Center - Main.screenPosition, frame, color * 0.8f, rotation, Item.Size * 0.5f, scale, 0, 0);
            spriteBatch.Draw(glow.Value, Item.Center - Main.screenPosition, frame, new Color(color.R, color.G, color.B, 0), rotation, Item.Size * 0.5f, scale, 0, 0);
            spriteBatch.Draw(bloom.Value, Item.Center - Main.screenPosition, null, new Color(color.R, color.G, color.B, 0) * 0.5f, rotation, bloom.Size() * 0.5f, scale * 1.5f, 0, 0);
        }
    }
}
