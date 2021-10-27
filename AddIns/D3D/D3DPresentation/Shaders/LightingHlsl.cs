#region Copyright

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

#endregion Copyright

using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using Vortice.Mathematics;

namespace Altaxo.Shaders
{
  /// <summary>
  /// Definitions and structures in C# that correspond to the contents of the Lighting.hlsl file.
  /// </summary>
  public static class LightingHlsl
  {
    public const int ColorGradient1DTexture_RegisterNumber = 0;

    /// <summary>
    /// Register number for the Matrix4x4 buffer containing the world view projection matrix.
    /// </summary>
    public const int WorldViewProj_RegisterNumber = 0;

    /// <summary>
    /// Register number for the <see cref="System.Numerics.Vector4"/> buffer containing the eye position.
    /// </summary>
    public const int EyePosition_RegisterNumber = 1;

    /// <summary>
    /// Register number for the <see cref="CbMaterial"/> buffer containing the material definition.
    /// </summary>
    public const int Material_RegisterNumber = 2;

    /// <summary>
    /// Structure of the buffer containing the material definition.
    /// </summary>
    public struct CbMaterial
    {
      public Color4 DiffuseColor;
      public float SpecularExponent;
      public float SpecularIntensity;
      public float DiffuseIntensity;
      // Metalness value for specular reflection: value between 0 and 1
      // if 0, the reflected specular light has the same color as the incident light (thus as if it is reflected at a white surface)
      // if 1, the reflected specular light is multiplied with the material diffuse color
      public float MetalnessValue;
    }

    /// <summary>
    /// Register number for the <see cref="CbLights"/> buffer containing the lighting definition.
    /// </summary>
    public const int Lights_RegisterNumber = 3;

    /// <summary>
    /// Lights structure that matches the structure in the shader code.
    /// </summary>
    public struct CbLights
    {
      public Color4 HemisphericLightColorBelow;
      public Color4 HemisphericLightColorAbove;
      public Vector4 HemisphericLightBelowToAboveVector;

      // Light positions of the 4 lights, every variable is one component; x, y, z, and w correspond to the 4 lights
      public Vector4 LightPosX;
      public Vector4 LightPosY;
      public Vector4 LightPosZ;

      // Light directions
      public Vector4 LightDirX;
      public Vector4 LightDirY;
      public Vector4 LightDirZ;

      public Vector4 LightColorR;
      public Vector4 LightColorG;
      public Vector4 LightColorB;

      public Vector4 LightRangeRcp; // reciprocal of light range

      public Vector4 CapsuleLen;

      public Vector4 SpotCosOuterCone;

      public Vector4 SpotCosInnerConeRcp;
    }


    /// <summary>
    /// Register number for the <see cref="CbClipPlanes"/> buffer containing clip planes.
    /// </summary>
    public const int ClipPlanes_RegisterNumber = 4;

    /// <summary>
    /// Structure for the 6 clip planes that match the corresponding register in the shader code.
    /// Clip planes are used in vertex shaders only.
    /// </summary>
    public struct CbClipPlanes
    {
      public Plane Plane0;
      public Plane Plane1;
      public Plane Plane2;
      public Plane Plane3;
      public Plane Plane4;
      public Plane Plane5;

      /// <summary>
      /// Gets or sets the <see cref="Plane"/> with the specified index i (0..5).
      /// </summary>
      /// <value>
      /// The <see cref="Plane"/>.
      /// </value>
      /// <param name="i">The index i (0..5).</param>
      /// <returns>The plane at index i.</returns>
      /// <exception cref="System.IndexOutOfRangeException"></exception>
      public Plane this[int i]
      {
        get
        {
          return i switch
          {
            0 => Plane0,
            1 => Plane1,
            2 => Plane2,
            3 => Plane3,
            4 => Plane4,
            5 => Plane5,
            _ => throw new IndexOutOfRangeException()
          };
        }
        set
        {
          switch (i)
          {
            case 0: Plane0 = value; break;
            case 1: Plane1 = value; break;
            case 2: Plane2 = value; break;
            case 3: Plane3 = value; break;
            case 4: Plane4 = value; break;
            case 5: Plane5 = value; break;
            default: throw new IndexOutOfRangeException();
          }
        }
      }

      /// <summary>
      /// Sets all 6 planes to empty planes.
      /// </summary>
      public void Clear()
      {
        Plane0 = Plane1 = Plane2 = Plane3 = Plane4 = Plane5 = new Plane();
      }
    }

  }
}
