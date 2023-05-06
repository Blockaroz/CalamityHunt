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
            switch (Mode)
            {
                default:
                case 0:
                    rotation = -2.1f + SwingProgress(MathF.Pow(Utils.GetLerpValue(0, 45, Time, true), 1.5f)) * 4f;

                    if (Time < 2 || Time > 40)
                        Projectile.damage = 0;
                    else
                        Projectile.damage = Owner.GetWeaponDamage(Owner.HeldItem);

                    if (Time > 45)
                    {
                        if (Owner.controlUseItem)
                        {
                            Time = -1;
                            Mode++;
                        }
                        else if (Time > 65)
                            canKill = true;

                        if (Owner.altFunctionUse == 1 && Owner.GetModPlayer<GoozmaWeaponsPlayer>().parasolBlood >= GoozmaWeaponsPlayer.ParasolBloodMinimum)
                        {
                            Time = -1;
                            Mode = 2;
                        }
                    }

                    if (Time == 22 && !Main.dedServ)
                        SoundEngine.PlaySound(SoundID.DD2_MonkStaffSwing.WithPitchOffset(0.5f), Projectile.Center);

                    Projectile.spriteDirection = -1;

                    break;

                case 1:

                    rotation = 2f - SwingProgress(MathF.Pow(Utils.GetLerpValue(0, 35, Time, true), 3f)) * 4f;

                    if (Time > 35)
                    {
                        Projectile.damage = 0;

                        if (Owner.controlUseItem)
                        {
                            Time = -1;
                            Mode--;
                        }
                        else if (Time > 55)
                            canKill = true;

                        if (Owner.altFunctionUse == 1 && Owner.GetModPlayer<GoozmaWeaponsPlayer>().parasolBlood >= GoozmaWeaponsPlayer.ParasolBloodMinimum)
                        {
                            Time = -1;
                            Mode = 2;
                        }
                    }
                    else
                        Projectile.damage = Owner.GetWeaponDamage(Owner.HeldItem);

                    if (Time == 15 && !Main.dedServ)
                        SoundEngine.PlaySound(SoundID.DD2_MonkStaffSwing.WithPitchOffset(0.5f), Projectile.Center);
                    
                    Projectile.spriteDirection = 1;

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
                            Vector2 pos = Projectile.Center + Main.rand.NextVector2Circular(30, 30) + Projectile.velocity * 20;

                        }
                    }
                    else
                    {
                        Owner.GetModPlayer<GoozmaWeaponsPlayer>().parasolBlood = 0;
                        Projectile.frame = 3;
                        Projectile.scale = 1.1f * Owner.GetAdjustedItemScale(Owner.HeldItem) * (0.7f + MathF.Sqrt(Utils.GetLerpValue(25, 27, Time, true)) * 0.3f);
                    }

                    if (Time > 40)
                    {
                        if (Owner.controlUseItem)
                        {
                            Time = -1;
                            Mode++;
                        }
                        else if (Time > 50)
                            canKill = true;
                    }


                    break;                
                
                case 3:

                    Projectile.localNPCHitCooldown = 7;
                    addRot = -MathHelper.TwoPi * 4f * MathHelper.SmoothStep(0, 1, Utils.GetLerpValue(15, 75, Time, true));
                    rotation = (1f - SwingProgress(MathF.Pow(Utils.GetLerpValue(0, 90, Time, true), 1.5f)) * 3f) * MathHelper.SmoothStep(0, 1, Utils.GetLerpValue(0, 10, Time, true));
                    Projectile.direction = Projectile.velocity.X < 0 ? -1 : 1;
                    Projectile.velocity = Vector2.Lerp(Projectile.velocity, new Vector2(Projectile.direction * 5, 0), 0.05f);
                    Projectile.frame = (int)(Utils.GetLerpValue(93, 87, Time, true) * 3);
                    Projectile.scale = 1.1f * Owner.GetAdjustedItemScale(Owner.HeldItem) * (1f + MathF.Sqrt(Utils.GetLerpValue(25, 0, Time, true) * Utils.GetLerpValue(95, 75, Time, true)) * 0.2f);

                    if (Time < 20)
                        Projectile.damage = 0;
                    else
                        Projectile.damage = Owner.GetWeaponDamage(Owner.HeldItem);

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

                    Projectile.spriteDirection = 1;

                    break;
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
                Projectile.oldRot[i] = Projectile.oldRot[i].AngleLerp(Projectile.oldRot[i - 1], 0.9f);

            Projectile.oldRot[0] = Projectile.oldRot[0].AngleLerp(slashRotation, 0.9f);
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
                CombatText.NewText(new Rectangle((int)Owner.position.X, (int)Owner.position.Y - 30, Owner.width, 30), new Color(150, 10, 0), 50 + (int)(damageDone * 0.1f), false, true);
                Owner.GetModPlayer<GoozmaWeaponsPlayer>().parasolBlood += 50 + (int)(damageDone * 0.02f);
                Owner.GetModPlayer<GoozmaWeaponsPlayer>().parasolBloodWaitTime = 120;
            }

            if (!Main.dedServ)
                SoundEngine.PlaySound(SoundID.DD2_MonkStaffGroundImpact, Projectile.Center);
        }

        public override void OnHitPlayer(Player target, Player.HurtInfo info)
        {
            if (Mode < 2)
            {
                Owner.GetModPlayer<GoozmaWeaponsPlayer>().parasolBlood += 50 + (int)(info.Damage * 0.02f);
                Owner.GetModPlayer<GoozmaWeaponsPlayer>().parasolBloodWaitTime = 120;
            }

            if (!Main.dedServ)
                SoundEngine.PlaySound(SoundID.DD2_MonkStaffGroundImpact, Projectile.Center);
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
            //Texture2D splatTexture = ModContent.Request<Texture2D>(Texture + "Splat").Value;
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

                            slashPower = Utils.GetLerpValue(25, 35, Time - i * 3, true) * Utils.GetLerpValue(43, 38, Time - i, true);

                            break;

                        case 1:

                            slashPower = Utils.GetLerpValue(25, 30, Time - i * 3, true) * Utils.GetLerpValue(40, 35, Time - i, true);

                            break;

                        case 3:

                            slashPower = Utils.GetLerpValue(10, 30, Time - i * 3, true) * Utils.GetLerpValue(90, 70, Time - i, true);

                            break;
                    }
                    SpriteEffects slashEffects = Projectile.spriteDirection < 0 ? SpriteEffects.None : SpriteEffects.FlipHorizontally;

                    Rectangle slashFrame = slash.Frame(1, 4, 0, i);
                    Color slashColor = Color.Lerp(new Color(10, 0, 0, 128), new Color(255, 8, 20), (i / 3f + 0.01f) * (0.3f + slashPower * 0.7f));
                    Main.EntitySpriteDraw(slash, Projectile.Center - Main.screenPosition, slashFrame, slashColor * slashPower, Projectile.oldRot[i] - MathHelper.PiOver2 - MathHelper.PiOver2 * Projectile.spriteDirection, slashFrame.Size() * 0.5f, Projectile.scale * 1.8f, slashEffects, 0);
                }
            }

            if (Owner.GetModPlayer<GoozmaWeaponsPlayer>().parasolBlood >= GoozmaWeaponsPlayer.ParasolBloodMinimum || Mode > 1)
            {
                for (int i = 0; i < 6; i++)
                {
                    Vector2 off = new Vector2(3).RotatedBy(MathHelper.TwoPi / 6f * i + Projectile.rotation);
                    Main.EntitySpriteDraw(texture, Projectile.Center + off - Main.screenPosition, frame, Color.Black * 0.2f, Projectile.rotation - MathHelper.PiOver4 * Projectile.direction, origin, Projectile.scale, spriteEffects, 0);
                    Main.EntitySpriteDraw(texture, Projectile.Center + off - Main.screenPosition, frame, new Color(255, 20, 30, 0), Projectile.rotation - MathHelper.PiOver4 * Projectile.direction, origin, Projectile.scale, spriteEffects, 0);
                }
            }

            Main.EntitySpriteDraw(texture, Projectile.Center - Main.screenPosition, frame, lightColor, Projectile.rotation - MathHelper.PiOver4 * Projectile.direction, origin, Projectile.scale, spriteEffects, 0);

            if (Owner.GetModPlayer<GoozmaWeaponsPlayer>().parasolBlood >= GoozmaWeaponsPlayer.ParasolBloodMinimum && Mode < 2)
            {
                Vector2 tip = new Vector2(Projectile.scale * 92, -5 * Projectile.direction).RotatedBy(Projectile.rotation);
                Main.EntitySpriteDraw(flare, Projectile.Center + tip - Main.screenPosition, flare.Frame(), Color.Black * 0.7f, 0, flare.Size() * 0.5f, new Vector2(0.3f, 1.5f), spriteEffects, 0);
                Main.EntitySpriteDraw(flare, Projectile.Center + tip - Main.screenPosition, flare.Frame(), Color.Black * 0.7f, MathHelper.PiOver2, flare.Size() * 0.5f, new Vector2(0.3f, 2f), spriteEffects, 0);
                Main.EntitySpriteDraw(flare, Projectile.Center + tip - Main.screenPosition, flare.Frame(), new Color(255, 20, 30, 0), 0, flare.Size() * 0.5f, new Vector2(0.4f, 1.5f), spriteEffects, 0);
                Main.EntitySpriteDraw(flare, Projectile.Center + tip - Main.screenPosition, flare.Frame(), new Color(255, 20, 30, 0), MathHelper.PiOver2, flare.Size() * 0.5f, new Vector2(0.4f, 2f), spriteEffects, 0);
            }
       
            if (Mode == 2)
            {
                //Rectangle frame = splatTexture.Frame();
            }
        }
    }
}
