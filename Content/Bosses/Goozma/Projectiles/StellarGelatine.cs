using CalamityHunt.Common.Systems.Particles;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using ReLogic.Utilities;
using System;
using System.Linq;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityHunt.Content.Bosses.Goozma.Projectiles
{
    public class StellarGelatine : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.TrailCacheLength[Type] = 20;
            ProjectileID.Sets.TrailingMode[Type] = -1;
        }

        public override void SetDefaults()
        {
            Projectile.width = 60;
            Projectile.height = 60;
            Projectile.tileCollide = false;
            Projectile.hostile = false;
            Projectile.friendly = false;
            Projectile.penetrate = -1;
            Projectile.aiStyle = -1;
            Projectile.manualDirectionChange = true;
        }

        public ref float Time => ref Projectile.ai[0];
        public ref float WhoAmI => ref Projectile.ai[1];

        public override void OnSpawn(IEntitySource source)
        {
            Projectile.frame = Main.rand.Next(3);
            Projectile.scale *= Main.rand.NextFloat(0.6f, 1.4f);
            Projectile.direction = Main.rand.NextBool().ToDirectionInt();
        }

        private Vector2 saveTarget;

        public override void AI()
        {
            int owner = -1;
            if (!Main.npc.Any(n => n.type == ModContent.NPCType<StellarGeliath>() && n.active))
            {
                Projectile.active = false;
                return;
            }
            else
                owner = Main.npc.First(n => n.type == ModContent.NPCType<StellarGeliath>() && n.active).whoAmI;

        }

        public override bool PreDraw(ref Color lightColor)
        {
            Asset<Texture2D> texture = ModContent.Request<Texture2D>(Texture);
            Asset<Texture2D> sparkle = TextureAssets.Extra[98];
            Asset<Texture2D> glow = ModContent.Request<Texture2D>($"{nameof(CalamityHunt)}/Assets/Textures/Goozma/GlowSoft");
            Rectangle frame = texture.Frame(3, 1, Projectile.frame, 0);

            float grown = Utils.GetLerpValue(-10, 50, Time, true);
            float scale = MathHelper.SmoothStep(Projectile.scale, 1f, grown);

            Main.EntitySpriteDraw(texture.Value, Projectile.Center - Main.screenPosition, frame, new Color(200, 50, 50, 0) * grown, Projectile.rotation, frame.Size() * 0.5f, scale * 1.5f, 0, 0);
            Main.EntitySpriteDraw(texture.Value, Projectile.Center - Main.screenPosition, frame, Color.White * (0.4f + grown * 0.6f), Projectile.rotation, frame.Size() * 0.5f, scale, 0, 0);

            return false;
        }
    }
}
