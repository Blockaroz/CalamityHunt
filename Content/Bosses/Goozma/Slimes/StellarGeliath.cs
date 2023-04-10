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

namespace CalamityHunt.Content.Bosses.Goozma.Slimes
{
    public class StellarGeliath : ModNPC
    {
        public override void SetStaticDefaults()
        {
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
                BestiaryDatabaseNPCsPopulator.CommonTags.SpawnConditions.Biomes.Sky,
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
        }

        private enum AttackList
        {
            Starfall,
            Hellstars,
            Wormholes,
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

        public override void AI()
        {
            //if (!Main.npc.Any(n => n.type == ModContent.NPCType<Goozma>() && n.active))
            //    NPC.active = false;
            //else
            //    NPC.ai[2] = Main.npc.First(n => n.type == ModContent.NPCType<Goozma>() && n.active).whoAmI;
            
            //NPC.realLife = Host.whoAmI;

            NPC.frameCounter++;
            if (NPC.frameCounter > 7)
            {
                NPC.frameCounter = 0;
                npcFrame = (npcFrame + 1) % 4;
            }

            return;

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
                    case (int)AttackList.Starfall:
                        Starfall();
                        break;

                    case (int)AttackList.Hellstars:
                        Hellstars();
                        break;

                    case (int)AttackList.Wormholes:
                        Wormholes();
                        break;

                    case (int)AttackList.Interrupt:
                        NPC.noTileCollide = true;
                        NPC.damage = 0;

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

        private void Starfall()
        {
            if (Time < 60)
            {
                NPC.velocity *= 0.1f;
                squishFactor = new Vector2(1f + (float)Math.Cbrt(Utils.GetLerpValue(40, 56, Time, true)) * 0.4f, 1f - (float)Math.Sqrt(Utils.GetLerpValue(40, 56, Time, true)) * 0.6f);
                if (Time == 55 && !Main.dedServ)
                {
                    SoundEngine.PlaySound(SoundID.Item163, NPC.Center); 
                    SoundEngine.PlaySound(SoundID.QueenSlime, NPC.Center);
                }
            }
            else if (Time < 81)
            {
                NPC.noTileCollide = true;
                NPC.velocity.Y = -60f;
                squishFactor = new Vector2(1f - Utils.GetLerpValue(60, 65, Time, true) * 0.7f, 1f + (float)Math.Sqrt(Utils.GetLerpValue(60, 62, Time, true)) * 0.5f);
            }
            else if (Time < 200)
            {
                Vector2 airTarget = Target.Center + new Vector2(Target.Velocity.X * 15, 0) - Vector2.UnitY * 1000;

                if (Time < 168)
                {
                    if (Time < 158)
                    {
                        int maxDown = 0;
                        for (int i = 0; i < 24; i++)
                        {
                            Point tile = (Target.Center + Target.Velocity * 30 + new Vector2(0, i * 16)).ToTileCoordinates();
                            maxDown++;
                            if (WorldGen.SolidTile(tile))
                                break;
                        }

                        saveTarget = NPC.FindSmashSpot(Target.Center + Target.Velocity * 32 + Vector2.UnitY * maxDown * 16);
                    }

                    NPC.velocity = Vector2.Lerp(NPC.velocity, NPC.DirectionTo(airTarget).SafeNormalize(Vector2.Zero) * Math.Max(15, NPC.Distance(airTarget)) * 0.1f, 0.2f) * Utils.GetLerpValue(150, 135, Time, true);
                    NPC.rotation = NPC.rotation.AngleLerp(NPC.Top.AngleTo(saveTarget) - MathHelper.PiOver2, 0.1f);
                    int starCount = 10;
                    if (Main.expertMode)
                        starCount = 17;
                    if (Time > 115 && Time < 120)
                        for (int i = 0; i < (int)(starCount / 6f) + Main.rand.Next(3, 7); i++)
                        {
                            Vector2 starRand = new Vector2((i + (Time - 120) / 5f) * 300 + Main.rand.Next(-1600, 1600), -(i + (Time - 120) / 5f) / 40f);
                            Projectile starR = Projectile.NewProjectileDirect(NPC.GetSource_FromAI(), NPC.Top + new Vector2(starRand.X, -Math.Abs(starRand.Y)), Vector2.Zero, ModContent.ProjectileType<ShootingStar>(), 25, 0);
                            Projectile starL = Projectile.NewProjectileDirect(NPC.GetSource_FromAI(), NPC.Top + new Vector2(-starRand.X, -Math.Abs(starRand.Y)), Vector2.Zero, ModContent.ProjectileType<ShootingStar>(), 25, 0);
                            starR.ai[1] = MathHelper.PiOver2 + Main.rand.NextFloat(-0.2f, 0.2f);
                            starR.velocity = new Vector2(0, Main.rand.NextFloat(5, 25)).RotatedByRandom(1f);
                            starL.ai[1] = MathHelper.PiOver2 + Main.rand.NextFloat(-0.2f, 0.2f);
                            starL.velocity = new Vector2(0, Main.rand.NextFloat(5, 25)).RotatedByRandom(1f);
                        }
                    squishFactor = new Vector2(0.6f, 1.5f);
                    saveAngle = NPC.Top.AngleTo(saveTarget);
                }
                else                
                {
                    NPC.Center = Vector2.Lerp(NPC.Center, NPC.FindSmashSpot(saveTarget), Utils.GetLerpValue(172, 180, Time, true));
                    NPC.velocity *= 0.5f;

                    if (Time > 178)
                    {
                        NPC.rotation = 0;
                        squishFactor = new Vector2(1f + (float)Math.Pow(Utils.GetLerpValue(200, 181, Time, true), 2) * 0.6f, 1f - (float)Math.Pow(Utils.GetLerpValue(200, 181, Time, true), 5) * 0.8f);
                    }
                    if (Time >= 175 && Time % 2 == 0)
                        Main.instance.CameraModifiers.Add(new PunchCameraModifier(saveTarget, Main.rand.NextVector2CircularEdge(3, 3), 5f, 20, 12));
                    if (Time == 179 && !Main.dedServ)
                    {
                        SoundStyle slam = new SoundStyle($"{nameof(CalamityHunt)}/Assets/Sounds/Goozma/Slimes/GoozmaSlimeSlam", 1, 3);
                        slam.MaxInstances = 0;
                        SoundEngine.PlaySound(slam, NPC.Center);

                        SoundEngine.PlaySound(SoundID.Item130, NPC.Center);
                    }
                }
            }

            if (Time > 60 && Time < 87)
                for (int i = 0; i < Main.rand.Next(5, 7); i++)
                {
                    Vector2 rocketVelocity = Vector2.UnitY.RotatedByRandom(1f) * Main.rand.Next(10, 15);
                    Projectile.NewProjectileDirect(NPC.GetSource_FromAI(), NPC.Center + Main.rand.NextVector2Circular(5, 2), rocketVelocity, ModContent.ProjectileType<Novacracker>(), 5, 0);
                }

            if (Time > 240)
                Reset();
        }

        private float randomAngle;

        private void Hellstars()
        {
            int collapserFrequency = 18;
            if (Main.expertMode)
                collapserFrequency = 12;

            bool tooClose = false;
            Point pos = NPC.Bottom.ToTileCoordinates();
            for (int i = 0; i < 10; i++)
            {
                if (WorldGen.SolidTile(pos.X, pos.Y + i))
                {
                    tooClose = true;
                    break;
                }
            }

            if (tooClose)
                NPC.velocity.Y -= 1f;
            else
                NPC.velocity.Y *= 0.9f;

            if (Time > 20 && Time < 490)
            {
                NPC.velocity = Vector2.Lerp(NPC.velocity, NPC.DirectionTo(Target.Center).SafeNormalize(Vector2.Zero) * (NPC.Distance(Target.Center) - 100) * 0.1f, 0.1f);
                NPC.rotation = NPC.velocity.X * 0.01f;

                Vector2 offset = new Vector2(Main.rand.Next(800, 1500), 0).RotatedBy(Time / 70f);
                if (Main.rand.NextBool(3) || Time % collapserFrequency == 0)
                {
                    for (int i = 0; i < 3; i++)
                        Projectile.NewProjectileDirect(NPC.GetSource_FromAI(), NPC.Center + offset.RotatedByRandom(0.8f).RotatedBy(MathHelper.TwoPi / 3f * i), Vector2.Zero, ModContent.ProjectileType<CollapsingStar>(), GetDamage(2), 0);

                }
            }

            if (Time < 680)
                squishFactor = Vector2.Lerp(squishFactor, new Vector2(1f + (float)Math.Sin(Time * 0.1f) * 0.2f, 1f + (float)Math.Cos(Time * 0.1f) * 0.2f) * (1f + Utils.GetLerpValue(50, 400, Time, true)), 0.2f);

            if (Time > 510)
            {
                NPC.velocity *= 0.83f;

                if (Time < 640)
                    NPC.Center += Main.rand.NextVector2Circular(7, 7);

                if (Time == 655)
                {
                    NPC.width = 140;
                    NPC.height = 100;
                    NPC.scale = 1f;

                    for (int i = 0; i < 75; i++)
                        Projectile.NewProjectileDirect(NPC.GetSource_FromAI(), NPC.Center, Main.rand.NextVector2CircularEdge(15, 15) + Main.rand.NextVector2Circular(5, 5), ModContent.ProjectileType<TrailingStar>(), GetDamage(3), 0);
                }

                float progress = (float)Math.Cbrt(Math.Sin(Utils.GetLerpValue(640, 690, Time, true) * MathHelper.TwoPi));
                squishFactor = Vector2.Lerp(squishFactor, new Vector2(1f + progress * 0.6f, 1f - progress * 0.7f), Utils.GetLerpValue(640, 690, Time, true));
                
                if (Time >= 640 && Time % 2 == 0 && Time < 700)
                    Main.instance.CameraModifiers.Add(new PunchCameraModifier(saveTarget, Main.rand.NextVector2CircularEdge(3, 3), 5f, 10, 12));
            }

            if (Time > 740)
                Reset();
        }

        private int nextPortal;
        private Vector2 oldPortal;
        private float oldPortalScale;

        private void Wormholes()
        {
            NPC.damage = 0;

            int portCount = 10;
            int portTime = 90;
            if (Main.expertMode)
            {
                portCount = 15;
                portTime = 60;
            }
            if (Time < 30)
            {
                NPC.velocity *= 0.1f;
                squishFactor = new Vector2(1f + (float)Math.Cbrt(Utils.GetLerpValue(0, 30, Time, true)) * 0.4f, 1f - (float)Math.Sqrt(Utils.GetLerpValue(0, 30, Time, true)) * 0.5f);
                if (Time == 29)
                {
                    NPC.velocity.Y = -40;
                    NPC.velocity.X = 0;
                    SoundEngine.PlaySound(SoundID.Item146, NPC.Center);
                    squishFactor = new Vector2(0.5f, 1.5f);
                }
            }
            else if (Time < 150)
            {
                if (Time == 100 && !Main.dedServ)
                    SoundEngine.PlaySound(SoundID.QueenSlime, NPC.Center);

                if (Time <= 140)
                {
                    NPC.velocity.Y = MathHelper.Lerp(NPC.velocity.Y, Utils.GetLerpValue(100, 140, Time, true) * 30, 0.1f);
                    squishFactor = Vector2.Lerp(squishFactor, Vector2.Lerp(Vector2.One, new Vector2(0.3f, 1.6f), Utils.GetLerpValue(110, 115, Time, true)), 0.05f);
                }
                else
                {
                    NPC.velocity.Y *= 0.01f;
                    squishFactor = Vector2.Lerp(squishFactor, Vector2.Zero, Utils.GetLerpValue(140, 150, Time, true));
                }

                oldPortalScale = Utils.GetLerpValue(100, 120, Time, true) * Utils.GetLerpValue(150, 120, Time, true);

                if (Time == 140)
                {
                    if (!Main.dedServ)
                    {
                        Particle crack = Particle.NewParticle(Particle.ParticleType<CrackSpot>(), NPC.Center, Vector2.Zero, new Color(255, 230, 50, 0), 50f);
                        crack.data = "Wormhole";

                        SoundEngine.PlaySound(SoundID.Item30, NPC.Center);
                    }
                }
            }
            else if(Time < 150 + portCount * portTime)
            {
                NPC.width = 180;
                NPC.height = 180;

                int localTime = (int)((Time - 150) % portTime);

                if (localTime == 0)
                {
                    nextPortal = -1;
                    saveTarget = Main.rand.NextVector2CircularEdge(320, 320) + Main.rand.NextVector2Circular(70, 70);
                    nextPortal = Projectile.NewProjectile(NPC.GetSource_FromAI(), Target.Center - saveTarget, Main.rand.NextVector2CircularEdge(10, 10), ModContent.ProjectileType<Wormhole>(), 0, 0);
                    Main.projectile[nextPortal].timeLeft = portTime + 10;
                    Main.projectile[nextPortal].ai[1] = NPC.whoAmI;
                }

                if (localTime < (int)(portTime * 0.66f) + 1)
                    oldPortal = NPC.Center;
                oldPortalScale = Utils.GetLerpValue((int)(portTime * 0.05f), (int)(portTime * 0.2f), localTime, true) * Utils.GetLerpValue((int)(portTime * 0.75f), (int)(portTime * 0.6f), localTime, true);

                if (localTime == (int)(portTime * 0.57f))
                {
                    if (!Main.dedServ)
                    {
                        Particle.NewParticle(Particle.ParticleType<CrackSpot>(), NPC.Center, Vector2.Zero, new Color(60, 30, 255, 0), 38f);
                        SoundEngine.PlaySound(SoundID.Item130, NPC.Center);
                    }
                }

                if (localTime < (int)(portTime * 0.66f))
                    NPC.Center = Vector2.Lerp(NPC.Center, Target.Center + new Vector2(320 * Math.Clamp(Target.Velocity.Length(), 0, 1f), 0).RotatedBy(Target.Velocity.ToRotation()), 0.12f);
                else
                {
                    if (Time % 2 == 0)
                        Main.instance.CameraModifiers.Add(new PunchCameraModifier(saveTarget, Main.rand.NextVector2CircularEdge(2, 2), 4f, 8, 20));

                    Vector2 targetPos = Target.Center;
                    if (nextPortal > -1)
                        targetPos = Main.projectile[nextPortal].Center;
                    float prog = Utils.GetLerpValue((int)(portTime * 0.67f), (int)(portTime * 0.72f), localTime, true) * Utils.GetLerpValue((int)(portTime * 0.97f), (int)(portTime * 0.9f), localTime, true);
                    NPC.Center = Vector2.Lerp(NPC.Center, targetPos, prog * 0.3f);
                    NPC.damage = GetDamage(4);
                    NPC.rotation = NPC.AngleTo(targetPos) - MathHelper.PiOver2;
                    squishFactor = new Vector2(0.8f, 1.2f) * prog;
                }
            }

            if (Time == 155 + portCount * portTime && Main.rand.NextBool(3000))
            {
                NPC.active = false;
                Host.ai[0] = 0;
                Host.ai[3] = -1;
            }

            if (Time >= 160 + portCount * portTime)
            {
                if (Time < 220 + portCount * portTime)
                {
                    Vector2 airTarget = Target.Center + Target.Velocity * 20 - Vector2.UnitY * 300;
                    NPC.velocity = Vector2.Lerp(NPC.velocity, NPC.DirectionTo(airTarget).SafeNormalize(Vector2.Zero) * Math.Max(15, NPC.Distance(airTarget)) * 0.1f, 0.2f) * Utils.GetLerpValue(220, 190, Time - portCount * portTime, true);
                    saveTarget = Target.Center + Vector2.UnitY * 40;
                }
                else
                {
                    NPC.damage = GetDamage(0);
                    NPC.Center = Vector2.Lerp(NPC.Center, NPC.FindSmashSpot(saveTarget), Utils.GetLerpValue(210, 250, Time - portCount * portTime, true));
                    NPC.velocity *= 0.5f;
                    NPC.rotation = 0;

                    if (Time == 220 + portCount * portTime)
                    {
                        if (!Main.dedServ)
                        {
                            Particle exit = Particle.NewParticle(Particle.ParticleType<CrackSpot>(), NPC.Center, Vector2.Zero, Main.OurFavoriteColor, 38f);
                            exit.data = "Wormhole";
                            SoundEngine.PlaySound(SoundID.Item30, NPC.Center);
                        }
                    }

                    if (Time < 240 + portCount * portTime)
                        squishFactor = new Vector2(0.6f, 1.4f) * Utils.GetLerpValue(200, 230, Time - portCount * portTime, true);

                    else
                        squishFactor = Vector2.Lerp(new Vector2(1.5f, 0.5f), Vector2.One, Utils.GetLerpValue(240, 280, Time - portCount * portTime, true));

                    if (Time >= 238 + portCount * portTime && Time % 3 == 0)
                        Main.instance.CameraModifiers.Add(new PunchCameraModifier(saveTarget, Main.rand.NextVector2CircularEdge(3, 3), 7f, 20, 12));
                    if (Time == 238 + portCount * portTime && !Main.dedServ)
                    {
                        SoundStyle slam = new SoundStyle($"{nameof(CalamityHunt)}/Assets/Sounds/Goozma/Slimes/GoozmaSlimeSlam", 1, 3);
                        slam.MaxInstances = 0;
                        SoundEngine.PlaySound(slam, NPC.Center);
                    }
                }

                NPC.width = 140;
                NPC.height = 100;

                oldPortalScale = Utils.GetLerpValue(160, 170, Time - portCount * portTime, true) * Utils.GetLerpValue(210, 200, Time - portCount * portTime, true);  
            }

            if (Time > 280 + portCount * portTime)
                Reset();
        }

        private int GetDamage(int attack, float modifier = 1f)
        {
            int damage = attack switch
            {
                0 => 70,//contact
                1 => 20,//falling stars 
                2 => 30,//collapsing stars
                3 => 40,//collapse explosion stars
                4 => 100,//contact during wormholes
                _ => 0
            };

            return (int)(damage * modifier);
        }

        private float saveAngle;

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
                case (int)AttackList.Starfall:

                    float width = (float)Math.Pow(Utils.GetLerpValue(175, 90, Time, true), 0.75f) * NPC.width * 1.5f;
                    Color tellColor = new Color(50, 40, 200, 0) * (float)Math.Pow(Utils.GetLerpValue(90, 180, Time, true), 2f) * Utils.GetLerpValue(170, 160, Time, true);
                    spriteBatch.Draw(tell.Value, new Vector2(NPC.Center.X - Main.screenPosition.X, -100), null, tellColor, saveAngle, tell.Size() * new Vector2(0f, 0.5f), new Vector2(1.8f, width * 1.2f), 0, 0);

                    if (Time > 172 && Time < 200)
                    {
                        float power = Utils.GetLerpValue(200, 185, Time, true);
                    }
                    break;

                case (int)AttackList.Hellstars:
                    color = Color.Lerp(Color.Lerp(Color.Transparent, new Color(255, 255, 255, 0), Utils.GetLerpValue(-50, -30, Time, true)), Color.White, Utils.GetLerpValue(-40, 0, Time, true));
                    break;

                case (int)AttackList.Wormholes:

                    if (Time > 150 && Time < 1050)
                    {
                        for (int i = 0; i < NPCID.Sets.TrailCacheLength[Type]; i++)
                        {
                            Vector2 oldPos = NPC.oldPos[i] + NPC.Size * 0.5f;
                            Color trailColor = Color.Lerp(new Color(100, 0, 255, 0), new Color(10, 0, 0, 0), (float)Math.Sqrt(i / 9f)) * 0.5f;
                            spriteBatch.Draw(texture.Value, oldPos - Main.screenPosition, frame, trailColor, NPC.rotation, frame.Size() * 0.5f, NPC.scale * squishFactor, 0, 0);
                        }
                        spriteBatch.Draw(texture.Value, NPC.Center - Main.screenPosition, frame, color, NPC.rotation, frame.Size() * 0.5f, NPC.scale * squishFactor, 0, 0);

                        for (int i = 0; i < 5; i++)
                        {
                            Vector2 offset = new Vector2(25 * NPC.scale + (float)Math.Cos(Main.GlobalTimeWrappedHourly * 4f % MathHelper.TwoPi) * 8f, 0).RotatedBy(MathHelper.TwoPi / 5f * i + Main.GlobalTimeWrappedHourly * NPC.direction);
                            spriteBatch.Draw(texture.Value, NPC.Center + offset - Main.screenPosition, frame, new Color(50, 30, 60, 0).MultiplyRGBA(color), NPC.rotation, frame.Size() * 0.5f, NPC.scale * squishFactor, 0, 0);
                        }

                        int portTime = 90;
                        if (Main.expertMode)
                            portTime = 60;

                        spriteBatch.Draw(bloom.Value, oldPortal - Main.screenPosition, null, new Color(10, 0, 80, 0), 0, bloom.Size() * 0.5f, 8 * oldPortalScale, 0, 0);
                        spriteBatch.Draw(bloom.Value, oldPortal - Main.screenPosition, null, new Color(50, 0, 150, 0), 0, bloom.Size() * 0.5f, 3 * oldPortalScale, 0, 0);
                        spriteBatch.Draw(flare.Value, oldPortal - Main.screenPosition, null, new Color(70, 10, 200, 0), 0, flare.Size() * 0.5f, new Vector2(0.6f, 4f) * oldPortalScale, 0, 0);
                        spriteBatch.Draw(flare.Value, oldPortal - Main.screenPosition, null, new Color(70, 10, 200, 0), MathHelper.PiOver2, flare.Size() * 0.5f, new Vector2(0.6f, 4f) * oldPortalScale, 0, 0);
                        spriteBatch.Draw(flare.Value, oldPortal - Main.screenPosition, null, new Color(255, 205, 255, 0), 0, flare.Size() * 0.5f, new Vector2(0.8f, 1f) * oldPortalScale, 0, 0);
                        spriteBatch.Draw(flare.Value, oldPortal - Main.screenPosition, null, new Color(255, 205, 255, 0), MathHelper.PiOver2, flare.Size() * 0.5f, new Vector2(0.8f, 1f) * oldPortalScale, 0, 0);

                        return false;
                    }
                    if (Time > 1060 && Time < 1210)
                    {
                        spriteBatch.Draw(bloom.Value, NPC.Center - Main.screenPosition, null, new Color(10, 0, 80, 0), 0, bloom.Size() * 0.5f, 8 * oldPortalScale, 0, 0);
                        spriteBatch.Draw(bloom.Value, NPC.Center - Main.screenPosition, null, new Color(120, 0, 150, 0), 0, bloom.Size() * 0.5f, 3 * oldPortalScale, 0, 0);

                        spriteBatch.Draw(flare.Value, NPC.Center - Main.screenPosition, null, new Color(110, 30, 255, 0), 0, flare.Size() * 0.5f, new Vector2(0.6f, 8f) * oldPortalScale, 0, 0);
                        spriteBatch.Draw(flare.Value, NPC.Center - Main.screenPosition, null, new Color(110, 30, 255, 0), MathHelper.PiOver2, flare.Size() * 0.5f, new Vector2(0.6f, 8f) * oldPortalScale, 0, 0);
                        spriteBatch.Draw(flare.Value, NPC.Center - Main.screenPosition, null, new Color(255, 255, 255, 0), 0, flare.Size() * 0.5f, new Vector2(0.8f, 3f) * oldPortalScale, 0, 0);
                        spriteBatch.Draw(flare.Value, NPC.Center - Main.screenPosition, null, new Color(255, 255, 255, 0), MathHelper.PiOver2, flare.Size() * 0.5f, new Vector2(0.8f, 3f) * oldPortalScale, 0, 0);
                    }

                    break;

                case (int)AttackList.TooFar:
                    color = Color.Lerp(new Color(100, 100, 100, 0), Color.White, Math.Clamp(NPC.Distance(Target.Center), 100, 300) / 200f);
                    break;
            }

            for (int i = 0; i < NPCID.Sets.TrailCacheLength[Type]; i++)
            {
                Vector2 oldPos = NPC.oldPos[i] + NPC.Size * 0.5f;
                Color trailColor = Color.Lerp(new Color(255, 255, 255, 0), new Color(10, 0, 10, 0), (float)Math.Sqrt(i / 10f)) * Math.Clamp(NPC.velocity.Length() * 0.01f, 0, 1);
                spriteBatch.Draw(texture.Value, oldPos - Main.screenPosition, frame, trailColor, NPC.rotation, frame.Size() * 0.5f, NPC.scale * squishFactor, 0, 0);
            }
            spriteBatch.Draw(texture.Value, NPC.Bottom - Main.screenPosition, frame, color, NPC.rotation, frame.Size() * new Vector2(0.5f, 1f), NPC.scale * squishFactor, 0, 0);
            
            for (int i = 0; i < 5; i++)
            {
                Vector2 offset = new Vector2(25 * NPC.scale + (float)Math.Cos(Main.GlobalTimeWrappedHourly * 4f % MathHelper.TwoPi) * 8f, 0).RotatedBy(MathHelper.TwoPi / 5f * i + Main.GlobalTimeWrappedHourly * NPC.direction);
                spriteBatch.Draw(texture.Value, NPC.Bottom + offset - Main.screenPosition, frame, new Color(50, 30, 60, 0).MultiplyRGBA(color), NPC.rotation, frame.Size() * new Vector2(0.5f, 1f), NPC.scale * squishFactor, 0, 0);
            }

            return false;
        }
    }
}
