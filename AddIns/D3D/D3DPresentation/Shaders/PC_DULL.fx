// Copyright (c) 2010-2013 SharpDX - Alexandre Mutel
//
// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:
//
// The above copyright notice and this permission notice shall be included in
// all copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
// THE SOFTWARE.
struct VS_IN
{
	float4 pos : POSITION;
	float4 col : COLOR;
};

struct PS_IN
{
	float4 pos : SV_POSITION;
	float4 col : COLOR;
	float4 clip0 : SV_ClipDistance0;
	float2 clip1 : SV_ClipDistance1;
};

cbuffer sceneBuffer
{
	float4x4 worldViewProj;
}

cbuffer clipBuffer
{
	float4 plane0;
	float4 plane1;
	float4 plane2;
	float4 plane3;
	float4 plane4;
	float4 plane5;
}

PS_IN VS(VS_IN input)
{
	PS_IN output = (PS_IN)0;

	output.pos = mul(input.pos, worldViewProj);
	output.col = input.col;
	output.clip0 = float4(dot(input.pos, plane0), dot(input.pos, plane1), dot(input.pos, plane2), dot(input.pos, plane3));
	output.clip1 = float2(dot(input.pos, plane4), dot(input.pos, plane5));
	return output;
}

float4 PS(PS_IN input) : SV_Target
{
	return input.col;
}