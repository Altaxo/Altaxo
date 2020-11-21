#region Copyright

/////////////////////////////////////////////////////////////////////////////
//    Altaxo:  a data processing and data plotting program
//    Copyright (C) 2002-2014 Dr. Dirk Lellinger
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

#nullable disable warnings
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;

namespace Altaxo.Gui
{
  /// <summary>
  /// Encapsulates the logic to map a Gdi bitmap on a Wpf bitmap source.
  /// </summary>
  public class GdiToWpfBitmap : IDisposable, System.ComponentModel.INotifyPropertyChanged
  {
    #region Native calls

    [DllImport("kernel32.dll", SetLastError = true)]
    private static extern IntPtr CreateFileMapping(IntPtr hFile,

                                           IntPtr lpFileMappingAttributes,

                                           uint flProtect,

                                           uint dwMaximumSizeHigh,

                                           uint dwMaximumSizeLow,

                                           string lpName);

    [DllImport("kernel32.dll", SetLastError = true)]
    private static extern IntPtr MapViewOfFile(IntPtr hFileMappingObject,

                                       uint dwDesiredAccess,

                                       uint dwFileOffsetHigh,

                                       uint dwFileOffsetLow,

                                       uint dwNumberOfBytesToMap);

    [DllImport("kernel32", EntryPoint = "UnmapViewOfFile", SetLastError = true)]
    private static extern bool UnmapViewOfFile(IntPtr lpBaseAddress);

    [DllImport("kernel32", EntryPoint = "CloseHandle", SetLastError = true)]
    private static extern bool CloseHandle(IntPtr handle);

    // Windows constants

    private static uint FILE_MAP_ALL_ACCESS = 0xF001F;

    private static uint PAGE_READWRITE = 0x04;

    private static IntPtr INVALID_HANDLE_VALUE = new IntPtr(-1);

    #endregion Native calls

    /// <summary>Only for debugging, designates the total number of bytes allocated in all those objects</summary>
    private static long _totalBytesAllocated;

    /// <summary>Only for debugging, designates the number of instances of this class.</summary>
    private static long _activeInstances; // only for debugging

    private const int BytesPerPixel = 4; // it is only possible to use ARGB format, otherwise Imaging.CreateBitmapSourceFromMemorySection would copy the bitmap instead of mapping it

    private IntPtr _section;
    private IntPtr _map;
    private System.Drawing.Bitmap _bmp;
    private System.Windows.Interop.InteropBitmap _interopBmp;
    private int _width, _height;

    /// <summary>
    /// Creates an instance of given width and height. Note that the GidBitmap is created during this calling thread. (the WpfBitmap is created later during the first access to WpfBitmap).
    /// </summary>
    /// <param name="width"></param>
    /// <param name="height"></param>
    public GdiToWpfBitmap(int width, int height)
    {
      Current.Dispatcher.VerifyAccess();
      InternalAllocate(width, height);
    }

    public GdiToWpfBitmap()
    {
    }

    /// <summary>
    /// Resizes the bitmap. Note that the GidBitmap is created during this calling thread. (the WpfBitmap is created later during the first access to WpfBitmap).
    /// </summary>
    /// <param name="width"></param>
    /// <param name="height"></param>
    public void Resize(int width, int height)
    {
      Current.Dispatcher.VerifyAccess();
      InternalDeallocate(true);
      InternalAllocate(width, height);
    }

    private void InternalAllocate(int width, int height)
    {
      width = Math.Max(width, 1);
      height = Math.Max(height, 1);

      _width = width;
      _height = height;
      uint numBytesToAllocate = (uint)(width * height * BytesPerPixel);

      _section = CreateFileMapping(INVALID_HANDLE_VALUE,
                                         IntPtr.Zero,
                                         PAGE_READWRITE,
                                         0,
                                         numBytesToAllocate,
                                         null);

      if (_section == IntPtr.Zero)
        throw new InvalidOperationException(string.Format("Unable to create file mapping for {0} x {1} pixel. ActiveInstances: {2}, NumberOfBytesAllocated={3} ", width, height, _activeInstances, _totalBytesAllocated));

      _map = MapViewOfFile(_section, FILE_MAP_ALL_ACCESS, 0, 0, numBytesToAllocate);

      if (IntPtr.Zero == _map)
      {
        System.GC.Collect();
        _map = MapViewOfFile(_section, FILE_MAP_ALL_ACCESS, 0, 0, numBytesToAllocate);
      }

      if (_map == IntPtr.Zero)
      {
        CloseHandle(_section);
        _section = IntPtr.Zero;
        string exception = string.Format("Unable to create view of file for {0} x {1} pixel. ActiveInstances: {2}, NumberOfBytesAllocated={3} ", width, height, _activeInstances, _totalBytesAllocated);
        width = height = 0;
        throw new InvalidOperationException(exception);
      }

      _bmp = new System.Drawing.Bitmap(_width, _height, _width * BytesPerPixel, System.Drawing.Imaging.PixelFormat.Format32bppArgb, _map);

      GC.AddMemoryPressure(numBytesToAllocate);
      _totalBytesAllocated += numBytesToAllocate;
      ++_activeInstances;

      _interopBmp = (System.Windows.Interop.InteropBitmap)System.Windows.Interop.Imaging.CreateBitmapSourceFromMemorySection(_section, _width, _height, System.Windows.Media.PixelFormats.Bgra32, _width * BytesPerPixel, 0);
    }

    private void InternalDeallocate(bool disposing)
    {
      if (disposing)
      {
        // free managed resources
        if (_bmp is not null)
        {
          _bmp.Dispose();
          _bmp = null;
        }

        if (_interopBmp is not null)
        {
          _interopBmp = null;
        }
      }

      // now free all unmanaged resources
      if (IntPtr.Zero != _map)
      {
        UnmapViewOfFile(_map);
        _map = IntPtr.Zero;
      }

      if (IntPtr.Zero != _section)
      {
        CloseHandle(_section);
        _section = IntPtr.Zero;
      }

      var bytesToFree = _width * _height * BytesPerPixel;
      if (0 != bytesToFree)
      {
        GC.RemoveMemoryPressure(bytesToFree);
        _totalBytesAllocated -= bytesToFree;
      }

      --_activeInstances;

      _width = 0;
      _height = 0;
    }

    public System.Drawing.Bitmap GdiBitmap
    {
      get
      {
        return _bmp;
      }
    }

    /// <summary>
    /// Begins the GDI painting by creating a new Gdi graphics context that can be used for drawing. If finished,
    /// <see cref="EndGdiPainting"/> must be called to invalidate the interop bitmap and to update the WpfBitmapSource.
    /// </summary>
    /// <returns></returns>
    public System.Drawing.Graphics BeginGdiPainting()
    {
      return System.Drawing.Graphics.FromImage(_bmp);
    }

    public void EndGdiPainting()
    {
      Current.Dispatcher.InvokeIfRequired(
          () =>
          {
            var bmp = _interopBmp;
            if (bmp is not null)
            {
              bmp.Invalidate();
              OnPropertyChanged(nameof(WpfBitmapSource));
            }
          });
    }

    public System.Drawing.Rectangle GdiRectangle
    {
      get
      {
        return new System.Drawing.Rectangle(0, 0, _width, _height);
      }
    }

    public System.Drawing.Size GdiSize
    {
      get
      {
        return new System.Drawing.Size(_width, _height);
      }
    }

    public System.Windows.Interop.InteropBitmap WpfBitmap
    {
      get
      {
        return _interopBmp;
      }
    }

    public System.Windows.Media.Imaging.BitmapSource WpfBitmapSource
    {
      get
      {
        var bmp = _interopBmp;
        if (bmp is not null)
        {
          bmp.Invalidate();
          return (System.Windows.Media.Imaging.BitmapSource)bmp.GetAsFrozen();
        }
        else
        {
          return null;
        }
      }
    }

    #region IDisposable Members

    public void Dispose()
    {
      InternalDeallocate(true);
      GC.SuppressFinalize(this);
    }

    ~GdiToWpfBitmap()
    {
      // Do not re-create Dispose clean-up code here.
      // Calling Dispose(false) is optimal in terms of
      // readability and maintainability.
      InternalDeallocate(false);
    }

    #endregion IDisposable Members

    public event System.ComponentModel.PropertyChangedEventHandler? PropertyChanged;

    public virtual void OnPropertyChanged(string name)
    {
      if (PropertyChanged is not null)
        PropertyChanged(this, new System.ComponentModel.PropertyChangedEventArgs(name));
    }
  }
}
