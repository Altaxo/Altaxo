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

#nullable enable
using System;
using System.Windows.Input;
using Altaxo.Collections;

namespace Altaxo.Gui.Common.MultiRename
{
  /// <summary>
  /// Interface that must be implemented by views that visualize <see cref="MultiRenameData"/>.
  /// </summary>
  public interface IMultiRenameView : IDataContextAwareView
  {
  }

  [ExpectedTypeOfView(typeof(IMultiRenameView))]
  [UserControllerForObject(typeof(MultiRenameData))]
  public class MultiRenameController : ControllerBase, IMVCANController
  {
    private MultiRenameData? _doc;
    private IMultiRenameView? _view;

    private ListNodeList _itemsShown = new ListNodeList();
    private string[] _columNames = new string[0];

    /// <summary>
    /// Initializes the list of available shortcuts. First column has to be the type of shortcut, then the shortcut itself, then the description text.
    /// </summary>
    private ListNodeList _shortcutDescriptionList = new ListNodeList();



    #region ListNode

    private class MyNode : ListNode
    {
      private MultiRenameData _data;

      public string NewName
      {
        get
        {
          return _data.GetNewNameForObject((int)_tag!);
        }
        set
        {
          var oldName = _data.GetNewNameForObjectOrNull((int)_tag!);
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

      public override string? Text
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

        var newName = _data.GetNewNameForObject((int)_tag!);

        if (fkt is not null)
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

      public override string? SubItemText(int i)
      {
        switch (i)
        {
          case 0:
            return _text;
          case 1:
            return (string)_tag!;
          case 2:
            return _description;
          default:
            throw new ArgumentOutOfRangeException("i");
        }
      }
    }

    #endregion ListNode

    public MultiRenameController()
    {
      CmdChooseBaseDirectory = new RelayCommand(EhChooseBaseDirectory);
    }

   


    #region Binding

    string _renameStringTemplate;
    public string RenameStringTemplate
    {
      get => _renameStringTemplate;
      set
      {
        if(!(_renameStringTemplate == value))
        {
          _renameStringTemplate = value;
          OnPropertyChanged(nameof(RenameStringTemplate));
          EhRenameStringTemplateChanged(value);
        }
      }
    }

    private void EhRenameStringTemplateChanged(string renameStringTemplate)
    {
      var walker = new MultiRenameTreeWalker(renameStringTemplate, _doc);
      var result = walker.VisitTree();

      for (int i = 0; i < _itemsShown.Count; ++i)
      {
        var item = (MyNode)_itemsShown[i];
        int originalIndex = (int)item.Tag!;
        string newName = result.GetContent(originalIndex, i);
        item.NewName = newName;
      }
    }


    public ICommand CmdChooseBaseDirectory { get; }
    private void EhChooseBaseDirectory()
    {
      var dlg = new Altaxo.Gui.FolderChoiceOptions()
      {
        ShowNewFolderButton = true,
        Description = "Choose base folder"
      };

      if (true == Current.Gui.ShowFolderDialog(dlg))
      {
        EhBaseDirectoryChosen(dlg.SelectedPath);
      }
    }

    bool _isBaseDirectoryButtonVisible;
    /// <summary>
    /// Sets a value indicating whether the button to choose the base directory should be visible.
    /// </summary>
    /// <value>
    ///   <c>true</c> if the button to choose the base directory is visible; otherwise, <c>false</c>.
    /// </value>
    public bool IsBaseDirectoryButtonVisible
    {
      get => _isBaseDirectoryButtonVisible;
      set
      {
        if(!(_isBaseDirectoryButtonVisible==value))
        {
          _isBaseDirectoryButtonVisible = value;
          OnPropertyChanged(nameof(IsBaseDirectoryButtonVisible));
        }
      }
    }

    /// <summary>
    /// Initializes the list of available shortcuts. First column has to be the type of shortcut, then the shortcut itself, then the description text.
    /// </summary>
    public ListNodeList AvailableShortcuts => _shortcutDescriptionList;

    /// <summary>
    /// Initializes the list view which shows the items.
    /// </summary>
    public ListNodeList ItemListItems => _itemsShown;

    /// <summary>
    /// Initializes the column names of the list of items.
    /// </summary>
    public string[] ColumNames => _columNames;

    #endregion

    public bool InitializeDocument(params object[] args)
    {
      if (args is null || args.Length == 0 || !(args[0] is MultiRenameData))
        return false;

      _doc = (MultiRenameData)args[0];

      Initialize(true);
      return true;
    }

    private void Initialize(bool initData)
    {
      if (_doc is null)
        throw NoDocumentException;

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

        IsBaseDirectoryButtonVisible = _doc.IsRenameOperationFileSystemBased;
        RenameStringTemplate = _doc.DefaultPatternString;
      }
    }

    public UseDocument UseDocumentCopy
    {
      set { }
    }

    public object? ViewObject
    {
      get
      {
        return _view;
      }
      set
      {
        DetatchView();
        _view = value as IMultiRenameView;

        if (_view is not null)
        {
          AttatchView();
          Initialize(false);
        }
      }
    }

    void AttatchView()
    {
      if (_view is IDataContextAwareView view)
        view.DataContext = this;
    }

    void DetatchView()
    {
      if (_view is IDataContextAwareView view)
        view.DataContext = null;
    }


    private void EhBaseDirectoryChosen(string path)
    {
      if (_view is { } view)
      {
        if (!path.EndsWith("\\"))
          path += "\\";


        string template = RenameStringTemplate;
        var idx = template.IndexOf("[");
        if (idx < 0)
          idx = template.Length;

        template = path + template.Substring(idx, template.Length - idx);

        RenameStringTemplate = template;
      }
    }

    

    public object ModelObject
    {
      get
      {
        if (_doc is null)
          throw NoDocumentException;
        return _doc;
      }
    }



    public bool Apply(bool disposeController)
    {
      if (_doc is null)
        throw NoDocumentException;

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
