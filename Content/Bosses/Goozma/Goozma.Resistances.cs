using Terraria;
using Terraria.ModLoader;

namespace CalamityHunt.Content.Bosses.Goozma
{
    public partial class Goozma : ModNPC
    {
        public override void ModifyHitByProjectile(Projectile projectile, ref NPC.HitModifiers modifiers)
        {
            if (ModLoader.HasMod("CalamityMod"))
            {
                Mod calamity = ModLoader.GetMod("CalamityMod");
                if (projectile.type == calamity.Find<ModProjectile>("SeraphimProjectile").Type)
                {
                    modifiers.SourceDamage *= 0.01f; // 99% damage resist from Seraphim's MAIN dagger
                }
                if (projectile.type == calamity.Find<ModProjectile>("SeraphimDagger").Type)
                {
                    modifiers.SourceDamage *= 0.01f; // 99% damage resist from Seraphim's SMALL daggers (these two obv arent the only ones, so add the rest/remove these or wtv)
                }
                if (projectile.type == calamity.Find<ModProjectile>("TaintedBladeSlasher").Type)
                {
                    modifiers.SourceDamage *= 0.4f; // 50% damage resist from the tainted enchantment's arms
                }
            }
        }
        public override void ModifyHitByItem(Player player, Item item, ref NPC.HitModifiers modifiers) //ModifyHitByItem is for TRUE MELEE items with NO custom holdouts (exoblade and arks use holdouts)
        {
            if (ModLoader.HasMod("CalamityMod"))
            {
                Mod calamity = ModLoader.GetMod("CalamityMod");
                if (item.type == calamity.Find<ModItem>("HolyCollider").Type)
                {
                    modifiers.SourceDamage *= 1.4f; // takes 40% more damage from Holy Collider true melee strikes
                }
            }
        }
    }
}
