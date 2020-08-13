float4x4 World;
float4x4 View;
float4x4 Projection;
float4x4 WorldInverseTranspose;

float3 ViewVector = float3(1, 0, 0);


Texture2D ModelTexture;
sampler2D textureSampler = sampler_state {
    Texture = (ModelTexture);
    MinFilter = Linear;
    MagFilter = Linear;
    AddressU = Clamp;
    AddressV = Clamp;
};

//Texture2D NormalsMap;
//sampler2D normalSampler = sampler_state {
//    Texture = (NormalsMap);
//    MinFilter = Linear;
//    MagFilter = Linear;
//    AddressU = Clamp;
//    AddressV = Clamp;
//};

struct VertexShaderInput
{
    float4 Position : SV_POSITION;
    float4 Normal : NORMAL0;
    float2 TextureCoordinate : TEXCOORD0;
};

struct VertexShaderOutput
{
    float4 Position : SV_POSITION;
    float4 Color : COLOR0;
    float3 Normal : TEXCOORD0;
    float2 TextureCoordinate : TEXCOORD1;
};

VertexShaderOutput VertexShaderFunction(VertexShaderInput input)
{
    VertexShaderOutput output;

    float4 worldPosition = mul(input.Position, World);
    float4 viewPosition = mul(worldPosition, View);
    output.Position = mul(viewPosition, Projection);

    float4 normal = normalize(mul(input.Normal, WorldInverseTranspose));
    output.Color = float4(1, 1, 1, 1);

    output.Normal = float3(1, 1, 1); 

    output.TextureCoordinate = input.TextureCoordinate;
    return output;
}

float4 PixelShaderFunction(VertexShaderOutput input) : SV_TARGET0
{
    float3 normal = normalize(input.Normal);
    float3 r = normalize(normal);
    float3 v = normalize(mul(float4(normalize(ViewVector),0), World)).xyz;
    float dotProduct = dot(r, v);

    float4 textureColor = tex2D(textureSampler, input.TextureCoordinate);
    textureColor.rgb *= textureColor.a;
    //textureColor = tex2D(normalSampler, input.TextureCoordinate);
    return saturate(textureColor * input.Color);
}

technique Basic2
{
    pass Pass1
    {
        VertexShader = compile vs_3_0 VertexShaderFunction();
        PixelShader = compile ps_3_0 PixelShaderFunction();
    }
}