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
            Projectile.velocity.Y = Utils.GetLerpValue(800, 0, Time, true) * -0.001f;

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

            if (Time % 3 == 0 && Time > 200)
                Main.instance.CameraModifiers.Add(new PunchCameraModifier(Projectile.Center, Main.rand.NextVector2CircularEdge(1, 1), Utils.GetLerpValue(200, 800, Time, true) * 7f, 6f, 10, 20000));

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

            if (Time > 1056)
            {
                for (int i = 0; i < 3; i++)
                {
                    Vector2 velocity = Vector2.UnitY.RotatedBy(MathHelper.TwoPi / 3f * i).RotatedByRandom(1f);
                    velocity.Y -= Main.rand.NextFloat();
                    Particle.NewParticle(Particle.ParticleType<GooBurst>(), Projectile.Center, velocity, new GradientColor(SlimeUtils.GoozColors, 0.2f, 0.2f).Value, 2f + Main.rand.NextFloat(2f));
                }
            }

            Projectile.direction = 1;
            Time++;
        }

        public override void Load()
        {
            On_Main.UpdateAudio += FadeMusicOut;
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
                    float volume = Main.musicFade[i] * Main.musicVolume * Utils.GetLerpValue(700, 100, goozma.ai[0], true);
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

        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D sparkle = AssetDirectory.Textures.Sparkle;
            Texture2D glow = AssetDirectory.Textures.Glow;
            Texture2D texture = TextureAssets.Projectile[Type].Value;
            Texture2D eye = AssetDirectory.Textures.Extras.GoozmaGodEye;

            Color glowColor = new GradientColor(SlimeUtils.GoozColors, 0.2f, 0.2f).Value * 1.2f;
            glowColor.A = 0;
            Vector2 drawOffset = new Vector2(14, 20).RotatedBy(Projectile.rotation) * Projectile.scale;

            int size = (int)(MathF.Pow(Utils.GetLerpValue(250, 900, Time, true), 0.7f) * 2f);
            Projectile.scale = 1f + (MathF.Round(Utils.GetLerpValue(30, 810, Time, true), 2) - MathF.Sqrt(size) * 0.5f) * 1.9f;

            float fastWobble = 0.6f + (float)Math.Sin(Time * 0.7f) * 0.4f;

            GetGradientMapValues(out float[] brightnesses, out Vector3[] colors);
            Effect effect = ModContent.Request<Effect>($"{nameof(CalamityHunt)}/Assets/Effects/HolographEffect", AssetRequestMode.ImmediateLoad).Value;
            effect.Parameters["uTime"].SetValue(Main.GlobalTimeWrappedHourly % 1f);
            effect.Parameters["colors"].SetValue(colors);
            effect.Parameters["brightnesses"].SetValue(brightnesses);
            effect.Parameters["baseToScreenPercent"].SetValue(1f);
            effect.Parameters["baseToMapPercent"].SetValue(0f);

            Vector2 drawPos = Projectile.Center + Main.rand.NextVector2Circular(2, 2);
            Rectangle frame = texture.Frame(3, 1, size, 0);
            
            Main.EntitySpriteDraw(texture, drawPos - Main.screenPosition, frame, Color.Black * 0.2f, 0, frame.Size() * 0.5f, Projectile.scale + fastWobble * 0.5f, 0, 0);

            for (int i = 0; i < 6; i++)
            {
                Vector2 off = new Vector2(2, 0).RotatedBy(Time * 0.2f + MathHelper.TwoPi / 6f * i);
                Main.EntitySpriteDraw(texture, drawPos + off - Main.screenPosition, frame, Color.Lerp(Color.Transparent, glowColor, Utils.GetLerpValue(150, 450, Time, true)) * Projectile.scale, 0, frame.Size() * 0.5f, Projectile.scale * 0.9f, 0, 0);
            }

            FlipShadersOnOff(Main.spriteBatch, effect, false);
            Main.EntitySpriteDraw(texture, drawPos - Main.screenPosition, frame, Color.White, 0, frame.Size() * 0.5f, Projectile.scale * 0.9f, 0, 0);
            FlipShadersOnOff(Main.spriteBatch, null, false);

            Main.EntitySpriteDraw(glow, drawPos - Main.screenPosition, glow.Frame(), Color.Lerp(Color.Transparent, glowColor * 0.2f, Utils.GetLerpValue(150, 450, Time, true)), 0, glow.Size() * 0.5f, 0.3f + Projectile.scale + size, 0, 0);

            Vector2 eyePos = Projectile.Center + drawOffset + new Vector2(-28, -20).RotatedBy(Projectile.rotation) * Projectile.scale;
            float eyeScale = (float)Math.Sqrt(Utils.GetLerpValue(840, 950, Time, true)) * 3f;
            float eyeRot = (float)Math.Cbrt(Utils.GetLerpValue(840, 1080, Time, true)) * MathHelper.PiOver2 - MathHelper.PiOver4;
            Main.EntitySpriteDraw(sparkle, eyePos - Main.screenPosition, sparkle.Frame(), glowColor * 0.1f, eyeRot + MathHelper.PiOver2, sparkle.Size() * 0.5f, eyeScale * new Vector2(0.3f, 2.4f), 0, 0);
            Main.EntitySpriteDraw(sparkle, eyePos - Main.screenPosition, sparkle.Frame(), glowColor * 0.1f, eyeRot, sparkle.Size() * 0.5f, eyeScale * new Vector2(0.3f, 2f), 0, 0);
            Main.EntitySpriteDraw(eye, eyePos - Main.screenPosition, eye.Frame(), glowColor, eyeRot, eye.Size() * 0.5f, eyeScale * 0.4f, 0, 0);
            Main.EntitySpriteDraw(eye, eyePos - Main.screenPosition, eye.Frame(), new Color(255, 255, 255, 0), eyeRot, eye.Size() * 0.5f, eyeScale * 0.4f, 0, 0);

            return false;
        }

        public void FlipShadersOnOff(SpriteBatch spriteBatch, Effect effect, bool immediate)
        {
            spriteBatch.End();
            SpriteSortMode sortMode = SpriteSortMode.Deferred;
            if (immediate)
                sortMode = SpriteSortMode.Immediate;
            spriteBatch.Begin(sortMode, BlendState.AlphaBlend, Main.DefaultSamplerState, DepthStencilState.None, Main.Rasterizer, effect, Main.Transform);
        }

        public void GetGradientMapValues(out float[] brightnesses, out Vector3[] colors)
        {
            float maxBright = 0.667f;
            brightnesses = new float[10];
            colors = new Vector3[10];

            float rainbowStartOffset = 0.35f + Projectile.ai[0] * 0.016f % (maxBright * 2f);
            //Calculate and store every non-modulo brightness, with the shifting offset. 
            //The first brightness is ignored for the moment, it will be relevant later. Setting it to -1 temporarily
            brightnesses[0] = -1;
            brightnesses[1] = rainbowStartOffset + 0.35f;
            brightnesses[2] = rainbowStartOffset + 0.42f;
            brightnesses[3] = rainbowStartOffset + 0.47f;
            brightnesses[4] = rainbowStartOffset + 0.51f;
            brightnesses[5] = rainbowStartOffset + 0.56f;
            brightnesses[6] = rainbowStartOffset + 0.61f;
            brightnesses[7] = rainbowStartOffset + 0.64f;
            brightnesses[8] = rainbowStartOffset + 0.72f;
            brightnesses[9] = rainbowStartOffset + 0.75f;

            //Pass the entire rainbow through modulo 1
            for (int i = 1; i < 10; i++)
                brightnesses[i] = HuntOfTheOldGodUtils.Modulo(brightnesses[i], maxBright) * maxBright;

            //Store the first element's value so we can find it again later
            float firstBrightnessValue = brightnesses[1];

            //Sort the values from lowest to highest
            Array.Sort(brightnesses);

            //Find the new index of the original first element after the list being sorted
            int rainbowStartIndex = Array.IndexOf(brightnesses, firstBrightnessValue);
            //Substract 1 from the index, because we are ignoring the currently negative first array slot.
            rainbowStartIndex--;

            //9 loop, filling a list of colors in a array of 10 elements (ignoring the first one)
            for (int i = 0; i < 9; i++)
            {
                colors[1 + (rainbowStartIndex + i) % 9] = SlimeUtils.GoozColorsVector3[i];
            }

            //We always want a brightness at index 0 to be the lower bound
            brightnesses[0] = 0;
            //Make the color at index 0 be a mix between the first and last colors in the list, based on the distance between the 2.
            float interpolant = (1 - brightnesses[9]) / (brightnesses[1] + (1 - brightnesses[9]));
            colors[0] = Vector3.Lerp(colors[9], colors[0], interpolant);
        }

    }
}
