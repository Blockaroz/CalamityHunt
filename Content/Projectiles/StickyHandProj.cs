using CalamityHunt.Common.Players;
using CalamityHunt.Content.Items.Misc;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;
using static Humanizer.In;

namespace CalamityHunt.Content.Projectiles
{
    public class StickyHandProj : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            //ProjectileID.Sets.SingleGrappleHook[Type] = true;
        }

        public override void SetDefaults()
        {
            Projectile.netImportant = true;
            Projectile.width = 18;
            Projectile.height = 18;
            Projectile.friendly = true;
            Projectile.penetrate = -1;
            Projectile.tileCollide = false;
            Projectile.timeLeft *= 10;
            Projectile.aiStyle = 7;
            Projectile.damage = 0;
        }

        public override bool PreAI()
        {
            Player player = Main.player[Projectile.owner];

            if (player.dead || player.CCed)
            {
                Projectile.Kill();
                return false;
            }

            if (Main.myPlayer == Projectile.owner)
            {
                int x = (int)(Projectile.Center.X / 16f);
                int y = (int)(Projectile.Center.Y / 16f);
                if (x > 0 && y > 0 && x < Main.maxTilesX && y < Main.maxTilesY && !Main.tile[x, y].IsActuated && TileID.Sets.CrackedBricks[Main.tile[x, y].TileType] && Main.rand.NextBool(16))
                {
                    WorldGen.KillTile(x, y);
                    if (Main.netMode != NetmodeID.SinglePlayer)
                        NetMessage.SendData(MessageID.TileManipulation, -1, -1, null, 20, x, y);
                }
            }

            Projectile.rotation = Projectile.AngleFrom(player.MountedCenter);
            float distance = Projectile.Distance(player.MountedCenter);

            if (distance > 2500)
                Projectile.Kill();

            if (Projectile.ai[0] == 0f)
            {
                Projectile.extraUpdates = 2;

                if (distance > GrappleRange())
                {
                    Projectile.ai[0] = 1f;
                    Projectile.ai[1] = 0f;
                }

                Projectile.ai[2] = player.Distance(Projectile.Center);

                GrappleTile();

            }
            else if (Projectile.ai[0] == 1f)
            {
                Projectile.extraUpdates = 4;

                if (distance < 24f)
                    Projectile.Kill();

                float retreatSpeed = 1f;
                GrappleRetreatSpeed(Main.player[Projectile.owner], ref retreatSpeed);
                Projectile.velocity = Projectile.DirectionTo(player.MountedCenter) * retreatSpeed * (0.1f + MathF.Pow(Utils.GetLerpValue(0, 70, Projectile.ai[1], true), 4f));
            }
            else if (Projectile.ai[0] == 2f)
            {
                if (distance > GrappleRange() * 2f)
                {
                    Projectile.ai[0] = 1f;
                    Projectile.ai[1] = 0f;
                }

                Point tile = Projectile.Center.ToTileCoordinates();
                if (Main.tile[tile] == null)
                    Main.tile[tile.X, tile.Y].ClearEverything();

                if (!CanLatchToTile(tile.X, tile.Y) || player.controlJump)
                    Projectile.ai[0] = 1f;

                player.GetModPlayer<MovementModifyPlayer>().stickyHand = true;

                player.velocity += player.DirectionTo(Projectile.Center).SafeNormalize(Vector2.Zero) * 0.5f;
                player.velocity = Vector2.Lerp(player.velocity, player.DirectionTo(Projectile.Center).SafeNormalize(Vector2.Zero) * (player.velocity.Length() + 0.1f), Utils.GetLerpValue(30, 200, Projectile.ai[1], true) * 0.1f);

                if (distance < 64f)
                    Projectile.Kill();
            }

            Projectile.ai[1]++;

            return false;
        }

        private void GrappleTile()
        {
            Vector2 vector3 = Projectile.Center - new Vector2(5f);
            Vector2 vector4 = Projectile.Center + new Vector2(5f);
            Point point = (vector3 - new Vector2(16f)).ToTileCoordinates();
            Point point2 = (vector4 + new Vector2(32f)).ToTileCoordinates();

            int xLeftLimit = point.X;
            int xRightLimit = point2.X;
            int yTopLimit = point.Y;
            int yBottomLimit = point2.Y;
            if (xLeftLimit < 0)
                xLeftLimit = 0;

            if (xRightLimit > Main.maxTilesX)
                xRightLimit = Main.maxTilesX;

            if (yTopLimit < 0)
                yTopLimit = 0;

            if (yBottomLimit > Main.maxTilesY)
                yBottomLimit = Main.maxTilesY;

            Player player = Main.player[Projectile.owner];
            Vector2 tileWorldCoordinates = default;
            for (int l = xLeftLimit; l < xRightLimit; l++)
            {
                for (int m = yTopLimit; m < yBottomLimit; m++)
                {
                    if (Main.tile[l, m] == null)
                        Main.tile[l, m].ClearEverything();

                    tileWorldCoordinates.X = l * 16;
                    tileWorldCoordinates.Y = m * 16;
                    if (!(vector3.X + 10f > tileWorldCoordinates.X) || !(vector3.X < tileWorldCoordinates.X + 16f) || !(vector3.Y + 10f > tileWorldCoordinates.Y) || !(vector3.Y < tileWorldCoordinates.Y + 16f))
                        continue;

                    Tile tile = Main.tile[l, m];
                    if (!CanLatchToTile(l, m) || player.IsBlacklistedForGrappling(new Point(l, m)))
                        continue;

                    if (Main.myPlayer != Projectile.owner)
                        continue;

                    int grappleCount = 0;
                    int projID = -1;
                    int maxGrappleCount = 1;

                    NumGrappleHooks(player, ref maxGrappleCount);

                    for (int proj = 0; proj < Main.maxProjectiles; proj++)
                    {
                        if (Main.projectile[proj].active && Main.projectile[proj].owner == Projectile.owner && Main.projectile[proj].type == ModContent.ProjectileType<StickyHandProj>())
                        {
                            if (Main.projectile[proj].whoAmI != Projectile.whoAmI)
                                projID = proj;

                            grappleCount++;
                            if (grappleCount > 1)
                                Main.projectile[projID].ai[0] = 1f;

                        }
                    }

                    WorldGen.KillTile(l, m, fail: true, effectOnly: true);
                    SoundEngine.PlaySound(SoundID.Dig, new Vector2(l, m));
                    Projectile.velocity = Vector2.Zero;
                    Projectile.ai[0] = 2f;
                    Projectile.position.X = l * 16 + 8 - Projectile.width / 2;
                    Projectile.position.Y = m * 16 + 8 - Projectile.height / 2;
                    Rectangle? tileVisualHitbox = WorldGen.GetTileVisualHitbox(l, m);
                    if (tileVisualHitbox.HasValue)
                        Projectile.Center = tileVisualHitbox.Value.Center.ToVector2();

                    Projectile.damage = 0;
                    Projectile.netUpdate = true;
                    if (Main.myPlayer == Projectile.owner)
                        NetMessage.SendData(MessageID.PlayerControls, -1, -1, null, Projectile.owner);

                    break;
                }

                if (Projectile.ai[0] == 2f)
                {
                    Projectile.ai[1] = 0f;
                    break;
                }
            }
        }

        public override bool? CanUseGrapple(Player player)
        {
            int hooksOut = 0;
            for (int l = 0; l < Main.maxProjectiles; l++)
            {
                if (Main.projectile[l].active && Main.projectile[l].owner == Main.myPlayer && Main.projectile[l].type == Projectile.type)
                    hooksOut++;
            }

            return true;
        }

        public override void NumGrappleHooks(Player player, ref int numHooks)
        {
            numHooks = 1;
        }

        public override void GrappleRetreatSpeed(Player player, ref float speed)
        {
            speed = 10;
        }

        public override void GrapplePullSpeed(Player player, ref float speed)
        {
            speed = 15f;
        }

        public override float GrappleRange()
        {
            if (Projectile.ai[0] == 0f)
                return 900f;

            return 500f;
        }

        private bool CanLatchToTile(int x, int y)
        {
            Tile theTile = Main.tile[x, y];
            bool vanilla = Main.tileSolid[theTile.TileType] | (theTile.TileType == 314);
            vanilla &= theTile.HasUnactuatedTile;

            if (GrappleCanLatchOnTo(Main.player[Projectile.owner], x, y) is bool modOverride)
                return modOverride;

            return vanilla;
        }

        public override bool PreDrawExtras() => true;

        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D texture = ModContent.Request<Texture2D>(Texture).Value;
            Rectangle frame = texture.Frame(1, 2, 0, (Projectile.ai[0] == 2f ? 0 : 1));



            return false;
        }
    }
}
