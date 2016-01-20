/////////////////////////////////////////////////////////////////////////////
//    Altaxo:  a data processing and data plotting program
//    Copyright (C) 2002-2016 Dr. Dirk Lellinger
//
//    This program is free software; you can redistribute it and/or modify
//    it under the terms of the GNU General Public License as published by
//    the Free Software Foundation; either version 2 of the License, or
//    (at your option) any later version.
//
//    This program is distributed in the hope that it will be useful,
//    but WITHOUT ANY WARRANTY; without even the implied warranty of
//    MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
//    GNU General Public License for more details.
//
//    You should have received a copy of the GNU General Public License
//    along with this program; if not, write to the Free Software
//    Foundation, Inc., 675 Mass Ave, Cambridge, MA 02139, USA.
//
/////////////////////////////////////////////////////////////////////////////

float4x4 WorldViewProj;

cbuffer cbViewTransformation
{
	float3 EyePosition;
}

cbuffer cbMaterial
{
	float4 MaterialDiffuseColor;
	float specExp;
	float specIntensity;
}

cbuffer cbLights
{
	// Light positions of the 4 lights, every variable is one component; x, y, z, and w correspond to the 4 lights
	float4 LightPosX;
	float4 LightPosY;
	float4 LightPosZ;

	// Light directions
	float4 LightDirX;
	float4 LightDirY;
	float4 LightDirZ;

	float4 LightColorR;
	float4 LightColorG;
	float4 LightColorB;

	float4 LightRangeRcp; // reciprocal of light range

	float4 CapsuleLen;

	float4 SpotCosOuterCone;

	float4 SpotCosInnerConeRcp;
}

// Buffer used for clipping operations
// plane0 ... plane5 are the six clip planes
cbuffer cbClipPlanes
{
	float4 plane0;
	float4 plane1;
	float4 plane2;
	float4 plane3;
	float4 plane4;
	float4 plane5;
}

float4 dot4x4(float4 aX, float4 aY, float4 aZ, float4 bX, float4 bY, float4 bZ)
{
	return aX * bX + aY * bY + aZ * bZ;
}

float4 dot4x1(float4 aX, float4 aY, float4 aZ, float3 b)
{
	return aX * b.xxxx + aY * b.yyyy + aZ * b.zzzz;
}

float4 CalcLighting(float3 position, float3 normal, float4 diffuseColor)
{
	float3 ToEye = EyePosition.xyz - position;
	// Find the shortest distance between the pixel and capsules segment
	float4 ToCapsuleStartX = position.xxxx - LightPosX;
	float4 ToCapsuleStartY = position.yyyy - LightPosY;
	float4 ToCapsuleStartZ = position.zzzz - LightPosZ;
	float4 DistOnLine = dot4x4(ToCapsuleStartX, ToCapsuleStartY, ToCapsuleStartZ, LightDirX, LightDirY, LightDirZ);
	float4 CapsuleLenSafe = max(CapsuleLen, 1.e-6);
	DistOnLine = CapsuleLen * saturate(DistOnLine / CapsuleLenSafe);
	float4 PointOnLineX = LightPosX + LightDirX * DistOnLine;
	float4 PointOnLineY = LightPosY + LightDirY * DistOnLine;
	float4 PointOnLineZ = LightPosZ + LightDirZ * DistOnLine;
	float4 ToLightX = PointOnLineX - position.xxxx;
	float4 ToLightY = PointOnLineY - position.yyyy;
	float4 ToLightZ = PointOnLineZ - position.zzzz;
	float4 DistToLightSqr = dot4x4(ToLightX, ToLightY, ToLightZ, ToLightX, ToLightY, ToLightZ);
	float4 DistToLight = sqrt(DistToLightSqr);

	// Phong diffuse
	ToLightX /= DistToLight; // Normalize
	ToLightY /= DistToLight; // Normalize
	ToLightZ /= DistToLight; // Normalize
	float4 NDotL = saturate(dot4x1(ToLightX, ToLightY, ToLightZ, normal));
	//float3 finalColor = float3(dot(LightColorR, NDotL),	dot(LightColorG, NDotL), dot(LightColorB, NDotL));

	// Blinn specular
	ToEye = normalize(ToEye);
	float4 HalfWayX = ToEye.xxxx + ToLightX;
	float4 HalfWayY = ToEye.yyyy + ToLightY;
	float4 HalfWayZ = ToEye.zzzz + ToLightZ;
	float4 HalfWaySize = sqrt(dot4x4(HalfWayX, HalfWayY, HalfWayZ, HalfWayX, HalfWayY, HalfWayZ));
	float4 NDotH = saturate(dot4x1(HalfWayX / HalfWaySize, HalfWayY / HalfWaySize, HalfWayZ / HalfWaySize, normal));
	float4 SpecValue = pow(NDotH, specExp.xxxx) * specIntensity;
	//finalColor += float3(dot(LightColorR, SpecValue), dot(LightColorG, SpecValue), dot(LightColorB, SpecValue));

	// Cone attenuation
	float4 cosAng = dot4x4(LightDirX, LightDirY, LightDirZ, ToLightX, ToLightY, ToLightZ);
	float4 conAtt = saturate((cosAng - SpotCosOuterCone) * SpotCosInnerConeRcp);
	conAtt *= conAtt;

	// Attenuation
	float4 DistToLightNorm = 1.0 - saturate(DistToLight * LightRangeRcp);
	float4 Attn = DistToLightNorm * DistToLightNorm;
	Attn *= conAtt; // Include the cone attenuation

	// Calculate the final color value
	float4 pixelIntensity = (NDotL + SpecValue) * Attn;
	float3 finalColor = float3(dot(LightColorR, pixelIntensity), dot(LightColorG, pixelIntensity), dot(LightColorB, pixelIntensity));
	finalColor *= diffuseColor;
	return float4(finalColor, diffuseColor.w);
}

struct VS_IN_P
{
	float4 pos : POSITION;
};

struct VS_IN_PC
{
	float4 pos : POSITION;
	float4 col : COLOR;
};

struct VS_IN_PT
{
	float4 pos : POSITION;
	float2 uv : TEXCOORD0;
};

struct VS_IN_PN
{
	float4 pos : POSITION;
	float4 nml : NORMAL;
};

struct VS_IN_PNC
{
	float4 pos : POSITION;
	float4 nml : NORMAL;
	float4 col : COLOR;
};

struct VS_IN_PNT
{
	float4 pos : POSITION;
	float4 nml : NORMAL;
	float2 uv : TEXCOORD0;
};

struct PS_IN
{
	float4 pos : SV_POSITION;
	float3 posW : POSITION;
	float3 normal : NORMAL;
	float4 col : COLOR;
	float4 clip0 : SV_ClipDistance0;
	float2 clip1 : SV_ClipDistance1;
};

PS_IN VS_P(VS_IN_P input)
{
	PS_IN output = (PS_IN)0;

	output.pos = mul(input.pos, WorldViewProj);
	output.posW = input.pos;
	output.normal = float3(0, 0, 1);
	output.col = MaterialDiffuseColor;

	return output;
}

PS_IN VS_PC(VS_IN_PC input)
{
	PS_IN output = (PS_IN)0;

	output.pos = mul(input.pos, WorldViewProj);
	output.posW = input.pos;
	output.normal = float3(0, 0, 1);
	output.col = input.col;
	return output;
}

PS_IN VS_PT(VS_IN_PT input)
{
	PS_IN output = (PS_IN)0;

	output.pos = mul(input.pos, WorldViewProj);
	output.posW = input.pos;
	output.normal = float3(0, 0, 1);
	output.col = float4(0, 0, 0, 1);

	return output;
}

PS_IN VS_PN(VS_IN_PN input)
{
	PS_IN output = (PS_IN)0;
	output.pos = mul(input.pos, WorldViewProj);
	output.posW = input.pos.xyz;
	output.normal = input.nml;
	output.col = MaterialDiffuseColor;

	return output;
}

PS_IN VS_PNC(VS_IN_PNC input)
{
	PS_IN output = (PS_IN)0;
	output.pos = mul(input.pos, WorldViewProj);
	output.posW = input.pos.xyz;
	output.normal = input.nml;
	output.col = input.col;

	output.clip0 = float4(dot(input.pos, plane0), dot(input.pos, plane1), dot(input.pos, plane2), dot(input.pos, plane3));
	output.clip1 = float2(dot(input.pos, plane4), dot(input.pos, plane5));

	return output;
}

PS_IN VS_PNT(VS_IN_PNT input)
{
	PS_IN output = (PS_IN)0;

	output.pos = mul(input.pos, WorldViewProj);
	output.posW = input.pos.xyz;
	output.normal = input.nml;
	output.col = float4(0, 0, 0, 1);

	return output;
}

float4 PS(PS_IN input) : SV_Target
{
	float4 col1 = float4(1,0,0,1);
	float4 col2 = float4(0, 1, 0, 1);
	float3 normal = normalize(input.normal);

	return lerp(col1, col2, 0.5*(normal.z + 1)); // downwards = col1 (red); upwards = col2 (green)
}

/*
float4 PS(PS_IN input) : SV_Target
{
	return CalcLighting(input.posW, input.normal, input.col);
}
*/

technique10 Shade_P
{
	pass P0
	{
		SetGeometryShader(0);
		SetVertexShader(CompileShader(vs_4_0, VS_P()));
		SetPixelShader(CompileShader(ps_4_0, PS()));
	}
}

technique10 Shade_PC
{
	pass P0
	{
		SetGeometryShader(0);
		SetVertexShader(CompileShader(vs_4_0, VS_PC()));
		SetPixelShader(CompileShader(ps_4_0, PS()));
	}
}

technique10 Shade_PT
{
	pass P0
	{
		SetGeometryShader(0);
		SetVertexShader(CompileShader(vs_4_0, VS_PT()));
		SetPixelShader(CompileShader(ps_4_0, PS()));
	}
}

technique10 Shade_PN
{
	pass P0
	{
		SetGeometryShader(0);
		SetVertexShader(CompileShader(vs_4_0, VS_PN()));
		SetPixelShader(CompileShader(ps_4_0, PS()));
	}
}

technique10 Shade_PNC
{
	pass P0
	{
		SetGeometryShader(0);
		SetVertexShader(CompileShader(vs_4_0, VS_PNC()));
		SetPixelShader(CompileShader(ps_4_0, PS()));
	}
}

technique10 Shade_PNT
{
	pass P0
	{
		SetGeometryShader(0);
		SetVertexShader(CompileShader(vs_4_0, VS_PNT()));
		SetPixelShader(CompileShader(ps_4_0, PS()));
	}
}