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
  public partial class ColorProviderARGBGradientControl : UserControl, IColorProviderARGBGradientView
  {
    public event Action? ChoiceChanged;

    public ColorProviderARGBGradientControl()
    {
      InitializeComponent();
    }

    public IColorProviderBaseView BaseView
    {
      get { return _guiBaseControl; }
    }

    public double Red0
    {
      get
      {
        return _guiRed0.ValueAsDouble;
      }
      set
      {
        _guiRed0.ValueAsDouble = value;
      }
    }

    public double Red1
    {
      get
      {
        return _guiRed1.ValueAsDouble;
      }
      set
      {
        _guiRed1.ValueAsDouble = value;
      }
    }

    public double Green0
    {
      get
      {
        return _guiGreen0.ValueAsDouble;
      }
      set
      {
        _guiGreen0.ValueAsDouble = value;
      }
    }

    public double Green1
    {
      get
      {
        return _guiGreen1.ValueAsDouble;
      }
      set
      {
        _guiGreen1.ValueAsDouble = value;
      }
    }

    public double Blue0
    {
      get
      {
        return _guiBlue0.ValueAsDouble;
      }
      set
      {
        _guiBlue0.ValueAsDouble = value;
      }
    }

    public double Blue1
    {
      get
      {
        return _guiBlue1.ValueAsDouble;
      }
      set
      {
        _guiBlue1.ValueAsDouble = value;
      }
    }

    public double Opaqueness0
    {
      get
      {
        return _guiOpaqueness0.ValueAsDouble;
      }
      set
      {
        _guiOpaqueness0.ValueAsDouble = value;
      }
    }

    public double Opaqueness1
    {
      get
      {
        return _guiOpaqueness1.ValueAsDouble;
      }
      set
      {
        _guiOpaqueness1.ValueAsDouble = value;
      }
    }

    private void EhDoubleUpDown_ValueChanged(object sender, RoutedPropertyChangedEventArgs<decimal> e)
    {
      if (ChoiceChanged is not null)
        ChoiceChanged();
    }
  }
}
