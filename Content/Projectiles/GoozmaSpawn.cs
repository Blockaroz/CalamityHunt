using CalamityHunt.Common.Graphics.SlimeMonsoon;
using CalamityHunt.Common.Systems.Particles;
using CalamityHunt.Content.Bosses.Goozma;
using CalamityHunt.Content.Particles;
using CalamityHunt.Content.Particles.FlyingSlimes;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using System;
using System.Linq;
using Terraria;
using Terraria.Audio;
using Terraria.Chat;
using Terraria.GameContent;
using Terraria.Graphics.CameraModifiers;
using Terraria.Graphics.Renderers;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;
using Terraria.Utilities;
using static Terraria.ModLoader.PlayerDrawLayer;

namespace CalamityHunt.Content.Projectiles
{
    public class GoozmaSpawn : ModProjectile
    {
        public static readonly SoundStyle slimeabsorb = new SoundStyle($"{nameof(CalamityHunt)}/Assets/Sounds/Goozma/GoozmaSlimeAbsorb", 8) with { MaxInstances = 100, Volume = 0.1f};
        public WeightedRandom<int> randomType = new WeightedRandom<int>();
        public override void SetDefaults()
        {
            Projectile.width = 92;
            Projectile.height = 88;
            Projectile.tileCollide = false;
            Projectile.hostile = true;
            Projectile.friendly = false;
            Projectile.penetrate = -1;
            Projectile.aiStyle = -1;
            Projectile.damage = 0;
            slimeMonsoonText = Language.GetOrRegister($"Mods.{nameof(CalamityHunt)}.Chat.SlimeMonsoon");
        }

        public LocalizedText slimeMonsoonText;

        public ref float Time => ref Projectile.ai[0];

        public override void AI()
        {
            if (Time < 0)
                Time = 0;

            Projectile.damage = 0;
            Projectile.velocity.X = 0;
            Projectile.velocity.Y = Utils.GetLerpValue(500, 0, Time, true) * -0.1f;

            if (Main.slimeRain)
                Main.StopSlimeRain(true);

            SlimeMonsoonBackground.strengthTarget = Utils.GetLerpValue(120, 900, Time, true);

            if (Time > 800)
            {
                for (int i = 0; i < (int)((Time - 400) / 800f) + 1; i++)
                {
                    Vector2 pos = Projectile.Center + Main.rand.NextVector2Circular(200, 150) + Main.rand.NextVector2Circular(40, 40) * ((Time - 400) / 500f);
                    Vector2 vel = pos.DirectionTo(Projectile.Center).SafeNormalize(Vector2.Zero) * pos.Distance(Projectile.Center) * 0.1f * ((Time - 400) / 500f);

                    Particle hue = Particle.NewParticle(Particle.ParticleType<HueLightDust>(), pos, vel, Color.White, 2f);
                    hue.data = Time * 0.33f;
                }

                Projectile.position.Y -= 0.5f;
            }

            for (int i = 0; i < (int)(Time / 1000f) + 1; i++)
            {
                Vector2 pos = Projectile.Center + Main.rand.NextVector2CircularEdge(200, 150) + Main.rand.NextVector2Circular(40, 40);

                Particle hue = Particle.NewParticle(Particle.ParticleType<HueLightDust>(), pos, pos.DirectionTo(Projectile.Center) * Main.rand.NextFloat(10f, 15f), Color.White, 1f);
                hue.data = Time * 0.33f;
            }

            if (Time == 0)
            {
                SoundStyle devour = new SoundStyle($"{nameof(CalamityHunt)}/Assets/Sounds/Goozma/GoozmaIntro");
                devour.MaxInstances = 0;
                devour.PlayOnlyIfFocused = true;
                //SoundEngine.PlaySound(devour, Projectile.Center);
            }

            if (((Time < 500 && Main.rand.NextBool(30)) || (!Main.rand.NextBool((int)Time + 1))) && Time < 750)
                for (int i = 0; i < 1 + (int)(Utils.GetLerpValue(400, 900, Time, true) * 10); i++)
                    SpawnSlimes();

            if (Time == 650)
            {
                SoundStyle intro = new SoundStyle($"{nameof(CalamityHunt)}/Assets/Sounds/Goozma/GoozmaIntro");
                intro.MaxInstances = 0;
                intro.PlayOnlyIfFocused = true;
                SoundEngine.PlaySound(intro, Projectile.Center);
            }  
           
            if (Time == 720)
            {
                if (Main.dedServ)
                    ChatHelper.BroadcastChatMessage(NetworkText.FromKey(slimeMonsoonText.Value), new Color(50, 255, 130));
                else
                    Main.NewText(NetworkText.FromKey(slimeMonsoonText.Value), new Color(50, 255, 130));
            }

            //if (Time > 50 && Time % 20 == 0)
            //{
            //    Particle crack = Particle.NewParticle(Particle.ParticleType<CrackSpot>(), Projectile.Center, Vector2.Zero, Color.DimGray, 5f + Time / 20f);
            //    crack.data = "GoozmaBlack";
            //}

            if (Time % 4 == 0 && Time > 600)
                Main.instance.CameraModifiers.Add(new PunchCameraModifier(Projectile.Center, Main.rand.NextVector2CircularEdge(1, 1), (float)Math.Pow((Time - 700) / 600f, 3) * 10f, 4f, 40, 20000));

            if (Time == 900)
            {
                Particle finalSlime = Particle.NewParticle(Particle.ParticleType<FlyingRainbowSlime>(), Projectile.Center - Vector2.UnitY * 700, Vector2.Zero, Color.White, 1f);
                finalSlime.data = Projectile.Center;
            }

            if (Time > 1060)
            {
                NPC.SpawnBoss((int)Projectile.Center.X, (int)Projectile.Bottom.Y, ModContent.NPCType<Goozma>(), 0);

                for (int i = 0; i < 200; i++)
                {
                    Dust.NewDustPerfect(Projectile.Center, DustID.TintableDust, Main.rand.NextVector2Circular(20f, 17f), 100, Color.Black, 3f + Main.rand.NextFloat()).noGravity = true;
                    if (Main.rand.NextBool())
                    {
                        Particle hue = Particle.NewParticle(Particle.ParticleType<HueLightDust>(), Projectile.Center, Main.rand.NextVector2Circular(18f, 15f), Color.White, 2f);
                        hue.data = Time * 0.33f;
                    }
                }

                Projectile.Kill();
            }

            Projectile.direction = 1;
            Time++;
        }

        public override void Load()
        {
            On_Main.UpdateAudio += FadeMusicOut;

            creatureTexture = ModContent.Request<Texture2D>(Texture + "Creature", AssetRequestMode.ImmediateLoad).Value;
        }

        public override void Unload()
        {
            On_Main.UpdateAudio -= FadeMusicOut;
        }

        private void FadeMusicOut(On_Main.orig_UpdateAudio orig, Main self)
        {
            orig(self);

            if (Main.projectile.Any(n => n.active && n.type == Type))
            {
                Projectile goozma = Main.projectile.FirstOrDefault(n => n.active && n.type == Type);
                for (int i = 0; i < Main.musicFade.Length; i++)
                {
                    float volume = Main.musicFade[i] * Main.musicVolume * Utils.GetLerpValue(600, 100, goozma.ai[0], true);
                    float tempFade = Main.musicFade[i];
                    Main.audioSystem.UpdateCommonTrackTowardStopping(i, volume, ref tempFade, Main.musicFade[i] > 0.1f && goozma.ai[0] < 600);
                    Main.musicFade[i] = tempFade;
                }
            }
        }

        public void SpawnSlimes()
        {
            Vector2 position = Projectile.Center + Main.rand.NextVector2CircularEdge(1100, 1100) + Main.rand.NextVector2Circular(600, 600);
            Vector2 velocity = position.DirectionTo(Projectile.Center).SafeNormalize(Vector2.Zero).RotatedByRandom(3f);

            randomType.Add(Particle.ParticleType<FlyingNormalSlime>(), 1f / 50f);
            randomType.Add(Particle.ParticleType<FlyingBigSlime>(), 1f / 100f); // this looks bad
            randomType.Add(Particle.ParticleType<FlyingBalloonSlime>(), 1f / 500f);
            randomType.Add(Particle.ParticleType<FlyingGastropod>(), 1f / 800f);
            randomType.Add(Particle.ParticleType<FlyingIlluminantSlime>(), 1f / 800f);
            randomType.Add(Particle.ParticleType<FlyingLavaSlime>(), 1f / 700f);
            randomType.Add(Particle.ParticleType<FlyingZombieSlime>(), 1f / 700f);
            randomType.Add(Particle.ParticleType<FlyingShimmerSlime>(), 1f / 700f);
            randomType.Add(Particle.ParticleType<FlyingIceSlime>(), 1f / 700f);
            randomType.Add(Particle.ParticleType<FlyingSandSlime>(), 1f / 700f);
            randomType.Add(Particle.ParticleType<FlyingJungleSlimeSpiked>(), 1f / 700f);
            randomType.Add(Particle.ParticleType<FlyingSpikedSlime>(), 1f / 700f);
            randomType.Add(Particle.ParticleType<FlyingBouncySlime>(), 1f / 800f);
            randomType.Add(Particle.ParticleType<FlyingCrystalSlime>(), 1f / 800f);
            randomType.Add(Particle.ParticleType<FlyingHeavenlySlime>(), 1f / 800f);
            randomType.Add(Particle.ParticleType<FlyingUmbrellaSlime>(), 1f / 1200f);
            randomType.Add(Particle.ParticleType<FlyingCorruptSlime>(), 1f / 500f);
            randomType.Add(Particle.ParticleType<FlyingSlimer>(), 1f / 1000f);
            randomType.Add(Particle.ParticleType<FlyingCrimslime>(), 1f / 500f);
            randomType.Add(Particle.ParticleType<FlyingToxicSludge>(), 1f / 1000f);
            randomType.Add(Particle.ParticleType<FlyingDungeonSlime>(), 1f / 1500f);
            randomType.Add(Particle.ParticleType<FlyingHoppinJack>(), Main.halloween ? (1f / 150f) : (1f / 2000f));
            randomType.Add(Particle.ParticleType<FlyingSlimeFish>(), 1f / 1000f);
            randomType.Add(Particle.ParticleType<FlyingSlimeStatue>(), 1f / 3000f);
            randomType.Add(Particle.ParticleType<FlyingFirstEncounter>(), 1f / 3000f);
            randomType.Add(Particle.ParticleType<FlyingGoldSlime>(), 1f / 5000f);
            randomType.Add(Particle.ParticleType<FlyingYuH>(), 1f / 10000f);

            if (Main.halloween)
            {
                randomType.Add(Particle.ParticleType<FlyingSlimeBunny>(), 1f / 150f);
                randomType.Add(Particle.ParticleType<FlyingBunnySlime>(), 1f / 150f);
            }
            if (Main.xMas)
                randomType.Add(Particle.ParticleType<FlyingPresentSlime>(), 1f / 150f);

            if (ModLoader.HasMod("CalamityMod"))
            {
                randomType.Add(Particle.ParticleType<FlyingAeroSlime>(), 1f / 800f);
                randomType.Add(Particle.ParticleType<FlyingEbonianBlightSlime>(), 1f / 1500f);
                randomType.Add(Particle.ParticleType<FlyingCrimulanBlightSlime>(), 1f / 1500f);
                randomType.Add(Particle.ParticleType<FlyingCorruptSlimeSpawn>(), 1f / 700f);
                randomType.Add(Particle.ParticleType<FlyingCrimsonSlimeSpawn>(), 1f / 700f);
                randomType.Add(Particle.ParticleType<FlyingAstralSlime>(), 1f / 1000f);
                randomType.Add(Particle.ParticleType<FlyingCryoSlime>(), 1f / 1000f);
                randomType.Add(Particle.ParticleType<FlyingIrradiatedSlime>(), 1f / 800f);
                randomType.Add(Particle.ParticleType<FlyingCharredSlime>(), 1f / 1000f);
                randomType.Add(Particle.ParticleType<FlyingPerennialSlime>(), 1f / 1000f);
                randomType.Add(Particle.ParticleType<AureusSpawn>(), 1f / 3000f);
                randomType.Add(Particle.ParticleType<FlyingPestilentSlime>(), 1f / 800f);
                randomType.Add(Particle.ParticleType<FlyingBloomSlime>(), 1f / 1000f);
                randomType.Add(Particle.ParticleType<FlyingGammaSlime>(), 1f / 800f);
                randomType.Add(Particle.ParticleType<FlyingCragmawMire>(), 1f / 5000f);
            }

            int type = randomType.Get();
            float scale = 1f;
            Color color = Color.White;

            Particle particle = Particle.NewParticle(randomType, position, velocity, color, scale);
            particle.data = Projectile.Center;
            particle.behindEntities = true;
        }

        public static Texture2D creatureTexture;

        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D sparkle = AssetDirectory.Textures.Sparkle;
            Texture2D glow = AssetDirectory.Textures.Glow;
            Texture2D texture = TextureAssets.Projectile[Type].Value;
            //Texture2D body = TextureAssets.Npc[ModContent.NPCType<Goozma>()].Value;
            //Texture2D dress = Goozma.dressTexture;
            //Texture2D crown = Goozma.crownTexture;
            Texture2D eye = AssetDirectory.Textures.Extras.GoozmaEye;
            Color glowColor = new GradientColor(SlimeUtils.GoozColors, 0.2f, 0.2f).Value * 1.5f;
            glowColor.A = 0;
            Vector2 drawOffset = new Vector2(14, 20).RotatedBy(Projectile.rotation) * Projectile.scale;

            Projectile.scale = Utils.GetLerpValue(190, 810, Time, true);

            //Vector2 crownPos = Projectile.Center + drawOffset - new Vector2(-6, 44).RotatedBy(Projectile.rotation) * (float)Math.Pow(Projectile.scale, 3);
            //Vector2 dressPos = Projectile.Center + drawOffset + new Vector2(4, 16).RotatedBy(Projectile.rotation) * (float)Math.Pow(Projectile.scale, 3);

            //float dressWobble = (float)Math.Sin(Time * 0.3f) * 0.05f;
            //Main.EntitySpriteDraw(dress, dressPos - Main.screenPosition, null, new Color(20, 20, 20), dressWobble, dress.Size() * new Vector2(0.5f, 0f), Projectile.scale, 0, 0);
            //Main.EntitySpriteDraw(body, Projectile.Center + drawOffset - Main.screenPosition, null, new Color(20, 20, 20), 0, body.Size() * 0.5f, Projectile.scale, 0, 0);
            //Main.EntitySpriteDraw(crown, crownPos - Main.screenPosition, null, new Color(20, 20, 20), 0, crown.Size() * new Vector2(0.5f, 1f), Projectile.scale, 0, 0);

            float fastWobble = 0.6f + (float)Math.Sin(Time * 0.7f) * 0.4f;

            Rectangle baseFrame = texture.Frame(1, 2, 0, 0);
            Rectangle glowFrame = texture.Frame(1, 2, 0, 1);
            Vector2 corePos = Projectile.Center + Main.rand.NextVector2Circular(2, 2);
            Main.EntitySpriteDraw(glow, corePos - Main.screenPosition, glow.Frame(), Color.Lerp(Color.Transparent, glowColor * 0.07f, Utils.GetLerpValue(50, 350, Time, true)), 0, glow.Size() * 0.5f, 2f + Projectile.scale * 0.7f, 0, 0);
            
            Main.EntitySpriteDraw(texture, corePos - Main.screenPosition, baseFrame, Color.Black * 0.5f, 0, baseFrame.Size() * 0.5f, 1f + Projectile.scale * 0.9f + fastWobble * 0.3f, 0, 0);
            
            Main.EntitySpriteDraw(texture, corePos - Main.screenPosition, baseFrame, Color.Lerp(lightColor, new Color(20, 20, 20, 200), Utils.GetLerpValue(250, 400, Time, true)), 0, baseFrame.Size() * 0.5f, 1f + Projectile.scale * 0.7f, 0, 0);
            Main.EntitySpriteDraw(texture, corePos - Main.screenPosition, glowFrame, Color.Lerp(Color.Transparent, glowColor, Utils.GetLerpValue(50, 250, Time, true) * Utils.GetLerpValue(750, 550, Time, true)), 0, glowFrame.Size() * 0.5f, 1f + Projectile.scale * 0.7f, 0, 0);
            Main.EntitySpriteDraw(texture, corePos - Main.screenPosition, glowFrame, Color.Lerp(Color.Transparent, glowColor, Utils.GetLerpValue(50, 300, Time, true) * Utils.GetLerpValue(750, 550, Time, true)), 0, glowFrame.Size() * 0.5f, 1.05f + Projectile.scale * 0.7f, 0, 0);

            Main.EntitySpriteDraw(creatureTexture, corePos - Main.screenPosition, creatureTexture.Frame(), Color.Black * 0.5f * Projectile.scale, 0, creatureTexture.Size() * new Vector2(0.4f, 0.4f), Projectile.scale * 0.9f + fastWobble * 0.3f, 0, 0);

            for (int i = 0; i < 6; i++)
            {
                Vector2 off = new Vector2(2, 0).RotatedBy(Time * 0.2f + MathHelper.TwoPi / 6f * i);
                Main.EntitySpriteDraw(creatureTexture, corePos + off - Main.screenPosition, creatureTexture.Frame(), glowColor * Projectile.scale, 0, creatureTexture.Size() * new Vector2(0.4f, 0.4f), Projectile.scale * 0.9f, 0, 0);
            }

            Main.EntitySpriteDraw(creatureTexture, corePos - Main.screenPosition, creatureTexture.Frame(), Color.Gray, 0, creatureTexture.Size() * new Vector2(0.4f, 0.4f), Projectile.scale * 0.9f, 0, 0);
            Main.EntitySpriteDraw(creatureTexture, corePos - Main.screenPosition, creatureTexture.Frame(), Color.Black * 0.6f, 0, creatureTexture.Size() * new Vector2(0.4f, 0.4f), Projectile.scale * 0.85f, 0, 0);

            Vector2 eyePos = Projectile.Center + drawOffset + new Vector2(-28, -20).RotatedBy(Projectile.rotation) * Projectile.scale;
            float eyeScale = (float)Math.Sqrt(Utils.GetLerpValue(840, 950, Time, true)) * 3f;
            float eyeRot = (float)Math.Cbrt(Utils.GetLerpValue(840, 1080, Time, true)) * MathHelper.PiOver2 - MathHelper.PiOver4;
            Main.EntitySpriteDraw(sparkle, eyePos - Main.screenPosition, sparkle.Frame(), glowColor * 0.1f, eyeRot + MathHelper.PiOver2, sparkle.Size() * 0.5f, eyeScale * new Vector2(0.3f, 2.4f), 0, 0);
            Main.EntitySpriteDraw(sparkle, eyePos - Main.screenPosition, sparkle.Frame(), glowColor * 0.1f, eyeRot, sparkle.Size() * 0.5f, eyeScale * new Vector2(0.3f, 2f), 0, 0);
            Main.EntitySpriteDraw(eye, eyePos - Main.screenPosition, eye.Frame(), glowColor, eyeRot, eye.Size() * 0.5f, eyeScale * 0.4f, 0, 0);
            Main.EntitySpriteDraw(eye, eyePos - Main.screenPosition, eye.Frame(), new Color(255, 255, 255, 0), eyeRot, eye.Size() * 0.5f, eyeScale * 0.4f, 0, 0);

            return false;
        }
    }
}
