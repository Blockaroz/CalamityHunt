using CalamityHunt.Common.Systems.Particles;
using CalamityHunt.Content.Particles;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Security.Policy;
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
        private int inertiaTimer;
        private int dashTime;

        public override void PostUpdateRunSpeeds()
        {
            bool inAir = !WorldGen.SolidOrSlopedTile(Main.tile[(Player.Bottom / 16f).ToPoint()]) && !Collision.SolidCollision(Player.position, Player.width, Player.height);
            if (active)
            {
                if (Player.controlDown && !Player.mount.Active)
                    Player.gravity *= 1.1111f;

                if (Player.controlDown && Player.velocity.Y > 1f && !Player.mount.Active)
                    slamPower++;
                else
                    slamPower = 0;

                slamPower = Math.Clamp(slamPower, 0, 10);

                Player.maxFallSpeed += slamPower;

                if (inertiaTimer > 0)
                {
                    inertiaTimer--;
                    Player.runSlowdown *= 0.33f;
                }

                if (Player.dashDelay < 0)
                {
                    inertiaTimer = 1;

                    Main.SetCameraLerp(0.2f, 25);
                    Player.fullRotation += (float)Math.Cbrt(Player.velocity.X) * 0.2f * (1 + dashTime);
                    Player.fullRotationOrigin = Player.Size * 0.5f;
                    if (Player.controlJump && Player.releaseJump)
                    {
                        Player.dashDelay = 0;
                        inertiaTimer = 60;
                    }

                    for (int i = 0; i < 6; i++)
                        Dust.NewDustPerfect(Player.Center + Main.rand.NextVector2Circular(25, 25), DustID.TintableDust, Player.velocity * -Main.rand.NextFloat(-0.5f, 1f), 100, Color.Black, 1f + Main.rand.NextFloat(1.5f)).noGravity = true;

                    Particle.NewParticle(Particle.ParticleType<HueLightDust>(), Player.Center + Main.rand.NextVector2Circular(25, 25), Player.velocity * -Main.rand.NextFloat(-0.6f, 0.6f), Player.shirtColor, 0.5f + Main.rand.NextFloat());
                }
                else if (slamPower > 0)
                {
                    Player.velocity.X += (Player.direction * 0.2f - Player.velocity.X * 0.01f) * Math.Clamp(Player.velocity.X, 0, 1);
                    Player.fullRotation = Player.fullRotation.AngleLerp(-Player.velocity.X * 0.05f, 0.1f);
                    Player.fullRotationOrigin = Player.Size * 0.5f;

                    Main.SetCameraLerp(0.2f, 25);
                }
                else if (bunnyHopCounter < 0)
                    Player.fullRotation = Player.velocity.X * 0.01f;

                else
                    Player.fullRotation = Player.fullRotation.AngleLerp(0f, 0.15f);

                if (Player.dashDelay > 0)
                    Player.dashDelay--;

                if (!inAir && slamPower > 6)
                    slamming = true;

                if (slamming)
                {
                    bunnyHopCounter += 25;
                    for (int i = 0; i < 40; i++)
                        Dust.NewDustPerfect(Player.Bottom + Main.rand.NextVector2Circular(20, 5), DustID.TintableDust, Main.rand.NextVector2Circular(10, 1) - Vector2.UnitY * Main.rand.NextFloat(5f), 100, Color.Black, 1f + Main.rand.NextFloat(1.5f)).noGravity = true;

                    for (int i = 0; i < 5; i++)
                        Particle.NewParticle(Particle.ParticleType<HueLightDust>(), Player.Bottom + Main.rand.NextVector2Circular(30, 5), Main.rand.NextVector2Circular(6, 1) - Vector2.UnitY * Main.rand.NextFloat(2f), Player.shirtColor, 1f);

                    slamPower = 0;
                }

                if (bunnyHopCounter > 0)
                {
                    bunnyHopCounter--;

                    if (Player.controlJump || Player.dashDelay < 0)
                    {
                        bunnyHopCounter = -20;
                        Player.velocity.X *= 2f;
                        for (int i = 0; i < 40; i++)
                            Dust.NewDustPerfect(Player.Bottom + Main.rand.NextVector2Circular(20, 5), DustID.TintableDust, -Vector2.UnitY.RotatedByRandom(1f) * Main.rand.NextFloat(7f) * (i / 40f) - new Vector2(Player.direction * 10f, 0f), 100, Color.Black, 1f + Main.rand.NextFloat(1.5f)).noGravity = true;

                        for (int i = 0; i < 5; i++)
                            Particle.NewParticle(Particle.ParticleType<HueLightDust>(), Player.Bottom + Main.rand.NextVector2Circular(20, 5), -Vector2.UnitY.RotatedByRandom(1f) * Main.rand.NextFloat(2f) * (i / 40f) - new Vector2(Player.direction * 10f, 0f), Player.shirtColor, 0.5f + Main.rand.NextFloat());

                    }
                }
                if (bunnyHopCounter < 0)
                {
                    bunnyHopCounter++;
                }

                if (inAir)
                    Player.maxRunSpeed *= 1.2f;
            }
            else
            {
                Player.fullRotation = 0;
                bunnyHopCounter = 0;
                slamPower = 0;
            }

            if (dashTime > 0)
                dashTime--;
        }

        public override void ResetEffects()
        {
            active = false;
            slamming = false;
        }

        public override void Load()
        {
            On_Player.DashMovement += ShogunDash;
        }

        private void ShogunDash(On_Player.orig_DashMovement orig, Player self)
        {
            if (self.GetModPlayer<ShogunArmorPlayer>().active)
            {
                //if (self.dashDelay > 0)
                //{
                //    if (self.eocDash > 0)
                //        self.eocDash--;

                //    if (self.eocDash == 0)
                //        self.eocHit = -1;

                //    self.dashDelay--;
                //}

                //else if (self.dashDelay < 0)
                //{
                //    self.StopVanityActions();

                //    self.doorHelper.AllowOpeningDoorsByVelocityAloneForATime(60);
                //    self.vortexStealthActive = false;

                //    self.dashDelay = 10;
                //    if (self.velocity.X < 0f)
                //        self.velocity.X = -Math.Max(self.accRunSpeed, self.maxRunSpeed) * 2f;
                //    else if (self.velocity.X > 0f)
                //        self.velocity.X = Math.Max(self.accRunSpeed, self.maxRunSpeed) * 2f;

                //    //self.dashType = 5;
                //}
                //else
                //{
                //    object[] parameters = new object[] { 0, false, null };
                //    self.GetType().GetMethod("DoCommonDashHandle", BindingFlags.NonPublic | BindingFlags.Instance).Invoke(self, parameters);
                //    int dir = (int)parameters[0];
                //    bool dashing = (bool)parameters[1];
                //    if (dashing)
                //    {
                //        if (self.mount.Active)
                //            self.mount.Dismount(self);

                //        self.dashDelay = -1;
                //        self.velocity.X = dir * 20;
                //    }
                //}

                self.dashType = 1;
            }
            
                orig(self);
        }
    }
}
