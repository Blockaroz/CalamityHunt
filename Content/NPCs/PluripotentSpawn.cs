using System;
using System.Linq;
using CalamityHunt.Common.Systems.Particles;
using CalamityHunt.Common.Utilities;
using CalamityHunt.Common.Utilities.Interfaces;
using CalamityHunt.Content.NPCs.Bosses.GoozmaBoss;
using CalamityHunt.Content.Particles;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.Chat;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;

namespace CalamityHunt.Content.NPCs;

[AutoloadHead]
public class PluripotentSpawn : ModNPC, ISubjectOfNPC<Goozma>
{
    public override void SetStaticDefaults()
    {
        NPCID.Sets.MustAlwaysDraw[Type] = true;
        NPCID.Sets.ShouldBeCountedAsBoss[Type] = false;
        NPCID.Sets.NPCBestiaryDrawOffset.Add(Type, new NPCID.Sets.NPCBestiaryDrawModifiers() { Hide = true });
    }

    public static LocalizedText slimeMonsoonText;

    public override void SetDefaults()
    {
        NPC.width = 100;
        NPC.height = 100;
        NPC.noTileCollide = true;
        NPC.noGravity = true;
        NPC.immortal = true;
        NPC.damage = 0;
        NPC.dontTakeDamage = true;
        NPC.dontCountMe = true;
        NPC.ShowNameOnHover = false;
        NPC.lifeMax = 2028;

        slimeMonsoonText = Language.GetOrRegister(Mod.GetLocalizationKey("Chat.SlimeMonsoon"));
    }

    public override bool? DrawHealthBar(byte hbPosition, ref float scale, ref Vector2 position) => false;

    public ref float Time => ref NPC.ai[0];

    public bool Skip => true;// NPC.ai[3] == 1;

    public float size;

    public override void AI()
    {
        bool spawnBoss = false;

        if (Main.rand.NextBool(20)) {
            CalamityHunt.particles.Add(Particle.Create<LightningParticle>(particle => {
                particle.position = NPC.Center + Main.rand.NextVector2Circular(30, 30) * NPC.scale;
                particle.velocity = particle.position.DirectionFrom(NPC.Center) * Main.rand.NextFloat(0.5f, 2f);
                particle.rotation = particle.velocity.ToRotation() + Main.rand.NextFloat(-0.1f, 0.1f);
                particle.scale = Main.rand.NextFloat(0.6f, 1.5f);
                particle.color = (new GradientColor(SlimeUtils.GoozColors, 0.2f, 0.2f).ValueAt(NPC.localAI[0]) * 1.5f) with { A = 128 };
                particle.maxTime = Main.rand.Next(4, 15);
                particle.anchor = () => NPC.velocity * 0.9f;
            }));
        }

        if (Skip) {
            size = Utils.GetLerpValue(80, 130, Time, true) * 6;
            NPC.scale = 1f + MathF.Round(Utils.GetLerpValue(70, 130, Time, true) - size * 0.2f, 2) * 1.1f;

            if (Time > 400) {
                spawnBoss = true;
            }
        }
        else {
            size = Utils.GetLerpValue(250, 850, Time, true) * 6;
            NPC.scale = 1f + MathF.Round(Utils.GetLerpValue(30, 820, Time, true) - size * 0.2f, 2) * 1.1f;

            if (Time == 720) {
                if (Main.dedServ) {
                    ChatHelper.BroadcastChatMessage(NetworkText.FromKey(slimeMonsoonText.Value), new Color(50, 255, 130));
                }
                else {
                    Main.NewText(NetworkText.FromKey(slimeMonsoonText.Value), new Color(50, 255, 130));
                }
            }

            if (Time > 1060) {
                spawnBoss = true;
            }
        }

        if (spawnBoss) {
            NPC.active = false;
            NPC.SpawnBoss((int)NPC.Center.X, (int)NPC.Bottom.Y, ModContent.NPCType<Goozma>(), 0);
        }

        Time++;
        NPC.localAI[0]++;
    }

    public override void Load()
    {
        On_Main.UpdateAudio += FadeMusicOut;
        LoadFlyingSlimes();
    }

    public override void Unload()
    {
        On_Main.UpdateAudio -= FadeMusicOut;
    }

    private void FadeMusicOut(On_Main.orig_UpdateAudio orig, Main self)
    {
        orig(self);

        if (Main.npc.Any(n => n.active && n.type == ModContent.NPCType<PluripotentSpawn>())) {
            NPC goozma = Main.npc.FirstOrDefault(n => n.active && n.type == ModContent.NPCType<PluripotentSpawn>());
            for (var i = 0; i < Main.musicFade.Length; i++) {
                var volume = Main.musicFade[i] * Main.musicVolume * Utils.GetLerpValue(200, 30, goozma.localAI[0], true);
                var tempFade = Main.musicFade[i];
                Main.audioSystem.UpdateCommonTrackTowardStopping(i, volume, ref tempFade, Main.musicFade[i] > 0.1f && goozma.ai[0] < 600);
                Main.musicFade[i] = tempFade;
            }
        }
    }

    public void LoadFlyingSlimes()
    {
    }


    //public void SpawnSlimes()
    //{
    //    Vector2 position = Projectile.Center + Main.rand.NextVector2CircularEdge(1100, 1100) + Main.rand.NextVector2Circular(600, 600);
    //    Vector2 velocity = position.DirectionTo(Projectile.Center).SafeNormalize(Vector2.Zero).RotatedByRandom(3f);

    //    randomType.Add(ModContent.GetInstance<FlyingNormalSlimeParticleBehavior>(), 1f / 50f);
    //    randomType.Add(ModContent.GetInstance<FlyingBigSlimeParticleBehavior>(), 1f / 100f); // this looks bad
    //    randomType.Add(ModContent.GetInstance<FlyingBalloonSlimeParticleBehavior>(), 1f / 500f);
    //    randomType.Add(ModContent.GetInstance<FlyingGastropodParticleBehavior>(), 1f / 800f);
    //    randomType.Add(ModContent.GetInstance<FlyingIlluminantSlimeParticleBehavior>(), 1f / 800f);
    //    randomType.Add(ModContent.GetInstance<FlyingLavaSlimeParticleBehavior>(), 1f / 700f);
    //    randomType.Add(ModContent.GetInstance<FlyingZombieSlimeParticleBehavior>(), 1f / 700f);
    //    randomType.Add(ModContent.GetInstance<FlyingShimmerSlimeParticleBehavior>(), 1f / 700f);
    //    randomType.Add(ModContent.GetInstance<FlyingIceSlimeParticleBehavior>(), 1f / 700f);
    //    randomType.Add(ModContent.GetInstance<FlyingSandSlimeParticleBehavior>(), 1f / 700f);
    //    randomType.Add(ModContent.GetInstance<FlyingJungleSlimeSpikedParticleBehavior>(), 1f / 700f);
    //    randomType.Add(ModContent.GetInstance<FlyingSpikedSlimeParticleBehavior>(), 1f / 700f);
    //    randomType.Add(ModContent.GetInstance<FlyingBouncySlimeParticleBehavior>(), 1f / 800f);
    //    randomType.Add(ModContent.GetInstance<FlyingCrystalSlimeParticleBehavior>(), 1f / 800f);
    //    randomType.Add(ModContent.GetInstance<FlyingHeavenlySlimeParticleBehavior>(), 1f / 800f);
    //    randomType.Add(ModContent.GetInstance<FlyingUmbrellaSlimeParticleBehavior>(), 1f / 1200f);
    //    randomType.Add(ModContent.GetInstance<FlyingCorruptSlimeParticleBehavior>(), 1f / 500f);
    //    randomType.Add(ModContent.GetInstance<FlyingSlimerParticleBehavior>(), 1f / 1000f);
    //    randomType.Add(ModContent.GetInstance<FlyingCrimslimeParticleBehavior>(), 1f / 500f);
    //    randomType.Add(ModContent.GetInstance<FlyingToxicSludgeParticleBehavior>(), 1f / 1000f);
    //    randomType.Add(ModContent.GetInstance<FlyingDungeonSlimeParticleBehavior>(), 1f / 1500f);
    //    randomType.Add(ModContent.GetInstance<FlyingHoppinJackParticleBehavior>(), Main.halloween ? (1f / 150f) : (1f / 2000f));
    //    randomType.Add(ModContent.GetInstance<FlyingSlimeFishParticleBehavior>(), 1f / 1000f);
    //    randomType.Add(ModContent.GetInstance<FlyingSlimeStatueParticleBehavior>(), 1f / 3000f);
    //    randomType.Add(ModContent.GetInstance<FlyingFirstEncounterParticleBehavior>(), 1f / 3000f);
    //    randomType.Add(ModContent.GetInstance<FlyingGoldSlimeParticleBehavior>(), 1f / 5000f);
    //    randomType.Add(ModContent.GetInstance<FlyingYuHParticleBehavior>(), 1f / 10000f);

    //    if (Main.halloween)
    //    {
    //        randomType.Add(ModContent.GetInstance<FlyingSlimeBunnyParticleBehavior>(), 1f / 150f);
    //        randomType.Add(ModContent.GetInstance<FlyingBunnySlimeParticleBehavior>(), 1f / 150f);
    //    }
    //    if (Main.xMas)
    //        randomType.Add(ModContent.GetInstance<FlyingPresentSlimeParticleBehavior>(), 1f / 150f);

    //    if (Main.zenithWorld)
    //    {
    //        randomType.Add(ModContent.GetInstance<FlyingYumeSlimeParticleBehavior>(), 1f / 15000f);
    //        randomType.Add(ModContent.GetInstance<FlyingCoreSlimeParticleBehavior>(), 1f / 15000f);
    //        randomType.Add(ModContent.GetInstance<FlyingDragonSlimeParticleBehavior>(), 1f / 15000f);
    //        randomType.Add(ModContent.GetInstance<FlyingFatPixieParticleBehavior>(), 1f / 5000f);
    //        randomType.Add(ModContent.GetInstance<FlyingMadnessSlimeParticleBehavior>(), 1f / 5000f);
    //        randomType.Add(ModContent.GetInstance<FlyingMireSlimeParticleBehavior>(), 1f / 5000f);
    //        randomType.Add(ModContent.GetInstance<FlyingInfernoSlimeParticleBehavior>(), 1f / 5000f);
    //        randomType.Add(ModContent.GetInstance<FlyingOilSlimeParticleBehavior>(), 1f / 5000f);
    //        randomType.Add(ModContent.GetInstance<FlyingWhiteSlimeParticleBehavior>(), 1f / 5000f);
    //    }

    //    if (ModLoader.HasMod(HuntOfTheOldGodsUtils.CalamityModName))
    //    {
    //        randomType.Add(ModContent.GetInstance<FlyingAeroSlimeParticleBehavior>(), 1f / 800f);
    //        randomType.Add(ModContent.GetInstance<FlyingEbonianBlightSlimeParticleBehavior>(), 1f / 1500f);
    //        randomType.Add(ModContent.GetInstance<FlyingCrimulanBlightSlimeParticleBehavior>(), 1f / 1500f);
    //        randomType.Add(ModContent.GetInstance<FlyingCorruptSlimeSpawnParticleBehavior>(), 1f / 700f);
    //        randomType.Add(ModContent.GetInstance<FlyingCrimsonSlimeSpawnParticleBehavior>(), 1f / 700f);
    //        randomType.Add(ModContent.GetInstance<FlyingAstralSlimeParticleBehavior>(), 1f / 1000f);
    //        randomType.Add(ModContent.GetInstance<FlyingCryoSlimeParticleBehavior>(), 1f / 1000f);
    //        randomType.Add(ModContent.GetInstance<FlyingIrradiatedSlimeParticleBehavior>(), 1f / 800f);
    //        randomType.Add(ModContent.GetInstance<FlyingCharredSlimeParticleBehavior>(), 1f / 1000f);
    //        randomType.Add(ModContent.GetInstance<FlyingPerennialSlimeParticleBehavior>(), 1f / 1000f);
    //        randomType.Add(ModContent.GetInstance<FlyingAureusSpawnSlimeParticleBehavior>(), 1f / 3000f);
    //        randomType.Add(ModContent.GetInstance<FlyingPestilentSlimeParticleBehavior>(), 1f / 800f);
    //        randomType.Add(ModContent.GetInstance<FlyingBloomSlimeParticleBehavior>(), 1f / 1000f);
    //        randomType.Add(ModContent.GetInstance<FlyingGammaSlimeParticleBehavior>(), 1f / 800f);
    //        randomType.Add(ModContent.GetInstance<FlyingCragmawMireParticleBehavior>(), 1f / 5000f);
    //    }
    //    if (ModLoader.HasMod(HuntOfTheOldGodsUtils.CatalystModName))
    //    {
    //        randomType.Add(ModContent.GetInstance<FlyingWulfrumSlimeParticleBehavior>(), 1f / 800f);
    //        randomType.Add(ModContent.GetInstance<FlyingAscendedAstralSlimeParticleBehavior>(), 1f / 1500f);
    //        //if (!NPC.downedMoonlord || (bool)ModLoader.GetMod(HuntOfTheOldGodsUtils.CatalystModName).Call("worlddefeats.astrageldon"))
    //        {
    //            randomType.Add(ModContent.GetInstance<FlyingNovaSlimeParticleBehavior>(), 1f / 700f);
    //            randomType.Add(ModContent.GetInstance<FlyingNovaSlimerParticleBehavior>(), 1f / 700f);
    //            randomType.Add(ModContent.GetInstance<FlyingMetanovaSlimeParticleBehavior>(), 1f / 1000f);
    //        }
    //    }

    //    float scale = 1f;
    //    Color color = Color.White;

    //    var particleBehavior = ParticleBehavior.NewParticle(randomType, position, velocity, color, scale);
    //    particleBehavior.Add(new ParticleData<Vector2> { Value = Projectile.Center }, new ParticleDrawBehindEntities());
    //}

    public override bool PreDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
    {
        if (NPC.IsABestiaryIconDummy) {
            return false;
        }

        Texture2D texture = TextureAssets.Npc[Type].Value;
        Texture2D eye = AssetDirectory.Textures.Goozma.GodEye.Value;
        Texture2D glow = AssetDirectory.Textures.Glow[0].Value;

        Color glowColor = new GradientColor(SlimeUtils.GoozColors, 0.2f, 0.2f).ValueAt(Time * 0.33f) * 1.2f;
        glowColor.A = 0;
        Vector2 drawOffset = new Vector2(14, 20).RotatedBy(NPC.rotation) * NPC.scale;

        Goozma.GetGradientMapValues(out var brightnesses, out var colors);
        Effect effect = AssetDirectory.Effects.HolographicGel.Value;
        effect.Parameters["uTime"].SetValue(Main.GlobalTimeWrappedHourly % 1f);
        effect.Parameters["colors"].SetValue(colors);
        effect.Parameters["brightnesses"].SetValue(brightnesses);
        effect.Parameters["baseToScreenPercent"].SetValue(1f);
        effect.Parameters["baseToMapPercent"].SetValue(0f);

        Vector2 drawPos = NPC.Center;
        Rectangle frame = texture.Frame(7, 1, (int)size, 0);

        spriteBatch.End();
        spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, Main.DefaultSamplerState, DepthStencilState.None, Main.Rasterizer, null, Main.Transform);

        Main.pixelShader.CurrentTechnique.Passes["ColorOnly"].Apply();

        for (var i = 0; i < 4; i++) {
            var off = new Vector2(2, 0).RotatedBy(MathHelper.TwoPi / 4f * i);
            Main.EntitySpriteDraw(texture, drawPos + off - screenPos, frame, glowColor, 0, frame.Size() * 0.5f, NPC.scale, 0, 0);
        }

        effect.CurrentTechnique.Passes[0].Apply();

        Main.EntitySpriteDraw(texture, drawPos - screenPos, frame, Color.White, 0, frame.Size() * 0.5f, NPC.scale, 0, 0);

        Main.pixelShader.CurrentTechnique.Passes[0].Apply();

        spriteBatch.End();
        spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, Main.DefaultSamplerState, DepthStencilState.None, Main.Rasterizer, null, Main.Transform);

        Main.EntitySpriteDraw(glow, drawPos - screenPos, glow.Frame(), Color.Lerp(Color.Transparent, glowColor * 0.4f, Utils.GetLerpValue(150, 450, Time, true)), 0, glow.Size() * 0.5f, 0.3f + NPC.scale + size * 0.5f, 0, 0);

        return false;
    }
}
