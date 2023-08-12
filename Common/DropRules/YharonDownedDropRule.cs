using CalamityHunt.Common.Systems;
using Terraria.GameContent.ItemDropRules;
using Terraria.ModLoader;

namespace CalamityHunt.Common.DropRules
{
    public class YharonDownedDropRule : IItemDropRuleCondition
    {
        bool IItemDropRuleCondition.CanDrop(DropAttemptInfo info)
            {
                if (!ModLoader.HasMod("CalamityMod"))
                    return false;
                return ModLoader.HasMod("CalamityMod") && !(bool)ModLoader.GetMod("CalamityMod").Call("GetBossDowned", "yharon");
            }

        bool IItemDropRuleCondition.CanShowItemDropInUI() => true;

        string IProvideItemConditionDescription.GetConditionDescription() => "Drops only on the first kill";
    }
}
