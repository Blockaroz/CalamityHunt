using CalamityHunt.Common.Systems.Particles;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Utilities;

namespace CalamityHunt.Content.Particles.FlyingSlimes
{
    public class FlyingNormalSlime : Particle
    {
        public int time;
        public float distanceFade;

        public override void OnSpawn()
        {
            scale *= Main.rand.NextFloat(0.9f, 1.1f);
        }

        public override void Update()
        {
            if (data is Vector2)
            {
                velocity = Vector2.Lerp(velocity, position.DirectionTo((Vector2)data).SafeNormalize(Vector2.Zero) * 22f, 0.1f);
                if (position.Distance((Vector2)data) < 10)
                    Active = false;

                distanceFade = Utils.GetLerpValue(20, 80, position.Distance((Vector2)data), true);
            }
            else
                distanceFade = 1f;

            if (Main.rand.NextBool(3))
            {
                Dust slime = Dust.NewDustPerfect(position + Main.rand.NextVector2Circular(20, 20), DustID.TintableDust, velocity * 0.2f, 200, color, 0.5f + Main.rand.NextFloat());
                slime.noGravity = true;
            }

            time++;

            if (time > 500)
                Active = false;

            velocity *= 0.98f;
            rotation = velocity.ToRotation();

            if (color == Color.White)
            {
                WeightedRandom<Color> slimeColor = new WeightedRandom<Color>();
                slimeColor.Add(ContentSamples.NpcsByNetId[NPCID.GreenSlime].color, 1f);
                slimeColor.Add(ContentSamples.NpcsByNetId[NPCID.BlueSlime].color, 0.9f);
                slimeColor.Add(ContentSamples.NpcsByNetId[NPCID.RedSlime].color, 0.3f);
                slimeColor.Add(ContentSamples.NpcsByNetId[NPCID.YellowSlime].color, 0.2f);
                //slimeColor.Add(ContentSamples.NpcsByNetId[NPCID.BlackSlime].color, 0.1f); // this looks bad
                slimeColor.Add(ContentSamples.NpcsByNetId[NPCID.JungleSlime].color, 0.3f);
                color = slimeColor.Get();

                if (Main.rand.NextBool(1000))
                {
                    color = ContentSamples.NpcsByNetId[NPCID.PurpleSlime].color;
                    scale = ContentSamples.NpcsByNetId[NPCID.PurpleSlime].scale;
                }
                if (Main.rand.NextBool(3000))
                {
                    color = ContentSamples.NpcsByNetId[NPCID.Pinky].color;
                    scale = ContentSamples.NpcsByNetId[NPCID.Pinky].scale;
                }
            }
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            Asset<Texture2D> texture = ModContent.Request<Texture2D>(Texture);

            float fadeIn = Utils.GetLerpValue(0, 30, time, true) * distanceFade;

            for (int i = 0; i < 10; i++)
                spriteBatch.Draw(texture.Value, position - velocity * i * 0.5f - Main.screenPosition, null, color.MultiplyRGBA(Lighting.GetColor(position.ToTileCoordinates())) * fadeIn * ((10f - i) / 80f), rotation + MathHelper.PiOver2, texture.Size() * 0.5f, scale * distanceFade * 1.05f, 0, 0);

            spriteBatch.Draw(texture.Value, position - Main.screenPosition, null, color.MultiplyRGBA(Lighting.GetColor(position.ToTileCoordinates())) * fadeIn, rotation + MathHelper.PiOver2, texture.Size() * 0.5f, scale * distanceFade, 0, 0);
        }
    }
}
