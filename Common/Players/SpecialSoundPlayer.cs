using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria.Audio;
using Terraria;
using Terraria.ModLoader;
using CalamityHunt.Content.Items.Weapons.Ranged;
using Microsoft.Xna.Framework;
using CalamityHunt.Common.Utilities;

namespace CalamityHunt.Common.Players
{
    public class SpecialSoundPlayer : ModPlayer
    {
        public LoopingSound trailBlazerSound;
        public float trailBlazerVolume;
        private bool trailBlazerInUse;

        public override void PostUpdateMiscEffects()
        {
            if (Player.HeldItem.type == ModContent.ItemType<Trailblazer>())
            {
                if (Player.ItemAnimationJustStarted && !trailBlazerInUse)
                {
                    SoundEngine.PlaySound(Player.HeldItem.UseSound.Value.WithVolumeScale(0.6f), Player.MountedCenter);
                    trailBlazerInUse = true;
                }
                if (!Player.ItemAnimationActive)
                    trailBlazerInUse = false;

                trailBlazerVolume += Player.ItemAnimationActive ? 0.2f : -0.05f;
            }
            else
            {
                trailBlazerVolume -= 0.05f;
                trailBlazerInUse = false;
            }

            trailBlazerVolume = MathHelper.Clamp(trailBlazerVolume, 0f, 0.6f);

            if (trailBlazerSound == null)
                trailBlazerSound = new LoopingSound(AssetDirectory.Sounds.Weapons.TrailblazerFireLoop, () => trailBlazerVolume > 0.1f);
            trailBlazerSound.PlaySound(() => Player.MountedCenter, () => trailBlazerVolume, () => MathF.Sin(Main.GlobalTimeWrappedHourly * 8) * 0.07f);

        }
    }
}
