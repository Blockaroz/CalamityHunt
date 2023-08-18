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
            Projectile.manualDirectionChange = true;
        }

        public ref float State => ref Projectile.ai[0];

        public Player Player => Main.player[Projectile.owner];

        public override void AI()
        {
            if (!Player.GetModPlayer<SlimeCanePlayer>().slimes || Player.dead)
                Projectile.Kill();
            else
                Projectile.timeLeft = 2;

            if (Math.Abs(Projectile.velocity.X) < 3f)
                Projectile.direction = Player.direction;
            else
                Projectile.direction = Math.Sign(Projectile.velocity.X);

            if (Projectile.Distance(HomePosition) > 800)
                State = (int)SlimeMinionState.IdleMoving;

            bool tooFar = Projectile.Distance(HomePosition) > 400 && State != (int)SlimeMinionState.Attacking;
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
                Idle();
            }
        }

        public Vector2 HomePosition => Player.Bottom + new Vector2(-50 * Player.direction, -20);

        public bool InAir => !Collision.SolidCollision(Player.MountedCenter - new Vector2(150), 300, 300);

        public void Idle()
        {
            State = (int)SlimeMinionState.Idle;

            Projectile.velocity.X *= 0.9f;

            if (Projectile.Distance(HomePosition) > 20 || InAir)
            {
                State = (int)SlimeMinionState.IdleMoving;
                Projectile.velocity.X = (HomePosition.X - Projectile.Center.X) * 0.07f;
            }

            Projectile.tileCollide = !InAir;

            if (Projectile.Bottom.Y > HomePosition.Y + 30 && InAir)
            {
                Projectile.tileCollide = false;
                OnTileCollide(Projectile.velocity);
            }

            if (State == (int)SlimeMinionState.IdleMoving)
            {
                if (Projectile.frameCounter++ > 4)
                {
                    Projectile.frameCounter = 0;
                    Projectile.frame = Math.Clamp(Projectile.frame + 1, 0, 6);
                }
            }
            else
            {
                Projectile.frameCounter = 0;
                Projectile.frame = 0;
            }

            Projectile.velocity.Y += Projectile.tileCollide ? 0.4f : 0.2f;
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D texture = TextureAssets.Projectile[Type].Value;
            Rectangle frame = texture.Frame(5, 11, Player.GetModPlayer<SlimeCanePlayer>().SlimeRank(), Projectile.frame, -2, -2);
            SpriteEffects direction = Projectile.direction < 0 ? SpriteEffects.FlipHorizontally : SpriteEffects.None;
            Main.EntitySpriteDraw(texture, Projectile.Bottom + Vector2.UnitY * 2 - Main.screenPosition, frame, lightColor, Projectile.rotation, frame.Size() * new Vector2(0.5f + 0.1f * Projectile.direction, 1f), Projectile.scale, direction, 0);

            return false;
        }

        public override bool OnTileCollide(Vector2 oldVelocity)
        {
            if (State == (int)SlimeMinionState.IdleMoving)
            {
                if (Projectile.velocity.Y >= 0)
                {
                    Projectile.position.Y += oldVelocity.Y;
                    Projectile.velocity.Y = -6 - Math.Max(Math.Abs(HomePosition.X - Projectile.Center.X) * 0.01f - 2, 0);
                    if (Projectile.tileCollide)
                        SoundEngine.PlaySound(SoundID.NPCHit13 with { MaxInstances = 0, Pitch = 0.8f, PitchVariance = 0.3f, Volume = 0.1f }, Projectile.Center);
                }
                Projectile.frame = 0;
            }

            return false;
        }
    }
}
