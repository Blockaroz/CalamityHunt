using System;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ModLoader;

namespace CalamityHunt.Content.Projectiles.Weapons.Rogue
{
	public class GoozmagaShrapnel : ModProjectile
	{
		public override void SetDefaults()
		{
			Projectile.width = 22;
			Projectile.height = 22;
			Projectile.friendly = true;
			Projectile.penetrate = -1;
			Projectile.timeLeft = 720;
			Projectile.tileCollide = false;
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

			if (mom != null && mom.type == ModContent.ProjectileType<GoozmagaBomb>())
			{
				bool hasTarget = mom.ai[0] > -1;
				NPC target = hasTarget ? Main.npc[(int)mom.ai[0]] : null;
				if (target != null)
				{
					Projectile.localAI[1] += Projectile.localAI[1] == 1 ? 0.75f : 1f;
					float distance = 100;
					distance = target.width >= target.height ? target.width : target.height;
					distance += Projectile.ai[1] == 1 ? 20 : 60;
					double deg = 120 * Projectile.ai[1] + Main.GlobalTimeWrappedHourly * 660 + Projectile.localAI[1];
					double rad = deg * (Math.PI / 180);
					float hyposx = target.Center.X - (int)(Math.Cos(rad) * distance) - Projectile.width / 2;
					float hyposy = target.Center.Y - (int)(Math.Sin(rad) * distance) - Projectile.height / 2;

					Projectile.position = new Vector2(hyposx, hyposy);
				}
			}
		}
	}
}
