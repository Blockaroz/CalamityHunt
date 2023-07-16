using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using Terraria;
using Terraria.DataStructures;
using Terraria.Graphics.Shaders;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityHunt.Content.Mounts
{
    public class PaladinPalanquinMount : ModMount
    {
        public override void SetStaticDefaults()
        {
            MountData.jumpHeight = 16;
            MountData.jumpSpeed = 8f;

            MountData.acceleration = 0.05f;
            MountData.blockExtraJumps = true;
            MountData.constantJump = true;
            MountData.fallDamage = 0;
            MountData.runSpeed = 12;
            MountData.dashSpeed = 16;

            MountData.usesHover = true;

            MountData.fatigueMax = 0;
            MountData.buff = ModContent.BuffType<Buffs.PaladinPalanquinBuff>();

            MountData.yOffset = -18;
            MountData.bodyFrame = 3;

            MountData.totalFrames = 1;

            MountData.heightBoost = 12;
            MountData.playerYOffsets = Enumerable.Repeat(MountData.heightBoost, MountData.totalFrames).ToArray();

            if (!Main.dedServ)
            {
                MountData.textureWidth = MountData.backTexture.Width() + 20;
                MountData.textureHeight = MountData.backTexture.Height();
            }
        }

        public override void SetMount(Player player, ref bool skipDust)
        {
            player.mount._mountSpecificData = new PaladinPalanquinData() { flying = false, rotation = 0, tilt = 0 };

            skipDust = true;
        }

        public override void Dismount(Player player, ref bool skipDust)
        {
            skipDust = true;
        }

        public override void UpdateEffects(Player player)
        {
            if (player.velocity.Length() > 1)
            {
                for (int i = 0; i < 3; i++)
                {
                    Dust d = Dust.NewDustPerfect(player.MountedCenter + Main.rand.NextVector2Circular(42, 42), DustID.TintableDust, player.velocity * 0.5f, 100, Color.Black, 1f + Main.rand.NextFloat());
                    d.noGravity = true;
                }
            }
            if (player.mount._mountSpecificData is PaladinPalanquinData data)
            {
                data.tilt = player.velocity.X * 0.01f;

                bool onGround = Collision.SolidCollision(player.MountedCenter - new Vector2(20, 10), 40, 50);

                if (player.controlJump && player.releaseJump && !onGround)
                    data.flying = !data.flying;

                if (!data.flying)
                {
                    data.rotation += player.velocity.X * 0.02f;

                    player.maxFallSpeed = 20;
                    player.velocity.Y += player.gravity * 0.7f;

                    if (player.controlJump && player.releaseJump && onGround)
                    {
                        player.velocity.Y -= 13;
                    }
                }
                else
                {
                    data.rotation += player.velocity.X * 0.005f;

                    if (player.controlUp)
                        player.velocity.Y -= MountData.acceleration;

                    if (player.controlDown)
                        player.velocity.Y += MountData.acceleration;

                    if (player.velocity.Length() > MountData.runSpeed)
                        player.velocity = player.velocity.SafeNormalize(Vector2.Zero) * MountData.runSpeed;
                }
            }
            else
                player.mount._mountSpecificData = new PaladinPalanquinData();

            if (Collision.SolidCollision(player.MountedCenter + new Vector2(player.velocity.X * 3f - 10, -1), 2, 20) && Math.Abs(player.velocity.X) > 1f)
                player.velocity.X *= -0.5f;

            if (Collision.SolidCollision(player.MountedCenter + new Vector2(-1, player.velocity.Y * 3f - 10), 2, 20) && Math.Abs(player.velocity.Y) > 1f)
                player.velocity.Y *= -0.9f;

        }

        protected class PaladinPalanquinData
        {
            public bool flying;

            public float rotation;

            public float tilt;
        }

        public override bool Draw(List<DrawData> playerDrawData, int drawType, Player drawPlayer, ref Texture2D texture, ref Texture2D glowTexture, ref Vector2 drawPosition, ref Rectangle frame, ref Color drawColor, ref Color glowColor, ref float rotation, ref SpriteEffects spriteEffects, ref Vector2 drawOrigin, ref float drawScale, float shadow)
        {
            if (drawPlayer.mount._mountSpecificData is PaladinPalanquinData data)
            {
                drawPlayer.fullRotationOrigin = drawPlayer.Size * 0.5f;
                drawPlayer.fullRotation = data.tilt;

                switch (drawType)
                {
                    case 0:

                        DrawData saddleData = new DrawData(MountData.backTexture.Value, drawPlayer.MountedCenter - Main.screenPosition, MountData.backTexture.Frame(), drawColor, data.tilt, MountData.backTexture.Size() * 0.5f, drawScale, spriteEffects, 0);
                        saddleData.shader = drawPlayer.cMount;
                        playerDrawData.Add(saddleData);

                        break;

                    case 2:

                        DrawData bubbleData = new DrawData(MountData.frontTexture.Value, drawPlayer.MountedCenter - drawPlayer.velocity * 0.3f - Main.screenPosition, MountData.frontTexture.Frame(), drawColor, data.rotation, MountData.frontTexture.Size() * 0.5f, drawScale, 0, 0);
                        bubbleData.shader = drawPlayer.cMount;
                        playerDrawData.Add(bubbleData);
                        break;

                    case 3:

                        break;
                }
            }
            else
                drawPlayer.mount._mountSpecificData = new PaladinPalanquinData();

            return false;   
        }
    }
}
