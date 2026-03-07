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
using System.Threading;
using Vortice;
using Vortice.Direct3D;
using Vortice.Direct3D12;
using Vortice.DXGI;
using Vortice.Mathematics;
using static Vortice.Direct3D12.D3D12;

namespace Altaxo.Gui.Graph.Graph3D.Common
{
  /// <summary>
  /// Offscreen Direct3D 12 renderer used for bitmap export without swap-chain or HWND dependency.
  /// </summary>
  public sealed class D3D12OffscreenRenderer : IDisposable, ID3D12RenderContext
  {
    private static readonly Format RenderTargetFormat = Format.B8G8R8A8_UNorm;
    private static readonly Format DepthStencilFormat = Format.D32_Float;

    private IDXGIFactory4? _factory;
    private ID3D12Device? _device;
    private ID3D12CommandQueue? _queue;
    private ID3D12CommandAllocator? _allocator;
    private ID3D12GraphicsCommandList? _cmd;

    private ID3D12DescriptorHeap? _rtvHeap;
    private ID3D12DescriptorHeap? _dsvHeap;
    private ID3D12Resource? _renderTarget;
    private ID3D12Resource? _depthStencil;
    private CpuDescriptorHandle _currentRtv;
    private CpuDescriptorHandle _currentDsv;

    private ID3D12Fence? _fence;
    private ulong _fenceValue;
    private AutoResetEvent? _fenceEvent;

    private int _width;
    private int _height;
    private ResourceStates _renderTargetState;
    private DateTime _timeBeginRendering;
    private bool _isDisposed;

    /// <summary>
    /// Gets or sets the scene rendered by this context.
    /// </summary>
    public ID3D12Scene? Scene
    {
      get => field;
      set
      {
        if (!ReferenceEquals(field, value))
        {
          field?.Detach();
          field = value;
          field?.Attach(this);
        }
      }
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="D3D12OffscreenRenderer"/> class.
    /// </summary>
    /// <param name="width">Render-target width in pixels.</param>
    /// <param name="height">Render-target height in pixels.</param>
    public D3D12OffscreenRenderer(int width, int height)
    {
      _width = Math.Max(width, 1);
      _height = Math.Max(height, 1);
      Initialize();
      _timeBeginRendering = DateTime.UtcNow;
    }

    /// <inheritdoc/>
    public ID3D12Device Device => _device ?? throw new InvalidOperationException("Renderer not initialized");

    /// <inheritdoc/>
    public ID3D12GraphicsCommandList CommandList => _cmd ?? throw new InvalidOperationException("Renderer not initialized");

    /// <inheritdoc/>
    public int Width => _width;

    /// <inheritdoc/>
    public int Height => _height;

    /// <inheritdoc/>
    public CpuDescriptorHandle CurrentRtv => _currentRtv;

    /// <summary>
    /// Renders one frame of the attached scene.
    /// </summary>
    public void Render()
    {
      Scene?.Update(DateTime.UtcNow - _timeBeginRendering);
      BeginFrame();

      var clearColor = Colors.White;
      var sceneBack = Scene?.SceneBackgroundColor;
      if (sceneBack.HasValue)
      {
        clearColor = new Color4(sceneBack.Value.ScR, sceneBack.Value.ScG, sceneBack.Value.ScB, sceneBack.Value.ScA);
      }

      Clear(clearColor.R, clearColor.G, clearColor.B, clearColor.A);
      Scene?.Render();
      EndFrame();
    }

    /// <summary>
    /// Reads back the latest rendered frame in tightly packed BGRA8 layout.
    /// </summary>
    /// <returns>Pixel bytes in BGRA8 order.</returns>
    public byte[] ReadBackBgraBytes()
    {
      var device = Device;
      var queue = _queue ?? throw new InvalidOperationException("Renderer not initialized");
      var source = _renderTarget ?? throw new InvalidOperationException("Render target not initialized");

      int rowPitch = AlignTo(_width * 4, 256);
      int totalBytes = rowPitch * _height;

      using var readback = device.CreateCommittedResource(
        new HeapProperties(HeapType.Readback),
        HeapFlags.None,
        ResourceDescription.Buffer((ulong)totalBytes),
        ResourceStates.CopyDest);

      _allocator!.Reset();
      _cmd!.Reset(_allocator, null);

      if (_renderTargetState != ResourceStates.CopySource)
      {
        _cmd.ResourceBarrier(new[]
        {
          new ResourceBarrier(new ResourceTransitionBarrier(source, _renderTargetState, ResourceStates.CopySource))
        });
        _renderTargetState = ResourceStates.CopySource;
      }

      var srcLocation = new TextureCopyLocation(source, 0);
      var dstLocation = new TextureCopyLocation(readback, new PlacedSubresourceFootPrint
      {
        Offset = 0,
        Footprint = new SubresourceFootPrint
        {
          Format = RenderTargetFormat,
          Width = (uint)_width,
          Height = (uint)_height,
          Depth = 1,
          RowPitch = (uint)rowPitch
        }
      });

      _cmd.CopyTextureRegion(dstLocation, 0, 0, 0, srcLocation);
      _cmd.Close();
      queue.ExecuteCommandList(_cmd);
      SignalAndWait();

      var readbackBytes = new byte[totalBytes];
      readback.GetData(readbackBytes);

      var data = new byte[_width * _height * 4];
      for (int y = 0; y < _height; ++y)
      {
        Buffer.BlockCopy(readbackBytes, y * rowPitch, data, y * _width * 4, _width * 4);
      }

      return data;
    }

    private void Initialize()
    {
      _factory = DXGI.CreateDXGIFactory2<IDXGIFactory4>(false);
      _factory.EnumAdapters1(0, out var adapter);
      D3D12CreateDevice(adapter, FeatureLevel.Level_11_0, out _device).CheckError();

      _queue = _device.CreateCommandQueue(new CommandQueueDescription(CommandListType.Direct));
      _allocator = _device.CreateCommandAllocator(CommandListType.Direct);
      _cmd = _device.CreateCommandList<ID3D12GraphicsCommandList>(0, CommandListType.Direct, _allocator, null);
      _cmd.Close();

      _fence = _device.CreateFence(0);
      _fenceValue = 0;
      _fenceEvent = new AutoResetEvent(false);

      CreateTargets();
    }

    private void CreateTargets()
    {
      _rtvHeap?.Dispose();
      _dsvHeap?.Dispose();
      _renderTarget?.Dispose();
      _depthStencil?.Dispose();

      var device = Device;
      _rtvHeap = device.CreateDescriptorHeap(new DescriptorHeapDescription(DescriptorHeapType.RenderTargetView, 1));
      _dsvHeap = device.CreateDescriptorHeap(new DescriptorHeapDescription(DescriptorHeapType.DepthStencilView, 1));

      _renderTarget = device.CreateCommittedResource(
        new HeapProperties(HeapType.Default),
        HeapFlags.None,
        ResourceDescription.Texture2D(RenderTargetFormat, (uint)_width, (uint)_height, 1, 0, 1, 0, ResourceFlags.AllowRenderTarget),
        ResourceStates.RenderTarget);

      _depthStencil = device.CreateCommittedResource(
        new HeapProperties(HeapType.Default),
        HeapFlags.None,
        ResourceDescription.Texture2D(DepthStencilFormat, (uint)_width, (uint)_height, 1, 0, 1, 0, ResourceFlags.AllowDepthStencil),
        ResourceStates.DepthWrite);

      _currentRtv = _rtvHeap.GetCPUDescriptorHandleForHeapStart();
      _currentDsv = _dsvHeap.GetCPUDescriptorHandleForHeapStart();

      var rtvDesc = new RenderTargetViewDescription
      {
        Format = Format.B8G8R8A8_UNorm_SRgb,
        ViewDimension = RenderTargetViewDimension.Texture2D,
        Texture2D = new Texture2DRenderTargetView { MipSlice = 0 }
      };
      device.CreateRenderTargetView(_renderTarget, rtvDesc, _currentRtv);

      device.CreateDepthStencilView(_depthStencil, new DepthStencilViewDescription
      {
        Format = DepthStencilFormat,
        ViewDimension = DepthStencilViewDimension.Texture2D
      }, _currentDsv);

      _renderTargetState = ResourceStates.RenderTarget;
    }

    private void BeginFrame()
    {
      _allocator!.Reset();
      _cmd!.Reset(_allocator, null);

      if (_renderTargetState != ResourceStates.RenderTarget)
      {
        _cmd.ResourceBarrier(new[]
        {
          new ResourceBarrier(new ResourceTransitionBarrier(_renderTarget!, _renderTargetState, ResourceStates.RenderTarget))
        });
        _renderTargetState = ResourceStates.RenderTarget;
      }

      _cmd.OMSetRenderTargets(_currentRtv, _currentDsv);
      _cmd.ClearDepthStencilView(_currentDsv, ClearFlags.Depth, 1.0f, 0);

      var vp = new Viewport(0, 0, _width, _height, 0.0f, 1.0f);
      _cmd.RSSetViewports(vp);
      _cmd.RSSetScissorRects(new RawRect(0, 0, _width, _height));
    }

    private void Clear(float r, float g, float b, float a)
    {
      _cmd!.ClearRenderTargetView(_currentRtv, new Color4(r, g, b, a));
    }

    private void EndFrame()
    {
      if (_renderTargetState != ResourceStates.CopySource)
      {
        _cmd!.ResourceBarrier(new[]
        {
          new ResourceBarrier(new ResourceTransitionBarrier(_renderTarget!, ResourceStates.RenderTarget, ResourceStates.CopySource))
        });
        _renderTargetState = ResourceStates.CopySource;
      }

      _cmd!.Close();
      _queue!.ExecuteCommandList(_cmd);
      SignalAndWait();
    }

    private void SignalAndWait()
    {
      _fenceValue++;
      _queue!.Signal(_fence!, _fenceValue);

      if (_fence!.CompletedValue < _fenceValue)
      {
        _fence.SetEventOnCompletion(_fenceValue, _fenceEvent!.SafeWaitHandle.DangerousGetHandle());
        _fenceEvent.WaitOne();
      }
    }

    private static int AlignTo(int value, int alignment)
    {
      return (value + alignment - 1) & ~(alignment - 1);
    }

    /// <inheritdoc/>
    public void Dispose()
    {
      if (_isDisposed)
      {
        return;
      }

      _isDisposed = true;

      Scene?.Detach();
      SignalAndWait();

      _rtvHeap?.Dispose();
      _dsvHeap?.Dispose();
      _renderTarget?.Dispose();
      _depthStencil?.Dispose();
      _allocator?.Dispose();
      _cmd?.Dispose();
      _queue?.Dispose();
      _fence?.Dispose();
      _factory?.Dispose();
      _device?.Dispose();
      _fenceEvent?.Dispose();

      _rtvHeap = null;
      _dsvHeap = null;
      _renderTarget = null;
      _depthStencil = null;
      _allocator = null;
      _cmd = null;
      _queue = null;
      _fence = null;
      _factory = null;
      _device = null;
      _fenceEvent = null;
    }
  }
}
