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
    public class CrimulanGlopstrosity : ModNPC
    {
        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Crimulan Glopstrosity"); 
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
            bestiaryEntry.Info.AddRange(new List<IBestiaryInfoElement> {
                new MoonLordPortraitBackgroundProviderBestiaryInfoElement(),
                new FlavorTextBestiaryInfoElement("Mods.CalamityHunt.Bestiary.CrimulanGlopstrosity"),
                BestiaryDatabaseNPCsPopulator.CommonTags.SpawnConditions.Events.SlimeRain,
                BestiaryDatabaseNPCsPopulator.CommonTags.SpawnConditions.Biomes.TheCrimson,
            });
        }

        public override void SetDefaults()
        {
            NPC.width = 210;
            NPC.height = 150;
            NPC.damage = 12;
            NPC.defense = 500;
            NPC.lifeMax = 3000000;
            NPC.HitSound = SoundID.NPCHit1;
            NPC.DeathSound = SoundID.NPCDeath1;
            NPC.knockBackResist = 0f;
            NPC.noGravity = true;
            NPC.noTileCollide = false;
            NPC.value = Item.buyPrice(gold: 5);
            NPC.SpawnWithHigherTime(30);
            NPC.npcSlots = 10f;
            NPC.aiStyle = -1;
            NPC.immortal = true;
            NPC.takenDamageMultiplier = 0.33f;
        }

        private enum AttackList
        {
            SlamWave,
            CollidingCrush,
            EndlessChase,
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
            if (!Main.npc.Any(n => n.type == ModContent.NPCType<Goozma>() && n.active))
                NPC.active = false;
            else
                NPC.ai[2] = Main.npc.First(n => n.type == ModContent.NPCType<Goozma>() && n.active).whoAmI;

            NPC.realLife = Host.whoAmI;

            NPC.frameCounter++;
            if (NPC.frameCounter > 9)
            {
                NPC.frameCounter = 0;
                npcFrame = (npcFrame + 1) % 4;
            }

            if (!NPC.HasPlayerTarget)
                NPC.TargetClosestUpgraded();
            if (!NPC.HasPlayerTarget)
                NPC.active = false;

            foreach (Player player in Main.player.Where(n => n.active && !n.dead))
            {
                float distance = 0;
                for (int i = 0; i < Main.tile.Height; i++)
                {
                    Point playerTile = player.MountedCenter.ToTileCoordinates();
                    if (WorldGen.InWorld(playerTile.X, playerTile.Y + i))
                    {
                        if (WorldGen.SolidTileAllowTopSlope(playerTile.X, playerTile.Y + i))
                        {
                            distance = i;
                            break;
                        }
                    }
                    else
                    {
                        distance = i;
                        break;
                    }
                }
                if (distance > 36)
                    player.velocity.Y += distance * 0.09f;
                //inflict debuff
            }

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
                    case (int)AttackList.SlamWave:
                        SlamWave();
                        break;
                    case (int)AttackList.CollidingCrush:
                        CollidingCrush();
                        break;
                    case (int)AttackList.EndlessChase:
                        EndlessChase();
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

        private void SlamWave()
        {
            int waitTime = 40;
            if (Time < 32)
            {
                NPC.velocity *= 0.1f;
                squishFactor = new Vector2(1f + (float)Math.Cbrt(Utils.GetLerpValue(10, 26, Time, true)) * 0.4f, 1f - (float)Math.Sqrt(Utils.GetLerpValue(10, 26, Time, true)) * 0.5f);
                if (Time == 25 && !Main.dedServ)
                {
                    SoundEngine.PlaySound(SoundID.Item146, NPC.Center);
                    SoundEngine.PlaySound(SoundID.QueenSlime, NPC.Center);
                }

            }
            else if (Time < 35)
            {
                NPC.noTileCollide = true;
                NPC.velocity.Y = -14;
                squishFactor = new Vector2(0.5f, 1.4f);
            }    

            else if (Time < waitTime + 100)
            {
                Vector2 airTarget = Target.Center - Vector2.UnitY * 450 + Target.Velocity * 20;

                if (Time < waitTime + 20)
                    squishFactor = new Vector2(1f - Utils.GetLerpValue(waitTime + 18, waitTime, Time, true) * 0.5f, 1f + (float)Math.Cbrt(Utils.GetLerpValue(waitTime + 18, waitTime, Time, true)) * 0.4f);

                if (Time < waitTime + 45)
                {
                    NPC.velocity = Vector2.Lerp(NPC.velocity, NPC.DirectionTo(airTarget).SafeNormalize(Vector2.Zero) * Math.Max(2, NPC.Distance(airTarget)) * 0.2f, 0.08f) * Utils.GetLerpValue(waitTime + 50, waitTime + 35, Time, true);
                    saveTarget = Target.Center + Target.Velocity * 30;
                    NPC.rotation = NPC.rotation.AngleLerp(NPC.Top.AngleTo(NPC.FindSmashSpot(saveTarget)) - MathHelper.PiOver2, 0.5f) * 0.4f;
                }
                else
                {
                    NPC.rotation = NPC.rotation.AngleLerp(NPC.Top.AngleTo(NPC.FindSmashSpot(saveTarget)) - MathHelper.PiOver2, 0.2f);
                    NPC.noTileCollide = false;
                    NPC.Center = Vector2.Lerp(NPC.Center, NPC.FindSmashSpot(saveTarget), Utils.GetLerpValue(waitTime + 45, waitTime + 60, Time, true));
                    NPC.velocity *= 0.5f;

                    if (Time < waitTime + 60)
                    {
                        squishFactor = new Vector2(1f - (float)Math.Pow(Utils.GetLerpValue(waitTime + 50, waitTime + 53, Time, true), 2) * 0.5f, 1f + (float)Math.Pow(Utils.GetLerpValue(waitTime + 50, waitTime + 52, Time, true), 2) * 0.5f);
                        NPC.frameCounter++;
                    }

                    if (Time > waitTime + 60)
                    {
                        NPC.rotation = 0;
                        squishFactor = new Vector2(1f + (float)Math.Pow(Utils.GetLerpValue(waitTime + 78, waitTime + 63, Time, true), 2) * 0.6f, 1f - (float)Math.Pow(Utils.GetLerpValue(waitTime + 78, waitTime + 63, Time, true), 4) * 0.8f);
                    }

                    if (Time == waitTime + 60)
                    {
                        int extension = 10;
                        if (Main.expertMode)
                            extension = 16;

                        for (int i = 0; i < extension; i++)
                        {
                            Projectile proj = Projectile.NewProjectileDirect(NPC.GetSource_FromAI(), NPC.FindSmashSpot(NPC.Center + new Vector2(150 * i * (NPC.Center.X > Target.Center.X ? -1 : 1), 0)), Vector2.Zero, ModContent.ProjectileType<CrimulanSmasher>(), 15, 0);
                            proj.ai[0] = -40 - 9 * i;
                            proj.ai[1] = -1;
                            proj.localAI[0] = 1;
                        }

                        foreach (Player player in Main.player.Where(n => n.active && !n.dead && n.Distance(NPC.Center) < 600))
                            player.velocity += player.DirectionFrom(NPC.Bottom + Vector2.UnitY * 10) * 5;

                        if (!Main.dedServ)
                            SoundEngine.PlaySound(SoundID.Item167, NPC.Center);
                    }

                    if (Time >= 106 && Time % 2 == 0)
                        Main.instance.CameraModifiers.Add(new PunchCameraModifier(saveTarget, Main.rand.NextVector2CircularEdge(3, 3), 5f, 10, 12));
                }
            }

            if (Time > waitTime + 180)
                Reset();
        }
        
        private void CollidingCrush()
        {
            int waitTime = 70;
            if (Time < 62)
            {
                NPC.velocity *= 0.1f;
                squishFactor = new Vector2(1f + (float)Math.Cbrt(Utils.GetLerpValue(15, 56, Time, true)) * 0.4f, 1f - (float)Math.Sqrt(Utils.GetLerpValue(15, 56, Time, true)) * 0.5f);
                if (Time == 58 && !Main.dedServ)
                {
                    SoundEngine.PlaySound(SoundID.Item146, NPC.Center);
                    SoundEngine.PlaySound(SoundID.QueenSlime, NPC.Center);
                }
            }
            else if (Time < 65)
            {
                NPC.noTileCollide = true;
                NPC.velocity.Y = -14;
                squishFactor = new Vector2(0.5f, 1.4f);
            }

            else if (Time < waitTime + 80)
            {
                Vector2 airTarget = Target.Center - Vector2.UnitY * 300;

                if (Time < waitTime + 20)
                    squishFactor = new Vector2(1f - Utils.GetLerpValue(waitTime + 18, waitTime, Time, true) * 0.5f, 1f + (float)Math.Cbrt(Utils.GetLerpValue(waitTime + 18, waitTime, Time, true)) * 0.4f);

                if (Time < waitTime + 50)
                {
                    NPC.velocity = Vector2.Lerp(NPC.velocity, NPC.DirectionTo(airTarget).SafeNormalize(Vector2.Zero) * Math.Max(2, NPC.Distance(airTarget)) * 0.1f, 0.06f) * Utils.GetLerpValue(waitTime + 50, waitTime + 35, Time, true);
                    saveTarget = Target.Center;
                    NPC.rotation = NPC.rotation.AngleLerp(NPC.Top.AngleTo(NPC.FindSmashSpot(saveTarget)) - MathHelper.PiOver2, 0.5f) * 0.4f;
                }
                else
                {
                    NPC.rotation = NPC.rotation.AngleLerp(NPC.Top.AngleTo(NPC.FindSmashSpot(saveTarget)) - MathHelper.PiOver2, 0.2f);
                    NPC.noTileCollide = false;
                    NPC.Center = Vector2.Lerp(NPC.Center, NPC.FindSmashSpot(saveTarget), Utils.GetLerpValue(waitTime + 53, waitTime + 60, Time, true));
                    NPC.velocity *= 0.5f;

                    if (Time < waitTime + 60)
                        squishFactor = new Vector2(1f - (float)Math.Pow(Utils.GetLerpValue(waitTime + 50, waitTime + 53, Time, true), 2) * 0.5f, 1f + (float)Math.Pow(Utils.GetLerpValue(waitTime + 50, waitTime + 52, Time, true), 2) * 0.5f);

                    if (Time > waitTime + 60)
                    {
                        NPC.rotation = 0;
                        squishFactor = new Vector2(1f + (float)Math.Pow(Utils.GetLerpValue(waitTime + 78, waitTime + 63, Time, true), 2) * 0.6f, 1f - (float)Math.Pow(Utils.GetLerpValue(waitTime + 78, waitTime + 63, Time, true), 4) * 0.8f);
                    }

                    if (Time == waitTime + 60)
                    {
                        int count = 8;
                        int time = 10;
                        if (Main.expertMode)
                            time = 7;

                        for (int i = 0; i < count; i++)
                        {
                            Projectile leftProj = Projectile.NewProjectileDirect(NPC.GetSource_FromAI(), NPC.FindSmashSpot(NPC.Center + new Vector2(i * 120 - 120 * count - 50, 0)), Vector2.Zero, ModContent.ProjectileType<CrimulanSmasher>(), 15, 0);
                            leftProj.ai[0] = -20 - time * i;
                            leftProj.ai[1] = -1;
                            leftProj.localAI[0] = 1;                            
                            Projectile rightProj = Projectile.NewProjectileDirect(NPC.GetSource_FromAI(), NPC.FindSmashSpot(NPC.Center + new Vector2(i * -120 + 120 * count + 50, 0)), Vector2.Zero, ModContent.ProjectileType<CrimulanSmasher>(), 15, 0);
                            rightProj.ai[0] = -20 - time * i;
                            rightProj.ai[1] = -1;
                            rightProj.localAI[0] = 1;
                        }
                        if (!Main.dedServ)
                            SoundEngine.PlaySound(SoundID.Item167, NPC.Center);
                    }


                    if (Time >= waitTime + 55 && Time % 2 == 0)
                        Main.instance.CameraModifiers.Add(new PunchCameraModifier(saveTarget, Main.rand.NextVector2CircularEdge(3, 3), 5f, 10, 12));
                }
            }

            if (Time > waitTime + 80)
            {
                if (Time < waitTime + 100)
                {
                    NPC.velocity *= 0.1f;
                    squishFactor = new Vector2(1f + (float)Math.Cbrt(Utils.GetLerpValue(waitTime + 80, waitTime + 100, Time, true)) * 0.4f, 1f - (float)Math.Sqrt(Utils.GetLerpValue(waitTime + 80, waitTime + 100, Time, true)) * 0.5f);
                    if (Time == waitTime + 95 && !Main.dedServ)
                        SoundEngine.PlaySound(SoundID.Item146, NPC.Center);
                }
                else if (Time < waitTime + 150)
                {
                    NPC.noTileCollide = true;
                    NPC.velocity.Y = -14 * Utils.GetLerpValue(waitTime + 150, waitTime + 110, Time, true);
                    squishFactor = new Vector2(1f - Utils.GetLerpValue(waitTime + 150, waitTime + 110, Time, true) * 0.5f, 1f + Utils.GetLerpValue(waitTime + 150, waitTime + 110, Time, true) * 0.5f);
                }
            }

            if (Time > waitTime + 150)
                Reset();
        }

        //private void Dash()
        //{
        //    NPC.noTileCollide = true;

        //    if (Time < 20)
        //    {
        //        NPC.velocity *= 0.1f;
        //        squishFactor = new Vector2(1f + (float)Math.Cbrt(Utils.GetLerpValue(0, 16, Time, true)) * 0.4f, 1f - (float)Math.Sqrt(Utils.GetLerpValue(0, 16, Time, true)) * 0.5f);
        //        if (Time == 15)
        //            SoundEngine.PlaySound(SoundID.Item146, NPC.Center);
        //    }
        //    else if (Time < 200)
        //    {
        //        if (Time == 21)
        //            NPC.velocity.Y -= 20;

        //        Vector2 side = Target.Center + new Vector2(400 * (Target.Center.X > NPC.Center.X ? -1 : 1), 0);

        //        if (Time < 65)
        //        {
        //            NPC.velocity = Vector2.Lerp(NPC.velocity, NPC.DirectionTo(side).SafeNormalize(Vector2.Zero) * 0.1f * Math.Max(1, NPC.Distance(side)), 0.15f);
        //            squishFactor = new Vector2(1f - (float)Math.Cbrt(Utils.GetLerpValue(50, 30, Time, true)) * 0.4f, 1f + (float)Math.Sqrt(Utils.GetLerpValue(50, 30, Time, true)) * 0.4f);
        //        }

        //        if (Time < 120)
        //        {
        //            side = Target.Center + new Vector2((400 + (float)Math.Sqrt(Utils.GetLerpValue(70, 118, Time, true)) * 100) * (Target.Center.X > NPC.Center.X ? -1 : 1), 0);
        //            NPC.velocity = Vector2.Lerp(NPC.velocity, NPC.DirectionTo(side).SafeNormalize(Vector2.Zero) * 0.1f * Math.Max(1, NPC.Distance(side)), 0.3f) * Utils.GetLerpValue(115, 110, Time, true);
        //            squishFactor = new Vector2(1f - MathHelper.SmoothStep(0, 1, Utils.GetLerpValue(70, 110, Time, true)) * 0.4f, 1f + (float)Math.Sqrt(Utils.GetLerpValue(70, 120, Time, true)) * 0.1f);
        //            if (Time < 115)
        //                saveTarget = NPC.Center - new Vector2(900 * (Target.Center.X > NPC.Center.X ? -1 : 1), 0);
        //        }
        //        else if (Time < 129)
        //        {
        //            if (Time == 121)
        //                NPC.velocity = NPC.DirectionTo(saveTarget).SafeNormalize(Vector2.Zero) * 15f;
        //            NPC.Center = Vector2.Lerp(NPC.Center, saveTarget, Utils.GetLerpValue(120, 128, Time, true));
        //            squishFactor = new Vector2(1.5f, 0.7f);
        //        }                
        //        else if (Time < 150)
        //        {
        //            NPC.velocity *= 0.8f;
        //            squishFactor = new Vector2(1f + (float)Math.Pow(Utils.GetLerpValue(150, 132, Time, true), 2) * 0.5f, 1f - (float)Math.Pow(Utils.GetLerpValue(150, 132, Time, true), 2) * 0.3f);
        //        }
        //    }

        //    if (Time > 150)
        //        Reset();
        //}

        private void EndlessChase()
        {
            int jumpCount = 8;
            int jumpTime = 50;
            if (Main.expertMode)
            {
                jumpCount = 10;
                jumpTime = 40;
            }

            if (Time < 35)
                squishFactor = new Vector2(1f + (float)Math.Pow(Utils.GetLerpValue(5, 35, Time, true), 2) * 0.5f, 1f - (float)Math.Pow(Utils.GetLerpValue(5, 35, Time, true), 2) * 0.5f);

            else if (Time < 35 + jumpCount * jumpTime)
            {
                float localTime = (Time - 35) % jumpTime;

                if (localTime == 0)
                {
                    NPC.velocity.Y -= 30;
                    saveTarget = Target.Center + new Vector2((Main.rand.Next(0, 50) + Math.Abs(Target.Velocity.X) * 25 + 360) * (Target.Center.X > NPC.Center.X ? 1 : -1), NPC.height);
                    if (!Main.dedServ)
                        SoundEngine.PlaySound(SoundID.Item146, NPC.Center);
                }
                else if (localTime < (int)(jumpTime * 0.72f))
                {
                    Vector2 midPoint = new Vector2((NPC.Center.X + saveTarget.X) / 2f, NPC.Center.Y - NPC.height * 2f);
                    Vector2 jumpTarget = Vector2.Lerp(Vector2.Lerp(NPC.Center, midPoint, Utils.GetLerpValue(0, jumpTime * 0.3f, localTime, true)), Vector2.Lerp(midPoint, NPC.FindSmashSpot(saveTarget), Utils.GetLerpValue(jumpTime * 0.1f, jumpTime * 0.6f, localTime, true)), Utils.GetLerpValue(0, jumpTime * 0.6f, localTime, true));
                    NPC.velocity = Vector2.Lerp(NPC.velocity, NPC.DirectionTo(jumpTarget).SafeNormalize(Vector2.Zero) * NPC.Distance(jumpTarget) * 0.3f * Utils.GetLerpValue(0, jumpTime * 0.7f, localTime, true), Utils.GetLerpValue(0, jumpTime * 0.7f, localTime, true));
                    NPC.rotation = -NPC.velocity.Y * 0.008f * Math.Sign(NPC.velocity.X);

                    float resquish = Utils.GetLerpValue(jumpTime * 0.4f, 0, localTime, true) + Utils.GetLerpValue(jumpTime * 0.4f, jumpTime * 0.7f, localTime, true);
                    squishFactor = new Vector2(1f - (float)Math.Pow(resquish, 2) * 0.5f, 1f + (float)Math.Pow(resquish, 2) * 0.5f);
                }

                if (localTime > (int)(jumpTime * 0.74f))
                {
                    NPC.frameCounter++;

                    NPC.velocity *= 0.2f;
                    NPC.rotation = 0;

                    if (localTime % 2 == 0)
                        Main.instance.CameraModifiers.Add(new PunchCameraModifier(saveTarget, Main.rand.NextVector2CircularEdge(3, 3), 5f, 10, 12));

                    squishFactor = new Vector2(1f + (float)Math.Pow(Utils.GetLerpValue(jumpTime, jumpTime * 0.74f, localTime, true), 2) * 0.6f, 1f - (float)Math.Pow(Utils.GetLerpValue(jumpTime, jumpTime * 0.74f, localTime, true), 2) * 0.5f);
                }
                if (localTime == (int)(jumpTime * 0.74f))
                {
                    foreach (Player player in Main.player.Where(n => n.active && !n.dead && n.Distance(NPC.Center) < 600))
                        player.velocity += player.DirectionFrom(NPC.Bottom + Vector2.UnitY * 10) * 3;

                    if (!Main.dedServ)
                        SoundEngine.PlaySound(SoundID.Item167, NPC.Center);
                }

            }

            if (Time > 36 + jumpCount * jumpTime)
                Reset();
        }

        private int GetDamage(int attack, float modifier = 1f)
        {
            int damage = attack switch
            {
                0 => 70,//contact
                1 => 0,// 
                2 => 0,//
                3 => 0,//
                _ => damage = 0
            };

            return (int)(damage * modifier);
        }

        public override bool PreDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {
            Asset<Texture2D> texture = ModContent.Request<Texture2D>(Texture);
            Asset<Texture2D> ninja = ModContent.Request<Texture2D>($"{nameof(CalamityHunt)}/Assets/Textures/Goozma/Crowns/DeadNinja");
            Rectangle frame = texture.Frame(1, 4, 0, npcFrame);
            Asset<Texture2D> tell = TextureAssets.Extra[178];
            Color color = Color.White;

            switch (Attack)
            {
                case (int)AttackList.SlamWave:
                    if (Time > 50 && Time < 110)
                    {
                        float tellFade = Utils.GetLerpValue(80, 105, Time, true);
                        spriteBatch.Draw(tell.Value, NPC.Bottom - new Vector2(0, NPC.height / 2f * squishFactor.Y).RotatedBy(NPC.rotation) - Main.screenPosition, null, new Color(200, 10, 20, 0) * tellFade, NPC.rotation + MathHelper.PiOver2, tell.Size() * new Vector2(0f, 0.5f), new Vector2(1f - tellFade, 110) * new Vector2(squishFactor.Y, squishFactor.X), 0, 0);
                        spriteBatch.Draw(tell.Value, NPC.Bottom - new Vector2(0, NPC.height / 2f * squishFactor.Y).RotatedBy(NPC.rotation) - Main.screenPosition, null, new Color(200, 10, 20, 0) * tellFade, NPC.rotation - MathHelper.PiOver2, tell.Size() * new Vector2(0f, 0.5f), new Vector2((1f - tellFade) * 0.2f, 110) * new Vector2(squishFactor.Y, squishFactor.X), 0, 0);
                    }
                    break;

                case (int)AttackList.CollidingCrush:
                    if (Time > 50 && Time < 130)
                    {
                        for (int i = 0; i < 7; i++)
                        {
                            Rectangle tellframe = texture.Frame(1, 4, 0, (int)((npcFrame + i * 0.5f) % 4));

                            Vector2 offset = new Vector2(90 * i * (float)Math.Pow(Utils.GetLerpValue(70, 100, Time, true), 2) * (float)Math.Sqrt(Utils.GetLerpValue(70, 100, Time, true)), 0);
                            spriteBatch.Draw(texture.Value, NPC.Bottom - offset - new Vector2(0, (float)Math.Pow(i, 2f)).RotatedBy(NPC.rotation) - Main.screenPosition, tellframe, new Color(255, 20, 20, 20) * (0.5f - i / 14f) * Utils.GetLerpValue(70, 80, Time, true), NPC.rotation, frame.Size() * new Vector2(0.5f, 1f), NPC.scale * squishFactor, 0, 0);
                            spriteBatch.Draw(texture.Value, NPC.Bottom + offset - new Vector2(0, (float)Math.Pow(i, 2f)).RotatedBy(NPC.rotation) - Main.screenPosition, tellframe, new Color(255, 20, 20, 20) * (0.5f - i / 14f) * Utils.GetLerpValue(70, 80, Time, true), NPC.rotation, frame.Size() * new Vector2(0.5f, 1f), NPC.scale * squishFactor, 0, 0);
                        }
                    }
                    break;

                case (int)AttackList.TooFar:
                    color = Color.Lerp(new Color(100, 100, 100, 0), Color.White, Math.Clamp(NPC.Distance(Target.Center), 100, 300) / 200f);
                    break;
            }

            for (int i = 0; i < NPCID.Sets.TrailCacheLength[Type]; i++)
            {
                Vector2 oldPos = NPC.oldPos[i] + NPC.Size * 0.5f;
                Color trailColor = Color.Lerp(new Color(255, 10, 10, 0), new Color(0, 0, 0, 100), (float)Math.Sqrt(i / 10f)) * Math.Clamp(NPC.velocity.Length() * 0.01f, 0, 1) * 0.5f;
                spriteBatch.Draw(texture.Value, oldPos - Main.screenPosition, frame, trailColor, NPC.rotation, frame.Size() * 0.5f, NPC.scale * squishFactor, 0, 0);
            }

            Vector2 ninjaPos = NPC.Bottom + new Vector2(0, -60 - (float)Math.Cos(npcFrame * MathHelper.PiOver2) * 4) * squishFactor;
            spriteBatch.Draw(ninja.Value, ninjaPos - NPC.oldVelocity - Main.screenPosition, null, color, NPC.rotation, ninja.Size() * 0.5f, NPC.scale * (new Vector2(0.5f) + squishFactor * 0.5f), 0, 0);
            spriteBatch.Draw(texture.Value, NPC.Bottom - Main.screenPosition, frame, color, NPC.rotation, frame.Size() * new Vector2(0.5f, 1f), NPC.scale * squishFactor, 0, 0);

            return false;
        }
    }
}
