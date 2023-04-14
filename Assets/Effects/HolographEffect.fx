sampler2D uImage0 : register(s0);
texture uColorMap;
sampler2D colorMap = sampler_state
{
    texture = <uColorMap>;
    magfilter = LINEAR;
    minfilter = LINEAR;
    mipfilter = LINEAR;
    AddressU = wrap;
    AddressV = wrap;
};
float uTime;
float baseToScreenPercent;
float baseToMapPercent;

float4 Screen(float4 base, float4 overlay)
{
    return 1 - ((1 - (base)) * (1 - (overlay)));
}

float4 PixelShaderFunction(float4 baseColor : COLOR0, float2 coords : TEXCOORD0) : COLOR0
{
    float4 img = tex2D(uImage0, coords);
    float brightness = (img.r + img.g + img.b) / 3.0;
    float4 map = tex2D(colorMap, float2(frac(uTime), 0.5));
    if (length(img) > 0)
        return lerp(lerp(img * baseColor, map, baseToMapPercent), Screen(lerp(img * baseColor, map, baseToMapPercent), map), baseToScreenPercent);
    return 0;

}

technique Technique1
{
    pass LiquidPass
    {
        PixelShader = compile ps_2_0 PixelShaderFunction();
    }
}