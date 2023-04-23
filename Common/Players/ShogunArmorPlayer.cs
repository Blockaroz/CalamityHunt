using CalamityHunt.Common.Systems.Particles;
using CalamityHunt.Content.Particles;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityHunt.Common.Players
{
    public class ShogunArmorPlayer : ModPlayer
    {
        public bool active;
        public float slamPower;
        private bool slamming;
        private int bunnyHopCounter;

        public override void PostUpdateRunSpeeds()
        {
            if (active)
            {
                if (Player.controlDown && Player.velocity.Y > 0.5f)
                    slamPower += 0.2f;

                else
                    slamPower = 0;

                slamPower = Math.Clamp(slamPower, 0, 10);
                ;
                Player.maxFallSpeed += slamPower;

                if (Player.dashDelay < 0)
                {
                    Main.SetCameraLerp(0.2f, 20);

                    Player.gravity *= 0.1f;
                    Player.fullRotation += Player.direction * 0.5f;
                    Player.fullRotationOrigin = Player.Size * 0.5f;
                    Player.velocity *= 1.006f;
                    for (int i = 0; i < 6; i++)
                        Dust.NewDustPerfect(Player.Center + Main.rand.NextVector2Circular(25, 25), DustID.TintableDust, Player.velocity * -Main.rand.NextFloat(-0.5f, 1f), 100, Color.Black, 1f + Main.rand.NextFloat(1.5f)).noGravity = true;

                    Particle.NewParticle(Particle.ParticleType<HueLightDust>(), Player.Center + Main.rand.NextVector2Circular(25, 25), Player.velocity * -Main.rand.NextFloat(-0.6f, 0.6f), Player.hairColor, 0.5f + Main.rand.NextFloat());

                }
                else if (slamPower > 0)
                {
                    if (slamPower < 3)
                        Player.velocity.X *= 0.9f;

                    Player.velocity.X += (Player.direction * 0.2f - Player.velocity.X * 0.01f) * Math.Clamp(Player.velocity.X, 0, 1);
                    Player.fullRotation = Player.fullRotation.AngleLerp(-Player.velocity.X * 0.05f, 0.1f);
                    Player.fullRotationOrigin = Player.Size * 0.5f;
                }
                else
                    Player.fullRotation = 0;

                if (Player.dashDelay > 0 && Player.dashDelay % 2 == 0)
                    Player.dashDelay--;

                if (Collision.SolidTiles(Player.position, Player.width, Player.height, true) && slamPower > 6)
                    slamming = true;

                if (slamming)
                {
                    bunnyHopCounter += 20;
                    for (int i = 0; i < 40; i++)
                        Dust.NewDustPerfect(Player.Bottom + Main.rand.NextVector2Circular(20, 5), DustID.TintableDust, Main.rand.NextVector2Circular(10, 1) - Vector2.UnitY * Main.rand.NextFloat(5f), 100, Color.Black, 1f + Main.rand.NextFloat(1.5f)).noGravity = true;

                    for (int i = 0; i < 5; i++)
                        Particle.NewParticle(Particle.ParticleType<HueLightDust>(), Player.Bottom + Main.rand.NextVector2Circular(30, 5), Main.rand.NextVector2Circular(6, 1) - Vector2.UnitY * Main.rand.NextFloat(2f), Player.hairColor, 1f);

                    slamPower = 0;
                }

                if (bunnyHopCounter > 0)
                {
                    bunnyHopCounter--;

                    if (Player.controlJump)
                    {
                        bunnyHopCounter = 0;
                        Player.velocity.X *= 2f;

                        for (int i = 0; i < 40; i++)
                            Dust.NewDustPerfect(Player.Bottom + Main.rand.NextVector2Circular(20, 5), DustID.TintableDust, -Vector2.UnitY.RotatedByRandom(1f) * Main.rand.NextFloat(7f) * (i / 40f) - new Vector2(Player.direction * 10f, 0f), 100, Color.Black, 1f + Main.rand.NextFloat(1.5f)).noGravity = true;

                        for (int i = 0; i < 5; i++)
                            Particle.NewParticle(Particle.ParticleType<HueLightDust>(), Player.Bottom + Main.rand.NextVector2Circular(20, 5), -Vector2.UnitY.RotatedByRandom(1f) * Main.rand.NextFloat(2f) * (i / 40f) - new Vector2(Player.direction * 10f, 0f), Player.hairColor, 0.5f + Main.rand.NextFloat());

                    }
                }

                if (slamPower > 0f)
                    Main.SetCameraLerp(0.2f, 25);
            }
            else
            {
                Player.fullRotation = 0;
                bunnyHopCounter = 0;
                slamPower = 0;
            }
        }

        public override void ResetEffects()
        {
            active = false;
            slamming = false;
        }

    //    public override void Load()
    //    {
    //        On_Player.DashMovement += ShogunDash;
    //    }

    //    private void ShogunDash(On_Player.orig_DashMovement orig, Player self)
    //    {
    //        if (self.GetModPlayer<ShogunArmorPlayer>().active)
    //        {
    //            if (self.dashDelay > 0)
    //            {
    //                if (self.eocDash > 0)
    //                    self.eocDash--;

    //                if (self.eocDash == 0)
    //                    self.eocHit = -1;

    //                self.dashDelay--;
    //            }
    //            else if (self.dashDelay < 0)
    //            {
    //            }
    //        }
    //        else
    //            orig(self);
    //    }
    }
}
