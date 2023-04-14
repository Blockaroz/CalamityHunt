using CalamityHunt.Content.Bosses.Goozma.Projectiles;
using CalamityHunt.Content.Particles;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using CalamityHunt.Common.Systems.Particles;
using ReLogic.Content;
using System;
using System.Linq;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.GameContent;
using Terraria.Graphics.CameraModifiers;
using Terraria.ID;
using Terraria.ModLoader;
using System.Collections.Generic;
using Terraria.GameContent.Bestiary;
using Terraria.Localization;

namespace CalamityHunt.Content.Bosses.Goozma.Slimes
{
    public class EbonianBehemuck : ModNPC
    {
        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Ebonian Behemuck"); 
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
            
            //NPCID.Sets.NPCBestiaryDrawModifiers drawModifiers = new NPCID.Sets.NPCBestiaryDrawModifiers(0) {  };
            //NPCID.Sets.NPCBestiaryDrawOffset.Add(Type, drawModifiers);
        }

        public override void SetBestiary(BestiaryDatabase database, BestiaryEntry bestiaryEntry)
        {
            database.FindEntryByNPCID(Type).UIInfoProvider = new HighestOfMultipleUICollectionInfoProvider(new CommonEnemyUICollectionInfoProvider(ContentSamples.NpcBestiaryCreditIdsByNpcNetIds[ModContent.NPCType<Goozma>()], true));
            bestiaryEntry.Info.AddRange(new List<IBestiaryInfoElement> {
                new FlavorTextBestiaryInfoElement("Mods.CalamityHunt.Bestiary.EbonianBehemuck"),
                BestiaryDatabaseNPCsPopulator.CommonTags.SpawnConditions.Events.SlimeRain,
                BestiaryDatabaseNPCsPopulator.CommonTags.SpawnConditions.Biomes.TheCorruption,
            });
        }

        public override void SetDefaults()
        {
            NPC.width = 150;
            NPC.height = 100;
            NPC.damage = 12;
            NPC.defense = 500;
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
        }

        private enum AttackList
        {
            ToxicBubbles,
            Trifecta,
            RockPillar,
            TooFar,
            Interrupt
        }

        public ref float Time => ref NPC.ai[0];
        public ref float Attack => ref NPC.ai[1];
        public ref NPC Host => ref Main.npc[(int)NPC.ai[2]];
        public ref float RememberAttack => ref NPC.ai[3];

        public NPCAimedTarget Target => NPC.GetTargetData();
        public Vector2 squishFactor = Vector2.One;

        public int npcFrame;

        private List<int> eyeType;

        public override void OnSpawn(IEntitySource source)
        {
            List<int> eyeTypes = new List<int>()
            {
                0, 1, 2, 3, 4
            };
            eyeType = new List<int>();
            for (int i = 0; i < 5; i++)
            {
                int rand = Main.rand.Next(eyeTypes.Count);
                eyeType.Add(rand);
                eyeTypes.RemoveAt(rand);
            }
        }

        public override void AI()
        {
            if (NPC.ai[2] < 0)
                NPC.ai[2] = Main.npc.First(n => n.type == ModContent.NPCType<Goozma>() && n.active).whoAmI;
            if (!Main.npc.Any(n => n.type == ModContent.NPCType<Goozma>() && n.active))
                NPC.active = false;

            NPC.realLife = Host.whoAmI;

            NPC.frameCounter++;
            if (NPC.frameCounter > 7)
            {
                NPC.frameCounter = 0;
                npcFrame = (npcFrame + 1) % 4;
            }

            if (!NPC.HasPlayerTarget)
                NPC.TargetClosestUpgraded();
            if (!NPC.HasPlayerTarget)
                NPC.active = false;

            NPC.damage = 0;

            if (Time < 0)
            {
                NPC.frameCounter++;

                NPC.velocity *= 0.9f;
                NPC.damage = 0;
                squishFactor = new Vector2(1f - (float)Math.Pow(Utils.GetLerpValue(-10, -45, Time, true), 2) * 0.5f, 1f + (float)Math.Pow(Utils.GetLerpValue(-10, -45, Time, true), 2) * 0.4f);
                if (Time == -2 && NPC.Distance(Target.Center) > 1000)
                {
                    RememberAttack = Attack;
                    Attack = (int)AttackList.TooFar;
                }
            }
            else switch (Attack)
                {
                    case (int)AttackList.ToxicBubbles:
                        ToxicBubbles();
                        break;
                                            
                    case (int)AttackList.Trifecta:
                        Trifecta();
                        break;                    
                    
                    case (int)AttackList.RockPillar:
                        RockPillar();
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

            Time++;
            NPC.localAI[0]++;
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

        private void ToxicBubbles()
        {
            int jumpCount = 3;
            int jumpTime = 120;
            if (Main.expertMode)
            {
                jumpCount = 5;
                jumpTime = 70;
            }

            if (Time < 40)
            {
                squishFactor = new Vector2(1f + (float)Math.Pow(Utils.GetLerpValue(2, 40, Time, true), 2) * 0.4f, 1f - (float)Math.Pow(Utils.GetLerpValue(2, 40, Time, true), 2) * 0.6f);

                if (Time == 38)
                    NPC.velocity += new Vector2(NPC.DirectionTo(saveTarget).SafeNormalize(Vector2.Zero).X * 10, -10);
            }
            else if (Time < 40 + jumpCount * jumpTime)
            {
                float localTime = (Time - 40) % jumpTime;

                if (localTime < (int)(jumpTime * 0.2f))
                    saveTarget = Target.Center + new Vector2(Target.Velocity.X * 45, 200);

                if (localTime == 0)
                {
                    NPC.velocity.Y -= 10;
                    if (!Main.dedServ)
                    {
                        SoundEngine.PlaySound(SoundID.Item146, NPC.Center);
                        SoundEngine.PlaySound(SoundID.QueenSlime, NPC.Center);
                    }
                }
                else if (localTime < (int)(jumpTime * 0.8f))
                {
                    Vector2 midPoint = new Vector2((NPC.Center.X + saveTarget.X) / 2f, NPC.Center.Y);
                    Vector2 jumpTarget = Vector2.Lerp(Vector2.Lerp(NPC.Center, midPoint, Utils.GetLerpValue(0, jumpTime * 0.3f, localTime, true)), Vector2.Lerp(midPoint, saveTarget, Utils.GetLerpValue(jumpTime * 0.3f, jumpTime * 0.75f, localTime, true)), Utils.GetLerpValue(0, jumpTime * 0.7f, localTime, true));
                    NPC.velocity = Vector2.Lerp(NPC.velocity, NPC.DirectionTo(jumpTarget).SafeNormalize(Vector2.Zero) * NPC.Distance(jumpTarget) * 0.7f * Utils.GetLerpValue(0, jumpTime * 0.7f, localTime, true), (float)Math.Pow(Utils.GetLerpValue(0, jumpTime * 0.8f, localTime, true), 2f));
                    NPC.rotation = -NPC.velocity.Y * 0.005f * Math.Sign(NPC.velocity.X);

                    float resquish = Utils.GetLerpValue(jumpTime * 0.4f, 0, localTime, true) + Utils.GetLerpValue(jumpTime * 0.3f, jumpTime * 0.6f, localTime, true);
                    squishFactor = new Vector2(1f - (float)Math.Pow(resquish, 2) * 0.5f, 1f + (float)Math.Pow(resquish, 2) * 0.5f);
                    NPC.frameCounter++;

                }

                if (localTime >= (int)(jumpTime * 0.8f) && localTime < (int)(jumpTime * 0.95f))
                {
                    NPC.velocity *= 0.2f;
                    NPC.rotation = 0;

                    for (int i = 0; i < Main.rand.Next(1, 2); i++)
                        Projectile.NewProjectile(NPC.GetSource_FromAI(), NPC.Bottom, Main.rand.NextVector2Circular(20, 15) + Vector2.UnitY * 15 + NPC.DirectionTo(Target.Center).SafeNormalize(Vector2.Zero) * 10f, ModContent.ProjectileType<ToxicSludge>(), GetDamage(1), 0);

                    if (localTime % 2 == 0)
                        Main.instance.CameraModifiers.Add(new PunchCameraModifier(saveTarget, Main.rand.NextVector2CircularEdge(3, 3), 5f, 10, 12));

                    squishFactor = new Vector2(1f + (float)Math.Sqrt(Utils.GetLerpValue(jumpTime, jumpTime * 0.8f, localTime, true)) * 0.6f, 1f - (float)Math.Sqrt(Utils.GetLerpValue(jumpTime, jumpTime * 0.8f, localTime, true)) * 0.5f);
                }
                if (localTime == (int)(jumpTime * 0.74f))
                {
                    foreach (Player player in Main.player.Where(n => n.active && !n.dead && n.Distance(NPC.Center) < 600))
                        player.velocity += player.DirectionFrom(NPC.Bottom + Vector2.UnitY * 10) * 3;

                    if (!Main.dedServ)
                    {
                        SoundStyle slam = new SoundStyle($"{nameof(CalamityHunt)}/Assets/Sounds/Goozma/Slimes/GoozmaSlimeSlam", 1, 3);
                        slam.MaxInstances = 0;
                        SoundEngine.PlaySound(slam, NPC.Center);
                    }

                    for (int i = 0; i < Main.rand.Next(14, 20); i++)
                    {
                        Vector2 velocity = Main.rand.NextVector2Circular(8, 1) - Vector2.UnitY * Main.rand.NextFloat(7f, 12f);
                        Vector2 position = NPC.Center + Main.rand.NextVector2Circular(1, 50) + new Vector2(velocity.X * 15f, 32f);
                        Particle.NewParticle(Particle.ParticleType<EbonBombChunk>(), position, velocity, Color.White, 0.1f + Main.rand.NextFloat(2f));
                    }
                }
            }

            if (Time > 50 + jumpCount * jumpTime)
                Reset();
        }

        private void Trifecta()
        {
            int pairCount = 3;
            int pairTime = 40;
            int waitTime = 150;
            int countOfMe = Main.projectile.Count(n => n.type == ModContent.ProjectileType<EbonianBehemuckClone>() && n.active);

            NPC.damage = 0;

            if (Time < 60)
            {
                NPC.dontTakeDamage = true;

                Vector2 squishBounce = new Vector2(1f - (float)Math.Sin(Time * 0.15f) * 0.3f, 1f - (float)Math.Cos(Time * 0.15f) * 0.3f);
                squishFactor = Vector2.Lerp(Vector2.One, squishBounce, Utils.GetLerpValue(0, 10, Time, true)) * (float)Math.Pow(Utils.GetLerpValue(55, 10, Time, true), 2f);

                if (Time > 10 && Time < 30)
                    for (int i = 0; i < Main.rand.Next(2, 3); i++)
                    {
                        Vector2 velocity = Main.rand.NextVector2Circular(8, 1) - Vector2.UnitY * Main.rand.NextFloat(7f, 12f);
                        Vector2 position = NPC.Center + Main.rand.NextVector2Circular(1, 50) + new Vector2(velocity.X * 15f, 32f);
                        Particle.NewParticle(Particle.ParticleType<EbonBombChunk>(), position, velocity, Color.White, 0.1f + Main.rand.NextFloat(2f));
                    }

                if (Time == 40)
                {
                    List<int> randLeft = new List<int>()
                    {
                        0, 1, 2
                    };
                    for (int i = 0; i < pairCount; i++)
                    {
                        int randChosen = Main.rand.Next(randLeft.Count);
                        Projectile clone = Projectile.NewProjectileDirect(NPC.GetSource_FromAI(), NPC.Center, new Vector2(5, 0).RotatedBy(MathHelper.TwoPi / pairCount * i), ModContent.ProjectileType<EbonianBehemuckClone>(), GetDamage(2), 0);
                        clone.ai[0] = -waitTime - randLeft[randChosen] * pairTime;
                        clone.ai[1] = randLeft[randChosen];
                        Projectile cloneBottom = Projectile.NewProjectileDirect(NPC.GetSource_FromAI(), NPC.Center, new Vector2(5, 0).RotatedBy(MathHelper.TwoPi / pairCount * i), ModContent.ProjectileType<EbonianBehemuckClone>(), GetDamage(2), 0);
                        cloneBottom.ai[0] = -waitTime - randLeft[randChosen] * pairTime;
                        cloneBottom.ai[1] = randLeft[randChosen] + 3;
                        randLeft.RemoveAt(randChosen);
                    }
                }
            }

            if (Time > 30 && Time < waitTime + pairTime * pairCount + 20)
            {
                if (Time > waitTime && (Time - waitTime) % pairTime > 30)
                    NPC.velocity *= 0.2f;
                else
                    NPC.velocity = Vector2.Lerp(NPC.velocity, NPC.DirectionTo(Target.Center).SafeNormalize(Vector2.Zero) * NPC.Distance(Target.Center) * Utils.GetLerpValue(50, waitTime, Time, true) * 0.6f, Utils.GetLerpValue(50, waitTime, Time, true) * 0.6f);

                if ((Time - waitTime + 5) % pairTime == 0)
                    saveTarget.X = Main.rand.NextFloatDirection() * Main.rand.NextFloat(0.9f, 1.1f);

                foreach (Projectile proj in Main.projectile.Where(n => n.active && n.type == ModContent.ProjectileType<EbonianBehemuckClone>()))
                    proj.ai[2] += saveTarget.X * Math.Abs(0.2f * (float)Math.Cbrt(Utils.GetLerpValue(pairTime, 0, (Time - waitTime) % pairTime, true) * Utils.GetLerpValue(waitTime, waitTime + 5, Time, true)));

                Vector2 squishBounce = new Vector2(1f - (float)Math.Sin(Time * 0.15f) * 0.3f, 1f - (float)Math.Cos(Time * 0.15f) * 0.3f);
                squishFactor = Vector2.Lerp(Vector2.One, squishBounce, Utils.GetLerpValue(30, 15, Time - waitTime - pairTime * pairCount, true)) * (float)Math.Sqrt(Utils.GetLerpValue(0, 10, Time - waitTime - pairTime * pairCount, true));
                NPC.scale = (float)Math.Sqrt(Utils.GetLerpValue(0, 30, Time - waitTime - pairTime * pairCount, true));
            }
            if (Time > waitTime + pairTime * pairCount + 20)
                NPC.velocity *= 0.9f;

            if (Time > waitTime + pairTime * pairCount + 70)
            {
                NPC.dontTakeDamage = false;
                Reset();
            }
        }

        private void RockPillar()
        {
            int spikeCount = 2;
            int spikeTime = 105;
            if (Main.expertMode)
            {
                spikeCount = 3;
                spikeTime = 70;
            }

            if (Time < 71)
            {
                saveTarget = NPC.FindSmashSpot(Target.Center + new Vector2(0, 100));

                NPC.velocity = Vector2.Lerp(NPC.velocity, NPC.DirectionTo(saveTarget).SafeNormalize(Vector2.Zero) * NPC.Distance(saveTarget) * 0.4f * Utils.GetLerpValue(0, 50, Time, true), 0.2f);
                NPC.rotation = -NPC.velocity.Y * 0.01f * Math.Sign(NPC.velocity.X) * Utils.GetLerpValue(30, 70, Time, true);

                squishFactor = new Vector2(1f + (float)Math.Sqrt(Utils.GetLerpValue(20, 70, Time, true)) * 0.3f, 1f - (float)Math.Sqrt(Utils.GetLerpValue(20, 70, Time, true)) * 0.3f);

                if (Time == 70)
                {
                    foreach (Player player in Main.player.Where(n => n.active && !n.dead && n.Distance(NPC.Center) < 8000))
                        player.velocity.Y = -20;
                    
                    Projectile leftPillar = Projectile.NewProjectileDirect(NPC.GetSource_FromAI(), saveTarget + new Vector2(-120, 100), Vector2.Zero, ModContent.ProjectileType<EbonstonePillar>(), GetDamage(3), 0);
                    leftPillar.ai[0] = -25;
                    leftPillar.ai[1] = 45;

                    Projectile rightPillar = Projectile.NewProjectileDirect(NPC.GetSource_FromAI(), saveTarget + new Vector2(120, 100), Vector2.Zero, ModContent.ProjectileType<EbonstonePillar>(), GetDamage(3), 0);
                    rightPillar.ai[0] = -25;
                    rightPillar.ai[1] = 45;

                    NPC.velocity.Y -= 40;
                    saveTarget = saveTarget - new Vector2(0, 1400);

                    if (!Main.dedServ)
                    {
                        SoundStyle raise = new SoundStyle($"{nameof(CalamityHunt)}/Assets/Sounds/Goozma/Slimes/EbonstoneRaise");
                        raise.MaxInstances = 0;
                        SoundEngine.PlaySound(raise, NPC.Center);
                    }
                }
            }
            else
            {
                NPC.rotation = 0;
                NPC.velocity = Vector2.Lerp(NPC.velocity, NPC.DirectionTo(saveTarget).SafeNormalize(Vector2.Zero) * NPC.Distance(saveTarget) * 0.5f, (float)Math.Pow(Utils.GetLerpValue(100, 70, Time, true), 2f) * 0.5f + 0.1f);

                if (Time == 110)
                {
                    Projectile bottomLeftSpike = Projectile.NewProjectileDirect(NPC.GetSource_FromAI(), new Vector2(saveTarget.X + 105, saveTarget.Y + 1560), (-Vector2.UnitX).RotatedByRandom(0.05f), ModContent.ProjectileType<EbonstoneTooth>(), GetDamage(4), 0);
                    bottomLeftSpike.ai[0] = -5;
                    bottomLeftSpike.ai[1] = 90 + spikeCount * spikeTime;
                    Projectile bottomRightSpike = Projectile.NewProjectileDirect(NPC.GetSource_FromAI(), new Vector2(saveTarget.X - 105, saveTarget.Y + 1560), Vector2.UnitX.RotatedByRandom(0.05f), ModContent.ProjectileType<EbonstoneTooth>(), GetDamage(4), 0);
                    bottomRightSpike.ai[0] = -5;
                    bottomRightSpike.ai[1] = 90 + spikeCount * spikeTime;
                }
                if (Time % spikeTime == 0 && Time > 100 && Time <= 100 + spikeCount * spikeTime)
                {
                    int spikeNumber = Main.rand.Next(12, 15);
                    for (int i = 0; i < spikeNumber; i++)
                    {
                        Vector2 position = saveTarget + new Vector2(0, MathHelper.Lerp(100, 1600, i / (float)spikeNumber));
                        float randRot = Main.rand.NextFloat(-0.4f, 0.4f);
                        Projectile leftSpike = Projectile.NewProjectileDirect(NPC.GetSource_FromAI(), position + new Vector2(112, Main.rand.Next(-5, 5)), (-Vector2.UnitX).RotatedByRandom(0.06f).RotatedBy(randRot), ModContent.ProjectileType<EbonstoneTooth>(), GetDamage(4), 0);
                        leftSpike.ai[0] = -50 + Main.rand.Next(-5, 5);
                        leftSpike.ai[1] = 80 + Main.rand.Next(-5, 5);
                        Projectile rightSpike = Projectile.NewProjectileDirect(NPC.GetSource_FromAI(), position + new Vector2(-112, Main.rand.Next(-5, 5)), Vector2.UnitX.RotatedByRandom(0.06f).RotatedBy(-randRot), ModContent.ProjectileType<EbonstoneTooth>(), GetDamage(4), 0);
                        rightSpike.ai[0] = -50 + Main.rand.Next(-5, 5);
                        rightSpike.ai[1] = 80 + Main.rand.Next(-5, 5);
                    }
                }
                if (Time % spikeTime == 70 && Time >= 150 && Time <= 150 + spikeCount * spikeTime && !Main.dedServ)
                {
                    SoundStyle spiked = new SoundStyle($"{nameof(CalamityHunt)}/Assets/Sounds/Goozma/GoozmaDartShoot", 1, 2);
                    spiked.MaxInstances = 0;
                    SoundEngine.PlaySound(spiked, NPC.Center);
                }
            }

            if (Time > 90 && Time < 190 + spikeCount * spikeTime)
                foreach (Player player in Main.player.Where(n => n.active && !n.dead && n.Distance(NPC.Center) < 8000))
                {
                    if (player.Center.Y < NPC.Bottom.Y)
                        player.velocity.Y = -player.velocity.Y + 15;
                    if (player.Center.Y > NPC.Bottom.Y + 1500)
                        player.velocity.Y -= Math.Max(player.Distance(NPC.Center) - 1500, 0) * 0.1f;

                    if (player.Center.X > NPC.Bottom.X + 70 || player.Center.X < NPC.Bottom.X - 70)
                    {
                        player.velocity.X = -player.velocity.X * 0.1f;
                        player.velocity.X -= (player.Center.X - NPC.Center.X) * 0.07f;
                    }
                }

            //if (Time < 160 + spikeCount * spikeTime)
            //    foreach (Player player in Main.player.Where(n => n.active && !n.dead && n.Distance(NPC.Center) < 10000))
            //        player.AddBuff(BuffID.Obstructed, 20, true);

            if (Time == 140 + spikeCount * spikeTime && !Main.dedServ)
            {
                SoundStyle raise = new SoundStyle($"{nameof(CalamityHunt)}/Assets/Sounds/Goozma/Slimes/EbonstoneRaise");
                raise.MaxInstances = 0;
                SoundEngine.PlaySound(raise, NPC.Center);
            }

            if (Time > 190 + spikeCount * spikeTime)
                Reset();
        }

        private int GetDamage(int attack, float modifier = 1f)
        {
            int damage = attack switch
            {
                0 => 70,//contact
                1 => 40,//bubble
                2 => 70,//clone
                3 => 40,//pillar
                4 => 50,//tooth
                _ => damage = 0
            };

            return (int)(damage * modifier);
        }

        public override bool PreDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {
            Asset<Texture2D> texture = ModContent.Request<Texture2D>(Texture);
            Rectangle frame = texture.Frame(1, 4, 0, npcFrame);
            Asset<Texture2D> eye = ModContent.Request<Texture2D>($"{nameof(CalamityHunt)}/Assets/Textures/Goozma/Crowns/CorruptEye");
            Asset<Texture2D> tell = TextureAssets.Extra[178];

            Color color = Color.White;

            if (eyeType is null)
            {
                eyeType = new List<int>
                {
                    0, 1, 2
                };
            }

            switch (Attack)
            {
                case (int)AttackList.ToxicBubbles:

                    color = Color.Lerp(new Color(100, 100, 100, 0), Color.White, Math.Clamp(NPC.Distance(Target.Center), 100, 300) / 200f);
                   
                    break;

                case (int)AttackList.RockPillar:

                    if (Time > 0 && Time < 150)
                    {
                        float tellFade = Utils.GetLerpValue(60, 30, Time, true);
                        spriteBatch.Draw(tell.Value, NPC.Bottom - new Vector2(0, NPC.height / 4f * squishFactor.Y).RotatedBy(NPC.rotation) - Main.screenPosition, null, new Color(140, 40, 200, 0) * tellFade, NPC.rotation - MathHelper.PiOver2, tell.Size() * new Vector2(0f, 0.5f), new Vector2(1f - tellFade, 80) * new Vector2(squishFactor.Y, squishFactor.X), 0, 0);
                        spriteBatch.Draw(tell.Value, NPC.Bottom - new Vector2(0, NPC.height / 4f * squishFactor.Y).RotatedBy(NPC.rotation) - Main.screenPosition, null, new Color(140, 40, 200, 0) * tellFade, NPC.rotation + MathHelper.PiOver2, tell.Size() * new Vector2(0f, 0.5f), new Vector2((1f - tellFade) * 0.2f, 80) * new Vector2(squishFactor.Y, squishFactor.X), 0, 0);
                    }

                    break;

                case (int)AttackList.TooFar:
                    color = Color.Lerp(new Color(100, 100, 100, 0), Color.White, Math.Clamp(NPC.Distance(Target.Center), 100, 300) / 200f);
                    break;
            }
            
            for (int i = 0; i < NPCID.Sets.TrailCacheLength[Type]; i++)
            {
                Vector2 oldPos = NPC.oldPos[i] + NPC.Size * new Vector2(0.5f, 1f);
                Color trailColor = Color.Lerp(new Color(200, 20, 255, 120), new Color(0, 0, 0, 100), (float)Math.Sqrt(i / 10f)) * Math.Clamp(NPC.velocity.Length() * 0.01f, 0, 1) * 0.5f;
                spriteBatch.Draw(texture.Value, oldPos - Main.screenPosition, frame, trailColor.MultiplyRGBA(color), NPC.rotation, frame.Size() * new Vector2(0.5f, 1f), NPC.scale * squishFactor, 0, 0);
            }

            for (int i = 0; i < 3; i++)
            {
                Rectangle eyeFrame = eye.Frame(5, 1, eyeType[i], 0);
                Vector2 offset = new Vector2((float)Math.Sin((NPC.localAI[0] * 0.05f + i * 2f) % MathHelper.TwoPi) * 14 * (i % 2 == 0 ? 1 : -1), 0).RotatedBy((NPC.localAI[0] * 0.008f + i * 2.4) % MathHelper.TwoPi);
                spriteBatch.Draw(eye.Value, NPC.Bottom + offset * squishFactor - new Vector2(0, 47).RotatedBy(NPC.rotation) * NPC.scale * squishFactor - Main.screenPosition, eyeFrame, new Color(110, 50, 255, 20), NPC.rotation + NPC.localAI[0] * 0.1f * (i % 2 == 0 ? 1 : -1), eyeFrame.Size() * 0.5f, NPC.scale * 1.2f * squishFactor.Length() * 0.5f, 0, 0);
                spriteBatch.Draw(eye.Value, NPC.Bottom + offset * squishFactor - new Vector2(0, 47).RotatedBy(NPC.rotation) * NPC.scale * squishFactor - Main.screenPosition, eyeFrame, color, NPC.rotation + NPC.localAI[0] * 0.1f * (i % 2 == 0 ? 1 : -1), eyeFrame.Size() * 0.5f, NPC.scale * squishFactor.Length() * 0.5f, 0, 0);
            }

            spriteBatch.Draw(texture.Value, NPC.Bottom - Main.screenPosition, frame, color * 0.8f, NPC.rotation, frame.Size() * new Vector2(0.5f, 1f), NPC.scale * squishFactor, 0, 0);
            
            return false;
        }
    }
}
