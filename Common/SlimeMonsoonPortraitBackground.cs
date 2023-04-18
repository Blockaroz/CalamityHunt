using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using Terraria.ModLoader;
using Terraria.UI;
using Terraria.GameContent.Bestiary;

namespace CalamityHunt.Common
{
	public class SlimeMonsoonPortraitBackground : IBestiaryInfoElement, IBestiaryBackgroundImagePathAndColorProvider
	{
		public Asset<Texture2D> GetBackgroundImage() => ModContent.Request<Texture2D>($"{nameof(CalamityHunt)}/Assets/Textures/SlimeMonsoonBG");
		public Color? GetBackgroundColor() => Color.White;
		public UIElement ProvideUIElement(BestiaryUICollectionInfo info) => null;
	}
}