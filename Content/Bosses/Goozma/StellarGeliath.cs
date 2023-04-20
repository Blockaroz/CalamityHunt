using CalamityHunt.Common.Systems.Particles;
using CalamityHunt.Content.Bosses.Goozma.Projectiles;
using CalamityHunt.Content.Particles;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using System;
using System.Collections.Generic;
using System.Linq;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.GameContent;
using Terraria.GameContent.Bestiary;
using Terraria.Graphics.CameraModifiers;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;

namespace CalamityHunt.Content.Bosses.Goozma
{
    public class StellarGeliath : ModNPC
    {
        public override void SetStaticDefaults()
        {
            Main.npcFrameCount[Type] = 4;
            NPCID.Sets.TrailCacheLength[Type] = 10;
            NPCID.Sets.TrailingMode[Type] = 1;
            NPCID.Sets.MPAllowedEnemies[Type] = true;
            NPCID.Sets.ShouldBeCountedAsBoss[Type] = false;
            NPCDebuffImmunityData debuffData = new NPCDebuffImmunityData
            {
                SpecificallyImmuneTo = new int[] {
                    BuffID.OnFire,
                    BuffID.Ichor,
                    BuffID.CursedInferno,
                    BuffID.Poisoned,
                    BuffID.Confused
				}
            };
            NPCID.Sets.DebuffImmunitySets.Add(Type, debuffData);

            //NPCID.Sets.NPCBestiaryDrawModifiers drawModifiers = new NPCID.Sets.NPCBestiaryDrawModifiers(0) { };
            //NPCID.Sets.NPCBestiaryDrawOffset.Add(Type, drawModifiers);
        }

        public override void SetBestiary(BestiaryDatabase database, BestiaryEntry bestiaryEntry)
        {
            database.FindEntryByNPCID(Type).UIInfoProvider = new HighestOfMultipleUICollectionInfoProvider(new CommonEnemyUICollectionInfoProvider(ContentSamples.NpcBestiaryCreditIdsByNpcNetIds[ModContent.NPCType<Goozma>()], true));
            bestiaryEntry.Info.AddRange(new List<IBestiaryInfoElement> {
                new FlavorTextBestiaryInfoElement("Mods.CalamityHunt.Bestiary.StellarGeliath"),
                BestiaryDatabaseNPCsPopulator.CommonTags.SpawnConditions.Events.SlimeRain,
                ModLoader.HasMod("CalamityMod") ? null : BestiaryDatabaseNPCsPopulator.CommonTags.SpawnConditions.Biomes.Sky,
            });
        }

        public override void SetDefaults()
        {
            NPC.width = 150;
            NPC.height = 100;
            NPC.damage = 12;
            NPC.defense = 10;
            NPC.lifeMax = 3000000;
            NPC.HitSound = SoundID.NPCHit1;
            NPC.DeathSound = SoundID.NPCDeath1;
            NPC.knockBackResist = 0f;
            NPC.noGravity = true;
            NPC.noTileCollide = true;
            NPC.value = Item.buyPrice(gold: 5);
            NPC.SpawnWithHigherTime(30);
            NPC.npcSlots = 10f;
            NPC.aiStyle = -1;
            NPC.immortal = true;
            NPC.takenDamageMultiplier = 0.33f;
            if (ModLoader.HasMod("CalamityMod"))
            {
                Mod calamity = ModLoader.GetMod("CalamityMod");
                calamity.Call("SetDebuffVulnerabilities", "poison", false);
                calamity.Call("SetDebuffVulnerabilities", "heat", true);
                calamity.Call("SetDefenseDamageNPC", Type, true);
                SpawnModBiomes = new int[1] { calamity.Find<ModBiome>("AbovegroundAstralBiome").Type };
            }
        }

        private enum AttackList
        {
            StarSigns,
            Starfall,
            BlackHole,
            TooFar,
            Interrupt
        }

        public ref float Time => ref NPC.ai[0];
        public ref float Attack => ref NPC.ai[1];
        public ref NPC Host => ref Main.npc[(int)NPC.ai[2]];
        public ref float RememberAttack => ref NPC.ai[3];

        public NPCAimedTarget Target => NPC.GetTargetData();
        public Vector2 squishFactor = Vector2.One;

        public override void AI()
        {
            if (!Main.npc.Any(n => n.type == ModContent.NPCType<Goozma>() && n.active))
                NPC.active = false;
            else
                NPC.ai[2] = Main.npc.First(n => n.type == ModContent.NPCType<Goozma>() && n.active).whoAmI;

            NPC.realLife = Host.whoAmI;

            if (!NPC.HasPlayerTarget)
                NPC.TargetClosestUpgraded();
            if (!NPC.HasPlayerTarget)
                NPC.active = false;

            NPC.damage = GetDamage(0);

            if (Time < 0)
            {
                NPC.velocity *= 0.9f;
                NPC.damage = 0;
                squishFactor = new Vector2(1f - (float)Math.Pow(Utils.GetLerpValue(-10, -45, Time, true), 2) * 0.5f, 1f + (float)Math.Pow(Utils.GetLerpValue(-10, -45, Time, true), 2) * 0.4f);
                if (Time == -2 && NPC.Distance(Target.Center) > 1000)
                {
                    RememberAttack = Attack;
                    Attack = (int)AttackList.TooFar;
                }
                NPC.frameCounter++;
            }
            else switch (Attack)
                {
                    case (int)AttackList.StarSigns:
                    case (int)AttackList.Starfall:
                    case (int)AttackList.BlackHole:

                        StarSigns();

                        break;

                    case (int)AttackList.Interrupt:
                        NPC.noTileCollide = true;
                        NPC.damage = 0;
                        NPC.velocity *= 0.5f;

                        if (Time < 15)
                            Host.ai[0] = 0;

                        break;

                    case (int)AttackList.TooFar:

                        if (Time < 5)
                            saveTarget = Target.Center;

                        Vector2 midPoint = new Vector2((NPC.Center.X + saveTarget.X) / 2f, NPC.Center.Y);
                        Vector2 jumpTarget = Vector2.Lerp(Vector2.Lerp(NPC.Center, midPoint, Utils.GetLerpValue(0, 15, Time, true)), Vector2.Lerp(midPoint, saveTarget, Utils.GetLerpValue(5, 30, Time, true)), Utils.GetLerpValue(0, 30, Time, true));
                        NPC.velocity = Vector2.Lerp(NPC.velocity, NPC.DirectionTo(jumpTarget).SafeNormalize(Vector2.Zero) * NPC.Distance(jumpTarget) * 0.5f * Utils.GetLerpValue(0, 35, Time, true), (float)Math.Pow(Utils.GetLerpValue(0, 40, Time, true), 2f));
                        Host.ai[0] = 0;

                        if (Time < 40)
                            squishFactor = Vector2.Lerp(Vector2.One, new Vector2(0.6f, 1.3f), Time / 40f);
                        else
                            squishFactor = Vector2.Lerp(new Vector2(1.4f, 0.5f), Vector2.One, Utils.GetLerpValue(40, 54, Time, true));

                        if (Time > 55)
                        {
                            NPC.velocity *= 0f;
                            Time = 0;
                            Attack = RememberAttack;
                        }
                        break;
                }

            Particle smoke = Particle.NewParticle(Particle.ParticleType<CosmicSmoke>(), NPC.Center + Main.rand.NextVector2Circular(70, 50) * NPC.scale, Main.rand.NextVector2Circular(2, 2), Color.White, (1f + Main.rand.NextFloat()) * NPC.scale);
            smoke.data = "Cosmos";

            Time++;
        }

        private void Reset()
        {
            Time = 0;
            Attack = (int)AttackList.Interrupt;
            squishFactor = Vector2.One;
            if (Main.netMode == NetmodeID.MultiplayerClient)
                NPC.netUpdate = true;

        }

        private Vector2 saveTarget;
        private float opacity;

        public void StarSigns()
        {
            NPC.damage = 0;
            NPC.dontTakeDamage = true;
            NPC.velocity *= 0.2f;

            if (Time >= 20 && Time < 80)
            {
            }

            if (Time == 35)
            {
                NPC.scale = 0.66f;

                int count = 17 + Main.rand.Next(5, 8);
                for (int i = 0; i < count; i++)
                {
                    Vector2 target = NPC.Center + new Vector2(Main.rand.Next(2200, 3500), 0).RotatedBy(MathHelper.TwoPi / count * i).RotatedByRandom(0.1f);
                    Projectile star = Projectile.NewProjectileDirect(NPC.GetSource_FromThis(), NPC.Center, GetDesiredVelocityForDistance(NPC.Center, target), ModContent.ProjectileType<ConstellationStar>(), GetDamage(1), 0);
                    star.direction = Main.rand.NextBool() ? -1 : 1;
                    star.localAI[0] = i / (float)count;
                    star.ai[0] = 0;
                    star.ai[1] = 0;
                }
            }
            if (Time == 53)
            {
                NPC.scale = 0.33f;

                int count = 15 + Main.rand.Next(4, 8);
                for (int i = 0; i < count; i++)
                {
                    Vector2 target = NPC.Center + new Vector2(Main.rand.Next(2000, 3000), 0).RotatedBy(MathHelper.TwoPi / count * i).RotatedByRandom(0.2f);
                    Projectile star = Projectile.NewProjectileDirect(NPC.GetSource_FromThis(), NPC.Bottom, GetDesiredVelocityForDistance(NPC.Center, target), ModContent.ProjectileType<ConstellationStar>(), GetDamage(1), 0);
                    star.direction = Main.rand.NextBool() ? -1 : 1;
                    star.localAI[0] = i / (float)count;
                    star.ai[0] = -18;
                    star.ai[1] = 1;
                }
            }
            if (Time == 70)
            {
                NPC.scale = 0f;

                int count = 12 + Main.rand.Next(8, 12);
                for (int i = 0; i < count; i++)
                {
                    Vector2 target = NPC.Center + new Vector2(Main.rand.Next(50, 1000), 0).RotatedBy(MathHelper.TwoPi / count * i).RotatedByRandom(0.3f);
                    Projectile star = Projectile.NewProjectileDirect(NPC.GetSource_FromThis(), NPC.Bottom, GetDesiredVelocityForDistance(NPC.Center, target), ModContent.ProjectileType<ConstellationStar>(), GetDamage(1), 0);
                    star.direction = Main.rand.NextBool() ? -1 : 1;
                    star.localAI[0] = i / (float)count;
                    star.ai[0] = i * 2;
                    star.ai[1] = 2;
                }

                for (int i = 0; i < 8; i++)
                {
                    Vector2 target = NPC.Center + new Vector2(Main.rand.Next(10, 500), 0).RotatedBy(MathHelper.TwoPi / count * i).RotatedByRandom(0.3f);

                    Projectile star = Projectile.NewProjectileDirect(NPC.GetSource_FromThis(), NPC.Bottom, GetDesiredVelocityForDistance(NPC.Center, target), ModContent.ProjectileType<ConstellationStar>(), GetDamage(1), 0);
                    star.direction = Main.rand.NextBool() ? -1 : 1;
                    star.localAI[0] = i / (float)count;
                    star.ai[0] = -35;
                    star.ai[1] = 2;
                }
            }

            if (Time > 70 && Time < 521)
            {
                if ((Time - 70) % 150 == 5)
                    SpawnConstellation(0, 8);
                if ((Time - 70) % 150 == 25)
                    SpawnConstellation(1, 12);
                if ((Time - 70) % 150 == 45)
                    SpawnConstellation(2, 10);
            }

            if (Time > 680)
            {
                NPC.active = false;
            }
        }

        private void SpawnConstellation(int checkType, int lineCount)
        {
            switch (checkType)
            {
                case 0:

                    for (int i = 0; i < lineCount; i++)
                    {
                        Func<Projectile, bool> check1 = n =>
                        n.active && n.type == ModContent.ProjectileType<ConstellationStar>() &&
                        n.ai[1] == 0 &&
                        n.ai[2] == 0;                        
                        if (Main.projectile.Any(check1))
                        {
                            Projectile firstStar = Main.rand.Next(Main.projectile.Where(check1).ToArray());
                            firstStar.ai[2] = 1;

                            for (int j = 0; j < Main.rand.Next(1, 2); j++)
                            {
                                Func<Projectile, bool> check2 = n =>
                                n.active && n.type == ModContent.ProjectileType<ConstellationStar>() &&
                                n.Distance(firstStar.Center) > 250 &&
                                n.Distance(firstStar.Center) < 1500 &&
                                n.ai[1] < 2 &&
                                n.ai[2] == 0;

                                if (!Main.projectile.Any(check2))
                                    break;

                                Projectile secondStar = Main.rand.Next(Main.projectile.Where(check2).ToArray());
                                secondStar.ai[2] = 1;

                                Projectile line = Projectile.NewProjectileDirect(NPC.GetSource_FromThis(), NPC.Center, Vector2.Zero, ModContent.ProjectileType<ConstellationLine>(), GetDamage(1), 0);
                                line.ai[1] = firstStar.whoAmI;
                                line.ai[2] = secondStar.whoAmI;
                            }
                        }
                    }
                    break;

                case 1:

                    for (int i = 0; i < lineCount; i++)
                    {
                        Func<Projectile, bool> check1 = n => n.active && n.type == ModContent.ProjectileType<ConstellationStar>() && 
                        n.ai[1] == 0 &&
                        n.ai[2] == 1;
                        if (Main.projectile.Any(check1))
                        {
                            Projectile firstStar = Main.rand.Next(Main.projectile.Where(check1).ToArray());
                            firstStar.ai[2] = 2;

                            for (int j = 0; j < Main.rand.Next(1, 3); j++)
                            {
                                Func<Projectile, bool> check2 = n =>
                                n.active && n.type == ModContent.ProjectileType<ConstellationStar>() &&
                                n.Distance(firstStar.Center) > 250 &&
                                n.Distance(firstStar.Center) < 1000 &&
                                n.ai[1] == 1 &&
                                n.ai[2] == 0;

                                if (!Main.projectile.Any(check2))
                                    break;

                                Projectile secondStar = Main.rand.Next(Main.projectile.Where(check2).ToArray());
                                secondStar.ai[2] = 1;

                                Projectile line = Projectile.NewProjectileDirect(NPC.GetSource_FromThis(), NPC.Center, Vector2.Zero, ModContent.ProjectileType<ConstellationLine>(), GetDamage(1), 0);
                                line.ai[1] = firstStar.whoAmI;
                                line.ai[2] = secondStar.whoAmI;
                            }
                        }
                    }

                    break;

                case 2:

                    for (int i = 0; i < lineCount; i++)
                    {
                        Func<Projectile, bool> check1 = n => n.active && n.type == ModContent.ProjectileType<ConstellationStar>() && 
                        n.ai[1] == 1 &&
                        n.ai[2] == 1;
                        if (Main.projectile.Any(check1))
                        {
                            Projectile firstStar = Main.rand.Next(Main.projectile.Where(check1).ToArray());
                            firstStar.ai[2] = 2;

                            for (int j = 0; j < Main.rand.Next(1, 3); j++)
                            {
                                Func<Projectile, bool> check2 = n =>
                                n.active && n.type == ModContent.ProjectileType<ConstellationStar>() &&
                                n.Distance(firstStar.Center) > 250 &&
                                n.Distance(firstStar.Center) < 1000 &&
                                ((n.ai[1] == 1 && n.ai[2] == 0) ||
                                (n.ai[1] == 2 && n.ai[2] == 0));

                                if (!Main.projectile.Any(check2))
                                    break;

                                Projectile secondStar = Main.rand.Next(Main.projectile.Where(check2).ToArray());
                                secondStar.ai[2] = 1;

                                Projectile line = Projectile.NewProjectileDirect(NPC.GetSource_FromThis(), NPC.Center, Vector2.Zero, ModContent.ProjectileType<ConstellationLine>(), GetDamage(1), 0);
                                line.ai[1] = firstStar.whoAmI;
                                line.ai[2] = secondStar.whoAmI;
                            }
                        }
                    }                    
                    
                    for (int i = 0; i < lineCount; i++)
                    {
                        Func<Projectile, bool> check1 = n => n.active && n.type == ModContent.ProjectileType<ConstellationStar>() && 
                        n.ai[1] == 2 &&
                        n.ai[2] == 0;
                        if (Main.projectile.Any(check1))
                        {
                            Projectile firstStar = Main.rand.Next(Main.projectile.Where(check1).ToArray());
                            firstStar.ai[2] = 2;

                            for (int j = 0; j < Main.rand.Next(1, 3); j++)
                            {
                                Func<Projectile, bool> check2 = n =>
                                n.active && n.type == ModContent.ProjectileType<ConstellationStar>() &&
                                n.Distance(firstStar.Center) > 250 &&
                                n.Distance(firstStar.Center) < 1200 &&
                                ((n.ai[1] == 1 && n.ai[2] == 1) ||
                                (n.ai[1] == 2 && n.ai[2] == 0));

                                if (!Main.projectile.Any(check2))
                                    break;

                                Projectile secondStar = Main.rand.Next(Main.projectile.Where(check2).ToArray());
                                secondStar.ai[2] = 1;

                                Projectile line = Projectile.NewProjectileDirect(NPC.GetSource_FromThis(), NPC.Center, Vector2.Zero, ModContent.ProjectileType<ConstellationLine>(), GetDamage(1), 0);
                                line.ai[1] = firstStar.whoAmI;
                                line.ai[2] = secondStar.whoAmI;
                            }
                        }
                    }

                    break;

            }
        }

        private Vector2 GetDesiredVelocityForDistance(Vector2 start, Vector2 end)
        {
            Vector2 velocity = start.DirectionTo(end).SafeNormalize(Vector2.Zero);
            float velocityFactor = (float)Math.Sqrt(start.Distance(end)) * 1.84f;
            return velocity * velocityFactor;
        }

        public void Starfall()
        {
            int gelTime = 20;
            if (Main.expertMode)
                gelTime = 15;

            if (Time < 40)
            {
                NPC.velocity *= 0.8f;
                squishFactor = Vector2.Lerp(Vector2.One, new Vector2(1.5f, 0.4f), (float)Math.Sqrt(Time / 40f));
            }

            else if (Time < 100)
            {
                NPC.velocity *= 0.9f;

                squishFactor = Vector2.SmoothStep(new Vector2(1.5f, 0.4f), new Vector2(0.7f, 1.5f), (float)Math.Pow(Utils.GetLerpValue(40, 80, Time, true), 2));

                if (Time > 80)
                {
                    int count = 1;
                    if (Main.expertMode)
                        count = 2;

                    for (int i = 0; i < count; i++)
                    {
                        Projectile gelatin = Projectile.NewProjectileDirect(NPC.GetSource_FromThis(), NPC.Center + Main.rand.NextVector2Circular(30, 40), -Vector2.UnitY.RotatedByRandom(1f) * Main.rand.NextFloat(5f, 10f), ModContent.ProjectileType<StellarGelatine>(), GetDamage(1), 0);
                        gelatin.ai[0] = Time - 140 - Main.projectile.Count(n => n.active && n.type == ModContent.ProjectileType<StellarGelatine>()) * gelTime;
                        gelatin.ai[1] = Main.projectile.Count(n => n.active && n.type == ModContent.ProjectileType<StellarGelatine>()) * gelTime;
                    }
                }
            }
            else
            {
                squishFactor = Vector2.Lerp(squishFactor, Vector2.One, 0.1f);
            }
            if (Time < 1200)
                NPC.velocity = Vector2.Lerp(NPC.velocity, NPC.DirectionTo(Target.Center - new Vector2(0, 300)).SafeNormalize(Vector2.Zero) * NPC.Distance(Target.Center) * 0.05f, 0.02f);

            NPC.rotation = NPC.velocity.X * 0.02f;

            if (Time > 1500)
                Reset();
        }

        public void BlackHole()
        {
            
        }

        private int GetDamage(int attack, float modifier = 1f)
        {
            int damage = attack switch
            {
                0 => 70,//contact
                1 => 40,//constellation star
                _ => 0
            };

            return (int)(damage * modifier);
        }

        public int npcFrame;

        public override void FindFrame(int frameHeight)
        {
            NPC.frameCounter++;
            if (NPC.frameCounter > 7)
            {
                NPC.frameCounter = 0;
                npcFrame = (npcFrame + 1) % Main.npcFrameCount[Type];
            }
        }

        public override bool PreDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {
            Asset<Texture2D> texture = ModContent.Request<Texture2D>(Texture);
            Rectangle frame = texture.Frame(1, 4, 0, npcFrame);
            Asset<Texture2D> bloom = ModContent.Request<Texture2D>($"{nameof(CalamityHunt)}/Assets/Textures/Goozma/GlowSoft");
            Asset<Texture2D> tell = TextureAssets.Extra[178];
            Asset<Texture2D> flare = TextureAssets.Extra[98];

            Color color = Color.White;
            switch (Attack)
            {
                case (int)AttackList.TooFar:
                    color = Color.Lerp(new Color(100, 100, 100, 0), Color.White, Math.Clamp(NPC.Distance(Target.Center), 100, 300) / 200f);
                    break;
            }
            Effect effect = ModContent.Request<Effect>($"{nameof(CalamityHunt)}/Assets/Effects/CosmosEffect", AssetRequestMode.ImmediateLoad).Value;
            effect.Parameters["uTextureClose"].SetValue(ModContent.Request<Texture2D>($"{nameof(CalamityHunt)}/Assets/Textures/Space0").Value);
            effect.Parameters["uTextureFar"].SetValue(ModContent.Request<Texture2D>($"{nameof(CalamityHunt)}/Assets/Textures/Space1").Value);
            effect.Parameters["uPosition"].SetValue(Main.screenPosition * 0.001f);
            effect.Parameters["uParallax"].SetValue(new Vector2(0.125f, 0.25f));
            effect.Parameters["uScrollClose"].SetValue(new Vector2(Main.GlobalTimeWrappedHourly * 0.004f % 2f, Main.GlobalTimeWrappedHourly * 0.014f % 2f));
            effect.Parameters["uScrollFar"].SetValue(new Vector2(-Main.GlobalTimeWrappedHourly * 0.01f % 2f, Main.GlobalTimeWrappedHourly * 0.012f % 2f));
            effect.Parameters["uCloseColor"].SetValue(Color.IndianRed.ToVector3());
            effect.Parameters["uFarColor"].SetValue(Color.Indigo.ToVector3());
            effect.Parameters["uOutlineColor"].SetValue(Color.White.ToVector3());
            effect.Parameters["uImageRatio"].SetValue(new Vector2(Main.screenWidth / (float)Main.screenHeight, 1f));

            if (NPC.IsABestiaryIconDummy)
            {
                //RasterizerState priorRrasterizerState = spriteBatch.GraphicsDevice.RasterizerState;
                //Rectangle priorScissorRectangle = spriteBatch.GraphicsDevice.ScissorRectangle;
                //spriteBatch.End();
                //spriteBatch.GraphicsDevice.RasterizerState = priorRrasterizerState;
                //spriteBatch.GraphicsDevice.ScissorRectangle = priorScissorRectangle;
                //effect.Parameters["uPosition"].SetValue(Vector2.Zero);

                //spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, Main.DefaultSamplerState, DepthStencilState.None, Main.Rasterizer, effect, Main.UIScaleMatrix);

                spriteBatch.Draw(texture.Value, NPC.Center - screenPos, frame, color, NPC.rotation, frame.Size() * 0.5f, NPC.scale * squishFactor, 0, 0);

                //spriteBatch.End();
                //spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, Main.DefaultSamplerState, DepthStencilState.None, Main.Rasterizer, null, Main.UIScaleMatrix);
            }
            else
            {
                //spriteBatch.End();
                //spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, Main.DefaultSamplerState, DepthStencilState.None, Main.Rasterizer, effect, Main.Transform);

                spriteBatch.Draw(texture.Value, NPC.Center - screenPos, frame, color, NPC.rotation, frame.Size() * 0.5f, NPC.scale * squishFactor, 0, 0);

                //spriteBatch.End();
                //spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, Main.DefaultSamplerState, DepthStencilState.None, Main.Rasterizer, null, Main.Transform);
            }

            return false;
        }
    }
}
