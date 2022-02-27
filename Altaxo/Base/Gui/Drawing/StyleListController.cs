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
using System.Windows.Input;
using Altaxo.Collections;
using Altaxo.Drawing;
using Altaxo.Gui.Common;
using Altaxo.Main;

namespace Altaxo.Gui.Drawing
{
  public interface IStyleListView : IDataContextAwareView
  {
  }

  /// <summary>
  /// Non-generic interface used as model for the Gui.
  /// </summary>
  public interface IStyleListController
  {
    public NGTreeNodeCollection AvailableLists { get; }

    public NGTreeNodeCollection AvailableItems { get; }

    SelectableListNodeList CurrentItems { get; }

    string NewListNameText { get; set; }

    string NewListNameToolTip { get; set; }

    bool NewListNameIsEnabled { get; set; }

    bool NewListNameIsMarked { get; set; }

    bool StoreInUserSettings { get; set; }

    ICommand CmdCurrentList_Store { get; }

    ICommand CmdAvailableItem_AddToCurrent { get; }


    ICommand CmdCurrentItem_Remove { get; }

    ICommand CmdCurrentItem_MoveUp { get; }

    ICommand CmdCurrentItem_MoveDown { get; }


    ICommand CmdCurrentItem_Edit { get; }

    IMVVMDragDropHandler AvailableItemsDragDropHandler { get; }
    IMVVMDragDropHandler CurrentItemsDragDropHandler { get; }


  }

  [ExpectedTypeOfView(typeof(IStyleListView))]
  public partial class StyleListController<TManager, TList, TItem>
    :
    MVCANControllerEditImmutableDocBase<TList, IStyleListView>, IStyleListController 
      where TItem : Altaxo.Main.IImmutable
      where TList : IStyleList<TItem>
      where TManager : IStyleListManager<TList, TItem>
  {
    private TManager _manager;

    private NGTreeNode _availableListsRootNode;
    protected NGTreeNode _availableItemsRootNode;
    // protected SelectableListNodeList _currentItems;

    private bool _currentItems_IsDirty;
    private bool _isNameOfNewListValid;

    public StyleListController(TManager managerInstance)
    {
      _manager = managerInstance;
      CmdCurrentList_Store = new RelayCommand(EhCurrentList_Store);
      CmdAvailableItem_AddToCurrent = new RelayCommand(EhAvailableItem_AddToCurrent);
      CmdCurrentItem_Remove = new RelayCommand(EhCurrentItem_Remove);
      CmdCurrentItem_MoveUp = new RelayCommand(EhCurrentItem_MoveUp);
      CmdCurrentItem_MoveDown = new RelayCommand(EhCurrentItem_MoveDown);
      CmdCurrentItem_Edit = new RelayCommand(EhCurrentItem_Edit);

      AvailableItemsDragDropHandler = new AvailableItems_DragDropHandler(this);
      CurrentItemsDragDropHandler = new CurrentItems_DragDropHandler(this);
    }

    #region Bindings

    public NGTreeNodeCollection AvailableLists => _availableListsRootNode.Nodes;

    public NGTreeNodeCollection AvailableItems => _availableItemsRootNode.Nodes;

    private SelectableListNodeList _currentItems;

    public SelectableListNodeList CurrentItems
    {
      get => _currentItems;
      set
      {
        if (!(_currentItems == value))
        {
          _currentItems = value;
          OnPropertyChanged(nameof(CurrentItems));
        }
      }
    }

    private string _newListNameText;

    public string NewListNameText
    {
      get => _newListNameText;
      set
      {
        if (!(_newListNameText == value))
        {
          _newListNameText = value;
          OnPropertyChanged(nameof(NewListNameText));
          SetListDirty();
        }
      }
    }

    private string _newListNameToolTip = "Enter a unique name for the new list.";

    public string NewListNameToolTip
    {
      get => _newListNameToolTip;
      set
      {
        if (!(_newListNameToolTip == value))
        {
          _newListNameToolTip = value;
          OnPropertyChanged(nameof(NewListNameToolTip));
        }
      }
    }

    private bool _newListNameIsEnabled;

    public bool NewListNameIsEnabled
    {
      get => _newListNameIsEnabled;
      set
      {
        if (!(_newListNameIsEnabled == value))
        {
          _newListNameIsEnabled = value;
          OnPropertyChanged(nameof(NewListNameIsEnabled));
        }
      }
    }
    private bool _newListNameIsMarked;

    public bool NewListNameIsMarked
    {
      get => _newListNameIsMarked;
      set
      {
        if (!(_newListNameIsMarked == value))
        {
          _newListNameIsMarked = value;
          OnPropertyChanged(nameof(NewListNameIsMarked));
        }
      }
    }


    private bool _storeInUserSettings;

    public bool StoreInUserSettings
    {
      get => _storeInUserSettings;
      set
      {
        if (!(_storeInUserSettings == value))
        {
          _storeInUserSettings = value;
          OnPropertyChanged(nameof(StoreInUserSettings));
        }
      }
    }

    public ICommand CmdCurrentList_Store { get; }

    public ICommand CmdAvailableItem_AddToCurrent { get; }

    public ICommand CmdCurrentItem_Remove { get; }

    public ICommand CmdCurrentItem_MoveUp { get; }

    public ICommand CmdCurrentItem_MoveDown { get; }

    public ICommand CmdCurrentItem_Edit { get; }

    public IMVVMDragDropHandler AvailableItemsDragDropHandler { get; }
    public IMVVMDragDropHandler CurrentItemsDragDropHandler { get; }

    #endregion

    protected override void Initialize(bool initData)
    {
      base.Initialize(initData);

      if (initData)
      {
        Controller_AvailableLists_Initialize();

        // Available items
        Controller_AvailableItems_Initialize();

        // Current items
        Controller_CurrentItems_Initialize();

        StoreInUserSettings = IsListAtUserLevel(_doc);
        CurrentItemListName_Initialize(_doc.Name, IsListAtUserOrProjectLevel(_doc), false, "Name can not be changed because list is already stored!");
      }
    }

    private void CurrentItemListName_Initialize(string name, bool isEnabled, bool isMarked, string toolTip)
    {
      NewListNameText = name;
      NewListNameIsEnabled = isEnabled;
      NewListNameIsMarked = isMarked;
      NewListNameToolTip = toolTip;
    }

    private bool IsListAtUserOrProjectLevel(TList list)
    {
      var entry = _manager.GetEntryValue(list.Name);
      return entry.Level == ItemDefinitionLevel.Project || entry.Level == ItemDefinitionLevel.UserDefined;
    }

    private bool IsListAtUserLevel(TList list)
    {
      var entry = _manager.GetEntryValue(list.Name);
      return entry.Level == ItemDefinitionLevel.UserDefined;
    }

    public override bool Apply(bool disposeController)
    {
      if (_currentItems_IsDirty && false == TryToStoreList())
        return ApplyEnd(false, disposeController);

      return ApplyEnd(true, disposeController);
    }

    public override IEnumerable<ControllerAndSetNullMethod> GetSubControllers()
    {
      yield break;
    }

    #region How to display items

    protected virtual string ToDisplayName(TItem item)
    {
      return ToDisplayName(item.GetType());
      ;
    }

    protected virtual string ToDisplayName(Type item)
    {
      return item.Name;
    }

    #endregion How to display items

    #region Available lists

    private void Controller_AvailableLists_Initialize()
    {
      _availableListsRootNode = new NGTreeNode();
      var levelDict = new Dictionary<ItemDefinitionLevel, NGTreeNode>();
      var allListsWithLevel = _manager.GetEntryValues().ToArray();
      Array.Sort(allListsWithLevel, (x, y) =>
      {
        int result = Comparer<ItemDefinitionLevel>.Default.Compare(x.Level, y.Level);
        return result != 0 ? result : string.Compare(x.List.Name, y.List.Name);
      }
      );

      var dict = new Dictionary<string, NGTreeNode>();

      foreach (var listAndLevel in allListsWithLevel)
      {
        if (!levelDict.TryGetValue(listAndLevel.Level, out var levelNode))
        {
          levelNode = new NGTreeNode(Enum.GetName(typeof(ItemDefinitionLevel), listAndLevel.Level));
          levelDict.Add(listAndLevel.Level, levelNode);
          _availableListsRootNode.Nodes.Add(levelNode);
        }
        levelNode.Nodes.Add(new NGTreeNode(listAndLevel.List.Name) { Tag = listAndLevel.List, IsSelected = object.ReferenceEquals(listAndLevel.List, _doc) });
      }
      OnPropertyChanged(nameof(AvailableLists));
    }

    #endregion Available lists

    #region AvailableItens

    protected virtual void Controller_AvailableItems_Initialize()
    {
      _availableItemsRootNode = new NGTreeNode();

      var availableItems = Altaxo.Main.Services.ReflectionService.GetNonAbstractSubclassesOf(typeof(TItem));
      foreach (var item in availableItems)
        _availableItemsRootNode.Nodes.Add(new NGTreeNode(ToDisplayName(item)) { Tag = item, IsSelected = false });

      OnPropertyChanged(nameof(AvailableItems));
    }



    #endregion AvailableItens

    #region Current items

    protected virtual void Controller_CurrentItems_Initialize()
    {

      var currentItems = new SelectableListNodeList();

      foreach (var currentItem in _doc)
      {
        currentItems.Add(new SelectableListNode(ToDisplayName(currentItem), currentItem, false));
      }

      CurrentItems = currentItems;
    }



    private void ControllerAndView_CurrentItemsAndName_Initialize()
    {
      Controller_CurrentItems_Initialize();
      StoreInUserSettings = IsListAtUserLevel(_doc);
      CurrentItemListName_Initialize(_doc.Name, IsListAtUserOrProjectLevel(_doc), false, "Name can't be changed because list is already stored!");
      _currentItems_IsDirty = false;
    }

    #endregion Current items

    private bool TryToStoreList()
    {
      if (CurrentItems.Count == 0)
      {
        Current.Gui.ErrorMessageBox("The list does not contains any items, thus it can not be stored");
        return false;
      }

      if (!_isNameOfNewListValid)
      {
        Current.Gui.ErrorMessageBox("Can not store the list because there is some issue with the new name. Please enter a valid and unique name for the list in the text box.");
        return false;
      }

      bool isUser = StoreInUserSettings;
      var doc = _manager.CreateNewList(NewListNameText, CurrentItems.Select(node => (TItem)node.Tag));
      _manager.TryRegisterList(doc, isUser ? Altaxo.Main.ItemDefinitionLevel.UserDefined : Altaxo.Main.ItemDefinitionLevel.Project, out doc);
      _doc = doc;

      _isNameOfNewListValid = true;
      _currentItems_IsDirty = false;

      Controller_AvailableLists_Initialize();
      ControllerAndView_CurrentItemsAndName_Initialize();

      return true;
    }

    private void EhCurrentList_Store()
    {
      if (_currentItems_IsDirty)
      {
        TryToStoreList();
      }
      else
      {
        // even if no items have changed, the user should be able to promote a list from project level to user defined level
        // or vice versa to demote a list from user defined level to project level.
        if (IsListAtUserOrProjectLevel(_doc))
        {
          bool isListAtUserLevel = _manager.GetEntryValue(_doc.Name).Level == ItemDefinitionLevel.UserDefined;
          bool shouldSwitchLevel = isListAtUserLevel ^ StoreInUserSettings; // if true, we should switch the levels
          if (shouldSwitchLevel)
          {
            _manager.SwitchItemDefinitionLevelBetweenUserAndProject(_doc.Name);
            Controller_AvailableLists_Initialize();
            ControllerAndView_CurrentItemsAndName_Initialize();
          }
        }
      }
    }

    protected virtual bool IsItemEditable(Altaxo.Main.IImmutable item)
    {
      if (item is null)
        return false;
      var prop = item.GetType().GetProperty("IsEditable", typeof(bool));
      if (prop is null)
        return false;
      return (bool)prop.GetValue(item, null);
    }

    protected virtual void EhCurrentItem_Edit()
    {
      var node = CurrentItems.FirstSelectedNode;
      if (node is null)
        return;
      var item = (TItem)node.Tag;
      if (item is null)
        return;

      var controller = (IMVCANController)Current.Gui.GetControllerAndControl(new object[] { item }, typeof(IMVCANController));
      if (controller is null || controller.ViewObject is null)
        return;

      if (true == Current.Gui.ShowDialog(controller, "Edit item"))
      {
        item = (TItem)controller.ModelObject;
        node.Text = ToDisplayName(item);
        node.Tag = item;

        SetListDirty();
      }
    }

    private void EhCurrentItem_Remove()
    {
      var selNodes = CurrentItems.Where((node) => node.IsSelected).ToArray();

      foreach (var node in selNodes)
      {
        CurrentItems.Remove(node);
      }

      SetListDirty();
    }

    private void EhCurrentItem_MoveDown()
    {
      CurrentItems.MoveSelectedItemsDown();
      SetListDirty();
    }

    private void EhCurrentItem_MoveUp()
    {
      CurrentItems.MoveSelectedItemsUp();
      SetListDirty();
    }

    protected virtual void EhAvailableItem_AddToCurrent()
    {
      var avNode = _availableItemsRootNode.FirstSelectedNode;
      if (avNode is null)
        return;
      var newItem = default(TItem);
      try
      {
        newItem = (TItem)Activator.CreateInstance((Type)avNode.Tag);
      }
      catch (Exception ex)
      {
        Current.Gui.ErrorMessageBox("The new item could not be created, message: " + ex.Message);
      }

      if (newItem is not null)
      {
        CurrentItems.Add(new SelectableListNode(ToDisplayName(newItem), newItem, false));
        SetListDirty();
      }
    }

    private void EhAvailableLists_SelectionChanged(NGTreeNode node)
    {
      if (!(node?.Tag is TList))
        return;

      if (_currentItems_IsDirty)
      {
        if (false == AskForDirtyListToSave())
          return;
      }

      // node.Tag contains the list to choose
      _doc = (TList)node.Tag;

      ControllerAndView_CurrentItemsAndName_Initialize();
    }

    private bool AskForDirtyListToSave()
    {
      if (_currentItems_IsDirty)
      {
        for (; ; )
        {
          bool? hasToBeStored = Current.Gui.YesNoCancelMessageBox("You selected a new list, but your current list is not stored yet, and all changes would be discarded. Would you like to store your current list now?", "Attention - list not stored!", null);

          if (false == hasToBeStored)
            return true; // true means we can discard the list
          if (hasToBeStored is null)
            return false; // Cancel will end this action unsuccessful
          if (true == TryToStoreList())
            return true;
        }
      }
      return true;
    }

    private void EhCurrentItemListName_Changed()
    {
      SetListDirty();
    }

    protected void SetListDirty()
    {
      if (_manager.TryGetListByMembers(CurrentItems.Select(node => (TItem)node.Tag), null, out var existingName))
      {
        _currentItems_IsDirty = false;
        _isNameOfNewListValid = true;
        _doc = _manager.GetList(existingName);
        CurrentItemListName_Initialize(existingName, false, false, "Name can't be changed because list is already stored!");
        _availableListsRootNode.FromHereToLeavesDo(treeNode => treeNode.IsSelected = object.ReferenceEquals(treeNode.Tag, _doc));
      }
      else // this list is not known up to now
      {
        _currentItems_IsDirty = true;
        _isNameOfNewListValid = true;
        string name = NewListNameText;
        _currentItems_IsDirty = true;
        if (!_manager.ContainsList(name) && !string.IsNullOrEmpty(name))
        {
          // is OK, we can use this name
          CurrentItemListName_Initialize(name, true, false, "The name is available as new name of the list");
        }
        else if (string.IsNullOrEmpty(name))
        {
          CurrentItemListName_Initialize(name, true, true, "Please enter the name of the new list!");
          _isNameOfNewListValid = false;
        }
        else
        {
          CurrentItemListName_Initialize(name, true, true, "Please choose another name since this name is already in use!");
          _isNameOfNewListValid = false;
        }
      }
    }

    private static DropReturnData DropFailedReturnData { get { return new DropReturnData { IsCopy = false, IsMove = false }; } }
  }
}
