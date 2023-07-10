using CalamityHunt.Content.Bosses.Goozma;
using CalamityHunt.Content.Buffs;
using CalamityHunt.Content.Items.Rarities;
using CalamityHunt.Content.Projectiles.Weapons.Summoner;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;

namespace CalamityHunt.Content.Items.Weapons.Summoner
{
    public class Gobflogger : ModItem
    {
        public override LocalizedText Tooltip => base.Tooltip.WithFormatArgs(Gobbed.TagDamage);

        public override void SetDefaults()
        {
            Item.DefaultToWhip(ModContent.ProjectileType<GobfloggerProj>(), 2000, 12f, 4f, 100);
            Item.width = 56;
            Item.height = 48;
            Item.shootSpeed = 4;
            Item.rare = ModContent.RarityType<VioletRarity>();
            Item.channel = true;
            if (ModLoader.HasMod("CalamityMod"))
            {
                ModRarity r;
                Mod calamity = ModLoader.GetMod("CalamityMod");
                calamity.TryFind<ModRarity>("Violet", out r);
                Item.rare = r.Type;
            }
        }

        public override void PostDrawInInventory(SpriteBatch spriteBatch, Vector2 position, Rectangle frame, Color drawColor, Color itemColor, Vector2 origin, float scale)
        {
            Texture2D glow = ModContent.Request<Texture2D>(Texture + "Glow").Value;
            Color glowColor = new GradientColor(SlimeUtils.GoozOilColors, 0.3f, 0.3f).Value;
            glowColor.A /= 2;
            spriteBatch.Draw(glow, position, frame, glowColor, 0, origin, scale, 0, 0);
            spriteBatch.Draw(glow, position, frame, glowColor, 0, origin, scale, 0, 0);
        }

        public override void PostDrawInWorld(SpriteBatch spriteBatch, Color lightColor, Color alphaColor, float rotation, float scale, int whoAmI)
        {
            Texture2D glow = ModContent.Request<Texture2D>(Texture + "Glow").Value;
            Color glowColor = new GradientColor(SlimeUtils.GoozOilColors, 0.3f, 0.3f).Value;
            glowColor.A /= 2;
            spriteBatch.Draw(glow, Item.Center - Main.screenPosition, glow.Frame(), glowColor, rotation, Item.Size * 0.5f, scale, 0, 0);
            spriteBatch.Draw(glow, Item.Center - Main.screenPosition, glow.Frame(), glowColor, rotation, Item.Size * 0.5f, scale, 0, 0);
        }

        public override bool MeleePrefix() => true;
    }
}
