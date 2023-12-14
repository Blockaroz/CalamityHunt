using System;
using System.Linq;
using CalamityHunt.Common.Utilities;
using CalamityHunt.Common.Utilities.Interfaces;
using CalamityHunt.Content.NPCs.Bosses.GoozmaBoss;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.Chat;
using Terraria.GameContent;
using Terraria.GameContent.Bestiary;
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
        NPCID.Sets.ShouldBeCountedAsBoss[Type] = true;
        NPCID.Sets.NPCBestiaryDrawOffset.Add(Type, new NPCID.Sets.NPCBestiaryDrawModifiers() { Hide = true });
    }

    public static LocalizedText slimeMonsoonText;

    public override void SetDefaults()
    {
        NPC.width = 92;
        NPC.height = 88;
        NPC.noTileCollide = true;
        NPC.noGravity = true;
        NPC.immortal = true;
        NPC.damage = 0;
        NPC.dontTakeDamage = true;

        NPC.dontCountMe = true;
        NPC.ShowNameOnHover = false;

        slimeMonsoonText = Language.GetOrRegister(Mod.GetLocalizationKey("Chat.SlimeMonsoon"));
    }

    public override bool? DrawHealthBar(byte hbPosition, ref float scale, ref Vector2 position) => false;

    public ref float Time => ref NPC.ai[0];

    public bool Skip => true;// NPC.ai[3] == 1;

    public override void AI()
    {
        bool spawnBoss = false;

        if (Skip) {
            if (Time > 400) {
                spawnBoss = true;
            }
        }
        else {


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
            NPC.SpawnBoss((int)NPC.Center.X, (int)NPC.Bottom.Y, ModContent.NPCType<Goozma>(), 0);

            NPC.active = false;
        }

        Time++;
        NPC.localAI[0]++;
    }

    public override void Load()
    {
        On_Main.UpdateAudio += FadeMusicOut;
    }

    public override void Unload()
    {
        On_Main.UpdateAudio -= FadeMusicOut;
    }

    private void FadeMusicOut(On_Main.orig_UpdateAudio orig, Main self)
    {
        orig(self);

        if (Main.npc.Any(n => n.active && n.type == ModContent.NPCType<PluripotentSpawn>())) {
            var goozma = Main.projectile.FirstOrDefault(n => n.active && n.type == ModContent.NPCType<PluripotentSpawn>());
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

    public struct FlyingSlimeData
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
        Texture2D texture = TextureAssets.Npc[Type].Value;
        Texture2D eye = AssetDirectory.Textures.Goozma.GodEye.Value;
        Texture2D glow = AssetDirectory.Textures.Glow[0].Value;

        var glowColor = new GradientColor(SlimeUtils.GoozColors, 0.2f, 0.2f).ValueAt(Time * 0.33f) * 1.2f;
        glowColor.A = 0;
        var drawOffset = new Vector2(14, 20).RotatedBy(NPC.rotation) * NPC.scale;

        var size = (int)(Utils.GetLerpValue(250, 850, Time, true) * 6);
        NPC.scale = Utils.GetLerpValue(0, 120, Time, true) + (MathF.Round(Utils.GetLerpValue(30, 820, Time, true), 2) - size * 0.12f) * 1.33f;

        var fastWobble = 0.6f + (float)Math.Sin(Time * 0.7f) * 0.4f;

        GetGradientMapValues(out var brightnesses, out var colors);
        var effect = AssetDirectory.Effects.HolographicGel.Value;
        effect.Parameters["uTime"].SetValue(Main.GlobalTimeWrappedHourly % 1f);
        effect.Parameters["colors"].SetValue(colors);
        effect.Parameters["brightnesses"].SetValue(brightnesses);
        effect.Parameters["baseToScreenPercent"].SetValue(1f);
        effect.Parameters["baseToMapPercent"].SetValue(0f);

        var drawPos = NPC.Center + Main.rand.NextVector2Circular(2, 2);
        var frame = texture.Frame(7, 1, size, 0);

        Main.EntitySpriteDraw(texture, drawPos - Main.screenPosition, frame, Color.Black * 0.2f, 0, frame.Size() * 0.5f, NPC.scale + fastWobble * 0.4f, 0, 0);
        Main.EntitySpriteDraw(texture, drawPos - Main.screenPosition, frame, Color.Black * 0.1f, 0, frame.Size() * 0.5f, NPC.scale + fastWobble, 0, 0);

        for (var i = 0; i < 6; i++) {
            var off = new Vector2(2, 0).RotatedBy(Time * 0.2f + MathHelper.TwoPi / 6f * i);
            Main.EntitySpriteDraw(texture, drawPos + off - Main.screenPosition, frame, Color.Lerp(Color.Transparent, glowColor, Utils.GetLerpValue(150, 450, Time, true)) * NPC.scale, 0, frame.Size() * 0.5f, NPC.scale * 0.9f, 0, 0);
        }

        FlipShadersOnOff(Main.spriteBatch, effect, false);
        Main.EntitySpriteDraw(texture, drawPos - Main.screenPosition, frame, Color.White, 0, frame.Size() * 0.5f, NPC.scale * 0.9f, 0, 0);
        FlipShadersOnOff(Main.spriteBatch, null, false);

        Main.EntitySpriteDraw(glow, drawPos - Main.screenPosition, glow.Frame(), Color.Lerp(Color.Transparent, glowColor * 0.2f, Utils.GetLerpValue(150, 450, Time, true)), 0, glow.Size() * 0.5f, 0.3f + NPC.scale + size, 0, 0);

        Vector2 eyePos = NPC.Center + drawOffset + new Vector2(-42, -37).RotatedBy(NPC.rotation) * NPC.scale;
        float eyeScale = (float)Math.Sqrt(Utils.GetLerpValue(940, 950, Time, true)) * 3f;
        float eyeRot = (float)Math.Cbrt(Utils.GetLerpValue(940, 1080, Time, true)) * MathHelper.PiOver2 - MathHelper.PiOver4;
        Main.EntitySpriteDraw(eye, eyePos - Main.screenPosition, eye.Frame(), glowColor, eyeRot, eye.Size() * 0.5f, eyeScale * 0.4f, 0, 0);
        Main.EntitySpriteDraw(eye, eyePos - Main.screenPosition, eye.Frame(), new Color(255, 255, 255, 0), eyeRot, eye.Size() * 0.5f, eyeScale * 0.4f, 0, 0);

        return false;
    }

    public void FlipShadersOnOff(SpriteBatch spriteBatch, Effect effect, bool immediate)
    {
        spriteBatch.End();
        var sortMode = SpriteSortMode.Deferred;
        if (immediate)
            sortMode = SpriteSortMode.Immediate;
        spriteBatch.Begin(sortMode, BlendState.AlphaBlend, Main.DefaultSamplerState, DepthStencilState.None, Main.Rasterizer, effect, Main.Transform);
    }

    public void GetGradientMapValues(out float[] brightnesses, out Vector3[] colors)
    {
        var maxBright = 0.667f;
        brightnesses = new float[10];
        colors = new Vector3[10];

        var rainbowStartOffset = 0.35f + NPC.ai[0] * 0.016f % (maxBright * 2f);
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

}
