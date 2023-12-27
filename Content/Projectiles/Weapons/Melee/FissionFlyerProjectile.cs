using System;
using CalamityHunt.Common.Utilities;
using CalamityHunt.Content.Buffs;
using CalamityHunt.Content.NPCs.Bosses.GoozmaBoss;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.GameContent;
using Terraria.ModLoader;

namespace CalamityHunt.Content.Projectiles.Weapons.Melee
{
    public class FissionFlyerProjectile : ModProjectile
    {
        public override void SetDefaults()
        {
            Projectile.width = 36;
            Projectile.height = 36;
            Projectile.tileCollide = true;
            Projectile.aiStyle = -1;
            Projectile.penetrate = -1;
            Projectile.friendly = true;
            Projectile.hostile = false;
            Projectile.timeLeft = 10000;
            Projectile.DamageType = DamageClass.Throwing;
            if (ModLoader.HasMod(HUtils.CalamityMod)) {
                DamageClass d;
                Mod calamity = ModLoader.GetMod(HUtils.CalamityMod);
                calamity.TryFind("RogueDamageClass", out d);
                Projectile.DamageType = d;
            }
        }

        public ref float Time => ref Projectile.ai[0];

        public override void AI()
        {
            Color glowColor = new GradientColor(SlimeUtils.GoozOilColors, 0.2f, 0.2f).ValueAt(Projectile.localAI[0]);

            Time++;
            Projectile.localAI[0] = Main.GlobalTimeWrappedHourly * 40f + Time;
        }

        public override bool OnTileCollide(Vector2 oldVelocity)
        {
            if (MathF.Abs(oldVelocity.X - Projectile.velocity.X) > 0) {
                Projectile.velocity.X = -oldVelocity.X * 1.33f;
            }

            if (MathF.Abs(oldVelocity.Y - Projectile.velocity.Y) > 0) {
                Projectile.velocity.Y = -oldVelocity.Y * 1.33f;
            }

            return false;
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            target.AddBuff(ModContent.BuffType<FusionBurn>(), 180);
        }

        public override void Load()
        {
            rodTexture = ModContent.Request<Texture2D>(Texture + "Rod", ReLogic.Content.AssetRequestMode.ImmediateLoad).Value;
        }

        public static Texture2D rodTexture;

        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D texture = TextureAssets.Projectile[Type].Value;
            Texture2D glow = AssetDirectory.Textures.Glow[0].Value;

            SpriteEffects spriteEffects = Projectile.direction > 0 ? SpriteEffects.None : SpriteEffects.FlipHorizontally;

            Main.EntitySpriteDraw(texture, Projectile.Center - Main.screenPosition, texture.Frame(), Color.White, Projectile.rotation, texture.Size() * 0.5f, Projectile.scale, 0, 0);

            return false;
        }
    }
}
