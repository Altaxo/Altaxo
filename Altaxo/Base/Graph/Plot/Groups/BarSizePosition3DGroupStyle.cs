#region Copyright

/////////////////////////////////////////////////////////////////////////////
//    Altaxo:  a data processing and data plotting program
//    Copyright (C) 2002-2016 Dr. Dirk Lellinger
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

using System;
using System.Collections.Generic;
using System.Text;

namespace Altaxo.Graph.Plot.Groups
{
	public class BarSizePosition3DGroupStyle
		:
		Main.SuspendableDocumentLeafNodeWithEventArgs,
		IPlotGroupStyle
	{
		private bool _isInitialized;
		private bool _isStepEnabled;

		/// <summary>Is set to true if a BarPlotStyle has touched the group style during a prepare step.
		/// Helps to prevent the counting of more than one item per step (in case there is more than one
		/// BarStyle in a PlotItem.</summary>
		private bool _wasTouchedInThisPrepareStep;

		private int _numberOfItems;

		/// <summary>
		/// Relative gap between the bars belonging to the same x-value.
		/// A value of 0.5 means that the gap has half of the width of one bar.
		/// </summary>
		private double _relInnerGapX;

		/// <summary>
		/// Relative gap between the bars between two consecutive x-values.
		/// A value of 1 means that the gap has the same width than one bar.
		/// </summary>
		private double _relOuterGapX;

		/// <summary>
		/// Relative gap between the bars belonging to the same y-value.
		/// A value of 0.5 means that the gap has half of the width of one bar.
		/// </summary>
		private double _relInnerGapY;

		/// <summary>
		/// Relative gap between the bars between two consecutive y-values.
		/// A value of 1 means that the gap has the same width than one bar.
		/// </summary>
		private double _relOuterGapY;

		/// <summary>
		/// The width of one cluster of bars (including the gaps) in units of logical scale values.
		/// </summary>
		private double _logicalClusterSizeX;

		/// <summary>
		/// The depth of one cluster of bars (including the gaps) in units of logical scale values.
		/// </summary>
		private double _logicalClusterSizeY;

		private double _sizeX;
		private double _positionX;

		/// <summary>The number of items in x-direction. This field is used in Step() to switch in y-Direction after every this number of items Steps in x-Direction.</summary>
		private int _numberOfItemsX;

		private double _sizeY;
		private double _positionY;

		#region Serialization

		[Altaxo.Serialization.Xml.XmlSerializationSurrogateFor(typeof(BarSizePosition3DGroupStyle), 0)]
		private class XmlSerializationSurrogate0 : Altaxo.Serialization.Xml.IXmlSerializationSurrogate
		{
			public void Serialize(object obj, Altaxo.Serialization.Xml.IXmlSerializationInfo info)
			{
				var s = (BarSizePosition3DGroupStyle)obj;
				info.AddValue("StepEnabled", s._isStepEnabled);
			}

			public object Deserialize(object o, Altaxo.Serialization.Xml.IXmlDeserializationInfo info, object parent)
			{
				var s = (BarSizePosition3DGroupStyle)o ?? new BarSizePosition3DGroupStyle();
				s._isStepEnabled = info.GetBoolean("StepEnabled");
				return s;
			}
		}

		#endregion Serialization

		private void CopyFrom(BarSizePosition3DGroupStyle from)
		{
			if (object.ReferenceEquals(this, from))
				return;

			_isInitialized = from._isInitialized;
			_isStepEnabled = from._isStepEnabled;
			_wasTouchedInThisPrepareStep = from._wasTouchedInThisPrepareStep;
			_numberOfItems = from._numberOfItems;
			_relInnerGapX = from._relInnerGapX;
			_relOuterGapX = from._relOuterGapX;
			_relInnerGapY = from._relInnerGapY;
			_relOuterGapY = from._relOuterGapY;
			_logicalClusterSizeX = from._logicalClusterSizeX;
			_logicalClusterSizeY = from._logicalClusterSizeY;
			_sizeX = from._sizeX;
			_positionX = from._positionX;
			_sizeY = from._sizeY;
			_positionY = from._positionY;
		}

		public void TransferFrom(IPlotGroupStyle fromb)
		{
			var from = (BarSizePosition3DGroupStyle)fromb;
			_isInitialized = from._isInitialized;
			_numberOfItems = from._numberOfItems;
			_relInnerGapX = from._relInnerGapX;
			_relOuterGapX = from._relOuterGapX;
			_relInnerGapY = from._relInnerGapY;
			_relOuterGapY = from._relOuterGapY;
			_logicalClusterSizeX = from._logicalClusterSizeX;
			_logicalClusterSizeY = from._logicalClusterSizeY;
			_sizeX = from._sizeX;
			_positionX = from._positionX;
			_sizeY = from._sizeY;
			_positionY = from._positionY;
		}

		public BarSizePosition3DGroupStyle()
		{
			_isStepEnabled = true;
		}

		#region ICloneable Members

		public BarSizePosition3DGroupStyle Clone()
		{
			var result = new BarSizePosition3DGroupStyle();
			result.CopyFrom(this);
			return result;
		}

		object ICloneable.Clone()
		{
			var result = new BarSizePosition3DGroupStyle();
			result.CopyFrom(this);
			return result;
		}

		#endregion ICloneable Members

		#region IPlotGroupStyle Members

		public void BeginPrepare()
		{
			_isInitialized = false;
			_numberOfItems = 0;
			_wasTouchedInThisPrepareStep = false;
			_logicalClusterSizeX = 0.5; // in case there is only one item, it takes half of the width of the x-scale
			_logicalClusterSizeY = 0.5; // in case there is only one item, it takes half of the width of the x-scale
		}

		public void EndPrepare()
		{
			_wasTouchedInThisPrepareStep = false;

			int tnumberOfItems = 1;
			if (this._isStepEnabled)
				tnumberOfItems = Math.Max(tnumberOfItems, _numberOfItems);

			// partition the total number of items in items in x-direction and in items in y-direction
			int numberOfItemsX, numberOfItemsY;

			PartitionItems(tnumberOfItems, out numberOfItemsX, out numberOfItemsY);

			_sizeX = 1.0 / (numberOfItemsX + (numberOfItemsX - 1) * _relInnerGapX + _relOuterGapX);
			_sizeX *= _logicalClusterSizeX;
			_positionX = 0.5 * (_sizeX * _relOuterGapX - _logicalClusterSizeX);

			_sizeY = 1.0 / (numberOfItemsY + (numberOfItemsY - 1) * _relInnerGapY + _relOuterGapY);
			_sizeY *= _logicalClusterSizeY;
			_positionY = 0.5 * (_sizeY * _relOuterGapY - _logicalClusterSizeY);
		}

		/// <summary>
		/// Partitions the total number of items in rows and columns. The strategy how to partition should be left to the user.
		/// </summary>
		/// <param name="totalNumberOfItems">The total number of items.</param>
		/// <param name="numberOfItemsX">The number of items in x-direction.</param>
		/// <param name="numberOfItemsY">The number of items in y-direction.</param>
		private void PartitionItems(int totalNumberOfItems, out int numberOfItemsX, out int numberOfItemsY)
		{
			if (0 == totalNumberOfItems)
			{
				numberOfItemsX = 0; numberOfItemsY = 0;
			}
			numberOfItemsX = (int)Math.Ceiling(Math.Sqrt(totalNumberOfItems));
			numberOfItemsY = (int)Math.Ceiling(totalNumberOfItems / (double)numberOfItemsX);
		}

		public void PrepareStep()
		{
			if (_wasTouchedInThisPrepareStep)
			{
				if (_isStepEnabled)
					_numberOfItems++;
				else
					_numberOfItems = 1;
			}

			_wasTouchedInThisPrepareStep = false;
		}

		public bool CanCarryOver
		{
			get
			{
				return false;
			}
		}

		public bool CanStep
		{
			get
			{
				return true;
			}
		}

		public int Step(int step)
		{
			_positionX += step * _sizeX * (1 + _relInnerGapX);
			return 0;
		}

		public bool IsStepEnabled
		{
			get
			{
				return _isStepEnabled;
			}
			set
			{
				_isStepEnabled = value;
			}
		}

		#endregion IPlotGroupStyle Members

		public bool IsInitialized
		{
			get
			{
				return _isInitialized;
			}
		}

		/// <summary>
		/// Call this function during a prepare step in case the plot item has a BarGraphPlotStyle.
		/// You can safely call it more than once in each prepare step. Only one item is counted per prepare step.
		/// </summary>
		private void IntendToApply(
			int numberOfClusterItems,
			double minimumLogicalXValue,
			double maximumLogicalXValue,
			double minimumLogicalYValue,
			double maximumLogicalYValue)
		{
			_wasTouchedInThisPrepareStep = true;

			if (numberOfClusterItems > 1)
			{
				double logicalClusterSizeX = (maximumLogicalXValue - minimumLogicalXValue) / (numberOfClusterItems - 1);
				if (logicalClusterSizeX < _logicalClusterSizeX)
					_logicalClusterSizeX = logicalClusterSizeX;

				double logicalClusterSizeY = (maximumLogicalYValue - minimumLogicalYValue) / (numberOfClusterItems - 1);
				if (logicalClusterSizeY < _logicalClusterSizeY)
					_logicalClusterSizeY = logicalClusterSizeY;
			}
		}

		/// <summary>
		/// Is initialized is called the first time a BarGraphPlotStyle.PrepareStyle was called.
		/// The BarGraphPlotStyle has stored two properties relGap and relBound, which are transferred
		/// to the group style in this process.
		/// </summary>
		/// <param name="relInnerGapX">Gap between to bars in a group in units of one bar width.</param>
		/// <param name="relOuterGapX">Gap between the items of two groups in units of one bar width.</param>
		public void Initialize(double relInnerGapX, double relOuterGapX, double relInnerGapY, double relOuterGapY)
		{
			_isInitialized = true;
			_relInnerGapX = relInnerGapX;
			_relOuterGapX = relOuterGapX;
			_relInnerGapY = relInnerGapY;
			_relOuterGapY = relOuterGapY;
		}

		public void Apply(
			out double relInnerGapX, out double relOuterGapX, out double sizeX, out double posX,
			out double relInnerGapY, out double relOuterGapY, out double sizeY, out double posY)
		{
			relInnerGapX = _relInnerGapX;
			relOuterGapX = _relOuterGapX;
			sizeX = _sizeX;
			posX = _positionX;

			relInnerGapY = _relInnerGapY;
			relOuterGapY = _relOuterGapY;
			sizeY = _sizeY;
			posY = _positionY;
		}

		#region Static Helpers

		public static void AddExternalGroupStyle(IPlotGroupStyleCollection externalGroups)
		{
			if (PlotGroupStyle.ShouldAddExternalGroupStyle(externalGroups, typeof(BarSizePosition3DGroupStyle)))
			{
				BarSizePosition3DGroupStyle gstyle = new BarSizePosition3DGroupStyle();
				gstyle.IsStepEnabled = true;
				externalGroups.Add(gstyle);
			}
		}

		/// <summary>
		/// Adds a local BarSizePosition3DGroupStyle in case there is no external one. In this case also BeginPrepare is called on
		/// this newly created group style.
		/// </summary>
		/// <param name="externalGroups">Collection of external plot group styles.</param>
		/// <param name="localGroups">Collection of plot group styles of the plot item.</param>
		public static void AddLocalGroupStyle(
		 IPlotGroupStyleCollection externalGroups,
		 IPlotGroupStyleCollection localGroups)
		{
			if (PlotGroupStyle.ShouldAddLocalGroupStyle(externalGroups, localGroups, typeof(BarSizePosition3DGroupStyle)))
			{
				var styleToAdd = new BarSizePosition3DGroupStyle();
				localGroups.Add(styleToAdd);
			}
		}

		public static void IntendToApply(
			IPlotGroupStyleCollection externalGroups,
			IPlotGroupStyleCollection localGroups,
			int numberOfItems,
			double minimumLogicalXValue,
			double maximumLogicalXValue,
			double minimumLogicalYValue,
			double maximumLogicalYValue
			)
		{
			if (externalGroups != null && externalGroups.ContainsType(typeof(BarSizePosition3DGroupStyle)))
			{
				((BarSizePosition3DGroupStyle)externalGroups.GetPlotGroupStyle(typeof(BarSizePosition3DGroupStyle))).IntendToApply(numberOfItems, minimumLogicalXValue, maximumLogicalXValue, minimumLogicalYValue, maximumLogicalYValue);
			}
			else if (localGroups != null && localGroups.ContainsType(typeof(BarSizePosition3DGroupStyle)))
			{
				((BarSizePosition3DGroupStyle)localGroups.GetPlotGroupStyle(typeof(BarSizePosition3DGroupStyle))).IntendToApply(numberOfItems, minimumLogicalXValue, maximumLogicalXValue, minimumLogicalYValue, maximumLogicalYValue);
			}
		}

		#endregion Static Helpers
	}
}