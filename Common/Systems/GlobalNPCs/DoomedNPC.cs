using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CalamityHunt.Common.Systems.Particles;
using CalamityHunt.Content.Particles;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ModLoader;

namespace CalamityHunt.Common.Systems.GlobalNPCs;

public class DoomedNPC : GlobalNPC
{
    public override bool InstancePerEntity => true;

    public int doomCount;

    public override void PostAI(NPC npc)
    {
        if (doomCount > 0) {
            doomCount--;

            CalamityHunt.particles.Add(Particle.Create<LightningParticle>(particle => {
                particle.position = Main.rand.NextVector2FromRectangle(npc.Hitbox);
                particle.velocity = particle.position.DirectionFrom(npc.Center) * Main.rand.NextFloat(2f);
                particle.scale = Main.rand.NextFloat(0.7f, 1.5f);
                particle.color = Color.Turquoise with { A = 40 };
                particle.anchor = () => npc.velocity;
            }));
        }
    }
}
