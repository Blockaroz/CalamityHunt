using CalamityHunt.Common.Players;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using Terraria;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityHunt.Content.Projectiles.Weapons.Summoner
{
    public class Goozmoem : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.MinionSacrificable[Type] = true;
            ProjectileID.Sets.MinionTargettingFeature[Type] = true;
            Main.projFrames[Type] = 1;
        }

        public override void SetDefaults()
        {
            Projectile.width = 30;
            Projectile.height = 50; 
            Projectile.friendly = true;
            Projectile.tileCollide = false;
            Projectile.penetrate = -1;
            Projectile.timeLeft = 18000;
            Projectile.minion = true;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 10;
            Projectile.DamageType = DamageClass.Summon;
            Projectile.manualDirectionChange = true;
        }

        public override bool? CanDamage() => State == (int)SlimeMinionState.Attacking && Time > 0;

        public ref float State => ref Projectile.ai[0];
        public ref float Time => ref Projectile.ai[1];
        public ref float AttackCD => ref Projectile.ai[2];

        public Player Player => Main.player[Projectile.owner];

        public Rope cordRope;

        public override void AI()
        {
            if (!Player.GetModPlayer<SlimeCanePlayer>().slimes || Player.dead || Player.GetModPlayer<SlimeCanePlayer>().SlimeRank() < SlimeCanePlayer.HighestRank)
                Projectile.Kill();
            else
                Projectile.timeLeft = 2;

            int target = -1;
            Projectile.Minion_FindTargetInRange(800, ref target, true);
            bool hasTarget = false;
            if (target > -1)
            {
                hasTarget = true;
                if (Main.npc[target].active && Main.npc[target].CanBeChasedBy(Projectile))
                    Attack(target);
                else
                    hasTarget = false;
            }
            if (!hasTarget)
                Idle();

            Dust gas = Dust.NewDustPerfect(Projectile.Center + Main.rand.NextVector2Circular(16, 25), DustID.TintableDust, Main.rand.NextVector2Circular(1, 1), 150, Color.Black, 0.5f + Main.rand.NextFloat());
            gas.noGravity = true;

            if (AttackCD > 0)
                AttackCD--;

            if (Projectile.frameCounter++ > 3)
            {
                Projectile.frameCounter = 0;
                Projectile.frame = (Projectile.frame + 1) % 6;
            }

            Vector2 cordStart = new Vector2(512) + new Vector2(10 * Projectile.direction, 2).RotatedBy(Projectile.rotation) * Projectile.scale * 0.5f;
            Vector2 cordEnd = new Vector2(512) + (Player.MountedCenter - Projectile.Center) * 0.5f;
            if (cordRope == null)
                cordRope = new Rope(cordStart, cordEnd, 16, 2f, Vector2.Zero, 0.5f, 10);

            cordRope.segmentLength = Projectile.Distance(Player.MountedCenter) * 0.07f;
            cordRope.StartPos = cordStart;
            cordRope.EndPos = cordEnd;
            cordRope.gravity = new Vector2(Player.direction * 1, -0.5f) * 0.3f;
            cordRope.Update();
        }

        public Vector2 HomePosition => Player.MountedCenter + new Vector2(-20 * Player.direction, -32);

        public void Idle()
        {
            Time = 0;

            if (Math.Abs(Projectile.velocity.X) < 1f)
                Projectile.direction = Player.direction;
            else
                Projectile.direction = Math.Sign(Projectile.velocity.X);

            if (Projectile.Distance(HomePosition) > 14)
                Projectile.velocity += Projectile.DirectionTo(HomePosition).SafeNormalize(Vector2.Zero) * MathF.Max(0.1f, Projectile.Distance(HomePosition) * 0.03f);
            else
            {
                Projectile.velocity = Vector2.Lerp(Projectile.velocity, Main.rand.NextVector2Circular(2, 2), 0.3f);
                Projectile.velocity *= 0.9f;
                Projectile.netUpdate = true;
            }
            Projectile.velocity *= 0.9f;

            eyeOffset = Vector2.Lerp(eyeOffset, Main.rand.NextVector2Circular(4, 4), 0.3f);
        }

        public void Attack(int whoAmI)
        {
            NPC target = Main.npc[whoAmI];
        }

        public Vector2 eyeOffset;

        public static GoozmoemTextureContent goozmoemTextureContent;
        public static GoozmoemCordTextureContent goozmoemCordTextureContent;

        public override void Load()
        {
            Main.ContentThatNeedsRenderTargets.Add(goozmoemTextureContent = new GoozmoemTextureContent());
            Main.ContentThatNeedsRenderTargets.Add(goozmoemCordTextureContent = new GoozmoemCordTextureContent());
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Vector2 scale = Projectile.scale * Vector2.One;

            goozmoemTextureContent.width = 1024;
            goozmoemTextureContent.height = 1024;
            goozmoemTextureContent.drawFunction = DrawCreature;
            goozmoemTextureContent.drawNonGlowFunction = DrawCreatureCrown;
            goozmoemTextureContent.Request();
            if (goozmoemTextureContent.IsReady)
            {
                Texture2D goozmoem = goozmoemTextureContent.GetTarget();
                Main.EntitySpriteDraw(goozmoem, Projectile.Center - Main.screenPosition, goozmoem.Frame(), Color.White, Projectile.rotation, goozmoem.Size() * 0.5f, scale, 0, 0);
            }

            return false;
        }

        public void DrawCreature()
        {
            SpriteEffects direction = Projectile.direction < 0 ? SpriteEffects.FlipHorizontally : SpriteEffects.None;

            if (cordRope != null)
            {
                Texture2D texture = TextureAssets.Projectile[Type].Value;
                Texture2D eyeTexture = AssetDirectory.Textures.Extras.GoozmoemEye;
                Rectangle frame = texture.Frame(1, 7, 0, Projectile.frame, -2, -2);

                goozmoemCordTextureContent.width = 1024;
                goozmoemCordTextureContent.height = 1024;
                List<Vector2> points = cordRope.GetPoints();
                points.Add(new Vector2(512) + (Player.MountedCenter - Projectile.Center) * 0.5f);
                goozmoemCordTextureContent.positions = points.ToArray();
                goozmoemCordTextureContent.Request();

                if (goozmoemCordTextureContent.IsReady)
                {
                    Texture2D goozmoemCord = goozmoemCordTextureContent.GetTarget();
                    Main.EntitySpriteDraw(goozmoemCord, new Vector2(512), goozmoemCord.Frame(), Color.White, Projectile.rotation, goozmoemCord.Size() * 0.5f, 2f, 0, 0);
                }

                Main.EntitySpriteDraw(texture, new Vector2(512), frame, Color.White, Projectile.rotation, new Vector2(frame.Width * 0.5f, 22), 1f, direction, 0);
                Main.EntitySpriteDraw(eyeTexture, new Vector2(512) + eyeOffset + new Vector2(4 * Projectile.direction, -4).RotatedBy(Projectile.rotation), eyeTexture.Frame(), Color.White, Projectile.rotation, eyeTexture.Size() * 0.5f, 1f, direction, 0);
            }
        }

        public void DrawCreatureCrown()
        {
            SpriteEffects direction = Projectile.direction < 0 ? SpriteEffects.FlipHorizontally : SpriteEffects.None;

            Texture2D texture = AssetDirectory.Textures.Extras.GoozmoemCrown;
            Rectangle frame = texture.Frame(1, 7, 0, Projectile.frame, -2, -2);
            Main.EntitySpriteDraw(texture, new Vector2(512), frame, Color.White, Projectile.rotation, new Vector2(frame.Width * 0.5f, 22), 1f, direction, 0);
        }
    }
}
