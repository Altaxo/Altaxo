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
    /// <summary>
    /// Initializes a new instance of the <see cref="TaskCancelDialog"/> class.
    /// </summary>
    public TaskCancelDialog()
    {
      InitializeComponent();
    }

    /// <summary>
    /// Closes the dialog window.
    /// </summary>
    /// <param name="sender">The event sender.</param>
    /// <param name="args">The event arguments.</param>
    public void EhCloseWindow(object? sender, EventArgs args)
    {
      this.Close();
    }

    /// <inheritdoc/>
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
