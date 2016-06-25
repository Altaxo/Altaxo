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
	public class ColumnDrivenSymbolSizePlotStyle
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
		/// Scatter size at logical value of 0;
		/// </summary>
		private double _symbolSizeAt0;

		/// <summary>
		/// Scatter size at logical value of 1;
		/// </summary>
		private double _symbolSizeAt1;

		/// <summary>
		/// Scatter size if thelogical value is above 1.
		/// </summary>
		private double _symbolSizeAbove;

		/// <summary>
		/// Scatter size of the logical value is below 0.
		/// </summary>
		private double _symbolSizeBelow;

		/// <summary>
		/// Scatter size if no value can be calculated.
		/// </summary>
		private double _symbolSizeInvalid;

		/// <summary>
		/// Number of steps of the scatter size between min and max. If this value is 0, then the scatter size is provided continuously.
		/// </summary>
		private int _numberOfSteps;

		#endregion Members

		#region Serialization

		/// <summary>
		/// 2015-09-29 initial version.
		/// </summary>
		[Altaxo.Serialization.Xml.XmlSerializationSurrogateFor(typeof(ColumnDrivenSymbolSizePlotStyle), 0)]
		private class XmlSerializationSurrogate0 : Altaxo.Serialization.Xml.IXmlSerializationSurrogate
		{
			public virtual void Serialize(object obj, Altaxo.Serialization.Xml.IXmlSerializationInfo info)
			{
				var s = (ColumnDrivenSymbolSizePlotStyle)obj;

				info.AddValue("DataColumn", s._dataColumnProxy);
				info.AddValue("Scale", s._scale);

				info.AddValue("SymbolSizeAt0", s._symbolSizeAt0);
				info.AddValue("SymbolSizeAt1", s._symbolSizeAt1);
				info.AddValue("SymbolSizeBelow", s._symbolSizeBelow);
				info.AddValue("SymbolSizeAbove", s._symbolSizeAbove);
				info.AddValue("SymbolSizeInvalid", s._symbolSizeInvalid);
				info.AddValue("NumberOfSteps", s._numberOfSteps);
			}

			protected virtual ColumnDrivenSymbolSizePlotStyle SDeserialize(object o, Altaxo.Serialization.Xml.IXmlDeserializationInfo info, object parent)
			{
				var s = (ColumnDrivenSymbolSizePlotStyle)o ?? new ColumnDrivenSymbolSizePlotStyle((Altaxo.Main.Properties.IReadOnlyPropertyBag)null);

				s._dataColumnProxy = (Altaxo.Data.INumericColumnProxy)info.GetValue("DataColumn", s);
				if (null != s._dataColumnProxy) s._dataColumnProxy.ParentObject = s;

				s._scale = (NumericalScale)info.GetValue("Scale", s);
				if (null != s._scale) s._scale.ParentObject = s;

				s._symbolSizeAt0 = info.GetDouble("SymbolSizeAt0");
				s._symbolSizeAt1 = info.GetDouble("SymbolSizeAt1");
				s._symbolSizeBelow = info.GetDouble("SymbolSizeBelow");
				s._symbolSizeAbove = info.GetDouble("SymbolSizeAbove");
				s._symbolSizeInvalid = info.GetDouble("SymbolSizeInvalid");
				s._numberOfSteps = info.GetInt32("NumberOfSteps");

				return s;
			}

			public object Deserialize(object o, Altaxo.Serialization.Xml.IXmlDeserializationInfo info, object parent)
			{
				return SDeserialize(o, info, parent);
			}
		}

		#endregion Serialization

		/// <summary>
		/// Creates a new instance with default values.
		/// </summary>
		public ColumnDrivenSymbolSizePlotStyle(Altaxo.Main.Properties.IReadOnlyPropertyBag context)
		{
			InternalSetScale(new LinearScale());
			InternalSetDataColumnProxy(NumericColumnProxyBase.FromColumn(new Altaxo.Data.EquallySpacedColumn(0, 0.25)));

			var symbolSizeBase = GraphDocument.GetDefaultSymbolSize(context);

			_symbolSizeAt0 = symbolSizeBase / 2;
			_symbolSizeAt1 = symbolSizeBase * 2;
			_symbolSizeAbove = symbolSizeBase * 2.5;
			_symbolSizeBelow = symbolSizeBase / 4;
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
				InternalSetDataColumnProxy(null == from._dataColumnProxy ? null : (INumericColumnProxy)from._dataColumnProxy.Clone());

				_symbolSizeAt0 = from._symbolSizeAt0;
				_symbolSizeAt1 = from._symbolSizeAt1;
				_symbolSizeBelow = from._symbolSizeBelow;
				_symbolSizeAbove = from._symbolSizeAbove;
				_symbolSizeInvalid = from._symbolSizeInvalid;

				_numberOfSteps = from._numberOfSteps;
				//_parent = from._parent;

				copied = true;
			}
			return copied;
		}

		protected override IEnumerable<DocumentNodeAndName> GetDocumentNodeChildrenWithName()
		{
			if (null != _dataColumnProxy)
				yield return new DocumentNodeAndName(_dataColumnProxy, "Data");
			if (null != _scale)
				yield return new DocumentNodeAndName(_scale, "Scale");
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

		#region DataColumnProxy handling

		/// <summary>
		/// Sets the data column proxy and creates the necessary event links.
		/// </summary>
		/// <param name="proxy"></param>
		protected void InternalSetDataColumnProxy(INumericColumnProxy proxy)
		{
			if (ChildSetMember(ref _dataColumnProxy, proxy))
				EhChildChanged(_dataColumnProxy, EventArgs.Empty);
		}

		/// <summary>
		/// Updates the scale if the data of the data column have changed.
		/// </summary>
		private void InternalUpdateScaleWithNewData()
		{
			// in order to set the bounds of the scale, the data column must
			// - be set (not null)
			// - have a defined count.

			var dataColumn = DataColumn;

			if (dataColumn.Count.HasValue)
			{
				int len = dataColumn.Count.Value;

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
				return _dataColumnProxy == null ? null : _dataColumnProxy.Document;
			}
			set
			{
				if (object.ReferenceEquals(DataColumn, value))
					return;

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
					EhSelfChanged(EventArgs.Empty);
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
					EhSelfChanged(EventArgs.Empty);
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
					EhSelfChanged(EventArgs.Empty);
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
					EhSelfChanged(EventArgs.Empty);
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
					EhSelfChanged(EventArgs.Empty);
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
					EhSelfChanged(EventArgs.Empty);
			}
		}

		#endregion Properties

		/// <summary>
		/// Gets the symbol size for the index idx.
		/// </summary>
		/// <param name="idx"></param>
		/// <returns></returns>
		private double GetSymbolSize(int idx)
		{
			double val = double.NaN;
			var dataColumn = DataColumn;
			if (null != dataColumn)
			{
				if (_doesScaleNeedsDataUpdate)
					InternalUpdateScaleWithNewData();

				val = dataColumn[idx];
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

		public object Clone()
		{
			return new ColumnDrivenSymbolSizePlotStyle(this);
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