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

namespace Altaxo.Gui.Graph.Graph3D.Viewing
{
  using System;
  using System.Numerics;
  using Altaxo.Geometry;
  using Altaxo.Graph.Graph3D;
  using Altaxo.Graph.Graph3D.Camera;
  using Altaxo.Graph.Graph3D.Lighting;
  using Altaxo.Gui.Graph.Graph3D.Common;
  using Altaxo.Shaders;
  using Vortice.Mathematics;
  
  public partial class D3D11Scene : ID3D11Scene
  {
    private class Lighting : IDisposable
    {
      Vector4 HemisphericLightBelowToAboveVector;
      Color4 HemisphericLightColorBelow;
      Color4 HemisphericLightColorAbove;

      private struct SingleLight
      {
        public Vector4 Color; // unused if Color == 0
        public Vector4 Position;
        public Vector4 Direction;
        public float LightRangeRcp;
        public float CapsuleLength;
        public float SpotCosOuterCone;
        public float SpotCosInnerConeRcp;
      }

      private SingleLight[] _singleLights;

      public Lighting()
      {
        _singleLights = new SingleLight[4];
      }

      #region IDisposable Support

      private bool _isDisposed = false; // To detect redundant calls

      ~Lighting()
      {
        // Do not change this code. Put cleanup code in Dispose(bool disposing) above.
        Dispose(false);
      }

      protected virtual void Dispose(bool disposing)
      {
        if (!_isDisposed)
        {
          _isDisposed = true;
        }
      }

      public void Dispose()
      {
        Dispose(true);
        GC.SuppressFinalize(this);
      }
      #endregion


      public void SetDefaultLighting()
      {
        HemisphericLightBelowToAboveVector = new Vector4(0, 0, 1, 1);
        HemisphericLightColorBelow = new Color4(0.055f, 0.05f, 0.05f, 1); // slightly red
        HemisphericLightColorAbove = new Color4(0.5f, 0.5f, 0.55f, 1); // slightly blue

        ClearSingleLight(0);
        ClearSingleLight(1);
        ClearSingleLight(2);
        ClearSingleLight(3);

        SetDirectionalLight(0, Altaxo.Drawing.NamedColors.White.Color, 0.5, new VectorD3D(-2, -1, 1));
        SetPointLight(1, Altaxo.Drawing.NamedColors.White.Color, 0.5, new PointD3D(200, 200, 200), 400);
        SetCapsuleLight(2, Altaxo.Drawing.NamedColors.Red, 1, new PointD3D(400, 200, 200), 500, new VectorD3D(0, 1, 0), 200);
      }

      public void SetLighting(LightSettings lightSettings, CameraBase camera)
      {
        var cameraM = new Altaxo.Geometry.Matrix4x3(1, 0, 0, 0, 1, 0, 0, 0, 1, 0, 0, 0);
        if (lightSettings.IsAnyLightAffixedToCamera)
        {
          // if a light is affixed to the camera, its position is considered to be in camera coordinates
          // but here we need the light in world coordinates
          // cameraM transforms from camera coordinates to world coordinates
          cameraM = camera.InverseLookAtRHMatrix;
        }

        // first ambient light
        var al = lightSettings.AmbientLight;
        SetAmbientLight(al.ColorBelow.Color, al.ColorAbove.Color, al.LightAmplitude, al.IsAffixedToCamera ? cameraM.Transform(al.DirectionBelowToAbove) : al.DirectionBelowToAbove);

        for (int idx = 0; idx < 4; ++idx)
        {
          var l = lightSettings.GetDiscreteLight(idx);
          if (l is null)
          {
            ClearSingleLight(idx);
          }
          else if (l is DirectionalLight)
          {
            var dl = (DirectionalLight)l;
            SetDirectionalLight(
              idx,
              dl.Color.Color,
              dl.LightAmplitude,
              dl.IsAffixedToCamera ? cameraM.Transform(dl.DirectionToLight) : dl.DirectionToLight
              );
          }
          else if (l is PointLight)
          {
            var pl = (PointLight)l;
            SetPointLight(
              idx,
              pl.Color.Color,
              pl.LightAmplitude,
              pl.IsAffixedToCamera ? cameraM.Transform(pl.Position) : pl.Position,
              pl.Range
              );
          }
          else if (l is SpotLight)
          {
            var sl = (SpotLight)l;

            // calculation of SpotCosInnerConeRcp: it is in reality not 1/CosInnerConeAngle, but it is  1/(Cos(InnerConeAngle) - Cos(OuterConeAngle))
            double diffCos = Math.Cos(sl.InnerConeAngle) - Math.Cos(sl.OuterConeAngle);
            double SpotCosInnerConeRcp = diffCos >= 1E-18 ? 1 / diffCos : 1E18;

            SetSpotLight(
              idx,
              sl.Color.Color,
              sl.LightAmplitude,
              sl.IsAffixedToCamera ? cameraM.Transform(sl.Position) : sl.Position,
              sl.IsAffixedToCamera ? cameraM.Transform(sl.DirectionToLight) : sl.DirectionToLight,
              sl.Range,
              Math.Cos(sl.OuterConeAngle),
              SpotCosInnerConeRcp
              );
          }
          else
          {
            throw new NotImplementedException($"The type of lighting ({l?.GetType()}) is not implemented here.");
          }
        }
      }

      public void SetAmbientLight(Altaxo.Drawing.AxoColor colorBelow, Altaxo.Drawing.AxoColor colorAbove, double lightAmplitude, VectorD3D directionBelowToAbove)
      {
        directionBelowToAbove = directionBelowToAbove.Normalized;
        HemisphericLightBelowToAboveVector = new Vector4((float)directionBelowToAbove.X, (float)directionBelowToAbove.Y, (float)directionBelowToAbove.Z, 0);
        HemisphericLightColorBelow = ToColor4(colorBelow, lightAmplitude, 1);
        HemisphericLightColorAbove = ToColor4(colorAbove, lightAmplitude, 1);
      }

      public void SetDirectionalLight(int idx, Altaxo.Drawing.AxoColor color, double colorAmplitude, VectorD3D directionToLight)
      {
        directionToLight = directionToLight.Normalized;
        SetSingleLight(idx, color, colorAmplitude, (PointD3D)(directionToLight * 1E7), directionToLight, 0, 0, 0, 1);
      }

      public void SetPointLight(int idx, Altaxo.Drawing.AxoColor color, double colorAmplitude, PointD3D position, double range)
      {
        if (range <= 0)
          throw new ArgumentOutOfRangeException(nameof(range));

        SetSingleLight(idx, color, colorAmplitude, position, new VectorD3D(1, 0, 0), 1 / range, 0, -1, 1E18);
      }

      public void SetSpotLight(int idx, Altaxo.Drawing.AxoColor color, double colorAmplitude, PointD3D position, VectorD3D directionToLight, double range, double cosOuterCone, double cosInnerConeRcp)
      {
        if (range <= 0)
          throw new ArgumentOutOfRangeException(nameof(range));

        SetSingleLight(idx, color, colorAmplitude, position, directionToLight, 1 / range, 0, cosOuterCone, cosInnerConeRcp);
      }

      public void SetCapsuleLight(int idx, Altaxo.Drawing.AxoColor color, double colorAmplitude, PointD3D position, double range, VectorD3D capsuleDirection, double capsuleLength)
      {
        if (range <= 0)
          throw new ArgumentOutOfRangeException(nameof(range));

        SetSingleLight(idx, color, colorAmplitude, position, capsuleDirection, 1 / range, capsuleLength, 0, 1);
      }

      private void SetSingleLight(int idx, Altaxo.Drawing.AxoColor color, double colorAmplitude, PointD3D position, VectorD3D direction, double lightRangeRcp, double capsuleLength, double spotCosOuterCone, double spotCosInnerConeRcp)
      {
        var sl = new SingleLight()
        {
          Color = ToColor4(color, colorAmplitude, 1),
          Position = ToVector4(position, 1f),
          Direction = ToVector4(direction, 0f),
          LightRangeRcp = (float)lightRangeRcp,
          CapsuleLength = (float)capsuleLength,
          SpotCosOuterCone = (float)spotCosOuterCone,
          SpotCosInnerConeRcp = (float)spotCosInnerConeRcp
        };

        _singleLights[idx] = sl;
      }

      public void ClearSingleLight(int idx)
      {
        _singleLights[idx] = new SingleLight();
      }

      public void AssembleLightsInto(ref LightingHlsl.CbLights _cbLights)
      {
        _cbLights.HemisphericLightBelowToAboveVector = HemisphericLightBelowToAboveVector;
        _cbLights.HemisphericLightColorBelow = HemisphericLightColorBelow;
        _cbLights.HemisphericLightColorAbove = HemisphericLightColorAbove;

        _cbLights.LightPosX = new Vector4(_singleLights[0].Position.X, _singleLights[1].Position.X, _singleLights[2].Position.X, _singleLights[3].Position.X);
        _cbLights.LightPosY = new Vector4(_singleLights[0].Position.Y, _singleLights[1].Position.Y, _singleLights[2].Position.Y, _singleLights[3].Position.Y);
        _cbLights.LightPosZ = new Vector4(_singleLights[0].Position.Z, _singleLights[1].Position.Z, _singleLights[2].Position.Z, _singleLights[3].Position.Z);

        _cbLights.LightDirX = new Vector4(_singleLights[0].Direction.X, _singleLights[1].Direction.X, _singleLights[2].Direction.X, _singleLights[3].Direction.X);
        _cbLights.LightDirY = new Vector4(_singleLights[0].Direction.Y, _singleLights[1].Direction.Y, _singleLights[2].Direction.Y, _singleLights[3].Direction.Y);
        _cbLights.LightDirZ = new Vector4(_singleLights[0].Direction.Z, _singleLights[1].Direction.Z, _singleLights[2].Direction.Z, _singleLights[3].Direction.Z);

        _cbLights.LightColorR = new Vector4(_singleLights[0].Color.X, _singleLights[1].Color.X, _singleLights[2].Color.X, _singleLights[3].Color.X);
        _cbLights.LightColorG = new Vector4(_singleLights[0].Color.Y, _singleLights[1].Color.Y, _singleLights[2].Color.Y, _singleLights[3].Color.Y);
        _cbLights.LightColorB = new Vector4(_singleLights[0].Color.Z, _singleLights[1].Color.Z, _singleLights[2].Color.Z, _singleLights[3].Color.Z);

        _cbLights.LightRangeRcp = new Vector4(_singleLights[0].LightRangeRcp, _singleLights[1].LightRangeRcp, _singleLights[2].LightRangeRcp, _singleLights[3].LightRangeRcp);
        _cbLights.CapsuleLen = new Vector4(_singleLights[0].CapsuleLength, _singleLights[1].CapsuleLength, _singleLights[2].CapsuleLength, _singleLights[3].CapsuleLength);
        _cbLights.SpotCosInnerConeRcp = new Vector4(_singleLights[0].SpotCosInnerConeRcp, _singleLights[1].SpotCosInnerConeRcp, _singleLights[2].SpotCosInnerConeRcp, _singleLights[3].SpotCosInnerConeRcp);
        _cbLights.SpotCosOuterCone = new Vector4(_singleLights[0].SpotCosOuterCone, _singleLights[1].SpotCosOuterCone, _singleLights[2].SpotCosOuterCone, _singleLights[3].SpotCosOuterCone);
      }
    }
  }
}
