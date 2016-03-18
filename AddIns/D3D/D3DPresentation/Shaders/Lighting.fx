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

// Texture which stores color values for colorization of data mesh plot style
Texture1D ColorGradient1DTexture;

SamplerState ColorGradient1DTextureSampler
{
	Filter = MIN_MAG_MIP_LINEAR;
	AddressU = Clamp;
};

cbuffer cbViewTransformation
{
	float4x4 WorldViewProj;
	float3 EyePosition;
}

cbuffer cbMaterial
{
	float4 MaterialDiffuseColor;
	float MaterialSpecularExponent;
	float MaterialSpecularIntensity;

	// Mixing coefficient for specular reflection: value between 0 and 1
	// if 0, the reflected specular light is multiplied with the material diffuse color
	// if 1, the reflected specular light has the same color as the incident light (thus as if it is reflected at a white surface)
	float MaterialSpecularMixingCoefficient;
}

cbuffer cbLights
{
	float3 HemisphericLightColorBelow;
	float3 HemisphericLightColorAbove;
	float3 HemisphericLightBelowToAboveVector;

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
// ClipPlane0 ... ClipPlane5 are the six clip planes
cbuffer cbClipPlanes
{
	float4 ClipPlane0;
	float4 ClipPlane1;
	float4 ClipPlane2;
	float4 ClipPlane3;
	float4 ClipPlane4;
	float4 ClipPlane5;
}

/////////////////////
// BLENDING STATES //
/////////////////////
BlendState AlphaBlendingOn
{
	BlendEnable[0] = TRUE;
	DestBlend = INV_SRC_ALPHA;
	SrcBlend = SRC_ALPHA;
};

float4 dot4x4(float4 aX, float4 aY, float4 aZ, float4 bX, float4 bY, float4 bZ)
{
	return aX * bX + aY * bY + aZ * bZ;
}

float4 dot4x1(float4 aX, float4 aY, float4 aZ, float3 b)
{
	return aX * b.xxxx + aY * b.yyyy + aZ * b.zzzz;
}

// Credits: Doron Feinstein, HLSL Development Cookbook, Packt Publishing, 2013, page 32
// dlellinger: The code in the book was augmented with hemispheric ambient lighting described in the same book on page 7
// dlellinger: I modified the code to use Phong specular instead of Blinn specular
// dlellinger: There is an error in the book: SpotCosInnerConeRcp is not 1/Cos(InnerConeAngle), but is 1/(Cos(InnerConeAngle)-Cos(OuterConeAngle))
// dlellinger: I modified the light range code: if the distance is <= lightRange, the light amplitude is constant = LightAmplitude, at distances > lightRange, the light amplitude is LightAmplitude * (LightRange/Distance)^2. This is closer to physics.
float4 CalcLighting(float3 position, float3 normal, float4 diffuseColor)
{
	normal = normalize(normal);
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

	ToEye = normalize(ToEye);
	/*
	// Blinn specular
	float4 HalfWayX = ToEye.xxxx + ToLightX;
	float4 HalfWayY = ToEye.yyyy + ToLightY;
	float4 HalfWayZ = ToEye.zzzz + ToLightZ;
	float4 HalfWaySize = sqrt(dot4x4(HalfWayX, HalfWayY, HalfWayZ, HalfWayX, HalfWayY, HalfWayZ));
	// dlellinger: saturate(NDotL*1e9) is not in the Feinstein book. I included it to make sure light is only reflected if the face normal points to the light.
	float4 NDotH = saturate(NDotL*1e9)*saturate(dot4x1(HalfWayX / HalfWaySize, HalfWayY / HalfWaySize, HalfWayZ / HalfWaySize, normal));
	float4 SpecValue = pow(NDotH, MaterialSpecularExponent.xxxx) * MaterialSpecularIntensity;
	//finalColor += float3(dot(LightColorR, SpecValue), dot(LightColorG, SpecValue), dot(LightColorB, SpecValue));
	*/

	// dlellinger: instead of Blinn specular, I use here Phong specular. This is not much more expensive and gives far better results
	// Phong specular
	float4 ReflectedX = 2 * normal.xxxx*NDotL - ToLightX;
	float4 ReflectedY = 2 * normal.yyyy*NDotL - ToLightY;
	float4 ReflectedZ = 2 * normal.zzzz*NDotL - ToLightZ;
	float4 NDotH = saturate(NDotL*1e9)*saturate(dot4x1(ReflectedX, ReflectedY, ReflectedZ, ToEye)); // saturate(NDotL*1e9) is here to switch very fast between 0 and 1 (0 if the light comes from the back side, 1 if it shines on the front side). In this way it is ensured the light intensity is zero if coming from the wrong side
	float4 SpecValue = pow(NDotH, MaterialSpecularExponent.xxxx) * MaterialSpecularIntensity;

	// Cone attenuation
	float4 cosAng = dot4x4(LightDirX, LightDirY, LightDirZ, ToLightX, ToLightY, ToLightZ);
	float4 conAtt = saturate((cosAng - SpotCosOuterCone) * SpotCosInnerConeRcp);
	conAtt *= conAtt;

	// Attenuation
	float4 DistToLightNorm = saturate(1.0 / (DistToLight * LightRangeRcp));
	float4 Attn = DistToLightNorm * DistToLightNorm;
	Attn *= conAtt; // Include the cone attenuation

	// Hemispheric ambient lighting
	float r = 0.5*(1 + dot(normal, HemisphericLightBelowToAboveVector));
	float3 finalColor = lerp(HemisphericLightColorBelow, HemisphericLightColorAbove, r);

	// Calculate the final color value
	float4 pixelIntensity = (NDotL + (1 - MaterialSpecularMixingCoefficient)*SpecValue) * Attn;
	finalColor += float3(dot(LightColorR, pixelIntensity), dot(LightColorG, pixelIntensity), dot(LightColorB, pixelIntensity));
	finalColor *= diffuseColor;

	// here comes the part that is specularly reflected with only the light color (but not the material color).
	float4 pixelIntensity2 = SpecValue * Attn * MaterialSpecularMixingCoefficient;
	finalColor += float3(dot(LightColorR, pixelIntensity2), dot(LightColorG, pixelIntensity2), dot(LightColorB, pixelIntensity2));

	return float4(finalColor, diffuseColor.w);
}

// ------------------------ Vertex shader input structures ---------------------------------------------

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

// Vertex input intended for DataMeshPlotStyle
struct VS_IN_PNT1
{
	float3 pos : POSITION;
	float3 nml : NORMAL;
	float2 uv : TEXCOORD0;
};

// ------------------------ End of vertex shader input structures ---------------------------------------------

// --------------- Scene Pixel shader input structures ------------------------------------------------

// Input to the pixel shader for the scene
struct PS_IN
{
	float4 pos : SV_POSITION; // position in camera coordinates
	float3 posW : POSITION; // position in world coordinates
	float3 normal : NORMAL; // normal vector in world coordinates
	float4 col : COLOR;  // pixel color
	float4 clip0 : SV_ClipDistance0; // clip distances 0..3
	float2 clip1 : SV_ClipDistance1; // clip distances 4..5
};

// Input for the pixel shader. Instead of a color, we provide a 1D texture coordinate, that
// will be lookuped in ColorGradient1DTexture. This is intended for rendering DataMeshPlotStyle.
struct PS_IN_T1
{
	float4 pos : SV_POSITION; // position in camera coordinates
	float3 posW : POSITION; // position in world coordinates
	float3 normal : NORMAL; // normal vector in world coordinates
	float2  uv : TEXCOORD0;  // pixel color
	float4 clip0 : SV_ClipDistance0; // clip distances 0..3
	float2 clip1 : SV_ClipDistance1; // clip distances 4..5
};

// --------------- End of Scene Pixel shader input structures ------------------------------------------------

// --------------- Overlay pixel shader input structures ------------------------------------------------

// Input to the pixel shader for the overlay (the markers). We don't need normals here.
struct PS_OVERLAY_IN
{
	float4 pos : SV_POSITION; // position in camera coordinates
	float4 col : COLOR;  // pixel color
};

// --------------- End of overlay pixel shader input structures ------------------------------------------------

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

	output.clip0 = float4(dot(input.pos, ClipPlane0), dot(input.pos, ClipPlane1), dot(input.pos, ClipPlane2), dot(input.pos, ClipPlane3));
	output.clip1 = float2(dot(input.pos, ClipPlane4), dot(input.pos, ClipPlane5));

	return output;
}

// Vertex shader intended for rendering data mesh plot style. Input is position, normal and a 1D texture coordinate.
// The color for this texture coordinate is looked up from ColorGradient1DTexture;
PS_IN_T1 VS_PNT1(VS_IN_PNT1 input)
{
	PS_IN_T1 output = (PS_IN_T1)0;
	float4 pos = float4(input.pos, 1);
	output.pos = mul(pos, WorldViewProj);
	output.posW = input.pos;
	output.normal = input.nml;
	output.uv = input.uv; // the 1D texture coordinate

	output.clip0 = float4(dot(pos, ClipPlane0), dot(pos, ClipPlane1), dot(pos, ClipPlane2), dot(pos, ClipPlane3));
	output.clip1 = float2(dot(pos, ClipPlane4), dot(pos, ClipPlane5));

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

/*
// Lighting based on the normal only - the color of the object is completely ignored
float4 PS(PS_IN input) : SV_Target
{
	float r = 0.5*(1 + dot(normalize(input.normal), HemisphericLightBelowToAboveVector));
	return float4(lerp(HemisphericLightColorBelow, HemisphericLightColorAbove, r), input.col.w);
}
*/

/*
// Lighting based on the normal and the color - but only for hemispheric light
float4 PS(PS_IN input) : SV_Target
{
	float r = 0.5*(1 + dot(normalize(input.normal), HemisphericLightBelowToAboveVector));
float3 materialColor = input.col.xyz;
return float4(materialColor*lerp(HemisphericLightColorBelow, HemisphericLightColorAbove, r), input.col.w);
}
*/

// --------------------  Scene rendering pixel shaders --------------------------------------

float4 PS(PS_IN input) : SV_Target
{
	return CalcLighting(input.posW, input.normal, input.col);
}

float4 PS_T1(PS_IN_T1 input) : SV_Target
{
	float4 col = ColorGradient1DTexture.Sample(ColorGradient1DTextureSampler, input.uv.x); // First texture coordinate is used to sample the ColorProvider's color
	float v = saturate(input.uv.y); // The second component of the texture coordinates is normally 0. It is only then a very high value if the point it is invalid
	col = lerp(col, MaterialDiffuseColor, v); // MaterialDiffuseColor is used here to represent the ColorProvider's InvalidColor
	clip(col.a < 0.1 ? -1 : 1); // don't show transparent values
	return CalcLighting(input.posW, input.normal, col);
}

// ---------------------End of scene rendering pixel shaders --------------------------------

// --------------------  Overlay rendering pixel shaders --------------------------------------

PS_OVERLAY_IN VS_OVERLAY_PC(VS_IN_PC input)
{
	PS_OVERLAY_IN output = (PS_OVERLAY_IN)0;

	output.pos = mul(input.pos, WorldViewProj);
	output.col = input.col;
	return output;
}

float4 PS_OVERLAY(PS_OVERLAY_IN input) : SV_Target
{
	return input.col;
}

// ---------------------End of overlay rendering pixel shaders --------------------------------

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
		SetVertexShader(CompileShader(vs_4_0, VS_OVERLAY_PC()));
		SetPixelShader(CompileShader(ps_4_0, PS_OVERLAY()));
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
		// SetBlendState(AlphaBlendingOn, float4(0.0f, 0.0f, 0.0f, 0.0f), 0xFFFFFFFF); // uncomment this to enable alpha blending (seems not to be useful in DX10)
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

technique10 Shade_PNT1
{
	pass P0
	{
		SetGeometryShader(0);
		SetVertexShader(CompileShader(vs_4_0, VS_PNT1()));
		SetPixelShader(CompileShader(ps_4_0, PS_T1()));
	}
}