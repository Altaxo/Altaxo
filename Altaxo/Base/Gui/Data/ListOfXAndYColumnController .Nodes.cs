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

namespace Altaxo.Gui.Data
{
  public partial class ListOfXAndYColumnController
  {
    private SelectableListNodeList _dataNodes = [];

    /// <summary>
    /// Creates a new empty Gui node (i.e. without an x-y curve).
    /// </summary>
    private MyNode NewEmptyGuiNode => new MyNode("---", null, false);

    /// <summary>
    /// Creates new Gui node that represents an x-y-curve.
    /// </summary>
    /// <param name="xycol">The x-y-curve.</param>
    /// <returns>New node for use in the Gui.</returns>
    private MyNode NewGuiNode(XAndYColumn xycol) => new MyNode(xycol?.GetName(0x21) ?? string.Empty, xycol, false);

    /// <summary>
    /// Initializes the Gui usable list of the data items <see cref="_dataNodes"/> from the document's items
    /// </summary>
    /// <param name="_doc">The document.</param>
    private void InitializeGuiNodesFromDocument(ListOfXAndYColumn _doc)
    {
      if (_dataNodes.Count > 0)
        throw new NotImplementedException("We assumed that the list is empty (otherwise disposal of is neccessary)");


      int numberOfItems = _doc.CurveData.Count;

      _dataNodes = new SelectableListNodeList(Enumerable.Range(0, numberOfItems)
                        .Select(j =>
                          {
                            var xycol = _doc.CurveData[j];
                            return xycol is not null ? NewGuiNode(xycol) : NewEmptyGuiNode;
                          }));
    }





    /// <summary>
    /// Appends and curve item to the Gui list.
    /// </summary>
    /// <param name="curves">The curves to add.</param>
    /// <param name="groupNumber">The group number to which to append.</param>
    /// <param name="toLast">If false, the item is put immediately before the first selected node (or at the top of the list).
    /// If true, the item is put immediately after the last selected node (or at the end of the list).</param>
    private void AddItemsToGuiList(IReadOnlyList<XAndYColumn> curves, bool toLast)
    {
      int maxListIndexFilled = _dataNodes.Count - 1;
      int insertPosition;
      if (toLast)
      {
        var lastIdx = _dataNodes.IndexOfLast((node, i) => node.IsSelected);
        insertPosition = lastIdx < 0 ? maxListIndexFilled + 1 : lastIdx + 1;
      }
      else
      {
        insertPosition = Math.Max(0, _dataNodes.IndexOfFirst((node, i) => node.IsSelected));
      }

      AddItemsToGuiList(curves, insertPosition);
    }

    /// <summary>
    /// Appends and curve item to the Gui list.
    /// </summary>
    /// <param name="curves">The curves to add.</param>
    /// <param name="insertPosition">The index where to insert the items.</param>
    private void AddItemsToGuiList(IReadOnlyList<XAndYColumn> curves, int insertPosition)
    {
      // Insert the curve nodes into the current group
      for (int i = 0; i < curves.Count; i++)
      {
        _dataNodes.Insert(insertPosition + i, NewGuiNode(curves[i]));
      }
    }

    #region Properties



    private DataColumn? GetRootDataColumn(IReadableColumn? column)
    {
      while (column is TransformedReadableColumn trc)
      {
        column = trc.UnderlyingReadableColumn;
      }
      return column as DataColumn;
    }


    public void PlotItems_MoveUpSelected()
    {
      var sublist = DataItems.Items;
      sublist.MoveSelectedItemsUp();
    }

    public void PlotItems_MoveDownSelected()
    {
      var sublist = DataItems.Items;
      sublist.MoveSelectedItemsDown();
    }

    #endregion

    private class MyNode : SelectableListNode
    {
      private int _index;
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
