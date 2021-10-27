/////////////////////////////////////////////////////////////////////////////
//    Altaxo:  a data processing and data plotting program
//    Copyright (C) 2002-2021 Dr. Dirk Lellinger
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

Texture2D ShaderTexture : register(t7);

// just take the default sampler
SamplerState SampleType
{
  Filter = MIN_MAG_MIP_LINEAR;
};

struct VS_IN
{
  float4 pos : POSITION;
  float2 col : TEXCOORD0;
};

struct PS_IN
{
  float4 pos : SV_POSITION;
  float2 col : TEXCOORD0;
};

PS_IN VS(VS_IN input)
{
  PS_IN output = (PS_IN)0;

  output.pos = input.pos;
  output.col = input.col;
  return output;
}

// Pixel shader to correct the gamma values
// of the output
// the power that is used is 1/2.2 =
float4 PS(PS_IN input) : SV_Target
{
  float4 col = ShaderTexture.Sample(SampleType, input.col);
  col.rgb = pow(col.rgb, 0.4545454545f);
  return col;
}
