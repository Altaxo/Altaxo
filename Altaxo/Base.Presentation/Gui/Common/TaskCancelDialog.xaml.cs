using System;
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
  }
}
