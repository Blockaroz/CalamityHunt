using CalamityHunt.Common.Graphics.SlimeMonsoon;
using CalamityHunt.Content.Items.Misc.AuricSouls;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.ModLoader;

namespace CalamityHunt.Common.Systems
{
    public class GoozmaAuricSoulScene : ModSceneEffect
    {
        public override SceneEffectPriority Priority => SceneEffectPriority.Event;

        public override int Music => MusicLoader.GetMusicSlot(Mod, "Assets/Music/GoozmaAuricSoulMusic");

        public override bool IsSceneEffectActive(Player player)
        {
            bool active = Main.item.Any(n => n.active && n.type == ModContent.ItemType<GoozmaAuricSoul>());

            if (active)
            {
                player.GetModPlayer<EffectTilePlayer>().effectorCount["SlimeMonsoon"] = 5;
                SlimeMonsoonBackground.lightningEnabled = false;
                Main.windSpeedTarget = Math.Sign(Main.windSpeedCurrent == 0 ? 1 : Main.windSpeedCurrent) * 20;
            }
            return active;
        }
    }
}
