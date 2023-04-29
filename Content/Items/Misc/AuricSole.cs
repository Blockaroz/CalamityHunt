using CalamityHunt.Content.Items.Rarities;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityHunt.Content.Items.Misc
{
    public class AuricSole : ModItem
    {
        public override void SetDefaults()
        {
            Item.width = 32;
            Item.height = 30;
            Item.rare = ModContent.RarityType<VioletRarity>();
        }
        public override bool CanRightClick() => true;

        public override void RightClick(Player player)
        {
            Terraria.Audio.SoundEngine.PlaySound(SoundID.DD2_DarkMageHealImpact);
            Item.SetDefaults(ModContent.ItemType<DischargedAuricSole>());
            Item.stack++;
        }

        public override void UpdateInventory(Player player)
        {
            player.runSlowdown = 0;
            player.moveSpeed *= 1.5f;
        }

        public override bool PreDrawInInventory(SpriteBatch spriteBatch, Vector2 position, Rectangle frame, Color drawColor, Color itemColor, Vector2 origin, float scale)
        {
            Asset<Texture2D> glow = ModContent.Request<Texture2D>(Texture + "Glow");
            Color glowColor = Color.DodgerBlue;
            glowColor.A = 0;
            spriteBatch.Draw(glow.Value, position, glow.Frame(), glowColor, 0, glow.Size() * 0.48f, scale, 0, 0);
            return true;
        }
    }
}
