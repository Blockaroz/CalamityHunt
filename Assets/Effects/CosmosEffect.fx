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
float2 uScrollClose;
float2 uScrollFar;
float4 uCloseColor;
float4 uFarColor;
float4 uOutlineColor;
float2 uImageSize;
float uNoiseRepeats;
float uZoom;

float2 resize(float2 coords, float2 offset)
{
    return ((coords * uImageSize) + offset) / uImageSize;
}

float EdgeColor(float4 screen, float2 coords)
{
    float edges[8];
    
    float imageRatio = uImageSize.x / uImageSize.y;
    float noise = length(tex2D(texN, uPosition * 0.05 + uScrollFar + float2(coords.x * 0.5 * imageRatio, coords.y * 0.5)).rgb) / 1.5 - 1;
    float noise2 = length(tex2D(texN, uPosition * 0.05 + uScrollClose + float2(coords.x * 0.5 * imageRatio, coords.y * 0.5 + uTime) + noise * 0.1).rgb) + 1;

    edges[0] = length(tex2D(uImage0, resize(coords, float2(noise2, 0))));
    edges[1] = length(tex2D(uImage0, resize(coords, float2(0, noise2))));
    edges[2] = length(tex2D(uImage0, resize(coords, float2(-noise2, 0))));
    edges[3] = length(tex2D(uImage0, resize(coords, float2(0, -noise2))));
    
    return smoothstep(0, 0.01, edges[0] + edges[1] + edges[2] + edges[3]) * smoothstep(0.01, 0, length(screen.rgba) - noise2 * 0.01);
}

float4 PixelShaderFunction(float4 baseColor : COLOR0, float2 coords : TEXCOORD0) : COLOR0
{  
    float4 screen = tex2D(uImage0, coords);
    float imageRatio = uImageSize.x / uImageSize.y;
    float2 center = (coords - 0.5) * imageRatio / uZoom;

    float2 realZoom = float2(uZoom, uZoom);
    
    float noise = length(tex2D(texN, uPosition * 0.025 + uScrollFar + float2(center.x * imageRatio, center.y) * uNoiseRepeats).rgb) / 3 - 0.5;
    float noise2 = length(tex2D(texN, uPosition * 0.05 + uScrollClose + float2(center.x * imageRatio, center.y - uTime * 2) * uNoiseRepeats + noise * 0.5).rgb) / 3 - 0.5;
    
    float spaceFar = length(tex2D(texF, uPosition * 0.05 + uScrollFar + float2(center.x * imageRatio, center.y) + noise2 * 0.5).rgb) / 1.5;
    float spaceClose = length(tex2D(texC, uPosition * 0.25 + uScrollClose + float2(center.x * imageRatio, center.y) + noise2 * 0.1).rgb) / 1.5;
    
    float edge = EdgeColor(screen, coords);
    float4 result = float4(((spaceFar + spaceClose * 2 + edge) * uFarColor + pow(spaceClose, 2) * uCloseColor).rgb, 1);
    
    return result * smoothstep(0, 0.0001, length(screen.rgba)) + edge * uOutlineColor * (0.3f + smoothstep(0, 0.2, noise + 0.1) * 0.6);
}

technique Technique1
{
    pass PixelShaderPass
    {
        PixelShader = compile ps_3_0 PixelShaderFunction();
    }
}