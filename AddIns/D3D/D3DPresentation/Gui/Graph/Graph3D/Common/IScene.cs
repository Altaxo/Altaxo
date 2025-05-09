﻿// Copyright (c) 2010-2013 SharpDX - Alexandre Mutel
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
namespace Altaxo.Gui.Graph.Graph3D.Common
{
  using System;
  using Altaxo.Geometry;
  using Vortice.Direct3D11;

  /// <summary>
  /// Scene.
  /// </summary>
  public interface IScene : IDisposable
  {
    /// <summary>
    /// Attaches the scene to the specified scene host.
    /// </summary>
    /// <param name="host">The scene host.</param>
    void Attach(SharpGen.Runtime.ComObject hostDevice, PointD2D hostSize);

    /// <summary>
    /// Informes the scene that the host size has changed.
    /// </summary>
    /// <param name="hostSize">Size of the host.</param>
    void SetHostSize(PointD2D hostSize);

    /// <summary>
    /// Detaches this scene from the scene host.
    /// </summary>
    void Detach();

    /// <summary>
    /// Updates the scene, taking into account the specified time.
    /// </summary>
    /// <param name="timeSpan">The current scene time.</param>
    void Update(TimeSpan timeSpan);

    /// <summary>
    /// Renders this scene to the scene host.
    /// </summary>
    void Render();

    /// <summary>
    /// Gets the color of the scene background. If null is returned, the render function is free to use its own color as scene background.
    /// </summary>
    /// <value>
    /// The color of the scene background.
    /// </value>
    Altaxo.Drawing.AxoColor? SceneBackgroundColor { get; }
  }

  /// <summary>
  /// Scene.
  /// </summary>
  public interface ID3D11Scene : IScene
  {
    /// <summary>
    /// Attaches the scene to the specified scene host.
    /// </summary>
    /// <param name="host">The scene host.</param>
    void Attach(ID3D11Device hostDevice, PointD2D hostSize);
  }
}
