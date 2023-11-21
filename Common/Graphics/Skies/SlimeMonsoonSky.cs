using System;
using CalamityHunt.Common.Systems;
using CalamityHunt.Common.Utilities;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.GameContent;
using Terraria.Graphics.Effects;
using Terraria.Utilities;

namespace CalamityHunt.Common.Graphics.Skies;

public class SlimeMonsoonSky : CustomSky
{
    public override void Activate(Vector2 position, params object[] args)
    {
        _active = true;
        radialDistortPos = position;
        strengthTarget = 1f;
        additionalLightningChance = 0;
        lightningEnabled = true;
    }

    public override void Deactivate(params object[] args)
    {
        _active = false;
        strengthTarget = 0;
        additionalLightningChance = 0;
    }

    public override bool IsActive() => _strength > 0.001f && !Main.gameMenu;

    public override void Reset()
    {
        _active = false;
        strengthTarget = 0;
        additionalLightningChance = 0;
    }

    private static UnifiedRandom _random = new UnifiedRandom();
    private bool _active;
    private float _strength;
    private float _windSpeed;
    private float _brightness;
    public static Vector2 radialDistortPos;
    public static float strengthTarget;
    public static int additionalLightningChance;
    public static bool lightningEnabled;
    public static Color lightColor;
    public static float DistortionStrength => Config.Instance.monsoonDistortion;

    public static float? forceStrength;

    public override Color OnTileColor(Color inColor)
    {
        if (inColor.R + inColor.G + inColor.B > 20) {
            float fastStrength = Math.Clamp(_strength * 3f, 0, 1f);
            Main.ColorOfTheSkies = Color.Lerp(Main.ColorOfTheSkies, Color.Black, fastStrength);
            return inColor.MultiplyRGBA(Color.Lerp(Color.White, lightColor, fastStrength));
        }
        return inColor;
    }

    public override float GetCloudAlpha() => 1f - _strength;

    public override void Update(GameTime gameTime)
    {
        if (_active)
            _strength = Math.Min(strengthTarget, _strength + 0.005f);
        else
            _strength = Math.Max(0f, _strength - 0.005f);

        if (forceStrength.HasValue) {
            _strength = forceStrength.Value;
            forceStrength = null;
        }

        radialDistortPos = Vector2.Lerp(radialDistortPos, Main.LocalPlayer.Center, 0.5f);

        _brightness = MathHelper.Lerp(_brightness, 0.15f, 0.08f);
        _windSpeed -= Main.WindForVisuals * 0.0025f;
        _windSpeed = _windSpeed % 1f;
    }

    public override void Draw(SpriteBatch spriteBatch, float minDepth, float maxDepth)
    {
        SkyManager.Instance["Ambience"].Deactivate();

        float yOffPower = (float)Utils.GetLerpValue(200, Main.rockLayer - 100, Main.LocalPlayer.Center.Y / 16f, true);
        int yOffset = (int)(yOffPower * 1600f + (float)Math.Sin(Main.GlobalTimeWrappedHourly * 0.275f % MathHelper.TwoPi) * 100f);

        //gold: new Color(40, 22, 15);
        Color brightColor = new GradientColor(new Color[]
        {
            new Color(20, 16, 42),
            new Color(25, 20, 32),
            new Color(30, 15, 20)

        }, 2f, 2f).Value;
        Color darkColor = Color.Lerp(brightColor, Color.Black, 0.3f);
        lightColor = Color.Lerp(Color.DimGray, brightColor, 0.9f);

        if (maxDepth >= float.MaxValue && minDepth < float.MaxValue) {
            spriteBatch.Draw(TextureAssets.BlackTile.Value, new Rectangle(0, 0, Main.screenWidth, Main.screenHeight), Color.Black * (float)Math.Sqrt(_strength));
            spriteBatch.Draw(AssetDirectory.Textures.Noise[4].Value, new Rectangle(0, -yOffset, Main.screenWidth, Main.screenHeight * 2), darkColor * _strength * 0.66f);
        }
    }

    public class MonsoonStrike
    {
        public MonsoonStrike(int layer)
        {

        }
    }
}
