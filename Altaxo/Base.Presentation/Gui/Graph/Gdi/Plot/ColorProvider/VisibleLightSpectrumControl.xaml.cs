#region Copyright

/////////////////////////////////////////////////////////////////////////////
//    Altaxo:  a data processing and data plotting program
//    Copyright (C) 2002-2012 Dr. Dirk Lellinger
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

namespace Altaxo.Gui.Graph.Gdi.Plot.ColorProvider
{
  /// <summary>
  /// Interaction logic for ColorProviderAHSBGradientControl.xaml
  /// </summary>
  public partial class VisibleLightSpectrumControl : UserControl, IVisibleLightSpectrumView
  {
    public event Action ChoiceChanged;

    public VisibleLightSpectrumControl()
    {
      InitializeComponent();
    }

    public IColorProviderBaseView BaseView
    {
      get { return _guiBaseControl; }
    }

    public double Gamma
    {
      get
      {
        return _guiGamma.ValueAsDouble;
      }
      set
      {
        _guiGamma.ValueAsDouble = value;
      }
    }

    public double Brightness
    {
      get
      {
        return _guiBrightness.ValueAsDouble;
      }
      set
      {
        _guiBrightness.ValueAsDouble = value;
      }
    }

    private void EhDoubleUpDown_ValueChanged(object sender, RoutedPropertyChangedEventArgs<decimal> e)
    {
      if (null != ChoiceChanged)
        ChoiceChanged();
    }
  }
}
