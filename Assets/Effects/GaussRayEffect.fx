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
    float4 base = tex2D(tex0, float2(frac(uTime * 2 + input.Coord.x * 1.4 * uFreq), input.Coord.y + sin((uTime * 4 + input.Coord.x * uFreq) * 6.28 - 1.57) * 0.05)) * float4(1, 1, 1, 0);
    float4 base2 = tex2D(tex1, float2(frac(uTime * 3 + input.Coord.x * uFreq), input.Coord.y));
    float4 glow = tex2D(glowTex, float2(frac(uTime + input.Coord.x), input.Coord.y));
    float4 laser = length(glow * base * base2) * float4(input.Color.rgb * 0.02, 0.8) + pow(length(base * base2), 2) * input.Color + pow(base * base2, 5);
    float4 bits = tex2D(bitsTex, float2(frac(uTime * 2 + input.Coord.x * uFreq * 1.1), input.Coord.y * 2)) * tex2D(bitsTex, float2(frac(uTime * 3 + input.Coord.x * uFreq), input.Coord.y)) * float4(input.Color.rgb * 0.05, 0.8);
    if (length(bits) > 0.4)
        return laser * 0.4 + bits + glow * input.Color * 0.4;
    return laser + glow * input.Color * 0.8;

}

technique Technique1
{
    pass ShaderPass
    {
        PixelShader = compile ps_2_0 PixelShaderFunction();
        VertexShader = compile vs_2_0 VertexShaderFunction();
    }
}