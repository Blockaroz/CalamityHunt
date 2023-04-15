using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using ReLogic.Content;
using System;
using System.Collections.Generic;
using Terraria;
using Terraria.ModLoader;
using Terraria.GameContent;
using Terraria.Graphics;
using Terraria.ID;
using System.Linq;

namespace CalamityHunt.Content.Bosses.Goozma
{
    public partial class Goozma : ModNPC
    {
        public RenderTarget2D cordTarget;

        public void DrawCord(SpriteBatch spriteBatch)
        { 
            if (cordTarget == null)
                return;

            spriteBatch.Draw(cordTarget, Main.LocalPlayer.Center - Main.screenPosition, null, Color.White, NPC.rotation, new Vector2(512), NPC.scale, 0, 0);
        }

        public override void Unload()
        {
            Main.QueueMainThreadAction(() =>
            {
                foreach (NPC npc in Main.npc.Where(n => n.ModNPC is Goozma))
                {
                    Goozma targetOwner = (Goozma)npc.ModNPC;
                    targetOwner.cordTarget.Dispose();
                }
            });
        }

        private Color ColorFunc(float progress) => Color.White;
        private float WidthFunc(float progress) => Utils.GetLerpValue(1.1f, 0.1f, progress, true) * 40f;


        private void TestDraws(On_Main.orig_DrawNPCs orig, Main self, bool behindTiles)
        {
            orig(self, behindTiles);
            foreach (NPC npc in Main.npc.Where(n => n.active && n.ModNPC is Goozma))
            {
                Goozma targetOwner = (Goozma)npc.ModNPC;
                if (targetOwner.cordTarget == null)
                    return;
                Main.spriteBatch.Draw(targetOwner.cordTarget, Vector2.Zero, null, Color.White, npc.rotation, new Vector2(512), npc.scale, 0, 0);
            }
        }
            
        public void DrawCordShapes(On_Main.orig_CheckMonoliths orig)
        {
            orig();

            Main.spriteBatch.Begin(default, default, default, default, default, null);

            foreach (NPC npc in Main.npc.Where(n => n.active && n.ModNPC is Goozma))
            {
                Goozma targetOwner = (Goozma)npc.ModNPC;

                if (targetOwner.cordTarget == null || targetOwner.cordTarget.IsDisposed)
                    targetOwner.cordTarget = new RenderTarget2D(Main.graphics.GraphicsDevice, 1024, 1024, false, Main.graphics.GraphicsDevice.PresentationParameters.BackBufferFormat, DepthFormat.None, 0, RenderTargetUsage.PreserveContents);

                if (targetOwner.cordTarget != null)
                {
                    Main.graphics.GraphicsDevice.SetRenderTarget(cordTarget);
                    Main.graphics.GraphicsDevice.Clear(Color.Transparent);

                    List<Vector2> positions = new List<Vector2>();
                    List<float> rotations = new List<float>();

                    float partitions = 50;
                    for (int i = 0; i < partitions; i++)
                    {
                        Vector2 offset = new Vector2(20 + Utils.GetLerpValue(0, partitions / 2f, i, true) * (110 + (float)Math.Sin((Main.GlobalTimeWrappedHourly * 4 - i / (partitions * 0.04f)) % MathHelper.TwoPi) * 12 * (i / partitions)), 0).RotatedBy(MathHelper.SmoothStep(0.5f, -3f, i / partitions));
                        offset.X *= npc.direction;
                        offset -= npc.velocity * Utils.GetLerpValue(partitions / 3f, partitions, i, true);
                        positions.Add(new Vector2(0, 10).RotatedBy(npc.rotation) + offset.RotatedBy(npc.rotation));
                        rotations.Add(offset.RotatedBy(npc.rotation).ToRotation() - MathHelper.PiOver2 * (i / partitions) * npc.direction);
                    }

                    VertexStrip cord = new VertexStrip();
                    cord.PrepareStripWithProceduralPadding(positions.ToArray(), rotations.ToArray(), ColorFunc, WidthFunc, Vector2.Zero, true);

                    Effect effect = ModContent.Request<Effect>($"{nameof(CalamityHunt)}/Assets/Effects/GoozmaCordMap", AssetRequestMode.ImmediateLoad).Value;
                    effect.Parameters["uTransformMatrix"].SetValue(Main.GameViewMatrix.NormalizedTransformationmatrix);
                    effect.Parameters["uTime"].SetValue(npc.localAI[0] * 0.0033f % 1f);
                    effect.Parameters["uTexture"].SetValue(ModContent.Request<Texture2D>($"{nameof(CalamityHunt)}/Assets/Textures/Goozma/LiquidTrail").Value);
                    effect.Parameters["uMap"].SetValue(ModContent.Request<Texture2D>($"{nameof(CalamityHunt)}/Assets/Textures/Goozma/GoozmaColorMap").Value);

                    effect.CurrentTechnique.Passes[0].Apply();
                    cord.DrawTrail();
                    Main.pixelShader.CurrentTechnique.Passes[0].Apply();

                    Main.graphics.GraphicsDevice.SetRenderTarget(null);
                    Main.graphics.GraphicsDevice.Clear(Color.Transparent);

                }
            }

            Main.spriteBatch.End();
        }
    }
}
