using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using System.Collections.Generic;

namespace CalamityHunt.Content.Bosses.Goozma
{
    public static class GoozmaResistances
    {
        
        public static void GoozmaProjectileResistances(Projectile projectile, ref NPC.HitModifiers modifiers)
        {
            if (ModLoader.HasMod("CalamityMod"))
            {
                List<(int, float)> projectiles = new List<(int, float)> { };
                Mod calamity = ModLoader.GetMod("CalamityMod");
                projectiles.Add((calamity.Find<ModProjectile>("SeraphimProjectile").Type, 0.99f)); // 99% damage resist from Seraphim's MAIN dagger
                projectiles.Add((calamity.Find<ModProjectile>("SeraphimDagger").Type, 0.99f)); // 99% damage resist from Seraphim's SMALL daggers (these two obv arent the only ones, so add the rest/remove these or wtv)
                projectiles.Add((calamity.Find<ModProjectile>("TaintedBladeSlasher").Type, 0.4f));  // 60% damage resist from the tainted enchantment's arms
                projectiles.Add((ProjectileID.InfernoFriendlyBlast, 0.21f)); // 79% damage resist to Inferno Fork's explosion
                projectiles.Add((ModContent.ProjectileType<Content.Projectiles.Weapons.Melee.ParasanguineHeld>(), 1.2f)); // takes 20% more damage from Parasanguine's umbrella
                projectiles.Add((ProjectileID.BloodyMachete, 999999f)); // Bloody Machete deals 9899% more damage
                for (int i = 0; i < projectiles.Count; i++)
                {
                    if (projectile.type == projectiles[i].Item1)
                    {
                        modifiers.SourceDamage *= projectiles[i].Item2;
                    }
                }
            }
        }
        public static void GoozmaItemResistances(Item item, ref NPC.HitModifiers modifiers) //TRUE MELEE (holdouts don't count)
        {
            if (ModLoader.HasMod("CalamityMod"))
            {
                List<(int, float)> items = new List<(int, float)> { };
                Mod calamity = ModLoader.GetMod("CalamityMod");
                items.Add((calamity.Find<ModProjectile>("Ataraxia").Type, 0.2f)); // 80% damage resist to Ataraxia
                items.Add((calamity.Find<ModProjectile>("PlagueKeeper").Type, 0.78f)); // 22% damage resist to Plague Keeper
                items.Add((ItemID.Meowmere, 0.8f));  // 20% damage resist to Meowmere
                //items.Add((ModContent.ItemType<somemeleeweaponfromcalamityhunt>(), 0.34f));  // 66% damage resist to an inexistant true melee weapon from CalamityHunt
                for (int i = 0; i < items.Count; i++)
                {
                    if (item.type == items[i].Item1)
                    {
                        modifiers.SourceDamage *= items[i].Item2;
                    }
                }
            }
        }
    }
}
