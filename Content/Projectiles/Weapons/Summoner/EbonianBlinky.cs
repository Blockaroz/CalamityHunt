using CalamityHunt.Common.Players;
using CalamityHunt.Content.Buffs;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityHunt.Content.Projectiles.Weapons.Summoner
{
    public class EbonianBlinky : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.MinionSacrificable[Type] = true;
            ProjectileID.Sets.MinionTargettingFeature[Type] = true;
            Main.projFrames[Type] = 11;
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
            Projectile.manualDirectionChange = true;
        }

        public override bool? CanDamage() => false;

        public ref float State => ref Projectile.ai[0];
        public ref float Time => ref Projectile.ai[1];

        public Player Player => Main.player[Projectile.owner];

        public override void AI()
        {
            if (!Player.GetModPlayer<SlimeCanePlayer>().slimes || Player.dead)
                Projectile.Kill();
            else
                Projectile.timeLeft = 2;

            if (Projectile.Distance(HomePosition) > 800)
                State = (int)SlimeMinionState.IdleMoving;

            bool tooFar = Projectile.Distance(HomePosition) > 500 && State != (int)SlimeMinionState.Attacking;
            if (tooFar)
            {
                State = (int)SlimeMinionState.IdleMoving;
                Projectile.velocity = Vector2.Lerp(Projectile.velocity, Projectile.DirectionTo(HomePosition).SafeNormalize(Vector2.Zero) * Projectile.Distance(HomePosition) * 0.05f, 0.1f);
                Projectile.rotation = Projectile.velocity.X * 0.02f;
                if (Projectile.velocity.Y > 0)
                    Projectile.frame = 5;
                else
                    Projectile.frame = 0;
            }
            else
            {
                Projectile.rotation = 0f;
                int target = -1;
                Projectile.Minion_FindTargetInRange(800, ref target, false);
                if (target > -1)
                {
                    if (Main.npc[target].active)
                    {
                        Attack(target);
                    }
                }
                else
                {
                    if (Math.Abs(Projectile.velocity.X) < 3f)
                        Projectile.direction = Player.direction;
                    else
                        Projectile.direction = Math.Sign(Projectile.velocity.X);

                    Idle();
                }
            }
        }

        public Vector2 HomePosition => Player.Bottom + new Vector2(-60 * Player.direction, -28);

        public bool InAir => !Collision.SolidCollision(Player.MountedCenter - new Vector2(150), 300, 300);

        public void Idle()
        {
            Time = 0;

            Projectile.velocity.X *= 0.6f;

            if (Math.Abs(Projectile.Center.X - HomePosition.X) > 10 || InAir)
            {
                State = (int)SlimeMinionState.IdleMoving;
                Projectile.velocity.X = (HomePosition.X - Projectile.Center.X) * 0.1f;
            }

            Projectile.tileCollide = !InAir;

            if (Projectile.Bottom.Y > HomePosition.Y + 30 && InAir && Projectile.velocity.Y >= 0)
            {
                Projectile.tileCollide = false;
                OnTileCollide(Projectile.velocity);
            }

            if (State == (int)SlimeMinionState.IdleMoving)
            {
                if (++Projectile.frameCounter >= (InAir ? 11 : 5))
                {
                    Projectile.frameCounter = 0;
                    if (Projectile.frame == 5)
                        Projectile.frame = 0;
                    else
                        Projectile.frame = Math.Clamp(Projectile.frame + 1, 0, 4);
                }
            }
            else
            {
                Projectile.frameCounter = 0;
                Projectile.frame = 0;
            }

            Projectile.velocity.Y += Projectile.tileCollide ? 0.4f : 0.2f;
        }

        public void Attack(int whoAmI)
        {
            NPC target = Main.npc[whoAmI];

            if (Projectile.Distance(target.Center) < 300)
            {
                State = (int)SlimeMinionState.Attacking;
                Projectile.velocity.X *= 0.5f;
            }
                        
            if (Projectile.Distance(target.Center) > 100)
            {
                State = (int)SlimeMinionState.Attacking;
                Projectile.velocity.X *= 0.5f;
            }

            if (Projectile.Distance(target.Center) > 500)
            {
                State = (int)SlimeMinionState.IdleMoving;

                if (++Projectile.frameCounter >= (InAir ? 11 : 5))
                {
                    Projectile.frameCounter = 0;
                    if (Projectile.frame == 5)
                        Projectile.frame = 0;
                    else
                        Projectile.frame = Math.Clamp(Projectile.frame + 1, 0, 4);
                }
            }

            if (Projectile.Bottom.Y > target.Center.Y + 50 && InAir)
            {
                Projectile.tileCollide = false;
                OnTileCollide(Projectile.velocity);
            }

            int maxTime = Player.GetModPlayer<SlimeCanePlayer>().ValueFromSlimeRank(10, 9, 8, 7);

            Time++;
            if (++Projectile.frameCounter >= maxTime)
            {
                Projectile.frameCounter = 0;
                Projectile.frame = Math.Clamp(Projectile.frame + 1, 6, 11);
                if (Projectile.frame == 11)
                    Projectile.frame = 6;
            }

            if (Time == maxTime * 3)
            {
                Projectile.NewProjectileDirect(Projectile.GetSource_FromThis(), Projectile.Center, new Vector2(3, -2), ProjectileID.BloodArrow, Projectile.damage, Projectile.knockBack, Player.whoAmI);
            }

            if (Time >= maxTime * 5)
                Time = 0;

            Projectile.velocity.Y += 0.2f;
        }

        public override bool OnTileCollide(Vector2 oldVelocity)
        {
            if (State == (int)SlimeMinionState.IdleMoving)
            {
                if (Projectile.velocity.Y >= 0)
                {
                    if (Projectile.tileCollide)
                        SoundEngine.PlaySound(SoundID.NPCHit13 with { MaxInstances = 0, Pitch = 0.8f, PitchVariance = 0.3f, Volume = 0.1f }, Projectile.Center);
                    else
                        SoundEngine.PlaySound(SoundID.Item24 with { MaxInstances = 0, Pitch = 0.9f, PitchVariance = 0.3f, Volume = 0.6f }, Projectile.Center);

                    Projectile.frame = 5;
                    if (Math.Abs(Projectile.Center.X - HomePosition.X) < 10 && Projectile.tileCollide)
                        State = (int)SlimeMinionState.Idle;

                    else
                    {
                        Projectile.position.Y += oldVelocity.Y;
                        Projectile.velocity.Y = -6 - Math.Max(Math.Abs(HomePosition.X - Projectile.Center.X) * 0.01f + (InAir ? Math.Abs(HomePosition.Y - Projectile.Center.Y) * 0.016f : 0) - 1.5f, 0);
                    }

                }
            }

            return false;
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D texture = TextureAssets.Projectile[Type].Value;
            Rectangle frame = texture.Frame(5, 11, Player.GetModPlayer<SlimeCanePlayer>().SlimeRank(), Projectile.frame, -2, -2);
            SpriteEffects direction = Projectile.direction < 0 ? SpriteEffects.FlipHorizontally : SpriteEffects.None;
            Vector2 scale = Projectile.scale * Vector2.One;

            Main.EntitySpriteDraw(texture, Projectile.Bottom + Vector2.UnitY * 2 - Main.screenPosition, frame, lightColor, Projectile.rotation, frame.Size() * new Vector2(0.5f + 0.1f * Projectile.direction, 1f), scale, direction, 0);

            return false;
        }

    }
}
