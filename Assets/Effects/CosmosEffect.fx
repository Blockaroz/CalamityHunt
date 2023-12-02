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

float2 resize(float2 coords, float2 offset)
{
    return ((coords * uImageSize) + offset) / uImageSize;
}

float EdgeColor(float4 screen, float2 coords)
{
    float edges[4];
    
    edges[0] = length(tex2D(uImage0, resize(coords, float2(0, 2))).rgba) / 3;
    edges[1] = length(tex2D(uImage0, resize(coords, float2(0, -2))).rgba) / 3;
    edges[2] = length(tex2D(uImage0, resize(coords, float2(2, 0))).rgba) / 3;
    edges[3] = length(tex2D(uImage0, resize(coords, float2(-2, 0))).rgba) / 3;
    
    return smoothstep(0, 0.002, edges[0] + edges[1] + edges[2] + edges[3]) * smoothstep(0.01, 0, length(screen.rgba));

}

float4 PixelShaderFunction(float4 baseColor : COLOR0, float2 coords : TEXCOORD0) : COLOR0
{  
    float4 screen = tex2D(uImage0, coords);
    float imageRatio = uImageSize.x / uImageSize.y;
    float noise = length(tex2D(texN, uPosition * 0.025 + uScrollFar + float2(coords.x * imageRatio, coords.y) * uNoiseRepeats).rgb) / 3 - 0.5;
    float noise2 = length(tex2D(texN, uPosition * 0.05 + uScrollClose + float2(coords.x * imageRatio, coords.y - uTime * 2) * uNoiseRepeats + noise * 0.5).rgb) / 3 - 0.5;
    float spaceClose = length(tex2D(texC, uPosition * 0.25 + uScrollClose + float2(coords.x * imageRatio, coords.y) + noise2 * 0.05).rgb) / 1.5;
    float spaceFar = length(tex2D(texF, uPosition * 0.15 + uScrollFar + float2(coords.x * imageRatio, coords.y) + noise2 * 0.4).rgb) / 1.5;
    
    float edge = EdgeColor(screen, coords);
    float4 result = float4(((spaceFar + spaceClose * 2 + edge) * uFarColor + pow(spaceClose, 2) * uCloseColor).rgb, 1);
    
    return result * smoothstep(0, 0.0001, length(screen.rgba)) + edge * uOutlineColor;
}

technique Technique1
{
    pass PixelShaderPass
    {
        PixelShader = compile ps_3_0 PixelShaderFunction();
    }
}