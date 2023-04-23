using CalamityHunt.Common.Graphics.SlimeMonsoon;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using Terraria;
using Terraria.Graphics.Effects;
using Terraria.Graphics.Shaders;
using Terraria.ModLoader;

namespace CalamityHunt
{
	public class CalamityHunt : Mod
	{
        public override void Load()
        {
            Ref<Effect> stellarblackhole = new Ref<Effect>(ModContent.Request<Effect>($"{nameof(CalamityHunt)}/Assets/Effects/SpaceHole", AssetRequestMode.ImmediateLoad).Value);
            Filters.Scene["HuntOfTheOldGods:StellarBlackHole"] = new Filter(new ScreenShaderData(stellarblackhole, "BlackHolePass"), EffectPriority.VeryHigh);
            Filters.Scene["HuntOfTheOldGods:StellarBlackHole"].Load();

            Ref<Effect> distort = new Ref<Effect>(ModContent.Request<Effect>($"{nameof(CalamityHunt)}/Assets/Effects/RadialDistortion", AssetRequestMode.ImmediateLoad).Value);
            Filters.Scene["HuntOfTheOldGods:SlimeMonsoon"] = new Filter(new ScreenShaderData(distort, "DistortionPass"), EffectPriority.Medium);
            Filters.Scene["HuntOfTheOldGods:SlimeMonsoon"].Load();

            SkyManager.Instance["HuntOfTheOldGods:SlimeMonsoon"] = new SlimeMonsoonBackground();
            SkyManager.Instance["HuntOfTheOldGods:SlimeMonsoon"].Load();
        }         
    }
}