#region Copyright

/////////////////////////////////////////////////////////////////////////////
//    Altaxo:  a data processing and data plotting program
//    Copyright (C) 2002-2015 Dr. Dirk Lellinger
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

using Altaxo.Graph.Scales.Boundaries;
using Altaxo.Main;
using Altaxo.Serialization;
using System;

using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Altaxo.Graph.Graph3D.Plot
{
	using Graph;
	using Graph.Plot.Data;
	using Graph.Plot.Groups;
	using GraphicsContext;
	using Groups;
	using Styles;

	/// <summary>
	/// Association of data and style specialized for x-y-plots of column data.
	/// </summary>
	public class DataMeshPlotItem
		:
		PlotItem,
		IXBoundsHolder,
		IYBoundsHolder,
		IZBoundsHolder
	{
		protected XYZMeshedColumnPlotData _plotData;
		protected DataMeshPlotStyle _plotStyle;

		#region Serialization

		/// <summary>
		/// 2015-11-14 initial version.
		/// </summary>
		[Altaxo.Serialization.Xml.XmlSerializationSurrogateFor(typeof(DataMeshPlotItem), 0)]
		private class XmlSerializationSurrogate0 : Altaxo.Serialization.Xml.IXmlSerializationSurrogate
		{
			public void Serialize(object obj, Altaxo.Serialization.Xml.IXmlSerializationInfo info)
			{
				var s = (DataMeshPlotItem)obj;
				info.AddValue("Data", s._plotData);
				info.AddValue("Style", s._plotStyle);
			}

			public object Deserialize(object o, Altaxo.Serialization.Xml.IXmlDeserializationInfo info, object parent)
			{
				var pa = (XYZMeshedColumnPlotData)info.GetValue("Data", null);
				var ps = (DataMeshPlotStyle)info.GetValue("Style", null);

				if (o == null)
				{
					return new DataMeshPlotItem(pa, ps);
				}
				else
				{
					DataMeshPlotItem s = (DataMeshPlotItem)o;
					s.Data = pa;
					s.Style = ps;
					return s;
				}
			}
		}

		#endregion Serialization

		protected override System.Collections.Generic.IEnumerable<Main.DocumentNodeAndName> GetDocumentNodeChildrenWithName()
		{
			if (null != _plotData)
				yield return new Main.DocumentNodeAndName(_plotData, () => _plotData = null, "Data");
			if (null != _plotStyle)
				yield return new Main.DocumentNodeAndName(_plotStyle, () => _plotStyle = null, "Style");
		}

		public DataMeshPlotItem(XYZMeshedColumnPlotData pa, DataMeshPlotStyle ps)
		{
			this.Data = pa;
			this.Style = ps;
		}

		public DataMeshPlotItem(DataMeshPlotItem from)
		{
			CopyFrom(from);
		}

		public override bool CopyFrom(object obj)
		{
			if (object.ReferenceEquals(this, obj))
				return true;

			var copied = base.CopyFrom(obj);
			if (copied)
			{
				var from = obj as DataMeshPlotItem;
				if (null != from)
				{
					this.Data = from._plotData.Clone();   // also wires the event
					this.Style = (DataMeshPlotStyle)from.Style.Clone(); // also wires the event
				}
			}
			return copied;
		}

		public override object Clone()
		{
			return new DataMeshPlotItem(this);
		}

		public object Data
		{
			get { return _plotData; }
			set
			{
				if (null == value)
					throw new System.ArgumentNullException();
				else if (!(value is XYZMeshedColumnPlotData))
					throw new System.ArgumentException("The provided data object is not of the type " + _plotData.GetType().ToString() + ", but of type " + value.GetType().ToString() + "!");
				else
				{
					if (ChildSetMember(ref _plotData, (XYZMeshedColumnPlotData)value))
					{
						EhSelfChanged(PlotItemDataChangedEventArgs.Empty);
					}
				}
			}
		}

		public override Main.IDocumentLeafNode StyleObject
		{
			get { return _plotStyle; }
			set { this.Style = (DataMeshPlotStyle)value; }
		}

		public override Main.IDocumentLeafNode DataObject
		{
			get { return _plotData; }
		}

		public DataMeshPlotStyle Style
		{
			get { return _plotStyle; }
			set
			{
				if (null == value)
					throw new System.ArgumentNullException();
				if (ChildSetMember(ref _plotStyle, (DataMeshPlotStyle)value))
				{
					EhSelfChanged(PlotItemStyleChangedEventArgs.Empty);
				}
			}
		}

		public override string GetName(int level)
		{
			return _plotData.ToString();
		}

		public override string GetName(string style)
		{
			return GetName(0);
		}

		public override string ToString()
		{
			return GetName(int.MaxValue);
		}

		public override void Paint(IGraphicContext3D g, Altaxo.Graph.IPaintContext context, IPlotArea layer, IGPlotItem previousPlotItem, IGPlotItem nextPlotItem)
		{
			if (null != this._plotStyle)
			{
				_plotStyle.Paint(g, layer, _plotData);
			}
		}

		/// <summary>
		/// This routine ensures that the plot item updates all its cached data and send the appropriate
		/// events if something has changed. Called before the layer paint routine paints the axes because
		/// it must be ensured that the axes are scaled correctly before the plots are painted.
		/// </summary>
		/// <param name="layer">The plot layer.</param>
		public override void PrepareScales(IPlotArea layer)
		{
			if (null != this._plotData)
			{
				_plotData.CalculateCachedData(layer.XAxis.DataBoundsObject, layer.YAxis.DataBoundsObject, layer.ZAxis.DataBoundsObject);

				// in case our plot style uses its own scale for coloring the mesh, we do prepare the scale used by the style
				_plotStyle?.PrepareScales(layer, _plotData);
			}
		}

		protected override void OnChanged(EventArgs e)
		{
			if (e is PlotItemDataChangedEventArgs)
			{
				// first inform our AbstractXYPlotStyle of the change, so it can invalidate its cached data
				if (null != this._plotStyle)
					_plotStyle.EhDataChanged(this);
			}

			base.OnChanged(e);
		}

		#region IXBoundsHolder Members

		public void SetXBoundsFromTemplate(IPhysicalBoundaries val)
		{
			this._plotData.SetXBoundsFromTemplate(val);
		}

		public void MergeXBoundsInto(IPhysicalBoundaries pb)
		{
			this._plotData.MergeXBoundsInto(pb);
		}

		#endregion IXBoundsHolder Members

		#region IYBoundsHolder Members

		public void SetYBoundsFromTemplate(IPhysicalBoundaries val)
		{
			this._plotData.SetYBoundsFromTemplate(val);
		}

		public void MergeYBoundsInto(IPhysicalBoundaries pb)
		{
			this._plotData.MergeYBoundsInto(pb);
		}

		#endregion IYBoundsHolder Members

		#region IZBoundHolder

		public void SetZBoundsFromTemplate(IPhysicalBoundaries val)
		{
			this._plotData.SetVBoundsFromTemplate(val);
		}

		public void MergeZBoundsInto(IPhysicalBoundaries pb)
		{
			this._plotData.MergeVBoundsInto(pb);
		}

		#endregion IZBoundHolder

		public override void CollectStyles(PlotGroupStyleCollection styles)
		{
		}

		public override void PrepareGroupStyles(PlotGroupStyleCollection externalGroups, IPlotArea layer)
		{
		}

		public override void ApplyGroupStyles(PlotGroupStyleCollection externalGroups)
		{
		}

		/// <summary>
		/// Sets the plot style (or sub plot styles) in this item according to a template provided by the plot item in the template argument.
		/// </summary>
		/// <param name="template">The template item to copy the plot styles from.</param>
		/// <param name="strictness">Denotes the strictness the styles are copied from the template. See <see cref="PlotGroupStrictness" /> for more information.</param>
		public override void SetPlotStyleFromTemplate(IGPlotItem template, PlotGroupStrictness strictness)
		{
			if (!(template is DataMeshPlotItem))
				return;
			DataMeshPlotItem from = (DataMeshPlotItem)template;
			_plotStyle.CopyFrom(from._plotStyle);
		}

		/// <summary>
		/// Gets a pixelwise image of the data. Horizontal or vertical axes are not taken into accout.
		/// The horizontal dimension of the image is associated with the columns of the data table. The
		/// vertical dimension of the image is associated with the rows of the data table.
		/// </summary>
		/// <returns>Bitmap with the plot image.</returns>
		public Bitmap GetPixelwiseImage()
		{
			Bitmap result = null;
			GetPixelwiseImage(ref result);
			return result;
		}

		/// <summary>
		/// Gets a pixelwise image of the data. Horizontal or vertical axes are not taken into accout.
		/// The horizontal dimension of the image is associated with the columns of the data table. The
		/// vertical dimension of the image is associated with the rows of the data table.
		/// </summary>
		/// <param name="image">Bitmap to fill with the plot image. If null, a new image is created.</param>
		/// <exception cref="ArgumentException">An exception will be thrown if the provided image is smaller than the required dimensions.</exception>
		public void GetPixelwiseImage(ref Bitmap image)
		{
			Altaxo.Calc.LinearAlgebra.IROMatrix matrix;
			Altaxo.Calc.LinearAlgebra.IROVector rowVec, colVec;
			_plotData.DataTableMatrix.GetWrappers(
				x => (double)x, // transformation function for row header values
				Altaxo.Calc.RMath.IsFinite,       // selection functiton for row header values
				x => (double)x, // transformation function for column header values
				Altaxo.Calc.RMath.IsFinite,       // selection functiton for column header values
				out matrix,
				out rowVec,
				out colVec
				);

			//_plotStyle.GetPixelwiseImage(matrix, ref image);
		}

		/// <summary>
		/// Replaces path of items (intended for data items like tables and columns) by other paths. Thus it is possible
		/// to change a plot so that the plot items refer to another table.
		/// </summary>
		/// <param name="Report">Function that reports the found <see cref="DocNodeProxy"/> instances to the visitor.</param>
		public override void VisitDocumentReferences(DocNodeProxyReporter Report)
		{
			_plotData.VisitDocumentReferences(Report);
		}
	}
}