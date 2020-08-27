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

namespace Altaxo.Gui.Data.Selections
{
  using Altaxo.Data;
  using Altaxo.Data.Selections;
  using Altaxo.Main.Services;
  using Collections;

  public interface IRowSelectionView
  {
    void InitRowSelections(List<RSEntry> rowSelections, SelectableListNodeList rowSelectionSimpleTypes, SelectableListNodeList rowSelectionCollectionTypes);

    void ChangeRowSelection(int idx, SelectableListNodeList rowSelectionTypes);

    event Action<int, Type> SelectionTypeChanged;

    event Action<int> CmdAddNewSelection;

    event Action<int> CmdRemoveSelection;

    event Action<int> CmdIndentSelection;

    event Action<int> CmdUnindentSelection;
  }

  public class RSEntry
  {
    public IRowSelection RowSelection { get; set; }

    public int IndentationLevel { get; set; }

    /// <summary>
    /// If true, this item is connected with the next item on the same level by intersection. If false, it is connected by union.
    /// </summary>
    public bool IsInterSection { get; set; }

    public IMVCANController DetailsController { get; set; }

    public object GuiItem { get; set; }
  }

  // The following rules will apply:
  // Wenn a new item is added, it is added at the same level as the previous item
  // If the item level of the previous item is 0, a new collection class is added at the beginning (root), and the existing and the new item get item levels of 1
  // Unindenting an item is possible only if its level is >= 2
  // Unindenting an item puts it after its parent collection, i.e. after the next item with level = originalLevel-1
  // Indenting an item is possible only if the number of items in the parent collection is >=3, or if the previous item has the new indentation level (in this case only the indentation level is increased)
  // Indenting an item will indent the next item (or if it was the last item: the previous item), too
  // Indenting an item will put the child collection at the same place at which the item previously was located
  // If the last item is deleted from its parent collection, the parent collection itself is deleted, too.
  // If only one item remains in the parent collection, the parent collection is not immediately deleted. Such simplification can be done after Applying the controller.

  [UserControllerForObject(typeof(IRowSelection), 10)]
  [ExpectedTypeOfView(typeof(IRowSelectionView))]
  public class RowSelectionController : MVCANControllerEditCopyOfDocBase<IRowSelection, IRowSelectionView>
  {
    private SelectableListNodeList _rowSelectionSimpleTypes;
    private SelectableListNodeList _rowSelectionCollectionTypes;

    private List<RSEntry> _rsEntryList;

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

    protected override void Initialize(bool initData)
    {
      base.Initialize(initData);

      if (initData)
      {
        // available row selection types
        var types = ReflectionService.GetNonAbstractSubclassesOf(typeof(IRowSelection));
        _rowSelectionSimpleTypes = new SelectableListNodeList();
        _rowSelectionCollectionTypes = new SelectableListNodeList();
        foreach (var type in types)
        {
          if (typeof(IRowSelectionCollection).IsAssignableFrom(type))
            _rowSelectionCollectionTypes.Add(new SelectableListNode(type.Name, type, false));
          else
            _rowSelectionSimpleTypes.Add(new SelectableListNode(type.Name, type, false));
        }

        // RS entries
        _rsEntryList = new List<RSEntry>();
        ConvertRowSelectionToListOfRSEntries(_doc, _rsEntryList, 0);
        AmendRSEntryListWithDetailControllers(_rsEntryList);
      }

      if (null != _view)
      {
        _view.InitRowSelections(_rsEntryList, _rowSelectionSimpleTypes, _rowSelectionCollectionTypes);
      }
    }

    /// <summary>
    /// Converts the row selection, which can be nested, to a linear list of rs entries.
    /// </summary>
    /// <param name="document">The document.</param>
    /// <param name="list">The list.</param>
    /// <param name="indentLevel">The indent level.</param>
    private static void ConvertRowSelectionToListOfRSEntries(IRowSelection document, List<RSEntry> list, int indentLevel)
    {
      if (document is IRowSelectionCollection)
      {
        list.Add(new RSEntry { IndentationLevel = indentLevel, RowSelection = document });

        foreach (var child in (IEnumerable<IRowSelection>)document)
        {
          ConvertRowSelectionToListOfRSEntries(child, list, indentLevel + 1);
        }
      }
      else if (null != document) // doc is single entity
      {
        list.Add(new RSEntry { IndentationLevel = indentLevel, RowSelection = document });
      }
      else
      {
        list.Add(new RSEntry { IndentationLevel = indentLevel, RowSelection = new AllRows() });
      }
    }

    private void AmendRSEntryListWithDetailControllers(List<RSEntry> list)
    {
      foreach (var entry in list)
      {
        if (entry.DetailsController == null)
        {
          var controller = (IMVCANController)Current.Gui.GetController(new object[] { entry.RowSelection, _supposedParentDataTable }, typeof(IMVCANController));
          if (controller is RowSelectionController)
            controller = null;

          if (null != controller)
            Current.Gui.FindAndAttachControlTo(controller);

          entry.DetailsController = controller;
        }
      }
    }

    private void AmendRSEntryListWithDetailControllers(List<RSEntry> list, int idx)
    {
      var entry = list[idx];
      {
        if (entry.DetailsController != null)
        {
          entry.DetailsController.Dispose();
          entry.DetailsController = null;
        }

        if (entry.DetailsController == null)
        {
          var controller = (IMVCANController)Current.Gui.GetController(new object[] { entry.RowSelection, _supposedParentDataTable }, typeof(IMVCANController));
          if (controller is RowSelectionController)
            controller = null;

          if (null != controller)
            Current.Gui.FindAndAttachControlTo(controller);

          entry.DetailsController = controller;
        }
      }
    }

    /// <summary>
    /// Applies all controllers, and stores the updated row selection entries in the list. But the document tree is not build here.
    /// </summary>
    /// <param name="disposeController">if set to <c>true</c> [dispose controller].</param>
    /// <returns></returns>
    private bool ApplyAllControllers(bool disposeController)
    {
      for (int i = 0; i < _rsEntryList.Count; ++i)
      {
        var ctrl = _rsEntryList[i].DetailsController;
        if (null == ctrl)
          continue;

        bool result = _rsEntryList[i].DetailsController.Apply(disposeController);

        if (false == result)
          return result;
        else
          _rsEntryList[i].RowSelection = (IRowSelection)ctrl.ModelObject;
      }

      return true;
    }

    private static IRowSelection ConvertListOfRSEntriesToRowSelection(List<RSEntry> list, int startIndex)
    {
      if (list[startIndex].RowSelection is IRowSelectionCollection)
      {
        // search for all items with a indentation level which is 1 higher, but only up to the point where the indentation level falls below this value

        var oldCollection = list[startIndex].RowSelection as IRowSelectionCollection;
        int startIndentationLevel = list[startIndex].IndentationLevel;

        var childItems = new List<IRowSelection>();

        for (int i = startIndex + 1; i < list.Count; ++i)
        {
          if (list[i].IndentationLevel <= startIndentationLevel)
            break;

          if (list[i].IndentationLevel != startIndentationLevel + 1)
            continue;

          var childItem = ConvertListOfRSEntriesToRowSelection(list, i);
          childItems.Add(childItem);
        }

        return oldCollection.NewWithItems(childItems);
      }
      else
      {
        return list[startIndex].RowSelection;
      }
    }

    public override bool Apply(bool disposeController)
    {
      if (!ApplyAllControllers(disposeController))
        return ApplyEnd(false, disposeController);

      _doc = ConvertListOfRSEntriesToRowSelection(_rsEntryList, 0);

      return ApplyEnd(true, disposeController);
    }

    protected override void AttachView()
    {
      base.AttachView();

      _view.SelectionTypeChanged += EhSelectionTypeChanged;

      _view.CmdAddNewSelection += EhCmdAddNewSelection;

      _view.CmdRemoveSelection += EhCmdRemoveSelection;

      _view.CmdIndentSelection += EhCmdIndentSelection;

      _view.CmdUnindentSelection += EhCmdUnindentSelection;
    }

    protected override void DetachView()
    {
      _view.SelectionTypeChanged -= EhSelectionTypeChanged;

      _view.CmdAddNewSelection -= EhCmdAddNewSelection;

      _view.CmdRemoveSelection -= EhCmdRemoveSelection;

      _view.CmdIndentSelection -= EhCmdIndentSelection;

      _view.CmdUnindentSelection -= EhCmdUnindentSelection;

      base.DetachView();
    }

    private void EhCmdUnindentSelection(int idx)
    {
      if (0 == idx) // item 0 can never be indented
        return;

      var rsEntry = _rsEntryList[idx];

      if (!(rsEntry.IndentationLevel >= 2))
        return; // we can unindent only if level >= 2;

      int parentIndex = GetParentIndex(idx);
      int? nextParentIdx = GetNextSibling(idx);

      rsEntry.IndentationLevel--; // unindent item

      if (nextParentIdx.HasValue)
      {
        _rsEntryList.Insert(nextParentIdx.Value, rsEntry);
      }
      else
      {
        _rsEntryList.Add(rsEntry);
      }

      _rsEntryList.RemoveAt(idx);

      _view.InitRowSelections(_rsEntryList, _rowSelectionSimpleTypes, _rowSelectionCollectionTypes);

      OnItemsChanged();
    }

    private void EhCmdIndentSelection(int idx)
    {
      if (0 == idx) // item 0 can never be indented
        return;

      int startLevel = _rsEntryList[idx].IndentationLevel;

      if (_rsEntryList[idx - 1].IndentationLevel == (startLevel + 1)) // if prev items has already the new indentation level, then we simply bring this item to the same indentation level, too
      {
        _rsEntryList[idx].IndentationLevel++;
        _view.ChangeRowSelection(idx, (_rsEntryList[idx].RowSelection is IRowSelectionCollection) ? _rowSelectionCollectionTypes : _rowSelectionSimpleTypes);
        return;
      }

      // nothing of the above.
      // thus we must use at least two items, and put a new collection item before

      int parentIndex = GetParentIndex(idx);
      int numberOfChilds = GetNumberOfChilds(parentIndex);

      if (numberOfChilds < 3)
        return; // we need at least 3 childs, of which 2 childs are then indented

      int? sibling = GetNextSibling(idx);

      int idxSibling;

      if (sibling.HasValue)
      {
        idxSibling = sibling.Value;
      }
      else // we have to search in the previous items, we also exchange the order so that idx is < idxSibling
      {
        idxSibling = idx;
        idx = GetPreviousSibling(idx).Value;
      }

      if (!(idx < idxSibling))
        throw new InvalidProgramException();

      IncreaseIndentationForThisAndChilds(idx);
      IncreaseIndentationForThisAndChilds(idxSibling);
      _rsEntryList.Insert(idx, new RSEntry { IndentationLevel = _rsEntryList[parentIndex].IndentationLevel + 1, RowSelection = new IntersectionOfRowSelections() });
      AmendRSEntryListWithDetailControllers(_rsEntryList, idx);
      _view.InitRowSelections(_rsEntryList, _rowSelectionSimpleTypes, _rowSelectionCollectionTypes);

      OnItemsChanged();
    }

    private void EhCmdRemoveSelection(int idx)
    {
      int startIndentationLevel = _rsEntryList[idx].IndentationLevel;
      int lastIndex = _rsEntryList.Count;
      int parentIdx = idx == 0 ? -1 : GetParentIndex(idx);

      // search for the last index with a higher level -> all this items are childs of our node
      for (int i = idx + 1; i < _rsEntryList.Count; ++i)
      {
        if (_rsEntryList[i].IndentationLevel <= startIndentationLevel)
        {
          lastIndex = i;
          break;
        }
      }

      // remove all the childs now
      for (int i = lastIndex - 1; i >= idx; --i)
      {
        _rsEntryList[i].DetailsController?.Dispose();
        _rsEntryList.RemoveAt(i);
      }

      // if there is only one child left, set it instead of the parent collection
      if (parentIdx >= 0 && parentIdx <= _rsEntryList.Count)
      {
        int numberOfChilds = GetNumberOfChilds(parentIdx);
        if (1 == numberOfChilds && ((IRowSelectionCollection)_rsEntryList[parentIdx].RowSelection).IsCollectionWithOneItemEquivalentToThisItem)
        {
          _rsEntryList[parentIdx + 1].IndentationLevel--; // unindent the only child item
          _rsEntryList.RemoveAt(parentIdx); // remove parent item
        }
        else if (0 == numberOfChilds)
        {
          _rsEntryList.RemoveAt(parentIdx);
        }
      }

      if (_rsEntryList.Count == 0) // in case we have deleted everything
      {
        _rsEntryList.Add(new RSEntry() { IndentationLevel = 0, RowSelection = new AllRows() });
        AmendRSEntryListWithDetailControllers(_rsEntryList);
      }

      _view.InitRowSelections(_rsEntryList, _rowSelectionSimpleTypes, _rowSelectionCollectionTypes);

      OnItemsChanged();
    }

    private void EhCmdAddNewSelection(int idx)
    {
      var level = _rsEntryList[idx].IndentationLevel;
      if (0 == idx && !(_rsEntryList[idx].RowSelection is IRowSelectionCollection)) // if there is a simple item at index 0
      {
        _rsEntryList.Insert(0, new RSEntry() { IndentationLevel = 0, RowSelection = new IntersectionOfRowSelections() }); // add collection at index 0
        level = 1;
        idx = 1;
        _rsEntryList[1].IndentationLevel = level; // increase Indentation level of simple item
      }
      else if (_rsEntryList[idx].RowSelection is IRowSelectionCollection)
      {
        ++level;
      }

      _rsEntryList.Insert(idx + 1, new RSEntry() { IndentationLevel = level, RowSelection = new AllRows() }); // insert the new item just after the clicked item

      AmendRSEntryListWithDetailControllers(_rsEntryList);
      _view.InitRowSelections(_rsEntryList, _rowSelectionSimpleTypes, _rowSelectionCollectionTypes);

      OnItemsChanged();
    }

    private void EhSelectionTypeChanged(int idx, Type type)
    {
      // Create an item of the new type
      var newSel = (IRowSelection)Activator.CreateInstance(type);

      _rsEntryList[idx].RowSelection = newSel;
      AmendRSEntryListWithDetailControllers(_rsEntryList, idx);
      _view.ChangeRowSelection(idx, (_rsEntryList[idx].RowSelection is IRowSelectionCollection) ? _rowSelectionCollectionTypes : _rowSelectionSimpleTypes);

      OnItemsChanged();
    }

    private int GetParentIndex(int idx)
    {
      var level = _rsEntryList[idx].IndentationLevel;
      if (level == 0)
        throw new InvalidOperationException();

      for (int i = idx - 1; i >= 0; --i)
        if (_rsEntryList[i].IndentationLevel < level)
          return i;

      throw new InvalidProgramException();
    }

    private int GetNumberOfChilds(int idx)
    {
      if (!(_rsEntryList[idx].RowSelection is IRowSelectionCollection))
        throw new InvalidProgramException();

      var startLevel = _rsEntryList[idx].IndentationLevel;

      int childCount = 0;
      for (int i = idx + 1; i < _rsEntryList.Count; ++i)
      {
        if (_rsEntryList[i].IndentationLevel <= startLevel)
          break;

        if (_rsEntryList[i].IndentationLevel == (startLevel + 1))
          ++childCount;
      }

      return childCount;
    }

    private int? GetNextSibling(int idx)
    {
      var startLevel = _rsEntryList[idx].IndentationLevel;

      for (int i = idx + 1; i < _rsEntryList.Count; ++i)
      {
        if (_rsEntryList[i].IndentationLevel < startLevel)
          return null; // no sibling found

        if (_rsEntryList[i].IndentationLevel == startLevel)
          return i;
      }

      return null;
    }

    private int? GetPreviousSibling(int idx)
    {
      var startLevel = _rsEntryList[idx].IndentationLevel;

      for (int i = idx - 1; i >= 0; --i)
      {
        if (_rsEntryList[i].IndentationLevel < startLevel)
          return null; // no sibling found

        if (_rsEntryList[i].IndentationLevel == startLevel)
          return i;
      }

      return null;
    }

    private void IncreaseIndentationForThisAndChilds(int idx)
    {
      var startLevel = _rsEntryList[idx].IndentationLevel;

      _rsEntryList[idx].IndentationLevel++;

      for (int i = idx + 1; i < _rsEntryList.Count; ++i)
      {
        if (_rsEntryList[i].IndentationLevel <= startLevel)
          break;

        _rsEntryList[i].IndentationLevel++;
      }
    }

    /// <summary>
    /// Gets the additional columns used by the current row selection
    /// </summary>
    /// <returns>
    /// Each tuple in this enumeration consist of
    /// (i) the label under which the column is announced in the view (first item),
    /// (ii) the column itself,
    /// (iii) the name of the column (only if it is a data column; otherwise empty)
    /// (iiii) an action to set the column if a value has been assigned to, or if the column has changed.
    /// </returns>
    public IEnumerable<Tuple<string, IReadableColumn, string, Action<IReadableColumn, DataTable, int>>> GetAdditionalColumns()
    {
      for (int i = 0; i < _rsEntryList.Count; ++i)
      {
        var rsEntry = _rsEntryList[i];
        var rowSel = rsEntry.RowSelection;
        if (rsEntry.DetailsController is IDataColumnController)
        {
          var controller = rsEntry.DetailsController as IDataColumnController;
          controller.SetIndex(i);

          yield return new Tuple<string, IReadableColumn, string, Action<IReadableColumn, DataTable, int>>(
            "Col#" + i.ToString(),
            controller.Column,
            controller.ColumnName,
            (column, table, group) =>
            {
              _supposedParentDataTable = table;
              _supposedGroupNumber = group;
              controller.SetDataColumn(column, table, group);
            }
            );
        }
      }
    }

    public DataTable SupposedParentDataTable { set { _supposedParentDataTable = value; } }

    public event Action ItemsChanged;

    protected void OnItemsChanged()
    {
      ItemsChanged?.Invoke();
    }
  }
}
