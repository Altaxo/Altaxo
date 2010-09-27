using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Altaxo.Data.Extensions
{
	public static class AnalysisRealFourierTransformationCommands
	{
		#region Helper types

		[Flags]
		public enum RealFourierTransformOutput
		{
			Re		=0x01,
			Im		=0x02,
			Abs		=0x04,
			Phase	=0x08,
			Power	=0x10
		}

		public enum RealFourierTransformOutputPlacement
		{
			CreateInSameWorksheet,
			CreateInNewWorksheet
		}

		public class RealFourierTransformOptions
		{
			public DataColumn ColumnToTransform { get; set; }
			public string XIncrementMessage { get; set; }
			public double XIncrementValue { get; set; }
			public RealFourierTransformOutput Output { get; set; }
			public RealFourierTransformOutputPlacement OutputPlacement { get; set; }
		}

		#endregion

		public static string DetermineXIncrement(DataColumn yColumnToTransform, out double xIncrement)
		{
			xIncrement = 1;
			var coll = DataColumnCollection.GetParentDataColumnCollectionOf(yColumnToTransform);

			if(null==coll)
				return "Can't find parent collection of provided data column to transform";

			var xColD = coll.FindXColumnOf(yColumnToTransform);
			if(null==xColD)
				return "Can't find x-column of provided data column to transform";

			var xCol = xColD as DoubleColumn;
			if(null==xCol)
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

		public static void RealFourierTransform(RealFourierTransformOptions options)
		{
			var yCol = options.ColumnToTransform;
			int fftLen = yCol.Count;
			DoubleColumn resultCol = new DoubleColumn();
			resultCol.Data = yCol;

			var transform = new Calc.Fourier.RealFourierTransform(fftLen);
			transform.Transform(resultCol.Array, Calc.Fourier.FourierDirection.Forward);

			var wrapper = new Calc.Fourier.RealFFTResultWrapper(resultCol.Array);

			DataTable outputTable = null;
			switch (options.OutputPlacement)
			{
				case RealFourierTransformOutputPlacement.CreateInNewWorksheet:
					outputTable = new DataTable();
					outputTable.Name = "Real FFT results";
					Current.Project.DataTableCollection.Add(outputTable);
					Current.ProjectService.OpenOrCreateWorksheetForTable(outputTable);
					break;
				case RealFourierTransformOutputPlacement.CreateInSameWorksheet:
					outputTable = DataTable.GetParentDataTableOf(yCol);
					if (null == outputTable)
						throw new ArgumentException("Provided y-column does not belong to a data table.");
					break;
				default:
					throw new ArgumentOutOfRangeException("Unkown  enum value: " + options.OutputPlacement.ToString() );
			}

			// create the x-Column first
			var freqCol = new DoubleColumn();
			freqCol.AssignVector = wrapper.FrequenciesFromXIncrement(options.XIncrementValue);
			int outputGroup = outputTable.DataColumns.GetUnusedColumnGroupNumber();
			outputTable.DataColumns.Add(freqCol,"Frequency",ColumnKind.X,outputGroup);

			// now create the other output cols
			if (options.Output.HasFlag(RealFourierTransformOutput.Re))
			{
				var col = new DoubleColumn();
				col.AssignVector = wrapper.RealPart;
				outputTable.DataColumns.Add(col, "Re", ColumnKind.V, outputGroup);
			}

			if (options.Output.HasFlag(RealFourierTransformOutput.Im))
			{
				var col = new DoubleColumn();
				col.AssignVector = wrapper.ImaginaryPart;
				outputTable.DataColumns.Add(col, "Im", ColumnKind.V, outputGroup);
			}

			if (options.Output.HasFlag(RealFourierTransformOutput.Abs))
			{
				var col = new DoubleColumn();
				col.AssignVector = wrapper.Amplitude;
				outputTable.DataColumns.Add(col, "Abs", ColumnKind.V, outputGroup);
			}

			if (options.Output.HasFlag(RealFourierTransformOutput.Phase))
			{
				var col = new DoubleColumn();
				col.AssignVector = wrapper.Phase;
				outputTable.DataColumns.Add(col, "Phase", ColumnKind.V, outputGroup);
			}

			if (options.Output.HasFlag(RealFourierTransformOutput.Power))
			{
				var col = new DoubleColumn();
				col.AssignVector = wrapper.Amplitude;
				col.Data = col * col;
				outputTable.DataColumns.Add(col, "Power", ColumnKind.V, outputGroup);
			}

		}

		public static void ShowRealFourierTransformDialog(DataColumn ycolumnToTransform)
		{
			var options = new RealFourierTransformOptions() { ColumnToTransform = ycolumnToTransform };

			double xIncrementValue;
			options.XIncrementMessage = DetermineXIncrement(ycolumnToTransform, out xIncrementValue);
			options.XIncrementValue = xIncrementValue;


			if (Current.Gui.ShowDialog(ref options, "Choose fourier transform options", false))
			{
				RealFourierTransform(options);
			}
		}
	}
}
