using System;
using System.Collections.Generic;
using System.Linq;
using CalamityHunt.Common.Systems.FlyingSlimes;
using CalamityHunt.Common.Systems.Particles;
using CalamityHunt.Common.Utilities;
using CalamityHunt.Common.Utilities.Interfaces;
using CalamityHunt.Content.NPCs.Bosses.GoozmaBoss;
using CalamityHunt.Content.Particles;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.Audio;
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

    public bool Skip => NPC.ai[1] == 1 || NPC.ai[3] == 1;

    public float size;

    public override void AI()
    {
        bool spawnBoss = false;

        if (Skip) {
            size = (int)(Utils.GetLerpValue(20, 105, Time, true) * 6f);
            NPC.scale = 1f + MathF.Round(Utils.GetLerpValue(20, 105, Time, true), 2);

            if (Time == 105) {
                if (Main.dedServ) {
                    ChatHelper.BroadcastChatMessage(NetworkText.FromKey(slimeMonsoonText.Value), new Color(50, 255, 130));
                }
                else {
                    Main.NewText(NetworkText.FromKey(slimeMonsoonText.Value), new Color(50, 255, 130));
                }
            }

            if (Time > 200) {
                Time = 0;
                //spawnBoss = true;
            }
        }
        else {
            size = (int)(Utils.GetLerpValue(30, 1000, Time, true) * 6f);
            NPC.scale = 1f + MathF.Round(Utils.GetLerpValue(30, 1000, Time, true), 2);

            //if (Time > 1000) {
            //    Time = 104;
            //    NPC.ai[1] = 1;
            //    slimes.Clear();
            //}

            if (Time > 900) {
                Time = 10;
            }

            slimes ??= new HashSet<FlyingSlime>();

            foreach (FlyingSlime slime in slimes.ToHashSet()) {
                slime.Update(3f);

                if (slime.ShouldRemove) {

                    if (Main.rand.NextBool(3 + (int)(Time * 0.001f))) {
                        CalamityHunt.particles.Add(Particle.Create<ChromaticGooBurst>(particle => {
                            particle.position = slime.currentPosition;
                            particle.velocity = (slime.rotation + MathHelper.PiOver2).ToRotationVector2();
                            particle.scale = Main.rand.NextFloat(0.4f, 1.1f) * slime.scale;
                            particle.color = Color.White;
                            particle.colorData = new ColorOffsetData(true, NPC.localAI[0] * 0.33f);
                        }));
                    }

                    SoundEngine.PlaySound(AssetDirectory.Sounds.Goozma.SlimeAbsorb, slime.currentPosition);

                    slimes.Remove(slime);
                }
            }

            if (Time == 200) {
                SoundEngine.PlaySound(AssetDirectory.Sounds.Goozma.Intro.WithVolumeScale(2f).WithPitchOffset(-0.95f), NPC.Center);
            }

            for (int i = 0; i < 50; i++) {
                if (Time < 830 && Main.rand.NextBool((int)Math.Max(1, 150 - Time * 0.15f))) {
                    FlyingSlime slime = FlyingSlime.CreateRandom();
                    Vector2 slimeOffset = Main.rand.NextVector2CircularEdge(1000, 1000) + Main.rand.NextVector2Circular(400, 400);
                    slime.startPosition = NPC.Center + slimeOffset;
                    slime.targetPosition = NPC.Center + slimeOffset.SafeNormalize(Vector2.Zero) * 10f * NPC.scale;
                    slimes?.Add(slime);
                }
            }
        }

        if (spawnBoss) {
            NPC.active = false;
            NPC.SpawnBoss((int)NPC.Center.X, (int)NPC.Bottom.Y, ModContent.NPCType<Goozma>(), 0);
        }

        Time++;

        if (Main.rand.NextBool(3)) {
            CalamityHunt.particles.Add(Particle.Create<LightningParticle>(particle => {
                particle.position = NPC.Center + Main.rand.NextVector2Circular(30, 30) * NPC.scale;
                particle.velocity = particle.position.DirectionFrom(NPC.Center) * Main.rand.NextFloat(0.5f, 3f);
                particle.rotation = particle.velocity.ToRotation() + Main.rand.NextFloat(-0.1f, 0.1f);
                particle.scale = Main.rand.NextFloat(0.5f, 1.5f);
                particle.color = (new GradientColor(SlimeUtils.GoozColors, 0.2f, 0.2f).ValueAt(NPC.localAI[0] * 0.33f) * 1.1f) with { A = 128 };
                particle.maxTime = Main.rand.Next(4, 10);
                particle.anchor = () => NPC.velocity * 0.9f;
            }));
        }

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
            NPC goozma = Main.npc.FirstOrDefault(n => n.active && n.type == ModContent.NPCType<PluripotentSpawn>());
            for (int i = 0; i < Main.musicFade.Length; i++) {
                float volume = Main.musicFade[i] * Main.musicVolume * Utils.GetLerpValue(200, 30, goozma.localAI[0], true);
                float tempFade = Main.musicFade[i];
                Main.audioSystem.UpdateCommonTrackTowardStopping(i, volume, ref tempFade, Main.musicFade[i] > 0.1f && goozma.ai[0] < 600);
                Main.musicFade[i] = tempFade;
            }
        }
    }

    private HashSet<FlyingSlime> slimes;

    public override bool PreDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
    {
        if (NPC.IsABestiaryIconDummy) {
            return false;
        }

        Texture2D texture = TextureAssets.Npc[Type].Value;
        Texture2D eye = AssetDirectory.Textures.Goozma.GodEye.Value;
        Texture2D glow = AssetDirectory.Textures.Glow[1].Value;

        Color glowColor = new GradientColor(SlimeUtils.GoozColors, 0.2f, 0.2f).ValueAt(NPC.localAI[0] * 0.33f) * 1.2f;
        glowColor.A = 0;
        Vector2 drawOffset = new Vector2(14, 20).RotatedBy(NPC.rotation) * NPC.scale;

        spriteBatch.End();
        spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, Main.DefaultSamplerState, DepthStencilState.None, Main.Rasterizer, null, Main.Transform);

        if (slimes != null) {
            foreach (FlyingSlime slime in slimes.ToHashSet()) {
                slime.Draw(spriteBatch, screenPos);
            }
        }

        Goozma.GetGradientMapValues(out float[] brightnesses, out Vector3[] colors);
        Effect effect = AssetDirectory.Effects.HolographicGel.Value;
        effect.Parameters["uTime"].SetValue(Main.GlobalTimeWrappedHourly % 1f);
        effect.Parameters["colors"].SetValue(colors);
        effect.Parameters["brightnesses"].SetValue(brightnesses);
        effect.Parameters["baseToScreenPercent"].SetValue(1f);
        effect.Parameters["baseToMapPercent"].SetValue(0f);

        Vector2 drawPos = NPC.Center;
        Rectangle frame = texture.Frame(7, 1, (int)size, 0);
        float sizeScale = size / 6f;

        Main.EntitySpriteDraw(glow, drawPos - screenPos, glow.Frame(), glowColor * 0.1f, 0, glow.Size() * 0.5f, NPC.scale * 0.5f, 0, 0);

        Main.pixelShader.CurrentTechnique.Passes["ColorOnly"].Apply();

        for (int i = 0; i < 4; i++) {
            Vector2 off = new Vector2(2, 0).RotatedBy(MathHelper.TwoPi / 4f * i);
            Main.EntitySpriteDraw(texture, drawPos + off - screenPos, frame, glowColor, 0, frame.Size() * 0.5f, NPC.scale - sizeScale, 0, 0);
        }

        effect.CurrentTechnique.Passes[0].Apply();

        Main.EntitySpriteDraw(texture, drawPos - screenPos, frame, Color.White, 0, frame.Size() * 0.5f, NPC.scale - sizeScale, 0, 0);

        Main.pixelShader.CurrentTechnique.Passes[0].Apply();

        spriteBatch.End();
        spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, Main.DefaultSamplerState, DepthStencilState.None, Main.Rasterizer, null, Main.Transform);

        if (Main.xMas) {
            Texture2D hat = AssetDirectory.Textures.SantaHat.Value;
            //Main.EntitySpriteDraw(hat, drawPos - screenPos + new Vector2(0, 15 - 25 * NPC.scale), hat.Frame(), Color.White, 0.7f, hat.Size() * new Vector2(0.7f, 0.5f), NPC.scale * 0.5f - 0.2f, 0, 0);
        }

        return false;
    }
}
