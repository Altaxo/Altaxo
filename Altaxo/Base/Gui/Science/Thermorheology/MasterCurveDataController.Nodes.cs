#region Copyright

/////////////////////////////////////////////////////////////////////////////
//    Altaxo:  a data processing and data plotting program
//    Copyright (C) 2024 Dr. Dirk Lellinger
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
using Altaxo.Collections;
using Altaxo.Data;
using Altaxo.Science.Thermorheology.MasterCurves;

namespace Altaxo.Gui.Science.Thermorheology
{
  public partial class MasterCurveDataController : MVCANControllerEditCopyOfDocBase<MasterCurveData, IMasterCurveDataView>
  {
    List<SelectableListNodeList> _dataNodes = [];

    /// <summary>
    /// Creates a new empty Gui node (i.e. without an x-y curve).
    /// </summary>
    MyNode NewEmptyGuiNode => new MyNode("---", null, false);

    /// <summary>
    /// Creates new Gui node that represents an x-y-curve.
    /// </summary>
    /// <param name="xycol">The x-y-curve.</param>
    /// <returns>New node for use in the Gui.</returns>
    MyNode NewGuiNode(XAndYColumn xycol) => new MyNode(xycol?.GetName(0x21) ?? string.Empty, xycol, false);

    /// <summary>
    /// Initializes the Gui usable list of the data items <see cref="_dataNodes"/> from the document's items
    /// </summary>
    /// <param name="_doc">The document.</param>
    void InitializeGuiNodesFromDocument(MasterCurveData _doc)
    {
      if (_dataNodes.Count > 0)
        throw new NotImplementedException("We assumed that the list is empty (otherwise disposal of is neccessary)");


      int numberOfGroups = _doc.CurveData.Count;
      int numberOfItems = numberOfGroups == 0 ? 0 : _doc.CurveData.Max(x => x.Length);

      _dataNodes = Enumerable.Range(0, numberOfGroups)
                    .Select(i => new SelectableListNodeList(
                      Enumerable.Range(0, numberOfItems)
                        .Select(j =>
                          {
                            var xycol = _doc.CurveData[i][j];
                            return xycol is not null ? NewGuiNode(xycol) : NewEmptyGuiNode;
                          })
                   )).ToList();
    }

    /// <summary>
    /// Ensures that the number of items in each group of the Gui list is at least the provided number.
    /// </summary>
    /// <param name="numberOfItems">The number of items.</param>
    void EnsureNumberOfItemsInGuiListIsAtLeast(int numberOfItems)
    {
      foreach (var sublist in _dataNodes)
      {
        for (int j = sublist.Count; j < numberOfItems; ++j)
          sublist.Add(NewEmptyGuiNode);
      }
    }

    /// <summary>
    /// Ensures that the number of groups in the Gui list is exactly the provided number.
    /// </summary>
    /// <param name="numberOfGroups">The number of groups.</param>
    void EnsureNumberOfGroupsInGuiListIsExactly(int numberOfGroups)
    {
      if (numberOfGroups > _dataNodes.Count)
      {
        var numberOfItems = _dataNodes.Count == 0 ? 0 : _dataNodes.Max(x => x.Count);
        for (int i = _dataNodes.Count; i < numberOfGroups; ++i)
        {
          var sublist = new SelectableListNodeList(Enumerable.Range(0, numberOfItems).Select(j => NewEmptyGuiNode));
          _dataNodes.Add(sublist);
        }
      }
      else if (numberOfGroups < _dataNodes.Count)
      {
        for (int i = _dataNodes.Count - 1; i >= numberOfGroups; --i)
        {
          var sublist = _dataNodes[i];
          _dataNodes.RemoveAt(i);
          sublist.ForEachDo(x => ((MyNode)x).Curve?.Dispose());
        }
      }
    }

    /// <summary>
    /// Appends and curve item to the Gui list.
    /// </summary>
    /// <param name="curves">The curves to add.</param>
    /// <param name="groupNumber">The group number to which to append.</param>
    /// <param name="toLast">If false, the item is put immediately before the first selected node (or at the top of the list).
    /// If true, the item is put immediately after the last selected node (or at the end of the list).</param>
    void AddItemsToGuiList(IReadOnlyList<XAndYColumn> curves, int groupNumber, bool toLast)
    {
      int maxListIndexFilled = _dataNodes.Count == 0 ? 0 : _dataNodes.Max(x => x.IndexOfLast((node, j) => node.Tag is not null));
      int insertPosition;
      if (toLast)
      {
        var lastIdx = _dataNodes[groupNumber].IndexOfLast((node, i) => node.IsSelected);
        insertPosition = lastIdx < 0 ? maxListIndexFilled + 1 : lastIdx + 1;
      }
      else
      {
        insertPosition = Math.Max(0, _dataNodes[groupNumber].IndexOfFirst((node, i) => node.IsSelected));
      }

      AddItemsToGuiList(curves, groupNumber, insertPosition);
    }

    /// <summary>
    /// Appends and curve item to the Gui list.
    /// </summary>
    /// <param name="curves">The curves to add.</param>
    /// <param name="groupNumber">The group number to which to append.</param>
    /// <param name="insertPosition">The index where to insert the items.</param>
    private void AddItemsToGuiList(IReadOnlyList<XAndYColumn> curves, int groupNumber, int insertPosition)
    {
      // Insert the curve nodes into the current group
      for (int i = 0; i < curves.Count; i++)
      {
        _dataNodes[groupNumber].Insert(insertPosition + i, NewGuiNode(curves[i]));
      }

      // Now insert the same amout of empty curve nodes into the other groups
      for (int idxGroup = 0; idxGroup < _dataNodes.Count; ++idxGroup)
      {
        if (idxGroup != groupNumber)
        {
          for (int i = 0; i < curves.Count; i++)
          {
            _dataNodes[groupNumber].Insert(insertPosition + i, NewEmptyGuiNode);
          }
        }
      }

      StartTaskUpdateAllGuiNodesWithProperties();
    }

    #region Properties

    void UpdateGuiNodeWithProperties(MyNode node, int idx)
    {
      object? property1 = null, property2 = null;

      if (node.Curve is { } curve)
      {
        (property1, property2) = MasterCurveCreationEx.GetPropertiesOfCurve(curve, _property1Name, _property2Name);
      }

      node.Index = idx;
      node.Property1 = property1;
      node.Property2 = property2;
    }

    DataColumn? GetRootDataColumn(IReadableColumn? column)
    {
      while (column is TransformedReadableColumn trc)
      {
        column = trc.UnderlyingReadableColumn;
      }
      return column as DataColumn;
    }

    void UpdateAllGuiNodesWithProperties()
    {
      foreach (var sublist in _dataNodes)
      {
        int idx = 0;
        foreach (MyNode node in sublist)
        {
          UpdateGuiNodeWithProperties(node, idx++);
        }
      }
    }

    public void PlotItems_MoveUpSelected()
    {
      var sublist = DataItems.Items;
      sublist.MoveSelectedItemsUp();
      StartTaskUpdateAllGuiNodesWithProperties();
    }

    public void PlotItems_MoveDownSelected()
    {
      var sublist = DataItems.Items;
      sublist.MoveSelectedItemsDown();
      StartTaskUpdateAllGuiNodesWithProperties();
    }

    #endregion

    class MyNode : SelectableListNode
    {
      private object? _Property1;

      public object? Property1
      {
        get => _Property1;
        set
        {
          if (!(_Property1 == value))
          {
            _Property1 = value;
            OnPropertyChanged(nameof(Property1));
            OnPropertyChanged(nameof(Text1));
          }
        }
      }

      private object? _Property2;

      public object? Property2
      {
        get => _Property2;
        set
        {
          if (!(_Property2 == value))
          {
            _Property2 = value;
            OnPropertyChanged(nameof(Property2));
            OnPropertyChanged(nameof(Text2));
          }
        }
      }



      int _index;
      public int Index
      {
        get { return _index; }
        set
        {
          if (!(_index == value))
          {
            _index = value;
            OnPropertyChanged(nameof(Index));
            OnPropertyChanged(nameof(Text0));
          }
        }
      }
      public XAndYColumn? Curve => (XAndYColumn?)Tag;

      public MyNode(string text, XAndYColumn? curve, bool isSelected) : base(text, curve, isSelected) { }

      public override string? SubItemText(int i)
      {
        return i switch
        {
          0 => Index.ToString(),
          1 => Property1?.ToString() ?? string.Empty,
          2 => Property2?.ToString() ?? string.Empty,
          _ => null,
        };
      }

      public void UpdateName()
      {
        Text = Tag is XAndYColumn xycolumn ? xycolumn.GetName(0x21) : string.Empty;
      }
    }

  }
}
