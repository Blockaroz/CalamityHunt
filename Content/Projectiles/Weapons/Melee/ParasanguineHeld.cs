using CalamityHunt.Common.Systems;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using System;
using System.Collections.Generic;
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
    public class ParasanguineHeld : ModProjectile
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
            Projectile.hide = true;
            Projectile.manualDirectionChange = true;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 12;
            Projectile.DamageType = DamageClass.Melee;
            Projectile.noEnchantmentVisuals = true;
        }

        public ref float Time => ref Projectile.ai[0];
        public ref float Mode => ref Projectile.ai[1];

        public ref Player Owner => ref Main.player[Projectile.owner];

        private float slashRotation;

        public override void AI()
        {
            if (!Owner.active || Owner.dead || Owner.noItems || Owner.CCed)
            {
                Projectile.Kill();
                return;
            }

            if (Time <= 1 && Mode != 3)
            {
                Projectile.velocity = Owner.DirectionTo(Main.MouseWorld).SafeNormalize(Vector2.Zero) * 5f;
                Projectile.direction = Projectile.velocity.X < 0 ? -1 : 1;
                Projectile.scale = 1.1f * Owner.GetAdjustedItemScale(Owner.HeldItem);
                Projectile.localNPCHitCooldown = 12;
            }

            float rotation = 0;
            float addRot = 0;
            bool canKill = false;

            SoundStyle swingSound = SoundID.DD2_SonicBoomBladeSlash;
            swingSound.MaxInstances = 0;
            swingSound.Volume = 1.5f;

            switch (Mode)
            {
                default:
                case 0:
                    rotation = -2.1f + SwingProgress(MathF.Pow(Utils.GetLerpValue(0, 45, Time, true), 1.5f)) * 4f;

                    if (Time < 5 || Time > 40)
                        Projectile.damage = 0;
                    else
                    {
                        Projectile.damage = Owner.GetWeaponDamage(Owner.HeldItem);

                        for (int i = 0; i < 2; i++)
                        {
                            Vector2 pos = Projectile.Center + Projectile.rotation.ToRotationVector2() * 70f + Main.rand.NextVector2Circular(70, 60).RotatedBy(Projectile.rotation);
                            Dust blood = Dust.NewDustPerfect(pos, DustID.Blood, Main.rand.NextVector2Circular(4, 4), 0, Color.DarkRed, 1f + Main.rand.NextFloat());
                            blood.noGravity = true;
                        }
                    }

                    if (Time > 16 && Time < 33 && Time % 2 == 0)
                    {
                        Vector2 stickyVelocity = (Projectile.velocity.ToRotation() + (rotation * 0.6f - 0.3f) * Projectile.direction).ToRotationVector2() * Main.rand.NextFloat(8f, 18f) * Utils.GetLerpValue(0, 35, Time, true);
                        Projectile.NewProjectileDirect(Projectile.GetSource_FromThis(), Projectile.Center + Projectile.rotation.ToRotationVector2() * 20f, stickyVelocity, ModContent.ProjectileType<ParasanguineBlood>(), Projectile.damage / 3, 0.5f, Owner.whoAmI);
                    }

                    if (Time > 45)
                    {
                        if (Owner.controlUseItem)
                        {
                            Time = -1;
                            Mode++;
                        }
                        else if (Time > 65)
                            canKill = true;

                        if (Owner.altFunctionUse == 1 && Owner.GetModPlayer<GoozmaWeaponsPlayer>().ParasolBloodPercent > 0.5f)
                        {
                            Time = -1;
                            Mode = 2;
                        }
                    }

                    if (Time == 22 && !Main.dedServ)
                        SoundEngine.PlaySound(swingSound.WithPitchOffset(0.2f), Projectile.Center);

                    Projectile.spriteDirection = -Projectile.direction;

                    break;

                case 1:

                    rotation = 2f - SwingProgress(MathF.Pow(Utils.GetLerpValue(0, 35, Time, true), 3f)) * 4f;

                    if (Time > 22 && Time < 32)
                    {
                        Vector2 stickyVelocity = (Projectile.velocity.ToRotation() + (rotation * 0.7f - 0.4f) * Projectile.direction).ToRotationVector2() * Main.rand.NextFloat(15f, 20f) * Utils.GetLerpValue(0, 35, Time, true) * Utils.GetLerpValue(35, 30, Time, true) * 1.1f;
                        Projectile.NewProjectileDirect(Projectile.GetSource_FromThis(), Projectile.Center + Projectile.rotation.ToRotationVector2() * 20f, stickyVelocity, ModContent.ProjectileType<ParasanguineBlood>(), Projectile.damage / 3, 0.5f, Owner.whoAmI);
                    }

                    if (Time > 40)
                    {
                        Projectile.damage = 0;

                        if (Owner.controlUseItem)
                        {
                            Time = -1;
                            Mode--;
                        }
                        else if (Time > 55)
                            canKill = true;

                        if (Owner.altFunctionUse == 1 && Owner.GetModPlayer<GoozmaWeaponsPlayer>().ParasolBloodPercent > 0.5f)
                        {
                            Time = -1;
                            Mode = 2;
                        }
                    }
                    else
                    {
                        Projectile.damage = Owner.GetWeaponDamage(Owner.HeldItem);

                        for (int i = 0; i < 2; i++)
                        {
                            Vector2 pos = Projectile.Center + Projectile.rotation.ToRotationVector2() * 70f + Main.rand.NextVector2Circular(70, 60).RotatedBy(Projectile.rotation);
                            Dust blood = Dust.NewDustPerfect(pos, DustID.Blood, Main.rand.NextVector2Circular(4, 4), 0, Color.DarkRed, 1f + Main.rand.NextFloat());
                            blood.noGravity = true;
                        }
                    }
                    if (Time == 15 && !Main.dedServ)
                        SoundEngine.PlaySound(swingSound.WithPitchOffset(0.2f), Projectile.Center);
                    
                    Projectile.spriteDirection = Projectile.direction;

                    break;

                case 2:

                    rotation = 0;
                    Projectile.damage = 0;
                    Projectile.direction = Projectile.velocity.X < 0 ? -1 : 1;

                    if (Time < 25)
                    {
                        Projectile.velocity = Vector2.Lerp(Projectile.velocity, Owner.DirectionTo(Main.MouseWorld).SafeNormalize(Vector2.Zero) * 5f, 0.3f * Utils.GetLerpValue(25, 0, Time, true));

                        Projectile.frame = (int)(Utils.GetLerpValue(5, 15, Time, true) * 2);
                        Projectile.scale = 1.1f * Owner.GetAdjustedItemScale(Owner.HeldItem) * (0.7f + MathF.Pow(Utils.GetLerpValue(25, 0, Time, true), 2f) * 0.2f);
                        for (int i = 0; i < 3; i++)
                        {
                            Vector2 pos = Projectile.Center + Main.rand.NextVector2Circular(120, 120) + Projectile.velocity * 20;
                            Dust blood = Dust.NewDustPerfect(pos, DustID.Blood, pos.DirectionTo(Projectile.Center) * Main.rand.NextFloat(5f), 100, Color.DarkRed, 1f + Main.rand.NextFloat(2f));
                            blood.noGravity = true;
                        }
                    }
                    else
                    {
                        Owner.GetModPlayer<GoozmaWeaponsPlayer>().parasolBlood -= 100;
                        Projectile.frame = 3;
                        Projectile.scale = 1.1f * Owner.GetAdjustedItemScale(Owner.HeldItem) * (0.7f + MathF.Sqrt(Utils.GetLerpValue(25, 27, Time, true)) * 0.3f);
                    }

                    if (Time > 25 && Time < 35)
                    {
                        for (int i = 0; i < 5; i++)
                        {
                            Vector2 stickyVelocity = Projectile.velocity.SafeNormalize(Vector2.Zero).RotatedByRandom(1f) * Main.rand.NextFloat(18f, 32f) * Utils.GetLerpValue(0, 35, Time, true);
                            Projectile.NewProjectileDirect(Projectile.GetSource_FromThis(), Projectile.Center + Projectile.velocity.SafeNormalize(Vector2.Zero) * 20f, stickyVelocity, ModContent.ProjectileType<ParasanguineBlood>(), Owner.GetWeaponDamage(Owner.HeldItem) / 2, 0.5f, Owner.whoAmI);
                        }
                    }

                    if (Time > 40)
                    {
                        if (Owner.controlUseItem && Owner.GetModPlayer<GoozmaWeaponsPlayer>().ParasolBloodPercent > 0.5f)
                        {
                            Time = -1;
                            Mode++;
                        }
                        else if (Time > 50)
                            canKill = true;
                    }


                    break;                
                
                case 3:

                    addRot = -MathHelper.TwoPi * 4f * MathHelper.SmoothStep(0, 1, Utils.GetLerpValue(15, 75, Time, true));
                    rotation = (1f - SwingProgress(MathF.Pow(Utils.GetLerpValue(0, 90, Time, true), 1.5f)) * 3f) * MathHelper.SmoothStep(0, 1, Utils.GetLerpValue(0, 10, Time, true));
                    Projectile.direction = Projectile.velocity.X < 0 ? -1 : 1;
                    Projectile.velocity = Vector2.Lerp(Projectile.velocity, new Vector2(Projectile.direction * 5, 0), 0.05f);
                    Projectile.frame = (int)(Utils.GetLerpValue(93, 87, Time, true) * 3);
                    Projectile.scale = 1.1f * Owner.GetAdjustedItemScale(Owner.HeldItem) * (1f + MathF.Sqrt(Utils.GetLerpValue(25, 0, Time, true) * Utils.GetLerpValue(95, 75, Time, true)) * 0.2f);

                    if (Time < 30 && Time > 70)
                        Projectile.damage = 0;
                    else
                    {
                        Owner.GetModPlayer<GoozmaWeaponsPlayer>().parasolBlood -= 100;
                        Projectile.damage = Owner.GetWeaponDamage(Owner.HeldItem);

                        for (int i = 0; i < 7; i++)
                        {
                            Vector2 pos = Projectile.Center + Projectile.rotation.ToRotationVector2() * 80f + Main.rand.NextVector2Circular(120, 60).RotatedBy(Projectile.rotation);
                            Vector2 vel = (Projectile.rotation - MathHelper.PiOver2 * Projectile.spriteDirection).ToRotationVector2() * Main.rand.NextFloat(5f);
                            Dust blood = Dust.NewDustPerfect(pos, DustID.Blood, vel.RotatedByRandom(1f), 0, Color.DarkRed, 1f + Main.rand.NextFloat());
                            blood.noGravity = true;
                        }
                    }

                    if (Time >= 30 && Time <= 65)
                    {
                        Projectile.localNPCHitCooldown = 10;

                        for (int i = 0; i < 2; i++)
                        {
                            Vector2 stickyVelocity = (Projectile.rotation + i * 0.2f * Projectile.direction).ToRotationVector2() * Main.rand.NextFloat(18f, 32f) * Utils.GetLerpValue(0, 35, Time, true);
                            Projectile.NewProjectileDirect(Projectile.GetSource_FromThis(), Projectile.Center + Projectile.velocity.SafeNormalize(Vector2.Zero) * 20f, stickyVelocity, ModContent.ProjectileType<ParasanguineBlood>(), Projectile.damage / 2, 0.5f, Owner.whoAmI);
                        }

                        if ((Time - 20) % 10 == 0 && !Main.dedServ)
                            SoundEngine.PlaySound(swingSound.WithPitchOffset(0.5f), Projectile.Center);

                    }

                    if (Time > 95)
                    {
                        if (Owner.controlUseItem)
                        {
                            Time = -1;
                            Mode = 0;
                        }
                        else
                            canKill = true;
                    }

                    Projectile.spriteDirection = Projectile.direction;

                    break;
            }

            if (Projectile.damage > 0)
            {
                Projectile.EmitEnchantmentVisualsAt(Projectile.Center + Projectile.rotation.ToRotationVector2() * 200 - new Vector2(100), 200, 200);

                if (Mode < 2)
                {
                    Owner.GetModPlayer<GoozmaWeaponsPlayer>().parasolBlood += 10;
                    Owner.GetModPlayer<GoozmaWeaponsPlayer>().parasolBloodWaitTime += 10;
                }
            }

            if (canKill)
                Projectile.Kill();

            Projectile.rotation = Projectile.velocity.ToRotation() + (rotation + MathHelper.WrapAngle(addRot)) * Projectile.direction;
            Owner.SetCompositeArmFront(true, Player.CompositeArmStretchAmount.Full, Projectile.rotation - MathHelper.PiOver2);
            Vector2 position = Owner.GetFrontHandPosition(Player.CompositeArmStretchAmount.Full, Projectile.rotation - MathHelper.PiOver2);
            if (Mode == 3)
            {
                Owner.SetCompositeArmFront(true, Player.CompositeArmStretchAmount.Full, Projectile.velocity.ToRotation() + (rotation * 0.5f - MathF.Sin(MathHelper.WrapAngle(addRot)) * 0.3f) * Projectile.direction - MathHelper.PiOver2);
                position = Owner.GetFrontHandPosition(Player.CompositeArmStretchAmount.Full, Projectile.velocity.ToRotation() + (rotation * 0.7f - MathF.Sin(MathHelper.WrapAngle(addRot)) * 0.05f) * Projectile.direction - MathHelper.PiOver2);
            }
            position.Y += Owner.gfxOffY;
            Projectile.Center = position;

            Owner.itemAnimation = 2;
            Owner.itemTime = 2;
            Owner.ChangeDir(Projectile.direction);
            Owner.heldProj = Projectile.whoAmI;

            Time++;

            slashRotation = slashRotation.AngleLerp(Projectile.rotation, 0.25f);
            for (int i = ProjectileID.Sets.TrailCacheLength[Type] - 1; i > 0; i--)
                Projectile.oldRot[i] = Projectile.oldRot[i].AngleLerp(Projectile.oldRot[i - 1], 0.5f);

            Projectile.oldRot[0] = Projectile.oldRot[0].AngleLerp(slashRotation, 0.5f);
        }

        public float SwingProgress(float x)
        {
            float[] functions = new float[]
            {
                -0.1f * MathF.Cbrt(x / 0.15f),
                MathF.Pow(2.1f * x - 0.315f, 3f),
                MathF.Pow(3.3f * x - 2.475f, 3f) + 1f,
                -2f * MathF.Pow(x - 0.85f, 2f) + 1.05f
            };

            if (x < 0f)
                return 0f;
            if (x < 0.25f)
                return MathHelper.Lerp(functions[0], functions[1], Utils.GetLerpValue(0.1f, 0.25f, x, true));
            if (x < 0.6f)
                return MathHelper.Lerp(functions[1], functions[2], Utils.GetLerpValue(0.5f, 0.6f, x, true));
            if (x < 0.7f)
                return MathHelper.Lerp(functions[2], functions[3], Utils.GetLerpValue(0.6f, 0.7f, x, true));
            else
                return MathHelper.Lerp(functions[3], 1f, Utils.GetLerpValue(0.7f, 0.97f, x, true));
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            if (Mode < 2)
            {
                Owner.GetModPlayer<GoozmaWeaponsPlayer>().parasolBlood += 500 + (int)(damageDone * 0.01f);
                Owner.GetModPlayer<GoozmaWeaponsPlayer>().parasolBloodWaitTime += 30;

                CombatText.NewText(new Rectangle((int)Owner.position.X, (int)Owner.position.Y - 30, Owner.width, 30), new Color(150, 10, 0), 500 + (int)(damageDone * 0.01f), true, true);
            }

            if (!Main.dedServ)
                SoundEngine.PlaySound(SoundID.DD2_MonkStaffGroundMiss, Projectile.Center);
        }

        public override void OnHitPlayer(Player target, Player.HurtInfo info)
        {
            if (Mode < 2)
            {
                Owner.GetModPlayer<GoozmaWeaponsPlayer>().parasolBlood += 500 + (int)(info.Damage * 0.01f);
                Owner.GetModPlayer<GoozmaWeaponsPlayer>().parasolBloodWaitTime += 30;

                CombatText.NewText(new Rectangle((int)Owner.position.X, (int)Owner.position.Y - 30, Owner.width, 30), new Color(150, 10, 0), 500 + (int)(info.Damage * 0.01f), true, true);
            }

            if (!Main.dedServ)
                SoundEngine.PlaySound(SoundID.DD2_MonkStaffGroundMiss, Projectile.Center);
        }

        public override void ModifyDamageHitbox(ref Rectangle hitbox)
        {
            int size = (int)(150 * Owner.GetAdjustedItemScale(Owner.HeldItem));

            hitbox.Width = size;
            hitbox.Height = size;
            hitbox.Location = (Projectile.Center + Projectile.rotation.ToRotationVector2() * 100f - new Vector2(size * 0.5f)).ToPoint();
        }

        public override bool PreDraw(ref Color lightColor) => false;

        public override void PostDraw(Color lightColor)
        {
            Texture2D texture = ModContent.Request<Texture2D>(Texture).Value;
            Texture2D flare = TextureAssets.Extra[89].Value;
            Texture2D slash = ModContent.Request<Texture2D>($"{nameof(CalamityHunt)}/Assets/Textures/BloodSwing").Value;
            Texture2D splatTexture = ModContent.Request<Texture2D>(Texture + "Splat").Value;
            Rectangle frame = texture.Frame(1, 4, 0, Projectile.frame);
            Vector2 origin = frame.Size() * new Vector2(0.1f, 0.5f - 0.35f * Projectile.direction);
            SpriteEffects spriteEffects = Projectile.direction < 0 ? SpriteEffects.None : SpriteEffects.FlipVertically;

            if (Mode != 2)
            {
                for (int i = 0; i < 4; i++)
                {
                    float slashPower = 0f;
                    switch (Mode)
                    {
                        case 0:

                            slashPower = Utils.GetLerpValue(20, 35, Time + i * 2f - 6f, true) * Utils.GetLerpValue(40, 35, Time - i * 3f + 2f, true);

                            break;

                        case 1:

                            slashPower = Utils.GetLerpValue(20, 35, Time + i * 2f - 6f, true) * Utils.GetLerpValue(40, 35, Time - i * 3f + 2f, true);

                            break;

                        case 3:

                            slashPower = Utils.GetLerpValue(25, 40, Time + i * 2f - 6f, true) * Utils.GetLerpValue(90, 70, Time - i * 3f + 2f, true);

                            break;
                    }

                    SpriteEffects slashEffects = Projectile.spriteDirection < 0 ? SpriteEffects.None : SpriteEffects.FlipHorizontally;
                    Rectangle slashFrame = slash.Frame(1, 4, 0, i);
                    Color slashColor = Color.Lerp(new Color(8, 0, 0), new Color(180, 8, 20, 230), (i / 3f + 0.01f) * (0.3f + slashPower * 0.7f)) * 0.7f;
                    Main.EntitySpriteDraw(slash, Projectile.Center - Main.screenPosition, slashFrame, slashColor * slashPower, Projectile.oldRot[i] - MathHelper.PiOver2 - (MathHelper.PiOver2 - 0.3f) * Projectile.spriteDirection, slashFrame.Size() * new Vector2(0.5f + 0.03f * Projectile.spriteDirection, 0.53f), Projectile.scale * 1.8f, slashEffects, 0);
                }
            }

            float strength = Utils.GetLerpValue(0.5f, 0.9f, Owner.GetModPlayer<GoozmaWeaponsPlayer>().ParasolBloodPercent, true);
            if (Owner.GetModPlayer<GoozmaWeaponsPlayer>().ParasolBloodPercent > 0.5f || Mode > 1)
            {
                for (int i = 0; i < 6; i++)
                {
                    Vector2 off = new Vector2(3).RotatedBy(MathHelper.TwoPi / 6f * i + Projectile.rotation);
                    Main.EntitySpriteDraw(texture, Projectile.Center + off - Main.screenPosition, frame, Color.Black * 0.15f * strength, Projectile.rotation - MathHelper.PiOver4 * Projectile.direction, origin, Projectile.scale, spriteEffects, 0);
                    Main.EntitySpriteDraw(texture, Projectile.Center + off - Main.screenPosition, frame, new Color(255, 20, 30, 0) * strength, Projectile.rotation - MathHelper.PiOver4 * Projectile.direction, origin, Projectile.scale, spriteEffects, 0);
                }
            }

            Main.EntitySpriteDraw(texture, Projectile.Center - Main.screenPosition, frame, lightColor, Projectile.rotation - MathHelper.PiOver4 * Projectile.direction, origin, Projectile.scale, spriteEffects, 0);

            if (Owner.GetModPlayer<GoozmaWeaponsPlayer>().ParasolBloodPercent > 0.5f && Mode < 2)
            {
                Vector2 tip = new Vector2(Projectile.scale * 92, -5 * Projectile.direction).RotatedBy(Projectile.rotation);
                Main.EntitySpriteDraw(flare, Projectile.Center + tip - Main.screenPosition, flare.Frame(), Color.Black * 0.7f * strength, 0, flare.Size() * 0.5f, new Vector2(0.3f, 1.5f), spriteEffects, 0);
                Main.EntitySpriteDraw(flare, Projectile.Center + tip - Main.screenPosition, flare.Frame(), Color.Black * 0.7f * strength, MathHelper.PiOver2, flare.Size() * 0.5f, new Vector2(0.3f, 2f), spriteEffects, 0);
                Main.EntitySpriteDraw(flare, Projectile.Center + tip - Main.screenPosition, flare.Frame(), new Color(255, 20, 30, 0) * strength, 0, flare.Size() * 0.5f, new Vector2(0.4f, 1.5f), spriteEffects, 0);
                Main.EntitySpriteDraw(flare, Projectile.Center + tip - Main.screenPosition, flare.Frame(), new Color(255, 20, 30, 0) * strength, MathHelper.PiOver2, flare.Size() * 0.5f, new Vector2(0.4f, 2f), spriteEffects, 0);
            }
       
            if (Mode == 2 && Time >= 25 && Time <= 45)
            {
                Rectangle splatFrame = splatTexture.Frame(1, 6, 0, (int)(Utils.GetLerpValue(25, 45, Time, true) * 6f));
                Vector2 tip = new Vector2(Projectile.scale * 60, -5 * Projectile.direction).RotatedBy(Projectile.rotation);
                Main.EntitySpriteDraw(splatTexture, Projectile.Center + tip * 1.03f - Main.screenPosition, splatFrame, Color.Black * 0.8f, Projectile.rotation + MathHelper.PiOver2, splatFrame.Size() * new Vector2(0.5f, 1f), 0.7f + Projectile.scale * 0.5f, 0, 0);
                Main.EntitySpriteDraw(splatTexture, Projectile.Center + tip - Main.screenPosition, splatFrame, new Color(255, 8, 20) * 0.6f, Projectile.rotation + MathHelper.PiOver2, splatFrame.Size() * new Vector2(0.5f, 1f), 0.66f + Projectile.scale * 0.5f, 0, 0);
            }
        }
    }
}
