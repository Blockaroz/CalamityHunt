using CalamityHunt.Common.Systems.Particles;
using CalamityHunt.Content.Bosses.Goozma;
using CalamityHunt.Content.Buffs;
using CalamityHunt.Content.Particles;
using CalamityHunt.Content.Projectiles;
using Microsoft.Xna.Framework;
using ReLogic.Utilities;
using System.Linq;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.Localization;
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

        private bool t25, t50, t75;
        private bool playFull;
        private float gooTime;
        private int delay;
        private int wait;
        SlotId loopSlot;

        public override void ResetEffects()
        {
            rainbow = false;
            active = false;
            wait = 0;
            delay = 0;
            if (!stressedOut)
            {
                checkStress = stress;
                if (SoundEngine.TryGetActiveSound(loopSlot, out var activeSound))
                    activeSound.Stop();
            }
            if (Player.controlDown && Player.releaseDown && Player.doubleTapCardinalTimer[0] < 15 && stress >= 0.25f && !stressedOut)
            {
                SoundEngine.PlaySound(new SoundStyle($"{nameof(CalamityHunt)}/Assets/Sounds/GoozmaRageActivate"), Player.Center);
                stressedOut = true;
                Player.AddBuff(ModContent.BuffType<SplendorJamBuff>(), (int)(checkStress * 16 + 4) * 60);
            }
        }
        public override void DrawEffects(PlayerDrawSet drawInfo, ref float r, ref float g, ref float b, ref float a, ref bool fullBright)
        {
            if (rainbow)
            {
                Color goo = new GradientColor(SlimeUtils.GoozColors, 0.2f, 0.2f).ValueAt(gooTime * 0.05f);
                if (drawInfo.shadow != 0)
                {
                    r = goo.R;
                    g = goo.G;
                    b = goo.B;
                }
                if (drawInfo.shadow == 0 && Main.rand.NextBool(8) && stressedOut)
                    Particle.NewParticle(Particle.ParticleType<HueLightDust>(), Player.Center + Main.rand.NextVector2Circular(30, 40), Main.rand.NextVector2Circular(1, 1) - Vector2.UnitY * 3f, goo, 1f);
            }
        }
        public override void FrameEffects()
        {
            if (rainbow)
                Player.armorEffectDrawShadow = true;
        }
        public override void UpdateEquips()
        {
            if (rainbow || active)
                gooTime = (gooTime + 1) % 60;
            if (active)
            {
                Color goo = new GradientColor(SlimeUtils.GoozColors, 0.2f, 0.2f).ValueAt(gooTime * 0.05f);
                Lighting.AddLight(Player.Center, new Vector3(goo.R, goo.G, goo.B) * 0.01f * checkStress);
                if (Player.ownedProjectileCounts[ModContent.ProjectileType<SplendorTentacle>()] < Player.GetModPlayer<SplendorJamPlayer>().tentacleCount && Player.whoAmI == Main.myPlayer)
                    Projectile.NewProjectile(Player.GetSource_FromThis(), Player.MountedCenter, Vector2.Zero, ModContent.ProjectileType<SplendorTentacle>(), 200, 0.5f, Player.whoAmI);

                if (Player.ownedProjectileCounts[ModContent.ProjectileType<SplendorTentacle>()] > Player.GetModPlayer<SplendorJamPlayer>().tentacleCount)
                    Main.projectile.First(n => n.active && n.owner == Player.whoAmI).Kill();

                tentacleCount = stressedOut ? tentacleCount : (int)(1 + stress * 4f);
                if (stress < 1f && !stressedOut)
                    stress += 0.0001f;
                else if (stress > 1f && !stressedOut)
                    stress = 1f;
                if (stress > 0.25f && !t25)
                {
                    SoundEngine.PlaySound(new SoundStyle($"{nameof(CalamityHunt)}/Assets/Sounds/GoozmaRageIndicator") with { Pitch = -0.4f }, Player.Center);
                    t25 = true;
                }
                else if (stress > 0.5f && !t50)
                {
                    SoundEngine.PlaySound(new SoundStyle($"{nameof(CalamityHunt)}/Assets/Sounds/GoozmaRageIndicator") with { Pitch = -0.2f }, Player.Center);
                    t50 = true;
                }
                if (stress > 0.755f && !t75)
                {
                    SoundEngine.PlaySound(new SoundStyle($"{nameof(CalamityHunt)}/Assets/Sounds/GoozmaRageIndicator"), Player.Center);
                    t75 = true;
                }
                if (stressedOut)
                {
                    stress -= checkStress / ((int)(checkStress * 16 + 4) * 60);
                    if (!SoundEngine.TryGetActiveSound(loopSlot, out var activeSound))
                        loopSlot = SoundEngine.PlaySound(new SoundStyle($"{nameof(CalamityHunt)}/Assets/Sounds/GoozmaRageLoop"), Player.Center);
                    else
                        activeSound.Position = Player.Center;
                }
                if (stressedOut && stress <= 0)
                {
                    t25 = false;
                    t50 = false;
                    t75 = false;
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
                Player.ClearBuff(ModContent.BuffType<SplendorJamBuff>());
                t25 = false;
                t50 = false;
                t75 = false;
                stressedOut = false;
                stress = 0;
                checkStress = 0;
            }
        }
        public override void PostUpdateMiscEffects()
        {
            if (Player.whoAmI == Main.myPlayer && playFull && checkStress >= 1f)
            {
                playFull = false;
                SoundEngine.PlaySound(new SoundStyle($"{nameof(CalamityHunt)}/Assets/Sounds/GoozmaRageFull"), Player.Center);
            }
            else if (!playFull && checkStress < 1f)
                playFull = true;
        }
        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            if (active && Player.whoAmI == Main.myPlayer && Player.HeldItem.type != ItemID.Zenith)
            {
                if (stress < 1f && delay <= 0 && !stressedOut && target.type != NPCID.TargetDummy)
                {
                    float dis = Player.Distance(target.Center);
                    stress += 0.0005f + ((dis > 0 && dis <= 320) ? 0.0005f : 0.0005f - (dis/320 * 0.0005f));
                    delay = 5;
                }
                if (Player.ownedProjectileCounts[ModContent.ProjectileType<StressExplosion>()] < 1 && wait == 0 && checkStress >= 0.25f && checkStress < 0.75)
                {
                    Projectile.NewProjectile(Player.GetSource_FromThis(), target.Center, Vector2.Zero, ModContent.ProjectileType<StressExplosion>(), (int)(damageDone * 0.2f), 0, Player.whoAmI, 0, 0);
                    wait = 60;
                }
                else if (Player.ownedProjectileCounts[ModContent.ProjectileType<StressExplosion>()] < 1 && wait == 0 && checkStress >= 0.75f)
                {
                    Projectile.NewProjectile(Player.GetSource_FromThis(), target.Center, Vector2.Zero, ModContent.ProjectileType<StressExplosion>(), (int)(damageDone * 0.4f), 0, Player.whoAmI, 0, 1);
                    wait = 60;
                }
            }
        }
        public override void OnHitNPCWithProj(Projectile proj, NPC target, NPC.HitInfo hit, int damageDone)
        {
            if (active)
            {
                if (proj.type != ModContent.ProjectileType<SplendorTentacle>() && Player.whoAmI == Main.myPlayer)
                {
                    if (stress < 1f && delay <= 0 && !stressedOut && target.type != NPCID.TargetDummy)
                    {
                        float dis = Player.Distance(target.Center);
                        stress += 0.0005f + ((dis > 0 && dis <= 320) ? 0.0005f : 0.0005f - (dis / 320 * 0.0005f));
                        delay = 5;
                    }
                    if (Player.ownedProjectileCounts[ModContent.ProjectileType<StressExplosion>()] < 1 && wait == 0 && checkStress >= 0.25f && checkStress < 0.75)
                    {
                        Projectile.NewProjectile(proj.GetSource_FromThis(), target.Center, Vector2.Zero, ModContent.ProjectileType<StressExplosion>(), (int)(damageDone * 0.2f), 0, Player.whoAmI, 0, 0);
                        wait = 60;
                    }
                    else if (Player.ownedProjectileCounts[ModContent.ProjectileType<StressExplosion>()] < 1 && wait == 0 && stress >= 0.75f)
                    {
                        Projectile.NewProjectile(proj.GetSource_FromThis(), target.Center, Vector2.Zero, ModContent.ProjectileType<StressExplosion>(), (int)(damageDone * 0.4f), 0, Player.whoAmI, 0, 1);
                        wait = 60;
                    }
                }
            }
        }
    }
}
