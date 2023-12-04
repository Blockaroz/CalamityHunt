﻿using System;
using System.Linq;
using CalamityHunt.Common.Graphics.RenderTargets;
using CalamityHunt.Common.Systems.Particles;
using CalamityHunt.Content.Particles;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using Terraria;
using Terraria.DataStructures;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Utilities;

namespace CalamityHunt.Content.Bosses.Goozma.Projectiles
{
    public class StellarGelatine : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.DrawScreenCheckFluff[Type] = 2000;
            ProjectileID.Sets.TrailCacheLength[Type] = 3;
            ProjectileID.Sets.TrailingMode[Type] = -1;
        }

        public override void SetDefaults()
        {
            Projectile.width = 60;
            Projectile.height = 60;
            Projectile.tileCollide = false;
            Projectile.hostile = true;
            Projectile.friendly = false;
            Projectile.penetrate = -1;
            Projectile.aiStyle = -1;
            Projectile.manualDirectionChange = true;
        }

        public ref float Time => ref Projectile.ai[0];
        public ref float Owner => ref Projectile.ai[2];

        public override void OnSpawn(IEntitySource source)
        {
            Projectile.localAI[0] = Main.rand.Next(20);
            Projectile.localAI[1] = Main.rand.NextFloat(0.8f, 1.4f);
            WeightedRandom<int> typeOfStarBit = new WeightedRandom<int>();
            typeOfStarBit.Add(0, 0.2f);
            typeOfStarBit.Add(1, 0.3f);
            typeOfStarBit.Add(2, 0.3f);
            typeOfStarBit.Add(3, 0.6f);
            Projectile.frame = typeOfStarBit;
            Projectile.direction = Main.rand.NextBool().ToDirectionInt();
            Projectile.rotation = Main.rand.NextFloat(-0.3f, 0.3f);

            for (int i = 0; i < ProjectileID.Sets.TrailCacheLength[Type]; i++) {
                Projectile.oldPos[i] = Projectile.Center;
                Projectile.oldRot[i] = Projectile.velocity.ToRotation();
            }

            Owner = -1;
        }

        public override void AI()
        {
            if (Owner < 0) {
                Projectile.active = false;
                return;
            }
            else if (!Main.npc[(int)Owner].active || Main.npc[(int)Owner].type != ModContent.NPCType<StellarGeliath>()) {
                Projectile.active = false;
                return;
            }

            CosmosMetaball.particles.Add(Particle.Create<SmokeSplatterParticle>(particle => {
                particle.position = Projectile.Center + Projectile.velocity * 2f + Main.rand.NextVector2Circular(24, 24);
                particle.velocity = Main.rand.NextVector2Circular(5, 5) + Projectile.velocity * 0.1f;
                particle.scale = Main.rand.NextFloat(1f, 3f) * Projectile.scale;
                particle.maxTime = Main.rand.Next(10, 30);
                particle.color = Color.White;
                particle.fadeColor = Color.White;
            }));

            if (Projectile.ai[1] == 0) {
                Projectile.velocity += Main.npc[(int)Owner].GetTargetData().Velocity * 0.012f;
                Projectile.velocity.Y += 0.3f;
                Projectile.velocity.X += (Main.npc[(int)Owner].GetTargetData().Center.X - Projectile.Center.X) * 0.0002f;

                foreach (Projectile otherBit in Main.projectile.Where(n => n.active && n.type == Type && n.whoAmI != Projectile.whoAmI && n.Distance(Projectile.Center) < 30)) {
                    otherBit.velocity += otherBit.DirectionFrom(Projectile.Center).SafeNormalize(Vector2.Zero) * 0.15f;
                    Projectile.velocity += Projectile.DirectionFrom(otherBit.Center).SafeNormalize(Vector2.Zero) * 0.15f;
                }
            }
            else if (Time < 40) {
                Projectile.velocity *= 0.945f;
            }
            else {
                Projectile.scale = Utils.GetLerpValue(30, 100, Projectile.Distance(Main.npc[(int)Owner].Center), true);
                Projectile.velocity = Vector2.Lerp(Projectile.velocity, Projectile.DirectionTo(Main.npc[(int)Owner].Center).SafeNormalize(Vector2.Zero) * Projectile.Distance(Main.npc[(int)Owner].GetTargetData().Center) * 0.07f, 0.15f * Utils.GetLerpValue(50, 150, Time, true)).RotatedBy(0.015f * Projectile.direction);
            }
            if (Time > 300 || Projectile.scale < 0.1f) {
                Projectile.Kill();
            }

            if (Time > 0) {
                Projectile.ai[1] = 1;
            }

            if (Main.rand.NextBool(30)) {
                CalamityHunt.particles.Add(Particle.Create<PrettySparkle>(particle => {
                    particle.position = Projectile.Center + Main.rand.NextVector2Circular(12, 12) * Projectile.scale + Projectile.velocity;
                    particle.velocity = Main.rand.NextVector2Circular(3, 3);
                    particle.scale = Main.rand.NextFloat(0.4f, 1.4f) * Projectile.scale;
                    particle.color = new Color(30, 15, 10, 0);
                }));
            }
            Projectile.rotation += Projectile.velocity.Length() * Projectile.direction * 0.02f;

            Time++;
            Projectile.localAI[0]++;
            Projectile.localAI[0] += Projectile.velocity.Length() * 0.015f;
        }

        public static Texture2D auraTexture;

        public override void Load()
        {
            auraTexture = ModContent.Request<Texture2D>(Texture + "Aura", AssetRequestMode.ImmediateLoad).Value;
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D texture = TextureAssets.Projectile[Type].Value;
            Rectangle frame = texture.Frame(4, 1, Projectile.frame, 0);

            float scale = Projectile.localAI[1] * 1.1f;

            if (Projectile.ai[1] == 0) {
                scale = Utils.GetLerpValue(-15, 15, Projectile.localAI[0], true) * Projectile.localAI[1] * 1.1f;
            }

            Vector2 flameSquish = new Vector2(1f + (float)Math.Sin(Projectile.localAI[0] * 0.2f) * 0.1f, 1f + (float)Math.Cos(Projectile.localAI[0] * 0.2f) * 0.1f) * new Vector2(1f - Projectile.velocity.Length() * 0.005f, 0.8f + Projectile.velocity.Length() * 0.005f);
            Color innerFlameColor = Color.Lerp(new Color(20, 20, 20, 0), new Color(20, 170, 200, 0), Utils.GetLerpValue(50, 30, Time, true));

            Main.EntitySpriteDraw(auraTexture, Projectile.Center - Main.screenPosition, auraTexture.Frame(), new Color(10, 30, 110, 0) * 0.7f * Projectile.scale, Projectile.velocity.ToRotation() - MathHelper.PiOver2, auraTexture.Size() * new Vector2(0.5f, 0.8f), Projectile.scale * scale * flameSquish, 0, 0);
            Main.EntitySpriteDraw(auraTexture, Projectile.Center - Main.screenPosition, auraTexture.Frame(), innerFlameColor * 0.7f * Projectile.scale, Projectile.velocity.ToRotation() - MathHelper.PiOver2, auraTexture.Size() * new Vector2(0.5f, 0.8f), Projectile.scale * 0.66f * scale * flameSquish, 0, 0);

            Main.EntitySpriteDraw(texture, Projectile.Center - Main.screenPosition, frame, Color.White, Projectile.rotation, frame.Size() * 0.5f, Projectile.scale * scale, 0, 0);
            Main.EntitySpriteDraw(texture, Projectile.Center - Main.screenPosition, frame, new Color(70, 40, 35, 0), Projectile.rotation, frame.Size() * 0.5f, Projectile.scale * 1.3f * scale, 0, 0);

            return false;
        }
    }
}
