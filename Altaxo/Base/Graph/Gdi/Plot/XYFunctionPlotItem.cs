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
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Altaxo.Graph.Gdi.Plot
{
	using Data;
	using Graph.Plot.Data;
	using Styles;

	/// <summary>
	/// Association of data and style specialized for x-y-plots of column data.
	/// </summary>
	public class XYFunctionPlotItem : G2DPlotItem
	{
		protected XYFunctionPlotData _plotData;

		#region Serialization

		[Altaxo.Serialization.Xml.XmlSerializationSurrogateFor("AltaxoBase", "Altaxo.Graph.XYFunctionPlotItem", 0)]
		private class XmlSerializationSurrogate0 : Altaxo.Serialization.Xml.IXmlSerializationSurrogate
		{
			public void Serialize(object obj, Altaxo.Serialization.Xml.IXmlSerializationInfo info)
			{
				XYFunctionPlotItem s = (XYFunctionPlotItem)obj;
				info.AddValue("Data", s._plotData);
				info.AddValue("Style", s._plotStyles);
			}

			public object Deserialize(object o, Altaxo.Serialization.Xml.IXmlDeserializationInfo info, object parent)
			{
				XYFunctionPlotData pa = (XYFunctionPlotData)info.GetValue("Data", null);
				XYLineScatterPlotStyle lsps = (XYLineScatterPlotStyle)info.GetValue("Style", null);

				G2DPlotStyleCollection ps = new G2DPlotStyleCollection();
				if (null != lsps.ScatterStyle)
					ps.Add(new ScatterPlotStyle(lsps.ScatterStyle));
				if (null != lsps.XYPlotLineStyle)
					ps.Add(new LinePlotStyle(lsps.XYPlotLineStyle));

				if (null == o)
				{
					return new XYFunctionPlotItem(pa, ps);
				}
				else
				{
					XYFunctionPlotItem s = (XYFunctionPlotItem)o;
					s.Data = pa;
					s.Style = ps;
					return s;
				}
			}
		}

		[Altaxo.Serialization.Xml.XmlSerializationSurrogateFor("AltaxoBase", "Altaxo.Graph.XYFunctionPlotItem", 1)]
		[Altaxo.Serialization.Xml.XmlSerializationSurrogateFor(typeof(XYFunctionPlotItem), 2)]
		private class XmlSerializationSurrogate1 : Altaxo.Serialization.Xml.IXmlSerializationSurrogate
		{
			public void Serialize(object obj, Altaxo.Serialization.Xml.IXmlSerializationInfo info)
			{
				XYFunctionPlotItem s = (XYFunctionPlotItem)obj;
				info.AddValue("Data", s._plotData);
				info.AddValue("Style", s._plotStyles);
			}

			public object Deserialize(object o, Altaxo.Serialization.Xml.IXmlDeserializationInfo info, object parent)
			{
				XYFunctionPlotData pa = (XYFunctionPlotData)info.GetValue("Data", null);
				G2DPlotStyleCollection ps = (G2DPlotStyleCollection)info.GetValue("Style", null);

				if (null == o)
				{
					return new XYFunctionPlotItem(pa, ps);
				}
				else
				{
					XYFunctionPlotItem s = (XYFunctionPlotItem)o;
					s.Data = pa;
					s.Style = ps;
					return s;
				}
			}
		}

		#endregion Serialization

		private System.Collections.Generic.IEnumerable<Main.DocumentNodeAndName> GetLocalDocumentNodeChildrenWithName()
		{
			if (null != _plotData)
				yield return new Main.DocumentNodeAndName(_plotData, () => _plotData = null, "Data");
		}

		protected override System.Collections.Generic.IEnumerable<Main.DocumentNodeAndName> GetDocumentNodeChildrenWithName()
		{
			return GetLocalDocumentNodeChildrenWithName().Concat(base.GetDocumentNodeChildrenWithName());
		}

		public XYFunctionPlotItem(XYFunctionPlotData pa, G2DPlotStyleCollection ps)
		{
			this.Data = pa;
			this.Style = ps;
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="XYFunctionPlotItem"/> class. Intended for use in derived classes.
		/// </summary>
		protected XYFunctionPlotItem()
		{
		}

		public XYFunctionPlotItem(XYFunctionPlotItem from)
		{
			CopyFrom((PlotItem)from);
		}

		public override bool CopyFrom(object obj)
		{
			if (object.ReferenceEquals(this, obj))
				return true;

			var copied = base.CopyFrom(obj);
			if (copied && obj is XYFunctionPlotItem from)
			{
				ChildCopyToMember(ref _plotData, from._plotData);
			}
			return copied;
		}

		public override object Clone()
		{
			return new XYFunctionPlotItem(this);
		}

		public override Main.IDocumentLeafNode DataObject
		{
			get { return _plotData; }
		}

		public virtual XYFunctionPlotData Data
		{
			get { return _plotData; }
			set
			{
				if (null == value)
					throw new System.ArgumentNullException();
				else
				{
					XYFunctionPlotData oldvalue = _plotData;
					_plotData = value;
					_plotData.ParentObject = this;

					if (!object.ReferenceEquals(_plotData, value))
					{
						EhSelfChanged(PlotItemDataChangedEventArgs.Empty);
					}
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

		public override Processed2DPlotData GetRangesAndPoints(IPlotArea layer)
		{
			return _plotData.GetRangesAndPoints(layer);
		}

		/// <summary>
		/// This routine ensures that the plot item updates all its cached data and send the appropriate
		/// events if something has changed. Called before the layer paint routine paints the axes because
		/// it must be ensured that the axes are scaled correctly before the plots are painted.
		/// </summary>
		/// <param name="layer">The plot layer.</param>
		public override void PrepareScales(IPlotArea layer)
		{
			// nothing really to do here
		}
	}
}