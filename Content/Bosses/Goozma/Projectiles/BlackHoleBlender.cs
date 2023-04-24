using CalamityHunt.Common.Systems.Particles;
using CalamityHunt.Content.Bosses.Goozma;
using CalamityHunt.Content.Items.Weapons.Rogue;
using CalamityHunt.Content.Particles;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using ReLogic.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.GameContent;
using Terraria.Graphics.CameraModifiers;
using Terraria.Graphics.Effects;
using Terraria.Graphics.Shaders;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityHunt.Content.Bosses.Goozma.Projectiles
{
    public class BlackHoleBlender : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.DrawScreenCheckFluff[Type] = 10000;
        }

        public override void SetDefaults()
        {
            Projectile.width = 16;
            Projectile.height = 16;
            Projectile.tileCollide = false;
            Projectile.hostile = false;
            Projectile.friendly = false;
            Projectile.penetrate = -1;
            Projectile.aiStyle = -1;
            Projectile.hide = true;
        }

        public ref float Time => ref Projectile.ai[0];
        public ref float MaxTime => ref Projectile.ai[1];

        public override void OnSpawn(IEntitySource source)
        {
        }

        public override void AI()
        {
            int owner = -1;
            if (!Main.npc.Any(n => n.type == ModContent.NPCType<StellarGeliath>() && n.active))
            {
                Projectile.scale *= 0.84f;
                if (Projectile.scale < 0.01f)
                    Projectile.Kill();

                return;
            }
            else
                owner = Main.npc.First(n => n.type == ModContent.NPCType<StellarGeliath>() && n.active).whoAmI;

            Projectile.scale = (float)Math.Sqrt(MathHelper.SmoothStep(0, 1, Utils.GetLerpValue(40, MaxTime - 80, Time, true) * Utils.GetLerpValue(MaxTime, MaxTime - 80, Time, true))) * 0.75f;
            Projectile.velocity = Projectile.DirectionTo(Main.npc[owner].Center).SafeNormalize(Vector2.Zero) * Projectile.Distance(Main.npc[owner].Center) * 0.2f;

            for (int i = 0; i < 6; i++)
            {
                Particle smoke = Particle.NewParticle(Particle.ParticleType<CosmicSmoke>(), Projectile.Center + Main.rand.NextVector2Circular(200, 200) * Projectile.scale + Projectile.velocity * (i / 6f) * 0.5f, (Main.rand.NextVector2Circular(15, 15) + Projectile.velocity * (i / 8f)) * Projectile.scale, Color.White, (3f + Main.rand.NextFloat(3f)) * Projectile.scale);
                smoke.data = "Cosmos";
            }

            HandleSound();

            Time++;
            if (Time > MaxTime)
                Projectile.Kill();

            Projectile.localAI[0]++;

            if (Math.Abs(Projectile.velocity.X) > 2)
                Projectile.direction = Math.Sign(Projectile.velocity.X);

            Projectile.rotation += 0.05f;
        }

        public static SlotId holeSound;
        public static SlotId windSound;
        public static float volume;
        public static float pitch;

        public void HandleSound()
        {
            SoundStyle blackholeSound = new SoundStyle($"{nameof(CalamityHunt)}/Assets/Sounds/Goozma/Slimes/StellarBlackHoleLoop");
            blackholeSound.IsLooped = true;
            SoundStyle windBlowingSound = new SoundStyle($"{nameof(CalamityHunt)}/Assets/Sounds/Goozma/GoozmaWindLoop");
            windBlowingSound.IsLooped = true;

            volume = Math.Clamp(1f + Projectile.velocity.Length() * 0.0001f - Main.LocalPlayer.Distance(Projectile.Center) * 0.0005f, 0, 1) * Projectile.scale;
            pitch = (float)Math.Sqrt(Utils.GetLerpValue(0, MaxTime - 80, Time, true) * Utils.GetLerpValue(MaxTime, MaxTime - 80, Time, true)) * 1.5f - 1f;

            bool holeActive = SoundEngine.TryGetActiveSound(BlackHoleBlender.holeSound, out ActiveSound holeSound);
            if (!holeActive || !BlackHoleBlender.holeSound.IsValid)
                BlackHoleBlender.holeSound = SoundEngine.PlaySound(blackholeSound, Projectile.Center);

            else if (holeActive)
            {
                holeSound.Volume = volume * 0.8f;
                holeSound.Pitch = pitch;
                holeSound.Position = Projectile.Center;
            }            
            
            bool windActive = SoundEngine.TryGetActiveSound(BlackHoleBlender.windSound, out ActiveSound windSound);
            if (!windActive || !BlackHoleBlender.windSound.IsValid)
                BlackHoleBlender.windSound = SoundEngine.PlaySound(windBlowingSound, Projectile.Center);

            else if (holeActive)
            {
                windSound.Volume = volume;
                windSound.Pitch = 0.3f + pitch * 0.5f;
                windSound.Position = Projectile.Center;
            }
        }

        public override bool PreKill(int timeLeft)
        {
            bool active = SoundEngine.TryGetActiveSound(holeSound, out ActiveSound sound);
            if (active)
                sound.Stop();

            Filters.Scene["HuntOfTheOldGods:StellarBlackHole"].Deactivate();

            return true;
        }

        public override void Load()
        {
            On_Main.UpdateAudio += QuietMusic;
        }

        private void QuietMusic(On_Main.orig_UpdateAudio orig, Main self)
        {
            orig(self);

            if (Main.projectile.Any(n => n.active && n.type == ModContent.ProjectileType<BlackHoleBlender>()))
            {
                Projectile projectile = Main.projectile.FirstOrDefault(n => n.active && n.type == ModContent.ProjectileType<BlackHoleBlender>());

                for (int i = 0; i < Main.musicFade.Length; i++)
                {
                    float volume = Main.musicFade[i] * Main.musicVolume * (1f - projectile.scale * 0.5f);
                    float tempFade = Main.musicFade[i];
                    Main.audioSystem.UpdateCommonTrackTowardStopping(i, volume, ref tempFade, Main.musicFade[i] > 0.15f);
                    Main.musicFade[i] = tempFade;
                }
            }
        }

        public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox) => false;

        public override void DrawBehind(int index, List<int> behindNPCsAndTiles, List<int> behindNPCs, List<int> behindProjectiles, List<int> overPlayers, List<int> overWiresUI) => overPlayers.Add(index);

        public override bool PreDraw(ref Color lightColor)
        {
            Asset<Texture2D> shadowTexture = ModContent.Request<Texture2D>(Texture + "Shadow");

            Main.EntitySpriteDraw(shadowTexture.Value, Projectile.Center - Main.screenPosition, shadowTexture.Frame(), Color.Black * 0.5f, -Projectile.rotation * 0.7f, shadowTexture.Size() * 0.5f, Projectile.scale * 1.3f, SpriteEffects.FlipHorizontally, 0);
            Main.EntitySpriteDraw(shadowTexture.Value, Projectile.Center - Main.screenPosition, shadowTexture.Frame(), Color.Black * 0.2f, -Projectile.rotation * 0.5f, shadowTexture.Size() * 0.5f, Projectile.scale * 1.5f, SpriteEffects.FlipHorizontally, 0);
            Main.EntitySpriteDraw(shadowTexture.Value, Projectile.Center - Main.screenPosition, shadowTexture.Frame(), Color.Black, Projectile.rotation * 0.33f, shadowTexture.Size() * 0.5f, Projectile.scale * 1.2f, 0, 0);

            if (Filters.Scene["HuntOfTheOldGods:StellarBlackHole"].Active)
            {
                Filters.Scene["HuntOfTheOldGods:StellarBlackHole"].GetShader()
                    .UseColor(Color.White)
                    .UseTargetPosition(Projectile.Center)
                    .UseProgress(Main.GlobalTimeWrappedHourly * 0.1f % 1f)
                    .UseIntensity((float)Math.Sqrt(Utils.GetLerpValue(0.1f, 0.9f, Projectile.scale, true)) * 0.4f);
                Filters.Scene["HuntOfTheOldGods:StellarBlackHole"].GetShader().Shader.Parameters["distortionSample"].SetValue(ModContent.Request<Texture2D>($"{nameof(CalamityHunt)}/Common/Graphics/SlimeMonsoon/DistortNoise").Value);
                Filters.Scene["HuntOfTheOldGods:StellarBlackHole"].GetShader().Shader.Parameters["distortSize"].SetValue(Vector2.One * 0.4f);
                Filters.Scene["HuntOfTheOldGods:StellarBlackHole"].GetShader().Shader.Parameters["uSize"].SetValue(new Vector2(1f));
            }
            else
                Filters.Scene.Activate("HuntOfTheOldGods:StellarBlackHole", Projectile.Center);

            return false;
        }
    }
}
