using Altaxo.Calc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Altaxo.Graph
{
	public class LinearPartitioning : System.Collections.ObjectModel.ObservableCollection<Altaxo.Calc.RelativeOrAbsoluteValue>
	{
		public double[] GetPartitionPositions(double totalSize)
		{
			double relSum = this.Sum(x => x.IsRelative ? x.Value : 0);
			double absSum = this.Sum(x => x.IsAbsolute ? x.Value : 0);

			double absValuePerRelativeValue = (totalSize - absSum) / relSum;

			int i = -1;
			double position = 0;
			double[] result = new double[this.Count];
			foreach (var x in this)
			{
				position += x.IsAbsolute ? x.Value : x.Value * absValuePerRelativeValue;
				result[++i] = position;
			}
			return result;
		}

		/// <summary>
		/// Gets the partition position. A relative value of 0 gives the absolute position 0, a value of 1 gives the size of the first partition, a value of two the size of the first plus second partition and so on.
		/// </summary>
		/// <param name="relativeValue">The relative value.</param>
		/// <param name="totalSize">The total size.</param>
		/// <returns></returns>
		public double GetPartitionPosition(double relativeValue, double totalSize)
		{
			var partPositions = GetPartitionPositions(totalSize);

			if (relativeValue < 0)
			{
				return relativeValue * totalSize;
			}
			else if (relativeValue < partPositions.Length)
			{
				int rlower = (int)Math.Floor(relativeValue);
				double r = relativeValue - rlower;
				double pl = rlower == 0 ? 0 : partPositions[rlower - 1];
				double pu = partPositions[rlower];
				return pl * (1 - r) + r * pu;
			}
			else if (relativeValue >= partPositions.Length)
			{
				return (relativeValue - partPositions.Length) * totalSize + totalSize;
			}
			else
			{
				return double.NaN;
			}
		}

		public void GetTilePositionSize(double start, double span, double totalSize, out double absoluteStart, out double absoluteSize)
		{
			absoluteStart = GetPartitionPosition(start, totalSize);
			absoluteSize = GetPartitionPosition(start + span, totalSize) - absoluteStart;
		}

		public double GetSumRelativeValues()
		{
			return this.Sum(x => x.IsRelative ? x.Value : 0);
		}
	}

	public class GridPartitioning
	{
		private LinearPartitioning _xPartitioning;
		private LinearPartitioning _yPartitioning;

		public GridPartitioning()
		{
			_xPartitioning = new LinearPartitioning();
			_yPartitioning = new LinearPartitioning();
		}

		public LinearPartitioning XPartitioning { get { return _xPartitioning; } }

		public LinearPartitioning YPartitioning { get { return _yPartitioning; } }

		public RectangleD GetTileRectangle(double column, double row, double columnSpan, double rowSpan, PointD2D totalSize)
		{
			double xstart, xsize;
			double ystart, ysize;
			_xPartitioning.GetTilePositionSize(column, columnSpan, totalSize.X, out xstart, out xsize);
			_yPartitioning.GetTilePositionSize(row, rowSpan, totalSize.Y, out ystart, out ysize);
			return new RectangleD(xstart, ystart, xsize, ysize);
		}
	}
}