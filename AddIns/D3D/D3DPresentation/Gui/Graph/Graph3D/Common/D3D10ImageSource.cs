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
// -----------------------------------------------------------------------------
// Original code from SlimMath project. http://code.google.com/p/slimmath/
// Greetings to SlimDX Group. Original code published with the following license:
// -----------------------------------------------------------------------------
/*
* Copyright (c) 2007-2011 SlimDX Group
*
* Permission is hereby granted, free of charge, to any person obtaining a copy
* of this software and associated documentation files (the "Software"), to deal
* in the Software without restriction, including without limitation the rights
* to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
* copies of the Software, and to permit persons to whom the Software is
* furnished to do so, subject to the following conditions:
*
* The above copyright notice and this permission notice shall be included in
* all copies or substantial portions of the Software.
*
* THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
* IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
* FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
* AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
* LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
* OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
* THE SOFTWARE.
*/

namespace Altaxo.Gui.Graph.Graph3D.Common
{
  using SharpDX.Direct3D9;
  using System;
  using System.Runtime.InteropServices;
  using System.Windows;
  using System.Windows.Interop;

  public class D3D10ImageSource : D3DImage, IDisposable
  {
    [DllImport("user32.dll", SetLastError = false)]
    private static extern IntPtr GetDesktopWindow();

    private static int _numberOfActiveClients;
    private static Direct3DEx _d3DContext;
    private static DeviceEx _d3DDevice;

    private Texture _renderTarget;

    public string Name { get; private set; }
    public int InstanceID { get; private set; }
    private static int _instanceCounter;

    private bool _isDisposed;

    public D3D10ImageSource() : this("Unnamed")
    {
    }

    public D3D10ImageSource(string name)
    {
      InstanceID = ++_instanceCounter;
      Name = name;

      // System.Diagnostics.Debug.WriteLine("D3DImageSource.ctor Name={0}, Id={1}", Name, InstanceID);

      if (1 == System.Threading.Interlocked.Increment(ref _numberOfActiveClients))
      {
        StartD3D();
      }
    }

    ~D3D10ImageSource()
    {
      Dispose(false);
    }

    public void Dispose()
    {
      Dispose(true);
    }

    public void Dispose(bool disposing)
    {
      // System.Diagnostics.Debug.WriteLine("D3DImageSource.Dispose Name={0}, Id={1}", Name, InstanceID);

      if (!_isDisposed)
      {
        _isDisposed = true;
        Disposer.RemoveAndDispose(ref this._renderTarget);
        if (0 == System.Threading.Interlocked.Decrement(ref _numberOfActiveClients))
        {
          EndD3D();
        }
      }
    }

    public void InvalidateD3DImage()
    {
      if (_isDisposed)
        throw new ObjectDisposedException(this.GetType().Name);

      if (this._renderTarget != null)
      {
        base.Lock();
        base.AddDirtyRect(new Int32Rect(0, 0, base.PixelWidth, base.PixelHeight));
        base.Unlock();
      }

      // System.Diagnostics.Debug.WriteLine("D3DImageSource.InvalidateD3DImage Name={0}, Id={1}", Name, InstanceID);
    }

    public void SetRenderTargetDX10(SharpDX.Direct3D10.Texture2D renderTarget)
    {
      if (_isDisposed)
        throw new ObjectDisposedException(this.GetType().Name);

      // System.Diagnostics.Debug.WriteLine("D3DImageSource.SetRenderTarget Name={0}, Id={1}, renderTarget={2}", Name, InstanceID, renderTarget);

      if (this._renderTarget != null)
      {
        Disposer.RemoveAndDispose(ref this._renderTarget);

        base.Lock();
        base.SetBackBuffer(D3DResourceType.IDirect3DSurface9, IntPtr.Zero);
        base.Unlock();
      }

      if (renderTarget != null)
      {
        if (!IsShareable(renderTarget))
          throw new ArgumentException("Texture must be created with ResourceOptionFlags.Shared");

        Format format = D3D10ImageSource.TranslateFormat(renderTarget);
        if (format == Format.Unknown)
          throw new ArgumentException("Texture format is not compatible with OpenSharedResource");

        IntPtr handle = GetSharedHandle(renderTarget);
        if (handle == IntPtr.Zero)
          throw new ArgumentNullException("Handle");

        this._renderTarget = new Texture(D3D10ImageSource._d3DDevice, renderTarget.Description.Width, renderTarget.Description.Height, 1, Usage.RenderTarget, format, Pool.Default, ref handle);
        using (Surface surface = this._renderTarget.GetSurfaceLevel(0))
        {
          base.Lock();
          base.SetBackBuffer(D3DResourceType.IDirect3DSurface9, surface.NativePointer);
          base.Unlock();
        }
      }
      // System.Diagnostics.Debug.WriteLine("D3DImageSource.SetRenderTarget(end) Name={0}, Id={1}, renderTarget={2}, Size={3}x{4}", Name, InstanceID, renderTarget, this.PixelWidth, this.PixelHeight);
    }

    private static void StartD3D()
    {
      _d3DContext = new Direct3DEx();

      var presentparams = new PresentParameters
      {
        Windowed = true,
        SwapEffect = SwapEffect.Discard,
        DeviceWindowHandle = GetDesktopWindow(),
        PresentationInterval = PresentInterval.Default
      };

      D3D10ImageSource._d3DDevice = new DeviceEx(_d3DContext, 0, DeviceType.Hardware, IntPtr.Zero, CreateFlags.HardwareVertexProcessing | CreateFlags.Multithreaded | CreateFlags.FpuPreserve, presentparams);
    }

    private static void EndD3D()
    {
      Disposer.RemoveAndDispose(ref D3D10ImageSource._d3DDevice);
      Disposer.RemoveAndDispose(ref D3D10ImageSource._d3DContext);
    }

    private static IntPtr GetSharedHandle(SharpDX.Direct3D10.Texture2D Texture)
    {
      SharpDX.DXGI.Resource resource = Texture.QueryInterface<SharpDX.DXGI.Resource>();
      IntPtr result = resource.SharedHandle;
      resource.Dispose();
      return result;
    }

    private static Format TranslateFormat(SharpDX.Direct3D10.Texture2D Texture)
    {
      switch (Texture.Description.Format)
      {
        case SharpDX.DXGI.Format.R10G10B10A2_UNorm:
          return SharpDX.Direct3D9.Format.A2B10G10R10;

        case SharpDX.DXGI.Format.R16G16B16A16_Float:
          return SharpDX.Direct3D9.Format.A16B16G16R16F;

        case SharpDX.DXGI.Format.B8G8R8A8_UNorm:
          return SharpDX.Direct3D9.Format.A8R8G8B8;

        default:
          return SharpDX.Direct3D9.Format.Unknown;
      }
    }

    private static bool IsShareable(SharpDX.Direct3D10.Texture2D Texture)
    {
      return (Texture.Description.OptionFlags & SharpDX.Direct3D10.ResourceOptionFlags.Shared) != 0;
    }
  }
}
