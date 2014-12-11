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

using Altaxo.Serialization;
using System;

namespace Altaxo.Graph.Scales.Boundaries
{
	[Flags]
	public enum BoundariesChangedData
	{
		LowerBoundChanged = 0x01,
		UpperBoundChanged = 0x02,
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

		public void Add(BoundariesChangedEventArgs other)
		{
			_data |= other._data;
		}
	}

	public class PlotItemBoundariesChangedEventArgs : BoundariesChangedEventArgs
	{
	}

	public class ScaleBoundariesChangedEventArgs : BoundariesChangedEventArgs
	{
	}
}