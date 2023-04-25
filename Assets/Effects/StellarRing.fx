sampler2D uImage0 : register(s0);
float4 uColorLight;
float4 uColorAura;
float4 uColorDark;
matrix uTransformMatrix;
float uTime;
float2 uDarkSpecial;

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

float4 PixelShaderFunction(VertexShaderOutput input) : COLOR0
{
    float4 liq = tex2D(tex0, float2(frac(input.Coord.x + uTime * 2), 1 - input.Coord.y));
    float4 liqFast = tex2D(tex0, float2(frac(input.Coord.x + uTime * 3), 1 - input.Coord.y));
    float4 base = (liq * liqFast);   
    float4 dark = base * float4(uColorDark.rgb * uDarkSpecial.x, length(base.rgb) * uDarkSpecial.y);  
    //float4 stars = tex2D(tex1, input.Coord * float2(1, 0.5) + float2(frac(uTime), 0));
    //float4 darkColor = lerp(uColorDark * 0.2, uColorAura, saturate(smoothstep(0.1, 0.3, length(stars.rgb))));
    
    //return (stars * darkColor * (1.2 - pow(stars, 1.5)) + stars * uColorAura + pow(stars, 5) * uColorLight * 0.7);
    
    float glowLine = smoothstep(0.9, 0.95, base.g);
    return dark + base.g * uColorAura * 0.5 + glowLine * uColorLight + base.b * uColorDark * 0.2;
}

technique Technique1
{
    pass ShaderPass
    {
        PixelShader = compile ps_2_0 PixelShaderFunction();
        VertexShader = compile vs_2_0 VertexShaderFunction();
    }
}