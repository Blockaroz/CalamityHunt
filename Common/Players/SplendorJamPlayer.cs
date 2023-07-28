using CalamityHunt.Common.Systems.Particles;
using CalamityHunt.Content.Bosses.Goozma;
using CalamityHunt.Content.Buffs;
using CalamityHunt.Content.Particles;
using CalamityHunt.Content.Projectiles;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityHunt.Common.Players
{
    public class SplendorJamPlayer : ModPlayer
    {
        public int tentacleCount;

        public bool active;
        public bool rainbow;
        public bool stressedOut;
        public float stress;
        public float checkStress;

        private float gooTime;
        private int delay;
        private int wait;
        private readonly int[] slimes =
{
            NPCID.GreenSlime,
            NPCID.BlueSlime,
            NPCID.PurpleSlime,
            NPCID.Pinky,
            NPCID.RedSlime,
            NPCID.YellowSlime,
            NPCID.BlackSlime,
            NPCID.BabySlime,
            NPCID.MotherSlime,
            NPCID.LavaSlime,
            NPCID.SandSlime,
            NPCID.IceSlime,
            NPCID.SpikedIceSlime,
            NPCID.JungleSlime,
            NPCID.SpikedJungleSlime,
            NPCID.DungeonSlime,
            NPCID.GoldenSlime,
            NPCID.ShimmerSlime,
            NPCID.WindyBalloon,
            NPCID.UmbrellaSlime,
            NPCID.SlimeMasked,
            NPCID.SlimeRibbonGreen,
            NPCID.SlimeRibbonRed,
            NPCID.SlimeRibbonWhite,
            NPCID.SlimeRibbonYellow,
            NPCID.ToxicSludge,
            NPCID.CorruptSlime,
            NPCID.Slimeling,
            NPCID.Slimer,
            NPCID.Slimer2,
            NPCID.Crimslime,
            NPCID.Gastropod,
            NPCID.IlluminantSlime,
            NPCID.RainbowSlime

        };
        public override void ResetEffects()
        {
            rainbow = false;
            active = false;
            wait = 0;
            delay = 0;
            if (!stressedOut)
            {
                checkStress = stress;
            }
            if (Player.controlDown && Player.releaseDown && Player.doubleTapCardinalTimer[0] < 15 && stress >= 0.25f && !stressedOut)
            {
                stressedOut = true;
            }
        }
        public override void DrawEffects(PlayerDrawSet drawInfo, ref float r, ref float g, ref float b, ref float a, ref bool fullBright)
        {
            if (rainbow)
            {
                gooTime = (gooTime + 1) % 60;

                if (drawInfo.shadow != 0)
                {
                    Color goo = new GradientColor(SlimeUtils.GoozColors, 0.2f, 0.2f).ValueAt(gooTime * 0.05f);
                    if (Main.rand.NextBool(8) && stressedOut)
                    {
                        Particle.NewParticle(Particle.ParticleType<HueLightDust>(), Player.Center + Main.rand.NextVector2Circular(30, 40), Main.rand.NextVector2Circular(1, 1) - Vector2.UnitY * 3f, goo, 1f);
                    }
                    r = goo.R;
                    g = goo.G;
                    b = goo.B;
                }
            }
        }
        public override void FrameEffects()
        {
            if (rainbow)
                Player.armorEffectDrawShadow = true;
        }
        public override void UpdateEquips()
        {
            if (active)
            {
                tentacleCount = stressedOut ? tentacleCount : (int)(1 + stress * 4f);

                if (!Player.HasBuff<SplendorJamBuff>())
                    Player.AddBuff(ModContent.BuffType<SplendorJamBuff>(), 5, false);

                for (int i = 0; i < slimes.Length; i++)
                {
                    //Player.npcTypeNoAggro[slimes[i]] = true;
                }
                if (stress < 1f && !stressedOut)
                    stress += 0.0001f;
                if (stressedOut)
                {
                    stress -= 0.001f;
                    if (checkStress >= 0.5 && checkStress < 0.75)
                    {
                        Player.GetDamage(DamageClass.Generic) += 0.7f;
                        Player.GetDamage(DamageClass.Generic).Flat += 12f;
                        Player.GetCritChance(DamageClass.Generic) += 3f;
                        Player.GetArmorPenetration(DamageClass.Generic) += 10f;
                    }
                    else if (checkStress >= 0.75 && checkStress < 1)
                    {
                        Player.GetDamage(DamageClass.Generic) += 0.16f;
                        Player.GetDamage(DamageClass.Generic).Flat += 16f;
                        Player.GetCritChance(DamageClass.Generic) += 7f;
                        Player.GetArmorPenetration(DamageClass.Generic) += 20f;

                    }
                    else if (checkStress >= 1)
                    {
                        Player.GetDamage(DamageClass.Generic) += 0.35f;
                        Player.GetDamage(DamageClass.Generic).Flat += 20f;
                        Player.GetCritChance(DamageClass.Generic) += 10f;
                        Player.GetArmorPenetration(DamageClass.Generic) += 40f;
                    }
                    else
                    {
                        Player.GetDamage(DamageClass.Generic) += 0.02f;
                        Player.GetDamage(DamageClass.Generic).Flat += 8f;
                        Player.GetCritChance(DamageClass.Generic) += 1f;
                        Player.GetArmorPenetration(DamageClass.Generic) += 5f;
                    }
                }
                if (stressedOut && stress <= 0)
                {
                    stressedOut = false;
                    stress = 0;
                    checkStress = 0;
                }
                if (wait > 0)
                    wait--;
                if (delay > 0)
                    delay--;
            }
            else
            {
                stressedOut = false;
                stress = 0;
                checkStress = 0;
            }
        }
        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            if (active)
            {
                if (stress < 1f && delay <= 0 && !stressedOut)
                {
                    stress += 0.001f;
                    delay = 5;
                }
                if (Player.ownedProjectileCounts[ModContent.ProjectileType<StressExplosion>()] < 1 && wait == 0 && checkStress >= 0.25f && checkStress < 0.75)
                {
                    Projectile.NewProjectile(Terraria.Entity.GetSource_None(), target.Center, Vector2.Zero, ModContent.ProjectileType<StressExplosion>(), (int)(damageDone * 0.2f), 0, Main.myPlayer, 0, 0);
                    wait = 60;
                }
                else if (Player.ownedProjectileCounts[ModContent.ProjectileType<StressExplosion>()] < 1 && wait == 0 && checkStress >= 0.75f)
                {
                    Projectile.NewProjectile(Terraria.Entity.GetSource_None(), target.Center, Vector2.Zero, ModContent.ProjectileType<StressExplosion>(), (int)(damageDone * 0.4f), 0, Main.myPlayer, 0, 1);
                    wait = 60;
                }
            }
        }
        public override void OnHitNPCWithProj(Projectile proj, NPC target, NPC.HitInfo hit, int damageDone)
        {
            if (active)
            {
                if (proj.type != ModContent.ProjectileType<StressExplosion>())
                {
                    if (stress < 1f && delay <= 0 && !stressedOut)
                    {
                        stress += 0.001f;
                        delay = 5;
                    }
                    if (Player.ownedProjectileCounts[ModContent.ProjectileType<StressExplosion>()] < 1 && wait == 0 && checkStress >= 0.25f && checkStress < 0.75)
                    {
                        Projectile.NewProjectile(Terraria.Entity.GetSource_None(), target.Center, Vector2.Zero, ModContent.ProjectileType<StressExplosion>(), (int)(damageDone * 0.2f), 0, Main.myPlayer, 0, 0);
                        wait = 60;
                    }
                    else if (Player.ownedProjectileCounts[ModContent.ProjectileType<StressExplosion>()] < 1 && wait == 0 && stress >= 0.75f)
                    {
                        Projectile.NewProjectile(Terraria.Entity.GetSource_None(), target.Center, Vector2.Zero, ModContent.ProjectileType<StressExplosion>(), (int)(damageDone * 0.4f), 0, Main.myPlayer, 0, 1);
                        wait = 60;
                    }
                }
            }
        }
    }
}
