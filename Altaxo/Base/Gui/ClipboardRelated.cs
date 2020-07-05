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

#nullable enable
using System;

namespace Altaxo.Gui
{
  /// <summary>
  /// Interface for a data object to put data on the clipboard.
  /// </summary>
  public interface IClipboardSetDataObject
  {
    void SetImage(System.Drawing.Image image);

    void SetFileDropList(System.Collections.Specialized.StringCollection filePaths);

    void SetData(string format, object data);

    void SetData(Type format, object data);

    void SetCommaSeparatedValues(string text);
  }

  /// <summary>
  /// Interface for a data object to get data from the clipboard.
  /// </summary>
  public interface IClipboardGetDataObject
  {
    string[] GetFormats();

    bool GetDataPresent(string format);

    bool GetDataPresent(System.Type type);

    object? GetData(string format);

    object? GetData(System.Type type);

    bool ContainsFileDropList();

    System.Collections.Specialized.StringCollection GetFileDropList();

    bool ContainsImage();

    System.Drawing.Image GetImage();

    /// <summary>
    /// Gets the bitmap image on the clipboard as optimized memory stream. Optimized means that it will be tested whether compression
    /// with jpeg or with png is more efficient, and the stream that is smaller in size will be returned.
    /// </summary>
    /// <returns>If successfull, the stream and the file extension that describes the kind of stream. If unsuccessfull, the tuple (null, null) is returned.</returns>
    (System.IO.Stream, string fileExtension) GetBitmapImageAsOptimizedMemoryStream();
  }
}
