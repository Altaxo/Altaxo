using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Altaxo.Gui.Main.Services
{
  /// <summary>
  /// Interaction logic for OutputView.xaml
  /// </summary>
  public partial class OutputView : UserControl, IOutputView
  {
    public event Action EnabledChanged;

    public OutputView()
    {
      InitializeComponent();
    }

    #region IOutputView Members

    private void InternalSetText(string text)
    {
      _view.Text = text;
    }

    public void SetText(string text)
    {
      if (this.Dispatcher.CheckAccess())
        _view.Text = text;
      else
        this.Dispatcher.BeginInvoke((Action)delegate ()
        { InternalSetText(text); }, System.Windows.Threading.DispatcherPriority.Normal, null);
    }

    bool IOutputView.IsEnabled { get { return _menuTextAppendEnabled.IsChecked; } set { _menuTextAppendEnabled.IsChecked = value; } }

    #endregion IOutputView Members

    private void EhMenuTextAppendEnabled1(object sender, RoutedEventArgs e)
    {
      if (null != EnabledChanged)
        EnabledChanged();
    }
  }
}
