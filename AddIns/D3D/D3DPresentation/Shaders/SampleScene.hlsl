cbuffer PerFrame : register(b0)
{
    float4 Overlay;
};

cbuffer PerFrame : register(b1)
{
    float4x4 WorldViewProj;
};

// Vertex shader input
struct VS_IN
{
    float4 pos : POSITION;
    float4 col : COLOR;
};

// Pixel shader input (also: vertex shader output)
struct PS_IN
{
    float4 pos : SV_POSITION;
    float4 col : COLOR;
};

// Main vertex shader program
PS_IN VS1(VS_IN input)
{
    PS_IN output = (PS_IN) 0;

    output.pos = mul(input.pos, WorldViewProj);
    output.col = input.col * Overlay;
    return output;
}

PS_IN VS2(VS_IN input)
{
    PS_IN output = (PS_IN) 0;

    output.pos = mul(input.pos, WorldViewProj);
    output.col = input.col * Overlay;
    return output;
}


// Main pixel shader program
float4 PS(PS_IN input) : SV_Target
{
    return input.col;
}

