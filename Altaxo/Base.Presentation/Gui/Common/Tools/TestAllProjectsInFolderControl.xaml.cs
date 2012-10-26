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

namespace Altaxo.Gui.Common.Tools
{
  /// <summary>
  /// Interaction logic for TestAllProjectsInFolderControl.xaml
  /// </summary>
  public partial class TestAllProjectsInFolderControl : UserControl, ITestAllProjectsInFolderView
  {
    public TestAllProjectsInFolderControl()
    {
      InitializeComponent();
    }

    public string FolderPaths
    {
      get
      {
        return _guiPathsToTest.Text;
      }
      set
      {
        _guiPathsToTest.Text = value;
      }
    }

    public bool TestSavingAndReopening
    {
      get
      {
        return _guiStoreLoadTest.IsChecked == true;
      }
      set
      {
        _guiStoreLoadTest.IsChecked = value;
      }
    }
  }
}
