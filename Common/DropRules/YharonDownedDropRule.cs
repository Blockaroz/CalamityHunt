using Terraria.GameContent.ItemDropRules;

namespace CalamityHunt.Common.DropRules;

public class YharonDownedDropRule : IItemDropRuleCondition
{
    bool IItemDropRuleCondition.CanDrop(DropAttemptInfo info)
    {
        if (!ModCompatibility.Calamity.IsLoaded)
            return false;

        return (bool)ModCompatibility.Calamity.Mod!.Call("GetBossDowned", "yharon");
    }

    bool IItemDropRuleCondition.CanShowItemDropInUI() => true;

    string IProvideItemConditionDescription.GetConditionDescription() => "Drops only on the first kill";
}
