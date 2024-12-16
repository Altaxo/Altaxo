#region Copyright

/////////////////////////////////////////////////////////////////////////////
//    Altaxo:  a data processing and data plotting program
//    Copyright (C) 2002-2024 Dr. Dirk Lellinger
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


namespace Altaxo.Serialization.Bitmaps
{

  /// <summary>
  /// Import options for importing bitmap files on a pixel-by-pixel basis.
  /// </summary>
  public record BitmapImportOptions : Main.IImmutable
  {
    /// <summary>
    /// Gets the neutral column name (base). The name is at request extended, e.g. by a number at the end.
    /// </summary>
    public string NeutralColumnName { get; init; } = "V";

    /// <summary>
    /// If true, the file name of the imported file is included as a column property.
    /// </summary>
    public bool IncludeFilePathAsProperty { get; init; } = true;

    /// <summary>
    /// Gets a value indicating whether to include the dimensions ("DimensionX" and "DimensionY")
    /// </summary>
    public bool IncludeDimensionColumns { get; init; } = true;

    /// <summary>
    /// Gets a value indicating whether to include the dimensions ("DimensionX" and "DimensionY")
    /// </summary>
    public bool IncludePixelNumberColumns { get; init; } = false;

    /// <summary>
    /// Gets a value indicating whether to transpose the table.
    /// Untransposed, the x-values are written into the rows of the table, which will result in a plot similar to the image, but the order of the pixels in the table is somewhat counterintuitive.
    /// Transposed, the x-values of the image are written into the columns of the table, so that the order of pixels in the table and the image is similar, but then if you plot that as a density image,
    /// the resulting density image has x and y exchanged.
    /// </summary>
    public bool ImportTransposed { get; init; }

    /// <summary>
    /// Gets a value indicating which part of the color of each pixel to use for the value that imported.
    /// </summary>
    public ColorChannel ColorChannel { get; init; }

    #region Serialization

    /// <summary>
    /// 
    /// </summary>
    /// <seealso cref="Altaxo.Serialization.Xml.IXmlSerializationSurrogate" />
    [Altaxo.Serialization.Xml.XmlSerializationSurrogateFor(typeof(BitmapImportOptions), 0)]
    public class SerializationSurrogate0 : Altaxo.Serialization.Xml.IXmlSerializationSurrogate
    {
      public void Serialize(object obj, Altaxo.Serialization.Xml.IXmlSerializationInfo info)
      {
        var s = (BitmapImportOptions)obj;
        info.AddValue("NeutralColumnName", s.NeutralColumnName);
        info.AddValue("IncludeFilePathAsProperty", s.IncludeFilePathAsProperty);
        info.AddValue("IncludePixelNumberColumns", s.IncludePixelNumberColumns);
        info.AddValue("IncludeDimensionColumns", s.IncludeDimensionColumns);
        info.AddValue("ImportTransposed", s.ImportTransposed);
        info.AddEnum("ColorChannel", s.ColorChannel);
      }

      public object Deserialize(object? o, Altaxo.Serialization.Xml.IXmlDeserializationInfo info, object? parent)
      {
        var neutralColumnName = info.GetString("NeutralColumnName");
        var includeFilePathAsProperty = info.GetBoolean("IncludeFilePathAsProperty");
        var includePixelNumberColumns = info.GetBoolean("IncludePixelNumberColumns");
        var includeDimensionColumns = info.GetBoolean("IncludeDimensionColumns");
        var importTransposed = info.GetBoolean("ImportTransposed");
        var colorChannel = info.GetEnum<ColorChannel>("ColorChannel");

        return o is null ? new BitmapImportOptions
        {
          NeutralColumnName = neutralColumnName,
          IncludeFilePathAsProperty = includeFilePathAsProperty,
          IncludePixelNumberColumns = includePixelNumberColumns,
          IncludeDimensionColumns = includeDimensionColumns,
          ImportTransposed = importTransposed,
          ColorChannel = colorChannel,
        } :
          ((BitmapImportOptions)o) with
          {
            NeutralColumnName = neutralColumnName,
            IncludeFilePathAsProperty = includeFilePathAsProperty,
            IncludePixelNumberColumns = includePixelNumberColumns,
            IncludeDimensionColumns = includeDimensionColumns,
            ImportTransposed = importTransposed,
            ColorChannel = colorChannel,
          };
      }
    }
    #endregion

  }
}
