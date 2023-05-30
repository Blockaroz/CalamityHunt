using CalamityHunt.Content.Bosses.Goozma;
using CalamityHunt.Content.Items.Rarities;
using CalamityHunt.Content.Projectiles.Weapons.Ranged;
using CalamityHunt.Content.Projectiles.Weapons.Summoner;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityHunt.Content.Items.Weapons.Summoner
{
    public class SlimeCane : ModItem
    {
        public override void SetDefaults()
        {
            Item.width = 54;
            Item.height = 84;
            Item.useTime = 30;
            Item.useAnimation = 30;
            Item.useTurn = true;
            Item.useStyle = ItemUseStyleID.Swing;
            Item.damage = 2000;
            Item.noMelee = true;
            Item.knockBack = 10f;
            SoundStyle useSound = SoundID.AbigailUpgrade;
            useSound.Volume = 1f;
            useSound.Pitch = -0.33f;
            useSound.MaxInstances = 0;
            Item.UseSound = useSound;
            Item.shoot = ModContent.ProjectileType<TrailblazerFlame>();
            Item.shootSpeed = 8f;

            Item.rare = ModContent.RarityType<VioletRarity>();
            if (ModLoader.HasMod("CalamityMod"))
            {
                ModRarity r;
                Mod calamity = ModLoader.GetMod("CalamityMod");
                calamity.TryFind<ModRarity>("Violet", out r);
                Item.rare = r.Type;
            }
            Item.DamageType = DamageClass.Summon;
        }

        public override void UseAnimation(Player player)
        {
            player.itemRotation += MathHelper.Pi / 8f * player.direction;
        }
    }
}
