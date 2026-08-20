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

using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Input;
using Altaxo.Collections;
using Altaxo.Gui.Common;
using Altaxo.Serialization.NamePropertyExtraction;

namespace Altaxo.Gui.Serialization.NamePropertyExtraction
{
  /// <summary>
  /// Defines the view contract for the ImportWithPropertiesOptionsController, which is responsible for managing the user interface related to importing files with properties extracted from their names. This interface extends IDataContextAwareView, indicating that it is aware of the data context and can bind to properties and commands defined in the controller.
  /// </summary>
  public interface IImportWithFileNameDerivedPropertiesActionView : IDataContextAwareView { }

  /// <summary>
  /// Controls the user interface for importing files with properties extracted from their names. This controller manages the interaction between the view and the underlying data model, represented by the ImportWithPropertiesOptions class. It provides properties and commands that are bound to the view, allowing users to specify file names, target table names, and manage the property extraction tree.
  /// </summary>
  [ExpectedTypeOfView(typeof(IImportWithFileNameDerivedPropertiesActionView))]
  [UserControllerForObject(typeof(ImportWithFileNameDerivedPropertiesAction))]
  public class ImportWithFileNameDerivedPropertiesActionController : MVCANDControllerEditImmutableDocBase<ImportWithFileNameDerivedPropertiesAction, IImportWithFileNameDerivedPropertiesActionView>
  {
    const string FileNameProperty = "FileName";
    const string FilePathProperty = "FilePath";
    const string TableNameProperty = "TableName";
    const string DiagnosticsProperty = "Diagnostics";
    const string KindProperty = "Kind";
    const string ItemNameProperty = "Item name";


    /// <inheritdoc />
    public override IEnumerable<ControllerAndSetNullMethod> GetSubControllers()
    {
      yield break;
    }

    #region Bindings

    /// <summary>
    /// Gets or sets the file names to be imported. This property is bound to the view and is used to specify the files that will be processed for property extraction and import into the target table.
    /// </summary>
    public string FileNames
    {
      get => field;
      set
      {
        if (!(field == value))
        {
          field = value;
          _doc = _doc with { FileNamePatternsIncluded = GetUnresolvedFileNames().ToImmutableList() };
          OnPropertyChanged(nameof(FileNames));
          OnMadeDirty();
          EhResolveFileNames();
        }
      }
    }





    /// <summary>
    /// Gets the command to add files for import. This command is bound to the view and is triggered when the user initiates the action to add files. The command executes the EhCmdAddFiles method, which handles the logic for adding files to the import process.
    /// </summary>

    public ICommand CmdAddFiles => field ??= new RelayCommand(EhCmdAddFiles);



    /// <summary>
    /// Gets or sets the target table name for the import process. Can contain placeholders for properties extracted from the file names. 
    /// </summary>
    public string TableTargetName
    {
      get => field;
      set
      {
        if (!(field == value))
        {
          field = value;
          OnPropertyChanged(nameof(TableTargetName));
          _doc = _doc with { TargetTableNameTemplate = TableTargetName };
          OnMadeDirty();
          UpdateTargetTablePreview();
          UpdatePropertyBagPreview();
        }
      }
    }


    /// <summary>
    /// Gets or sets the property extraction tree controller, which manages the display and editing of the property extraction tree. This controller is responsible for handling the logic related to the property extraction process, including adding and removing nodes in the tree structure.
    /// </summary>
    public PropertyExtractionTreeController TreeController
    {
      get => field;
      set
      {
        if (!(field == value))
        {
          field?.Dispose();
          field?.MadeDirty -= EhExtractionTreeChanged;
          field = value;
          field?.MadeDirty += EhExtractionTreeChanged;
          OnPropertyChanged(nameof(TreeController));
        }
      }
    }


    /// <summary>
    /// Gets or sets the rows of the property preview, which is a DataView representing the extracted properties from the file names. This property is bound to the view and is updated whenever the property extraction tree changes, reflecting the current set of properties that will be used in the import process.
    /// </summary>
    public System.Data.DataView RowsOfPropertyPreview
    {
      get => field;
      set
      {
        if (!(field == value))
        {
          field = value;
          OnPropertyChanged(nameof(RowsOfPropertyPreview));
        }
      }
    }



    /// <summary>
    /// Gets or sets the rows of the target table preview, which is a DataView representing the target table names derived from the extracted properties of the file names. This property is bound to the view and is updated whenever the property extraction tree changes, reflecting the current set of target table names that will be used in the import process.
    /// </summary>
    public System.Data.DataView RowsOfTargetTablePreview
    {
      get => field;
      set
      {
        if (!(field == value))
        {
          field = value;
          OnPropertyChanged(nameof(RowsOfTargetTablePreview));
        }
      }
    }



    /// <summary>
    /// Gets or sets the rows of the property preview, which is a DataView representing the extracted properties from the file names. This property is bound to the view and is updated whenever the property extraction tree changes, reflecting the current set of properties that will be used in the import process.
    /// </summary>
    public System.Data.DataView RowsOfPropertyBagPreview
    {
      get => field;
      set
      {
        if (!(field == value))
        {
          field = value;
          OnPropertyChanged(nameof(RowsOfPropertyBagPreview));
        }
      }
    }



    /// <summary>
    /// Gets or sets the list of property names that are extracted from the file names during the import process. This property is bound to the view and is updated whenever the property extraction tree changes, reflecting the current set of properties that will be used in the import process.
    /// </summary>
    public ObservableCollection<string> PropertyNames
    {
      get => field;
      set
      {
        if (!(field == value))
        {
          field = value;
          OnPropertyChanged(nameof(PropertyNames));
        }
      }
    }

    /// <summary>
    /// Gets or sets the collection of actions to be performed on properties during the import process. Each action specifies a property name and the level of the property bag where the property should be put. This collection is bound to the view and allows users to manage the actions that will be applied to properties extracted from file names during the import process.
    /// </summary>


    public ItemsController<IActionOnProperty> ActionsPutToPropertyBag
    {
      get => field;
      set
      {
        if (!(field == value))
        {
          field = value;
          OnPropertyChanged(nameof(ActionsPutToPropertyBag));
        }
      }
    }

    /// <summary>
    /// Gets the command to add an action to put a property into the property bag. This command is bound to the view and is triggered when the user initiates the action to add a new property action. The command executes the EhCmdAddActionPutToPropertyBag method, which handles the logic for adding a new action to the collection of actions that will be applied during the import process.
    /// </summary>
    public ICommand CmdAddActionPutToPropertyBag => field ??= new RelayCommand(EhCmdAddActionPutToPropertyBag);

    private void EhCmdAddActionPutToPropertyBag()
    {
      var item = new ActionPutToPropertyBag
      {
        PropertyName = PropertyNames.FirstOrDefault() ?? string.Empty,
        Level = 0,
      };

      if (Current.Gui.ShowDialog(ref item, "Add action to put property to property bag", showApplyButton: false))
      {
        ActionsPutToPropertyBag.Items.Add(new SelectableListNode(item.PropertyName, item, false));
      }
    }

    /// <summary>
    /// Gets the command to add an action that conditionally copies project items to a target folder based on a specified condition. This command is bound to the view and is triggered when the user initiates the action to add a new conditional action. The command executes the EhCmdAddActionConditionalTemplate method, which handles the logic for adding a new conditional action to the collection of actions that will be applied during the import process.
    /// </summary>
    public ICommand CmdAddActionConditionalTemplate => field ??= new RelayCommand(EhCmdAddActionConditionalTemplate);

    private void EhCmdAddActionConditionalTemplate()
    {
      var item = new ActionTextConditionForTemplate
      {
        PropertyName = PropertyNames.FirstOrDefault() ?? string.Empty,
        Condition = string.Empty,
        ProjectItemsUsedAsTemplate = ImmutableList<string>.Empty,
      };

      if (Current.Gui.ShowDialog(ref item, "Use condition for template", showApplyButton: false))
      {
        ActionsPutToPropertyBag.Items.Add(new SelectableListNode(item.PropertyName, item, false));
      }
    }

    /// <summary>
    /// Gets the command to delete the selected action from the collection of actions that will be applied during the import process. This command is bound to the view and is triggered when the user initiates the action to delete an existing action. The command executes the EhCmdDeleteAction method, which handles the logic for removing the selected action from the collection.
    /// </summary>
    public ICommand CmdDeleteAction => field ??= new RelayCommand(EhCmdDeleteAction);

    private void EhCmdDeleteAction()
    {
      ActionsPutToPropertyBag.Items.Remove(ActionsPutToPropertyBag.SelectedItem);
    }

    /// <summary>
    /// Gets the command to edit the selected action in the collection of actions that will be applied during the import process. This command is bound to the view and is triggered when the user initiates the action to edit an existing action. The command executes the EhCmdEditAction method, which handles the logic for editing the selected action and updating its properties based on user input.
    /// </summary>
    public ICommand CmdEditAction => field ??= new RelayCommand(EhCmdEditAction);

    private void EhCmdEditAction()
    {
      var selectedNode = ActionsPutToPropertyBag.SelectedItem;
      if (selectedNode is null)
      {
        return;
      }
      var item = selectedNode.Tag as IActionOnProperty;
      if (item is not null)
      {
        if (Current.Gui.ShowDialog(ref item, "Edit action", showApplyButton: false))
        {
          selectedNode.Tag = item;
          selectedNode.Text = item.PropertyName;
        }
      }
    }



    /// <summary>
    /// Gets or sets the name of a folder or atable that is used as a template if the target table does not exist.
    /// In this case, if the string represents a folder, the contents of the folder is copied to the folder of the target table, or,
    /// if the string represents a table, the table is copied to the target table before importing the files.
    /// </summary>
    public string FolderOrTableNameUsedAsTemplateIfTargetTableIsMissing
    {
      get => field;
      set
      {
        if (!(field == value))
        {
          field = value;
          OnPropertyChanged(nameof(FolderOrTableNameUsedAsTemplateIfTargetTableIsMissing));
          _doc = _doc with { FolderOrTableNameUsedAsTemplateIfTargetTableIsMissing = FolderOrTableNameUsedAsTemplateIfTargetTableIsMissing };
          OnMadeDirty();
        }
      }
    }



    #endregion Bindings

    class ActionListNode : SelectableListNode
    {
      public ActionListNode(IActionOnProperty action)
      {
        Text = action.PropertyName;
        Tag = action;
      }


      public override string? Text0 => (Tag as IActionOnProperty)?.Description;
    }


    /// <inheritdoc />
    protected override void Initialize(bool initData)
    {
      base.Initialize(initData);


      if (initData)
      {
        FileNames = string.Join(Environment.NewLine, _doc.FileNamePatternsIncluded);
        TableTargetName = _doc.TargetTableNameTemplate;
        var treeController = new PropertyExtractionTreeController();
        treeController.InitializeDocument(_doc.NameSplitter);
        Current.Gui.FindAndAttachControlTo(treeController);
        TreeController = treeController;
        FolderOrTableNameUsedAsTemplateIfTargetTableIsMissing = _doc.FolderOrTableNameUsedAsTemplateIfTargetTableIsMissing;

        PropertyNames = new ObservableCollection<string>(_doc.NameSplitter.EnumeratePropertyNames());
        var actionsPutToPropertyBag = new SelectableListNodeList();
        foreach (var action in _doc.ActionsOnProperties)
        {

          actionsPutToPropertyBag.Add(new SelectableListNode(action.PropertyName, action, false));
        }
        actionsPutToPropertyBag.CollectionChanged += (s, e) => EhActionsPutToPropertyBagChanged();
        ActionsPutToPropertyBag = new ItemsController<IActionOnProperty>(actionsPutToPropertyBag);
      }
    }

    /// <summary>
    /// Gets the file names of the GUI as a list of strings.
    /// Please note that the names can contain joker chars, and must be resolved before use.
    /// </summary>
    /// <returns></returns>
    public List<string> GetUnresolvedFileNames()
    {
      return FileNames.Split(new char[] { '\n', '\r' }, StringSplitOptions.RemoveEmptyEntries).Where(s => !string.IsNullOrWhiteSpace(s)).ToList();
    }


    CancellationTokenSource _ctsResolveFileNames = new CancellationTokenSource();

    private ImmutableList<string> _resolvedFileNames = ImmutableList<string>.Empty;

    private void EhResolveFileNames()
    {
      _ctsResolveFileNames?.Cancel();
      _ctsResolveFileNames?.Dispose();

      _ctsResolveFileNames = new CancellationTokenSource();

      var task = Task.Run(() =>
      {
        _resolvedFileNames = ImmutableList<string>.Empty;
        _resolvedFileNames = ImportWithFileNameDerivedPropertiesAction.ResolveFileNames(GetUnresolvedFileNames(), _ctsResolveFileNames.Token).Select(f => f.FullName).ToImmutableList();
      }).ContinueWith(t =>
      {
        if (t.IsCompletedSuccessfully)
        {
          UpdatePropertyPreview();
          UpdateTargetTablePreview();
          UpdatePropertyBagPreview();
        }
      }, TaskScheduler.FromCurrentSynchronizationContext());
    }

    /// <summary>
    /// Gets the resolved file names of the GUI as a list of strings. This method resolves any wildcard characters in the file names and returns the full paths of the files that match the specified patterns.
    /// </summary>
    /// <returns></returns>
    public ImmutableList<string> GetResolvedFileNames()
    {
      return _resolvedFileNames;
    }

    private void EhCmdAddFiles()
    {
      var dlg = new Altaxo.Gui.OpenFileOptions
      {
        Title = "Select files to import",
        Multiselect = true,
        RestoreDirectory = true,
        FilterIndex = 0,
      };
      dlg.AddFilter("*.*", "All files (*.*)");

      if (Current.Gui.ShowOpenFileDialog(dlg) == true)
      {
        var fileNames = dlg.FileNames;
        if (fileNames.Length > 0)
        {
          if (string.IsNullOrWhiteSpace(FileNames))
            FileNames += string.Join(Environment.NewLine, fileNames);
          else
            FileNames += Environment.NewLine + string.Join(Environment.NewLine, fileNames);
        }
      }
    }


    private void EhExtractionTreeChanged(IMVCANDController controller)
    {
      if (TreeController is { } ctrl)
      {
        var tree = (IPropertyExtractionTreeNode)ctrl.ProvisionalModelObject;
        _doc = _doc with { NameSplitter = tree };
        OnMadeDirty();
        PropertyNames = new ObservableCollection<string>(tree.EnumeratePropertyNames());
        UpdatePropertyPreview();
      }
    }

    private void UpdatePropertyPreview()
    {
      if (TreeController is { } ctrl)
      {
        var tree = (IPropertyExtractionTreeNode)ctrl.ProvisionalModelObject;

        var table = new System.Data.DataTable();

        table.Columns.Add(FileNameProperty, typeof(object));

        foreach (var name in PropertyNames)
        {
          table.Columns.Add(name, typeof(object));
        }

        table.Columns.Add(FilePathProperty, typeof(object));

        // now add the rows

        foreach (var fileName in GetResolvedFileNames())
        {
          var row = table.NewRow();
          var entries = tree.ExtractProperties(fileName);
          foreach (var entry in entries)
          {
            row[entry.PropertyName] = entry.PropertyValue;
          }

          row[FileNameProperty] = Path.GetFileName(fileName);
          row[FilePathProperty] = fileName;

          table.Rows.Add(row);
        }

        RowsOfPropertyPreview = table.DefaultView;
      }
    }

    void UpdateTargetTablePreview()
    {
      if (string.IsNullOrEmpty(TableTargetName) || TreeController is not { } ctrl)
      {
        RowsOfTargetTablePreview = null;
        return;
      }

      var tree = (IPropertyExtractionTreeNode)ctrl.ProvisionalModelObject;

      PropertyNames = new ObservableCollection<string>(tree.EnumeratePropertyNames());

      var table = new System.Data.DataTable();

      table.Columns.Add(FileNameProperty, typeof(object));

      table.Columns.Add(TableNameProperty, typeof(object));

      table.Columns.Add(DiagnosticsProperty, typeof(object));

      table.Columns.Add(FilePathProperty, typeof(object));

      // now add the rows

      var fileNames = GetResolvedFileNames();

      var tablesToFileNames = ImportWithFileNameDerivedPropertiesAction.GetTableNamesToFileNamesRelationship(fileNames, TableTargetName, tree);

      var fileNamesToTables = tablesToFileNames.SelectMany(kvp => kvp.Value.FileNames.Select(fileName => (fileName, tableName: kvp.Key))).ToDictionary(x => x.fileName, x => x.tableName);

      foreach (var fileName in fileNames)
      {
        var row = table.NewRow();

        var tableName = fileNamesToTables.ContainsKey(fileName) ? fileNamesToTables[fileName] : string.Empty;
        string diagnostics = "OK";

        if (string.IsNullOrEmpty(tableName))
        {
          diagnostics = "ERROR: No table name could be derived from the file name.";
        }

        if (tableName.EndsWith('\\'))
        {
          diagnostics = "ERROR: The derived table name ends with a backslash, which is not allowed.";
        }

        if (tablesToFileNames[tableName].FileNames.Count > 1)
        {
          diagnostics = "WARNING: Multiple files are mapped to the same table name.";
        }



        row[FileNameProperty] = Path.GetFileName(fileName);
        row[TableNameProperty] = tableName;
        row[DiagnosticsProperty] = diagnostics;
        row[FilePathProperty] = fileName;

        table.Rows.Add(row);
      }

      RowsOfTargetTablePreview = table.DefaultView;

    }

    void EhActionsPutToPropertyBagChanged()
    {
      if (ActionsPutToPropertyBag is not null)
      {
        _doc = _doc with
        {
          ActionsOnProperties = ActionsPutToPropertyBag.Items.Select(a => (IActionOnProperty)a.Tag).ToImmutableList(),
        };
        OnMadeDirty();
      }

      UpdatePropertyBagPreview();
    }

    void UpdatePropertyBagPreview()
    {
      if (TreeController is null || string.IsNullOrEmpty(TableTargetName))
        return;

      var tree = (IPropertyExtractionTreeNode)TreeController.ProvisionalModelObject;

      PropertyNames = new ObservableCollection<string>(tree.EnumeratePropertyNames());

      var table = new System.Data.DataTable();

      table.Columns.Add(KindProperty, typeof(object));
      table.Columns.Add(ItemNameProperty, typeof(object));

      table.Columns.Add(DiagnosticsProperty, typeof(object));

      foreach (var name in ActionsPutToPropertyBag.Items.Select(x => (IActionOnProperty)x.Tag).Select(x => x.PropertyName).Distinct())
      {
        table.Columns.Add($"'{name}'", typeof(object));
      }

      ImportWithFileNameDerivedPropertiesAction.GetPropertiesPlacedInProjectItemsWithDiagnostics(GetResolvedFileNames(), tree, TableTargetName, ActionsPutToPropertyBag.Items.Select(a => (IActionOnProperty)a.Tag).ToImmutableList())
        .ForEach(item =>
        {
          var row = table.NewRow();
          row[KindProperty] = item.Kind.ToString();
          row[ItemNameProperty] = item.ProjectItemName;
          row[DiagnosticsProperty] = item.Diagnostic ?? "OK";

          row[$"'{item.PropertyName}'"] = item.PropertyValue;

          table.Rows.Add(row);
        });



      RowsOfPropertyBagPreview = table.DefaultView;
    }



    /// <inheritdoc />
    public override bool Apply(bool disposeController)
    {
      if (!TreeController.Apply(disposeController))
      {
        return ApplyEnd(false, disposeController);
      }

      _doc = _doc with
      {
        FileNamePatternsIncluded = GetUnresolvedFileNames().ToImmutableList(),
        NameSplitter = (IPropertyExtractionTreeNode)TreeController.ModelObject,
        TargetTableNameTemplate = TableTargetName,
        FolderOrTableNameUsedAsTemplateIfTargetTableIsMissing = FolderOrTableNameUsedAsTemplateIfTargetTableIsMissing,
        ActionsOnProperties = ActionsPutToPropertyBag.Items.Select(a => (IActionOnProperty)a.Tag).ToImmutableList(),
      };


      return ApplyEnd(true, disposeController);
    }


  }
}
