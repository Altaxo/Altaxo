#region Copyright

/////////////////////////////////////////////////////////////////////////////
//    AltaxoMarkdownEditing
//    Copyright (C) 2018 Dr. Dirk Lellinger
//    This source file is licensed under the MIT license.
//    See the LICENSE.md file in the root of the AltaxoMarkdownEditing library for more information.
/////////////////////////////////////////////////////////////////////////////

#endregion Copyright

using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using Markdig;
using Markdig.Wpf;

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
      Process.Start(new ProcessStartInfo($"{e.Parameter}") { UseShellExecute = true });
    }

    private void ToggleExtensionsButton_OnClick(object sender, RoutedEventArgs e)
    {
      useExtensions = !useExtensions;
      Viewer.Pipeline = useExtensions ? new MarkdownPipelineBuilder().UseSupportedExtensions().Build() : new MarkdownPipelineBuilder().Build();
    }

    private void EhTextChanged(object sender, TextChangedEventArgs e)
    {
      if (Viewer is not null && _guiRawText is not null)
        Viewer.Markdown = _guiRawText.Text;
    }
  }
}
