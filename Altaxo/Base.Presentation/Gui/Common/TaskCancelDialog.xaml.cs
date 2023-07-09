using System;
using System.ComponentModel;
using System.Windows;

namespace Altaxo.Gui.Common
{
  /// <summary>
  /// Interaction logic for TaskCancelDialog.xaml
  /// </summary>
  public partial class TaskCancelDialog : Window
  {
    public TaskCancelDialog()
    {
      InitializeComponent();
    }

    public void EhCloseWindow(object? sender, EventArgs args)
    {
      this.Close();
    }

    protected override void OnClosing(CancelEventArgs e)
    {
      if (DataContext is TaskCancelController tc && tc.IsExecutionInProgress)
      {
        e.Cancel = true;
      }
      else
      {
        (DataContext as IDisposable)?.Dispose();
        DataContext = null;
      }

      base.OnClosing(e);
    }
  }
}
