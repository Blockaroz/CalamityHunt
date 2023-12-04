using CalamityHunt.Common.DropRules;
using CalamityHunt.Common.Systems;
using CalamityHunt.Content.Items.Dyes;
using CalamityHunt.Content.Items.Misc;
using Terraria;
using Terraria.GameContent.ItemDropRules;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityHunt.Common.GlobalNPCs
{
    public class DropAdditions : GlobalNPC
    {
        public override bool InstancePerEntity => true;

        public static Condition goozDown = new("After Goozma has been defeated", () => ModContent.GetInstance<BossDownedSystem>().GoozmaDowned);

        public override void ModifyShop(NPCShop shop)
        {
            var type = shop.NpcType;
            if (type == NPCID.Stylist) {
                shop.Add(ModContent.ItemType<GoopHairDye>(), goozDown);
            }
        }

        public override void ModifyNPCLoot(NPC npc, NPCLoot npcLoot)
        {
            if (npc.type == NPCID.QueenSlimeBoss) {
                npcLoot.Add(ItemDropRule.ByCondition(new RemixWorldDropRule(), ModContent.ItemType<SludgeFocus>()));
            }
            if (ModLoader.HasMod(HUtils.CalamityMod)) {
                var cal = ModLoader.GetMod(HUtils.CalamityMod);
                if (npc.type == cal.Find<ModNPC>("Yharon").Type) {
                    npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<IOUASoul>()));
                }
            }
        }
    }
}
