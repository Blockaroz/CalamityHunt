using CalamityHunt.Common.DropRules;
using CalamityHunt.Content.Items.Misc;
using CalamityHunt.Content.Items.Misc.AuricSouls;
using Terraria;
using Terraria.GameContent.ItemDropRules;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityHunt.Content.NPCs
{
    public class DropAdditions : GlobalNPC
    {
        public override bool InstancePerEntity => true;

        public override void ModifyNPCLoot(NPC npc, NPCLoot npcLoot)
        {
            if (npc.type == NPCID.QueenSlimeBoss)
            {
                npcLoot.Add(ItemDropRule.ByCondition(new DDIDropRule(), ModContent.ItemType<SludgeFocus>()));
            }
            if (ModLoader.HasMod("CalamityMod"))
            {
                Mod cal = ModLoader.GetMod("CalamityMod");
                if (npc.type == cal.Find<ModNPC>("Yharon").Type)
                {
                    npcLoot.Add(ItemDropRule.ByCondition(new YharonDownedDropRule(), ModContent.ItemType<FieryAuricSoul>()));
                }
            }
        }
    }
}
