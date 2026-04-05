#region Copyright

/////////////////////////////////////////////////////////////////////////////
//    Altaxo:  a data processing and data plotting program
//    Copyright (C) 2002-2026 Dr. Dirk Lellinger
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

using System.Collections.Generic;
using System.Linq;
using Altaxo.Calc.FitFunctions.Peaks;
using Altaxo.Gui.Common;

namespace Altaxo.Gui.Calc.FitFunctions.Peaks
{
  /// <summary>
  /// Defines the view contract for editing <see cref="InterpolatedPeakFunctionFrom2DTable"/>.
  /// </summary>
  public interface IInterpolatedPeakFunctionFrom2DTableView : IDataContextAwareView
  {
  }

  /// <summary>
  /// Controller for <see cref="InterpolatedPeakFunctionFrom2DTable"/>.
  /// </summary>
  [UserControllerForObject(typeof(InterpolatedPeakFunctionFrom2DTable))]
  [ExpectedTypeOfView(typeof(IInterpolatedPeakFunctionFrom2DTableView))]
  public class InterpolatedPeakFunctionFrom2DTableController : MVCANControllerEditImmutableDocBase<InterpolatedPeakFunctionFrom2DTable, IInterpolatedPeakFunctionFrom2DTableView>
  {
    /// <inheritdoc/>
    public override IEnumerable<ControllerAndSetNullMethod> GetSubControllers()
    {
      yield break;
    }

    #region Bindings


    /// <summary>
    /// Gets or sets the baseline polynomial order.
    /// </summary>
    public int OrderOfBaselinePolynominal
    {
      get => field;
      set
      {
        if (!(field == value))
        {
          field = value;
          OnPropertyChanged(nameof(OrderOfBaselinePolynominal));
        }
      }
    }


    /// <summary>
    /// Gets or sets the number of terms.
    /// </summary>
    public int NumberOfTerms
    {
      get => field;
      set
      {
        if (!(field == value))
        {
          field = value;
          OnPropertyChanged(nameof(NumberOfTerms));
        }
      }
    }


    /// <summary>
    /// Gets or sets the available table names.
    /// </summary>
    public ItemsController<string> TableNames
    {
      get => field;
      set
      {
        if (!(field == value))
        {
          field?.Dispose();
          field = value;
          OnPropertyChanged(nameof(TableNames));
        }
      }
    }


    /// <summary>
    /// Gets or sets the available group numbers of participating columns.
    /// </summary>
    public ItemsController<int> GroupNumberOfParticipatingColumns
    {
      get => field;
      set
      {
        if (!(field == value))
        {
          field?.Dispose();
          field = value;
          OnPropertyChanged(nameof(GroupNumberOfParticipatingColumns));
        }
      }
    }



    /// <summary>
    /// Gets or sets the available property names.
    /// </summary>
    public ItemsController<string> NameOfProperties
    {
      get => field;
      set
      {
        if (!(field == value))
        {
          field?.Dispose();
          field = value;
          OnPropertyChanged(nameof(NameOfProperties));
        }
      }
    }


    bool _propertyIsWidth;
    /// <summary>
    /// Gets or sets a value indicating whether the selected property represents peak width.
    /// </summary>
    public bool PropertyIsWidth
    {
      get => _propertyIsWidth;
      set
      {
        if (!(_propertyIsWidth == value))
        {
          _propertyIsWidth = value;
          OnPropertyChanged(nameof(PropertyIsWidth));
          OnPropertyChanged(nameof(PropertyIsPosition));
        }
      }
    }
    /// <summary>
    /// Gets or sets a value indicating whether the selected property represents peak position.
    /// </summary>
    public bool PropertyIsPosition
    {
      get => !_propertyIsWidth;
      set
      {
        if (!(!_propertyIsWidth == !value))
        {
          _propertyIsWidth = !value;
          OnPropertyChanged(nameof(PropertyIsWidth));
          OnPropertyChanged(nameof(PropertyIsPosition));
        }
      }
    }


    #endregion

    /// <inheritdoc/>
    protected override void Initialize(bool initData)
    {
      base.Initialize(initData);

      if (initData)
      {
        NumberOfTerms = _doc.NumberOfTerms;
        OrderOfBaselinePolynominal = _doc.OrderOfBaselinePolynomial;

        var tableNames = new List<string>(Current.Project.DataTableCollection.Names);
        tableNames.Sort();
        TableNames = new ItemsController<string>(new Collections.SelectableListNodeList(tableNames.Select(n => new Collections.SelectableListNode(n, n, false))), EhSelectedTableChanged);
        EhSelectedTableChanged(_doc.TableName);
      }
    }

    private void EhSelectedTableChanged(string newTableName)
    {
      Current.Project.DataTableCollection.TryGetValue(newTableName, out var table);

      if (table is not null && TableNames.SelectedValue != newTableName)
      {
        TableNames.SelectedValue = _doc.TableName;
      }

      if (table is not null)
      {
        var columnNames = new List<string>(table.PropCols.GetColumnNames());
        columnNames.Sort();
        NameOfProperties = new ItemsController<string>(new Collections.SelectableListNodeList(columnNames.Select(n => new Collections.SelectableListNode(n, n, false))));
        if (table.PropCols.TryGetColumn(_doc.NameOfPropertyForPeakPositionOrWidth) is not null)
        {
          NameOfProperties.SelectedValue = _doc.NameOfPropertyForPeakPositionOrWidth;
        }
        else if (columnNames.Count > 0)
        {
          NameOfProperties.SelectedValue = columnNames[0];
        }
      }

      if (table is not null)
      {
        var groupNumbers = table.DataColumns.GetGroupNumbersAll();
        GroupNumberOfParticipatingColumns = new ItemsController<int>(new Collections.SelectableListNodeList(groupNumbers.Select(n => new Collections.SelectableListNode(n.ToString(), n, false))));
        if (groupNumbers.Contains(_doc.GroupNumberOfParticipatingColumns))
        {
          GroupNumberOfParticipatingColumns.SelectedValue = _doc.GroupNumberOfParticipatingColumns;
        }
        else if (groupNumbers.Count > 0)
        {
          GroupNumberOfParticipatingColumns.SelectedValue = groupNumbers.First();
        }
      }
    }

    /// <inheritdoc/>
    public override bool Apply(bool disposeController)
    {
      _doc = _doc with
      {
        NumberOfTerms = NumberOfTerms,
        OrderOfBaselinePolynomial = OrderOfBaselinePolynominal,
        TableName = TableNames.SelectedValue,
        GroupNumberOfParticipatingColumns = GroupNumberOfParticipatingColumns.SelectedValue,
        PropertyIsPeakWidth = PropertyIsWidth,
        NameOfPropertyForPeakPositionOrWidth = NameOfProperties.SelectedValue,
      };

      return ApplyEnd(true, disposeController);
    }
  }
}

