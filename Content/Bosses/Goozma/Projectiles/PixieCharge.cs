using CalamityHunt.Common.Systems.Particles;
using CalamityHunt.Content.Particles;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.GameContent;
using Terraria.GameContent.Bestiary;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityHunt.Content.Bosses.Goozma.Projectiles
{
    public class PixieCharge : ModNPC
    {
        public override void SetStaticDefaults()
        {
            NPCID.Sets.ProjectileNPC[Type] = true;
            NPCID.Sets.NPCBestiaryDrawOffset[Type] = new() { Hide = true };
            NPCID.Sets.ShimmerImmunity[Type] = true;
            NPCID.Sets.DebuffImmunitySets.Add(Type, new() { ImmuneToAllBuffsThatAreNotWhips = true });
        }

        public override void SetDefaults()
        {
            NPC.width = 70;
            NPC.height = 70;
            NPC.noGravity = true;
            NPC.noTileCollide = true;
            NPC.aiStyle = -1;
            NPC.lifeMax = 50000;
            NPC.knockBackResist = 0.01f;
        }

        public ref float Time => ref NPC.ai[0];
        public ref float Owner => ref NPC.ai[2];

        public override void AI()
        {
            if (Owner < 0)
            {
                NPC.active = false;
                return;
            }
            else if (!Main.npc[(int)Owner].active || Main.npc[(int)Owner].type != ModContent.NPCType<DivineGargooptuar>())
            {
                NPC.active = false;
                return;
            }

            if (Time < 100)
            {
                NPC.defense = 2000;
                NPC.velocity *= 0.93f;
            }
            else
            {
                NPC.defense = 0;
                NPC.ai[1] = 1;
            }

            Particle.NewParticle(Particle.ParticleType<CosmicSmoke>(), NPC.Center, Main.rand.NextVector2Circular(3, 3) + NPC.velocity.RotatedByRandom(0.1f) * 0.5f, Main.hslToRgb((NPC.localAI[0] * 0.01f) % 1f, 1f, 0.7f, 0), NPC.scale + Main.rand.NextFloat(0.7f));
            
            NPC.localAI[0]++;
            Time++;
        }

        public override void OnHitByProjectile(Projectile projectile, NPC.HitInfo hit, int damageDone) => OnHitEffect();

        public override void OnHitByItem(Player player, Item item, NPC.HitInfo hit, int damageDone) => OnHitEffect();

        private void OnHitEffect()
        {
            for (int i = 0; i < 2; i++)
            {
                Color glowColor = Main.hslToRgb(NPC.localAI[0] * 0.01f % 1f, 1f, 0.5f, 0);
                glowColor.A /= 2;
                Dust.NewDustPerfect(NPC.Center + Main.rand.NextVector2Circular(36, 36), DustID.AncientLight, Main.rand.NextVector2Circular(15, 15) + NPC.velocity, 0, glowColor, 1f + Main.rand.NextFloat(2f)).noGravity = true;

                if (Main.rand.NextBool(3))
                    Particle.NewParticle(Particle.ParticleType<PrettySparkle>(), NPC.Center + Main.rand.NextVector2Circular(54, 54), Main.rand.NextVector2Circular(10, 10) + NPC.velocity * 0.1f, Main.hslToRgb((NPC.localAI[0] * 0.01f) % 1f, 1f, 0.7f, 0) * 0.5f, 0.5f + Main.rand.NextFloat());
            }
        }

        public override bool PreDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {
            Texture2D texture = TextureAssets.Npc[Type].Value;
            Texture2D sparkle = TextureAssets.Extra[98].Value;
            Texture2D ring = AssetDirectory.Textures.GlowRing;
            Texture2D glow = AssetDirectory.Textures.Glow;

            Color bloomColor = Main.hslToRgb((NPC.localAI[0] * 0.01f) % 1f, 1f, 0.7f, 0);
            SpriteEffects direction = NPC.velocity.X > 0 ? SpriteEffects.FlipHorizontally : SpriteEffects.None;

            Main.EntitySpriteDraw(texture, NPC.Center - screenPos, null, Color.Black * 0.1f, NPC.rotation * 0.5f, texture.Size() * 0.5f, NPC.scale * 1.5f, 0, 0);
            Main.EntitySpriteDraw(texture, NPC.Center - screenPos, null, bloomColor, NPC.rotation * 1.5f, texture.Size() * 0.5f, NPC.scale * 0.8f, 0, 0);

            Main.EntitySpriteDraw(texture, NPC.Center - screenPos, null, new Color(100, 100, 100, 0), NPC.rotation, texture.Size() * 0.5f, NPC.scale * 0.8f, direction, 0);
            Main.EntitySpriteDraw(texture, NPC.Center - screenPos, null, new Color(200, 200, 200, 0), NPC.rotation, texture.Size() * 0.5f, NPC.scale * 0.7f, direction, 0);
            Main.EntitySpriteDraw(glow, NPC.Center - screenPos, null, bloomColor * 0.2f, NPC.rotation, glow.Size() * 0.5f, NPC.scale, direction, 0);

            float lensAngle = NPC.AngleFrom(Main.LocalPlayer.Center) + MathHelper.PiOver2;
            float lensPower = 1f + NPC.Distance(Main.LocalPlayer.Center) * 0.003f;

            float sparkRotation = NPC.velocity.X * 0.01f;
            float wobble = 1f + (float)Math.Sin(NPC.localAI[0] * 0.5f) * 0.05f;
            Vector2 sparkleScale = new Vector2(0.5f, 6f) * wobble * Utils.GetLerpValue(0.3f, 1f, NPC.scale, true);
            Main.EntitySpriteDraw(sparkle, NPC.Center - screenPos, null, bloomColor * 0.1f, sparkRotation + MathHelper.PiOver2 + 0.2f, sparkle.Size() * 0.5f, sparkleScale, 0, 0);
            Main.EntitySpriteDraw(sparkle, NPC.Center - screenPos, null, new Color(255, 255, 255, 0), sparkRotation + MathHelper.PiOver2 + 0.2f, sparkle.Size() * 0.5f, sparkleScale * 0.4f, 0, 0);
            Main.EntitySpriteDraw(sparkle, NPC.Center - screenPos, null, bloomColor * 0.1f, sparkRotation + MathHelper.PiOver2 - 0.3f, sparkle.Size() * 0.5f, sparkleScale * 0.7f, 0, 0);
            Main.EntitySpriteDraw(sparkle, NPC.Center - screenPos, null, new Color(255, 255, 255, 0), sparkRotation + MathHelper.PiOver2 - 0.3f, sparkle.Size() * 0.5f, sparkleScale * 0.3f, 0, 0);

            float ringWobble0 = 1.05f + (float)Math.Sin(NPC.localAI[0] * 0.1f + 0.6f) * 0.01f;
            float ringWobble1 = 1.05f + (float)Math.Sin(NPC.localAI[0] * 0.1f + 0.3f) * 0.01f;
            Vector2 middleRingOff = new Vector2(0, 50 * lensPower).RotatedBy(lensAngle - 0.3f);

            Main.EntitySpriteDraw(ring, NPC.Center + middleRingOff - screenPos, null, bloomColor * 0.05f, sparkRotation + MathHelper.PiOver2 + 0.2f, ring.Size() * 0.5f, new Vector2(1f, 1.05f) * NPC.scale * ringWobble0, 0, 0);
            Main.EntitySpriteDraw(ring, NPC.Center - screenPos, null, bloomColor * 0.1f, sparkRotation + MathHelper.PiOver2 + 0.2f, ring.Size() * 0.5f, new Vector2(1f, 1.05f) * NPC.scale * 1.4f * ringWobble1, 0, 0);

            Vector2 bottomRingOff = new Vector2(0, 40).RotatedBy(lensAngle + 0.2f) * lensPower;
            Main.EntitySpriteDraw(ring, NPC.Center + bottomRingOff - screenPos, null, bloomColor * 0.1f, sparkRotation + MathHelper.PiOver2 + 0.2f, ring.Size() * 0.5f, new Vector2(1f, 1.05f) * NPC.scale * 0.4f, 0, 0);
            Vector2 topRingOff = new Vector2(0, -60).RotatedBy(lensAngle) * lensPower;
            Main.EntitySpriteDraw(ring, NPC.Center + topRingOff - screenPos, null, bloomColor * 0.1f, sparkRotation + MathHelper.PiOver2 + 0.2f, ring.Size() * 0.5f, new Vector2(1f, 1.05f) * NPC.scale * 0.8f, 0, 0);

            Vector2 topFlareOff = topRingOff * ringWobble0 * 1.1f;
            for (int i = 0; i < 3; i++)
                Main.EntitySpriteDraw(sparkle, NPC.Center + topFlareOff - screenPos, null, bloomColor * 0.2f, MathHelper.TwoPi / 3f * i, sparkle.Size() * 0.5f, new Vector2(0.3f, 1.5f), 0, 0);

            Main.EntitySpriteDraw(glow, NPC.Center - screenPos, null, bloomColor * (0.3f / lensPower), NPC.rotation * 0.5f, glow.Size() * 0.5f, NPC.scale * 4f, 0, 0);
            Main.EntitySpriteDraw(glow, NPC.Center - screenPos, null, bloomColor * (0.08f / lensPower), NPC.rotation * 0.5f, glow.Size() * 0.5f, NPC.scale * 8f, 0, 0);

            return false;
        }
    }
}
