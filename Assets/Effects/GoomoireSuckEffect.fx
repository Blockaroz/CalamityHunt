sampler2D uImage0 : register(s0);
matrix uTransformMatrix;
float uTime;
float uFreq;
float uMiddleBrightness;
float uBackPhaseShift;
float uSlant;

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

float sineCoord(float x)
{
    if (x > 0.5)
        return 1 - sqrt(2 - x * 2);
    else
        return sqrt(x * 2);
}

float4 PixelShaderFunction(VertexShaderOutput input) : COLOR0
{
    float fadeConst = sin(input.Coord.y * 3.1415) * 0.5 + 0.5 - smoothstep(0.5, 1.0, input.Coord.x);
    float4 scrollingBits = tex2D(tex1, float2(frac(input.Coord.x * 0.66 * uFreq + uTime * 3), frac(sineCoord(input.Coord.y) + input.Coord.x * uSlant + uTime * 2)));
    float4 scrollingBitsUnder = tex2D(tex1, float2(frac(input.Coord.x * 0.66 * uFreq + uTime * 3 + uBackPhaseShift), frac(1 - sineCoord(input.Coord.y) + input.Coord.x * uSlant + uTime * 2)));
    float glow = length(sqrt(tex2D(tex0, float2(input.Coord.x * uFreq + uTime * 3, input.Coord.y + uTime * 2)))) / 3;
    float4 core = (smoothstep(0.05, 0.08, glow / 3 * fadeConst) * input.Color + glow) * fadeConst * uMiddleBrightness;
      
    if (input.Coord.y > 0.1 && input.Coord.y < 0.9)
    {
        if (length(scrollingBits.rgb) / 3 > 0.4 && scrollingBits.a > 0.1)
            return input.Color * 1.5 * fadeConst + core;
    
        if (length(scrollingBitsUnder.rgb * fadeConst) / 3 > 0)
            return float4(input.Color.rgb * 0.4, input.Color.a + 0.5) * fadeConst * scrollingBitsUnder + core;
    }
    return core;
}

technique Technique1
{
    pass ShaderPass
    {
        PixelShader = compile ps_3_0 PixelShaderFunction();
        VertexShader = compile vs_3_0 VertexShaderFunction();
    }
}