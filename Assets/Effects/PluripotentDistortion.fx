sampler uImage0 : register(s0);
sampler uImage1 : register(s1);
sampler uImage2 : register(s2);
sampler uImage3 : register(s3);
float3 uColor;
float3 uSecondaryColor;
float2 uScreenResolution;
float2 uScreenPosition;
float2 uTargetPosition;
float2 uDirection;
float uOpacity;
float uTime;
float uIntensity;
float uProgress;
float2 uImageSize1;
float2 uImageSize2;
float2 uImageSize3;
float2 uImageOffset;
float uSaturation;
float4 uSourceRect;
float2 uZoom;

texture distortionSample0;
sampler2D dSample0 = sampler_state
{
    texture = <distortionSample0>;
    magfilter = LINEAR;
    minfilter = LINEAR;
    mipfilter = LINEAR;
    AddressU = wrap;
    AddressV = wrap;
};
texture distortionSample1;
sampler2D dSample1 = sampler_state
{
    texture = <distortionSample1>;
    magfilter = LINEAR;
    minfilter = LINEAR;
    mipfilter = LINEAR;
    AddressU = wrap;
    AddressV = wrap;
};

float2 distortSize;

float4 PixelShaderFunction(float4 baseColor : COLOR0, float2 coords : TEXCOORD0) : COLOR0
{
    float2 targetCoords = (uTargetPosition - uScreenPosition) / uScreenResolution;
    float2 center = ((coords - targetCoords)) * (uScreenResolution / uScreenResolution.y) / uZoom;    

    float2 polar = float2(atan2(center.y, center.x) / (3.1415), length(center) * 0.2 * distortSize.y);

    float dist0 = tex2D(dSample0, uScreenPosition * 0.000001 + (polar / 2 + float2(polar.y * 0.2 + uProgress, -uProgress * 4)));
    float dist1 = tex2D(dSample1, uScreenPosition * 0.000001 + (polar + float2(-polar.y * 0.2, -uProgress * 3)) + dist0 * 0.1);
    
    float distortion = (dist1 - 0.5) * uOpacity * smoothstep(0.3, 0.9, length(center));
    float2 direction = center;
    return tex2D(uImage0, coords + distortion * direction);

}

technique Technique1
{
    pass DistortionPass
    {
        PixelShader = compile ps_2_0 PixelShaderFunction();
    }
}