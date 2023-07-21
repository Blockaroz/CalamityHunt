using CalamityHunt.Content.Bosses.Goozma;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using ReLogic.Utilities;
using System;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent;
using Terraria.Graphics;
using Terraria.ModLoader;

namespace CalamityHunt.Content.Projectiles.Weapons.Magic
{
    public class GoomoireSuck : ModProjectile
    {
        public override void SetDefaults()
        {
            Projectile.width = 24;
            Projectile.height = 24;
            Projectile.friendly = true;
            Projectile.timeLeft = 10000;
            Projectile.penetrate = -1;
            Projectile.tileCollide = false;
            Projectile.ownerHitCheck = true;
            Projectile.hide = true;
            Projectile.manualDirectionChange = true;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 10;
            Projectile.DamageType = DamageClass.Magic;
        }

        public ref float Time => ref Projectile.ai[0];
        public ref float Size => ref Projectile.ai[1];

        public ref Player Owner => ref Main.player[Projectile.owner];

        public override void AI()
        {
            if (!Owner.active || Owner.dead || Owner.noItems || Owner.CCed)
            {
                Projectile.Kill();
                return;
            }

            bool canKill = false;

            Owner.ChangeDir(Projectile.velocity.X > 0 ? 1 : -1);
            Owner.SetDummyItemTime(4);

            Owner.heldProj = Projectile.whoAmI;
            Projectile.velocity = Vector2.Lerp(Projectile.velocity, Owner.DirectionTo(Main.MouseWorld).SafeNormalize(Vector2.Zero) * Owner.HeldItem.shootSpeed, 0.06f);
            Projectile.Center = Owner.MountedCenter + new Vector2(30 * Owner.direction, Owner.gfxOffY);
            Projectile.rotation = Projectile.velocity.ToRotation();

            if (Time % (5 + (int)(Owner.itemAnimationMax)) == 1)
                Owner.CheckMana(15, true);

            //if ((Time - 8) % 5 == 1)
            //{
            //    Vector2 piercerVelocity = Projectile.velocity;

            //    if (Main.myPlayer == Projectile.owner)
            //    {
            //        piercerVelocity = (Main.MouseWorld - Projectile.Center).RotatedByRandom(0.2f) * (0.06f / MathHelper.E);
            //        Projectile.netUpdate = true;
            //    }
            //    Projectile.NewProjectileDirect(Projectile.GetSource_FromThis(), Projectile.Center + Projectile.velocity * 5f + Main.rand.NextVector2Circular(10, 10), piercerVelocity, ModContent.ProjectileType<CrystalPiercer>(), Owner.HeldItem.damage, 1f, Owner.whoAmI, ai1: Projectile.whoAmI);
            //}

            if ((!Owner.channel || !Owner.CheckMana(15)))
                canKill = true;

            if (!canKill)
                Projectile.timeLeft = 10000;

            if (canKill && Projectile.timeLeft > 30)
                Projectile.timeLeft = 30;

            Time++;

            Size = 600;
            Projectile.localAI[0] = MathF.Sqrt(Utils.GetLerpValue(0, 30, Time, true) * Utils.GetLerpValue(1, 30, Projectile.timeLeft, true));
            Projectile.spriteDirection = Owner.direction;

            if (Time < 10)
                oldRot = Projectile.rotation;

            oldRot = Utils.AngleLerp(oldRot, Projectile.rotation, 0.1f);

            HandleSound();
        }

        public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
        {
            return Utils.IntersectsConeFastInaccurate(targetHitbox, Projectile.Center, Size, Projectile.rotation, MathHelper.Pi / 7f);
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D texture = TextureAssets.Projectile[Type].Value;
            Rectangle frame = texture.Frame();

            float bookRot = 0;
            SpriteEffects bookEffect = Owner.direction * (int)Owner.gravDir > 0 ? SpriteEffects.None : SpriteEffects.FlipHorizontally;
            if (Owner.gravDir < 0)
                bookRot = MathHelper.Pi;

            Main.EntitySpriteDraw(texture, Projectile.Center + new Vector2(0, 12 * Owner.gravDir + Owner.gfxOffY) - Main.screenPosition, frame, lightColor, bookRot + Owner.fullRotation, frame.Size() * 0.5f, 1f, bookEffect, 0);

            DrawLaserCone();

            return false;
        }

        public static SlotId windSound;
        public static float windVolume;
        public static float windPitch;

        public void HandleSound()
        {
            SoundStyle theSound = new SoundStyle($"{nameof(CalamityHunt)}/Assets/Sounds/Goozma/GoozmaWindLoop");
            theSound.IsLooped = true;

            windPitch = Utils.GetLerpValue(0f, 0.8f, Projectile.localAI[0]);
            windVolume = 0.5f * Projectile.localAI[0];
            bool active = SoundEngine.TryGetActiveSound(windSound, out ActiveSound sound);
            if ((!active || !windSound.IsValid))
                windSound = SoundEngine.PlaySound(theSound.WithVolumeScale(1.1f), Projectile.Center);

            if (active)
            {
                sound.Volume = windVolume;
                sound.Pitch = windPitch;
                sound.Position = Projectile.Center;
            }
        }

        public static Texture2D laserTexture;
        public static Texture2D laserTexture2;

        public override void Load()
        {
            laserTexture = new TextureAsset(Texture + "Laser" + 0);
            laserTexture2 = new TextureAsset(Texture + "Laser" + 1);
        }

        private float oldRot;

        public void DrawLaserCone()
        {
            Vector2[] positions = new Vector2[500];
            float[] rotations = new float[500];
            for (int i = 0; i < 500; i++)
            {
                rotations[i] = Utils.AngleLerp(Projectile.rotation, oldRot, MathF.Sqrt(i / 500f)) + MathF.Sin(Time * 0.2f + i / 70f) * 0.12f * (1f - i / 500f) * Projectile.localAI[0];
                positions[i] = Projectile.Center + new Vector2((Size * 1.4f) * (i / 500f) * Projectile.localAI[0], 0).RotatedBy(rotations[i]);
            }

            VertexStrip strip = new VertexStrip();
            strip.PrepareStripWithProceduralPadding(positions, rotations, StripColor, StripWidth, -Main.screenPosition, true);

            Effect lightningEffect = ModContent.Request<Effect>($"{nameof(CalamityHunt)}/Assets/Effects/FusionRayEffect", AssetRequestMode.ImmediateLoad).Value;
            lightningEffect.Parameters["uTransformMatrix"].SetValue(Main.GameViewMatrix.NormalizedTransformationmatrix);
            lightningEffect.Parameters["uTexture0"].SetValue(laserTexture);
            lightningEffect.Parameters["uTexture1"].SetValue(laserTexture2);
            lightningEffect.Parameters["uGlow"].SetValue(laserTexture2);
            lightningEffect.Parameters["uBits"].SetValue(laserTexture);
            lightningEffect.Parameters["uTime"].SetValue(Time * 0.02f);
            lightningEffect.Parameters["uFreq"].SetValue(0.5f);
            lightningEffect.CurrentTechnique.Passes[0].Apply();
            strip.DrawTrail();
            Main.pixelShader.CurrentTechnique.Passes[0].Apply();

            Texture2D sparkle = AssetDirectory.Textures.Sparkle;
            Color sparkleColor = new GradientColor(SlimeUtils.GoozColors, 0.2f, 0.2f).ValueAt(Time + 10);
            sparkleColor.A = 0;

            Vector2 sparkleScale = new Vector2(0.5f, 1.5f);
            Main.EntitySpriteDraw(sparkle, Projectile.Center - Main.screenPosition, sparkle.Frame(), sparkleColor, 0f, sparkle.Size() * 0.5f, sparkleScale, 0, 0);
            Main.EntitySpriteDraw(sparkle, Projectile.Center - Main.screenPosition, sparkle.Frame(), sparkleColor, MathHelper.PiOver2, sparkle.Size() * 0.5f, sparkleScale, 0, 0);

            Main.EntitySpriteDraw(sparkle, Projectile.Center - Main.screenPosition, sparkle.Frame(), new Color(255, 255, 255, 0), 0f, sparkle.Size() * 0.5f, sparkleScale * 0.5f, 0, 0);
            Main.EntitySpriteDraw(sparkle, Projectile.Center - Main.screenPosition, sparkle.Frame(), new Color(255, 255, 255, 0), MathHelper.PiOver2, sparkle.Size() * 0.5f, sparkleScale * 0.5f, 0, 0);
        }

        public Color StripColor(float progress)
        {
            float grow = 0.1f + (float)Math.Pow(Projectile.localAI[0], 3f);

            Color color = new GradientColor(SlimeUtils.GoozColors, 0.2f, 0.2f).ValueAt(Time + 10 + progress * 30f);
            color.A /= 2;
            return color * grow * Projectile.localAI[0];
        }

        public float StripWidth(float progress)
        {
            float start = (float)Math.Pow(progress, 0.6f);
            float grow = (float)Math.Pow(Projectile.localAI[0], 3f);
            return start * grow * Size;
        }
    }
}
