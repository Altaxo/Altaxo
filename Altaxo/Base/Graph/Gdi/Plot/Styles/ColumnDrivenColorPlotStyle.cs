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

using Altaxo.Data;
using System;
using System.Collections.Generic;
using System.Drawing;

namespace Altaxo.Graph.Gdi.Plot.Styles
{
	using Altaxo.Main;
	using Graph.Plot.Groups;
	using Graph.Scales;
	using Plot.Data;
	using Plot.Groups;

	/// <summary>
	/// This style provides a variable symbol size dependent on the data of a user choosen column. The data of that column at the index of the data point determine the symbol size.
	/// This plot style is non-visual, i.e. it has no visual equivalent,
	/// since it is only intended to provide the symbol size to other plot styles.
	/// </summary>
	public class ColumnDrivenColorPlotStyle
		:
		Main.SuspendableDocumentNodeWithEventArgs,
		IG2DPlotStyle
	{
		#region Members

		/// <summary>
		/// Data which are converted to scatter size.
		/// </summary>
		private INumericColumnProxy _dataColumnProxy;

		/// <summary>True if the data in the data column changed, but the scale was not updated up to now.</summary>
		[NonSerialized]
		private bool _doesScaleNeedsDataUpdate;

		/// <summary>
		/// Converts the numerical values of the data colum into logical values.
		/// </summary>
		private NumericalScale _scale;

		/// <summary>
		/// Converts the logical value (from the scale) to a color value.
		/// </summary>
		private IColorProvider _colorProvider;

		/// <summary>If true, the color is applied as a fill color for symbols, bar graphs etc.</summary>
		private bool _appliesToFill = true;

		/// <summary>If true, the color is applied as a stroke color for framing symbols, bar graphs etc.</summary>
		private bool _appliesToStroke;

		/// <summary>If true, the color is used to color the background, for instance of labels.</summary>
		private bool _appliesToBackground;

		#endregion Members

		/// <summary>
		/// Creates a new instance with default values.
		/// </summary>
		public ColumnDrivenColorPlotStyle()
		{
			InternalSetScale(new LinearScale());
			InternalSetDataColumnProxy(NumericColumnProxyBase.FromColumn(new Altaxo.Data.EquallySpacedColumn(0, 0.25)));
			_colorProvider = new ColorProvider.VisibleLightSpectrum() { ParentObject = this };
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
			if (object.ReferenceEquals(this, obj))
				return true;

			var from = obj as ColumnDrivenColorPlotStyle;
			if (null != from)
			{
				using (var suspendToken = SuspendGetToken())
				{
					_appliesToFill = from._appliesToFill;
					_appliesToStroke = from._appliesToStroke;
					_appliesToBackground = from._appliesToBackground;

					InternalSetScale(null == from._scale ? null : (NumericalScale)from._scale.Clone());
					InternalSetDataColumnProxy(null == from._dataColumnProxy ? null : (INumericColumnProxy)from._dataColumnProxy.Clone());

					_colorProvider = null == from._colorProvider ? null : (IColorProvider)from._colorProvider.Clone();
					_colorProvider.ParentObject = this;

					//_parent = from._parent;

					suspendToken.ResumeSilently();
				}
				EhSelfChanged(EventArgs.Empty);
				return true;
			}
			return false;
		}

		protected override IEnumerable<DocumentNodeAndName> GetDocumentNodeChildrenWithName()
		{
			if (null != _dataColumnProxy)
				yield return new DocumentNodeAndName(_dataColumnProxy, "Data");
			if (null != _scale)
				yield return new DocumentNodeAndName(_scale, "Scale");
			if (null != _colorProvider)
				yield return new DocumentNodeAndName(_colorProvider, "ColorProvider");
		}

		#region DataColumnProxy handling

		/// <summary>
		/// Sets the data column proxy and creates the necessary event links.
		/// </summary>
		/// <param name="proxy"></param>
		protected void InternalSetDataColumnProxy(INumericColumnProxy proxy)
		{
			ChildSetMember(ref _dataColumnProxy, proxy);
		}

		#region Changed event handling

		protected override bool HandleHighPriorityChildChangeCases(object sender, ref EventArgs e)
		{
			if (object.ReferenceEquals(_dataColumnProxy, sender))
			{
				if (e is Main.InstanceChangedEventArgs) // Data column object has changed
				{
					_doesScaleNeedsDataUpdate = true; // Instance that the proxy holds has changed
				}
				else
				{
					_doesScaleNeedsDataUpdate = true; // data in data column have changed
				}
			}
			return base.HandleHighPriorityChildChangeCases(sender, ref e);
		}

		#endregion Changed event handling

		/// <summary>
		/// Updates the scale if the data of the data column have changed.
		/// </summary>
		private void InternalUpdateScaleWithNewData()
		{
			// in order to set the bounds of the scale, the data column must
			// - be set (not null)
			// - have a defined count.

			var dataColumn = _dataColumnProxy.Document;

			if (dataColumn is IDefinedCount)
			{
				int len = ((IDefinedCount)dataColumn).Count;

				var bounds = _scale.DataBounds;

				using (var suspendToken = bounds.SuspendGetToken())
				{
					for (int i = 0; i < len; i++)
						bounds.Add(dataColumn, i);

					suspendToken.Resume();
				}
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
				return null == _dataColumnProxy ? null : _dataColumnProxy.Document;
			}
			set
			{
				if (ChildSetMember(ref _dataColumnProxy, NumericColumnProxyBase.FromColumn(value)))
					EhSelfChanged(EventArgs.Empty);
			}
		}

		#endregion DataColumnProxy handling

		#region Scale handling

		/// <summary>
		/// Sets the scale and create the necessary event links.
		/// </summary>
		/// <param name="scale"></param>
		protected void InternalSetScale(NumericalScale scale)
		{
			if (ChildSetMember(ref _scale, scale))
			{
				_doesScaleNeedsDataUpdate = true;
			}
		}

		#endregion Scale handling

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
				EhSelfChanged(EventArgs.Empty);
			}
		}

		public IColorProvider ColorProvider
		{
			get { return _colorProvider; }
			set
			{
				if (ChildSetMember(ref _colorProvider, value))
					EhSelfChanged(EventArgs.Empty);
			}
		}

		#endregion Properties

		/// <summary>
		/// Gets the color for the index idx.
		/// </summary>
		/// <param name="idx">Index into the row of the data column.</param>
		/// <returns>The calculated color for the provided index.</returns>
		private Color GetColor(int idx)
		{
			var dataColumn = DataColumn;
			double val = double.NaN;
			if (null != dataColumn)
			{
				if (_doesScaleNeedsDataUpdate)
					InternalUpdateScaleWithNewData();

				val = dataColumn[idx];
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

		public object Clone()
		{
			return new ColumnDrivenColorPlotStyle(this);
		}

		/// <summary>
		/// Replaces path of items (intended for data items like tables and columns) by other paths. Thus it is possible
		/// to change a plot so that the plot items refer to another table.
		/// </summary>
		/// <param name="Report">Function that reports the found <see cref="DocNodeProxy"/> instances to the visitor.</param>
		public void VisitDocumentReferences(DocNodeProxyReporter Report)
		{
			Report(_dataColumnProxy, this, "DataColumn");
		}
	}
}