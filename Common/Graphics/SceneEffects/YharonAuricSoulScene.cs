using System.Linq;
using Terraria;
using Terraria.ModLoader;

namespace CalamityHunt.Common.Graphics.SceneEffects;

public class YharonAuricSoulScene : ModSceneEffect
{
    public override SceneEffectPriority Priority => SceneEffectPriority.Event;

    public override int Music => MusicLoader.GetMusicSlot(Mod, "Assets/Music/YharonAuricSoulMusic");

    public override bool IsSceneEffectActive(Player player)
    {
        return false;
    }
}
