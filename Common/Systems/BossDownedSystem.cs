using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria.GameContent.ItemDropRules;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;

namespace CalamityHunt.Common.Systems
{
    public class BossDownedSystem : ModSystem
    {
        public static Dictionary<string, bool> downedBoss = new Dictionary<string, bool>()
        {
            { "Goozma", false }
        };

        public override void SaveWorldData(TagCompound tag)
        {
            foreach (string entry in downedBoss.Keys)
                tag["downedBoss" + entry] = downedBoss[entry];
        }

        public override void LoadWorldData(TagCompound tag)
        {
            foreach (string entry in downedBoss.Keys)
                downedBoss[entry] = tag.GetBool("downedBoss" + entry);
        }
    }
}
