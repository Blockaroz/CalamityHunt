using CalamityHunt.Common.Players;
using CalamityHunt.Content.Items.Misc;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent;
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
            Projectile.manualDirectionChange = true;
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

            Projectile.direction = player.MountedCenter.X > Projectile.Center.X ? -1 : 1;
            Projectile.frame = 1;

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

                Projectile.ai[2] = distance;

                if (Projectile.ai[1] > 3)
                    Projectile.frame = 0;

                GrappleTile();

            }
            else if (Projectile.ai[0] == 1f)
            {
                bendPoint = Vector2.Lerp(bendPoint, Vector2.Lerp(player.MountedCenter, Projectile.Center, 0.3f) - player.velocity, 0.1f * Utils.GetLerpValue(10, 70, Projectile.ai[1], true));
                gravPoint = Vector2.Lerp(gravPoint, Vector2.Lerp(player.MountedCenter, Projectile.Center, 0.3f) - player.velocity * 3f, 0.1f * Utils.GetLerpValue(10, 70, Projectile.ai[1], true));
                Projectile.frame = 0;
                Projectile.extraUpdates = 4;

                if (distance < 24f)
                    Projectile.Kill();

                float retreatSpeed = 1f;
                GrappleRetreatSpeed(Main.player[Projectile.owner], ref retreatSpeed);
                Projectile.velocity = Projectile.DirectionTo(player.MountedCenter) * retreatSpeed * (0.1f + MathF.Pow(Utils.GetLerpValue(0, 70, Projectile.ai[1], true), 4f));
            }
            else if (Projectile.ai[0] == 2f)
            {
                bendPoint += (player.AngleTo(Projectile.Center) - MathHelper.PiOver2).ToRotationVector2() * 6f * Utils.GetLerpValue(8, 5, Projectile.ai[1], true);
                gravPoint += (player.AngleTo(Projectile.Center) + MathHelper.PiOver2).ToRotationVector2() * 6f * Utils.GetLerpValue(8, 5, Projectile.ai[1], true);

                bendPoint = Vector2.Lerp(bendPoint, Projectile.Center, 0.1f * Utils.GetLerpValue(10, 70, Projectile.ai[1], true));
                gravPoint = Vector2.Lerp(gravPoint, player.MountedCenter, 0.1f * Utils.GetLerpValue(10, 70, Projectile.ai[1], true));

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

                float factor = 0.2f;
                if (player.controlHook)
                {
                    factor = 0.33f;
                    player.velocity = Vector2.Lerp(player.velocity, player.DirectionTo(Projectile.Center).SafeNormalize(Vector2.Zero) * (player.velocity.Length() + 0.01f), 0.01f);
                }
                player.velocity += player.DirectionTo(Projectile.Center).SafeNormalize(Vector2.Zero) * factor;
                player.velocity = Vector2.Lerp(player.velocity, player.DirectionTo(Projectile.Center).SafeNormalize(Vector2.Zero) * (player.velocity.Length() + 0.01f), Utils.GetLerpValue(10, 240, Projectile.ai[1], true) * 0.1f * factor);

                if (player.velocity.Length() > 31f)
                    player.velocity *= 0.9f;

                if (distance < 96f && !player.controlHook)
                {
                    Projectile.ai[0] = 1f;
                    Projectile.ai[1] = 0f;
                }
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

        public Vector2 gravPoint;
        public Vector2 bendPoint;

        public override bool PreDrawExtras() => false;

        public override bool PreDraw(ref Color lightColor)
        {
            Vector2 playerCenter = Main.player[Projectile.owner].MountedCenter;

            if (Projectile.ai[1] < 2f)
            {
                gravPoint = Vector2.Lerp(playerCenter, Projectile.Center, 0.3f);
                bendPoint = Vector2.Lerp(playerCenter, Projectile.Center, 0.7f);
            }

            Texture2D lineTexture = TextureAssets.FishingLine.Value;

            List<Vector2> controls = new List<Vector2>()
            {
                playerCenter,
                gravPoint,
                bendPoint,
                Projectile.Center
            };

            BezierCurve curve = new BezierCurve(controls);

            int pointCount = 3 + (int)(curve.Distance(300) * 0.1f);
            List<Vector2> points = curve.GetPoints(pointCount);
            points.Add(Projectile.Center);

            Texture2D texture = ModContent.Request<Texture2D>(Texture).Value;
            Rectangle frame = texture.Frame(1, 2, 0, Projectile.frame);
            SpriteEffects effects = Projectile.direction < 0 ? SpriteEffects.FlipHorizontally : SpriteEffects.None;

            for (int i = 1; i < points.Count; i++)
            {
                float rotation = points[i - 1].AngleTo(points[i]);
                Vector2 stretch = new Vector2(Projectile.scale, points[i-1].Distance(points[i]) / (lineTexture.Height));
                Color drawColor = Lighting.GetColor(points[i].ToTileCoordinates());
                Main.EntitySpriteDraw(lineTexture, points[i] - Main.screenPosition, lineTexture.Frame(), drawColor, rotation + MathHelper.PiOver2, lineTexture.Size() * new Vector2(0.5f, 0f), stretch, 0, 0);
            }

            Main.EntitySpriteDraw(texture, Projectile.Center - Main.screenPosition, frame, lightColor, points[pointCount - 2].AngleTo(points[pointCount - 1]) + MathHelper.PiOver2, frame.Size() * 0.5f, Projectile.scale, effects, 0);

            return false;
        }
    }
}
