using CalamityHunt.Common.Systems;
using Terraria.GameContent.ItemDropRules;

namespace CalamityHunt.Common.DropRules
{
    public class GoozmaDownedDropRule : IItemDropRuleCondition
    {
        bool IItemDropRuleCondition.CanDrop(DropAttemptInfo info) => !BossDownedSystem.downedBoss["Goozma"];

        bool IItemDropRuleCondition.CanShowItemDropInUI() => true;

        string IProvideItemConditionDescription.GetConditionDescription() => "Drops only on the first kill";
    }
}
