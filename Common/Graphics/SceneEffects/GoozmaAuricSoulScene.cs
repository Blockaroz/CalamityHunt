using System.Linq;
using CalamityHunt.Common.Graphics.Skies;
using CalamityHunt.Common.Systems;
using CalamityHunt.Content.Items.Misc.AuricSouls;
using Terraria;
using Terraria.Graphics.Effects;
using Terraria.ModLoader;

namespace CalamityHunt.Common.Graphics.SceneEffects;

public class GoozmaAuricSoulScene : ModSceneEffect
{
    public override SceneEffectPriority Priority => SceneEffectPriority.Event;

    public override int Music => MusicLoader.GetMusicSlot(Mod, "Assets/Music/GoozmaAuricSoulMusic");

    public override bool IsSceneEffectActive(Player player)
    {
        var active = Main.item.Any(n => n.active && n.type == ModContent.ItemType<GoozmaAuricSoul>()) || GoozmaSystem.GoozmaActive;

        if (active) {
            player.GetModPlayer<SceneEffectPlayer>().effectActive[(ushort)SceneEffectPlayer.EffectorType.SlimeMonsoon] = 15;
            SlimeMonsoonSky.lightningEnabled = false;
            Main.windSpeedTarget = 0.5f;
        }
        
        return active;
    }

    public override void SpecialVisuals(Player player, bool isActive)
    {
        if (!SkyManager.Instance["HuntOfTheOldGods:SlimeMonsoon"].IsActive() && isActive) {
            SkyManager.Instance.Activate("HuntOfTheOldGods:SlimeMonsoon", player.Center);
        }        
        else if (SkyManager.Instance["HuntOfTheOldGods:SlimeMonsoon"].IsActive() && !isActive && !GoozmaSystem.GoozmaActive) {
            SkyManager.Instance["HuntOfTheOldGods:SlimeMonsoon"].Deactivate();
        }
    }
}
