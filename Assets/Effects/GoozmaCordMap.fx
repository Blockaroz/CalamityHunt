sampler2D uImage0 : register(s0);
matrix uTransformMatrix;
float uTime;

texture uTexture;
sampler2D tex = sampler_state
{
    texture = <uTexture>;
    magfilter = LINEAR;
    minfilter = LINEAR;
    mipfilter = LINEAR;
    AddressU = wrap;
    AddressV = wrap;
};

texture uMap;
sampler2D map = sampler_state
{
    texture = <uMap>;
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
    float4 img0 = tex2D(tex, input.Coord * float2(3, 1) - float2(uTime, sin(uTime * 6.28) * 0.02));
    float4 img1 = tex2D(tex, input.Coord * float2(2, 1) - float2(uTime * 3, 0));
    float4 flat = (length(img0 * img1) > input.Coord.x ? 1 : 0);
    float4 colors = tex2D(map, float2(flat.r + input.Coord.x * 0.5, 0.33));
    float4 colorsDark = tex2D(map, float2(flat.r + input.Coord.x * 0.5, 0.66));
    
    if (length(flat.rgba) <= 0.0001)
        return 0;
    
    if (flat.g > 0)
        return colorsDark;
    
    return colors;
}

technique Technique1
{
    pass LiquidPass
    {
        PixelShader = compile ps_3_0 PixelShaderFunction();
        VertexShader = compile vs_3_0 VertexShaderFunction();
    }
}