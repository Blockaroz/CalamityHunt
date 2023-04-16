using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using System;
using System.Collections.Generic;
using Terraria;
using Terraria.DataStructures;
using Terraria.Graphics;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityHunt.Content.Bosses.Goozma.Projectiles
{
    public class GaussRay : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.DrawScreenCheckFluff[Type] = 1000;
        }
        public override void SetDefaults()
        {
            Projectile.width = 800;
            Projectile.height = 800;
            Projectile.tileCollide = false;
            Projectile.hostile = true;
            Projectile.friendly = false;
            Projectile.penetrate = -1;
            Projectile.aiStyle = -1;
        }

        public ref float Time => ref Projectile.ai[0];

        private int ChargeTime = 300;

        public override void AI()
        {
            Projectile.localAI[0]++;
            Time++;
        }

        public override void DrawBehind(int index, List<int> behindNPCsAndTiles, List<int> behindNPCs, List<int> behindProjectiles, List<int> overPlayers, List<int> overWiresUI) => overPlayers.Add(index);

        public override bool PreDraw(ref Color lightColor)
        {
            Asset<Texture2D> texture = ModContent.Request<Texture2D>(Texture);
            Asset<Texture2D> glow = ModContent.Request<Texture2D>($"{nameof(CalamityHunt)}/Assets/Textures/Goozma/GlowSoft");

            Color startColor = new GradientColor(SlimeUtils.GoozColorArray, 0.2f, 0.2f).ValueAt(Projectile.localAI[0]);
            startColor.A = 0;
            ProjectileID.Sets.DrawScreenCheckFluff[Type] = 5000;

            Main.EntitySpriteDraw(glow.Value, Projectile.Center - Main.screenPosition, null, startColor, 0, glow.Size() * 0.5f, 1f, 0, 0);
            Main.EntitySpriteDraw(glow.Value, Projectile.Center - Main.screenPosition, null, new Color(255, 255, 255, 0), 0, glow.Size() * 0.5f, 0.2f, 0, 0);

            Vector2[] positions = new Vector2[600];
            float[] rotations = new float[600];
            for (int i = 0; i < 600; i++)
            {
                positions[i] = Projectile.Center + new Vector2(5 * i, 0).RotatedBy(Projectile.rotation);
                rotations[i] = Projectile.rotation;
            }

            VertexStrip strip = new VertexStrip();
            strip.PrepareStripWithProceduralPadding(positions, rotations, StripColor, StripWidth, -Main.screenPosition, true);

            Effect lightningEffect = ModContent.Request<Effect>($"{nameof(CalamityHunt)}/Assets/Effects/GooLightningEffect", AssetRequestMode.ImmediateLoad).Value;
            lightningEffect.Parameters["uTransformMatrix"].SetValue(Main.GameViewMatrix.NormalizedTransformationmatrix);
            lightningEffect.Parameters["uTexture"].SetValue(texture.Value);
            lightningEffect.Parameters["uGlow"].SetValue(ModContent.Request<Texture2D>($"{nameof(CalamityHunt)}/Assets/Textures/Goozma/Lightning").Value);
            lightningEffect.Parameters["uColor"].SetValue(Vector3.One);
            lightningEffect.Parameters["uTime"].SetValue(Projectile.timeLeft * 0.02f + 0.2f);
            lightningEffect.CurrentTechnique.Passes[0].Apply();

            strip.DrawTrail();

            Main.pixelShader.CurrentTechnique.Passes[0].Apply();

            return false;
        }

        public Color StripColor(float progress) => new GradientColor(SlimeUtils.GoozColorArray, 0.2f, 0.2f).ValueAt(Projectile.localAI[0] + progress * 60f);
        public float StripWidth(float progress) => (float)Math.Sqrt(Utils.GetLerpValue(1.1f, 0.9f, progress, true)) * (float)Math.Sqrt(progress) * 500;
    }
}
