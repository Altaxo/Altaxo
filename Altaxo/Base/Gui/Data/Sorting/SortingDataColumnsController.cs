#region Copyright

/////////////////////////////////////////////////////////////////////////////
//    Altaxo:  a data processing and data plotting program
//    Copyright (C) 2002-2025 Dr. Dirk Lellinger
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
using System.Windows.Input;
using Altaxo.Collections;
using Altaxo.Data;

namespace Altaxo.Gui.Data.Sorting
{
  /// <summary>
  /// Data model for sorting data columns by row. Or for sorting the order of data columns by property columns.
  /// </summary>
  /// <seealso cref="System.ICloneable" />
  public class SortingDataColumnsModel : ICloneable
  {
    /// <summary>
    /// Gets the columns to sort together with their sort order. The columns could be either data columns or property columns.
    /// </summary>
    public List<(DataColumn column, bool isAscendingOrder)> ColumnsToSort { get; } = [];

    /// <summary>
    /// Gets a value indicating whether the sorting columns are property columns (value == true) or data columns (value == false).
    /// </summary>
    public required bool SortingColumnsArePropertyColumns { get; init; }

    /// <summary>
    /// This property is only relevant if <see cref="SortingColumnsArePropertyColumns"/> is true.
    /// If the value is true, only data columns that match the property column group will be moved.
    /// If the value is false, all data columns will be moved.
    /// </summary>
    public bool MoveOnlyDataColumnsThatMatchPropertyColumnGroup { get; set; }

    /// <summary>
    /// Gets or sets a value indicating how to treat empty elements during sorting.
    /// If true, empty elements are treated as the lowest possible value.
    /// If false, they are treated as the highest possible value.
    /// </summary>
    public bool TreatEmptyElementsAsLowest { get; set; }

    /// <inheritdoc/>
    public object Clone()
    {
      var clone = new SortingDataColumnsModel()
      {
        SortingColumnsArePropertyColumns = this.SortingColumnsArePropertyColumns,
        MoveOnlyDataColumnsThatMatchPropertyColumnGroup = this.MoveOnlyDataColumnsThatMatchPropertyColumnGroup,
        TreatEmptyElementsAsLowest = this.TreatEmptyElementsAsLowest
      };
      foreach (var (column, isAscendingOrder) in ColumnsToSort)
      {
        clone.ColumnsToSort.Add((column, isAscendingOrder));
      }

      return clone;
    }
  }

  public interface ISortingDataColumnsView : IDataContextAwareView
  {
  }

  [ExpectedTypeOfView(typeof(ISortingDataColumnsView))]
  [UserControllerForObject(typeof(SortingDataColumnsModel))]
  public class SortingDataColumnsController : MVCANControllerEditCopyOfDocBase<SortingDataColumnsModel, ISortingDataColumnsView>
  {
    private class MyListNode : SelectableListNode
    {

      public bool IsAscendingOrder
      {
        get => field;
        set
        {
          if (!(field == value))
          {
            field = value;
            OnPropertyChanged(nameof(IsAscendingOrder));
          }
        }
      }



      public MyListNode(string displayText, (DataColumn column, bool inAscendingOrder) tag, bool isSelected)
        : base(displayText, tag, isSelected)
      {
        IsAscendingOrder = tag.inAscendingOrder;
      }
      public override string ToString()
      {
        var (column, inAscendingOrder) = ((DataColumn column, bool inAscendingOrder))Tag!;
        return $"{column.Name} ({(inAscendingOrder ? "Ascending" : "Descending")})";
      }
    }

    public ICommand CmdMoveUpSelected { get; }
    public ICommand CmdMoveDownSelected { get; }

    public SortingDataColumnsController()
    {
      CmdMoveDownSelected = new RelayCommand(EhMoveDownSelected);
      CmdMoveUpSelected = new RelayCommand(EhMoveUpSelected);
    }

    private void EhMoveUpSelected()
    {
      Columns.MoveSelectedItemsUp();
    }

    private void EhMoveDownSelected()
    {
      Columns.MoveSelectedItemsDown();
    }

    public override IEnumerable<ControllerAndSetNullMethod> GetSubControllers()
    {
      yield break;
    }


    public bool TreatEmptyElementsAsLowest
    {
      get => field;
      set
      {
        if (!(field == value))
        {
          field = value;
          OnPropertyChanged(nameof(TreatEmptyElementsAsLowest));
        }
      }
    }


    #region Bindings


    public SelectableListNodeList Columns
    {
      get => field;
      set
      {
        if (!(field == value))
        {
          field = value;
          OnPropertyChanged(nameof(Columns));
        }
      }
    }


    public bool MoveOnlyDataColumnsThatMatchPropertyColumnGroup
    {
      get => field;
      set
      {
        if (!(field == value))
        {
          field = value;
          OnPropertyChanged(nameof(MoveOnlyDataColumnsThatMatchPropertyColumnGroup));
        }
      }
    }


    public bool MoveOnlyDataColumnsThatMatchPropertyColumnGroupIsVisible
    {
      get => field;
      set
      {
        if (!(field == value))
        {
          field = value;
          OnPropertyChanged(nameof(MoveOnlyDataColumnsThatMatchPropertyColumnGroupIsVisible));
        }
      }
    }


    #endregion

    protected override void Initialize(bool initData)
    {
      base.Initialize(initData);

      if (initData)
      {
        var list = new SelectableListNodeList();
        foreach (var (column, isAscendingOrder) in _doc.ColumnsToSort)
        {
          var node = new MyListNode(column.Name, (column, isAscendingOrder), false);
          list.Add(node);
        }
        Columns = list;

        MoveOnlyDataColumnsThatMatchPropertyColumnGroupIsVisible = _doc.SortingColumnsArePropertyColumns;
        MoveOnlyDataColumnsThatMatchPropertyColumnGroup = false;
        TreatEmptyElementsAsLowest = _doc.TreatEmptyElementsAsLowest;
      }
    }


    public override bool Apply(bool disposeController)
    {
      _doc.ColumnsToSort.Clear();
      foreach (MyListNode node in Columns)
      {
        var (column, _) = ((DataColumn column, bool inAscendingOrder))node.Tag!;
        _doc.ColumnsToSort.Add((column, node.IsAscendingOrder));
      }

      _doc.MoveOnlyDataColumnsThatMatchPropertyColumnGroup = MoveOnlyDataColumnsThatMatchPropertyColumnGroup;
      _doc.TreatEmptyElementsAsLowest = TreatEmptyElementsAsLowest;

      return ApplyEnd(true, disposeController);
    }



  }
}
