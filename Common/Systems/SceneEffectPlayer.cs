using System;
using System.Collections.Generic;
using System.Linq;
using Terraria;
using Terraria.ModLoader;

namespace CalamityHunt.Common.Systems
{
    public class SceneEffectPlayer : ModPlayer
    {
        public enum EffectorType : ushort
        {
            SlimeMonsoon,
            SlimeMonsoonOld
        }

        public Dictionary<ushort, int> effectActive = new Dictionary<ushort, int>()
        {
            { (ushort)EffectorType.SlimeMonsoon, 0 },
            { (ushort)EffectorType.SlimeMonsoonOld, 0 } 
        };

        public override void ResetEffects()
        {
            for (int i = 0; i <  effectActive.Count; i++) {
                effectActive[(ushort)i] = Math.Max(0, effectActive[(ushort)i] - 1);
            }
        }

        public override void PostUpdateMiscEffects()
        {
            if (GoozmaSystem.GoozmaActive)
                effectActive[(ushort)EffectorType.SlimeMonsoon] = 50;

            //Player.ManageSpecialBiomeVisuals("HuntOfTheOldGods:SlimeMonsoon", effectorCount[(ushort)EffectorType.SlimeMonsoon] > 0, Player.Center);
        }
    }
}
