using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Altaxo.Collections;
using Altaxo.Data;
using Altaxo.Gui.Common;
using Markdig.Extensions.Tables;

namespace Altaxo.Gui.Data
{
  public interface IExtractCommonColumnsToTableDataView : IDataContextAwareView { }

  [ExpectedTypeOfView(typeof(IExtractCommonColumnsToTableDataView))]
  [UserControllerForObject(typeof(ExtractCommonColumnsToTableData))]
  public class ExtractCommonColumnsToTableDataController : MVCANControllerEditCopyOfDocBase<ExtractCommonColumnsToTableData, IExtractCommonColumnsToTableDataView>
  {
    public override IEnumerable<ControllerAndSetNullMethod> GetSubControllers()
    {
      yield break;
    }

    public ExtractCommonColumnsToTableDataController()
    {
      AddToParticipatingTablesCommand = new RelayCommand(EhAddToParticipatingDataTable);
      RemoveFromParticipatingTablesCommand = new RelayCommand(EhRemoveFromParticipatingTablesCommand);
      ParticipatingTablesUpCommand = new RelayCommand(EhParticipatingTablesUpCommand);
      ParticipatingTablesDownCommand = new RelayCommand(EhParticipatingTablesDownCommand);
    }

   

    #region Bindings

    public ICommand AddToParticipatingTablesCommand { get; }
    public ICommand RemoveFromParticipatingTablesCommand { get; }
    public ICommand ParticipatingTablesUpCommand { get; }
    public ICommand ParticipatingTablesDownCommand { get; }


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

    private ItemsController<string> _xColumn;

    public ItemsController<string> XColumn
    {
      get => _xColumn;
      set
      {
        if (!(_xColumn == value))
        {
          _xColumn = value;
          OnPropertyChanged(nameof(XColumn));
        }
      }
    }

    private SelectableListNodeList _yColumns;

    public SelectableListNodeList YColumns
    {
      get => _yColumns;
      set
      {
        if (!(_yColumns == value))
        {
          _yColumns = value;
          OnPropertyChanged(nameof(YColumns));
        }
      }
    }

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

          var tables = new List<DataTable>(_doc.Tables.Select(proxy => proxy.Document).Where(t => t is not null));
          ParticipatingTables = new SelectableListNodeList(tables.Select(t => new SelectableListNode(t.Name, t, false)));
        }

        OnParticipatingTablesChanged();
      }
    }

    private void OnParticipatingTablesChanged()
    {
      var xColumnName = XColumn is null ? _doc.XColumnName : XColumn.SelectedValue;
      var yColumnNames = new HashSet<string>(
        YColumns is null ?
        _doc.YColumnNames :
        YColumns.Where(n => n.IsSelected).Select(n => (string)n.Tag))
        ;

      HashSet<string>? commonColumnNames = null;

      foreach(var t in ParticipatingTables.Select(node => (DataTable)node.Tag))
          {
          var columnNames = new HashSet<string>(t.DataColumns.Columns.Select(c => t.DataColumns.GetColumnName(c)));
        if (commonColumnNames is null)
          commonColumnNames = columnNames;
        else
          commonColumnNames.IntersectWith(columnNames);
      }
    
    commonColumnNames ??= new HashSet<string>(); // in order to avoid null reference exceptions

      { // Common X-Columns
        XColumn = new ItemsController<string>(new SelectableListNodeList(commonColumnNames.Select(s => new SelectableListNode(s, s, false))));
        XColumn.SelectedValue = xColumnName;
      }

      { // Y-Columns
        YColumns = new SelectableListNodeList(commonColumnNames.Select(s => new SelectableListNode(s, s, yColumnNames.Contains(s))));
      }
    }

    private void EhAddToParticipatingDataTable()
    {
      for (int i = 0; i < AvailableTables.Count; i++)
      {
        if (AvailableTables[i].IsSelected)
        {
          ParticipatingTables.Add(new SelectableListNode(AvailableTables[i].Text, AvailableTables[i].Tag, true));
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

    public override bool Apply(bool disposeController)
    {
      if (ParticipatingTables.Count == 0)
      {
        Current.Gui.ErrorMessageBox("Please select at least one participating table");
        return ApplyEnd(false, disposeController);
      }

      string xColumnName = XColumn.SelectedValue;
      if (string.IsNullOrEmpty(xColumnName))
      {
        Current.Gui.ErrorMessageBox("Please select a common x-column!");
        return ApplyEnd(false, disposeController);
      }


      var yColumnNames = YColumns.Where(node => node.IsSelected).Select(node => (string)node.Tag).ToImmutableArray();
      if(yColumnNames.Length == 0)
      {
        Current.Gui.ErrorMessageBox("Please select at least one y-column!");
        return ApplyEnd(false, disposeController);
      }



      _doc = new ExtractCommonColumnsToTableData(
        ParticipatingTables.Select(node => new DataTableProxy(((DataTable)node.Tag))),
        xColumnName,
        yColumnNames
        );

      return ApplyEnd(true, disposeController);
    }

    
  }
}
