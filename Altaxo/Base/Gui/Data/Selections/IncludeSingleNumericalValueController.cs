﻿#region Copyright

/////////////////////////////////////////////////////////////////////////////
//    Altaxo:  a data processing and data plotting program
//    Copyright (C) 2002-2022 Dr. Dirk Lellinger
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
using Altaxo.Data;
using Altaxo.Data.Selections;
using Altaxo.Gui.Graph.Plot.Data;

namespace Altaxo.Gui.Data.Selections
{
  public interface IIncludeSingleNumericalValueView : IDataContextAwareView
  {
  }

  [UserControllerForObject(typeof(IncludeSingleNumericalValue), 100)]
  [ExpectedTypeOfView(typeof(IIncludeSingleNumericalValueView))]
  public class IncludeSingleNumericalValueController : MVCANControllerEditCopyOfDocBase<IncludeSingleNumericalValue, IIncludeSingleNumericalValueView>, IDataColumnController
  {
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
      if (args.Length >= 2 && (args[1] is DataTable dt))
        _supposedParentDataTable = dt;

      if (args.Length >= 3 && args[2] is int gn)
        _supposedGroupNumber = gn;

      return base.InitializeDocument(args);
    }

    #region Bindings

    private string _columnText;

    public string ColumnText
    {
      get => _columnText;
      set
      {
        if (!(_columnText == value))
        {
          _columnText = value;
          OnPropertyChanged(nameof(ColumnText));
        }
      }
    }
    private string _columnToolTip;

    public string ColumnToolTip
    {
      get => _columnToolTip;
      set
      {
        if (!(_columnToolTip == value))
        {
          _columnToolTip = value;
          OnPropertyChanged(nameof(ColumnToolTip));
        }
      }
    }
    private int _columnStatus;

    public int ColumnStatus
    {
      get => _columnStatus;
      set
      {
        if (!(_columnStatus == value))
        {
          _columnStatus = value;
          OnPropertyChanged(nameof(ColumnStatus));
        }
      }
    }
    private string _columnTransformationText;

    public string ColumnTransformationText
    {
      get => _columnTransformationText;
      set
      {
        if (!(_columnTransformationText == value))
        {
          _columnTransformationText = value;
          OnPropertyChanged(nameof(ColumnTransformationText));
        }
      }
    }
    private string _columnTransformationToolTip;

    public string ColumnTransformationToolTip
    {
      get => _columnTransformationToolTip;
      set
      {
        if (!(_columnTransformationToolTip == value))
        {
          _columnTransformationToolTip = value;
          OnPropertyChanged(nameof(ColumnTransformationToolTip));
        }
      }
    }
    private string _dataLabel;

    public string DataLabel
    {
      get => _dataLabel;
      set
      {
        if (!(_dataLabel == value))
        {
          _dataLabel = value;
          OnPropertyChanged(nameof(DataLabel));
        }
      }
    }
    private double _value;

    public double Value
    {
      get => _value;
      set
      {
        if (!(_value == value))
        {
          _value = value;
          OnPropertyChanged(nameof(Value));
        }
      }
    }


    #endregion

    protected override void Initialize(bool initData)
    {
      base.Initialize(initData);

      if (initData)
      {

        Value = _doc.Value;
        View_InitializeColumn();
      }
    }

    private void View_InitializeColumn()
    {
      var info = new PlotColumnInformation(_doc.Column, _doc.ColumnName) { PlotColumnBoxStateIfColumnIsMissing = PlotColumnControlState.Error };
      info.Update(_supposedParentDataTable, _supposedGroupNumber);
      ColumnText = info.PlotColumnBoxText;
      ColumnToolTip = info.PlotColumnToolTip;
      ColumnStatus = (int)info.PlotColumnBoxState;
      ColumnTransformationText = info.TransformationTextToShow;
      ColumnTransformationToolTip = info.TransformationToolTip;
    }

    public override bool Apply(bool disposeController)
    {
      var column = _doc.Column;

      if (column is null)
      {
        Current.Gui.ErrorMessageBox(
          "No column is set in the range of numerical values that is part of the plot range selection.\r\n" +
          "Thus it is high likely that the plot range selection will yield no points.\r\n" +
          "Please select a column for this range selection or remove this range selection."
          );

        return ApplyEnd(false, disposeController);
      }

      double value = Value;

      _doc = new IncludeSingleNumericalValue(value, column);

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
      DataLabel = $"Col#{idx}:";
    }
  }
}
