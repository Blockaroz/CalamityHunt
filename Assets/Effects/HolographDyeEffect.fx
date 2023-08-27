sampler2D uImage0 : register(s0);
float brightnesses[10];
float3 colors[10];
float baseToScreenPercent;
float baseToMapPercent;

float3 uColor;
float3 uSecondaryColor;
float uOpacity;
float2 uTargetPosition;
float uSaturation;
float uRotation;
float uTime;
float4 uSourceRect;
float2 uWorldPosition;
float uDirection;
float3 uLightSource;
float2 uImageSize0;
float2 uImageSize1;
float4 uLegacyArmorSourceRect;
float2 uLegacyArmorSheetSize;

float4 Screen(float4 base, float4 overlay)
{
    return 1 - ((1 - (base)) * (1 - (overlay)));
}

float3 GradientMap(float brightness, int gradientSegments, float segmentBrightness[10], float3 segmentColors[10])
{
    
    for (int i = 1; i < gradientSegments; i++)
    {
        float currentBrightness = segmentBrightness[i];
        
        if (currentBrightness >= brightness)
        {
            if (currentBrightness == brightness)
            {
                return segmentColors[i];
            }
            
            float previousBrightness = segmentBrightness[i - 1];
            float segmentLenght = currentBrightness - previousBrightness;
            float segmentProgress = (brightness - previousBrightness) / segmentLenght;
            
            return lerp(segmentColors[i - 1], segmentColors[i], segmentProgress);
        }
    }
    
    return segmentColors[gradientSegments - 1];
}

float4 PixelShaderFunction(float4 baseColor : COLOR0, float2 coords : TEXCOORD0) : COLOR0
{
    float4 sampleColor = tex2D(uImage0, coords);
    sampleColor.xyz *= uOpacity;
    
    float brightness = (sampleColor.r + sampleColor.g + sampleColor.b) / 3;
    
    //We tint the effect to look darker AFTER getting the brightness
    sampleColor.xyz *= uColor;
    
    
    float4 map = float4(GradientMap(brightness, 10, brightnesses, colors), 1);
    float4 color = lerp(sampleColor, map, baseToMapPercent);
    float4 screenColor = Screen(color, map);
    
    color = lerp(color, screenColor, baseToScreenPercent);
    return color * sampleColor.a * baseColor;
}

technique Technique1
{
    pass LiquidPass
    {
        PixelShader = compile ps_3_0 PixelShaderFunction();
    }
}