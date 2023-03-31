using CalamityHunt.Content.Items.Dyes;
using CalamityHunt.Content.Items.Materials;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using Terraria;
using Terraria.Graphics.Shaders;
using Terraria.ModLoader;

namespace CalamityHunt.Common.Systems
{
    public class CraftingSystem : ModSystem
    {
        public override void PostAddRecipes()
        {
            Mod cal;
            ModLoader.TryGetMod("CalamityMod", out cal);
            if (cal != null)
            {
                for (int i = 0; i < Recipe.numRecipes; i++)
                {
                    Recipe recipe = Main.recipe[i];
                    if (recipe.HasResult(cal.Find<ModItem>("ShadowspecBar").Type))
                        recipe.AddIngredient(ModContent.ItemType<ShatteredHeartOfDarkness>());

                }
            }
        }
    }
}
