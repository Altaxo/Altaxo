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
  /// Interaction logic for PropertyControl.xaml
  /// </summary>
  public partial class PropertyControl : UserControl, IPropertyView
  {
    public PropertyControl()
    {
      InitializeComponent();
    }
#if !NETFRAMEWORK
    private object _instance;
    public object[] SelectedObjectsToView
    {
      get
      {
        return new object[1] { _instance };
      }
      set
      {
        if (value != null && value.Length >= 1)
          _instance = value[0];
        else
          _instance = null;
      }
    }
#else
    public object[] SelectedObjectsToView
    {
      get
      {
        return new object[1] { _propertyGrid.Instance };
      }
      set
      {
        if (value != null && value.Length >= 1)
          _propertyGrid.Instance = value[0];
        else
          _propertyGrid.Instance = null;
      }
    }
#endif
  }
}
