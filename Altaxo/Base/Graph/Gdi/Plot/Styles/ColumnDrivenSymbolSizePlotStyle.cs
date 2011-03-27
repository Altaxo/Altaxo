#region Copyright
/////////////////////////////////////////////////////////////////////////////
//    Altaxo:  a data processing and data plotting program
//    Copyright (C) 2002-2007 Dr. Dirk Lellinger
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
#endregion

using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;

using Altaxo.Graph.Gdi.Plot.Styles.XYPlotScatterStyles;
using Altaxo.Serialization;
using Altaxo.Data;

namespace Altaxo.Graph.Gdi.Plot.Styles
{
	using Altaxo.Main;
	using Graph.Plot.Groups;
	using Graph.Plot.Data;
	using Graph.Scales;

	using Plot.Groups;
	using Plot.Data;


	/// <summary>
	/// This style provides a variable symbol size dependent on the data of a user choosen column. The data of that column at the index of the data point determine the symbol size. 
	/// This plot style is non-visual, i.e. it has no visual equivalent,
	/// since it is only intended to provide the symbol size to other plot styles.
	/// </summary>
	public class ColumnDrivenSymbolSizePlotStyle
		:
		IG2DPlotStyle,
		ICloneable,
		Main.IChangedEventSource,
		Main.IChildChangedEventSink
	{

		#region Members

		/// <summary>
		/// Data which are converted to scatter size.
		/// </summary>
		private NumericColumnProxy _dataColumn;

		[NonSerialized]
		private INumericColumn _cachedDataColumn;

		/// <summary>True if the data in the data column changed, but the scale was not updated up to now.</summary>
		[NonSerialized]
		private bool _doesScaleNeedsDataUpdate;

		/// <summary>
		/// Converts the numerical values of the data colum into logical values.
		/// </summary>
		NumericalScale _scale;

		/// <summary>
		/// Scatter size at logical value of 0;
		/// </summary>
		double _symbolSizeAt0;
		/// <summary>
		/// Scatter size at logical value of 1;
		/// </summary>
		double _symbolSizeAt1;
		/// <summary>
		/// Scatter size if thelogical value is above 1.
		/// </summary>
		double _symbolSizeAbove;
		/// <summary>
		/// Scatter size of the logical value is below 0.
		/// </summary>
		double _symbolSizeBelow;

		/// <summary>
		/// Scatter size if no value can be calculated.
		/// </summary>
		double _symbolSizeInvalid;

		/// <summary>
		/// Number of steps of the scatter size between min and max. If this value is 0, then the scatter size is provided continuously.
		/// </summary>
		int _numberOfSteps;

		object _parent;

		public event EventHandler Changed;


		#endregion


		/// <summary>
		/// Creates a new instance with default values.
		/// </summary>
		public ColumnDrivenSymbolSizePlotStyle()
		{
			InternalSetScale(new LinearScale());
			InternalSetDataColumnProxy(new NumericColumnProxy(new Altaxo.Data.EquallySpacedColumn(0, 0.25)));
			_symbolSizeAt0 = 4;
			_symbolSizeAt1 = 16;
			_symbolSizeAbove = 20;
			_symbolSizeBelow = 2;
			_symbolSizeInvalid = 0;
			_numberOfSteps = 0;
		}

		/// <summary>
		/// Copy constructor.
		/// </summary>
		/// <param name="from">Other instance to copy the data from.</param>
		public ColumnDrivenSymbolSizePlotStyle(ColumnDrivenSymbolSizePlotStyle from)
		{
			CopyFrom(from);
		}

		/// <summary>
		/// Copies the member variables from another instance.
		/// </summary>
		/// <param name="obj">Another instance to copy the data from.</param>
		/// <returns>True if data was copied, otherwise false.</returns>
		public bool CopyFrom(object obj)
		{
			if (object.ReferenceEquals(this, obj))
				return true;

			bool copied = false;
			var from = obj as ColumnDrivenSymbolSizePlotStyle;
			if (null != from)
			{
				InternalSetScale(null == from._scale ? null : (NumericalScale)from._scale.Clone());
				InternalSetDataColumnProxy(null == from._dataColumn ? null : (NumericColumnProxy)from._dataColumn.Clone());

				_symbolSizeAt0 = from._symbolSizeAt0;
				_symbolSizeAt1 = from._symbolSizeAt1;
				_symbolSizeBelow = from._symbolSizeBelow;
				_symbolSizeAbove = from._symbolSizeAbove;
				_symbolSizeInvalid = from._symbolSizeInvalid;

				_numberOfSteps = from._numberOfSteps;
				_parent = from._parent;

				copied = true;
			}
			return copied;
		}

		#region DataColumnProxy handling


		/// <summary>
		/// Sets the data column proxy and creates the necessary event links.
		/// </summary>
		/// <param name="proxy"></param>
		protected void InternalSetDataColumnProxy(NumericColumnProxy proxy)
		{
			if (null != _dataColumn)
				this._dataColumn.Changed -= EhDataColumnProxyChanged;

			_dataColumn = proxy;

			if (null != _dataColumn)
				this._dataColumn.Changed += EhDataColumnProxyChanged;

			_cachedDataColumn = null == _dataColumn ? null : _dataColumn.Document;

			InternalSetCachedDataColumn(null == _dataColumn ? null : _dataColumn.Document);
		}

		protected void InternalSetCachedDataColumn(INumericColumn col)
		{
			if (!object.ReferenceEquals(col, _cachedDataColumn))
			{
				if (_cachedDataColumn is Altaxo.Main.IChangedEventSource)
				{
					((Altaxo.Main.IChangedEventSource)_cachedDataColumn).Changed -= EhDataColumnDataChanged;
				}

				_cachedDataColumn = col;

				if (_cachedDataColumn is Altaxo.Main.IChangedEventSource)
				{
					((Altaxo.Main.IChangedEventSource)_cachedDataColumn).Changed += EhDataColumnDataChanged;
				}

				// fake a change of the data of the column in order to calculate the boundaries
				EhDataColumnDataChanged(_cachedDataColumn, EventArgs.Empty);

				OnChanged();
			}
		}

		/// <summary>
		/// Function that is called if the data column proxy changed.
		/// </summary>
		/// <param name="sender">Originator.</param>
		/// <param name="e">Event args.</param>
		void EhDataColumnProxyChanged(object sender, EventArgs e)
		{
			InternalSetCachedDataColumn(null == _dataColumn ? null : _dataColumn.Document);
		}

		/// <summary>
		/// Called when the data of the data column changed.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		void EhDataColumnDataChanged(object sender, EventArgs e)
		{
			_doesScaleNeedsDataUpdate = true;
		}

		/// <summary>
		/// Updates the scale if the data of the data column have changed.
		/// </summary>
		void InternalUpdateScaleWithNewData()
		{
			// in order to set the bounds of the scale, the data column must 
			// - be set (not null)
			// - have a defined count.

			if (_cachedDataColumn is IDefinedCount)
			{
				int len = ((IDefinedCount)_cachedDataColumn).Count;

				var bounds = _scale.DataBounds;

				bounds.BeginUpdate();

				for (int i = 0; i < len; i++)
					bounds.Add(_cachedDataColumn, i);

				bounds.EndUpdate();
				_doesScaleNeedsDataUpdate = false;
			}
		}

		/// <summary>
		/// Gets/sets the data column that provides the data that is used to calculate the symbol size.
		/// </summary>
		public Altaxo.Data.INumericColumn DataColumn
		{
			get
			{
				if (null != _dataColumn && null == _cachedDataColumn)
					InternalSetCachedDataColumn(_dataColumn.Document);
				return _cachedDataColumn;
			}
			set
			{
				_dataColumn.SetDocNode(value);
				EhDataColumnProxyChanged(this, EventArgs.Empty);
			}
		}

		#endregion

		#region Scale handling

		/// <summary>
		/// Sets the scale and create the necessary event links.
		/// </summary>
		/// <param name="scale"></param>
		protected void InternalSetScale(NumericalScale scale)
		{
			if (null != _scale)
				_scale.Changed -= EhChildChanged;

			_scale = scale;

			if (null != _scale)
				_scale.Changed += EhChildChanged;

			_doesScaleNeedsDataUpdate = true;
		}

		#endregion

		#region Properties

		/// <summary>
		/// Gets/sets the scale.
		/// </summary>
		public NumericalScale Scale
		{
			get
			{
				return _scale;
			}
			set
			{
				if (null == value)
					throw new ArgumentNullException("Scale");

				InternalSetScale(value);
				OnChanged();
			}
		}

		/// <summary>
		/// Symbol size at the logical value of 0 of the scale, i.e. at the scale's origin.
		/// </summary>
		public double SymbolSizeAt0
		{
			get { return _symbolSizeAt0; }
			set
			{
				bool changed = _symbolSizeAt0 != value;
				_symbolSizeAt0 = value;
				if (changed)
					OnChanged();
			}
		}

		/// <summary>
		/// Symbol size at the logical value of 1 of the scale, i.e. at the scale's end.
		/// </summary>
		public double SymbolSizeAt1
		{
			get { return _symbolSizeAt1; }
			set
			{
				bool changed = _symbolSizeAt1 != value;
				_symbolSizeAt1 = value;
				if (changed)
					OnChanged();
			}
		}

		/// <summary>
		/// Symbol size at logical values of smaller than 0 of the scale, i.e. below the scale's origin.
		/// </summary>
		public double SymbolSizeBelow
		{
			get { return _symbolSizeBelow; }
			set
			{
				bool changed = _symbolSizeBelow != value;
				_symbolSizeBelow = value;
				if (changed)
					OnChanged();
			}
		}

		/// <summary>
		/// Symbol size at logical values of greater than 1 of the scale, i.e. above the scale's end.
		/// </summary>
		public double SymbolSizeAbove
		{
			get { return _symbolSizeAbove; }
			set
			{
				bool changed = _symbolSizeAbove != value;
				_symbolSizeAbove = value;
				if (changed)
					OnChanged();
			}
		}

		/// <summary>
		/// Symbol size that is used if the calculated value is double.NaN or if the calculation has thrown an exception.
		/// </summary>
		public double SymbolSizeInvalid
		{
			get { return _symbolSizeInvalid; }
			set
			{
				bool changed = _symbolSizeInvalid != value;
				_symbolSizeInvalid = value;
				if (changed)
					OnChanged();
			}
		}

		/// <summary>
		/// Number of steps of the symbol size. If the value is zero, the symbol size is a continuous value.
		/// </summary>
		public int NumberOfSteps
		{
			get { return _numberOfSteps; }
			set
			{
				bool changed = _numberOfSteps != value;
				_numberOfSteps = value;
				if (changed)
					OnChanged();
			}
		}

		#endregion

		/// <summary>
		/// Gets the symbol size for the index idx.
		/// </summary>
		/// <param name="idx"></param>
		/// <returns></returns>
		double GetSymbolSize(int idx)
		{
			double val = double.NaN;
			if (null != _cachedDataColumn)
			{
				if (_doesScaleNeedsDataUpdate)
					InternalUpdateScaleWithNewData();

				val = _cachedDataColumn[idx];
			}
			val = _scale.PhysicalToNormal(val);



			if (val >= 0 && val <= 1)
			{
				if (_numberOfSteps > 0)
					val = val < 1 ? (Math.Floor(val * _numberOfSteps) + 0.5) / _numberOfSteps : (_numberOfSteps - 0.5) / _numberOfSteps;

				return _symbolSizeAt0 + val * (_symbolSizeAt1 - _symbolSizeAt0);
			}

			if (val < 0)
				return _symbolSizeBelow;
			if (val > 1)
				return _symbolSizeAbove;

			return _symbolSizeInvalid;
		}

		public void CollectExternalGroupStyles(PlotGroupStyleCollection externalGroups)
		{
			// this is only for internal use inside one plot item
		}

		public void CollectLocalGroupStyles(PlotGroupStyleCollection externalGroups, PlotGroupStyleCollection localGroups)
		{
			VariableSymbolSizeGroupStyle.AddLocalGroupStyle(externalGroups, localGroups);
		}

		public void PrepareGroupStyles(PlotGroupStyleCollection externalGroups, PlotGroupStyleCollection localGroups, IPlotArea layer, Processed2DPlotData pdata)
		{
			VariableSymbolSizeGroupStyle.PrepareStyle(externalGroups, localGroups, GetSymbolSize);
		}

		public void ApplyGroupStyles(PlotGroupStyleCollection externalGroups, PlotGroupStyleCollection localGroups)
		{
			// there is nothing to apply here, because it is only a provider
		}

		public void Paint(Graphics g, IPlotArea layer, Processed2DPlotData pdata, Processed2DPlotData prevItemData, Processed2DPlotData nextItemData)
		{
			// this is not a visible style, thus doing nothing
		}

		public RectangleF PaintSymbol(Graphics g, RectangleF bounds)
		{
			// this is not a visible style, thus doing nothing
			return RectangleF.Empty;
		}

		public object ParentObject
		{
			get { return _parent; }
			set { _parent = value; }
		}

		public object Clone()
		{
			return new ColumnDrivenSymbolSizePlotStyle(this);
		}

		public string Name
		{
			get { return "ColumnDrivenSymbolSize"; }
		}


		protected virtual void OnChanged()
		{
			if (_parent is Main.IChildChangedEventSink)
				((Main.IChildChangedEventSink)_parent).EhChildChanged(this, EventArgs.Empty);

			if (null != Changed)
				Changed(this, new EventArgs());
		}

		public void EhChildChanged(object child, EventArgs e)
		{
			OnChanged();
		}

		/// <summary>
		/// Replaces path of items (intended for data items like tables and columns) by other paths. Thus it is possible
		/// to change a plot so that the plot items refer to another table.
		/// </summary>
		/// <param name="options">Information what to replace.</param>
		public void EnumerateDocumentReferences(IDocNodeProxyVisitor options)
		{
			options.Visit(_dataColumn);
		}
	}
}
