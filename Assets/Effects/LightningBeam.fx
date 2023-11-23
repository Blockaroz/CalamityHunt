sampler uImage0 : register(s0);

texture uTexture0;
sampler2D tex0 = sampler_state
{
    texture = <uTexture0>;
    magfilter = LINEAR;
    minfilter = LINEAR;
    mipfilter = LINEAR;
    AddressU = wrap;
    AddressV = wrap;
};
texture uTexture1;
sampler2D tex1 = sampler_state
{
    texture = <uTexture1>;
    magfilter = LINEAR;
    minfilter = LINEAR;
    mipfilter = LINEAR;
    AddressU = wrap;
    AddressV = wrap;
};
float4 uColor;
float uTime;
float uThickness;
float uVaryThickness;
float uLength;
matrix transformMatrix;

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
    VertexShaderOutput output = (VertexShaderOutput)0;
    output.Color = input.Color;
    output.Coord = input.Coord;
    output.Position = mul(input.Position, transformMatrix);
    return output;
}

float4 PixelShaderFunction(in VertexShaderOutput input) : COLOR0
{
    float noise1 = length(tex2D(tex0, float2(input.Coord.x * uLength * 0.2, 0.3) + float2(-uTime, 0)).rgb) / 3 - 0.5;
    float noise2 = length(tex2D(tex1, float2(input.Coord.x * uLength * 0.2, 0.3) + float2(-uTime * 2, 0) + noise1 * 0.5).rgb) / 3;
    
    return smoothstep(0.95, 1, 1 - abs(input.Coord.y * 2 - 1));

}

technique Technique1
{
    pass EnergyPass
    {
        VertexShader = compile vs_2_0 VertexShaderFunction();
        PixelShader = compile ps_2_0 PixelShaderFunction();
    }
}