using CalamityHunt.Content.Buffs;
using Microsoft.Xna.Framework;
using System.Collections.Generic;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;
using static Humanizer.In;

namespace CalamityHunt.Content.Projectiles.Weapons.Summoner
{
    public class SlimeCaneGemCounter : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.MinionSacrificable[Type] = true;
            ProjectileID.Sets.MinionTargettingFeature[Type] = true;
            Main.projFrames[Type] = 6;
        }
        public override void SetDefaults()
        {
            Projectile.netImportant = true;
            Projectile.width = 10;
            Projectile.height = 10;
            Projectile.penetrate = -1;
            Projectile.ignoreWater = true;
            Projectile.tileCollide = false;
            Projectile.friendly = true;
            Projectile.minion = true;
            Projectile.minionSlots = 1f;
            Projectile.timeLeft = 60;
            Projectile.aiStyle = 164;
            Projectile.hide = true;
        }

        public override void OnSpawn(IEntitySource source)
        {
            for (int i = 0; i < 4; i++)
            {
                int slimeType = ModContent.ProjectileType<CrimulanClyde>();
                switch (i)
                {
                    case 1:
                        slimeType = ModContent.ProjectileType<EbonianBlinky>();
                        break;
                    case 2:
                        slimeType = ModContent.ProjectileType<DivinePinky>();
                        break;
                    case 3:
                        slimeType = ModContent.ProjectileType<StellarInky>();
                        break;
                    default:
                        slimeType = ModContent.ProjectileType<CrimulanClyde>();
                        break;
                }
                if (Main.player[Projectile.owner].ownedProjectileCounts[slimeType] <= 0)
                {
                    int p = Projectile.NewProjectile(source, Projectile.position.X - (20 * i), Projectile.position.Y, 0, 0, slimeType, Projectile.damage, Projectile.knockBack, Projectile.owner);
                    if (Main.projectile.IndexInRange(p))
                        Main.projectile[p].originalDamage = Projectile.damage;
                }
            }
        }

        public override void AI()
        {
            Player player = Main.player[Projectile.owner];

            if (player.dead)
                player.GetModPlayer<SlimeCanePlayer>().slimes = false;

            if (player.GetModPlayer<SlimeCanePlayer>().slimes)
                Projectile.timeLeft = 2;

            if (++Projectile.frameCounter >= 4)
            {
                Projectile.frameCounter = 0;
                if (++Projectile.frame >= 6)
                    Projectile.frame = 0;
            }
        }

        public override bool? CanDamage() => false;

        public override bool? CanCutTiles() => false;
    }
}
