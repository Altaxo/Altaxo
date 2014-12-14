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

	public class BoundariesChangedEventArgs : Main.SelfAccumulateableEventArgs
	{
		protected BoundariesChangedData _data;

		public BoundariesChangedData Data { get { return _data; } }

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

		public override void Add(Main.SelfAccumulateableEventArgs e)
		{
			var other = e as BoundariesChangedEventArgs;
			if (other == null)
				throw new ArgumentException(string.Format("Argument e should be of type {0}, but is: {1}", typeof(BoundariesChangedEventArgs), e.GetType()));

			_data |= other._data;
		}

		public void Add(BoundariesChangedData other)
		{
			_data |= other;
		}
	}

	public class ScaleInstanceChangedEventArgs : Main.SelfAccumulateableEventArgs
	{
		private int _scaleIndex = -1;
		private Altaxo.Graph.Scales.Scale _oldScale;
		private Altaxo.Graph.Scales.Scale _newScale;

		public int ScaleIndex { get { return _scaleIndex; } set { _scaleIndex = value; } }

		public Altaxo.Graph.Scales.Scale OldScale { get { return _oldScale; } }

		public Altaxo.Graph.Scales.Scale NewScale { get { return _newScale; } }

		public ScaleInstanceChangedEventArgs(Altaxo.Graph.Scales.Scale oldScale, Altaxo.Graph.Scales.Scale newScale)
		{
			_oldScale = oldScale;
			_newScale = newScale;
		}

		public override void Add(Main.SelfAccumulateableEventArgs e)
		{
			var other = e as ScaleInstanceChangedEventArgs;
			if (null == other)
				throw new ArgumentException("Expect event args of type: " + typeof(ScaleInstanceChangedEventArgs).ToString());

			if (this.ScaleIndex != other.ScaleIndex)
				throw new InvalidProgramException("This should not happen, because the overrides for GetHashCode and Equals should prevent this.");

			this._newScale = other._newScale;
		}

		/// <summary>
		/// Override to ensure that only one instance with a given ScaleIndex is contained in the event args collection.
		/// </summary>
		/// <returns>
		/// A hash code for this instance, suitable for use in hashing algorithms and data structures like a hash table.
		/// </returns>
		public override int GetHashCode()
		{
			return 17 * this.GetType().GetHashCode() + 31 * _scaleIndex;
		}

		/// <summary>
		/// Override to ensure that only one instance with a given ScaleIndex is contained in the event args collection.
		/// </summary>
		/// <param name="obj">The <see cref="System.Object" /> to compare with this instance.</param>
		/// <returns>
		///   <c>true</c> if the specified <see cref="System.Object" /> is equal to this instance; otherwise, <c>false</c>.
		/// </returns>
		public override bool Equals(object obj)
		{
			var other = obj as ScaleInstanceChangedEventArgs;
			if (null == other)
				return false;
			else
				return this._scaleIndex == other._scaleIndex;
		}
	}
}