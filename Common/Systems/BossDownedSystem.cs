using System;
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
        public static bool downedGoozma;

        public override void SaveWorldData(TagCompound tag)
        {
            tag["downedGoozma"] = downedGoozma;
        }

        public override void LoadWorldData(TagCompound tag)
        {
            downedGoozma = tag.GetBool("downedGoozma");
        }
    }
}
