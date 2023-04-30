using CalamityHunt.Common.Systems;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria.GameContent.ItemDropRules;

namespace CalamityHunt.Common.DropRules
{
    public class GoozmaDownedDropRule : IItemDropRuleCondition
    {
        bool IItemDropRuleCondition.CanDrop(DropAttemptInfo info) => !BossDownedSystem.downedGoozma;

        bool IItemDropRuleCondition.CanShowItemDropInUI() => true;

        string IProvideItemConditionDescription.GetConditionDescription() => "Drops only on the first kill";
    }
}
