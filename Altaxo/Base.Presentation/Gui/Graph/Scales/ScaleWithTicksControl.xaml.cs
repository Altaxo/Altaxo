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

namespace Altaxo.Gui.Graph.Scales
{
  /// <summary>
  /// Interaction logic for AxisScaleControl.xaml
  /// </summary>
  public partial class ScaleWithTicksControl : UserControl, IScaleWithTicksView
  {
    public ScaleWithTicksControl()
    {
      InitializeComponent();
    }

    private void EhAxisType_SelectionChangeCommit(object sender, SelectionChangedEventArgs e)
    {
      e.Handled = true;
      if (null != ScaleTypeChanged)
      {
        GuiHelper.SynchronizeSelectionFromGui(this.m_Scale_cbType);
        ScaleTypeChanged();
      }
    }

    private void EhLinkTarget_SelectionChangeCommitted(object sender, SelectionChangedEventArgs e)
    {
      e.Handled = true;
      if (null != LinkTargetChanged)
      {
        GuiHelper.SynchronizeSelectionFromGui(this._cbLinkTarget);
        LinkTargetChanged();
      }
    }

    private void EhTickSpacingType_SelectionChangeCommitted(object sender, SelectionChangedEventArgs e)
    {
      e.Handled = true;
      if (null != TickSpacingTypeChanged)
      {
        ComboBox _cbTickSpacingType = (ComboBox)sender;
        GuiHelper.SynchronizeSelectionFromGui(_cbTickSpacingType);
        TickSpacingTypeChanged();
      }
    }

    #region IAxisScaleView

    public void InitializeAxisType(Collections.SelectableListNodeList names)
    {
      GuiHelper.Initialize(this.m_Scale_cbType, names);
    }

    public void InitializeTickSpacingType(Collections.SelectableListNodeList names)
    {
      ComboBox _cbTickSpacingType = (ComboBox)LogicalTreeHelper.FindLogicalNode((DependencyObject)_tickSpacingGroupBox.Header, "_cbTickSpacingType");
      GuiHelper.Initialize(_cbTickSpacingType, names);
    }

    public void InitializeLinkTargets(Collections.SelectableListNodeList names)
    {
      GuiHelper.Initialize(this._cbLinkTarget, names);
    }

    public void SetRescalingView(object guiobject)
    {
      _guiBoundaryHost.Child = guiobject as UIElement;
    }

    public void SetLinkedScalePropertiesView(object guiobject)
    {
      _guiLinkedScalePropertiesHost.Child = guiobject as UIElement;
    }

    public void SetScaleView(object guiobject)
    {
      _guiBoundaryHost.Child = null;
      _guiScaleDetailsHost.Child = guiobject as UIElement;
    }

    public void SetTickSpacingView(object guiobject)
    {
      _tickSpacingGroupBox.Content = guiobject as UIElement;
    }

    public event Action ScaleTypeChanged;

    public event Action TickSpacingTypeChanged;

    public event Action LinkTargetChanged;

    #endregion IAxisScaleView

    public bool LinkScaleType
    {
      get
      {
        return true == _guiLinkScaleType.IsChecked;
      }
      set
      {
        _guiLinkScaleType.IsChecked = value;
      }
    }

    public bool LinkTickSpacing
    {
      get
      {
        return _guiLinkTicks.IsChecked == true;
      }
      set
      {
        _guiLinkTicks.IsChecked = value;
      }
    }

    public void SetVisibilityOfLinkElements(bool showLinkTargets, bool showOtherLinkProperties)
    {
      _cbLinkTarget.Visibility = showLinkTargets ? Visibility.Visible : Visibility.Collapsed;
      _guiLabelForLinkTarget.Visibility = showLinkTargets ? Visibility.Visible : Visibility.Collapsed;

      _guiLinkScaleType.Visibility = showOtherLinkProperties ? Visibility.Visible : Visibility.Collapsed;
      _guiLinkTicks.Visibility = showOtherLinkProperties ? Visibility.Visible : Visibility.Collapsed;
    }
  }
}
