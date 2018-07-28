#region Copyright

/////////////////////////////////////////////////////////////////////////////
//    Altaxo:  a data processing and data plotting program
//    Copyright (C) 2002-2011 Dr. Dirk Lellinger
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
using System.Windows;
using System.Windows.Controls;

namespace Altaxo.Gui.Graph.Graph3D.Axis
{
  /// <summary>
  /// Interaction logic for TitleFormatLayerControl.xaml
  /// </summary>
  public partial class AxisStyleControl : UserControl, IAxisStyleView
  {
    public AxisStyleControl()
    {
      InitializeComponent();
    }

    #region ITitleFormatLayerView

    public bool ShowAxisLine
    {
      get
      {
        return ((UIElement)_axisLineGroupBox.Content).IsEnabled == true;
      }
      set
      {
        ((UIElement)_axisLineGroupBox.Content).IsEnabled = value;
      }
    }

    public bool ShowMajorLabels
    {
      get
      {
        return _chkShowMajorLabels.IsChecked == true;
      }
      set
      {
        _chkShowMajorLabels.IsChecked = value;
      }
    }

    public bool ShowMinorLabels
    {
      get
      {
        return _chkShowMinorLabels.IsChecked == true;
      }
      set
      {
        _chkShowMinorLabels.IsChecked = value;
      }
    }

    public bool ShowCustomTickSpacing
    {
      get
      {
        return ((UIElement)_guiCustomAxisTicksGroupBox.Content).IsEnabled == true;
      }
      set
      {
        ((UIElement)_guiCustomAxisTicksGroupBox.Content).IsEnabled = value;
      }
    }

    public event Action ShowAxisLineChanged;

    public event Action ShowMajorLabelsChanged;

    public event Action ShowMinorLabelsChanged;

    public event Action ShowCustomTickSpacingChanged;

    public event Action EditTitle;

    public object LineStyleView
    {
      set
      {
        var oldControl = (UIElement)_axisLineGroupBox.Content;
        bool wasEnabled = oldControl.IsEnabled == true;

        var newControl = value as UIElement;

        if (newControl == null)
          newControl = new Label();

        newControl.IsEnabled = wasEnabled;
        _axisLineGroupBox.Content = newControl;
      }
    }

    public object TickSpacingView
    {
      set
      {
        var oldControl = (UIElement)_guiCustomAxisTicksGroupBox.Content;
        bool wasEnabled = oldControl.IsEnabled == true;

        var newControl = value as UIElement;

        if (newControl == null)
          newControl = new Label();

        newControl.IsEnabled = wasEnabled;
        _guiCustomAxisTicksGroupBox.Content = newControl;
      }
    }

    public string AxisTitle
    {
      get
      {
        return m_Format_edTitle.Text;
      }
      set
      {
        m_Format_edTitle.Text = value;
      }
    }

    #endregion ITitleFormatLayerView

    private void EhShowAxisLineChanged(object sender, RoutedEventArgs e)
    {
      if (null != ShowAxisLineChanged)
        ShowAxisLineChanged();
    }

    private void EhShowMajorLabelsChanged(object sender, RoutedEventArgs e)
    {
      if (null != ShowMajorLabelsChanged)
        ShowMajorLabelsChanged();
    }

    private void EhShowMinorLabelsChanged(object sender, RoutedEventArgs e)
    {
      if (null != ShowMinorLabelsChanged)
        ShowMinorLabelsChanged();
    }

    private void EhCustomTickSpacingChanged(object sender, RoutedEventArgs e)
    {
      if (null != ShowCustomTickSpacingChanged)
        ShowCustomTickSpacingChanged();
    }

    private void EhEditTitle_Click(object sender, RoutedEventArgs e)
    {
      EditTitle?.Invoke();
    }
  }
}
