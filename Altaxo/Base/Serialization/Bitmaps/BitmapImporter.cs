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

using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using Altaxo.Data;
using Altaxo.Units;

namespace Altaxo.Serialization.Bitmaps
{
  /// <summary>
  /// Importer for bitmap files into data tables.
  /// </summary>
  public record BitmapImporter : DataFileImporterBase, Main.IImmutable
  {
    /// <inheritdoc/>
    public override (IReadOnlyList<string> FileExtensions, string Explanation) GetFileExtensions()
    {
      return ([".bmp", ".gif", ".jpg", ".jpeg", ".png", ".tif", ".tiff"], "Image files (*.bmp;*.gif;*.jpg;*.jpeg;*.png;*.tif;*.tiff)");
    }

    /// <inheritdoc/>
    public override object CheckOrCreateImportOptions(object? importOptions)
    {
      return (importOptions as BitmapImportOptions) ?? new BitmapImportOptions();
    }

    /// <inheritdoc/>
    public override IAltaxoTableDataSource? CreateTableDataSource(IReadOnlyList<string> fileNames, object importOptions)
    {
      return new BitmapImportDataSource(fileNames, (BitmapImportOptions)importOptions);
    }


    /// <inheritdoc/>
    public override double GetProbabilityForBeingThisFileFormat(string fileName)
    {
      double p = 0;
      var fe = GetFileExtensions();
      if (fe.FileExtensions.ToHashSet().Contains(Path.GetExtension(fileName).ToLowerInvariant()))
      {
        p += 0.5;
      }

      try
      {
        using var stream = new FileStream(fileName, FileMode.Open, FileAccess.Read, FileShare.Read);
        var img = Bitmap.FromStream(stream);

        if (img.Width > 0 && img.Height > 0)
        {
          p += 0.5;
        }
      }
      catch
      {
        p = 0;
      }

      return p;
    }

    /// <summary>
    /// Imports an image to a table.
    /// </summary>
    /// <param name="filenames">An array of filenames to import.</param>
    /// <param name="table">The table the spectra should be imported to.</param>
    /// <returns>Null if no error occurs, or an error description.</returns>
    public override string? Import(IReadOnlyList<string> filenames, Altaxo.Data.DataTable table, object importOptionsObj, bool attachDataSource)
    {
      var importOptions = (BitmapImportOptions)importOptionsObj;
      Altaxo.Data.DoubleColumn? xcol = null;
      var errorList = new System.Text.StringBuilder();
      int lastColumnGroup = 0;

      if (table.DataColumns.ColumnCount > 0)
      {
        lastColumnGroup = table.DataColumns.GetColumnGroup(table.DataColumns.ColumnCount - 1);
        var xColumnOfRightMost = table.DataColumns.FindXColumnOfGroup(lastColumnGroup);
        if (xColumnOfRightMost is DoubleColumn dcolMostRight)
          xcol = dcolMostRight;
      }

      DataColumn? colPixelsC = null, colPixelsR = null;
      if (importOptions.IncludePixelNumberColumns)
      {
        var xy = importOptions.ImportTransposed ? "Y" : "X";
        colPixelsC = table.PropCols.EnsureExistence($"PixelNumber{xy}", typeof(DoubleColumn), ColumnKind.V, 0);
      }

      DataColumn? colDimC = null, colDimR = null;
      if (importOptions.IncludeDimensionColumns)
      {
        var xy = importOptions.ImportTransposed ? "Y" : "X";
        colDimC = table.PropCols.EnsureExistence($"Dimension{xy}", typeof(DoubleColumn), ColumnKind.V, 0);
      }

      DataColumn? colPath = null;
      if (importOptions.IncludeFilePathAsProperty && filenames.Count > 1)
      {
        colPath = table.PropCols.EnsureExistence("FilePath", typeof(TextColumn), ColumnKind.V, 0);
      }

      var yColumnNameBase = importOptions.NeutralColumnName ?? string.Empty;

      int idxFile = -1;
      foreach (string filename in filenames)
      {
        ++idxFile;
        var img = (Bitmap)Bitmap.FromFile(filename);

        if (importOptions.IncludePixelNumberColumns)
        {
          var xy = importOptions.ImportTransposed ? "X" : "Y";
          var columnName = filenames.Count > 1 ? $"PixelNumber{xy}{idxFile}" : $"PixelNumber{xy}";
          colPixelsR = table.DataColumns.EnsureExistence(columnName, typeof(DoubleColumn), ColumnKind.X, idxFile);
        }

        if (importOptions.IncludeDimensionColumns)
        {
          var xy = importOptions.ImportTransposed ? "X" : "Y";
          var columnName = filenames.Count > 1 ? $"Dimension{xy}{idxFile}" : $"Dimension{xy}";
          colDimR = table.DataColumns.EnsureExistence(columnName, typeof(DoubleColumn), ColumnKind.V, idxFile);
        }

        if (importOptions.ImportTransposed)
        {
          for (int iy = 0; iy < img.Height; ++iy)
          {
            if (colPixelsR is not null)
            {
              colPixelsR[iy] = iy;
            }
            if (colDimR is not null)
            {
              colDimR[iy] = new DimensionfulQuantity(iy / img.VerticalResolution, Altaxo.Units.Length.Inch.Instance).AsValueInSIUnits;
            }
          }

          for (int ix = 0; ix < img.Width; ix++)
          {
            var columnName = filenames.Count > 1 ? $"{yColumnNameBase}{idxFile}_{ix}" : $"{yColumnNameBase}{ix}";
            var ycol = table.DataColumns.EnsureExistence(columnName, typeof(DoubleColumn), ColumnKind.V, idxFile);
            var ycolNumber = table.DataColumns.GetColumnNumber(ycol);
            if (colPixelsC is not null)
            {
              colPixelsC[ycolNumber] = ix;
            }
            if (colDimC is not null)
            {
              colDimC[ycolNumber] = new DimensionfulQuantity(ix / img.HorizontalResolution, Altaxo.Units.Length.Inch.Instance).AsValueInSIUnits;
            }
            if (colPath is not null)
            {
              colPath[ycolNumber] = filename;
            }

            for (int iy = 0; iy < img.Height; iy++)
            {
              ycol[iy] = GetPixelValue(img.GetPixel(ix, iy), importOptions.ColorChannel);
            }
          }
        }
        else
        {
          for (int ix = 0; ix < img.Width; ++ix)
          {
            if (colPixelsR is not null)
            {
              colPixelsR[ix] = ix;
            }
            if (colDimR is not null)
            {
              colDimR[ix] = new DimensionfulQuantity(ix / img.HorizontalResolution, Altaxo.Units.Length.Inch.Instance).AsValueInSIUnits;
            }
          }

          for (int iy = 0; iy < img.Height; iy++)
          {
            var columnName = filenames.Count > 1 ? $"{yColumnNameBase}{idxFile}_{iy}" : $"{yColumnNameBase}{iy}";
            var ycol = table.DataColumns.EnsureExistence(columnName, typeof(DoubleColumn), ColumnKind.V, idxFile);
            var ycolNumber = table.DataColumns.GetColumnNumber(ycol);
            if (colPixelsC is not null)
            {
              colPixelsC[ycolNumber] = iy;
            }
            if (colDimC is not null)
            {
              colDimC[ycolNumber] = new DimensionfulQuantity(iy / img.VerticalResolution, Altaxo.Units.Length.Inch.Instance).AsValueInSIUnits;
            }
            if (colPath is not null)
            {
              colPath[ycolNumber] = filename;
            }

            for (int ix = 0; ix < img.Width; ix++)
            {
              ycol[ix] = GetPixelValue(img.GetPixel(ix, iy), importOptions.ColorChannel);
            }
          }
        }



      } // foreach file


      if (attachDataSource)
      {
        table.DataSource = CreateTableDataSource(filenames, importOptions);
      }

      return errorList.Length == 0 ? null : errorList.ToString();
    }

    /// <summary>
    /// Gets the pixel value, depending on the color channel.
    /// </summary>
    /// <param name="c">The color that should be converted.</param>
    /// <param name="channel">The color channel.</param>
    /// <returns>The pixel value.</returns>
    private double GetPixelValue(Color c, ColorChannel channel)
    {
      return channel switch
      {
        ColorChannel.HSLLightness => c.GetBrightness(),
        ColorChannel.HSLSaturation => c.GetSaturation(),
        ColorChannel.HSLHue => c.GetHue() / 360d,
        ColorChannel.Red => c.R / 255d,
        ColorChannel.Green => c.G / 255d,
        ColorChannel.Blue => c.B / 255d,
        ColorChannel.Alpha => c.A / 255d,
        _ => throw new System.NotImplementedException($"Color channel {channel} is not implemented yet."),
      };
    }
  }
}
