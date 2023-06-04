using CalamityHunt.Common.Systems;
using CalamityHunt.Common.UI;
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
    public class ScytheOfTheOldGodHeld : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.HeldProjDoesNotUsePlayerGfxOffY[Type] = true;
            ProjectileID.Sets.TrailCacheLength[Type] = 4;
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

            if (Time <= 1)
            {
                Projectile.velocity = Owner.DirectionTo(Main.MouseWorld).SafeNormalize(Vector2.Zero) * 5f;
                Projectile.direction = Projectile.velocity.X < 0 ? -1 : 1;
                Projectile.scale = 1.1f * Owner.GetAdjustedItemScale(Owner.HeldItem);
                Projectile.localNPCHitCooldown = 12;
            }

            float rotation = 0;
            float addRot = 0;
            bool canKill = false;

            SoundStyle swingSound = SoundID.Item61;
            swingSound.MaxInstances = 0;
            swingSound.Volume = 1.5f;

            Projectile.EmitEnchantmentVisualsAt(Projectile.Center + Projectile.rotation.ToRotationVector2() * 100 - new Vector2(50), 100, 100);

            if (canKill)
                Projectile.Kill();

            Projectile.rotation = Projectile.velocity.ToRotation() + (rotation + MathHelper.WrapAngle(addRot)) * Projectile.direction;
            Owner.SetCompositeArmFront(true, Player.CompositeArmStretchAmount.Full, Projectile.rotation - MathHelper.PiOver2);
            Vector2 position = Owner.GetFrontHandPosition(Player.CompositeArmStretchAmount.Full, Projectile.rotation - MathHelper.PiOver2);

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

            SoundEngine.PlaySound(SoundID.DD2_MonkStaffGroundMiss, Projectile.Center);

            WeaponBars.DisplayBar();
        }

        public override void OnHitPlayer(Player target, Player.HurtInfo info)
        {
            if (Mode < 2)
            {
                Owner.GetModPlayer<GoozmaWeaponsPlayer>().parasolBlood += 500 + (int)(info.Damage * 0.01f);
                Owner.GetModPlayer<GoozmaWeaponsPlayer>().parasolBloodWaitTime += 30;

                CombatText.NewText(new Rectangle((int)Owner.position.X, (int)Owner.position.Y - 30, Owner.width, 30), new Color(150, 10, 0), 500 + (int)(info.Damage * 0.01f), true, true);
            }

            SoundEngine.PlaySound(SoundID.DD2_MonkStaffGroundMiss, Projectile.Center);

            WeaponBars.DisplayBar();
        }

        public override void ModifyDamageHitbox(ref Rectangle hitbox)
        {
            int size = (int)(150 * Owner.GetAdjustedItemScale(Owner.HeldItem));

            hitbox.Width = size;
            hitbox.Height = size;
            hitbox.Location = (Projectile.Center + Projectile.rotation.ToRotationVector2() * 100f - new Vector2(size * 0.5f)).ToPoint();
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D texture = TextureAssets.Projectile[Type].Value;
            Texture2D flare = TextureAssets.Extra[89].Value;
            Texture2D slash = ModContent.Request<Texture2D>($"{nameof(CalamityHunt)}/Assets/Textures/BloodSwing").Value;
            Rectangle frame = texture.Frame(1, 4, 0, Projectile.frame);
            Vector2 origin = frame.Size() * new Vector2(0.1f, 0.5f - 0.35f * Projectile.direction);
            SpriteEffects spriteEffects = Projectile.direction < 0 ? SpriteEffects.None : SpriteEffects.FlipVertically;

            if (Mode != 2)
            {
                for (int i = 0; i < 4; i++)
                {
                    SpriteEffects slashEffects = Projectile.spriteDirection < 0 ? SpriteEffects.None : SpriteEffects.FlipHorizontally;
                    Rectangle slashFrame = slash.Frame(1, 4, 0, i);
                    Color slashColor = Color.White;
                    Main.EntitySpriteDraw(slash, Projectile.Center - Main.screenPosition, slashFrame, slashColor, Projectile.rotation - MathHelper.PiOver2 - (MathHelper.PiOver2 - 0.3f) * Projectile.spriteDirection, slashFrame.Size() * new Vector2(0.5f + 0.02f * Projectile.spriteDirection, 0.53f), Projectile.scale * 3f, slashEffects, 0);
                }

            }

            Main.EntitySpriteDraw(texture, Projectile.Center - Main.screenPosition, frame, lightColor, Projectile.rotation - MathHelper.PiOver4 * Projectile.direction, origin, Projectile.scale, spriteEffects, 0);

            return false;
        }
    }
}
