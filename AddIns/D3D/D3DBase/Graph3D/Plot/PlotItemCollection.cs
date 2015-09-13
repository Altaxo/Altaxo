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

using Altaxo.Collections;
using Altaxo.Graph;
using Altaxo.Graph.Gdi.Plot.Groups;
using Altaxo.Graph.Plot;
using Altaxo.Graph.Plot.Data;
using Altaxo.Graph.Plot.Groups;
using Altaxo.Graph.Scales;
using Altaxo.Graph.Scales.Boundaries;
using Altaxo.Main;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;

namespace Altaxo.Graph3D.Plot
{
	using Altaxo.Graph.Plot.Groups;
	using Graph.Gdi;
	using System.Collections;

	[Serializable]
	public class PlotItemCollection
		:
		Main.SuspendableDocumentNodeWithSetOfEventArgs,
		IGPlotItem,
		IEnumerable<IGPlotItem>,
		IXBoundsHolder,
		IYBoundsHolder
	{
		private XYPlotLayer3D xYPlotLayer3D;
		private PlotItemCollection _plotItems;

		public PlotItemCollection(XYPlotLayer3D xYPlotLayer3D)
		{
			this.xYPlotLayer3D = xYPlotLayer3D;
		}

		public PlotItemCollection(XYPlotLayer3D xYPlotLayer3D, PlotItemCollection _plotItems) : this(xYPlotLayer3D)
		{
			this._plotItems = _plotItems;
		}

		public IGPlotItem[] Flattened { get; }

		public IList<IGPlotItem> ChildNodes
		{
			get
			{
				throw new NotImplementedException();
			}
		}

		IEnumerable<IGPlotItem> ITreeNode<IGPlotItem>.ChildNodes
		{
			get
			{
				throw new NotImplementedException();
			}
		}

		public IGPlotItem ParentNode
		{
			get
			{
				throw new NotImplementedException();
			}
		}

		public IEnumerator<IGPlotItem> GetEnumerator()
		{
			throw new NotImplementedException();
		}

		public void MergeXBoundsInto(IPhysicalBoundaries pb)
		{
			throw new NotImplementedException();
		}

		public void MergeYBoundsInto(IPhysicalBoundaries pb)
		{
			throw new NotImplementedException();
		}

		public void PaintSymbol(IGraphicContext3D g, RectangleD3D symbolRect)
		{
			throw new NotImplementedException();
		}

		protected override IEnumerable<DocumentNodeAndName> GetDocumentNodeChildrenWithName()
		{
			throw new NotImplementedException();
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			throw new NotImplementedException();
		}

		internal void PrepareScales(XYPlotLayer3D xYPlotLayer3D)
		{
			throw new NotImplementedException();
		}

		internal void PrepareGroupStyles(object p, XYPlotLayer3D xYPlotLayer3D)
		{
			throw new NotImplementedException();
		}

		internal void ApplyGroupStyles(object p)
		{
			throw new NotImplementedException();
		}

		internal void PaintPostprocessing()
		{
			throw new NotImplementedException();
		}

		internal void VisitDocumentReferences(DocNodeProxyReporter report)
		{
			throw new NotImplementedException();
		}

		internal void CopyFrom(PlotItemCollection _plotItems, GraphCopyOptions options)
		{
			throw new NotImplementedException();
		}

		public string GetName(int v)
		{
			throw new NotImplementedException();
		}

		public bool CopyFrom(object obj)
		{
			throw new NotImplementedException();
		}

		public object Clone()
		{
			throw new NotImplementedException();
		}
	}
}