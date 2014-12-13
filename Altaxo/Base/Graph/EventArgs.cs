using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Altaxo.Graph
{
	public class PlotItemDataChangedEventArgs : EventArgs
	{
		public static new readonly PlotItemDataChangedEventArgs Empty = new PlotItemDataChangedEventArgs();

		private PlotItemDataChangedEventArgs()
		{
		}
	}

	public class PlotItemStyleChangedEventArgs : EventArgs
	{
		public static new readonly PlotItemStyleChangedEventArgs Empty = new PlotItemStyleChangedEventArgs();

		private PlotItemStyleChangedEventArgs()
		{
		}
	}

	[Flags]
	public enum BoundariesChangedData
	{
		NumberOfItemsChanged = 0x01,
		LowerBoundChanged = 0x02,
		UpperBoundChanged = 0x04,
		XBoundariesChanged = 0x10,
		YBoundariesChanged = 0x20,
		ZBoundariesChanged = 0x40,
		VBoundariesChanged = 0x80,
	}

	public class BoundariesChangedEventArgs : System.EventArgs
	{
		protected BoundariesChangedData _data;

		public static T FromLowerAndUpperBoundChanged<T>(bool haslowerBoundChanged, bool hasUpperBoundChanged) where T : BoundariesChangedEventArgs, new()
		{
			var result = new T();

			if (haslowerBoundChanged)
				result._data |= BoundariesChangedData.LowerBoundChanged;
			if (hasUpperBoundChanged)
				result._data |= BoundariesChangedData.UpperBoundChanged;

			return result;
		}

		public BoundariesChangedEventArgs()
		{
		}

		public BoundariesChangedEventArgs(BoundariesChangedData data)
		{
			_data = data;
		}

		public BoundariesChangedEventArgs(bool bLowerBound, bool bUpperBound)
		{
			if (bLowerBound)
				_data |= BoundariesChangedData.LowerBoundChanged;
			if (bUpperBound)
				_data |= BoundariesChangedData.UpperBoundChanged;
		}

		public bool LowerBoundChanged
		{
			get { return _data.HasFlag(BoundariesChangedData.LowerBoundChanged); }
		}

		public bool UpperBoundChanged
		{
			get { return _data.HasFlag(BoundariesChangedData.UpperBoundChanged); }
		}

		public void SetXBoundaryChangedFlag()
		{
			_data |= BoundariesChangedData.XBoundariesChanged;
		}

		public void SetYBoundaryChangedFlag()
		{
			_data |= BoundariesChangedData.YBoundariesChanged;
		}

		public void SetZBoundaryChangedFlag()
		{
			_data |= BoundariesChangedData.ZBoundariesChanged;
		}

		public void SetVBoundaryChangedFlag()
		{
			_data |= BoundariesChangedData.VBoundariesChanged;
		}

		public void Add(BoundariesChangedEventArgs other)
		{
			_data |= other._data;
		}

		public void Add(BoundariesChangedData other)
		{
			_data |= other;
		}
	}

	public class PlotItemBoundariesChangedEventArgs : BoundariesChangedEventArgs
	{
	}

	public class ScaleBoundariesChangedEventArgs : BoundariesChangedEventArgs
	{
	}
}