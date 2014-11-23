#region Copyright

/////////////////////////////////////////////////////////////////////////////
//    Altaxo:  a data processing and data plotting program
//    Copyright (C) 2002-2014 Dr. Dirk Lellinger
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
using Altaxo.Graph.Gdi;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Altaxo.Gui.Graph
{
	public interface IDefaultLineScatterGraphDocumentView
	{
		SelectableListNodeList GraphsInProject { set; }

		event Action GraphFromProjectSelected;

		void SetPreviewBitmap(string title, System.Drawing.Bitmap bmp);
	}

	[ExpectedTypeOfView(typeof(IDefaultLineScatterGraphDocumentView))]
	public class DefaultLineScatterGraphDocumentController : MVCANControllerBase<GraphDocument, IDefaultLineScatterGraphDocumentView>
	{
		private SelectableListNodeList _graphsInProject = new SelectableListNodeList();

		protected override void Initialize(bool initData)
		{
			if (initData)
			{
				_graphsInProject.Clear();

				foreach (GraphDocument graph in Current.Project.GraphDocumentCollection)
				{
					_graphsInProject.Add(new SelectableListNode(graph.Name, graph, false));
				}
			}
			if (null != _view)
			{
				_view.GraphsInProject = _graphsInProject;
				RenderPreview(_doc);
			}
		}

		protected override void AttachView()
		{
			base.AttachView();

			_view.GraphFromProjectSelected += EhGraphFromProjectSelected;
		}

		protected override void DetachView()
		{
			_view.GraphFromProjectSelected -= EhGraphFromProjectSelected;

			base.DetachView();
		}

		private void EhGraphFromProjectSelected()
		{
			var node = _graphsInProject.FirstSelectedNode;
			if (null == node)
				return;

			if (!IsGraphAppropriate((GraphDocument)node.Tag))
				return;

			_doc = (GraphDocument)((GraphDocument)node.Tag).Clone();
			StripGraphFromPlotItems(_doc);

			RenderPreview(_doc);
		}

		private bool IsGraphAppropriate(GraphDocument doc)
		{
			if (doc.RootLayer.Layers.Count == 0)
			{
				Current.Gui.ErrorMessageBox("The selected graph is not appropriate as template because it does not have a child layer");
				return false;
			}

			if (!(doc.RootLayer.Layers[0] is XYPlotLayer))
			{
				Current.Gui.ErrorMessageBox("The selected graph is not appropriate as template because the first child layer is not an XYPlotLayer");
				return false;
			}
			return true;
		}

		private void StripGraphFromPlotItems(GraphDocument doc)
		{
			foreach (var layer in doc.RootLayer.Layers.OfType<XYPlotLayer>())
			{
				layer.PlotItems.ClearPlotItemsAndGroupStyles();
			}
		}

		private void RenderPreview(GraphDocument doc)
		{
			if (null == _view)
				return;

			if (null != doc)
			{
				using (var bmp = GraphDocumentExportActions.RenderAsBitmap(doc, null, null, System.Drawing.Imaging.PixelFormat.Format32bppArgb, 150, 150))
				{
					_view.SetPreviewBitmap("Preview of graph template:", bmp);
				}
			}
			else
			{
				_view.SetPreviewBitmap("No graph document selected", null);
			}
		}

		public override bool Apply()
		{
			if (!object.ReferenceEquals(_originalDoc, _doc))
				CopyHelper.Copy(ref _originalDoc, _doc);

			return true;
		}
	}
}