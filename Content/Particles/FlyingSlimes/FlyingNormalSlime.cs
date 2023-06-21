using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.Utilities;

namespace CalamityHunt.Content.Particles.FlyingSlimes
{
    public class FlyingNormalSlime : FlyingSlime
    {
        public override void PostUpdate()
        {
            if (Main.rand.NextBool(3))
            {
                Dust slime = Dust.NewDustPerfect(position + Main.rand.NextVector2Circular(20, 20), DustID.TintableDust, velocity * 0.2f, 200, color, 0.5f + Main.rand.NextFloat());
                slime.noGravity = true;
            }

            if (color == Color.White)
            {
                WeightedRandom<Color> slimeColor = new WeightedRandom<Color>();
                slimeColor.Add(ContentSamples.NpcsByNetId[NPCID.GreenSlime].color, 1f);
                slimeColor.Add(ContentSamples.NpcsByNetId[NPCID.BlueSlime].color, 0.9f);
                slimeColor.Add(ContentSamples.NpcsByNetId[NPCID.RedSlime].color, 0.3f);
                slimeColor.Add(ContentSamples.NpcsByNetId[NPCID.YellowSlime].color, 0.2f);
                slimeColor.Add(Color.Gray, 0.1f);
                slimeColor.Add(ContentSamples.NpcsByNetId[NPCID.JungleSlime].color, 0.3f);
                color = slimeColor.Get();

                if (Main.rand.NextBool(20))
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
    }
}
