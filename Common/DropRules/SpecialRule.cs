using Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria.GameContent.ItemDropRules;

namespace CalamityHunt.Common.DropRules
{
    public static class SpecialRule
    {
        public static IItemDropRule BossBagStrange(int denominator, int mainItemId, int otherItemId)
        {
            List<int> options = new List<int>() { otherItemId };
            for (int i = 0; i < denominator; i++)
                options.Add(mainItemId);
            return new DropBasedOnExpertMode(ItemDropRule.DropNothing(), ItemDropRule.OneFromOptions(1, options.ToArray()));
        }
    }
}
