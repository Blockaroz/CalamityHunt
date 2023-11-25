using System;
using CalamityHunt.Common.Graphics.RenderTargets;
using CalamityHunt.Common.Systems.Particles;
using CalamityHunt.Common.Utilities;
using CalamityHunt.Content.Particles;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityHunt.Content.Projectiles.Weapons.Ranged;

public class AntiMassBioBall : ModProjectile
{
    public override void SetDefaults()
    {
        Projectile.width = 24;
        Projectile.height = 24;
        Projectile.friendly = true;
        Projectile.timeLeft = 200;
        Projectile.penetrate = -1;
        Projectile.tileCollide = true;
        Projectile.manualDirectionChange = true;
    }

    public const float MAX_MARKER_RANGE = 500;

    public ref float Time => ref Projectile.ai[0];

    public override void AI()
    {
        Projectile.velocity.Y += 0.001f;

        Projectile.scale = Utils.GetLerpValue(-15, 10, Time, true);

        if (Main.myPlayer == Projectile.owner) {
            if (Main.rand.NextBool(3) || Time % 5 == 0) {
                Projectile.netUpdate = true;
            }
        }

        if (Main.rand.NextBool(9)) {
            Color color = Color.Lerp(AntiMassColliderProj.MainColor, Color.Turquoise, (!Main.rand.NextBool(10)).ToInt()) with { A = 20 };
            Dust sparks = Dust.NewDustPerfect(Projectile.Center + Main.rand.NextVector2Circular(16, 16), 278, Projectile.velocity.RotatedByRandom(0.2f) * 0.5f, 0, color * 0.7f, Main.rand.NextFloat(0.6f));
            sparks.noGravity = true;
        }

        Time++;

        for (int i = 0; i < 3; i++) {
            CalamityHunt.particles.Add(Particle.Create<LightningParticle>(particle => {
                particle.position = Projectile.Center + Main.rand.NextVector2Circular(20, 20).RotatedBy(Projectile.rotation) * Projectile.scale;
                particle.velocity = Main.rand.NextVector2Circular(6, 6);
                particle.scale = Main.rand.NextFloat(0.2f, 0.9f) * Projectile.scale;
                particle.color = Color.Lerp(AntiMassColliderProj.MainColor, Color.Turquoise, (!Main.rand.NextBool((int)(Time / 30f + 2))).ToInt()) with { A = 0 };
                particle.rotation = Main.rand.NextFloat(MathHelper.TwoPi);
                particle.anchor = () => Projectile.velocity;
            }));       
        }

        Lighting.AddLight(Projectile.Center, Color.MediumTurquoise.ToVector3() * 0.6f);

        HandleSound();
    }

    public LoopingSound auraSound;

    public void HandleSound()
    {
        SoundStyle sound = AssetDirectory.Sounds.Weapons.ElectricLoop;

        auraSound ??= new LoopingSound(sound, new ProjectileAudioTracker(Projectile).IsActiveAndInGame);

        auraSound.PlaySound(() => Projectile.Center, () => Math.Max(0f, 0.5f - Projectile.Distance(Main.LocalPlayer.Center) * 0.00025f), () => -0.5f);
    }

    public override void OnKill(int timeLeft)
    {
        auraSound.StopSound();

        SoundEngine.PlaySound(SoundID.DD2_KoboldIgnite.WithPitchOffset(-0.33f), Projectile.Center);

        for (int i = 0; i < 40; i++) {
            CalamityHunt.particles.Add(Particle.Create<LightningParticle>(particle => {
                particle.position = Projectile.Center + Main.rand.NextVector2Circular(10, 10).RotatedBy(Projectile.rotation) * Projectile.scale;
                particle.velocity = Main.rand.NextVector2Circular(16, 16);
                particle.scale = Main.rand.NextFloat(0.5f, 1.5f) * Projectile.scale;
                particle.color = Color.Lerp(AntiMassColliderProj.MainColor, Color.MediumAquamarine, (!Main.rand.NextBool((int)(Time / 30f + 8))).ToInt()) with { A = 0 };
                particle.rotation = Main.rand.NextFloat(MathHelper.TwoPi);
                particle.anchor = () => Projectile.velocity * 0.2f;
            }));

            if (Main.rand.NextBool()) {
                Color color = Color.Lerp(AntiMassColliderProj.MainColor, Color.Turquoise, (!Main.rand.NextBool(10)).ToInt()) with { A = 20 };
                Dust sparks = Dust.NewDustPerfect(Projectile.Center + Main.rand.NextVector2Circular(16, 16), 278, Main.rand.NextVector2Circular(5, 5), 0, color * 0.7f, Main.rand.NextFloat());
                sparks.noGravity = true;
            }
        }
    }

    public override bool PreDraw(ref Color lightColor)
    {
        Texture2D texture = TextureAssets.Projectile[Type].Value;
        Texture2D glow = AssetDirectory.Textures.Glow.Value;

        float scaleWobble = Projectile.scale + MathF.Sin(Projectile.timeLeft / 1.2f) * 0.2f;

        Main.EntitySpriteDraw(glow, Projectile.Center - Main.screenPosition, glow.Frame(), Color.Black * 0.4f, 0, glow.Size() * 0.5f, Projectile.scale * 1.5f, 0, 0);
        Main.EntitySpriteDraw(texture, Projectile.Center - Main.screenPosition, texture.Frame(), Color.PaleTurquoise with { A = 200 }, 0, texture.Size() * 0.5f, (Projectile.scale + scaleWobble) / 2f, 0, 0);
        Main.EntitySpriteDraw(glow, Projectile.Center - Main.screenPosition, glow.Frame(), (Color.Cyan * 0.3f) with { A = 0 }, 0, glow.Size() * 0.5f, scaleWobble, 0, 0);
        Main.EntitySpriteDraw(glow, Projectile.Center - Main.screenPosition, glow.Frame(), (Color.MediumTurquoise * 0.2f) with { A = 0 }, 0, glow.Size() * 0.5f, Projectile.scale * 1.5f, 0, 0);

        //Effect trailEffect = AssetDirectory.Effects.BasicTrail.Value;
        //trailEffect.Parameters["transformMatrix"].SetValue(Main.GameViewMatrix.NormalizedTransformationmatrix);
        //trailEffect.Parameters["uColor"].SetValue((Color.MediumTurquoise with { A = 40 }).ToVector3());
        //trailEffect.Parameters["uThickness"].SetValue(1f);
        //trailEffect.Parameters["uVaryThickness"].SetValue(1f);
        //trailEffect.Parameters["uTime"].SetValue(Main.GlobalTimeWrappedHourly * 0.1f);
        //trailEffect.Parameters["uTexture0"].SetValue(TextureAssets.Extra[194].Value);
        //trailEffect.Parameters["uTexture1"].SetValue(TextureAssets.Extra[196].Value);

        //Main.pixelShader.CurrentTechnique.Passes[0].Apply();

        return false;
    }
}
