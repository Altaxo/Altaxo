using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Input;
using Altaxo.Collections;
using Altaxo.Data;
using Altaxo.Data.Selections;

namespace Altaxo.Gui.Data
{
  /// <summary>
  /// Represents the view for editing <see cref="DataTablesAggregationProcessData"/>.
  /// </summary>
  public interface IDataTablesAggregationDataView : IDataContextAwareView { }

  /// <summary>
  /// Controller for editing <see cref="DataTablesAggregationProcessData"/> in a GUI.
  /// </summary>
  [ExpectedTypeOfView(typeof(IDataTablesAggregationDataView))]
  [UserControllerForObject(typeof(DataTablesAggregationProcessData))]
  public class DataTablesAggregationDataController : MVCANControllerEditCopyOfDocBase<DataTablesAggregationProcessData, IDataTablesAggregationDataView>
  {


    /// <inheritdoc/>
    public override IEnumerable<ControllerAndSetNullMethod> GetSubControllers()
    {
      yield break;
    }

    #region Bindings

    /// <summary>
    /// Gets the command that adds selected available tables to the list of participating tables.
    /// </summary>
    public ICommand CmdAddToParticipatingTables => field ??= new RelayCommand(EhAddToParticipatingDataTable);

    /// <summary>
    /// Gets the command that removes selected tables from the list of participating tables.
    /// </summary>
    public ICommand CmdRemoveFromParticipatingTables => field ??= new RelayCommand(EhRemoveFromParticipatingTablesCommand);

    /// <summary>
    /// Gets the command that moves selected participating tables up in the list.
    /// </summary>
    public ICommand CmdParticipatingTablesUp => field ??= new RelayCommand(EhParticipatingTablesUpCommand);

    /// <summary>
    /// Gets the command that moves selected participating tables down in the list.
    /// </summary>
    public ICommand CmdParticipatingTablesDown => field ??= new RelayCommand(EhParticipatingTablesDownCommand);

    /// <summary>
    /// Gets the command that automatically renames participating tables to unique names.
    /// </summary>
    public ICommand CmdAutoRename => field ??= new RelayCommand(EhAutoRename);


    private SelectableListNodeList _availableTables;

    /// <summary>
    /// Gets or sets the list of available tables that can participate in the aggregation.
    /// </summary>
    public SelectableListNodeList AvailableTables
    {
      get => _availableTables;
      set
      {
        if (!(_availableTables == value))
        {
          _availableTables = value;
          OnPropertyChanged(nameof(AvailableTables));
        }
      }
    }

    private SelectableListNodeList _participatingTables;

    /// <summary>
    /// Gets or sets the list of tables that currently participate in the aggregation.
    /// </summary>
    public SelectableListNodeList ParticipatingTables
    {
      get => _participatingTables;
      set
      {
        if (!(_participatingTables == value))
        {
          _participatingTables = value;
          OnPropertyChanged(nameof(ParticipatingTables));
        }
      }
    }


    /// <summary>
    /// Gets or sets the collection of filter strings used to select tables by name.
    /// </summary>
    public ObservableCollection<NotifyChangedValue<string>> Filters
    {
      get => field;
      set
      {
        if (!(field == value))
        {
          field = value;
          OnPropertyChanged(nameof(Filters));
        }
      }
    }


    /// <summary>
    /// Gets or sets a value indicating whether tables that match the filters should be added to the participating tables before execution.
    /// </summary>
    public bool AddMatched
    {
      get => field;
      set
      {
        if (!(field == value))
        {
          field = value;
          OnPropertyChanged(nameof(AddMatched));
        }
      }
    }


    /// <summary>
    /// Gets or sets a value indicating whether tables that do not match the filters should be removed from the participating tables before execution.
    /// </summary>
    public bool RemoveUnmatched
    {
      get => field;
      set
      {
        if (!(field == value))
        {
          field = value;
          OnPropertyChanged(nameof(RemoveUnmatched));
        }
      }
    }

    /// <summary>
    /// Gets the command that adds all tables matching the current filters to the participating tables.
    /// </summary>
    public ICommand CmdFilterMatchesToParticipatingTables => field ??= new RelayCommand(EhCmdNowAddFilterMatchesToParticipatingTables);

    /// <summary>
    /// Gets the command that triggers a test of the current data and options.
    /// </summary>
    public ICommand CmdTestDataAndOptions => field ??= new RelayCommand(() => TestDataAndOptions?.Invoke(), () => TestDataAndOptions is not null);


    /// <summary>
    /// Gets or sets the callback used to test data and options.
    /// </summary>
    public Action? TestDataAndOptions { get; set; }

    #endregion

    /// <inheritdoc/>
    protected override void Initialize(bool initData)
    {
      base.Initialize(initData);

      if (initData)
      {
        { // Available tables
          var tables = new List<DataTable>(Current.Project.DataTableCollection);
          tables.Sort((x, y) => string.Compare(x.Name, y.Name));
          AvailableTables = new SelectableListNodeList(tables.Select(t => new SelectableListNode(t.Name, t, false)));
        }

        { // Participating tables

          ParticipatingTables = new SelectableListNodeList(_doc.TableProxies.Where(p => p.Document is not null)
            .Select(p => new SelectableListNode(p.Document.Name, p.Document, false)));
        }

        OnParticipatingTablesChanged();

        Filters = new ObservableCollection<NotifyChangedValue<string>>(_doc.DataTableNameFilter.Select(s => new NotifyChangedValue<string>(s)));
        AddMatched = _doc.AddFilterMatchedTablesBeforeExecution;
        RemoveUnmatched = _doc.RemoveFilterUnmatchedTablesBeforeExecution;

      }
    }

    /// <summary>
    /// Adds all tables matching the current filters to the participating tables list.
    /// </summary>
    private void EhCmdNowAddFilterMatchesToParticipatingTables()
    {
      var filterStrings = Filters.Select(f => f.Value).ToList();
      var tablesToAdd = Current.Project.DataTableCollection.Where(t => DataTablesAggregationProcessData.IsTableNameMatching(filterStrings, t.Name)).ToHashSet();
      tablesToAdd.AddRange(ParticipatingTables.Select(n => (DataTable)n.Tag));
      ParticipatingTables.Clear();
      ParticipatingTables.AddRange(tablesToAdd.Select(t => new SelectableListNode(t.Name, t, false)));
    }

    /// <summary>
    /// Called when the list of participating tables has changed.
    /// </summary>
    private void OnParticipatingTablesChanged()
    {
    }

    /// <summary>
    /// Adds selected available tables to the participating tables list.
    /// </summary>
    private void EhAddToParticipatingDataTable()
    {
      for (int i = 0; i < AvailableTables.Count; i++)
      {
        if (AvailableTables[i].IsSelected)
        {
          var dataTable = (DataTable)AvailableTables[i].Tag;
          if (!ParticipatingTables.Any(sln => object.ReferenceEquals(sln.Tag, dataTable)))
          {
            ParticipatingTables.Add(new SelectableListNode(dataTable.Name, dataTable, true));
          }
        }
      }
      OnParticipatingTablesChanged();
    }

    /// <summary>
    /// Removes selected tables from the participating tables list.
    /// </summary>
    private void EhRemoveFromParticipatingTablesCommand()
    {
      for (int i = ParticipatingTables.Count - 1; i >= 0; i--)
      {
        if (ParticipatingTables[i].IsSelected)
        {
          ParticipatingTables.RemoveAt(i);
        }
      }
      OnParticipatingTablesChanged();
    }

    /// <summary>
    /// Moves selected participating tables up in the list.
    /// </summary>
    private void EhParticipatingTablesUpCommand()
    {
      ParticipatingTables.MoveSelectedItemsUp();
    }

    /// <summary>
    /// Moves selected participating tables down in the list.
    /// </summary>
    private void EhParticipatingTablesDownCommand()
    {
      ParticipatingTables.MoveSelectedItemsDown();
    }

    /// <summary>
    /// Automatically renames participating tables to unique numeric names.
    /// </summary>
    private void EhAutoRename()
    {
      for (int i = 0; i < ParticipatingTables.Count; ++i)
      {
        ParticipatingTables[i].Text = i.ToString();
      }
    }

    /// <inheritdoc/>
    public override bool Apply(bool disposeController)
    {
      if (ParticipatingTables.Count == 0)
      {
        Current.Gui.ErrorMessageBox("Please select at least one participating table");
        return ApplyEnd(false, disposeController);
      }

      // Test for double names
      var hash = new Dictionary<string, int>();
      for (int i = 0; i < ParticipatingTables.Count; ++i)
      {
        if (hash.TryGetValue(ParticipatingTables[i].Text, out int firstIndex))
        {
          Current.Gui.ErrorMessageBox(
            $"The names are not unique: both element[{firstIndex}] and element[{i}] are named '{ParticipatingTables[i].Text}'\r\n" +
            "You can use the automatic rename button in order to get unique names");

          return ApplyEnd(false, disposeController);
        }
        hash.Add(ParticipatingTables[i].Text, i);
      }


      _doc = new DataTablesAggregationProcessData(
        ParticipatingTables.Select(node => new DataTableProxy(((DataTable)node.Tag))),
        new AllRows(),
        Filters.Select(f => f.Value).Where(s => !string.IsNullOrEmpty(s)).ToImmutableList(),
        AddMatched,
        RemoveUnmatched);

      return ApplyEnd(true, disposeController);
    }


  }
}
