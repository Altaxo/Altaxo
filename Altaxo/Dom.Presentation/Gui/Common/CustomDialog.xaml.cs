using Altaxo.Main.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace Altaxo.Gui.Common
{
  /// <summary>
  /// Interaction logic for SaveErrorChooseDialog.xaml
  /// </summary>
  public partial class CustomDialog : Window
  {
    public int Result { get; private set; } = -1;

    private string displayMessage;
    private Exception exceptionGot;

    private int cancelButton;
    private int acceptButton;

    private Button AcceptButton;

    private Button CancelButton;

    public CustomDialog()
    {
      InitializeComponent();
    }

    public CustomDialog(string caption, string message, int acceptButton, int cancelButton, string[] buttonLabels)
    {
      InitializeComponent();

      this.Icon = null;
      this.acceptButton = acceptButton;
      this.cancelButton = cancelButton;

      message = StringParser.Parse(message);
      this.Title = StringParser.Parse(caption);

      buttonGrid.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(1, GridUnitType.Star) });

      int idx = 0;
      foreach (var lbl in buttonLabels)
      {
        buttonGrid.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(1, GridUnitType.Auto) });
        buttonGrid.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(1, GridUnitType.Star) });

        var newButton = new Button()
        {
          Tag = idx,
          Content = StringParser.Parse(lbl)
        };

        newButton.Click += EhButtonClick;

        if (acceptButton == idx)
        {
          AcceptButton = newButton;
        }
        if (cancelButton == idx)
        {
          CancelButton = newButton;
        }

        ++idx;
      }

      label.Content = message;
    }

    protected override void OnKeyDown(KeyEventArgs e)
    {
      if (cancelButton == -1 && e.Key == Key.Escape)
      {
        this.Close();
      }
    }

    private void EhButtonClick(object sender, RoutedEventArgs e)
    {
      Result = (int)((Button)sender).Tag;
      this.DialogResult = true;
      this.Close();
    }
  }
}
