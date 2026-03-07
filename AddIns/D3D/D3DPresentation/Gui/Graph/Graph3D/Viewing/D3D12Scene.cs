#region Copyright

/////////////////////////////////////////////////////////////////////////////
//    Altaxo:  a data processing and data plotting program
//    Copyright (C) 2002-2026 Dr. Dirk Lellinger
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
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using Altaxo.Drawing;
using Altaxo.Drawing.D3D;
using Altaxo.Geometry;
using Altaxo.Graph.Graph3D;
using Altaxo.Graph.Graph3D.Camera;
using Altaxo.Graph.Graph3D.GraphicsContext.D3D;
using Altaxo.Gui.Graph.Graph3D.Common;
using Altaxo.Shaders;
using SharpGen.Runtime;
using Vortice.Direct3D;
using Vortice.Direct3D12;
using Vortice.DXGI;
using Vortice.Mathematics;
using NMatrix4x4 = System.Numerics.Matrix4x4;

namespace Altaxo.Gui.Graph.Graph3D.Viewing
{
  public partial class D3D12Scene : ID3D12Scene
  {
    /// <summary>
    /// The _this triangle buffers. These buffers are used for current rendering
    /// 0: Position, 1: PositionColor, 2: PositionUV, 3: PositionNormal, 4: PositionNormalColor, 5: PositionNormalUV
    /// </summary>
    private List<VertexAndIndexDeviceBuffer>?[] _thisTriangleDeviceBuffers = new List<VertexAndIndexDeviceBuffer>?[7];

    /// <summary>
    /// The next triangle buffers prepared for a future frame swap.
    /// </summary>
    private List<VertexAndIndexDeviceBuffer>?[] _nextTriangleDeviceBuffers = new List<VertexAndIndexDeviceBuffer>?[7];

    /// <summary>
    /// Marker triangle geometry currently bound for rendering.
    /// </summary>
    private VertexAndIndexDeviceBufferNoMaterial? _markerGeometryTriangleDeviceBuffer;
    /// <summary>
    /// Marker line-list geometry currently bound for rendering.
    /// </summary>
    private VertexBufferNoMaterial? _markerGeometryLineListBuffer;
    /// <summary>
    /// Overlay triangle geometry currently bound for rendering.
    /// </summary>
    private VertexAndIndexDeviceBufferNoMaterial? _overlayGeometryTriangleDeviceBuffer;
    /// <summary>
    /// Overlay line-list geometry currently bound for rendering.
    /// </summary>
    private VertexBufferNoMaterial? _overlayGeometryLineListBuffer;

    /// <summary>
    /// Cached renderer this scene is currently attached to.
    /// </summary>
    private ID3D12RenderContext? _cachedRenderer;

    /// <summary>
    /// Cached host size in device pixels.
    /// </summary>
    private PointD2D _hostSize;
    /// <summary>
    /// Frame counter used for simple render-time bookkeeping.
    /// </summary>
    private int _renderCounter;

    // Transformation / lighting state mirrored from the D3D11 scene implementation
    /// <summary>
    /// Current world-view-projection matrix uploaded to shaders.
    /// </summary>
    private NMatrix4x4 _worldViewProj = NMatrix4x4.Identity;
    /// <summary>
    /// Current camera eye position uploaded to shaders.
    /// </summary>
    private Vector4 _eyePosition;
    /// <summary>
    /// Working material constant-buffer payload.
    /// </summary>
    private LightingHlsl.CbMaterial _material = new();
    /// <summary>
    /// Working lighting constant-buffer payload.
    /// </summary>
    private LightingHlsl.CbLights _lights = new();
    /// <summary>
    /// Lighting helper used to assemble shader light constants.
    /// </summary>
    private Lighting? _lighting;
    /// <summary>
    /// Working clip-plane constant-buffer payload.
    /// </summary>
    private LightingHlsl.CbClipPlanes _clipPlanes = new();

    /// <summary>
    /// Root signature shared by all scene pipeline states.
    /// </summary>
    private ID3D12RootSignature? _rootSignature;
    /// <summary>
    /// Pipeline state for overlay triangle rendering.
    /// </summary>
    private ID3D12PipelineState? _psoOverlayTriangles;
    /// <summary>
    /// Pipeline state for overlay line rendering.
    /// </summary>
    private ID3D12PipelineState? _psoOverlayLines;
    /// <summary>
    /// Pipeline state for position-normal triangle rendering.
    /// </summary>
    private ID3D12PipelineState? _psoPN;
    /// <summary>
    /// Pipeline state for position-normal-color triangle rendering.
    /// </summary>
    private ID3D12PipelineState? _psoPNC;
    /// <summary>
    /// Pipeline state for position-normal-U triangle rendering.
    /// </summary>
    private ID3D12PipelineState? _psoPNT1;

    /// <summary>
    /// Shader-visible descriptor heap for global CBV/SRV bindings.
    /// </summary>
    private ID3D12DescriptorHeap? _cbvHeap;
    /// <summary>
    /// Global constant buffer storing world-view-projection data.
    /// </summary>
    private ID3D12Resource? _bufWorldViewProj;
    /// <summary>
    /// Global constant buffer storing eye position data.
    /// </summary>
    private ID3D12Resource? _bufEyePosition;
    /// <summary>
    /// Global constant buffer storing material fallback data.
    /// </summary>
    private ID3D12Resource? _bufMaterial;
    /// <summary>
    /// Global constant buffer storing light data.
    /// </summary>
    private ID3D12Resource? _bufLights;
    /// <summary>
    /// Global constant buffer storing clip-plane data.
    /// </summary>
    private ID3D12Resource? _bufClipPlanes;
    /// <summary>
    /// GPU texture resource for 1D color-provider lookup.
    /// </summary>
    private ID3D12Resource? _textureFor1DColorProvider;
    /// <summary>
    /// Upload resource used to stage color-provider texture data.
    /// </summary>
    private ID3D12Resource? _textureFor1DColorProviderUpload;
    /// <summary>
    /// Indicates whether the color-provider texture is in shader-resource state.
    /// </summary>
    private bool _isColorProviderTextureInShaderReadState;
    /// <summary>
    /// Per-draw constant buffers allocated during command recording.
    /// </summary>
    private readonly List<ID3D12Resource> _perDrawConstantBuffers = new();
    /// <summary>
    /// Queue of resources scheduled for deferred disposal on the render thread.
    /// </summary>
    private readonly ConcurrentQueue<IDisposable> _pendingDisposals = new();

    /// <summary>
    /// Width of the 1D color-provider texture.
    /// </summary>
    private const int ColorProviderTextureWidth = 1024;
    /// <summary>
    /// Height of the 1D color-provider texture.
    /// </summary>
    private const int ColorProviderTextureHeight = 1;
    /// <summary>
    /// Depth of the color-provider texture footprint.
    /// </summary>
    private const int ColorProviderTextureDepth = 1;
    /// <summary>
    /// Bytes per texel for the color-provider texture format (<c>R32G32B32A32_Float</c>).
    /// </summary>
    private const int ColorProviderTextureBytesPerPixel = 16; // R32G32B32A32_Float
    /// <summary>
    /// Required D3D12 row-pitch alignment for texture upload data.
    /// </summary>
    private const int D3D12TextureDataPitchAlignment = 256;
    /// <summary>
    /// Default empty color-provider texture payload.
    /// </summary>
    private static readonly byte[] _emptyColorProviderTexture = new byte[ColorProviderTextureWidth * ColorProviderTextureBytesPerPixel];

    #region Members that do not utilize unmanaged resources, and thus are not associated with a 3D device

    /// <summary>
    /// Optional scene background color used for clearing the render target.
    /// </summary>
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
    /// The camera associated with the scene.
    /// </summary>
    private CameraBase? _altaxoCamera;

    /// <summary>The light settings from AltaxoBase.</summary>
    private LightSettings? _altaxoLightSettings;

    #endregion

    /// <inheritdoc/>
    public AxoColor? SceneBackgroundColor => _sceneBackgroundColor;

    /// <inheritdoc/>
    public void Attach(ID3D12RenderContext hostDevice)
    {
      if (_cachedRenderer is not null)
      {
        throw new InvalidOperationException("Try to attach to renderer without deattach former renderer!");
      }

      _cachedRenderer = hostDevice ?? throw new ArgumentNullException(nameof(hostDevice));
      _hostSize = new PointD2D(hostDevice.Width, hostDevice.Height);

      _material.SpecularExponent = 4;
      _material.SpecularIntensity = 1;
      _material.MetalnessValue = 0.75f;

      _lighting = new Lighting();
      _lighting.SetDefaultLighting();

      InitializePipelineResources(hostDevice.Device);

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

    /// <inheritdoc/>
    public void Attach(ComObject hostDevice, PointD2D hostSize)
    {
      throw new NotSupportedException($"Use {nameof(Attach)}({nameof(ID3D12RenderContext)}) for Direct3D12 scenes.");
    }

    /// <inheritdoc/>
    public void Detach()
    {
      // Dispose resources that were deferred from background geometry updates.
      DrainPendingDisposals();

      // Release queued triangle buffers not yet swapped into the active frame.
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

      // Release currently active triangle buffers.
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

      // Release marker/overlay buffers and all pipeline/global GPU resources.
      Disposer.RemoveAndDispose(ref _markerGeometryTriangleDeviceBuffer);
      Disposer.RemoveAndDispose(ref _markerGeometryLineListBuffer);
      Disposer.RemoveAndDispose(ref _overlayGeometryTriangleDeviceBuffer);
      Disposer.RemoveAndDispose(ref _overlayGeometryLineListBuffer);

      Disposer.RemoveAndDispose(ref _lighting);

      Disposer.RemoveAndDispose(ref _psoOverlayTriangles);
      Disposer.RemoveAndDispose(ref _psoOverlayLines);
      Disposer.RemoveAndDispose(ref _psoPN);
      Disposer.RemoveAndDispose(ref _psoPNC);
      Disposer.RemoveAndDispose(ref _psoPNT1);
      Disposer.RemoveAndDispose(ref _rootSignature);
      Disposer.RemoveAndDispose(ref _cbvHeap);
      Disposer.RemoveAndDispose(ref _bufWorldViewProj);
      Disposer.RemoveAndDispose(ref _bufEyePosition);
      Disposer.RemoveAndDispose(ref _bufMaterial);
      Disposer.RemoveAndDispose(ref _bufLights);
      Disposer.RemoveAndDispose(ref _bufClipPlanes);
      Disposer.RemoveAndDispose(ref _textureFor1DColorProvider);
      Disposer.RemoveAndDispose(ref _textureFor1DColorProviderUpload);

      foreach (var resource in _perDrawConstantBuffers)
      {
        resource.Dispose();
      }
      _perDrawConstantBuffers.Clear();

      _cachedRenderer = null;
    }

    #region IDisposable Support

    /// <summary>
    /// Indicates whether this instance has already been disposed.
    /// </summary>
    private bool _isDisposed;

    /// <summary>
    /// Releases managed and unmanaged resources for this scene.
    /// </summary>
    /// <param name="disposing">
    /// <see langword="true"/> to release managed and unmanaged resources; otherwise only unmanaged resources.
    /// </param>
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

        if (_cachedRenderer is not null)
        {
          Detach();
        }

        _isDisposed = true;
      }
    }

    /// <summary>
    /// Finalizes the scene instance.
    /// </summary>
    ~D3D12Scene()
    {
      Dispose(false);
    }

    /// <inheritdoc/>
    public void Dispose()
    {
      Dispose(true);
      GC.SuppressFinalize(this);
    }

    #endregion

    /// <inheritdoc/>
    public void Render()
    {
      // Ensure resources replaced on worker/UI paths are only destroyed on render thread.
      DrainPendingDisposals();

      // Material/clip per-draw constant buffers are rebuilt each frame.
      foreach (var resource in _perDrawConstantBuffers)
      {
        resource.Dispose();
      }
      _perDrawConstantBuffers.Clear();

      var renderer = _cachedRenderer;
      if (renderer is null)
      {
        throw new InvalidOperationException("Rendering failed because renderer is null");
      }
      if (_altaxoCamera is null)
      {
        throw new InvalidOperationException("Rendering failed because camera is null");
      }
      if (_altaxoDrawingGeometry is null)
      {
        throw new InvalidOperationException("Rendering failed because drawing is null");
      }

      // Atomically publish newly prepared geometry buffers.
      UseNextTriangleDeviceBuffers();

      _hostSize = new PointD2D(renderer.Width, renderer.Height);

      float time = _renderCounter / 100f;
      ++_renderCounter;

      NMatrix4x4 worldViewProjTr;

      if (_altaxoCamera is not null)
      {
        var cam = _altaxoCamera;
        var viewProjD3D = cam.GetViewProjectionMatrix(_hostSize.Y / _hostSize.X);
        worldViewProjTr = new NMatrix4x4(
                (float)viewProjD3D.M11, (float)viewProjD3D.M21, (float)viewProjD3D.M31, (float)viewProjD3D.M41,
                (float)viewProjD3D.M12, (float)viewProjD3D.M22, (float)viewProjD3D.M32, (float)viewProjD3D.M42,
                (float)viewProjD3D.M13, (float)viewProjD3D.M23, (float)viewProjD3D.M33, (float)viewProjD3D.M43,
                (float)viewProjD3D.M14, (float)viewProjD3D.M24, (float)viewProjD3D.M34, (float)viewProjD3D.M44
                );
      }
      else
      {
        var view = NMatrix4x4.CreateLookAt(new Vector3(0, 0, -1500), new Vector3(0, 0, 0), Vector3.UnitY);
        var proj = NMatrix4x4.CreatePerspectiveFieldOfView((float)Math.PI / 4.0f, (float)(_hostSize.X / _hostSize.Y), 0.1f, float.MaxValue);
        var viewProj = NMatrix4x4.Multiply(view, proj);

        worldViewProjTr = NMatrix4x4.Transpose(viewProj);
      }

      // Update frame-global constants from current camera and lighting.
      _worldViewProj = worldViewProjTr;
      _eyePosition = ToVector4(_altaxoCamera.EyePosition, 1f);

      if (_lighting is null || _altaxoLightSettings is null)
      {
        throw new InvalidOperationException("Rendering failed because lighting is null");
      }

      _lighting.SetLighting(_altaxoLightSettings, _altaxoCamera);
      _lighting.AssembleLightsInto(ref _lights);

      UpdateGlobalShaderVariables();

      // Bind global root signature/descriptor heap once per frame.
      var cmd = renderer.CommandList;
      cmd.SetGraphicsRootSignature(_rootSignature!);
      cmd.SetDescriptorHeaps(_cbvHeap!);

      SetGlobalRootDescriptorTables(renderer);

      // Draw solid scene geometry by vertex format group.
      foreach (var entry in Enumerate(_thisTriangleDeviceBuffers[1])) // Position-Color
      {
        DrawPositionColorIndexedTriangleBuffer(cmd, entry);
      }

      foreach (var entry in Enumerate(_thisTriangleDeviceBuffers[3])) // Position-Normal
      {
        DrawPositionNormalIndexedTriangleBuffer(cmd, entry);
      }

      foreach (var entry in Enumerate(_thisTriangleDeviceBuffers[4])) // Position-Normal-Color
      {
        DrawPositionNormalColorIndexedTriangleBuffer(cmd, entry);
      }

      foreach (var entry in Enumerate(_thisTriangleDeviceBuffers[6])) // Position-Normal-U
      {
        DrawPositionNormalUIndexedTriangleBuffer(cmd, entry);
      }

      // Draw marker and transient overlay geometry without material bindings.
      var markerTriangles = _markerGeometryTriangleDeviceBuffer;
      if (markerTriangles is not null)
      {
        DrawPositionColorIndexedTriangleBufferNoMaterial(cmd, markerTriangles);
      }

      var markerLines = _markerGeometryLineListBuffer;
      if (markerLines is not null && markerLines.VertexCount > 0)
      {
        DrawPositionColorLineListBufferNoMaterial(cmd, markerLines);
      }

      var overlayTriangles = _overlayGeometryTriangleDeviceBuffer;
      if (overlayTriangles is not null)
      {
        DrawPositionColorIndexedTriangleBufferNoMaterial(cmd, overlayTriangles);
      }

      var overlayLines = _overlayGeometryLineListBuffer;
      if (overlayLines is not null && overlayLines.VertexCount > 0)
      {
        DrawPositionColorLineListBufferNoMaterial(cmd, overlayLines);
      }

      _ = time;
    }

    /// <inheritdoc/>
    public void SetHostSize(PointD2D hostSize)
    {
      _hostSize = hostSize;
    }

    /// <inheritdoc/>
    public void Update(TimeSpan timeSpan)
    {
      // use sceneTime.TotalSeconds to update the scene here in dependence on the scene time
    }

    #region Helper

    // helper

    /// <summary>
    /// Converts a 3D point to a <see cref="Vector4"/> with an explicit fourth component.
    /// </summary>
    /// <param name="a">The input point.</param>
    /// <param name="c4">The fourth component value.</param>
    /// <returns>The converted vector.</returns>
    private static Vector4 ToVector4(PointD3D a, float c4)
    {
      return new Vector4((float)a.X, (float)a.Y, (float)a.Z, c4);
    }

    /// <summary>
    /// Converts a 3D vector to a <see cref="Vector4"/> with an explicit fourth component.
    /// </summary>
    /// <param name="a">The input vector.</param>
    /// <param name="c4">The fourth component value.</param>
    /// <returns>The converted vector.</returns>
    private static Vector4 ToVector4(VectorD3D a, float c4)
    {
      return new Vector4((float)a.X, (float)a.Y, (float)a.Z, c4);
    }

    /// <summary>
    /// Converts a 3D point to a homogeneous <see cref="Vector4"/> with <c>w=1</c>.
    /// </summary>
    /// <param name="a">The input point.</param>
    /// <returns>The converted vector.</returns>
    private static Vector4 ToVector4(PointD3D a)
    {
      return new Vector4((float)a.X, (float)a.Y, (float)a.Z, 1.0f);
    }

    /// <summary>
    /// Converts an <see cref="AxoColor"/> to <see cref="Color4"/> while applying amplitude and alpha overrides.
    /// </summary>
    /// <param name="color">The source color.</param>
    /// <param name="amplitude">The multiplicative amplitude for RGB channels.</param>
    /// <param name="alpha">The alpha value to assign.</param>
    /// <returns>The converted color.</returns>
    private static Color4 ToColor4(Altaxo.Drawing.AxoColor color, double amplitude, double alpha)
    {
      float amp = (float)amplitude;
      return new Color4(color.ScR * amp, color.ScG * amp, color.ScB * amp, (float)alpha);
    }

    /// <summary>
    /// Converts an <see cref="AxoColor"/> to a <see cref="Vector4"/>.
    /// </summary>
    /// <param name="color">The source color.</param>
    /// <returns>The converted vector.</returns>
    private static Vector4 ToVector4(Altaxo.Drawing.AxoColor color)
    {
      return new Vector4(color.ScR, color.ScG, color.ScB, color.ScA);
    }

    /// <summary>
    /// Converts an <see cref="AxoColor"/> to a <see cref="Color4"/>.
    /// </summary>
    /// <param name="color">The source color.</param>
    /// <returns>The converted color.</returns>
    private static Color4 ToColor4(Altaxo.Drawing.AxoColor color)
    {
      return new Color4(color.ScR, color.ScG, color.ScB, color.ScA);
    }

    /// <inheritdoc/>
    public void SetMarkerGeometry(D3DOverlayContext markerGeometry)
    {
      _altaxoMarkerGeometry = markerGeometry;
      BringMarkerGeometryIntoDeviceBuffers(markerGeometry);
    }

    /// <inheritdoc/>
    public void SetDrawing(D3DGraphicsContext drawing)
    {
      _altaxoDrawingGeometry = drawing;
      BringDrawingIntoBuffers(drawing);
    }

    /// <inheritdoc/>
    public void SetLighting(LightSettings lightSettings)
    {
      if (lightSettings is null)
      {
        throw new ArgumentNullException(nameof(lightSettings));
      }

      _altaxoLightSettings = lightSettings;
    }

    /// <inheritdoc/>
    public void SetCamera(CameraBase camera)
    {
      if (camera is null)
      {
        throw new ArgumentNullException(nameof(camera));
      }

      _altaxoCamera = camera;
    }

    /// <inheritdoc/>
    public void SetSceneBackColor(AxoColor? sceneBackColor)
    {
      _sceneBackgroundColor = sceneBackColor;
    }

    /// <inheritdoc/>
    public void SetOverlayGeometry(D3DOverlayContext overlayGeometry)
    {
      _altaxoOverlayGeometry = overlayGeometry;
      BringOverlayGeometryIntoDeviceBuffers(overlayGeometry);
    }

    /// <summary>
    /// Swaps prepared triangle buffers into the active set and disposes replaced buffers.
    /// </summary>
    private void UseNextTriangleDeviceBuffers()
    {
      // Swap each format bucket atomically and retire the previous active buffers.
      for (int i = 0; i < _nextTriangleDeviceBuffers.Length; ++i)
      {
        if (_nextTriangleDeviceBuffers[i] is not null)
        {
          var oldBuffers = _thisTriangleDeviceBuffers[i];
          _thisTriangleDeviceBuffers[i] = System.Threading.Interlocked.Exchange(ref _nextTriangleDeviceBuffers[i], null);

          if (oldBuffers is not null)
          {
            foreach (var entry in oldBuffers)
            {
              entry.Dispose();
            }
          }
        }
      }
    }

#nullable disable

    /// <summary>
    /// Initializes all pipeline states, root-signature resources, and global GPU resources.
    /// </summary>
    /// <param name="device">The D3D12 device used to create resources.</param>
    private void InitializePipelineResources(ID3D12Device device)
    {
      // Root signature defines all shader-visible bindings used by scene pipelines.
      _rootSignature = CreateRootSignature(device);

      // Load embedded compiled shaders.
      using var vsOverlayPc = LoadShaderBytecode("Altaxo.CompiledShaders.Lighting_VS_OVERLAY_PC.cso");
      using var vsPn = LoadShaderBytecode("Altaxo.CompiledShaders.Lighting_VS_PN.cso");
      using var vsPnc = LoadShaderBytecode("Altaxo.CompiledShaders.Lighting_VS_PNC.cso");
      using var vsPnt1 = LoadShaderBytecode("Altaxo.CompiledShaders.Lighting_VS_PNT1.cso");
      using var psOverlay = LoadShaderBytecode("Altaxo.CompiledShaders.Lighting_PS_OVERLAY.cso");
      using var ps = LoadShaderBytecode("Altaxo.CompiledShaders.Lighting_PS.cso");
      using var psT1 = LoadShaderBytecode("Altaxo.CompiledShaders.Lighting_PS_T1.cso");

      // Create pipeline states for each supported vertex format.
      _psoOverlayTriangles = CreatePipelineState(
        device,
        _rootSignature,
        vsOverlayPc.Bytecode,
        psOverlay.Bytecode,
        PrimitiveTopologyType.Triangle,
        new[]
        {
          new InputElementDescription("POSITION", 0, Format.R32G32B32A32_Float, 0, 0),
          new InputElementDescription("COLOR", 0, Format.R32G32B32A32_Float, 16, 0)
        });

      _psoOverlayLines = CreatePipelineState(
        device,
        _rootSignature,
        vsOverlayPc.Bytecode,
        psOverlay.Bytecode,
        PrimitiveTopologyType.Line,
        new[]
        {
          new InputElementDescription("POSITION", 0, Format.R32G32B32A32_Float, 0, 0),
          new InputElementDescription("COLOR", 0, Format.R32G32B32A32_Float, 16, 0)
        });

      _psoPN = CreatePipelineState(
        device,
        _rootSignature,
        vsPn.Bytecode,
        ps.Bytecode,
        PrimitiveTopologyType.Triangle,
        new[]
        {
          new InputElementDescription("POSITION", 0, Format.R32G32B32A32_Float, 0, 0),
          new InputElementDescription("NORMAL", 0, Format.R32G32B32A32_Float, 16, 0)
        });

      _psoPNC = CreatePipelineState(
        device,
        _rootSignature,
        vsPnc.Bytecode,
        ps.Bytecode,
        PrimitiveTopologyType.Triangle,
        new[]
        {
          new InputElementDescription("POSITION", 0, Format.R32G32B32A32_Float, 0, 0),
          new InputElementDescription("NORMAL", 0, Format.R32G32B32A32_Float, 16, 0),
          new InputElementDescription("COLOR", 0, Format.R32G32B32A32_Float, 32, 0)
        });

      _psoPNT1 = CreatePipelineState(
        device,
        _rootSignature,
        vsPnt1.Bytecode,
        psT1.Bytecode,
        PrimitiveTopologyType.Triangle,
        new[]
        {
          new InputElementDescription("POSITION", 0, Format.R32G32B32_Float, 0, 0),
          new InputElementDescription("NORMAL", 0, Format.R32G32B32_Float, 12, 0),
          new InputElementDescription("TEXCOORD", 0, Format.R32G32_Float, 24, 0)
        });

      // Create global constant buffers bound via descriptor tables.
      _bufWorldViewProj = CreateConstantBuffer(device, _worldViewProj);
      _bufEyePosition = CreateConstantBuffer(device, _eyePosition);
      _bufMaterial = CreateConstantBuffer(device, _material);
      _bufLights = CreateConstantBuffer(device, _lights);
      _bufClipPlanes = CreateConstantBuffer(device, _clipPlanes);

      // Create texture resources used for 1D color-provider sampling.
      _textureFor1DColorProvider = device.CreateCommittedResource(
        new HeapProperties(HeapType.Default),
        HeapFlags.None,
        ResourceDescription.Texture1D(Format.R32G32B32A32_Float, ColorProviderTextureWidth, 1, 1),
        ResourceStates.CopyDest);

      _textureFor1DColorProviderUpload = CreateUploadBuffer(device, (ulong)(AlignTo(ColorProviderTextureWidth * ColorProviderTextureBytesPerPixel, D3D12TextureDataPitchAlignment) * ColorProviderTextureHeight));

      _isColorProviderTextureInShaderReadState = false;

      _textureFor1DColorProviderUpload.SetData(new byte[ColorProviderTextureWidth * ColorProviderTextureBytesPerPixel]);

      // Build a shader-visible heap containing global CBV/SRV descriptors.
      _cbvHeap = device.CreateDescriptorHeap(new DescriptorHeapDescription(DescriptorHeapType.ConstantBufferViewShaderResourceViewUnorderedAccessView, 6, DescriptorHeapFlags.ShaderVisible));

      var handle = _cbvHeap.GetCPUDescriptorHandleForHeapStart();
      int increment = (int)device.GetDescriptorHandleIncrementSize(DescriptorHeapType.ConstantBufferViewShaderResourceViewUnorderedAccessView);

      device.CreateConstantBufferView(new ConstantBufferViewDescription(_bufWorldViewProj.GPUVirtualAddress, (uint)AlignConstantBufferSize((uint)System.Runtime.InteropServices.Marshal.SizeOf<NMatrix4x4>())), handle);
      handle.Ptr += (nuint)increment;
      device.CreateConstantBufferView(new ConstantBufferViewDescription(_bufEyePosition.GPUVirtualAddress, (uint)AlignConstantBufferSize((uint)System.Runtime.InteropServices.Marshal.SizeOf<Vector4>())), handle);
      handle.Ptr += (nuint)increment;
      device.CreateConstantBufferView(new ConstantBufferViewDescription(_bufMaterial.GPUVirtualAddress, (uint)AlignConstantBufferSize((uint)System.Runtime.InteropServices.Marshal.SizeOf<LightingHlsl.CbMaterial>())), handle);
      handle.Ptr += (nuint)increment;
      device.CreateConstantBufferView(new ConstantBufferViewDescription(_bufLights.GPUVirtualAddress, (uint)AlignConstantBufferSize((uint)System.Runtime.InteropServices.Marshal.SizeOf<LightingHlsl.CbLights>())), handle);
      handle.Ptr += (nuint)increment;
      device.CreateConstantBufferView(new ConstantBufferViewDescription(_bufClipPlanes.GPUVirtualAddress, (uint)AlignConstantBufferSize((uint)System.Runtime.InteropServices.Marshal.SizeOf<LightingHlsl.CbClipPlanes>())), handle);
      handle.Ptr += (nuint)increment;
      device.CreateShaderResourceView(_textureFor1DColorProvider, new ShaderResourceViewDescription
      {
        Format = Format.R32G32B32A32_Float,
        ViewDimension = Vortice.Direct3D12.ShaderResourceViewDimension.Texture1D,
        Shader4ComponentMapping = 5768,
        Texture1D = new Texture1DShaderResourceView { MipLevels = 1, MostDetailedMip = 0, ResourceMinLODClamp = 0 }
      }, handle);
    }

    /// <summary>
    /// Creates the root signature used by scene rendering shaders.
    /// </summary>
    /// <param name="device">The D3D12 device used to create the root signature.</param>
    /// <returns>The created root signature.</returns>
    private static ID3D12RootSignature CreateRootSignature(ID3D12Device device)
    {
      var ranges = new[]
      {
        new DescriptorRange(DescriptorRangeType.ConstantBufferView, 1, LightingHlsl.WorldViewProj_RegisterNumber),
        new DescriptorRange(DescriptorRangeType.ConstantBufferView, 1, LightingHlsl.EyePosition_RegisterNumber),
        new DescriptorRange(DescriptorRangeType.ConstantBufferView, 1, LightingHlsl.Material_RegisterNumber),
        new DescriptorRange(DescriptorRangeType.ConstantBufferView, 1, LightingHlsl.Lights_RegisterNumber),
        new DescriptorRange(DescriptorRangeType.ConstantBufferView, 1, LightingHlsl.ClipPlanes_RegisterNumber),
        new DescriptorRange(DescriptorRangeType.ShaderResourceView, 1, LightingHlsl.ColorGradient1DTexture_RegisterNumber),
      };

      var rootParams = new[]
      {
        new RootParameter(new RootDescriptorTable(ranges[0]), ShaderVisibility.Vertex),
        new RootParameter(new RootDescriptorTable(ranges[1]), ShaderVisibility.Pixel),
        new RootParameter(RootParameterType.ConstantBufferView, new RootDescriptor(LightingHlsl.Material_RegisterNumber, 0), ShaderVisibility.All),
        new RootParameter(new RootDescriptorTable(ranges[3]), ShaderVisibility.Pixel),
        new RootParameter(RootParameterType.ConstantBufferView, new RootDescriptor(LightingHlsl.ClipPlanes_RegisterNumber, 0), ShaderVisibility.Vertex),
        new RootParameter(new RootDescriptorTable(ranges[5]), ShaderVisibility.Pixel),
      };

      var staticSamplers = new[]
      {
        new StaticSamplerDescription
        {
          ShaderRegister = 0, // s0
          RegisterSpace = 0,
          ShaderVisibility = ShaderVisibility.Pixel,
          Filter = Filter.MinMagMipLinear,
          AddressU = TextureAddressMode.Clamp,
          AddressV = TextureAddressMode.Clamp,
          AddressW = TextureAddressMode.Clamp,
          MaxLOD = float.MaxValue
        }
      };

      var rootSignatureDesc = new RootSignatureDescription(
        RootSignatureFlags.AllowInputAssemblerInputLayout,
        rootParams,
        staticSamplers);

      var versionedRootSignatureDesc = new VersionedRootSignatureDescription(rootSignatureDesc);
      using var deviceConfiguration = device.QueryInterface<ID3D12DeviceConfiguration>();
      deviceConfiguration.SerializeVersionedRootSignature(versionedRootSignatureDesc, out Vortice.Direct3D.Blob signature, out Vortice.Direct3D.Blob error).CheckError();
      return device.CreateRootSignature(signature);
    }

    /// <summary>
    /// Creates a graphics pipeline state object.
    /// </summary>
    /// <param name="device">The D3D12 device used to create the pipeline state.</param>
    /// <param name="rootSignature">The root signature referenced by the pipeline.</param>
    /// <param name="vertexShader">Vertex shader bytecode.</param>
    /// <param name="pixelShader">Pixel shader bytecode.</param>
    /// <param name="topologyType">Primitive topology type for the pipeline.</param>
    /// <param name="inputElements">Input layout elements used by the vertex shader.</param>
    /// <returns>The created pipeline state object.</returns>
    private static ID3D12PipelineState CreatePipelineState(
      ID3D12Device device,
      ID3D12RootSignature rootSignature,
      byte[] vertexShader,
      byte[] pixelShader,
      PrimitiveTopologyType topologyType,
      InputElementDescription[] inputElements)
    {
      var psoDesc = new GraphicsPipelineStateDescription
      {
        RootSignature = rootSignature,
        VertexShader = vertexShader,
        PixelShader = pixelShader,
        BlendState = BlendDescription.Opaque,
        RasterizerState = RasterizerDescription.CullNone,
        DepthStencilState = DepthStencilDescription.Default,
        SampleMask = uint.MaxValue,
        PrimitiveTopologyType = topologyType,
        InputLayout = new InputLayoutDescription(inputElements),
        SampleDescription = new SampleDescription(1, 0),
        RenderTargetFormats = new[] { Format.B8G8R8A8_UNorm_SRgb },
        DepthStencilFormat = Format.D32_Float
      };

      return device.CreateGraphicsPipelineState(psoDesc);
    }

    /// <summary>
    /// Creates an upload-heap constant buffer initialized with the provided value.
    /// </summary>
    /// <typeparam name="T">The unmanaged data type stored in the constant buffer.</typeparam>
    /// <param name="device">The D3D12 device used to create the resource.</param>
    /// <param name="initialData">The initial data to write to the resource.</param>
    /// <returns>The created constant-buffer resource.</returns>
    private static ID3D12Resource CreateConstantBuffer<T>(ID3D12Device device, T initialData) where T : unmanaged
    {
      var sizeInBytes = AlignConstantBufferSize((uint)System.Runtime.InteropServices.Marshal.SizeOf<T>());
      var buffer = device.CreateCommittedResource(
        new HeapProperties(HeapType.Upload),
        HeapFlags.None,
        ResourceDescription.Buffer((ulong)sizeInBytes),
        ResourceStates.GenericRead);

      buffer.SetData(initialData);
      return buffer;
    }

    /// <summary>
    /// Aligns a constant-buffer size to the required 256-byte boundary.
    /// </summary>
    /// <param name="sizeInBytes">The unaligned size in bytes.</param>
    /// <returns>The aligned size in bytes.</returns>
    private static int AlignConstantBufferSize(uint sizeInBytes)
    {
      return (int)((sizeInBytes + 255u) & ~255u);
    }

    /// <summary>
    /// Aligns a value to the specified alignment.
    /// </summary>
    /// <param name="value">The value to align.</param>
    /// <param name="alignment">The required alignment.</param>
    /// <returns>The aligned value.</returns>
    private static int AlignTo(int value, int alignment)
    {
      return (value + alignment - 1) & ~(alignment - 1);
    }

    /// <summary>
    /// Updates frame-global constant buffers before issuing draw calls.
    /// </summary>
    private void UpdateGlobalShaderVariables()
    {
      if (_bufWorldViewProj is null || _bufEyePosition is null || _bufLights is null)
      {
        throw new InvalidOperationException("Rendering resources are not initialized");
      }

      _bufWorldViewProj.SetData(_worldViewProj);
      _bufEyePosition.SetData(_eyePosition);
      _bufLights.SetData(_lights);
    }

    /// <summary>
    /// Binds global descriptor tables required by the root signature.
    /// </summary>
    /// <param name="renderer">The current render context.</param>
    private void SetGlobalRootDescriptorTables(ID3D12RenderContext renderer)
    {
      if (_cbvHeap is null)
      {
        throw new InvalidOperationException("Descriptor heap is not initialized");
      }

      var cmd = renderer.CommandList;
      var gpuHandle = _cbvHeap.GetGPUDescriptorHandleForHeapStart();
      int incSize = (int)renderer.Device.GetDescriptorHandleIncrementSize(DescriptorHeapType.ConstantBufferViewShaderResourceViewUnorderedAccessView);

      cmd.SetGraphicsRootDescriptorTable(0, gpuHandle);
      gpuHandle.Ptr += (nuint)incSize;
      cmd.SetGraphicsRootDescriptorTable(1, gpuHandle);
      gpuHandle.Ptr += (nuint)(2 * incSize);
      cmd.SetGraphicsRootDescriptorTable(3, gpuHandle);
      gpuHandle.Ptr += (nuint)(2 * incSize);
      cmd.SetGraphicsRootDescriptorTable(5, gpuHandle);
    }

    /// <summary>
    /// Safely enumerates a potentially <see langword="null"/> list.
    /// </summary>
    /// <typeparam name="T">The element type.</typeparam>
    /// <param name="list">The list to enumerate.</param>
    /// <returns>An empty sequence if the list is <see langword="null"/>; otherwise the list sequence.</returns>
    private static IEnumerable<T> Enumerate<T>(List<T>? list)
    {
      return list is null ? Array.Empty<T>() : list;
    }

    /// <summary>
    /// Creates and binds per-draw material constants.
    /// </summary>
    /// <param name="cmd">The command list receiving root bindings.</param>
    /// <param name="material">The material values to upload.</param>
    private void SetShaderMaterialVariables(ID3D12GraphicsCommandList cmd, IMaterial material)
    {
      var renderer = _cachedRenderer;
      if (renderer is null)
      {
        throw new InvalidOperationException("Renderer is not initialized");
      }

      _material.SpecularIntensity = (float)material.PhongModelSpecularIntensity;
      _material.DiffuseIntensity = (float)material.PhongModelDiffuseIntensity;
      _material.SpecularExponent = (float)material.PhongModelSpecularExponent;
      _material.MetalnessValue = (float)material.Metalness;
      if (material.HasColor)
      {
        _material.DiffuseColor = ToColor4(material.Color.Color);
      }

      var buffer = CreateConstantBuffer(renderer.Device, _material);
      _perDrawConstantBuffers.Add(buffer);
      cmd.SetGraphicsRootConstantBufferView(2, buffer.GPUVirtualAddress);
    }

    /// <summary>
    /// Creates and binds per-draw clip-plane constants.
    /// </summary>
    /// <param name="cmd">The command list receiving root bindings.</param>
    /// <param name="planes">Optional clip planes; when <see langword="null"/>, all planes are cleared.</param>
    private void SetClipPlanes(ID3D12GraphicsCommandList cmd, Plane[]? planes)
    {
      var renderer = _cachedRenderer;
      if (renderer is null)
      {
        throw new InvalidOperationException("Renderer is not initialized");
      }

      _clipPlanes.Clear();
      if (planes is not null)
      {
        for (int i = 0; i < Math.Min(6, planes.Length); ++i)
        {
          _clipPlanes[i] = planes[i];
        }
      }

      var buffer = CreateConstantBuffer(renderer.Device, _clipPlanes);
      _perDrawConstantBuffers.Add(buffer);
      cmd.SetGraphicsRootConstantBufferView(4, buffer.GPUVirtualAddress);
    }

    /// <summary>
    /// Draws indexed triangles with position-color vertex format and material/clip bindings.
    /// </summary>
    /// <param name="cmd">The command list used for recording draw commands.</param>
    /// <param name="deviceBuffers">The device buffers to draw.</param>
    private void DrawPositionColorIndexedTriangleBuffer(ID3D12GraphicsCommandList cmd, VertexAndIndexDeviceBuffer deviceBuffers)
    {
      cmd.SetPipelineState(_psoOverlayTriangles!);
      cmd.IASetPrimitiveTopology(PrimitiveTopology.TriangleList);

      SetShaderMaterialVariables(cmd, deviceBuffers.Material);
      SetClipPlanes(cmd, deviceBuffers.ClipPlanes);

      var vb = new VertexBufferView(deviceBuffers.VertexBuffer.GPUVirtualAddress, (uint)(deviceBuffers.VertexCount * 32), 32);
      var ib = new IndexBufferView(deviceBuffers.IndexBuffer.GPUVirtualAddress, (uint)(deviceBuffers.IndexCount * sizeof(int)), Format.R32_UInt);
      cmd.IASetVertexBuffers(0, vb);
      cmd.IASetIndexBuffer(ib);
      cmd.DrawIndexedInstanced((uint)deviceBuffers.IndexCount, 1, 0, 0, 0);

      if (deviceBuffers.ClipPlanes is not null)
      {
        SetClipPlanes(cmd, null);
      }
    }

    /// <summary>
    /// Draws indexed triangles with position-normal-U data and optional color-provider texture upload.
    /// </summary>
    /// <param name="cmd">The command list used for recording draw commands.</param>
    /// <param name="deviceBuffers">The device buffers to draw.</param>
    private void DrawPositionNormalUIndexedTriangleBuffer(ID3D12GraphicsCommandList cmd, VertexAndIndexDeviceBuffer deviceBuffers)
    {
      cmd.SetPipelineState(_psoPNT1!);
      cmd.IASetPrimitiveTopology(PrimitiveTopology.TriangleList);

      SetClipPlanes(cmd, deviceBuffers.ClipPlanes);
      SetShaderMaterialVariables(cmd, deviceBuffers.Material);

      if (_textureFor1DColorProvider is not null)
      {
        if (deviceBuffers.UColors is not null)
        {
          UploadColorProviderTexture(cmd, deviceBuffers.UColors);
        }
        else if (!_isColorProviderTextureInShaderReadState)
        {
          UploadColorProviderTexture(cmd, _emptyColorProviderTexture);
        }
      }

      var vb = new VertexBufferView(deviceBuffers.VertexBuffer.GPUVirtualAddress, (uint)(deviceBuffers.VertexCount * 32), 32);
      var ib = new IndexBufferView(deviceBuffers.IndexBuffer.GPUVirtualAddress, (uint)(deviceBuffers.IndexCount * sizeof(int)), Format.R32_UInt);
      cmd.IASetVertexBuffers(0, vb);
      cmd.IASetIndexBuffer(ib);
      cmd.DrawIndexedInstanced((uint)deviceBuffers.IndexCount, 1, 0, 0, 0);

      if (deviceBuffers.ClipPlanes is not null)
      {
        SetClipPlanes(cmd, null);
      }
    }

    /// <summary>
    /// Draws indexed triangles with position-normal vertex format.
    /// </summary>
    /// <param name="cmd">The command list used for recording draw commands.</param>
    /// <param name="deviceBuffers">The device buffers to draw.</param>
    private void DrawPositionNormalIndexedTriangleBuffer(ID3D12GraphicsCommandList cmd, VertexAndIndexDeviceBuffer deviceBuffers)
    {
      cmd.SetPipelineState(_psoPN!);
      cmd.IASetPrimitiveTopology(PrimitiveTopology.TriangleList);

      SetShaderMaterialVariables(cmd, deviceBuffers.Material);
      SetClipPlanes(cmd, deviceBuffers.ClipPlanes);

      var vb = new VertexBufferView(deviceBuffers.VertexBuffer.GPUVirtualAddress, (uint)(deviceBuffers.VertexCount * 32), 32);
      var ib = new IndexBufferView(deviceBuffers.IndexBuffer.GPUVirtualAddress, (uint)(deviceBuffers.IndexCount * sizeof(int)), Format.R32_UInt);
      cmd.IASetVertexBuffers(0, vb);
      cmd.IASetIndexBuffer(ib);
      cmd.DrawIndexedInstanced((uint)deviceBuffers.IndexCount, 1, 0, 0, 0);

      if (deviceBuffers.ClipPlanes is not null)
      {
        SetClipPlanes(cmd, null);
      }
    }

    /// <summary>
    /// Draws indexed triangles with position-normal-color vertex format.
    /// </summary>
    /// <param name="cmd">The command list used for recording draw commands.</param>
    /// <param name="deviceBuffers">The device buffers to draw.</param>
    private void DrawPositionNormalColorIndexedTriangleBuffer(ID3D12GraphicsCommandList cmd, VertexAndIndexDeviceBuffer deviceBuffers)
    {
      cmd.SetPipelineState(_psoPNC!);
      cmd.IASetPrimitiveTopology(PrimitiveTopology.TriangleList);

      SetShaderMaterialVariables(cmd, deviceBuffers.Material);
      SetClipPlanes(cmd, deviceBuffers.ClipPlanes);

      var vb = new VertexBufferView(deviceBuffers.VertexBuffer.GPUVirtualAddress, (uint)(deviceBuffers.VertexCount * 48), 48);
      var ib = new IndexBufferView(deviceBuffers.IndexBuffer.GPUVirtualAddress, (uint)(deviceBuffers.IndexCount * sizeof(int)), Format.R32_UInt);
      cmd.IASetVertexBuffers(0, vb);
      cmd.IASetIndexBuffer(ib);
      cmd.DrawIndexedInstanced((uint)deviceBuffers.IndexCount, 1, 0, 0, 0);

      if (deviceBuffers.ClipPlanes is not null)
      {
        SetClipPlanes(cmd, null);
      }
    }

    /// <summary>
    /// Draws indexed position-color triangles without material bindings.
    /// </summary>
    /// <param name="cmd">The command list used for recording draw commands.</param>
    /// <param name="deviceBuffers">The device buffers to draw.</param>
    private void DrawPositionColorIndexedTriangleBufferNoMaterial(ID3D12GraphicsCommandList cmd, VertexAndIndexDeviceBufferNoMaterial deviceBuffers)
    {
      cmd.SetPipelineState(_psoOverlayTriangles!);
      cmd.IASetPrimitiveTopology(PrimitiveTopology.TriangleList);

      var vb = new VertexBufferView(deviceBuffers.VertexBuffer.GPUVirtualAddress, (uint)(deviceBuffers.VertexCount * 32), 32);
      var ib = new IndexBufferView(deviceBuffers.IndexBuffer.GPUVirtualAddress, (uint)(deviceBuffers.IndexCount * sizeof(int)), Format.R32_UInt);
      cmd.IASetVertexBuffers(0, vb);
      cmd.IASetIndexBuffer(ib);
      cmd.DrawIndexedInstanced((uint)deviceBuffers.IndexCount, 1, 0, 0, 0);
    }

    /// <summary>
    /// Draws position-color line-list geometry without material bindings.
    /// </summary>
    /// <param name="cmd">The command list used for recording draw commands.</param>
    /// <param name="deviceBuffers">The device buffers to draw.</param>
    private void DrawPositionColorLineListBufferNoMaterial(ID3D12GraphicsCommandList cmd, VertexBufferNoMaterial deviceBuffers)
    {
      cmd.SetPipelineState(_psoOverlayLines!);
      cmd.IASetPrimitiveTopology(PrimitiveTopology.LineList);

      var vb = new VertexBufferView(deviceBuffers.VertexBuffer.GPUVirtualAddress, (uint)(deviceBuffers.VertexCount * 32), 32);
      cmd.IASetVertexBuffers(0, vb);
      cmd.DrawInstanced((uint)deviceBuffers.VertexCount, 1, 0, 0);
    }

    /// <summary>
    /// Lightweight holder for loaded shader bytecode.
    /// </summary>
    private sealed class ShaderBytecodeHolder : IDisposable
    {
      /// <summary>
      /// Initializes a new instance of the <see cref="ShaderBytecodeHolder"/> class.
      /// </summary>
      /// <param name="bytecode">The shader bytecode payload.</param>
      public ShaderBytecodeHolder(byte[] bytecode) => Bytecode = bytecode;

      /// <summary>
      /// Gets the shader bytecode payload.
      /// </summary>
      public byte[] Bytecode { get; }

      /// <summary>
      /// Releases resources associated with this instance.
      /// </summary>
      public void Dispose() { }
    }

    /// <summary>
    /// Loads compiled shader bytecode embedded as an assembly resource.
    /// </summary>
    /// <param name="resourceName">The manifest resource name of the compiled shader.</param>
    /// <returns>A holder containing the loaded bytecode.</returns>
    private static ShaderBytecodeHolder LoadShaderBytecode(string resourceName)
    {
      using var stream = System.Reflection.Assembly.GetExecutingAssembly().GetManifestResourceStream(resourceName);
      if (stream is null)
      {
        throw new InvalidOperationException($"Compiled shader resource not found: {resourceName}");
      }

      byte[] bytes = new byte[stream.Length];
      _ = stream.Read(bytes, 0, bytes.Length);
      return new ShaderBytecodeHolder(bytes);
    }

    /// <summary>
    /// Converts drawing geometry buffers to D3D12 upload resources and schedules them for frame swap.
    /// </summary>
    /// <param name="altaxoDrawingGeometry">The source drawing geometry.</param>
    private void BringDrawingIntoBuffers(D3DGraphicsContext altaxoDrawingGeometry)
    {
      var renderer = _cachedRenderer;
      if (renderer is null)
      {
        return;
      }

      var device = renderer.Device;

      // Enumerate all triangle-buffer groups in shader input-layout order.
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
          {
            continue;
          }

          // Upload vertex/index streams into upload-heap resources.
          var vertexBuffer = CreateUploadBuffer(device, (ulong)altaxoTriangleBuffer.VertexStreamLength);
          vertexBuffer.SetData(altaxoTriangleBuffer.VertexStream.AsSpan(0, altaxoTriangleBuffer.VertexStreamLength / sizeof(float)));

          var indexBuffer = CreateUploadBuffer(device, (ulong)altaxoTriangleBuffer.IndexStreamLength);
          indexBuffer.SetData(altaxoTriangleBuffer.IndexStream.AsSpan(0, altaxoTriangleBuffer.IndexStreamLength / sizeof(int)));

          var indexCount = altaxoTriangleBuffer.TriangleCount * 3;

          // Convert clip-plane metadata into shader plane representation.
          Plane[] clipPlanes = null;
          if (altaxoBuffer.Key is MaterialPlusClippingKey)
          {
            var axoClipPlanes = ((MaterialPlusClippingKey)altaxoBuffer.Key).ClipPlanes;
            if (axoClipPlanes is not null)
            {
              clipPlanes = axoClipPlanes.Select(axoPlane => new Plane((float)axoPlane.X, (float)axoPlane.Y, (float)axoPlane.Z, (float)-axoPlane.W)).ToArray();
            }
          }

          // For U-color meshes, build the color-provider 1D texture payload.
          byte[] uColors = null;
          var material = altaxoBuffer.Key.Material;
          if (altaxoTriangleBuffer is PositionNormalUIndexedTriangleBuffer && altaxoBuffer.Key is MaterialPlusClippingPlusColorProviderKey)
          {
            var bufs = (PositionNormalUIndexedTriangleBuffer)altaxoTriangleBuffer;
            var colorProvider = ((MaterialPlusClippingPlusColorProviderKey)(altaxoBuffer.Key)).ColorProvider;
            var fColors = bufs.GetColorArrayForColorProvider(colorProvider);
            uColors = new byte[fColors.Length * 4];
            Buffer.BlockCopy(fColors, 0, uColors, 0, uColors.Length);
            material = material.WithColor(new NamedColor(colorProvider.GetAxoColor(double.NaN)));
          }

          newDeviceBuffers.Add(new VertexAndIndexDeviceBuffer(material: material, vertexBuffer: vertexBuffer, vertexCount: altaxoTriangleBuffer.VertexCount, indexBuffer: indexBuffer, indexCount: indexCount, clipPlanes: clipPlanes, uColors: uColors));
        }
        // Publish this bucket atomically; it becomes active during the next render swap.
        System.Threading.Interlocked.Exchange(ref _nextTriangleDeviceBuffers[i], newDeviceBuffers);
      }
    }

    /// <summary>
    /// Converts marker overlay geometry to device buffers and swaps them into active marker resources.
    /// </summary>
    /// <param name="overlayGeometry">The marker geometry source.</param>
    private void BringMarkerGeometryIntoDeviceBuffers(D3DOverlayContext overlayGeometry)
    // Rebuild marker triangle buffers and defer disposal of replaced resources.
    {
      var renderer = _cachedRenderer;
      if (renderer is null)
      // Rebuild marker line-list buffers and defer disposal of replaced resources.
      {
        return;
      }

      var device = renderer.Device;

      // Rebuild overlay triangle buffers and defer disposal of replaced resources.
      {
        var buf = (PositionColorIndexedTriangleBuffer)overlayGeometry.PositionColorIndexedTriangleBuffers;

        if (buf.TriangleCount > 0 && buf.VertexStreamLength > 0 && buf.IndexStreamLength > 0)
        // Rebuild overlay line-list buffers and defer disposal of replaced resources.
        {
          var vertexBuffer = CreateUploadBuffer(device, (ulong)buf.VertexStreamLength);
          vertexBuffer.SetData(buf.VertexStream.AsSpan(0, buf.VertexStreamLength / sizeof(float)));

          var indexBuffer = CreateUploadBuffer(device, (ulong)buf.IndexStreamLength);
          indexBuffer.SetData(buf.IndexStream.AsSpan(0, buf.IndexStreamLength / sizeof(int)));

          var indexCount = buf.TriangleCount * 3;

          var devBuffer = new VertexAndIndexDeviceBufferNoMaterial(vertexBuffer: vertexBuffer, vertexCount: buf.VertexCount, indexBuffer: indexBuffer, indexCount: indexCount);

          var oldBuffer = System.Threading.Interlocked.Exchange(ref _markerGeometryTriangleDeviceBuffer, devBuffer);
          EnqueueForDisposal(oldBuffer);
        }
        else
        {
          var oldBuffer = System.Threading.Interlocked.Exchange(ref _markerGeometryTriangleDeviceBuffer, null);
          EnqueueForDisposal(oldBuffer);
        }
      }

      {
        var buf = (PositionColorLineListBuffer)overlayGeometry.PositionColorLineListBuffer;

        if (buf.VertexCount > 0 && buf.VertexStreamLength > 0)
        {
          var vertexBuffer = CreateUploadBuffer(device, (ulong)buf.VertexStreamLength);
          vertexBuffer.SetData(buf.VertexStream.AsSpan(0, buf.VertexStreamLength / sizeof(float)));

          var oldBuffer = System.Threading.Interlocked.Exchange(
            ref _markerGeometryLineListBuffer,
            new VertexBufferNoMaterial(vertexBuffer: vertexBuffer, vertexCount: buf.VertexCount));
          EnqueueForDisposal(oldBuffer);
        }
        else
        {
          var oldBuffer = System.Threading.Interlocked.Exchange(ref _markerGeometryLineListBuffer, null);
          EnqueueForDisposal(oldBuffer);
        }
      }
    }

    /// <summary>
    /// Converts transient overlay geometry to device buffers and swaps them into active overlay resources.
    /// </summary>
    /// <param name="overlayGeometry">The overlay geometry source.</param>
    private void BringOverlayGeometryIntoDeviceBuffers(D3DOverlayContext overlayGeometry)
    {
      var renderer = _cachedRenderer;
      if (renderer is null)
      {
        return;
      }

      var device = renderer.Device;

      {
        var buf = (PositionColorIndexedTriangleBuffer)overlayGeometry.PositionColorIndexedTriangleBuffers;

        if (buf.TriangleCount > 0 && buf.VertexStreamLength > 0 && buf.IndexStreamLength > 0)
        {
          var vertexBuffer = CreateUploadBuffer(device, (ulong)buf.VertexStreamLength);
          vertexBuffer.SetData(buf.VertexStream.AsSpan(0, buf.VertexStreamLength / sizeof(float)));

          var indexBuffer = CreateUploadBuffer(device, (ulong)buf.IndexStreamLength);
          indexBuffer.SetData(buf.IndexStream.AsSpan(0, buf.IndexStreamLength / sizeof(int)));

          var indexCount = buf.TriangleCount * 3;

          var oldBuffer = System.Threading.Interlocked.Exchange(
            ref _overlayGeometryTriangleDeviceBuffer,
            new VertexAndIndexDeviceBufferNoMaterial(vertexBuffer: vertexBuffer, vertexCount: buf.VertexCount, indexBuffer: indexBuffer, indexCount: indexCount));
          EnqueueForDisposal(oldBuffer);
        }
        else
        {
          var oldBuffer = System.Threading.Interlocked.Exchange(ref _overlayGeometryTriangleDeviceBuffer, null);
          EnqueueForDisposal(oldBuffer);
        }
      }

      {
        var buf = (PositionColorLineListBuffer)overlayGeometry.PositionColorLineListBuffer;

        if (buf.VertexCount > 0 && buf.VertexStreamLength > 0)
        {
          var vertexBuffer = CreateUploadBuffer(device, (ulong)buf.VertexStreamLength);
          vertexBuffer.SetData(buf.VertexStream.AsSpan(0, buf.VertexStreamLength / sizeof(float)));

          var oldBuffer = System.Threading.Interlocked.Exchange(
            ref _overlayGeometryLineListBuffer,
            new VertexBufferNoMaterial(vertexBuffer: vertexBuffer, vertexCount: buf.VertexCount));

          EnqueueForDisposal(oldBuffer);
        }
        else
        {
          var oldBuffer = System.Threading.Interlocked.Exchange(ref _overlayGeometryLineListBuffer, null);
          EnqueueForDisposal(oldBuffer);
        }
      }
    }

    /// <summary>
    /// Enqueues a disposable resource for deferred disposal on the render thread.
    /// </summary>
    /// <param name="resource">The resource to dispose later.</param>
    private void EnqueueForDisposal(IDisposable? resource)
    {
      if (resource is not null)
      {
        _pendingDisposals.Enqueue(resource);
      }
    }

    /// <summary>
    /// Disposes all resources currently queued for deferred disposal.
    /// </summary>
    private void DrainPendingDisposals()
    {
      while (_pendingDisposals.TryDequeue(out var resource))
      {
        resource.Dispose();
      }
    }

    /// <summary>
    /// Uploads a 1D color-provider texture to the GPU and transitions it to shader-read state.
    /// </summary>
    /// <param name="cmd">The command list used for copy and barrier commands.</param>
    /// <param name="textureBytes">The tightly packed texture payload.</param>
    private void UploadColorProviderTexture(ID3D12GraphicsCommandList cmd, byte[] textureBytes)
    {
      if (_textureFor1DColorProvider is null || _textureFor1DColorProviderUpload is null)
      {
        return;
      }

      int expectedSize = ColorProviderTextureWidth * ColorProviderTextureBytesPerPixel;
      if (textureBytes.Length != expectedSize)
      {
        throw new InvalidOperationException($"Unexpected color provider texture size: {textureBytes.Length}, expected {expectedSize}.");
      }

      // Stage CPU texture bytes into upload heap memory.
      _textureFor1DColorProviderUpload.SetData(textureBytes);

      // Transition texture to copy destination if it is currently shader-readable.
      if (_isColorProviderTextureInShaderReadState)
      {
        var toCopyDest = new ResourceBarrier(new ResourceTransitionBarrier(_textureFor1DColorProvider, ResourceStates.PixelShaderResource, ResourceStates.CopyDest));
        cmd.ResourceBarrier(new[] { toCopyDest });
      }

      // Copy staged upload data into the 1D default-heap texture.
      int rowPitch = AlignTo(ColorProviderTextureWidth * ColorProviderTextureBytesPerPixel, D3D12TextureDataPitchAlignment);
      var srcLocation = new TextureCopyLocation(_textureFor1DColorProviderUpload, new PlacedSubresourceFootPrint
      {
        Offset = 0,
        Footprint = new SubresourceFootPrint
        {
          Format = Format.R32G32B32A32_Float,
          Width = ColorProviderTextureWidth,
          Height = ColorProviderTextureHeight,
          Depth = ColorProviderTextureDepth,
          RowPitch = (uint)rowPitch
        }
      });

      var dstLocation = new TextureCopyLocation(_textureFor1DColorProvider, 0);
      cmd.CopyTextureRegion(dstLocation, 0, 0, 0, srcLocation);

      // Transition the texture back to shader-readable state for sampling.
      var toShaderRead = new ResourceBarrier(new ResourceTransitionBarrier(_textureFor1DColorProvider, ResourceStates.CopyDest, ResourceStates.PixelShaderResource));
      cmd.ResourceBarrier(new[] { toShaderRead });

      _isColorProviderTextureInShaderReadState = true;
    }

    /// <summary>
    /// Creates an upload-heap buffer resource.
    /// </summary>
    /// <param name="device">The D3D12 device used to create the resource.</param>
    /// <param name="sizeInBytes">The buffer size in bytes.</param>
    /// <returns>The created upload resource.</returns>
    private static ID3D12Resource CreateUploadBuffer(ID3D12Device device, ulong sizeInBytes)
    {
      return device.CreateCommittedResource(
        new HeapProperties(HeapType.Upload),
        HeapFlags.None,
        ResourceDescription.Buffer(sizeInBytes),
        ResourceStates.GenericRead);
    }

    #endregion Helper
  }
}
