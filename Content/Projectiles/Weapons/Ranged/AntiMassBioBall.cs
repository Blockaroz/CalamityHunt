using System;
using System.Collections.Generic;
using CalamityHunt.Common.Utilities;
using CalamityHunt.Content.Particles;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.Audio;
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
        Projectile.velocity.Y += 0.005f;
        Projectile.velocity *= 0.995f;

        Time++;

        Lighting.AddLight(Projectile.Center, Color.SteelBlue.ToVector3() * 0.6f);
        Dust.QuickDust(Projectile.Center, Color.CornflowerBlue);
        HandleSound();
    }

    public LoopingSound auraSound;

    public void HandleSound()
    {
        SoundStyle sound = SoundID.DD2_KoboldIgniteLoop;
        sound.MaxInstances = 0;

        if (auraSound is null)
            auraSound = new LoopingSound(sound, new ProjectileAudioTracker(Projectile).IsActiveAndInGame);

        auraSound.PlaySound(() => Projectile.Center, () => Math.Min(1f, Projectile.timeLeft / 20f), () => 1f - Time / 200f);
    }

    public override void OnKill(int timeLeft)
    {
    }

    public override bool OnTileCollide(Vector2 oldVelocity)
    {
        Projectile.velocity = Vector2.Zero;
        Projectile.timeLeft = Math.Min(Projectile.timeLeft, 20);

        return false;
    }

    public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
    {
        targetPoints = new List<Vector2>();

        Vector2 targetCenter = targetHitbox.Center.ToVector2();
        Vector2 aimedCenter = Projectile.DirectionTo(targetCenter) * Math.Min(Projectile.Distance(targetCenter), MAX_MARKER_RANGE);
        if (targetHitbox.Contains(aimedCenter.ToPoint())) {
            targetPoints.Add(targetCenter);
            return Time > 5;
        }

        return false;
    }

    private List<Vector2> targetPoints;
    private List<List<Vector2>> targetBeams;

    public override bool PreDraw(ref Color lightColor)
    {
        if (targetPoints != null) {
            foreach (Vector2 target in targetPoints) {

            }
        }
        return false;
    }
}
