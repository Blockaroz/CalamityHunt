using CalamityHunt.Common.Systems;
using CalamityHunt.Common.Systems.Particles;
using CalamityHunt.Content.Items.Rarities;
using CalamityHunt.Content.Particles;
using CalamityHunt.Content.Projectiles.Weapons.Magic;
using CalamityHunt.Content.Projectiles.Weapons.Melee;
using CalamityHunt.Content.Projectiles.Weapons.Ranged;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityHunt.Content.Items.Weapons.Magic
{
    public class Goomoire : ModItem
    {
        public override void SetDefaults()
        {
            Item.width = 40;
            Item.height = 48;
            Item.damage = 2800;
            Item.DamageType = DamageClass.Magic;
            Item.rare = ModContent.RarityType<VioletRarity>();
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.mana = 25;
            Item.useTime = 30;
            Item.useAnimation = 30;
            Item.channel = true;
            Item.noUseGraphic = true;
            Item.noMelee = true;
            Item.value = Item.sellPrice(gold: 20);
            Item.shoot = ModContent.ProjectileType<TrailblazerFlame>();
            Item.shootSpeed = 4f;
            if (ModLoader.HasMod("CalamityMod"))
            {
                ModRarity r;
                Mod calamity = ModLoader.GetMod("CalamityMod");
                calamity.TryFind<ModRarity>("Violet", out r);
                Item.rare = r.Type;
            }
        }
    }
}
