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

using Altaxo.Collections;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Controls;

namespace Altaxo.Gui.Graph.Gdi.LabelFormatting
{
  /// <summary>
  /// Interaction logic for DateTimeLabelFormattingControl.xaml
  /// </summary>
  public partial class DateTimeLabelFormattingControl : UserControl, IDateTimeLabelFormattingView
  {
    public DateTimeLabelFormattingControl()
    {
      InitializeComponent();
    }

    public IMultiLineLabelFormattingBaseView MultiLineLabelFormattingBaseView { get { return _guiMultiLineLabelFormattingControl; } }

    public void InitializeTimeConversion(SelectableListNodeList items)
    {
      GuiHelper.InitializeChoicePanel<RadioButton>(_guiTimeConversionPanel, items);
    }

    public string FormattingString
    {
      get
      {
        return _guiFormattingText.Text;
      }
      set
      {
        _guiFormattingText.Text = value;
      }
    }

    public string FormattingStringAlternate
    {
      get
      {
        return _guiAlternateFormattingText.Text;
      }
      set
      {
        _guiAlternateFormattingText.Text = value;
      }
    }

    public bool ShowAlternateFormattingOnMidnight
    {
      get
      {
        return true == _guiShowAlternateFormattingOnMidnight.IsChecked;
      }
      set
      {
        _guiShowAlternateFormattingOnMidnight.IsChecked = value;
      }
    }

    public bool ShowAlternateFormattingOnNoon
    {
      get
      {
        return true == _guiShowAlternateFormattingOnNoon.IsChecked;
      }
      set
      {
        _guiShowAlternateFormattingOnNoon.IsChecked = value;
      }
    }
  }
}
