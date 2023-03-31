sampler2D uImage0 : register(s0);
float4 uColor;
matrix uTransformMatrix;
float uTime;

texture uTexture;
sampler tex = sampler_state
{
    texture = <uTexture>;
    magfilter = LINEAR;
    minfilter = LINEAR;
    mipfilter = LINEAR;
    AddressU = wrap;
    AddressV = wrap;
};
texture uGlow;
sampler glow = sampler_state
{
    texture = <uGlow>;
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
    float4 base = tex2D(tex, input.Coord + float2(uTime * 2, 0));
    return (base * float4(0, 0, 0, 1) + base * input.Color + tex2D(glow, input.Coord + float2(uTime, 0)) * (0.2 + input.Color * 2)) * uColor;
}

technique Technique1
{
    pass ShaderPass
    {
        PixelShader = compile ps_2_0 PixelShaderFunction();
        VertexShader = compile vs_2_0 VertexShaderFunction();
    }
}