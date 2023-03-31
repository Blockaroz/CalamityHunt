using CalamityHunt.Content.Bosses.Goozma.Projectiles;
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
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;

namespace CalamityHunt.Content.Bosses.Goozma.Slimes
{
    public class DivineGargooptuar : ModNPC
    {
        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Divine Gargooptuar"); 
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

            NPCID.Sets.NPCBestiaryDrawModifiers drawModifiers = new NPCID.Sets.NPCBestiaryDrawModifiers(0) { };
            NPCID.Sets.NPCBestiaryDrawOffset.Add(Type, drawModifiers);
        }

        public override void SetBestiary(BestiaryDatabase database, BestiaryEntry bestiaryEntry)
        {
            bestiaryEntry.Info.AddRange(new List<IBestiaryInfoElement> {
                new MoonLordPortraitBackgroundProviderBestiaryInfoElement(),
                new FlavorTextBestiaryInfoElement("Mods.CalamityHunt.Bestiary.DivineGargooptuar"),
                BestiaryDatabaseNPCsPopulator.CommonTags.SpawnConditions.Events.SlimeRain,
                BestiaryDatabaseNPCsPopulator.CommonTags.SpawnConditions.Biomes.TheHallow,
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
            PrismDestroyer,
            CrystalStorm,
            PixieBall,
            BlowUp,
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
        public int wingFrame;

        public override void AI()
        {
            if (NPC.ai[2] < 0)
                NPC.ai[2] = Main.npc.First(n => n.type == ModContent.NPCType<Goozma>() && n.active).whoAmI;
            if (!Main.npc.Any(n => n.type == ModContent.NPCType<Goozma>() && n.active))
                NPC.active = false;

            NPC.frameCounter++;
            if (NPC.frameCounter % 7 == 0)
                npcFrame = (npcFrame + 1) % 4;

            if (NPC.frameCounter % 8 == 0 && Time >= 0)
                wingFrame = (wingFrame + 1) % 4;

            if (NPC.frameCounter > 56)
                NPC.frameCounter = 0;

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

        private void PrismDestroyer()
        {
            int danceCount = 3;
            int prismCount = 3;
            if (Main.expertMode)
                prismCount = 5;

            NPC.rotation = NPC.velocity.X * 0.02f;

            if (Time < danceCount * 150)
            {
                if (Time % 150 == 0 && Time <= danceCount * 150)
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

                    if (!Main.dedServ)
                    {
                        SoundEngine.PlaySound(SoundID.Item147, NPC.Center);
                        SoundEngine.PlaySound(SoundID.QueenSlime, NPC.Center);
                    }
                }

                foreach (Projectile proj in Main.projectile.Where(n => n.active && n.type == ModContent.ProjectileType<PrismDestroyer>() && n.ai[0] < 2))
                    proj.ai[2] += (NPC.velocity.X > 0 ? 1 : -1) * 0.07f * Utils.GetLerpValue(80, 20, Time % 150, true);

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

            if (Time > danceCount * 150 + 10)
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
                squishFactor = Vector2.Lerp(Vector2.Lerp(squishFactor, new Vector2(0.5f, 1.6f), Utils.GetLerpValue(40, lengthOfAttack, Time, true) * 0.2f), Vector2.Lerp(squishFactor, Vector2.One, 0.1f), Utils.GetLerpValue(40 + lengthOfAttack, 110 + lengthOfAttack, Time, true)); 
                NPC.velocity = Vector2.Lerp(NPC.velocity, NPC.DirectionTo(Target.Center + new Vector2(0, -170)).SafeNormalize(Vector2.Zero) * NPC.Distance(Target.Center + new Vector2(0, -170)) * 0.2f, 0.2f + (float)Math.Sin(Time * 0.3f) * 0.1f);

                if (Time <= lengthOfAttack + 100 && Time % 15 == 0)
                    for (int i = 0; i < crystalFrequency; i++)
                        Projectile.NewProjectile(NPC.GetSource_FromAI(), NPC.Top, Main.rand.NextVector2Circular(8, 5) - Vector2.UnitY * 10 + NPC.DirectionTo(Target.Center).SafeNormalize(Vector2.Zero) * 5f, ModContent.ProjectileType<GelCrystalShard>(), GetDamage(2), 0);
            }

            if (Time > lengthOfAttack + 110)
                Reset();
        }        

        private void PixieBall()
        {
            NPC.rotation = NPC.velocity.X * 0.022f;

            if (Time < 120)
            {
                if (Time == 50)
                {
                    NPC.velocity = Vector2.UnitY * 5f;
                    Projectile.NewProjectileDirect(NPC.GetSource_FromAI(), NPC.Center, Vector2.UnitY * -30f, ModContent.ProjectileType<PixieBall>(), 0, 0, ai1: 15);

                    //if (!Main.dedServ)
                    //{
                    //    SoundEngine.PlaySound(SoundID.Item147, NPC.Center);
                    //}
                }

                NPC.velocity *= 0.9f;

                float progress = MathHelper.SmoothStep(0, 1, Utils.GetLerpValue(0, 60, Time, true) * Utils.GetLerpValue(120, 80, Time, true));
                squishFactor = Vector2.Lerp(squishFactor, new Vector2(1f + (float)Math.Cos(progress * MathHelper.Pi) * 0.2f, 1f + (float)Math.Cos(progress * MathHelper.Pi + MathHelper.Pi) * 0.4f), 0.3f);
            }
            else if (Time < 500)
            {
                squishFactor = Vector2.Lerp(squishFactor, Vector2.One, 0.1f);
                NPC.velocity = Vector2.Lerp(NPC.velocity, NPC.DirectionTo(Target.Center).SafeNormalize(Vector2.Zero) * NPC.Distance(Target.Center) * 0.01f, 0.1f);

                if (Time % 80 == 0)
                    for (int i = 0; i < Main.rand.Next(10, 30); i++)
                        Projectile.NewProjectile(NPC.GetSource_FromAI(), NPC.Center, Main.rand.NextVector2Circular(10, 10) - Vector2.UnitY * 10, ModContent.ProjectileType<GelCrystalShard>(), GetDamage(3), 0);

            }
            else
                NPC.velocity *= 0.9f;

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

            if (Time == 2)
            {

            }
            if (Time > 30)
            {
                foreach (Player player in Main.player.Where(n => n.active && !n.dead && n.Distance(NPC.Center) < 8000))
                {
                    player.Hurt(PlayerDeathReason.ByCustomReason("{0} saw the light."), 9999, -1, false, true, 200, false, 0, 0, 0);
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

        public override bool PreDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {
            Asset<Texture2D> texture = ModContent.Request<Texture2D>(Texture);
            Asset<Texture2D> trailTexture = ModContent.Request<Texture2D>(Texture + "Trail");
            Asset<Texture2D> core = ModContent.Request<Texture2D>($"{nameof(CalamityHunt)}/Assets/Textures/Goozma/Crowns/CrystalMine");
            Asset<Texture2D> wings = TextureAssets.Extra[185];
            Rectangle frame = texture.Frame(1, 4, 0, npcFrame);
            Rectangle wingRect = wings.Frame(1, 4, 0, wingFrame);

            Color color = Color.White;

            switch (Attack)
            {
                case (int)AttackList.TooFar:
                    color = Color.Lerp(new Color(100, 100, 100, 0), Color.White, Math.Clamp(NPC.Distance(Target.Center), 100, 300) / 200f);
                    break;
            }

            for (int i = 0; i < NPCID.Sets.TrailCacheLength[Type]; i++)
            {
                Vector2 oldPos = NPC.oldPos[i] + NPC.Size * new Vector2(0.5f, 1f);
                Color trailColor = Main.hslToRgb((NPC.localAI[0] * 0.03f - ((float)i / NPCID.Sets.TrailCacheLength[Type])) % 1f, 1f, 0.6f, 0) * Math.Clamp(NPC.velocity.Length() * 0.01f, 0, 1) * (1f - (float)i / NPCID.Sets.TrailCacheLength[Type]) * 0.6f;
                spriteBatch.Draw(trailTexture.Value, oldPos - Main.screenPosition, frame, trailColor.MultiplyRGBA(color), NPC.rotation, frame.Size() * new Vector2(0.5f, 1f), NPC.scale * squishFactor, 0, 0);
            }

            Color rainbowColor = Main.hslToRgb((NPC.localAI[0] * 0.03f) % 1f, 1f, 0.7f, 0) * 0.5f;
            Vector2 corePos = NPC.Bottom + new Vector2(0, -45 - (float)Math.Cos(npcFrame * MathHelper.PiOver2) * 3) * squishFactor;
            Vector2 leftWingPos = corePos + new Vector2(-10, -4).RotatedBy(NPC.rotation) * NPC.scale;
            Vector2 rightWingPos = corePos + new Vector2(10, -4).RotatedBy(NPC.rotation) * NPC.scale;
            
            spriteBatch.Draw(wings.Value, leftWingPos - Main.screenPosition, wingRect, color, NPC.rotation * 0.5f, wingRect.Size() * new Vector2(1f, 0.5f), NPC.scale, 0, 0);
            spriteBatch.Draw(wings.Value, rightWingPos - Main.screenPosition, wingRect, color, NPC.rotation * 0.5f, wingRect.Size() * new Vector2(0f, 0.5f), NPC.scale, SpriteEffects.FlipHorizontally, 0);
            
            spriteBatch.Draw(wings.Value, leftWingPos - Main.screenPosition, wingRect, color.MultiplyRGBA(rainbowColor), NPC.rotation * 0.5f, wingRect.Size() * new Vector2(1f, 0.5f), NPC.scale * 1.05f, 0, 0);
            spriteBatch.Draw(wings.Value, rightWingPos - Main.screenPosition, wingRect, color.MultiplyRGBA(rainbowColor), NPC.rotation * 0.5f, wingRect.Size() * new Vector2(0f, 0.5f), NPC.scale * 1.05f, SpriteEffects.FlipHorizontally, 0);

            spriteBatch.Draw(core.Value, corePos - Main.screenPosition, null, color, NPC.rotation + (float)Math.Sin(NPC.localAI[0] * 0.1f % MathHelper.TwoPi) * 0.1f, core.Size() * 0.5f, NPC.scale * (new Vector2(0.5f) + squishFactor * 0.5f), 0, 0);
            spriteBatch.Draw(core.Value, corePos + Main.rand.NextVector2Circular(5, 5) - Main.screenPosition, null, color.MultiplyRGBA(new Color(120, 50, 120, 0)), NPC.rotation + (float)Math.Sin(NPC.localAI[0] * 0.1f % MathHelper.TwoPi) * 0.1f, core.Size() * 0.5f, NPC.scale * (new Vector2(0.5f) + squishFactor * 0.5f), 0, 0);
            spriteBatch.Draw(core.Value, corePos + Main.rand.NextVector2Circular(10, 10) - Main.screenPosition, null, color.MultiplyRGBA(new Color(40, 0, 50, 0)), NPC.rotation + (float)Math.Sin(NPC.localAI[0] * 0.1f % MathHelper.TwoPi) * 0.1f, core.Size() * 0.5f, NPC.scale * (new Vector2(0.5f) + squishFactor * 0.5f), 0, 0);

            spriteBatch.Draw(texture.Value, NPC.Bottom - Main.screenPosition, frame, color, NPC.rotation, frame.Size() * new Vector2(0.5f, 1f), NPC.scale * squishFactor, 0, 0);

            return false;
        }
    }
}
