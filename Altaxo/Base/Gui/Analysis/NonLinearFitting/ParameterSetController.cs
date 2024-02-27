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

#nullable disable
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Text;
using Altaxo.Calc.Regression.Nonlinear;
using Altaxo.Collections;

namespace Altaxo.Gui.Analysis.NonLinearFitting
{
  public class ParameterSetViewItem : INotifyPropertyChanged
  {
    private double _value;

    public string Name { get; set; }

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

    private bool _vary;

    public bool Vary
    {
      get => _vary;
      set
      {
        if (!(_vary == value))
        {
          _vary = value;
          OnPropertyChanged(nameof(Vary));
        }
      }
    }


    private double _variance;

    public double Variance
    {
      get => _variance;
      set
      {
        if (!(_variance == value))
        {
          _variance = value;
          OnPropertyChanged(nameof(Variance));
        }
      }
    }


    private double? _lowerBound;

    public double? LowerBound
    {
      get => _lowerBound;
      set
      {
        if (!(_lowerBound == value))
        {
          _lowerBound = value;
          OnPropertyChanged(nameof(LowerBound));
        }
      }
    }

    private double? _upperBound;

    public double? UpperBound
    {
      get => _upperBound;
      set
      {
        if (!(_upperBound == value))
        {
          _upperBound = value;
          OnPropertyChanged(nameof(UpperBound));
        }
      }
    }

    private bool _isLowerBoundExclusive;

    public bool IsLowerBoundExclusive
    {
      get => _isLowerBoundExclusive;
      set
      {
        if (!(_isLowerBoundExclusive == value))
        {
          _isLowerBoundExclusive = value;
          OnPropertyChanged(nameof(IsLowerBoundExclusive));
        }
      }
    }

    private bool _isUpperBoundExclusive;

    public bool IsUpperBoundExclusive
    {
      get => _isUpperBoundExclusive;
      set
      {
        if (!(_isUpperBoundExclusive == value))
        {
          _isUpperBoundExclusive = value;
          OnPropertyChanged(nameof(IsUpperBoundExclusive));
        }
      }
    }

    public event PropertyChangedEventHandler PropertyChanged;
    protected virtual void OnPropertyChanged(string propertyName) => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
  }

  public interface IParameterSetView : IDataContextAwareView
  {
  }



  /// <summary>
  /// Summary description for ParameterSetController.
  /// </summary>
  [UserControllerForObject(typeof(ParameterSet))]
  [ExpectedTypeOfView(typeof(IParameterSetView))]
  public class ParameterSetController : MVCANControllerEditOriginalDocBase<ParameterSet, IParameterSetView>
  {
    public ObservableCollection<ParameterSetViewItem> ParameterList { get; } = new ObservableCollection<ParameterSetViewItem>();

    public SelectableListNodeList LowerBoundConditions { get; } = new SelectableListNodeList {
      new SelectableListNode(">=", false, false),
      new SelectableListNode(">", true, false),
      };

    public SelectableListNodeList UpperBoundConditions { get; } = new SelectableListNodeList {
      new SelectableListNode("<=", false, false),
      new SelectableListNode("<", true, false),
      };


    public override IEnumerable<ControllerAndSetNullMethod> GetSubControllers()
    {
      yield break;
    }

    public void OnParametersChanged()
    {
      ParameterList.Clear();

      for (int i = 0; i < _doc.Count; i++)
      {
        var item = new ParameterSetViewItem
        {
          Name = _doc[i].Name,
          Value = _doc[i].Parameter,
          Vary = _doc[i].Vary,
          Variance = _doc[i].Variance,
          LowerBound = _doc[i].LowerBound,
          UpperBound = _doc[i].UpperBound,
          IsLowerBoundExclusive = _doc[i].IsLowerBoundExclusive,
          IsUpperBoundExclusive = _doc[i].IsUpperBoundExclusive,
        };

        ParameterList.Add(item);
      }
    }

    protected override void Initialize(bool initData)
    {
      base.Initialize(initData);

      OnParametersChanged();
    }

    protected override void AttachView()
    {
      base.AttachView();
      _view.DataContext = this;
    }

    protected override void DetachView()
    {
      _view.DataContext = null;
      base.DetachView();
    }


    public override bool Apply(bool disposeController)
    {
      var list = ParameterList;

      for (int i = 0; i < _doc.Count; i++)
      {
        var l = list[i];

        // Parameter
        _doc[i] = new ParameterSetElement(l.Name, l.Value, l.Variance, l.Vary, l.LowerBound, l.IsLowerBoundExclusive, l.UpperBound, l.IsUpperBoundExclusive);
      }

      return ApplyEnd(true, disposeController);
    }

    public void EhPasteParameterValues()
    {
      Altaxo.Data.DataTable table = Altaxo.Worksheet.Commands.EditCommands.GetTableFromClipboard();
      if (table is null)
        return;
      Altaxo.Data.DoubleColumn col = null;
      // Find the first column that contains numeric values
      for (int i = 0; i < table.DataColumnCount; i++)
      {
        if (table[i] is Altaxo.Data.DoubleColumn)
        {
          col = table[i] as Altaxo.Data.DoubleColumn;
          break;
        }
      }
      if (col is null)
        return;

      int len = Math.Max(col.Count, _doc.Count);
      for (int i = 0; i < len; i++)
        _doc[i] = _doc[i] with { Parameter = col[i] };

      InitializeDocument(_doc);
    }

    public void EhCopyParameterValues()
    {
      if (true == Apply(false))
      {
        var dao = Current.Gui.GetNewClipboardDataObject();
        var col = new Altaxo.Data.DoubleColumn();
        for (int i = 0; i < _doc.Count; i++)
          col[i] = _doc[i].Parameter;

        var tb = new Altaxo.Data.DataTable();
        tb.DataColumns.Add(col, "Value", Altaxo.Data.ColumnKind.V, 0);
        Altaxo.Worksheet.Commands.EditCommands.WriteAsciiToClipBoardIfDataCellsSelected(
            tb, new Altaxo.Collections.AscendingIntegerCollection(),
            new Altaxo.Collections.AscendingIntegerCollection(),
            new Altaxo.Collections.AscendingIntegerCollection(),
            dao);
        Current.Gui.SetClipboardDataObject(dao, true);
      }
      else
      {
        Current.Gui.ErrorMessageBox("Some of your parameter input is not valid!");
      }
    }

    public void EhCopyParameterVAsCDef()
    {
      if (true == Apply(false))
      {
        var dao = Current.Gui.GetNewClipboardDataObject();
        var stb = new System.Text.StringBuilder();
        for (int i = 0; i < _doc.Count; i++)
        {
          stb.AppendFormat(System.Globalization.CultureInfo.InvariantCulture, "double {0} = {1};\r\n", _doc[i].Name, _doc[i].Parameter);
        }
        dao.SetData(typeof(string), stb.ToString());
        Current.Gui.SetClipboardDataObject(dao, true);
      }
      else
      {
        Current.Gui.ErrorMessageBox("Some of your parameter input is not valid!");
      }
    }

    public void EhCopyParameterNV()
    {
      if (true == Apply(false))
      {
        var dao = Current.Gui.GetNewClipboardDataObject();
        var txt = new Altaxo.Data.TextColumn();
        var col = new Altaxo.Data.DoubleColumn();

        for (int i = 0; i < _doc.Count; i++)
        {
          txt[i] = _doc[i].Name;
          col[i] = _doc[i].Parameter;
        }

        var tb = new Altaxo.Data.DataTable();
        tb.DataColumns.Add(txt, "Name", Altaxo.Data.ColumnKind.V, 0);
        tb.DataColumns.Add(col, "Value", Altaxo.Data.ColumnKind.V, 0);
        Altaxo.Worksheet.Commands.EditCommands.WriteAsciiToClipBoardIfDataCellsSelected(
            tb, new Altaxo.Collections.AscendingIntegerCollection(),
            new Altaxo.Collections.AscendingIntegerCollection(),
            new Altaxo.Collections.AscendingIntegerCollection(),
            dao);
        Current.Gui.SetClipboardDataObject(dao, true);
      }
      else
      {
        Current.Gui.ErrorMessageBox("Some of your parameter input is not valid!");
      }
    }

    public void EhCopyParameterNVV()
    {
      if (true == Apply(false))
      {
        var dao = Current.Gui.GetNewClipboardDataObject();
        var txt = new Altaxo.Data.TextColumn();
        var col = new Altaxo.Data.DoubleColumn();
        var var = new Altaxo.Data.DoubleColumn();

        for (int i = 0; i < _doc.Count; i++)
        {
          txt[i] = _doc[i].Name;
          col[i] = _doc[i].Parameter;
          var[i] = _doc[i].Variance;
        }

        var tb = new Altaxo.Data.DataTable();
        tb.DataColumns.Add(txt, "Name", Altaxo.Data.ColumnKind.V, 0);
        tb.DataColumns.Add(col, "Value", Altaxo.Data.ColumnKind.V, 0);
        tb.DataColumns.Add(var, "Variance", Altaxo.Data.ColumnKind.V, 0);
        Altaxo.Worksheet.Commands.EditCommands.WriteAsciiToClipBoardIfDataCellsSelected(
            tb, new Altaxo.Collections.AscendingIntegerCollection(),
            new Altaxo.Collections.AscendingIntegerCollection(),
            new Altaxo.Collections.AscendingIntegerCollection(),
            dao);
        Current.Gui.SetClipboardDataObject(dao, true);
      }
      else
      {
        Current.Gui.ErrorMessageBox("Some of your parameter input is not valid!");
      }
    }

    /// <summary>
    /// Tests the values of parameters and boundaries for inconsistencies, and corrects them.
    /// </summary>
    /// <param name="parameters">The parameters.</param>
    /// <returns>A <see cref="StringBuilder"/> containing warnings and errors, and a flag indicating if any inconsistence could not be corrected automatically.</returns>
    public static (StringBuilder Message, bool isFatal) TestAndCorrectParametersAndBoundaries(ParameterSet parameters)
    {
      var stb = new StringBuilder();
      bool isFatal = false;

      for (int i = 0; i < parameters.Count; i++)
      {
        parameters[i] = parameters[i].TestAndCorrectParameterAndBoundaries(stb, ref isFatal);
      }
      return (stb, isFatal);
    }
  }
}
