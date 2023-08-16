using CalamityHunt.Common.Systems.Particles;
using CalamityHunt.Content.Particles;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
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

namespace CalamityHunt.Content.Projectiles.Weapons.Rogue
{
    public class ThrowingStarsProjectile : ModProjectile
    {
        public override void SetDefaults()
        {
            Projectile.width = 20;
            Projectile.height = 20;
            Projectile.friendly = true;
            Projectile.penetrate = -1;
            Projectile.tileCollide = true;
            Projectile.extraUpdates = 8;
            Projectile.DamageType = DamageClass.Throwing;
            if (ModLoader.HasMod("CalamityMod"))
            {
                DamageClass d;
                Mod calamity = ModLoader.GetMod("CalamityMod");
                calamity.TryFind<DamageClass>("RogueDamageClass", out d);
                Projectile.DamageType = d;
            }
            Projectile.timeLeft = 60;
        }

        public override void AI()
        {
            if (Projectile.ai[0] == 0)
            {
                Dust d = Dust.NewDustPerfect(Projectile.Center - Projectile.velocity * Main.rand.NextFloat(), DustID.SparkForLightDisc, Projectile.velocity.RotatedByRandom(0.1f), 0, new Color(50, 180, 255, 0), 0.7f);
                d.noGravity = true;                
                
                Projectile.rotation = Projectile.velocity.ToRotation();
                if (Projectile.ai[1] > -1)
                {

                }

                Projectile.localAI[0] += Projectile.velocity.Length() * 0.066f;

                if (Projectile.timeLeft < 2)
                {
                    Projectile.ai[0] = -1;
                    Projectile.timeLeft = 100;
                }
            }
            else
            {
                Projectile.velocity *= 0.8f;
                Projectile.localAI[0] -= 0.8f;

                if (Projectile.timeLeft == 35)
                {
                    Particle.NewParticle(Particle.ParticleType<CrossSparkle>(), Projectile.Center, Vector2.Zero, new Color(50, 180, 255, 0), 1f);
                }

                //transfer to kill
                if (Projectile.timeLeft == 1)
                {
                    SoundEngine.PlaySound(SoundID.DD2_LightningBugZap.WithPitchOffset(1f).WithVolumeScale(0.5f), Projectile.Center);
                    //for (int i = 0; i < 5; i++)
                    //    Dust.NewDust(Projectile.Center - new Vector2(5), 10, 10, DustID.Gold, Scale: 0.7f);
                }
            }
        }

        public override bool OnTileCollide(Vector2 oldVelocity)
        {
            if (Projectile.ai[0] == 0)
            {
                Projectile.velocity *= 0.1f;
                Projectile.Center += oldVelocity * 1.5f;
                Projectile.ai[0] = -1;
                Projectile.timeLeft = 100;

                SoundEngine.PlaySound(SoundID.DD2_WitherBeastCrystalImpact.WithPitchOffset(1f).WithVolumeScale(0.5f), Projectile.Center);
            }

            return false;
        }

        public override bool? CanDamage() => Projectile.ai[0] == 0;

        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D texture = TextureAssets.Projectile[Type].Value;


            float strength = Utils.GetLerpValue(0, 30, Projectile.localAI[0], true);

            Main.EntitySpriteDraw(texture, Projectile.Center - Main.screenPosition, texture.Frame(), lightColor, Projectile.rotation, texture.Size() * 0.5f, Projectile.scale, 0, 0);

            Vector2 direction = Projectile.rotation.ToRotationVector2() * 3;
            DrawCappedFlare(Projectile.Center - Main.screenPosition, new Color(10, 40, 200, 0), Projectile.rotation, new Vector2(2f, 18 * strength) * Projectile.scale);
            DrawCappedFlare(Projectile.Center - direction - Main.screenPosition, new Color(50, 170, 255, 0), Projectile.rotation, new Vector2(1f, 15 * strength) * Projectile.scale);
            DrawCappedFlare(Projectile.Center - direction - Main.screenPosition, new Color(255, 255, 255, 0), Projectile.rotation, new Vector2(1f, 3 * strength) * Projectile.scale);
            DrawCappedFlare(Projectile.Center - direction * 2 - Main.screenPosition, Color.MidnightBlue, Projectile.rotation, new Vector2(0.1f, 5 * strength) * Projectile.scale);

            return false;
        }

        private void DrawCappedFlare(Vector2 position, Color color, float rotation, Vector2 scale)
        {
            Texture2D flare = AssetDirectory.Textures.Sparkle;
            Main.EntitySpriteDraw(flare, position, flare.Frame(1, 2, 0, 0), color, rotation - MathHelper.PiOver2, flare.Size() * 0.5f, scale, 0, 0);
            Main.EntitySpriteDraw(flare, position, flare.Frame(1, 2, 0, 1), color, rotation - MathHelper.PiOver2, flare.Size() * new Vector2(0.5f, 0f), new Vector2(scale.X, Math.Min(scale.Y, 0.8f)), 0, 0);
        }
    }
}
