sampler2D uImage0 : register(s0);
matrix uTransformMatrix;
float uTime;

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
texture uColorMap;
sampler2D tex1 = sampler_state
{
    texture = <uColorMap>;
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

float brightnesses[10];
float3 colors[10];
float baseToScreenPercent;
float baseToMapPercent;
float maxBright;

float4 Screen(float4 base, float4 overlay)
{
    return 1 - ((1 - (base)) * (1 - (overlay)));
}

float3 GradientMap(float brightness, int gradientSegments, float segmentBrightness[10], float3 segmentColors[10])
{
    for (int i = 1; i < gradientSegments; i++)
    {
        float currentBrightness = segmentBrightness[i];
        
        if (currentBrightness >= brightness)
        {
            if (currentBrightness == brightness)
            {
                return segmentColors[i];
            }
            
            float previousBrightness = segmentBrightness[i - 1];
            float segmentLenght = currentBrightness - previousBrightness;
            float segmentProgress = (brightness - previousBrightness) / segmentLenght;
            
            return lerp(segmentColors[i - 1], segmentColors[i], segmentProgress);
        }
    }
    
    return segmentColors[gradientSegments - 1];
}

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
    float4 img0 = tex2D(tex0, input.Coord * float2(3, 1) - float2(uTime, sin(uTime * 6.28) * 0.01));
    float4 img1 = tex2D(tex0, input.Coord * float2(2, 1) - float2(uTime * 3, 0));
    float4 flat = (length(img0 * img1) > input.Coord.x ? 1 : 0);
    float4 color = tex2D(tex1, float2(clamp(input.Coord.x - (img0 * img1).r * 0.2, 0.01, 1), 0.2));
    float4 colorDark = tex2D(tex1, float2(clamp(input.Coord.x - (img0 * img1).r * 0.2, 0.01, 1), 0.8));
    float4 map = float4(GradientMap((length(color.rgb / maxBright) / 3), 10, brightnesses, colors), 1);
    float4 mapDark = float4(GradientMap((length(colorDark.rgb / maxBright) / 3), 10, brightnesses, colors), 1);
    if ((img0 * img1).b > 0)
        return flat * lerp(lerp(color, map, baseToMapPercent), Screen(lerp(color, map, baseToMapPercent), map), baseToScreenPercent);
    else
        return flat * lerp(lerp(colorDark, mapDark, baseToMapPercent), Screen(lerp(colorDark, mapDark, baseToMapPercent), mapDark), baseToScreenPercent);
}

technique Technique1
{
    pass LiquidPass
    {
        PixelShader = compile ps_3_0 PixelShaderFunction();
        VertexShader = compile vs_3_0 VertexShaderFunction();
    }
}