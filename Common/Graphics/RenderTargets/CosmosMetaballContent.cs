using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using Terraria.GameContent;
using Terraria.Graphics;
using Terraria.ModLoader;
using Terraria;

namespace CalamityHunt.Common.Graphics.RenderTargets;

public class CosmosMetaballContent : ARenderTargetContentByRequest
{
    protected override void HandleUseReqest(GraphicsDevice device, SpriteBatch spriteBatch)
    {
        Color color = (Color.Lerp(Color.Turquoise, Color.LightCyan, 0.2f) * 0.66f) with { A = 0 };

        device.SetRenderTarget(_target);
        device.Clear(Color.Transparent);

        PrepareARenderTarget_AndListenToEvents(ref _target, device, Main.screenWidth, Main.screenHeight, RenderTargetUsage.PreserveContents);

        CosmosMetaballDrawer.cosmosParticles.Draw(spriteBatch, true);

        device.SetRenderTarget(null);
        _wasPrepared = true;
    }
}
