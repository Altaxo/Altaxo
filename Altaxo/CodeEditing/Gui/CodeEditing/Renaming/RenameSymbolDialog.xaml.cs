using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.Composition;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using Altaxo.CodeEditing.Renaming;

namespace Altaxo.Gui.CodeEditing.Renaming
{
  /// <summary>
  /// Interaction logic for RenameSymbolDialog.xaml
  /// </summary>
  [Export(typeof(IRenameSymbolDialog))]
  public partial class RenameSymbolDialog : Window, IRenameSymbolDialog
  {
    private static readonly Regex _identifierRegex = new Regex(@"^(?:((?!\d)\w+(?:\.(?!\d)\w+)*)\.)?((?!\d)\w+)$");
    private string _symbolName;

    public RenameSymbolDialog()
    {
      InitializeComponent();
      SymbolTextBox.DataContext = this;
    }

    public void Show(object topLevelWindow)
    {
      ShowDialog();
    }

    public void Initialize(string symbolName)
    {
      Loaded += (sender, args) =>
      {
        SymbolTextBox.Focus();
        SymbolTextBox.SelectionStart = symbolName.Length;
      };
      SymbolName = symbolName;
    }

    public bool ShouldRename { get; private set; }

    public string SymbolName
    {
      get { return _symbolName; }
      set
      {
        _symbolName = value;
        OnPropertyChanged();
        RenameButton.IsEnabled = value != null && _identifierRegex.IsMatch(value);
      }
    }

    protected override void OnKeyDown(KeyEventArgs e)
    {
      base.OnKeyDown(e);
      if (e.Key == Key.Escape)
      {
        Close();
      }
    }

    private void Rename_Click(object sender, RoutedEventArgs e)
    {
      ShouldRename = true;
      Close();
    }

    private void Cancel_Click(object sender, RoutedEventArgs e)
    {
      Close();
    }

    private void SymbolText_KeyDown(object sender, KeyEventArgs e)
    {
      if (e.Key == Key.Enter && RenameButton.IsEnabled)
      {
        ShouldRename = true;
        Close();
      }
    }

    public event PropertyChangedEventHandler PropertyChanged;

    protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
    {
      PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
  }
}
