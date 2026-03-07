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
using System.Numerics;
using Altaxo.Drawing;
using Altaxo.Graph.Graph3D;
using Altaxo.Graph.Graph3D.Camera;
using Altaxo.Graph.Graph3D.GraphicsContext.D3D;
using Altaxo.Gui.Graph.Graph3D.Common;
using Vortice.Direct3D;
using Vortice.Direct3D12;
using Vortice.DXGI;
using Vortice.Mathematics;

namespace Altaxo.Gui.Graph.Graph3D.Viewing
{
  /// <summary>
  /// Sample Direct3D 12 scene that mirrors the DX11 sample: it draws two colored triangles
  /// using two different vertex shaders and animates via constant buffers.
  /// </summary>
  public sealed class D3D12SampleScene : ID3D12Scene
  {
    /// <summary>
    /// The renderer this scene is attached to.
    /// </summary>
    private ID3D12RenderContext? _renderer;

    /// <summary>
    /// Root signature describing how shaders access constant buffers.
    /// </summary>
    private ID3D12RootSignature? _rootSignature;

    /// <summary>
    /// Pipeline state for the first triangle (vertex shader <c>VS1</c>).
    /// </summary>
    private ID3D12PipelineState? _pso1;

    /// <summary>
    /// Pipeline state for the second triangle (vertex shader <c>VS2</c>).
    /// </summary>
    private ID3D12PipelineState? _pso2;

    /// <summary>
    /// Descriptor heap containing constant buffer views used by this scene.
    /// </summary>
    private ID3D12DescriptorHeap? _cbvHeap;

    /// <summary>
    /// Upload buffer holding interleaved position/color vertex data for the first triangle.
    /// </summary>
    private ID3D12Resource? _vertexBuffer1;

    /// <summary>
    /// Upload buffer holding interleaved position/color vertex data for the second triangle.
    /// </summary>
    private ID3D12Resource? _vertexBuffer2;

    /// <summary>
    /// Vertex buffer view for <see cref="_vertexBuffer1"/>.
    /// </summary>
    private VertexBufferView _vertexBufferView1;

    /// <summary>
    /// Vertex buffer view for <see cref="_vertexBuffer2"/>.
    /// </summary>
    private VertexBufferView _vertexBufferView2;

    /// <summary>
    /// Constant buffer providing the overlay color multiplier.
    /// </summary>
    private ID3D12Resource? _overlayColorBuffer;

    /// <summary>
    /// Constant buffer providing the projection matrix.
    /// </summary>
    private ID3D12Resource? _projectionBuffer;

    /// <summary>
    /// Reserved for mapped constant buffer access (not used with <c>SetData</c>-based updates).
    /// </summary>
    private IntPtr _overlayColorPtr;

    /// <summary>
    /// Reserved for mapped constant buffer access (not used with <c>SetData</c>-based updates).
    /// </summary>
    private IntPtr _projectionPtr;

    /// <summary>
    /// Current animated overlay color.
    /// </summary>
    private Color4 _overlayColor = new Color4(1.0f);

    /// <summary>
    /// Current animated projection matrix.
    /// </summary>
    private Matrix4x4 _projection = Matrix4x4.Identity;

    /// <summary>
    /// Timestamp used to compute elapsed scene time.
    /// </summary>
    private DateTime _startTimeUtc;

    /// <summary>
    /// Gets the optional scene background color.
    /// </summary>
    public AxoColor? SceneBackgroundColor => throw new NotImplementedException();

    /// <summary>
    /// Attaches this scene to a generic COM host device.
    /// </summary>
    /// <param name="hostDevice">Host device.</param>
    /// <param name="hostSize">Host surface size.</param>
    public void Attach(SharpGen.Runtime.ComObject hostDevice, Altaxo.Geometry.PointD2D hostSize)
    {
      throw new NotImplementedException();
    }

    /// <summary>
    /// Attaches the scene to a renderer and creates pipeline state and GPU resources.
    /// </summary>
    /// <param name="renderer">Renderer that provides the D3D12 device and command list.</param>
    public void Attach(ID3D12RenderContext renderer)
    {
      _renderer = renderer;

      var device = renderer.Device;

      using var vs1 = LoadShaderBytecode("Altaxo.CompiledShaders.SampleScene_VS1.cso");
      using var vs2 = LoadShaderBytecode("Altaxo.CompiledShaders.SampleScene_VS2.cso");
      using var ps = LoadShaderBytecode("Altaxo.CompiledShaders.SampleScene_PS.cso");

      var inputElements = new[]
      {
                new InputElementDescription("POSITION", 0, Format.R32G32B32A32_Float, 0, 0),
                new InputElementDescription("COLOR", 0, Format.R32G32B32A32_Float, 16, 0)
            };

      var ranges = new[]
      {
                new DescriptorRange(DescriptorRangeType.ConstantBufferView, 1, 0),
                new DescriptorRange(DescriptorRangeType.ConstantBufferView, 1, 1)
            };

      var rootParams = new[]
      {
                new RootParameter(new RootDescriptorTable(ranges[0]), ShaderVisibility.Vertex),
                new RootParameter(new RootDescriptorTable(ranges[1]), ShaderVisibility.Vertex)
            };

      using var rs = LoadShaderBytecode("Altaxo.CompiledShaders.SampleSceneRootSignature_RSMain.cso");
      _rootSignature = device.CreateRootSignature(rs.Bytecode);

      var psoDesc = new GraphicsPipelineStateDescription
      {
        RootSignature = _rootSignature,
        BlendState = BlendDescription.Opaque,
        RasterizerState = RasterizerDescription.CullNone,
        DepthStencilState = DepthStencilDescription.None,
        SampleMask = uint.MaxValue,
        PrimitiveTopologyType = PrimitiveTopologyType.Triangle,
        InputLayout = new InputLayoutDescription(inputElements),
        SampleDescription = new SampleDescription(1, 0),
        RenderTargetFormats = new[] { Format.B8G8R8A8_UNorm }
      };

      psoDesc.VertexShader = vs1.Bytecode;
      psoDesc.PixelShader = ps.Bytecode;
      _pso1 = device.CreateGraphicsPipelineState(psoDesc);

      psoDesc.VertexShader = vs2.Bytecode;
      _pso2 = device.CreateGraphicsPipelineState(psoDesc);

      CreateVertexBuffers(device);
      CreateConstantBuffers(device);
      CreateCbvHeap(device);
    }

    /// <summary>
    /// Detaches the scene and releases GPU resources created in <see cref="Attach"/>.
    /// </summary>
    public void Detach()
    {
      if (_overlayColorBuffer is not null)
      {
        _overlayColorBuffer.Unmap(0);
        _overlayColorPtr = IntPtr.Zero;
      }

      if (_projectionBuffer is not null)
      {
        _projectionBuffer.Unmap(0);
        _projectionPtr = IntPtr.Zero;
      }

      _vertexBuffer1?.Dispose();
      _vertexBuffer2?.Dispose();
      _overlayColorBuffer?.Dispose();
      _projectionBuffer?.Dispose();
      _cbvHeap?.Dispose();
      _pso1?.Dispose();
      _pso2?.Dispose();
      _rootSignature?.Dispose();

      _renderer = null;
    }

    /// <summary>
    /// Updates animated constants based on elapsed time.
    /// </summary>
    /// <param name="timeSpan">Elapsed time since rendering started.</param>
    public void Update(TimeSpan timeSpan)
    {
      if (_renderer is null)
      {
        return;
      }

      float t = (float)(0.5 * (1 + Math.Sin(timeSpan.TotalSeconds)));
      _overlayColor = new Color4(t, t, t, t);
      _projection = Matrix4x4.CreateRotationY((float)(0.5 * Math.Sin(timeSpan.TotalSeconds * 3)));

      _overlayColorBuffer?.SetData(_overlayColor);
      _projectionBuffer?.SetData(_projection);
    }

    /// <summary>
    /// Records draw commands for the current frame.
    /// </summary>
    public void Render()
    {
      if (_renderer is null || _rootSignature is null || _pso1 is null || _pso2 is null || _cbvHeap is null)
      {
        return;
      }

      var cmd = _renderer.CommandList;
      cmd.OMSetRenderTargets(_renderer.CurrentRtv);
      cmd.SetGraphicsRootSignature(_rootSignature);
      cmd.SetDescriptorHeaps(_cbvHeap);
      cmd.IASetPrimitiveTopology(PrimitiveTopology.TriangleList);

      var gpuHandle = _cbvHeap.GetGPUDescriptorHandleForHeapStart();
      int incSize = (int)_renderer.Device.GetDescriptorHandleIncrementSize(DescriptorHeapType.ConstantBufferViewShaderResourceViewUnorderedAccessView);

      cmd.SetGraphicsRootDescriptorTable(0, gpuHandle);
      gpuHandle.Ptr += (nuint)incSize;
      cmd.SetGraphicsRootDescriptorTable(1, gpuHandle);

      cmd.SetPipelineState(_pso1);
      cmd.IASetVertexBuffers(0, _vertexBufferView1);
      cmd.DrawInstanced(3, 1, 0, 0);

      cmd.SetPipelineState(_pso2);
      cmd.IASetVertexBuffers(0, _vertexBufferView2);
      cmd.DrawInstanced(3, 1, 0, 0);
    }

    /// <summary>
    /// Creates the vertex buffers used to draw the two triangles.
    /// </summary>
    /// <param name="device">Device used to allocate resources.</param>
    private void CreateVertexBuffers(ID3D12Device device)
    {
      float[] vertices1 =
      {
                0.0f, 0.5f, 0.5f, 1.0f,  1.0f, 0.0f, 0.0f, 1.0f,
                0.5f, -0.5f, 0.5f, 1.0f,  0.0f, 1.0f, 0.0f, 1.0f,
                -0.5f, -0.5f, 0.5f, 1.0f,  0.0f, 0.0f, 1.0f, 1.0f
            };

      float[] vertices2 =
      {
                -0.5f, 0.5f, 0.5f, 1.0f,  0.0f, 1.0f, 0.0f, 1.0f,
                0.5f, 0.5f, 0.5f, 1.0f,  1.0f, 0.0f, 0.0f, 1.0f,
                0.0f, -0.5f, 0.5f, 1.0f,  0.0f, 0.0f, 1.0f, 1.0f
            };

      (_vertexBuffer1, _vertexBufferView1) = CreateUploadBuffer(device, vertices1);
      (_vertexBuffer2, _vertexBufferView2) = CreateUploadBuffer(device, vertices2);
    }

    /// <summary>
    /// Creates an upload-heap buffer initialized with the specified vertex data.
    /// </summary>
    /// <param name="device">Device used to allocate resources.</param>
    /// <param name="data">Vertex data to upload.</param>
    /// <param name="view">Receives the corresponding vertex buffer view.</param>
    /// <returns>The created upload buffer resource.</returns>
    private static (ID3D12Resource buffer, VertexBufferView view) CreateUploadBuffer(ID3D12Device device, ReadOnlySpan<float> data)
    {
      int sizeInBytes = data.Length * sizeof(float);
      var buffer = device.CreateCommittedResource(
          new HeapProperties(HeapType.Upload),
          HeapFlags.None,
          ResourceDescription.Buffer((ulong)sizeInBytes),
          ResourceStates.GenericRead);

      buffer.SetData(data);

      var view = new VertexBufferView
      {
        BufferLocation = buffer.GPUVirtualAddress,
        StrideInBytes = 32,
        SizeInBytes = (uint)sizeInBytes
      };

      return (buffer, view);
    }

    /// <summary>
    /// Creates and initializes the constant buffer resources.
    /// </summary>
    /// <param name="device">Device used to allocate resources.</param>
    private void CreateConstantBuffers(ID3D12Device device)
    {
      _overlayColorBuffer = CreateMappedUploadConstantBuffer(device, 256, out _overlayColorPtr);
      _projectionBuffer = CreateMappedUploadConstantBuffer(device, 256, out _projectionPtr);

      _overlayColorBuffer.SetData(_overlayColor);
      _projectionBuffer.SetData(_projection);
    }

    /// <summary>
    /// Creates an upload-heap buffer sized for constant buffer usage.
    /// </summary>
    /// <param name="device">Device used to allocate resources.</param>
    /// <param name="sizeInBytes">Size of the buffer in bytes (typically 256-byte aligned).</param>
    /// <param name="mappedPtr">Receives a mapped pointer if mapping is performed (unused here).</param>
    /// <returns>The created buffer resource.</returns>
    private static ID3D12Resource CreateMappedUploadConstantBuffer(ID3D12Device device, int sizeInBytes, out IntPtr mappedPtr)
    {
      var buffer = device.CreateCommittedResource(
          new HeapProperties(HeapType.Upload),
          HeapFlags.None,
          ResourceDescription.Buffer((ulong)sizeInBytes),
          ResourceStates.GenericRead);

      mappedPtr = IntPtr.Zero;
      return buffer;
    }

    /// <summary>
    /// Creates a shader-visible descriptor heap and populates it with CBVs for the scene constant buffers.
    /// </summary>
    /// <param name="device">Device used to allocate descriptors.</param>
    private void CreateCbvHeap(ID3D12Device device)
    {
      _cbvHeap = device.CreateDescriptorHeap(new DescriptorHeapDescription(DescriptorHeapType.ConstantBufferViewShaderResourceViewUnorderedAccessView, 2, DescriptorHeapFlags.ShaderVisible));

      var handle = _cbvHeap.GetCPUDescriptorHandleForHeapStart();
      device.CreateConstantBufferView(new ConstantBufferViewDescription(_overlayColorBuffer!.GPUVirtualAddress, 256), handle);

      handle.Ptr += device.GetDescriptorHandleIncrementSize(DescriptorHeapType.ConstantBufferViewShaderResourceViewUnorderedAccessView);
      device.CreateConstantBufferView(new ConstantBufferViewDescription(_projectionBuffer!.GPUVirtualAddress, 256), handle);
    }

    /// <summary>
    /// Simple holder type used to scope shader bytecode lifetime.
    /// </summary>
    private sealed class ShaderBytecodeHolder : IDisposable
    {
      /// <summary>
      /// Initializes a new instance.
      /// </summary>
      /// <param name="bytecode">Compiled shader bytecode.</param>
      public ShaderBytecodeHolder(byte[] bytecode) => Bytecode = bytecode;

      /// <summary>
      /// Gets the compiled shader bytecode.
      /// </summary>
      public byte[] Bytecode { get; }

      /// <summary>
      /// Disposes the instance.
      /// </summary>
      public void Dispose() { }
    }

    /// <summary>
    /// Loads compiled shader bytecode from an embedded resource.
    /// </summary>
    /// <param name="resourceName">Fully qualified embedded resource name.</param>
    /// <returns>The loaded shader bytecode.</returns>
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
    /// Sets the host size.
    /// </summary>
    /// <param name="hostSize">New host size.</param>
    public void SetHostSize(Altaxo.Geometry.PointD2D hostSize)
    {

    }

    /// <summary>
    /// Disposes this scene.
    /// </summary>
    public void Dispose()
    {

    }

    /// <summary>
    /// Sets marker geometry.
    /// </summary>
    /// <param name="markerGeometry">Marker geometry.</param>
    public void SetMarkerGeometry(D3DOverlayContext markerGeometry)
    {
    }

    /// <summary>
    /// Sets drawing geometry.
    /// </summary>
    /// <param name="drawing">Drawing context.</param>
    public void SetDrawing(D3DGraphicsContext drawing)
    {
    }

    /// <summary>
    /// Sets lighting settings.
    /// </summary>
    /// <param name="lightSettings">Lighting settings.</param>
    public void SetLighting(LightSettings lightSettings)
    {
    }

    /// <summary>
    /// Sets the camera.
    /// </summary>
    /// <param name="camera">Camera instance.</param>
    public void SetCamera(CameraBase camera)
    {
    }

    /// <summary>
    /// Sets the scene background color.
    /// </summary>
    /// <param name="sceneBackColor">Background color.</param>
    public void SetSceneBackColor(AxoColor? sceneBackColor)
    {
    }

    /// <summary>
    /// Sets overlay geometry.
    /// </summary>
    /// <param name="overlayGeometry">Overlay geometry.</param>
    public void SetOverlayGeometry(D3DOverlayContext overlayGeometry)
    {
    }
  }
}
