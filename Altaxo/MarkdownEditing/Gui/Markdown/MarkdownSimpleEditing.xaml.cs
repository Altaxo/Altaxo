#region Copyright

/////////////////////////////////////////////////////////////////////////////
//    AltaxoMarkdownEditing
//    Copyright (C) 2018 Dr. Dirk Lellinger
//    This source file is licensed under the MIT license.
//    See the LICENSE.md file in the root of the AltaxoMarkdownEditing library for more information.
/////////////////////////////////////////////////////////////////////////////

#endregion Copyright

using Markdig;
using Markdig.Wpf;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Altaxo.Gui.Markdown
{
  /// <summary>
  /// Interaction logic for MarkdownSimpleEditing.xaml
  /// </summary>
  public partial class MarkdownSimpleEditing : UserControl
  {
    private bool useExtensions = true;

    public MarkdownSimpleEditing()
    {
      InitializeComponent();
      Loaded += EhLoaded;
    }

    private void EhLoaded(object sender, RoutedEventArgs e)
    {
      Viewer.Markdown = _guiRawText.Text;
    }

    private void OpenHyperlink(object sender, System.Windows.Input.ExecutedRoutedEventArgs e)
    {
      Process.Start(e.Parameter.ToString());
    }

    private void ToggleExtensionsButton_OnClick(object sender, RoutedEventArgs e)
    {
      useExtensions = !useExtensions;
      Viewer.Pipeline = useExtensions ? new MarkdownPipelineBuilder().UseSupportedExtensions().Build() : new MarkdownPipelineBuilder().Build();
    }

    private void EhTextChanged(object sender, TextChangedEventArgs e)
    {
      if (null != Viewer && null != _guiRawText)
        Viewer.Markdown = _guiRawText.Text;
    }
  }
}
