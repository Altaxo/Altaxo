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
  using System.Numerics;
  using Altaxo.Drawing;
  using Altaxo.Drawing.D3D;
  using Altaxo.Geometry;
  using Altaxo.Graph.Graph3D;
  using Altaxo.Graph.Graph3D.Camera;
  using Altaxo.Graph.Graph3D.GraphicsContext;
  using Altaxo.Graph.Graph3D.GraphicsContext.D3D;
  using Altaxo.Graph.Graph3D.Lighting;
  using Altaxo.Gui.Graph.Graph3D.Common;
  using Vortice;
  using Vortice.D3DCompiler;
  using Vortice.Direct3D;
  using Vortice.Direct3D11;
  using Vortice.DXGI;
  using Vortice.Mathematics;
  using Buffer = Vortice.Direct3D11.ID3D11Buffer;
  using Device = Vortice.Direct3D11.ID3D11Device;
  using DeviceContext = Vortice.Direct3D11.ID3D11DeviceContext;

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

    private int _renderCounter;

    // Meaning: P = PointCoordinates, C = Color, T = TextureCoordinates, N = Normal
    private string[] _layoutNames = new string[7] { "P", "PC", "PT", "PN", "PNC", "PNT", "PNT1" };

    private ID3D11InputLayout[] _renderLayouts = new ID3D11InputLayout[7];

    // Effect variables

    // Transformation variables
    private System.Numerics.Matrix4x4 _worldViewProj = System.Numerics.Matrix4x4.Identity;
    private Buffer? _evWorldViewProj;

    private System.Numerics.Vector4 _eyePosition;
    private Buffer? _evEyePosition;

    // Materials

    // Texture for color providers to colorize a mesh by its height
    private Texture1DDescription _descriptionTextureFor1DColorProvider;
    private ID3D11Texture1D? _textureFor1DColorProvider;
    private ID3D11ShaderResourceView? _textureFor1DColorProviderView;

    private struct CbClipPlanes
    {
      public Plane Plane0;
      public Plane Plane1;
      public Plane Plane2;
      public Plane Plane3;
      public Plane Plane4;
      public Plane Plane5;

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
    }

    private struct CbMaterial
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

    private CbMaterial _material = new CbMaterial();
    private Buffer? _evMaterial;

    // Clip planes
    private CbClipPlanes _clipPlanes = new CbClipPlanes();
    private Buffer? _evClipPlanes;

    // Lighting
    private Lighting? _lighting;


    private ID3D11VertexShader _vertexShader_P;
    private ID3D11VertexShader _vertexShader_OVERLAY_PC;
    private ID3D11VertexShader _vertexShader_PT;
    private ID3D11VertexShader _vertexShader_PN;
    private ID3D11VertexShader _vertexShader_PNC;
    private ID3D11VertexShader _vertexShader_PNT;
    private ID3D11VertexShader _vertexShader_PNT1;

    private ID3D11PixelShader _pixelShader;
    private ID3D11PixelShader _pixelShader_OVERLAY;
    private ID3D11PixelShader _pixelShader_T1;



    #region Members that do not utilize unmanaged resources, and thus are not associated with a 3D device
    // --------------------------------------------------------------------------------------------------

    private AxoColor? _sceneBackgroundColor;

    /// <summary>
    /// The geometry to render, i.e. the scene itself.
    /// </summary>
    private D3DGraphicsContext? _altaxoDrawingGeometry;

    /// <summary>
    /// Helper geometry that draws X-Y-Z arrows for better orientation.
    /// </summary>
    private D3DOverlayContext? _altaxoMarkerGeometry;

    /// <summary>
    /// Geometry that is used temporarily, e.g. to show boxes when objects are moved or selected.
    /// </summary>
    private D3DOverlayContext? _altaxoOverlayGeometry;

    /// <summary>
    /// The camera associated with the scene
    /// </summary>
    private CameraBase? _altaxoCamera;

    /// <summary>The light settings from AltaxoBase</summary>
    private LightSettings? _altaxoLightSettings;

    #endregion

    private ID3D11VertexShader CreateVertexShader(ID3D11Device device, string entryPoint, out byte[] vertexShaderBytes)
    {
      var resourceName = $"Altaxo.CompiledShaders.Lighting_{entryPoint}.cso";
      using (var stream = System.Reflection.Assembly.GetExecutingAssembly().GetManifestResourceStream(resourceName))
      {
        if (stream is null)
          throw new InvalidOperationException($"Compiled shader resource not found: {resourceName}");

        vertexShaderBytes = new byte[stream.Length];
        stream.Read(vertexShaderBytes, 0, vertexShaderBytes.Length);
        return device.CreateVertexShader(vertexShaderBytes);
      }
    }

    private ID3D11PixelShader CreatePixelShader(ID3D11Device device, string entryPoint, out byte[] pixelShaderBytes)
    {
      var resourceName = $"Altaxo.CompiledShaders.Lighting_{entryPoint}.cso";
      using (var stream = System.Reflection.Assembly.GetExecutingAssembly().GetManifestResourceStream(resourceName))
      {
        if (stream is null)
          throw new InvalidOperationException($"Compiled shader resource not found: {resourceName}");

        pixelShaderBytes = new byte[stream.Length];
        stream.Read(pixelShaderBytes, 0, pixelShaderBytes.Length);
        return device.CreatePixelShader(pixelShaderBytes);
      }
    }

    public void Attach(SharpGen.Runtime.ComObject hostDevice, PointD2D hostSize)
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

      _vertexShader_P = CreateVertexShader(device, "VS_P", out var bytes_VS_P);
      _vertexShader_OVERLAY_PC = CreateVertexShader(device, "VS_OVERLAY_PC", out var bytes_VS_OVERLAY_PC);
      _vertexShader_PT = CreateVertexShader(device, "VS_PT", out var bytes_VS_PT);
      _vertexShader_PN = CreateVertexShader(device, "VS_PN", out var bytes_VS_PN);
      _vertexShader_PNC = CreateVertexShader(device, "VS_PNC", out var bytes_VS_PNC);
      _vertexShader_PNT = CreateVertexShader(device, "VS_PNT", out var bytes_VS_PNT);
      _vertexShader_PNT1 = CreateVertexShader(device, "VS_PNT1", out var bytes_VS_PNT1);

      _pixelShader = CreatePixelShader(device, "PS", out var bytes_PS);
      _pixelShader_OVERLAY = CreatePixelShader(device, "PS_OVERLAY", out var bytes_PS_OVERLAY);
      _pixelShader_T1 = CreatePixelShader(device, "PS_T1", out var bytes_PS_T1);

      for (int i = 0; i < _layoutNames.Length; ++i)
      {
        ID3D11InputLayout inputLayout;
        switch (i)
        {
          case 0:
            inputLayout = device.CreateInputLayout(new[] {
                                          new InputElementDescription("POSITION", 0, Format.R32G32B32A32_Float, 0, 0)
                                          },
                                          bytes_VS_P);
            break;
          case 1:
            inputLayout = device.CreateInputLayout(new[] {
                                          new InputElementDescription("POSITION", 0, Format.R32G32B32A32_Float, 0, 0),
                                          new InputElementDescription("COLOR", 0, Format.R32G32B32A32_Float, 16, 0)
                                          },
                                          bytes_VS_OVERLAY_PC);
            break;
          case 2:
            inputLayout = device.CreateInputLayout(new[] {
                                          new InputElementDescription("POSITION", 0, Format.R32G32B32A32_Float, 0, 0),
                                          new InputElementDescription("TEXCOORD", 0, Format.R32G32_Float, 16, 0)
                                          },
                                          bytes_VS_PT);
            break;
          case 3:
            inputLayout = device.CreateInputLayout(new[] {
                                          new InputElementDescription("POSITION", 0, Format.R32G32B32A32_Float, 0, 0),
                                          new InputElementDescription("NORMAL", 0, Format.R32G32B32A32_Float, 16, 0)
                                          },
                                          bytes_VS_PN);
            break;
          case 4:
            inputLayout = device.CreateInputLayout(new[] {
                                          new InputElementDescription("POSITION", 0, Format.R32G32B32A32_Float, 0, 0),
                                          new InputElementDescription("NORMAL", 0, Format.R32G32B32A32_Float, 16, 0),
                                          new InputElementDescription("COLOR", 0, Format.R32G32B32A32_Float, 32, 0)
                                          },
                                          bytes_VS_PNC);
            break;
          case 5:
            inputLayout = device.CreateInputLayout(new[] {
                                          new InputElementDescription("POSITION", 0, Format.R32G32B32A32_Float, 0, 0),
                                          new InputElementDescription("NORMAL", 0, Format.R32G32B32A32_Float, 16, 0),
                                          new InputElementDescription("TEXCOORD", 0, Format.R32G32_Float, 32, 0)
                                          },
                                          bytes_VS_PNT);
            break;
          case 6:
            inputLayout = device.CreateInputLayout(new[] {
                                          new InputElementDescription("POSITION", 0, Format.R32G32B32_Float, 0, 0),
                                          new InputElementDescription("NORMAL", 0, Format.R32G32B32_Float, 12, 0),
                                          new InputElementDescription("TEXCOORD", 0, Format.R32G32_Float, 24, 0)
                                          },
                                          bytes_VS_PNT1);
            break;
          default:
            throw new NotImplementedException();
        }

        _renderLayouts[i] = inputLayout;
      }




      // View transformation variables
      _evWorldViewProj = device.CreateBuffer(ref _worldViewProj, new BufferDescription((4 * 4) * sizeof(float), BindFlags.ConstantBuffer, ResourceUsage.Default));
      device.ImmediateContext.VSSetConstantBuffer(0, _evWorldViewProj);

      _evEyePosition = device.CreateBuffer(ref _eyePosition, new BufferDescription(4 * sizeof(float), BindFlags.ConstantBuffer, ResourceUsage.Default));
      device.ImmediateContext.PSSetConstantBuffer(1, _evEyePosition);

      // Material
      _material.SpecularExponent = 4;
      _material.SpecularIntensity = 1;
      _material.MetalnessValue = 0.75f;
      _evMaterial = device.CreateBuffer(ref _material, new BufferDescription(8 * sizeof(float), BindFlags.ConstantBuffer, ResourceUsage.Default));
      device.ImmediateContext.VSSetConstantBuffer(2, _evMaterial);
      device.ImmediateContext.PSSetConstantBuffer(2, _evMaterial);

      // Color providers
      BindTextureFor1DColorProviders(device.ImmediateContext);

      // Clip plane variables
      _evClipPlanes = device.CreateBuffer(ref _clipPlanes, new BufferDescription(6 * 4 * sizeof(float), BindFlags.ConstantBuffer, ResourceUsage.Default));
      device.ImmediateContext.VSSetConstantBuffer(4, _evClipPlanes);


      // Lighting variables
      _lighting = new Lighting(device);
      _lighting.SetDefaultLighting(device);

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

      // dispose shaders

      Disposer.RemoveAndDispose(ref _vertexShader_P);
      Disposer.RemoveAndDispose(ref _vertexShader_OVERLAY_PC);
      Disposer.RemoveAndDispose(ref _vertexShader_PT);
      Disposer.RemoveAndDispose(ref _vertexShader_PN);
      Disposer.RemoveAndDispose(ref _vertexShader_PNC);
      Disposer.RemoveAndDispose(ref _vertexShader_PNT);
      Disposer.RemoveAndDispose(ref _vertexShader_PNT1);
      Disposer.RemoveAndDispose(ref _pixelShader);
      Disposer.RemoveAndDispose(ref _pixelShader_OVERLAY);
      Disposer.RemoveAndDispose(ref _pixelShader_T1);


      // now dispose all other device dependent variables, in inverse order than in Attach()

      Disposer.RemoveAndDispose(ref _lighting);
      Disposer.RemoveAndDispose(ref _evClipPlanes);
      ReleaseTextureFor1DColorProviders();
      Disposer.RemoveAndDispose(ref _evMaterial);
      Disposer.RemoveAndDispose(ref _evEyePosition);
      Disposer.RemoveAndDispose(ref _evWorldViewProj);

      for (int i = 0; i < _renderLayouts.Length; ++i)
      {
        _renderLayouts[i]?.Dispose();
      }

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

    internal void SetMarkerGeometry(D3DOverlayContext markerGeometry)
    {
      _altaxoMarkerGeometry = markerGeometry;
      BringMarkerGeometryIntoDeviceBuffers(markerGeometry);
    }

    public void SetHostSize(PointD2D hostSize)
    {
      _hostSize = hostSize;
    }

    public void SetDrawing(D3DGraphicsContext drawing)
    {
      _altaxoDrawingGeometry = drawing;
      BringDrawingIntoBuffers(drawing);
    }

    public void SetOverlayGeometry(D3DOverlayContext overlayGeometry)
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

    private void BindTextureFor1DColorProviders(DeviceContext context)
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

      _textureFor1DColorProvider = _cachedDevice.CreateTexture1D(_descriptionTextureFor1DColorProvider);
      _textureFor1DColorProviderView = _cachedDevice.CreateShaderResourceView(_textureFor1DColorProvider);
      // for how to create a UnorderedAccessView, see https://stackoverflow.com/questions/44251230/how-can-i-write-to-my-id3d11unorderedaccessviews-buffer-before-dispatching-my-s?rq=1
      // var tt = _cachedDevice.CreateUnorderedAccessView(_textureFor1DColorProvider, new UnorderedAccessViewDescription())

      context.PSSetShaderResource(0, _textureFor1DColorProviderView);
    }

    private void ReleaseTextureFor1DColorProviders()
    {
      Disposer.RemoveAndDispose(ref _textureFor1DColorProviderView);
      Disposer.RemoveAndDispose(ref _textureFor1DColorProvider);
    }

    private void BringDrawingIntoBuffers(D3DGraphicsContext altaxoDrawingGeometry)
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

          var vertexBuffer = device.CreateBuffer(altaxoTriangleBuffer.VertexStream, new BufferDescription()
          {
            BindFlags = BindFlags.VertexBuffer,
            CpuAccessFlags = CpuAccessFlags.None,
            OptionFlags = ResourceOptionFlags.None,
            SizeInBytes = altaxoTriangleBuffer.VertexStreamLength,
            Usage = ResourceUsage.Default
          });

          var indexBuffer = device.CreateBuffer(altaxoTriangleBuffer.IndexStream, new BufferDescription()
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

    private void BringMarkerGeometryIntoDeviceBuffers(D3DOverlayContext overlayGeometry)
    {
      var device = _cachedDevice;
      if (device is null || device.IsDisposed)
        return;

      // ------------------  Triangle buffer ------------------------------------
      {
        var buf = (PositionColorIndexedTriangleBuffer)overlayGeometry.PositionColorIndexedTriangleBuffers;

        var vertexBuffer = device.CreateBuffer(buf.VertexStream, new BufferDescription()
        {
          BindFlags = BindFlags.VertexBuffer,
          CpuAccessFlags = CpuAccessFlags.None,
          OptionFlags = ResourceOptionFlags.None,
          SizeInBytes = buf.VertexStreamLength,
          Usage = ResourceUsage.Default
        });

        var indexBuffer = device.CreateBuffer(buf.IndexStream, new BufferDescription()
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

        var vertexBuffer = device.CreateBuffer(buf.VertexStream, new BufferDescription()
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

    private void BringOverlayGeometryIntoDeviceBuffers(D3DOverlayContext overlayGeometry)
    {
      var device = _cachedDevice;
      if (device is null || device.IsDisposed)
        return;

      // ------------------  Triangle buffer ------------------------------------
      {
        var buf = (PositionColorIndexedTriangleBuffer)overlayGeometry.PositionColorIndexedTriangleBuffers;

        var vertexBuffer = device.CreateBuffer(buf.VertexStream, new BufferDescription()
        {
          BindFlags = BindFlags.VertexBuffer,
          CpuAccessFlags = CpuAccessFlags.None,
          OptionFlags = ResourceOptionFlags.None,
          SizeInBytes = buf.VertexStreamLength,
          Usage = ResourceUsage.Default
        });

        var indexBuffer = device.CreateBuffer(buf.IndexStream, new BufferDescription()
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

        var vertexBuffer = device.CreateBuffer(buf.VertexStream, new BufferDescription()
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

      System.Numerics.Matrix4x4 worldViewProjTr; // world-view matrix, transposed

      if (_altaxoCamera is not null)
      {
        var cam = _altaxoCamera;
        var eye = cam.EyePosition;
        var target = cam.TargetPosition;
        var up = cam.UpVector;
        //view = Matrix.LookAtRH(new Vector3((float)eye.X, (float)eye.Y, (float)eye.Z), new Vector3((float)target.X, (float)target.Y, (float)target.Z), new Vector3((float)up.X, (float)up.Y, (float)up.Z));

        //var viewProjD3D = (cam as PerspectiveCamera).GetLookAtRHTimesPerspectiveRHMatrix(_hostSize.Y / _hostSize.X);
        var viewProjD3D = cam.GetViewProjectionMatrix(_hostSize.Y / _hostSize.X);
        worldViewProjTr = new System.Numerics.Matrix4x4(
                (float)viewProjD3D.M11, (float)viewProjD3D.M21, (float)viewProjD3D.M31, (float)viewProjD3D.M41,
                (float)viewProjD3D.M12, (float)viewProjD3D.M22, (float)viewProjD3D.M32, (float)viewProjD3D.M42,
                (float)viewProjD3D.M13, (float)viewProjD3D.M23, (float)viewProjD3D.M33, (float)viewProjD3D.M43,
                (float)viewProjD3D.M14, (float)viewProjD3D.M24, (float)viewProjD3D.M34, (float)viewProjD3D.M44
                );
      }
      else
      {
        var view = System.Numerics.Matrix4x4.CreateLookAt(new Vector3(0, 0, -1500), new Vector3(0, 0, 0), Vector3.UnitY);
        var proj = System.Numerics.Matrix4x4.CreatePerspectiveFieldOfView((float)Math.PI / 4.0f, (float)(_hostSize.X / _hostSize.Y), 0.1f, float.MaxValue);
        var viewProj = System.Numerics.Matrix4x4.Multiply(view, proj);

        // Update WorldViewProj Matrix
        worldViewProjTr = viewProj;
        worldViewProjTr = System.Numerics.Matrix4x4.Transpose(worldViewProjTr);
      }

      // World projection and camera
      _worldViewProj = worldViewProjTr;
      device.ImmediateContext.UpdateSubresource(ref _worldViewProj, _evWorldViewProj);


      _eyePosition = ToVector4(_altaxoCamera.EyePosition, 1f);
      device.ImmediateContext.UpdateSubresource(ref _eyePosition, _evEyePosition);

      // lighting
      _lighting.SetLighting(device, _altaxoLightSettings, _altaxoCamera);

      // Material is separate for each buffer, therefore it is set there

      foreach (var entry in _thisTriangleDeviceBuffers[1]) // Position-Color
      {
        DrawPositionColorIndexedTriangleBuffer(device.ImmediateContext, entry, worldViewProjTr);
      }

      foreach (var entry in _thisTriangleDeviceBuffers[3]) // Position-Normal
      {
        DrawPositionNormalIndexedTriangleBuffer(device.ImmediateContext, entry, worldViewProjTr);
      }

      foreach (var entry in _thisTriangleDeviceBuffers[4]) // Position-Normal-Color
      {
        DrawPositionNormalColorIndexedTriangleBuffer(device.ImmediateContext, entry, worldViewProjTr);
      }

      foreach (var entry in _thisTriangleDeviceBuffers[6]) // Position-Normal-U
      {
        DrawPositionNormalUIndexedTriangleBuffer(device.ImmediateContext, entry, worldViewProjTr);
      }


      // ------------------ end of document geometry drawing ----------------------------------

      // ------------------ start of marker geometry drawing ----------------------------------

      var markerTriangles = _markerGeometryTriangleDeviceBuffer;
      if (markerTriangles is not null)
        DrawPositionColorIndexedTriangleBufferNoMaterial(device.ImmediateContext, markerTriangles, worldViewProjTr);

      var markerLines = _markerGeometryLineListBuffer;
      if (markerLines is not null && markerLines.VertexCount > 0)
        DrawPositionColorLineListBufferNoMaterial(device.ImmediateContext, markerLines, worldViewProjTr);

      // ------------------ end of marker geometry drawing ----------------------------------

      // ------------------ start of overlay geometry drawing ----------------------------------

      var overlayTriangles = _overlayGeometryTriangleDeviceBuffer;
      if (overlayTriangles is not null)
        DrawPositionColorIndexedTriangleBufferNoMaterial(device.ImmediateContext, overlayTriangles, worldViewProjTr);

      var overlayLines = _overlayGeometryLineListBuffer;
      if (overlayLines is not null && overlayLines.VertexCount > 0)
        DrawPositionColorLineListBufferNoMaterial(device.ImmediateContext, overlayLines, worldViewProjTr);

      // ------------------ end of overlay geometry drawing ----------------------------------
    }

    private void SetShaderMaterialVariables(DeviceContext device, IMaterial material)
    {
      _material.SpecularIntensity = (float)material.PhongModelSpecularIntensity;
      _material.DiffuseIntensity = (float)material.PhongModelDiffuseIntensity;
      _material.SpecularExponent = (float)material.PhongModelSpecularExponent;
      _material.MetalnessValue = (float)material.Metalness;
      if (material.HasColor)
      {
        _material.DiffuseColor = ToColor4(material.Color.Color);
      }
      device.UpdateSubresource(ref _material, _evMaterial);
    }

    private void DrawPositionColorIndexedTriangleBuffer(DeviceContext device, VertexAndIndexDeviceBuffer deviceBuffers, System.Numerics.Matrix4x4 worldViewProj)
    {
      int layoutNumber = 1;
      device.VSSetShader(_vertexShader_OVERLAY_PC);
      device.PSSetShader(_pixelShader_OVERLAY);
      device.IASetInputLayout(_renderLayouts[layoutNumber]);
      device.IASetPrimitiveTopology(PrimitiveTopology.TriangleList);

      SetShaderMaterialVariables(device, deviceBuffers.Material);

      if (deviceBuffers.ClipPlanes is not null)
      {
        for (int i = 0; i < Math.Min(6, deviceBuffers.ClipPlanes.Length); ++i)
        {
          _clipPlanes[i] = deviceBuffers.ClipPlanes[i];
        }
        device.UpdateSubresource(ref _clipPlanes, _evClipPlanes);
      }

      device.IASetVertexBuffers(0, new VertexBufferView(deviceBuffers.VertexBuffer, 32, 0));
      device.IASetIndexBuffer(deviceBuffers.IndexBuffer, Format.R32_UInt, 0);

      // _renderLayouts[layoutNumber].Pass.Apply();
      device.DrawIndexed(deviceBuffers.IndexCount, 0, 0);
    }

    private void DrawPositionNormalColorIndexedTriangleBuffer(DeviceContext device, VertexAndIndexDeviceBuffer deviceBuffers, System.Numerics.Matrix4x4 worldViewProj)
    {
      int layoutNumber = 4;
      device.VSSetShader(_vertexShader_PNC);
      device.PSSetShader(_pixelShader);
      device.IASetInputLayout(_renderLayouts[layoutNumber]);
      device.IASetPrimitiveTopology(PrimitiveTopology.TriangleList);

      if (deviceBuffers.ClipPlanes is not null)
      {
        for (int i = 0; i < Math.Min(6, deviceBuffers.ClipPlanes.Length); ++i)
        {
          _clipPlanes[i] = deviceBuffers.ClipPlanes[i];
        }
        device.UpdateSubresource(ref _clipPlanes, _evClipPlanes);
      }

      SetShaderMaterialVariables(device, deviceBuffers.Material);

      device.IASetVertexBuffers(0, new VertexBufferView(deviceBuffers.VertexBuffer, 48, 0));
      device.IASetIndexBuffer(deviceBuffers.IndexBuffer, Format.R32_UInt, 0);

      // _renderLayouts[layoutNumber].Pass.Apply();
      device.DrawIndexed(deviceBuffers.IndexCount, 0, 0);

      if (deviceBuffers.ClipPlanes is not null)
      {
        for (int i = 0; i < Math.Min(6, deviceBuffers.ClipPlanes.Length); ++i)
        {
          _clipPlanes[i] = new Plane();
        }
        device.UpdateSubresource(ref _clipPlanes, _evClipPlanes);
      }
    }


    private void DrawPositionNormalUIndexedTriangleBuffer(DeviceContext device, VertexAndIndexDeviceBuffer deviceBuffers, System.Numerics.Matrix4x4 worldViewProj)
    {
      int layoutNumber = 6;
      device.VSSetShader(_vertexShader_PNT1);
      device.PSSetShader(_pixelShader_T1);
      device.IASetInputLayout(_renderLayouts[layoutNumber]);
      device.IASetPrimitiveTopology(Vortice.Direct3D.PrimitiveTopology.TriangleList);

      // set clip planes
      if (deviceBuffers.ClipPlanes is not null)
      {
        for (int i = 0; i < Math.Min(6, deviceBuffers.ClipPlanes.Length); ++i)
        {
          _clipPlanes[i] = deviceBuffers.ClipPlanes[i];
        }
        device.UpdateSubresource(ref _clipPlanes, _evClipPlanes);
      }


      // set UColor texture
      var stream = device.Map(_textureFor1DColorProvider, 0, MapMode.WriteDiscard, Vortice.Direct3D11.MapFlags.None);
      // var stream = _textureFor1DColorProvider.Map(0, MapMode.WriteDiscard, SharpDX.Direct3D10.MapFlags.None);
      //stream.Seek(0, System.IO.SeekOrigin.Begin);
      var destSpan = stream.AsSpan<byte>(deviceBuffers.UColors.Length);
      var srcSpan = new Span<byte>(deviceBuffers.UColors, 0, deviceBuffers.UColors.Length);
      srcSpan.CopyTo(destSpan);
      device.Unmap(_textureFor1DColorProvider, 0);
      device.PSSetShaderResource(0, _textureFor1DColorProviderView);

      // set invalid color
      SetShaderMaterialVariables(device, deviceBuffers.Material); // note: the material's color must be set to the ColorProviders InvalidColor!

      // draw now

      device.IASetVertexBuffers(0, new VertexBufferView(deviceBuffers.VertexBuffer, 32, 0));
      device.IASetIndexBuffer(deviceBuffers.IndexBuffer, Format.R32_UInt, 0);

      device.DrawIndexed(deviceBuffers.IndexCount, 0, 0);


      // clear clip planes afterwards
      if (deviceBuffers.ClipPlanes is not null)
      {
        for (int i = 0; i < Math.Min(6, deviceBuffers.ClipPlanes.Length); ++i)
        {
          _clipPlanes[i] = new Plane();
        }
        device.UpdateSubresource(ref _clipPlanes, _evClipPlanes);
      }

    }

    private void DrawPositionNormalIndexedTriangleBuffer(DeviceContext device, VertexAndIndexDeviceBuffer deviceBuffers, System.Numerics.Matrix4x4 worldViewProj)
    {
      int layoutNumber = 3;
      device.VSSetShader(_vertexShader_PN);
      device.PSSetShader(_pixelShader);
      device.IASetInputLayout(_renderLayouts[layoutNumber]);
      device.IASetPrimitiveTopology(Vortice.Direct3D.PrimitiveTopology.TriangleList);

      SetShaderMaterialVariables(device, deviceBuffers.Material);

      device.IASetVertexBuffers(0, new VertexBufferView(deviceBuffers.VertexBuffer, 32, 0));
      device.IASetIndexBuffer(deviceBuffers.IndexBuffer, Format.R32_UInt, 0);

      device.DrawIndexed(deviceBuffers.IndexCount, 0, 0);
    }

    private void DrawPositionColorIndexedTriangleBufferNoMaterial(DeviceContext device, VertexAndIndexDeviceBufferNoMaterial deviceBuffers, System.Numerics.Matrix4x4 worldViewProj)
    {
      int layoutNumber = 1;
      device.VSSetShader(_vertexShader_OVERLAY_PC);
      device.PSSetShader(_pixelShader_OVERLAY);
      device.IASetInputLayout(_renderLayouts[layoutNumber]);
      device.IASetPrimitiveTopology(Vortice.Direct3D.PrimitiveTopology.TriangleList);

      device.IASetVertexBuffers(0, new VertexBufferView(deviceBuffers.VertexBuffer, 32, 0));
      device.IASetIndexBuffer(deviceBuffers.IndexBuffer, Format.R32_UInt, 0);

      device.DrawIndexed(deviceBuffers.IndexCount, 0, 0);
    }

    private void DrawPositionColorLineListBufferNoMaterial(DeviceContext device, VertexBufferNoMaterial deviceBuffers, System.Numerics.Matrix4x4 worldViewProj)
    {
      int layoutNumber = 1;
      device.VSSetShader(_vertexShader_OVERLAY_PC);
      device.PSSetShader(_pixelShader_OVERLAY);
      device.IASetInputLayout(_renderLayouts[layoutNumber]);
      device.IASetPrimitiveTopology(Vortice.Direct3D.PrimitiveTopology.LineList);

      device.IASetVertexBuffers(0, new VertexBufferView(deviceBuffers.VertexBuffer, (8 * 4), 0));

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

    private static Color4 ToColor4(Altaxo.Drawing.AxoColor color, double amplitude, double alpha)
    {
      float amp = (float)amplitude;
      return new Color4(color.ScR * amp, color.ScG * amp, color.ScB * amp, (float)alpha);
    }

    private static Vector4 ToVector4(Altaxo.Drawing.AxoColor color)
    {
      return new Vector4(color.ScR, color.ScG, color.ScB, color.ScA);
    }

    private static Color4 ToColor4(Altaxo.Drawing.AxoColor color)
    {
      return new Color4(color.ScR, color.ScG, color.ScB, color.ScA);
    }
  }
}
