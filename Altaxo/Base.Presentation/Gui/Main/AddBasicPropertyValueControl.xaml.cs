using Altaxo.Collections;
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

namespace Altaxo.Gui.Main
{
  /// <summary>
  /// Interaction logic for AddBasicPropertyValueControl.xaml
  /// </summary>
  public partial class AddBasicPropertyValueControl : UserControl, IAddBasicPropertyValueView
  {
    public AddBasicPropertyValueControl()
    {
      InitializeComponent();
    }

    public string PropertyName
    {
      get
      {
        return _guiPropertyName.Text;
      }

      set
      {
        _guiPropertyName.Text = value;
      }
    }

    public SelectableListNodeList PropertyTypes
    {
      set
      {
        GuiHelper.Initialize(_guiPropertyType, value);
      }
    }

    private void ShowNoValueControl()
    {
      _guiPropertyValueAsString.Visibility = Visibility.Collapsed;
      _guiPropertyValueAsDouble.Visibility = Visibility.Collapsed;
      _guiPropertyValueAsInt.Visibility = Visibility.Collapsed;
      _guiPropertyValueAsDateTime.Visibility = Visibility.Collapsed;
    }

    public bool ShowValueAsDateTime
    {
      set
      {
        if (true == value)
        {
          ShowNoValueControl();
          _guiPropertyValueAsDateTime.Visibility = Visibility.Visible;
        }
      }
    }

    public bool ShowValueAsDouble
    {
      set
      {
        if (true == value)
        {
          ShowNoValueControl();
          _guiPropertyValueAsDouble.Visibility = Visibility.Visible;
        }
      }
    }

    public bool ShowValueAsInt
    {
      set
      {
        if (true == value)
        {
          ShowNoValueControl();
          _guiPropertyValueAsInt.Visibility = Visibility.Visible;
        }
      }
    }

    public bool ShowValueAsString
    {
      set
      {
        if (true == value)
        {
          ShowNoValueControl();
          _guiPropertyValueAsString.Visibility = Visibility.Visible;
        }
      }
    }

    public DateTime ValueAsDateTime
    {
      get
      {
        return _guiPropertyValueAsDateTime.SelectedValue;
      }

      set
      {
        _guiPropertyValueAsDateTime.SelectedValue = value;
      }
    }

    public double ValueAsDouble
    {
      get
      {
        return _guiPropertyValueAsDouble.SelectedValue;
      }

      set
      {
        _guiPropertyValueAsDouble.SelectedValue = value;
      }
    }

    public int ValueAsInt
    {
      get
      {
        return _guiPropertyValueAsInt.Value;
      }

      set
      {
        _guiPropertyValueAsInt.Value = value;
      }
    }

    public string ValueAsString
    {
      get
      {
        return _guiPropertyValueAsString.Text;
      }

      set
      {
        _guiPropertyValueAsString.Text = value;
      }
    }

    public event Action PropertyTypeChanged;

    private void EhPropertyTypeChanged(object sender, SelectionChangedEventArgs e)
    {
      GuiHelper.SynchronizeSelectionFromGui(_guiPropertyType);
      var ev = PropertyTypeChanged;
      if (null != ev)
        ev();
    }
  }
}
