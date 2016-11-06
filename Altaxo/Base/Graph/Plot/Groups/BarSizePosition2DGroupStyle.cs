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
	public class BarSizePosition2DGroupStyle
		:
		Main.SuspendableDocumentLeafNodeWithEventArgs,
		IPlotGroupStyle,
		IShiftLogicalXYGroupStyle
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
		/// The width of one cluster of bars (including the gaps) in units of logical scale values.
		/// </summary>
		private double _logicalClusterSizeX;

		private double _logicalItemSizeX;
		private double _logicalItemOffsetX;

		#region Serialization

		/// <summary>
		/// 2016-11-05 Renaming from BarWidthPositionGroupStyle to BarSizePosition2DGroupStyle
		/// </summary>
		/// <seealso cref="Altaxo.Serialization.Xml.IXmlSerializationSurrogate" />
		[Altaxo.Serialization.Xml.XmlSerializationSurrogateFor("AltaxoBase", "Altaxo.Graph.Plot.Groups.BarWidthPositionGroupStyle", 0)]
		[Altaxo.Serialization.Xml.XmlSerializationSurrogateFor(typeof(BarSizePosition2DGroupStyle), 1)]
		private class XmlSerializationSurrogate1 : Altaxo.Serialization.Xml.IXmlSerializationSurrogate
		{
			public void Serialize(object obj, Altaxo.Serialization.Xml.IXmlSerializationInfo info)
			{
				var s = (BarSizePosition2DGroupStyle)obj;
				info.AddValue("StepEnabled", s._isStepEnabled);
			}

			public object Deserialize(object o, Altaxo.Serialization.Xml.IXmlDeserializationInfo info, object parent)
			{
				var s = (BarSizePosition2DGroupStyle)o ?? new BarSizePosition2DGroupStyle();
				s._isStepEnabled = info.GetBoolean("StepEnabled");
				return s;
			}
		}

		#endregion Serialization

		private void CopyFrom(BarSizePosition2DGroupStyle from)
		{
			if (object.ReferenceEquals(this, from))
				return;

			_isStepEnabled = from._isStepEnabled;

			_relInnerGapX = from._relInnerGapX;
			_relOuterGapX = from._relOuterGapX;

			_logicalClusterSizeX = from._logicalClusterSizeX;
			_logicalItemSizeX = from._logicalItemSizeX;
			_logicalItemOffsetX = from._logicalItemOffsetX;
		}

		public void TransferFrom(IPlotGroupStyle fromb)
		{
			var from = (BarSizePosition2DGroupStyle)fromb;

			_relInnerGapX = from._relInnerGapX;
			_relOuterGapX = from._relOuterGapX;
			_logicalClusterSizeX = from._logicalClusterSizeX;
			_logicalItemSizeX = from._logicalItemSizeX;
			_logicalItemOffsetX = from._logicalItemOffsetX;
		}

		public BarSizePosition2DGroupStyle()
		{
			_isStepEnabled = true;
		}

		#region ICloneable Members

		public BarSizePosition2DGroupStyle Clone()
		{
			BarSizePosition2DGroupStyle result = new BarSizePosition2DGroupStyle();
			result.CopyFrom(this);
			return result;
		}

		object ICloneable.Clone()
		{
			BarSizePosition2DGroupStyle result = new BarSizePosition2DGroupStyle();
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

		public void EndPrepare()
		{
			_wasTouchedInThisPrepareStep = false;

			int tnumberOfItems = 1;
			if (this._isStepEnabled)
				tnumberOfItems = Math.Max(tnumberOfItems, _numberOfItems);

			_logicalItemSizeX = 1.0 / (tnumberOfItems + (tnumberOfItems - 1) * _relInnerGapX + _relOuterGapX);
			_logicalItemSizeX *= _logicalClusterSizeX;

			_logicalItemOffsetX = 0.5 * (_logicalItemSizeX * _relOuterGapX - _logicalClusterSizeX);
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
			_logicalItemOffsetX += step * _logicalItemSizeX * (1 + _relInnerGapX);
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
			double maximumLogicalXValue)
		{
			_wasTouchedInThisPrepareStep = true;

			if (numberOfClusterItems > 1)
			{
				double logicalClusterSizeX = (maximumLogicalXValue - minimumLogicalXValue) / (numberOfClusterItems - 1);
				if (logicalClusterSizeX < _logicalClusterSizeX)
					_logicalClusterSizeX = logicalClusterSizeX;
			}
		}

		/// <summary>
		/// Is initialized is called the first time a BarGraphPlotStyle.PrepareStyle was called.
		/// The BarGraphPlotStyle has stored two properties relGap and relBound, which are transferred
		/// to the group style in this process.
		/// </summary>
		/// <param name="relInnerGapX">Gap between to bars in a group in units of one bar width.</param>
		/// <param name="relOuterGapX">Gap between the items of two groups in units of one bar width.</param>
		public void Initialize(double relInnerGapX, double relOuterGapX)
		{
			_isInitialized = true;
			_relInnerGapX = relInnerGapX;
			_relOuterGapX = relOuterGapX;
		}

		public void Apply(out double relInnerGapX, out double relOuterGapX, out double sizeX, out double posX)
		{
			relInnerGapX = _relInnerGapX;
			relOuterGapX = _relOuterGapX;
			sizeX = _logicalItemSizeX;
			posX = _logicalItemOffsetX;
		}

		bool IShiftLogicalXYGroupStyle.IsConstant { get { return true; } }

		void IShiftLogicalXYGroupStyle.Apply(out double logicalShiftX, out double logicalShiftY)
		{
			logicalShiftX = _logicalItemOffsetX;
			logicalShiftY = 0;
		}

		void IShiftLogicalXYGroupStyle.Apply(out Func<int, double> logicalShiftX, out Func<int, double> logicalShiftY)
		{
			throw new NotImplementedException("Use this function only if IsConstant returns false");
		}

		#region Static Helpers

		public static void AddExternalGroupStyle(IPlotGroupStyleCollection externalGroups)
		{
			if (PlotGroupStyle.ShouldAddExternalGroupStyle(externalGroups, typeof(BarSizePosition2DGroupStyle)))
			{
				BarSizePosition2DGroupStyle gstyle = new BarSizePosition2DGroupStyle();
				gstyle.IsStepEnabled = true;
				externalGroups.Add(gstyle);
			}
		}

		/// <summary>
		/// Adds a local BarWidthPositionGroupStyle in case there is no external one. In this case also BeginPrepare is called on
		/// this newly created group style.
		/// </summary>
		/// <param name="externalGroups">Collection of external plot group styles.</param>
		/// <param name="localGroups">Collection of plot group styles of the plot item.</param>
		public static void AddLocalGroupStyle(
		 IPlotGroupStyleCollection externalGroups,
		 IPlotGroupStyleCollection localGroups)
		{
			if (PlotGroupStyle.ShouldAddLocalGroupStyle(externalGroups, localGroups, typeof(BarSizePosition2DGroupStyle)))
			{
				var styleToAdd = new BarSizePosition2DGroupStyle();
				localGroups.Add(styleToAdd);
			}
		}

		public static void IntendToApply(
			IPlotGroupStyleCollection externalGroups,
			IPlotGroupStyleCollection localGroups,
			int numberOfItems,
			double minimumLogicalXValue,
			double maximumLogicalXValue
			)
		{
			if (externalGroups != null && externalGroups.ContainsType(typeof(BarSizePosition2DGroupStyle)))
			{
				((BarSizePosition2DGroupStyle)externalGroups.GetPlotGroupStyle(typeof(BarSizePosition2DGroupStyle))).IntendToApply(numberOfItems, minimumLogicalXValue, maximumLogicalXValue);
			}
			else if (localGroups != null && localGroups.ContainsType(typeof(BarSizePosition2DGroupStyle)))
			{
				((BarSizePosition2DGroupStyle)localGroups.GetPlotGroupStyle(typeof(BarSizePosition2DGroupStyle))).IntendToApply(numberOfItems, minimumLogicalXValue, maximumLogicalXValue);
			}
		}

		#endregion Static Helpers
	}
}