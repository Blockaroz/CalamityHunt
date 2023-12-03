using System.Linq;
using CalamityHunt.Common.Graphics.Skies;
using CalamityHunt.Common.Systems;
using Terraria;
using Terraria.ModLoader;

namespace CalamityHunt.Common.Graphics.SceneEffects;

public class GoozmaAuricSoulScene : ModSceneEffect
{
    public override SceneEffectPriority Priority => SceneEffectPriority.Event;

    public override int Music => MusicLoader.GetMusicSlot(Mod, "Assets/Music/GoozmaAuricSoulMusic");

    public override bool IsSceneEffectActive(Player player)
    {
        var active = GoozmaSystem.GoozmaActive;

        if (active) {
            player.GetModPlayer<SceneEffectPlayer>().effectActive[(ushort)SceneEffectPlayer.EffectorType.SlimeMonsoon] = 15;
            SlimeMonsoonSky.lightningEnabled = false;
            Main.windSpeedTarget = 0.5f;
        }
        
        return active;
    }

    public override void SpecialVisuals(Player player, bool isActive)
    {
    }
}
