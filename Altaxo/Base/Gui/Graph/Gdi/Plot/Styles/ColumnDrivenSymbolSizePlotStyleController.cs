#region Copyright

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
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Altaxo.Gui.Graph.Gdi.Plot.Styles
{
  using Altaxo.Data;
  using Altaxo.Graph.Gdi.Plot.Styles;
  using Altaxo.Graph.Scales;
  using Altaxo.Units;
  using Data;
  using Graph.Plot.Data;
  using Scales;

  public interface IColumnDrivenSymbolSizePlotStyleView : IDataContextAwareView
  {
  }

  [UserControllerForObject(typeof(ColumnDrivenSymbolSizePlotStyle))]
  [ExpectedTypeOfView(typeof(IColumnDrivenSymbolSizePlotStyleView))]
  public class ColumnDrivenSymbolSizePlotStyleController : MVCANControllerEditOriginalDocBase<ColumnDrivenSymbolSizePlotStyle, IColumnDrivenSymbolSizePlotStyleView>, IColumnDataExternallyControlled
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

    public override IEnumerable<ControllerAndSetNullMethod> GetSubControllers()
    {
      yield return new ControllerAndSetNullMethod(_scaleController, () => _scaleController = null);
    }

    #region Bindings

    private DensityScaleController _scaleController;

    public DensityScaleController ScaleController
    {
      get => _scaleController;
      set
      {
        if (!(_scaleController == value))
        {
          _scaleController?.Dispose();
          _scaleController = value;
          OnPropertyChanged(nameof(ScaleController));
        }
      }
    }

    private string _DataColumnText;

    public string DataColumnText
    {
      get => _DataColumnText;
      set
      {
        if (!(_DataColumnText == value))
        {
          _DataColumnText = value;
          OnPropertyChanged(nameof(DataColumnText));
        }
      }
    }
    private string _DataColumnToolTip;

    public string DataColumnToolTip
    {
      get => _DataColumnToolTip;
      set
      {
        if (!(_DataColumnToolTip == value))
        {
          _DataColumnToolTip = value;
          OnPropertyChanged(nameof(DataColumnToolTip));
        }
      }
    }
    private int _DataColumnStatus;

    public int DataColumnStatus
    {
      get => _DataColumnStatus;
      set
      {
        if (!(_DataColumnStatus == value))
        {
          _DataColumnStatus = value;
          OnPropertyChanged(nameof(DataColumnStatus));
        }
      }
    }
    private string _DataColumnTransformationText;

    public string DataColumnTransformationText
    {
      get => _DataColumnTransformationText;
      set
      {
        if (!(_DataColumnTransformationText == value))
        {
          _DataColumnTransformationText = value;
          OnPropertyChanged(nameof(DataColumnTransformationText));
        }
      }
    }
    private string _DataColumnTransformationToolTip;

    public string DataColumnTransformationToolTip
    {
      get => _DataColumnTransformationToolTip;
      set
      {
        if (!(_DataColumnTransformationToolTip == value))
        {
          _DataColumnTransformationToolTip = value;
          OnPropertyChanged(nameof(DataColumnTransformationToolTip));
        }
      }
    }

    public QuantityWithUnitGuiEnvironment SymbolSizeEnvironment => SizeEnvironment.Instance;


    private DimensionfulQuantity _symbolSizeAt0;

    public DimensionfulQuantity SymbolSizeAt0
    {
      get => _symbolSizeAt0;
      set
      {
        if (!(_symbolSizeAt0 == value))
        {
          _symbolSizeAt0 = value;
          OnPropertyChanged(nameof(SymbolSizeAt0));
        }
      }
    }

    private DimensionfulQuantity _symbolSizeAt1;

    public DimensionfulQuantity SymbolSizeAt1
    {
      get => _symbolSizeAt1;
      set
      {
        if (!(_symbolSizeAt1 == value))
        {
          _symbolSizeAt1 = value;
          OnPropertyChanged(nameof(SymbolSizeAt1));
        }
      }
    }



    private DimensionfulQuantity _symbolSizeAbove;

    public DimensionfulQuantity SymbolSizeAbove
    {
      get => _symbolSizeAbove;
      set
      {
        if (!(_symbolSizeAbove == value))
        {
          _symbolSizeAbove = value;
          OnPropertyChanged(nameof(SymbolSizeAbove));
        }
      }
    }


    private DimensionfulQuantity _symbolSizeBelow;

    public DimensionfulQuantity SymbolSizeBelow
    {
      get => _symbolSizeBelow;
      set
      {
        if (!(_symbolSizeBelow == value))
        {
          _symbolSizeBelow = value;
          OnPropertyChanged(nameof(SymbolSizeBelow));
        }
      }
    }


    private DimensionfulQuantity _symbolSizeInvalid;

    public DimensionfulQuantity SymbolSizeInvalid
    {
      get => _symbolSizeInvalid;
      set
      {
        if (!(_symbolSizeInvalid == value))
        {
          _symbolSizeInvalid = value;
          OnPropertyChanged(nameof(SymbolSizeInvalid));
        }
      }
    }


    private int _numberOfSteps;

    public int NumberOfSteps
    {
      get => _numberOfSteps;
      set
      {
        if (!(_numberOfSteps == value))
        {
          _numberOfSteps = value;
          OnPropertyChanged(nameof(NumberOfSteps));
        }
      }
    }



    #endregion
    public override void Dispose(bool isDisposing)
    {
      base.Dispose(isDisposing);
    }

    protected override void Initialize(bool initData)
    {
      base.Initialize(initData);

      if (initData)
      {
        var scaleController = new DensityScaleController(newScale => _doc.Scale = (NumericalScale)newScale);
        scaleController.InitializeDocument(_doc.Scale);
        ScaleController = scaleController;
        InitializeDataColumnText();
        SymbolSizeAt0 =     new DimensionfulQuantity(_doc.SymbolSizeAt0, Altaxo.Units.Length.Point.Instance).AsQuantityIn(SymbolSizeEnvironment.DefaultUnit);
        SymbolSizeAt1 =     new DimensionfulQuantity(_doc.SymbolSizeAt1, Altaxo.Units.Length.Point.Instance).AsQuantityIn(SymbolSizeEnvironment.DefaultUnit);
        SymbolSizeBelow =   new DimensionfulQuantity(_doc.SymbolSizeBelow, Altaxo.Units.Length.Point.Instance).AsQuantityIn(SymbolSizeEnvironment.DefaultUnit);
        SymbolSizeAbove =   new DimensionfulQuantity(_doc.SymbolSizeAbove, Altaxo.Units.Length.Point.Instance).AsQuantityIn(SymbolSizeEnvironment.DefaultUnit);
        SymbolSizeInvalid = new DimensionfulQuantity(_doc.SymbolSizeInvalid, Altaxo.Units.Length.Point.Instance).AsQuantityIn(SymbolSizeEnvironment.DefaultUnit);
        NumberOfSteps = _doc.NumberOfSteps;
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

      _doc.SymbolSizeAt0 = SymbolSizeAt0.AsValueIn(Altaxo.Units.Length.Point.Instance);
      _doc.SymbolSizeAt1 = SymbolSizeAt1.AsValueIn(Altaxo.Units.Length.Point.Instance);
      _doc.SymbolSizeBelow = SymbolSizeBelow.AsValueIn(Altaxo.Units.Length.Point.Instance);
      _doc.SymbolSizeAbove = SymbolSizeAbove.AsValueIn(Altaxo.Units.Length.Point.Instance);
      _doc.SymbolSizeInvalid = SymbolSizeInvalid.AsValueIn(Altaxo.Units.Length.Point.Instance);
      _doc.NumberOfSteps = NumberOfSteps;

      return ApplyEnd(true, disposeController);
    }

    private void InitializeDataColumnText()
    {
      var info = new PlotColumnInformation(_doc.DataColumn, _doc.DataColumnName);
      info.Update(_supposedParentDataTable, _supposedGroupNumber);

      DataColumnText = info.PlotColumnBoxText;
      DataColumnToolTip = info.PlotColumnToolTip;
      DataColumnStatus = (int)info.PlotColumnBoxState;

      DataColumnTransformationText = info.TransformationTextToShow;
      DataColumnTransformationToolTip = info.TransformationToolTip;
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
        "SymbolSize", // label to be shown
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
