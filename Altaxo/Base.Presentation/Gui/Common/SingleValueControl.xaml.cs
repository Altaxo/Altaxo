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
using System.Windows.Controls;

namespace Altaxo.Gui.Common
{
  /// <summary>
  /// Interaction logic for SingleValueControl.xaml
  /// </summary>
  public partial class SingleValueControl : UserControl, ISingleValueView
  {
    public SingleValueControl()
    {
      InitializeComponent();
    }

    #region ISingleValueView

    public string DescriptionText
    {
      set { _lblDescription.Content = value; }
    }

    public string ValueText
    {
      get
      {
        return _edEditText.Text;
      }
      set
      {
        _edEditText.Text = value;
      }
    }

    public event Action<ValidationEventArgs<string>> ValueText_Validating;

    #endregion ISingleValueView

    private void EhValidating(object sender, ValidationEventArgs<string> e)
    {
      if (null != ValueText_Validating)
        ValueText_Validating(e);
    }
  }
}
