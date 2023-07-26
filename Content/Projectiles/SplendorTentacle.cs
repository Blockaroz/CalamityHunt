using CalamityHunt.Common.Players;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Linq;
using Terraria;
using Terraria.GameContent;
using Terraria.ModLoader;

namespace CalamityHunt.Content.Projectiles
{
    public class SplendorTentacle : ModProjectile
    {
        public override void SetDefaults()
        {
            Projectile.width = 32;
            Projectile.height = 32;
            Projectile.friendly = true;
            Projectile.hostile = false;
            Projectile.aiStyle = -1;
        }

        public ref float Time => ref Projectile.ai[0];
        public ref float Attack => ref Projectile.ai[1];

        public ref Player Player => ref Main.player[Projectile.owner];

        public override void AI()
        {
            if (Player.dead || !Player.active)
            {
                Projectile.Kill();
                return;
            }    

            int selfCount = 0;
            foreach (Projectile proj in Main.projectile.Where(n => n.type == Type && n.active && n.owner == Projectile.owner))
            {
                selfCount++;
            }
            if (selfCount > Player.GetModPlayer<SplendorJamPlayer>().tentacleCount)
                Projectile.Kill();

            Projectile.damage = 10000;
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D texture = TextureAssets.Projectile[Type].Value;

            Rectangle frame = texture.Frame(1, 5, 0, 4);

            return false;
        }
    }

    public struct Segment
    {
        public Vector2 position;
        public float size;
    }
}
