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
    /// <summary>
    /// Sets an image on the clipboard.
    /// </summary>
    /// <param name="image">The image.</param>
    void SetImage(System.Drawing.Image image);

    /// <summary>
    /// Sets a file-drop list on the clipboard.
    /// </summary>
    /// <param name="filePaths">The file paths.</param>
    void SetFileDropList(System.Collections.Specialized.StringCollection filePaths);

    /// <summary>
    /// Sets clipboard data using a string format.
    /// </summary>
    /// <param name="format">The data format.</param>
    /// <param name="data">The data.</param>
    void SetData(string format, object data);

    /// <summary>
    /// Sets clipboard data using a type format.
    /// </summary>
    /// <param name="format">The data type.</param>
    /// <param name="data">The data.</param>
    void SetData(Type format, object data);

    /// <summary>
    /// Sets comma-separated values text on the clipboard.
    /// </summary>
    /// <param name="text">The text.</param>
    void SetCommaSeparatedValues(string text);
  }

  /// <summary>
  /// Interface for a data object to get data from the clipboard.
  /// </summary>
  public interface IClipboardGetDataObject
  {
    /// <summary>
    /// Gets the available clipboard formats.
    /// </summary>
    /// <returns>The available formats.</returns>
    string[] GetFormats();

    /// <summary>
    /// Determines whether data in the specified format is present.
    /// </summary>
    /// <param name="format">The format.</param>
    /// <returns><see langword="true"/> if the data is present; otherwise, <see langword="false"/>.</returns>
    bool GetDataPresent(string format);

    /// <summary>
    /// Determines whether data of the specified type is present.
    /// </summary>
    /// <param name="type">The type.</param>
    /// <returns><see langword="true"/> if the data is present; otherwise, <see langword="false"/>.</returns>
    bool GetDataPresent(System.Type type);

    /// <summary>
    /// Gets clipboard data for the specified format.
    /// </summary>
    /// <param name="format">The format.</param>
    /// <returns>The data, or <see langword="null"/>.</returns>
    object? GetData(string format);

    /// <summary>
    /// Gets clipboard data for the specified type.
    /// </summary>
    /// <param name="type">The type.</param>
    /// <returns>The data, or <see langword="null"/>.</returns>
    object? GetData(System.Type type);

    /// <summary>
    /// Determines whether the clipboard contains a file-drop list.
    /// </summary>
    /// <returns><see langword="true"/> if a file-drop list is present; otherwise, <see langword="false"/>.</returns>
    bool ContainsFileDropList();

    /// <summary>
    /// Gets the file-drop list from the clipboard.
    /// </summary>
    /// <returns>The file-drop list.</returns>
    System.Collections.Specialized.StringCollection GetFileDropList();

    /// <summary>
    /// Determines whether the clipboard contains an image.
    /// </summary>
    /// <returns><see langword="true"/> if an image is present; otherwise, <see langword="false"/>.</returns>
    bool ContainsImage();

    /// <summary>
    /// Gets the image (or null if no image is present).
    /// </summary>
    /// <returns>The image that the clipboard contains; or null.</returns>
    System.Drawing.Image? GetImage();

    /// <summary>
    /// Gets the bitmap image on the clipboard as optimized memory stream. Optimized means that it will be tested whether compression
    /// with jpeg or with png is more efficient, and the stream that is smaller in size will be returned.
    /// </summary>
    /// <returns>If successful, the stream and the file extension that describes the kind of stream. If unsuccessful, the tuple (<see langword="null"/>, <see langword="null"/>) is returned.</returns>
    (System.IO.Stream? Stream, string? FileExtension) GetBitmapImageAsOptimizedMemoryStream();
  }
}
