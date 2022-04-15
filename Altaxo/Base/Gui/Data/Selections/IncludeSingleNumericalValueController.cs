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
      if (args.Length >= 2 && (args[1] is DataTable))
        _supposedParentDataTable = (DataTable)args[1];

      if (args.Length >= 3 && args[2] is int)
        _supposedGroupNumber = (int)args[2];

      return base.InitializeDocument(args);
    }

    #region Bindings

    private string _ColumnText;

    public string ColumnText
    {
      get => _ColumnText;
      set
      {
        if (!(_ColumnText == value))
        {
          _ColumnText = value;
          OnPropertyChanged(nameof(ColumnText));
        }
      }
    }
    private string _ColumnToolTip;

    public string ColumnToolTip
    {
      get => _ColumnToolTip;
      set
      {
        if (!(_ColumnToolTip == value))
        {
          _ColumnToolTip = value;
          OnPropertyChanged(nameof(ColumnToolTip));
        }
      }
    }
    private int _ColumnStatus;

    public int ColumnStatus
    {
      get => _ColumnStatus;
      set
      {
        if (!(_ColumnStatus == value))
        {
          _ColumnStatus = value;
          OnPropertyChanged(nameof(ColumnStatus));
        }
      }
    }
    private string _ColumnTransformationText;

    public string ColumnTransformationText
    {
      get => _ColumnTransformationText;
      set
      {
        if (!(_ColumnTransformationText == value))
        {
          _ColumnTransformationText = value;
          OnPropertyChanged(nameof(ColumnTransformationText));
        }
      }
    }
    private string _ColumnTransformationToolTip;

    public string ColumnTransformationToolTip
    {
      get => _ColumnTransformationToolTip;
      set
      {
        if (!(_ColumnTransformationToolTip == value))
        {
          _ColumnTransformationToolTip = value;
          OnPropertyChanged(nameof(ColumnTransformationToolTip));
        }
      }
    }
    private string _DataLabel;

    public string DataLabel
    {
      get => _DataLabel;
      set
      {
        if (!(_DataLabel == value))
        {
          _DataLabel = value;
          OnPropertyChanged(nameof(DataLabel));
        }
      }
    }
    private double _Value;

    public double Value
    {
      get => _Value;
      set
      {
        if (!(_Value == value))
        {
          _Value = value;
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
      ColumnTransformationToolTip= info.TransformationToolTip;
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
