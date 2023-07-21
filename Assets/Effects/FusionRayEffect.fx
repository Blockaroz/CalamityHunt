sampler2D uImage0 : register(s0);
matrix uTransformMatrix;
float uTime;
float uFreq;

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
texture uGlow;
sampler glowTex = sampler_state
{
    texture = <uGlow>;
    magfilter = LINEAR;
    minfilter = LINEAR;
    mipfilter = LINEAR;
    AddressU = wrap;
    AddressV = wrap;
};
texture uBits;
sampler bitsTex = sampler_state
{
    texture = <uBits>;
    magfilter = LINEAR;
    minfilter = LINEAR;
    mipfilter = LINEAR;
    AddressU = wrap;
    AddressV = wrap;
};

struct VertexShaderInput
{
    float2 Coord : TEXCOORD0;
    float4 Position : POSITION0;
    float4 Color : COLOR0;
};

struct VertexShaderOutput
{
    float2 Coord : TEXCOORD0;
    float4 Position : POSITION0;
    float4 Color : COLOR0;
};

VertexShaderOutput VertexShaderFunction(in VertexShaderInput input)
{
    VertexShaderOutput output = (VertexShaderOutput) 0;
    output.Color = input.Color;
    output.Coord = input.Coord;
    output.Position = mul(input.Position, uTransformMatrix);
    return output;
}

float4 PixelShaderFunction(VertexShaderOutput input) : COLOR0
{
    float fadeConst = smoothstep(0.5, 0.0, input.Coord.x - sin(input.Coord.y * 3.1415) * 0.5);
    float4 base = tex2D(tex0, float2(frac(uTime * 3 + input.Coord.x * 1.5 * uFreq), input.Coord.y + sin((uTime * 4 + input.Coord.x * uFreq) * 6.28 - 1.57) * 0.05));
    float4 base2 = tex2D(tex1, float2(frac(uTime * 4 + input.Coord.x * 1.2 * uFreq), input.Coord.y)) * float4(1, 1, 1, 0);
    float4 glow = tex2D(glowTex, float2(frac(uTime * 2 + input.Coord.x), input.Coord.y));
    float4 laser = length(glow * base * base2 * fadeConst) * float4(input.Color.rgb * 0.03, 0.5) + pow(length(base * base2), 2) * input.Color * fadeConst;
    float4 bits = tex2D(bitsTex, float2(frac(uTime * 3 + input.Coord.x * uFreq), frac(input.Coord.y + sin((input.Coord.x * uFreq * 2 + uTime) * 6.28) * 0.01))) * tex2D(bitsTex, float2(frac(uTime * 5 + input.Coord.x * uFreq), frac(input.Coord.y + sin((input.Coord.x * uFreq * 0.5 + uTime * 2) * 6.28) * 0.02))) * float4(input.Color.rgb * 0.05, 0.8);
    
    if (length(bits * (base * (1.5 - fadeConst))) > 0.2)
        return bits * smoothstep(1.0, 0.6, input.Coord.x) + (glow * float4(input.Color.rgb, 0)) * fadeConst;

    if (length(laser) > 0.4)
        return 1;
    
    return laser + (glow * float4(input.Color.rgb, 0)) * fadeConst;
}

technique Technique1
{
    pass ShaderPass
    {
        PixelShader = compile ps_3_0 PixelShaderFunction();
        VertexShader = compile vs_3_0 VertexShaderFunction();
    }
}