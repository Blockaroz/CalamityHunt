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

public class AntiMassMacroLaser : ModProjectile
{
    public override void SetDefaults()
    {
        Projectile.width = 24;
        Projectile.height = 24;
        Projectile.friendly = true;
        Projectile.timeLeft = 1200;
        Projectile.penetrate = -1;
        Projectile.tileCollide = true;
        Projectile.extraUpdates = 10;
    }

    public ref float Time => ref Projectile.ai[0];

    public override void AI()
    {
        Dust.QuickDustLine(Projectile.Center, Projectile.Center - Projectile.velocity, 10, AntiMassColliderProj.MainColor);
    }

    public override void OnKill(int timeLeft)
    {

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

    public override bool PreDraw(ref Color lightColor)
    {
        Texture2D texture = TextureAssets.Projectile[Type].Value;

        return false;
    }
}
