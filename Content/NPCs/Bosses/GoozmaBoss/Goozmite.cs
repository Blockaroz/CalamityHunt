using System;
using System.Collections.Generic;
using System.Linq;
using CalamityHunt.Common;
using CalamityHunt.Common.Systems.Particles;
using CalamityHunt.Common.Utilities;
using CalamityHunt.Content.NPCs.Bosses.GoozmaBoss.Projectiles;
using CalamityHunt.Content.Particles;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent;
using Terraria.GameContent.Bestiary;
using Terraria.Graphics;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityHunt.Content.NPCs.Bosses.GoozmaBoss
{
    public class Goozmite : ModNPC
    {
        public override void SetStaticDefaults()
        {
            NPCID.Sets.TrailCacheLength[Type] = 10;
            NPCID.Sets.TrailingMode[Type] = 1;
            NPCID.Sets.MPAllowedEnemies[Type] = true;
            NPCID.Sets.ShouldBeCountedAsBoss[Type] = false;
            NPCID.Sets.SpecificDebuffImmunity[Type][BuffID.OnFire] = true;
            NPCID.Sets.SpecificDebuffImmunity[Type][BuffID.Ichor] = true;
            NPCID.Sets.SpecificDebuffImmunity[Type][BuffID.CursedInferno] = true;
            NPCID.Sets.SpecificDebuffImmunity[Type][BuffID.Poisoned] = true;
            NPCID.Sets.SpecificDebuffImmunity[Type][BuffID.Confused] = true;
            if (ModCompatibility.Calamity.IsLoaded) {
                NPCID.Sets.SpecificDebuffImmunity[Type][ModCompatibility.Calamity.Mod.Find<ModBuff>("MiracleBlight").Type] = true;
            }

            NPCID.Sets.NPCBestiaryDrawModifiers drawModifiers = new NPCID.Sets.NPCBestiaryDrawModifiers()
            {
                PortraitScale = 1f,
                PortraitPositionYOverride = 16,
            };

            NPCID.Sets.NPCBestiaryDrawOffset.Add(Type, drawModifiers);
        }

        public override void SetBestiary(BestiaryDatabase database, BestiaryEntry bestiaryEntry)
        {
            database.FindEntryByNPCID(Type).UIInfoProvider = new HighestOfMultipleUICollectionInfoProvider(new CommonEnemyUICollectionInfoProvider(ContentSamples.NpcBestiaryCreditIdsByNpcNetIds[ModContent.NPCType<Goozma>()], true));
            bestiaryEntry.Info.AddRange(new List<IBestiaryInfoElement> {
                new FlavorTextBestiaryInfoElement($"Mods.{nameof(CalamityHunt)}.Bestiary.Goozmite"),
                BestiaryDatabaseNPCsPopulator.CommonTags.SpawnConditions.Events.SlimeRain,
                new SlimeMonsoonPortraitBackground()
            });
        }

        public override void SetDefaults()
        {
            NPC.width = 75;
            NPC.height = 80;
            NPC.damage = 0;
            NPC.defense = 100;
            NPC.lifeMax = 10000;
            NPC.HitSound = SoundID.NPCDeath9;
            NPC.DeathSound = SoundID.NPCDeath9;
            NPC.knockBackResist = 0.1f;
            NPC.noGravity = true;
            NPC.noTileCollide = true;
            NPC.value = 0;
            NPC.SpawnWithHigherTime(30);
            NPC.npcSlots = 10f;
            NPC.aiStyle = -1;
            NPC.takenDamageMultiplier = 0.33f;
            if (ModLoader.HasMod(HUtils.CalamityMod)) {
                var calamity = ModLoader.GetMod(HUtils.CalamityMod);
                calamity.Call("SetDebuffVulnerabilities", "poison", false);
                calamity.Call("SetDebuffVulnerabilities", "heat", true);
                //calamity.Call("SetDefenseDamageNPC", Type, true);
            }
        }

        public ref float Time => ref NPC.ai[0];
        public ref float TimeUntilDeath => ref NPC.ai[1];
        public ref NPC Host => ref Main.npc[(int)NPC.ai[2]];

        private float ambientCounter;

        public override void AI()
        {
            NPC.damage = GetDamage(0);
            NPC.TargetClosestUpgraded();

            if (TimeUntilDeath < 5) {
                TimeUntilDeath = 400;
            }

            if (NPC.ai[3] != -1) {
                if (!Main.npc.Any(n => n.type == ModContent.NPCType<Goozma>() && n.active && n.ai[2] != 3)) {
                    NPC.ai[3] = -1;
                    Time = 0;
                    return;
                }
                else {
                    NPC.ai[2] = Main.npc.First(n => n.type == ModContent.NPCType<Goozma>() && n.active && n.ai[2] != 3).whoAmI;
                }
            }
            if (NPC.ai[3] == 0) {
                NPC.direction = Math.Sign(NPC.Center.X - Host.GetTargetData().Center.X);
                NPC.Center += (Host.position - Host.oldPos[3]) * 0.07f;

                if (NPC.velocity.Length() > 0.5f) {
                    NPC.velocity *= 0.9f;
                }

                NPC.dontTakeDamage = Time < 70 || Time > TimeUntilDeath;

                lookVector = Vector2.Lerp(lookVector, NPC.DirectionTo(Host.GetTargetData().Center).SafeNormalize(Vector2.Zero) * Math.Clamp(NPC.Distance(Host.GetTargetData().Center) * 0.5f, 0, 5f), 0.1f);
                if (!NPC.IsABestiaryIconDummy) {
                    NPC.scale = Utils.GetLerpValue(-20, 20, NPC.localAI[1], true);
                }

                ambientCounter++;
                if (ambientCounter > Main.rand.Next(100, 160)) {
                    ambientCounter = 0;
                    var ambientNoise = AssetDirectory.Sounds.Goozmite.Ambient;
                    SoundEngine.PlaySound(ambientNoise, NPC.Center);
                }

                if (Time > 100 && Time < TimeUntilDeath) {
                    if (Time % 100 == 5) {
                        NPC.velocity += Main.rand.NextVector2Circular(9, 9);
                    }

                    if (Host.Distance(NPC.GetTargetData().Center) > 800 && Time % 40 == 5) {
                        Projectile.NewProjectile(NPC.GetSource_FromThis(), NPC.Center, NPC.DirectionTo(NPC.GetTargetData().Center).SafeNormalize(Vector2.Zero).RotatedByRandom(0.3f), ModContent.ProjectileType<RainbowLaser>(), GetDamage(1), 0, ai0: -10, ai1: NPC.whoAmI);
                    }

                    var rateOfFiring = 30 + Main.npc.Count(n => n.active && n.type == ModContent.NPCType<Goozmite>()) * 5;
                    if (Time % rateOfFiring == 10 && Main.rand.NextBool(3)) {
                        Projectile.NewProjectile(NPC.GetSource_FromThis(), NPC.Center, NPC.DirectionTo(NPC.GetTargetData().Center).SafeNormalize(Vector2.Zero).RotatedByRandom(0.3f), ModContent.ProjectileType<RainbowLaser>(), GetDamage(1), 0, ai0: -Main.rand.Next(20, 30), ai1: NPC.whoAmI);
                    }
                }

                if (Time > TimeUntilDeath - 10) {
                    NPC.scale = 1f + (float)Math.Pow(Utils.GetLerpValue(TimeUntilDeath - 10, TimeUntilDeath, Time, true), 4f) * 0.2f;
                }

                if (Time > TimeUntilDeath) {
                    Host.life += (int)(NPC.life * 8f);
                    if (Host.life > Host.lifeMax) {
                        Host.life = Host.lifeMax;
                    }

                    for (var i = 0; i < 50; i++) {
                        CalamityHunt.particles.Add(Particle.Create<ChromaticEnergyDust>(particle => {
                            particle.position = NPC.Center + Main.rand.NextVector2Circular(10, 10);
                            particle.velocity = Main.rand.NextVector2Circular(9, 9);
                            particle.scale = Main.rand.NextFloat(1f, 2f);
                            particle.color = Color.White;
                            particle.colorData = new ColorOffsetData(true, NPC.localAI[0]);
                        }));
                    }
                    NPC.active = false;
                }
            }
            else if (NPC.ai[3] == 1) {
                NPC.friendly = true;
                NPC.velocity = Vector2.Lerp(NPC.velocity, NPC.DirectionTo(Host.Center).SafeNormalize(Vector2.Zero) * (float)Math.Pow(Utils.GetLerpValue(-20, 60, Time, true), 2f) * 50f, (float)Math.Pow(Utils.GetLerpValue(-20, 60, Time, true), 2f) * 0.3f);
                NPC.velocity = NPC.velocity.RotatedBy(0.02f * NPC.direction);
                NPC.rotation = NPC.rotation.AngleLerp(NPC.velocity.ToRotation() + MathHelper.PiOver2, 0.2f * Utils.GetLerpValue(0, 40, Time, true));

                CalamityHunt.particles.Add(Particle.Create<ChromaticEnergyDust>(particle => {
                    particle.position = NPC.Center + Main.rand.NextVector2Circular(40, 40);
                    particle.velocity = NPC.velocity * Main.rand.NextFloat();
                    particle.scale = Main.rand.NextFloat(1f, 2f);
                    particle.color = Color.White;
                    particle.colorData = new ColorOffsetData(true, NPC.localAI[0]);
                }));

                if (NPC.Distance(Host.Center) < 80) {
                    NPC.life = 0;
                    Host.life -= 700;
                    NPC.checkDead();
                    for (var i = 0; i < 30; i++) {
                        CalamityHunt.particles.Add(Particle.Create<ChromaticEnergyDust>(particle => {
                            particle.position = NPC.Center + Main.rand.NextVector2Circular(10, 10);
                            particle.velocity = -NPC.velocity.RotatedByRandom(1f) * Main.rand.NextFloat(0.3f);
                            particle.scale = 1f;
                            particle.color = Color.White;
                            particle.colorData = new ColorOffsetData(true, NPC.localAI[0]);
                        }));
                    }
                    for (var i = 0; i < 2; i++) {
                        var gooVelocity = new Vector2(2, 0).RotatedBy(MathHelper.TwoPi / 2f * i + NPC.velocity.ToRotation());

                        CalamityHunt.particles.Add(Particle.Create<ChromaticGooBurst>(particle => {
                            particle.position = NPC.Center + gooVelocity;
                            particle.velocity = gooVelocity;
                            particle.scale = 1.5f - i * 0.8f;
                            particle.color = Color.White;
                            particle.colorData = new ColorOffsetData(true, NPC.localAI[0]);
                        }));
                    }
                }
            }
            else {
                NPC.scale = 1f + (float)Math.Pow(Time / 10f, 4f) * 0.2f;
                if (Time > 10) {
                    for (var i = 0; i < 20; i++) {
                        CalamityHunt.particles.Add(Particle.Create<ChromaticEnergyDust>(particle => {
                            particle.position = NPC.Center + Main.rand.NextVector2Circular(10, 10);
                            particle.velocity = Main.rand.NextVector2Circular(10, 10);
                            particle.scale = 1f;
                            particle.color = Color.White;
                            particle.colorData = new ColorOffsetData(true, NPC.localAI[0]);
                        }));
                    }
                    for (var i = 0; i < 3; i++) {
                        var gooVelocity = new Vector2(1).RotatedBy(MathHelper.TwoPi / 3f * i).RotatedByRandom(0.6f);

                        CalamityHunt.particles.Add(Particle.Create<ChromaticGooBurst>(particle => {
                            particle.position = NPC.Center + gooVelocity;
                            particle.velocity = gooVelocity;
                            particle.scale = Main.rand.NextFloat(0.5f, 1.5f);
                            particle.color = Color.White;
                            particle.colorData = new ColorOffsetData(true, NPC.localAI[0]);
                        }));
                    }

                    NPC.active = false;
                }
            }

            if (Main.rand.NextBool(5)) {
                Dust dust = Dust.NewDustDirect(NPC.Center - new Vector2(25), 50, 50, DustID.TintableDust, Main.rand.NextFloat(-1f, 1f), -4f, 230, Color.Black, 2f + Main.rand.NextFloat());
                dust.noGravity = true;
            }
            if (Main.rand.NextBool(6)) {
                CalamityHunt.particles.Add(Particle.Create<ChromaticEnergyDust>(particle => {
                    particle.position = NPC.Center + Main.rand.NextVector2Circular(50, 50);
                    particle.velocity = Main.rand.NextVector2Circular(1, 1) - Vector2.UnitY * 3f;
                    particle.scale = 1f;
                    particle.color = Color.White;
                    particle.colorData = new ColorOffsetData(true, NPC.localAI[0]);
                }));
            }

            Time++;
            NPC.localAI[0]++;
            NPC.localAI[1]++;

            var randCounter = Main.rand.Next(35, 50);
            if ((Time - 40) % randCounter < Main.rand.Next(3)) {
                zapPoints = new List<Vector2>();
                zapVelocities = new List<Vector2>();
                zapPoints.Add(NPC.Center);
                var count = 6;
                var offMid = Main.rand.NextVector2Circular(10, 120).RotatedBy(NPC.AngleTo(Host.Center));
                for (var i = 0; i < count; i++) {
                    var prog = Utils.GetLerpValue(0, count, i, true);
                    var point = Vector2.Lerp(NPC.Center, Host.Center, i / (float)count) + (offMid * 0.2f + Main.rand.NextVector2Circular(5, 25).RotatedBy(NPC.AngleTo(Host.Center))) * prog;
                    zapPoints.Add(point);
                }
                zapPoints.Add(Host.Center);

                for (var i = 0; i < zapPoints.Count; i++) {
                    var direction = NPC.DirectionTo(Host.Center).SafeNormalize(Vector2.Zero).RotatedByRandom(0.1f);
                    zapVelocities.Add(direction * -Main.rand.NextFloat(-1f, 5f) + Main.rand.NextVector2Circular(1f, 2f).RotatedBy(direction.ToRotation()) + offMid * 0.05f);
                }

                if (zapPointsReal == null) {
                    zapPointsReal = new List<Vector2>();
                    for (var i = 0; i < zapPoints.Count; i++)
                        zapPointsReal.Add(zapPoints[i]);
                }
            }

            if (Time > 40) {
                for (var i = 0; i < zapPoints.Count; i++) {
                    if ((Time + i) % randCounter > 15)
                        zapVelocities[i] *= 0.93f;
                    zapPoints[i] += zapVelocities[i] * (float)Math.Sin(i / (float)zapPoints.Count * MathHelper.Pi);
                    zapPoints[i] += (NPC.position - NPC.oldPosition) * Utils.GetLerpValue(zapPoints.Count, 0, i, true);
                    zapPoints[i] += (Host.position - Host.oldPosition) * Utils.GetLerpValue(0, zapPoints.Count, i, true);
                }

                for (var i = 0; i < zapPointsReal.Count; i++)
                    zapPointsReal[i] = Vector2.Lerp(zapPointsReal[i], zapPoints[i], 0.8f + 0.2f * (Utils.GetLerpValue(0.1f, 0f, i / (float)zapPointsReal.Count, true) + Utils.GetLerpValue(0.9f, 1f, i / (float)zapPointsReal.Count, true))) + Main.rand.NextVector2CircularEdge(2, 2) * (float)Math.Sin(i / (float)zapPointsReal.Count * MathHelper.Pi);
            }
        }

        public override bool CheckDead()
        {
            if (NPC.life <= 0 && NPC.ai[3] == 0) {
                Time = 0;
                NPC.direction = Main.rand.NextBool() ? -1 : 1;
                NPC.life = 1;
                NPC.dontTakeDamage = true;
                NPC.ai[3] = 1;

                var deathNoise = AssetDirectory.Sounds.Goozmite.Death;
                SoundEngine.PlaySound(deathNoise, NPC.Center);
            }
            else if (NPC.ai[3] == 1) {
                var impactNoise = AssetDirectory.Sounds.Goozmite.Impact;
                SoundEngine.PlaySound(impactNoise, NPC.Center);
            }
            return NPC.ai[3] == 0 && Time > TimeUntilDeath || NPC.ai[3] == 1 && Time > 10;
        }

        private int GetDamage(int attack, float modifier = 1f)
        {
            var damage = attack switch
            {
                0 => 0,//contact
                1 => 100,//mini rainbow balls
                _ => 0
            };

            return (int)(damage * modifier);
        }

        public override void FindFrame(int frameHeight)
        {
            if (NPC.IsABestiaryIconDummy)
                NPC.localAI[0]++;
        }

        private Vector2 lookVector;

        public static Texture2D dressTexture;
        public static Texture2D bubbleTexture;
        public static Texture2D eyeTexture;

        public override void Load()
        {
            dressTexture = ModContent.Request<Texture2D>(Texture + "Dress", AssetRequestMode.ImmediateLoad).Value;
            bubbleTexture = ModContent.Request<Texture2D>(Texture + "Bubble", AssetRequestMode.ImmediateLoad).Value;
            eyeTexture = ModContent.Request<Texture2D>(Texture + "Eye", AssetRequestMode.ImmediateLoad).Value;
        }

        public override bool PreDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {
            var texture = TextureAssets.Npc[Type].Value;
            var glow = AssetDirectory.Textures.Glow.Value;
            var flare = AssetDirectory.Textures.Sparkle.Value;

            var spriteEffect = NPC.direction > 0 ? SpriteEffects.None : SpriteEffects.FlipHorizontally;

            var myColor = new GradientColor(SlimeUtils.GoozColors, 0.2f, 0.2f).ValueAt(NPC.localAI[0]);
            myColor.A = 0;

            if (Time > 40 && NPC.ai[3] == 0) {
                FlipShadersOnOff(spriteBatch, null, true);
                DrawHealZap(spriteBatch, screenPos);
            }

            Vector2 bubbleSquish = new Vector2(1f + (float)Math.Sin(NPC.localAI[1] * 0.1f) * 0.1f, 1f - (float)Math.Cos(NPC.localAI[1] * 0.1f) * 0.1f);
            spriteBatch.Draw(glow, NPC.Center + new Vector2(0, -27) - screenPos, glow.Frame(), myColor * 0.15f, NPC.rotation, glow.Size() * 0.5f, NPC.scale * 4f, 0, 0);
            spriteBatch.Draw(glow, NPC.Center + new Vector2(0, -18) - screenPos, glow.Frame(), myColor, NPC.rotation, glow.Size() * 0.5f, NPC.scale, 0, 0);
            spriteBatch.Draw(bubbleTexture, NPC.Center - screenPos, bubbleTexture.Frame(), myColor, NPC.rotation, bubbleTexture.Size() * new Vector2(0.5f, 0.9f), NPC.scale * 1.01f * bubbleSquish, 0, 0);
            spriteBatch.Draw(bubbleTexture, NPC.Center - screenPos, bubbleTexture.Frame(), myColor, NPC.rotation, bubbleTexture.Size() * new Vector2(0.5f, 0.9f), NPC.scale * bubbleSquish, 0, 0);

            GetGradientMapValues(out var brightnesses, out var colors);
            var effect = AssetDirectory.Effects.HolographicGel.Value;
            effect.Parameters["uTime"].SetValue(Main.GlobalTimeWrappedHourly % 1f);
            effect.Parameters["colors"].SetValue(colors);
            effect.Parameters["brightnesses"].SetValue(brightnesses);
            effect.Parameters["baseToScreenPercent"].SetValue(1f);
            effect.Parameters["baseToMapPercent"].SetValue(0f);
            FlipShadersOnOff(spriteBatch, effect, false);

            spriteBatch.Draw(dressTexture, NPC.Center + new Vector2(0, -2).RotatedBy(NPC.rotation) * NPC.scale - screenPos, dressTexture.Frame(), Color.White, NPC.rotation + (float)Math.Sin(NPC.localAI[0] * 0.4f) * 0.06f, dressTexture.Size() * new Vector2(0.5f, 0f), NPC.scale, spriteEffect, 0);
            spriteBatch.Draw(texture, NPC.Center + new Vector2(0, -12).RotatedBy(NPC.rotation) * NPC.scale - screenPos, texture.Frame(), Color.White, NPC.rotation, texture.Size() * 0.5f, NPC.scale, spriteEffect, 0);
            spriteBatch.Draw(eyeTexture, NPC.Center + lookVector - screenPos, eyeTexture.Frame(), Color.White, NPC.rotation, eyeTexture.Size() * 0.5f, NPC.scale, spriteEffect, 0);

            FlipShadersOnOff(spriteBatch, null, false);

            var flareScale = 1f + (float)Math.Sin(NPC.localAI[0] * 0.5f) * 0.05f;
            spriteBatch.Draw(flare, NPC.Center + lookVector - screenPos, flare.Frame(), myColor, -MathHelper.PiOver4, flare.Size() * 0.5f, new Vector2(0.3f, 2f) * flareScale, 0, 0);
            spriteBatch.Draw(flare, NPC.Center + lookVector - screenPos, flare.Frame(), myColor, MathHelper.PiOver4, flare.Size() * 0.5f, new Vector2(0.3f, 1.5f) * flareScale, 0, 0);
            spriteBatch.Draw(flare, NPC.Center + lookVector - screenPos, flare.Frame(), new Color(255, 255, 255, 0), -MathHelper.PiOver4, flare.Size() * 0.5f, new Vector2(0.1f, 0.7f) * flareScale, 0, 0);
            spriteBatch.Draw(flare, NPC.Center + lookVector - screenPos, flare.Frame(), new Color(255, 255, 255, 0), MathHelper.PiOver4, flare.Size() * 0.5f, new Vector2(0.1f, 0.7f) * flareScale, 0, 0);

            if (NPC.ai[3] <= 0 && !NPC.IsABestiaryIconDummy) {
                var power = (float)Math.Pow(Utils.GetLerpValue(TimeUntilDeath - 20, TimeUntilDeath, Time, true), 2f);
                if (NPC.ai[3] < 0)
                    power = (float)Math.Pow(Utils.GetLerpValue(0f, 8f, Time, true), 2f);

                spriteBatch.Draw(flare, NPC.Center + lookVector - screenPos, flare.Frame(), myColor, -MathHelper.PiOver4, flare.Size() * 0.5f, new Vector2(1f, 5f) * power, 0, 0);
                spriteBatch.Draw(flare, NPC.Center + lookVector - screenPos, flare.Frame(), myColor, MathHelper.PiOver4, flare.Size() * 0.5f, new Vector2(1f, 5f) * power, 0, 0);
                spriteBatch.Draw(flare, NPC.Center + lookVector - screenPos, flare.Frame(), myColor, 0, flare.Size() * 0.5f, new Vector2(1f, 9f) * power, 0, 0);
                spriteBatch.Draw(flare, NPC.Center + lookVector - screenPos, flare.Frame(), myColor, MathHelper.PiOver2, flare.Size() * 0.5f, new Vector2(1f, 7f) * power, 0, 0);
                spriteBatch.Draw(flare, NPC.Center + lookVector - screenPos, flare.Frame(), new Color(255, 255, 255, 0), 0, flare.Size() * 0.5f, new Vector2(1f, 2f) * power, 0, 0);
                spriteBatch.Draw(flare, NPC.Center + lookVector - screenPos, flare.Frame(), new Color(255, 255, 255, 0), MathHelper.PiOver2, flare.Size() * 0.5f, new Vector2(1f, 2f) * power, 0, 0);
            }

            return false;
        }

        public void FlipShadersOnOff(SpriteBatch spriteBatch, Effect effect, bool immediate)
        {
            if (NPC.IsABestiaryIconDummy) {
                var priorRasterizer = spriteBatch.GraphicsDevice.RasterizerState;
                var priorScissorRectangle = spriteBatch.GraphicsDevice.ScissorRectangle;
                spriteBatch.End();
                spriteBatch.GraphicsDevice.RasterizerState = priorRasterizer;
                spriteBatch.GraphicsDevice.ScissorRectangle = priorScissorRectangle;
                spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, Main.DefaultSamplerState, DepthStencilState.None, priorRasterizer, effect, Main.UIScaleMatrix);
            }
            else {
                spriteBatch.End();
                var sortMode = SpriteSortMode.Deferred;
                if (immediate)
                    sortMode = SpriteSortMode.Immediate;
                spriteBatch.Begin(sortMode, BlendState.AlphaBlend, Main.DefaultSamplerState, DepthStencilState.None, Main.Rasterizer, effect, Main.Transform);
            }
        }

        public void GetGradientMapValues(out float[] brightnesses, out Vector3[] colors)
        {
            var maxBright = 0.667f;
            brightnesses = new float[10];
            colors = new Vector3[10];

            var rainbowStartOffset = 0.35f + NPC.localAI[0] * 0.01f % (maxBright * 2f);
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
            for (var i = 1; i < 10; i++)
                brightnesses[i] = HUtils.Modulo(brightnesses[i], maxBright) * maxBright;

            //Store the first element's value so we can find it again later
            var firstBrightnessValue = brightnesses[1];

            //Sort the values from lowest to highest
            Array.Sort(brightnesses);

            //Find the new index of the original first element after the list being sorted
            var rainbowStartIndex = Array.IndexOf(brightnesses, firstBrightnessValue);
            //Substract 1 from the index, because we are ignoring the currently negative first array slot.
            rainbowStartIndex--;

            //9 loop, filling a list of colors in a array of 10 elements (ignoring the first one)
            for (var i = 0; i < 9; i++) {
                colors[1 + (rainbowStartIndex + i) % 9] = SlimeUtils.GoozColorsVector3[i];
            }

            //We always want a brightness at index 0 to be the lower bound
            brightnesses[0] = 0;
            //Make the color at index 0 be a mix between the first and last colors in the list, based on the distance between the 2.
            var interpolant = (1 - brightnesses[9]) / (brightnesses[1] + (1 - brightnesses[9]));
            colors[0] = Vector3.Lerp(colors[9], colors[0], interpolant);
        }

        private List<Vector2> zapPointsReal;
        private List<Vector2> zapPoints;
        private List<Vector2> zapVelocities;

        public void DrawHealZap(SpriteBatch spriteBatch, Vector2 screenPos)
        {
            VertexStrip strip = new VertexStrip();

            var myColor = new GradientColor(SlimeUtils.GoozColors, 0.2f, 0.2f).ValueAt(NPC.localAI[0]) with { A = 0 };
            var goozmaColor = new GradientColor(SlimeUtils.GoozColors, 0.2f, 0.2f).ValueAt(Host.localAI[0]) with { A = 0 };

            Color StripColor(float progress) => (Color.Lerp(myColor, goozmaColor * 0.2f, progress * 1.1f) * 0.2f) with { A = 0 };
            float StripWidth(float progress) => (1f - (float)Math.Sin(progress * 2.5f) * 0.85f) * 100f;

            var rots = new float[zapPointsReal.Count];
            for (var i = 1; i < zapPointsReal.Count; i++)
                rots[i] = zapPointsReal[i - 1].AngleTo(zapPoints[i]);
            rots[0] = zapPointsReal[0].AngleTo(zapPoints[1]);
            strip.PrepareStrip(zapPointsReal.ToArray(), rots, StripColor, StripWidth, -Main.screenPosition, zapPointsReal.Count, true);

            var lightningEffect = AssetDirectory.Effects.GooLightning.Value;
            lightningEffect.Parameters["uTransformMatrix"].SetValue(Main.GameViewMatrix.NormalizedTransformationmatrix);
            lightningEffect.Parameters["uTexture"].SetValue(AssetDirectory.Textures.Goozma.LightningGlow.Value);
            lightningEffect.Parameters["uGlow"].SetValue(AssetDirectory.Textures.Goozma.LightningGlow.Value);
            lightningEffect.Parameters["uColor"].SetValue(Vector3.One);
            lightningEffect.Parameters["uTime"].SetValue(NPC.localAI[0] * 0.02f % 1f);
            lightningEffect.Parameters["uBackPower"].SetValue(0.2f);
            lightningEffect.CurrentTechnique.Passes[0].Apply();

            strip.DrawTrail();

            Main.pixelShader.CurrentTechnique.Passes[0].Apply();

        }

        public override void ModifyHitByProjectile(Projectile projectile, ref NPC.HitModifiers modifiers)
        {
            if (ModLoader.HasMod(HUtils.CalamityMod)) {
                if (projectile.type == ModLoader.GetMod(HUtils.CalamityMod).Find<ModProjectile>("CelestusMiniScythe").Type || projectile.type == ModLoader.GetMod(HUtils.CalamityMod).Find<ModProjectile>("CelestusProj").Type) {
                    modifiers.SourceDamage *= 0.9f;
                }
                if (projectile.type == ModLoader.GetMod(HUtils.CalamityMod).Find<ModProjectile>("PhotonRipperProjectile").Type || projectile.type == ModLoader.GetMod(HUtils.CalamityMod).Find<ModProjectile>("PrismTooth").Type) {
                    modifiers.SourceDamage *= 0.7f;
                }
            }
        }
    }
}
