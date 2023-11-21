using System;
using Arch.Core.Extensions;
using CalamityHunt.Common.Systems.Particles;
using CalamityHunt.Content.Particles;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent;
using Terraria.Graphics.Renderers;
using Terraria.ID;
using Terraria.Map;
using Terraria.ModLoader;

namespace CalamityHunt.Content.Projectiles.Weapons.Ranged;

public class AntiMassColliderProj : ModProjectile
{
    public override void SetStaticDefaults()
    {
        ProjectileID.Sets.HeldProjDoesNotUsePlayerGfxOffY[Type] = true;
    }

    public override void SetDefaults()
    {
        Projectile.width = 50;
        Projectile.height = 50;
        Projectile.friendly = true;
        Projectile.timeLeft = 10000;
        Projectile.penetrate = -1;
        Projectile.tileCollide = false;
        Projectile.ownerHitCheck = true;
        Projectile.hide = true;
        Projectile.manualDirectionChange = true;
        Projectile.DamageType = DamageClass.Ranged;
    }

    public ref float Time => ref Projectile.ai[0];

    public ref Player Player => ref Main.player[Projectile.owner];

    public override void AI()
    {
        Player.heldProj = Projectile.whoAmI;

        if (!Player.active || Player.dead || Player.noItems || Player.CCed) {
            Projectile.Kill();
            return;
        }

        Vector2 muzzlePosition = Projectile.Center + new Vector2(20, -4 * Projectile.direction).RotatedBy(Projectile.velocity.ToRotation());
        int firstShot = (int)(20 * Player.GetAttackSpeed(DamageClass.Ranged));
        int secondShot = (int)(70 * Player.GetAttackSpeed(DamageClass.Ranged));

        if (Main.netMode != NetmodeID.MultiplayerClient) {
            if (Time == 0) {
                Projectile.velocity = Player.DirectionTo(Main.MouseWorld) * Player.HeldItem.shootSpeed;
                Projectile.netUpdate = true;
            }

            if (Time == firstShot) {
                Projectile.NewProjectileDirect(Projectile.GetSource_FromThis(), muzzlePosition, Projectile.velocity.SafeNormalize(Vector2.Zero) * 12f, ModContent.ProjectileType<AntiMassBioBall>(), 1 + Projectile.damage / 4, Projectile.knockBack, Player.whoAmI);

                Player.velocity -= Projectile.velocity.SafeNormalize(Vector2.Zero) * 2f;
                Projectile.netUpdate = true;
            }

            if (Time > (int)(35 * Player.GetAttackSpeed(DamageClass.Ranged)) && Time < (int)(60 * Player.GetAttackSpeed(DamageClass.Ranged))) {
                float timeToMove = Utils.GetLerpValue(55, 35, Time / Player.GetAttackSpeed(DamageClass.Ranged), true);
                Projectile.velocity = Vector2.Lerp(Projectile.velocity, Player.DirectionTo(Main.MouseWorld) * Player.HeldItem.shootSpeed, 0.2f * timeToMove);
                Projectile.netUpdate = true;

                if (Time > (int)(45 * Player.GetAttackSpeed(DamageClass.Ranged)) && !Player.channel) {
                    Player.SetDummyItemTime(25);
                    Projectile.Kill();
                }
            }

            if (Time == secondShot) {
                Player.velocity -= Projectile.velocity.SafeNormalize(Vector2.Zero) * 3f;
                Projectile.netUpdate = true;
            }

            if (Time > secondShot + 10) {
                Projectile.velocity = Vector2.Lerp(Projectile.velocity, Player.DirectionTo(Main.MouseWorld) * Player.HeldItem.shootSpeed, 0.1f);
                Projectile.netUpdate = true;
            }
        }

        if (Time == firstShot) {
            SoundEngine.PlaySound(SoundID.Item61, Projectile.Center);
            recoilFactor = 1f;
        }

        if (Time == secondShot) {
            SoundEngine.PlaySound(SoundID.Item61, Projectile.Center);
            recoilFactor = 1f;
        }

        Projectile.rotation = Projectile.velocity.ToRotation() - recoilFactor * 0.01f * Projectile.direction;

        Player.ChangeDir(Projectile.velocity.X > 0 ? 1 : -1);
        Projectile.direction = Projectile.velocity.X > 0 ? 1 : -1;
        Player.itemRotation = (Projectile.velocity * Projectile.direction).ToRotation();
        Projectile.Center = Player.MountedCenter - new Vector2(0, 6 * Player.gravDir) + Projectile.velocity.SafeNormalize(Vector2.Zero) * 10;
        recoilFactor = MathHelper.Lerp(recoilFactor, 0f, 0.2f);

        Time++;

        for (int i = 0; i < 4; i++) {
            if (recoilFactor > 0.03f && Main.rand.NextBool((int)(15 - recoilFactor * 14))) {

                float particleRotation = Projectile.rotation + Main.rand.NextFloat(-0.4f, 0.4f);
                var hue = ParticleBehavior.NewParticle(ModContent.GetInstance<LightningParticleParticleBehavior>(), muzzlePosition + Main.rand.NextVector2Circular(30, 10).RotatedBy(Projectile.rotation), particleRotation.ToRotationVector2(), Color.Gold with { A = 40 }, Main.rand.NextFloat(0.2f, 0.5f));
                hue.Add(new ParticleRotation() { Value = particleRotation });
                hue.Add(new ParticleData<Func<Vector2>> { Value = () => Player.velocity });
            }
        }

        Lighting.AddLight(Projectile.Center, Color.DarkGoldenrod.ToVector3() * 0.5f);

        if (Time < Player.HeldItem.useTime) {
            Player.SetDummyItemTime(3);
        }

        if (Time > Player.HeldItem.useTime) {
            Projectile.Kill();
        }
    }

    public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox) => false;

    public override bool? CanCutTiles() => false;

    public float recoilFactor;

    public static Asset<Texture2D> glowmask;

    public override void Load()
    {
        glowmask = ModContent.Request<Texture2D>($"{Texture}_Glow", AssetRequestMode.ImmediateLoad);
    }

    public override bool PreDraw(ref Color lightColor)
    {
        Texture2D texture = TextureAssets.Projectile[Type].Value;
        Texture2D glow = glowmask.Value;

        Rectangle baseFrame = texture.Frame(1, 2, 0, 0);
        Rectangle barrelFrame = texture.Frame(1, 2, 0, 1);

        Vector2 origin = baseFrame.Size() * new Vector2(0.1f, 0.5f + 0.4f * Projectile.direction);
        SpriteEffects spriteEffects = Projectile.direction < 0 ? SpriteEffects.FlipVertically : SpriteEffects.None;

        Vector2 drawScale = Projectile.scale * Vector2.Lerp(Vector2.One, new Vector2(0.97f, 1.05f), recoilFactor);
        Vector2 holdOffset = new Vector2(35 + recoilFactor * 5f, -16 * Projectile.direction).RotatedBy(Projectile.rotation) * drawScale;
        Vector2 barrelOffset = new Vector2(MathF.Cbrt(Utils.GetLerpValue(0.01f, 0.4f, recoilFactor, true)) * 16, 0).RotatedBy(Projectile.rotation) * drawScale;

        Main.EntitySpriteDraw(texture, Projectile.Center - holdOffset - barrelOffset - Main.screenPosition, barrelFrame, lightColor, Projectile.rotation, origin, drawScale, spriteEffects, 0);
        Main.EntitySpriteDraw(glow, Projectile.Center - holdOffset - barrelOffset - Main.screenPosition, barrelFrame, Color.Gold with { A = 40 } * Utils.GetLerpValue(0f, 0.1f, recoilFactor, true), Projectile.rotation, origin, drawScale, spriteEffects, 0);
        Main.EntitySpriteDraw(texture, Projectile.Center - holdOffset - Main.screenPosition, baseFrame, lightColor, Projectile.rotation, origin, drawScale, spriteEffects, 0);
        Main.EntitySpriteDraw(glow, Projectile.Center - holdOffset - Main.screenPosition, baseFrame, Color.Gold with { A = 40 }, Projectile.rotation, origin, drawScale, spriteEffects, 0);

        return false;
    }
}
