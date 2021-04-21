using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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

namespace Altaxo.Gui.Common.PropertyGrid
{
  /// <summary>
  /// Interaction logic for PropertyGrid.xaml
  /// </summary>
  public partial class PropertyGrid : UserControl, IPropertyGridView
  {
    private List<Tuple<Type, FrameworkElement>> _elements = new List<Tuple<Type, FrameworkElement>>();

    public PropertyGrid()
    {
      InitializeComponent();
    }

    public void Values_Initialize(IEnumerable<Tuple<string, Type, object>> values)
    {
      _elements.Clear();
      _guiGrid.Children.Clear();

      _guiGrid.RowDefinitions.Clear();

      int idx = 0;
      foreach (var tuple in values)
      {
        _guiGrid.RowDefinitions.Add(new RowDefinition());
        _guiGrid.RowDefinitions.Add(new RowDefinition() { Height = new GridLength(4) });

        var label = new Label { Content = tuple.Item1 + ":" };
        label.SetValue(Grid.RowProperty, idx * 2);
        _guiGrid.Children.Add(label);

        FrameworkElement fe;
        if (tuple.Item2 == typeof(double))
        {
          var item = new Altaxo.Gui.Common.NumericDoubleTextBox();
          fe = item;
          if (tuple.Item3 is double)
            item.SelectedValue = (double)tuple.Item3;
        }
        else
        {
          throw new NotImplementedException();
        }

        fe.SetValue(Grid.ColumnProperty, 2);
        fe.SetValue(Grid.RowProperty, idx * 2);
        _guiGrid.Children.Add(fe);

        _elements.Add(new Tuple<Type, FrameworkElement>(tuple.Item2, fe));
        ++idx;
      }

      // append another RowDefinition to ensure that the textboxes are not expanded vertically
      _guiGrid.RowDefinitions.Add(new RowDefinition { Height = new GridLength(4, GridUnitType.Star) });
    }

    public object Value_Get(int idx)
    {
      if (_elements[idx].Item2 is Altaxo.Gui.Common.NumericDoubleTextBox)
      {
        return (_elements[idx].Item2 as Altaxo.Gui.Common.NumericDoubleTextBox).SelectedValue;
      }
      else
      {
        throw new NotImplementedException();
      }
    }

   


  }
}
