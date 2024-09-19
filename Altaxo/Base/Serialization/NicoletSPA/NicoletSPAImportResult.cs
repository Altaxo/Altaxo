using System;
using System.IO;
using Altaxo.Data;

namespace Altaxo.Serialization.NicoletSPA
{
  public class NicoletSPAImportResult
  {
    public double XFirst { get; protected set; }
    public double XLast { get; protected set; }
    public double XIncrement { get; protected set; }

    public int NumberOfPoints { get; protected set; }

    /// <summary>The label of the x-axis.</summary>
    public string? XLabel { get; protected set; } = null;

    /// <summary>The label of the y-axis.</summary>
    public string? YLabel { get; protected set; } = null;

    /// <summary>The unit of the x-axis.</summary>
    public string? XUnit { get; protected set; } = null;

    /// <summary>The unit of the y-axis.</summary>
    public string? YUnit { get; protected set; } = null;

    /// <summary>
    /// Messages about any errors during the import of the file.
    /// </summary>
    public string? ErrorMessages { get; protected set; } = null;

    public string Comment = string.Empty;

    public double[] X { get; protected set; }
    public double[] Y { get; protected set; }


    public NicoletSPAImportResult(Stream stream)
    {
      const int Pos_BeginComment = 0x1E; // 30 dez, position where the comment starts
      const int Pos_EndComment = 0x100; // 255 dez, position where the comment ends (exclusive)
      const int Pos_StartSearchMarkerBeforeOffset = 0x120; // 288 dez, position at which to search for the start marker 0x0003, after which the offset to the data can be found
      const int StartMarkerForOffset = 0x0003;
      const int Pos_NumberOfPoints = 0x234; // 564 dez, position where to find the number of data points
      const int Pos_MinMax = 0x240; // 576 dez, position where to find minimum and maximum of the x-axis

      if (stream is null)
        throw new ArgumentNullException(nameof(stream));
      if (!stream.CanSeek)
        throw new ArgumentException($"{nameof(stream)} must be seekable!");

      stream.Seek(Pos_BeginComment, SeekOrigin.Begin); // Begin of the comment section
      var buffer = new byte[Pos_EndComment - Pos_BeginComment];
      stream.Read(buffer, 0, buffer.Length);
      var comment = System.Text.Encoding.UTF8.GetString(buffer);
      Comment = comment.TrimEnd('\0');
      stream.Seek(Pos_NumberOfPoints, SeekOrigin.Begin);
      stream.Read(buffer, 0, sizeof(Int32));
      var numberOfPoints = BitConverter.ToInt32(buffer, 0);

      // get minimum and maximum wavenumbers
      stream.Seek(Pos_MinMax, SeekOrigin.Begin);
      stream.Read(buffer, 0, 2 * sizeof(Single));
      var min = BitConverter.ToSingle(buffer, 0);
      var max = BitConverter.ToSingle(buffer, sizeof(Single));

      XFirst = min;
      XLast = max;
      XIncrement = (max - min) / (numberOfPoints - 1d);
      NumberOfPoints = numberOfPoints;

      // locate the offset to the data
      // search for the start marker 0x0003, after which the offset can be found
      stream.Seek(Pos_StartSearchMarkerBeforeOffset, SeekOrigin.Begin);
      do
      {
        stream.Read(buffer, 0, sizeof(Int16));
      } while (StartMarkerForOffset != BitConverter.ToInt16(buffer, 0));

      // now read the offset
      stream.Read(buffer, 0, sizeof(Int16));
      var offset = BitConverter.ToInt16(buffer, 0);

      var ybuffer = new byte[numberOfPoints * sizeof(float)];
      stream.Seek(offset, SeekOrigin.Begin);
      stream.Read(ybuffer, 0, ybuffer.Length);

      var x = new double[numberOfPoints];
      var y = new double[numberOfPoints];

      for (int i = 0; i < numberOfPoints; i++)
      {
        var rel = i / (numberOfPoints - 1d);
        x[i] = min * (1 - rel) + max * rel;
        y[i] = BitConverter.ToSingle(ybuffer, i * sizeof(float));
      }

      X = x;
      Y = y;
    }

    /// <summary>
    /// Imports the data of this <see cref="Import"/> instance into a <see cref="DataTable"/>.
    /// </summary>
    /// <param name="table">The table.</param>
    public NicoletSPAImportResult(Stream stream, DataTable table) : this(stream)
    {
      var xCol = new DoubleColumn();
      var yCol = new DoubleColumn();
      table.DataColumns.Add(xCol, string.IsNullOrEmpty(XLabel) ? "X" : XLabel, ColumnKind.X);
      table.DataColumns.Add(yCol, string.IsNullOrEmpty(YLabel) ? "Y" : YLabel, ColumnKind.V);

      if (!string.IsNullOrEmpty(XUnit) || !string.IsNullOrEmpty(YUnit))
      {
        table.PropCols.Add(new TextColumn(), "Unit", ColumnKind.V);
        table.PropCols["Unit"][0] = XUnit;
        table.PropCols["Unit"][1] = YUnit;
      }

      if (!string.IsNullOrEmpty(XLabel) || !string.IsNullOrEmpty(YLabel))
      {
        table.PropCols.Add(new TextColumn(), "Label", ColumnKind.V);
        table.PropCols["Label"][0] = XLabel;
        table.PropCols["Label"][1] = YLabel;
      }

      xCol.Data = X;
      yCol.Data = Y;
    }

    /// <summary>
    /// Compare the values in a double array with values in a double column and see if they match.
    /// </summary>
    /// <param name="values">An array of double values.</param>
    /// <param name="col">A double column to compare with the double array.</param>
    /// <returns>True if the length of the array is equal to the length of the <see cref="DoubleColumn" /> and the values in
    /// both array match to each other, otherwise false.</returns>
    public static bool ValuesMatch(DoubleColumn values, DoubleColumn col)
    {
      if (values.Count != col.Count)
        return false;

      for (int i = 0; i < values.Count; i++)
        if (col[i] != values[i])
          return false;

      return true;
    }
  }
}
