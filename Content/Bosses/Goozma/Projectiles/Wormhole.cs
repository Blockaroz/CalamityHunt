using CalamityHunt.Content.Bosses.Goozma.Slimes;
using CalamityHunt.Content.Particles;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using CalamityHunt.Common.Systems.Particles;
using ReLogic.Content;
using System.Linq;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityHunt.Content.Bosses.Goozma.Projectiles
{
    public class Wormhole : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Wormhole");
        }

        public override void SetDefaults()
        {
            Projectile.width = 156;
            Projectile.height = 156;
            Projectile.tileCollide = false;
            Projectile.hostile = true;
            Projectile.friendly = false;
            Projectile.penetrate = -1;
            Projectile.aiStyle = -1;
            Projectile.damage = 0;
        }

        public ref float Time => ref Projectile.ai[0];
        public ref NPC Host => ref Main.npc[(int)Projectile.ai[1]];

        public override void AI()
        {
            if (!Main.npc.Any(n => n.type == ModContent.NPCType<StellarGeliath>() && n.active))
                Projectile.active = false;
            else
                Projectile.ai[1] = Main.npc.First(n => n.type == ModContent.NPCType<StellarGeliath>() && n.active).whoAmI;

            Projectile.scale = Utils.GetLerpValue(0, 15, Time, true) * Utils.GetLerpValue(20, 25, Projectile.timeLeft, true);
            Projectile.Center += Host.GetTargetData().Velocity * 0.9f * Utils.GetLerpValue(50, 30, Time, true);

            Projectile.velocity = Projectile.AngleFrom(Host.Center).ToRotationVector2() * Projectile.oldVelocity.Length();
            Projectile.velocity *= 0.9f;

            Time++;
            Projectile.rotation += 0.01f * Projectile.direction;

            if (Time == 45 && !Main.dedServ)
            {
                Particle crack = Particle.NewParticle(Particle.ParticleType<CrackSpot>(), Projectile.Center, Vector2.Zero, Main.OurFavoriteColor, 40f);
                crack.data = "Wormhole";
                SoundEngine.PlaySound(SoundID.Item176, Projectile.Center);
            }
        }

        public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox) => false;

        public override bool PreDraw(ref Color lightColor)
        {
            Asset<Texture2D> bloom = ModContent.Request<Texture2D>($"{nameof(CalamityHunt)}/Assets/Textures/Goozma/GlowSoft");
            Asset<Texture2D> flare = TextureAssets.Extra[98];

            Main.EntitySpriteDraw(bloom.Value, Projectile.Center - Main.screenPosition, null, new Color(10, 0, 80, 0), 0, bloom.Size() * 0.5f, 8 * Projectile.scale, 0, 0);
            Main.EntitySpriteDraw(bloom.Value, Projectile.Center - Main.screenPosition, null, new Color(710, 0, 150, 0), 0, bloom.Size() * 0.5f, 3 * Projectile.scale, 0, 0);
            Main.EntitySpriteDraw(flare.Value, Projectile.Center - Main.screenPosition, null, new Color(30, 10, 150, 0), 0, flare.Size() * 0.5f, new Vector2(1f, 10f) * Projectile.scale, 0, 0);
            Main.EntitySpriteDraw(flare.Value, Projectile.Center - Main.screenPosition, null, new Color(30, 10, 150, 0), MathHelper.PiOver2, flare.Size() * 0.5f, new Vector2(1f, 10f) * Projectile.scale, 0, 0);
            Main.EntitySpriteDraw(flare.Value, Projectile.Center - Main.screenPosition, null, new Color(255, 230, 60, 0), 0, flare.Size() * 0.5f, new Vector2(0.5f, 5f) * Projectile.scale, 0, 0);
            Main.EntitySpriteDraw(flare.Value, Projectile.Center - Main.screenPosition, null, new Color(255, 230, 60, 0), MathHelper.PiOver2, flare.Size() * 0.5f, new Vector2(0.5f, 5f) * Projectile.scale, 0, 0);
            Main.EntitySpriteDraw(flare.Value, Projectile.Center - Main.screenPosition, null, new Color(255, 255, 255, 0), 0, flare.Size() * 0.5f, new Vector2(0.8f, 2f) * Projectile.scale, 0, 0);
            Main.EntitySpriteDraw(flare.Value, Projectile.Center - Main.screenPosition, null, new Color(255, 255, 255, 0), MathHelper.PiOver2, flare.Size() * 0.5f, new Vector2(0.8f, 2f) * Projectile.scale, 0, 0);

            return false;
        }
    }
}
