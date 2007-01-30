#region Copyright
/////////////////////////////////////////////////////////////////////////////
//    Altaxo:  a data processing and data plotting program
//    Copyright (C) 2002-2007 Dr. Dirk Lellinger
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
#endregion

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;

namespace Altaxo.Gui.Graph.Scales
{
  public partial class AngularScaleControl : UserControl, IAngularScaleView
  {
    public AngularScaleControl()
    {
      InitializeComponent();
    }

    #region IAngularScaleView Members

    public bool UseDegrees
    {
      get
      {
        return _rbDegree.Checked;
      }
      set
      {
        if (value)
          _rbDegree.Checked = true;
        else
          _rbRadian.Checked = true;
      }
    }

    public bool UsePositiveNegativeValues
    {
      get
      {
        return _chkPosNegValues.Checked;
      }
      set
      {
        _chkPosNegValues.Checked = value;
      }
    }

    public Altaxo.Collections.SelectableListNodeList Origin
    {
      set
      {
        Main.Services.GUIFactoryService.InitComboBox(_cbOrigin, value);
      }
    }

    public Altaxo.Collections.SelectableListNodeList MajorTicks
    {
      set
      {
        Main.Services.GUIFactoryService.InitComboBox(_cbMajorTicks, value);
      }
    }

    public Altaxo.Collections.SelectableListNodeList MinorTicks
    {
      set
      {
        Main.Services.GUIFactoryService.InitComboBox(_cbMinorTicks, value);
      }
    }

    public event EventHandler MajorTicksChanged;

    #endregion

    private void _cbOrigin_SelectionChangeCommitted(object sender, EventArgs e)
    {
      Main.Services.GUIFactoryService.SynchronizeSelectableListNodes(_cbOrigin);
    }

    private void _cbMajorTicks_SelectedIndexChanged(object sender, EventArgs e)
    {
      Main.Services.GUIFactoryService.SynchronizeSelectableListNodes(_cbMajorTicks);
      if (null != MajorTicksChanged)
        MajorTicksChanged(sender, e);
    }

    private void _cbMinorTicks_SelectedIndexChanged(object sender, EventArgs e)
    {
      Main.Services.GUIFactoryService.SynchronizeSelectableListNodes(_cbMinorTicks);
    }
  }
}
