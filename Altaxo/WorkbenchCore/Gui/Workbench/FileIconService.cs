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
using System.Drawing;
using Altaxo.Main.Services;

namespace Altaxo.Gui.Workbench
{
  /// <summary>
  /// This Class handles bitmap resources
  /// for the application which were loaded from the filesystem.
  /// </summary>
  public static class FileIconService
  {
    private static Dictionary<string, Bitmap> _bitmapCache = new Dictionary<string, Bitmap>();
    private static Bitmap? _defaultBitmap;

    /// <summary>
    /// Returns a bitmap from the file system. Placeholders like ${SharpDevelopBinPath}
    /// and AddinPath (e. g. ${AddInPath:ICSharpCode.FiletypeRegisterer}) are resolved
    /// through the StringParser.
    /// The bitmaps are reused, you must not dispose the Bitmap!
    /// </summary>
    /// <returns>
    /// The bitmap loaded from the file system.
    /// </returns>
    /// <param name="name">
    /// The name of the requested bitmap (prefix, path and filename).
    /// </param>
    public static Bitmap GetBitmap(string name)
    {
      Bitmap? bmp = null;
      if (IsFileImage(name))
      {
        lock (_bitmapCache)
        {
          if (_bitmapCache.TryGetValue(name, out bmp))
            return bmp;
          string fileName = StringParser.Parse(name.Substring(5, name.Length - 5));
          bmp = (Bitmap)Image.FromFile(fileName);
          _bitmapCache[name] = bmp;
        }
      }
      return bmp ?? GetDefaultBitmap();
    }

    public static Bitmap GetDefaultBitmap()
    {
      if (_defaultBitmap is { } defBmp)
      {
        return defBmp;
      }
      else
      {
        var bmp = new Bitmap(16, 16);
        using (var g = Graphics.FromImage(bmp))
        {
          g.DrawLine(Pens.Red, 0, 0, 16, 16);
          g.DrawLine(Pens.Red, 0, 16, 16, 0);
        }
        System.Threading.Interlocked.Exchange(ref _defaultBitmap, bmp);
        return bmp;
      }
    }

    public static bool IsFileImage(string name)
    {
      return name.StartsWith("file:", StringComparison.OrdinalIgnoreCase);
    }
  }
}
