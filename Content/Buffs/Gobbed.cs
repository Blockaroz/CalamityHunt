using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityHunt.Content.Buffs
{
    public class Gobbed : ModBuff
    {
        public override void Update(Player player, ref int buffIndex)
        {
            BuffID.Sets.IsAnNPCWhipDebuff[Type] = true;
        }

        public override void Update(NPC npc, ref int buffIndex)
        {
            npc.GetGlobalNPC<GobbedNPC>().marked = true;
        }
    }

    public class GobbedNPC : GlobalNPC
    {
        public override bool InstancePerEntity => true;

        public bool marked;

        public override void ResetEffects(NPC npc)
        {
            marked = false;
        }
    }
}
