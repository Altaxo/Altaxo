#region Copyright

/////////////////////////////////////////////////////////////////////////////
//    Altaxo:  a data processing and data plotting program
//    Copyright (C) 2002-2015 Dr. Dirk Lellinger
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
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vortice.Direct3D11;
using Vortice.DXGI;
using Vortice.WIC;

namespace Altaxo.Gui.Graph.Graph3D.Common
{
  /// <summary>
  /// Operations on textures.
  /// </summary>
  public static class Texture2DExtensions
  {
    /// <summary>
    /// Saves a texture to a file as an image.
    /// </summary>
    /// <param name="texture">The texture to save.</param>
    /// <param name="imageFormat">The image format of the saved image.</param>
    /// <param name="imageResolutionInDpi">The image resolution in dpi.</param>
    /// <param name="fileName">The name of the file to save the texture to.</param>
    public static void SaveToFile(this ID3D11Texture2D texture, System.Drawing.Imaging.ImageFormat imageFormat, double imageResolutionInDpi, string fileName)
    {
      using (var toStream = new System.IO.FileStream(fileName, System.IO.FileMode.OpenOrCreate, System.IO.FileAccess.ReadWrite))
      {
        SaveToStream(texture, imageFormat, imageResolutionInDpi, toStream);
      }
    }

    /// <summary>
    /// Saves a texture to a stream as an image.
    /// </summary>
    /// <param name="texture">The texture to save.</param>
    /// <param name="imageFormat">The image format of the saved image.</param>
    /// <param name="imageResolutionInDpi">The image resolution in dpi.</param>
    /// <param name="toStream">The stream to save the texture to.</param>
    public static void SaveToStream(this ID3D11Texture2D texture, System.Drawing.Imaging.ImageFormat imageFormat, double imageResolutionInDpi, System.IO.Stream toStream)
    {
      ID3D11Texture2D? textureCopy = null;
      IWICImagingFactory? imagingFactory = null;
      IWICBitmap? bitmap = null;
      IWICBitmapEncoder? bitmapEncoder = null;

      try
      {
        textureCopy = texture.Device.CreateTexture2D(new Texture2DDescription
        {
          Width = texture.Description.Width,
          Height = texture.Description.Height,
          MipLevels = 1,
          ArraySize = 1,
          Format = texture.Description.Format,
          Usage = ResourceUsage.Staging,
          SampleDescription = new SampleDescription(1, 0),
          BindFlags = BindFlags.None,
          CpuAccessFlags = CpuAccessFlags.Read,
          OptionFlags = ResourceOptionFlags.None
        });

        texture.Device.ImmediateContext.CopyResource(texture, textureCopy);

        var dataRectangle = texture.Device.ImmediateContext.Map(textureCopy, 0, MapMode.Read, Vortice.Direct3D11.MapFlags.None);

        imagingFactory = new IWICImagingFactory(); 
        bitmap = imagingFactory.CreateBitmapFromMemory(
            textureCopy.Description.Width,
            textureCopy.Description.Height,
            PixelFormat.Format32bppBGRA,
            dataRectangle.AsSpan<int>(textureCopy.Description.ArraySize));

        toStream.Position = 0;

        if (imageFormat == System.Drawing.Imaging.ImageFormat.Png)
          bitmapEncoder = imagingFactory.CreateEncoder(ContainerFormat.Png, toStream);
        else if (imageFormat == System.Drawing.Imaging.ImageFormat.Bmp)
          bitmapEncoder = imagingFactory.CreateEncoder(ContainerFormat.Bmp, toStream);
        else if (imageFormat == System.Drawing.Imaging.ImageFormat.Gif)
          bitmapEncoder = imagingFactory.CreateEncoder(ContainerFormat.Gif, toStream);
        else if (imageFormat == System.Drawing.Imaging.ImageFormat.Jpeg)
          bitmapEncoder = imagingFactory.CreateEncoder(ContainerFormat.Jpeg, toStream);
        else if (imageFormat == System.Drawing.Imaging.ImageFormat.Tiff)
          bitmapEncoder = imagingFactory.CreateEncoder(ContainerFormat.Tiff, toStream);
        else
          bitmapEncoder = imagingFactory.CreateEncoder(ContainerFormat.Png, toStream);

        using (var bitmapFrameEncode = bitmapEncoder.CreateNewFrame(null))
        {
          bitmapFrameEncode.Initialize();
          bitmapFrameEncode.SetSize(bitmap.Size.Width, bitmap.Size.Height);
          var pixelFormat = PixelFormat.FormatDontCare;
          bitmapFrameEncode.SetPixelFormat(ref pixelFormat);
          bitmapFrameEncode.SetResolution(imageResolutionInDpi, imageResolutionInDpi);
          bitmapFrameEncode.WriteSource(bitmap);
          bitmapFrameEncode.Commit();
          bitmapEncoder.Commit();
        }
      }
      finally
      {
        bitmapEncoder?.Dispose();
        texture.Device.ImmediateContext.Unmap(textureCopy, 0);
        textureCopy?.Dispose();
        bitmap?.Dispose();
        imagingFactory?.Dispose();
      }
    }
  }
}
