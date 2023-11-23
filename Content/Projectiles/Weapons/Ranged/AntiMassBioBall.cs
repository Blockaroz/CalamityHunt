using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Arch.Core.Extensions;
using CalamityHunt.Common.Systems.Particles;
using CalamityHunt.Common.Utilities;
using CalamityHunt.Content.Particles;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent;
using Terraria.Graphics;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;

namespace CalamityHunt.Content.Projectiles.Weapons.Ranged;

public class AntiMassBioBall : ModProjectile
{
    public override void SetDefaults()
    {
        Projectile.width = 24;
        Projectile.height = 24;
        Projectile.friendly = true;
        Projectile.timeLeft = 500;
        Projectile.penetrate = -1;
        Projectile.tileCollide = true;
        Projectile.manualDirectionChange = true;
    }

    public const float MAX_MARKER_RANGE = 500;

    public ref float Time => ref Projectile.ai[0];

    public override void AI()
    {
        Projectile.velocity.Y += 0.001f;
        Projectile.velocity *= 0.995f;

        Projectile.scale = Utils.GetLerpValue(-15, 10, Time, true);

        Time++;

        if (Main.myPlayer == Projectile.owner) {
            if (Main.rand.NextBool(3) || Time % 5 == 0) {



                Projectile.netUpdate = true;
            }
        }

        for (int i = 0; i < 8; i++) {
            Color particleColor = Color.Lerp(AntiMassColliderProj.MainColor, Color.Turquoise, (!Main.rand.NextBool((int)(Time / 30f + 2))).ToInt()) with { A = 0 };
            var lightningParticle = ParticleBehavior.NewParticle(ModContent.GetInstance<LightningParticleParticleBehavior>(), Projectile.Center + Projectile.velocity + Main.rand.NextVector2Circular(20, 20).RotatedBy(Projectile.rotation) * Projectile.scale, Main.rand.NextVector2Circular(6, 6), particleColor, Main.rand.NextFloat(0.2f, 0.7f) * Projectile.scale);
            lightningParticle.Add(new ParticleRotation() { Value = Main.rand.NextFloat(-4f, 4f) });
            lightningParticle.Add(new ParticleData<Func<Vector2>> { Value = () => Projectile.velocity });
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

        //for (int i = 0; i < 50; i++) {
        //    Color particleColor = Color.Lerp(Color.Gold, Color.MediumAquamarine, (!Main.rand.NextBool((int)(Time / 20f + 2))).ToInt()) with { A = 40 };
        //    Vector2 particleVelocity = Main.rand.NextVector2Circular(16, 16);
        //    var lightningParticle = ParticleBehavior.NewParticle(ModContent.GetInstance<LightningParticleParticleBehavior>(), Projectile.Center + Main.rand.NextVector2Circular(20, 20).RotatedBy(Projectile.rotation) * Projectile.scale, particleVelocity, particleColor with { A = 40 }, Main.rand.NextFloat(0.5f, 1f) * Projectile.scale);
        //    lightningParticle.Add(new ParticleRotation() { Value = particleVelocity.ToRotation() + Main.rand.NextFloat(-1f, 1f) });
        //}
    }

    public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
    {
        return false;
    }

    private List<int> targets;
    private List<Rope> ropeList;

    public override bool PreDraw(ref Color lightColor)
    {
        Texture2D texture = TextureAssets.Projectile[Type].Value;
        Texture2D glow = AssetDirectory.Textures.Glow.Value;

        float scaleWobble = Projectile.scale + MathF.Sin(Projectile.timeLeft / 1.2f) * 0.2f;
        Main.EntitySpriteDraw(texture, Projectile.Center - Main.screenPosition, texture.Frame(), Color.PaleTurquoise with { A = 200 }, 0, texture.Size() * 0.5f, (Projectile.scale + scaleWobble) / 2f, 0, 0);
        Main.EntitySpriteDraw(glow, Projectile.Center - Main.screenPosition, glow.Frame(), (Color.Cyan * 0.3f) with { A = 0 }, 0, glow.Size() * 0.5f, scaleWobble, 0, 0);
        Main.EntitySpriteDraw(glow, Projectile.Center - Main.screenPosition, glow.Frame(), (Color.MediumTurquoise * 0.2f) with { A = 0 }, 0, glow.Size() * 0.5f, Projectile.scale * 1.5f, 0, 0);

        Effect trailEffect = AssetDirectory.Effects.BasicTrail.Value;
        trailEffect.Parameters["transformMatrix"].SetValue(Main.GameViewMatrix.NormalizedTransformationmatrix);
        trailEffect.Parameters["uColor"].SetValue((Color.MediumTurquoise with { A = 40 }).ToVector3());
        //trailEffect.Parameters["uThickness"].SetValue(1f);
        //trailEffect.Parameters["uVaryThickness"].SetValue(1f);
        trailEffect.Parameters["uTime"].SetValue(Main.GlobalTimeWrappedHourly * 0.1f);
        trailEffect.Parameters["uTexture0"].SetValue(TextureAssets.Extra[194].Value);
        trailEffect.Parameters["uTexture1"].SetValue(TextureAssets.Extra[196].Value);

        Main.pixelShader.CurrentTechnique.Passes[0].Apply();

        return false;
    }
}
