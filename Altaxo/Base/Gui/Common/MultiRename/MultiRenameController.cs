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

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Altaxo.Collections;

namespace Altaxo.Gui.Common.MultiRename
{
  /// <summary>
  /// Interface that must be implemented by views that visualize <see cref="MultiRenameData"/>.
  /// </summary>
  public interface IMultiRenameView
  {
    /// <summary>
    /// Initializes the column names of the list of items.
    /// </summary>
    /// <param name="columnHeaders"></param>
    void InitializeItemListColumns(string[] columnHeaders);

    /// <summary>
    /// Initializes the list view which shows the items.
    /// </summary>
    /// <param name="list">Item list to show.</param>
    void InitializeItemListItems(ListNodeList list);

    /// <summary>
    /// Initializes the list of available shortcuts. First column has to be the type of shortcut, then the shortcut itself, then the description text.
    /// </summary>
    /// <param name="list">Item list.</param>
    void InitializeAvailableShortcuts(Collections.ListNodeList list);

    /// <summary>Returns the string template that is used to rename the items.</summary>
    string RenameStringTemplate { get; set; }

    /// <summary>Fired when the rename string template has changed.</summary>
    event Action RenameStringTemplateChanged;

    /// <summary>
    /// Sets a value indicating whether the button to choose the base directory should be visible.
    /// </summary>
    /// <value>
    ///   <c>true</c> if the button to choose the base directory is visible; otherwise, <c>false</c>.
    /// </value>
    bool IsBaseDirectoryButtonVisible { set; }

    /// <summary>Fired when the user has chosen a base directory. The argument is the selected path.</summary>
    event Action<string> BaseDirectoryChosen;
  }

  [ExpectedTypeOfView(typeof(IMultiRenameView))]
  [UserControllerForObject(typeof(MultiRenameData))]
  public class MultiRenameController : IMVCANController
  {
    private MultiRenameData _doc;
    private IMultiRenameView _view;

    private ListNodeList _itemsShown = new ListNodeList();
    private string[] _columNames;
    private ListNodeList _shortcutDescriptionList = new ListNodeList();



    #region ListNode

    private class MyNode : ListNode
    {
      private MultiRenameData _data;

      public string NewName
      {
        get
        {
          return _data.GetNewNameForObject((int)_tag);
        }
        set
        {
          var oldName = _data.GetNewNameForObject((int)_tag);
          if (oldName != value)
          {
            _data.SetNewNameForObject((int)_tag, value);

            for (int i = 0; i < _data.ColumnsOfObjectInformation.Count; ++i)
              OnPropertyChanged("Text" + (i != 0 ? i.ToString() : string.Empty));
          }
        }
      }

      public MyNode(MultiRenameData data, int idx)
        : base(data.ColumnsOfObjectInformation[0].Value(data.GetObjectToRename(idx), string.Empty), idx)
      {
        _data = data;
      }

      public override string Text
      {
        get
        {
          return SubItemText(0);
        }
        set
        {
        }
      }

      /// <summary>Get the text for column i</summary>
      /// <param name="i">Column index.</param>
      /// <returns></returns>
      public override string SubItemText(int i)
      {
        var fkt = _data.ColumnsOfObjectInformation[i].Value;

        var newName = _data.GetNewNameForObject((int)_tag);

        if (null != fkt)
          return fkt(_data.GetObjectToRename((int)_tag), newName);
        else
          return newName;
      }

      public override int SubItemCount
      {
        get
        {
          return _data.ColumnsOfObjectInformation.Count + 1;
        }
      }
    }

    private class DescriptionNode : ListNode
    {
      private string _description;

      public DescriptionNode(string shortcutType, string shortcut, string description)
        : base(shortcutType, shortcut)
      {
        _description = description;
      }

      public override string SubItemText(int i)
      {
        switch (i)
        {
          case 0:
            return _text;
          case 1:
            return (string)_tag;
          case 2:
            return _description;
          default:
            throw new ArgumentOutOfRangeException("i");
        }
      }
    }

    #endregion ListNode

    public bool InitializeDocument(params object[] args)
    {
      if (args == null || args.Length == 0 || !(args[0] is MultiRenameData))
        return false;

      _doc = (MultiRenameData)args[0];

      Initialize(true);
      return true;
    }

    private void Initialize(bool initData)
    {
      if (initData)
      {
        _itemsShown.Clear();

        for (int i = 0; i < _doc.ObjectsToRenameCount; ++i)
        {
          _itemsShown.Add(new MyNode(_doc, i));
        }

        _columNames = new string[_doc.ColumnsOfObjectInformation.Count];
        for (int i = 0; i < _doc.ColumnsOfObjectInformation.Count; ++i)
          _columNames[i] = _doc.ColumnsOfObjectInformation[i].Key;

        // Description list
        _shortcutDescriptionList.Clear();
        var scList = _doc.GetIntegerShortcuts();
        foreach (string s in scList)
          _shortcutDescriptionList.Add(new DescriptionNode("Number", s, _doc.GetShortcutDescription(s)));
        scList = _doc.GetStringShortcuts();
        foreach (string s in scList)
          _shortcutDescriptionList.Add(new DescriptionNode("Text", s, _doc.GetShortcutDescription(s)));
        scList = _doc.GetDateTimeShortcuts();
        foreach (string s in scList)
          _shortcutDescriptionList.Add(new DescriptionNode("DateTime", s, _doc.GetShortcutDescription(s)));
        scList = _doc.GetArrayShortcuts();
        foreach (string s in scList)
          _shortcutDescriptionList.Add(new DescriptionNode("Array", s, _doc.GetShortcutDescription(s)));
      }

      if (null != _view)
      {
        _view.IsBaseDirectoryButtonVisible = _doc.IsRenameOperationFileSystemBased;
        _view.RenameStringTemplate = _doc.DefaultPatternString;
        _view.InitializeItemListColumns(_columNames);
        _view.InitializeItemListItems(_itemsShown);
        _view.InitializeAvailableShortcuts(_shortcutDescriptionList);
      }
    }

    public UseDocument UseDocumentCopy
    {
      set { }
    }

    public object ViewObject
    {
      get
      {
        return _view;
      }
      set
      {
        if (null != _view)
        {
          _view.RenameStringTemplateChanged -= EhRenameStringTemplateChanged;
          _view.BaseDirectoryChosen -= EhBaseDirectoryChosen;
        }
        _view = value as IMultiRenameView;

        if (null != _view)
        {
          _view.RenameStringTemplateChanged += EhRenameStringTemplateChanged;
          _view.BaseDirectoryChosen += EhBaseDirectoryChosen;
          Initialize(false);
        }
      }
    }

    private void EhBaseDirectoryChosen(string path)
    {
      if (!path.EndsWith("\\"))
        path += "\\";


      string template = _view.RenameStringTemplate;
      var idx = template.IndexOf("[");
      if (idx < 0)
        idx = template.Length;

      template = path + template.Substring(idx, template.Length - idx);

      _view.RenameStringTemplate = template;
    }

    private void EhRenameStringTemplateChanged()
    {
      string template = _view.RenameStringTemplate;
      var walker = new MultiRenameTreeWalker(template, _doc);
      var result = walker.VisitTree();

      for (int i = 0; i < _itemsShown.Count; ++i)
      {
        var item = (MyNode)_itemsShown[i];
        int originalIndex = (int)item.Tag;
        string newName = result.GetContent(originalIndex, i);
        item.NewName = newName;
      }
    }

    public object ModelObject
    {
      get { return _doc; }
    }

    public void Dispose()
    {
    }

    public bool Apply(bool disposeController)
    {
      bool success = _doc.DoRename();

      if (!success)
        Initialize(true); // if not successfull, initialize the lists to show the remaining data

      return success;
    }

    /// <summary>
    /// Try to revert changes to the model, i.e. restores the original state of the model.
    /// </summary>
    /// <param name="disposeController">If set to <c>true</c>, the controller should release all temporary resources, since the controller is not needed anymore.</param>
    /// <returns>
    ///   <c>True</c> if the revert operation was successfull; <c>false</c> if the revert operation was not possible (i.e. because the controller has not stored the original state of the model).
    /// </returns>
    public bool Revert(bool disposeController)
    {
      return false;
    }
  }
}
