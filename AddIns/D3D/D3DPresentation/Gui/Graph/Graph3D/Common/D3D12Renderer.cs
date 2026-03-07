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
using Altaxo.Geometry;
using Vortice;
using Vortice.Direct3D;
using Vortice.Direct3D12;
using Vortice.DXGI;
using Vortice.Mathematics;
using static Vortice.Direct3D12.D3D12;

namespace Altaxo.Gui.Graph.Graph3D.Common
{
  /// <summary>
  /// Minimal Direct3D 12 renderer responsible for device creation, swap chain management,
  /// command list submission, and per-frame synchronization.
  /// </summary>
  public sealed class D3D12Renderer : IDisposable, ID3D12Renderer, ID3D12RenderContext
  {
    /// <summary>
    /// Native window handle for the host window.
    /// </summary>
    private IntPtr _hwnd;

    /// <summary>
    /// Current swap chain width in pixels.
    /// </summary>
    private int _width;

    /// <summary>
    /// Current swap chain height in pixels.
    /// </summary>
    private int _height;

    /// <summary>
    /// DXGI factory used to enumerate adapters and create the swap chain.
    /// </summary>
    private IDXGIFactory4? _factory;

    /// <summary>
    /// The Direct3D 12 device.
    /// </summary>
    private ID3D12Device? _device;

    /// <summary>
    /// The command queue used to execute command lists.
    /// </summary>
    private ID3D12CommandQueue? _queue;

    /// <summary>
    /// Swap chain presenting rendered frames.
    /// </summary>
    private IDXGISwapChain3? _swapChain;

    /// <summary>
    /// Descriptor heap holding render target view descriptors for the back buffers.
    /// </summary>
    private ID3D12DescriptorHeap? _rtvHeap;

    /// <summary>
    /// Descriptor heap holding the depth-stencil view descriptor.
    /// </summary>
    private ID3D12DescriptorHeap? _dsvHeap;

    /// <summary>
    /// Depth-stencil buffer resource.
    /// </summary>
    private ID3D12Resource? _depthStencil;

    /// <summary>
    /// Size in bytes of a single RTV descriptor.
    /// </summary>
    private int _rtvDescriptorSize;

    /// <summary>
    /// Swap chain back buffer resources.
    /// </summary>
    private ID3D12Resource[] _backBuffers = Array.Empty<ID3D12Resource>();

    /// <summary>
    /// Command allocator used to reset per-frame command recording.
    /// </summary>
    private ID3D12CommandAllocator? _allocator;

    /// <summary>
    /// Graphics command list used for recording commands for the current frame.
    /// </summary>
    private ID3D12GraphicsCommandList? _cmd;

    /// <summary>
    /// Index of the swap chain back buffer used for the current frame.
    /// </summary>
    private int _currentBackBufferIndex;

    /// <summary>
    /// Cached RTV descriptor handle for the current back buffer.
    /// </summary>
    private CpuDescriptorHandle _currentRtv;

    /// <summary>
    /// Cached DSV descriptor handle for the current frame.
    /// </summary>
    private CpuDescriptorHandle _currentDsv;

    /// <summary>
    /// Depth-stencil format used by renderer resources and pipeline states.
    /// </summary>
    private static readonly Format DepthStencilFormat = Format.D32_Float;

    /// <summary>
    /// Fence used to synchronize CPU and GPU.
    /// </summary>
    private ID3D12Fence? _fence;

    /// <summary>
    /// Monotonically increasing fence value.
    /// </summary>
    private ulong _fenceValue;

    /// <summary>
    /// Event used to wait on fence completion.
    /// </summary>
    private AutoResetEvent? _fenceEvent;

    /// <summary>
    /// Timestamp when rendering began.
    /// </summary>
    private DateTime _timeBeginRendering;
    /// <summary>
    /// Tracks disposal state.
    /// </summary>
    private bool _isDisposed;

    /// <summary>
    /// Gets or sets the scene that records draw commands for each frame.
    /// </summary>
    public ID3D12Scene? Scene
    {
      get => field;
      set
      {
        if (!object.ReferenceEquals(field, value))
        {
          field?.Detach();
          field = value;
          if (_hwnd != IntPtr.Zero)
          {
            field?.Attach(this); // otherwise, do attach the scene only after the renderer is initialized
          }
        }
      }
    }

    /// <summary>
    /// Initializes a new renderer instance for the specified host window.
    /// </summary>
    /// <param name="hwnd">Native window handle for swap chain creation.</param>
    public D3D12Renderer()
    {
    }

    /// <summary>
    /// Gets the initialized D3D12 device.
    /// </summary>
    public ID3D12Device Device => _device ?? throw new InvalidOperationException("Renderer not initialized");

    /// <summary>
    /// Gets the graphics command list used for the current frame.
    /// </summary>
    public ID3D12GraphicsCommandList CommandList => _cmd ?? throw new InvalidOperationException("Renderer not initialized");

    /// <summary>
    /// Gets the command queue used to execute command lists.
    /// </summary>
    public ID3D12CommandQueue CommandQueue => _queue ?? throw new InvalidOperationException("Renderer not initialized");

    /// <summary>
    /// Gets the current swap chain back buffer index for the frame.
    /// </summary>
    public int CurrentBackBufferIndex => _swapChain is null ? throw new InvalidOperationException("Renderer not initialized") : _currentBackBufferIndex;

    /// <summary>
    /// Gets the render target view descriptor handle for the current back buffer.
    /// </summary>
    public CpuDescriptorHandle CurrentRtv => _currentRtv;

    /// <summary>
    /// Gets the current swap chain back buffer resource.
    /// </summary>
    public ID3D12Resource CurrentRenderTarget => _backBuffers[CurrentBackBufferIndex];

    /// <summary>
    /// Gets the current swap chain width in pixels.
    /// </summary>
    public int Width => _width;

    /// <summary>
    /// Gets the current swap chain height in pixels.
    /// </summary>
    public int Height => _height;


    /// <summary>
    /// Creates the D3D12 device, command objects, synchronization primitives and swap chain.
    /// </summary>
    /// <param name="width">Initial swap chain width.</param>
    /// <param name="height">Initial swap chain height.</param>
    public void Initialize(IntPtr hWnd, int width, int height)
    {
      _hwnd = hWnd;
      _width = Math.Max(width, 1);
      _height = Math.Max(height, 1);

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

      CreateSwapChain(_width, _height);

      Scene?.Attach(this);
    }

    /// <summary>
    /// Renders one frame for the specified host window and size.
    /// </summary>
    /// <param name="hWnd">Host window handle.</param>
    /// <param name="width">Target width in pixels.</param>
    /// <param name="height">Target height in pixels.</param>
    public void Render(IntPtr hWnd, PointD2D size)
    {
      if (hWnd == IntPtr.Zero)
      {
        return;
      }

      int width = Math.Max((int)size.X, 1);
      int height = Math.Max((int)size.Y, 1);

      if (hWnd != _hwnd)
      {
        Initialize(hWnd, width, height);
        _timeBeginRendering = DateTime.UtcNow;
      }
      else if (_width != width || _height != height)
      {
        Resize(width, height);
      }

      Scene?.Update(DateTime.UtcNow - _timeBeginRendering);
      BeginFrame(out var rtv);

      var clearColor = Colors.White;
      var sceneBack = Scene?.SceneBackgroundColor;
      if (sceneBack.HasValue)
      {
        clearColor = new Color4(sceneBack.Value.ScR, sceneBack.Value.ScG, sceneBack.Value.ScB, sceneBack.Value.ScA);
      }

      Clear(rtv, clearColor.R, clearColor.G, clearColor.B, clearColor.A);
      Scene?.Render();
      EndFrame(rtv);
    }

    /// <summary>
    /// Creates or recreates the swap chain for the host window.
    /// </summary>
    /// <param name="width">Swap chain buffer width.</param>
    /// <param name="height">Swap chain buffer height.</param>
    private void CreateSwapChain(int width, int height)
    {
      if (_factory is null || _queue is null)
      {
        throw new InvalidOperationException();
      }

      var swapDesc = new SwapChainDescription1
      {
        Width = (uint)Math.Max(width, 1),
        Height = (uint)Math.Max(height, 1),
        Format = Format.B8G8R8A8_UNorm,
        BufferCount = 2,
        SampleDescription = new SampleDescription(1, 0),
        BufferUsage = Usage.RenderTargetOutput,
        SwapEffect = SwapEffect.FlipDiscard,
        Scaling = Scaling.Stretch,
        AlphaMode = AlphaMode.Ignore
      };

      using var sc1 = _factory.CreateSwapChainForHwnd(_queue, _hwnd, swapDesc);
      _swapChain = sc1.QueryInterface<IDXGISwapChain3>();

      CreateRenderTargets();
    }

    /// <summary>
    /// Creates the render target views for each swap chain back buffer.
    /// </summary>
    private void CreateRenderTargets()
    {
      if (_device is null || _swapChain is null)
      {
        throw new InvalidOperationException();
      }

      _rtvHeap?.Dispose();
      _dsvHeap?.Dispose();
      _depthStencil?.Dispose();

      _rtvHeap = _device.CreateDescriptorHeap(new DescriptorHeapDescription(DescriptorHeapType.RenderTargetView, 2));
      _dsvHeap = _device.CreateDescriptorHeap(new DescriptorHeapDescription(DescriptorHeapType.DepthStencilView, 1));
      _rtvDescriptorSize = (int)_device.GetDescriptorHandleIncrementSize(DescriptorHeapType.RenderTargetView);

      _backBuffers = new ID3D12Resource[2];
      var handle = _rtvHeap.GetCPUDescriptorHandleForHeapStart();

      for (int i = 0; i < 2; i++)
      {
        _backBuffers[i] = _swapChain.GetBuffer<ID3D12Resource>((uint)i);

        var rtvDesc = new RenderTargetViewDescription // neccessary for 'CreateRenderTargetView' to create an sRGB view instead of UNorm (to have gamma correction for the back buffer)
        {
          Format = Format.B8G8R8A8_UNorm_SRgb,  // sRGB statt null (= UNorm)
          ViewDimension = RenderTargetViewDimension.Texture2D,
          Texture2D = new Texture2DRenderTargetView { MipSlice = 0 }
        };

        _device.CreateRenderTargetView(_backBuffers[i], rtvDesc, handle);
        handle.Ptr += (nuint)_rtvDescriptorSize;
      }

      _depthStencil = _device.CreateCommittedResource(
        new HeapProperties(HeapType.Default),
        HeapFlags.None,
        ResourceDescription.Texture2D(DepthStencilFormat, (uint)Math.Max(_width, 1), (uint)Math.Max(_height, 1), 1, 0, 1, 0, ResourceFlags.AllowDepthStencil),
        ResourceStates.DepthWrite);

      _currentDsv = _dsvHeap.GetCPUDescriptorHandleForHeapStart();
      _device.CreateDepthStencilView(_depthStencil, new DepthStencilViewDescription
      {
        Format = DepthStencilFormat,
        ViewDimension = DepthStencilViewDimension.Texture2D
      }, _currentDsv);
    }

    /// <summary>
    /// Resizes swap chain buffers and recreates render target views.
    /// </summary>
    /// <param name="width">New width.</param>
    /// <param name="height">New height.</param>
    public void Resize(int width, int height)
    {
      if (_swapChain is null)
      {
        return;
      }

      _width = Math.Max(width, 1);
      _height = Math.Max(height, 1);

      WaitForGpu();

      foreach (var bb in _backBuffers)
      {
        bb.Dispose();
      }

      _swapChain.ResizeBuffers(2, (uint)_width, (uint)_height, Format.B8G8R8A8_UNorm, SwapChainFlags.None);
      CreateRenderTargets();
    }

    /// <summary>
    /// Begins recording commands for a new frame and transitions the back buffer to render-target state.
    /// </summary>
    /// <param name="rtv">Receives the render target view handle for the current back buffer.</param>
    public void BeginFrame(out CpuDescriptorHandle rtv)
    {
      if (_swapChain is null || _allocator is null || _cmd is null || _rtvHeap is null)
      {
        throw new InvalidOperationException();
      }

      _allocator.Reset();
      _cmd.Reset(_allocator, null);

      int index = (int)_swapChain.CurrentBackBufferIndex;
      _currentBackBufferIndex = index;

      var barrier = new ResourceBarrier(new ResourceTransitionBarrier(_backBuffers[index], ResourceStates.Present, ResourceStates.RenderTarget));
      _cmd.ResourceBarrier(new[] { barrier });

      var start = _rtvHeap.GetCPUDescriptorHandleForHeapStart();
      start.Ptr += (nuint)(index * _rtvDescriptorSize);
      rtv = start;
      _currentRtv = start;

      _cmd.OMSetRenderTargets(rtv, _currentDsv);
      _cmd.ClearDepthStencilView(_currentDsv, ClearFlags.Depth, 1.0f, 0);

      var vp = new Viewport(0, 0, _width, _height, 0.0f, 1.0f);
      _cmd.RSSetViewports(vp);
      _cmd.RSSetScissorRects(new RawRect(0, 0, _width, _height));

    }

    /// <summary>
    /// Clears the current render target.
    /// </summary>
    /// <param name="rtv">The render target view to clear.</param>
    /// <param name="r">Red component.</param>
    /// <param name="g">Green component.</param>
    /// <param name="b">Blue component.</param>
    /// <param name="a">Alpha component.</param>
    public void Clear(CpuDescriptorHandle rtv, float r, float g, float b, float a)
    {
      _cmd!.ClearRenderTargetView(rtv, new(r, g, b, a));
    }

    /// <summary>
    /// Ends the frame by transitioning the back buffer to present state, submitting the command list,
    /// and presenting the swap chain.
    /// </summary>
    /// <param name="rtv">The render target view for the current back buffer.</param>
    public void EndFrame(CpuDescriptorHandle rtv)
    {
      if (_swapChain is null || _cmd is null)
      {
        throw new InvalidOperationException();
      }

      int index = (int)_swapChain.CurrentBackBufferIndex;

      var barrier = new ResourceBarrier(new ResourceTransitionBarrier(_backBuffers[index], ResourceStates.RenderTarget, ResourceStates.Present));
      _cmd.ResourceBarrier(new[] { barrier });

      _cmd.Close();
      _queue!.ExecuteCommandList(_cmd);
      _swapChain.Present(1, PresentFlags.None);
      SignalAndWait();
    }

    /// <summary>
    /// Signals the fence and waits until the GPU has completed work up to the signaled value.
    /// </summary>
    private void SignalAndWait()
    {
      if (_fence is null || _queue is null || _fenceEvent is null)
      {
        throw new InvalidOperationException();
      }

      _fenceValue++;
      _queue.Signal(_fence, _fenceValue);

      if (_fence.CompletedValue < _fenceValue)
      {
        _fence.SetEventOnCompletion(_fenceValue, _fenceEvent.SafeWaitHandle.DangerousGetHandle());
        _fenceEvent.WaitOne();
      }
    }

    /// <summary>
    /// Waits until the GPU has completed all previously submitted work.
    /// </summary>
    private void WaitForGpu()
    {
      if (_fence is null || _queue is null || _fenceEvent is null || _fence.NativePointer == IntPtr.Zero || _queue.NativePointer == IntPtr.Zero)
      {
        return;
      }

      _fenceValue++;
      _queue.Signal(_fence, _fenceValue);
      _fence.SetEventOnCompletion(_fenceValue, _fenceEvent.SafeWaitHandle.DangerousGetHandle());
      _fenceEvent.WaitOne();
    }

    /// <summary>
    /// Disposes managed and unmanaged resources owned by the renderer.
    /// </summary>
    public void Dispose()
    {
      if (_isDisposed)
      {
        return;
      }

      _isDisposed = true;

      Scene?.Detach();
      WaitForGpu();

      foreach (var bb in _backBuffers)
      {
        bb.Dispose();
      }

      _rtvHeap?.Dispose();
      _dsvHeap?.Dispose();
      _depthStencil?.Dispose();
      _swapChain?.Dispose();
      _allocator?.Dispose();
      _cmd?.Dispose();
      _queue?.Dispose();
      _fence?.Dispose();
      _factory?.Dispose();
      _device?.Dispose();
      _fenceEvent?.Dispose();

      _rtvHeap = null;
      _dsvHeap = null;
      _depthStencil = null;
      _swapChain = null;
      _allocator = null;
      _cmd = null;
      _queue = null;
      _fence = null;
      _factory = null;
      _device = null;
      _fenceEvent = null;
      _backBuffers = Array.Empty<ID3D12Resource>();
    }
  }
}
