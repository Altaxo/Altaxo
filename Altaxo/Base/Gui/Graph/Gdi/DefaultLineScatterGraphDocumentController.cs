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

#nullable disable
using System.Collections.Generic;
using System.Linq;
using System.Windows.Input;
using Altaxo.Collections;
using Altaxo.Graph.Gdi;
using Altaxo.Gui.Common;

namespace Altaxo.Gui.Graph.Gdi
{
  public interface IDefaultLineScatterGraphDocumentView : IDataContextAwareView
  {
  }

  [ExpectedTypeOfView(typeof(IDefaultLineScatterGraphDocumentView))]
  public class DefaultLineScatterGraphDocumentController : MVCANControllerEditOriginalDocBase<GraphDocument, IDefaultLineScatterGraphDocumentView>
  {


    public override IEnumerable<ControllerAndSetNullMethod> GetSubControllers()
    {
      yield break;
    }

    public DefaultLineScatterGraphDocumentController()
    {
      CmdGraphFromProjectSelected = new RelayCommand(EhGraphFromProjectSelected);
    }

    #region Bindings

    public ICommand CmdGraphFromProjectSelected { get; }

    private ItemsController<GraphDocument> _graphsInProject;

    public ItemsController<GraphDocument> GraphsInProject
    {
      get => _graphsInProject;
      set
      {
        if (!(_graphsInProject == value))
        {
          _graphsInProject?.Dispose();
          _graphsInProject = value;
          OnPropertyChanged(nameof(GraphsInProject));
        }
      }
    }

    private string _previewTitle;

    public string PreviewTitle
    {
      get => _previewTitle;
      set
      {
        if (!(_previewTitle == value))
        {
          _previewTitle = value;
          OnPropertyChanged(nameof(PreviewTitle));
        }
      }
    }

    private System.Drawing.Bitmap _previewBitmap;

    public System.Drawing.Bitmap PreviewBitmap
    {
      get => _previewBitmap;
      set
      {
        if (!(_previewBitmap == value))
        {
          _previewBitmap?.Dispose();
          _previewBitmap = value;
          OnPropertyChanged(nameof(PreviewBitmap));
        }
      }
    }



    #endregion

    public override void Dispose(bool isDisposing)
    {
      GraphsInProject = null;
      PreviewBitmap = null;
      base.Dispose(isDisposing);
    }

    protected override void Initialize(bool initData)
    {
      base.Initialize(initData);

      if (initData)
      {
        var graphsInProject = new SelectableListNodeList();

        foreach (GraphDocument graph in Current.Project.GraphDocumentCollection)
        {
          graphsInProject.Add(new SelectableListNode(graph.Name, graph, false));
        }

        GraphsInProject = new ItemsController<GraphDocument>(graphsInProject);
      }
    }

    public override bool Apply(bool disposeController)
    {
      return ApplyEnd(true, disposeController);
    }



    private void EhGraphFromProjectSelected()
    {
      var graph = _graphsInProject.SelectedValue;
      if (graph is null)
        return;

      if (!IsGraphAppropriate(graph))
        return;

      _doc = (GraphDocument)graph.Clone();
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
      if (_view is null)
        return;

      if (doc is not null)
      {
        var bmp = GraphDocumentExportActions.RenderAsBitmap(doc, null, null, System.Drawing.Imaging.PixelFormat.Format32bppArgb, 150, 150);
        PreviewTitle = "Preview of graph template:";
        PreviewBitmap = bmp;
      }
      else
      {
        PreviewTitle = "No graph document selected";
        PreviewBitmap = null;
      }
    }
  }
}
