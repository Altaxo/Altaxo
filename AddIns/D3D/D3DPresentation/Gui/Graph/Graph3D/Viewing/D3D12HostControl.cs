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
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Interop;
using System.Windows.Media;
using Altaxo.Geometry;
using Altaxo.Gui.Graph.Graph3D.Common;

namespace Altaxo.Gui.Graph.Graph3D.Viewing
{
  /// <summary>
  /// WPF host control that creates a child HWND and drives a <see cref="D3D12Renderer"/> render loop
  /// using <see cref="CompositionTarget.Rendering"/>.
  /// </summary>
  public sealed class D3D12HostControl : HwndHost
  {
    /// <summary>
    /// Native handle of the child host window.
    /// </summary>
    private IntPtr _hwnd;

    /// <summary>
    /// Indicates whether the WPF render callback is currently subscribed.
    /// </summary>
    private bool _isRendering;

    private Action<IntPtr, PointD2D>? _render;
    public void AttachRenderer(Action<IntPtr, PointD2D> render)
    {
      _render = render;
      StartRendering();
    }
    public void DetachRenderer()
    {
      StopRendering();
      _render = null;
    }


    /// <summary>
    /// Creates the underlying child window and initializes Direct3D resources.
    /// </summary>
    /// <param name="hwndParent">Handle wrapper for the parent window.</param>
    /// <returns>A handle reference to the created child window.</returns>
    protected override HandleRef BuildWindowCore(HandleRef hwndParent)
    {
      _hwnd = CreateHostWindow(hwndParent.Handle);
      return new HandleRef(this, _hwnd);
    }

    /// <summary>
    /// Destroys the underlying child window and releases Direct3D resources.
    /// </summary>
    /// <param name="hwnd">Handle wrapper for the window being destroyed.</param>
    protected override void DestroyWindowCore(HandleRef hwnd)
    {
      StopRendering();

      if (_hwnd != IntPtr.Zero)
      {
        DestroyWindow(_hwnd);
        _hwnd = IntPtr.Zero;
      }
    }

    /// <summary>
    /// Notifies the host that the control's size or position changed and resizes the renderer.
    /// </summary>
    /// <param name="rcBoundingBox">New bounding rectangle.</param>
    protected override void OnWindowPositionChanged(Rect rcBoundingBox)
    {
      base.OnWindowPositionChanged(rcBoundingBox);
      OnRendering(this, EventArgs.Empty);
    }

    /// <summary>
    /// Starts the render loop by subscribing to <see cref="CompositionTarget.Rendering"/>.
    /// </summary>
    private void StartRendering()
    {
      if (_isRendering)
      {
        return;
      }

      CompositionTarget.Rendering += OnRendering;
      _isRendering = true;
    }

    /// <summary>
    /// Stops the render loop by unsubscribing from <see cref="CompositionTarget.Rendering"/>.
    /// </summary>
    private void StopRendering()
    {
      if (!_isRendering)
      {
        return;
      }

      CompositionTarget.Rendering -= OnRendering;
      _isRendering = false;
    }

    /// <summary>
    /// WPF render callback that updates the scene, clears the back buffer and asks the scene to render.
    /// </summary>
    /// <param name="sender">Event source.</param>
    /// <param name="e">Event arguments.</param>
    private void OnRendering(object? sender, EventArgs e)
    {
      _render?.Invoke(_hwnd, new PointD2D(ActualWidth, ActualHeight));
    }

    /// <summary>
    /// Creates a child window to be used as the Direct3D swap chain target.
    /// </summary>
    /// <param name="parent">Parent window handle.</param>
    /// <returns>Handle to the created child window.</returns>
    private static IntPtr CreateHostWindow(IntPtr parent)
    {
      const int WS_CHILD = 0x40000000;
      const int WS_VISIBLE = 0x10000000;

      var hwnd = CreateWindowEx(
          0,
          "static",
          "",
          WS_CHILD | WS_VISIBLE,
          0,
          0,
          1,
          1,
          parent,
          IntPtr.Zero,
          IntPtr.Zero,
          IntPtr.Zero);

      return hwnd;
    }

    /// <summary>
    /// Creates a window using the Win32 API.
    /// </summary>
    [DllImport("user32.dll", CharSet = CharSet.Unicode, SetLastError = true)]
    private static extern IntPtr CreateWindowEx(
        int exStyle,
        string className,
        string windowName,
        int style,
        int x,
        int y,
        int width,
        int height,
        IntPtr parent,
        IntPtr menu,
        IntPtr instance,
        IntPtr param);

    /// <summary>
    /// Destroys a window using the Win32 API.
    /// </summary>
    [DllImport("user32.dll", SetLastError = true)]
    private static extern bool DestroyWindow(IntPtr hWnd);
  }
}
