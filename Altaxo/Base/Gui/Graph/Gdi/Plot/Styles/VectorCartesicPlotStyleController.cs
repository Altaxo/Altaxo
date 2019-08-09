#region Copyright

/////////////////////////////////////////////////////////////////////////////
//    Altaxo:  a data processing and data plotting program
//    Copyright (C) 2002-2016 Dr. Dirk Lellinger
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
using System.Text;
using Altaxo.Collections;
using Altaxo.Data;
using Altaxo.Graph.Gdi;
using Altaxo.Graph.Gdi.Plot.Styles;
using Altaxo.Gui.Data;
using Altaxo.Gui.Graph;
using Altaxo.Gui.Graph.Plot.Data;
using Altaxo.Gui.Graph.Plot.Groups;

namespace Altaxo.Gui.Graph.Gdi.Plot.Styles
{
  #region Interfaces

  public interface IVectorCartesicPlotStyleView
  {
    bool IndependentColor { get; set; }

    bool ShowPlotColorsOnly { set; }

    PenX Pen { get; set; }

    bool IndependentSymbolSize { get; set; }

    double SymbolSize { get; set; }

    double LineWidth1Offset { get; set; }
    double LineWidth1Factor { get; set; }

    double EndCapSizeOffset { get; set; }
    double EndCapSizeFactor { get; set; }

    bool UseSymbolGap { get; set; }

    double SymbolGapOffset { get; set; }

    double SymbolGapFactor { get; set; }

    int SkipFrequency { get; set; }

    bool IndependentSkipFrequency { get; set; }

    bool IgnoreMissingDataPoints { get; set; }

    bool IndependentOnShiftingGroupStyles { get; set; }

    bool UseManualVectorLength { get; set; }

    double VectorLengthOffset { get; set; }

    double VectorLengthFactor { get; set; }

    void Initialize_MeaningOfValues(SelectableListNodeList list);

    /// <summary>
    /// Initializes the common error column.
    /// </summary>
    /// <param name="columnAsText">Column's name.</param>
    /// <param name="toolTip">Column's tooltip.</param>
    /// <param name="status">Column's status display.</param>
    void Initialize_ColumnX(string columnAsText, string toolTip, int status);

    void Initialize_ColumnXTransformation(string transformationTextToShow, string transformationToolTip);

    /// <summary>
    /// Initializes the positive error column.
    /// </summary>
    /// <param name="columnAsText">Column's name.</param>
    /// <param name="columnToolTip">Column's tooltip.</param>
    /// <param name="columnStatus">Column's status display.</param>
    void Initialize_ColumnY(string columnAsText, string columnToolTip, int columnStatus);

    void Initialize_ColumnYTransformation(string transformationTextToShow, string transformationToolTip);

    /// <summary>
    /// Occurs when the user choice for IndependentColor of the fill brush has changed.
    /// </summary>
    event Action IndependentColorChanged;
  }

  #endregion Interfaces

  [UserControllerForObject(typeof(VectorCartesicPlotStyle))]
  [ExpectedTypeOfView(typeof(IVectorCartesicPlotStyleView))]
  public class VectorCartesicPlotStyleController : MVCANControllerEditOriginalDocBase<VectorCartesicPlotStyle, IVectorCartesicPlotStyleView>, IColumnDataExternallyControlled
  {
    /// <summary>Tracks the presence of a color group style in the parent collection.</summary>
    private ColorGroupStylePresenceTracker _colorGroupStyleTracker;

    private SelectableListNodeList _meaningOfValues;

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
      yield break;
    }

    public override void Dispose(bool isDisposing)
    {
      _colorGroupStyleTracker = null;

      base.Dispose(isDisposing);
    }

    protected override void Initialize(bool initData)
    {
      base.Initialize(initData);

      if (initData)
      {
        _colorGroupStyleTracker = new ColorGroupStylePresenceTracker(_doc, EhIndependentColorChanged);

        _meaningOfValues = new SelectableListNodeList(_doc.MeaningOfValues);
      }
      if (_view != null)
      {
        _view.UseManualVectorLength = _doc.UseManualVectorLength;
        _view.VectorLengthOffset = _doc.VectorLengthOffset;
        _view.VectorLengthFactor = _doc.VectorLengthFactor;

        _view.IndependentColor = _doc.IndependentColor;
        _view.ShowPlotColorsOnly = _colorGroupStyleTracker.MustUsePlotColorsOnly(_doc.IndependentColor);
        _view.Pen = _doc.Pen;

        _view.IndependentSymbolSize = _doc.IndependentSymbolSize;
        _view.SymbolSize = _doc.SymbolSize;

        _view.LineWidth1Offset = _doc.LineWidth1Offset;
        _view.LineWidth1Factor = _doc.LineWidth1Factor;

        _view.EndCapSizeOffset = _doc.EndCapSizeOffset;
        _view.EndCapSizeFactor = _doc.EndCapSizeFactor;

        _view.UseSymbolGap = _doc.UseSymbolGap;
        _view.SymbolGapOffset = _doc.SymbolGapOffset;
        _view.SymbolGapFactor = _doc.SymbolGapFactor;

        _view.SkipFrequency = _doc.SkipFrequency;
        _view.IndependentSkipFrequency = _doc.IndependentSkipFrequency;
        _view.IgnoreMissingDataPoints = _doc.IgnoreMissingDataPoints;
        _view.IndependentOnShiftingGroupStyles = _doc.IndependentOnShiftingGroupStyles;

        _view.Initialize_MeaningOfValues(_meaningOfValues);

        InitializeColumnXText();
        InitializeColumnYText();
      }
    }

    public override bool Apply(bool disposeController)
    {
      _doc.UseManualVectorLength = _view.UseManualVectorLength;
      _doc.VectorLengthOffset = _view.VectorLengthOffset;
      _doc.VectorLengthFactor = _view.VectorLengthFactor;

      _doc.IndependentColor = _view.IndependentColor;
      _doc.Pen = _view.Pen;
      _doc.IndependentSymbolSize = _view.IndependentSymbolSize;
      _doc.SymbolSize = _view.SymbolSize;

      _doc.LineWidth1Offset = _view.LineWidth1Offset;
      _doc.LineWidth1Factor = _view.LineWidth1Factor;

      _doc.EndCapSizeOffset = _view.EndCapSizeOffset;
      _doc.EndCapSizeFactor = _view.EndCapSizeFactor;

      _doc.UseSymbolGap = _view.UseSymbolGap;
      _doc.SymbolGapOffset = _view.SymbolGapOffset;
      _doc.SymbolGapFactor = _view.SymbolGapFactor;

      _doc.IndependentSkipFrequency = _view.IndependentSkipFrequency;
      _doc.SkipFrequency = _view.SkipFrequency;
      _doc.IgnoreMissingDataPoints = _view.IgnoreMissingDataPoints;
      _doc.IndependentOnShiftingGroupStyles = _view.IndependentOnShiftingGroupStyles;

      _doc.MeaningOfValues = (VectorCartesicPlotStyle.ValueInterpretation)_meaningOfValues.FirstSelectedNode.Tag;

      return ApplyEnd(true, disposeController);
    }

    protected override void AttachView()
    {
      base.AttachView();

      _view.IndependentColorChanged += EhIndependentColorChanged;
    }

    protected override void DetachView()
    {
      _view.IndependentColorChanged -= EhIndependentColorChanged;

      base.DetachView();
    }

    private void InitializeColumnXText()
    {
      var info = new PlotColumnInformation(_doc.ColumnX, _doc.ColumnXDataColumnName);
      info.Update(_supposedParentDataTable, _supposedGroupNumber);

      _view?.Initialize_ColumnX(info.PlotColumnBoxText, info.PlotColumnToolTip, (int)info.PlotColumnBoxState);
      _view?.Initialize_ColumnXTransformation(info.TransformationTextToShow, info.TransformationToolTip);
    }

    private void InitializeColumnYText()
    {
      var info = new PlotColumnInformation(_doc.ColumnY, _doc.ColumnYDataColumnName);
      info.Update(_supposedParentDataTable, _supposedGroupNumber);

      _view?.Initialize_ColumnY(info.PlotColumnBoxText, info.PlotColumnToolTip, (int)info.PlotColumnBoxState);
      _view?.Initialize_ColumnYTransformation(info.TransformationTextToShow, info.TransformationToolTip);
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
        "X", // label to be shown
        _doc.ColumnX,
        _doc.ColumnXDataColumnName,
        (column, table, group) =>
        {
          _doc.ColumnX = column;
          _supposedParentDataTable = table;
          _supposedGroupNumber = group;
          InitializeColumnXText();
        }
      );

      yield return (
        "Y", // label to be shown
        _doc.ColumnY,
        _doc.ColumnYDataColumnName,
        (column, table, group) =>
        {
          _doc.ColumnY = column;
          _supposedParentDataTable = table;
          _supposedGroupNumber = group;
          InitializeColumnYText();
        }
      );
    }

    private void EhIndependentColorChanged()
    {
      if (null != _view)
      {
        _doc.IndependentColor = _view.IndependentColor;
        _view.ShowPlotColorsOnly = _colorGroupStyleTracker.MustUsePlotColorsOnly(_doc.IndependentColor);
      }
    }
  }
}
