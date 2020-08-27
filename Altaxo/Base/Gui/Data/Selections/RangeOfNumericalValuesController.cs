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

#nullable disable
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Altaxo.Collections;
using Altaxo.Data;
using Altaxo.Data.Selections;
using Altaxo.Gui.Graph.Plot.Data;

namespace Altaxo.Gui.Data.Selections
{
  public interface IRangeOfNumericalValuesView
  {
    void Init_Column(string boxText, string toolTip, int status);

    void Init_ColumnTransformation(string boxText, string toolTip);

    void Init_LowerInclusive(Altaxo.Collections.SelectableListNodeList list);

    void Init_UpperInclusive(Altaxo.Collections.SelectableListNodeList list);

    void Init_Index(int idx);

    double LowerValue { get; set; }
    double UpperValue { get; set; }
  }

  [UserControllerForObject(typeof(RangeOfNumericalValues), 100)]
  [ExpectedTypeOfView(typeof(IRangeOfNumericalValuesView))]
  public class RangeOfNumericalValuesController : MVCANControllerEditCopyOfDocBase<RangeOfNumericalValues, IRangeOfNumericalValuesView>, IDataColumnController
  {
    /// <summary>
    /// The data table that the column of the style should belong to.
    /// </summary>
    private DataTable _supposedParentDataTable;

    /// <summary>
    /// The group number that the column of the style should belong to.
    /// </summary>
    private int _supposedGroupNumber;

    private SelectableListNodeList _lowerInclusive;
    private SelectableListNodeList _upperInclusive;

    public override bool InitializeDocument(params object[] args)
    {
      if (args.Length >= 2 && (args[1] is DataTable))
        _supposedParentDataTable = (DataTable)args[1];

      if (args.Length >= 3 && args[2] is int)
        _supposedGroupNumber = (int)args[2];

      return base.InitializeDocument(args);
    }

    protected override void Initialize(bool initData)
    {
      base.Initialize(initData);

      if (initData)
      {
        _lowerInclusive = new SelectableListNodeList();
        _upperInclusive = new SelectableListNodeList();

        _lowerInclusive.Add(new SelectableListNode("<", false, !_doc.IsLowerValueInclusive));
        _lowerInclusive.Add(new SelectableListNode("<=", true, _doc.IsLowerValueInclusive));

        _upperInclusive.Add(new SelectableListNode("<", false, !_doc.IsUpperValueInclusive));
        _upperInclusive.Add(new SelectableListNode("<=", true, _doc.IsUpperValueInclusive));
      }
      if (null != _view)
      {
        _view.LowerValue = _doc.LowerValue;
        _view.Init_LowerInclusive(_lowerInclusive);
        _view.UpperValue = _doc.UpperValue;
        _view.Init_UpperInclusive(_upperInclusive);

        View_InitializeColumn();
      }
    }

    private void View_InitializeColumn()
    {
      var info = new PlotColumnInformation(_doc.Column, _doc.ColumnName) { PlotColumnBoxStateIfColumnIsMissing = PlotColumnControlState.Error };
      info.Update(_supposedParentDataTable, _supposedGroupNumber);
      _view?.Init_Column(info.PlotColumnBoxText, info.PlotColumnToolTip, (int)info.PlotColumnBoxState);
      _view?.Init_ColumnTransformation(info.TransformationTextToShow, info.TransformationToolTip);
    }

    public override bool Apply(bool disposeController)
    {
      var column = _doc.Column;

      if (null == column)
      {
        Current.Gui.ErrorMessageBox(
          "No column is set in the range of numerical values that is part of the plot range selection.\r\n" +
          "Thus it is high likely that the plot range selection will yield no points.\r\n" +
          "Please select a column for this range selection or remove this range selection."
          );

        return ApplyEnd(false, disposeController);
      }

      bool isLowerInclusive = (bool)_lowerInclusive.FirstSelectedNode.Tag;
      bool isUpperInclusive = (bool)_upperInclusive.FirstSelectedNode.Tag;

      double lower = _view.LowerValue;
      double upper = _view.UpperValue;

      _doc = new RangeOfNumericalValues(lower, isLowerInclusive, upper, isUpperInclusive, column);

      return ApplyEnd(true, disposeController);
    }

    public override IEnumerable<ControllerAndSetNullMethod> GetSubControllers()
    {
      yield break;
    }

    void IDataColumnController.SetDataColumn(IReadableColumn column, DataTable supposedParentDataTable, int supposedGroupNumber)
    {
      _supposedParentDataTable = supposedParentDataTable;
      _supposedGroupNumber = supposedGroupNumber;
      _doc.Column = column;
      View_InitializeColumn();
    }

    IReadableColumn IDataColumnController.Column
    {
      get
      {
        return _doc.Column;
      }
    }

    string IDataColumnController.ColumnName
    {
      get
      {
        return _doc.ColumnName;
      }
    }

    void IDataColumnController.SetIndex(int idx)
    {
      _view.Init_Index(idx);
    }
  }
}
