using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Input;
using Altaxo.Collections;
using Altaxo.Data;

namespace Altaxo.Gui.Data
{
  public interface IDataTablesAggregationDataView : IDataContextAwareView { }

  [ExpectedTypeOfView(typeof(IDataTablesAggregationDataView))]
  [UserControllerForObject(typeof(DataTablesAggregationProcessData))]
  public class DataTablesAggregationDataController : MVCANControllerEditCopyOfDocBase<DataTablesAggregationProcessData, IDataTablesAggregationDataView>
  {


    public override IEnumerable<ControllerAndSetNullMethod> GetSubControllers()
    {
      yield break;
    }

    #region Bindings

    public ICommand CmdAddToParticipatingTables => field ??= new RelayCommand(EhAddToParticipatingDataTable);
    public ICommand CmdRemoveFromParticipatingTables => field ??= new RelayCommand(EhRemoveFromParticipatingTablesCommand);
    public ICommand CmdParticipatingTablesUp => field ??= new RelayCommand(EhParticipatingTablesUpCommand);
    public ICommand CmdParticipatingTablesDown => field ??= new RelayCommand(EhParticipatingTablesDownCommand);
    public ICommand CmdAutoRename => field ??= new RelayCommand(EhAutoRename);


    private SelectableListNodeList _availableTables;

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

    public ICommand CmdFilterMatchesToParticipatingTables => field ??= new RelayCommand(EhCmdNowAddFilterMatchesToParticipatingTables);

    public ICommand CmdTestDataAndOptions => field ??= new RelayCommand(() => TestDataAndOptions?.Invoke(), () => TestDataAndOptions is not null);


    public Action? TestDataAndOptions { get; set; }

    #endregion

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

    private void EhCmdNowAddFilterMatchesToParticipatingTables()
    {
      var filterStrings = Filters.Select(f => f.Value).ToList();
      var tablesToAdd = Current.Project.DataTableCollection.Where(t => DataTablesAggregationProcessData.IsTableNameMatching(filterStrings, t.Name)).ToHashSet();
      tablesToAdd.AddRange(ParticipatingTables.Select(n => (DataTable)n.Tag));
      ParticipatingTables.Clear();
      ParticipatingTables.AddRange(tablesToAdd.Select(t => new SelectableListNode(t.Name, t, false)));
    }

    private void OnParticipatingTablesChanged()
    {
    }

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

    private void EhParticipatingTablesUpCommand()
    {
      ParticipatingTables.MoveSelectedItemsUp();
    }

    private void EhParticipatingTablesDownCommand()
    {
      ParticipatingTables.MoveSelectedItemsDown();
    }

    private void EhAutoRename()
    {
      for (int i = 0; i < ParticipatingTables.Count; ++i)
      {
        ParticipatingTables[i].Text = i.ToString();
      }
    }

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
        Filters.Select(f => f.Value).Where(s => !string.IsNullOrEmpty(s)).ToImmutableList(),
        AddMatched,
        RemoveUnmatched);

      return ApplyEnd(true, disposeController);
    }


  }
}
