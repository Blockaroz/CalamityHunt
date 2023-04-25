using CalamityHunt.Common;
using CalamityHunt.Common.Systems;
using CalamityHunt.Common.Systems.Camera;
using CalamityHunt.Common.Systems.Particles;
using CalamityHunt.Content.Bosses.Goozma.Projectiles;
using CalamityHunt.Content.Items.BossBags;
using CalamityHunt.Content.Items.Masks;
using CalamityHunt.Content.Items.Materials;
using CalamityHunt.Content.Particles;
using CalamityHunt.Content.Pets.BloatBabyPet;
using CalamityHunt.Content.Projectiles;
using Humanizer;
using Microsoft.CodeAnalysis;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.GameContent;
using Terraria.GameContent.Bestiary;
using Terraria.GameContent.ItemDropRules;
using Terraria.GameContent.UI.Elements;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;

namespace CalamityHunt.Content.Bosses.Goozma
{
    [AutoloadBossHead]
    public partial class Goozma : ModNPC
    {
        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Goozma");
            NPCID.Sets.TrailCacheLength[Type] = 10;
            NPCID.Sets.TrailingMode[Type] = -1;
            NPCID.Sets.MPAllowedEnemies[Type] = true;
            NPCID.Sets.BossBestiaryPriority.Add(Type);
            NPCDebuffImmunityData debuffData = new NPCDebuffImmunityData { ImmuneToAllBuffsThatAreNotWhips = true };
            NPCID.Sets.DebuffImmunitySets.Add(Type, debuffData);
            NPCID.Sets.ShouldBeCountedAsBoss[Type] = true;

            NPCID.Sets.NPCBestiaryDrawModifiers drawModifiers = new NPCID.Sets.NPCBestiaryDrawModifiers(0)
            {
                Rotation = 0.01f,
                Velocity = 1f,
                Position = Vector2.One * 20f,
                PortraitScale = 0.66f
            };

            NPCID.Sets.NPCBestiaryDrawOffset.Add(Type, drawModifiers);
        }

        public override void SetBestiary(BestiaryDatabase database, BestiaryEntry bestiaryEntry)
        {
            bestiaryEntry.Info.AddRange(new List<IBestiaryInfoElement> {
                new FlavorTextBestiaryInfoElement("Mods.CalamityHunt.Bestiary.Goozma"),
                BestiaryDatabaseNPCsPopulator.CommonTags.SpawnConditions.Events.SlimeRain,
                new SlimeMonsoonPortraitBackground()
            });
        }

        public override void SetDefaults()
        {
            NPC.width = 120;
            NPC.height = 130;
            NPC.damage = 0;
            NPC.defense = 100;
            NPC.lifeMax = 3500000;
            NPC.HitSound = null;
            NPC.DeathSound = SoundID.NPCDeath1;
            NPC.knockBackResist = 0f;
            NPC.noGravity = true;
            NPC.noTileCollide = true;
            NPC.value = Item.buyPrice(gold: 25);
            NPC.SpawnWithHigherTime(30);
            NPC.direction = 1;
            NPC.boss = true;
            NPC.npcSlots = 10f;
            NPC.aiStyle = -1;
            NPC.dontTakeDamage = true;
            if (!Main.dedServ)
            {
                Music = MusicLoader.GetMusicSlot($"{nameof(CalamityHunt)}/Assets/Music/GlutinousArbitration");
                Music2 = MusicLoader.GetMusicSlot($"{nameof(CalamityHunt)}/Assets/Music/ViscousDesperation");
            }
            if (ModLoader.HasMod("CalamityMod"))
            {
                Mod calamity = ModLoader.GetMod("CalamityMod");
                calamity.Call("SetDebuffVulnerabilities", "poison", false);
                calamity.Call("SetDebuffVulnerabilities", "heat", true);
                calamity.Call("SetDefenseDamageNPC", Type, true);
            }
        }

        public int Music2;

        private static int relicType;
        private static int trophyType;

        public override void Load()
        {
            relicType = BossDropAutoloader.AddBossRelic("Goozma");
            trophyType = BossDropAutoloader.AddBossTrophy("Goozma");          
            On_Main.UpdateAudio += FadeMusicOut;
            On_Main.CheckMonoliths += DrawCordShapes;

            Main.QueueMainThreadAction(() =>
            {
                cordTarget = new RenderTarget2D(Main.graphics.GraphicsDevice, Main.screenWidth / 2, Main.screenHeight / 2);
            });
        }

        public override void ModifyNPCLoot(NPCLoot npcLoot)
        {
            //Bag
            if (Main.rand.NextBool(20))
                npcLoot.Add(ItemDropRule.BossBag(ModContent.ItemType<TreasureBucket>()));
            else
                npcLoot.Add(ItemDropRule.BossBag(ModContent.ItemType<TreasureTrunk>()));

            //Trophy
            npcLoot.Add(ItemDropRule.Common(trophyType, 10));

            //Relic
            npcLoot.Add(ItemDropRule.MasterModeCommonDrop(relicType));

            //Master Drop
            npcLoot.Add(ItemDropRule.MasterModeCommonDrop(ModContent.ItemType<ImperialGelato>()));

            //Masks
            LeadingConditionRule classic = new LeadingConditionRule(new Conditions.NotExpert());

            classic.OnSuccess(ItemDropRule.Common(ModContent.ItemType<EntropyMatter>(), 1, 20, 30));
            classic.OnSuccess(ItemDropRule.FewFromOptions(1, 7, ModContent.ItemType<GoozmaMask>(), ModContent.ItemType<GoozmaMask>(), ModContent.ItemType<GoozmaMask>(), ModContent.ItemType<GoozmaMask>()));
                       
            //Weapon
            //classic.OnSuccess(ItemDropRule.Common(ModContent.ItemType<EntropyMatter>()));
        }

        public ref float Time => ref NPC.ai[0];
        public ref float Attack => ref NPC.ai[1];
        public ref float Phase => ref NPC.ai[2];
        public ref NPC ActiveSlime => ref Main.npc[(int)NPC.ai[3]];

        private int currentSlime;
        private int[] nextAttack;

        public NPCAimedTarget Target => NPC.ai[3] < 0 ? NPC.GetTargetData() : (ActiveSlime.GetTargetData().Invalid ? NPC.GetTargetData() : ActiveSlime.GetTargetData());

        private enum AttackList
        {
            Shimmering,
            SpawnSelf,
            SpawnSlime,
            BurstLightning,
            DrillDash,
            GaussRay
        }

        public override void OnSpawn(IEntitySource source)
        {
            nextAttack = new int[4] { -1, -1, -1, -1 };
            NPC.ai[3] = -1;
            currentSlime = -1;
            Phase = -1;
            headScale = 0.9f;
            oldVel = new Vector2[NPCID.Sets.TrailCacheLength[Type]];
            Main.newMusic = Music;
            for (int i = 0; i < Main.musicFade.Length; i++)
                Main.musicFade[i] = 0.1f;
            Main.musicFade[Main.newMusic] = 1f;

            if (!Main.dedServ)
            {
                //Particle crack = Particle.NewParticle(Particle.ParticleType<CrackSpot>(), NPC.Center, Vector2.Zero, Color.Black, 36f);
                //crack.data = "GoozmaColor";

                SoundStyle roar = new SoundStyle($"{nameof(CalamityHunt)}/Assets/Sounds/Goozma/GoozmaAwaken");
                SoundEngine.PlaySound(roar, NPC.Center);
            }

            for (int i = 0; i < NPCID.Sets.TrailCacheLength[Type]; i++)
            {
                NPC.oldPos[i] = NPC.position;
                NPC.oldRot[i] = NPC.rotation;
                oldVel[i] = NPC.velocity;
            }
        }

        public override void OnHitPlayer(Player target, Player.HurtInfo hurtInfo)
        {
            //target.AddBuff(BuffID.VortexDebuff, 480);
        }

        private void ChangeWeather()
        {
            if (Main.slimeRain)
                Main.StopSlimeRain(false);

            Main.StopRain();

            //Main.LocalPlayer.ManageSpecialBiomeVisuals("HuntOfTheOldGods:SlimeMonsoon", true, Main.LocalPlayer.Center);
        }

        private Vector2 saveTarget;
        private Vector2 eyePower;
        private Vector2[] oldVel;

        public override bool CheckDead()
        {
            if (NPC.life < 1)
            {
                switch (Phase)
                {
                    case 0:

                        Time = 0;
                        Phase++;
                        NPC.life = 1;
                        NPC.lifeMax = (int)(NPC.lifeMax * 0.3f);
                        NPC.dontTakeDamage = true;

                        break;

                    case 2:

                        Time = 0;
                        Phase++;
                        NPC.life = 1;
                        NPC.dontTakeDamage = true;

                        break;
                }
            }
            return Phase > 3 || (Phase == 3 && Time >= 300);
        }

        public override void AI()
        {
            ChangeWeather();
            bool noSlime = NPC.ai[3] < 0 || NPC.ai[3] >= Main.maxNPCs || ActiveSlime.ai[1] > 3 || !ActiveSlime.active;
            if (Phase == 0 && noSlime)
                Attack = (int)AttackList.SpawnSlime;

            if (Target.Invalid)
                NPC.TargetClosestUpgraded();
            if (NPC.GetTargetData().Invalid && Phase != -5)
            {
                Phase = -5;
                Time = 0;
                NPC.dontTakeDamage = true;
            }

            if (NPC.Distance(Target.Center) > 700 && Phase != 1 && Phase != 3)
                NPC.Center = Vector2.Lerp(NPC.Center, NPC.Center + NPC.DirectionTo(Target.Center).SafeNormalize(Vector2.Zero) * Math.Max(0, NPC.Distance(Target.Center) - 700), 0.05f);

            drawOffset = new Vector2(-5f * NPC.direction, 8f) + new Vector2((float)Math.Sin(NPC.localAI[0] * 0.05f % MathHelper.TwoPi) * 2, (float)Math.Cos(NPC.localAI[0] * 0.025f % MathHelper.TwoPi) * 4);
            float offsetWobble = (float)Math.Cos(NPC.localAI[0] * 0.05f % MathHelper.TwoPi) * 0.07f;
            NPC.rotation = MathHelper.Lerp(NPC.rotation, Math.Clamp(NPC.velocity.X * 0.02f, -1f, 1f) - (offsetWobble - 0.1f) * NPC.direction, 0.2f);
            extraTilt = MathHelper.Lerp(extraTilt, Math.Clamp(-NPC.velocity.X * 0.025f, -1f, 1f) - 0.09f * NPC.direction, 0.15f);
            headScale = MathHelper.Lerp(headScale, 1f, 0.05f);

            if (NPC.velocity.Length() < 50f)
            {
                if (Math.Abs(NPC.Center.X - Target.Center.X) > 20)
                    NPC.direction = NPC.Center.X > Target.Center.X ? -1 : 1;
            }
            else
                NPC.direction = NPC.velocity.X > 0 ? 1 : -1;

            NPC.damage = 0;

            if (!NPC.dontTakeDamage || Phase == 0)
            {
                NPC.damage = GetDamage(0);

                if (!noSlime)
                    if (NPC.Distance(ActiveSlime.Center) < 400)
                        NPC.damage = 0;
            }

            switch (Phase)
            {
                case -1:

                    Attack = (int)AttackList.SpawnSelf;
                    NPC.direction = -1;
                    NPC.velocity.Y = -Utils.GetLerpValue(20, 40, Time, true);
                    if (Time > 60)
                    {
                        NPC.dontTakeDamage = false;
                        Phase = 0;
                        Time = 0;
                        headScale = 1f;
                    }

                    Dust.NewDustPerfect(NPC.Center + Main.rand.NextVector2Circular(10, 10), DustID.TintableDust, Main.rand.NextVector2CircularEdge(10, 10), 200, Color.Black, Main.rand.NextFloat(2, 4)).noGravity = true;

                    break;

                case 0:

                    if (Attack == (int)AttackList.SpawnSlime)
                    {
                        NPC.TargetClosestUpgraded();
                        NPC.direction = NPC.DirectionTo(NPC.GetTargetData().Center).X > 0 ? 1 : -1;
                        NPC.defense = 1000;

                        if (Time > 5 && Time < 45)
                        {
                            NPC.velocity = Vector2.Lerp(NPC.velocity, NPC.DirectionTo(NPC.GetTargetData().Center) * Math.Max(NPC.Distance(NPC.GetTargetData().Center) - 150, 0) * 0.12f, 0.1f);
                            NPC.position += Main.rand.NextVector2Circular(6, 6);

                            for (int i = 0; i < 3; i++)
                            {
                                Vector2 inward = NPC.Center + Main.rand.NextVector2Circular(70, 70) + Main.rand.NextVector2CircularEdge(100 - Time, 100 - Time);
                                Particle hue = Particle.NewParticle(Particle.ParticleType<HueLightDust>(), inward, inward.DirectionTo(NPC.Center) * Main.rand.NextFloat(3f), Color.White, 1f);
                                hue.data = NPC.localAI[0];
                            }
                        }

                        if (Time > 42 && Time <= 50 && !(NPC.ai[3] < 0 || NPC.ai[3] >= Main.maxNPCs))
                            KillSlime(currentSlime);

                        if (Time == 50)
                        {
                            NPC.velocity *= -1f;
                            for (int i = 0; i < 45; i++)
                            {
                                Vector2 outward = NPC.Center + Main.rand.NextVector2Circular(10, 10);
                                Particle hue = Particle.NewParticle(Particle.ParticleType<HueLightDust>(), outward, outward.DirectionFrom(NPC.Center) * Main.rand.NextFloat(3f, 10f), Color.White, 2f);
                                hue.data = NPC.localAI[0];
                            }

                            //for (int i = 0; i < Main.rand.Next(3, 8); i++)
                            //{
                            //    NPC slime = NPC.NewNPCDirect(NPC.GetSource_FromAI(), (int)NPC.Center.X, (int)NPC.Center.Y - 50, Main.rand.Next(SlimeUtils.SlimeIDs));
                            //    slime.velocity = new Vector2(Main.rand.Next(-4, 4), Main.rand.Next(-7, -5));
                            //    if (Main.rand.NextBool(10))
                            //    {
                            //        NPC jellyfish = NPC.NewNPCDirect(NPC.GetSource_FromAI(), (int)NPC.Center.X, (int)NPC.Center.Y - 50, Main.rand.Next(SlimeUtils.JellyfishIDs));
                            //        jellyfish.velocity = new Vector2(Main.rand.Next(-4, 4), Main.rand.Next(-7, -5));
                            //    }
                            //}

                            currentSlime = 3;// (currentSlime + 1) % 4;
                            nextAttack[currentSlime] = 1;

                            for (int i = 0; i < nextAttack.Length; i++)
                                nextAttack[i] = nextAttack[i] % 3;

                            int[] slimeTypes = new int[]
                            {
                                ModContent.NPCType<EbonianBehemuck>(),
                                ModContent.NPCType<DivineGargooptuar>(),
                                ModContent.NPCType<CrimulanGlopstrosity>(),
                                ModContent.NPCType<StellarGeliath>()
                            };

                            if (Main.netMode != NetmodeID.MultiplayerClient)
                            {
                                if (NPC.ai[3] < 0 || NPC.ai[3] >= Main.maxNPCs || !ActiveSlime.active)
                                {
                                    NPC.ai[3] = NPC.NewNPC(NPC.GetSource_FromAI(), (int)NPC.Center.X, (int)NPC.Center.Y - 50, slimeTypes[currentSlime], ai0: -50, ai1: nextAttack[currentSlime], ai2: NPC.whoAmI);
                                    ActiveSlime.velocity.Y -= 16;
                                }
                                else
                                {
                                    Vector2 pos = ActiveSlime.Bottom;
                                    ActiveSlime.active = false;
                                    NPC.ai[3] = NPC.NewNPC(NPC.GetSource_FromAI(), (int)pos.X, (int)pos.Y, slimeTypes[currentSlime], ai0: -50, ai1: nextAttack[currentSlime], ai2: NPC.whoAmI);
                                }

                                if (!Main.dedServ)
                                {
                                    SoundStyle spawn = new SoundStyle($"{nameof(CalamityHunt)}/Assets/Sounds/Goozma/GoozmaSpawnSlime");
                                    SoundEngine.PlaySound(spawn, NPC.Center);
                                }
                            }
                            else if (Main.netMode == NetmodeID.MultiplayerClient)
                                NPC.netUpdate = true;
                        }

                        NPC.velocity *= 0.9f;

                        if (Time > 70)
                        {
                            Time = 0;
                            Attack++;
                        }
                    }
                    else
                    {
                        NPC.defense = 100;

                        //attack depends on slime's attack, which depends on slime
                        switch (currentSlime)
                        {
                            //ebonian
                            case 0:
                                switch (ActiveSlime.ai[1])
                                {
                                    case 0:

                                        AresLockTo(Target.Center - new Vector2(0, 400) + Target.Velocity * new Vector2(5f, 2f));

                                        if (Time > 50)
                                        {
                                            SortedProjectileAttack(Target.Center - Target.Velocity * 10, SortedProjectileAttackTypes.EbonianBubbles);
                                            NPC.velocity.X *= 0.8f;
                                        }

                                        break;
                                    case 1:

                                        Fly();
                                        NPC.velocity *= 1.01f;
                                        //if (Time > 80)
                                        //{
                                        //    if (Time % 70 == 0)
                                        //        Projectile.NewProjectileDirect(NPC.GetSource_FromAI(), NPC.Center, NPC.DirectionTo(Target.Center + Target.Velocity * 10).SafeNormalize(Vector2.Zero) * 10, ModContent.ProjectileType<PureSlimeball>(), GetDamage(2), 0);
                                           
                                        //    SortedProjectileAttack(Target.Center + Target.Velocity * 20 + Main.rand.NextVector2Circular(10, 10), SortedProjectileAttackTypes.EbonianTrifecta);
                                        //}

                                        break;

                                    case 2:

                                        AresLockTo(Target.Center - Vector2.UnitY * 270);

                                        break;
                                }
                                break;

                            //divine
                            case 1:
                                switch (ActiveSlime.ai[1])
                                {
                                    case 0:

                                        Vector2 outerRing = Target.Center.DirectionTo(ActiveSlime.Center) * 800;
                                        FlyTo(ActiveSlime.Center - outerRing);
                                        NPC.velocity *= 0.6f;

                                        break;

                                    case 1:

                                        AresLockTo(Target.Center + new Vector2(0, 360) + Target.Velocity * new Vector2(4f, 3f));

                                        if (Time > 50)
                                        {
                                            SortedProjectileAttack(Target.Center - Target.Velocity * 10, SortedProjectileAttackTypes.CrystalStorm);
                                            NPC.velocity.X *= 0.8f;
                                        }

                                        break;

                                    case 2:

                                        Orbit(500, new Vector2(600, 100));
                                        SortedProjectileAttack(Target.Center, SortedProjectileAttackTypes.PixieBallDisruption);

                                        break;
                                }
                                break;

                            //crimulan
                            case 2:
                                switch (ActiveSlime.ai[1])
                                {
                                    case 0:

                                        //if (Time > 70 && Time < 230)
                                        //{
                                        //    FlyTo(Target.Center + new Vector2(700 * (ActiveSlime.Center.X > Target.Center.X ? -1 : 1) * Utils.GetLerpValue(0, 100, Time, true), (float)Math.Sin(Time % 110 * MathHelper.TwoPi / 110f) * 130));

                                        //    if ((Main.rand.NextBool(15) || Time % 30 == 0) && Time > 90)
                                        //        Projectile.NewProjectileDirect(NPC.GetSource_FromAI(), NPC.Center, Main.rand.NextVector2Circular(15, 15), ModContent.ProjectileType<PureSlimeball>(), GetDamage(2), 0);

                                        //}
                                        //else
                                        //    Fly();
                                        break;
                                    case 1:

                                        Fly();
                                        NPC.velocity *= 0.9f;

                                        break;
                                    case 2:

                                        Orbit(300, new Vector2(800, 0));

                                        if (Time > 40 && Time < 400)
                                            SortedProjectileAttack(Target.Center, SortedProjectileAttackTypes.CrimulanHop);


                                        NPC.velocity *= 0.9f;

                                        break;
                                }
                                break;

                            //stellar
                            case 3:
                                switch (ActiveSlime.ai[1])
                                {
                                    case 0:

                                        if (Time < 5)
                                            saveTarget = Target.Center;
                                        else
                                            saveTarget = Vector2.Lerp(saveTarget, Target.Center, 0.05f);

                                        if (Time >= 70 && Time < 550)
                                            SortedProjectileAttack(saveTarget, SortedProjectileAttackTypes.StellarDisruption);

                                        FlyTo(saveTarget);
                                        NPC.velocity *= 0.96f * Utils.GetLerpValue(-50, 60, Time, true);
                                        NPC.damage = 0;

                                        break;

                                    case 1:
                                        Fly();

                                        break;

                                    case 2:

                                        Vector2 outerRing = Target.Center.DirectionTo(ActiveSlime.Center) * 1500;
                                        FlyTo(ActiveSlime.Center - outerRing);
                                        NPC.velocity *= 0.6f;
                                        if (Time < 590)
                                            SortedProjectileAttack(ActiveSlime.Center, SortedProjectileAttackTypes.StellarTaunt);

                                        break;
                                }
                                break;
                        }
                    }

                    eyePower = Vector2.One * 1.5f;

                    break;

                case 1:

                    NPC.velocity = Vector2.Zero;
                    if (NPC.ai[3] > -1 && NPC.ai[3] <= Main.maxNPCs)
                        if (ActiveSlime.active)
                            ActiveSlime.active = false;

                    drawOffset += Main.rand.NextVector2Circular(10, 10) * Utils.GetLerpValue(0, 300, Time, true) * Utils.GetLerpValue(302, 300, Time, true);
                    NPC.dontTakeDamage = true;
                    NPC.life = 1 + (int)((float)Math.Pow(Utils.GetLerpValue(300, 530, Time, true), 3) * (NPC.lifeMax - 1));
                    eyePower = Vector2.SmoothStep(Vector2.One * 1.5f, new Vector2(5f, 3.6f), Utils.GetLerpValue(300, 500, Time, true));
                    
                    if (Time < 15)
                        KillSlime(currentSlime);

                    if (Time > 45 && Time < 53)
                    {
                        for (int i = 0; i < 5; i++)
                        {
                            Dust.NewDustPerfect(NPC.Center + Main.rand.NextVector2Circular(10, 10), DustID.TintableDust, Main.rand.NextVector2CircularEdge(20, 20), 200, Color.Black, Main.rand.NextFloat(2, 4)).noGravity = true;
                            Particle.NewParticle(Particle.ParticleType<GoozBombChunk>(), NPC.Center + Main.rand.NextVector2Circular(10, 10), Main.rand.NextVector2Circular(20, 20) - Vector2.UnitY * 8f, Color.White, 0.5f + Main.rand.NextFloat(1.5f));
                        }
                    }

                    if (Time < 50)
                    {
                        NPC.scale = (float)Math.Sqrt(Utils.GetLerpValue(50, 30, Time, true));
                        for (int i = 0; i < Main.rand.Next(1, 4); i++)
                        {
                            Particle gelBit = Particle.NewParticle(Particle.ParticleType<GoozGelBit>(), NPC.Center + Main.rand.NextVector2Circular(30, 40), Main.rand.NextVector2CircularEdge(10, 10) + Main.rand.NextVector2Circular(20, 20), Color.White, 1f + Main.rand.NextFloat());
                            gelBit.data = (int)(300 - Time + Main.rand.Next(55));
                        }
                    }
                    else
                        NPC.scale = MathHelper.Lerp(NPC.scale, MathHelper.SmoothStep(0f, 1f, Utils.GetLerpValue(400, 500, Time, true)), 0.2f);

                    //if (Time > 400 && Time % 12 == 0)
                    //{
                    //    Particle crack = Particle.NewParticle(Particle.ParticleType<CrackSpot>(), NPC.Center, Vector2.Zero, Color.Black, 40f);
                    //    crack.data = "GoozmaBlack";
                    //    crack.behindEntities = true;
                    //}

                    if (Time < 70 || Time > 400)
                    {
                        Vector2 gooVelocity = Main.rand.NextVector2Circular(2, 3);
                        Particle goo = Particle.NewParticle(Particle.ParticleType<GooBurst>(), NPC.Center + Main.rand.NextVector2Circular(50, 80) * NPC.scale, gooVelocity, Color.White, 0.1f + Main.rand.NextFloat(1.5f));
                        goo.data = NPC.localAI[0] + Main.rand.NextFloat(0.2f, 0.5f);
                    }

                    //if (!Main.dedServ)
                    //{
                    //    if (Time == 1)
                    //    {
                    //        Main.instance.CameraModifiers.Add(new SlowPan(NPC.Bottom, 300, 200, 30, "GoozmaEnrage"));
                    //        SoundStyle fakeDeathSound = new SoundStyle($"{nameof(CalamityHunt)}/Assets/Sounds/GoozmaFakeDeath");
                    //        SoundEngine.PlaySound(fakeDeathSound, NPC.Center);
                    //    }
                    //    if (Time == 500)
                    //    {
                    //        SoundStyle roar = new SoundStyle($"{nameof(CalamityHunt)}/Assets/Sounds/GoozmaEnrage");
                    //        SoundEngine.PlaySound(roar, NPC.Center);
                    //    }
                    //}

                    if (Time > 550)
                    {
                        Music = Music2;
                        Main.newMusic = Music;
                        Main.musicFade[Main.curMusic] = 0f;
                        Main.musicFade[Main.newMusic] = 1f;

                        Time = 0;
                        Phase++;
                        NPC.dontTakeDamage = false;
                        NPC.defense += 20;
                    }

                    break;

                case 2:

                    switch (Attack)
                    {
                        case (int)AttackList.BurstLightning:

                            Orbit((int)(600 + ((float)NPC.life / NPC.lifeMax * 100)), new Vector2(0, -450));
                            SortedProjectileAttack(Target.Center + Target.Velocity * 1.5f, SortedProjectileAttackTypes.BurstLightning);

                            if (Main.rand.NextBool(30))
                                Particle.NewParticle(Particle.ParticleType<GoozBombChunk>(), NPC.Bottom + Main.rand.NextVector2Circular(10, 10), Main.rand.NextVector2Circular(5, 2) - Vector2.UnitY * 5f + NPC.velocity * 0.1f, Color.White, 0.1f + Main.rand.NextFloat(1.5f));
                           
                            if (Time > 640)
                            {
                                Time = 0;
                                Attack = (int)AttackList.DrillDash;
                            }

                            break;

                        case (int)AttackList.DrillDash:

                            int dashCount = 4;
                            int dashTime = 110;
                            if (Main.expertMode)
                            {
                                dashCount = 6;
                                dashTime = 90;
                            }

                            if (Time <= dashTime * dashCount)
                            {
                                if (Time % dashTime == 21 && !Main.dedServ)
                                {
                                    SoundStyle roar = new SoundStyle($"{nameof(CalamityHunt)}/Assets/Sounds/Goozma/GoozmaDash", 1, 2);
                                    roar.MaxInstances = 0;
                                    roar.PitchVariance = 0.15f;
                                    SoundEngine.PlaySound(roar, NPC.Center);
                                }

                                if (Time % dashTime < 20)
                                {
                                    NPC.Center = Vector2.Lerp(NPC.Center, NPC.Center + NPC.DirectionTo(Target.Center) * (NPC.Distance(Target.Center) - 300) * 0.2f, 0.6f);
                                    NPC.velocity = NPC.DirectionTo(Target.Center).SafeNormalize(Vector2.Zero);
                                }
                                else if (Time % dashTime < 74)
                                {
                                    for (int i = 0; i < 8; i++)
                                    {
                                        Vector2 position = Vector2.Lerp(NPC.position, NPC.oldPos[0], i / 24f) + NPC.Size * 0.5f;

                                        Particle hueTop = Particle.NewParticle(Particle.ParticleType<HueLightDust>(), position + Main.rand.NextVector2Circular(8, 8) + NPC.velocity, -NPC.velocity.RotatedBy((float)Math.Sin((Time - (i / 8f)) * 0.23f) * 0.8f), Color.White, 2f);
                                        hueTop.data = NPC.localAI[0];

                                        Particle hueBot = Particle.NewParticle(Particle.ParticleType<HueLightDust>(), position + Main.rand.NextVector2Circular(8, 8) + NPC.velocity, -NPC.velocity.RotatedBy(-(float)Math.Sin((Time - (i / 8f)) * 0.23f) * 0.8f), Color.White, 2f);
                                        hueBot.data = NPC.localAI[0];
                                    }

                                    NPC.velocity += NPC.DirectionTo(Target.Center).SafeNormalize(Vector2.Zero) * (4f - Time % dashTime * 0.07f);
                                    NPC.velocity = Vector2.Lerp(NPC.velocity, NPC.DirectionTo(Target.Center).SafeNormalize(Vector2.Zero).RotatedBy(0.05f) * 50f, 0.01f);
                                }
                                else
                                    NPC.velocity *= 0.6f;

                                SortedProjectileAttack(Target.Center, SortedProjectileAttackTypes.DrillDash);
                            }

                            if (Time > dashTime * dashCount + 20)
                            {
                                Time = 0;
                                if (NPC.life < NPC.lifeMax / 2f)
                                    Attack = (int)AttackList.GaussRay;
                                else
                                    Attack = (int)AttackList.BurstLightning;
                            }

                            NPC.damage = GetDamage(6, 0.9f + Time % dashTime / dashTime * 0.2f);

                            break;

                        case (int)AttackList.GaussRay:

                            SortedProjectileAttack(Target.Center, SortedProjectileAttackTypes.GaussRay);

                            Fly();

                            if (Time > 1350)
                            {
                                Time = 0;
                                Attack = (int)AttackList.BurstLightning;
                            }

                            break;

                        default:

                            Attack = (int)AttackList.BurstLightning;
                            Time = 0;
                            break;
                    }

                    break;

                case 3:

                    NPC.velocity.X = 0;
                    NPC.velocity.Y = Utils.GetLerpValue(10, 300, Time, true) * -1f;
                    NPC.Center += Main.rand.NextVector2Circular(4, 4) * Utils.GetLerpValue(10, 300, Time, true);

                    NPC.dontTakeDamage = true;
                    NPC.life = 1;

                    if (Time == 2)
                    {
                        Projectile.NewProjectileDirect(NPC.GetSource_FromThis(), NPC.Center, Vector2.Zero, ModContent.ProjectileType<GoozmaDeathGodrays>(), 0, 0, ai1: NPC.whoAmI);

                        if (!Main.dedServ)
                        {
                            SoundStyle deathRoar = new SoundStyle($"{nameof(CalamityHunt)}/Assets/Sounds/Goozma/GoozmaDeathBuildup");
                            deathRoar.MaxInstances = 0;
                            SoundEngine.PlaySound(deathRoar, NPC.Center);
                        }
                    }

                    if (Main.rand.NextBool(2 + (int)(60 - Time / 5f)))
                        for (int i =0; i < Main.rand.Next(1, 4); i++)
                            Particle.NewParticle(Particle.ParticleType<GoozBombChunk>(), NPC.Center + Main.rand.NextVector2Circular(10, 10), Main.rand.NextVector2Circular(20, 20) - Vector2.UnitY * 10f, Color.White, 0.5f + Main.rand.NextFloat(1.5f));

                    if (Main.rand.NextBool(1 + (int)(65 - Time / 4.66f)) || Time > 250)
                    {
                        Vector2 deathGooVelocity = Main.rand.NextVector2Circular(2, 3);
                        Particle deathGoo = Particle.NewParticle(Particle.ParticleType<GooBurst>(), NPC.Center + Main.rand.NextVector2Circular(50, 80) * NPC.scale, deathGooVelocity, Color.White, 0.2f + Main.rand.NextFloat(1.5f));
                        deathGoo.data = NPC.localAI[0] + Main.rand.NextFloat(0.2f, 0.5f);
                    }

                    if (Time > 290)
                        headScale += 0.15f;

                    if (Time > 300)
                    {
                        //Particle kill = Particle.NewParticle(Particle.ParticleType<CrackSpot>(), NPC.Center, Vector2.Zero, Color.Black, 36f);
                        //kill.data = "GoozmaBlack";
                        Main.musicFade[Main.curMusic] = 0f;
                        Main.curMusic = -1;

                        NPC.justHit = true;
                        NPC.life = 0;
                        NPC.checkDead();
                    }

                    break;

                case -5:

                    NPC.velocity *= 0.8f;

                    if (Time > 30 && Time % 3 == 0)
                    {
                        //Particle leave = Particle.NewParticle(Particle.ParticleType<CrackSpot>(), NPC.Center, Vector2.Zero, Color.Black, 40f);
                        //leave.data = "GoozmaBlack";
                    }

                    if (Time == 50)
                    {
                        //Particle leave = Particle.NewParticle(Particle.ParticleType<CrackSpot>(), NPC.Center, Vector2.Zero, Color.Black, 50f);
                        //leave.data = "GoozmaColor";

                        NPC.active = false;
                    }

                    break;

                case -2:

                    Attack = (int)AttackList.Shimmering;

                    NPC.velocity = Vector2.Lerp(NPC.velocity, Vector2.UnitY * -8, 0.2f) * Utils.GetLerpValue(60, 20, Time, true);

                    NPC.Center += Main.rand.NextVector2Circular(5, 5) * Utils.GetLerpValue(0, 100, Time, true);
                    
                    if (Time < 15)
                        KillSlime(currentSlime);

                    if (Time > 80)
                    {
                        NPC.active = false;
                        foreach (NPC left in Main.npc.Where(n => !n.active))
                        {
                            int type = Utils.SelectRandom(Main.rand,
                                NPCID.BlueSlime,
                                NPCID.BlueSlime,
                                NPCID.BlueSlime,
                                NPCID.BlueSlime,
                                NPCID.GreenSlime,
                                NPCID.GreenSlime,
                                NPCID.GreenSlime,
                                NPCID.GreenSlime,
                                NPCID.PurpleSlime,
                                NPCID.PurpleSlime,
                                NPCID.PurpleSlime,
                                NPCID.RedSlime,
                                NPCID.RedSlime,
                                NPCID.YellowSlime,
                                NPCID.BlackSlime);
                            NPC npc = NPC.NewNPCDirect(NPC.GetSource_FromAI(), (int)NPC.Center.X, (int)NPC.Center.Y, type);
                            npc.velocity = Main.rand.NextVector2Circular(20, 20);
                        }
                    }

                    break;

                case -22:

                    NPC.dontTakeDamage = false;
                    NPC.velocity *= 0.9f;

                    if (Main.mouseRight)
                        FlyTo(Main.MouseWorld);

                    break;

                default:
                    Phase = -5;
                    break;
            };

            HandleLoopedSounds();

            Time++;

            if (!Main.dedServ)
            {
                NPC.localAI[0]++;
                NPC.localAI[1] -= NPC.velocity.X * 0.001f + NPC.direction * 0.02f;

                for (int i = NPCID.Sets.TrailCacheLength[Type] - 1; i > 0; i--)
                {
                    NPC.oldPos[i] = Vector2.Lerp(NPC.oldPos[i], NPC.oldPos[i - 1], 0.37f + (float)Math.Sin(NPC.localAI[0] * 0.33f - i * 2f) * 0.25f);
                    NPC.oldRot[i] = MathHelper.Lerp(NPC.oldRot[i], NPC.oldRot[i - 1], 0.37f + (float)Math.Sin(NPC.localAI[0] * 0.33f - i * 2f) * 0.25f);
                    oldVel[i] = Vector2.Lerp(oldVel[i], oldVel[i - 1], 0.37f + (float)Math.Sin(NPC.localAI[0] * 0.33f - i * 2f) * 0.25f);
                }
                NPC.oldPos[0] = Vector2.Lerp(NPC.oldPos[0], NPC.position, 0.37f + (float)Math.Sin(NPC.localAI[0] * 0.33f) * 0.25f);
                NPC.oldRot[0] = MathHelper.Lerp(NPC.oldRot[0], NPC.rotation, 0.37f + (float)Math.Sin(NPC.localAI[0] * 0.33f) * 0.25f);
                oldVel[0] = Vector2.Lerp(oldVel[0], NPC.velocity, 0.37f + (float)Math.Sin(NPC.localAI[0] * 0.33f) * 0.25f);

                //for (int i = NPCID.Sets.TrailCacheLength[Type] - 1; i > 0; i--)
                //{
                //    NPC.oldPos[i] = Vector2.Lerp(NPC.oldPos[i], NPC.oldPos[i - 1], 0.3f);
                //    NPC.oldRot[i] = MathHelper.Lerp(NPC.oldRot[i], NPC.oldRot[i - 1], 0.3f);
                //    oldVel[i] = Vector2.Lerp(oldVel[i], oldVel[i - 1], 0.3f);
                //}
                //NPC.oldPos[0] = Vector2.Lerp(NPC.oldPos[0], NPC.position, 0.3f);
                //NPC.oldRot[0] = MathHelper.Lerp(NPC.oldRot[0], NPC.rotation, 0.3f);
                //oldVel[0] = Vector2.Lerp(oldVel[0], NPC.velocity, 0.3f);

                if (Main.rand.NextBool())
                {
                    Dust dust = Dust.NewDustDirect(NPC.Center - new Vector2(50), 100, 160, DustID.TintableDust, Main.rand.NextFloat(-1f, 1f), -4f, 230, Color.Black, 2f + Main.rand.NextFloat());
                    dust.noGravity = true;
                }
                if (Main.rand.NextBool(8))
                {
                    Particle hue = Particle.NewParticle(Particle.ParticleType<HueLightDust>(), NPC.Center + Main.rand.NextVector2Circular(60, 80), Main.rand.NextVector2Circular(2, 2) - Vector2.UnitY * 2f, Color.White, 1f);
                    hue.data = NPC.localAI[0];
                }
            }

            //shimmer
            //if (Phase != -2 && Phase != -1 && Main.tile[NPC.Center.ToTileCoordinates()].LiquidType == LiquidID.Shimmer)
            //{
            //    Phase = -2;
            //    Time = 0;
            //    for (int i = 0; i < 4; i++)
            //        KillSlime(currentSlime);
            //    ActiveSlime.active = false;
            //}

            if (hitTimer > 0)
                hitTimer--;
        }

        private void KillSlime(int index)
        {
            switch (index)
            {
                case 0:

                    for (int i = 0; i < Main.rand.Next(4, 9); i++)
                    {
                        Vector2 velocity = Main.rand.NextVector2Circular(8, 1) - Vector2.UnitY * Main.rand.NextFloat(7f, 16f);
                        Vector2 position = ActiveSlime.Center + Main.rand.NextVector2Circular(1, 50) + new Vector2(velocity.X * 12f, 32f);
                        Particle.NewParticle(Particle.ParticleType<EbonBombChunk>(), position, velocity + NPC.velocity, Color.White, 0.1f + Main.rand.NextFloat(2f));
                    }

                    break;

                case 1:

                    for (int i = 0; i < Main.rand.Next(4, 9); i++)
                    {
                        Vector2 velocity = Main.rand.NextVector2Circular(8, 1) - Vector2.UnitY * Main.rand.NextFloat(7f, 16f);
                        Vector2 position = ActiveSlime.Center + Main.rand.NextVector2Circular(1, 50) + new Vector2(velocity.X * 12f, 32f);
                        Particle.NewParticle(Particle.ParticleType<HolyBombChunk>(), position, velocity, Color.White, 0.1f + Main.rand.NextFloat(2f));
                    }

                    break;

                case 2:

                    for (int i = 0; i < Main.rand.Next(4, 9); i++)
                    {
                        Vector2 velocity = Main.rand.NextVector2Circular(8, 1) - Vector2.UnitY * Main.rand.NextFloat(7f, 18f);
                        Vector2 position = ActiveSlime.Center + Main.rand.NextVector2Circular(1, 50) + new Vector2(velocity.X * 16f, 32f);
                        Particle.NewParticle(Particle.ParticleType<CrimBombChunk>(), position, velocity, Color.White, 0.1f + Main.rand.NextFloat(2f));
                    }

                    break;

                case 3:

                    for (int i = 0; i < Main.rand.Next(4, 9); i++)
                    {
                        Vector2 velocity = Main.rand.NextVector2Circular(8, 1) - Vector2.UnitY * Main.rand.NextFloat(7f, 16f);
                        Vector2 position = ActiveSlime.Center + Main.rand.NextVector2Circular(1, 50) + new Vector2(velocity.X * 12f, 32f);
                        Particle.NewParticle(Particle.ParticleType<StarBombChunk>(), position, velocity, Color.White, 0.1f + Main.rand.NextFloat(2f));
                    }

                    break;
            }
        }

        private int hitTimer;

        public override void OnHitByItem(Player player, Item item, NPC.HitInfo hit, int damageDone)
        {
            if (!Main.dedServ && hitTimer <= 0)
            {
                hitTimer += Main.rand.Next(10, 18);
                SoundStyle hurt = new SoundStyle($"{nameof(CalamityHunt)}/Assets/Sounds/Goozma/GoozmaHurt", 1, 3);
                hurt.MaxInstances = 0;
                SoundEngine.PlaySound(hurt, NPC.Center);
            }

            if (Main.rand.NextBool(3))
                for (int i = 0; i < Main.rand.Next(6); i++)
                    Particle.NewParticle(Particle.ParticleType<GoozBombChunk>(), NPC.Center + Main.rand.NextVector2Circular(30, 30), NPC.DirectionFrom(player.Center).RotatedByRandom(0.2f) * Main.rand.Next(4, 10), Color.White, 0.5f + Main.rand.NextFloat());
        }

        public override void OnHitByProjectile(Projectile projectile, NPC.HitInfo hit, int damageDone)
        {
            if (!Main.dedServ && hitTimer <= 0)
            {
                hitTimer += Main.rand.Next(10, 18);
                SoundStyle hurt = new SoundStyle($"{nameof(CalamityHunt)}/Assets/Sounds/Goozma/GoozmaHurt", 1, 3);
                hurt.MaxInstances = 0;
                SoundEngine.PlaySound(hurt, NPC.Center);
            }

            if (Main.rand.NextBool(3))
                for (int i = 0; i < Main.rand.Next(6); i++)
                    Particle.NewParticle(Particle.ParticleType<GoozBombChunk>(), NPC.Center + Main.rand.NextVector2Circular(30, 30), NPC.DirectionFrom(projectile.Center).RotatedByRandom(0.2f) * Main.rand.Next(4, 10), Color.White, 0.5f + Main.rand.NextFloat());
        }

        public override void OnKill()
        {
            BossDownedSystem.downedGoozma = true;
            bool active = SoundEngine.TryGetActiveSound(goozmaWarble, out ActiveSound sound);
            if (active)
                sound.Stop();

            if (!Main.dedServ)
            {
                for (int i = 0; i < Main.musicFade.Length; i++)
                    Main.musicFade[i] = 0.1f;

                SoundStyle deathSound = new SoundStyle($"{nameof(CalamityHunt)}/Assets/Sounds/Goozma/GoozmaPop");
                SoundEngine.PlaySound(deathSound, NPC.Center);
            }

            for (int i = 0; i < 200; i++)
                Particle.NewParticle(Particle.ParticleType<GoozBombChunk>(), NPC.Center + Main.rand.NextVector2Circular(20, 20), Main.rand.NextVector2Circular(30, 30) - Vector2.UnitY * 15f, Color.White, 0.1f + Main.rand.NextFloat(2f));
        }

        private void Fly()
        {
            if (NPC.Distance(Target.Center) < 300)
                NPC.velocity *= 0.9f;

            NPC.velocity = Vector2.Lerp(NPC.velocity, NPC.DirectionTo(Target.Center).SafeNormalize(Vector2.Zero) * Math.Max(0, NPC.Distance(Target.Center) - 300) * 0.04f, 0.02f);
        }        
        
        private void FlyTo(Vector2 target)
        {
            if (NPC.Distance(target) < 20)
                NPC.velocity *= 0.9f;

            NPC.velocity = Vector2.Lerp(NPC.velocity, NPC.DirectionTo(target).SafeNormalize(Vector2.Zero) * Math.Max(1, NPC.Distance(target) * 0.2f), 0.1f);
        }        
        
        private void AresLockTo(Vector2 target)
        {
            NPC.velocity = Vector2.Lerp(NPC.velocity, NPC.DirectionTo(target).SafeNormalize(Vector2.Zero) * Math.Max(1, NPC.Distance(target) * 0.4f), 0.27f);
        }

        private void Dash(int dashCD)
        {
            if (NPC.Center.Y < Target.Center.Y + 90 && NPC.Center.Y > Target.Center.Y - 90)
            {
                Time = 0;
                NPC.velocity.Y = MathHelper.Lerp(NPC.velocity.Y, (NPC.Center.Y - Target.Center.Y) * 0.2f, 0.1f);
                Fly();
            }

                float power = 0.05f;
            if (Time > dashCD - 6)
                power = 0.2f;
            else if (Time > dashCD - 15)
                NPC.velocity *= 0.9f;

            NPC.velocity = Vector2.Lerp(NPC.velocity, NPC.DirectionTo(Target.Center).SafeNormalize(Vector2.Zero) * Math.Clamp(NPC.Distance(Target.Center) * power * 1.5f, 20, 100), power > 0.05f ? 0.1f : 0.03f);

            if (Time > dashCD)
                Time = 0;
        }
        
        private void DashTo(Vector2 targetPos, int time)
        {
            float power = 0.05f;
            if (Time > time - 6)
                power = 0.2f;
            else if (Time > time - 15)
                NPC.velocity *= 0.9f;

            NPC.velocity = Vector2.Lerp(NPC.velocity, NPC.DirectionTo(targetPos).SafeNormalize(Vector2.Zero) * Math.Clamp(NPC.Distance(targetPos) * power * 1.5f, 20, 100), power > 0.05f ? 0.1f : 0.03f);

            if (Time > time)
                Time = 0;
        }

        private int randDirection;

        private void Orbit(int frequency, Vector2 startingVector)
        {
            Vector2 target = Target.Center + startingVector.RotatedBy(Time % frequency / (float)frequency * MathHelper.TwoPi);

            NPC.velocity = Vector2.Lerp(NPC.velocity, NPC.DirectionTo(target).SafeNormalize(Vector2.Zero) * NPC.Distance(target) * 0.1f, 0.1f);
        }

        private enum SortedProjectileAttackTypes
        {
            Default,
            EbonianBubbles,
            EbonianTrifecta,
            CrystalStorm,
            PixieBallDisruption,
            CrimulanSlam,
            CrimulanHop,
            StellarDisruption,
            StellarTaunt,
            BurstLightning,
            DrillDash,
            GaussRay
        }

        private void HandleLoopedSounds()
        {
            HandleGoozmaShootSound();
            HandleGoozmaWarbleSound();
        }

        public static SlotId goozmaWarble;
        public static float goozmaWarbleVolume;
        public static float goozmaWarblePitch;

        public void HandleGoozmaWarbleSound()
        {
            SoundStyle warbleSound = new SoundStyle($"{nameof(CalamityHunt)}/Assets/Sounds/Goozma/GoozmaTravelLoop");
            warbleSound.IsLooped = true;

            float volumeScale = (Phase > 1 ? 0.9f : 0.4f);
            if (Phase == 3)
                volumeScale *= Utils.GetLerpValue(140, 0, Time, true);
            goozmaWarbleVolume = MathHelper.Lerp(goozmaWarbleVolume, Math.Clamp(1f - Main.LocalPlayer.Distance(NPC.Center) * 0.0001f, 0, 1) * NPC.scale * volumeScale + NPC.velocity.Length() * 0.2f, 0.1f);
            goozmaWarblePitch = MathHelper.Lerp(goozmaWarblePitch, Math.Clamp(NPC.velocity.Length() * 0.02f - Main.LocalPlayer.Distance(NPC.Center) * 0.0001f, -0.8f, 0.8f), 0.1f);

            bool active = SoundEngine.TryGetActiveSound(goozmaWarble, out ActiveSound sound);
            if (!active || !goozmaWarble.IsValid)
                goozmaWarble = SoundEngine.PlaySound(warbleSound, NPC.Center);

            else if (active)
            {
                sound.Volume = goozmaWarbleVolume;
                sound.Pitch = goozmaWarblePitch;
                sound.Position = NPC.Center;
            }
        }

        public static SlotId goozmaShoot;
        public static float goozmaShootPowerCurrent;
        public static float goozmaShootPowerTarget;

        public void HandleGoozmaShootSound()
        {
            SoundStyle shootSound = new SoundStyle($"{nameof(CalamityHunt)}/Assets/Sounds/Goozma/GoozmaShootLoop");
            shootSound.IsLooped = true;

            goozmaShootPowerCurrent = MathHelper.Lerp(goozmaShootPowerCurrent, goozmaShootPowerTarget, 0.2f);
            if (goozmaShootPowerCurrent < 0.05f)
                goozmaShootPowerCurrent = 0f;

            bool active = SoundEngine.SoundPlayer.TryGetActiveSound(goozmaShoot, out ActiveSound sound);
            if (!active && goozmaShootPowerCurrent > 0.05f)
                goozmaShoot = SoundEngine.PlaySound(shootSound, NPC.Center);

            else if (active)
            {
                sound.Volume = goozmaShootPowerCurrent * 1.5f;
                sound.Position = NPC.Center;

                if (goozmaShootPowerCurrent <= 0.05f)
                    sound.Stop();
            }

            goozmaShootPowerTarget -= 0.05f;
            if (goozmaShootPowerTarget < 0.1f)
                goozmaShootPowerTarget = 0f;
        }


        private void SortedProjectileAttack(Vector2 targetPos, SortedProjectileAttackTypes type)
        {
            SoundStyle fizzSound = new SoundStyle($"{nameof(CalamityHunt)}/Assets/Sounds/Goozma/GoozmaSlimeShoot", 1, 3);
            fizzSound.MaxInstances = 0;
            fizzSound.Volume = 0.66f;
            fizzSound.PitchVariance = 0.1f;  
            
            SoundStyle bloatSound = new SoundStyle($"{nameof(CalamityHunt)}/Assets/Sounds/Goozma/GoozmaBloatedBlastShoot");
            bloatSound.MaxInstances = 0; 
            bloatSound.PitchVariance = 0.15f;    
            
            SoundStyle dartSound = new SoundStyle($"{nameof(CalamityHunt)}/Assets/Sounds/Goozma/GoozmaDartShoot", 1, 2);
            dartSound.MaxInstances = 0;
            dartSound.Volume = 0.66f;
            dartSound.PitchVariance = 0.2f;   
            
            SoundStyle fireballSound = new SoundStyle($"{nameof(CalamityHunt)}/Assets/Sounds/Goozma/GoozmaRainbowShoot", 1, 2);
            fireballSound.MaxInstances = 0;
            fireballSound.PitchVariance = 0.15f;            

            SoundStyle gaussSound = new SoundStyle($"{nameof(CalamityHunt)}/Assets/Sounds/Goozma/GoozmaShot", 1, 2);
            gaussSound.MaxInstances = 0;

            switch (type)
            {
                case SortedProjectileAttackTypes.Default:

                    if (NPC.Distance(Target.Center) > 50)
                    {
                        if (Time % 60 == 0)
                            Projectile.NewProjectileDirect(NPC.GetSource_FromAI(), NPC.Center, NPC.DirectionTo(targetPos).SafeNormalize(Vector2.Zero).RotatedByRandom(0.3f) * 3f, ModContent.ProjectileType<SlimeShot>(), 5, 0);
                    }

                    break;

                case SortedProjectileAttackTypes.EbonianBubbles:

                    if (NPC.Distance(Target.Center) > 300)
                    {
                        if (Time % 15 == 0 && !Main.dedServ)
                        {
                            SoundEngine.PlaySound(fizzSound, NPC.Center);
                            goozmaShootPowerTarget = 1f;
                        }

                        float angle = MathHelper.SmoothStep(1.3f, 0.7f, Time / 350f);
                        if (Time % 6 == 0)
                        {
                            Projectile.NewProjectileDirect(NPC.GetSource_FromAI(), NPC.Center, Vector2.UnitY.RotatedBy(angle).RotatedByRandom(0.3f) * Main.rand.Next(4, 7), ModContent.ProjectileType<SlimeShot>(), GetDamage(1), 0);
                            Projectile.NewProjectileDirect(NPC.GetSource_FromAI(), NPC.Center, Vector2.UnitY.RotatedBy(-angle).RotatedByRandom(0.3f) * Main.rand.Next(4, 7), ModContent.ProjectileType<SlimeShot>(), GetDamage(1), 0);
                        }
                        if ((Time + 6) % 6 == 0)
                        {
                            Projectile.NewProjectileDirect(NPC.GetSource_FromAI(), NPC.Center, Vector2.UnitY.RotatedBy(angle + 0.3f).RotatedByRandom(0.3f) * Main.rand.Next(2, 4), ModContent.ProjectileType<SlimeShot>(), GetDamage(1), 0);
                            Projectile.NewProjectileDirect(NPC.GetSource_FromAI(), NPC.Center, Vector2.UnitY.RotatedBy(-angle - 0.3f).RotatedByRandom(0.3f) * Main.rand.Next(2, 4), ModContent.ProjectileType<SlimeShot>(), GetDamage(1), 0);
                        }
                        if (Target.Center.Y < NPC.Top.Y - 300)
                            Projectile.NewProjectileDirect(NPC.GetSource_FromAI(), NPC.Center, NPC.DirectionTo(Target.Center).SafeNormalize(Vector2.Zero) * Main.rand.Next(2, 8), ModContent.ProjectileType<SlimeShot>(), GetDamage(1), 0);
                    }

                    break;

                case SortedProjectileAttackTypes.EbonianTrifecta:

                    if (Time >= 180)
                    {
                        if (Time - 180 % 60 == 0)
                        {
                            if (!Main.dedServ)
                            {
                                SoundEngine.PlaySound(fizzSound, NPC.Center);
                                goozmaShootPowerTarget = 1f;
                            }

                            Projectile.NewProjectileDirect(NPC.GetSource_FromAI(), NPC.Center, NPC.DirectionTo(targetPos).SafeNormalize(Vector2.Zero).RotatedByRandom(0.3f) * 3f, ModContent.ProjectileType<SlimeShot>(), GetDamage(1), 0);
                        }
                    }


                    break;

                case SortedProjectileAttackTypes.CrystalStorm:

                    if (NPC.Distance(Target.Center) > 300)
                    {
                        if (Time % 18 == 0 && !Main.dedServ)
                        {
                            SoundEngine.PlaySound(fizzSound, NPC.Center);
                            goozmaShootPowerTarget = 1f;
                        }

                        float angle = MathHelper.SmoothStep(-1.3f, -0.8f, Time / 350f);
                        if (Time % 15 == 0)
                        {
                            Projectile.NewProjectileDirect(NPC.GetSource_FromAI(), NPC.Center, -Vector2.UnitY.RotatedBy(angle).RotatedByRandom(0.2f) * Main.rand.Next(4, 7), ModContent.ProjectileType<SlimeShot>(), GetDamage(1), 0);
                            Projectile.NewProjectileDirect(NPC.GetSource_FromAI(), NPC.Center, -Vector2.UnitY.RotatedBy(-angle).RotatedByRandom(0.2f) * Main.rand.Next(4, 7), ModContent.ProjectileType<SlimeShot>(), GetDamage(1), 0);
                        }
                        if ((Time + 8) % 8 == 0)
                        {
                            Projectile.NewProjectileDirect(NPC.GetSource_FromAI(), NPC.Center, -Vector2.UnitY.RotatedBy(angle - 0.3f).RotatedByRandom(0.2f) * Main.rand.Next(2, 4), ModContent.ProjectileType<SlimeShot>(), GetDamage(1), 0);
                            Projectile.NewProjectileDirect(NPC.GetSource_FromAI(), NPC.Center, -Vector2.UnitY.RotatedBy(-angle + 0.3f).RotatedByRandom(0.2f) * Main.rand.Next(2, 4), ModContent.ProjectileType<SlimeShot>(), GetDamage(1), 0);
                        }
                        if (Target.Center.Y > NPC.Top.Y + 300)
                            Projectile.NewProjectileDirect(NPC.GetSource_FromAI(), NPC.Center, NPC.DirectionTo(Target.Center).SafeNormalize(Vector2.Zero) * Main.rand.Next(2, 8), ModContent.ProjectileType<SlimeShot>(), GetDamage(1), 0);
                    }

                    break;

                case SortedProjectileAttackTypes.PixieBallDisruption:

                    if (Time % 80 == 75)
                    {
                        int count = Main.rand.Next(10, 18);
                        for (int i = 0; i < count; i++)
                        {
                            Projectile dart = Projectile.NewProjectileDirect(NPC.GetSource_FromAI(), NPC.Center, new Vector2(Main.rand.Next(10, 20), 0).RotatedBy(MathHelper.TwoPi / count * i), ModContent.ProjectileType<BloatedBlast>(), GetDamage(1), 0);
                            dart.ai[1] = 1;
                            dart.localAI[0] = NPC.localAI[0] + i / (float)count * 90f;
                        }
                        if (!Main.dedServ)
                            SoundEngine.PlaySound(dartSound, NPC.Center);
                    }

                    break;
                
                case SortedProjectileAttackTypes.CrimulanSlam:

                    if (NPC.Distance(Target.Center) > 300)
                    {
                        if (Time % 25 == 0 && !Main.dedServ)
                        {
                            SoundEngine.PlaySound(fizzSound, NPC.Center);
                            goozmaShootPowerTarget = 1f;
                        }

                        if ((Time + Main.rand.Next(0, 2)) % 8 == 0)
                            Projectile.NewProjectileDirect(NPC.GetSource_FromAI(), NPC.Center, NPC.DirectionTo(targetPos).SafeNormalize(Vector2.Zero).RotatedByRandom(0.15f).RotatedBy(0.4f) * 20f, ModContent.ProjectileType<SlimeShot>(), GetDamage(1), 0);
                        if ((Time + Main.rand.Next(0, 2)) % 8 == 0)
                            Projectile.NewProjectileDirect(NPC.GetSource_FromAI(), NPC.Center, NPC.DirectionTo(targetPos).SafeNormalize(Vector2.Zero).RotatedByRandom(0.15f).RotatedBy(-0.4f) * 20f, ModContent.ProjectileType<SlimeShot>(), GetDamage(1), 0);

                    }

                    break;

                case SortedProjectileAttackTypes.CrimulanHop:

                    if (NPC.Distance(Target.Center) > 100)
                    {
                        if (Time % 10 == 0 && !Main.dedServ)
                        {
                            SoundEngine.PlaySound(fizzSound, NPC.Center);
                            goozmaShootPowerTarget = 1f;
                        }

                        if (Time % 25 == 0)
                            Projectile.NewProjectileDirect(NPC.GetSource_FromAI(), NPC.Center, NPC.DirectionTo(targetPos).SafeNormalize(Vector2.Zero).RotatedByRandom(0.3f) * 3f, ModContent.ProjectileType<SlimeShot>(), GetDamage(1), 0);

                        if (Time % 40 == 0)
                            Projectile.NewProjectileDirect(NPC.GetSource_FromAI(), NPC.Center, NPC.DirectionFrom(targetPos).SafeNormalize(Vector2.Zero).RotatedByRandom(0.3f) * 4f, ModContent.ProjectileType<SlimeBomb>(), GetDamage(2), 0);

                    }

                    break;

                case SortedProjectileAttackTypes.StellarDisruption:

                    //if ((Time - 70) % 160 > 100)
                    //{
                    //    if ((Time - 70) % 5 == 0 && !Main.dedServ)
                    //    {
                    //        SoundEngine.PlaySound(fizzSound, NPC.Center);
                    //        goozmaShootPowerTarget = 1f;
                    //    }
                    //    Projectile.NewProjectileDirect(NPC.GetSource_FromAI(), NPC.Center, NPC.DirectionTo(targetPos).SafeNormalize(Vector2.Zero).RotatedByRandom(0.3f) * 8f, ModContent.ProjectileType<SlimeShot>(), GetDamage(1), 0);
                    //}
                    if ((Time - 70) % 160 > 80 && (Time - 70) % 15 == 2)
                    {
                        Projectile pure = Projectile.NewProjectileDirect(NPC.GetSource_FromAI(), NPC.Center, NPC.DirectionTo(targetPos).SafeNormalize(Vector2.Zero).RotatedByRandom(1.3f) * (NPC.Distance(targetPos) + 220) * 0.02f * Main.rand.NextFloat(0.9f, 1.2f), ModContent.ProjectileType<SlimeBomb>(), GetDamage(2), 0);
                        pure.ai[0] = (int)((Time - 70) % 160 / 1.5f) - 30;
                    }

                    break;

                case SortedProjectileAttackTypes.StellarTaunt:

                    if (Time % 100 == 40 || Main.rand.NextBool(180))
                    {
                        if (!Main.dedServ)
                        {
                            SoundEngine.PlaySound(fizzSound, NPC.Center);
                            goozmaShootPowerTarget = 1f;
                        }

                        Projectile.NewProjectileDirect(NPC.GetSource_FromAI(), NPC.Center, NPC.DirectionTo(targetPos).SafeNormalize(Vector2.Zero).RotatedByRandom(1f) * 2f, ModContent.ProjectileType<SlimeShot>(), GetDamage(1), 0);
                    }

                    if (Time % 130 == 10)
                        Projectile.NewProjectileDirect(NPC.GetSource_FromAI(), NPC.Center, NPC.DirectionTo(targetPos).SafeNormalize(Vector2.Zero).RotatedByRandom(1.3f) * NPC.Distance(targetPos) * 0.016f * Main.rand.NextFloat(0.9f, 1.2f), ModContent.ProjectileType<SlimeBomb>(), GetDamage(2), 0);

                    break;

                case SortedProjectileAttackTypes.BurstLightning:

                    if (Time % 170 > 130)
                    {
                        int freq = 10;
                        if (Main.expertMode)
                            freq = 8;
                        if ((Time % 170) % freq == 0)
                        {
                            if (!Main.dedServ)
                                SoundEngine.PlaySound(fireballSound, NPC.Center);

                            Projectile fireball = Projectile.NewProjectileDirect(NPC.GetSource_FromAI(), NPC.Center, NPC.DirectionTo(Target.Center).SafeNormalize(Vector2.Zero).RotatedByRandom(0.6f) * 16f, ModContent.ProjectileType<RainbowBall>(), GetDamage(4), 0);
                            fireball.localAI[0] = NPC.localAI[0];
                        }
                    }
                    else if (Time % 20 == 0)
                    {
                        if (!Main.dedServ)
                        {
                            SoundEngine.PlaySound(fizzSound, NPC.Center);
                            goozmaShootPowerTarget = 1f;
                        }
                        Projectile.NewProjectileDirect(NPC.GetSource_FromAI(), NPC.Center, NPC.DirectionTo(targetPos).SafeNormalize(Vector2.Zero).RotatedByRandom(1f), ModContent.ProjectileType<SlimeShot>(), GetDamage(1), 0);
                    }

                    if (Time % 270 == 0)
                    {
                        Projectile.NewProjectileDirect(NPC.GetSource_FromAI(), NPC.Center, NPC.DirectionTo(targetPos).SafeNormalize(Vector2.Zero) * 10, ModContent.ProjectileType<BloatedBlast>(), GetDamage(5), 0);
                        if (!Main.dedServ)
                            SoundEngine.PlaySound(bloatSound, NPC.Center);
                    }

                    if (Time % 140 == 0)
                        Projectile.NewProjectileDirect(NPC.GetSource_FromAI(), NPC.Center, NPC.DirectionTo(targetPos).SafeNormalize(Vector2.Zero), ModContent.ProjectileType<GooLightning>(), GetDamage(3), 0, -1, -80, 1500, 0);

                    if (Time % 55 == 0 && !Main.rand.NextBool(20))
                        Projectile.NewProjectileDirect(NPC.GetSource_FromAI(), NPC.Center, NPC.DirectionTo(targetPos).SafeNormalize(Vector2.Zero).RotatedByRandom(2f), ModContent.ProjectileType<GooLightning>(), GetDamage(4), 0, -1, -50, 1500, 1);

                    break;

                case SortedProjectileAttackTypes.DrillDash:

                    int dashTime = 110;
                    if (Main.expertMode)
                        dashTime = 90;

                    if (Time % dashTime > 24)
                    {
                        if (Time % dashTime == 25 && !Main.dedServ)
                        {
                            SoundEngine.PlaySound(fizzSound, NPC.Center);
                            goozmaShootPowerTarget = 1f;
                        }
                        if (Time % dashTime < 30)
                            Projectile.NewProjectileDirect(NPC.GetSource_FromAI(), NPC.Center, NPC.DirectionTo(targetPos).SafeNormalize(Vector2.Zero).RotatedByRandom(1f) * 2.5f, ModContent.ProjectileType<SlimeShot>(), GetDamage(1), 0);

                    }

                    if (Time % 30 == 5)
                        Projectile.NewProjectileDirect(NPC.GetSource_FromAI(), NPC.Center, NPC.DirectionTo(targetPos).SafeNormalize(Vector2.Zero).RotatedByRandom(0.1f) * 10f, ModContent.ProjectileType<SlimeBomb>(), GetDamage(2), 0);
                    
                    if (Time % dashTime == 70)
                    {
                        if (!Main.dedServ)
                            SoundEngine.PlaySound(dartSound, NPC.Center);

                        int count = Main.rand.Next(18, 25);
                        for (int i = 0; i < count; i++)
                        {
                            Projectile dart = Projectile.NewProjectileDirect(NPC.GetSource_FromAI(), NPC.Center, new Vector2(Main.rand.Next(8, 15), 0).RotatedBy(MathHelper.TwoPi / count * i), ModContent.ProjectileType<BloatedBlast>(), GetDamage(1), 0);
                            dart.ai[1] = 1;
                            dart.localAI[0] = NPC.localAI[0] + i / (float)count * 90f;
                        }
                    }

                    break;                
                
                case SortedProjectileAttackTypes.GaussRay:

                    if (Time < 350 && Time % 25 == 0)
                    {
                        if (!Main.dedServ)
                        {
                            SoundEngine.PlaySound(fizzSound, NPC.Center);
                            goozmaShootPowerTarget = 1f;
                        }

                        for (int i = 0; i < Main.rand.Next(5, 8); i++)
                            Projectile.NewProjectileDirect(NPC.GetSource_FromAI(), NPC.Center, NPC.DirectionTo(targetPos).SafeNormalize(Vector2.Zero).RotatedByRandom(1f) * 2.5f, ModContent.ProjectileType<SlimeShot>(), GetDamage(1), 0);
                        
                        if (Time > 100 && Time % 50 == 0)
                        {
                            if (!Main.dedServ)
                                SoundEngine.PlaySound(fireballSound, NPC.Center);

                            Projectile fireball = Projectile.NewProjectileDirect(NPC.GetSource_FromAI(), NPC.Center, NPC.DirectionTo(Target.Center).SafeNormalize(Vector2.Zero).RotatedByRandom(0.6f) * 16f, ModContent.ProjectileType<RainbowBall>(), GetDamage(4), 0);
                            fireball.localAI[0] = NPC.localAI[0];
                        }
                    }

                    if (Time > 150 && Time <= 450 && Time % 90 == 0)
                    {
                        Projectile.NewProjectileDirect(NPC.GetSource_FromAI(), NPC.Center, NPC.DirectionTo(targetPos).SafeNormalize(Vector2.Zero) * 5, ModContent.ProjectileType<BloatedBlast>(), GetDamage(5), 0);
                        if (!Main.dedServ)
                            SoundEngine.PlaySound(bloatSound, NPC.Center);
                    }

                    if (Time > 250 && Time < 530 && Time % 40 == 0)
                        Projectile.NewProjectileDirect(NPC.GetSource_FromAI(), NPC.Center, NPC.DirectionTo(Target.Center).SafeNormalize(Vector2.Zero).RotatedByRandom(1f), ModContent.ProjectileType<GooLightning>(), GetDamage(4), 0, -1, -50, 1500, 1);

                    if (Time == 250)
                    {
                        Projectile gaussRay = Projectile.NewProjectileDirect(NPC.GetSource_FromAI(), NPC.Center, NPC.DirectionTo(Target.Center).SafeNormalize(Vector2.Zero), ModContent.ProjectileType<GaussRay>(), GetDamage(7), 0);
                        gaussRay.ai[2] = NPC.whoAmI;
                    }

                    break;
            }
        }

        private int GetDamage(int attack, float modifier = 1f)
        {
            int damage = attack switch
            {
                0 => 5,//contact
                1 => 50,//slime balls
                2 => 80,//pure gel
                3 => 120, //lightning
                4 => 20, //static
                5 => 50, //bloat
                6 => 60, //drill dash
                7 => 100, //gauss ray
                _ => 0
            };

            return (int)(damage * modifier);
        }

        private float extraTilt;
        private Vector2 drawOffset;
        private float headScale;

        private void FadeMusicOut(On_Main.orig_UpdateAudio orig, Main self)
        {
            orig(self);

            if (Main.npc.Any(n => n.active && n.type == ModContent.NPCType<Goozma>()))
            {
                NPC goozma = Main.npc.FirstOrDefault(n => n.active && n.type == Type);
                if (goozma.ai[2] == 1)
                {
                    for (int i = 0; i < Main.musicFade.Length; i++)
                    {
                        float volume = Main.musicFade[i] * Main.musicVolume * Utils.GetLerpValue(320, 20, goozma.ai[0], true);
                        float tempFade = Main.musicFade[i];
                        Main.audioSystem.UpdateCommonTrackTowardStopping(i, volume, ref tempFade, Main.musicFade[i] > 0.15f && goozma.ai[0] < 320);
                        Main.musicFade[i] = tempFade;
                    }
                }                
                if (goozma.ai[2] == 3)
                {
                    for (int i = 0; i < Main.musicFade.Length; i++)
                    {
                        float volume = Main.musicFade[i] * Main.musicVolume * Utils.GetLerpValue(170, 20, goozma.ai[0], true);
                        float tempFade = Main.musicFade[i];
                        Main.audioSystem.UpdateCommonTrackTowardStopping(i, volume, ref tempFade, Main.musicFade[i] > 0.15f && goozma.ai[0] < 320);
                        Main.musicFade[i] = tempFade;
                    }
                }
            }
        }
    }
}
