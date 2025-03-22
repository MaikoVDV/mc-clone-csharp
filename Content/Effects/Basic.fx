float4x4 World;
float4x4 View;
float4x4 Projection;

float4 AmbientColor;
float AmbientIntensity;

texture ModelTexture;
sampler2D textureSampler = sampler_state
{
    Texture = (ModelTexture);
    MagFilter = Point;
    MinFilter = Point;
    AddressU = Clamp;
    AddressV = Clamp;
};

struct VertexShaderInput
{
    float4 Position : POSITION0;
    float2 TextureCoordinate : TEXCOORD0;
    float MaterialId : TEXCOORD1;
};

struct VertexShaderOutput
{
    float4 Position : SV_POSITION;
    float2 TextureCoordinate : TEXCOORD0;
    float MaterialId : TEXCOORD1;
};

VertexShaderOutput VertexShaderFunction(VertexShaderInput input)
{
    VertexShaderOutput output;

    float4 worldPosition = mul(input.Position, World);
    float4 viewPosition = mul(worldPosition, View);
    
    output.Position = mul(viewPosition, Projection);
    output.TextureCoordinate = input.TextureCoordinate;
    output.MaterialId = input.MaterialId;

    return output;
}

float4 PixelShaderFunction(VertexShaderOutput input) : SV_Target
{
    float4 textureColor = tex2D(textureSampler, input.TextureCoordinate);
    float4 result = saturate(textureColor + AmbientColor * AmbientIntensity);
    
    int materialId = (int)round(input.MaterialId);
    if (input.MaterialId == 1.0)
    {
        result.a = 0.5;
    }
    
    return result;
}

technique Diffuse
{
    pass Pass1
    {
        AlphaBlendEnable = TRUE;
        DestBlend = INVSRCALPHA;
        SrcBlend = SRCALPHA;

        VertexShader = compile vs_2_0 VertexShaderFunction();
        PixelShader = compile ps_2_0 PixelShaderFunction();
    }
}