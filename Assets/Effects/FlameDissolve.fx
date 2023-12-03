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
float2 uTextureScale;
float uFrameCount;
float uProgress;
float uPower;
float uNoiseStrength;

float4 PixelShaderFunction(float4 baseColor : COLOR0, float2 coords : TEXCOORD0) : COLOR0
{
    float4 original = tex2D(uImage0, coords);
    float noiseCoord = length(tex2D(tex0, float2(coords.x * uTextureScale.x + uProgress / 3, coords.y * uTextureScale.y * uFrameCount + uProgress * 1.5)).rgb) / 3 - 0.5;
    float4 noise = tex2D(tex0, float2(coords.x * uTextureScale.x, coords.y * uTextureScale.y * uFrameCount + uProgress / 1.5) + noiseCoord * 0.05) * smoothstep(0.1, 0.3, original);
    float power = pow(original * (1 + noise * uNoiseStrength * 0.33 - uProgress * (0.5f + uNoiseStrength)), uPower);
    return clamp(power, 0, 1) * baseColor;
}
technique Technique1
{
    pass PixelShaderPass
    {
        PixelShader = compile ps_2_0 PixelShaderFunction();
    }
}