using Microsoft.Xna.Framework;
using System;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria;
using Terraria.ModLoader;
using System.Linq;
using CalamityHunt.Common.Systems;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using Terraria.Graphics.CameraModifiers;

namespace CalamityHunt.Content.Projectiles.Weapons.Ranged
{
    public class GoozmortarHeld : ModProjectile
    {
        public override void SetDefaults()
        {
            Projectile.width = 32;
            Projectile.height = 32;
            Projectile.hostile = false;
            Projectile.friendly = true;
            Projectile.ignoreWater = true;
            Projectile.tileCollide = false;
            Projectile.penetrate = -1;
            Projectile.DamageType = DamageClass.Ranged;
            Projectile.hide = true;
        }

        public ref float Time => ref Projectile.ai[0];

        public ref Player Player => ref Main.player[Projectile.owner];

        private float recoil;

        public override void AI()
        {
            Player.SetDummyItemTime(15);
            Player.direction = Projectile.velocity.X >= 0 ? 1 : -1;
            Player.heldProj = Projectile.whoAmI;
            Projectile.timeLeft = 5;

            float armRot = Projectile.velocity.ToRotation() * Player.gravDir - MathHelper.PiOver2 - recoil * 0.6f * Player.direction;
            float armPos = Projectile.velocity.ToRotation() - MathHelper.PiOver2 - recoil * 0.6f * Player.direction * Player.gravDir;

            int additionalWait = (int)(Utils.GetLerpValue(Player.GetModPlayer<GoozmaWeaponsPlayer>().mortarMaxUses * 0.7f, Player.GetModPlayer<GoozmaWeaponsPlayer>().mortarMaxUses, Player.GetModPlayer<GoozmaWeaponsPlayer>().mortarUses, true) * Player.itemAnimationMax * 1.7f);
            float projSpeed = 0.7f + Utils.GetLerpValue(Player.GetModPlayer<GoozmaWeaponsPlayer>().mortarMaxUses, Player.GetModPlayer<GoozmaWeaponsPlayer>().mortarMaxUses * 0.7f, Player.GetModPlayer<GoozmaWeaponsPlayer>().mortarUses, true) * 0.4f;
            bool clogged = Player.GetModPlayer<GoozmaWeaponsPlayer>().mortarUses >= Player.GetModPlayer<GoozmaWeaponsPlayer>().mortarMaxUses;

            if (Player.dead || !Player.active || Player.CCed)
                Projectile.active = false;

            switch (Projectile.ai[1])
            {
                case 0:

                    if (Time == 4)
                    {
                        Player.GetModPlayer<GoozmaWeaponsPlayer>().mortarUses++;

                        if (Player.GetModPlayer<GoozmaWeaponsPlayer>().mortarUses == Player.GetModPlayer<GoozmaWeaponsPlayer>().mortarMaxUses)
                        {
                            SoundStyle clogsound = SoundID.GuitarG.WithPitchOffset(0.1f);
                            clogsound.MaxInstances = 0;
                            SoundEngine.PlaySound(clogsound, Projectile.Center);
                        }

                        SoundStyle shootSound = SoundID.DeerclopsIceAttack.WithPitchOffset(0.1f);
                        shootSound.MaxInstances = 0;
                        SoundEngine.PlaySound(shootSound, Projectile.Center);
                    }

                    if (Time < 7)
                        recoil += 0.04f;

                    if (Time < 9 && Time > 4)
                    {
                        Projectile.velocity = Player.DirectionTo(Main.MouseWorld).SafeNormalize(Vector2.Zero).RotatedByRandom(0.1f) * 2f;
                        Projectile.direction = Player.direction;

                        for (int i = 0; i < Main.rand.Next(1, 3); i++)
                        {
                            IEntitySource source = Player.GetSource_ItemUse(Player.HeldItem);
                            Vector2 bulletVelocity = Projectile.velocity.SafeNormalize(Vector2.Zero) * Main.rand.NextFloat(15, 20);
                            int bulletType = ModContent.ProjectileType<GoozmortarMinishot>();
                            Projectile.NewProjectileDirect(source, Projectile.Center, bulletVelocity * projSpeed, bulletType, (int)(Player.HeldItem.damage * 0.8f), 1f, Projectile.owner);
                        }
                        if (Time < 7)
                            recoil += 0.6f;
                        Main.instance.CameraModifiers.Add(new PunchCameraModifier(Projectile.Center, -Projectile.velocity.SafeNormalize(Vector2.Zero).RotatedByRandom(0.3f), 2f, 11, 20));
                    }

                    if (Time > Player.itemAnimationMax + 7 + additionalWait)
                    {
                        if (Main.mouseRight && clogged)
                        {
                            Time = 0;
                            Projectile.ai[1] = 1;
                        }

                        if (Player.channel && Player.altFunctionUse == 0)
                            Time = 0;

                        if (Time > Player.itemAnimationMax * 2 + 10 + additionalWait)
                        {
                            Projectile.Kill();
                            Player.itemAnimation = 15;
                        }
                    }

                    break;

                case 1:

                    if (Time == 2)
                    {
                        Projectile.velocity = Player.DirectionTo(Main.MouseWorld).SafeNormalize(Vector2.Zero).RotatedByRandom(0.1f) * 2f;
                        Projectile.direction = Player.direction;

                        IEntitySource source = Player.GetSource_ItemUse(Player.HeldItem);
                        Vector2 lobVelocity = Projectile.velocity.SafeNormalize(Vector2.Zero) * 12f;
                        int bulletType = ModContent.ProjectileType<GoozmortarLob>();
                        Projectile.NewProjectileDirect(source, Projectile.Center, lobVelocity, bulletType, Player.HeldItem.damage * 3, 1f, Projectile.owner);

                        Player.GetModPlayer<GoozmaWeaponsPlayer>().mortarUses = 0;

                        SoundStyle shootSound = SoundID.Item38.WithPitchOffset(0.1f);
                        shootSound.MaxInstances = 0;
                        SoundEngine.PlaySound(shootSound, Projectile.Center);
                    }

                    if (Time < 5 && Time > 2)
                    {
                        recoil += 0.6f;
                        Main.instance.CameraModifiers.Add(new PunchCameraModifier(Projectile.Center, -Projectile.velocity.SafeNormalize(Vector2.Zero).RotatedByRandom(0.3f), 3.5f, 12, 25));
                    }

                    if (Time > 5)
                    {
                        if (Player.channel)
                        {
                            Time = 0;
                            Projectile.ai[1] = 0;
                        }

                        if (Time > 60)
                        {
                            Projectile.Kill();
                            Player.itemAnimation = 15;
                        }
                    }

                    break;
            }

            recoil = MathHelper.Lerp(recoil, 0, 0.24f);

            Player.SetCompositeArmFront(true, Player.CompositeArmStretchAmount.Full, armRot);
            Projectile.Center = Player.RotatedRelativePoint(Player.GetFrontHandPosition(Player.CompositeArmStretchAmount.Full, armPos));
            Projectile.rotation = Projectile.velocity.ToRotation() - recoil * Projectile.direction * (int)Player.gravDir;


            Time++;
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Asset<Texture2D> texture = ModContent.Request<Texture2D>(Texture);
            //Asset<Texture2D> glow = ModContent.Request<Texture2D>(Texture + "_Glow");
            SpriteEffects direction = Projectile.direction * (int)Player.gravDir > 0 ? SpriteEffects.None : SpriteEffects.FlipVertically;
            Vector2 origin = texture.Size() * new Vector2(0.35f, 0.5f + 0.25f * Projectile.direction * (int)Player.gravDir);

            Main.EntitySpriteDraw(texture.Value, Projectile.Center - Main.screenPosition, null, lightColor, Projectile.rotation, origin, Projectile.scale, direction, 0);
            //Main.EntitySpriteDraw(glow.Value, Projectile.Center - Main.screenPosition, null, Color.White, Projectile.rotation, origin, Projectile.scale, direction, 0);

            return false;
        }
    }
}
