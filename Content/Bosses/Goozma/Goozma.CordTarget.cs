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

        public void DrawCordShapes(On_Main.orig_CheckMonoliths orig)
        {
            orig();
            
            bool switchedTargets = false;

            //WE DO NOT USE GAMEVIEWMATRIX TRANSFORMATIONMATRIX because that ruins the pixelated thing for reasons. Just use the ortographic projection witout the view matrix
            Matrix theRealMatrix = Matrix.CreateOrthographicOffCenter(0, Main.screenWidth, Main.screenHeight, 0, -1, 1);
            Effect effect = ModContent.Request<Effect>($"{nameof(CalamityHunt)}/Assets/Effects/GoozmaCordMap", AssetRequestMode.ImmediateLoad).Value;

            foreach (NPC npc in Main.npc.Where(n => n.active && n.ModNPC is Goozma))
            {
                Goozma targetOwner = (Goozma)npc.ModNPC;

                if (targetOwner.cordTarget == null || targetOwner.cordTarget.IsDisposed)
                    targetOwner.cordTarget = new RenderTarget2D(Main.graphics.GraphicsDevice, Main.screenWidth / 2, Main.screenHeight / 2);

                else if (targetOwner.cordTarget.Size() != new Vector2(Main.screenWidth / 2, Main.screenHeight / 2) )
                {
                    Main.QueueMainThreadAction(() =>
                    {
                        targetOwner.cordTarget.Dispose();
                        targetOwner.cordTarget = new RenderTarget2D(Main.graphics.GraphicsDevice, Main.screenWidth / 2, Main.screenHeight / 2);
                    });
                    return;
                }
				
                if (targetOwner.cordTarget != null)
                {
                    //Be sure to switch to the **TARGET OWNER's** cordTarget, not just cordTarget
                    //If you just use cordTarget that'll use the one from the autoloaded instance who loaded the detour
                    Main.graphics.GraphicsDevice.SetRenderTarget(targetOwner.cordTarget);
                    Main.graphics.GraphicsDevice.Clear(Color.Transparent);
                    if (!switchedTargets)
                    {
                        Main.spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, SamplerState.PointClamp, DepthStencilState.None, Main.Rasterizer, null);
                        switchedTargets = true;
                    }

                    List<Vector2> positions = new List<Vector2>();
                    List<float> rotations = new List<float>();

                    float partitions = 50;
                    for (int i = 0; i < partitions; i++)
                    {
                        Vector2 offset = new Vector2(20 + Utils.GetLerpValue(0, partitions / 2f, i, true) * Utils.GetLerpValue(partitions * 1.5f, partitions / 2f, i, true) * (140 + (float)Math.Sin((Main.GlobalTimeWrappedHourly * 4 - i / (partitions * 0.04f)) % MathHelper.TwoPi) * 16 * (i / partitions)), 0).RotatedBy(MathHelper.SmoothStep(0.1f, -3.5f, i / partitions));
                        offset.X *= npc.direction;
                        offset -= npc.velocity * Utils.GetLerpValue(partitions / 3f, partitions, i, true);
                        positions.Add((new Vector2(0, 30).RotatedBy(npc.rotation) + offset.RotatedBy(npc.rotation)) * npc.scale);
                        rotations.Add(offset.RotatedBy(npc.rotation).ToRotation() - MathHelper.PiOver2 * (i / partitions) * npc.direction);
                    }

                    effect.Parameters["uTransformMatrix"].SetValue(theRealMatrix);
                    effect.Parameters["uTime"].SetValue(npc.localAI[0] * 0.0033f % 1f);
                    effect.Parameters["uTexture"].SetValue(ModContent.Request<Texture2D>($"{nameof(CalamityHunt)}/Assets/Textures/Goozma/LiquidTrail").Value);
                    effect.Parameters["uMap"].SetValue(ModContent.Request<Texture2D>($"{nameof(CalamityHunt)}/Assets/Textures/Goozma/GoozmaColorMap").Value);
                    effect.CurrentTechnique.Passes[0].Apply();

                    VertexStrip cord = new VertexStrip();
                    cord.PrepareStripWithProceduralPadding(positions.ToArray(), rotations.ToArray(), ColorFunc, WidthFunc, npc.Center - Main.screenPosition, true);
                    cord.DrawTrail();
                    Main.pixelShader.CurrentTechnique.Passes[0].Apply();
                }
            }

            //Go back to the backbuffer RT if we switched away from it.
            //Putting it at the end like that avoids having to switch back and forth if somehow theres more than 1 goozma
            //Also you don't have to clear it transparent when switching back to it
            if (switchedTargets)
            {
                Main.graphics.GraphicsDevice.SetRenderTarget(null);
                Main.spriteBatch.End();
            }
        }
    }
}
