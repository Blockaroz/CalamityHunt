﻿using System;
using CalamityHunt.Common.Graphics.RenderTargets;
using CalamityHunt.Common.Systems.Particles;
using CalamityHunt.Common.Utilities;
using CalamityHunt.Content.Particles;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityHunt.Content.Projectiles.Weapons.Ranged
{
    public class SludgeShakerHeld : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.HeldProjDoesNotUsePlayerGfxOffY[Type] = true;
            ProjectileID.Sets.TrailCacheLength[Type] = 4;
            ProjectileID.Sets.TrailingMode[Type] = -1;
        }

        public override void SetDefaults()
        {
            Projectile.width = 180;
            Projectile.height = 180;
            Projectile.friendly = true;
            Projectile.timeLeft = 10000;
            Projectile.penetrate = -1;
            Projectile.tileCollide = false;
            Projectile.ownerHitCheck = true;
            //Projectile.hide = true;
            Projectile.manualDirectionChange = true;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 12;
            Projectile.DamageType = DamageClass.Melee;
            Projectile.noEnchantmentVisuals = true;
        }

        public ref float Time => ref Projectile.ai[0];
        public ref float Mode => ref Projectile.ai[1];

        public ref Player Owner => ref Main.player[Projectile.owner];

        public override void AI()
        {
            if (!Owner.active || Owner.dead || Owner.noItems || Owner.CCed) {
                Projectile.Kill();
                return;
            }


            int shootSpeed = (int)(3f * Owner.GetAttackSpeed(DamageClass.Ranged));

            if (Projectile.owner == Main.myPlayer) {

                if (Time == 0) {
                    Projectile.rotation = Projectile.velocity.ToRotation();
                    Projectile.netUpdate = true;
                }

                Owner.ChangeDir(Projectile.velocity.X > 0 ? 1 : -1);
                Projectile.direction = Projectile.velocity.X > 0 ? 1 : -1;
                Owner.SetDummyItemTime(3);
                Owner.itemRotation = (Projectile.velocity * Projectile.direction).ToRotation();

                Projectile.Center = Owner.MountedCenter - new Vector2(0, 6 * Owner.gravDir) + Projectile.velocity.SafeNormalize(Vector2.Zero) * 10;
                Projectile.rotation = Projectile.rotation.AngleLerp(Projectile.rotation.AngleTowards(Owner.AngleTo(Main.MouseWorld), 0.4f), 0.5f * Utils.GetLerpValue(1, 3, Time % 6, true));

                if (Projectile.velocity != Projectile.rotation.ToRotationVector2()) {
                    Projectile.velocity = Projectile.rotation.ToRotationVector2();
                    Projectile.netUpdate = true;
                }
            }

            bool canKill = false;

            if (Main.rand.NextBool()) {
                Vector2 sludgeVelocity = (-(MathHelper.PiOver2 + MathHelper.PiOver4) * Projectile.direction + Projectile.rotation).ToRotationVector2() * Main.rand.NextFloat(1f, 4f);
                Vector2 sludgeOff = -(new Vector2(22, 18 * Projectile.direction) + Main.rand.NextVector2Circular(15, 5).RotatedBy(-MathHelper.PiOver4 * Projectile.direction)).RotatedBy(Projectile.rotation);
                Dust sludge = Dust.NewDustPerfect(Projectile.Center + sludgeOff * Projectile.scale * gunSquish, DustID.TintableDust, sludgeVelocity, 100, Color.Black, Main.rand.NextFloat(0.5f, 1.5f));
                sludge.fadeIn = 1f;
                sludge.noGravity = Main.rand.NextBool(15);
            }

            if (Time % shootSpeed == 0) {

                for (int i = 0; i < 8; i++) {
                    CalamityHunt.particles.Add(Particle.Create<FlameParticle>(particle => {
                        particle.position = Projectile.Center + Projectile.velocity.SafeNormalize(Vector2.Zero) * 80;
                        particle.scale = Main.rand.NextFloat(1f, 6f);
                        particle.velocity = Projectile.velocity * Main.rand.NextFloat(15f, 20f);
                        particle.maxTime = Main.rand.Next(25, 40);
                        particle.color = Color.Orange with { A = 40 };
                        particle.fadeColor = Color.Red with { A = 40 };
                        particle.emitLight = true;
                    }));

                    CosmosMetaball.particles.Add(Particle.Create<SmokeSplatterParticle>(particle => {
                        particle.position = Projectile.Center + Projectile.velocity.SafeNormalize(Vector2.Zero) * 80;
                        particle.scale = Main.rand.NextFloat(1f, 5f);
                        particle.velocity = Projectile.velocity * Main.rand.NextFloat(10f, 15f);
                        particle.maxTime = 1;// Main.rand.Next(35, 50);
                        particle.color = Color.White;
                        particle.fadeColor = Color.White;
                        particle.gravity = -Vector2.UnitY * 0.05f;
                    }));
                }

                // Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center, Projectile.velocity.RotatedByRandom(0.05f) * Main.rand.Next(15, 20), ModContent.ProjectileType<ShakerSludge>(), Owner.HeldItem.damage, 1f, Owner.whoAmI);
            }

            if (!Owner.channel) {
                canKill = true;
            }

            if (!canKill) {
                Projectile.timeLeft = 10000;
            }

            if (canKill && Projectile.timeLeft > 27) {
                Projectile.timeLeft = 27;
            }

            float gunSquishProg = Utils.GetLerpValue(0, shootSpeed * 3f, Time % (shootSpeed * 4), true) * Utils.GetLerpValue(shootSpeed * 4f, shootSpeed * 2f, Time % (shootSpeed * 4), true);
            gunSquish = new Vector2(1f + gunSquishProg * 0.1f, 1f - gunSquishProg * 0.1f);

            Time++;

            HandleSound();
        }

        public override void OnKill(int timeLeft)
        {
            squartSound.StopSound();
        }

        public LoopingSound squartSound;
        public float volume;
        public float pitch;

        public void HandleSound()
        {
            volume = Utils.GetLerpValue(0, 8, Projectile.timeLeft, true) * Utils.GetLerpValue(0, 5, Time, true) * 0.2f;
            pitch = Utils.GetLerpValue(0, 8, Projectile.timeLeft, true) * Utils.GetLerpValue(0, 20, Time, true) - 0.1f + MathF.Sin(Time * 0.1f) * 0.1f;
            squartSound ??= new LoopingSound(AssetDirectory.Sounds.Weapons.SludgeShakerFiringLoop, new ProjectileAudioTracker(Projectile).IsActiveAndInGame);
            squartSound.PlaySound(() => Projectile.Center, () => volume, () => pitch);
        }

        public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox) => false;

        public Vector2 gunSquish;

        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D texture = ModContent.Request<Texture2D>(Texture).Value;
            Vector2 origin = texture.Size() * new Vector2(0.15f, 0.5f - 0.1f * Projectile.direction);
            SpriteEffects spriteEffects = Projectile.direction < 0 ? SpriteEffects.FlipVertically : SpriteEffects.None;
            Main.EntitySpriteDraw(texture, Projectile.Center - new Vector2(30, 0).RotatedBy(Projectile.rotation) * Projectile.scale - Main.screenPosition, texture.Frame(), lightColor, Projectile.rotation, origin, Projectile.scale * gunSquish, spriteEffects, 0);

            return false;
        }
    }
}
