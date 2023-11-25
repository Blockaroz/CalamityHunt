using System;
using System.Linq;
using CalamityHunt.Common.Systems.Particles;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using Terraria;
using Terraria.ModLoader;

namespace CalamityHunt.Content.Projectiles.Weapons.Ranged
{
    public partial class DarkSludge : ModProjectile
    {
        //private void DrawSludge(On_Main.orig_DoDraw_Tiles_Solid orig, Main self)
        //{
        //    Texture2D baseTex = AssetDirectory.Textures.Goozma.DarkSludge.Value;
        //    Texture2D glowTex = AssetDirectory.Textures.Goozma.DarkSludgeGlow.Value;
        //    Effect effect = ModContent.Request<Effect>($"{nameof(CalamityHunt)}/Assets/Effects/SludgeEffect", AssetRequestMode.ImmediateLoad).Value;
        //    effect.Parameters["uPosition"].SetValue(Main.screenPosition * 0.5f);
        //    effect.Parameters["uBaseTexture"].SetValue(baseTex);
        //    effect.Parameters["uGlowTexture"].SetValue(glowTex);
        //    effect.Parameters["uSize"].SetValue(Vector2.One * 5);
        //    effect.Parameters["uTime"].SetValue(Main.GlobalTimeWrappedHourly * 0.002f % 1f);
        //    effect.Parameters["uScreenSize"].SetValue(Main.ScreenSize.ToVector2());

        //    Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, Main.DefaultSamplerState, DepthStencilState.None, Main.Rasterizer, effect, Main.Transform);

        //    if (SludgeTarget != null)
        //        Main.spriteBatch.Draw(SludgeTarget, Vector2.Zero, null, Color.White, 0f, Vector2.Zero, 2f, 0, 0);

        //    Main.spriteBatch.End();

        //    orig(self);
        //}
    }
}
