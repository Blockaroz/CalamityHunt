using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria.ModLoader;

namespace CalamityHunt.Common.Systems.Particles;

public static class ParticleLoader
{
    public static int IDCount = 0;

    public static int ReserveID() => IDCount++;

    public static Dictionary<Type, Particle> Instance = new Dictionary<Type, Particle>();
}
