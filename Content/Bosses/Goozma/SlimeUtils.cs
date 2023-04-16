using CalamityHunt.Content.Bosses.Goozma.Projectiles;
using Terraria.ID;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using Terraria;
using Terraria.ModLoader;

namespace CalamityHunt.Content.Bosses.Goozma
{
    public static class SlimeUtils
    {
        public static Vector2 FindSmashSpot(this NPC NPC, Vector2 target)
        {
            Vector2 pos = target;
            for (int j = 0; j < 32; j++)
            {
                Point world = new Point(pos.ToTileCoordinates().X, pos.ToTileCoordinates().Y + j);
                if (WorldGen.InWorld(world.X, world.Y))
                {
                    if (WorldGen.SolidTileAllowTopSlope(world.X, world.Y + j))
                    {
                        pos.Y = (int)(pos.Y / 16f) * 16f + j * 16 - NPC.height / 2f + 16;
                        break;
                    }
                }
            }
            return pos;
        }

        public static Vector2 FindSmashSpot(this Projectile Projectile, Vector2 target)
        {
            Vector2 pos = target;
            for (int j = 0; j < 32; j++)
            {
                Point world = new Point(pos.ToTileCoordinates().X, pos.ToTileCoordinates().Y + j);
                if (WorldGen.InWorld(world.X, world.Y))
                {
                    if (WorldGen.SolidTileAllowTopSlope(world.X, world.Y + j))
                    {
                        pos.Y = (int)(pos.Y / 16f) * 16f + j * 16 - Projectile.height / 2f + 16;
                        break;
                    }
                }
            }
            return pos;
        }

        public static Color[] GoozColorArray = new Color[]
        {
            new Color(174, 23, 189),
            new Color(255, 85, 228),
            new Color(237, 128, 60),
            new Color(247, 255, 101),
            new Color(176, 234, 85),
            new Color(102, 219, 249),
            new Color(113, 53, 146)
        };

        public static void DrawWormhole(Vector2 position, Color innerColor, float rotation, float scale)
        {
            Asset<Texture2D> wormhole = ModContent.Request<Texture2D>($"{nameof(CalamityHunt)}/Assets/Textures/Goozma/Wormhole");
            Main.EntitySpriteDraw(wormhole.Value, position - Main.screenPosition, null, innerColor, rotation, wormhole.Size() * 0.5f, scale * 2f, 0, 0);

        }
    }
}
