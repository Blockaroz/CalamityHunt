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

        public static Color[] GoozColors => new Color[]
        {
            new Color(GoozColorsVector3[3]),
            new Color(GoozColorsVector3[4]),
            new Color(GoozColorsVector3[5]),
            new Color(GoozColorsVector3[6]),
            new Color(GoozColorsVector3[7]),
        };

        public static Vector3[] GoozColorsVector3
        {
            get
            {
                if (Main.drunkWorld)
                    return ColorsForFunnyWorlds(GoozmaColorType);                
                
                if (Main.getGoodWorld)
                    return GoozmaColorUtils.Masterful;                
                
                if (Main.getGoodWorld && Main.masterMode)
                    return GoozmaColorUtils.Nuclear;

                if (Main.notTheBeesWorld)
                    return GoozmaColorUtils.Honey;

                return GoozmaColorUtils.Oil;
            }
        }

        public static int GoozmaColorType;

        private static Vector3[] ColorsForFunnyWorlds(int type)
        {
            switch (GoozmaColorType)
            {
                default:
                    return GoozmaColorUtils.Oil;
            }
        }

        public static void DrawWormhole(Vector2 position, Color innerColor, float rotation, float scale)
        {
            Asset<Texture2D> wormhole = ModContent.Request<Texture2D>($"{nameof(CalamityHunt)}/Assets/Textures/Goozma/Wormhole");
            Main.EntitySpriteDraw(wormhole.Value, position - Main.screenPosition, null, innerColor, rotation, wormhole.Size() * 0.5f, scale * 2f, 0, 0);

        }
    }
}
