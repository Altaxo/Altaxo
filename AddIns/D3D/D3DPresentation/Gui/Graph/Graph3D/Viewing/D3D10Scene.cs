#region Copyright

/////////////////////////////////////////////////////////////////////////////
//    Altaxo:  a data processing and data plotting program
//    Copyright (C) 2002-2019 Dr. Dirk Lellinger
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
  using Altaxo.Drawing;
  using Altaxo.Drawing.D3D;
  using Altaxo.Geometry;
  using Altaxo.Graph.Graph3D;
  using Altaxo.Graph.Graph3D.Camera;
  using Altaxo.Graph.Graph3D.GraphicsContext;
  using Altaxo.Graph.Graph3D.GraphicsContext.D3D;
  using Altaxo.Graph.Graph3D.Lighting;
  using Altaxo.Gui.Graph.Graph3D.Common;
  using SharpDX;
  using SharpDX.D3DCompiler;
  using SharpDX.Direct3D10;
  using SharpDX.DXGI;
  using Buffer = SharpDX.Direct3D10.Buffer;
  using Device = SharpDX.Direct3D10.Device;

  public partial class D3D10Scene : ID3D10Scene
  {
    /// <summary>
    /// The _this triangle buffers. These buffers are used for current rendering
    /// 0: Position, 1: PositionColor, 2: PositionUV, 3: PositionNormal, 4: PositionNormalColor, 5: PositionNormalUV
    /// </summary>
    private List<VertexAndIndexDeviceBuffer>?[] _thisTriangleDeviceBuffers = new List<VertexAndIndexDeviceBuffer>?[7];

    private List<VertexAndIndexDeviceBuffer>?[] _nextTriangleDeviceBuffers = new List<VertexAndIndexDeviceBuffer>?[7];

    private VertexAndIndexDeviceBufferNoMaterial? _markerGeometryTriangleDeviceBuffer;
    private VertexBufferNoMaterial? _markerGeometryLineListBuffer;
    private VertexAndIndexDeviceBufferNoMaterial? _overlayGeometryTriangleDeviceBuffer;
    private VertexBufferNoMaterial? _overlayGeometryLineListBuffer;



    /// <summary>
    /// The cached D3D device. Do not dispose this device when disposing this class!
    /// </summary>
    private Device? _cachedDevice;

    private PointD2D _hostSize;

    protected Buffer? _constantBuffer;

    protected Buffer? _constantBufferForColor;

    protected Buffer? _constantBufferForSixPlanes;

    private int _renderCounter;

    // Meaning: P = PointCoordinates, C = Color, T = TextureCoordinates, N = Normal
    private string[] _layoutNames = new string[7] { "P", "PC", "PT", "PN", "PNC", "PNT", "PNT1" };

    private RenderLayout[] _renderLayouts = new RenderLayout[7];



    // Effect variables

    // Transformation variables
    private EffectConstantBuffer? _cbViewTransformation;

    private EffectMatrixVariable? _evWorldViewProj;
    private EffectVectorVariable? _evEyePosition;

    // Materials
    private EffectConstantBuffer? _cbMaterial;

    private EffectVectorVariable? _evMaterialDiffuseColor;
    private EffectScalarVariable? _evMaterialSpecularExponent;
    private EffectScalarVariable? _evMaterialSpecularIntensity;
    private EffectScalarVariable? _evMaterialDiffuseIntensity;
    private EffectScalarVariable? _evMaterialMetalnessValue;

    // Texture for color providers to colorize a mesh by its height
    private Texture1DDescription _descriptionTextureFor1DColorProvider;

    private Texture1D? _textureFor1DColorProvider;
    private ShaderResourceView? _textureFor1DColorProviderView;
    private EffectVariable? _textureFor1DColorProviderVariable;
    private EffectShaderResourceVariable? _textureFor1DColorProviderShaderResourceVariable;

    // Clip planes
    private EffectConstantBuffer? _cbClipPlanes;

    private EffectVectorVariable[] _evClipPlanes = new EffectVectorVariable[6];

    // Lighting
    private Lighting? _lighting;

    private Effect? _lightingEffect;



    #region Members that do not utilize unmanaged resources, and thus are not associated with a 3D device
    // --------------------------------------------------------------------------------------------------

    private AxoColor? _sceneBackgroundColor;

    /// <summary>
    /// The geometry to render, i.e. the scene itself.
    /// </summary>
    private D3D10GraphicsContext? _altaxoDrawingGeometry;

    /// <summary>
    /// Helper geometry that draws X-Y-Z arrows for better orientation.
    /// </summary>
    private D3D10OverlayContext? _altaxoMarkerGeometry;

    /// <summary>
    /// Geometry that is used temporarily, e.g. to show boxes when objects are moved or selected.
    /// </summary>
    private D3D10OverlayContext? _altaxoOverlayGeometry;

    /// <summary>
    /// The camera associated with the scene
    /// </summary>
    private CameraBase? _altaxoCamera;

    /// <summary>The light settings from AltaxoBase</summary>
    private LightSettings? _altaxoLightSettings;

    #endregion

    public void Attach(SharpDX.ComObject hostDevice, PointD2D hostSize)
    {
      Attach((Device)hostDevice, hostSize);
    }
    /// <summary>
    /// Attaches the scene to the device specified in <paramref name="device"/>, and allocate resources specific to that device.
    /// </summary>
    /// <param name="device">The device to attach to.</param>
    /// <param name="hostSize">The size of the device.</param>
    public void Attach(Device device, PointD2D hostSize)
    {
      if (_cachedDevice is not null)
        throw new InvalidOperationException("Try to attach to device without deattach former device!");

      _cachedDevice = device ?? throw new ArgumentNullException(nameof(device));
      _hostSize = hostSize;

      using (var stream = System.Reflection.Assembly.GetExecutingAssembly().GetManifestResourceStream("Altaxo.CompiledShaders.Effects.Lighting.cso"))
      {
        if (stream is null)
          throw new InvalidOperationException(string.Format("Compiled shader resource not found: {0}", "Altaxo.CompiledShaders.Effects.Lighting.cso"));

        using (var shaderBytes = ShaderBytecode.FromStream(stream))
        {
          _lightingEffect = new Effect(device, shaderBytes);
        }
      }



      for (int i = 0; i < _layoutNames.Length; ++i)
      {
        string techniqueName = "Shade_" + _layoutNames[i];
        var technique = _lightingEffect.GetTechniqueByName(techniqueName);
        if (technique is null || !technique.IsValid)
          throw new InvalidProgramException(string.Format("Technique {0} was not found or is invalid", techniqueName));

        var pass = technique.GetPassByIndex(0);
        if (pass is null || !pass.IsValid)
          throw new InvalidProgramException(string.Format("Pass[0] of technique {0} was not found or is invalid", techniqueName));

        InputLayout inputLayout;
        switch (i)
        {
          case 0:
            inputLayout = new InputLayout(device, pass.Description.Signature, new[] {
                                          new InputElement("POSITION", 0, Format.R32G32B32A32_Float, 0, 0)
                                          });
            break;
          case 1:
            inputLayout = new InputLayout(device, pass.Description.Signature, new[] {
                                          new InputElement("POSITION", 0, Format.R32G32B32A32_Float, 0, 0),
                                          new InputElement("COLOR", 0, Format.R32G32B32A32_Float, 16, 0)
                                          });
            break;
          case 2:
            inputLayout = new InputLayout(device, pass.Description.Signature, new[] {
                                          new InputElement("POSITION", 0, Format.R32G32B32A32_Float, 0, 0),
                                          new InputElement("TEXCOORD", 0, Format.R32G32_Float, 16, 0)
                                          });
            break;
          case 3:
            inputLayout = new InputLayout(device, pass.Description.Signature, new[] {
                                          new InputElement("POSITION", 0, Format.R32G32B32A32_Float, 0, 0),
                                          new InputElement("NORMAL", 0, Format.R32G32B32A32_Float, 16, 0)
                                          });
            break;
          case 4:
            inputLayout = new InputLayout(device, pass.Description.Signature, new[] {
                                          new InputElement("POSITION", 0, Format.R32G32B32A32_Float, 0, 0),
                                          new InputElement("NORMAL", 0, Format.R32G32B32A32_Float, 16, 0),
                                          new InputElement("COLOR", 0, Format.R32G32B32A32_Float, 32, 0)
                                          });
            break;
          case 5:
            inputLayout = new InputLayout(device, pass.Description.Signature, new[] {
                                          new InputElement("POSITION", 0, Format.R32G32B32A32_Float, 0, 0),
                                          new InputElement("NORMAL", 0, Format.R32G32B32A32_Float, 16, 0),
                                          new InputElement("TEXCOORD", 0, Format.R32G32_Float, 32, 0)
                                          });
            break;
          case 6:
            inputLayout = new InputLayout(device, pass.Description.Signature, new[] {
                                          new InputElement("POSITION", 0, Format.R32G32B32_Float, 0, 0),
                                          new InputElement("NORMAL", 0, Format.R32G32B32_Float, 12, 0),
                                          new InputElement("TEXCOORD", 0, Format.R32G32_Float, 24, 0)
                                          });
            break;
          default:
            throw new NotImplementedException();
        }

        _renderLayouts[i] = new RenderLayout(technique, pass, inputLayout);
      }



      // Create Constant Buffers
      //_constantBuffer = new Buffer(device, Utilities.SizeOf<Matrix>(), ResourceUsage.Default, BindFlags.ConstantBuffer, CpuAccessFlags.None, ResourceOptionFlags.None);
      //_constantBufferForColor = new Buffer(device, Utilities.SizeOf<Vector4>(), ResourceUsage.Default, BindFlags.ConstantBuffer, CpuAccessFlags.None, ResourceOptionFlags.None);
      //_constantBufferForSixPlanes = new Buffer(device, Utilities.SizeOf<Vector4>() * 6, ResourceUsage.Default, BindFlags.ConstantBuffer, CpuAccessFlags.None, ResourceOptionFlags.None);

      // View transformation variables
      _cbViewTransformation = _lightingEffect.GetConstantBufferByName("cbViewTransformation");
      _evWorldViewProj = _cbViewTransformation.GetMemberByName("WorldViewProj").AsMatrix();
      _evEyePosition = _cbViewTransformation.GetMemberByName("EyePosition").AsVector();

      _cbMaterial = _lightingEffect.GetConstantBufferByName("cbMaterial");
      _evMaterialDiffuseColor = _cbMaterial.GetMemberByName("MaterialDiffuseColor").AsVector();
      _evMaterialSpecularExponent = _cbMaterial.GetMemberByName("MaterialSpecularExponent").AsScalar();
      _evMaterialSpecularExponent.Set(4.0f);
      _evMaterialSpecularIntensity = _cbMaterial.GetMemberByName("MaterialSpecularIntensity").AsScalar();
      _evMaterialSpecularIntensity.Set(1.0f);
      _evMaterialDiffuseIntensity = _cbMaterial.GetMemberByName("MaterialDiffuseIntensity").AsScalar();
      _evMaterialMetalnessValue = _cbMaterial.GetMemberByName("MaterialMetalnessValue").AsScalar();
      _evMaterialMetalnessValue.Set(0.75f);

      // Color providers
      BindTextureFor1DColorProviders();

      // Clip plane variables
      _cbClipPlanes = _lightingEffect.GetConstantBufferByName("cbClipPlanes");
      for (int i = 0; i < 6; ++i)
      {
        _evClipPlanes[i] = _cbClipPlanes.GetMemberByName("ClipPlane" + i.ToString(System.Globalization.CultureInfo.InvariantCulture)).AsVector();
      }

      // Lighting variables

      _lighting = new Lighting(_lightingEffect);
      _lighting.SetDefaultLighting();

      // -------------------- now draw the scene again with the new attached device --------------

      if (_altaxoCamera is not null && _altaxoLightSettings is not null)
      {
        if (_altaxoDrawingGeometry is not null)
        {
          BringDrawingIntoBuffers(_altaxoDrawingGeometry);
        }

        if (_altaxoMarkerGeometry is not null)
        {
          BringMarkerGeometryIntoDeviceBuffers(_altaxoMarkerGeometry);
        }

        if (_altaxoOverlayGeometry is not null)
        {
          BringOverlayGeometryIntoDeviceBuffers(_altaxoOverlayGeometry);
        }
      }
    }

    /// <summary>
    /// Detaches the scene from the device it was attached to, freeing all resources associated with the formerly attached device.
    /// </summary>
    public void Detach()
    {
      // Dispose all buffers
      if (_nextTriangleDeviceBuffers is not null)
      {
        for (int i = _nextTriangleDeviceBuffers.Length - 1; i >= 0; --i)
        {
          if (_nextTriangleDeviceBuffers[i] is { } nextTriangleDeviceBuffers_i)
          {
            foreach (var ele in nextTriangleDeviceBuffers_i)
            {
              ele.Dispose();
            }
            _nextTriangleDeviceBuffers[i] = null;
          }
        }
      }

      if (_thisTriangleDeviceBuffers is not null)
      {
        for (int i = _thisTriangleDeviceBuffers.Length - 1; i >= 0; --i)
        {
          if (_thisTriangleDeviceBuffers[i] is { } thisTriangleDeviceBuffers_i)
          {
            foreach (var ele in thisTriangleDeviceBuffers_i)
            {
              ele.Dispose();
            }
            _thisTriangleDeviceBuffers[i] = null;
          }
        }
      }

      Disposer.RemoveAndDispose(ref _markerGeometryTriangleDeviceBuffer);
      Disposer.RemoveAndDispose(ref _markerGeometryLineListBuffer);
      Disposer.RemoveAndDispose(ref _overlayGeometryTriangleDeviceBuffer);
      Disposer.RemoveAndDispose(ref _overlayGeometryLineListBuffer);
      // ------- end disposing buffers -----


      // now dispose all other device dependent variables, in inverse order than in Attach()

      Disposer.RemoveAndDispose(ref _lighting);

      for (int i = 0; i < _evClipPlanes.Length; ++i)
      {
        Disposer.RemoveAndDispose(ref _evClipPlanes[i]!);
      }
      Disposer.RemoveAndDispose(ref _cbClipPlanes);
      ReleaseTextureFor1DColorProviders();
      Disposer.RemoveAndDispose(ref _evMaterialMetalnessValue);
      Disposer.RemoveAndDispose(ref _evMaterialDiffuseIntensity);
      Disposer.RemoveAndDispose(ref _evMaterialSpecularIntensity);
      Disposer.RemoveAndDispose(ref _evMaterialSpecularExponent);
      Disposer.RemoveAndDispose(ref _evMaterialDiffuseColor);
      Disposer.RemoveAndDispose(ref _cbMaterial);
      Disposer.RemoveAndDispose(ref _evEyePosition);
      Disposer.RemoveAndDispose(ref _evWorldViewProj);
      Disposer.RemoveAndDispose(ref _cbViewTransformation);
      for (int i = 0; i < _renderLayouts.Length; ++i)
      {
        _renderLayouts[i]?.Dispose();
      }

      Disposer.RemoveAndDispose(ref _lightingEffect);

      _cachedDevice = null;
    }

    #region IDisposable Support

    private bool _isDisposed = false; // To detect redundant calls

    protected virtual void Dispose(bool disposing)
    {
      if (!_isDisposed)
      {
        if (disposing)
        {
          _altaxoDrawingGeometry?.Dispose();
          _altaxoDrawingGeometry = null;
          _altaxoMarkerGeometry?.Dispose();
          _altaxoMarkerGeometry = null;
          _altaxoOverlayGeometry?.Dispose();
          _altaxoOverlayGeometry = null;
          _altaxoCamera = null;
          _altaxoLightSettings = null;
        }

        if (_cachedDevice is not null)
        {
          Detach();
        }

        _isDisposed = true;
      }
    }

    ~D3D10Scene()
    {
      // Do not change this code. Put cleanup code in Dispose(bool disposing) above.
      Dispose(false);
    }

    public void Dispose()
    {
      Dispose(true);
      GC.SuppressFinalize(this);
    }
    #endregion


    public void SetSceneBackColor(AxoColor? sceneBackColor)
    {
      _sceneBackgroundColor = sceneBackColor;
    }

    public AxoColor? SceneBackgroundColor
    {
      get
      {
        return _sceneBackgroundColor;
      }
    }

    internal void SetMarkerGeometry(D3D10OverlayContext markerGeometry)
    {
      _altaxoMarkerGeometry = markerGeometry;
      BringMarkerGeometryIntoDeviceBuffers(markerGeometry);
    }

    public void SetHostSize(PointD2D hostSize)
    {
      _hostSize = hostSize;
    }

    public void SetDrawing(D3D10GraphicsContext drawing)
    {
      _altaxoDrawingGeometry = drawing;
      BringDrawingIntoBuffers(drawing);
    }

    public void SetOverlayGeometry(D3D10OverlayContext overlayGeometry)
    {
      _altaxoOverlayGeometry = overlayGeometry;
      BringOverlayGeometryIntoDeviceBuffers(overlayGeometry);
    }

    public void SetCamera(CameraBase camera)
    {
      if (camera is null)
        throw new ArgumentNullException(nameof(camera));

      _altaxoCamera = camera;
    }

    public void SetLighting(LightSettings lightSettings)
    {
      if (lightSettings is null)
        throw new ArgumentNullException(nameof(lightSettings));

      _altaxoLightSettings = lightSettings;
    }

    private void UseNextTriangleDeviceBuffers()
    {
      for (int i = 0; i < _nextTriangleDeviceBuffers.Length; ++i)
      {
        if (_nextTriangleDeviceBuffers[i] is not null)
        {
          var oldBuffers = _thisTriangleDeviceBuffers[i];
          _thisTriangleDeviceBuffers[i] = System.Threading.Interlocked.Exchange(ref _nextTriangleDeviceBuffers[i], null);

          if (oldBuffers is not null)
          {
            foreach (var entry in oldBuffers)
              entry.Dispose();
          }
        }
      }
    }

#nullable disable

    private void BindTextureFor1DColorProviders()
    {
      _descriptionTextureFor1DColorProvider = new Texture1DDescription()
      {
        ArraySize = 1,
        BindFlags = BindFlags.ShaderResource,
        CpuAccessFlags = CpuAccessFlags.Write,
        Format = Format.R32G32B32A32_Float,
        MipLevels = 1,
        OptionFlags = ResourceOptionFlags.None,
        Usage = ResourceUsage.Dynamic,
        Width = 1024
      };

      _textureFor1DColorProvider = new Texture1D(_cachedDevice, _descriptionTextureFor1DColorProvider);

      _textureFor1DColorProviderView = new ShaderResourceView(_cachedDevice, _textureFor1DColorProvider);

      _textureFor1DColorProviderVariable = _lightingEffect.GetVariableByName("ColorGradient1DTexture");
      _textureFor1DColorProviderShaderResourceVariable = _textureFor1DColorProviderVariable.AsShaderResource();
      _textureFor1DColorProviderShaderResourceVariable.SetResource(_textureFor1DColorProviderView);
    }

    private void ReleaseTextureFor1DColorProviders()
    {
      Disposer.RemoveAndDispose(ref _textureFor1DColorProviderShaderResourceVariable);
      Disposer.RemoveAndDispose(ref _textureFor1DColorProviderVariable);
      Disposer.RemoveAndDispose(ref _textureFor1DColorProviderView);
      Disposer.RemoveAndDispose(ref _textureFor1DColorProvider);
    }

    private void BringDrawingIntoBuffers(D3D10GraphicsContext altaxoDrawingGeometry)
    {
      var device = _cachedDevice;
      if (device is null || device.IsDisposed)
        return;

      var altaxoBuffersOfType =
              new IEnumerable<KeyValuePair<MaterialKey, IndexedTriangleBuffer>>[]
              {
                                altaxoDrawingGeometry.PositionIndexedTriangleBuffersAsIndexedTriangleBuffers,
                                altaxoDrawingGeometry.PositionColorIndexedTriangleBuffersAsIndexedTriangleBuffers,
                                altaxoDrawingGeometry.PositionUVIndexedTriangleBuffersAsIndexedTriangleBuffers,
                                altaxoDrawingGeometry.PositionNormalIndexedTriangleBuffersAsIndexedTriangleBuffers,
                                altaxoDrawingGeometry.PositionNormalColorIndexedTriangleBuffersAsIndexedTriangleBuffers,
                                altaxoDrawingGeometry.PositionNormalUVIndexedTriangleBuffersAsIndexedTriangleBuffers,
                                altaxoDrawingGeometry.PositionNormalUIndexedTriangleBuffersAsIndexedTriangleBuffers
              };

      for (int i = 0; i < altaxoBuffersOfType.Length; ++i)
      {
        var newDeviceBuffers = new List<VertexAndIndexDeviceBuffer>();
        foreach (var altaxoBuffer in altaxoBuffersOfType[i])
        {
          var altaxoTriangleBuffer = altaxoBuffer.Value;
          if (altaxoTriangleBuffer.TriangleCount == 0)
            continue;

          var vertexBuffer = Buffer.Create<float>(device, altaxoTriangleBuffer.VertexStream, new BufferDescription()
          {
            BindFlags = BindFlags.VertexBuffer,
            CpuAccessFlags = CpuAccessFlags.None,
            OptionFlags = ResourceOptionFlags.None,
            SizeInBytes = altaxoTriangleBuffer.VertexStreamLength,
            Usage = ResourceUsage.Default
          });

          var indexBuffer = Buffer.Create<int>(device, altaxoTriangleBuffer.IndexStream, new BufferDescription()
          {
            BindFlags = BindFlags.IndexBuffer,
            CpuAccessFlags = CpuAccessFlags.None,
            OptionFlags = ResourceOptionFlags.None,
            SizeInBytes = altaxoTriangleBuffer.IndexStreamLength,
            Usage = ResourceUsage.Default
          });
          var indexCount = altaxoTriangleBuffer.TriangleCount * 3;

          Plane[] clipPlanes = null;
          if (altaxoBuffer.Key is MaterialPlusClippingKey)
          {
            var axoClipPlanes = ((MaterialPlusClippingKey)altaxoBuffer.Key).ClipPlanes;
            if (axoClipPlanes is not null)
              clipPlanes = axoClipPlanes.Select(axoPlane => new Plane((float)axoPlane.X, (float)axoPlane.Y, (float)axoPlane.Z, (float)-axoPlane.W)).ToArray();
          }

          byte[] uColors = null;
          var material = altaxoBuffer.Key.Material;
          if (altaxoTriangleBuffer is PositionNormalUIndexedTriangleBuffer && altaxoBuffer.Key is MaterialPlusClippingPlusColorProviderKey)
          {
            var bufs = (PositionNormalUIndexedTriangleBuffer)altaxoTriangleBuffer;
            var colorProvider = ((MaterialPlusClippingPlusColorProviderKey)(altaxoBuffer.Key)).ColorProvider;
            var fColors = bufs.GetColorArrayForColorProvider(colorProvider);
            uColors = new byte[fColors.Length * 4];
            System.Buffer.BlockCopy(fColors, 0, uColors, 0, uColors.Length);
            material = material.WithColor(new NamedColor(colorProvider.GetAxoColor(double.NaN))); // Material needs to have InvalidColor, because this can not be represented in the Texture1D
          }
          newDeviceBuffers.Add(new VertexAndIndexDeviceBuffer(material: altaxoBuffer.Key.Material, vertexBuffer: vertexBuffer, vertexCount: altaxoTriangleBuffer.VertexCount, indexBuffer: indexBuffer, indexCount: indexCount, clipPlanes: clipPlanes, uColors: uColors));
        }
        System.Threading.Interlocked.Exchange(ref _nextTriangleDeviceBuffers[i], newDeviceBuffers);
      }
    }

    private void BringMarkerGeometryIntoDeviceBuffers(D3D10OverlayContext overlayGeometry)
    {
      var device = _cachedDevice;
      if (device is null || device.IsDisposed)
        return;

      // ------------------  Triangle buffer ------------------------------------
      {
        var buf = (PositionColorIndexedTriangleBuffer)overlayGeometry.PositionColorIndexedTriangleBuffers;

        var vertexBuffer = Buffer.Create<float>(device, buf.VertexStream, new BufferDescription()
        {
          BindFlags = BindFlags.VertexBuffer,
          CpuAccessFlags = CpuAccessFlags.None,
          OptionFlags = ResourceOptionFlags.None,
          SizeInBytes = buf.VertexStreamLength,
          Usage = ResourceUsage.Default
        });

        var indexBuffer = Buffer.Create<int>(device, buf.IndexStream, new BufferDescription()
        {
          BindFlags = BindFlags.IndexBuffer,
          CpuAccessFlags = CpuAccessFlags.None,
          OptionFlags = ResourceOptionFlags.None,
          SizeInBytes = buf.IndexStreamLength,
          Usage = ResourceUsage.Default
        });
        var indexCount = buf.TriangleCount * 3;

        var devBuffer = new VertexAndIndexDeviceBufferNoMaterial(vertexBuffer: vertexBuffer, vertexCount: buf.VertexCount, indexBuffer: indexBuffer, indexCount: indexCount);

        var oldBuffer = System.Threading.Interlocked.Exchange(ref _markerGeometryTriangleDeviceBuffer, devBuffer);
        oldBuffer?.Dispose();
      }

      // ------------------  Line list buffer ------------------------------------
      {
        var buf = (PositionColorLineListBuffer)overlayGeometry.PositionColorLineListBuffer;

        var vertexBuffer = Buffer.Create<float>(device, buf.VertexStream, new BufferDescription()
        {
          BindFlags = BindFlags.VertexBuffer,
          CpuAccessFlags = CpuAccessFlags.None,
          OptionFlags = ResourceOptionFlags.None,
          SizeInBytes = buf.VertexStreamLength,
          Usage = ResourceUsage.Default
        });


        var oldBuffer = System.Threading.Interlocked.Exchange(
          ref _markerGeometryLineListBuffer,
          new VertexBufferNoMaterial(vertexBuffer: vertexBuffer, vertexCount: buf.VertexCount));
        Disposer.RemoveAndDispose(ref oldBuffer);
      }
    }

    private void BringOverlayGeometryIntoDeviceBuffers(D3D10OverlayContext overlayGeometry)
    {
      var device = _cachedDevice;
      if (device is null || device.IsDisposed)
        return;

      // ------------------  Triangle buffer ------------------------------------
      {
        var buf = (PositionColorIndexedTriangleBuffer)overlayGeometry.PositionColorIndexedTriangleBuffers;

        var vertexBuffer = Buffer.Create<float>(device, buf.VertexStream, new BufferDescription()
        {
          BindFlags = BindFlags.VertexBuffer,
          CpuAccessFlags = CpuAccessFlags.None,
          OptionFlags = ResourceOptionFlags.None,
          SizeInBytes = buf.VertexStreamLength,
          Usage = ResourceUsage.Default
        });

        var indexBuffer = Buffer.Create<int>(device, buf.IndexStream, new BufferDescription()
        {
          BindFlags = BindFlags.IndexBuffer,
          CpuAccessFlags = CpuAccessFlags.None,
          OptionFlags = ResourceOptionFlags.None,
          SizeInBytes = buf.IndexStreamLength,
          Usage = ResourceUsage.Default
        });
        var indexCount = buf.TriangleCount * 3;


        var oldBuffer = System.Threading.Interlocked.Exchange(
          ref _overlayGeometryTriangleDeviceBuffer,
          new VertexAndIndexDeviceBufferNoMaterial(vertexBuffer: vertexBuffer, vertexCount: buf.VertexCount, indexBuffer: indexBuffer, indexCount: indexCount));
        Disposer.RemoveAndDispose(ref oldBuffer);
      }

      // ------------------  Line list buffer ------------------------------------
      {
        var buf = (PositionColorLineListBuffer)overlayGeometry.PositionColorLineListBuffer;

        var vertexBuffer = Buffer.Create<float>(device, buf.VertexStream, new BufferDescription()
        {
          BindFlags = BindFlags.VertexBuffer,
          CpuAccessFlags = CpuAccessFlags.None,
          OptionFlags = ResourceOptionFlags.None,
          SizeInBytes = buf.VertexStreamLength,
          Usage = ResourceUsage.Default
        });


        var oldBuffer = System.Threading.Interlocked.Exchange(
          ref _overlayGeometryLineListBuffer,
          new VertexBufferNoMaterial(vertexBuffer: vertexBuffer, vertexCount: buf.VertexCount));

        oldBuffer?.Dispose();
      }
    }



    void IScene.Update(TimeSpan sceneTime)
    {
      // use sceneTime.TotalSeconds to update the scene here in dependence on the scene time
    }

    void IScene.Render()
    {
      var device = _cachedDevice;
      if (device is null)
        throw new InvalidOperationException("Rendering failed because device is null");
      if (device.IsDisposed)
        throw new InvalidOperationException("Rendering failed because device is disposed");
      if (_altaxoCamera is null)
        throw new InvalidOperationException("Rendering failed because camera is null");
      if (_altaxoDrawingGeometry is null)
        throw new InvalidOperationException("Rendering failed because drawing is null");

      UseNextTriangleDeviceBuffers();

      float time = _renderCounter / 100f;
      ++_renderCounter;

      Matrix worldViewProjTr; // world-view matrix, transposed

      if (_altaxoCamera is not null)
      {
        var cam = _altaxoCamera;
        var eye = cam.EyePosition;
        var target = cam.TargetPosition;
        var up = cam.UpVector;
        //view = Matrix.LookAtRH(new Vector3((float)eye.X, (float)eye.Y, (float)eye.Z), new Vector3((float)target.X, (float)target.Y, (float)target.Z), new Vector3((float)up.X, (float)up.Y, (float)up.Z));

        //var viewProjD3D = (cam as PerspectiveCamera).GetLookAtRHTimesPerspectiveRHMatrix(_hostSize.Y / _hostSize.X);
        var viewProjD3D = cam.GetViewProjectionMatrix(_hostSize.Y / _hostSize.X);
        worldViewProjTr = new Matrix(
                (float)viewProjD3D.M11, (float)viewProjD3D.M21, (float)viewProjD3D.M31, (float)viewProjD3D.M41,
                (float)viewProjD3D.M12, (float)viewProjD3D.M22, (float)viewProjD3D.M32, (float)viewProjD3D.M42,
                (float)viewProjD3D.M13, (float)viewProjD3D.M23, (float)viewProjD3D.M33, (float)viewProjD3D.M43,
                (float)viewProjD3D.M14, (float)viewProjD3D.M24, (float)viewProjD3D.M34, (float)viewProjD3D.M44
                );
      }
      else
      {
        var view = Matrix.LookAtRH(new Vector3(0, 0, -1500), new Vector3(0, 0, 0), Vector3.UnitY);
        var proj = Matrix.PerspectiveFovRH((float)Math.PI / 4.0f, (float)(_hostSize.X / _hostSize.Y), 0.1f, float.MaxValue);
        var viewProj = Matrix.Multiply(view, proj);

        // Update WorldViewProj Matrix
        worldViewProjTr = viewProj;
        worldViewProjTr.Transpose();
      }

      // World projection and camera
      _evWorldViewProj.SetMatrixTranspose(ref worldViewProjTr);
      _evEyePosition.Set(ToVector4(_altaxoCamera.EyePosition, 1f));

      // lighting
      _lighting.SetLighting(_altaxoLightSettings, _altaxoCamera);

      // Material is separate for each buffer, therefore it is set there

      foreach (var entry in _thisTriangleDeviceBuffers[1]) // Position-Color
      {
        DrawPositionColorIndexedTriangleBuffer(device, entry, worldViewProjTr);
      }

      foreach (var entry in _thisTriangleDeviceBuffers[3]) // Position-Normal
      {
        DrawPositionNormalIndexedTriangleBuffer(device, entry, worldViewProjTr);
      }

      foreach (var entry in _thisTriangleDeviceBuffers[4]) // Position-Normal-Color
      {
        DrawPositionNormalColorIndexedTriangleBuffer(device, entry, worldViewProjTr);
      }

      foreach (var entry in _thisTriangleDeviceBuffers[6]) // Position-Normal-U
      {
        DrawPositionNormalUIndexedTriangleBuffer(device, entry, worldViewProjTr);
      }

      // ------------------ end of document geometry drawing ----------------------------------

      // ------------------ start of marker geometry drawing ----------------------------------

      var markerTriangles = _markerGeometryTriangleDeviceBuffer;
      if (markerTriangles is not null)
        DrawPositionColorIndexedTriangleBufferNoMaterial(device, markerTriangles, worldViewProjTr);

      var markerLines = _markerGeometryLineListBuffer;
      if (markerLines is not null && markerLines.VertexCount > 0)
        DrawPositionColorLineListBufferNoMaterial(device, markerLines, worldViewProjTr);

      // ------------------ end of marker geometry drawing ----------------------------------

      // ------------------ start of overlay geometry drawing ----------------------------------

      var overlayTriangles = _overlayGeometryTriangleDeviceBuffer;
      if (overlayTriangles is not null)
        DrawPositionColorIndexedTriangleBufferNoMaterial(device, overlayTriangles, worldViewProjTr);

      var overlayLines = _overlayGeometryLineListBuffer;
      if (overlayLines is not null && overlayLines.VertexCount > 0)
        DrawPositionColorLineListBufferNoMaterial(device, overlayLines, worldViewProjTr);

      // ------------------ end of overlay geometry drawing ----------------------------------
    }

    private struct SixPlanes
    {
      public Plane v0, v1, v2, v3, v4, v5;

      public Plane this[int i]
      {
        get
        {
          switch (i)
          {
            case 0:
              return v0;

            case 1:
              return v1;

            case 2:
              return v2;

            case 3:
              return v3;

            case 4:
              return v4;

            case 5:
              return v5;

            default:
              throw new IndexOutOfRangeException();
          }
        }
        set
        {
          switch (i)
          {
            case 0:
              v0 = value;
              break;

            case 1:
              v1 = value;
              break;

            case 2:
              v2 = value;
              break;

            case 3:
              v3 = value;
              break;

            case 4:
              v4 = value;
              break;

            case 5:
              v5 = value;
              break;

            default:
              throw new IndexOutOfRangeException();
          }
        }
      }
    }

    private void SetShaderMaterialVariables(IMaterial material)
    {
      _evMaterialSpecularIntensity.Set((float)material.PhongModelSpecularIntensity);
      _evMaterialDiffuseIntensity.Set((float)material.PhongModelDiffuseIntensity);
      _evMaterialSpecularExponent.Set((float)material.PhongModelSpecularExponent);
      _evMaterialMetalnessValue.Set((float)material.Metalness);
      if (material.HasColor)
      {
        Vector4 colorVec = ToVector4(material.Color.Color);
        _evMaterialDiffuseColor.Set(ref colorVec);
      }
    }

    private void DrawPositionColorIndexedTriangleBuffer(Device device, VertexAndIndexDeviceBuffer deviceBuffers, Matrix worldViewProj)
    {
      int layoutNumber = 1;
      device.InputAssembler.InputLayout = _renderLayouts[layoutNumber].VertexLayout;
      device.InputAssembler.PrimitiveTopology = SharpDX.Direct3D.PrimitiveTopology.TriangleList;

      SetShaderMaterialVariables(deviceBuffers.Material);

      var planes = new SixPlanes();
      if (deviceBuffers.ClipPlanes is not null)
      {
        for (int i = 0; i < Math.Min(6, deviceBuffers.ClipPlanes.Length); ++i)
        {
          planes[i] = deviceBuffers.ClipPlanes[i];
        }
      }

      device.InputAssembler.SetVertexBuffers(0, new VertexBufferBinding(deviceBuffers.VertexBuffer, 32, 0));
      device.InputAssembler.SetIndexBuffer(deviceBuffers.IndexBuffer, Format.R32_UInt, 0);

      _renderLayouts[layoutNumber].Pass.Apply();
      device.DrawIndexed(deviceBuffers.IndexCount, 0, 0);
    }

    private void DrawPositionNormalColorIndexedTriangleBuffer(Device device, VertexAndIndexDeviceBuffer deviceBuffers, Matrix worldViewProj)
    {
      int layoutNumber = 4;
      device.InputAssembler.InputLayout = _renderLayouts[layoutNumber].VertexLayout;
      device.InputAssembler.PrimitiveTopology = SharpDX.Direct3D.PrimitiveTopology.TriangleList;

      if (deviceBuffers.ClipPlanes is not null)
      {
        for (int i = 0; i < Math.Min(6, deviceBuffers.ClipPlanes.Length); ++i)
        {
          _evClipPlanes[i].Set(deviceBuffers.ClipPlanes[i]);
        }
      }

      SetShaderMaterialVariables(deviceBuffers.Material);

      device.InputAssembler.SetVertexBuffers(0, new VertexBufferBinding(deviceBuffers.VertexBuffer, 48, 0));
      device.InputAssembler.SetIndexBuffer(deviceBuffers.IndexBuffer, Format.R32_UInt, 0);

      _renderLayouts[layoutNumber].Pass.Apply();
      device.DrawIndexed(deviceBuffers.IndexCount, 0, 0);

      if (deviceBuffers.ClipPlanes is not null)
      {
        var emptyPlane = new Plane();
        for (int i = 0; i < Math.Min(6, deviceBuffers.ClipPlanes.Length); ++i)
        {
          _evClipPlanes[i].Set(emptyPlane);
        }
      }
    }

    private void DrawPositionNormalUIndexedTriangleBuffer(Device device, VertexAndIndexDeviceBuffer deviceBuffers, Matrix worldViewProj)
    {
      int layoutNumber = 6;
      device.InputAssembler.InputLayout = _renderLayouts[layoutNumber].VertexLayout;
      device.InputAssembler.PrimitiveTopology = SharpDX.Direct3D.PrimitiveTopology.TriangleList;

      // set clip planes
      if (deviceBuffers.ClipPlanes is not null)
      {
        for (int i = 0; i < Math.Min(6, deviceBuffers.ClipPlanes.Length); ++i)
        {
          _evClipPlanes[i].Set(deviceBuffers.ClipPlanes[i]);
        }
      }

      // set UColor texture

      var stream = _textureFor1DColorProvider.Map(0, MapMode.WriteDiscard, SharpDX.Direct3D10.MapFlags.None);
      //stream.Seek(0, System.IO.SeekOrigin.Begin);
      stream.Write(deviceBuffers.UColors, 0, deviceBuffers.UColors.Length);
      //stream.Close();
      _textureFor1DColorProvider.Unmap(0);

      SetShaderMaterialVariables(deviceBuffers.Material); // note: the material's color must be set to the ColorProviders InvalidColor!

      // draw now

      device.InputAssembler.SetVertexBuffers(0, new VertexBufferBinding(deviceBuffers.VertexBuffer, 32, 0));
      device.InputAssembler.SetIndexBuffer(deviceBuffers.IndexBuffer, Format.R32_UInt, 0);

      _renderLayouts[layoutNumber].Pass.Apply();
      device.DrawIndexed(deviceBuffers.IndexCount, 0, 0);

      // clear clip planes afterwards
      if (deviceBuffers.ClipPlanes is not null)
      {
        var emptyPlane = new Plane();
        for (int i = 0; i < Math.Min(6, deviceBuffers.ClipPlanes.Length); ++i)
        {
          _evClipPlanes[i].Set(emptyPlane);
        }
      }
    }

    private void DrawPositionNormalIndexedTriangleBuffer(Device device, VertexAndIndexDeviceBuffer deviceBuffers, Matrix worldViewProj)
    {
      int layoutNumber = 3;
      device.InputAssembler.InputLayout = _renderLayouts[layoutNumber].VertexLayout;
      device.InputAssembler.PrimitiveTopology = SharpDX.Direct3D.PrimitiveTopology.TriangleList;

      SetShaderMaterialVariables(deviceBuffers.Material);

      device.InputAssembler.SetVertexBuffers(0, new VertexBufferBinding(deviceBuffers.VertexBuffer, 32, 0));
      device.InputAssembler.SetIndexBuffer(deviceBuffers.IndexBuffer, Format.R32_UInt, 0);

      _renderLayouts[layoutNumber].Pass.Apply();
      device.DrawIndexed(deviceBuffers.IndexCount, 0, 0);
    }

    private void DrawPositionColorIndexedTriangleBufferNoMaterial(Device device, VertexAndIndexDeviceBufferNoMaterial deviceBuffers, Matrix worldViewProj)
    {
      int layoutNumber = 1;
      device.InputAssembler.InputLayout = _renderLayouts[layoutNumber].VertexLayout;
      device.InputAssembler.PrimitiveTopology = SharpDX.Direct3D.PrimitiveTopology.TriangleList;

      device.InputAssembler.SetVertexBuffers(0, new VertexBufferBinding(deviceBuffers.VertexBuffer, 32, 0));
      device.InputAssembler.SetIndexBuffer(deviceBuffers.IndexBuffer, Format.R32_UInt, 0);

      _renderLayouts[layoutNumber].Pass.Apply();
      device.DrawIndexed(deviceBuffers.IndexCount, 0, 0);
    }

    private void DrawPositionColorLineListBufferNoMaterial(Device device, VertexBufferNoMaterial deviceBuffers, Matrix worldViewProj)
    {
      int layoutNumber = 1;
      device.InputAssembler.InputLayout = _renderLayouts[layoutNumber].VertexLayout;
      device.InputAssembler.PrimitiveTopology = SharpDX.Direct3D.PrimitiveTopology.LineList;

      device.InputAssembler.SetVertexBuffers(0, new VertexBufferBinding(deviceBuffers.VertexBuffer, (8 * 4), 0));

      _renderLayouts[layoutNumber].Pass.Apply();
      device.Draw(deviceBuffers.VertexCount, 0);
    }

    // helper

    private static Vector4 ToVector4(PointD3D a, float c4)
    {
      return new Vector4((float)a.X, (float)a.Y, (float)a.Z, c4);
    }

    private static Vector4 ToVector4(VectorD3D a, float c4)
    {
      return new Vector4((float)a.X, (float)a.Y, (float)a.Z, c4);
    }

    private static Vector4 ToVector4(PointD3D a)
    {
      return new Vector4((float)a.X, (float)a.Y, (float)a.Z, 1.0f);
    }

    private static Vector4 ToVector4(Altaxo.Drawing.AxoColor color, double amplitude, double alpha)
    {
      float amp = (float)amplitude;
      return new Vector4(color.ScR * amp, color.ScG * amp, color.ScB * amp, (float)alpha);
    }

    private static Vector4 ToVector4(Altaxo.Drawing.AxoColor color)
    {
      return new Vector4(color.ScR, color.ScG, color.ScB, color.ScA);
    }
  }
}
