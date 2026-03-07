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
using Altaxo.Geometry;

namespace Altaxo.Gui.Graph.Graph3D.Common
{
  /// <summary>
  /// Common base interface for D3D renderers.
  /// </summary>
  public interface ID3DRenderer : IDisposable
  {

  }

  /// <summary>
  /// Marker interface for D3D11-based renderers.
  /// </summary>
  public interface ID3D11Renderer : ID3DRenderer
  {

  }

  /// <summary>
  /// Shared render context contract for D3D12 scene implementations.
  /// </summary>
  public interface ID3D12RenderContext
  {
    /// <summary>
    /// Gets the D3D12 device.
    /// </summary>
    public Vortice.Direct3D12.ID3D12Device Device { get; }

    /// <summary>
    /// Gets the graphics command list used for scene draw recording.
    /// </summary>
    public Vortice.Direct3D12.ID3D12GraphicsCommandList CommandList { get; }

    /// <summary>
    /// Gets the current render target width in pixels.
    /// </summary>
    public int Width { get; }

    /// <summary>
    /// Gets the current render target height in pixels.
    /// </summary>
    public int Height { get; }

    /// <summary>
    /// Gets the current render-target-view handle.
    /// </summary>
    public Vortice.Direct3D12.CpuDescriptorHandle CurrentRtv { get; }
  }

  /// <summary>
  /// Interface for D3D12-based renderers.
  /// </summary>
  public interface ID3D12Renderer : ID3DRenderer
  {
    /// <summary>
    /// Renders one frame to the specified host window.
    /// </summary>
    /// <param name="hwnd">Host window handle.</param>
    /// <param name="targetSize">Target width/height in pixels.</param>
    public void Render(IntPtr hwnd, PointD2D targetSize);
  }
}
