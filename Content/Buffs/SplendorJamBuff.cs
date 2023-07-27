using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria.ID;
using Terraria;
using Terraria.ModLoader;
using CalamityHunt.Common.Players;
using CalamityHunt.Content.Projectiles;
using Microsoft.Xna.Framework;

namespace CalamityHunt.Content.Buffs
{
    public class SplendorJamBuff : ModBuff
    {
        public override void SetStaticDefaults()
        {
            Main.buffNoTimeDisplay[Type] = true;
            Main.buffNoSave[Type] = true;
        }

        public override void Update(Player player, ref int buffIndex)
        {
            if (player.ownedProjectileCounts[ModContent.ProjectileType<SplendorTentacle>()] < player.GetModPlayer<SplendorJamPlayer>().tentacleCount)
                Projectile.NewProjectile(player.GetSource_FromThis(), player.MountedCenter, Vector2.Zero, ModContent.ProjectileType<SplendorTentacle>(), 200, 0.5f);
            
            if (player.ownedProjectileCounts[ModContent.ProjectileType<SplendorTentacle>()] > player.GetModPlayer<SplendorJamPlayer>().tentacleCount)
                Main.projectile.First(n => n.active && n.owner == player.whoAmI).Kill();

            if (!player.GetModPlayer<SplendorJamPlayer>().active)
                player.DelBuff(buffIndex);
        }
    }
}
