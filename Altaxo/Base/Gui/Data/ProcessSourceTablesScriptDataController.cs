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
  public interface IProcessSourceTablesScriptDataView : IDataContextAwareView { }

  [ExpectedTypeOfView(typeof(IProcessSourceTablesScriptDataView))]
  [UserControllerForObject(typeof(ProcessSourceTablesScriptData))]
  public class ProcessSourceTablesScriptDataController : MVCANControllerEditCopyOfDocBase<ProcessSourceTablesScriptData, IProcessSourceTablesScriptDataView>
  {
    class MyListNode : SelectableListNode
    {
      public MyListNode(string name, DataTable table, bool isSelected) : base(name, table, isSelected) { }
      public override string? Text0 => ((DataTable)Tag).Name;
    }

    public override IEnumerable<ControllerAndSetNullMethod> GetSubControllers()
    {
      yield break;
    }

    public ProcessSourceTablesScriptDataController()
    {
      CmdAddToParticipatingTables = new RelayCommand(EhAddToParticipatingDataTable);
      CmdRemoveFromParticipatingTables = new RelayCommand(EhRemoveFromParticipatingTablesCommand);
      CmdParticipatingTablesUp = new RelayCommand(EhParticipatingTablesUpCommand);
      CmdParticipatingTablesDown = new RelayCommand(EhParticipatingTablesDownCommand);
      CmdAutoRename = new RelayCommand(EhAutoRename);
    }

    #region Bindings

    public ICommand CmdAddToParticipatingTables { get; }
    public ICommand CmdRemoveFromParticipatingTables { get; }
    public ICommand CmdParticipatingTablesUp { get; }
    public ICommand CmdParticipatingTablesDown { get; }
    public ICommand CmdAutoRename { get; }


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

          ParticipatingTables = new SelectableListNodeList(_doc.TableProxies.Where(p => p.Proxy.Document is not null)
            .Select(p => new MyListNode(p.Name, p.Proxy.Document, false)));
        }

        OnParticipatingTablesChanged();
      }
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
          ParticipatingTables.Add(new MyListNode(ParticipatingTables.Count.ToString(), (DataTable)AvailableTables[i].Tag, true));
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
      for(int i=0;i<ParticipatingTables.Count;++i)
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
        if(hash.TryGetValue(ParticipatingTables[i].Text, out int firstIndex))
        {
          Current.Gui.ErrorMessageBox(
            $"The names are not unique: both element[{firstIndex}] and element[{i}] are named '{ParticipatingTables[i].Text}'\r\n" +
            "You can use the automatic rename button in order to get unique names");

          return ApplyEnd(false, disposeController);
        }
        hash.Add(ParticipatingTables[i].Text, i);
      }


      _doc = new ProcessSourceTablesScriptData(ParticipatingTables.Select(node => (node.Text, new DataTableProxy(((DataTable)node.Tag)))));

      return ApplyEnd(true, disposeController);
    }

    
  }
}
