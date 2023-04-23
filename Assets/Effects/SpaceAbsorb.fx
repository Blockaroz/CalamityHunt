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
float uSize;
float2 uStrength;
float4 uColor;
float uTime;
float uRepeats;

float4 PixelShaderFunction(float4 baseColor : COLOR0, float2 coords : TEXCOORD0) : COLOR0
{
    float2 center = ((coords * 2) - 1);
    float power = (1 - length(center)) * uStrength;
    float2 polar = float2(atan2(center.y, center.x) / 6.28 * uRepeats, length(center) * 0.05 * uSize);
    float4 crack0 = tex2D(tex0, polar + float2(uTime, uTime * 2));
    float4 crack1 = tex2D(tex1, polar + float2(uTime + 0.5, uTime * 3));
    float effect = length(crack0 + crack1) * 0.6 - length(center + length(crack0 + crack1) * center * 0.1) * 2;

    return smoothstep(-0.1, 0.5, effect) * baseColor;

}

technique Technique1
{
    pass CrackPass
    {
        PixelShader = compile ps_2_0 PixelShaderFunction();
    }
}