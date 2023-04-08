sampler2D uImage0 : register(s0);
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
float2 uImageSize;
float4 uSourceRect;
float uRbThreshold;
float uTime;
float uRbTime;
float uFrequency;

float3 HSLtoRGB(float hue, float saturation, float luminosity)
{
    float r = abs(hue * 6.0 - 3) - 1.0;
    float g = 2.0 - abs(hue * 6.0 - 2.0);
    float b = 2.0 - abs(hue * 6.0 - 4.0);
    float3 rgb = saturate(float3(r, g, b));
    return (rgb - 0.5) * (1 - abs(2 * luminosity - 1)) * saturation + luminosity;
}

float4 PixelShaderFunction(float4 baseColor : COLOR0, float2 coords : TEXCOORD0) : COLOR0
{
    float yCoord = (coords.y * uImageSize.y - uSourceRect.y) / uSourceRect.w;
    float4 img = tex2D(uImage0, coords);
    float4 color = tex2D(map, float2(img.r * uFrequency + frac(yCoord * 0.33 + uTime), saturate(img.r - img.g)));
    float4 final = float4(color.rgb * (0.4 + img.r), 1);

    float shine = smoothstep(uRbThreshold - 0.1, uRbThreshold + 0.1, img.r);
    float3 hue = HSLtoRGB(frac(yCoord * uFrequency + uRbTime), 1.1, 0.8);
    
    return lerp(final * img.a, float4(hue, 1), shine);
}

technique Technique1
{
    pass GelPass
    {
        PixelShader = compile ps_2_0 PixelShaderFunction();
    }
}