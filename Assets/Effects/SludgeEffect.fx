sampler2D uImage0 : register(s0);
texture uBaseTexture;
sampler tex0 = sampler_state
{
    texture = <uBaseTexture>;
    magfilter = LINEAR;
    minfilter = LINEAR;
    mipfilter = LINEAR;
    AddressU = wrap;
    AddressV = wrap;
};
texture uGlowTexture;
sampler tex1 = sampler_state
{
    texture = <uGlowTexture>;
    magfilter = LINEAR;
    minfilter = LINEAR;
    mipfilter = LINEAR;
    AddressU = wrap;
    AddressV = wrap;
};
float2 uPosition;
float4 uBaseColor;
float uTime;
float2 uSize;
float2 uScreenSize;

float4 PixelShaderFunction(float4 baseColor : COLOR0, float2 coords : TEXCOORD0) : COLOR0
{
    float4 screen = tex2D(uImage0, coords);
    float4 color = float4(screen.g, screen.g, screen.g, 1) * (length(screen) > 0 ? 1 : 0);
    
    float2 offsetCoords = (coords + uPosition / uScreenSize) * (uScreenSize / uScreenSize.y);
    float4 baseTexture = tex2D(tex0, float2(frac(offsetCoords.x - sin((offsetCoords.y + uTime) * 50 + uTime * 6.28) * 0.01), frac(offsetCoords.y + uTime * 3)) * uSize);
    float4 glowTexture = tex2D(tex0, float2(frac(offsetCoords.x - sin((offsetCoords.y + uTime) * 50 + uTime * 6.28) * 0.01), frac(offsetCoords.y + uTime * 3)) * uSize);

    return (baseTexture + glowTexture) * color;

}

technique Technique1
{
    pass GelPass
    {
        PixelShader = compile ps_2_0 PixelShaderFunction();
    }
}