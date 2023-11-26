using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using CalamityHunt.Common.Graphics.RenderTargets;
using CalamityHunt.Common.Systems.GlobalNPCs;
using CalamityHunt.Common.Systems.Particles;
using CalamityHunt.Common.Utilities;
using CalamityHunt.Content.Buffs;
using CalamityHunt.Content.Particles;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityHunt.Content.Projectiles.Weapons.Ranged;

public class AntiMassBioStrike : ModProjectile
{
    public override void SetDefaults()
    {
        Projectile.width = 32;
        Projectile.height = 32;
        Projectile.friendly = true;
        Projectile.timeLeft = 60;
        Projectile.penetrate = -1;
        Projectile.tileCollide = true;
        Projectile.manualDirectionChange = true;
        Projectile.idStaticNPCHitCooldown = 30;
        Projectile.usesIDStaticNPCImmunity = true;
        Projectile.ignoreWater = true;
        Projectile.extraUpdates = 1;
    }

    public ref float Time => ref Projectile.ai[0];
    public ref float Target => ref Projectile.ai[1];
    public ref float Owner => ref Projectile.ai[2];

    public override void AI()
    {
        Projectile.velocity.Y += 0.001f;
  
        Projectile.scale = Utils.GetLerpValue(-15, 10, Time, true);

        if (Main.myPlayer == Projectile.owner) {
            if (Main.projectile.IndexInRange((int)Owner)) {
                Projectile ownerProjectile = Main.projectile[(int)Owner];
                if (Main.npc.IndexInRange((int)Target)) {
                    NPC nPC = Main.npc[(int)Target];
                    if (nPC.active && nPC.CanBeChasedBy(ownerProjectile) && nPC.Distance(ownerProjectile.Center) < AntiMassBioBall.MAX_RANGE * 1.5f) {
                        Projectile.Center = nPC.Center;
                        Projectile.netUpdate = true;
                    }
                }

                rope ??= new Rope(ownerProjectile.Center, Projectile.Center, 24, 3f, Main.rand.NextVector2CircularEdge(1, 1) * Main.rand.NextFloat(0.4f, 0.7f));

                rope.StartPos = ownerProjectile.Center;
                rope.EndPos = Projectile.Center;
                rope.segmentLength = ownerProjectile.Distance(Projectile.Center) * 0.05f;
                rope.gravity *= 0.9f;
                rope.Update();
            }
        }

        Time++;

        Lighting.AddLight(Projectile.Center, Color.MediumTurquoise.ToVector3() * 0.6f);
    }

    public override void OnKill(int timeLeft)
    {
        SoundStyle deathSound = SoundID.DD2_KoboldIgnite;
        deathSound.MaxInstances = 0;
        deathSound.PitchVariance = 0.1f;
        SoundEngine.PlaySound(SoundID.DD2_KoboldIgnite.WithPitchOffset(-0.1f).WithVolumeScale(0.1f), Projectile.Center);

        for (int i = 0; i < 10; i++) {
            CalamityHunt.particles.Add(Particle.Create<LightningParticle>(particle => {
                particle.position = Projectile.Center + Main.rand.NextVector2Circular(10, 10).RotatedBy(Projectile.rotation) * Projectile.scale;
                particle.velocity = Main.rand.NextVector2Circular(16, 16);
                particle.scale = Main.rand.NextFloat(0.5f, 1.5f) * Projectile.scale;
                particle.color = Color.Lerp(AntiMassColliderProj.MainColor, Color.MediumAquamarine, (!Main.rand.NextBool((int)(Time / 30f + 8))).ToInt()) with { A = 0 };
                particle.rotation = Main.rand.NextFloat(MathHelper.TwoPi);
                particle.anchor = () => Projectile.velocity * 0.2f;
            }));

            if (!Main.rand.NextBool(4)) {
                Color color = Color.Lerp(AntiMassColliderProj.MainColor, Color.Turquoise, (!Main.rand.NextBool(10)).ToInt()) with { A = 40 };
                Dust sparks = Dust.NewDustPerfect(Projectile.Center + Main.rand.NextVector2Circular(16, 16), 278, Main.rand.NextVector2Circular(5, 5), 0, color * 0.7f, Main.rand.NextFloat());
                sparks.noGravity = true;
            }
        }
    }

    public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
    {
        target.GetGlobalNPC<DoomedNPC>().doomCount = 240;
    }

    public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
    {
        return base.Colliding(projHitbox, targetHitbox);
    }

    public Rope rope;

    public override bool PreDraw(ref Color lightColor)
    {
        if (rope != null) {
            Vector2[] points = rope.GetPoints().ToArray();
            float[] rotations = new float[points.Length];
            for (int i = 0; i < points.Length - 1; i++) {
                rotations[i] = points[i].AngleFrom(points[i + 1]);
            }
            rotations[^1] = rotations[^2];

            for (int i = 0; i < points.Length - 2; i++) {
                Vector2 stretch = new Vector2(points[i].Distance(points[i + 1]) + 0.1f, 1f);
                Main.EntitySpriteDraw(TextureAssets.BlackTile.Value, points[i] - Main.screenPosition, new Rectangle(0, 0, 1, 2), Color.Turquoise with { A = 0 }, rotations[i], Vector2.UnitX, stretch, 0, 0);
            }

        }

        return false;
    }
}
