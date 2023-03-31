sampler uImage0 : register(s0);
sampler uImage1 : register(s1);
texture uMap;
sampler2D map = sampler_state
{
    texture = <uMap>;
    magfilter = LINEAR;
    minfilter = LINEAR;
    mipfilter = LINEAR;
    AddressU = wrap;
    AddressV = wrap;
};
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

float4 PixelShaderFunction(float4 base : COLOR0, float2 input : TEXCOORD0) : COLOR0
{
    float4 color = tex2D(uImage0, input);
    float baseAlpha = length(base) / 4 > 0 ? 1 : 0;
    float power = (color.r + color.g + color.b) / 3;
    float4 mapColor = tex2D(map, float2(power * 0.7 + uTime * 0.5, 0.5)) * float4(1, 1, 1, 0) * sqrt(length(base.rgb) + 0.1);
    
    return (mapColor + float4(uColor * power * sqrt(length(base.rgb)), 1)) * color.a * sqrt(base.a);

}

technique Technique1
{
    pass LiquidPass
    {
        PixelShader = compile ps_2_0 PixelShaderFunction();
    }
}