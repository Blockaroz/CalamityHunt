using System;
using CalamityHunt.Common.Systems.Particles;
using CalamityHunt.Content.Particles;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityHunt.Content.Projectiles.Weapons.Ranged;

public class AntiMassDeathLaser : ModProjectile
{
    public override void SetStaticDefaults()
    {
        ProjectileID.Sets.DrawScreenCheckFluff[Type] = 8000;
    }

    public override void SetDefaults()
    {
        Projectile.width = 18;
        Projectile.height = 18;
        Projectile.friendly = true;
        Projectile.timeLeft = 280;
        Projectile.penetrate = -1;
        Projectile.tileCollide = true;
        Projectile.extraUpdates = 30;
    }

    public ref float Time => ref Projectile.ai[0];

    public override void AI()
    {
        if (Projectile.velocity.Length() > 1) {
            Projectile.rotation = Projectile.velocity.ToRotation();

            Color color = Color.Lerp(AntiMassColliderProj.MainColor, Color.Orange, Main.rand.NextBool((int)(Projectile.timeLeft / 1200f * 10) + 1).ToInt()) with { A = 20 };
            Dust sparks = Dust.NewDustPerfect(Projectile.Center, 278, Projectile.velocity.RotatedByRandom(0.2f) * Main.rand.NextFloat(), 0, color * 0.8f, Main.rand.NextFloat(0.9f));
            sparks.noGravity = true;

            if (Time % 9 == 4) {
                CalamityHunt.particles.Add(Particle.Create<StraightLightningParticle>(particle => {
                    particle.position = Projectile.Center - Projectile.velocity * 0.5f;
                    particle.velocity = Projectile.velocity * 0.15f;
                    particle.scale = Main.rand.NextFloat(1f, 2f) * Projectile.scale;
                    particle.color = Color.Lerp(AntiMassColliderProj.MainColor, Color.Orange, Main.rand.NextBool((int)(Projectile.timeLeft / 1200f * 10) + 1).ToInt()) with { A = 50 };
                    particle.rotation = Projectile.rotation;
                    particle.maxTime = (int)(Utils.GetLerpValue(120, 0, Time, true) * 7) + Main.rand.Next(4, 10);
                    particle.stretch = Projectile.velocity.Length() * 0.25f / particle.scale * Projectile.scale;
                    particle.flickerSpeed = Main.rand.NextFloat(0.5f, 1.5f);
                }));
            }            
            
            if (Time % 6 == 3) {
                CalamityHunt.particles.Add(Particle.Create<StraightLightningParticle>(particle => {
                    particle.position = Projectile.Center - Projectile.velocity * 0.5f;
                    particle.velocity = Projectile.velocity * 0.15f;
                    particle.scale = Main.rand.NextFloat(0.5f, 0.7f) * Projectile.scale;
                    particle.color = Color.Lerp(AntiMassColliderProj.MainColor, Color.Turquoise, Main.rand.NextBool(3).ToInt()) with { A = 50 };
                    particle.rotation = Projectile.rotation;
                    particle.maxTime = (int)(Utils.GetLerpValue(120, 0, Time, true) * 6) + Main.rand.Next(9, 15);
                    particle.stretch = Projectile.velocity.Length() * 0.1f * Projectile.scale;
                    particle.flickerSpeed = Main.rand.NextFloat(1f, 2f);
                }));
            }
        }

        if (Main.rand.NextBool(8)) {
            CalamityHunt.particles.Add(Particle.Create<LightningParticle>(particle => {
                particle.position = Projectile.Center;
                particle.velocity = Projectile.velocity.RotatedByRandom(1f) * Main.rand.NextFloat(0.2f);
                particle.scale = Main.rand.NextFloat(0.1f, 0.5f) * Projectile.scale;
                particle.color = AntiMassColliderProj.MainColor with { A = 40 };
                particle.rotation = Projectile.rotation;
            }));
        }

        Time++;

        if (Projectile.timeLeft > 180) {
            visualSpeed = Time * (Projectile.velocity.Length() + 1f);
        }
        else {
            Projectile.velocity *= 0.9f;
        }

        if (Main.rand.NextBool(6)) {
            CalamityHunt.particles.Add(Particle.Create<LightningParticle>(particle => {
                particle.position = Projectile.Center;
                particle.velocity = Main.rand.NextVector2Circular(10, 10);
                particle.scale = Main.rand.NextFloat(1f, 2f);
                particle.color = Color.Lerp(AntiMassColliderProj.MainColor, Color.MediumTurquoise, Main.rand.NextBool(3).ToInt()) with { A = 40 };
                particle.rotation = Projectile.rotation;
            }));
        }
    }

    public override bool OnTileCollide(Vector2 oldVelocity)
    {
        Projectile.tileCollide = false;
        Projectile.velocity = Vector2.Zero;

        Projectile.timeLeft = (int)MathHelper.Min(Projectile.timeLeft, 180);

        return false;
    }

    public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
    {
        int repetitions = (int)Math.Min(Time, 20);
        for (int i = 0; i < repetitions; i++) {
            Rectangle rect = projHitbox;
            rect.Location -= (Projectile.velocity * i).ToPoint();
            return base.Colliding(rect, targetHitbox);
        }
        return false;
    }

    private float visualSpeed;

    public override bool PreDraw(ref Color lightColor)
    {
        Texture2D texture = TextureAssets.Projectile[Type].Value;
        Rectangle half = texture.Frame(1, 2, 0, 0);

        float thickness = MathF.Cbrt(Utils.GetLerpValue(0, 120, Projectile.timeLeft, true));
        Vector2 trailDrawScale = new Vector2(thickness, visualSpeed / texture.Height * 2f);
        Main.EntitySpriteDraw(texture, Projectile.Center - Main.screenPosition, half, AntiMassColliderProj.MainColor, Projectile.rotation - MathHelper.PiOver2, half.Size() * new Vector2(0.5f, 1f), trailDrawScale, 0, 0);
        Main.EntitySpriteDraw(texture, Projectile.Center - Main.screenPosition, half, AntiMassColliderProj.MainColor, Projectile.rotation + MathHelper.PiOver2, half.Size() * new Vector2(0.5f, 1f), thickness, 0, 0);

        Vector2 glowScale = new Vector2(thickness * 0.5f, 1f);
        Main.EntitySpriteDraw(texture, Projectile.Center - Main.screenPosition, half, Color.White with { A = 0 }, Projectile.rotation - MathHelper.PiOver2, half.Size() * new Vector2(0.5f, 1f), glowScale * trailDrawScale, 0, 0);
        Main.EntitySpriteDraw(texture, Projectile.Center - Main.screenPosition, half, Color.White with { A = 0 }, Projectile.rotation + MathHelper.PiOver2, half.Size() * new Vector2(0.5f, 1f), glowScale, 0, 0);

        return false;
    }
}
