using CalamityHunt.Content.Buffs;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityHunt.Content.Projectiles.Weapons.Summoner
{
    public class DivinePinky : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.MinionSacrificable[Type] = true;
            ProjectileID.Sets.MinionTargettingFeature[Type] = true;
            Main.projFrames[Type] = 1;
        }

        public override void SetDefaults()
        {
            Projectile.width = 22;
            Projectile.height = 22;
            Projectile.friendly = true;
            Projectile.tileCollide = true;
            Projectile.penetrate = -1;
            Projectile.timeLeft = 18000;
            Projectile.minion = true;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 10;
            Projectile.DamageType = DamageClass.Summon;
        }

        public Player Player => Main.player[Projectile.owner];

        public override void AI()
        {
            if (!Player.GetModPlayer<SlimeCanePlayer>().slimes || Player.dead)
                Projectile.Kill();
            else
                Projectile.timeLeft = 2;
        }

        public override bool PreDraw(ref Color lightColor)
        {
            return true;
        }

        public override bool OnTileCollide(Vector2 oldVelocity)
        {
            return false;
        }
    }
}
