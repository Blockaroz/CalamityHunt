using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Audio;
using Terraria.GameContent;
using ReLogic.Content;
using Terraria.Graphics.Shaders;

namespace CalamityHunt.Content.Projectiles.Weapons
{
	public class GoombombProj : ModProjectile
	{
		public override string Texture => "CalamityHunt/Content/Items/Weapons/Goozmaga";

		public override void SetDefaults()
		{
			Projectile.width = 40;
			Projectile.height = 40;
			Projectile.friendly = true;
			Projectile.penetrate = -1;
			Projectile.timeLeft = 720;
			Projectile.tileCollide = false;
			Projectile.scale = 0.25f;
			Projectile.DamageType = DamageClass.Throwing;
			if (ModLoader.HasMod("CalamityMod"))
			{
				ModRarity r;
				DamageClass d;
				Mod calamity = ModLoader.GetMod("CalamityMod");
				calamity.TryFind<DamageClass>("RogueDamageClass", out d);
				Projectile.DamageType = d;
			}
		}

		public override void AI()
		{
			Projectile mom = Main.projectile[(int)Projectile.ai[0]];

			if (mom != null && mom.type == ModContent.ProjectileType<GoombombShard>())
			{
				bool hasTarget = mom.ai[0] > -1;
				NPC target = hasTarget ? Main.npc[(int)mom.ai[0]] : null;
				if (target != null)
				{
					float dist = Projectile.ai[1] == 1 ? 60 : 200;
					double deg = 120 * Projectile.ai[1] + Main.GlobalTimeWrappedHourly * 660;
					double rad = deg * (Math.PI / 180);
					float hyposx = target.Center.X - (int)(Math.Cos(rad) * dist) - Projectile.width / 2;
					float hyposy = target.Center.Y - (int)(Math.Sin(rad) * dist) - Projectile.height / 2;

					Projectile.position = new Vector2(hyposx, hyposy);
				}
			}
		}
	}
}
