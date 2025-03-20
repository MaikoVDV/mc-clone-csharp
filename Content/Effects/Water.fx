float4x4 World;
float4x4 View;
float4x4 Projection;

float4 AmbientColor;
float AmbientIntensity;

float Opacity;

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
    //float4 Normal : NORMAL0;
    float2 TextureCoordinate : TEXCOORD0;
};

struct VertexShaderOutput
{
    float4 Position : SV_POSITION;
    //float4 Color : COLOR0;
    float2 TextureCoordinate : TEXCOORD0;
};

VertexShaderOutput VertexShaderFunction(VertexShaderInput input)
{
    VertexShaderOutput output;

    float4 worldPosition = mul(input.Position, World);
    float4 viewPosition = mul(worldPosition, View);
    output.Position = mul(viewPosition, Projection);
    
    output.TextureCoordinate = input.TextureCoordinate;
    
    //float4 normal = mul(input.Normal, WorldInverseTranspose);
    //float lightIntensity = dot(normal, DiffuseLightDirection);
    //output.Color = saturate(DiffuseColor * DiffuseIntensity * lightIntensity);

    return output;
}

float4 PixelShaderFunction(VertexShaderOutput input) : SV_Target
{
    float4 textureColor = tex2D(textureSampler, input.TextureCoordinate);
    //textureColor.a = 0.5;
    float4 output = saturate(textureColor + AmbientColor * AmbientIntensity);
    output.a = Opacity;
    return output;
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