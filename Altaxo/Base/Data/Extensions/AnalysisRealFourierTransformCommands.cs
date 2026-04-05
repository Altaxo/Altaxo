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

#nullable enable
using System;

namespace Altaxo.Data
{
  /// <summary>
  /// Commands for applying a real Fourier transform to worksheet columns.
  /// </summary>
  public static class AnalysisRealFourierTransformationCommands
  {
    #region Helper types

    /// <summary>
    /// Output kinds for the real Fourier transform.
    /// </summary>
    [Flags]
    public enum RealFourierTransformOutput
    {
      /// <summary>
      /// Real part.
      /// </summary>
      Re = 0x01,
      /// <summary>
      /// Imaginary part.
      /// </summary>
      Im = 0x02,
      /// <summary>
      /// Magnitude.
      /// </summary>
      Abs = 0x04,
      /// <summary>
      /// Phase.
      /// </summary>
      Phase = 0x08,
      /// <summary>
      /// Power.
      /// </summary>
      Power = 0x10
    }

    /// <summary>
    /// Determines where transform results are written.
    /// </summary>
    public enum RealFourierTransformOutputPlacement
    {
      /// <summary>
      /// Create the result in the same worksheet.
      /// </summary>
      CreateInSameWorksheet,
      /// <summary>
      /// Create the result in a new worksheet.
      /// </summary>
      CreateInNewWorksheet
    }

    /// <summary>
    /// Options for a real Fourier transform operation.
    /// </summary>
    public class RealFourierTransformOptions : ICloneable
    {
      /// <summary>
      /// Gets or sets the column to transform.
      /// </summary>
      public DataColumn? ColumnToTransform { get; set; }

      /// <summary>
      /// Gets or sets the message describing the x-increment determination.
      /// </summary>
      public string? XIncrementMessage { get; set; }

      /// <summary>
      /// Gets or sets the x-increment value.
      /// </summary>
      public double XIncrementValue { get; set; }

      /// <summary>
      /// Gets or sets the requested output kinds.
      /// </summary>
      public RealFourierTransformOutput Output { get; set; }

      /// <summary>
      /// Gets or sets where the output is written.
      /// </summary>
      public RealFourierTransformOutputPlacement OutputPlacement { get; set; }

      /// <inheritdoc />
      public object Clone()
      {
        return MemberwiseClone();
      }
    }

    #endregion Helper types

    /// <summary>
    /// Determines the x-increment from the x-column associated with the specified y-column.
    /// </summary>
    /// <param name="yColumnToTransform">The y-column to be transformed.</param>
    /// <param name="xIncrement">Receives the determined x-increment.</param>
    /// <returns>An error message if the increment could not be determined exactly; otherwise, <see langword="null"/>.</returns>
    public static string? DetermineXIncrement(DataColumn yColumnToTransform, out double xIncrement)
    {
      xIncrement = 1;
      var coll = DataColumnCollection.GetParentDataColumnCollectionOf(yColumnToTransform);

      if (coll is null)
        return "Can't find parent collection of provided data column to transform";

      var xColD = coll.FindXColumnOf(yColumnToTransform);
      if (xColD is null)
        return "Can't find x-column of provided data column to transform";

      if (!(xColD is DoubleColumn xCol))
        return "X-column of provided data column to transform is not a numeric column";

      var spacing = new Calc.LinearAlgebra.VectorSpacingEvaluator(xCol.ToROVector());
      if (!spacing.IsStrictlyMonotonicIncreasing)
        return "X-column of provided column to transform is not monotonically increasing";

      xIncrement = spacing.SpaceMeanValue;

      if (!spacing.IsStrictlyEquallySpaced)
        return "X-Column is not strictly equally spaced, the relative deviation is " + spacing.RelativeSpaceDeviation.ToString();
      else
        return null;
    }

    /// <summary>
    /// Performs a real Fourier transform using the specified options.
    /// </summary>
    /// <param name="options">The transform options.</param>
    public static void RealFourierTransform(RealFourierTransformOptions options)
    {
      var yCol = options.ColumnToTransform ?? throw new InvalidOperationException("Y-column to be transformed is null");
      int fftLen = yCol.Count;

      double[] resultCol = new double[fftLen];
      for (int i = 0; i < resultCol.Length; ++i)
        resultCol[i] = yCol[i];

      var transform = new Calc.Fourier.RealFourierTransform(fftLen);
      transform.Transform(resultCol, Calc.Fourier.FourierDirection.Forward);

      var wrapper = new Calc.Fourier.RealFFTResultWrapper(resultCol);

      DataTable? outputTable;
      switch (options.OutputPlacement)
      {
        case RealFourierTransformOutputPlacement.CreateInNewWorksheet:
          outputTable = new DataTable
          {
            Name = "Real FFT results"
          };
          Current.Project.DataTableCollection.Add(outputTable);
          Current.ProjectService.OpenOrCreateWorksheetForTable(outputTable);
          break;

        case RealFourierTransformOutputPlacement.CreateInSameWorksheet:
          outputTable = DataTable.GetParentDataTableOf(yCol);
          if (outputTable is null)
            throw new ArgumentException("Provided y-column does not belong to a data table.");
          break;

        default:
          throw new ArgumentOutOfRangeException("Unkown  enum value: " + options.OutputPlacement.ToString());
      }

      // create the x-Column first
      var freqCol = new DoubleColumn
      {
        AssignVector = wrapper.FrequenciesFromXIncrement(options.XIncrementValue)
      };
      int outputGroup = outputTable.DataColumns.GetUnusedColumnGroupNumber();
      outputTable.DataColumns.Add(freqCol, "Frequency", ColumnKind.X, outputGroup);

      // now create the other output cols
      if (options.Output.HasFlag(RealFourierTransformOutput.Re))
      {
        var col = new DoubleColumn
        {
          AssignVector = wrapper.RealPart
        };
        outputTable.DataColumns.Add(col, "Re", ColumnKind.V, outputGroup);
      }

      if (options.Output.HasFlag(RealFourierTransformOutput.Im))
      {
        var col = new DoubleColumn
        {
          AssignVector = wrapper.ImaginaryPart
        };
        outputTable.DataColumns.Add(col, "Im", ColumnKind.V, outputGroup);
      }

      if (options.Output.HasFlag(RealFourierTransformOutput.Abs))
      {
        var col = new DoubleColumn
        {
          AssignVector = wrapper.Amplitude
        };
        outputTable.DataColumns.Add(col, "Abs", ColumnKind.V, outputGroup);
      }

      if (options.Output.HasFlag(RealFourierTransformOutput.Phase))
      {
        var col = new DoubleColumn
        {
          AssignVector = wrapper.Phase
        };
        outputTable.DataColumns.Add(col, "Phase", ColumnKind.V, outputGroup);
      }

      if (options.Output.HasFlag(RealFourierTransformOutput.Power))
      {
        var col = new DoubleColumn
        {
          AssignVector = wrapper.Amplitude
        };
        col.Data = col * col;
        outputTable.DataColumns.Add(col, "Power", ColumnKind.V, outputGroup);
      }
    }

    /// <summary>
    /// Shows the dialog for configuring and running a real Fourier transform.
    /// </summary>
    /// <param name="ycolumnToTransform">The y-column to transform.</param>
    public static void ShowRealFourierTransformDialog(DataColumn ycolumnToTransform)
    {
      var options = new RealFourierTransformOptions() { ColumnToTransform = ycolumnToTransform };

      options.XIncrementMessage = DetermineXIncrement(ycolumnToTransform, out var xIncrementValue);
      options.XIncrementValue = xIncrementValue;

      if (Current.Gui.ShowDialog(ref options, "Choose fourier transform options", false) && !(options is null))
      {
        RealFourierTransform(options);
      }
    }
  }
}
