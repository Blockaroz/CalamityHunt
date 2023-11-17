using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CalamityHunt.Content.Items.Misc.AuricSouls;
using Terraria.ModLoader;
using Terraria;

namespace CalamityHunt.Common.Graphics.SceneEffects;

public class YharonAuricSoulScene : ModSceneEffect
{
    public override SceneEffectPriority Priority => SceneEffectPriority.Event;

    public override int Music => MusicLoader.GetMusicSlot(Mod, "Assets/Music/YharonAuricSoulMusic");

    public override bool IsSceneEffectActive(Player player)
    {
        var active = Main.item.Any(n => n.active && n.type == ModContent.ItemType<FieryAuricSoul>());

        if (active && ModLoader.TryGetMod("CalamityMod", out var calamity)) {
        }
        return active;
    }
}
