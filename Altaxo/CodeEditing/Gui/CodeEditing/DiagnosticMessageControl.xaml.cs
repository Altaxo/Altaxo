#region Copyright

/////////////////////////////////////////////////////////////////////////////
//    Altaxo:  a data processing and data plotting program
//    Copyright (C) 2002-2017 Dr. Dirk Lellinger
//
//    This program is free software; you can redistribute it and/or modify
//    it under the terms of the GNU General Public License as published by
//    the Free Software Foundation; either version 2 of the License, or
//    (at your option) any later version.
//
//    This program is distributed in the hope that it will be useful,
//    but WITHOUT ANY WARRANTY; without even the implied warranty of
//    MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
//    GNU General Public License for more details.
//
//    You should have received a copy of the GNU General Public License
//    along with this program; if not, write to the Free Software
//    Foundation, Inc., 675 Mass Ave, Cambridge, MA 02139, USA.
//
/////////////////////////////////////////////////////////////////////////////

#endregion Copyright

using System;
using System.Collections.Generic;
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
using Altaxo.CodeEditing.CompilationHandling;

namespace Altaxo.Gui.CodeEditing
{
  /// <summary>
  /// Interaction logic for MessageControl.xaml
  /// </summary>
  public partial class DiagnosticMessageControl : UserControl
  {
    public DiagnosticMessageControl()
    {
      InitializeComponent();
    }

    /// <summary>
    /// Occurs when the user wants to go to the location that corresponds to a diagnostic he has clicked.
    /// </summary>
    public event Action<AltaxoDiagnostic> DiagnosticClicked;

    public void SetMessages(IReadOnlyList<AltaxoDiagnostic> diagnostics)
    {
      _guiMessageList.ItemsSource = diagnostics;
    }

    private void EhListView_MouseDoubleClick(object sender, MouseButtonEventArgs e)
    {
      var listItem = GetListViewItemFromMouseEvent(e);

      if (listItem != null)
      {
        // wenn we have double clicked on a listview item, we can use the selected value property to get the clicked item
        if (_guiMessageList.SelectedValue is AltaxoDiagnostic diag)
        {
          DiagnosticClicked(diag);
        }
      }
    }

    public ListViewItem GetListViewItemFromMouseEvent(MouseButtonEventArgs e)
    {
      var obj = (DependencyObject)e.OriginalSource;

      while (obj != null && obj != _guiMessageList)
      {
        if (obj is ListViewItem lvi)
        {
          return lvi;
        }
        obj = VisualTreeHelper.GetParent(obj);
      }
      return null;
    }
  }

  [ValueConversion(typeof(int), typeof(Brush))]
  public class MessageLevelToBrushConverter : IValueConverter
  {
    private static Brush _hiddenBrush = new SolidColorBrush(Colors.LightGray);
    private static Brush _infoBrush = new SolidColorBrush(Colors.LightGreen);
    private static Brush _warningBrush = new SolidColorBrush(Colors.Yellow);
    private static Brush _errorBrush = new SolidColorBrush(Colors.LightPink);

    public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
    {
      var c1 = (int)value;
      switch (c1)
      {
        case 0:
          return _hiddenBrush;

        case 1:
          return _infoBrush;

        case 2:
          return _warningBrush;

        case 3:
          return _errorBrush;

        default:
          throw new NotImplementedException("Unknown MessageLevel");
      }
    }

    public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
    {
      throw new NotImplementedException();
    }
  }
}
