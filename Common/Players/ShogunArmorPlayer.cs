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

                    Player.gravity *= 0.6f;
                    Player.fullRotation += Player.direction * 0.5f;
                    Player.fullRotationOrigin = Player.Size * 0.5f;
                    Player.velocity *= 1.006f;
                    for (int i = 0; i < 6; i++)
                    {
                        Dust.NewDustPerfect(Player.Center + Main.rand.NextVector2Circular(25, 25), DustID.TintableDust, Player.velocity * -Main.rand.NextFloat(-0.5f, 1f), 100, Color.Black, 1f + Main.rand.NextFloat(1.5f)).noGravity = true;
                    }
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
                    Player.velocity.X *= 0.1f;
                    for (int i = 0; i < 40; i++)
                        Dust.NewDustPerfect(Player.Bottom + Main.rand.NextVector2Circular(30, 5), DustID.TintableDust, -Vector2.UnitY.RotatedByRandom(1f) * Main.rand.NextFloat(7f) * (i / 40f) + new Vector2(Player.velocity.X * 2f, 0f), 100, Color.Black, 1f + Main.rand.NextFloat(1.5f)).noGravity = true;

                    slamPower = 0;
                }

                if (slamPower > 0f)
                    Main.SetCameraLerp(0.2f, 25);
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
