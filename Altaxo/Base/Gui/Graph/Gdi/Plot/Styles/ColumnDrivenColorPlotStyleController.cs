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

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Altaxo.Gui.Graph.Gdi.Plot.Styles
{
  using Altaxo.Data;
  using Altaxo.Graph.Gdi.Plot.Styles;
  using Altaxo.Graph.Scales;
  using ColorProvider;
  using Data;
  using Graph.Plot.Data;
  using Scales;

  public interface IColumnDrivenColorPlotStyleView
  {
    IDensityScaleView ScaleView { get; }

    IColorProviderView ColorProviderView { get; }

    /// <summary>
    /// Initializes the name of the label column.
    /// </summary>
    /// <param name="columnAsText">Label column's name.</param>
    void Init_DataColumn(string columnAsText, string toolTip, int status);

    /// <summary>
    /// Initializes the transformation text.
    /// </summary>
    /// <param name="text">Text for the transformation</param>
    void Init_DataColumnTransformation(string text, string toolTip);
  }

  [UserControllerForObject(typeof(ColumnDrivenColorPlotStyle))]
  [ExpectedTypeOfView(typeof(IColumnDrivenColorPlotStyleView))]
  public class ColumnDrivenColorPlotStyleController : MVCANControllerEditOriginalDocBase<ColumnDrivenColorPlotStyle, IColumnDrivenColorPlotStyleView>, IColumnDataExternallyControlled
  {
    private DensityScaleController _scaleController;
    private ColorProviderController _colorProviderController;

    /// <summary>
    /// The data table that the column of the style should belong to.
    /// </summary>
    private DataTable _supposedParentDataTable;

    /// <summary>
    /// The group number that the column of the style should belong to.
    /// </summary>
    private int _supposedGroupNumber;

    public override bool InitializeDocument(params object[] args)
    {
      if (args.Length >= 2 && (args[1] is DataTable))
        _supposedParentDataTable = (DataTable)args[1];

      if (args.Length >= 3 && args[2] is int)
        _supposedGroupNumber = (int)args[2];

      return base.InitializeDocument(args);
    }

    public override IEnumerable<ControllerAndSetNullMethod> GetSubControllers()
    {
      yield return new ControllerAndSetNullMethod(_scaleController, () => _scaleController = null);
      yield return new ControllerAndSetNullMethod(_colorProviderController, () => _colorProviderController = null);
    }

    public override void Dispose(bool isDisposing)
    {
      base.Dispose(isDisposing);
    }

    protected override void Initialize(bool initData)
    {
      base.Initialize(initData);

      if (initData)
      {
        _scaleController = new DensityScaleController(newScale => _doc.Scale = (NumericalScale)newScale) { UseDocumentCopy = UseDocument.Directly };
        _scaleController.InitializeDocument(_doc.Scale);

        _colorProviderController = new ColorProviderController(newColorProvider => _doc.ColorProvider = newColorProvider) { UseDocumentCopy = UseDocument.Directly };
        _colorProviderController.InitializeDocument(_doc.ColorProvider);
      }

      if (null != _view)
      {
        _scaleController.ViewObject = _view.ScaleView;
        _colorProviderController.ViewObject = _view.ColorProviderView;
        InitializeDataColumnText();
      }
    }

    public override bool Apply(bool disposeController)
    {
      if (!_scaleController.Apply(disposeController))
        return false;

      if (!(_scaleController.ModelObject is Altaxo.Graph.Scales.NumericalScale))
      {
        Current.Gui.ErrorMessageBox("Please choose a numerical scale, since only those scales are supported here");
        return false;
      }

      _doc.Scale = (Altaxo.Graph.Scales.NumericalScale)_scaleController.ModelObject;

      if (!_colorProviderController.Apply(disposeController))
        return false;
      _doc.ColorProvider = (Altaxo.Graph.Gdi.Plot.IColorProvider)_colorProviderController.ModelObject;

      return ApplyEnd(true, disposeController);
    }

    private void InitializeDataColumnText()
    {
      var info = new PlotColumnInformation(_doc.DataColumn, _doc.DataColumnName);
      info.Update(_supposedParentDataTable, _supposedGroupNumber);

      _view?.Init_DataColumn(info.PlotColumnBoxText, info.PlotColumnToolTip, (int)info.PlotColumnBoxState);
      _view?.Init_DataColumnTransformation(info.TransformationTextToShow, info.TransformationToolTip);
    }

    /// <summary>
    /// Gets the additional columns that the controller's document is referring to.
    /// </summary>
    /// <returns>Enumeration of tuples.
    /// Item1 is a label to be shown in the column data dialog to let the user identify the column.
    /// Item2 is the column itself,
    /// Item3 is the column name (last part of the full path to the column), and
    /// Item4 is an action which sets the column (and by the way the supposed data table the column belongs to.</returns>
    public IEnumerable<(string ColumnLabel, IReadableColumn Column, string ColumnName, Action<IReadableColumn, DataTable, int> ColumnSetAction)> GetDataColumnsExternallyControlled()
    {
      yield return (
        "Color", // label to be shown
        _doc.DataColumn,
        _doc.DataColumnName,
        (column, table, group) =>
        {
          _doc.DataColumn = column;
          _supposedParentDataTable = table;
          _supposedGroupNumber = group;
          InitializeDataColumnText();
        }
      );
    }
  }
}
