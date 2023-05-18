using CalamityHunt.Common.Systems.Particles;
using CalamityHunt.Content.Bosses.Goozma.Projectiles;
using CalamityHunt.Content.Gores.CrystalShieldGores;
using CalamityHunt.Content.Particles;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using System;
using System.Collections.Generic;
using System.Drawing.Imaging;
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
    public class DivineGargooptuar : ModNPC
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
                new FlavorTextBestiaryInfoElement($"Mods.{nameof(CalamityHunt)}.Bestiary.DivineGargooptuar"),
                BestiaryDatabaseNPCsPopulator.CommonTags.SpawnConditions.Events.SlimeRain,
                BestiaryDatabaseNPCsPopulator.CommonTags.SpawnConditions.Biomes.TheHallow,
            });
        }

        public override void SetDefaults()
        {
            NPC.width = 140;
            NPC.height = 120;
            NPC.damage = 12;
            NPC.defense = 100;
            NPC.lifeMax = 3500000;
            NPC.takenDamageMultiplier = 0.5f;
            NPC.HitSound = SoundID.NPCHit1;
            NPC.DeathSound = SoundID.NPCDeath1;
            NPC.knockBackResist = 0f;
            NPC.noGravity = true;
            NPC.noTileCollide = true;
            NPC.value = Item.buyPrice(gold: 5);
            NPC.SpawnWithHigherTime(30);
            NPC.npcSlots = 10f;
            NPC.aiStyle = -1;
            if (ModLoader.HasMod("CalamityMod"))
            {
                Mod calamity = ModLoader.GetMod("CalamityMod");
                calamity.Call("SetDebuffVulnerabilities", "poison", false);
                calamity.Call("SetDebuffVulnerabilities", "heat", true);
                calamity.Call("SetDefenseDamageNPC", Type, true);
            }
        }

        private enum AttackList
        {
            PrismDestroyer,
            CrystalStorm,
            PixieBall,
            TooFar,
            Interrupt,
            BlowUp = -1
        }

        public ref float Time => ref NPC.ai[0];
        public ref float Attack => ref NPC.ai[1];
        public ref NPC Host => ref Main.npc[(int)NPC.ai[2]];
        public ref float RememberAttack => ref NPC.ai[3];

        public NPCAimedTarget Target => NPC.GetTargetData();
        public Vector2 squishFactor = Vector2.One;

        public override void AI()
        {
            if (NPC.ai[2] < 0)
                NPC.ai[2] = Main.npc.First(n => n.type == ModContent.NPCType<Goozma>() && n.active).whoAmI;
            if (!Main.npc.Any(n => n.type == ModContent.NPCType<Goozma>() && n.active))
                NPC.active = false;

            if (wingFrame == 2 && NPC.frameCounter % 8 == 0)
                SoundEngine.PlaySound(SoundID.Item32.WithVolumeScale(2f), NPC.Center);

            NPC.realLife = Host.whoAmI;

            if (!NPC.HasPlayerTarget)
                NPC.TargetClosestUpgraded();
            if (!NPC.HasPlayerTarget)
                NPC.active = false;

            NPC.damage = 0;

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
                    case (int)AttackList.PrismDestroyer:
                        PrismDestroyer();
                        break;

                    case (int)AttackList.CrystalStorm:
                        CrystalStorm();
                        break;
                                            
                    case (int)AttackList.PixieBall:
                        PixieBall();
                        break;                                                      
                    
                    case (int)AttackList.BlowUp:
                        BlowUp();
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

            if (Main.rand.NextBool(10))
                Particle.NewParticle(Particle.ParticleType<PrettySparkle>(), NPC.Center + Main.rand.NextVector2Circular(100, 80), Main.rand.NextVector2Circular(3, 3), Main.hslToRgb(NPC.localAI[0] * 0.1f % 1f, 0.5f, 0.7f, 0), 0.3f + Main.rand.NextFloat());

            Time++;
            NPC.localAI[0]++;
        }

        private void Reset()
        {
            Time = 0;
            Attack = (int)AttackList.Interrupt;
            squishFactor = Vector2.One;

            for (int i = 0; i < 10; i++)
                Dust.NewDustPerfect(NPC.Center + Main.rand.NextVector2Circular(40, 20), DustID.TintableDust, Main.rand.NextVector2Circular(15, 8), 200, Color.Pink, 1.5f);

            if (Main.netMode == NetmodeID.MultiplayerClient)
                NPC.netUpdate = true;
        }

        private Vector2 saveTarget;

        private void PrismDestroyer()
        {
            int danceCount = 3;
            int prismCount = 3;
            if (Main.expertMode)
                prismCount = 5;

            NPC.rotation = NPC.velocity.X * 0.02f;

            if (Time < danceCount * 150)
            {
                if (Time % 150 == 2)
                { 
                    float randSpin = Main.rand.NextFloat(-3f, 3f);
                    for (int i = 0; i < prismCount; i++)
                    {
                        Projectile prism = Projectile.NewProjectileDirect(NPC.GetSource_FromAI(), NPC.Center, Vector2.Zero, ModContent.ProjectileType<PrismDestroyer>(), GetDamage(1), 0);
                        prism.direction = (Main.rand.Next(5) > 2 ? 1 : -1);
                        prism.rotation = Main.rand.NextFloat(-2f, 2f);
                        prism.ai[0] = -66 + i;
                        prism.ai[1] = i;
                        prism.ai[2] = randSpin;
                    }              

                    SoundEngine.PlaySound(SoundID.QueenSlime, NPC.Center);
                }

                if (Time % 150 > 70 && Time % 150 < 100 && Time % 3 == 0)
                    Main.instance.CameraModifiers.Add(new PunchCameraModifier(saveTarget, Main.rand.NextVector2CircularEdge(3, 3), 4f * Utils.GetLerpValue(100, 70, Time % 150, true), 10, 12));

                if (Time % 150 == 5)
                {
                    SoundStyle createSound = new SoundStyle($"{nameof(CalamityHunt)}/Assets/Sounds/Goozma/Slimes/PrismDestroyerTelegraph");
                    createSound.PitchVariance = 0.1f;
                    createSound.MaxInstances = 0;
                    SoundEngine.PlaySound(createSound, NPC.Center);

                    for (int i = 0; i < Main.rand.Next(14, 20); i++)
                        Particle.NewParticle(Particle.ParticleType<HolyBombChunk>(), NPC.Center + Main.rand.NextVector2Circular(80, 50), NPC.velocity.RotatedByRandom(1f) * Main.rand.NextFloat(), Color.White, 0.1f + Main.rand.NextFloat(2f));

                }

                if (Time % 150 == 70)
                {
                    SoundStyle expandSound = new SoundStyle($"{nameof(CalamityHunt)}/Assets/Sounds/Goozma/Slimes/PrismDestroyerExpand");
                    expandSound.PitchVariance = 0.1f;
                    expandSound.MaxInstances = 0;
                    SoundEngine.PlaySound(expandSound, NPC.Center);
                }

                foreach (Projectile proj in Main.projectile.Where(n => n.active && n.type == ModContent.ProjectileType<PrismDestroyer>() && n.ai[0] < 2))
                    proj.ai[2] += (NPC.velocity.X > 0 ? 1 : -1) * 0.07f * Utils.GetLerpValue(53, 10, Time % 150, true);

                NPC.velocity = Vector2.Lerp(NPC.velocity, NPC.DirectionTo(Target.Center).SafeNormalize(Vector2.Zero) * NPC.Distance(Target.Center) * 0.07f * Utils.GetLerpValue(147, 40, Time % 150, true), 0.1f + 0.1f * Utils.GetLerpValue(170, 40, Time % 150, true));

                float progress = MathHelper.SmoothStep(0, 1, Utils.GetLerpValue(0, 110, (Time - 10) % 150, true) * Utils.GetLerpValue(150, 110, (Time - 10) % 150, true));
                squishFactor = Vector2.Lerp(squishFactor, new Vector2(1f + (float)Math.Cos(progress * MathHelper.Pi) * 0.2f, 1f + (float)Math.Cos(progress * MathHelper.Pi + MathHelper.Pi) * 0.4f), 0.3f);
            }
            else 
            {
                NPC.velocity *= 0.8f;
                squishFactor = Vector2.Lerp(squishFactor, Vector2.One, 0.1f);
            }

            NPC.damage = 0;

            if (Time > danceCount * 150 + 30)
                Reset();

        }

        private void CrystalStorm()
        {
            int lengthOfAttack = 240;
            if (Main.expertMode)
                lengthOfAttack = 360;
            int crystalFrequency = 1;
            if (Main.expertMode)
                crystalFrequency = 3;

            if (Time < 40)
            {
                NPC.velocity = Vector2.Lerp(NPC.velocity, NPC.DirectionTo(Target.Center + new Vector2(0, -170)).SafeNormalize(Vector2.Zero) * NPC.Distance(Target.Center + new Vector2(0, -170)) * 0.2f, 0.1f);
                squishFactor = Vector2.Lerp(squishFactor, new Vector2(1.5f, 0.6f), Time / 60f);
            }
            else
            {
                Vector2 wobble = Vector2.Lerp(new Vector2(0.5f, 1.6f), new Vector2(1.3f, 1f), 0.5f + (float)Math.Sin(Time * 0.2f) * 0.5f);
                squishFactor = Vector2.Lerp(Vector2.Lerp(squishFactor, wobble, Utils.GetLerpValue(40, lengthOfAttack, Time, true) * 0.2f), Vector2.Lerp(squishFactor, Vector2.One, 0.1f), Utils.GetLerpValue(40 + lengthOfAttack, 110 + lengthOfAttack, Time, true)); 
                NPC.velocity = Vector2.Lerp(NPC.velocity, NPC.DirectionTo(Target.Center + new Vector2(0, -170)).SafeNormalize(Vector2.Zero) * NPC.Distance(Target.Center + new Vector2(0, -170)) * 0.2f, 0.2f + (float)Math.Sin(Time * 0.3f) * 0.1f);

                if (Time <= lengthOfAttack + 100 && Time % 15 == 0)
                    for (int i = 0; i < crystalFrequency; i++)
                        Projectile.NewProjectile(NPC.GetSource_FromAI(), NPC.Top, Main.rand.NextVector2Circular(8, 5) - Vector2.UnitY * 12 + NPC.DirectionTo(Target.Center).SafeNormalize(Vector2.Zero) * 5f, ModContent.ProjectileType<GelCrystalShard>(), GetDamage(2), 0);
            }
            if (Time > lengthOfAttack)
                NPC.velocity *= 0.9f;

            if (Time > lengthOfAttack + 110)
                Reset();
        }        

        private void PixieBall()
        {
            NPC.rotation = NPC.velocity.X * 0.022f;

            if (Time < 60)
            {
                if (Time == 50)
                {
                    NPC.velocity = Vector2.UnitY * 5f;
                    Projectile.NewProjectileDirect(NPC.GetSource_FromAI(), Host.Center, Host.DirectionTo(Target.Center - Vector2.UnitY * 15).SafeNormalize(Vector2.Zero), ModContent.ProjectileType<PixieBall>(), 0, 0, ai1: 15, ai2: -1);

                    //if (!Main.dedServ)
                    //{
                    //    SoundEngine.PlaySound(SoundID.Item147, NPC.Center);
                    //}
                }

                NPC.velocity *= 0.9f;

                if (Time < 50)
                    squishFactor = Vector2.Lerp(Vector2.One, new Vector2(1.5f, 0.6f), (float)Math.Sqrt(Utils.GetLerpValue(0, 50, Time, true)));
                else
                    squishFactor = Vector2.SmoothStep(new Vector2(1.5f, 0.8f), Vector2.One, (float)Math.Sqrt(Utils.GetLerpValue(50, 60, Time, true)));
            }
            else if (Time < 500)
            {
                squishFactor = Vector2.Lerp(squishFactor, new Vector2(1f + (float)Math.Cos(Time * 0.05f) * 0.2f, 1f + (float)Math.Cos(Time * 0.05f + MathHelper.Pi) * 0.2f), 0.3f);
                NPC.velocity = Vector2.Lerp(NPC.velocity, NPC.DirectionTo(Target.Center).SafeNormalize(Vector2.Zero) * NPC.Distance(Target.Center) * 0.01f, 0.1f);

                //if (Time % 80 == 0)
                //    for (int i = 0; i < Main.rand.Next(10, 30); i++)
                //        Projectile.NewProjectile(NPC.GetSource_FromAI(), NPC.Center, Main.rand.NextVector2Circular(10, 10) - Vector2.UnitY * 10, ModContent.ProjectileType<GelCrystalShard>(), GetDamage(3), 0);
            }
            else
            {
                NPC.localAI[1]++;
                NPC.velocity *= 0.9f;
            }

            if (Time == 60)
                SoundEngine.PlaySound(SoundID.Shatter, NPC.Center);

            if (Time > 60)
            {
                if (NPC.localAI[1] == 1)
                {
                    NPC.localAI[1]++;

                    for (int i = 0; i < 2; i++)
                        Gore.NewGore(NPC.GetSource_FromThis(), NPC.Center + Main.rand.NextVector2Circular(50, 50), Main.rand.NextVector2Circular(9, 9), ModContent.GoreType<CrystalShieldFragment0>(), 1f);
                    
                    for (int i = 0; i < 3; i++)
                        Gore.NewGore(NPC.GetSource_FromThis(), NPC.Center + Main.rand.NextVector2Circular(50, 50), Main.rand.NextVector2Circular(9, 9), ModContent.GoreType<CrystalShieldFragment1>(), 1f);
                   
                    Gore.NewGore(NPC.GetSource_FromThis(), NPC.Center + Main.rand.NextVector2Circular(50, 50), Main.rand.NextVector2Circular(9, 9), ModContent.GoreType<CrystalShieldFragment2>(), 1f);

                    for (int i = 0; i < 3; i++)
                        Gore.NewGore(NPC.GetSource_FromThis(), NPC.Center + Main.rand.NextVector2Circular(50, 50), Main.rand.NextVector2Circular(9, 9), ModContent.GoreType<CrystalShieldFragment3>(), 1f);
                    
                    for (int i = 0; i < 5; i++)
                        Gore.NewGore(NPC.GetSource_FromThis(), NPC.Center + Main.rand.NextVector2Circular(50, 50), Main.rand.NextVector2Circular(9, 9), ModContent.GoreType<CrystalShieldFragment4>(), 1f);
                    
                    for (int i = 0; i < 2; i++)
                        Gore.NewGore(NPC.GetSource_FromThis(), NPC.Center + Main.rand.NextVector2Circular(50, 50), Main.rand.NextVector2Circular(9, 9), ModContent.GoreType<CrystalShieldFragment5>(), 1f);
                    
                    for (int i = 0; i < 3; i++)
                        Gore.NewGore(NPC.GetSource_FromThis(), NPC.Center + Main.rand.NextVector2Circular(50, 50), Main.rand.NextVector2Circular(9, 9), ModContent.GoreType<CrystalShieldFragment6>(), 1f);
                    
                    for (int i = 0; i < 7; i++)
                        Gore.NewGore(NPC.GetSource_FromThis(), NPC.Center + Main.rand.NextVector2Circular(50, 50), Main.rand.NextVector2Circular(9, 9), ModContent.GoreType<CrystalShieldFragment7>(), 1f);
                    
                    for (int i = 0; i < 9; i++)
                        Gore.NewGore(NPC.GetSource_FromThis(), NPC.Center + Main.rand.NextVector2Circular(50, 50), Main.rand.NextVector2Circular(11, 11), ModContent.GoreType<CrystalShieldFragment8>(), 1f);
                    
                    for (int i = 0; i < 14; i++)
                        Gore.NewGore(NPC.GetSource_FromThis(), NPC.Center + Main.rand.NextVector2Circular(50, 50), Main.rand.NextVector2Circular(12, 12), ModContent.GoreType<CrystalShieldFragment9>(), 1f);

                    SoundStyle shatter = new SoundStyle($"{nameof(CalamityHunt)}/Assets/Sounds/Goozma/Slimes/PixiePrismDestroyed");
                    shatter.MaxInstances = 0;
                    SoundEngine.PlaySound(shatter, NPC.Center);
                }
            }

            if (Main.projectile.Any(n => n.active && n.type == ModContent.ProjectileType<PixieBall>()))
            {
                Projectile ball = Main.projectile.First(n => n.active && n.type == ModContent.ProjectileType<PixieBall>());
                pixieBallDangerShine = Utils.GetLerpValue(900, 50, ball.Distance(NPC.Center), true);
            }

            //foreach (Projectile proj in Main.projectile.Where(n => n.type == ModContent.ProjectileType<PixieBall>() && n.active))
            //{
            //    if (proj.Distance(NPC.Center) < 50)
            //    {
            //        Time = 0;
            //        Attack = (int)AttackList.BlowUp;
            //        if (Main.netMode == NetmodeID.MultiplayerClient)
            //            NPC.netUpdate = true;
            //    }
            //}

            if (Time > 600)
                Reset();
        }

        private void BlowUp()
        {
            NPC.velocity *= 0.9f;
            squishFactor = Vector2.Lerp(squishFactor * 0.98f, squishFactor * 1.1f, 0.05f + Time / 60f);
            NPC.frameCounter += 2;

            if (Time == 1)
                Projectile.NewProjectile(NPC.GetSource_FromThis(), NPC.Center, Vector2.Zero, ModContent.ProjectileType<HolyExplosion>(), 0, 0);

            if (Time > 30)
            {
                foreach (Player player in Main.player.Where(n => n.active && !n.dead && n.Distance(NPC.Center) < 8000))
                {
                    player.Hurt(PlayerDeathReason.ByCustomReason($"{player.name} saw the light."), 9999, -1, false, true, 200, false, 0, 0, 0);
                    player.statLife = Math.Min(5, player.statLife);
                    player.immuneTime += 120;
                }
                
                NPC.active = false;
                Host.ai[0] = 0;
                Host.ai[3] = -1;
            }
        }

        private int GetDamage(int attack, float modifier = 1f)
        {
            int damage = attack switch
            {
                0 => 70,//contact
                1 => 40,//Prism Destroyer
                2 => 30,//Crystal Storm
                3 => 5,//Pixieball Crystal Shard
                4 => 0,//Explosion
                _ => 0
            };

            return (int)(damage * modifier);
        }

        public override void FindFrame(int frameHeight)
        {
            NPC.frameCounter++;
            if (NPC.frameCounter % 7 == 0)
                npcFrame = (npcFrame + 1) % Main.npcFrameCount[Type];

            if (NPC.frameCounter % 8 == 0 && Time >= 0)
                wingFrame = (wingFrame + 1) % 4;

            if (NPC.frameCounter > 56)
                NPC.frameCounter = 0;

        }

        public int npcFrame;
        public int wingFrame;
        public float shineStrength;
        public float pixieBallDangerShine;

        public override bool PreDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {
            Asset<Texture2D> texture = ModContent.Request<Texture2D>(Texture);
            Asset<Texture2D> shineTexture = ModContent.Request<Texture2D>(Texture + "Shine");
            Asset<Texture2D> trailTexture = ModContent.Request<Texture2D>(Texture + "Trail");
            Asset<Texture2D> core = ModContent.Request<Texture2D>($"{nameof(CalamityHunt)}/Assets/Textures/Goozma/Crowns/CrystalMine");
            Asset<Texture2D> wings = TextureAssets.Extra[185];
            Asset<Texture2D> dangerTexture = ModContent.Request<Texture2D>(Texture + "Danger");
            Asset<Texture2D> dangerShield = ModContent.Request<Texture2D>(Texture + "DangerShield");
            Asset<Texture2D> dangerShineTexture = ModContent.Request<Texture2D>(Texture + "DangerShine");
            Asset<Texture2D> flare = TextureAssets.Extra[89];
            Asset<Texture2D> bloom = ModContent.Request<Texture2D>($"{nameof(CalamityHunt)}/Assets/Textures/Goozma/GlowSoftBig");
            Asset<Texture2D> ring = ModContent.Request<Texture2D>($"{nameof(CalamityHunt)}/Assets/Textures/Goozma/GlowRing");

            Rectangle frame = texture.Frame(1, 4, 0, npcFrame);
            Rectangle wingRect = wings.Frame(1, 4, 0, wingFrame);

            Color color = Color.White;

            switch (Attack)
            {
                case (int)AttackList.TooFar:
                    color = Color.Lerp(new Color(100, 100, 100, 0), Color.White, Math.Clamp(NPC.Distance(Target.Center), 100, 300) / 200f);
                    break;
            }
            
            if (Attack != (int)AttackList.PixieBall)
                for (int i = 0; i < NPCID.Sets.TrailCacheLength[Type]; i++)
                {
                    Vector2 oldPos = NPC.oldPos[i] + NPC.Size * new Vector2(0.5f, 1f);
                    Color trailColor = Main.hslToRgb((NPC.localAI[0] * 0.03f - ((float)i / NPCID.Sets.TrailCacheLength[Type])) % 1f, 1f, 0.6f, 0) * Math.Clamp(NPC.velocity.Length() * 0.01f, 0, 1) * (1f - (float)i / NPCID.Sets.TrailCacheLength[Type]) * 0.6f;
                    spriteBatch.Draw(trailTexture.Value, oldPos - screenPos, frame, trailColor.MultiplyRGBA(color), NPC.rotation, frame.Size() * new Vector2(0.5f, 1f), NPC.scale * squishFactor, 0, 0);
                }

            Color rainbowColor = Main.hslToRgb((NPC.localAI[0] * 0.03f) % 1f, 1f, 0.7f, 0) * 0.8f;
            Vector2 corePos = NPC.Bottom + new Vector2(0, -50 - (float)Math.Cos(npcFrame * MathHelper.PiOver2)) * squishFactor;
            if ((Attack == (int)AttackList.PixieBall && (Time > 50 && Time < 540)) || Attack == (int)AttackList.BlowUp)
                corePos = NPC.Center + new Vector2(0, 4 - (float)Math.Cos(npcFrame * MathHelper.PiOver2)) * squishFactor; ;
            Vector2 leftWingPos = corePos + new Vector2(-12, 2).RotatedBy(NPC.rotation) * NPC.scale;
            Vector2 rightWingPos = corePos + new Vector2(12, 2).RotatedBy(NPC.rotation) * NPC.scale;
            
            spriteBatch.Draw(wings.Value, leftWingPos - screenPos, wingRect, color, NPC.rotation * 0.5f, wingRect.Size() * new Vector2(1f, 0.5f), NPC.scale, 0, 0);
            spriteBatch.Draw(wings.Value, rightWingPos - screenPos, wingRect, color, NPC.rotation * 0.5f, wingRect.Size() * new Vector2(0f, 0.5f), NPC.scale, SpriteEffects.FlipHorizontally, 0);
            
            spriteBatch.Draw(wings.Value, leftWingPos - screenPos, wingRect, color.MultiplyRGBA(rainbowColor), NPC.rotation * 0.5f, wingRect.Size() * new Vector2(1f, 0.5f), NPC.scale * 1.02f, 0, 0);
            spriteBatch.Draw(wings.Value, rightWingPos - screenPos, wingRect, color.MultiplyRGBA(rainbowColor), NPC.rotation * 0.5f, wingRect.Size() * new Vector2(0f, 0.5f), NPC.scale * 1.02f, SpriteEffects.FlipHorizontally, 0);

            spriteBatch.Draw(core.Value, corePos - screenPos, null, color, NPC.rotation + (float)Math.Sin(NPC.localAI[0] * 0.1f % MathHelper.TwoPi) * 0.1f, core.Size() * 0.5f, NPC.scale * (new Vector2(0.5f) + squishFactor * 0.5f), 0, 0);
            spriteBatch.Draw(core.Value, corePos - screenPos, null, color.MultiplyRGBA(rainbowColor), NPC.rotation + (float)Math.Sin(NPC.localAI[0] * 0.1f % MathHelper.TwoPi) * 0.1f, core.Size() * 0.5f, NPC.scale * 1.1f * (new Vector2(0.5f) + squishFactor * 0.5f), 0, 0);

            Asset<Texture2D> colorMap = ModContent.Request<Texture2D>($"{nameof(CalamityHunt)}/Assets/Textures/RainbowGelMap");
            Effect gelEffect = ModContent.Request<Effect>($"{nameof(CalamityHunt)}/Assets/Effects/RainbowGel", AssetRequestMode.ImmediateLoad).Value;
            gelEffect.Parameters["uImageSize"].SetValue(texture.Size());
            gelEffect.Parameters["uSourceRect"].SetValue(new Vector4(frame.Left, frame.Top, frame.Width, frame.Height));
            gelEffect.Parameters["uMap"].SetValue(colorMap.Value);
            gelEffect.Parameters["uRbThresholdLower"].SetValue(0.53f);
            gelEffect.Parameters["uRbThresholdUpper"].SetValue(0.6f);
            gelEffect.Parameters["uTime"].SetValue(Main.GlobalTimeWrappedHourly * 0.5f % 1f);
            gelEffect.Parameters["uRbTime"].SetValue(Main.GlobalTimeWrappedHourly * 0.8f % 1f);
            gelEffect.Parameters["uFrequency"].SetValue(0.8f);

            switch (Attack)
            {
                case (int)AttackList.PixieBall:
                case (int)AttackList.BlowUp:

                    if ((Time > 50 && Time < 540) || Attack == (int)AttackList.BlowUp)
                    {
                        float rainbowShine = (0.5f + (float)Math.Sin(NPC.localAI[0] * (0.1f + NPC.localAI[1] * 0.15f)) * 0.5f) * pixieBallDangerShine;

                        gelEffect.Parameters["uImageSize"].SetValue(dangerTexture.Size());
                        gelEffect.Parameters["uSourceRect"].SetValue(new Vector4(0, 0, dangerTexture.Width(), dangerTexture.Height()));

                        spriteBatch.End();
                        spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, Main.DefaultSamplerState, DepthStencilState.None, Main.Rasterizer, gelEffect, Main.Transform);

                        spriteBatch.Draw(dangerTexture.Value, NPC.Center - screenPos, dangerTexture.Frame(), color, NPC.rotation, dangerTexture.Size() * 0.5f, NPC.scale * squishFactor, 0, 0);

                        spriteBatch.End();
                        spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, Main.DefaultSamplerState, DepthStencilState.None, Main.Rasterizer, null, Main.Transform);

                        spriteBatch.Draw(dangerShineTexture.Value, NPC.Center - screenPos, dangerShineTexture.Frame(), new Color(color.R, color.G, color.B, 0) * 0.4f, NPC.rotation, dangerTexture.Size() * 0.5f, NPC.scale * squishFactor, 0, 0);
                        
                        spriteBatch.Draw(dangerShineTexture.Value, NPC.Center - screenPos, dangerShineTexture.Frame(), rainbowColor * rainbowShine, NPC.rotation, dangerTexture.Size() * 0.5f, NPC.scale * squishFactor, 0, 0);
                        spriteBatch.Draw(dangerTexture.Value, NPC.Center - screenPos, dangerShineTexture.Frame(), rainbowColor * rainbowShine * 0.1f, NPC.rotation, dangerTexture.Size() * 0.5f, NPC.scale * squishFactor, 0, 0);

                        if (NPC.localAI[1] == 0)
                        {
                            spriteBatch.Draw(dangerShield.Value, NPC.Center - screenPos, dangerShield.Frame(), new Color(150, 150, 150, 150), NPC.rotation, dangerShield.Size() * 0.5f, NPC.scale * 1f * (Vector2.One * 0.5f + squishFactor * 0.5f), 0, 0);
                            spriteBatch.Draw(dangerShield.Value, NPC.Center - screenPos, dangerShield.Frame(), rainbowColor * (0.7f + rainbowShine * 0.3f), NPC.rotation, dangerShield.Size() * 0.5f, NPC.scale * 1.01f * (Vector2.One * 0.5f + squishFactor * 0.5f), 0, 0);
                            spriteBatch.Draw(dangerShield.Value, NPC.Center - screenPos, dangerShield.Frame(), rainbowColor * rainbowShine * 0.4f, NPC.rotation, dangerShield.Size() * 0.5f, NPC.scale * 1.5f * (Vector2.One * 0.5f + squishFactor * 0.5f), 0, 0);
                        }
                        else
                        {
                            spriteBatch.Draw(bloom.Value, NPC.Center - screenPos, bloom.Frame(), rainbowColor * (0.8f + rainbowShine * 0.2f), NPC.rotation, bloom.Size() * 0.5f, NPC.scale * 0.5f * (Vector2.One * 0.5f + squishFactor * 0.5f), 0, 0);
                            spriteBatch.Draw(bloom.Value, NPC.Center - screenPos, bloom.Frame(), rainbowColor * rainbowShine, NPC.rotation, bloom.Size() * 0.5f, NPC.scale * 1.5f * (Vector2.One * 0.5f + squishFactor * 0.5f), 0, 0);
                            spriteBatch.Draw(ring.Value, NPC.Center - screenPos, bloom.Frame(), rainbowColor * rainbowShine * 0.15f, NPC.rotation, ring.Size() * 0.5f, NPC.scale * 3f * (Vector2.One * 0.5f + squishFactor * 0.5f), 0, 0);
                        }

                        Vector2 flareOff = new Vector2(25, -25 + (float)Math.Sin(npcFrame * MathHelper.PiOver2 - MathHelper.PiOver2) * 2f) * squishFactor;

                        spriteBatch.Draw(flare.Value, NPC.Center + flareOff - screenPos, flare.Frame(), rainbowColor, 0, flare.Size() * 0.5f, NPC.scale * new Vector2(0.5f, 2f), 0, 0);
                        spriteBatch.Draw(flare.Value, NPC.Center + flareOff - screenPos, flare.Frame(), rainbowColor, MathHelper.PiOver2, flare.Size() * 0.5f, NPC.scale * new Vector2(0.5f, 2f), 0, 0);
                        spriteBatch.Draw(flare.Value, NPC.Center + flareOff - screenPos, flare.Frame(), rainbowColor, MathHelper.PiOver4, flare.Size() * 0.5f, NPC.scale * new Vector2(0.5f, 1.5f), 0, 0);
                        spriteBatch.Draw(flare.Value, NPC.Center + flareOff - screenPos, flare.Frame(), rainbowColor, MathHelper.PiOver2 + MathHelper.PiOver4, flare.Size() * 0.5f, NPC.scale * new Vector2(0.5f, 1.5f), 0, 0);

                        spriteBatch.Draw(flare.Value, NPC.Center + flareOff - screenPos, flare.Frame(), new Color(120, 120, 120, 0), 0, flare.Size() * 0.5f, NPC.scale * new Vector2(0.33f, 1.5f), 0, 0);
                        spriteBatch.Draw(flare.Value, NPC.Center + flareOff - screenPos, flare.Frame(), new Color(120, 120, 120, 0), MathHelper.PiOver2, flare.Size() * 0.5f, NPC.scale * new Vector2(0.33f, 1.5f), 0, 0);
                        spriteBatch.Draw(flare.Value, NPC.Center + flareOff - screenPos, flare.Frame(), new Color(120, 120, 120, 0), MathHelper.PiOver4, flare.Size() * 0.5f, NPC.scale * new Vector2(0.33f, 1f), 0, 0);
                        spriteBatch.Draw(flare.Value, NPC.Center + flareOff - screenPos, flare.Frame(), new Color(120, 120, 120, 0), MathHelper.PiOver2 + MathHelper.PiOver4, flare.Size() * 0.5f, NPC.scale * new Vector2(0.33f, 1f), 0, 0);

                    }
                    else
                        goto default;

                    break;

                default:

                    if (NPC.IsABestiaryIconDummy)
                    {
                        RasterizerState priorRasterizerState = spriteBatch.GraphicsDevice.RasterizerState;
                        Rectangle priorScissorRectangle = spriteBatch.GraphicsDevice.ScissorRectangle;

                        spriteBatch.End();
                        spriteBatch.GraphicsDevice.RasterizerState = priorRasterizerState;
                        spriteBatch.GraphicsDevice.ScissorRectangle = priorScissorRectangle;
                        spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, Main.DefaultSamplerState, DepthStencilState.None, priorRasterizerState, gelEffect, Main.UIScaleMatrix);

                        spriteBatch.Draw(texture.Value, NPC.Bottom - screenPos, frame, color, NPC.rotation, frame.Size() * new Vector2(0.5f, 1f), NPC.scale * squishFactor, 0, 0);

                        spriteBatch.End();
                        spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, Main.DefaultSamplerState, DepthStencilState.None, priorRasterizerState, null, Main.UIScaleMatrix);

                        spriteBatch.Draw(shineTexture.Value, NPC.Bottom - screenPos, frame, new Color(color.R, color.G, color.B, 0) * 0.4f, NPC.rotation, frame.Size() * new Vector2(0.5f, 1f), NPC.scale * squishFactor, 0, 0);

                        Vector2 flareOff = new Vector2(41, -69 + (float)Math.Sin(npcFrame * MathHelper.PiOver2 - MathHelper.PiOver2) * 2f) * NPC.scale * squishFactor;

                        spriteBatch.Draw(flare.Value, NPC.Bottom + flareOff - screenPos, flare.Frame(), rainbowColor, 0, flare.Size() * 0.5f, NPC.scale * new Vector2(0.5f, 2f), 0, 0);
                        spriteBatch.Draw(flare.Value, NPC.Bottom + flareOff - screenPos, flare.Frame(), rainbowColor, MathHelper.PiOver2, flare.Size() * 0.5f, NPC.scale * new Vector2(0.5f, 2f), 0, 0);
                        spriteBatch.Draw(flare.Value, NPC.Bottom + flareOff - screenPos, flare.Frame(), rainbowColor, MathHelper.PiOver4, flare.Size() * 0.5f, NPC.scale * new Vector2(0.5f, 1.5f), 0, 0);
                        spriteBatch.Draw(flare.Value, NPC.Bottom + flareOff - screenPos, flare.Frame(), rainbowColor, MathHelper.PiOver2 + MathHelper.PiOver4, flare.Size() * 0.5f, NPC.scale * new Vector2(0.5f, 1.5f), 0, 0);

                        spriteBatch.Draw(flare.Value, NPC.Bottom + flareOff - screenPos, flare.Frame(), new Color(120, 120, 120, 0), 0, flare.Size() * 0.5f, NPC.scale * new Vector2(0.33f, 1.5f), 0, 0);
                        spriteBatch.Draw(flare.Value, NPC.Bottom + flareOff - screenPos, flare.Frame(), new Color(120, 120, 120, 0), MathHelper.PiOver2, flare.Size() * 0.5f, NPC.scale * new Vector2(0.33f, 1.5f), 0, 0);
                        spriteBatch.Draw(flare.Value, NPC.Bottom + flareOff - screenPos, flare.Frame(), new Color(120, 120, 120, 0), MathHelper.PiOver4, flare.Size() * 0.5f, NPC.scale * new Vector2(0.33f, 1f), 0, 0);
                        spriteBatch.Draw(flare.Value, NPC.Bottom + flareOff - screenPos, flare.Frame(), new Color(120, 120, 120, 0), MathHelper.PiOver2 + MathHelper.PiOver4, flare.Size() * 0.5f, NPC.scale * new Vector2(0.33f, 1f), 0, 0);

                        break;
                    }
                    else
                    {
                        spriteBatch.End();
                        spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, Main.DefaultSamplerState, DepthStencilState.None, Main.Rasterizer, gelEffect, Main.Transform);

                        spriteBatch.Draw(texture.Value, NPC.Bottom - screenPos, frame, color, NPC.rotation, frame.Size() * new Vector2(0.5f, 1f), NPC.scale * squishFactor, 0, 0);

                        spriteBatch.End();
                        spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, Main.DefaultSamplerState, DepthStencilState.None, Main.Rasterizer, null, Main.Transform);
                        
                        spriteBatch.Draw(shineTexture.Value, NPC.Bottom - screenPos, frame, new Color(color.R, color.G, color.B, 0) * 0.4f, NPC.rotation, frame.Size() * new Vector2(0.5f, 1f), NPC.scale * squishFactor, 0, 0);

                        Vector2 flareOff = new Vector2(41, -69 + (float)Math.Sin(npcFrame * MathHelper.PiOver2 - MathHelper.PiOver2) * 2f) * NPC.scale * squishFactor;

                        spriteBatch.Draw(flare.Value, NPC.Bottom + flareOff - screenPos, flare.Frame(), rainbowColor, 0, flare.Size() * 0.5f, NPC.scale * new Vector2(0.5f, 2f), 0, 0);
                        spriteBatch.Draw(flare.Value, NPC.Bottom + flareOff - screenPos, flare.Frame(), rainbowColor, MathHelper.PiOver2, flare.Size() * 0.5f, NPC.scale * new Vector2(0.5f, 2f), 0, 0);
                        spriteBatch.Draw(flare.Value, NPC.Bottom + flareOff - screenPos, flare.Frame(), rainbowColor, MathHelper.PiOver4, flare.Size() * 0.5f, NPC.scale * new Vector2(0.5f, 1.5f), 0, 0);
                        spriteBatch.Draw(flare.Value, NPC.Bottom + flareOff - screenPos, flare.Frame(), rainbowColor, MathHelper.PiOver2 + MathHelper.PiOver4, flare.Size() * 0.5f, NPC.scale * new Vector2(0.5f, 1.5f), 0, 0);

                        spriteBatch.Draw(flare.Value, NPC.Bottom + flareOff - screenPos, flare.Frame(), new Color(120, 120, 120, 0), 0, flare.Size() * 0.5f, NPC.scale * new Vector2(0.33f, 1.5f), 0, 0);
                        spriteBatch.Draw(flare.Value, NPC.Bottom + flareOff - screenPos, flare.Frame(), new Color(120, 120, 120, 0), MathHelper.PiOver2, flare.Size() * 0.5f, NPC.scale * new Vector2(0.33f, 1.5f), 0, 0);
                        spriteBatch.Draw(flare.Value, NPC.Bottom + flareOff - screenPos, flare.Frame(), new Color(120, 120, 120, 0), MathHelper.PiOver4, flare.Size() * 0.5f, NPC.scale * new Vector2(0.33f, 1f), 0, 0);
                        spriteBatch.Draw(flare.Value, NPC.Bottom + flareOff - screenPos, flare.Frame(), new Color(120, 120, 120, 0), MathHelper.PiOver2 + MathHelper.PiOver4, flare.Size() * 0.5f, NPC.scale * new Vector2(0.33f, 1f), 0, 0);


                        break;
                    }
            }

            return false;
        }
    }
}
