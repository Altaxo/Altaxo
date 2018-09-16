// Copyright (c) 2014 AlphaSierraPapa for the SharpDevelop Team
//
// Permission is hereby granted, free of charge, to any person obtaining a copy of this
// software and associated documentation files (the "Software"), to deal in the Software
// without restriction, including without limitation the rights to use, copy, modify, merge,
// publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons
// to whom the Software is furnished to do so, subject to the following conditions:
//
// The above copyright notice and this permission notice shall be included in all copies or
// substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED,
// INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR
// PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE
// FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR
// OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER
// DEALINGS IN THE SOFTWARE.

using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using Altaxo.Gui.Common;
using Altaxo.Main.Services;

namespace Altaxo.Gui
{
  /// <summary>
  /// Creates WPF BitmapSource objects from images in the ResourceService.
  /// </summary>
  public static class PresentationResourceService
  {
    private static readonly Dictionary<string, BitmapSource> _bitmapCache = new Dictionary<string, BitmapSource>();
    private static readonly IResourceService _resourceService;

    static PresentationResourceService()
    {
      _resourceService = Altaxo.Current.GetRequiredService<IResourceService>();
      _resourceService.LanguageChanged += OnLanguageChanged;
    }

    /// <summary>
    /// Gets a value indicating whether a instance  of the resource service is available. This maybe is
    /// not the case if we are in Wpf design mode.
    /// </summary>
    /// <value>
    /// <c>True</c> if a resource service instance is available; otherwise, <c>false</c>.
    /// </value>
    public static bool InstanceAvailable
    {
      get { return null != _resourceService; }
    }

    private static void OnLanguageChanged(object sender, EventArgs e)
    {
      lock (_bitmapCache)
      {
        _bitmapCache.Clear();
      }
    }

    /// <summary>
    /// Creates a new System.Windows.Controls.Image object containing the image with the
    /// specified resource name.
    /// </summary>
    /// <param name="name">
    /// The name of the requested bitmap.
    /// </param>
    /// <exception cref="ResourceNotFoundException">
    /// Is thrown when the GlobalResource manager can't find a requested resource.
    /// </exception>
    [Obsolete("Use SD.ResourceService.GetImage(name).CreateImage() instead, or just create the image manually")]
    public static System.Windows.Controls.Image GetImage(string name)
    {
      return new System.Windows.Controls.Image
      {
        Source = GetBitmapSource(name)
      };
    }

    public static BitmapSource IconToBitmapSource(System.Drawing.Icon icon)
    {
      using (var stream = new System.IO.MemoryStream())
      {
        icon.Save(stream);
        stream.Seek(0, System.IO.SeekOrigin.Begin);
        return BitmapFrame.Create(stream, BitmapCreateOptions.None, BitmapCacheOption.OnLoad);
      }
    }

    /// <summary>
    /// Returns a BitmapSource from the resource database, it handles localization
    /// transparent for the user.
    /// </summary>
    /// <param name="name">
    /// The name of the requested bitmap.
    /// </param>
    /// <exception cref="ResourceNotFoundException">
    /// Is thrown when the GlobalResource manager can't find a requested resource.
    /// </exception>
    public static ImageSource GetBitmapSource(string name)
    {
      if (_resourceService == null)
      {
        throw new InvalidProgramException(string.Format("Member {0} is null. Did you start the resource service?", nameof(_resourceService)));
      }

      lock (_bitmapCache)
      {
        if (_bitmapCache.TryGetValue(name, out var bs))
          return bs;

        var imageObject = _resourceService.GetImageResource(name);

        if (imageObject is System.Drawing.Icon icon)
        {
          var bi = IconToBitmapSource(icon);
          bi.Freeze();
          _bitmapCache[name] = bi;
          return bi;
        }

        if (imageObject is System.Drawing.Bitmap bmp)
        {
          IntPtr hBitmap = bmp.GetHbitmap();
          try
          {
            bs = Imaging.CreateBitmapSourceFromHBitmap(hBitmap, IntPtr.Zero,
                                                                                                 Int32Rect.Empty, BitmapSizeOptions.FromEmptyOptions());
            bs.Freeze();
            _bitmapCache[name] = bs;
          }
          finally
          {
            NativeMethods.DeleteObject(hBitmap);
          }
          return bs;
        }
        else if (imageObject is byte[] byteArray)
        {
          // then it is probably a Xaml text
          // Note: do not catch Exceptions here, because we want to know about wrong resources
          using (var ms = new System.IO.MemoryStream(byteArray))
          {
            var presentation = System.Windows.Markup.XamlReader.Load(ms);
            return (ImageSource)presentation;
          }
        }
        else if (imageObject != null)
        {
          throw new ResourceNotFoundException(string.Format("Resource of type {0} can not be converted in a Wpf image source", imageObject.GetType()));
        }
        else
        {
          throw new ResourceNotFoundException(name);
        }
      }
    }
  }
}
