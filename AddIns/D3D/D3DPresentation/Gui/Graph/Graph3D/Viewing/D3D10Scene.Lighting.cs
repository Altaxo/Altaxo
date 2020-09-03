#region Copyright

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

#endregion Copyright

namespace Altaxo.Gui.Graph.Graph3D.Viewing
{
  using System;
  using System.Collections.Generic;
  using System.Linq;
  using Altaxo.Geometry;
  using Altaxo.Graph.Graph3D;
  using Altaxo.Graph.Graph3D.Camera;
  using Altaxo.Graph.Graph3D.GraphicsContext.D3D;
  using Altaxo.Graph.Graph3D.Lighting;
  using Altaxo.Gui.Graph.Graph3D.Common;
  using Drawing.D3D;
  using SharpDX;
  using SharpDX.D3DCompiler;
  using SharpDX.Direct3D10;
  using SharpDX.DXGI;
  using Buffer = SharpDX.Direct3D10.Buffer;
  using Device = SharpDX.Direct3D10.Device;

  public partial class D3D10Scene : ID3D10Scene
  {
    private class Lighting
    {
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

      public EffectConstantBuffer _cbLighting;

      public EffectVectorVariable HemisphericLightColorBelow;
      public EffectVectorVariable HemisphericLightColorAbove;
      public EffectVectorVariable HemisphericLightBelowToAboveVector;

      public EffectVectorVariable LightPosX;
      public EffectVectorVariable LightPosY;
      public EffectVectorVariable LightPosZ;

      // Light directions
      public EffectVectorVariable LightDirX;

      public EffectVectorVariable LightDirY;
      public EffectVectorVariable LightDirZ;

      public EffectVectorVariable LightColorR;
      public EffectVectorVariable LightColorG;
      public EffectVectorVariable LightColorB;

      public EffectVectorVariable LightRangeRcp; // reciprocal of light range

      public EffectVectorVariable CapsuleLen;

      public EffectVectorVariable SpotCosOuterCone;

      public EffectVectorVariable SpotCosInnerConeRcp;



      private SingleLight[] _singleLights;


      public Lighting(Effect effect)
      {
        _singleLights = new SingleLight[4];

        _cbLighting = effect.GetConstantBufferByName("cbLights");
        HemisphericLightColorBelow = _cbLighting.GetMemberByName("HemisphericLightColorBelow").AsVector();
        HemisphericLightColorAbove = _cbLighting.GetMemberByName("HemisphericLightColorAbove").AsVector();
        HemisphericLightBelowToAboveVector = _cbLighting.GetMemberByName("HemisphericLightBelowToAboveVector").AsVector();

        LightPosX = _cbLighting.GetMemberByName("LightPosX").AsVector();
        LightPosY = _cbLighting.GetMemberByName("LightPosY").AsVector();
        LightPosZ = _cbLighting.GetMemberByName("LightPosZ").AsVector();

        LightDirX = _cbLighting.GetMemberByName("LightDirX").AsVector();
        LightDirY = _cbLighting.GetMemberByName("LightDirY").AsVector();
        LightDirZ = _cbLighting.GetMemberByName("LightDirZ").AsVector();

        LightColorR = _cbLighting.GetMemberByName("LightColorR").AsVector();
        LightColorG = _cbLighting.GetMemberByName("LightColorG").AsVector();
        LightColorB = _cbLighting.GetMemberByName("LightColorB").AsVector();

        LightRangeRcp = _cbLighting.GetMemberByName("LightRangeRcp").AsVector();
        CapsuleLen = _cbLighting.GetMemberByName("CapsuleLen").AsVector();
        SpotCosOuterCone = _cbLighting.GetMemberByName("SpotCosOuterCone").AsVector();
        SpotCosInnerConeRcp = _cbLighting.GetMemberByName("SpotCosInnerConeRcp").AsVector();
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
          Disposer.RemoveAndDispose(ref SpotCosInnerConeRcp);
          Disposer.RemoveAndDispose(ref SpotCosOuterCone);
          Disposer.RemoveAndDispose(ref CapsuleLen);
          Disposer.RemoveAndDispose(ref LightRangeRcp);
          Disposer.RemoveAndDispose(ref LightColorB);
          Disposer.RemoveAndDispose(ref LightColorG);
          Disposer.RemoveAndDispose(ref LightColorR);
          Disposer.RemoveAndDispose(ref LightDirZ);
          Disposer.RemoveAndDispose(ref LightDirY);
          Disposer.RemoveAndDispose(ref LightDirX);
          Disposer.RemoveAndDispose(ref LightPosZ);
          Disposer.RemoveAndDispose(ref LightPosY);
          Disposer.RemoveAndDispose(ref LightPosX);
          Disposer.RemoveAndDispose(ref HemisphericLightBelowToAboveVector);
          Disposer.RemoveAndDispose(ref HemisphericLightColorAbove);
          Disposer.RemoveAndDispose(ref HemisphericLightColorBelow);
          Disposer.RemoveAndDispose(ref _cbLighting);

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
        HemisphericLightBelowToAboveVector.Set(new Vector4(0, 0, 1, 1));
        HemisphericLightColorBelow.Set(0.1f * new Vector4(0.55f, 0.5f, 0.5f, 1)); // slightly red
        HemisphericLightColorAbove.Set(new Vector4(0.5f, 0.5f, 0.55f, 1)); // slightly blue

        ClearSingleLight(0);
        ClearSingleLight(1);
        ClearSingleLight(2);
        ClearSingleLight(3);

        SetDirectionalLight(0, Altaxo.Drawing.NamedColors.White.Color, 0.5, new VectorD3D(-2, -1, 1));
        SetPointLight(1, Altaxo.Drawing.NamedColors.White.Color, 0.5, new PointD3D(200, 200, 200), 400);
        SetCapsuleLight(2, Altaxo.Drawing.NamedColors.Red, 1, new PointD3D(400, 200, 200), 500, new VectorD3D(0, 1, 0), 200);

        AssembleLights();
      }

      public void SetLighting(LightSettings lightSettings, CameraBase camera)
      {
        Matrix4x3 cameraM = Matrix4x3.Identity;
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

        AssembleLights();
      }

      public void SetAmbientLight(Altaxo.Drawing.AxoColor colorBelow, Altaxo.Drawing.AxoColor colorAbove, double lightAmplitude, VectorD3D directionBelowToAbove)
      {
        directionBelowToAbove = directionBelowToAbove.Normalized;
        HemisphericLightBelowToAboveVector.Set(new Vector4((float)directionBelowToAbove.X, (float)directionBelowToAbove.Y, (float)directionBelowToAbove.Z, 0));
        HemisphericLightColorBelow.Set(ToVector4(colorBelow, lightAmplitude, 1));
        HemisphericLightColorAbove.Set(ToVector4(colorAbove, lightAmplitude, 1));
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
          Color = ToVector4(color, colorAmplitude, 1),
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

      private void AssembleLights()
      {
        LightPosX.Set(new Vector4(_singleLights[0].Position.X, _singleLights[1].Position.X, _singleLights[2].Position.X, _singleLights[3].Position.X));
        LightPosY.Set(new Vector4(_singleLights[0].Position.Y, _singleLights[1].Position.Y, _singleLights[2].Position.Y, _singleLights[3].Position.Y));
        LightPosZ.Set(new Vector4(_singleLights[0].Position.Z, _singleLights[1].Position.Z, _singleLights[2].Position.Z, _singleLights[3].Position.Z));

        LightDirX.Set(new Vector4(_singleLights[0].Direction.X, _singleLights[1].Direction.X, _singleLights[2].Direction.X, _singleLights[3].Direction.X));
        LightDirY.Set(new Vector4(_singleLights[0].Direction.Y, _singleLights[1].Direction.Y, _singleLights[2].Direction.Y, _singleLights[3].Direction.Y));
        LightDirZ.Set(new Vector4(_singleLights[0].Direction.Z, _singleLights[1].Direction.Z, _singleLights[2].Direction.Z, _singleLights[3].Direction.Z));

        LightColorR.Set(new Vector4(_singleLights[0].Color.X, _singleLights[1].Color.X, _singleLights[2].Color.X, _singleLights[3].Color.X));
        LightColorG.Set(new Vector4(_singleLights[0].Color.Y, _singleLights[1].Color.Y, _singleLights[2].Color.Y, _singleLights[3].Color.Y));
        LightColorB.Set(new Vector4(_singleLights[0].Color.Z, _singleLights[1].Color.Z, _singleLights[2].Color.Z, _singleLights[3].Color.Z));

        LightRangeRcp.Set(new Vector4(_singleLights[0].LightRangeRcp, _singleLights[1].LightRangeRcp, _singleLights[2].LightRangeRcp, _singleLights[3].LightRangeRcp));
        CapsuleLen.Set(new Vector4(_singleLights[0].CapsuleLength, _singleLights[1].CapsuleLength, _singleLights[2].CapsuleLength, _singleLights[3].CapsuleLength));
        SpotCosInnerConeRcp.Set(new Vector4(_singleLights[0].SpotCosInnerConeRcp, _singleLights[1].SpotCosInnerConeRcp, _singleLights[2].SpotCosInnerConeRcp, _singleLights[3].SpotCosInnerConeRcp));
        SpotCosOuterCone.Set(new Vector4(_singleLights[0].SpotCosOuterCone, _singleLights[1].SpotCosOuterCone, _singleLights[2].SpotCosOuterCone, _singleLights[3].SpotCosOuterCone));
      }
    }
  }
}
