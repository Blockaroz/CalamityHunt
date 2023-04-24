using CalamityHunt.Common.Systems.Particles;
using CalamityHunt.Content.Projectiles;
using Terraria.Audio;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using Terraria;
using Terraria.ModLoader;

namespace CalamityHunt.Content.Particles.FlyingSlimes
{
    public abstract class FlyingSlime : Particle
    {
        public int time;
        public float distanceFade;

        /// <summary>
        /// The update function for the slime. Runs after the rest of Update()
        /// </summary>
        public virtual void PostUpdate(){}
        /// <summary>
        /// The draw function for the slime, return false when making custom drawing
        /// </summary>
        public virtual void DrawSlime(SpriteBatch spriteBatch){}
        /// <summary>
        /// Effects for when the slime DIES
        /// </summary>
        public virtual void KillEffect(){}
        /// <summary>
        /// The acceleration of the slime, defaults to 0.1
        /// </summary>
        public virtual float SlimeSpeed => 22f;
        /// <summary>
        /// The speed of the slime, defaults to 22
        /// </summary>
        public virtual float SlimeAcceleration => 0.1f;
        /// <summary>
        ///  Whether or not normal slime drawing applies. While false, the slime won't draw on its own.
        /// </summary>
        public virtual bool ShouldDraw => true;
        /// <summary>
        ///  Whether or not normal slime rotation applies. While false, the slime won't rotate on its own.
        /// </summary>
        public virtual bool ShouldRotate => true;


        public override void OnSpawn()
        {
            scale *= Main.rand.NextFloat(0.9f, 1.1f);
        }

        public override void Update()
        {
            if (data is Vector2)
            {
                velocity = Vector2.Lerp(velocity, position.DirectionTo((Vector2)data).SafeNormalize(Vector2.Zero) * SlimeSpeed, SlimeAcceleration);
                if (position.Distance((Vector2)data) < 10)
                {
                    Active = false;
                    SoundEngine.PlaySound(GoozmaSpawn.slimeabsorb, position);
                    KillEffect();
                }

                distanceFade = Utils.GetLerpValue(20, 80, position.Distance((Vector2)data), true);
            }
            else
                distanceFade = 1f;

            time++;

            if (time > 500)
                Active = false;

            velocity *= 0.98f;
            rotation = velocity.ToRotation();

            PostUpdate();
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            if (ShouldDraw)
            {
                Asset<Texture2D> texture = ModContent.Request<Texture2D>(Texture);

                float fadeIn = Utils.GetLerpValue(0, 30, time, true) * distanceFade;

                for (int i = 0; i < 10; i++)
                    spriteBatch.Draw(texture.Value, position - velocity * i * 0.5f - Main.screenPosition, null, color.MultiplyRGBA(Lighting.GetColor(position.ToTileCoordinates())) * fadeIn * ((10f - i) / 80f), rotation + MathHelper.PiOver2, texture.Size() * 0.5f, scale * distanceFade * 1.05f, 0, 0);

                spriteBatch.Draw(texture.Value, position - Main.screenPosition, null, color.MultiplyRGBA(Lighting.GetColor(position.ToTileCoordinates())) * fadeIn, rotation + MathHelper.PiOver2, texture.Size() * 0.5f, scale * distanceFade, 0, 0);
            }
            else
            {
                DrawSlime(spriteBatch);
            }
        }
    }
}
