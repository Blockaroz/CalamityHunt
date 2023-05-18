using CalamityHunt.Content.Items.Weapons.Magic;
using CalamityHunt.Content.Items.Weapons.Summoner;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.DataStructures;
using Terraria.ModLoader;

namespace CalamityHunt.Common.Systems
{
    public class GoozmaWeaponsPlayer : ModPlayer
    {
        public static int ParasolBloodMax = 8000;
        public int parasolBlood;
        public int parasolBloodWaitTime;
        public float ParasolBloodPercent => Math.Clamp((float)parasolBlood / ParasolBloodMax, 0, 1);

        public bool gobfloggerBuff;

        public bool crystalGauntletsHeld;
        public bool crystalGauntletsClapping;

        public int crystalGauntletsWaitTime;
        public float CrystalGauntletsCharge;

        public int crystalGauntletsFingerGunTime;
        public Vector2 crystalGauntletsFingerGunDirection;
        public bool crystalGauntletsFingerGunOff;

        public override void PostUpdateMiscEffects()
        {
            if (parasolBloodWaitTime > 0)
                parasolBloodWaitTime--;            
            
            if (crystalGauntletsWaitTime > 0)
                crystalGauntletsWaitTime--;

            if (parasolBlood > 0 && parasolBloodWaitTime <= 0)
                parasolBlood -= 100;            
            
            if (CrystalGauntletsCharge > 0 && crystalGauntletsWaitTime <= 0)
                CrystalGauntletsCharge *= 0.85f;

            if (CrystalGauntletsCharge < 0.002f)
                CrystalGauntletsCharge = 0f;

            parasolBlood = Math.Clamp(parasolBlood, 0, ParasolBloodMax);
            CrystalGauntletsCharge = Math.Clamp(CrystalGauntletsCharge, 0f, 1f);
        }

        public override void DrawEffects(PlayerDrawSet drawInfo, ref float r, ref float g, ref float b, ref float a, ref bool fullBright)
        {
            if (drawInfo.shadow == 0f)
            {
                if (crystalGauntletsFingerGunTime > 0)
                {
                    float recoil = -MathHelper.SmoothStep(0, 1, MathF.Pow(Utils.GetLerpValue(30, 47, crystalGauntletsFingerGunTime, true), 0.6f)) * Utils.GetLerpValue(50, 47, crystalGauntletsFingerGunTime, true);
                    float backArmFly = -MathHelper.SmoothStep(0, 1, Utils.GetLerpValue(0, 40, crystalGauntletsFingerGunTime, true) * Utils.GetLerpValue(50, 40, crystalGauntletsFingerGunTime, true)) * 0.5f - 1.8f;

                    if (!crystalGauntletsFingerGunOff)
                    {
                        Player.SetCompositeArmFront(true, Player.CompositeArmStretchAmount.Full, crystalGauntletsFingerGunDirection.ToRotation() - MathHelper.PiOver2 - backArmFly * Player.direction);
                        Player.SetCompositeArmBack(true, Player.CompositeArmStretchAmount.Full, crystalGauntletsFingerGunDirection.ToRotation() - MathHelper.PiOver2 + recoil * Player.direction);
                    }
                    else
                    {
                        Player.SetCompositeArmFront(true, Player.CompositeArmStretchAmount.Full, crystalGauntletsFingerGunDirection.ToRotation() - MathHelper.PiOver2 + recoil * Player.direction);
                        Player.SetCompositeArmBack(true, Player.CompositeArmStretchAmount.Full, crystalGauntletsFingerGunDirection.ToRotation() - MathHelper.PiOver2 - backArmFly * Player.direction);
                    }
                }
            }
        }

        public override float UseSpeedMultiplier(Item item)
        {
            if (gobfloggerBuff && item.type == ModContent.ItemType<Gobflogger>())
                return 6.66f;
            return 1f;
        }

        public override void ResetEffects()
        {
            gobfloggerBuff = false;

            crystalGauntletsHeld = false;
            crystalGauntletsClapping = false;

            if (crystalGauntletsFingerGunTime > 0)
                crystalGauntletsFingerGunTime--;

            if (Player.HeldItem.type != ModContent.ItemType<CrystalGauntlets>())
                crystalGauntletsFingerGunOff = false;
        }
    }
}
