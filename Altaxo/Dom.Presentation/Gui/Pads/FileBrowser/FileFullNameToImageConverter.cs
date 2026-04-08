#region Copyright

/////////////////////////////////////////////////////////////////////////////
//    Altaxo:  a data processing and data plotting program
//    Copyright (C) 2002-2011 Dr. Dirk Lellinger
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
using System.Windows;
using System.Windows.Data;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace Altaxo.Gui.Pads.FileBrowser
{
  /// <summary>
  /// Converts a file path to the corresponding shell icon.
  /// </summary>
  public class FileFullNameToImageConverter : IValueConverter
  {
    #region Interop

    /// <summary>
    /// Shell flag for requesting an icon.
    /// </summary>
    public const uint SHGFI_ICON = 0x100;

    /// <summary>
    /// Shell flag for requesting a large icon.
    /// </summary>
    public const uint SHGFI_LARGEICON = 0x0;

    /// <summary>
    /// Shell flag for requesting a small icon.
    /// </summary>
    public const uint SHGFI_SMALLICON = 0x1;

    /// <summary>
    /// Receives shell file information.
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
    public struct SHFILEINFO
    {
      /// <summary>
      /// The icon handle.
      /// </summary>
      public IntPtr hIcon;
      /// <summary>
      /// The icon index.
      /// </summary>
      public IntPtr iIcon;
      /// <summary>
      /// The file attributes.
      /// </summary>
      public uint dwAttributes;

      /// <summary>
      /// The display name.
      /// </summary>
      [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 260)]
      public string szDisplayName;

      /// <summary>
      /// The type name.
      /// </summary>
      [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 80)]
      public string szTypeName;
    }

    /// <summary>
    /// Retrieves shell file information.
    /// </summary>
    [DllImport("shell32.dll")]
    public static extern IntPtr SHGetFileInfo(string pszPath, uint dwFileAttributes, ref SHFILEINFO psfi, uint cbSizeFileInfo, uint uFlags);

    /// <summary>
    /// Destroys an icon handle.
    /// </summary>
    [DllImport("User32.dll")]
    public static extern int DestroyIcon(IntPtr hIcon);

    #endregion Interop

    /// <summary>
    /// Gets or sets the default image source.
    /// </summary>
    public ImageSource DefaultImageSource { get; set; }

    /// <summary>
    /// Gets the small icon for a file.
    /// </summary>
    /// <param name="fileName">The file name.</param>
    /// <returns>The image source.</returns>
    public ImageSource GetSmallIcon(string fileName)
    {
      return GetIcon(fileName, SHGFI_SMALLICON);
    }

    /// <summary>
    /// Gets the icon for a file.
    /// </summary>
    /// <param name="fileName">The file name.</param>
    /// <param name="flags">The shell flags.</param>
    /// <returns>The image source.</returns>
    public ImageSource GetIcon(string fileName, uint flags)
    {
      var shinfo = new SHFILEINFO();
      SHGetFileInfo(fileName, 0, ref shinfo, (uint)Marshal.SizeOf(shinfo), SHGFI_ICON | flags);
      IntPtr iconHandle = shinfo.hIcon;
      if (IntPtr.Zero == iconHandle)
        return DefaultImageSource;
      ImageSource img = Imaging.CreateBitmapSourceFromHIcon(iconHandle, Int32Rect.Empty, BitmapSizeOptions.FromEmptyOptions());
      DestroyIcon(iconHandle);
      return img;
    }

    /// <inheritdoc/>
    public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
    {
      var s = value as string;
      if (string.IsNullOrEmpty(s))
        return DefaultImageSource;
      else
        return GetSmallIcon(s);
    }

    /// <inheritdoc/>
    public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
    {
      throw new NotImplementedException();
    }
  }
}
