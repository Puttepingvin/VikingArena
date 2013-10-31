float4x4 View;
float4x4 Projection;
float4 AmbientColor = float4(1, 1, 1, 1);

float spriteWidth;
float spriteHeight;
float3 spritePos;
float spriteRotation;

float shinyness;
float3 cameraPos;

float3 lightPos;
float4 lightColor = float4(1, 1, 1, 1);;
float lightPower = 20;
texture2D NormalMap;
texture2D ColorMap;
sampler2D ColorMapSampler = sampler_state
{
   Texture = <ColorMap>;
};

sampler2D NormalMapSampler = sampler_state
{
   Texture = <NormalMap>;
};

struct VertexShaderInput
{
   float4 Position : POSITION0;
   float2 TexCoord : TEXCOORD0;
};

struct VertexShaderOutput
{
   float4 Position : POSITION0;
   float2 TexCoord : TEXCOORD0;
};

VertexShaderOutput VertexShaderFunction(VertexShaderInput input)
{
    VertexShaderOutput output;

    float4 viewPosition = mul(input.Position, View);
    output.Position = mul(viewPosition, Projection);
	output.TexCoord = input.TexCoord;
    return output;
}

float4 PixelShaderFunction(VertexShaderOutput input) : COLOR0
{
   float4 basecolor = tex2D(ColorMapSampler, input.TexCoord);
   float3 normal = tex2D(NormalMapSampler, input.TexCoord);
   
   float3 pixelPosition = float3(input.TexCoord.x*spriteWidth-spriteWidth/2, -(input.TexCoord.y*spriteHeight-spriteHeight/2), 0);
   float newx = pixelPosition.x*cos(spriteRotation) - pixelPosition.y*sin(spriteRotation);
   float newy = pixelPosition.x*sin(spriteRotation) + pixelPosition.y*cos(spriteRotation);
   pixelPosition.x = newx + spritePos.x;
   pixelPosition.y = newy + spritePos.y;
   
   float3 direction = lightPos - pixelPosition;
   float distance = 1 / length(lightPos - pixelPosition) * lightPower;
   float amount = normalize(max(dot(normal, normalize(distance)), 0));
	// float3 ref =  reflect( direction, N );

   return basecolor*AmbientColor*0.2 + basecolor*lightColor*distance*amount; //+ basecolor*lightColor*distance;
   //I=Ai*Ac+Di*Dc*N.L+Si*Sc*(R.V)n
}

technique Technique1
{
    pass Pass1
    {
        VertexShader = compile vs_2_0 VertexShaderFunction();
        PixelShader = compile ps_2_0 PixelShaderFunction();
    }
}