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
	public class ColumnDrivenColorPlotStyle
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
		/// Converts the logical value (from the scale) to a color value.
		/// </summary>
		IColorProvider _colorProvider;

		/// <summary>If true, the color is applied as a fill color for symbols, bar graphs etc.</summary>
		bool _appliesToFill=true;
		/// <summary>If true, the color is applied as a stroke color for framing symbols, bar graphs etc.</summary>
		bool _appliesToStroke;
		/// <summary>If true, the color is used to color the background, for instance of labels.</summary>
		bool _appliesToBackground;

		object _parent;

		public event EventHandler Changed;


		#endregion


		/// <summary>
		/// Creates a new instance with default values.
		/// </summary>
		public ColumnDrivenColorPlotStyle()
		{
			InternalSetScale(new LinearScale());
			InternalSetDataColumnProxy(new NumericColumnProxy(new Altaxo.Data.EquallySpacedColumn(0, 0.25)));
			_colorProvider = new ColorProvider.VisibleLightSpectrum();
		}

		/// <summary>
		/// Copy constructor.
		/// </summary>
		/// <param name="from">Other instance to copy the data from.</param>
		public ColumnDrivenColorPlotStyle(ColumnDrivenColorPlotStyle from)
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
			bool copied = false;
			var from = obj as ColumnDrivenColorPlotStyle;
			if (null != from)
			{
				_appliesToFill = from._appliesToFill;
				_appliesToStroke = from._appliesToStroke;
				_appliesToBackground = from._appliesToBackground;

				InternalSetScale(null == from._scale ? null : (NumericalScale)from._scale.Clone());
				InternalSetDataColumnProxy(null == from._dataColumn ? null : (NumericColumnProxy)from._dataColumn.Clone());

				_colorProvider = null == from._colorProvider ? null : (IColorProvider)from._colorProvider.Clone();
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

		public IColorProvider ColorProvider
		{
			get { return _colorProvider; }
			set
			{
				if (null != _colorProvider)
				{
					_colorProvider.Changed -= EhChildChanged;
				}

				bool changed = !object.ReferenceEquals(_colorProvider, value);
				_colorProvider = value;

				if (null != _colorProvider)
				{
					_colorProvider.Changed += EhChildChanged;
				}

				if (changed)
					OnChanged();
			}
		}





		#endregion

		/// <summary>
		/// Gets the color for the index idx.
		/// </summary>
		/// <param name="idx">Index into the row of the data column.</param>
		/// <returns>The calculated color for the provided index.</returns>
		Color GetColor(int idx)
		{
			double val = double.NaN;
			if (null != _cachedDataColumn)
			{
				if (_doesScaleNeedsDataUpdate)
					InternalUpdateScaleWithNewData();

				val = _cachedDataColumn[idx];
			}

			val = _scale.PhysicalToNormal(val);
			return _colorProvider.GetColor(val);
		}



		public void CollectExternalGroupStyles(PlotGroupStyleCollection externalGroups)
		{
			// this is only for internal use inside one plot item
		}

		public void CollectLocalGroupStyles(PlotGroupStyleCollection externalGroups, PlotGroupStyleCollection localGroups)
		{
			VariableColorGroupStyle.AddLocalGroupStyle(externalGroups, localGroups);
		}

		public void PrepareGroupStyles(PlotGroupStyleCollection externalGroups, PlotGroupStyleCollection localGroups, IPlotArea layer, Processed2DPlotData pdata)
		{
			VariableColorGroupStyle.PrepareStyle(externalGroups, localGroups, GetColor);
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
			return new ColumnDrivenColorPlotStyle(this);
		}

		public string Name
		{
			get { return "ColumnDrivenColor"; }
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
	}
}
