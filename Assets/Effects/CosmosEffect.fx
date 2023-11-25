sampler2D uImage0 : register(s0);
texture uTextureNoise;
sampler texN = sampler_state
{
    texture = <uTextureNoise>;
    magfilter = LINEAR;
    minfilter = LINEAR;
    mipfilter = LINEAR;
    AddressU = wrap;
    AddressV = wrap;
};
texture uTextureClose;
sampler texC = sampler_state
{
    texture = <uTextureClose>;
    magfilter = LINEAR;
    minfilter = LINEAR;
    mipfilter = LINEAR;
    AddressU = wrap;
    AddressV = wrap;
};
texture uTextureFar;
sampler texF = sampler_state
{
    texture = <uTextureFar>;
    magfilter = LINEAR;
    minfilter = LINEAR;
    mipfilter = LINEAR;
    AddressU = wrap;
    AddressV = wrap;
};
float uTime;
float2 uPosition;
float2 uParallax;
float2 uScrollClose;
float2 uScrollFar;
float4 uCloseColor;
float4 uFarColor;
float4 uOutlineColor;
float2 uImageSize;
float uNoiseRepeats;

float4 PixelShaderFunction(float4 baseColor : COLOR0, float2 coords : TEXCOORD0) : COLOR0
{  
    float4 screen = tex2D(uImage0, coords);
    float imageRatio = uImageSize.x / uImageSize.y;
    float noise = length(tex2D(texN, float2(coords.x * imageRatio, coords.y + uTime) * uNoiseRepeats).rgb) / 3 - 0.5;
    float noise2 = length(tex2D(texN, uPosition * 0.05 + float2(coords.x * imageRatio, coords.y) * uNoiseRepeats + noise).rgb) / 3 - 0.5;
    float spaceClose = length(tex2D(texC, uPosition * 0.25 + uScrollClose + float2(coords.x * imageRatio, coords.y) + noise2 * 0.2).rgb) / 1.5;
    float spaceFar = length(tex2D(texF, uPosition * 0.15 + uScrollFar + float2(coords.x * imageRatio, coords.y) + noise2 * 0.4).rgb) / 1.5;
    
    float4 result = float4((spaceFar * uFarColor + pow(spaceClose, 1.5) * uCloseColor).rgb, 1);
    return (result + uOutlineColor * smoothstep(0, 0.02, length(screen.rgb) / 2 - screen.a)) * smoothstep(0.5, 1, screen.a);
}

technique Technique1
{
    pass PixelShaderPass
    {
        PixelShader = compile ps_2_0 PixelShaderFunction();
    }
}