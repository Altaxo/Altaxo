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
namespace Altaxo.Gui.Graph.Graph3D.Common
{
  using System;
  using Altaxo.Drawing;
  using Altaxo.Geometry;
  using Altaxo.Graph.Graph3D;
  using Altaxo.Graph.Graph3D.Camera;
  using Altaxo.Graph.Graph3D.GraphicsContext.D3D;

  /// <summary>
  /// Defines scene APIs used by Altaxo graph rendering code.
  /// </summary>
  public interface IAltaxo3DScene
  {
    /// <summary>
    /// Sets marker geometry.
    /// </summary>
    public void SetMarkerGeometry(D3DOverlayContext markerGeometry);
    /// <summary>
    /// Sets primary drawing geometry.
    /// </summary>
    public void SetDrawing(D3DGraphicsContext drawing);
    /// <summary>
    /// Sets light settings.
    /// </summary>
    public void SetLighting(LightSettings lightSettings);
    /// <summary>
    /// Sets the active camera.
    /// </summary>
    public void SetCamera(CameraBase camera);
    /// <summary>
    /// Sets the scene background color.
    /// </summary>
    public void SetSceneBackColor(AxoColor? sceneBackColor);
    /// <summary>
    /// Sets overlay geometry.
    /// </summary>
    public void SetOverlayGeometry(D3DOverlayContext overlayGeometry);

    /// <summary>
    /// Gets the color of the scene background. If null is returned, the render function is free to use its own color as scene background.
    /// </summary>
    /// <value>
    /// The color of the scene background.
    /// </value>
    public Altaxo.Drawing.AxoColor? SceneBackgroundColor { get; }
  }

  /// <summary>
  /// Scene.
  /// </summary>
  public interface IScene : IDisposable, IAltaxo3DScene
  {
    /// <summary>
    /// Attaches the scene to the specified scene host.
    /// </summary>
    /// <param name="host">The scene host.</param>
    public void Attach(SharpGen.Runtime.ComObject hostDevice, PointD2D hostSize);

    /// <summary>
    /// Informes the scene that the host size has changed.
    /// </summary>
    /// <param name="hostSize">Size of the host.</param>
    public void SetHostSize(PointD2D hostSize);

    /// <summary>
    /// Detaches this scene from the scene host.
    /// </summary>
    public void Detach();

    /// <summary>
    /// Updates the scene, taking into account the specified time.
    /// </summary>
    /// <param name="timeSpan">The current scene time.</param>
    public void Update(TimeSpan timeSpan);

    /// <summary>
    /// Renders this scene to the scene host.
    /// </summary>
    public void Render();
  }
}
