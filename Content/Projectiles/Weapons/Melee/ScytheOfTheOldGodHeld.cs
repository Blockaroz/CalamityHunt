using CalamityHunt.Common.Systems;
using CalamityHunt.Common.UI;
using CalamityHunt.Content.Bosses.Goozma;
using CalamityHunt.Content.Buffs;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityHunt.Content.Projectiles.Weapons.Melee
{
    public class ScytheOfTheOldGodHeld : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.HeldProjDoesNotUsePlayerGfxOffY[Type] = true;
            ProjectileID.Sets.TrailCacheLength[Type] = 9;
            ProjectileID.Sets.TrailingMode[Type] = -1;
        }

        public override void SetDefaults()
        {
            Projectile.width = 250;
            Projectile.height = 250;
            Projectile.friendly = true;
            Projectile.timeLeft = 10000;
            Projectile.penetrate = -1;
            Projectile.tileCollide = false;
            Projectile.ownerHitCheck = true;
            Projectile.hide = true;
            Projectile.manualDirectionChange = true;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 10;
            Projectile.DamageType = DamageClass.Melee;
            Projectile.noEnchantmentVisuals = true;
        }

        public ref float Time => ref Projectile.ai[0];
        public ref float SwingStyle => ref Projectile.ai[1];

        public ref Player Player => ref Main.player[Projectile.owner];

        private float slashRotation;
        private float swingPercent;
        private float swingPercent2;

        public override void AI()
        {
            if (!Player.active || Player.dead || Player.noItems || Player.CCed)
            {
                Projectile.Kill();
                return;
            }

            Projectile.scale = Player.GetAdjustedItemScale(Player.HeldItem);

            if (Time <= 0)
            {
                for (int i = 0; i < ProjectileID.Sets.TrailCacheLength[Type]; i++)
                    Projectile.oldRot[i] = Projectile.rotation;

                Projectile.velocity = Player.DirectionTo(Main.MouseWorld).SafeNormalize(Vector2.Zero) * 5f;
                Projectile.direction = Projectile.velocity.X < 0 ? -1 : 1;
            }

            float rotation = 0;
            float addRot = 0;
            bool canKill = false;

            SoundStyle swingSound = SoundID.Item71;
            swingSound.MaxInstances = 0;
            swingSound.Pitch = -0.2f;
            swingSound.Volume = 2f;

            float speed = Player.itemAnimationMax / 30f * 0.85f;

            switch (SwingStyle)
            {
                default:
                case 0:

                    Projectile.spriteDirection = Projectile.direction;
                    swingPercent2 = MathF.Pow(Utils.GetLerpValue((int)(-20 * speed), (int)(60 * speed), Time, true), 1.5f);
                    swingPercent = SwingProgress(MathHelper.SmoothStep(0, 1, swingPercent2));
                    rotation = MathHelper.Lerp(-3f, 2f, swingPercent);

                    if (Time == (int)(35 * speed))
                        SoundEngine.PlaySound(swingSound, Projectile.Center);

                    if (Time > (int)(60 * speed))
                    {
                        Time = -1;
                        SwingStyle++;
                        if (Main.myPlayer == Projectile.owner)
                            Projectile.Kill();
                    }

                    break;

                case 1:

                    Projectile.spriteDirection = -Projectile.direction;
                    swingPercent2 = Utils.GetLerpValue((int)(0 * speed), (int)(80 * speed), Time, true);
                    swingPercent = SwingProgress(MathHelper.SmoothStep(0, 1, swingPercent2));
                    rotation = MathHelper.Lerp(2f, -3f, swingPercent);
                    addRot = -MathHelper.TwoPi * MathHelper.SmoothStep(0, 1, MathHelper.SmoothStep(0, 1, Utils.GetLerpValue((int)(-10 * speed), (int)(70 * speed), Time, true)));

                    if (Time == (int)(20 * speed))
                        SoundEngine.PlaySound(swingSound.WithVolumeScale(0.3f).WithPitchOffset(-0.2f), Projectile.Center);
                    
                    if (Time == (int)(45 * speed))
                        SoundEngine.PlaySound(swingSound, Projectile.Center);

                    if (Time > (int)(70 * speed))
                    {
                        Time = -1;
                        SwingStyle--;
                        if (Main.myPlayer == Projectile.owner)
                            Projectile.Kill();
                    }

                    break;

                case 2:

                    Projectile.scale *= 1.2f;
                    Projectile.spriteDirection = Projectile.direction;
                    swingPercent2 = Utils.GetLerpValue(0, (int)(50 * speed), Time, true);
                    swingPercent = SwingProgress(swingPercent2);
                    rotation = MathHelper.Lerp(-3f, 6f, swingPercent);
                    addRot = -MathHelper.Pi * MathF.Sqrt(MathHelper.SmoothStep(0, 1, Utils.GetLerpValue(0, (int)(60 * speed), Time, true)));

                    if (Time == (int)(30 * speed))
                        SoundEngine.PlaySound(swingSound, Projectile.Center);

                    if (Time > (int)(50 * speed))
                    {
                        Time = -1;
                        SwingStyle--;
                        if (Main.myPlayer == Projectile.owner)
                            Projectile.Kill();
                    }

                    break;
            }

            Vector2 scytheEnd = Projectile.Center + Projectile.rotation.ToRotationVector2() * 90 * (Projectile.scale + (1f - swingPercent) * swingPercent * 4f);

            if (swingPercent2 > 0.2f && swingPercent2 < 0.8f)
            {
                for (int i = 0; i < 5; i++)
                {
                    Color glowColor = new GradientColor(SlimeUtils.GoozOilColors, 0.15f, 0.15f).Value;
                    glowColor.A = 240;
                    float scale = 0.8f + Main.rand.NextFloat();
                    Dust light = Dust.NewDustPerfect(scytheEnd + Main.rand.NextVector2Circular(20, 70).RotatedBy(Projectile.rotation), DustID.AncientLight, (Projectile.rotation + MathHelper.PiOver4 * Projectile.spriteDirection).ToRotationVector2().RotatedByRandom(0.5f) * Main.rand.NextFloat(-5f, 15f), 0, glowColor, scale);
                    light.noGravity = true;
                    light.noLightEmittence = true;
                }
            }

            Projectile.EmitEnchantmentVisualsAt(scytheEnd - new Vector2(75), 150, 150);

            Projectile.rotation = Projectile.velocity.ToRotation() + (rotation + MathHelper.WrapAngle(addRot)) * Projectile.direction;
            Player.SetCompositeArmFront(true, Player.CompositeArmStretchAmount.Full, Projectile.rotation - MathHelper.PiOver2);
            Vector2 position = Player.GetFrontHandPosition(Player.CompositeArmStretchAmount.Full, Projectile.rotation - MathHelper.PiOver2);

            position.Y += Player.gfxOffY;
            Projectile.Center = position;

            Player.SetDummyItemTime(2);
            Player.itemTime = 2;
            Player.ChangeDir(Projectile.direction);
            Player.heldProj = Projectile.whoAmI;

            Time++;

            slashRotation = Projectile.rotation;
            for (int i = ProjectileID.Sets.TrailCacheLength[Type] - 1; i > 0; i--)
                Projectile.oldRot[i] = Projectile.oldRot[i].AngleLerp(Projectile.oldRot[i - 1], 0.8f);

            Projectile.oldRot[0] = Projectile.oldRot[0].AngleLerp(slashRotation, 0.8f);
        }

        public float SwingProgress(float x)
        {
            float[] functions = new float[]
            {
                0.2f * MathF.Sqrt(x),
                18f * MathF.Pow(x - 0.25f, 4f) + 0.1f,
                0.3f * MathF.Cbrt(x - 0.7f) + 0.85f,
                0.2f * x + 0.85f
            };

            if (x < 0.05f)
                return MathHelper.Lerp(0f, functions[0], Utils.GetLerpValue(0f, 0.05f, x, true));
            if (x < 0.3f)
                return MathHelper.Lerp(functions[0], functions[1], Utils.GetLerpValue(0.2f, 0.3f, x, true));
            if (x < 0.71f)
                return MathHelper.Lerp(functions[1], functions[2], Utils.GetLerpValue(0.701f, 0.71f, x, true));
            else
                return MathHelper.Lerp(functions[2], functions[3], Utils.GetLerpValue(0.8f, 1f, x, true));
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            target.AddBuff(ModContent.BuffType<FusionBurn>(), 300);
        }

        public override void OnHitPlayer(Player target, Player.HurtInfo info)
        {
            target.AddBuff(ModContent.BuffType<FusionBurn>(), 300);
        }

        public override void ModifyDamageHitbox(ref Rectangle hitbox)
        {
            int size = (int)(400 * Player.GetAdjustedItemScale(Player.HeldItem));

            hitbox.Width = size;
            hitbox.Height = size;
            hitbox.Location = (Projectile.Center + Projectile.rotation.ToRotationVector2() * 150f - new Vector2(size * 0.5f)).ToPoint();
        }

        public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
        {
            if (swingPercent2 > 0.2f && swingPercent2 < 0.8f)
                return base.Colliding(projHitbox, targetHitbox);
            return false;
        }

        public override void SendExtraAI(BinaryWriter writer)
        {
            writer.Write(swingPercent2);
        }

        public override void ReceiveExtraAI(BinaryReader reader)
        {
            swingPercent2 = reader.ReadSingle();
        }

        public override void Load()
        {
            glowTexture = ModContent.Request<Texture2D>(Texture + "Glow", AssetRequestMode.ImmediateLoad).Value;
            bladeTexture = ModContent.Request<Texture2D>(Texture + "Blade", AssetRequestMode.ImmediateLoad).Value;
        }

        public static Texture2D glowTexture;
        public static Texture2D bladeTexture;

        public override bool PreDraw(ref Color lightColor)
        {
            if (Time < 1)
                return false;

            Texture2D texture = TextureAssets.Projectile[Type].Value;
            Texture2D swingTexture = AssetDirectory.Textures.SwordSwing.Basic;

            Vector2 endOrigin = new Vector2(0.35f, 0.5f + 0.25f * Projectile.spriteDirection);
            Vector2 origin = endOrigin;
            SpriteEffects spriteEffects = Projectile.spriteDirection > 0 ? SpriteEffects.None : SpriteEffects.FlipVertically;

            Color glowColor = new GradientColor(SlimeUtils.GoozOilColors, 0.15f, 0.15f).Value;
            glowColor.A = 0;

            float drawScale = Projectile.scale + (1f - swingPercent) * swingPercent * 4f;
            float swingStrength = Utils.GetLerpValue(0.1f, 0.7f, swingPercent, true) * Utils.GetLerpValue(0.9f, 0.6f, swingPercent2, true);

            switch (SwingStyle)
            {
                case 1:

                    origin = Vector2.Lerp(endOrigin, new Vector2(0.5f), Utils.GetLerpValue(0f, 0.4f, swingPercent * (0.7f - swingPercent) * 4f, true));
                    swingStrength = Utils.GetLerpValue(0.1f, 0.7f, swingPercent2, true) * Utils.GetLerpValue(0.9f, 0.6f, swingPercent2, true);

                    break;
            }

            if (swingStrength > 0f)
            {
                for (int i = 0; i < Projectile.oldRot.Length; i++)
                {
                    Color trailGlowColor = new GradientColor(SlimeUtils.GoozOilColors, 0.15f, 0.15f).ValueAt(Main.GlobalTimeWrappedHourly * 60 + i * 4) * 0.5f * swingStrength * (1f - (float)i / Projectile.oldRot.Length);
                    trailGlowColor.A = 0;

                    Main.EntitySpriteDraw(bladeTexture, Projectile.Center - Main.screenPosition, bladeTexture.Frame(), trailGlowColor, Projectile.oldRot[i] + MathHelper.PiOver4 * Projectile.spriteDirection, bladeTexture.Size() * origin, drawScale * (1.1f + i * 0.07f), spriteEffects, 0);
                }
            }

            Main.EntitySpriteDraw(texture, Projectile.Center - Main.screenPosition, texture.Frame(), lightColor, Projectile.rotation + MathHelper.PiOver4 * Projectile.spriteDirection, texture.Size() * origin, drawScale, spriteEffects, 0);
            Main.EntitySpriteDraw(glowTexture, Projectile.Center - Main.screenPosition, glowTexture.Frame(), glowColor, Projectile.rotation + MathHelper.PiOver4 * Projectile.spriteDirection, texture.Size() * origin, drawScale, spriteEffects, 0);

            for (int i = 0; i < 4; i++)
            {
                Color trailBackColor = new GradientColor(SlimeUtils.GoozOilColors, 0.15f, 0.15f).ValueAt(Main.GlobalTimeWrappedHourly * 60 + i * 4) * 0.7f * (1f - (float)i / Projectile.oldRot.Length);
                trailBackColor.A = 200;

                Rectangle frame = swingTexture.Frame(1, 4, 0, i);
                Main.EntitySpriteDraw(swingTexture, Projectile.Center - Main.screenPosition, frame, trailBackColor * 0.8f * swingStrength * (1f - i / 4f) * swingStrength, Projectile.oldRot[i ] - (0.3f + i * 0.2f) * Projectile.spriteDirection, frame.Size() * 0.5f, drawScale + 2.5f, spriteEffects, 0);
            }            
            for (int i = 0; i < 4; i++)
            {
                Color trailGlowColor = Color.Lerp(new Color(200, 200, 200, 0), new GradientColor(SlimeUtils.GoozOilColors, 0.15f, 0.15f).ValueAt(Main.GlobalTimeWrappedHourly * 60 + i * 4), 0.6f + i / 10f) * swingStrength * (1f - (float)i / Projectile.oldRot.Length);
                trailGlowColor.A = 0;

                Rectangle frame = swingTexture.Frame(1, 4, 0, i);
                Main.EntitySpriteDraw(swingTexture, Projectile.Center - Main.screenPosition, frame, trailGlowColor * 0.5f * (1f - i / 4f) * swingStrength, Projectile.oldRot[i * 2] - (0.3f + i * 0.1f) * Projectile.spriteDirection, frame.Size() * 0.5f, drawScale + 2.7f, spriteEffects, 0);
                Main.EntitySpriteDraw(swingTexture, Projectile.Center - Main.screenPosition, frame, trailGlowColor * 0.5f * (1f - i / 4f) * swingStrength, Projectile.oldRot[i] - (0.4f + i * 0.3f) * Projectile.spriteDirection, frame.Size() * 0.5f, (drawScale + 2.5f) * (1f + i / 10f), spriteEffects, 0);
            }

            Main.EntitySpriteDraw(swingTexture, Projectile.Center - Main.screenPosition, swingTexture.Frame(1, 4, 0, 3), new Color(50, 50, 50, 0) * swingStrength, Projectile.rotation - 0.3f * Projectile.spriteDirection, swingTexture.Frame(1, 4, 0, 3).Size() * 0.5f, drawScale + 1.85f, spriteEffects, 0);
            Main.EntitySpriteDraw(swingTexture, Projectile.Center - Main.screenPosition, swingTexture.Frame(1, 4, 0, 3), new Color(150, 150, 150, 0) * swingStrength, Projectile.rotation - 0.5f * Projectile.spriteDirection, swingTexture.Frame(1, 4, 0, 3).Size() * 0.5f, drawScale + 2.55f, spriteEffects, 0);

            float sparkStrength = (1f - swingPercent2) * swingPercent2 * 4f * Utils.GetLerpValue(0.3f, 0.5f, swingPercent, true);
            DrawSpark(Projectile.Center + new Vector2(75, 0).RotatedBy(Projectile.rotation + 0.2f * Projectile.spriteDirection) * (drawScale + 2.7f), glowColor * 0.5f * sparkStrength, sparkStrength);
            DrawSpark(Projectile.Center + new Vector2(75, 0).RotatedBy(Projectile.rotation - 0.5f * Projectile.spriteDirection) * (drawScale + 2.7f), glowColor * 0.7f * sparkStrength, 2f * sparkStrength);
            DrawSpark(Projectile.Center + new Vector2(75, 0).RotatedBy(Projectile.rotation - 1f * Projectile.spriteDirection) * (drawScale + 2.7f), glowColor * sparkStrength, 1.5f * sparkStrength);
            DrawSpark(Projectile.Center + new Vector2(75, 0).RotatedBy(Projectile.rotation - 1.4f * Projectile.spriteDirection) * (drawScale + 2.7f), glowColor * 0.7f * sparkStrength, sparkStrength);
            DrawSpark(Projectile.Center + new Vector2(75, 0).RotatedBy(Projectile.rotation - 1.8f * Projectile.spriteDirection) * (drawScale + 2.7f), glowColor * 0.3f * sparkStrength, 0.5f * sparkStrength);

            return false;
        }

        public void DrawSpark(Vector2 position, Color color, float scale)
        {
            Texture2D texture = AssetDirectory.Textures.Extra.Spark;
            Main.EntitySpriteDraw(texture, position - Main.screenPosition, texture.Frame(), color, MathHelper.PiOver4, texture.Size() * 0.5f, new Vector2(1f, 2f) * scale, 0, 0);
            Main.EntitySpriteDraw(texture, position - Main.screenPosition, texture.Frame(), color, -MathHelper.PiOver4, texture.Size() * 0.5f, new Vector2(1f, 2f) * scale, 0, 0);
            Main.EntitySpriteDraw(texture, position - Main.screenPosition, texture.Frame(), new Color(200, 200, 200, 0), MathHelper.PiOver4, texture.Size() * 0.5f, new Vector2(0.5f, 1f) * scale, 0, 0);
            Main.EntitySpriteDraw(texture, position - Main.screenPosition, texture.Frame(), new Color(200, 200, 200, 0), -MathHelper.PiOver4, texture.Size() * 0.5f, new Vector2(0.5f, 1f) * scale, 0, 0);
        }
    }
}
