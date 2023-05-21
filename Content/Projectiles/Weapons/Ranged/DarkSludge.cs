using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityHunt.Content.Projectiles.Weapons.Ranged
{
    public partial class DarkSludge : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.TrailCacheLength[Type] = 4;
            ProjectileID.Sets.TrailingMode[Type] = 2;
        }

        public override void SetDefaults()
        {
            Projectile.width = 14;
            Projectile.height = 14;
            Projectile.friendly = true;
            Projectile.timeLeft = 10000;
            Projectile.penetrate = -1;
            Projectile.tileCollide = true;
            Projectile.usesIDStaticNPCImmunity = true;
            Projectile.idStaticNPCHitCooldown = 15;
            Projectile.DamageType = DamageClass.Melee;
        }

        public ref float Time => ref Projectile.ai[0];
        public ref float Mode => ref Projectile.ai[1];
        public ref float StickHost => ref Projectile.ai[2];

        public override void OnSpawn(IEntitySource source)
        {
            Projectile.localAI[0] = Main.rand.Next(0, 2);
            Projectile.localAI[1] = Main.rand.NextFloat(0.8f, 1.2f);
            StickHost = -1;
            Projectile.spriteDirection = Main.rand.NextBool() ? 1 : -1;
        }

        public override void AI()
        {
            Projectile.scale = MathF.Sqrt(Utils.GetLerpValue(0, 10, Time, true) * Utils.GetLerpValue(450, 400, Time, true)) * 1.1f * Projectile.localAI[1];

            if (Mode == 0)
                Projectile.rotation = Projectile.velocity.ToRotation();
            if (Mode == 2)
                Projectile.rotation = MathHelper.PiOver2;

            if (Mode != 1)
                Projectile.velocity += Time < 20 ? Vector2.Zero : Vector2.UnitY;

            if (Time == 21)
                Projectile.velocity += Main.rand.NextVector2Circular(5, 10).RotatedBy(Projectile.velocity.ToRotation());

            if (Projectile.velocity.Length() > 25f)
                Projectile.velocity *= 0.98f;

            if (Time > 450)
                Projectile.Kill();

            if (Main.rand.NextBool(25) || (Main.rand.NextBool(7) && Mode == 0))
            {
                Dust sludge = Dust.NewDustPerfect(Projectile.Center + Main.rand.NextVector2Circular(30, 30), DustID.Corruption, Main.rand.NextVector2Circular(3, 2), 150, Color.White, 1f + Main.rand.NextFloat());
                sludge.noGravity = true;
            }

            if (StickHost > -1)
            {
                if (Time < 400)
                    Time = 400;

                if (!Main.npc[(int)StickHost].active)
                    Projectile.Kill();

                Projectile.velocity *= 0.5f;
                Projectile.velocity += Projectile.DirectionTo(Main.npc[(int)StickHost].Center) * 0.4f;
                Projectile.Center += Main.npc[(int)StickHost].position - Main.npc[(int)StickHost].oldPosition;
            }

            Time++;
        }

        public override void ModifyDamageHitbox(ref Rectangle hitbox)
        {
            if (Mode != 0)
            {
                hitbox.Width = 50;
                hitbox.Height = 50;
                hitbox.Location = (Projectile.Center - new Vector2(25)).ToPoint();
            }
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            Mode = 1;
            Projectile.velocity *= 0.9f;
            StickHost = target.whoAmI;
        }

        public override bool OnTileCollide(Vector2 oldVelocity)
        {
            Projectile.velocity.X *= 0.7f;

            if (Mode == 0 && Projectile.velocity.Y >= 0)
            {
                Mode = 2;
                Projectile.rotation = MathHelper.PiOver2;

                if (Time < 200)
                    Time = 200;

                SoundEngine.PlaySound(SoundID.NPCDeath9, Projectile.Center);

                for (int i = 0; i < 10; i++)
                {
                    Dust sludge = Dust.NewDustPerfect(Projectile.Center + Main.rand.NextVector2Circular(50, 40), DustID.Corruption, Main.rand.NextVector2Circular(3, 2) - Vector2.UnitY * 2, 150, Color.White, 1f + Main.rand.NextFloat());
                    sludge.noGravity = true;
                }
            }

            if (Mode == 0 && MathF.Abs(Projectile.velocity.Y - oldVelocity.Y) > 0)
                Projectile.velocity.Y *= -1;

            return false;
        }

        public override bool PreDraw(ref Color lightColor) => false;
    }
}
