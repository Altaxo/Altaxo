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
  using System;
  using System.Runtime.InteropServices;
  using System.Windows;
  using System.Windows.Interop;
  using Vortice.Direct3D9;

  /// <summary>
  /// An image source that can be used to present to WPF.
  /// </summary>
  /// <seealso cref="System.Windows.Interop.D3DImage" />
  /// <seealso cref="System.IDisposable" />
  public class D3D11ImageSource : D3DImage, IDisposable
  {
    [DllImport("user32.dll", SetLastError = false)]
    private static extern IntPtr GetDesktopWindow();

    private static int _numberOfActiveClients;
    private static IDirect3D9Ex _d3DContext;
    private static IDirect3DDevice9Ex _d3DDevice;

    /// <summary>
    /// The DX9 render target used for the image source in order to present to WPF.
    /// </summary>
    private IDirect3DTexture9 _renderTarget;

    /// <summary>
    /// Initializes a new instance of the <see cref="DX11ImageSource"/> class.
    /// </summary>
    public D3D11ImageSource()
    {
      StartD3D();
      _numberOfActiveClients++;
    }

    /// <summary>
    /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
    /// </summary>
    public void Dispose()
    {
      this.SetRenderTargetDX11(null);
      Disposer.RemoveAndDispose(ref this._renderTarget);

      _numberOfActiveClients--;
      this.EndD3D();
    }

    /// <summary>
    /// Announce that the content of the shared DX11 texture has changed. Sets the entire area of the image source to dirty.
    /// </summary>
    public void InvalidateD3DImage()
    {
      if (this._renderTarget != null)
      {
        base.Lock();
        base.AddDirtyRect(new Int32Rect(0, 0, base.PixelWidth, base.PixelHeight));
        base.Unlock();
      }
    }

    /// <summary>
    /// Sets the DX9 render target used for the image source based on the provided DX11 texture.
    /// The DX11 texture must be sharable, and is then shared with the DX9 image source.
    /// If the DX11 texture changed its contents, this changed must be announced using <see cref="InvalidateD3DImage"/> in
    /// order to set the entire areay of the image source to dirty.
    /// </summary>
    /// <param name="renderTargetDX11">The render target.</param>
    /// <exception cref="System.ArgumentException">
    /// Texture must be created with ResourceOptionFlags.Shared
    /// or
    /// Texture format is not compatible with OpenSharedResource
    /// </exception>
    /// <exception cref="System.ArgumentNullException">Handle</exception>
    public void SetRenderTargetDX11(Vortice.Direct3D11.ID3D11Texture2D renderTargetDX11)
    {
      if (this._renderTarget is not null)
      {
        this._renderTarget = null;

        base.Lock();
        base.SetBackBuffer(D3DResourceType.IDirect3DSurface9, IntPtr.Zero);
        base.Unlock();
      }

      if (renderTargetDX11 is null)
        return;

      if (!IsShareable(renderTargetDX11))
        throw new ArgumentException("Texture must be created with ResourceOptionFlags.Shared");

      Format format = TranslateFormat(renderTargetDX11);
      if (format == Format.Unknown)
        throw new ArgumentException("Texture format is not compatible with OpenSharedResource");

      IntPtr handle = GetSharedHandle(renderTargetDX11);
      if (handle == IntPtr.Zero)
        throw new ArgumentNullException("Handle");


      // Attention - could be a bug- the handle at the end of the argument list could be given by ref?
      this._renderTarget = _d3DDevice.CreateTexture(renderTargetDX11.Description.Width, renderTargetDX11.Description.Height, 1, Usage.RenderTarget, format, Pool.Default, ref handle);
      using (var surface = this._renderTarget.GetSurfaceLevel(0))
      {
        base.Lock();
        base.SetBackBuffer(D3DResourceType.IDirect3DSurface9, surface.NativePointer);
        base.Unlock();
      }
    }

    /// <summary>
    /// Announces the starts of the usage of this image source. If this is the first usage, the resources needed are allocated.
    /// </summary>
    private void StartD3D()
    {
      if (_numberOfActiveClients != 0)
        return;

      D3D9.Create9Ex(out _d3DContext);

      var presentparams = new PresentParameters
      {
        Windowed = true,
        SwapEffect = SwapEffect.Discard,
        DeviceWindowHandle = GetDesktopWindow(),
        PresentationInterval = PresentInterval.Default
      };

      // Creates the DX9 device (static field)

      _d3DDevice = _d3DContext.CreateDeviceEx(0, DeviceType.Hardware, IntPtr.Zero, CreateFlags.HardwareVertexProcessing | CreateFlags.Multithreaded | CreateFlags.FpuPreserve, presentparams);
    }

    /// <summary>
    /// Announces the end of usage of this image source. If the counter to the active clients reach zero,
    /// the resources used by this instance are disposed of.
    /// </summary>
    private void EndD3D()
    {
      if (_numberOfActiveClients != 0)
        return;

      Disposer.RemoveAndDispose(ref this._renderTarget);
      Disposer.RemoveAndDispose(ref _d3DDevice);
      Disposer.RemoveAndDispose(ref _d3DContext);
    }

    /// <summary>
    /// Gets a shared handle of the provided texture.
    /// </summary>
    /// <param name="Texture">The texture.</param>
    /// <returns>The shared handle of the texture.</returns>
    private static IntPtr GetSharedHandle(Vortice.Direct3D11.ID3D11Texture2D Texture)
    {
      var resource = Texture.QueryInterface<Vortice.DXGI.IDXGIResource>();
      IntPtr result = resource.SharedHandle;
      resource.Dispose();
      return result;
    }

    /// <summary>
    /// Translates the format of the DirectX11 texture to the corresponding DX9 Format.
    /// </summary>
    /// <param name="texture">The texture.</param>
    /// <returns>The format of the texture as DX9 format.</returns>
    private static Format TranslateFormat(Vortice.Direct3D11.ID3D11Texture2D texture)
    {
      switch (texture.Description.Format)
      {
        case Vortice.DXGI.Format.R10G10B10A2_UNorm:
          return Vortice.Direct3D9.Format.A2B10G10R10;

        case Vortice.DXGI.Format.R16G16B16A16_Float:
          return Vortice.Direct3D9.Format.A16B16G16R16F;

        case Vortice.DXGI.Format.B8G8R8A8_UNorm:
          return Vortice.Direct3D9.Format.A8R8G8B8;

        default:
          return Vortice.Direct3D9.Format.Unknown;
      }
    }

    /// <summary>
    /// Determines whether the specified texture is shareable.
    /// </summary>
    /// <param name="Texture">The texture.</param>
    /// <returns>
    ///   <c>true</c> if the specified texture is shareable; otherwise, <c>false</c>.
    /// </returns>
    private static bool IsShareable(Vortice.Direct3D11.ID3D11Texture2D Texture)
    {
      return (Texture.Description.OptionFlags & Vortice.Direct3D11.ResourceOptionFlags.Shared) != 0;
    }
  }
}
