sampler uImage0 : register(s0);
texture uTexture0;
sampler tex0 = sampler_state
{
    texture = <uTexture0>;
    magfilter = LINEAR;
    minfilter = LINEAR;
    mipfilter = LINEAR;
    AddressU = wrap;
    AddressV = wrap;
};
texture uTexture1;
sampler tex1 = sampler_state
{
    texture = <uTexture1>;
    magfilter = LINEAR;
    minfilter = LINEAR;
    mipfilter = LINEAR;
    AddressU = wrap;
    AddressV = wrap;
};
float2 random0;
float2 random1;
float uSize;
float2 uStrength;
float4 uColor;

float4 PixelShaderFunction(float4 baseColor : COLOR0, float2 coords : TEXCOORD0) : COLOR0
{
    float2 center = ((coords * 2) - 1);
    float power = (1 - length(center)) * uStrength;
    float2 polar = float2(atan2(center.y, center.x) / 6.28, length(center) * 0.008 * uSize);
    float4 crack0 = tex2D(tex0, polar + random0);
    float4 crack1 = tex2D(tex1, polar + random1);
    
    float finalPower = ((crack0 * crack1) * 5 * power + power);
    if (finalPower * 0.22 + power > 0.8)
        return lerp(float4(uColor.rgb * 0.4, 1), float4(0, 0, 0, 1), power * 1.1);
    else if (finalPower > 0.7 - power * 0.3)
        return float4((uColor * crack0 * crack1 + ((finalPower * 0.5 + power > 0.9) ? uColor : 0)).rgb, pow(finalPower, 4));
    return 0;
}

technique Technique1
{
    pass CrackPass
    {
        PixelShader = compile ps_2_0 PixelShaderFunction();
    }
}