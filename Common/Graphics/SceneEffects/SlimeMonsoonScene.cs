using CalamityHunt.Common.Systems;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.ModLoader;

namespace CalamityHunt.Common.Graphics.SceneEffects
{
    public class SlimeMonsoonScene : ModSceneEffect
    {
        public override SceneEffectPriority Priority => SceneEffectPriority.BossHigh;

        public override string MapBackground => $"{nameof(CalamityHunt)}/Assets/Textures/SlimeMonsoonBG";

        public override bool IsSceneEffectActive(Player player) => GoozmaSystem.GoozmaActive;

        public override void SpecialVisuals(Player player, bool isActive)
        {
            player.ManageSpecialBiomeVisuals("HuntOfTheOldGods:SlimeMonsoon", isActive, player.Center);
        }
    }
}
