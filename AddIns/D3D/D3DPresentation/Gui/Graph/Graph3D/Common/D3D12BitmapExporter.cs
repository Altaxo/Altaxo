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
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Runtime.InteropServices;
using Altaxo.Geometry;
using Altaxo.Graph.Graph3D.Camera;
using Altaxo.Graph.Graph3D.GraphicsContext.D3D;
using Altaxo.Gui.Graph.Graph3D.Viewing;

namespace Altaxo.Gui.Graph.Graph3D.Common
{
  /// <summary>
  /// Exports 3D graph scenes to bitmap streams using D3D12 offscreen rendering.
  /// </summary>
  public class D3D12BitmapExporter : Altaxo.Main.IProjectItemImageExporter
  {
    /// <inheritdoc/>
    public (int PixelsX, int PixelsY) ExportAsImageToStream(Altaxo.Main.IProjectItem item, Altaxo.Graph.Gdi.GraphExportOptions options, System.IO.Stream toStream)
    {
      if (item is null)
        throw new ArgumentNullException(nameof(item));
      if (item is not Altaxo.Graph.Graph3D.GraphDocument doc)
        throw new ArgumentException($"Expected item of type {typeof(Altaxo.Graph.Graph3D.GraphDocument)}, but it is of type {item.GetType()}");

      double sourceDpi = options.SourceDpiResolution;

      D3D12Scene? scene = null;
      D3DGraphicsContext? g = null;
      try
      {
        scene = new D3D12Scene();
        g = new D3DGraphicsContext();

        doc.Paint(g);

        var matrix = doc.Camera.LookAtRHMatrix;
        var rect = new RectangleD3D(PointD3D.Empty, doc.RootLayer.Size);
        var bounds = RectangleD3D.NewRectangleIncludingAllPoints(rect.Vertices.Select(x => matrix.Transform(x)));

        int pixelsX = (int)Math.Ceiling(sourceDpi * bounds.SizeX / 72.0);
        pixelsX = (int)(4 * Math.Ceiling((pixelsX + 3) / 4.0));

        int pixelsY = (int)(sourceDpi * bounds.SizeY / 72.0);
        double aspectRatio = pixelsY / (double)pixelsX;

        var sceneCamera = doc.Camera;
        if (sceneCamera is OrthographicCamera orthoCamera)
        {
          orthoCamera = (OrthographicCamera)orthoCamera.WithWidthAtZNear(bounds.SizeX);
          double offsX = -(1 + 2 * bounds.X / bounds.SizeX);
          double offsY = -(1 + 2 * bounds.Y / bounds.SizeY);
          sceneCamera = orthoCamera.WithScreenOffset(new PointD2D(offsX, offsY));
        }
        else if (sceneCamera is PerspectiveCamera perspCamera)
        {
          var viewProj = sceneCamera.GetViewProjectionMatrix(1);
          var screenBounds = RectangleD3D.NewRectangleIncludingAllPoints(rect.Vertices.Select(x => viewProj.Transform(x)));
          aspectRatio = screenBounds.SizeY / screenBounds.SizeX;
          viewProj = sceneCamera.GetViewProjectionMatrix(aspectRatio);
          screenBounds = RectangleD3D.NewRectangleIncludingAllPoints(rect.Vertices.Select(x => viewProj.Transform(x)));

          double scaleFactor = 2 / screenBounds.SizeX;
          double offsX = -(1 + scaleFactor * screenBounds.X);
          double offsY = -(1 + scaleFactor * screenBounds.Y);
          pixelsY = (int)(4 * Math.Ceiling((aspectRatio * pixelsX + 3) / 4.0));

          sceneCamera = perspCamera.WithWidthAtZNear(perspCamera.WidthAtZNear / scaleFactor);
          sceneCamera = sceneCamera.WithScreenOffset(new PointD2D(offsX, offsY));
        }
        else
        {
          throw new NotImplementedException();
        }

        scene.SetCamera(sceneCamera);
        scene.SetLighting(doc.Lighting);
        scene.SetDrawing(g);

        Export(pixelsX, pixelsY, scene, options, toStream);
        return (pixelsX, pixelsY);
      }
      finally
      {
        Disposer.RemoveAndDispose(ref g);
        Disposer.RemoveAndDispose(ref scene);
      }
    }

    /// <summary>
    /// Renders the specified scene and writes the result to a stream.
    /// </summary>
    public void Export(int sizeX, int sizeY, ID3D12Scene scene, Altaxo.Graph.Gdi.GraphExportOptions options, System.IO.Stream toStream)
    {
      using var renderer = new D3D12OffscreenRenderer(sizeX, sizeY)
      {
        Scene = scene
      };

      scene.SetSceneBackColor(options.BackgroundBrush?.Color.Color);
      renderer.Render();

      var pixelBytes = renderer.ReadBackBgraBytes();
      SaveBgraToStream(pixelBytes, sizeX, sizeY, options.ImageFormat, options.DestinationDpiResolution, toStream);
    }

    private static void SaveBgraToStream(byte[] bgra, int width, int height, ImageFormat imageFormat, double dpi, System.IO.Stream toStream)
    {
      using var bitmap = new Bitmap(width, height, PixelFormat.Format32bppArgb);
      var rect = new Rectangle(0, 0, width, height);
      var bmpData = bitmap.LockBits(rect, ImageLockMode.WriteOnly, PixelFormat.Format32bppArgb);
      try
      {
        for (int y = 0; y < height; ++y)
        {
          Marshal.Copy(bgra, y * width * 4, IntPtr.Add(bmpData.Scan0, y * bmpData.Stride), width * 4);
        }
      }
      finally
      {
        bitmap.UnlockBits(bmpData);
      }

      bitmap.SetResolution((float)dpi, (float)dpi);
      toStream.Position = 0;
      bitmap.Save(toStream, imageFormat);
    }

  }
}
